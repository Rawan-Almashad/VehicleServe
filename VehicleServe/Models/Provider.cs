using System.ComponentModel.DataAnnotations;

namespace VehicleServe.Models
{
    public class Provider
    {
        [Key]
        public string UserId { get; set; }
        bool IsAvailable { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public int  ServiceId { get; set; }
        public Service Service  { get; set; }
        public User User { get; set; }
        public List<ServiceRequest> ServiceRequests { get; set; } = new List<ServiceRequest>();
    }
}
