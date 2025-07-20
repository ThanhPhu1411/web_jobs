using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NuGet.ProjectModel;
using web_jobs.Models;
using web_jobs.Repository;

namespace web_jobs.Controllers
{
    [Authorize]
    public class CandidateProfileController : Controller
    {
        private readonly ICandidateProfileRepository _candidateProfileRepositor;
        private readonly IWebHostEnvironment _env;
        public CandidateProfileController(ICandidateProfileRepository candidateProfileRepositor, IWebHostEnvironment env)
        {
            _candidateProfileRepositor = candidateProfileRepositor;
            _env = env;
        }
        // trả về luôn kiểu Guid
        private Guid? GetCurrentUserId()
        {
            if (User.Identity.IsAuthenticated)
            {
                var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if (Guid.TryParse(userIdString, out var userId))
                {
                    return userId;
                }
                
            }
            return null;
        }
        public async Task<IActionResult> Index()
        {
            var userID = GetCurrentUserId();
            var profile = await _candidateProfileRepositor.GetByUserIdAsync(userID.Value);
            if (profile == null)
            {
                // Nếu chưa có hồ sơ ứng viên, chuyển hướng đến trang tạo mới
                return RedirectToAction("Add");
            }
            return View(profile);
        }
        public async Task<IActionResult> Add()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]

        public async Task<IActionResult> Add(CandidateProfile candidateProfile, IFormFile avatarFile    )
        {
            var userID = GetCurrentUserId();
            if (userID == null)
            {
                return Unauthorized("Bạn cần đăng nhập để tạo hồ sơ ứng viên.");
            }

            candidateProfile.UserID = userID.Value;

            if (avatarFile == null || avatarFile.Length == 0)
            {
                ModelState.AddModelError("avatarFile", "Vui lòng chọn ảnh đại diện.");
            }

            if (!ModelState.IsValid)
            {
                return View(candidateProfile);
            }
            string fileName = Guid.NewGuid().ToString() + Path.GetExtension(avatarFile.FileName);
            string filePath = Path.Combine(_env.WebRootPath, "images", fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await avatarFile.CopyToAsync(stream);
            }
            candidateProfile.UserAvatar = fileName;
            await _candidateProfileRepositor.AddAsync(candidateProfile);
            return RedirectToAction("Index");
        }
        public async Task<IActionResult> Update(Guid id)
        {
            var profile = await _candidateProfileRepositor.GetByIdAsync(id);
            if (profile == null)
            {
                return NotFound("Không tìm thấy hồ sơ ứng viên.");
            }
            var userID = GetCurrentUserId();
            if (userID == null || userID.Value != profile.UserID)
            {
                return Unauthorized("Bạn không có quyền sửa hồ sơ này.");
            }
            return View(profile);
        }
        [HttpPost]
        public async Task<IActionResult> Update(Guid id, CandidateProfile candidateProfile, IFormFile avatarFile)
        {
            if (id != candidateProfile.ID)
            {
                return BadRequest("ID không khớp.");
            }

            // 2. Lấy bản ghi GỐC từ Database để kiểm tra quyền và cập nhật
            var existingProfileInDb = await _candidateProfileRepositor.GetByIdAsync(id);
            if (existingProfileInDb == null)
            {
                return NotFound("Không tìm thấy hồ sơ ứng viên.");
            }

            // 3. KIỂM TRA QUYỀN SỞ HỮU DỰA TRÊN DỮ LIỆU TỪ DATABASE (AN TOÀN)
            var currentUserID = GetCurrentUserId();
            if (currentUserID == null || currentUserID.Value != existingProfileInDb.UserID)
            {
                return Unauthorized("Bạn không có quyền sửa hồ sơ này.");
            }
            if (avatarFile != null && avatarFile.Length > 0)
            {
                if (!string.IsNullOrEmpty(existingProfileInDb.UserAvatar))
                {
                    // Xóa ảnh cũ nếu có
                    var oldAvatarPath = Path.Combine(_env.WebRootPath, "images", existingProfileInDb.UserAvatar);
                    if (System.IO.File.Exists(oldAvatarPath))
                    {
                        System.IO.File.Delete(oldAvatarPath);
                    }
                }
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(avatarFile.FileName);
                string filePath = Path.Combine(_env.WebRootPath, "images", fileName);
                //ghi nội dung của file người dùng upload vào đường dẫn đó.
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await avatarFile.CopyToAsync(stream);
                }
                candidateProfile.UserAvatar = fileName; // Cập nhật tên ảnh mới
            }
            else
            {
                // Nếu không có ảnh mới → giữ lại ảnh cũ
                candidateProfile.UserAvatar = existingProfileInDb.UserAvatar;
            }
            existingProfileInDb.UserName = candidateProfile.UserName;
            existingProfileInDb.UserEmail = candidateProfile.UserEmail;
            existingProfileInDb.UserPhone = candidateProfile.UserPhone;
            existingProfileInDb.UserAddress = candidateProfile.UserAddress;
            // ... Cập nhật tất cả các trường khác tương tự ...
            existingProfileInDb.UserPosition = candidateProfile.UserPosition;
            existingProfileInDb.UserBirthDate = candidateProfile.UserBirthDate;
            existingProfileInDb.UserFacebook = candidateProfile.UserFacebook;
            existingProfileInDb.UserDesiredJob = candidateProfile.UserDesiredJob;
            existingProfileInDb.DesiredSalary = candidateProfile.DesiredSalary;
            existingProfileInDb.ExperienceYear = candidateProfile.ExperienceYear;
            existingProfileInDb.Experience = candidateProfile.Experience;
            existingProfileInDb.CertificateYear = candidateProfile.CertificateYear;
            existingProfileInDb.CertificateName = candidateProfile.CertificateName;
            existingProfileInDb.PrizeYear = candidateProfile.PrizeYear;
            existingProfileInDb.PrizeDesc = candidateProfile.PrizeDesc;
            existingProfileInDb.Language = candidateProfile.Language;
            existingProfileInDb.SoftSkill = candidateProfile.SoftSkill;
            existingProfileInDb.Interest = candidateProfile.Interest;
            existingProfileInDb.CareerObjective = candidateProfile.CareerObjective;
            existingProfileInDb.Education = candidateProfile.Education;
            existingProfileInDb.EducationYear = candidateProfile.EducationYear;
            existingProfileInDb.UserAvatar = candidateProfile.UserAvatar;
            await _candidateProfileRepositor.UpdateAsync(existingProfileInDb);
            return RedirectToAction("Index");
        }
            // Cập nhật các trường khác nếu cần
            //     

        public async Task<IActionResult> Delete(Guid id)
        {
            var profile = await _candidateProfileRepositor.GetByIdAsync(id);
            if (profile == null)
            {
                return NotFound("Không tìm thấy hồ sơ ứng viên.");
            }
            var userID = GetCurrentUserId();
            if (userID == null || userID != profile.UserID)
            {
                return Unauthorized("Bạn không có quyền xóa hồ sơ này.");
            }
            return View(profile);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id, IFormCollection form, IFormFile avatarFile)
        {
            var profile = await _candidateProfileRepositor.GetByIdAsync(id);
            if (profile == null)
            {
                return NotFound("Không tìm thấy hồ sơ ứng viên.");
            }
            var userID = GetCurrentUserId();
            if (userID == null || userID != profile.UserID)
            {
                return Unauthorized("Bạn không có quyền xóa hồ sơ này.");
            }
            await _candidateProfileRepositor.DeleteAsync(id);
            return RedirectToAction("Index");
        }
    }
}
