using System.Net.Mail;
using System.Threading.Tasks;
using Micro.Auth.Business.Extensions;
using Micro.Auth.Business.Users.Exceptions;
using Micro.Auth.Business.Users.ViewModels;
using Micro.Auth.Storage;
using Micro.Mails;
using Micro.Mails.Content;
using Microsoft.AspNetCore.Identity;

namespace Micro.Auth.Business.PasswordManager
{
    public class PasswordManager : IPasswordManager
    {
        private readonly EmailUrlBuilder _emailUrlBuilder;
        private readonly MailBuilder _mailBuilder;
        private readonly IMailService _mailService;
        private readonly UserManager<User> _userManager;

        public PasswordManager(
            UserManager<User> userManager,
            IMailService mailService,
            MailBuilder mailBuilder,
            EmailUrlBuilder emailUrlBuilder
        )
        {
            _userManager = userManager;
            _mailService = mailService;
            _mailBuilder = mailBuilder;
            _emailUrlBuilder = emailUrlBuilder;
        }

        public async Task<Result> RequestPasswordReset(string login)
        {
            var user = await _userManager.GetUserByLogin(login);
            if (user == null) throw new UserNotFoundException();

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var mailMessage = await _mailBuilder.ForgotPasswordEmail().Build(new ForgotPasswordEmailDetails
                {
                    Name = user.UserName,
                    PasswordResetUrl = _emailUrlBuilder.BuildPasswordResetFormUrl(token, user.Email)
                },
                new MailAddress(user.Email, user.UserName));
            await _mailService.SendAsync(mailMessage);
            return new Result(true);
        }

        public async Task<Users.User> ResetPassword(ResetPasswordRequest request)
        {
            var user = await _userManager.GetUserByLogin(request.Login);
            if (user == null) throw new UserNotFoundException();

            var result = await _userManager.ResetPasswordAsync(user, request.Token, request.NewPassword);
            if (!result.Succeeded) throw new PasswordResetFailedException(result.Errors);

            return Users.User.FromDbUser(user);
        }

        public async Task<Users.User> ChangePassword(string userId, ChangePasswordRequest request)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) throw new UserNotFoundException();

            await _userManager.ChangePasswordAsync(user, request.OldPassword, request.NewPassword);
            return Users.User.FromDbUser(user);
        }
    }
}
