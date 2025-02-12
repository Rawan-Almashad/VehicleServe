using System.ComponentModel.DataAnnotations;

namespace VehicleServe.DTOs
{
    public class GetServiceDto
    {
        public int Id { get; set; }
        [MaxLength(100)]
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;
    }
}
