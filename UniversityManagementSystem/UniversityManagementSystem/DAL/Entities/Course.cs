namespace UniversityManagementSystem.DAL.Entities
{
    public class Course
    {
        public Guid CourseId { get; set; }

        public string Name { get; set; }

        public string? Description { get; set; }

        public virtual ICollection<GroupCourseTeacher> GroupCourseTeachers { get; set; }

        public Course(Guid courseId, string name, string? description)
        {
            CourseId = courseId;
            Name = name;
            Description = description;

            GroupCourseTeachers = new List<GroupCourseTeacher>();
        }
    }
}

