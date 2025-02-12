using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using VehicleServe.Data;
using VehicleServe.DTOs;
using VehicleServe.Models;

namespace VehicleServe.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServicesController : ControllerBase
    {
        private readonly AppDbContext _appDbContext;
        public ServicesController(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }
        [HttpGet]
        public async Task<IActionResult>GetServices()
        {
            var services = await _appDbContext.Services
       .Select(s => new GetServiceDto { 
           Id = s.Id,
           Name = s.Name,
           Description = s.Description
       })
       .ToListAsync();
            if (services == null || services.Count == 0)
            {
                return NoContent();
            }
            return Ok(services);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetService(int id)
        {
            var service = await _appDbContext.Services.FindAsync(id);
            if (service == null)
            {
                return NotFound($"Service with ID {id} is not found.");
            }
            var serviceDto= new GetServiceDto { Id =service.Id,Name=service.Name,Description=service.Description};   
            return Ok(serviceDto);
        }
        [HttpPost]
        public async Task<IActionResult> CreateService([FromBody] ServiceDto serviceDto)
        {
            if (!ModelState.IsValid) 
            {
                return BadRequest(ModelState); 
            }
            var service = new Service { Name=serviceDto.Name,
                Description=serviceDto.Description};

            _appDbContext.Services.Add(service);

            await _appDbContext.SaveChangesAsync();
            return CreatedAtAction(nameof(GetService), new { id = service.Id }, service);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateServicce(int id, [FromBody] ServiceDto serviceDto)
        {
            var service = await _appDbContext.Services.SingleOrDefaultAsync(x => x.Id == id);
            if (service == null)
            {
                return NotFound($"Service with ID {id} is not found.");
            }
            service.Name = serviceDto.Name;
            service.Description = serviceDto.Description;
            _appDbContext.Services.Update(service);
            await _appDbContext.SaveChangesAsync();
            return Ok(service);
        }
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteServicce(int id)
        {
            var service = await _appDbContext.Services.SingleOrDefaultAsync(x => x.Id == id);
            if (service == null)
            {
                return NotFound($"Service with ID {id} is not found.");
            }
            _appDbContext.Services.Remove(service);
            await _appDbContext.SaveChangesAsync();
            return NoContent();
        }

    }
}
