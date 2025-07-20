using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;

namespace web_jobs.Controllers
{
    public class AccountController : Controller
    {
        private readonly IEmailSender _emailSender;

        public AccountController(IEmailSender emailSender)
        {
            _emailSender = emailSender;
        }

        public async Task<IActionResult> TestEmail()
        {
            await _emailSender.SendEmailAsync("nguoinhan@gmail.com", "Xác thực tài khoản", "<p>Đây là email xác thực!</p>");
            return Content("Gửi email thành công!");
        }
    }
}
