using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using VehicleServe.Models;
namespace VehicleServe.Data
{
    public class AppDbContext: IdentityDbContext<IdentityUser>
    {
        public DbSet<Customer> Customers { get; set; } 
        public DbSet<Service> Services { get; set; }
        
        public DbSet<Provider> Providers { get; set; }
        public DbSet<ServiceRequest> ServiceRequests { get; set; }
        public DbSet<Vehicle>Vehicles { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Customer>()
            .HasOne(c => c.User)
            .WithOne()
            .HasForeignKey<Customer>(c => c.UserId) // 1-to-1 Relationship
            .IsRequired().OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Provider>()
            .HasOne(c => c.User)
            .WithOne()
            .HasForeignKey<Provider>(c => c.UserId) 
            .IsRequired().OnDelete(DeleteBehavior.Cascade);


            modelBuilder.Entity<Provider>().HasOne(x=>x.Service)
                .WithMany(x=>x.Providers).HasForeignKey(x=>x.ServiceId);

            modelBuilder.Entity<Customer>().HasMany(x => x.Vehicles)
                .WithOne(x => x.Customer).HasForeignKey(x => x.CustomerId);

            modelBuilder.Entity<Customer>().HasMany(x=>x.ServiceRequests)
                .WithOne(x=>x.Customer).HasForeignKey(x=>x.CustomerId);

            modelBuilder.Entity<Provider>().HasMany(x => x.ServiceRequests)
                .WithOne(x => x.Provider).HasForeignKey(x => x.ProviderId)
                .OnDelete(DeleteBehavior.NoAction); 

        }
    }
}
