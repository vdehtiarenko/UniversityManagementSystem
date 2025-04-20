namespace UniversityManagementSystem.DAL.Entities
{
    public class Student
    {
        public Guid StudentId { get; set; }

        public Guid GroupId { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public virtual Group Group { get; set; }

        public Student(string firstName, string lastName, Guid groupId, Guid studentId)
        {
            FirstName = firstName;
            LastName = lastName;
            GroupId = groupId;
            StudentId = studentId;
        }
    }

}
