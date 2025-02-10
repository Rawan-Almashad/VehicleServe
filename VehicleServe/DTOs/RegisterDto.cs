using System.ComponentModel.DataAnnotations;

namespace VehicleServe.DTOs
{
    public class RegisterDto
    {
     
        [Required]
        public string Username { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }
        = string.Empty;
        [Required]
        public string PhoneNumber { get; set; }
    }
}
