using web_jobs.Models;

namespace web_jobs.Repository
{
    public interface ICandidateProfileRepository
    {
        Task<CandidateProfile> GetByIdAsync(Guid guid);
        Task AddAsync(CandidateProfile candidateProfile);
        Task UpdateAsync(CandidateProfile candidateProfile);
        Task DeleteAsync(Guid id);
        Task<CandidateProfile> GetByUserIdAsync(Guid userId); // Lấy hồ sơ theo UserID
        Task<IEnumerable<CandidateProfile>> SearchAsync(string searchTerm);
    }
}
