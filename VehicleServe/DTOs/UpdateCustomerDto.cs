﻿using System.ComponentModel.DataAnnotations;

namespace VehicleServe.DTOs
{
    public class UpdateCustomerDto
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Email { get; set; }

        [Required]
        public string PhoneNumber { get; set; }
    }
}
