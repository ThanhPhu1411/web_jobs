namespace web_jobs.ViewModels
{
    public class AdminStatisticsViewModel
    {
        public int TotalJobs { get; set; }
        public int ApprovedJobs { get; set; }
        public int ExpiredJobs { get; set; }
        public int JobsExpiringSoon { get; set; }
        public int PendingJobs { get; set; }

        // Thống kê công ty
        public int TotalEmployers { get; set; }
        public int ApprovedEmployers { get; set; }
        public int PendingEmployers { get; set; }
        public int EmployersWithJobs { get; set; }
    }
}
