using System.Threading.Tasks;
using Micro.Auth.Storage;
using Microsoft.AspNetCore.Identity;

namespace Micro.Auth.Business.Extensions
{
    public static class UserManager
    {
        public static Task<User> GetUserByLogin(this UserManager<User> userManager, string login)
        {
            return login.Contains("@")
                ? userManager.FindByEmailAsync(login)
                : userManager.FindByNameAsync(login);
        }
    }
}
