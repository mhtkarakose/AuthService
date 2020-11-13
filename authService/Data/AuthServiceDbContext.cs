using authService.Models;
using Microsoft.EntityFrameworkCore;

namespace authService.Data
{
    public class AuthServiceDbContext : DbContext
    {
        public AuthServiceDbContext(DbContextOptions options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
    }
}