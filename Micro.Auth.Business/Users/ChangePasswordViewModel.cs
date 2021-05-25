using System.ComponentModel.DataAnnotations;

namespace Micro.Auth.Business.Users
{
    public class ChangePasswordRequest
    {
        [Required]
        public string OldPassword { set; get; }

        [Required]
        public string NewPassword { set; get; }
    }
}
