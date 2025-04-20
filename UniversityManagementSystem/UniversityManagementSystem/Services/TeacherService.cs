using Microsoft.EntityFrameworkCore;
using UniversityManagementSystem.DAL.Entities;
using UniversityManagementSystem.DAL;
using UniversityManagementSystem.Services.ServicesInterfaces;

namespace UniversityManagementSystem.Services
{
    public class TeacherService : ITeacherService
    {
        private readonly ApplicationDBContext _dbContext;

        public TeacherService(ApplicationDBContext dBContext) 
        {
            _dbContext = dBContext;
        }

        public Teacher GetById(Guid teacherId)
        {
            return _dbContext.Teachers.FirstOrDefault(t => t.TeacherId == teacherId)
                ?? throw new InvalidOperationException($"Teacher with ID {teacherId} not found.");
        }

        public Teacher Create(string firstName, string lastName)
        {
            if (string.IsNullOrEmpty(firstName))
            {
                throw new ArgumentException("The first name cannot be empty.");
            }

            if (string.IsNullOrEmpty(lastName))
            {
                throw new ArgumentException("The last name cannot be empty.");
            }

            var id = Guid.NewGuid();

            var teacher = new Teacher(firstName.Trim(), lastName.Trim(), id);

            _dbContext.Teachers.Add(teacher);
            _dbContext.SaveChanges();

            return teacher;
        }

        public void AddGroupsToTeacher(List<GroupCourseTeacher> groupCourseTeachers)
        {
            if (groupCourseTeachers == null || !groupCourseTeachers.Any())
            {
                throw new ArgumentException("The list of group-course-teacher entries must not be null or empty.");
            }

            foreach (var gct in groupCourseTeachers)
            {
                if (gct.TeacherId == Guid.Empty || gct.GroupId == Guid.Empty || gct.CourseId == Guid.Empty)
                {
                    throw new ArgumentException("Each GroupCourseTeacher must have valid IDs for Teacher, Group, and Course.");
                }

                var existingEntry = _dbContext.GroupCourseTeachers.FirstOrDefault(e =>
                    e.TeacherId == gct.TeacherId &&
                    e.GroupId == gct.GroupId &&
                    e.CourseId == gct.CourseId);

                if (existingEntry == null)
                {
                    _dbContext.GroupCourseTeachers.Add(gct);
                }
            }

            _dbContext.SaveChanges();
        }

        public List<Teacher> GetAllTeachers()
        {
            return _dbContext.Teachers
                .AsNoTracking()
                .ToList();
        }

        public List<Group> GetAllGroups()
        {
            return _dbContext.Groups.ToList();
        }

        public List<Group> GetGroupsInWhichTheTeacherTeach(Guid teacherId)
        {
            var existingTeacher = _dbContext.Teachers.FirstOrDefault(t => t.TeacherId == teacherId);

            if (existingTeacher == null)
            {
                throw new ArgumentNullException("The teacher with the specified ID does not exist.");
            }

            var groupCourseTeachers = _dbContext.GroupCourseTeachers
                .Where(gct => gct.TeacherId == teacherId)
                .ToList();

            var groupsInWhichTheTeacherTeach = new List<Group>();

            foreach (var group in groupCourseTeachers)
            {
                var groupInWhichTheTeacherTeach = _dbContext.Groups.FirstOrDefault(g => g.GroupId == group.GroupId && g.CourseId == group.CourseId);
                groupsInWhichTheTeacherTeach.Add(groupInWhichTheTeacherTeach);
            }

            return groupsInWhichTheTeacherTeach;
        }

        public Teacher UpdateTeacher(Guid teacherId, string firstName, string lastName)
        {
            if (teacherId == Guid.Empty)
            {
                throw new ArgumentException("The teacher ID cannot be empty.");
            }

            if (string.IsNullOrEmpty(firstName))
            {
                throw new ArgumentException("The first name cannot be empty.");
            }

            if (string.IsNullOrEmpty(lastName))
            {
                throw new ArgumentException("The last name cannot be empty.");
            }

            var existingTeacher = _dbContext.Teachers.FirstOrDefault(t => t.TeacherId == teacherId);

            if (existingTeacher == null)
            {
                throw new ArgumentNullException("The teacher with the specified ID does not exist.");
            }

            existingTeacher.FirstName = firstName;
            existingTeacher.LastName = lastName;

            _dbContext.SaveChanges();

            return existingTeacher;
        }

        public void UpdateTeacherGroups(Guid teacherId, List<GroupCourseTeacher> newGroupCourseTeachers)
        {
            if (teacherId == Guid.Empty)
                throw new ArgumentException("Teacher ID must not be empty.");

            if (newGroupCourseTeachers == null)
                throw new ArgumentException("The list of group-course-teacher entries must not be empty.");

            var existingEntries = _dbContext.GroupCourseTeachers
                .Where(gct => gct.TeacherId == teacherId)
                .ToList();

            var entriesToRemove = existingEntries
                .Where(existing => !newGroupCourseTeachers.Any(newEntry =>
                    newEntry.GroupId == existing.GroupId &&
                    newEntry.CourseId == existing.CourseId))
                .ToList();

            var entriesToAdd = newGroupCourseTeachers
                .Where(newEntry => !existingEntries.Any(existing =>
                    existing.GroupId == newEntry.GroupId &&
                    existing.CourseId == newEntry.CourseId))
                .ToList();

            if (entriesToRemove.Any())
                _dbContext.GroupCourseTeachers.RemoveRange(entriesToRemove);

            foreach (var gct in entriesToAdd)
            {
                if (gct.TeacherId != teacherId)
                    throw new ArgumentException("All GroupCourseTeacher entries must have the same TeacherId as the provided teacherId.");

                if (gct.GroupId == Guid.Empty || gct.CourseId == Guid.Empty)
                    throw new ArgumentException("Each GroupCourseTeacher must have valid IDs for Group and Course.");

                _dbContext.GroupCourseTeachers.Add(gct);
            }

            _dbContext.SaveChanges();
        }

        public void DeleteTeacher(Guid teacherId)
        {
            if (teacherId == Guid.Empty)
            {
                throw new ArgumentException("The teacher ID cannot be empty.");
            }

            var existingTeacher = _dbContext.Teachers.FirstOrDefault(t => t.TeacherId == teacherId);

            if (existingTeacher == null)
            {
                throw new ArgumentException("The teacher with the specified ID does not exist.");
            }

            var relatedEntries = _dbContext.GroupCourseTeachers
                .Where(gct => gct.TeacherId == existingTeacher.TeacherId)
                .ToList();

            if (relatedEntries.Any())
            {
                _dbContext.GroupCourseTeachers.RemoveRange(relatedEntries);
            }

            _dbContext.Teachers.Remove(existingTeacher);

            _dbContext.SaveChanges();
        }
    }
}
