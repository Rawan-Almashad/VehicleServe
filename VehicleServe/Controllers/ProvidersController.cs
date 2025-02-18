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
                .Where(p => p.Id == userId)
                .Select(p => new
                {
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
       

       
       




    }
}
