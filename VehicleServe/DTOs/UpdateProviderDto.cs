using System.ComponentModel.DataAnnotations;

namespace VehicleServe.DTOs
{
    public class UpdateProviderDto
    {
        [Required]
        public string Make { get; set; }
        [Required]
        public string LicensePlate { get; set; } = string.Empty;
        [Required]
        public string Model { get; set; }
    }
}
