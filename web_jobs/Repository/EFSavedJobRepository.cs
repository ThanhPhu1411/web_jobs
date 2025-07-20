    using Microsoft.EntityFrameworkCore;
    using web_jobs.Models;

    namespace web_jobs.Repository
    {
        public class EFSavedJobRepository : ISavedJobRepository
        {
            private readonly ApplicationDbContext _context;
            public EFSavedJobRepository(ApplicationDbContext context)
            {
                _context = context;
            }
            public async Task<IEnumerable<SavedJob>> GetSavedJobsByUserIdAsync(Guid userId)
            {
                return await _context.SavedJobs
                 .Where(s => s.UserId == userId)
                 .Include(s => s.Job)
                 .ThenInclude(j => j.Employer)
                 .Include(s => s.Job)
                 .ThenInclude(j => j.Category)
                 .Where(s => s.Job.Status == "approved") // Chỉ lấy job đã duyệt
                 .ToListAsync();
            }
            public async Task<bool> IsJobSavedAsync(Guid userId, Guid jobId)
            {
                return await _context.SavedJobs
                    .AnyAsync(SavedJob => SavedJob.UserId == userId && SavedJob.JobId == jobId);
            }
            public async Task<bool> DeleteSavedJobAsync (int savedJobId, Guid userId)
            {
                var savedJob = await _context.SavedJobs.FirstOrDefaultAsync(sj=>sj.Id ==savedJobId && sj.UserId == userId);
                if (savedJob == null)
                {
                    Console.WriteLine("Saved job không tồn tại hoặc không thuộc về người dùng này");
                    return false;
                }
                _context.SavedJobs.Remove(savedJob);
                await _context.SaveChangesAsync();
                Console.WriteLine("Xóa saved job thành công");
                return true;
            }
            public async Task<bool> SaveJobAsync(Guid userId, Guid jobId)
            {
                var jobExists = await _context.Jobs.AnyAsync(j => j.ID == jobId && j.Status == "approved");
                if (!jobExists)
                {
                    Console.WriteLine("Job không tồn tại hoặc chưa được duyệt");
                    return false;
                }

                if (await IsJobSavedAsync(userId, jobId))
                {
                    Console.WriteLine("Job đã được lưu trước đó");
                    return false;
                }

                var savedJob = new SavedJob
                {
                    UserId = userId,
                    JobId = jobId,
                    SavedAt = DateTime.UtcNow
                };
                _context.SavedJobs.Add(savedJob);
                await _context.SaveChangesAsync();
                Console.WriteLine("Lưu job thành công");
                return true;
            }
        }
    }
