using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Models;

namespace API.Interfaces
{
    public interface IOrderRepository
    {
        public Task CreateOrderAsync(Order order);

        public Task CompleteOrderAsync(string orderId);

        public Task<Order?> FindOrderByIdAsync(string id);
    }
}