using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace web_jobs.Models
{
    public class SavedJob
    {
        public int Id { get; set; }

        [Required]
        public Guid UserId { get; set; }
        [Required]
        public Guid JobId { get; set; }

        public DateTime SavedAt { get; set; } = DateTime.Now;

        [ForeignKey("JobId")]
        public Job Job { get; set; }
    }
}
