using UniversityManagementSystem.DAL.Entities;

public class GroupCourseTeacher
{
    public Guid GroupId { get; set; }
    public Guid CourseId { get; set; }
    public Guid? TeacherId { get; set; } 

    public virtual Group Group { get; set; }
    public virtual Course Course { get; set; }
    public virtual Teacher Teacher { get; set; }

    public GroupCourseTeacher() { }

    public GroupCourseTeacher(Guid groupId, Guid courseId, Guid? teacherId = null)
    {
        GroupId = groupId;
        CourseId = courseId;
        TeacherId = teacherId;
    }
}