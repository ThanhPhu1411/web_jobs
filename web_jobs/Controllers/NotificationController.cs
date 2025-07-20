using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using web_jobs.Models;

namespace web_jobs.Controllers
{
    public class NotificationController : Controller
    {
        private readonly ApplicationDbContext _context;
        public NotificationController(ApplicationDbContext context)
        {
            _context = context;
        }
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
        // Xem thông báo chưa đọc
        public async Task<IActionResult> Unread()
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return Challenge();


            var notifications = await _context.Notifications
                .Where(n => n.Receiver_ID == userId && !n.IsRead)
                .OrderByDescending(n => n.SentDate) // Giả sử bạn có trường CreatedAt để sắp xếp
                .ToListAsync();

            ViewData["Title"] = "Thông báo chưa đọc";
            return View("MyNotifications", notifications);
        }
        // Xem thông báo đã đọc
        public async Task<IActionResult> Read()
        {
            var userId = GetCurrentUserId();
            if (userId == null)
                return Challenge();

         

            var notifications = await _context.Notifications
                .Where(n => n.Receiver_ID == userId && n.IsRead)
                .OrderByDescending(n => n.SentDate)
                .ToListAsync();

            ViewData["Title"] = "Thông báo đã đọc";
            return View("MyNotifications",notifications);
        }
        public async Task<IActionResult> Detail(int id)
        {
            var notification = await _context.Notifications.FindAsync(id);
            if (notification == null)
            {
                return NotFound();
            }
            // Đánh dấu thông báo là đã đọc
            if (!notification.IsRead)
            {
                notification.IsRead = true;
                _context.Notifications.Update(notification);
                await _context.SaveChangesAsync();
            }
            return View(notification);
        }
        public async Task<IActionResult> MyNotifications()
        {
            var userId = GetCurrentUserId();
            if (userId == null)
            {
                return Unauthorized("Bạn cần đăng nhập để xem thông báo.");
            }
            var notifications = await _context.Notifications
                .Where(n => n.Receiver_ID == userId.Value)
                .OrderByDescending(n => n.SentDate)
                .ToListAsync();
            return View(notifications);
        }
        public IActionResult Index()
        {
            return View();
        }
    }
}
