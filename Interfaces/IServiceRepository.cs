using API.Models;

namespace API.Interfaces
{
    public interface IServiceRepository
    {
        public Task<List<Service>> GetServicesAsync();

        public Task BuyServiceAsync(Service service, string steamId);

        public Task DeleteBoughtServicesAsync(string steamId);
        public Task<bool> IsUserExistAsync(string steamId);
        public Task DeleteLastBoughtServiceAsync(string steamId);
    }
}