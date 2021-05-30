using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Micro.Auth.Common;
using Micro.Auth.Storage.Exceptions;
using Microsoft.EntityFrameworkCore;

namespace Micro.Auth.Storage
{
    public interface IRefreshTokenRepository
    {
        Task<RefreshToken> FindById(string id);
        Task<IEnumerable<RefreshToken>> FindByUser(string userId);
        Task<ILookup<string, RefreshToken>> FindByUserIds(IEnumerable<string> userIds);
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

        public async Task<ILookup<string, RefreshToken>> FindByUserIds(IEnumerable<string> users)
        {
            var results = await _db.RefreshTokens.AsNoTracking().Where(x => users.Contains(x.User)).ToListAsync();
            return results.ToLookup(x => x.User);
        }

        public Task<RefreshToken> FindById(string id)
        {
            return _db.RefreshTokens.AsNoTracking().Where(x => x.Id == id).FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<RefreshToken>> FindByUser(string userId)
        {
            return await _db.RefreshTokens.AsNoTracking().Where(x => x.User == userId).ToListAsync();
        }

        public async Task<RefreshToken> Create(RefreshToken token)
        {
            token.Id = _uuid.GenerateUuId("refresh_token");
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
