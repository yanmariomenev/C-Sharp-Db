using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;
using P01_StudentSystem.Data.Models;

namespace P01_StudentSystem.Data
{
   public class StudentSystemContext : DbContext
    {
        public StudentSystemContext() { }

        public StudentSystemContext(DbContextOptions options) : base(options) { }

        public DbSet<Course> Courses { get; set; }
        public DbSet<Homework> HomeworkSubmissions { get; set; }
        public DbSet<Resource> Resources { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<StudentCourse> StudentCourses { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(ServerConfiguration.ConnectionString);
            }
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Student>(e =>
            {
                e.HasKey(e => e.StudentId);

                e.Property(s => s.Name)
                    .HasMaxLength(100)
                    .IsRequired()
                    .IsUnicode();

                e.Property(s => s.PhoneNumber)
                    .HasMaxLength(10)
                    .IsRequired(false)
                    .IsUnicode(false)
                    .IsFixedLength(true);

                e.Property(s => s.RegisteredOn)
                    .IsRequired();

                e.Property(s => s.Birthday)
                    .IsRequired(false);

            });

            modelBuilder.Entity<Course>(e =>
            {
                e.HasKey(e => e.CourseId);

                e.Property(c => c.Name)
                    .HasMaxLength(50)
                    .IsRequired()
                    .IsUnicode();

                e.Property(c => c.Description)
                    .HasMaxLength(250)
                    .IsRequired(false)
                    .IsUnicode();

                e.Property(c => c.StartDate)
                    .IsRequired();
                e.Property(c => c.EndDate)
                    .IsRequired();

                e.Property(c => c.Price)
                    .IsRequired();


            });

            modelBuilder.Entity<Resource>(e =>
            {
                e.HasKey(r => r.ResourceId);

                e.Property(r => r.Name)
                    .HasMaxLength(50)
                    .IsRequired()
                    .IsUnicode();

                e.Property(r => r.Url)
                    .HasMaxLength(300)
                    .IsRequired()
                    .IsUnicode(false);

                e.Property(r => r.ResourceType)
                    .IsRequired();

                e.HasOne(c => c.Course)
                    .WithMany(r => r.Resources)
                    .HasForeignKey(c => c.CourseId)
                    .OnDelete(DeleteBehavior.Restrict);

            });

            modelBuilder.Entity<Homework>(e =>
            {
                e.HasKey(h => h.HomeworkId);

                e.Property(h => h.Content)
                    .HasMaxLength(250)
                    .IsRequired()
                    .IsUnicode(false);

                e.Property(h => h.ContentType)
                    .IsRequired();

                e.Property(h => h.SubmissionTime)
                    .IsRequired();

                e.HasOne(s => s.Student)
                    .WithMany(h => h.HomeworkSubmissions)
                    .HasForeignKey(s => s.StudentId)
                    .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(c => c.Course)
                    .WithMany(h => h.HomeworkSubmissions)
                    .HasForeignKey(c => c.CourseId)
                    .OnDelete(DeleteBehavior.Restrict);

            });

            modelBuilder.Entity<StudentCourse>(e =>
            {
                e.HasKey(e => new {e.CourseId, e.StudentId});

                e.HasOne(s => s.Student)
                    .WithMany(c => c.CourseEnrollments)
                    .HasForeignKey(s => s.StudentId)
                    .OnDelete(DeleteBehavior.Restrict);

                e.HasOne(e => e.Course)
                    .WithMany(c => c.StudentsEnrolled)
                    .HasForeignKey(e => e.CourseId)
                    .OnDelete(DeleteBehavior.Restrict);
            });

        }
    }
}
