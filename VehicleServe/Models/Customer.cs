namespace VehicleServe.Models
{
    public class Customer
    {
        public int Id { get; set; }

        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public int UserId {  get; set; }
        public User User { get; set; }  
        public List<Vehicle> Vehicles { get; set; } = new List<Vehicle>();
        public List<ServiceRequest> ServiceRequests { get; set;} = new List<ServiceRequest>();

    }
}
