using Microsoft.AspNetCore.Mvc;

namespace Micro.Auth.Api.Controllers.Extensions
{
    public static class LocationExtension
    {
        public static string GetRoughLocation(this ControllerBase controller)
        {
            return "Bangkok";
        }
    }
}
