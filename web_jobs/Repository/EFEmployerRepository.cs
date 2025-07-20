using Microsoft.EntityFrameworkCore;
using web_jobs.Models;

namespace web_jobs.Repository
{
    public class EFEmployerRepository : IEmployerRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;
        public EFEmployerRepository(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public async Task<IEnumerable<Employer>> GetAllAsync()
        {
            return await _context.Employers.ToListAsync();
        }
        public async Task<IEnumerable<Job>> GetJobsByEmployerIdAsync(Guid employerId)
        {
            return await _context.Jobs
                .Include(j => j.Applications)
                .ThenInclude(a => a.CandidateProfile)
                .Where(j => j.EmployerID == employerId)
                .ToListAsync();
        }
        public async Task<Employer> GetByIdAsync(Guid id)
        {
            return await _context.Employers.FindAsync(id);
        }
        public async Task AddAsync(Employer employer)
        {
            _context.Employers.Add(employer);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateAsync(Employer employer)
        {
            _context.Employers.Update(employer);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(Guid id)
        {
            var employer = await _context.Employers.FindAsync(id);
            if (employer == null)
            {
                throw new KeyNotFoundException($"Không tìm thấy nhà tuyển dụng với Id = {id}.");
            }

            // Xóa logo nếu có
            if (!string.IsNullOrEmpty(employer.CompanyLogo))
            {
                var logoPath = Path.Combine(_env.WebRootPath, "images", employer.CompanyLogo);
                if (File.Exists(logoPath))
                {
                    File.Delete(logoPath);
                }
            }

            // Xóa giấy phép nếu có
            if (!string.IsNullOrEmpty(employer.LicenseDocument))
            {
                var licensePath = Path.Combine(_env.WebRootPath, "licenses", employer.LicenseDocument);
                if (File.Exists(licensePath))
                {
                    File.Delete(licensePath);
                }
            }

            _context.Employers.Remove(employer);
            await _context.SaveChangesAsync();
        }
        public async Task<Employer> GetByUserIdAsync(Guid userId)
        {
            return await _context.Employers.FirstOrDefaultAsync(e => e.UserID == userId);
        }
        public async Task<int> CountAllEmployersAsync()
        {
            return await _context.Employers.CountAsync();
        }
        public async Task<int> CountApprovedEmployersAsync()
        {
            return await _context.Employers.CountAsync(e => e.Status.ToLower() == "approved");
        }
        public async Task<int> CountUnapprovedEmployersAsync()
        {
            return await _context.Employers.CountAsync(e => e.Status.ToLower() == "pending");
        }
        public async Task<int> CountEmployersWithJobsAsync()
        {
            return await _context.Employers
                .Where(e => _context.Jobs.Any(j => j.EmployerID == e.ID))
                .CountAsync();
        }
        public async Task<IEnumerable<Employer>> GetAllEmployersAsync()
        {
            return await _context.Employers.ToListAsync();

        }
        public async Task<IEnumerable<Employer>> GetApprovedEmployersAsync()
        {
            return await _context.Employers
                .Where(e => e.Status.ToLower() == "approved").ToListAsync();
        }
        public async Task<IEnumerable<Employer>> GetUnapprovedEmployersAsync()
        {
            return await _context.Employers
                .Where(e => e.Status.ToLower() == "pending" || e.Status.ToLower() == "unapproved").ToListAsync();
        }
        public async Task<IEnumerable<Employer>> GetEmployersWithJobsAsync()
        {
            return await _context.Employers
                .Where(e => _context.Jobs.Any(j => j.EmployerID == e.ID))
                .ToListAsync();
        }
    }
}
