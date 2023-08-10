using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Models;

namespace API.Interfaces
{
    public interface IUserRepository
    {
        public Task<List<User>> GetUsersAsync();

        public Task<User?> GetBySteamIdAsync(string steamId);
        
        public Task CreateUserAsync(User user);

        public Task<bool> TryBuyServiceAsync(int serviceId, int userId);
    }
}