using System.ComponentModel.DataAnnotations;

namespace VehicleServe.DTOs
{
    public class AddReviewDto
    {
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
        public int Rating { get; set; } 

        [MaxLength(500, ErrorMessage = "Review must be at most 500 characters.")]
        public string Comment { get; set; }
    }
}
