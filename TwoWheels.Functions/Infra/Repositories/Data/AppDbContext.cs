using Microsoft.EntityFrameworkCore;
using TwoWheels.Functions.Domains.Entities;

namespace TwoWheels.Functions.Infra.Repositories.Data
{
    public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
    {
        public DbSet<Motorcycle> Motorcycles { get; set; }
        public DbSet<Deliverer> Deliverers { get; set; }
        public DbSet<Rental> Rentals { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Motorcycle>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.LicensePlate).IsRequired().HasMaxLength(10);
                entity.HasIndex(e => e.LicensePlate).IsUnique();
                entity.Property(e => e.Model).IsRequired().HasMaxLength(100);
            });

            modelBuilder.Entity<Deliverer>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Cnpj).IsRequired().HasMaxLength(14);
                entity.HasIndex(e => e.Cnpj).IsUnique();
                entity.Property(e => e.CnhNumber).IsRequired().HasMaxLength(20);
                entity.HasIndex(e => e.CnhNumber).IsUnique();
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.CnhType).HasConversion<int>();
            });

            modelBuilder.Entity<Rental>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.TotalAmount).HasPrecision(18, 2);

                entity.OwnsOne(e => e.Plan, plan =>
                {
                    plan.Property(p => p.Days).IsRequired();
                    plan.Property(p => p.DailyRate).HasPrecision(18, 2);
                    plan.Property(p => p.EarlyReturnPenaltyPercentage).HasPrecision(5, 2);
                    plan.Property(p => p.LateReturnDailyFee).HasPrecision(18, 2);
                });
                entity.HasOne(e => e.Deliverer)
                      .WithMany()
                      .HasForeignKey(e => e.DelivererId)
                      .OnDelete(DeleteBehavior.Restrict);

                entity.HasOne(e => e.Motorcycle)
                      .WithMany()
                      .HasForeignKey(e => e.MotorcycleId)
                      .OnDelete(DeleteBehavior.Restrict);
            });

            base.OnModelCreating(modelBuilder);
        }
    }
}
