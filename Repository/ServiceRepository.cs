using System.Text.Json;
using System.Text.RegularExpressions;
using API.Data;
using API.Interfaces;
using API.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Encodings.Web;
using System.Text.Unicode;

namespace API.Repository
{
    public class ServiceRepository : IServiceRepository
    {
        private readonly AppDbContext _context;
        public ServiceRepository(AppDbContext _context) => this._context = _context;

        public async Task<List<Service>> GetServicesAsync() => await _context.Services.ToListAsync();

        public async Task<Service?> GetServiceByIdAsync(int id) => await _context.Services.FindAsync(id);

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

        public async Task RemoveExpiredServicesAsync(List<Service> expiredService, string steamId)
        {
            var user = await _context.Users.FirstAsync(x => x.SteamId == steamId);
            var userServices = JsonSerializer.Deserialize<List<Service>>(user.BoughtServicesJson!);

            userServices!.RemoveAll(userService => expiredService.Any(expiredService => expiredService.Id == userService.Id));
            if (userServices.Count == 0)
                user.BoughtServicesJson = null;
            else
            {
                string serviceListJson = JsonSerializer.Serialize(userServices,
                new JsonSerializerOptions { Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic) });
                user.BoughtServicesJson = Regex.Replace(serviceListJson, @"\\u20B4", "₴");
            }

            _context.Update(user);
            await _context.SaveChangesAsync();
        }
        private async Task UpdateUserBoughtServicesAsync(User userToUpdate, Service service, List<Service> serviceList)
        {
            serviceList!.Add(service!);

            string serviceListJson = JsonSerializer.Serialize(serviceList,
            new JsonSerializerOptions { Encoder = JavaScriptEncoder.Create(UnicodeRanges.BasicLatin, UnicodeRanges.Cyrillic) });
            userToUpdate.BoughtServicesJson = Regex.Replace(serviceListJson, @"\\u20B4", "₴");
            _context.Update(userToUpdate);
            await _context.SaveChangesAsync();
        }
    }
}