using Microsoft.EntityFrameworkCore;
using web_jobs.Models;

namespace web_jobs.Repository
{
    public class EFCandidateProfileRepository : ICandidateProfileRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;
        public EFCandidateProfileRepository(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }
        public async Task<CandidateProfile> GetByIdAsync(Guid guid)
        {
            return await _context.CandidateProfiles.FindAsync(guid);
        }
        public async Task<CandidateProfile> GetByUserIdAsync(Guid userID)
        {
            return await _context.CandidateProfiles.FirstOrDefaultAsync(cp => cp.UserID == userID);
        }
        public async Task AddAsync(CandidateProfile candidateProfile_)
        {
            _context.CandidateProfiles.Add(candidateProfile_);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateAsync(CandidateProfile candidateProfile)
        {
            _context.CandidateProfiles.Update(candidateProfile);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(Guid id)
        {
            var candidateProfile = await _context.CandidateProfiles.FindAsync(id);
            if (candidateProfile == null)
            {
                throw new KeyNotFoundException($"Không tìm thấy hồ sơ ứng viên với Id = {id}.");
            }
            if(!string.IsNullOrEmpty(candidateProfile.UserAvatar))
            {
                var avatarPath = Path.Combine(_env.WebRootPath, "images", candidateProfile.UserAvatar);
                if (File.Exists(avatarPath))
                {
                    File.Delete(avatarPath);
                }
            }
            _context.CandidateProfiles.Remove(candidateProfile);
            await _context.SaveChangesAsync();
        }
        public async Task<IEnumerable<CandidateProfile>> SearchAsync(string searchTerm)
        {
            return await _context.CandidateProfiles
                .Where(p => p.UserName.Contains(searchTerm) ||
                            p.UserPosition.Contains(searchTerm) ||
                            p.UserEmail.Contains(searchTerm))
                .ToListAsync();
        }
    }
}
