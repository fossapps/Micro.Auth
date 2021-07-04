using System.Net;

namespace Micro.Auth.Business.Sessions
{
    public record LoginRequest
    {
        public string Login { set; get; }
        public string Password { set; get; }
        public string UserAgent { set; get; }
        public IPAddress IpAddress { set; get; }
        public string Location { set; get; }
    }
}
