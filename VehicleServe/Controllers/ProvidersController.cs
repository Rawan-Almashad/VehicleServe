using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using VehicleServe.Data;
using VehicleServe.DTOs;
using VehicleServe.Models;
using VehicleServe.Services;

namespace VehicleServe.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProvidersController : ControllerBase
    {
        private readonly AppDbContext _appDbContext;
        private readonly UserManager<IdentityUser> _userManager;
        public ProvidersController(AppDbContext appDbContext, UserManager<IdentityUser> userManager)
        {
            _appDbContext = appDbContext;
            _userManager = userManager;
        }
        [HttpGet("me")]
        [Authorize(Roles ="PROVIDER")]
        public async Task<IActionResult> GetProviderProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var provider = await _appDbContext.Providers
                .Include(p => p.User)
                .Where(p => p.Id == userId)
                .Select(p => new
                {
                    p.Id,
                    p.User.UserName,
                    p.User.Email,
                    p.NationalId,
                    p.Rating,   
                    p.LicensePlate,
                    p.Make,
                    p.Model,
                    p.IsAvailable
                })
                .SingleOrDefaultAsync();

            if (provider == null)
                return NotFound("Provider profile not found.");

            return Ok(provider);
        }
        [Authorize(Roles = "PROVIDER")]
        [HttpGet("get-my-services")]
        public async Task<IActionResult> GetProviderServices()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier); // Get the current user's Id

            var provider = await _appDbContext.Providers
                .Include(p => p.ProviderServices)
                .ThenInclude(ps => ps.Service)
                .FirstOrDefaultAsync(p => p.Id == userId);

            if (provider == null)
            {
                return NotFound("Provider not found.");
            }
            var serviceDtos = provider.ProviderServices
                .Select(ps => new GetServiceDto
                {
                    Id = ps.Service.Id,
                    Name = ps.Service.Name,
                    Description= ps.Service.Description
                })
                .ToList();

            return Ok(serviceDtos);
        }


        [HttpGet("{id}")]
        public async Task<IActionResult> GetProviderById(string id)
        {
            var provider = await _appDbContext.Providers
            .Include(p => p.User)
            .Include(p => p.ProviderServices)
            .Where(p => p.Id == id) 
            .Select(p => new GetProviderDto
            {
            Username = p.User.UserName,
            Email = p.User.Email,
            PhoneNumber = p.User.PhoneNumber,
              providerServices = p.ProviderServices ,
                NationalId=p.NationalId,
                Rating=p.Rating,
                LicensePlate= p.LicensePlate,
                Make= p.Make,
                Model= p.Model,
                IsAvailable=p.IsAvailable
            })
            .FirstOrDefaultAsync();
            if (provider == null)
            {
                return NotFound(new { message = "Provider not found" });
            }
            return Ok(provider);
        }
        [HttpPost]
        [Route("AddProvider")]
        public async Task<IActionResult> AddProvider([FromBody] ProviderDto model)
        {
            
            IdentityUser user = new()
            {
                Email = model.Email,
                UserName = model.Username,
                SecurityStamp = Guid.NewGuid().ToString(),
                PhoneNumber = model.PhoneNumber,
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
            {
                return BadRequest(new { Status = "Error", Message = "User creation failed!", Errors = result.Errors.Select(e => e.Description) });
            }

            // Assign "Provider" role
            var roleResult = await _userManager.AddToRoleAsync(user, "Provider");
            if (!roleResult.Succeeded)
            {
                return BadRequest(new { Status = "Error", Message = "Failed to assign role.", Errors = roleResult.Errors.Select(e => e.Description) });
            }
            Provider provider = new()
            {
                Id = user.Id,
                User = user,
                LicensePlate = model.LicensePlate,
                Make = model.Make,
                NationalId = model.NationalId,
                Model=model.Model
            };

            _appDbContext.Providers.Add(provider);
            await _appDbContext.SaveChangesAsync();

            return Ok(new { Status = "Success", Message = "Provider created successfully!", ProviderId = provider.Id });
        }
        [Authorize(Roles ="PROVIDER")]
        [HttpPost("add-service")]
        public async Task<IActionResult> AddServiceToCurrentProvider(int serviceId)
        {
            
            var providerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            

            if (providerId == null)
            {
                return Unauthorized("Provider not found.");
            }

            
            var serviceExists = await _appDbContext.Services.AnyAsync(s => s.Id == serviceId);
            if (!serviceExists)
            {
                return NotFound("Service not found.");
            }

            
            var existingProviderService = await _appDbContext.ProviderServices
                .FirstOrDefaultAsync(ps => ps.ProviderId == providerId && ps.ServiceId == serviceId);
            if (existingProviderService != null)
            {
                return BadRequest("You already offers this service.");
            }

            
            var providerService = new ProviderService
            {
                ProviderId = providerId,
                ServiceId = serviceId
            };

            _appDbContext.ProviderServices.Add(providerService);
            await _appDbContext.SaveChangesAsync();

            return Ok(new { Message = "Service added successfully." });
        }
        [Authorize(Roles = "PROVIDER")]
        [HttpPut("update-profile")]
        public async Task<IActionResult> UpdateProviderProfile([FromBody] UpdateProviderDto model)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var provider = await _appDbContext.Providers
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.Id == userId);

            if (provider == null)
            {
                return NotFound("Provider not found.");
            }

            provider.LicensePlate = model.LicensePlate;
            provider.Make = model.Make;
            provider.Model = model.Model;

            await _appDbContext.SaveChangesAsync();

            return Ok(new { Message = "Profile updated successfully." });
        }
        [Authorize(Roles = "PROVIDER")]
        [HttpPut("update-availability")]
        public async Task<IActionResult> UpdateAvailability(bool IsAvailable)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var provider = await _appDbContext.Providers
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.Id == userId);

            if (provider == null)
            {
                return NotFound("Provider not found.");
            }
            provider.IsAvailable = IsAvailable; 

            await _appDbContext.SaveChangesAsync();

            return Ok(new { Message = "Availability updated successfully." });
        }
        [Authorize(Roles = "PROVIDER")]
        [HttpPut("update-location")]
        public async Task<IActionResult> UpdateLocation(LocatioDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var provider = await _appDbContext.Providers
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.Id == userId);

            if (provider == null)
            {
                return NotFound("Provider not found.");
            }
            provider.Latitude = dto.Latitude;
            provider.Longitude = dto.Longitude;

            await _appDbContext.SaveChangesAsync();

            return Ok(new { Message = "Availability updated successfully." });
        }
        [Authorize(Roles = "PROVIDER")]
        [HttpDelete("remove-service/{serviceId}")]
        public async Task<IActionResult> RemoveServiceFromProvider(int serviceId)
        {
            var providerId = User.FindFirstValue(ClaimTypes.NameIdentifier);

            var providerService = await _appDbContext.ProviderServices
                .FirstOrDefaultAsync(ps => ps.ProviderId == providerId && ps.ServiceId == serviceId);

            if (providerService == null)
            {
                return NotFound("This service is not associated with the provider.");
            }

            _appDbContext.ProviderServices.Remove(providerService);
            await _appDbContext.SaveChangesAsync();

            return Ok(new { Message = "Service removed successfully." });
        }

        [HttpGet("available-providers/{serviceId}")]
        [Authorize(Roles = "CUSTOMER")]
        public async Task<IActionResult> GetAvailableProviders(int serviceId)
        {
            var onlineProviderIds = OnlineUsersTracker.GetOnlineProviders();

            var availableProviders = await _appDbContext.Providers
                .Where(p => p.IsAvailable && onlineProviderIds.Contains(p.Id) &&
                            p.ProviderServices.Any(ps => ps.ServiceId == serviceId)) 
                .Include(p => p.User)
                .OrderByDescending(p => p.Rating) 
                .Select(p => new
                {
                    ProviderId = p.Id,
                    Username = p.User.UserName,
                    Rating = p.Rating,
                    Latitude = p.Latitude,
                    Longitude = p.Longitude
                })
                .ToListAsync();

            return Ok(availableProviders);
        }


    }


}

