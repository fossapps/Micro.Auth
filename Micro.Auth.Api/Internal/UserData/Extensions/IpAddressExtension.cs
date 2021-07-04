using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Micro.Auth.Api.Internal.UserData.Extensions
{
    public static class IpAddressExtension
    {
        public static IPAddress GetIpAddress(this ControllerBase controller)
        {
            // todo: this might not be accurate behind a load balancer, so we might have to get X-Forwarded-For
            return controller?.HttpContext?.Connection?.RemoteIpAddress;
        }
        public static IPAddress GetIpAddress(this IHttpContextAccessor ctx)
        {
            // todo: this might not be accurate behind a load balancer, so we might have to get X-Forwarded-For
            return ctx.HttpContext?.Connection.RemoteIpAddress;
        }
    }
}
