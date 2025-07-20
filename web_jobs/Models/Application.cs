namespace web_jobs.Models
{
    public class Application
    {
        public Guid Job_ID { get; set; }
        public Guid User_ID { get; set; }

        // Thêm thuộc tính Job để có thể truy cập thông tin công việc
        public virtual Job Job { get; set; }  // Mối quan hệ giữa Application và Job
        public virtual CandidateProfile CandidateProfile { get; set; }

        public DateTime ApplyDate { get; set; } = DateTime.Now;

        public string Status { get; set; }
        public string SaveStatus { get; set; }
        public string? Note { get; set; }

    }
}
