using web_jobs.Models;

namespace web_jobs.ViewModels
{
    public class JobPostingViewModel
    {
        public List<JobTypes> JobTypes { get; set; }
        public List<Category> Categories { get; set; }
    }
}
