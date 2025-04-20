using UniversityManagementSystem.DAL.Entities;
using UniversityManagementSystem.FileProcessors;
using UniversityManagementSystem.DTO;
    
namespace UniversityManagementSystem.Test.FileProcessorsTests
{
    public class DocxProcessorTests
    {
        private readonly DocxProcessor _docxProcessor;

        public DocxProcessorTests()
        {
            _docxProcessor = new DocxProcessor();
        }

        [Fact]
        public void ExportToDocx_СorrectlyThrowsArgumentExceptionWhenOutputDirectoryIsEmpty()
        {
            // Arrange

            var course = new Course(Guid.NewGuid(), "Course1", "Description of the course");
            var group = new Group(Guid.NewGuid(), "Group1", course.CourseId);
            var students = new List<Student>
            {
                new Student("John", "Doe", group.GroupId, Guid.NewGuid())
            };

            var dataModel = new TextDataModel(course, group, students);
            string outputDirectory = null;

            // Act

            var exception = Assert.Throws<ArgumentException>(() => _docxProcessor.ExportToDocx(dataModel, outputDirectory));

            // Assert

            Assert.IsType<ArgumentException>(exception);
            Assert.Equal("Output directory cannot be empty.", exception.Message);
        }

        [Fact]
        public void ExportToDocx_СorrectlyThrowsArgumentExceptionWhenDataModelIsEmpty()
        {
            // Arrange

            TextDataModel dataModel = null;
            string outputDirectory = "";

            // Act

            var exception = Assert.Throws<ArgumentException>(() => _docxProcessor.ExportToDocx(dataModel, outputDirectory));

            // Assert
            Assert.IsType<ArgumentException>(exception);
            Assert.Equal("DataModel cannot be null.", exception.Message);
        }

        [Fact]
        public void ExportToDocx_CorrectlyCreateFile()
        {
            // Arrange

            var course = new Course(Guid.NewGuid(), "Course1", "Description of the course");
            var group = new Group(Guid.NewGuid(), "Group1", course.CourseId);
            var students = new List<Student>
            {
                new Student("John", "Doe", group.GroupId, Guid.NewGuid())
            };

            var dataModel = new TextDataModel(course, group, students);
            string outputDirectory = "E:\\TestFiles";
            string expectedFilePath = Path.Combine(outputDirectory, $"{dataModel.Course.Name}_{dataModel.Group.Name}.docx");

            try
            {
                // Act

                _docxProcessor.ExportToDocx(dataModel, outputDirectory);

                // Assert 
                Assert.True(File.Exists(expectedFilePath));
            }
            finally
            {
                if (File.Exists(expectedFilePath))
                {
                    File.Delete(expectedFilePath);  
                }
            }
        }
    }
}
