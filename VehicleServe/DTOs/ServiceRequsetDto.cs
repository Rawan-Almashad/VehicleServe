using System.ComponentModel.DataAnnotations;
using VehicleServe.Models;

namespace VehicleServe.DTOs
{
    public class ServiceRequsetDto
    {
       
        public string Notes { get; set; } = string.Empty;
        [Required]
        public decimal Latitude { get; set; }
        [Required]
        public decimal Longitude { get; set; }
        [Required]
        public string ProviderId { get; set; }
        [Required]
        public int ServiceId {  get; set; }
        [Required]  
        public int VehicleId { get; set; }
    }
}
