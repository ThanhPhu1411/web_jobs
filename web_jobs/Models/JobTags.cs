using Azure;

namespace web_jobs.Models
{
    public class JobTags
    {
        public Guid Job_ID { get; set; }
        public int Tag_ID { get; set; }

        public Job Job { get; set; }
        public Tag Tag { get; set; }
    }
}
