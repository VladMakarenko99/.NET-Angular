using System.Text.Json;
using System.Text.RegularExpressions;
using API.Data;
using API.Interfaces;
using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Repository
{
    public class ServiceRepository : IServiceRepository
    {
        private readonly AppDbContext _context;
        public ServiceRepository(AppDbContext _context) => this._context = _context;

        public async Task<List<Service>> GetServicesAsync() => await _context.Services.ToListAsync();

        public async Task BuyServiceAsync(Service service, string steamId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.SteamId == steamId);
            var serviceList = new List<Service>();

            string option = service!.SelectedOption!;
            string pattern = @"\d+";

            MatchCollection matches = Regex.Matches(option, pattern);

            double price = Convert.ToDouble(matches[0].Value);

            if (user!.BoughtServicesJson == null)
                await UpdateUserBoughtServicesAsync(user, service!, serviceList);
            else
            {
                serviceList = JsonSerializer.Deserialize<List<Service>>(user.BoughtServicesJson);
                await UpdateUserBoughtServicesAsync(user, service!, serviceList!);
            }
            user.Balance -= price;
            _context.Update(user);
            await _context.SaveChangesAsync();


        }

        public async Task DeleteBoughtServicesAsync(string steamId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.SteamId == steamId);
            user!.BoughtServicesJson = null;
            _context.Update(user);
            await _context.SaveChangesAsync();
        }

        private async Task UpdateUserBoughtServicesAsync(User userToUpdate, Service service, List<Service> serviceList)
        {
            serviceList!.Add(service!);

            string serviceListJson = JsonSerializer.Serialize(serviceList);
            userToUpdate.BoughtServicesJson = serviceListJson;
            _context.Update(userToUpdate);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> IsUserExistAsync(string steamId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.SteamId == steamId);
            if (user != null)
                return true;
            return false;
        }
        public async Task DeleteLastBoughtServiceAsync(string steamId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.SteamId == steamId);
            var serviceList = new List<Service>();
            if (user!.BoughtServicesJson != null)
            {
                serviceList = JsonSerializer.Deserialize<List<Service>>(user.BoughtServicesJson);
                serviceList!.RemoveAt(serviceList.Count - 1);
                user.BoughtServicesJson = JsonSerializer.Serialize(serviceList);
                _context.Update(user);
                await _context.SaveChangesAsync();
            }

        }
    }
}