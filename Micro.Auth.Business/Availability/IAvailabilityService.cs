using System.Threading.Tasks;
using Micro.Auth.Business.Users.ViewModels;

namespace Micro.Auth.Business.Availability
{
    public interface IAvailabilityService
    {
        Task<AvailabilityResponse> AvailabilityByEmail(string email);
        Task<AvailabilityResponse> AvailabilityByLogin(string login);
    }
}
