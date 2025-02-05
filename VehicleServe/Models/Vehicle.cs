using System.ComponentModel.DataAnnotations;

namespace VehicleServe.Models
{
    public class Vehicle
    {
        public int Id { get; set; }
        public string Model { get; set; }   
        public int Year {  get; set; }  
        public int Make { get; set; }
        [Required]
        public string LicensePlate { get; set; } = string.Empty;    
        public int CustomerId { get; set; }
        public Customer Customer { get; set; }
    }
}
