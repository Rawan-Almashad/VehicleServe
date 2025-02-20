using System.ComponentModel.DataAnnotations;

namespace VehicleServe.Models
{
    public class Service
    {
        public int Id { get; set; }
        [MaxLength(100)]
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]  
        [StringLength(500)]  
        public string Description { get; set; } = string.Empty;
        public List<ProviderService> ProviderServices { get; set; }
        public List<ServiceRequest> ServiceRequests { get; set; }
    }
}
