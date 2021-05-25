using System;
using System.Net;
using Micro.Auth.Storage;

namespace Micro.Auth.Api.Users.ViewModels
{
    public class LoginRequest
    {
        public string Login { set; get; }
        public string Password { set; get; }
        public string UserAgent { set; get; }
        public IPAddress IpAddress { set; get; }
        public string Location { set; get; }
        public User User { set; get; }
    }

    public static class LoginRequestExtensions
    {
        public static RefreshToken ToRefreshToken(this LoginRequest request, string tokenValue)
        {
            return new RefreshToken
            {
                Location = request.Location,
                Useragent = request.UserAgent,
                User = request.User.Id,
                IpAddress = request.IpAddress,
                LastUsed = DateTime.Now,
                Value = tokenValue
            };
        }
    }

    public class LoginSuccessResponse
    {
        public string Jwt { set; get; }
        public string RefreshToken { set; get; }
    }
}
