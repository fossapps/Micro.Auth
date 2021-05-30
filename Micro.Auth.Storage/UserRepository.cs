using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Micro.Auth.Storage
{
    public interface IUserRepository
    {
        Task<IDictionary<string, User>> FindByIds(IEnumerable<string> ids);
        Task<IEnumerable<User>> List();
        Task<User> FindByEmail(string email);
        Task<User> FindByUsername(string username);
        Task<User> FindById(string id);
    }
    public class UserRepository : IUserRepository
    {
        private readonly UserManager<User> _userManager;
        private readonly ApplicationContext _db;

        public UserRepository(UserManager<User> userManager, ApplicationContext db)
        {
            _userManager = userManager;
            _db = db;
        }

        public async Task<IDictionary<string, User>> FindByIds(IEnumerable<string> ids)
        {
            return await _db.Users.AsNoTracking().Where(x => ids.Contains(x.Id)).ToDictionaryAsync(x => x.Id);
        }

        public async Task<IEnumerable<User>> List()
        {
            return await _db.Users.AsNoTracking().ToListAsync();
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
