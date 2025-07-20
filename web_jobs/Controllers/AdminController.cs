using Microsoft.AspNetCore.Mvc;
using web_jobs.Repository;
using web_jobs.ViewModels;

namespace web_jobs.Controllers
{
    public class AdminController : Controller
    {
        private readonly IJobRepository _jobRepository;
        private readonly IEmployerRepository _employerRepository;
        public AdminController(IJobRepository jobRepository, IEmployerRepository employerRepository)
        {
            _jobRepository = jobRepository;
            _employerRepository = employerRepository;
        }
        public async Task<IActionResult> Statistics()
        {
            var viewModel = new AdminStatisticsViewModel
            {
                TotalJobs = await _jobRepository.CountAllJobsAsync(),
                ApprovedJobs = await _jobRepository.CountApprovedJobsAsync(),
                ExpiredJobs = await _jobRepository.CountExpiredJobsAsync(),
                JobsExpiringSoon = await _jobRepository.CountJobsExpiringSoonAsync(TimeSpan.FromDays(7)),
                PendingJobs = await _jobRepository.CountPendingJobsAsync(),

                TotalEmployers = await _employerRepository.CountAllEmployersAsync(),
                ApprovedEmployers = await _employerRepository.CountApprovedEmployersAsync(),
                PendingEmployers = await _employerRepository.CountUnapprovedEmployersAsync(),
                EmployersWithJobs = await _employerRepository.CountEmployersWithJobsAsync()
            };

            return View(viewModel);
        }
        public async Task<IActionResult> GetDataPartial(string type)
        {
            switch (type.ToLower())
            {
                case "alljobs":
                    var allJobs = await _jobRepository.GetAllAsync();
                    ViewBag.TotalCount = allJobs.Count();
                    return PartialView("_JobListPartial", allJobs);

                case "approvedjobs":
                    var approvedJobs = await _jobRepository.GetApprovedJobsAsync();
                    ViewBag.TotalCount = approvedJobs.Count();
                    return PartialView("_JobListPartial", approvedJobs);

                case "pendingjobs":
                    var pendingJobs = await _jobRepository.GetPendingJobsAsync();
                    ViewBag.TotalCount = pendingJobs.Count();
                    return PartialView("_JobListPartial", pendingJobs);

                case "expiredjobs":
                    var expiredJobs = await _jobRepository.GetExpiredJobsAsync();
                    ViewBag.TotalCount = expiredJobs.Count();
                    return PartialView("_JobListPartial", expiredJobs);

                case "jobsexpiringsoon":
                    var expiringJobs = await _jobRepository.GetJobsExpiringSoonAsync(TimeSpan.FromDays(7));
                    ViewBag.TotalCount = expiringJobs.Count();
                    return PartialView("_JobListPartial", expiringJobs);

                case "allemployers":
                    var allEmployers = await _employerRepository.GetAllEmployersAsync();
                    ViewBag.TotalCount = allEmployers.Count();
                    return PartialView("_EmployerListPartial", allEmployers);

                case "approvedemployers":
                    var approvedEmployers = await _employerRepository.GetApprovedEmployersAsync();
                    ViewBag.TotalCount = approvedEmployers.Count();
                    return PartialView("_EmployerListPartial", approvedEmployers);

                case "pendingemployers":
                    var pendingEmployers = await _employerRepository.GetUnapprovedEmployersAsync();
                    ViewBag.TotalCount = pendingEmployers.Count();
                    return PartialView("_EmployerListPartial", pendingEmployers);

                case "employerswithjobs":
                    var employersWithJobs = await _employerRepository.GetEmployersWithJobsAsync();
                    ViewBag.TotalCount = employersWithJobs.Count();
                    return PartialView("_EmployerListPartial", employersWithJobs);

                default:
                    return Content("Không tìm thấy loại dữ liệu.");
            }
        }
    }
}
