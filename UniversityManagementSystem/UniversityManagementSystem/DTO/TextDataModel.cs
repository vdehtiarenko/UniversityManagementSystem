using UniversityManagementSystem.DAL.Entities;

namespace UniversityManagementSystem.DTO
{
    public class TextDataModel
    {
        public Course Course { get; set; }
        public Group Group { get; set; }
        public List<Student> Students { get; set; }

        public TextDataModel(Course course, Group group, List<Student> groupStudents)
        {
            Course = course;
            Group = group;
            Students = groupStudents;
        }
    }
}
