using Microsoft.AspNetCore.Mvc;

namespace Micro.Auth.Api.UserData.Extensions
{
    public static class LocationExtension
    {
        public static string GetRoughLocation(this ControllerBase controller)
        {
            return "Bangkok";
        }
    }
}
