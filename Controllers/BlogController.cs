using JobPortal.Data;
using JobPortal.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JobPortal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogController : ControllerBase
    {
        private readonly JobPortalContext _context;

        public BlogController(JobPortalContext context)
        {
            _context = context;
        }

        // POST /api/companies/{companyId}/blogs
        [HttpPost("companies/{companyId}/blogs")]
        public async Task<IActionResult> CreateCompanyBlog(Guid companyId, [FromBody] CreateBlog blog)
        {
            if (!_context.Companies.Any(c => c.CompanyId == companyId))
                return NotFound("Company not found.");


            _context.Blogs.Add(new Blog { CompanyId=companyId, Title = blog.title, Content = blog.content, PostedDate = DateTime.UtcNow });
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetCompanyBlogs), new { companyId }, blog);
        }

        // GET /api/companies/{companyId}/blogs
        [HttpGet("companies/{companyId}/blogs")]
        public async Task<IActionResult> GetCompanyBlogs(Guid companyId)
        {
            if (!_context.Companies.Any(c => c.CompanyId == companyId))
                return NotFound("Company not found.");

            var blogs = await _context.Blogs.Where(b => b.CompanyId == companyId).ToListAsync();
            return Ok(blogs);
        }

        // POST /api/employees/{employeeId}/blogs
        [HttpPost("employees/{employeeId}/blogs")]
        public async Task<IActionResult> CreateEmployeeBlog(Guid employeeId, [FromBody] CreateBlog blog)
        {
            if (!_context.Employees.Any(e => e.EmployeeId == employeeId))
                return NotFound("Employee not found.");

            _context.Blogs.Add(new Blog {EmployeeId = employeeId,Title = blog.title,Content = blog.content,PostedDate = DateTime.UtcNow });
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetEmployeeBlogs), new { employeeId }, blog);
        }

        // GET /api/employees/{employeeId}/blogs
        [HttpGet("employees/{employeeId}/blogs")]
        public async Task<IActionResult> GetEmployeeBlogs(Guid employeeId)
        {
            if (!_context.Employees.Any(e => e.EmployeeId == employeeId))
                return NotFound("Employee not found.");

            var blogs = await _context.Blogs.Where(b => b.EmployeeId == employeeId).ToListAsync();
            return Ok(blogs);
        }
    }

    public record CreateBlog(string title,string content);
}
