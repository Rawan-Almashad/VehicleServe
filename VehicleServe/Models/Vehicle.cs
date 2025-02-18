using System.ComponentModel.DataAnnotations;

namespace VehicleServe.Models
{
    public class Vehicle
    {
        public int Id { get; set; }
        [MaxLength(200)]
        public string Model { get; set; }   
        public int Year {  get; set; }
        [MaxLength(100)]
        public string Make { get; set; }
        [Required]
        public string LicensePlate { get; set; } = string.Empty;    
        public string CustomerId { get; set; }
        public Customer Customer { get; set; }
    }
}
