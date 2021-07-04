using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Micro.Auth.Api.Internal.UserData.Extensions
{
    public static class LocationExtension
    {
        public static string GetRoughLocation(this ControllerBase controller)
        {
            return "Bangkok";
        }
        public static string GetRoughLocation(this IHttpContextAccessor ctx)
        {
            return "Bangkok";
        }
    }
}
