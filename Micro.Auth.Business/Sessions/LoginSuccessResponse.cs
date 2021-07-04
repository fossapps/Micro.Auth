#nullable enable
namespace Micro.Auth.Business.Sessions
{
    public class LoginSuccessResponse
    {
        public string RefreshToken { set; get; }
        public string? Jwt { set; get; }
    }
}
