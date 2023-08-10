using System.Text.Json;
using API.Data;
using API.Interfaces;
using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;
        public UserRepository(AppDbContext _context) => this._context = _context;

        public async Task CreateUserAsync(User user)
        {
            await _context.AddAsync(user);
            await _context.SaveChangesAsync();
        }

        public async Task<User?> GetBySteamIdAsync(string steamId)
        {
            return await _context.Users.FirstOrDefaultAsync(x => x.SteamId == steamId);
        }

        public async Task<List<User>> GetUsersAsync()
        {
            return await _context.Users.ToListAsync();
        }

        public async Task<bool> TryBuyServiceAsync(int serviceId, int userId)
        {
            var user = await _context.Users.FindAsync(userId);
            var service = await _context.Services.FindAsync(serviceId);
            if(user == null || service == null)
                return false;
            var serviceList = new List<Service>();

            if (user!.BoughtServicesJson == null)
                await UpdateUserBoughtServicesAsync(user, service!, serviceList);

            else
            {
                serviceList = JsonSerializer.Deserialize<List<Service>>(user.BoughtServicesJson);
                await UpdateUserBoughtServicesAsync(user, service!, serviceList!);
            }
            return true;
        }

        private async Task UpdateUserBoughtServicesAsync(User userToUpdate, Service service, List<Service> serviceList)
        {
            serviceList!.Add(service!);
            string serviceListJson = JsonSerializer.Serialize(serviceList);
            userToUpdate.BoughtServicesJson = serviceListJson;
            _context.Update(userToUpdate);
            await _context.SaveChangesAsync();
        }
    }
}