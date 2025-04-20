using Microsoft.EntityFrameworkCore;
using UniversityManagementSystem.DAL.Entities;
using UniversityManagementSystem.DAL;
using UniversityManagementSystem.Services.ServicesInterfaces;

namespace UniversityManagementSystem.Services
{
    public class CourseService : ICourseService
    {
        private readonly ApplicationDBContext _dbContext;

        public CourseService(ApplicationDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Course GetOrCreate(string name, string description = null)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException("The course name cannot be empty.");
            }

            var existingCourse = _dbContext.Courses
                .FirstOrDefault(c => string.Equals(c.Name.Trim().ToUpper(), name.Trim().ToUpper()));

            if (existingCourse != null)
            {
                return existingCourse;
            }

            var id = Guid.NewGuid();

            var course = new Course(id, name, description);

            _dbContext.Courses.Add(course);
            _dbContext.SaveChanges();

            return course;
        }

        public List<Course> GetAllCourses()
        {
            return _dbContext.Courses
                             .AsNoTracking()
                             .Include(c => c.GroupCourseTeachers)
                             .ThenInclude(gct => gct.Group)
                             .ToList();
        }

        public Course GetCourseById(Guid courseId)
        {
            var course = _dbContext.Courses
                                   .FirstOrDefault(c => c.CourseId == courseId);

            if (course == null)
            {
                throw new KeyNotFoundException($"Course with the specified ID not exist.");
            }

            return course;
        }
    }
}
