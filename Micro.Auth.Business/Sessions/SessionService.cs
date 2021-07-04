using System;
using System.Threading.Tasks;
using Micro.Auth.Business.Internal.Extensions;
using Micro.Auth.Business.Internal.Tokens;
using Micro.Auth.Business.Sessions.Exceptions;
using Micro.Auth.Business.Users;
using Micro.Auth.Common;
using Micro.Auth.Storage;
using Microsoft.AspNetCore.Identity;
using User = Micro.Auth.Storage.User;

namespace Micro.Auth.Business.Sessions
{
    public interface ISessionService
    {
        Task<string> Refresh(string token);
        Task<LoginSuccessResponse> Login(LoginRequest request);
    }

    public class SessionService : ISessionService
    {
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IUserRepository _userRepository;
        private readonly SignInManager<User> _signInManager;
        private readonly ITokenFactory _tokenFactory;
        private readonly UserManager<User> _userManager;
        private readonly IUuidService _uuidService;

        public SessionService(
            IRefreshTokenRepository refreshTokenRepository,
            IUserRepository userRepository,
            SignInManager<User> signInManager,
            ITokenFactory tokenFactory,
            UserManager<User> userManager,
            IUuidService uuidService)
        {
            _refreshTokenRepository = refreshTokenRepository;
            _userRepository = userRepository;
            _signInManager = signInManager;
            _tokenFactory = tokenFactory;
            _userManager = userManager;
            _uuidService = uuidService;
        }

        public async Task<string> Refresh(string token)
        {
            var refreshToken = await _refreshTokenRepository.TouchLastUsed(token);
            var user = await _userRepository.FindById(refreshToken.User);
            var principal = await _signInManager.CreateUserPrincipalAsync(user);
            return _tokenFactory.GenerateJwtToken(principal);
        }

        public async Task<LoginSuccessResponse> Login(LoginRequest request)
        {
            var user = await _userManager.GetUserByLogin(request.Login);
            if (user == null)
            {
                throw new UserNotFoundException();
            }

            var signInResult = await _signInManager.PasswordSignInAsync(user, request.Password, false, true);
            if (!signInResult.Succeeded)
            {
                throw new InvalidCredentialsException(ProcessErrorResult(signInResult));
            }
            var principal = await _signInManager.CreateUserPrincipalAsync(user);
            var jwt = _tokenFactory.GenerateJwtToken(principal);
            if (principal.IsServiceAccount())
            {
                return new LoginSuccessResponse
                {
                    Jwt = jwt,
                };
            }
            var refreshToken = await _refreshTokenRepository.Create(new RefreshToken
            {
                Location = request.Location,
                User = user.Id,
                Useragent = request.UserAgent,
                IpAddress = request.IpAddress,
                LastUsed = DateTime.Now,
                Value = _uuidService.GenerateUuId("session"),
            });
            return new LoginSuccessResponse
            {
                Jwt = jwt,
                RefreshToken = refreshToken.Value,
            };
        }

        private static string ProcessErrorResult(Microsoft.AspNetCore.Identity.SignInResult result)
        {
            if (result.IsLockedOut)
            {
                return "Account Locked Out";
            }

            return result.IsNotAllowed ? "Login not allowed" : "Wrong Credentials";
        }
    }
}
