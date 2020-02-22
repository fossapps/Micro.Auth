using System;
using System.Linq;
using System.Threading.Tasks;
using Micro.Auth.Api.Models;
using Micro.Auth.Api.RefreshTokens.Exceptions;
using Micro.Auth.Api.Uuid;
using Microsoft.EntityFrameworkCore;

namespace Micro.Auth.Api.RefreshTokens
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshToken> FindById(string id);
        Task<RefreshToken> FindByUser(string userId);
        Task<RefreshToken> Create(RefreshToken token);
        Task Delete(string id);
        Task<RefreshToken> TouchLastUsed(string id);
    }

    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly ApplicationContext _db;
        private readonly IUuidService _uuid;

        public RefreshTokenRepository(ApplicationContext db, IUuidService uuid)
        {
            _db = db;
            _uuid = uuid;
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
            token.Id = _uuid.GenerateUuId();
            var result = await _db.RefreshTokens.AddAsync(token);
            await _db.SaveChangesAsync();
            return result.Entity;
        }

        public Task Delete(string id)
        {
            _db.RefreshTokens.Remove(new RefreshToken {Id = id});
            return _db.SaveChangesAsync();
        }

        public async Task<RefreshToken> TouchLastUsed(string id)
        {
            var token = await FindById(id);
            if (token == null)
            {
                throw new RefreshTokenNotFoundException();
            }
            token.LastUsed = DateTime.Now;
            var entry = _db.RefreshTokens.Update(token);
            await _db.SaveChangesAsync();
            return entry.Entity;
        }
    }
}
