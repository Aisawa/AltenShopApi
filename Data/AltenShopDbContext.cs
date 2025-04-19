using AltenShopApi.Models;
using AltenShopApi.Models.Database;
using Microsoft.EntityFrameworkCore;

namespace AltenShopApi.Data
{
    public class AltenShopDbContext : DbContext
    {
        public AltenShopDbContext(DbContextOptions<AltenShopDbContext> options)
            : base(options)
        {
        }

        public DbSet<DbProduct> Products { get; set; }
        public DbSet<User> Users { get; set; }
    }
}
