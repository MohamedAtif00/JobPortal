namespace JobPortal.Model
{
    public class Job
    {
        public Guid JobId { get; set; } = Guid.NewGuid();
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string? Location { get; set; }
        public string? SalaryRange { get; set; }
        public string Category { get; set; } = string.Empty;
        public DateTime PostedDate { get; set; } = DateTime.UtcNow;

        public Guid CompanyId { get; set; }
        public Company Company { get; set; } = null!;
        public ICollection<Application> Applications { get; set; } = new List<Application>();
    }
}
