using System.Linq;
using System.Threading.Tasks;
using Micro.Auth.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Micro.Auth.Api.RefreshTokens
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshToken> FindById(string id);
        Task<RefreshToken> FindByUser(string userId);
        Task<RefreshToken> Create(RefreshToken token);
    }

    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly ApplicationContext _db;

        public RefreshTokenRepository(ApplicationContext db)
        {
            _db = db;
        }

        public Task<RefreshToken> FindById(string id)
        {
            return _db.RefreshTokens.AsNoTracking().Where(x => x.Id == id).FirstOrDefaultAsync();
        }

        public Task<RefreshToken> FindByUser(string userId)
        {
            return _db.RefreshTokens.AsNoTracking().Where(x => x.User == userId).FirstOrDefaultAsync();
        }

        public async Task<RefreshToken> Create(RefreshToken token)
        {
            var result = await _db.RefreshTokens.AddAsync(token);
            await _db.SaveChangesAsync();
            return result.Entity;
        }
    }
}
