using Microsoft.EntityFrameworkCore;
using P01_StudentSystem.Data.Models;
using System;

namespace P01_StudentSystem.Data
{
    public class StudentSystemContext : DbContext
    {
        public StudentSystemContext()
        {

        }

        public StudentSystemContext(DbContextOptions options)
            : base(options)
        {

        }

        public DbSet<Course> Courses { get; set; }

        public DbSet<Homework> HomeworkSubmissions { get; set; }

        public DbSet<Resource> Resources { get; set; }

        public DbSet<Student> Students { get; set; }

        public DbSet<StudentCourse> StudentCourses { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=.;Database=SoftUni;Integrated Security=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Student>(e =>
            {
                e.HasCheckConstraint("CHK_PhoneNumber", "LEN(PhoneNumber)=10");

                e.Property(e => e.Name).IsUnicode(true);

            });

            modelBuilder.Entity<Course>(c =>
            {
                c.Property(x => x.Name).IsUnicode(true);

                c.Property(x => x.CourseId).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<Homework>(h =>
            {
                h.Property(h => h.HomeWorkId).ValueGeneratedOnAdd();

                h.Property(h => h.Content).IsUnicode(false);

                h.HasOne(h => h.Course)
                .WithMany(c => c.HomeworkSubmissions)
                .HasForeignKey(h => h.CourseId)
                .OnDelete(DeleteBehavior.Restrict);

                h.HasOne(h => h.Student)
                .WithMany(s => s.HomeworkSubmissions)
                .HasForeignKey(x => x.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

                h.HasCheckConstraint("CHK_ContentType", "ContentType IN ('Application','Pdf' ,'Zip')");
            });

            modelBuilder.Entity<Resource>(r =>
            {
                r.Property(r => r.ResourceId)
                .ValueGeneratedOnAdd();

                r.Property(r => r.Name)
                .IsUnicode(true);

                r.Property(r => r.Url)
                .IsUnicode(false);

                r.HasCheckConstraint("CHK_ResourceType", "ResourceType IN ('Video', 'Presentation', 'Document')");

                r.HasOne(r => r.Course)
                .WithMany(c => c.Resources)
                .HasForeignKey(r => r.CourseId)
                .OnDelete(DeleteBehavior.Restrict);
            });

            modelBuilder.Entity<StudentCourse>(Sc =>
            {
                Sc.HasKey(sc => new { sc.CourseId, sc.StudentId });

                Sc.HasOne(Sc => Sc.Student)
                .WithMany(s => s.CourseEnrollments)
                .HasForeignKey(s => s.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

                Sc.HasOne(sc => sc.Course)
                .WithMany(x => x.StudentsEnrolled)
                .HasForeignKey(x => x.CourseId)
                .OnDelete(DeleteBehavior.Restrict);
            });
            base.OnModelCreating(modelBuilder);
        }

    }
}
