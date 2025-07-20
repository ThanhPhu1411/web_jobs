using web_jobs.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

namespace web_jobs.Models
{
    public class ApplicationDbContext : IdentityDbContext<AppUser, IdentityRole, string>
    {
       public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
         : base(options)
       {
       }
       public DbSet<Job> Jobs { get; set; }
       public DbSet<Employer> Employers { get; set; }
       public DbSet<CandidateProfile> CandidateProfiles { get; set; }
       public DbSet<Notification> Notifications { get; set; }
       public DbSet<Application> Applications { get; set; }
       public DbSet<JobTypes> JobTypes { get; set; }
       public DbSet<Tag> Tags { get; set; } 
       public DbSet<JobTags> JobTags { get; set; }
       public DbSet<Category> Categories { get; set; }
        public DbSet<SavedJob> SavedJobs { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Application>()
                .HasKey(a => new {a.Job_ID, a.User_ID });
            modelBuilder.Entity<JobTags>()
                .HasKey(jt => new { jt.Job_ID, jt.Tag_ID });
            modelBuilder.Entity<JobTags>()
                .HasOne(jt => jt.Job)
                .WithMany(j => j.JobTags)
                .HasForeignKey(jt => jt.Job_ID)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<JobTags>()
                .HasOne(jt => jt.Tag)
                .WithMany(t => t.JobTags)
                .HasForeignKey(jt => jt.Tag_ID)
               .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Application>()
                .HasOne(a => a.Job)
                .WithMany(j => j.Applications)
                .HasForeignKey(a => a.Job_ID)
                .OnDelete(DeleteBehavior.Cascade);
            // Định nghĩa mối quan hệ giữa Job và Employer
            modelBuilder.Entity<Job>()
                .HasOne(j => j.Employer)
                .WithMany()
                .HasForeignKey(j => j.EmployerID)
                .OnDelete(DeleteBehavior.Restrict);
            // Định nghĩa mối quan hệ giữa Job và JobType
            modelBuilder.Entity<Job>()
             .HasOne(j => j.JobType)
             .WithMany()
             .HasForeignKey(j => j.JobTypeId)
             .OnDelete(DeleteBehavior.Restrict);
            // Định nghĩa mối quan hệ giữa Job và Category
            modelBuilder.Entity<Job>()
             .HasOne(j => j.Category)
             .WithMany()
             .HasForeignKey(j => j.CategoryId)
             .OnDelete(DeleteBehavior.Restrict);
            modelBuilder.Entity<Application>()
             .HasOne(a => a.CandidateProfile)
             .WithMany()
             .HasForeignKey(a => a.User_ID)
              .HasPrincipalKey(c => c.UserID) // 👈 Trỏ tới UserID trong CandidateProfile, không phải ID
             .OnDelete(DeleteBehavior.Restrict); // Không tự động xóa Candidate khi xóa application
        }

    }

}



