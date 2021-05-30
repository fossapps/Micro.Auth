using System.Threading.Tasks;
using Fossapps.Micro.KeyStore;
using Fossapps.Micro.KeyStore.Models;
using Micro.Auth.Business.Internal.Keys.Exceptions;

namespace Micro.Auth.Business.Internal.Keys
{
    public interface IKeyResolver
    {
        Task<string> ResolveKey(string keyId);
    }

    public class KeyResolver : IKeyResolver
    {
        private readonly IKeyStoreClient _keyStoreClient;

        public KeyResolver(IKeyStoreClient keyStoreClient)
        {
            _keyStoreClient = keyStoreClient;
        }

        public async Task<string> ResolveKey(string keyId)
        {
            var response = await _keyStoreClient.Keys.GetAsync(keyId);
            return response switch
            {
                KeyCreatedResponse keyCreatedResponse => keyCreatedResponse.Body,
                _ => throw new KeyNotFoundException($"key: '{keyId}' not found")
            };
        }
    }
}
