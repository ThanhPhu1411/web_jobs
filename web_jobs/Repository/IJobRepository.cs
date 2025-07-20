using web_jobs.Models;

namespace web_jobs.Repository
{
    public interface IJobRepository
    {
        Task<IEnumerable<Job>> GetAllAsync();
        Task<Job> GetByIdAsync(Guid id);
        Task AddAsync(Job job);
        Task UpdateAsync(Job job);
        Task DeleteAsync(Guid id);
        Task<IEnumerable<Job>> SearchJobsAsync(string keyword, string location, string category);
        Task<int> CountAllJobsAsync();
        Task<int> CountExpiredJobsAsync();
        Task<int> CountJobsExpiringSoonAsync(TimeSpan timeSpan);
        Task<int> CountApprovedJobsAsync();        // Giả sử Job có cờ duyệt như IsApproved
        Task<int> CountUnapprovedJobsAsync();
        Task<int> CountPendingJobsAsync();

        Task<IEnumerable<Job>> GetApprovedJobsAsync();
        Task<IEnumerable<Job>> GetPendingJobsAsync();
        Task<IEnumerable<Job>> GetExpiredJobsAsync();
        Task<IEnumerable<Job>> GetJobsExpiringSoonAsync(TimeSpan timeSpan);
    }
}
