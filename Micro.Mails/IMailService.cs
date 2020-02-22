using System.Net.Mail;
using System.Threading.Tasks;
using Micro.Mails.Exceptions;

namespace Micro.Mails
{
    public interface IMailService
    {
        /// <summary>
        /// Send Email given MailMessage
        /// </summary>
        /// <param name="mailMessage"></param>
        /// <returns></returns>
        /// <exception cref="EmailSendingFailureException"></exception>
        Task SendAsync(MailMessage mailMessage);
    }
}
