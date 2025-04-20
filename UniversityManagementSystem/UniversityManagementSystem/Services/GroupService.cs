using Microsoft.EntityFrameworkCore;
using System.Collections.ObjectModel;
using UniversityManagementSystem.DAL.Entities;
using UniversityManagementSystem.DAL;
using UniversityManagementSystem.Services.ServicesInterfaces;

namespace UniversityManagementSystem.Services
{
    public class GroupService : IGroupService
    {
        private readonly ApplicationDBContext _dbContext;

        public GroupService(ApplicationDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public Group GetOrCreate(string name, Guid courseId, List<Guid> teachersId)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("The group name cannot be empty.");
            }

            if (courseId == Guid.Empty)
            {
                throw new ArgumentException("The course ID cannot be empty.");
            }

            if (teachersId == null || teachersId.Count == 0)
            {
                throw new ArgumentException("At least one teacher ID must be provided.");
            }

            var existingGroup = _dbContext.Groups
                .FirstOrDefault(g => string.Equals(g.Name.Trim().ToUpper(), name.Trim().ToUpper()));

            if (existingGroup != null)
            {
                return existingGroup;
            }

            var existingCourse = _dbContext.Courses
                .FirstOrDefault(c => c.CourseId == courseId);

            if (existingCourse == null)
            {
                throw new ArgumentException("The specified course does not exist.");
            }

            foreach (var teacherId in teachersId)
            {
                if (teacherId == Guid.Empty)
                {
                    throw new ArgumentException("Teacher ID cannot be empty.");
                }

                var existingTeacher = _dbContext.Teachers
                    .FirstOrDefault(t => t.TeacherId == teacherId);

                if (existingTeacher == null)
                {
                    throw new ArgumentException($"Teacher with ID {teacherId} does not exist.");
                }
            }

            Guid id = Guid.NewGuid();

            var group = new Group(id, name, courseId);

            _dbContext.Groups.Add(group);

            foreach (var teacherId in teachersId)
            {
                var groupCourseTeachers = new GroupCourseTeacher(id, courseId, teacherId);
                _dbContext.GroupCourseTeachers.Add(groupCourseTeachers);
            }

            _dbContext.SaveChanges();

            return group;
        }

        public ObservableCollection<Student> GetStudentsByGroup(Guid groupId)
        {
            var students = _dbContext.Students
                                      .AsNoTracking()
                                      .Where(s => s.GroupId == groupId)
                                      .ToList();

            return new ObservableCollection<Student>(students);
        }

        public ObservableCollection<Teacher> GetTeachersByGroup(Guid groupId)
        {
            var teachers = (from gct in _dbContext.GroupCourseTeachers
                            where gct.GroupId == groupId && gct.TeacherId != null
                            select gct.Teacher)
                            .AsNoTracking()
                            .Distinct()
                            .ToList();

            return new ObservableCollection<Teacher>(teachers);
        }

        public ObservableCollection<Group> GetCourseGroups(Guid courseId)
        {
            var groups = _dbContext.Groups
                .AsNoTracking()
                .Where(g => g.CourseId == courseId)
                .ToList();

            return new ObservableCollection<Group>(groups);
        }

        public Group GetGroupById(Guid courseId, Guid groupId)
        {
            var groupCourseTeacher = _dbContext.GroupCourseTeachers
                                               .AsNoTracking()
                                               .FirstOrDefault(gct => gct.CourseId == courseId && gct.GroupId == groupId);

            return groupCourseTeacher?.Group;
        }

        public Group UpdateGroup(Guid groupId, string name, List<Guid> teachersId)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentException("The group name cannot be empty.");
            }

            if (groupId == Guid.Empty)
            {
                throw new ArgumentException("The group ID cannot be empty.");
            }

            if (teachersId == null || teachersId.Count == 0)
            {
                throw new ArgumentException("At least one teacher ID must be provided.");
            }

            var existingGroup = _dbContext.Groups.FirstOrDefault(g => g.GroupId == groupId);

            if (existingGroup == null)
                throw new ArgumentException("The group cannot be null.");

            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("The group name cannot be empty.");

            foreach (var teacherId in teachersId)
            {
                bool existingTeacher = _dbContext.Teachers.Any(t => t.TeacherId == teacherId);

                if (!existingTeacher)
                {
                    throw new InvalidOperationException("The teacher does not exist.");
                }              
            }
            
            existingGroup.Name = name;

            var oldGroupCourseTeachers = _dbContext.GroupCourseTeachers
                .Where(gct => gct.GroupId == groupId)
                .ToList();

            _dbContext.GroupCourseTeachers.RemoveRange(oldGroupCourseTeachers);

            foreach (var teacherId in teachersId)
            {
                var courseId = existingGroup.GroupCourseTeachers?.FirstOrDefault()?.CourseId
                               ?? oldGroupCourseTeachers.FirstOrDefault()?.CourseId
                               ?? Guid.Empty;

                var groupCourseTeacher = new GroupCourseTeacher(groupId, courseId, teacherId);
                _dbContext.GroupCourseTeachers.Add(groupCourseTeacher);
            }

            _dbContext.Groups.Update(existingGroup);

            _dbContext.SaveChanges();

            return existingGroup;
        }

        public void DeleteGroup(Guid groupId)
        {
            if (groupId == Guid.Empty)
            {
                throw new ArgumentException("The group ID cannot be empty.");
            }

            var existingGroup = _dbContext.Groups.FirstOrDefault(g => g.GroupId == groupId);

            if (existingGroup == null)
                throw new ArgumentException("The group cannot be null.");

            var groupStudents = _dbContext.Students
               .Where(s => s.GroupId == groupId)
               .ToList();

            if (groupStudents.Count > 0)
            {
                throw new InvalidOperationException("The group already has assigned students.");
            }

            var groupCourseTeachers = _dbContext.GroupCourseTeachers
                .Where(gct => gct.GroupId == groupId)
                .ToList();

            _dbContext.GroupCourseTeachers.RemoveRange(groupCourseTeachers);

            _dbContext.Groups.Remove(existingGroup);

            _dbContext.SaveChanges();
        }

        public void ClearStudentsFromGroup(Guid groupId)
        {
            if (groupId == Guid.Empty)
                throw new ArgumentException("Invalid group ID.", nameof(groupId));

            var studentsToDelete = _dbContext.Students
                                              .Where(s => s.GroupId == groupId)
                                              .ToList();
            if (studentsToDelete.Any())
            {
                _dbContext.Students.RemoveRange(studentsToDelete);

                _dbContext.SaveChanges();
            }
        }
    }
}
