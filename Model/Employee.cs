using Microsoft.AspNetCore.Mvc.ViewEngines;

namespace JobPortal.Model
{
    public class Employee
    {
        public Guid EmployeeId { get; set; } = Guid.NewGuid();
        public string FirstName { get; set; } = string.Empty;
        public string? LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? CV { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public ICollection<Application> Applications { get; set; } = new List<Application>();
        public ICollection<Blog> Blogs { get; set; } = new List<Blog>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
    }

}
