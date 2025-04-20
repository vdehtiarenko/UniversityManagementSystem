using CsvHelper;
using System.Globalization;
using System.IO;
using UniversityManagementSystem.DTO;
using UniversityManagementSystem.Helpers;
using UniversityManagementSystem.Services.ServicesInterfaces;

namespace UniversityManagementSystem.FileProcessors
{
    public class CsvProcessor
    {
        private readonly IStudentService _studentService;

        public NotificationHelper NotificationHelper { get; set; } = new NotificationHelper();

        public void ExportToCsv(TextDataModel dataModel, string outputDirectory)
        {

            if (dataModel == null)
                throw new ArgumentException("DataModel cannot be null.");

            if (string.IsNullOrWhiteSpace(outputDirectory))
                throw new ArgumentException("Output directory cannot be empty.");

            var studenCsvModel = dataModel.Students
                .Select(s => new StudentCsvModel(s.FirstName, s.LastName))
                .ToList();

            string csvFilePath = Path.Combine(outputDirectory, $"{dataModel.Course.Name}_{dataModel.Group.Name}.csv");

            using (var writer = new StreamWriter(csvFilePath))
            using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
            {
                csv.WriteRecords(studenCsvModel);
            }
        }
        public void ImportFromCsv(string filePath, Group group)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("File path cannot be empty.");

            if (group == null)
                throw new ArgumentException("Group cannot be null");

            try
            {
                if (File.Exists(filePath))
                {
                    using (var reader = new StreamReader(filePath))
                    using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
                    {
                        while (csv.Read())
                        {
                            string firstName = csv.TryGetField<string>(0, out var firstNameResult) ? firstNameResult : "";
                            string lastName = csv.TryGetField<string>(1, out var lastNameResult) ? lastNameResult : "";

                            var student = _studentService.GetOrCreate(firstName, lastName, group.GroupId);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                NotificationHelper.ShowWarning(ex.Message);
            }
        }

        public CsvProcessor(IStudentService studentService)
        {
            _studentService = studentService;
        }

        public CsvProcessor() { }
    }
}
