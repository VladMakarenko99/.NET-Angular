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

        public async Task<User?> GetBySteamIdAsync(string steamId) => await _context.Users.FirstOrDefaultAsync(x => x.SteamId == steamId);

        public async Task<List<User>> GetUsersAsync() => await _context.Users.ToListAsync();

        public async Task<bool> IsUserExistAsync(string steamId)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.SteamId == steamId);
            if (user != null)
                return true;
            return false;
        }
    }
}