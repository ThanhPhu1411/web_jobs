using web_jobs.Models;

namespace web_jobs.Repository
{
    public interface IJobTypeRepository
    {
        Task<IEnumerable<JobTypes>> GetAllAsync();
        Task<JobTypes> GetByIdAsync(int id);
        Task AddAsync(JobTypes jobType);
        Task UpdateAsync(JobTypes jobType);
        Task DeleteAsync(int id);
    }
}
