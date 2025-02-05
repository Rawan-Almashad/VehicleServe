namespace VehicleServe.Models
{
    public class Provider
    {
        public int Id { get; set; }
        bool IsAvailable { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public int  ServiceId { get; set; }
        public Service Service  { get; set; }
        public int UserId {  get; set; }
        public User User { get; set; }
        public List<ServiceRequest> ServiceRequests { get; set; } = new List<ServiceRequest>();
    }
}
