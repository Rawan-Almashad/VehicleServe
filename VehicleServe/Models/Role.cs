using System.ComponentModel.DataAnnotations;

namespace VehicleServe.Models
{
    public class Role
    {
        public int Id { get; set; }
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;    
        public List<User> Users { get; set; }
         
    }
}
