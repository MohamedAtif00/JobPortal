namespace JobPortal.Model
{
    public class Company
    {
        public Guid CompanyId { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? ProfileImage { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public Guid IndustryId { get; set; }
        public ICollection<Job> Jobs { get; set; } = new List<Job>();
        public ICollection<Blog> Blogs { get; set; } = new List<Blog>();
        public ICollection<Review> Reviews { get; set; } = new List<Review>();
    }
}
