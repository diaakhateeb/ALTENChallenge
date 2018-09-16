using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace DataDomainService.Models
{
    public partial class ALTENStockholmChallengeContext : DbContext
    {
        public ALTENStockholmChallengeContext()
        {
        }

        public ALTENStockholmChallengeContext(DbContextOptions<ALTENStockholmChallengeContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Customer> Customer { get; set; }
        public virtual DbSet<Vehicle> Vehicle { get; set; }
        public virtual DbSet<VehicleStatusTrans> VehicleStatusTrans { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (optionsBuilder.IsConfigured) return;

            //TODO: change when deploy.
            var configuration = new ConfigurationBuilder().
                SetBasePath(Directory.GetParent(@"./").FullName)
                .AddJsonFile("appsettings.json")
                .Build();

            optionsBuilder.UseSqlServer(configuration.GetConnectionString("ALTENChallengeDatabase"));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>(entity =>
            {
                entity.Property(e => e.Name).HasMaxLength(250);
            });

            modelBuilder.Entity<Vehicle>(entity =>
            {
                entity.Property(e => e.Code).HasMaxLength(100);

                entity.Property(e => e.RegNumber).HasMaxLength(50);

                entity.HasOne(d => d.Customer)
                    .WithMany(p => p.Vehicle)
                    .HasForeignKey(d => d.CustomerId)
                    .HasConstraintName("FK_Vehicle_Customer");
            });

            modelBuilder.Entity<VehicleStatusTrans>(entity =>
            {
                entity.Property(e => e.PingTime).HasColumnType("datetime");

                entity.Property(e => e.Status).HasMaxLength(3);

                entity.HasOne(d => d.Vehicle)
                    .WithMany(p => p.VehicleStatusTrans)
                    .HasForeignKey(d => d.VehicleId)
                    .HasConstraintName("FK_VehStatusTrans_Vehicle");
            });
        }
    }
}
