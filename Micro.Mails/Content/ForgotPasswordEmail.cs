using System.Net.Mail;
using System.Threading.Tasks;

namespace Micro.Mails.Content
{
    public class ForgotPasswordEmail : BaseEmail
    {
        public ForgotPasswordEmail(Sender mail) : base(mail)
        {
        }
        public async Task<MailMessage> Build(ForgotPasswordEmailDetails details, MailAddress recipient)
        {
            var template = await GetHtmlTemplateAsync();
            var content = template.Replace("{{PasswordResetUrl}}", details.PasswordResetUrl)
                .Replace("{{Name}}", details.Name);
            var messageBuilder = new MailMessageBuilder();
            var mailMessageCollection = new MailAddressCollection() {recipient};
            return messageBuilder
                .From(new MailAddress(MailConfig.From, MailConfig.Name))
                .WithSender(new MailAddress(MailConfig.From, MailConfig.Name))
                .WithSubject("You requested to reset your password")
                .WithHtmlBody(content)
                .AddRecipients(mailMessageCollection)
                .Build();
        }
    }

    public class ForgotPasswordEmailDetails
    {
        public string PasswordResetUrl { set; get; }
        public string Name { set; get; }
    }
}
