using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace VehicleServe.Models
{
    public class Review
    {
        public int Id { get; set; }
        public string ProviderId { get; set; }
        public Provider Provider { get; set; }
        public int ServiceRequestId { get; set; }
        public ServiceRequest ServiceRequest { get; set; }
        public string CustomerId { get; set; }
        public Customer Customer { get; set; }
       
        [Range(1, 5)]
        public int Rating { get; set; }

        public string? Comment { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
