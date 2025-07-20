using Microsoft.AspNetCore.Mvc;
using web_jobs.Repository;

namespace web_jobs.Controllers
{
    public class CandidateController : Controller
    {
        private readonly IJobRepository _jobRepository;
        public CandidateController(IJobRepository jobRepository)
        {
            _jobRepository = jobRepository;
        }
        [HttpGet]
        public async Task<IActionResult> SearchJobs(string keyword, string location, string category)
        {
            var jobs = await _jobRepository.SearchJobsAsync(keyword, location, category);
            ViewData["Keyword"] = keyword;
            ViewData["Location"] = location;
            ViewData["Category"] = category;
            return View(jobs);
        }
        
    }
}
