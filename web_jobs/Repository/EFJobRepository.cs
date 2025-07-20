using Microsoft.EntityFrameworkCore;
using web_jobs.Models;

namespace web_jobs.Repository
{
    public class EFJobRepository : IJobRepository
    {
        private readonly ApplicationDbContext _context;

        public EFJobRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Job>> GetAllAsync()
        {
            return await _context.Jobs
                .Include(j => j.Category) // Eager loading Category
                .Include(j => j.Employer)
                .Include(j => j.JobType)
                .Include(j => j.JobTags)
                .ToListAsync();
        }
        public async Task<Job> GetByIdAsync(Guid id)
        {
            return await _context.Jobs
                .Include(j => j.Category)
                .Include(j => j.Employer)
                .Include(j => j.JobType)
                .Include(j => j.JobTags)
                .FirstOrDefaultAsync(j => j.ID == id);
        }
        public async Task AddAsync(Job job)
        {
            _context.Jobs.Add(job);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateAsync(Job job)
        {
            _context.Jobs.Update(job);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(Guid id)
        {
            var job = await _context.Jobs.FindAsync(id);
            _context.Jobs.Remove(job);
            await _context.SaveChangesAsync();
        }
        public async Task<IEnumerable<Job>> SearchJobsAsync(string keyword, string location, string category)
        {
            var query = _context.Jobs
                .Include(j => j.Category)
                .Include(j => j.Employer)
                .Include(j => j.JobType)
                .Include(j => j.JobTags)
                .Where(j => j.Status == "approved") // Chỉ lấy các job đã duyệt
                .AsQueryable();

            if (!string.IsNullOrEmpty(keyword))
            {
                query = query.Where(j =>
                    j.JobTitle.Contains(keyword) ||
                    j.JobDescription.Contains(keyword) ||
                    j.Requirements.Contains(keyword));
            }

            if (!string.IsNullOrEmpty(location))
            {
                query = query.Where(j => j.Locate.Contains(location));
            }

            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(j => j.Category.Name.Contains(category));
            }

            return await query.ToListAsync();
        }
        public async Task<int> CountAllJobsAsync()
        {
            return await _context.Jobs.CountAsync();
        }
        public async Task<int> CountExpiredJobsAsync()
        {
            var now = DateTime.Now;
            return await _context.Jobs.CountAsync(j => j.ApplicationDeadline != null && j.ApplicationDeadline < now);
        }
        public async Task<int> CountJobsExpiringSoonAsync(TimeSpan timeSpan)
        {
            var now = DateTime.Now;
            var soon = now.Add(timeSpan);
            return await _context.Jobs.CountAsync(j => j.ApplicationDeadline != null 
            && j.ApplicationDeadline > now 
            && j.ApplicationDeadline <= soon);
        }
        public async Task<int> CountApprovedJobsAsync()
        {
            return await _context.Jobs.CountAsync(j => j.Status == "approved");
        }
        public async Task<int> CountUnapprovedJobsAsync()
        {
            return await _context.Jobs.CountAsync(j => j.Status == "unapproved");
        }
        public async Task<int> CountPendingJobsAsync()
        {
            return await _context.Jobs.CountAsync(j => j.Status == "pending");

        }
        public async Task<IEnumerable<Job>> GetApprovedJobsAsync()
        {
            return await _context.Jobs
                .Include(j => j.Category)
                .Include(j => j.Employer)
                .Include(j => j.JobType)
                .Where(j => j.Status.ToLower() == "approved")
                .ToListAsync();
        }
        public async Task<IEnumerable<Job>> GetPendingJobsAsync()
        {
            return await _context.Jobs
                .Include(j => j.Category)
                .Include(j => j.Employer)
                .Include(j => j.JobType)
                .Where(j => j.Status.ToLower() == "pending")
                .ToListAsync();
        }
        public async Task<IEnumerable<Job>> GetExpiredJobsAsync()
        {
            var now = DateTime.Now;
            return await _context.Jobs
                .Include(j => j.Category)
                .Include(j => j.Employer)
                .Include(j => j.JobType)
                .Where(j => j.ApplicationDeadline != null && j.ApplicationDeadline < now)
                .ToListAsync();
        }
        public async Task<IEnumerable<Job>> GetJobsExpiringSoonAsync(TimeSpan timeSpan)
        {
            var now = DateTime.Now;
            var soon = now.Add(timeSpan);
            return await _context.Jobs
                .Include(j => j.Category)
                .Include(j => j.Employer)
                .Include(j => j.JobType)
                .Where(j => j.ApplicationDeadline != null && j.ApplicationDeadline >= now && j.ApplicationDeadline <= soon)
                .ToListAsync();
        }
    }
}
