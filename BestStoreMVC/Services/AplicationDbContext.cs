using BestStoreMVC.Models;
using Microsoft.EntityFrameworkCore;
namespace BestStoreMVC.Services
{
    public class AplicationDbContext : DbContext
    {
        public AplicationDbContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<Product> Products { get; set; }
    }
}
