using System;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using Micro.Auth.Business.Tokens;
using Micro.Auth.Business.Users.Exceptions;
using Micro.Auth.Business.Users.ViewModels;
using Micro.Auth.Common;
using Micro.Auth.Storage;
using Micro.Mails;
using Micro.Mails.Content;
using Micro.Mails.Exceptions;
using Microsoft.AspNetCore.Identity;

namespace Micro.Auth.Business.Users
{
    public interface IUserService
    {
        Task<User> Create(RegisterInput request);
        Task SendActivationEmail(string login);
        Task<(SignInResult, LoginSuccessResponse)> Login(LoginRequest loginRequest);
        Task<User> ConfirmEmail(VerifyEmailInput input);
        Task RequestPasswordReset(string login);
        Task ResetPassword(ResetPasswordRequest request);
        Task<IdentityResult> ChangePassword(string userId, ChangePasswordRequest request);
    }

    public class UserService : IUserService
    {
        private readonly UserManager<Micro.Auth.Storage.User> _userManager;
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly SignInManager<Micro.Auth.Storage.User> _signInManager;
        private readonly ITokenFactory _tokenFactory;
        private readonly IUuidService _uuidService;
        private readonly MailBuilder _mailBuilder;
        private readonly IMailService _mailService;
        private readonly EmailUrlBuilder _emailUrlBuilder;

        public UserService(UserManager<Micro.Auth.Storage.User> userManager, IRefreshTokenRepository refreshTokenRepository, SignInManager<Micro.Auth.Storage.User> signInManager, ITokenFactory tokenFactory, IUuidService uuidService, MailBuilder mailBuilder, IMailService mailService, EmailUrlBuilder emailUrlBuilder)
        {
            _userManager = userManager;
            _refreshTokenRepository = refreshTokenRepository;
            _signInManager = signInManager;
            _tokenFactory = tokenFactory;
            _uuidService = uuidService;
            _mailBuilder = mailBuilder;
            _mailService = mailService;
            _emailUrlBuilder = emailUrlBuilder;
        }

        /// <summary>
        /// Create and send activation email
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <exception cref="EmailSendingFailureException"></exception>
        public async Task<User> Create(RegisterInput request)
        {
            var result = await _userManager.CreateAsync(new Micro.Auth.Storage.User
            {
                Email = request.Email,
                UserName = request.Username
            }, request.Password);
            if (!result.Succeeded)
            {
                throw new CreateUserFailedException(result.Errors.First().Description);
            }

            var dbUser = await _userManager.FindByEmailAsync(request.Email);
            await SendActivationEmail(dbUser);
            return User.FromDbUser(dbUser);
        }

        public async Task SendActivationEmail(string login)
        {
            await SendActivationEmail(await GetUserByLogin(login));
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        /// <exception cref="UserNotFoundException"></exception>
        /// <exception cref="UserAlreadyActivatedException"></exception>
        /// <exception cref="EmailSendingFailureException"></exception>
        public async Task SendActivationEmail(Micro.Auth.Storage.User user)
        {
            if (user == null)
            {
                throw new UserNotFoundException();
            }

            if (user.EmailConfirmed)
            {
                throw new UserAlreadyActivatedException();
            }

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            _emailUrlBuilder.BuildActivationUrl(token);
            var mail = await _mailBuilder.ActivationEmail()
                .Build(new ActivationMailData
                {
                    Name = user.UserName,
                    ActivationUrl = _emailUrlBuilder.BuildActivationUrl(token)
                }, new MailAddress(user.Email, user.UserName));
            await _mailService.SendAsync(mail);
        }

        public async Task<(SignInResult, LoginSuccessResponse)> Login(LoginRequest loginRequest)
        {
            var user = await GetUserByLogin(loginRequest.Login);
            loginRequest.User = user;
            return await AuthenticateUser(loginRequest);
        }

        public async Task<User> ConfirmEmail(VerifyEmailInput input)
        {
            var user = await GetUserByLogin(input.Login);
            await ConfirmEmail(user, input.Token);
            return User.FromDbUser(user);
        }

        public async Task RequestPasswordReset(string login)
        {
            var user = await GetUserByLogin(login);
            if (user == null)
            {
                throw new UserNotFoundException();
            }

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var mailMessage = await _mailBuilder.ForgotPasswordEmail().Build(new ForgotPasswordEmailDetails
                {
                    Name = user.UserName,
                    PasswordResetUrl = _emailUrlBuilder.BuildPasswordResetFormUrl(token, user.Email)
                },
                new MailAddress(user.Email, user.UserName));
            await _mailService.SendAsync(mailMessage);
        }

        public async Task ResetPassword(ResetPasswordRequest request)
        {
            var user = await GetUserByLogin(request.Login);
            if (user == null)
            {
                throw new UserNotFoundException();
            }

            var result = await _userManager.ResetPasswordAsync(user, request.Token, request.NewPassword);
            if (!result.Succeeded)
            {
                throw new PasswordResetFailedException(result.Errors);
            }
        }

        public async Task<IdentityResult> ChangePassword(string userId, ChangePasswordRequest request)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                throw new UserNotFoundException();
            }

            return await _userManager.ChangePasswordAsync(user, request.OldPassword, request.NewPassword);
        }

        private async Task ConfirmEmail(Micro.Auth.Storage.User user, string token)
        {
            if (user == null)
            {
                throw new UserNotFoundException();
            }
            var result = await _userManager.ConfirmEmailAsync(user, token);
            if (!result.Succeeded)
            {
                throw new EmailConfirmationFailedException(result.Errors.First().Description);
            }
        }

        private Task<Micro.Auth.Storage.User> GetUserByLogin(string usernameOrEmail)
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
