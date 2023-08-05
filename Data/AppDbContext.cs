using Microsoft.EntityFrameworkCore;
using API.Models;

namespace API.Data;

public class AppDbContext : DbContext
{
    public DbSet<User> Users { get; set; } = null!;
    

    public AppDbContext(DbContextOptions<AppDbContext> option) : base(option) { }

    public AppDbContext()
    {
    }
}