using Microsoft.EntityFrameworkCore;
using UpriseMidLevel.Models;
using UpriseMidTask.Models;

namespace UpriseMidTask.Data
{
    public class AppDbContext : DbContext
    {

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; } 
        public DbSet<SolarPlant> SolarPlants { get; set; }
        public DbSet<ProductionData> ProductionData { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<SolarPlant>()
                .Property(p => p.PowerInstalled)
                .HasPrecision(18, 2); // 18 digits total, 2 decimal places

            modelBuilder.Entity<ProductionData>()
                .Property(p => p.Production)
                .HasPrecision(18, 2); // 18 digits total, 2 decimal places

            // Configure the one-to-many relationship explicitly
            modelBuilder.Entity<ProductionData>()
                .HasOne(p => p.SolarPlant)          // One ProductionData has one SolarPowerPlant
                .WithMany(s => s.ProductionData)        // One SolarPowerPlant has many ProductionData
                .HasForeignKey(p => p.SolarPlantId); // ProductionData has a foreign key to SolarPowerPlantId
        }

    }

}
