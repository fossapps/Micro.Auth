using Micro.Mails;

namespace Micro.Auth.Api.Internal.Configs
{
    public class Mail
    {
        public EmailUrlConfig EmailUrlConfig { set; get; }
        public Smtp Smtp { set; get; }
        public Sender DefaultSender { set; get; }
    }
}
