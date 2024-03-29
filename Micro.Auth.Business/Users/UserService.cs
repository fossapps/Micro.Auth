using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Micro.Auth.Business.EmailVerification;
using Micro.Auth.Business.Internal.Extensions;
using Micro.Auth.Storage;
using Microsoft.AspNetCore.Identity;

namespace Micro.Auth.Business.Users
{
    public interface IUserService
    {
        Task<User> Create(RegisterInput request);
        Task<User> FindById(string id);
        Task<User> FindByLogin(string login);
        Task<User> FindByEmail(string login);
        Task<IEnumerable<User>> List();
    }

    public class UserService : IUserService
    {
        private readonly UserManager<Micro.Auth.Storage.User> _userManager;
        private readonly IEmailVerificationService _emailVerificationService;
        private readonly IUserRepository _userRepository;

        public UserService(UserManager<Micro.Auth.Storage.User> userManager, IEmailVerificationService emailVerificationService, IUserRepository userRepository)
        {
            _userManager = userManager;
            _emailVerificationService = emailVerificationService;
            _userRepository = userRepository;
        }

        public async Task<User> Create(RegisterInput request)
        {
            var result = await _userManager.CreateAsync(new Micro.Auth.Storage.User
            {
                Email = request.Email,
                UserName = request.Username
            }, request.Password);
            if (!result.Succeeded)
            {
                throw new CreateUserFailedException(result.Errors.First().Description);
            }

            var dbUser = await _userManager.FindByEmailAsync(request.Email);
            await _emailVerificationService.SendActivationEmail(dbUser.Email);
            return User.FromDbUser(dbUser);
        }

        public async Task<User> FindById(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                throw new UserNotFoundException();
            }
            return User.FromDbUser(user);
        }

        public async Task<User> FindByLogin(string login)
        {
            var user = await _userManager.GetUserByLogin(login);
            if (user == null)
            {
                throw new UserNotFoundException();
            }
            return User.FromDbUser(user);
        }

        public async Task<User> FindByEmail(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                throw new UserNotFoundException();
            }
            return User.FromDbUser(user);
        }

        public async Task<IEnumerable<User>> List()
        {
            var users = await _userRepository.List();
            return users.Select(User.FromDbUser);
        }
    }
}
