using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;

namespace Micro.Auth.Api.UserData.Extensions
{
    public static class UserIdExtension
    {
        public static string? GetUserId(this ControllerBase controller)
        {
            return controller.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        }
    }
}
