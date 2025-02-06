using System.ComponentModel.DataAnnotations;

namespace VehicleServe.DTOs
{
    public class RoleDto
    {
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
    }
}
