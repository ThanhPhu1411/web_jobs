namespace web_jobs.Models
{
    public class Notification
    {
        public int ID { get; set; }

        public Guid Receiver_ID { get; set; }

        public string Title { get; set; } = string.Empty;
        public string? Message { get; set; }

        public DateTime SentDate { get; set; } = DateTime.Now;
        public bool IsRead { get; set; } = false;
    }
}
