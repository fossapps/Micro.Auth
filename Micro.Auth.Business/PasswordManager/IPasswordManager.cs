using System.Threading.Tasks;
using Micro.Auth.Business.Users;
using Micro.Auth.Business.Users.ViewModels;

namespace Micro.Auth.Business.PasswordManager
{
    public interface IPasswordManager
    {
        Task<Result> RequestPasswordReset(string login);
        Task<User> ResetPassword(ResetPasswordRequest request);
        Task<User> ChangePassword(string userId, ChangePasswordRequest request);
    }
}
