using UniversityManagementSystem.DAL.Entities;

namespace UniversityManagementSystem.Services.ServicesInterfaces
{
    public interface IStudentService
    {
        public Student GetOrCreate(string firstName, string lastName, Guid groupId);

        public Student UpdateStudent(Guid studentId, string firstName, string lastName);

        public void DeleteStudent(Guid studentId);
    }
}
