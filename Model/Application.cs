namespace JobPortal.Model
{
    public class Application
    {
        public Guid ApplicationId { get; set; } = Guid.NewGuid();
        public Guid JobId { get; set; }
        public string CvFilePath { get; set; }
        public Job Job { get; set; } = null!;
        public Guid EmployeeId { get; set; }
        public Employee Employee { get; set; } = null!;
        public DateTime AppliedDate { get; set; } = DateTime.UtcNow;
        public string Status { get; set; } = "Pending";
    }
}
