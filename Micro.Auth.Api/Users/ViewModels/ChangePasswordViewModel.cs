using System.ComponentModel.DataAnnotations;

namespace Micro.Auth.Api.Users.ViewModels
{
    public class ChangePasswordRequest
    {
        [Required]
        public string OldPassword { set; get; }

        [Required]
        public string NewPassword { set; get; }
    }
}
