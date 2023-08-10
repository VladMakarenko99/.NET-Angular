using API.Models;

namespace API.Interfaces
{
    public interface IServiceRepository
    {
        public Task<List<Service>> GetServicesAsync();
    }
}