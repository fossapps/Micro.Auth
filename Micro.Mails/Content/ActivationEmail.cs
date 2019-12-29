using System.Net.Mail;
using System.Threading.Tasks;

namespace Micro.Mails.Content
{
    public class ActivationEmail : BaseEmail
    {
        public ActivationEmail(Sender mail) : base(mail)
        {
        }

        public async Task<MailMessage> Build(ActivationMailData mailData, MailAddress recipient)
        {
            var template = await GetHtmlTemplateAsync();
            var content = template.Replace("{{ActivationUrl}}", mailData.ActivationUrl)
                .Replace("{{Name}}", mailData.Name);
            var messageBuilder = new MailMessageBuilder();
            var mailMessageCollection = new MailAddressCollection {recipient};

            return messageBuilder
                .From(new MailAddress(MailConfig.From, MailConfig.Name))
                .WithSender(new MailAddress(MailConfig.From, MailConfig.Name))
                .WithSubject("Activate your Account")
                .WithHtmlBody(content)
                .AddRecipients(mailMessageCollection)
                .Build();
        }
    }

    public class ActivationMailData
    {
        public string Name { set; get; }
        public string ActivationUrl { set; get; }
    }
}
