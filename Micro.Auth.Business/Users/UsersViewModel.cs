using System.ComponentModel.DataAnnotations;

namespace Micro.Auth.Business.Users
{
    public class CreateUserRequest
    {
        [Required]
        [MinLength(3)]
        public string Username { set; get; }

        [Required]
        [EmailAddress]
        public string Email { set; get; }

        [Required]
        public string Password { set; get; }
    }
}