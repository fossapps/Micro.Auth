namespace Micro.Auth.Business.Users.ViewModels
{
    public class LoginSuccessResponse
    {
        public string Jwt { set; get; }
        public string RefreshToken { set; get; }
    }
}
