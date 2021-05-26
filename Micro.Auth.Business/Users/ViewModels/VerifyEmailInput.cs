using System.ComponentModel.DataAnnotations;

namespace Micro.Auth.Business.Users.ViewModels
{
    public class VerifyEmailInput
    {
        [Required]
        public string Login { get; set; }

        [Required]
        public string Token { get; set;  }
    }
}
