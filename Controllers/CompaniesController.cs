using JobPortal.Data;
using JobPortal.DTO;
using JobPortal.Helper;
using JobPortal.Model;
using JobPortal.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

namespace JobPortal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CompaniesController : ControllerBase
    {
        private readonly CompanyService _companyService;
        private readonly JobPortalContext _context;
        private readonly IWebHostEnvironment _environment;

        public CompaniesController(CompanyService companyService, JobPortalContext context, IWebHostEnvironment environment)
        {
            _companyService = companyService;
            _context = context;
            _environment = environment;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromForm] RegisterCompanyDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var company = new Company
            {
                Name = model.Name,
                Email = model.Email,
                IndustryId = model.IndustryId
                // Map other properties as needed
            };

            var result = await _companyService.RegisterCompanyAsync(company, model.Image, model.Password);

            if (result == null)
                return BadRequest("Failed to register the company.");


            return CreatedAtAction(nameof(GetCompanyById), new { companyId = result.CompanyId }, result);
        }

        //[HttpPost("{companyId}/AddImage")]
        //public async Task<IActionResult> AddImage([FromForm] IFormFile image)
        //{
        //    var path = Path.Combine(_webHostEnvironment.WebRootPath,"upload/companyImages");

        //    if (!Directory.Exists(path))
        //    {
        //        Directory.CreateDirectory(path);
        //    }

        //    var fileExtension = Path.GetExtension(image.FileName);

        //    string? uniqueFileName = Guid.NewGuid().ToString() + fileExtension;


        //    var filePath = Path.Combine(path, uniqueFileName);

        //    try
        //    {
        //        using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
        //        {
        //            await imageFile.CopyToAsync(fileStream);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        // Log exception (if logging is configured)
        //        // _logger.LogError(ex, "Error occurred while saving image file.");
        //        throw;
        //    }

        //    return Path.Combine(folder, uniqueFileName).Replace("\\", "/");

        //}

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto login)
        {
            var company = await _companyService.AuthenticateCompanyAsync(login.Email, login.Password);
            var existCompan = await _context.Companies.Where(x => x.Email == company.Email).SingleOrDefaultAsync();

            if (existCompan == null)
                return Unauthorized("Invalid email or password.");

            // Generate JWT Token
            var token = TokenHelper.GenerateJwtToken(existCompan.CompanyId.ToString(), existCompan.Email, "company", existCompan.Name);

            return Ok(new
            {
                Token = token,
                Company = new
                {
                    existCompan.CompanyId,
                    company.CompanyName,
                    company.Email
                }
            });
        }
        [HttpGet("{companyId}/GetCompanyImage")]
        public async Task<IActionResult> GetImage(Guid companyId)
        {
            var file = ImageHelper.GetImageFilePath($"Upload/CompanyImage/{companyId.ToString()}", _environment.WebRootPath);

            if (System.IO.File.Exists(file))
            {


                // Read the file into a byte array
                byte[] imageData = System.IO.File.ReadAllBytes(file);


                // Return the image data along with appropriate content type
                return File(imageData, "image/jpeg");
            }

            return NotFound("this master image is not exist");
        }

        [HttpGet]
        public async Task<IActionResult> GetCompanies()
        {
            return Ok(await _companyService.GetCompanies());
        }

        [HttpGet("{companyId}")]
        public async Task<IActionResult> GetCompanyById(Guid companyId)
        {
            var company = await _companyService.GetCompanyById(companyId);
            return company != null ? Ok(company) : NotFound();
        }

        [HttpGet("category/{industry}")]
        public async Task<IActionResult> GetCompaniesByIndustry(Guid industryId)
        {
            return Ok(await _companyService.GetCompaniesByIndustry(industryId));
        }

        [HttpGet("{companyId}/jobs")]
        public async Task<IActionResult> GetJobsByCompany(Guid companyId)
        {
            return Ok(await _companyService.GetJobsByCompany(companyId));
        }

        [HttpPost("AddIndustry")]
        public async Task<IActionResult> AddIndustry(string name)
        {
            _context.industries.Add(new Industry { Name= name});
            _context.SaveChanges();

            return Ok();
        }
        [HttpGet("GetSingleIndustry")]
        public async Task<IActionResult> GetSingleIndustry(Guid id) {
            return Ok(_context.industries.Where(x => x.Id == id).FirstOrDefault());
        }
        [HttpGet("GetAllIndustries")]
        public async Task<IActionResult> GetAllIndustries()
        {
            return Ok(_context.industries);
        }

        [HttpPost("{companyId}/jobs")]
        public async Task<IActionResult> PostJob(Guid companyId, [FromBody] CreateJob job)
        {
            return Ok(await _companyService.PostJob(companyId, new Job {Title=job.title,Description=job.description,Location=job.location,SalaryRange=job.salaryRange,CompanyId=companyId }));
        }
    }

    public record CreateJob(string title, string description, string location, string salaryRange);
}
