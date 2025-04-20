using Moq;
using Microsoft.EntityFrameworkCore;
using UniversityManagementSystem.DAL;
using UniversityManagementSystem.DAL.Entities;
using UniversityManagementSystem.Services;

public class StudentServiceTests
{
    private readonly Mock<DbSet<Student>> _mockStudentDbSet;
    private readonly Mock<ApplicationDBContext> _mockDbContext;
    private readonly StudentService _studentService;

    public StudentServiceTests()
    {
        var students = new List<Student>
        {
            new Student("John", "Doe", new Guid("fbd47fd5-6d67-46c0-9509-43e991ee1de6"), new Guid("d65f5c34-3c26-4f6f-bc93-cd0353fa7f79")),
            new Student("Jane", "Smith", new Guid("bd7db7a9-fc49-48ac-96c5-c35c87bbd086"), new Guid("d65f5c34-3c26-4f6f-bc93-cd0353fa7f79")),
            new Student("Mike", "Johnson", new Guid("663c3da9-6ae8-4f10-9f8e-b77f356e0c83"), new Guid("d65f5c34-3c26-4f6f-bc93-cd0353fa7f79"))
        };

        _mockStudentDbSet = CreateMockDbSet<Student>(students);

        _mockDbContext = new Mock<ApplicationDBContext>();
        _mockDbContext.Setup(db => db.Set<Student>()).Returns(_mockStudentDbSet.Object);

        _studentService = new StudentService(_mockDbContext.Object);
    }

    [Fact]
    public void GetOrCreate_CorrectlyThrowsArgumentExceptionWhenFirstNameIsEmpty()
    {
        // Arrange

        string firstName = null;
        string lastName = "Doe";
        Guid groupId = new Guid("d65f5c34-3c26-4f6f-bc93-cd0353fa7f79");

        // Act 

        var exception = Assert.Throws<ArgumentException>(() => _studentService.GetOrCreate(firstName, lastName, groupId));

        // Assert

        Assert.IsType<ArgumentException>(exception);
        Assert.Equal("The first name cannot be empty.", exception.Message);
    }

    [Fact]
    public void GetOrCreate_CorrectlyThrowsArgumentExceptionWhenLastNameIsEmpty()
    {
        // Arrange

        string firstName = "John";
        string lastName = null;
        Guid groupId = new Guid("d65f5c34-3c26-4f6f-bc93-cd0353fa7f79");

        // Act

        var exception = Assert.Throws<ArgumentException>(() => _studentService.GetOrCreate(firstName, lastName, groupId));

        // Assert

        Assert.IsType<ArgumentException>(exception);
        Assert.Equal("The last name cannot be empty.", exception.Message);
    }

    [Fact]
    public void GetOrCreate_CorrectlyThrowsArgumentExceptionWhenGroupIdIsEmpty()
    {
        // Arrange

        string firstName = "John";
        string lastName = "Doe";
        Guid groupId = Guid.Empty;

        // Act 

        var exception = Assert.Throws<ArgumentException>(() => _studentService.GetOrCreate(firstName, lastName, groupId));

        // Assert

        Assert.IsType<ArgumentException>(exception);
        Assert.Equal("The group ID cannot be empty.", exception.Message);
    }

    [Fact]
    public void UpdateStudent_CorrectlyThrowsArgumentExceptionWhenStudentIdIsEmpty()
    {
        // Arrange

        Guid studentId = Guid.Empty;
        string firstName = "John";
        string lastName = "Doe";

        // Act 

        var exception = Assert.Throws<ArgumentException>(() => _studentService.UpdateStudent(studentId, firstName, lastName));

        // Assert

        Assert.IsType<ArgumentException>(exception);
        Assert.Equal("The student ID cannot be empty.", exception.Message);
    }

    [Fact]
    public void UpdateStudent_CorrectlyThrowsArgumentExceptionWhenFirstNameIsEmpty()
    {
        // Arrange

        Guid studentId = new Guid("fbd47fd5-6d67-46c0-9509-43e991ee1de6");
        string firstName = null;
        string lastName = "Doe";

        // Act 

        var exception = Assert.Throws<ArgumentException>(() => _studentService.UpdateStudent(studentId, firstName, lastName));

        // Assert

        Assert.IsType<ArgumentException>(exception);
        Assert.Equal("The first name cannot be empty.", exception.Message);
    }

    [Fact]
    public void UpdateStudent_CorrectlyThrowsArgumentExceptionWhenLastNameIsEmpty()
    {
        // Arrange

        Guid studentId = new Guid("fbd47fd5-6d67-46c0-9509-43e991ee1de6");
        string firstName = "John";
        string lastName = null;

        // Act 

        var exception = Assert.Throws<ArgumentException>(() => _studentService.UpdateStudent(studentId, firstName, lastName));

        // Assert

        Assert.IsType<ArgumentException>(exception);
        Assert.Equal("The last name cannot be empty.", exception.Message);
    }

    [Fact]
    public void DeleteStudent_CorrectlyThrowsArgumentExceptionWhenStudentIdIsEmpty()
    {
        // Arrange

        Guid studentId = Guid.Empty;

        // Act 

        var exception = Assert.Throws<ArgumentException>(() => _studentService.DeleteStudent(studentId));

        // Assert

        Assert.IsType<ArgumentException>(exception);
        Assert.Equal("The student ID cannot be empty.", exception.Message);
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
