using JobPortal.Data;
using JobPortal.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JobPortal.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class JobController : ControllerBase
    {
        private readonly JobPortalContext _context;

        public JobController(JobPortalContext context)
        {
            _context = context;
        }

        // GET: /api/jobs
        [HttpGet]
        public IActionResult GetAllJobs()
        {
            var jobs = _context.Jobs.ToList();
            return Ok(jobs);
        }

        

        // GET: /api/jobs/{jobId}
        [HttpGet("GetJob/{jobId}")]
        public IActionResult GetJobDetails(Guid jobId)
        {
            var job = _context.Jobs.FirstOrDefault(j => j.JobId == jobId);
            if (job == null)
            {
                return NotFound("Job not found.");
            }
            return Ok(job);
        }

        // GET: /api/jobs/search?title={title}&location={location}&category={category}
        [HttpGet("search")]
        public IActionResult SearchJobs([FromQuery] string? title, [FromQuery] string? location, [FromQuery] string? category)
        {
            var jobs = _context.Jobs.AsQueryable();

            if (!string.IsNullOrWhiteSpace(title))
            {
                jobs = jobs.Where(j => j.Title.Contains(title, System.StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(location))
            {
                jobs = jobs.Where(j => j.Location.Contains(location, System.StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrWhiteSpace(category))
            {
                jobs = jobs.Where(j => j.Category.Contains(category, System.StringComparison.OrdinalIgnoreCase));
            }

            return Ok(jobs.ToList());
        }
    }

    
}
