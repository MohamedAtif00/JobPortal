using JobPortal.Data;
using JobPortal.DTO;
using JobPortal.Helper;
using JobPortal.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.Design;
using static System.Net.Mime.MediaTypeNames;

namespace JobPortal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly JobPortalContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public EmployeeController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, JobPortalContext context, IWebHostEnvironment webHostEnvironment)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // POST: /api/employees/register
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromForm] RegisterEmployeeDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // Generate a GUID for both user.Id and employee.EmployeeId
            var id = Guid.NewGuid();

            var user = new ApplicationUser
            {
                Id = id.ToString(), // Assign generated ID
                UserName = model.Email,
                Email = model.Email,
                FullName = model.FullName,
            };

            // Save the profile image using the generated ID
            string path = await ImageHelper.SaveImageAsync(model.image, _webHostEnvironment.WebRootPath, "Upload/EmployeeImage", user.Id);

            user.ProfileImageUrl = path;

            // Create the Employee with the same ID
            var employee = new Employee
            {
                EmployeeId = id, // Assign the same ID
                FirstName = model.FullName,
                Email = model.Email,
                CreatedDate = DateTime.UtcNow
            };

            // Add the employee to the context
            _context.Employees.Add(employee);

            // Save user to identity
            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                await _context.SaveChangesAsync(); // Save the employee entity to the database
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

            var employee = await _context.Employees.Where(x => x.Email == user.Email).FirstOrDefaultAsync();
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
                    employee.EmployeeId,
                    user.Email,
                    Roles = roles
                }
            });
        }
        [HttpGet("{employeeId}/GetEmployeeImage")]
        public async Task<IActionResult> GetEmployeeImage(Guid employeeId)
        {
            var file = ImageHelper.GetImageFilePath($"Upload/EmployeeImage/{employeeId.ToString()}", _webHostEnvironment.WebRootPath);

            if (System.IO.File.Exists(file))
            {


                // Read the file into a byte array
                byte[] imageData = System.IO.File.ReadAllBytes(file);


                // Return the image data along with appropriate content type
                return File(imageData, "image/jpeg");
            }

            return NotFound("this master image is not exist");
        }


        // POST: /api/employees/{employeeId}/apply/{jobId}

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
            //var cvFilePath = Path.Combine("UploadedCVs", $"{Guid.NewGuid()}_{model.Cv.FileName}");
            //using (var stream = new FileStream(cvFilePath, FileMode.Create))
            //{
            //    await model.Cv.CopyToAsync(stream);
            //}

            // Create the job application
            var application = new JobPortal.Model.Application
            {
                ApplicationId = Guid.NewGuid(),
                EmployeeId = employeeId,
                JobId = jobId,
                AppliedDate = DateTime.UtcNow
            };

            string cvpath = await ImageHelper.SaveImageAsync(model.Cv,  _webHostEnvironment.WebRootPath, $"Upload/CV/{application.ApplicationId}", model.Cv.FileName);

            application.CvFilePath = cvpath;
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

        [HttpGet("GetCv")]
        public async Task<IActionResult> GetCv(Guid applicationId)
        {
            var file = ImageHelper.GetImageFilePath($"Upload/CompanyImage/{applicationId.ToString()}", _webHostEnvironment.WebRootPath);

            if (System.IO.File.Exists(file))
            {


                // Read the file into a byte array
                byte[] imageData = System.IO.File.ReadAllBytes(file);


                // Return the image data along with appropriate content type
                return File(imageData, "image/jpeg");
            }

            return NotFound("this master image is not exist");
        }




        [HttpGet("{jobId}/GetAllApplication")]
        public async Task<IActionResult> GetAllApplication(Guid jobId)
        {
            var comps = await _context.Applications.Where(x => x.JobId == jobId).ToListAsync();

            return Ok(comps);
        }

    }

}
