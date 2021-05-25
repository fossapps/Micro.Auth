using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;

namespace Micro.Auth.Api.Internal.UserData.Extensions
{
    public static class UserAgentExtension
    {
        public static string? GetUserAgent(this ControllerBase controller)
        {
            var userAgent = StringValues.Empty;
            var containsData = controller?.HttpContext?.Request?.Headers?.TryGetValue("User-Agent", out userAgent);
            return containsData.HasValue && containsData.Value ? userAgent.ToString() : null;
        }
    }
}
