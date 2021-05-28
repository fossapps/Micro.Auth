using System.Threading.Tasks;
using Micro.Auth.Business.Common;
using Micro.Auth.Business.Users;

namespace Micro.Auth.Business.EmailVerification
{
    public interface IEmailVerificationService
    {
        Task<Result> SendActivationEmail(string login);
        Task<User> ConfirmEmail(VerifyEmailInput input);
    }
}
