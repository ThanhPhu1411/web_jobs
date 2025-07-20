using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SQLitePCL;
using web_jobs.Repository;

namespace web_jobs.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminJobController : Controller
    {
        
        private readonly IJobRepository _jobRepository;
        private readonly IEmployerRepository _employerRepository;

        public AdminJobController(IJobRepository jobRepository, IEmployerRepository employerRepository)
        {
            _jobRepository = jobRepository;
            _employerRepository = employerRepository;
        }
        public async Task<IActionResult> Pending()
        {
            var jobs = (await _jobRepository.GetAllAsync()).Where(j =>j.Status =="pending").ToList();
            return View(jobs);
        }
        [HttpPost]
        public async Task<IActionResult> Approve(Guid id)
        {
            var job = await _jobRepository.GetByIdAsync(id);
            if (job == null)
            {
                return NotFound();
            }
            job.Status = "approved";
            await _jobRepository.UpdateAsync(job);
            return RedirectToAction("Pending");
        }
        [HttpPost]
        public async Task<IActionResult> Reject(Guid id)
        {
            var job = await _jobRepository.GetByIdAsync(id);
            if (job == null)
            {
                return NotFound();
            }
            job.Status = "rejected";
            await _jobRepository.UpdateAsync(job);
            return RedirectToAction("Pending");
        }
        public async Task<IActionResult> ApprovedAsync()
        {
            var jobs = (await _jobRepository.GetAllAsync())
               .Where(j => j.Status == "approved")
               .ToList();
            return View(jobs);
        }
        public async Task<IActionResult> Rejected()
        {
            var jobs = (await _jobRepository.GetAllAsync())
                        .Where(j => j.Status == "rejected")
                        .ToList();
            return View(jobs);
        }
        public async Task<IActionResult> PendingEmployers()
        {
            var employers = (await _employerRepository.GetAllAsync())
                            .Where(e => e.Status == "pending")
                            .ToList();
            return View(employers);
        }
        [HttpPost]
        public async Task<IActionResult> ApproveEmployer(Guid id)
        {
            var employer = await _employerRepository.GetByIdAsync(id);
            if (employer == null)
            {
                return NotFound();
            }
            employer.Status = "approved";
            await _employerRepository.UpdateAsync(employer);
            return RedirectToAction("PendingEmployers");
        }

        [HttpPost]
        public async Task<IActionResult> RejectEmployer(Guid id)
        {
            var employer = await _employerRepository.GetByIdAsync(id);
            if (employer == null)
            {
                return NotFound();
            }
            employer.Status = "rejected";
            await _employerRepository.UpdateAsync(employer);
            return RedirectToAction("PendingEmployers");
        }

        public async Task<IActionResult> EmployerDetails(Guid id)
        {
            var employer = await _employerRepository.GetByIdAsync(id);
            if (employer == null)
            {
                return NotFound();
            }
            return View(employer);
        }
        public async Task<IActionResult> AllEmployers(string filter = "all")
        {
            var allEmployers = (await _employerRepository.GetAllAsync())
                                .Where(e => !string.IsNullOrEmpty(e.Status))
                                .ToList();

            // Đếm theo trạng thái
            ViewBag.ApprovedCount = allEmployers.Count(e => e.Status.ToLower() == "approved");
            ViewBag.PendingCount = allEmployers.Count(e => e.Status.ToLower() == "pending");
            ViewBag.RejectedCount = allEmployers.Count(e => e.Status.ToLower() == "rejected");

            // Lọc theo filter (nếu có)
            if (!string.IsNullOrEmpty(filter) && filter.ToLower() != "all")
            {
                allEmployers = allEmployers
                    .Where(e => e.Status?.ToLower() == filter.ToLower())
                    .ToList();
                ViewBag.Filter = filter.ToLower();
            }
            else
            {
                ViewBag.Filter = "";
            }

            return View(allEmployers);
        }


    }
}
