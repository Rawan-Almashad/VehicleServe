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
        public DbSet<ProviderService> ProviderServices { get; set; }
        public DbSet<Provider> Providers { get; set; }
        public DbSet<ServiceRequest> ServiceRequests { get; set; }
        public DbSet<Vehicle>Vehicles { get; set; }
        public DbSet<Review> Reviews { get; set; }  

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Customer>()
               .HasOne(c => c.User) 
               .WithOne()           
               .HasForeignKey<Customer>(c => c.Id) 
               .IsRequired()
               .OnDelete(DeleteBehavior.Restrict);

            // Configure Provider-IdentityUser Relationship
            modelBuilder.Entity<Provider>()
                .HasOne(p => p.User) 
                .WithOne()           
                .HasForeignKey<Provider>(p => p.Id) 
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Review>()
                .HasOne(r => r.ServiceRequest) 
                .WithOne() 
                .HasForeignKey<Review>(r => r.ServiceRequestId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ProviderService>()
                   .HasKey(ps => new { ps.ProviderId, ps.ServiceId }); // Composite primary key

            modelBuilder.Entity<ProviderService>()
                .HasOne(ps => ps.Provider)
                .WithMany(p => p.ProviderServices)
                .HasForeignKey(ps => ps.ProviderId);

            modelBuilder.Entity<ProviderService>()
                .HasOne(ps => ps.Service)
                .WithMany(s => s.ProviderServices)
                .HasForeignKey(ps => ps.ServiceId);



            modelBuilder.Entity<Customer>().HasMany(x => x.Vehicles)
                .WithOne(x => x.Customer).HasForeignKey(x => x.CustomerId);

            modelBuilder.Entity<Customer>().HasMany(x=>x.ServiceRequests)
                .WithOne(x=>x.Customer).HasForeignKey(x=>x.CustomerId);

            modelBuilder.Entity<Provider>().HasMany(x => x.ServiceRequests)
                .WithOne(x => x.Provider).HasForeignKey(x => x.ProviderId)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder.Entity<Review>()
      .HasOne(r => r.Customer)
      .WithMany(c => c.Reviews)
      .HasForeignKey(r => r.CustomerId)
      .OnDelete(DeleteBehavior.Restrict); 

            
            modelBuilder.Entity<Review>()
                .HasOne(r => r.Provider)
                .WithMany(p => p.Reviews)
                .HasForeignKey(r => r.ProviderId)
                .OnDelete(DeleteBehavior.Restrict);

        }
    }
}
