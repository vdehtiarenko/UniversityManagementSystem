using UniversityManagementSystem.DAL.Entities;
using UniversityManagementSystem.DAL;
using UniversityManagementSystem.Services.ServicesInterfaces;

namespace UniversityManagementSystem.Services
{
    public class StudentService : IStudentService
    {
        private ApplicationDBContext _dbContext;

        public StudentService(ApplicationDBContext dbContext) 
        {
            _dbContext = dbContext;
        }

        public Student GetOrCreate(string firstName, string lastName, Guid groupId)
        {
            if (string.IsNullOrEmpty(firstName))
            {
                throw new ArgumentException("The first name cannot be empty.");
            }

            if (string.IsNullOrEmpty(lastName))
            {
                throw new ArgumentException("The last name cannot be empty.");
            }

            if (groupId == Guid.Empty)
            {
                throw new ArgumentException("The group ID cannot be empty.");
            }

            var existingStudent = _dbContext.Students
                      .FirstOrDefault(s => string.Equals(s.FirstName.Trim(), firstName.Trim())
                      && string.Equals(s.LastName.Trim(), lastName.Trim()) 
                      && s.GroupId == groupId);

            if ( existingStudent != null)
            {
                return existingStudent;
            }

            var id = Guid.NewGuid();

            var student = new Student(firstName, lastName, groupId, id);

            _dbContext.Students.Add(student);

            _dbContext.SaveChanges();

            return student;
        }

        public Student UpdateStudent(Guid studentId, string firstName, string lastName)
        {
            if (studentId == Guid.Empty)
            {
                throw new ArgumentException("The student ID cannot be empty.");
            }

            if (string.IsNullOrEmpty(firstName))
            {
                throw new ArgumentException("The first name cannot be empty.");
            }

            if (string.IsNullOrEmpty(lastName))
            {
                throw new ArgumentException("The last name cannot be empty.");
            }

            var existingStudent = _dbContext.Students.FirstOrDefault(s => s.StudentId == studentId);

            if (existingStudent == null)
            {
                throw new InvalidOperationException("The student with the specified ID does not exist.");
            }

            existingStudent.FirstName = firstName
                                        ?? throw new ArgumentException("First name cannot be empty.");

            existingStudent.LastName = lastName
                                       ?? throw new ArgumentNullException("Last name cannot be empty.");
            _dbContext.SaveChanges();

            return existingStudent;
        }

        public void DeleteStudent(Guid studentId)
        {
            if (studentId == Guid.Empty)
            {
                throw new ArgumentException("The student ID cannot be empty.");
            }

            var existingStudent = _dbContext.Students.FirstOrDefault(s => s.StudentId == studentId);

            if (existingStudent == null)
            {
                throw new InvalidOperationException("The student with the specified ID does not exist.");
            }

            _dbContext.Students.Remove(existingStudent);

            _dbContext.SaveChanges();
        }
    }
}
