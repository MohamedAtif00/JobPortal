namespace JobPortal.Model
{
    public class Category
    {
        public Guid CategoryId { get; set; } = Guid.NewGuid();
        public string Name { get; set; } = string.Empty; // Required and unique
        public string? Description { get; set; } // Optional description

        public ICollection<Job> Jobs { get; set; } = new List<Job>();
    }
}
