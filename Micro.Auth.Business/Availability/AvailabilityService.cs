using System.Threading.Tasks;
using Micro.Auth.Storage;
using Microsoft.AspNetCore.Identity;

namespace Micro.Auth.Business.Availability
{
    public class AvailabilityService : IAvailabilityService
    {
        private readonly UserManager<User> _userManager;

        public AvailabilityService(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<AvailabilityResponse> AvailabilityByEmail(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            return new AvailabilityResponse(user == null);
        }

        public async Task<AvailabilityResponse> AvailabilityByLogin(string login)
        {
            var user = await _userManager.FindByNameAsync(login);
            return new AvailabilityResponse(user == null);
        }
    }
}
