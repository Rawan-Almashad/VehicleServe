using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VehicleServe.Data;
using VehicleServe.DTOs;
using VehicleServe.Models;

namespace VehicleServe.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RolesController : ControllerBase
    {
        private readonly AppDbContext _appDbContext;
        public RolesController(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }
        [HttpGet]
        public async Task<IActionResult> GetRoles()
        {
            var roles = await _appDbContext.Roles.ToListAsync();
            if (roles == null || roles.Count == 0)
            {
                return NoContent();
            }
            return Ok(roles);
        }
        [HttpGet("{id}")]
        public async Task<IActionResult> GetRole(int id)
        {
            var role = await _appDbContext.Roles.FindAsync(id);
            if (role == null)
            {
                return NotFound($"Role with ID {id} is not found.");
            }
            return Ok(role);
        }
        [HttpPost]
        public async Task<IActionResult> CreateRole(RoleDto roleDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            bool roleExists = await _appDbContext.Roles.AnyAsync(r => r.Name == roleDto.Name);
            if (roleExists)
            {
                return Conflict($"Role '{roleDto.Name}' already exists.");
            }

            var role = new Role { Name = roleDto.Name };
            _appDbContext.Roles.Add(role);

            await _appDbContext.SaveChangesAsync();
            return CreatedAtAction(nameof(GetRole), new { id = role.Id }, role);
        }
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRole(int id,RoleDto roleDto)
        {
            var role = await _appDbContext.Roles.SingleOrDefaultAsync(x => x.Id == id);
            if (role == null)
            {
                return NotFound($"Service with ID {id} is not found.");
            }
            role.Name = roleDto.Name;
           
            _appDbContext.Roles.Update(role);
            await _appDbContext.SaveChangesAsync();
            return Ok(role);
        }
    }
}
