using System;
using System.Net;

namespace Micro.Auth.Storage
{
    public class RefreshToken
    {
        public string Id { set; get; }
        public string User { set; get; }
        public string Value { set; get; }
        public string Useragent { set; get; }
        public DateTime LastUsed { set; get; }
        public IPAddress IpAddress { set; get; }
        public string Location { set; get; }
    }
}
