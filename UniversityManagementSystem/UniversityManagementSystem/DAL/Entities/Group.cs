using System.Collections.ObjectModel;
using UniversityManagementSystem.DAL.Entities;

public class Group
{
    public Guid GroupId { get; set; }

    public string Name { get; set; }

    public Guid CourseId { get; set; }

    public virtual ICollection<Student> Students { get; set; }
    public virtual ICollection<GroupCourseTeacher> GroupCourseTeachers { get; set; }

    public Group()
    {
        Students = new List<Student>();
        GroupCourseTeachers = new List<GroupCourseTeacher>();
    }

    public Group(Guid id, string name, Guid courseId) 
    {
        GroupId = id;
        Name = name;
        CourseId = courseId;
    }

    public Group(Group other)
    {
        GroupId = other.GroupId;
        Name = other.Name;
        Students = new ObservableCollection<Student>(other.Students);
        GroupCourseTeachers = new ObservableCollection<GroupCourseTeacher>(other.GroupCourseTeachers);
    }

}
