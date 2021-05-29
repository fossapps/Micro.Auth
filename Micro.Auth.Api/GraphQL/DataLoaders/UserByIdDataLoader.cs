using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using GraphQL.DataLoader;
using Micro.Auth.Storage;
using User = Micro.Auth.Business.Users.User;

namespace Micro.Auth.Api.GraphQL.DataLoaders
{
    public class UserByIdDataLoader : DataLoaderBase<string, User>
    {
        private readonly IUserRepository _userRepository;

        public UserByIdDataLoader(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        protected override async Task FetchAsync(IEnumerable<DataLoaderPair<string, User>> list, CancellationToken cancellationToken)
        {
            var ids = list.Select(x => x.Key).ToList();
            var users = await _userRepository.FindByIds(ids.ToArray());
            foreach (var entry in list)
            {
                var exists = users.TryGetValue(entry.Key, out var user);
                entry.SetResult(exists ? User.FromDbUser(user) : null);
            }
        }
    }
}
