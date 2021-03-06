namespace Micro.Auth.Api.Users.ViewModels
{
    public class FindByEmailResponse
    {
        public string Email { set; get; }
        public bool Available { set; get; }
    }

    public class FindByUsernameResponse
    {
        public string Username { set; get; }
        public bool Available { set; get; }
    }
}
