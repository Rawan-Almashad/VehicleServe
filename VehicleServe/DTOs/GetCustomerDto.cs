using System.ComponentModel.DataAnnotations;

namespace VehicleServe.DTOs
{
    public class GetCustomerDto
    {
        public string Username { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
       
    }
}
