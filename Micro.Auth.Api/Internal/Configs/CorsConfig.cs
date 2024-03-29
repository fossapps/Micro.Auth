using System.Collections.Generic;

namespace Micro.Auth.Api.Internal.Configs
{
    public class CorsConfig
    {
        public IEnumerable<string> Origins { set; get; }
        public IEnumerable<string> Headers { set; get; }
        public bool AllowCredentials { set; get; }
        public string PolicyToUse { set; get; }
    }
}
