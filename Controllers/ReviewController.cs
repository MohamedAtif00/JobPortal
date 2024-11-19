using JobPortal.Data;
using JobPortal.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JobPortal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReviewController : ControllerBase
    {
        private readonly JobPortalContext _context;

        public ReviewController(JobPortalContext context)
        {
            _context = context;
        }

        // POST /api/reviews
        [HttpPost("reviews")]
        public async Task<IActionResult> CreateReview([FromBody] CreateReview review)
        {
            if (!_context.Companies.Any(c => c.CompanyId == review.companyId))
                return NotFound("Company not found.");

            if (!_context.Employees.Any(e => e.EmployeeId == review.employeId))
                return NotFound("Employee not found.");


            _context.Reviews.Add(new Review {Content=review.content,Rating=review.rating,PostedDate=DateTime.UtcNow,CompanyId=review.companyId,EmployeeId=review.employeId });
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCompanyReviews), new { companyId = review.companyId }, review);
        }

        // GET /api/companies/{companyId}/reviews
        [HttpGet("companies/{companyId}/reviews")]
        public async Task<IActionResult> GetCompanyReviews(Guid companyId,CancellationToken cancellationToken)
        {
            if (!_context.Companies.Any(c => c.CompanyId == companyId))
                return NotFound("Company not found.");

            var reviews = await _context.Reviews.Where(r => r.CompanyId == companyId).ToListAsync(cancellationToken);
            return Ok(reviews);
        } 

        // GET /api/employees/{employeeId}/reviews
        [HttpGet("employees/{employeeId}/reviews")]
        public async Task<IActionResult> GetEmployeeReviews(Guid employeeId)
        {
            if (!_context.Employees.Any(e => e.EmployeeId == employeeId))
                return NotFound("Employee not found.");

            var reviews = await _context.Reviews.Where(r => r.EmployeeId == employeeId).ToListAsync();
            return Ok(reviews);
        }
    }

    public record CreateReview(string content,int rating,Guid companyId,Guid employeId);
}
