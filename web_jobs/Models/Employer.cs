namespace web_jobs.Models
{
    public class Employer
    {
        public Guid ID { get; set; } = Guid.NewGuid();
        public Guid? UserID { get; set; }
        public string CompanyName { get; set; }
        public string CompanyEmail { get; set; }
        public string CompanyLogo { get; set; }
        public string CompanySize { get; set; }
        public string CompanyDescription { get; set; }
        public string LicenseDocument { get; set; }
        public string Status { get; set; }
    }
}
