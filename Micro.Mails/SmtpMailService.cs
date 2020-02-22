using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Micro.Mails.Exceptions;

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

        /// <summary>
        /// Send Email given MailMessage
        /// </summary>
        /// <param name="mailMessage"></param>
        /// <returns></returns>
        /// <exception cref="EmailSendingFailureException"></exception>
        public async Task SendAsync(MailMessage mailMessage)
        {
            try
            {
                await _smtpClient.SendMailAsync(mailMessage);
            }
            catch (Exception e)
            {
                throw new EmailSendingFailureException(e.Message, e);
            }
        }
    }
}
