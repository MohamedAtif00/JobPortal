namespace JobPortal.DTO
{
    public class RegisterCompanyDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public Guid IndustryId { get; set; }
        public IFormFile Image { get; set; }
    }
}
