using Micro.Mails;

namespace Micro.Auth.Api.Configs
{
    public class Mail
    {
        public Smtp Smtp { set; get; }
        public Sender DefaultSender { set; get; }
    }
}
