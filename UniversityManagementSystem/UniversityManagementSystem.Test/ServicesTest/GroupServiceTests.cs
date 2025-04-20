using Moq;
using Microsoft.EntityFrameworkCore;
using UniversityManagementSystem.DAL;
using UniversityManagementSystem.DAL.Entities;
using UniversityManagementSystem.Services;

public class GroupServiceTests
{
    private readonly Mock<DbSet<Group>> _mockGroupDbSet;
    private readonly Mock<DbSet<Course>> _mockCourseDbSet;
    private readonly Mock<DbSet<Teacher>> _mockTeacherDbSet;
    private readonly Mock<ApplicationDBContext> _mockDbContext;
    private readonly GroupService _groupService;

    public GroupServiceTests()
    {
        var groups = new List<Group>
        {
            new Group(new Guid("fbd47fd5-6d67-46c0-9509-43e991ee1de6"), "Group 1", new Guid("6957368b-f547-4166-b727-00fad69b77a3")),
            new Group(new Guid("bd7db7a9-fc49-48ac-96c5-c35c87bbd086"), "Group 2", new Guid("f2b52c71-50b2-467f-a77e-473399376d10")),
            new Group(new Guid("663c3da9-6ae8-4f10-9f8e-b77f356e0c83"), "Group 3", new Guid("35ee778c-df64-4cb7-ba9b-c2b9982d897d"))
        };

        var courses = new List<Course>
        {
            new Course(new Guid("6957368b-f547-4166-b727-00fad69b77a3"), "Course 1", "Description"),
            new Course(new Guid("f2b52c71-50b2-467f-a77e-473399376d10"), "Course 2", "Description"),
            new Course(new Guid("35ee778c-df64-4cb7-ba9b-c2b9982d897d"), "Course 3", "Description"),
        };

        var teachers = new List<Teacher>
        {
            new Teacher("James", "Anderson", new Guid("ef9ec992-44d4-4897-af33-aae9968e1747")),
            new Teacher("Emily", "Johnson", new Guid("2d0f9ce7-9d2a-441b-9c21-9f6867a1179e")),
            new Teacher("Michael", "Roberts", new Guid("a9d650dd-6a96-4b30-8786-ab4dfa38788b"))
        };

        _mockGroupDbSet = CreateMockDbSet<Group>(groups);

        _mockCourseDbSet = CreateMockDbSet<Course>(courses);

        _mockTeacherDbSet = CreateMockDbSet<Teacher>(teachers);

        _mockDbContext = new Mock<ApplicationDBContext>();
        _mockDbContext.Setup(db => db.Set<Group>()).Returns(_mockGroupDbSet.Object);
        _mockDbContext.Setup(db => db.Set<Course>()).Returns(_mockCourseDbSet.Object);
        _mockDbContext.Setup(db => db.Set<Teacher>()).Returns(_mockTeacherDbSet.Object);

        _groupService = new GroupService(_mockDbContext.Object);
    }

    [Fact]
    public void GetOrCreate_CorrectlyThrowsArgumentExceptionWhenNameIsEmpty()
    {
        // Arrange

        string name = null;
        Guid courseId = new Guid("6957368b-f547-4166-b727-00fad69b77a3");
        var teachersId = new List<Guid> { Guid.NewGuid() };

        // Act 

        var exception = Assert.Throws<ArgumentException>(() => _groupService.GetOrCreate(name, courseId, teachersId));

        // Assert

        Assert.IsType<ArgumentException>(exception);
        Assert.Equal("The group name cannot be empty.", exception.Message);
    }

    [Fact]
    public void GetOrCreate_CorrectlyThrowsArgumentExceptionWhenCourseIdIsEmpty()
    {
        // Arrange

        string name = "Group";
        Guid courseId = Guid.Empty;
        var teachersId = new List<Guid> { Guid.NewGuid() };

        // Act 

        var exception = Assert.Throws<ArgumentException>(() => _groupService.GetOrCreate(name, courseId, teachersId));

        // Assert

        Assert.IsType<ArgumentException>(exception);
        Assert.Equal("The course ID cannot be empty.", exception.Message);
    }

    [Fact]
    public void GetOrCreate_CorrectlyThrowsArgumentExceptionWhenTeachersIdIsEmpty()
    {
        // Arrange

        string name = "Group";
        Guid courseId = new Guid("6957368b-f547-4166-b727-00fad69b77a3");
        List<Guid> teachersId = null;

        // Act 

        var exception = Assert.Throws<ArgumentException>(() => _groupService.GetOrCreate(name, courseId, teachersId));

        // Assert

        Assert.IsType<ArgumentException>(exception);
        Assert.Equal("At least one teacher ID must be provided.", exception.Message);
    }

    [Fact]
    public void UpdateGroup_CorrectlyThrowsArgumentExceptionWhenNameIsEmpty()
    {
        // Arrange

        string name = null;
        Guid groupId = new Guid("fbd47fd5-6d67-46c0-9509-43e991ee1de6");
        var teachersId = new List<Guid> { Guid.NewGuid() };

        // Act 

        var exception = Assert.Throws<ArgumentException>(() => _groupService.UpdateGroup(groupId, name, teachersId));

        // Assert

        Assert.IsType<ArgumentException>(exception);
        Assert.Equal("The group name cannot be empty.", exception.Message);
    }

    [Fact]
    public void UpdateGroup_CorrectlyThrowsArgumentExceptionWhenGroupIdIsEmpty()
    {
        // Arrange

        string name = "Updated Group";
        Guid groupId = Guid.Empty;
        var teachersId = new List<Guid> { Guid.NewGuid() };

        // Act 

        var exception = Assert.Throws<ArgumentException>(() => _groupService.UpdateGroup(groupId, name, teachersId));

        // Assert

        Assert.IsType<ArgumentException>(exception);
        Assert.Equal("The group ID cannot be empty.", exception.Message);
    }

    [Fact]
    public void UpdateGroup_CorrectlyThrowsArgumentExceptionWhenTeachersIdIsEmpty()
    {
        // Arrange

        string name = "Updated Group";
        Guid groupId = new Guid("fbd47fd5-6d67-46c0-9509-43e991ee1de6");
        List<Guid> teachersId = null;

        // Act 

        var exception = Assert.Throws<ArgumentException>(() => _groupService.UpdateGroup(groupId, name, teachersId));

        // Assert

        Assert.IsType<ArgumentException>(exception);
        Assert.Equal("At least one teacher ID must be provided.", exception.Message);
    }

    [Fact]
    public void DeleteGroup_CorrectlyThrowsArgumentExceptionWhenGroupIdIsEmpty()
    {
        // Arrange

        Guid groupId = Guid.Empty;

        // Act 

        var exception = Assert.Throws<ArgumentException>(() => _groupService.DeleteGroup(groupId));

        // Assert

        Assert.IsType<ArgumentException>(exception);
        Assert.Equal("The group ID cannot be empty.", exception.Message);
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


