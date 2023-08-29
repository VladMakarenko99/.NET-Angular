using API.Models;

namespace API.Interfaces
{
    public interface IServiceRepository
    {
        public Task<List<Service>> GetServicesAsync();

        public Task BuyServiceAsync(Service service, string steamId);

        public Task RemoveExpiredServicesAsync(List<Service> expiredService, string steamId);
    }
}