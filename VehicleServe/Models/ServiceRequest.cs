using Microsoft.Identity.Client;

namespace VehicleServe.Models
{
    public class ServiceRequest
    {
        public int Id {  get; set; }
        public string Status { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public int CustomerId {  get; set; }
        public Customer Customer { get; set; }
        public Provider Provider { get; set; }  
        public int ProviderId {  get; set; }    
    }
}
