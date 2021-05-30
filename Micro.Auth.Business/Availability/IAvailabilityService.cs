using System.Threading.Tasks;

namespace Micro.Auth.Business.Availability
{
    public interface IAvailabilityService
    {
        Task<AvailabilityResponse> AvailabilityByEmail(string email);
        Task<AvailabilityResponse> AvailabilityByLogin(string login);
    }
}
