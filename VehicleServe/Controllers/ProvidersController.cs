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
        [Authorize(Roles = "Provider")]
        public async Task<IActionResult> GetProviderProfile()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var provider = await _appDbContext.Providers
                .Include(p => p.User)
                .Where(p => p.UserId == userId)
                .Select(p => new
                {
                    p.User.UserName,
                    p.User.Email,
                    p.Latitude,
                    p.Longitude,
                    p.ServiceId
                })
                .SingleOrDefaultAsync();

            if (provider == null)
                return NotFound("Provider profile not found.");

            return Ok(provider);
        }

        [HttpGet]
        public async Task<IActionResult>GetProviders()
        {
            var providers = await _appDbContext.Providers
          .Include(p => p.User)
          .Include(p => p.Service)
          .Select(p => new GetProviderDto
          {
              Username = p.User.UserName,
              Email = p.User.Email,
              PhoneNumber = p.User.PhoneNumber,
              ServiceId = p.ServiceId,
              ServiceName=p.Service.Name

          })
          .ToListAsync();
            return Ok(providers);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProviderById(int id)
        {
            var provider = await _appDbContext.Providers
            .Include(p => p.User)
            .Include(p => p.Service)
            .Where(p => p.Id == id) 
            .Select(p => new GetProviderDto
            {
            Username = p.User.UserName,
            Email = p.User.Email,
            PhoneNumber = p.User.PhoneNumber,
            ServiceId = p.ServiceId,
            ServiceName = p.Service.Name
           })
            .FirstOrDefaultAsync();
            if (provider == null)
            {
                return NotFound(new { message = "Provider not found" });
            }
            return Ok(provider);
        }
        [HttpPost]
        public async Task<IActionResult> AddProvider([FromBody] ProviderDto model)
        {
            // Validate if ServiceId exists first
            var serviceExists = await _appDbContext.Services.AnyAsync(s => s.Id == model.ServiceId);
            if (!serviceExists)
            {
                return BadRequest(new { message = "ServiceId doesn't exist" });
            }

            
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
                UserId = user.Id,
                User = user,
                ServiceId = model.ServiceId,
                IsAvailable = true,
                Longitude = model.Longitude,
                Latitude = model.Latitude
            };

            _appDbContext.Providers.Add(provider);
            await _appDbContext.SaveChangesAsync();

            return Ok(new { Status = "Success", Message = "Provider created successfully!", ProviderId = provider.Id });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProvider(int id, [FromBody] UpdateProviderDto model)
        {
            var provider = await _appDbContext.Providers.FindAsync(id);
            if (provider == null)
            {
                return NotFound(new { message = "Provider not found." });
            }

            var serviceExists = await _appDbContext.Services.AnyAsync(s => s.Id == model.ServiceId);
            if (!serviceExists)
            {
                return BadRequest(new { message = "ServiceId doesn't exist" });
            }

            provider.ServiceId = model.ServiceId;
            provider.Latitude = model.Latitude;
            provider.Longitude = model.Longitude;

            var user = await _userManager.FindByIdAsync(provider.UserId);
            if (user != null)
            {
                user.PhoneNumber = model.PhoneNumber;
                user.UserName=model.Username;
                user.Email = model.Email;   
                var identityUpdateResult = await _userManager.UpdateAsync(user);

                if (!identityUpdateResult.Succeeded)
                {
                    return BadRequest(new { message = "Failed to update user details.", Errors = identityUpdateResult.Errors });
                }
            }

            await _appDbContext.SaveChangesAsync();
            return Ok(new { message = "Provider updated successfully", provider });
        }
        [HttpPut("me/location")]
        [Authorize(Roles = "Provider")]
        public async Task<IActionResult> UpdateLocation([FromBody] LocatioDto updateDto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var provider = await _appDbContext.Providers.SingleOrDefaultAsync(c => c.UserId == userId);

            if (provider == null)
                return NotFound("Provider not found.");

            provider.Latitude = updateDto.Latitude;
            provider.Longitude = updateDto.Longitude;

            await _appDbContext.SaveChangesAsync();
            return NoContent();
        }
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteProvider(int id)
        {
            var provider = await _appDbContext.Providers
                .Include(p => p.User) 
                .FirstOrDefaultAsync(p => p.Id == id);

            if (provider == null)
                return NotFound("Provider not found.");

            
            if (provider.User != null)
            {
                var identityResult = await _userManager.DeleteAsync(provider.User);
                if (!identityResult.Succeeded)
                {
                    return BadRequest(new
                    {
                        message = "Failed to delete associated user.",
                        errors = identityResult.Errors.Select(e => e.Description)
                    });
                }
            }

            // Remove provider only if user deletion was successful
            _appDbContext.Providers.Remove(provider);
            await _appDbContext.SaveChangesAsync();

            return NoContent();
        }




    }
}
