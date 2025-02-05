namespace VehicleServe.Models
{
    public class Service
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public List<Provider>Providers{ get; set; } = new List<Provider>();
    }
}
