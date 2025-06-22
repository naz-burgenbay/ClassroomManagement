using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using ClassroomManagement.Models;

namespace ClassroomManagement.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        public DbSet<Course> Courses { get; set; }
        public DbSet<StudentCourse> StudentCourses { get; set; }
        public DbSet<CourseMaterial> CourseMaterials { get; set; }
        public DbSet<CourseMaterialFile> CourseMaterialFiles { get; set; }

        public DbSet<HomeworkTask> HomeworkTasks { get; set; }
        public DbSet<HomeworkTaskFile> HomeworkTaskFiles { get; set; }
        public DbSet<HomeworkSubmission> HomeworkSubmissions { get; set; }
        public DbSet<HomeworkSubmissionFile> HomeworkSubmissionFiles { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<ApplicationUser>()
                .Property(u => u.StudId)
                .HasMaxLength(6);

            builder.Entity<StudentCourse>()
                .HasKey(ss => new { ss.StudentId, ss.CourseId });

            builder.Entity<StudentCourse>()
                .HasOne(ss => ss.Student)
                .WithMany(u => u.StudentCourses)
                .HasForeignKey(ss => ss.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<StudentCourse>()
                .HasOne(ss => ss.Course)
                .WithMany(s => s.StudentCourse)
                .HasForeignKey(ss => ss.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<CourseMaterial>()
                .HasOne(cm => cm.Course)
                .WithMany(c => c.Materials)
                .HasForeignKey(cm => cm.CourseId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<HomeworkTask>()
                .HasOne(ht => ht.Course)
                .WithMany(c => c.HomeworkTasks)
                .HasForeignKey(ht => ht.CourseId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<HomeworkTask>()
                .HasOne(ht => ht.Instructor)
                .WithMany()
                .HasForeignKey(ht => ht.InstructorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<HomeworkTaskFile>()
                .HasOne(f => f.HomeworkTask)
                .WithMany(t => t.Files)
                .HasForeignKey(f => f.HomeworkTaskId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<HomeworkSubmission>()
                .HasOne(s => s.HomeworkTask)
                .WithMany(ht => ht.Submissions)
                .HasForeignKey(s => s.HomeworkTaskId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Entity<HomeworkSubmission>()
                .Property(hs => hs.Grade)
                .HasPrecision(3, 1);

            builder.Entity<HomeworkSubmission>()
                .HasOne(s => s.Student)
                .WithMany()
                .HasForeignKey(s => s.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<HomeworkSubmissionFile>()
                .HasOne(f => f.HomeworkSubmission)
                .WithMany(s => s.Files)
                .HasForeignKey(f => f.HomeworkSubmissionId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}