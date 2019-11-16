using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Micro.Auth.Api.Models;

namespace Micro.Auth.Api.Repository
{
    public interface IWeatherRepository
    {
        Task<IEnumerable<Weather>> GetAll();
        Task<Weather> FindById(string id);
        Task<Weather> Create([NotNull] Weather weather);
        Task Delete(string id);
    }
}
