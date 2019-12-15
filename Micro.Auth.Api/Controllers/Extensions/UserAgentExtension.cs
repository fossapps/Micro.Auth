using Microsoft.AspNetCore.Mvc;

namespace Micro.Auth.Api.Controllers.Extensions
{
    public static class UserAgentExtension
    {
        public static string? GetUserAgent(this ControllerBase controller)
        {
            var containsData = controller.HttpContext.Request.Headers.TryGetValue("User-Agent", out var userAgent);
            return containsData ? userAgent.ToString() : null;
        }
    }
}