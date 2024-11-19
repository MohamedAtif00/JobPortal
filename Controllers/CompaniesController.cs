using JobPortal.Data;
using JobPortal.DTO;
using JobPortal.Helper;
using JobPortal.Model;
using JobPortal.Services;
using Microsoft.AspNetCore.Http;
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

        public CompaniesController(CompanyService companyService, JobPortalContext context)
        {
            _companyService = companyService;
            _context = context;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register( RegisterCompanyDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var company = new Company
            {
                Name = model.Name,
                Email = model.Email,
                // Map other properties as needed
            };

            var result = await _companyService.RegisterCompanyAsync(company, model.Password);

            if (result == null)
                return BadRequest("Failed to register the company.");


            return CreatedAtAction(nameof(GetCompanyById), new { companyId = result.CompanyId }, result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto login)
        {
            var company = await _companyService.AuthenticateCompanyAsync(login.Email, login.Password);
            var existCompan = await _context.Companies.Where(x => x.Email == company.Email).SingleOrDefaultAsync();

            if (existCompan == null)
                return Unauthorized("Invalid email or password.");

            // Generate JWT Token
            var token = TokenHelper.GenerateJwtToken(existCompan.CompanyId.ToString(), existCompan.Email,"company", existCompan.Name);

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
        public async Task<IActionResult> GetCompaniesByIndustry(string industry)
        {
            return Ok(await _companyService.GetCompaniesByIndustry(industry));
        }

        [HttpGet("{companyId}/jobs")]
        public async Task<IActionResult> GetJobsByCompany(Guid companyId)
        {
            return Ok(await _companyService.GetJobsByCompany(companyId));
        }

        [HttpPost("{companyId}/jobs")]
        public async Task<IActionResult> PostJob(Guid companyId, [FromBody] CreateJob job)
        {
            return Ok(await _companyService.PostJob(companyId, new Job {Title=job.title,Description=job.description,Location=job.location,SalaryRange=job.salaryRange,CompanyId=companyId }));
        }
    }

    public record CreateJob(string title, string description, string location, string salaryRange);
}
