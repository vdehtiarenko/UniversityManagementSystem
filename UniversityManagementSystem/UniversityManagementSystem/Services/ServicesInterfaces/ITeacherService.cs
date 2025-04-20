using UniversityManagementSystem.DAL.Entities;

namespace UniversityManagementSystem.Services.ServicesInterfaces
{
    public interface ITeacherService
    {
        public Teacher GetById(Guid teacherId);

        public Teacher Create(string firstName, string lastName);

        public void AddGroupsToTeacher(List<GroupCourseTeacher> groupCourseTeachers);

        public List<Teacher> GetAllTeachers();

        public List<Group> GetAllGroups();

        public List<Group> GetGroupsInWhichTheTeacherTeach(Guid teacherId);

        public Teacher UpdateTeacher(Guid teacherId, string firstName, string lastName);

        public void UpdateTeacherGroups(Guid teacherId, List<GroupCourseTeacher> newGroupCourseTeachers);

        public void DeleteTeacher(Guid teacherId);
    }
}
