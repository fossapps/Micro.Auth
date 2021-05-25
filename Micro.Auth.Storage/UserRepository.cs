using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace Micro.Auth.Storage
{
    public interface IUserRepository
    {
        Task<User> FindByEmail(string email);
        Task<User> FindByUsername(string username);
        Task<User> FindById(string id);
    }
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<User> _userManager;

        public UserRepository(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public Task<User> FindByEmail(string email)
        {
            return _userManager.FindByEmailAsync(email);
        }

        public Task<User> FindByUsername(string username)
        {
            return _userManager.FindByNameAsync(username);
        }

        public Task<User> FindById(string id)
        {
            return _userManager.FindByIdAsync(id);
        }
    }
}
