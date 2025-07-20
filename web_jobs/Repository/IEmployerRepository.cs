using web_jobs.Models;

namespace web_jobs.Repository
{
    public interface IEmployerRepository
    {
        Task<IEnumerable<Employer>> GetAllAsync();
        Task<Employer> GetByIdAsync(Guid id);
        Task AddAsync(Employer employer);
        Task UpdateAsync(Employer employer);
        Task DeleteAsync(Guid id);
        Task<Employer> GetByUserIdAsync(Guid guid);
        Task<IEnumerable<Job>> GetJobsByEmployerIdAsync(Guid employerId);
        // 🔍 Thống kê thêm
        Task<int> CountAllEmployersAsync();
        Task<int> CountApprovedEmployersAsync();     // Giả sử Employer có IsApproved
        Task<int> CountUnapprovedEmployersAsync();
        Task<int> CountEmployersWithJobsAsync();
        // Lấy tất cả công ty
        Task<IEnumerable<Employer>> GetAllEmployersAsync();

        // Lấy danh sách công ty đã duyệt
        Task<IEnumerable<Employer>> GetApprovedEmployersAsync();

        // Lấy danh sách công ty chưa duyệt
        Task<IEnumerable<Employer>> GetUnapprovedEmployersAsync();

        // Lấy danh sách công ty có ít nhất 1 công việc đăng
        Task<IEnumerable<Employer>> GetEmployersWithJobsAsync();
    }
}
