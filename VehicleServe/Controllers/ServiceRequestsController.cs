using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;

using VehicleServe.Data;
using VehicleServe.Hubs;
using VehicleServe.Models;
using VehicleServe.Services;
using VehicleServe.DTOs;
using System.ComponentModel;
using Microsoft.AspNetCore.Identity;

namespace VehicleServe.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceRequestsController : ControllerBase
    {
        private readonly AppDbContext _appDbContext;
        private readonly IHubContext<ProviderHub> _hubContext;
        private readonly UserManager<IdentityUser> _userManager;

        public ServiceRequestsController(AppDbContext appDbContext, IHubContext<ProviderHub> hubContext, UserManager<IdentityUser> userManager)
        {
            _appDbContext = appDbContext;
            _hubContext = hubContext;
            _userManager = userManager;
            
        }

        [HttpPost]
        [Authorize(Roles = "CUSTOMER")]
        public async Task<IActionResult> CreateServiceRequest([FromBody] ServiceRequsetDto dto)
        {
            var customerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (customerId == null)
                return Unauthorized("Customer not found.");

            // Ensure customer exists
            var customerExists = await _appDbContext.Customers.AnyAsync(c => c.Id == customerId);
            if (!customerExists)
                return BadRequest("Invalid customer ID.");

            // Check if provider exists
            var providerExists = await _appDbContext.Providers.AnyAsync(p => p.Id == dto.ProviderId);
            if (!providerExists)
                return NotFound("Provider not found.");

            var serviceExists = await _appDbContext.Services.AnyAsync(s => s.Id == dto.ServiceId);
            if (!serviceExists)
                return BadRequest("The selected service does not exist.");

            // Check if provider offers the selected service
            var serviceOffered = await _appDbContext.ProviderServices
                .AnyAsync(ps => ps.ProviderId == dto.ProviderId && ps.ServiceId == dto.ServiceId);
            if (!serviceOffered)
                return BadRequest("Provider does not offer this service.");
            /*

            // Check if provider is online
            if (!OnlineUsersTracker.TryGetConnectionId(dto.ProviderId, out var connectionId))
                return BadRequest("Provider is not online.");
            */
            // Ensure vehicle exists
            var vehicle = await _appDbContext.Vehicles.FindAsync(dto.VehicleId);
            if (vehicle == null)
                return BadRequest("Vehicle not found.");

            // Create and save the service request
            var serviceRequest = new ServiceRequest
            {
                ProviderId = dto.ProviderId,
                CustomerId = customerId,
                Status = "In Progress",
                Longitude = dto.Longitude,
                Latitude = dto.Latitude,
                Notes = dto.Notes,
                ServiceId= dto.ServiceId,
                DateRequested = DateTime.UtcNow
            };
            var user = await _appDbContext.Customers.FirstOrDefaultAsync(x => x.Id == customerId);

            _appDbContext.ServiceRequests.Add(serviceRequest);
            await _appDbContext.SaveChangesAsync();
            /*
            // Send notification to provider
            await _hubContext.Clients.Client(connectionId)
                .SendAsync("ReceiveNotification", new
                {
                    Message = "You have a new Service Request",
                    Longitude = dto.Longitude,
                    Latitude = dto.Latitude,
                    LicensePlate = vehicle.LicensePlate,
                    Make = vehicle.Make,
                    PhoneNumber = user.User.PhoneNumber

                });
            */

            return Ok( "Service request created and notification sent.");
        }
        [HttpGet]
        [Authorize(Roles = "CUSTOMER")]
        public async Task<IActionResult> GetCustomerServiceRequests()
        {
            var customerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (customerId == null)
                return Unauthorized("Invalid customer token.");

            var serviceRequests = await _appDbContext.ServiceRequests
                .Where(sr => sr.CustomerId == customerId)
                .Include(sr => sr.Provider).ThenInclude(p => p.User)
                .Include(sr => sr.Service)// Get provider's name
                .Select(sr => new
                {
                    ServiceRequestId = sr.Id,
                    ProviderName = sr.Provider.User.UserName,
                    ServiceName = sr.Service.Name,
                    Status = sr.Status,
                    DateRequested = sr.DateRequested,
                    Notes = sr.Notes,
                    ProviderPhoneNumber = sr.Provider.User.PhoneNumber
                })
                .OrderByDescending(sr => sr.DateRequested)
                .ToListAsync();

            return Ok(serviceRequests);
        }
        [HttpGet("Provider")]
        [Authorize(Roles = "Provider")]
        public async Task<IActionResult> GetProviderServiceRequests()
        {
            var ProviderId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (ProviderId == null)
                return Unauthorized("Invalid customer token.");

            var serviceRequests = await _appDbContext.ServiceRequests
                .Where(sr => sr.ProviderId == ProviderId)
                .Include(sr => sr.Provider).ThenInclude(p => p.User)
                .Include(sr => sr.Service)// Get provider's name
                .Select(sr => new
                {
                    ServiceRequestId = sr.Id,
                    CustomerName = sr.Customer.User.UserName,
                    ServiceName = sr.Service.Name,
                    Status = sr.Status,
                    DateRequested = sr.DateRequested,
                    Notes = sr.Notes,
                    CustomerPhoneNumber = sr.Customer.User.PhoneNumber
                })
                .OrderByDescending(sr => sr.DateRequested)
                .ToListAsync();

            return Ok(serviceRequests);
        }
        [HttpPut("{serviceRequestId}/status")]
        [Authorize(Roles = "PROVIDER")]
        public async Task<IActionResult> UpdateServiceRequestStatus(int serviceRequestId, [FromBody] StatusDto dto)
        {
            var providerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Find the service request and ensure it belongs to the provider
            var serviceRequest = await _appDbContext.ServiceRequests
                .FirstOrDefaultAsync(sr => sr.Id == serviceRequestId && sr.ProviderId == providerId);

            if (serviceRequest == null)
                return NotFound("Service request not found or does not belong to you.");

            // Update status
            serviceRequest.Status = dto.status;
            await _appDbContext.SaveChangesAsync();

           
            return Ok(new { Message = "Service request status updated successfully." });
        }
       






    }
}
