using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace VehicleServe.Models
{
    public class User : IdentityUser
    {
        
        
        public Customer? Customer { get; set; }  
        public Provider? Provider { get; set; } 
    }
}
