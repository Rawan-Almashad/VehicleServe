using System.ComponentModel.DataAnnotations;
using VehicleServe.Models;

namespace VehicleServe.DTOs
{
    public class GetVehivleDto
    {
        public int Id { get; set; } 
        [MaxLength(200)]
        public string Model { get; set; }
        public int Year { get; set; }
        [MaxLength(100)]
        public string Make { get; set; }

        [Required]
        public string LicensePlate { get; set; } = string.Empty;
       
    }
}
