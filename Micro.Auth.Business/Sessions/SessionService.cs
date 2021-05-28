using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Micro.Auth.Business.Internal.Extensions;
using Micro.Auth.Business.Internal.Tokens;
using Micro.Auth.Common;
using Micro.Auth.Storage;
using Microsoft.AspNetCore.Identity;

namespace Micro.Auth.Business.Sessions
{
    public interface ISessionService
    {
        Task<string> Refresh(string token);
        Task<IEnumerable<RefreshToken>> GetForUser(string userId);
        Task<(SignInResult, LoginSuccessResponse)> Login(LoginRequest loginRequest);
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

        public async Task<IEnumerable<RefreshToken>> GetForUser(string userId)
        {
            return await _refreshTokenRepository.FindByUser(userId);
        }
        public async Task<(SignInResult, LoginSuccessResponse)> Login(LoginRequest loginRequest)
        {
            var user = await _userManager.GetUserByLogin(loginRequest.Login);
            loginRequest.User = user;
            return await AuthenticateUser(loginRequest);
        }

        private async Task<(SignInResult signInResult, LoginSuccessResponse login)> AuthenticateUser(LoginRequest login)
        {
            if (login.User == null)
            {
                throw new ArgumentNullException(nameof(login), "not null expected");
            }
            var signInResult = await _signInManager.PasswordSignInAsync(login.User, login.Password, false, true);
            if (!signInResult.Succeeded)
            {
                return (signInResult, null);
            }
            var principal = await _signInManager.CreateUserPrincipalAsync(login.User);
            var jwt = _tokenFactory.GenerateJwtToken(principal);
            var refreshToken = await _refreshTokenRepository.Create(login.ToRefreshToken(_uuidService.GenerateUuId()));
            var res = new LoginSuccessResponse
            {
                RefreshToken = refreshToken.Value,
                Jwt = jwt
            };
            return (signInResult, res);
        }
    }
}
