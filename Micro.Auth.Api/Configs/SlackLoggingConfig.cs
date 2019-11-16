using Microsoft.Extensions.Logging;

namespace Micro.Auth.Api.Configs
{
    public class SlackLoggingConfig
    {
        public string WebhookUrl { set; get; }
        public LogLevel MinLogLevel { set; get; }
    }
}
