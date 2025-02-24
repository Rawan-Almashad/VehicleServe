using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using VehicleServe.Data;
using VehicleServe.DTOs;
using VehicleServe.Models;
using Microsoft.EntityFrameworkCore;

namespace VehicleServe.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewsController : ControllerBase
    {
        private readonly AppDbContext _appDbContext;
        public ReviewsController(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }
        [HttpPut("review/{serviceRequestId}")]
        [Authorize(Roles = "CUSTOMER")]
        public async Task<IActionResult> AddReview(int serviceRequestId, [FromBody] AddReviewDto dto)
        {
            var customerId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // Find the service request
            var serviceRequest = await _appDbContext.ServiceRequests
                .Include(sr => sr.Review) // Ensure related review is loaded
                .FirstOrDefaultAsync(sr => sr.Id == serviceRequestId && sr.CustomerId == customerId);

            if (serviceRequest == null)
                return NotFound("Service request not found or does not belong to you.");

            // Ensure the service request is completed before allowing a review
            if (serviceRequest.Status != "Completed")
                return BadRequest("You can only review completed service requests.");

            // Ensure the review has not already been added (for 1-to-1 relationships)
            if (serviceRequest.Review != null)
                return BadRequest("You have already reviewed this service request.");

            var review = new Review
            {
                ServiceRequestId = serviceRequestId,
                Rating = dto.Rating,
                Comment = dto.Comment,
                CustomerId = serviceRequest.CustomerId,
                ProviderId = serviceRequest.ProviderId,
                CreatedAt = DateTime.UtcNow
            };

            _appDbContext.Reviews.Add(review);
            await _appDbContext.SaveChangesAsync();

            return Ok(new { Message = "Review added successfully." });
        }

        [HttpGet("provider/reviews/{providerId}")]
        public async Task<IActionResult> GetProviderReviews(string providerId)
        {
            // Ensure the provider exists
            var providerExists = await _appDbContext.Providers.AnyAsync(p => p.Id == providerId);
            if (!providerExists)
                return NotFound("Provider not found.");

            // Get reviews for the provider
            var reviews = await _appDbContext.Reviews
                .Where(r => r.ProviderId == providerId)
                .OrderByDescending(r => r.CreatedAt) 
                .Select(r => new
                {
                    r.Id,
                    r.Rating,
                    r.Comment,
                    r.CreatedAt,
                    CustomerName = r.Customer.User.UserName ,
                    r.ServiceRequest.Service.Name
                })
                .ToListAsync();

            return Ok(reviews);
        }
        [HttpGet("customer/reviews/{CustomerId}")]
        public async Task<IActionResult> GetCustomerReviews(string CustomerId)
        {
            // Ensure the provider exists
            var customerExists = await _appDbContext.Providers.AnyAsync(p => p.Id == CustomerId);
            if (!customerExists)
                return NotFound("Customer not found.");

            // Get reviews for the provider
            var reviews = await _appDbContext.Reviews
                .Where(r => r.ProviderId == CustomerId)
                .OrderByDescending(r => r.CreatedAt)
                .Select(r => new
                {
                    r.Id,
                    r.Rating,
                    r.Comment,
                    r.CreatedAt,
                    ProviderName = r.Provider.User.UserName,
                    r.ServiceRequest.Service.Name
                })
                .ToListAsync();

            return Ok(reviews);
        }

    }
}
