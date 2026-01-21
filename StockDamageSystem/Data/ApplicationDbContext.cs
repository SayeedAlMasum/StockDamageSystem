using Microsoft.EntityFrameworkCore;
using StockDamageSystem.Models;

namespace StockDamageSystem.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<StockDamage> StockDamages { get; set; }
        public DbSet<Godown> Godowns { get; set; }
        public DbSet<SubItemCode> SubItemCodes { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Stock> Stocks { get; set; }
    }
}
