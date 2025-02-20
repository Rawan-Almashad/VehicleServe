using System.ComponentModel.DataAnnotations;

namespace VehicleServe.DTOs
{
    public class StatusDto
    {
        [Required]
        public string status {  get; set; }

    }
}
