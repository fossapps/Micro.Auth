using System.ComponentModel.DataAnnotations;

namespace Micro.Auth.Business.Users.ViewModels
{
    public class SendActivationEmailRequest
    {
        [Required]
        public string Login { get; set; }
    }
}
