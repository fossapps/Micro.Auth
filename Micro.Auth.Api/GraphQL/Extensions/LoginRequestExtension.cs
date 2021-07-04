using Micro.Auth.Api.Internal.UserData.Extensions;
using Micro.Auth.Business.Sessions;
using Microsoft.AspNetCore.Http;

namespace Micro.Auth.Api.GraphQL.Extensions
{
    public static class LoginRequestExtension
    {
        public static LoginRequest GetLoginRequest(this IHttpContextAccessor httpContextAccessor)
        {
            var (login, password) = httpContextAccessor.MustGetBasicToken();
            return new LoginRequest
            {
                Login = login,
                Password = password,
                IpAddress = httpContextAccessor.GetIpAddress(),
                UserAgent = httpContextAccessor.GetUserAgent(),
                Location = httpContextAccessor.GetRoughLocation()
            };
        }
    }
}
