using JobPortal.Data;
using JobPortal.DTO;
using JobPortal.Helper;
using JobPortal.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JobPortal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly JobPortalContext _context;
        public EmployeeController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, JobPortalContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
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
            _context.Employees.Add(new Employee { FirstName = model.FullName, Email = model.Email, CreatedDate = DateTime.UtcNow });
            _context.SaveChanges();
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
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Authenticate user
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
                return Unauthorized("Invalid email or password.");

            var result = await _signInManager.PasswordSignInAsync(user, model.Password, false, false);

            var employee = _context.Employees.Where(x => x.Email == user.Email).SingleOrDefaultAsync();
            if (!result.Succeeded)
                return Unauthorized("Invalid login attempt.");

            // Generate JWT token
            var roles = await _userManager.GetRolesAsync(user);
            var token = TokenHelper.GenerateJwtToken(user.Id, user.Email, roles.FirstOrDefault());

            return Ok(new
            {
                Token = token,
                User = new
                {
                    employee.Id,
                    user.Email,
                    Roles = roles
                }
            });
        }


        // POST: /api/employees/{employeeId}/apply/{jobId}
        [Authorize(Roles = "Employee")]
        [HttpPost("{employeeId}/apply/{jobId}")]
        public async Task<IActionResult> ApplyForJob(Guid employeeId, Guid jobId, [FromForm] ApplyJobDto model)
        {
            // Validate employee existence
            var employee = await _context.Employees.FindAsync(employeeId);
            if (employee == null)
            {
                return NotFound($"Employee with ID {employeeId} not found.");
            }

            // Validate job existence
            var job = await _context.Jobs.FindAsync(jobId);
            if (job == null)
            {
                return NotFound($"Job with ID {jobId} not found.");
            }

            // Validate CV file
            if (model.Cv == null || model.Cv.Length == 0)
            {
                return BadRequest("A CV file is required.");
            }

            // Save the CV file (example: to a local folder)
            var cvFilePath = Path.Combine("UploadedCVs", $"{Guid.NewGuid()}_{model.Cv.FileName}");
            using (var stream = new FileStream(cvFilePath, FileMode.Create))
            {
                await model.Cv.CopyToAsync(stream);
            }

            // Create the job application
            var application = new Application
            {
                ApplicationId = Guid.NewGuid(),
                EmployeeId = employeeId,
                JobId = jobId,
                CvFilePath = cvFilePath,
                AppliedDate = DateTime.UtcNow
            };

            // Save to database
            _context.Applications.Add(application);
            await _context.SaveChangesAsync();

            return Ok($"Application submitted successfully by employee {employeeId} for job {jobId}.");
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
