using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using web_jobs.Models;
using web_jobs.Repository;

namespace web_jobs.Controllers
{
    [Authorize(Roles = "Employer")]

    public class JobController : Controller

    {
        private readonly IJobRepository _jobRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IJobTypeRepository _jobTypeRepository;
        private readonly IEmployerRepository _employerRepository;
        public JobController(IJobRepository jobRepository, ICategoryRepository categoryRepository, IJobTypeRepository jobTypeRepository, IEmployerRepository employerRepository)
        {
            _jobRepository = jobRepository;
            _categoryRepository = categoryRepository;
            _jobTypeRepository = jobTypeRepository;
            _employerRepository = employerRepository;
        }
        public string GetCurrentUserId()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (!string.IsNullOrEmpty(userId))
                return userId;

            // Fallback lấy claim sub
            userId = User.FindFirst("sub")?.Value;
            if (!string.IsNullOrEmpty(userId))
                return userId;

            // Fallback lấy tên đăng nhập
            var name = User.Identity.Name;
            return name ?? string.Empty;
        }
        public async Task<IActionResult> Index(string filter = "all")
        {
            var userIdStr = GetCurrentUserId();
            if (!Guid.TryParse(userIdStr, out Guid userId))
            {
                // userId không hợp lệ, xử lý theo ý bạn, ví dụ trả về lỗi hoặc rỗng
                return View(new List<Job>());
            }

            var jobs = (await _jobRepository.GetAllAsync())
                        .Where(j => j.Employer.UserID == userId)
                        .ToList();
            // Lọc theo trạng thái nếu có
            ViewBag.ApprovedCount = jobs.Count(j => j.Status == "approved" && j.ApplicationDeadline >= DateTime.Now);
            ViewBag.PendingCount = jobs.Count(j => j.Status == "pending" && j.ApplicationDeadline >= DateTime.Now);
            ViewBag.RejectedCount = jobs.Count(j => j.Status == "rejected" && j.ApplicationDeadline >= DateTime.Now);
            ViewBag.ExpiredCount = jobs.Count(j => j.ApplicationDeadline < DateTime.Now);

            // ⚙️ Lọc theo filter
            if (!string.IsNullOrEmpty(filter) && filter.ToLower() != "all")
            {
                if (filter.ToLower() == "expired")
                {
                    jobs = jobs.Where(j => j.ApplicationDeadline < DateTime.Now).ToList();
                }
                else
                {
                    jobs = jobs
                        .Where(j => j.Status?.ToLower() == filter.ToLower() && j.ApplicationDeadline >= DateTime.Now)
                        .ToList();
                }
            }
            else
            {
                ViewBag.Filter = ""; // ← THÊM DÒNG NÀY ĐỂ ĐẢM BẢO ViewBag KHÔNG BỊ LƯU GIÁ TRỊ CŨ
            }

            return View(jobs);
        }
        public async Task<IActionResult> Add()
        {
            ViewBag.Categories = new SelectList(await _categoryRepository.GetAllAsync(), "Id", "Name");
            ViewBag.JobTypes = new SelectList(await _jobTypeRepository.GetAllAsync(), "ID", "JobType_Name");
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Add(Job job)
        {
            //Kiểm tra xem người dùng đã đăng nhập chưa.
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("/Identity/Account/Login");
            }
            //Lấy ID của người dùng hiện tại (dạng chuỗi).
            var userIdStr = GetCurrentUserId();
            //Kiểm tra và chuyển chuỗi sang kiểu Guid.
            if (!Guid.TryParse(userIdStr, out Guid userId))
            {
                ModelState.AddModelError("", "Không thể xác định người dùng.");
                //Nạp lại danh sách ngành nghề (Categories) và loại công việc (JobTypes) để hiển thị lại form.
                ViewBag.Categories = new SelectList(await _categoryRepository.GetAllAsync(), "Id", "Name");
                ViewBag.JobTypes = new SelectList(await _jobTypeRepository.GetAllAsync(), "ID", "JobType_Name");
                //Trả về lại view để người dùng sửa.
                return View(job);
            }

            // Lấy Employer theo UserId
            var employer = await _employerRepository.GetByUserIdAsync(userId);
            if (employer == null)
            {
                ModelState.AddModelError("", "Không tìm thấy nhà tuyển dụng tương ứng.");
                ViewBag.Categories = new SelectList(await _categoryRepository.GetAllAsync(), "Id", "Name");
                ViewBag.JobTypes = new SelectList(await _jobTypeRepository.GetAllAsync(), "ID", "JobType_Name");
                return View(job);
            }
            if (employer.Status == "rejected")
            {
                ModelState.AddModelError("", "Công ty của bạn đã bị từ chối, không thể đăng tin tuyển dụng.");
                ViewBag.Categories = new SelectList(await _categoryRepository.GetAllAsync(), "Id", "Name");
                ViewBag.JobTypes = new SelectList(await _jobTypeRepository.GetAllAsync(), "ID", "JobType_Name");
                return View(job);
            }

            if (employer.Status != "approved")
            {
                ModelState.AddModelError("", "Công ty của bạn chưa được duyệt, không thể đăng tin tuyển dụng.");
                ViewBag.Categories = new SelectList(await _categoryRepository.GetAllAsync(), "Id", "Name");
                ViewBag.JobTypes = new SelectList(await _jobTypeRepository.GetAllAsync(), "ID", "JobType_Name");
                return View(job);
            }

            job.EmployerID = employer.ID;  // Gán EmployerID đúng theo user hiện tại
            job.Status = "pending";
            //Gọi repository để thêm công việc vào cơ sở dữ liệu.
            await _jobRepository.AddAsync(job);
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Detail(Guid id)
        {
            var jobs = await _jobRepository.GetByIdAsync(id);
            if (jobs == null)
            {
                return NotFound();
            }
            return View(jobs);
        }
        public async Task<IActionResult> Update(Guid id)
        {
            var job = await _jobRepository.GetByIdAsync(id);
            if (job == null)
                return NotFound();
            var userIdStr = GetCurrentUserId();
            if (!Guid.TryParse(userIdStr, out Guid currentUserId))
            {
                return Forbid();
            }
            var employer = await _employerRepository.GetByUserIdAsync(currentUserId);
            if (employer == null)
            {
                return Forbid();
            }
            if(job.EmployerID != employer.ID)
            {
                return Forbid();
            }

            ViewBag.Categories = new SelectList(await _categoryRepository.GetAllAsync(), "Id", "Name", job.CategoryId);
            ViewBag.JobTypes = new SelectList(await _jobTypeRepository.GetAllAsync(), "ID", "JobType_Name", job.JobTypeId);

            return View(job);
        }
        [HttpPost]
        public async Task<IActionResult> Update(Guid id, Job job)
        {
            if (id != job.ID)
            {
                return NotFound();
            }
            var userIdStr = GetCurrentUserId();
            if (!Guid.TryParse(userIdStr, out Guid currentUserId))
            {
                return Forbid();
            }
            var employer = await _employerRepository.GetByUserIdAsync(currentUserId);
            if (employer==null)
            {
                return Forbid();
            }
            if (job.EmployerID != employer.ID)
                return Forbid();
            //nếu dữ liệu gửi lên từ form không hợp lệ
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = new SelectList(await _categoryRepository.GetAllAsync(), "Id", "Name", job.CategoryId);
                ViewBag.JobTypes = new SelectList(await _jobTypeRepository.GetAllAsync(), "ID", "JobType_Name", job.JobTypeId);
                return View(job);
            }

            await _jobRepository.UpdateAsync(job);
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Delete(Guid id)
        {
            var job = await _jobRepository.GetByIdAsync(id);
            //Cố gắng chuyển chuỗi userIdStr thành kiểu Guid (currentUserId).
            var userIdStr = GetCurrentUserId();

            if (!Guid.TryParse(userIdStr, out Guid currentUserId))
            {
                return Forbid();
            }

            if (job == null)
            {
                return NotFound();
            }

            var employer = await _employerRepository.GetByUserIdAsync(currentUserId);
            if (employer == null)
            {
                return Forbid();
            }

            if (job.EmployerID != employer.ID)
            {
                return Forbid();
            }

            return View(job);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id, IFormCollection form)
        {
            var job = await _jobRepository.GetByIdAsync(id);
            var userIdStr = GetCurrentUserId();

            if (!Guid.TryParse(userIdStr, out Guid currentUserId))
            {
                return Forbid();
            }

            if (job == null)
            {
                return NotFound();
            }

            var employer = await _employerRepository.GetByUserIdAsync(currentUserId);
            if (employer == null || job.EmployerID != employer.ID)
            {
                return Forbid();
            }

            await _jobRepository.DeleteAsync(id);
            return RedirectToAction("Index");
        }
    }
}
