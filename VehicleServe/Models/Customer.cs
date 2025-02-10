using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
namespace VehicleServe.Models
{
    public class Customer
    {
        [Key]
        public int Id { get; set; }
        public string UserId { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        
        public IdentityUser User { get; set; } 
        public List<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
        public List<ServiceRequest> ServiceRequests { get; set;} = new List<ServiceRequest>();

    }
}
