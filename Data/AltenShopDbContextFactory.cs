using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace AltenShopApi.Data
{
    public class AltenShopDbContextFactory : IDesignTimeDbContextFactory<AltenShopDbContext>
    {
        public AltenShopDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AltenShopDbContext>();
            optionsBuilder.UseSqlServer("Server=localhost;Database=AltenShop;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True");

            return new AltenShopDbContext(optionsBuilder.Options);
        }
    }
}
