namespace Micro.Auth.Business.Sessions
{
    public class LoginSuccessResponse
    {
        public string Jwt { set; get; }
        public string RefreshToken { set; get; }
    }
}
