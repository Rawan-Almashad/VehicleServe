namespace VehicleServe.Models
{
    public class ProviderService
    {
        public string ProviderId { get; set; }   
        public int ServiceId { get; set; }        

        
        public Provider Provider { get; set; }
        public Service Service { get; set; }
    }
}
