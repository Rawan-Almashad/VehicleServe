using System.ComponentModel.DataAnnotations;

namespace VehicleServe.DTOs
{
    public class GetProviderDto
    {
        public string Username { get; set; }
       
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public int ServiceId { get; set; }
        public string ServiceName { get; set; }
    }
}
