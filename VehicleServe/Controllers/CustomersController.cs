using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using VehicleServe.Data;
using VehicleServe.DTOs;

namespace VehicleServe.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CustomersController : ControllerBase
    {
        private readonly AppDbContext _appDbContext;
        private readonly UserManager<IdentityUser> _userManager;
        public CustomersController(AppDbContext appDbContext, UserManager<IdentityUser> userManager)
        {
            _appDbContext = appDbContext;
            _userManager = userManager;
        }
        [HttpGet("me")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> GetCustomerProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var customer = await _appDbContext.Customers
                .Include(c => c.User)
                .Where(c => c.Id == userId)
                .Select(c => new GetCustomerDto
                {
                    Username = c.User.UserName,
                    Email = c.User.Email,
                    PhoneNumber = c.User.PhoneNumber,
                    Latitude = c.Latitude,
                    Longitude = c.Longitude
                })
                .FirstOrDefaultAsync();

            if (customer == null)
                return NotFound(new { message = "Customer profile not found." });

            return Ok(customer);
        }
        [HttpGet("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetCustomerById(string id)
        {

            var customer = await _appDbContext.Customers
                .Include(c => c.User)
                .Where(c => c.Id == id)
                .Select(c => new GetCustomerDto
                {
                    Username = c.User.UserName,
                    Email = c.User.Email,
                    PhoneNumber = c.User.PhoneNumber,
                    Latitude = c.Latitude,
                    Longitude = c.Longitude
                })
                .FirstOrDefaultAsync();

            if (customer == null)
                return NotFound(new { message = "Customer profile not found." });

            return Ok(customer);
        }
        [HttpPut("me/location")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> UpdateLocation([FromBody] LocatioDto updateDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var customer = await _appDbContext.Customers.SingleOrDefaultAsync(c => c.Id == userId);

            if (customer == null)
                return NotFound("Customer not found.");

            customer.Latitude = updateDto.Latitude;
            customer.Longitude = updateDto.Longitude;

            await _appDbContext.SaveChangesAsync();
            return NoContent();
        }
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetAllCustomers()
        {
            var customers = await _appDbContext.Customers.
                Include(x=>x.User)
                .Select(c => new GetCustomerDto
                {
                    Username = c.User.UserName,
                    Email = c.User.Email,
                    PhoneNumber = c.User.PhoneNumber,
                    Latitude = c.Latitude,
                    Longitude = c.Longitude
                })
                .ToListAsync();

            return Ok(customers);
        }
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteCustomer(string id)
        {
            // Find the customer
            var customer = await _appDbContext.Customers
                .Include(c => c.ServiceRequests)
                .Include(c => c.Vehicles)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (customer == null)
            {
                return NotFound(new { message = "Customer not found." });
            }

           
            if (customer.ServiceRequests.Any() || customer.Vehicles.Any())
            {
                return BadRequest(new { message = "Customer cannot be deleted because they have related data (service requests or vehicles)." });
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound(new { message = "User not found." });
            }

            // Remove customer from database
            _appDbContext.Customers.Remove(customer);
            await _appDbContext.SaveChangesAsync(); 

            // Delete the user from ASP.NET Identity
            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                return BadRequest(new { message = "Failed to delete user." });
            }

            return NoContent(); 
        }
        [HttpPut("me")]
        [Authorize(Roles = "Customer")]
        public async Task<IActionResult> UpdateCustomerProfile([FromBody] UpdateCustomerDto model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var customer = await _appDbContext.Customers
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.Id == userId);

            if (customer == null)
                return NotFound(new { message = "Customer profile not found." });

            
            if (customer.User != null)
            {
                customer.User.UserName = model.Username;
                customer.User.Email = model.Email;
                customer.User.PhoneNumber = model.PhoneNumber;

                var identityResult = await _userManager.UpdateAsync(customer.User);
                if (!identityResult.Succeeded)
                {
                    return BadRequest(new
                    {
                        message = "Failed to update user details.",
                        errors = identityResult.Errors.Select(e => e.Description)
                    });
                }
            }
            await _appDbContext.SaveChangesAsync();
            return Ok(new { message = "Profile updated successfully", customer });
        }





    }
}
