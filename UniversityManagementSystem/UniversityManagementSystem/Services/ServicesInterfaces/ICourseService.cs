using UniversityManagementSystem.DAL.Entities;

namespace UniversityManagementSystem.Services.ServicesInterfaces
{
    public interface ICourseService
    {
        public Course GetOrCreate(string name, string description = null);

        public List<Course> GetAllCourses();

        public Course GetCourseById(Guid courseId);
    }
}
