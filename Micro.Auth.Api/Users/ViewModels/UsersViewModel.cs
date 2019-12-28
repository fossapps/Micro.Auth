using System.ComponentModel.DataAnnotations;

namespace Micro.Auth.Api.Users.ViewModels
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