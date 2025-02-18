using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using VehicleServe.Data;
using VehicleServe.DTOs;
using VehicleServe.Models;

namespace VehicleServe.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class VehiclesController : ControllerBase
    {
        private readonly AppDbContext _appDbContext;
        private readonly UserManager<IdentityUser> _userManager;
        public VehiclesController(AppDbContext appDbContext, UserManager<IdentityUser> userManager)
        {
            _appDbContext = appDbContext;
            _userManager = userManager;
        }
        [HttpGet]
        public async Task<IActionResult> GetCustomerVehicles()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var vehicles = await _appDbContext.Vehicles.Where(v=>v.Customer.Id == userId)
                .Select(v => new GetVehivleDto
            {
                Id = v.Id,
                LicensePlate = v.LicensePlate,
                Make = v.Make,
                Year = v.Year,
                Model = v.Model
            }).ToListAsync();

            return Ok(vehicles);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetVehicleById(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var vehicleDto = await _appDbContext.Vehicles
        .Where(v => v.Id == id && v.Customer.Id == userId)
        .Select(v => new GetVehivleDto
        {
            Id=v.Id,
            LicensePlate = v.LicensePlate,
            Make = v.Make,
            Year = v.Year,
            Model = v.Model
        })
        .SingleOrDefaultAsync();

            if (vehicleDto == null)
                return NotFound("Vehicle not found or you don't have access.");
            return Ok(vehicleDto);
        }
        [HttpPost]
        public async Task<IActionResult> AddVehicle([FromBody] AddVehicle vehicle)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var customer=await _appDbContext.Customers.Where(x=>x.Id==userId).SingleOrDefaultAsync();
            if (customer == null)
                return Unauthorized();
            var newvehicle = new Vehicle
            {
                CustomerId = customer.Id,
                LicensePlate = vehicle.LicensePlate,
                Make = vehicle.Make,
                Year = vehicle.Year,
                Model = vehicle.Model,
                Customer = customer

            };
           _appDbContext.Vehicles.Add(newvehicle);
            await _appDbContext.SaveChangesAsync();
            return CreatedAtAction(nameof(GetVehicleById), new { id = newvehicle.Id });
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateVehicle(int id, [FromBody] AddVehicle updatedVehicle)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var vehicle = await _appDbContext.Vehicles
    .FirstOrDefaultAsync(v => v.Id == id && v.Customer.Id == userId);

            if (vehicle == null)
                return NotFound("Vehicle not found or you don't have access.");

            // Update fields
            vehicle.Make = updatedVehicle.Make;
            vehicle.Model = updatedVehicle.Model;
            vehicle.Year = updatedVehicle.Year;
            vehicle.LicensePlate = updatedVehicle.LicensePlate;

            await _appDbContext.SaveChangesAsync();
            return NoContent();
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteVehicle(int id)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var vehicle = await _appDbContext.Vehicles
    .FirstOrDefaultAsync(v => v.Id == id && v.Customer.Id == userId);

            if (vehicle == null)
                return NotFound("Vehicle not found or you don't have access.");

            _appDbContext.Vehicles.Remove(vehicle);
            await _appDbContext.SaveChangesAsync();
            return NoContent();
        }
        [HttpGet("customer/{customerId}")]
        [Authorize(Roles = "Admin")] 
        public async Task<IActionResult> GetCustomerVehicles(int customerId)
        {
            var vehicles = await _appDbContext.Vehicles
                .Where(v => v.Id == customerId)
                .Select(v => new GetVehivleDto
                {
                    Id = v.Id,
                    LicensePlate = v.LicensePlate,
                    Make = v.Make,
                    Year = v.Year,
                    Model = v.Model
                })
                .ToListAsync();

            if (!vehicles.Any())
                return NotFound("No vehicles found for this customer.");

            return Ok(vehicles);
        }



    }
}
