namespace JobPortal.Model
{
    public class Blog
    {
        public Guid BlogId { get; set; } = Guid.NewGuid();
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public DateTime PostedDate { get; set; } = DateTime.UtcNow;

        public string AuthorType { get; set; } = "Company"; // Or "Employee"
        public Guid? CompanyId { get; set; }
        public Company? Company { get; set; }
        public Guid? EmployeeId { get; set; }
        public Employee? Employee { get; set; }
    }
}
