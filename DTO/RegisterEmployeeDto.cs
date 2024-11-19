using System.ComponentModel.DataAnnotations;

namespace JobPortal.DTO
{
    public class RegisterEmployeeDto
    {
        [Required]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(6)]
        public string Password { get; set; }
    }


    public class ApplyJobDto
    {
        [Required]
        public string CoverLetter { get; set; }

        [Required]
        public IFormFile Cv { get; set; } // CV file upload
    }
}
