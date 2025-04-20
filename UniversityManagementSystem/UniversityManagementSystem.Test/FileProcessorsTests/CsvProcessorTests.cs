using UniversityManagementSystem.DAL.Entities;
using UniversityManagementSystem.DTO;
using UniversityManagementSystem.FileProcessors;

namespace UniversityManagementSystem.Test.FileProcessorsTests
{
    public class CsvProcessorTests
    {
        private readonly CsvProcessor _csvProcessor;

        public CsvProcessorTests()
        {


            _csvProcessor = new CsvProcessor();
        }

        [Fact]
        public void ExportToCsv_CorrectlyThrowsArgumentExceptionWhenOutputDirectoryIsEmpty()
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

            var exception = Assert.Throws<ArgumentException>(() => _csvProcessor.ExportToCsv(dataModel, outputDirectory));

            // Assert

            Assert.IsType<ArgumentException>(exception);
            Assert.Equal("Output directory cannot be empty.", exception.Message);
        }

        [Fact]
        public void ExportToCsv_CorrectlyThrowsArgumentExceptionWhenDataModelIsEmpty()
        {
            // Arrange

            TextDataModel dataModel = null;
            string outputDirectory = "";

            // Act

            var exception = Assert.Throws<ArgumentException>(() => _csvProcessor.ExportToCsv(dataModel, outputDirectory));

            // Assert

            Assert.IsType<ArgumentException>(exception);
            Assert.Equal("DataModel cannot be null.", exception.Message);
        }

        [Fact]
        public void ExportToCsv_CorrectlyCreateFile()
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
            string expectedFilePath = Path.Combine(outputDirectory, $"{dataModel.Course.Name}_{dataModel.Group.Name}.csv");

            try
            {
                // Act

                _csvProcessor.ExportToCsv(dataModel, outputDirectory);

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


        [Fact]
        public void ImportFromCsv_CorrectlyThrowsArgumentExceptionWhenFilePathIsEmpty()
        {
            // Arrange

            string filePath = null;
            var group = new Group(Guid.NewGuid(), "Group1", Guid.NewGuid());

            // Act

            var exception = Assert.Throws<ArgumentException>(() => _csvProcessor.ImportFromCsv(filePath, group));

            // Assert

            Assert.IsType<ArgumentException>(exception);
            Assert.Equal("File path cannot be empty.", exception.Message);
        }

        [Fact]
        public void ImportFromCsv_CorrectlyThrowsArgumentExceptionWhenGroupIsEmpty()
        {
            // Arrange

            string filePath = "E:\\TestFiles";
            Group group = null;

            // Act

            var exception = Assert.Throws<ArgumentException>(() => _csvProcessor.ImportFromCsv(filePath, group));

            // Assert

            Assert.IsType<ArgumentException>(exception);
            Assert.Equal("Group cannot be null", exception.Message);
        }
    }
}
