using Microsoft.EntityFrameworkCore;
using web_jobs.Models;

namespace web_jobs.Repository
{
    public class EFJobTypeRepository : IJobTypeRepository
    {
        private readonly ApplicationDbContext _context;
        public EFJobTypeRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<JobTypes>> GetAllAsync()
        {
            return await _context.JobTypes.ToListAsync();
        }
        public async Task<JobTypes> GetByIdAsync(int id)
        {
            return await _context.JobTypes.FindAsync(id);
        }
        public async Task AddAsync(JobTypes jobTypes)
        {
            _context.JobTypes.Add(jobTypes);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateAsync(JobTypes jobTypes)
        {
            _context.JobTypes.Update(jobTypes);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(int id)
        {
            var jobType = await _context.JobTypes.FindAsync(id);
            if (jobType == null)
            {
                throw new KeyNotFoundException($"Không tìm thấy thể loại với Id = {id}.");
            }
            _context.JobTypes.Remove(jobType);
            await _context.SaveChangesAsync();
        }
    }
}
