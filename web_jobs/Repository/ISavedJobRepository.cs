using web_jobs.Models;

namespace web_jobs.Repository
{
    public interface  ISavedJobRepository
    {
        //: LƯU một công việc cho người dùng.
        Task<bool> SaveJobAsync(Guid userId, Guid jobId);

        //KIỂM TRA xem một công việc đã được người dùng lưu hay chưa.
        Task<bool> IsJobSavedAsync(Guid userId, Guid jobId);
        Task<IEnumerable<SavedJob>> GetSavedJobsByUserIdAsync(Guid userId);
        Task<bool> DeleteSavedJobAsync(int savedJobId, Guid userId);

    }
}
