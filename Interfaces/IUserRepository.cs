using API.Models;

namespace API.Interfaces
{
    public interface IUserRepository
    {
        public Task<List<User>> GetUsersAsync();

        public Task<User?> GetBySteamIdAsync(string steamId);

        public Task CreateUserAsync(User user);

        public Task<bool> IsUserExistAsync(string steamId);

    }
}