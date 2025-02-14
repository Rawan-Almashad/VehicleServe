﻿using System.ComponentModel.DataAnnotations;

namespace VehicleServe.DTOs
{
    public class UpdateProviderDto
    {
        [Required]
        public string Username { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        [Required]
        public int ServiceId {  get; set; }
        [Required]
        public decimal Latitude { get; set; }
        [Required]
        public decimal Longitude { get; set; }
    }
}
