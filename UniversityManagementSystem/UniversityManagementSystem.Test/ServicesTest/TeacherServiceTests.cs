using Moq;
using Microsoft.EntityFrameworkCore;
using UniversityManagementSystem.DAL;
using UniversityManagementSystem.DAL.Entities;
using UniversityManagementSystem.Services;

public class TeacherServiceTests
{
    private readonly Mock<DbSet<Teacher>> _mockTeacherDbSet;
    private readonly Mock<DbSet<GroupCourseTeacher>> _mockGroupCourseTeacherDbSet;
    private readonly Mock<ApplicationDBContext> _mockDbContext;
    private readonly TeacherService _teacherService;

    public TeacherServiceTests()
    {
        var teachers = new List<Teacher>
        {
            new Teacher("John", "Doe", new Guid("fbd47fd5-6d67-46c0-9509-43e991ee1de6")),
            new Teacher("Jane", "Smith", new Guid("bd7db7a9-fc49-48ac-96c5-c35c87bbd086"))
        };

        var groupCourseTeachers = new List<GroupCourseTeacher>
        {
            new GroupCourseTeacher { TeacherId = new Guid("fbd47fd5-6d67-46c0-9509-43e991ee1de6"), GroupId = new Guid("d65f5c34-3c26-4f6f-bc93-cd0353fa7f79"), CourseId = new Guid("7d465dc4-28fa-4f0b-9a3b-5eafca4d4c1e") }
        };

        _mockTeacherDbSet = CreateMockDbSet<Teacher>(teachers);
        _mockGroupCourseTeacherDbSet = CreateMockDbSet<GroupCourseTeacher>(groupCourseTeachers);

        _mockDbContext = new Mock<ApplicationDBContext>();
        _mockDbContext.Setup(db => db.Set<Teacher>()).Returns(_mockTeacherDbSet.Object);
        _mockDbContext.Setup(db => db.Set<GroupCourseTeacher>()).Returns(_mockGroupCourseTeacherDbSet.Object);

        _teacherService = new TeacherService(_mockDbContext.Object);
    }

    [Fact]
    public void Create_CorrectlyThrowsArgumentExceptionWhenFirstNameIsEmpty()
    {
        // Arrange

        string firstName = null;
        string lastName = "Doe";

        // Act

        var exception = Assert.Throws<ArgumentException>(() => _teacherService.Create(firstName, lastName));

        // Assert

        Assert.IsType<ArgumentException>(exception);
        Assert.Equal("The first name cannot be empty.", exception.Message);
    }

    [Fact]
    public void Create_CorrectlyThrowsArgumentExceptionWhenLastNameIsEmpty()
    {
        // Arrange

        string firstName = "John";
        string lastName = null;

        // Act

        var exception = Assert.Throws<ArgumentException>(() => _teacherService.Create(firstName, lastName));

        // Assert

        Assert.IsType<ArgumentException>(exception);
        Assert.Equal("The last name cannot be empty.", exception.Message);
    }

    [Fact]
    public void UpdateTeacher_CorrectlyThrowsArgumentExceptionWhenTeacherIdIsEmpty()
    {
        // Arrange

        Guid teacherId = Guid.Empty;
        string firstName = "John";
        string lastName = "Doe";

        // Act

        var exception = Assert.Throws<ArgumentException>(() => _teacherService.UpdateTeacher(teacherId, firstName, lastName));

        // Assert

        Assert.IsType<ArgumentException>(exception);
        Assert.Equal("The teacher ID cannot be empty.", exception.Message);
    }

    [Fact]
    public void UpdateTeacher_CorrectlyThrowsArgumentExceptionWhenFirstNameIsEmpty()
    {
        // Arrange

        Guid teacherId = new Guid("fbd47fd5-6d67-46c0-9509-43e991ee1de6");
        string firstName = null;
        string lastName = "Doe";

        // Act

        var exception = Assert.Throws<ArgumentException>(() => _teacherService.UpdateTeacher(teacherId, firstName, lastName));

        // Assert

        Assert.IsType<ArgumentException>(exception);
        Assert.Equal("The first name cannot be empty.", exception.Message);
    }

    [Fact]
    public void UpdateTeacher_CorrectlyThrowsArgumentExceptionWhenLastNameIsEmpty()
    {
        // Arrange

        Guid teacherId = new Guid("fbd47fd5-6d67-46c0-9509-43e991ee1de6");
        string firstName = "John";
        string lastName = null;

        // Act

        var exception = Assert.Throws<ArgumentException>(() => _teacherService.UpdateTeacher(teacherId, firstName, lastName));

        // Assert

        Assert.IsType<ArgumentException>(exception);
        Assert.Equal("The last name cannot be empty.", exception.Message);
    }

    [Fact]
    public void UpdateTeacherGroups_CorrectlyThrowsArgumentExceptionWhenTeacherIdIsEmpty()
    {
        // Arrange

        Guid teacherId = Guid.Empty;
        var newGroupCourseTeachers = new List<GroupCourseTeacher>();

        // Act

        var exception = Assert.Throws<ArgumentException>(() => _teacherService.UpdateTeacherGroups(teacherId, newGroupCourseTeachers));

        // Assert

        Assert.IsType<ArgumentException>(exception);
        Assert.Equal("Teacher ID must not be empty.", exception.Message);
    }

    [Fact]
    public void UpdateTeacherGroups_CorrectlyThrowsArgumentNullExceptionWhenNewGroupCourseTeachersIsNull()
    {
        // Arrange

        Guid teacherId = new Guid("fbd47fd5-6d67-46c0-9509-43e991ee1de6");

        // Act

        var exception = Assert.Throws<ArgumentException>(() => _teacherService.UpdateTeacherGroups(teacherId, null));

        // Assert

        Assert.IsType<ArgumentException>(exception);
        Assert.Equal("The list of group-course-teacher entries must not be empty.", exception.Message);
    }

    [Fact]
    public void DeleteTeacher_CorrectlyThrowsArgumentExceptionWhenTeacherIdIsEmpty()
    {
        // Arrange

        Guid teacherId = Guid.Empty;

        // Act

        var exception = Assert.Throws<ArgumentException>(() => _teacherService.DeleteTeacher(teacherId));

        // Assert

        Assert.IsType<ArgumentException>(exception);
        Assert.Equal("The teacher ID cannot be empty.", exception.Message);
    }

    private Mock<DbSet<T>> CreateMockDbSet<T>(IList<T> sourceList) where T : class
    {
        var queryable = sourceList.AsQueryable();

        var mockSet = new Mock<DbSet<T>>();
        mockSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(queryable.Provider);
        mockSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryable.Expression);
        mockSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
        mockSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(queryable.GetEnumerator());

        mockSet.Setup(d => d.Add(It.IsAny<T>())).Callback<T>(sourceList.Add);
        mockSet.Setup(d => d.Remove(It.IsAny<T>())).Callback<T>(item => sourceList.Remove(item));

        return mockSet;
    }
}
