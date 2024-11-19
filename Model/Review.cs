namespace JobPortal.Model
{
    public class Review
    {
        public Guid ReviewId { get; set; } = Guid.NewGuid();
        public string Content { get; set; } = string.Empty; // Required
        public int Rating { get; set; } // Rating: 1 to 5
        public DateTime PostedDate { get; set; } = DateTime.UtcNow;

        // Foreign Key to Company
        public Guid CompanyId { get; set; }
        public Company Company { get; set; } = null!;

        // Foreign Key to Employee
        public Guid EmployeeId { get; set; }
        public Employee Employee { get; set; } = null!;
    }
}
