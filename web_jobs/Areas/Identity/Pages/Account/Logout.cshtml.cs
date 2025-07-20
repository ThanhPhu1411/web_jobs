    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.RazorPages;
    using web_jobs.Models;

    namespace web_jobs.Areas.Identity.Pages.Account
    {
        [AllowAnonymous]
        public class LogoutModel : PageModel
        {
            private readonly SignInManager<AppUser> _signInManager;

            public LogoutModel(SignInManager<AppUser> signInManager)
            {
                _signInManager = signInManager;
            }
            public async Task<IActionResult> OnGet(string? returnUrl = null)
            {
                await _signInManager.SignOutAsync();
                if (returnUrl != null)
                {
                    return LocalRedirect(returnUrl);
                }

                return RedirectToPage("/Index");
            }

            public async Task<IActionResult> OnPost(string? returnUrl = null)
            {
                await _signInManager.SignOutAsync();

                if (returnUrl != null)
                {
                    return LocalRedirect(returnUrl);
                }

                return RedirectToPage("/Index");
            }
        }
    }
