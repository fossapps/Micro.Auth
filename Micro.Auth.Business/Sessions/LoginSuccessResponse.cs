namespace Micro.Auth.Business.Sessions
{
    public class LoginSuccessResponse : ServiceAccountLoginResponse
    {
        public string RefreshToken { set; get; }
    }

    public class ServiceAccountLoginResponse
    {
        public string Jwt { set; get; }
    }
}
