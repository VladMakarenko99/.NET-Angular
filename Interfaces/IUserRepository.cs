using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Models;

namespace API.Interfaces
{
    public interface IUserRepository
    {
        public Task<User?> GetByEmailAsync(string email);
        
        public Task CreateAsync(User user);
    }
}