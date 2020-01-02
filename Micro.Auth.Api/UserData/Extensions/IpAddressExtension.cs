using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace Micro.Auth.Api.UserData.Extensions
{
    public static class IpAddressExtension
    {
        public static IPAddress GetIpAddress(this ControllerBase controller)
        {
            // todo: this might not be accurate behind a load balancer, so we might have to get X-Forwarded-For
            return controller?.HttpContext?.Connection?.RemoteIpAddress;
        }
    }
}
