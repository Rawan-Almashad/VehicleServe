using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace VehicleServe.Models
{
    public class Provider
    {
        [Key]
        public string Id { get; set; }
        public bool IsAvailable { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        [Required]
        public string NationalId {  get; set;}
        [Required]
        public string Make { get; set; }
        [Required]
        public string LicensePlate { get; set; } = string.Empty;

        public double Rating => Reviews.Any() ? Reviews.Average(r => r.Rating) : 0;
        [Required]
        public string Model { get; set; }
        public IdentityUser User { get; set; }
        public List<ServiceRequest> ServiceRequests { get; set; } = new List<ServiceRequest>();
        public List<Review>Reviews { get; set; }
        public List<ProviderService> ProviderServices { get; set; }

    }
}
