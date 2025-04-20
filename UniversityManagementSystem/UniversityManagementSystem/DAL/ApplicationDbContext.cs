using Microsoft.EntityFrameworkCore;
using UniversityManagementSystem.DAL.Entities;

namespace UniversityManagementSystem.DAL
{
    public class ApplicationDBContext : DbContext
    {
        public virtual DbSet<Course> Courses { get; set; }
        public virtual DbSet<Group> Groups { get; set; }
        public virtual DbSet<Student> Students { get; set; }
        public virtual DbSet<Teacher> Teachers { get; set; }
        public virtual DbSet<GroupCourseTeacher> GroupCourseTeachers { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<GroupCourseTeacher>()
                .HasKey(gct => new { gct.GroupId, gct.CourseId, gct.TeacherId });

            modelBuilder.Entity<GroupCourseTeacher>()
                .HasOne(gct => gct.Group)
                .WithMany(g => g.GroupCourseTeachers)
                .HasForeignKey(gct => gct.GroupId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<GroupCourseTeacher>()
                .HasOne(gct => gct.Course)
                .WithMany(c => c.GroupCourseTeachers)
                .HasForeignKey(gct => gct.CourseId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<GroupCourseTeacher>()
                .HasOne(gct => gct.Teacher)
                .WithMany(t => t.GroupCourseTeachers)
                .HasForeignKey(gct => gct.TeacherId)
                .OnDelete(DeleteBehavior.Restrict);
        }

        public ApplicationDBContext(DbContextOptions<ApplicationDBContext> options)
           : base(options)
        {
        }

        public ApplicationDBContext() : base(new DbContextOptions<ApplicationDBContext>()) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Data Source=localhost;Initial Catalog=University;Persist Security Info=True;User ID=User1;Password=sa;Trust Server Certificate=True");
        }
    }
}