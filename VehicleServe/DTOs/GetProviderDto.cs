using System.ComponentModel.DataAnnotations;
using VehicleServe.Models;

namespace VehicleServe.DTOs
{
    public class GetProviderDto
    {
        public string Username { get; set; }
       
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        [Required]
        public string NationalId { get; set; }
        [Required]
        public string Make { get; set; }
        [Required]
        public string LicensePlate { get; set; } = string.Empty;
        public bool IsAvailable { get; set; }
        [Required]
        public string Model { get; set; }
        public double Rating { get; set; }
        public List<ProviderService>providerServices { get; set; }
    }
}
