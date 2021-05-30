using System.ComponentModel.DataAnnotations;

namespace Micro.Auth.Business.PasswordManager
{
    public class ChangePasswordRequest
    {
        [Required]
        public string OldPassword { set; get; }

        [Required]
        public string NewPassword { set; get; }
    }
}
