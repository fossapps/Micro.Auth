using System;
using System.Net.Mail;
using System.Threading.Tasks;
using Micro.Auth.Api.Models;
using Micro.Auth.Api.RefreshTokens;
using Micro.Auth.Api.Tokens;
using Micro.Auth.Api.Users.ViewModels;
using Micro.Auth.Api.Uuid;
using Micro.Mails;
using Micro.Mails.Content;
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
        private readonly IUuidService _uuidService;
        private readonly MailBuilder _mailBuilder;
        private readonly IMailService _mailService;

        public UserService(UserManager<User> userManager, IRefreshTokenRepository refreshTokenRepository, SignInManager<User> signInManager, ITokenFactory tokenFactory, IUuidService uuidService, MailBuilder mailBuilder, IMailService mailService)
        {
            _userManager = userManager;
            _refreshTokenRepository = refreshTokenRepository;
            _signInManager = signInManager;
            _tokenFactory = tokenFactory;
            _uuidService = uuidService;
            _mailBuilder = mailBuilder;
            _mailService = mailService;
        }

        public async Task<IdentityResult> Create(CreateUserRequest request)
        {
            var result = await _userManager.CreateAsync(new User
            {
                Email = request.Email,
                UserName = request.Username
            }, request.Password);
            if (!result.Succeeded)
            {
                return result;
            }

            try
            {
                var mail = await _mailBuilder.ActivationEmail().Build(new ActivationMailData {Name = request.Username},
                    new MailAddress(request.Email, request.Username));
                await _mailService.SendAsync(mail);
                return result;
            }
            catch (Exception e)
            {
                throw new SendingEmailFailedException("sending email failed", e);
            }
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
