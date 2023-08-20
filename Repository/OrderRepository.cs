using API.Interfaces;
using API.Models;
using API.Data;
using Microsoft.EntityFrameworkCore;

namespace API.Repository
{
    public class OrderRepository : IOrderRepository
    {
        private readonly AppDbContext _context;
        public OrderRepository(AppDbContext _context) => this._context = _context;

        public async Task CreateOrderAsync(Order order)
        {
            await _context.Orders.AddAsync(order);
            await _context.SaveChangesAsync();
        }
        public async Task CompleteOrderAsync(string orderId)
        {
            var order = await _context.Orders.FirstOrDefaultAsync(x => x.Id == orderId);
            var user = await _context.Users.FirstOrDefaultAsync(x => x.SteamId == order!.SteamId);

            user!.Balance += order!.Amount;
            _context.Update(user);

            order.Status = "completed";
            _context.Update(order);
            await _context.SaveChangesAsync();
        }
    }
}