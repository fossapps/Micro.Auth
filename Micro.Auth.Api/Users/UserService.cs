using System;
using System.Threading.Tasks;
using Micro.Auth.Api.Models;
using Micro.Auth.Api.RefreshTokens;
using Micro.Auth.Api.Tokens;
using Micro.Auth.Api.Users.ViewModels;
using Microsoft.AspNetCore.Identity;

namespace Micro.Auth.Api.Users
{
    public interface IUserService
    {
        Task<IdentityResult> Create(CreateUserRequest request);
        Task<(SignInResult, LoginSuccessResponse)> Login(LoginRequest loginRequest);
    }

    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly SignInManager<User> _signInManager;
        private readonly ITokenFactory _tokenFactory;

        public UserService(UserManager<User> userManager, IRefreshTokenRepository refreshTokenRepository, SignInManager<User> signInManager, ITokenFactory tokenFactory)
        {
            _userManager = userManager;
            _refreshTokenRepository = refreshTokenRepository;
            _signInManager = signInManager;
            _tokenFactory = tokenFactory;
        }

        public Task<IdentityResult> Create(CreateUserRequest request)
        {
            return _userManager.CreateAsync(new User
            {
                Email = request.Email,
                UserName = request.Username
            }, request.Password);
        }

        public async Task<(SignInResult, LoginSuccessResponse)> Login(LoginRequest loginRequest)
        {
            var user = await GetUserByLogin(loginRequest.Login);
            loginRequest.User = user;
            return await AuthenticateUser(loginRequest);
        }

        private Task<User> GetUserByLogin(string usernameOrEmail)
        {
            return usernameOrEmail.Contains("@")
                ? _userManager.FindByEmailAsync(usernameOrEmail)
                : _userManager.FindByNameAsync(usernameOrEmail);
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
            var refreshToken = await _refreshTokenRepository.Create(login.ToRefreshToken(_tokenFactory.GenerateToken(32)));
            var res = new LoginSuccessResponse
            {
                RefreshToken = refreshToken.Value,
                Jwt = jwt
            };
            return (signInResult, res);
        }

    }
}
