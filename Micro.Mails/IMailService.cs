using System.Net.Mail;
using System.Threading.Tasks;

namespace Micro.Mails
{
    public interface IMailService
    {
        Task SendAsync(MailMessage mailMessage);
    }
}
