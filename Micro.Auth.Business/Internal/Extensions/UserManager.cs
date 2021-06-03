using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Micro.Auth.Storage;
using Microsoft.AspNetCore.Identity;

namespace Micro.Auth.Business.Internal.Extensions
{
    public static class UserManager
    {
        public static Task<User> GetUserByLogin(this UserManager<User> userManager, string login)
        {
            return login.Contains("@")
                ? userManager.FindByEmailAsync(login)
                : userManager.FindByNameAsync(login);
        }

        public static int GetTokenExpiry(this ClaimsPrincipal principal, int defaultValue = 15)
        {
            if (!principal.IsServiceAccount())
            {
                return defaultValue;
            }

            var expiry = principal.Claims.FirstOrDefault(x => x.Type == "token_expiry");
            if (expiry == null)
            {
                return defaultValue; // by default if this claim doesn't exist on service account, we still return 15
            }

            var parsed = int.TryParse(expiry.Value, out var result);
            return parsed ? result : defaultValue;
        }

        public static bool IsServiceAccount(this ClaimsPrincipal principal)
        {
            return principal.IsInRole("service_account");
        }
    }
}
