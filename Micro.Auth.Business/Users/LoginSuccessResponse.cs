namespace Micro.Auth.Business.Users
{
    public class LoginSuccessResponse
    {
        public string Jwt { set; get; }
        public string RefreshToken { set; get; }
    }
}
