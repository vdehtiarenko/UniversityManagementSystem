namespace UniversityManagementSystem.DAL.Entities
{
    public class Teacher
    {
        public Guid TeacherId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public Teacher() { }

        public Teacher(string firstName, string lastName, Guid teacherId)
        {
            FirstName = firstName;
            LastName = lastName;
            TeacherId = teacherId;
        }

        public virtual ICollection<GroupCourseTeacher> GroupCourseTeachers { get; set; }
    }
}
