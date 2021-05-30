using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GraphQL.DataLoader;
using Micro.Auth.Storage;

namespace Micro.Auth.Api.GraphQL.DataLoaders
{
    public class SessionByUserDataLoader : DataLoaderBase<string, IEnumerable<RefreshToken>>
    {
        private readonly IRefreshTokenRepository _refreshTokenRepository;

        public SessionByUserDataLoader(IRefreshTokenRepository refreshTokenRepository)
        {
            _refreshTokenRepository = refreshTokenRepository;
        }

        protected override async Task FetchAsync(IEnumerable<DataLoaderPair<string, IEnumerable<RefreshToken>>> list, CancellationToken cancellationToken)
        {
            var userIds = list.Select(x => x.Key);
            var sessions = await _refreshTokenRepository.FindByUserIds(userIds);
            foreach (var entry in list)
            {
                entry.SetResult(sessions[entry.Key]);
            }
        }
    }
}
