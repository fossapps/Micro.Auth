using System.Collections.Generic;
using System.Threading.Tasks;
using Micro.Auth.Business.Tokens;
using Micro.Auth.Storage;
using Microsoft.AspNetCore.Identity;

namespace Micro.Auth.Business.RefreshTokens
{
    public interface ISessionService
    {
        Task<string> Refresh(string token);
        Task<IEnumerable<RefreshToken>> GetForUser(string userId);
    }

    public class SessionService : ISessionService
    {
        private readonly IRefreshTokenRepository _refreshTokenRepository;
        private readonly IUserRepository _userRepository;
        private readonly SignInManager<User> _signInManager;
        private readonly ITokenFactory _tokenFactory;

        public SessionService(IRefreshTokenRepository refreshTokenRepository, IUserRepository userRepository, SignInManager<User> signInManager, ITokenFactory tokenFactory)
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

        public async Task<IEnumerable<RefreshToken>> GetForUser(string userId)
        {
            return await _refreshTokenRepository.FindByUser(userId);
        }
    }
}
