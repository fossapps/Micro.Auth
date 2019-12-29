using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Micro.Mails
{
    public class SmtpMailService : IMailService
    {
        private readonly SmtpClient _smtpClient;
        public SmtpMailService(Smtp mailConfig)
        {
            var client = new SmtpClient(mailConfig.Host, mailConfig.Port)
            {
                Credentials = new NetworkCredential(mailConfig.User, mailConfig.Password)
            };
            _smtpClient = client;
        }

        public Task SendAsync(MailMessage mailMessage)
        {
            return _smtpClient.SendMailAsync(mailMessage);
        }
    }
}
