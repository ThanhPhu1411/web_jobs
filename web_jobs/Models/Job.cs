using System.ComponentModel.DataAnnotations;

namespace web_jobs.Models
{
    public class Job
    {
        public Guid ID { get; set; } = Guid.NewGuid();
        public Guid? EmployerID { get; set; }
        [Required]
        public string JobTitle { get; set; }
        [Required]
        public string JobDescription { get; set; }
        [Required]
        public string Requirements { get; set; }
        public string Salary { get; set; }
        public string Benefits { get; set; }
        [Required]
        public int JobTypeId { get; set; }      
        public DateTime PostedDate { get; set; } = DateTime.Now;
        public DateTime? ApplicationDeadline { get; set; }
        [Required]

        public string Locate { get; set; }
        public ICollection<JobTags> JobTags { get; set; } = new List<JobTags>();
        public JobTypes? JobType { get; set; }   
        public Employer? Employer { get; set; }
        [Required]
        public int CategoryId { get; set; }
        public Category? Category { get; set; }
        public string Status { get; set; }
        public ICollection<Application> Applications { get; set; } = new List<Application>();
    }
}
