using System.ComponentModel.DataAnnotations;

namespace VehicleServe.DTOs
{
    public class ProviderDto
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        = string.Empty;
        [Required]
        public string PhoneNumber { get; set; }
     
        [Required]
        public string NationalId { get; set; }
        [Required]
        public string Make { get; set; }
        [Required]
        public string LicensePlate { get; set; } = string.Empty;
        [Required]
        public string Model { get; set; }

    }
}
