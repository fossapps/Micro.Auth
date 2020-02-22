using System.ComponentModel.DataAnnotations;

namespace Micro.Auth.Api.Users.ViewModels
{
    public class SendActivationEmailRequest
    {
        [Required]
        public string Login { get; set; }
    }
}
