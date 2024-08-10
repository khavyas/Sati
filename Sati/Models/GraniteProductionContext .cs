using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Sati.Models
{
    public class GraniteProductionContext : DbContext
    {
        public DbSet<Stone> Stones { get; set; }
        public DbSet<ProductionStep> ProductionSteps { get; set; }
        public DbSet<StoneProduction> StoneProductions { get; set; }

        public GraniteProductionContext(DbContextOptions<GraniteProductionContext> options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=KRTRIMAIQ-0018;Database=GraniteProduction;Trusted_Connection=True;",
                    sqlOptions => sqlOptions.CommandTimeout(180));  // Set the timeout to 180 seconds (or any value you need)
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Define primary key for ProductionStep
            modelBuilder.Entity<ProductionStep>()
                .HasKey(ps => ps.StepId);

            // Seed the ProductionSteps
            modelBuilder.Entity<ProductionStep>().HasData(
                new ProductionStep { StepId = 1, StepName = "Cutting" },
                new ProductionStep { StepId = 2, StepName = "Grinding" },
                new ProductionStep { StepId = 3, StepName = "Smoothening" },
                new ProductionStep { StepId = 4, StepName = "Cleaning" },
                new ProductionStep { StepId = 5, StepName = "Watering" },
                new ProductionStep { StepId = 6, StepName = "Polishing" },
                new ProductionStep { StepId = 7, StepName = "Epoxy" },
                new ProductionStep { StepId = 8, StepName = "Glittering" }
            );

            modelBuilder.Entity<StoneProduction>()
                .HasOne(sp => sp.Stone)
                .WithMany()
                .HasForeignKey(sp => sp.StoneId);

            modelBuilder.Entity<StoneProduction>()
                .HasOne(sp => sp.ProductionStep)
                .WithMany()
                .HasForeignKey(sp => sp.StepId);
        }


    }
}
