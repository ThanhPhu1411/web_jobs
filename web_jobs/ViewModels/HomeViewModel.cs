using web_jobs.Models;

namespace web_jobs.ViewModels
{
    public class HomeViewModel
    {
        public List<Category> Categories { get; set; }
        public List<Job> Jobs { get; set; }
        public int? SelectedCategoryId { get; set; }
        //public int CurrentPage { get; set; }
        //public int TotalJobs { get; set; }
        //public int PageSize { get; set; }
        //public int TotalPages => (int)Math.Ceiling((double)TotalJobs / PageSize);
    }
}
