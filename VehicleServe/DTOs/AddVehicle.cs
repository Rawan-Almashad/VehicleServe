using System.ComponentModel.DataAnnotations;

namespace VehicleServe.DTOs
{
    public class AddVehicle
    {
        [Required]
        [MaxLength(200)]
        public string Model { get; set; }
        [Required]
        public int Year { get; set; }
        [MaxLength(100)]
        [Required]
        public string Make { get; set; }

        [Required]
        public string LicensePlate { get; set; } = string.Empty;
    }
}
