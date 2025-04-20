using System.Collections.ObjectModel;
using UniversityManagementSystem.DAL.Entities;

namespace UniversityManagementSystem.Services.ServicesInterfaces
{
    public interface IGroupService
    {
        public Group GetOrCreate(string name, Guid courseId, List<Guid> teachersId);

        public ObservableCollection<Student> GetStudentsByGroup(Guid groupId);

        public ObservableCollection<Teacher> GetTeachersByGroup(Guid groupId);

        public ObservableCollection<Group> GetCourseGroups(Guid courseId);

        public Group GetGroupById(Guid courseId, Guid groupId);

        public Group UpdateGroup(Guid groupId, string name, List<Guid> teachersId);

        public void DeleteGroup(Guid groupId);

        public void ClearStudentsFromGroup(Guid groupId);
    }
}
