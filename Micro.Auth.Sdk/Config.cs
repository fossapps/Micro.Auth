using System;

namespace Micro.Auth.Sdk
{
    public class Config
    {
        public string KeyStoreUrl { set; get; }
        public bool ValidateIssuer { set; get; } = true;
        public bool ValidateIssuerSigningKey { set; get; } = true;
        public bool ValidateLifetime { set; get; } = true;
        public bool ValidateActor { set; get; } = true;
        public bool ValidateAudience { set; get; } = true;
        public string ValidIssuer { set; get; }
        public string[] ValidAudiences { set; get; }
        public TimeSpan ClockSkew { set; get; } = TimeSpan.Zero;
    }
}
