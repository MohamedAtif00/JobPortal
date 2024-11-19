using JobPortal.DTO;
using JobPortal.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace JobPortal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public EmployeeController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // POST: /api/employees/register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterEmployeeDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                FullName = model.FullName,
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Employee");
                return Ok("Employee registered successfully.");
            }

            return BadRequest(result.Errors);
        }

        // POST: /api/employees/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto model)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, false, false);
            if (result.Succeeded)
            {
                return Ok("Employee logged in successfully.");
            }

            return Unauthorized("Invalid login attempt.");
        }

        // POST: /api/employees/{employeeId}/apply/{jobId}
        [Authorize(Roles = "Employee")]
        [HttpPost("{employeeId}/apply/{jobId}")]
        public IActionResult ApplyForJob(string employeeId, string jobId, [FromForm] ApplyJobDto model)
        {
            // Simulate job application storage (replace with your implementation)
            // Store the CV file and associate with the employee and job
            // You can save it to a database or file storage system

            return Ok($"Application submitted by employee {employeeId} for job {jobId}.");
        }

        // GET: /api/employees/{employeeId}/applications
        [Authorize(Roles = "Employee")]
        [HttpGet("{employeeId}/applications")]
        public IActionResult GetApplications(string employeeId)
        {
            // Simulate fetching applications (replace with your implementation)
            var applications = new[]
            {
                new { JobId = "1", JobTitle = "Software Engineer", AppliedDate = "2024-11-15" },
                new { JobId = "2", JobTitle = "Data Analyst", AppliedDate = "2024-11-14" }
            };

            return Ok(applications);
        }

        // GET: /api/employees/search?name={name}
        [HttpGet("search")]
        public IActionResult SearchEmployees(string name)
        {
            // Simulate employee search (replace with your implementation)
            var employees = new[]
            {
                new { EmployeeId = "1", FullName = "John Doe", Email = "john.doe@example.com" },
                new { EmployeeId = "2", FullName = "Jane Smith", Email = "jane.smith@example.com" }
            };

            var results = employees.Where(e => e.FullName.Contains(name, StringComparison.OrdinalIgnoreCase)).ToList();

            return Ok(results);
        }
    }
}
