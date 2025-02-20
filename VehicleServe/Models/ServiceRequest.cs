using Microsoft.Identity.Client;
using System.ComponentModel.DataAnnotations.Schema;

namespace VehicleServe.Models
{
    public class ServiceRequest
    {
        public int Id {  get; set; }
        public string Status { get; set; } = string.Empty;
      
        public string Notes { get; set; } = string.Empty;
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public DateTime DateRequested { get; set; }
        public string CustomerId {  get; set; }
        public Customer Customer { get; set; }
        public Provider Provider { get; set; }  
        public string ProviderId {  get; set; }  
        public Review? Review { get; set; }     
        public int ServiceId {  get; set; }
        public Service Service { get; set; }    

    }
}
