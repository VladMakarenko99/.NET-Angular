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
        
    }
}