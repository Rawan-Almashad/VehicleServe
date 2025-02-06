using Microsoft.EntityFrameworkCore;
using VehicleServe.Models;
namespace VehicleServe.Data
{
    public class AppDbContext: DbContext
    {
        public DbSet<User> Users { get; set; }  
        public DbSet<Customer> Customers { get; set; } 
        public DbSet<Service> Services { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Provider> Providers { get; set; }
        public DbSet<ServiceRequest> ServiceRequests { get; set; }
        public DbSet<Vehicle>Vehicles { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options)  : base(options)
        {

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>()
            .HasOne(x => x.User).WithOne(x => x.Customer).
            HasForeignKey<Customer>(x=>x.UserId);

            modelBuilder.Entity<Provider>()
              .HasOne(x => x.User).WithOne(x => x.Provider).
              HasForeignKey<Provider>(x => x.UserId);

            modelBuilder.Entity<User>().HasOne(x=>x.Role)
                .WithMany(x=>x.Users).HasForeignKey(x=>x.RoleId);

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
