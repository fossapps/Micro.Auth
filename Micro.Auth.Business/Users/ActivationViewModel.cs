using System.ComponentModel.DataAnnotations;

namespace Micro.Auth.Business.Users
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
