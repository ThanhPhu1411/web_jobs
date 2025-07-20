using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using web_jobs.Models;
using web_jobs.Repository;
using Microsoft.AspNetCore.Identity.UI;

var builder = WebApplication.CreateBuilder(args);

// Kết nối CSDL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Cấu hình Identity
builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders()
.AddDefaultUI();

// Đăng ký dịch vụ DI
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<ICategoryRepository, EFCategoryRepository>();
builder.Services.AddScoped<IJobRepository, EFJobRepository>();
builder.Services.AddTransient<IEmailSender, EmailSender>();
builder.Services.AddScoped<IEmployerRepository, EFEmployerRepository>();
builder.Services.AddScoped<IJobTypeRepository, EFJobTypeRepository>();
builder.Services.AddScoped<ICandidateProfileRepository, EFCandidateProfileRepository>();
builder.Services.AddScoped<ISavedJobRepository, EFSavedJobRepository>();
builder.Services.AddRazorPages();

var app = builder.Build();

// Cấu hình middleware pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapRazorPages();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// 👉 Gọi seed dữ liệu tại đây
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var userManager = services.GetRequiredService<UserManager<AppUser>>();
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    await SeedAdminUserAsync(userManager, roleManager);
}

// 👉 Hàm seed role và user Admin
async Task SeedAdminUserAsync(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
{
    // Danh sách các vai trò cần tạo
    string[] roles = new[] { "Admin", "Employer", "Candidate" };

    // Tạo các vai trò nếu chưa có
    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }

    // Tạo tài khoản admin mẫu
    string adminEmail = "Bon@gmail.com";
    string adminPassword = "Bon123456@";

    var adminUser = await userManager.FindByEmailAsync(adminEmail);
    if (adminUser == null)
    {
        adminUser = new AppUser
        {
            UserName = adminEmail,
            Email = adminEmail,
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(adminUser, adminPassword);
        if (!result.Succeeded)
        {
            throw new Exception("Failed to create admin user: " + string.Join(", ", result.Errors.Select(e => e.Description)));
        }
    }

    // Gán vai trò Admin nếu chưa có
    if (!await userManager.IsInRoleAsync(adminUser, "Admin"))
    {
        await userManager.AddToRoleAsync(adminUser, "Admin");
    }
}

app.Run();
