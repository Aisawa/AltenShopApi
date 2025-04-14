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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //// Configure one-to-many relationship between Series and Sets
            //modelBuilder.Entity<DbSet>()
            //    .HasOne(s => s.Series)
            //    .WithMany(s => s.Sets)
            //    .HasForeignKey(s => s.SeriesId)
            //    .OnDelete(DeleteBehavior.Cascade);

            //// Configure one-to-one relationship between Set and CardCount
            //modelBuilder.Entity<DbCardCount>()
            //    .HasOne(cc => cc.Set)
            //    .WithOne(s => s.CardCount)
            //    .HasForeignKey<DbCardCount>(cc => cc.SetId)
            //    .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
