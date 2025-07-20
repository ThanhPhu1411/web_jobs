    using System.Diagnostics;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using SQLitePCL;
    using web_jobs.Models;
    using web_jobs.ViewModels;

    namespace web_jobs.Controllers
    {
        public class HomeController : Controller
        {
            private readonly ILogger<HomeController> _logger;
            private readonly ApplicationDbContext _context;

            public HomeController(ILogger<HomeController> logger,ApplicationDbContext context)
            {
                _logger = logger;
                _context = context;
            }

            public IActionResult Index(int? categoryId)
            { 
                var categories =_context.Categories.ToList();
                var jobs = _context.Jobs
                    .Include(j => j.Category)
                    .Include(j => j.Employer) 
                    .Where(j => j.Status == "approved") // Chỉ lấy các công việc đã được phê duyệt
                    .ToList();
                if (categoryId.HasValue){
                    jobs=jobs.Where(j=>j.CategoryId== categoryId.Value).ToList();
                }
                var viewModel = new HomeViewModel
                {
                    Categories = categories.Take(3).ToList(),
                    Jobs = jobs.Take(3).ToList(),
                    SelectedCategoryId = categoryId
                };
                return View(viewModel);
            }

            public IActionResult Privacy()
            {
                return View();
            }
            public IActionResult Login()
            {
                return View();
            }

            public IActionResult Profile()
            {
                return View();
            }

            [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
            public IActionResult Error()
            {
                return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
            }
        }
    }
