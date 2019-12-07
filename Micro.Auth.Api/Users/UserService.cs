using System.Threading.Tasks;
using Micro.Auth.Api.Controllers.ViewModels;
using Micro.Auth.Api.Models;
using Microsoft.AspNetCore.Identity;

namespace Micro.Auth.Api.Users
{
    public interface IUserService
    {
        Task<IdentityResult> Create(CreateUserRequest request);
    }

    public class UserService : IUserService
    {
        private readonly UserManager<User> _userManager;

        public UserService(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public Task<IdentityResult> Create(CreateUserRequest request)
        {
            return _userManager.CreateAsync(new User
            {
                Email = request.Email,
                UserName = request.Username
            }, request.Password);
        }
    }
}
