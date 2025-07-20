namespace web_jobs.Models
{
    public class Tag
    {
        public int ID { get; set; }
        public string Tag_Name { get; set; } = string.Empty;
        public ICollection<JobTags> JobTags { get; set; } = new List<JobTags>();
    }
}
