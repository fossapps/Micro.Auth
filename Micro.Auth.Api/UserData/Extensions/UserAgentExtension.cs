using Microsoft.AspNetCore.Mvc;

namespace Micro.Auth.Api.UserData.Extensions
{
    public static class UserAgentExtension
    {
        public static string? GetUserAgent(this ControllerBase controller)
        {
            var containsData = controller?.HttpContext?.Request?.Headers?.TryGetValue("User-Agent", out var userAgent);
            return containsData.HasValue && containsData.Value ? userAgent.ToString() : null;
        }
    }
}
