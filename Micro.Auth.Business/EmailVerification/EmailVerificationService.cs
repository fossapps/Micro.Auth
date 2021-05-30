using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;
using Micro.Auth.Business.Common;
using Micro.Auth.Business.Internal.Extensions;
using Micro.Auth.Business.Users;
using Micro.Mails;
using Micro.Mails.Content;
using Microsoft.AspNetCore.Identity;

namespace Micro.Auth.Business.EmailVerification
{
    public class EmailVerificationService : IEmailVerificationService
    {
        private readonly UserManager<Storage.User> _userManager;
        private readonly EmailUrlBuilder _emailUrlBuilder;
        private readonly MailBuilder _mailBuilder;
        private readonly IMailService _mailService;

        public EmailVerificationService(
            UserManager<Storage.User> userManager,
            EmailUrlBuilder emailUrlBuilder,
            MailBuilder mailBuilder,
            IMailService mailService)
        {
            _userManager = userManager;
            _emailUrlBuilder = emailUrlBuilder;
            _mailBuilder = mailBuilder;
            _mailService = mailService;
        }

        public async Task<Result> SendActivationEmail(string login)
        {
            var user = await _userManager.GetUserByLogin(login);
            if (user == null)
            {
                throw new UserNotFoundException();
            }

            if (user.EmailConfirmed)
            {
                throw new UserAlreadyActivatedException();
            }

            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var mail = await _mailBuilder.ActivationEmail()
                .Build(new ActivationMailData
                {
                    Name = user.UserName,
                    ActivationUrl = _emailUrlBuilder.BuildActivationUrl(token, user.Email)
                }, new MailAddress(user.Email, user.UserName));
            await _mailService.SendAsync(mail);
            return new Result(true);
        }

        public async Task<User> ConfirmEmail(VerifyEmailInput input)
        {
            var user = await _userManager.GetUserByLogin(input.Login);
            if (user == null)
            {
                throw new UserNotFoundException();
            }
            var result = await _userManager.ConfirmEmailAsync(user, input.Token);
            if (!result.Succeeded)
            {
                throw new EmailConfirmationFailedException(result.Errors.First().Description);
            }

            return User.FromDbUser(user);
        }
    }
}
