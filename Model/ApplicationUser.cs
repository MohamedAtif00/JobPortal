using Microsoft.AspNetCore.Identity;

namespace JobPortal.Model
{
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }
        public string? ProfileImageUrl { get; set; } // Optional for storing profile images
        public string? CompanyName { get; set; }  // Optional, only for companies
        // Additional properties can be added here if needed.
    }
}
