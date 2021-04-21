using CqrsMediator.Demo.Dal.Entities;

using Microsoft.EntityFrameworkCore;

namespace CqrsMediator.Demo.Dal
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options)
            : base(options)
        {
        }

        public DbSet<Product> Products { get; set; }
        public DbSet<Order> Orders { get; set; }


    }
}
