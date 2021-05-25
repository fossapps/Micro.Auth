using System.Threading.Tasks;
using Micro.Auth.Api.Tokens;
using Micro.Auth.Api.Users;
using Micro.Auth.Storage;
using Microsoft.AspNetCore.Identity;

namespace Micro.Auth.Api.RefreshTokens
{
    public interface IRefreshTokenService
    {
        Task<string> Refresh(string token);
    }

    public class RefreshTokenService : IRefreshTokenService
    {
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IUserRepository _userRepository;
        private readonly SignInManager<User> _signInManager;
        private readonly ITokenFactory _tokenFactory;

        public RefreshTokenService(IRefreshTokenRepository refreshTokenRepository, IUserRepository userRepository, SignInManager<User> signInManager, ITokenFactory tokenFactory)
        {
            _refreshTokenRepository = refreshTokenRepository;
            _userRepository = userRepository;
            _signInManager = signInManager;
            _tokenFactory = tokenFactory;
        }

        public async Task<string> Refresh(string token)
        {
            var refreshToken = await _refreshTokenRepository.TouchLastUsed(token);
            var user = await _userRepository.FindById(refreshToken.User);
            var principal = await _signInManager.CreateUserPrincipalAsync(user);
            return _tokenFactory.GenerateJwtToken(principal);
        }
    }
}
