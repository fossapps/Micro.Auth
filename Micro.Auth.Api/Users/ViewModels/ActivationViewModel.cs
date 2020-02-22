using System.ComponentModel.DataAnnotations;

namespace Micro.Auth.Api.Users.ViewModels
{
    public class SendActivationEmailRequest
    {
        [Required]
        public string Login { get; set; }
    }
    public class ConfirmEmailRequest
    {
        [Required]
        public string Login { get; set; }

        [Required]
        public string Token { get; set;  }
    }

}
