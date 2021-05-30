using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Micro.Auth.Api.Internal.UserData.Extensions
{
    public static class UserIdExtension
    {
        public static string? GetUserId(this ControllerBase controller)
        {
            return controller.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }

        public static string? GetUserId(this IHttpContextAccessor httpContextAccessor)
        {
            return httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }
    }
}
