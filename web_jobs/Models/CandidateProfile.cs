namespace web_jobs.Models
{
    public class CandidateProfile
    {
        public Guid ID { get; set; } = Guid.NewGuid();
        public Guid? UserID { get; set; }
        public string UserName { get; set; }
        public string UserPosition { get; set; }
        public string UserEmail { get; set; }
        public string UserPhone { get; set; }
        public string UserAddress { get; set; }
        public DateTime? UserBirthDate { get; set; }
        public string UserFacebook { get; set; }
        public string? UserAvatar { get; set; } // nếu dùng để nhận file upload

        public string CareerObjective { get; set; }

        public string EducationYear { get; set; }
        public string Education { get; set; }

        public string ExperienceYear { get; set; }
        public string Experience { get; set; }

        public string DesiredSalary { get; set; }
        public string UserDesiredJob { get; set; }

        public string CertificateYear { get; set; }
        public string CertificateName { get; set; }

        public string PrizeYear { get; set; }
        public string PrizeDesc { get; set; }

        public string Language { get; set; }
        public string SoftSkill { get; set; }
        public string Interest { get; set; }
    }
}
