using System.ComponentModel.DataAnnotations;

namespace Micro.Auth.Business.PasswordManager
{
    public class ResetPasswordRequest
    {
        [Required]
        public string Login { set; get; }

        [Required]
        public string Token { set; get; }
        
        [Required]
        [MinLength(8)]
        public string NewPassword { set; get; }
    }
}
