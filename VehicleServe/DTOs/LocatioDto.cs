using System.ComponentModel.DataAnnotations;

namespace VehicleServe.DTOs
{
    public class LocatioDto
    {
        [Required]
        public decimal Latitude { get; set; }
        [Required]
        public decimal Longitude { get; set; }
    }
}
