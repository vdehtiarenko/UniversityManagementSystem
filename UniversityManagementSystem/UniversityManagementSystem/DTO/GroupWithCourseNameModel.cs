using UniversityManagementSystem.Services.ServicesInterfaces;

namespace UniversityManagementSystem.DTO
{
    public class GroupWithCourseNameModel
    {
        public string CourseName { get; private set; }
        public Group Group { get; private set; }

        public GroupWithCourseNameModel(Group group, ICourseService courseService)
        {
            Group = group;

            var course = courseService.GetCourseById(group.CourseId);
            CourseName = course.Name;
        }
    }
}
