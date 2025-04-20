using System.IO;
using UniversityManagementSystem.DTO;
using Xceed.Document.NET;
using Xceed.Words.NET;

namespace UniversityManagementSystem.FileProcessors
{
    public class DocxProcessor
    {
        public void ExportToDocx(TextDataModel dataModel, string outputDirectory)
        {
            if (dataModel == null)
                throw new ArgumentException("DataModel cannot be null.");

            if (string.IsNullOrWhiteSpace(outputDirectory))
                throw new ArgumentException("Output directory cannot be empty.");

            string fileName = $"{dataModel.Course.Name}_{dataModel.Group.Name}.docx";
            string filePath = Path.Combine(outputDirectory, fileName);

            using (var doc = DocX.Create(filePath))
            {
                if (dataModel.Students != null && dataModel.Students.Count > 0)
                {
                    doc.InsertParagraph("Students List:")
                        .FontSize(14);

                    var list = doc.AddList($"{dataModel.Students[0].FirstName} {dataModel.Students[0].LastName}", 0, ListItemType.Numbered);

                    for (int i = 1; i < dataModel.Students.Count; i++)
                    {
                        string studentInfo = $"{dataModel.Students[i].FirstName} {dataModel.Students[i].LastName}";
                        doc.AddListItem(list, studentInfo);
                    }

                    doc.InsertList(list);
                }
                else
                {
                    doc.InsertParagraph("No students found in this group.")
                        .FontSize(12);
                }

                doc.Save();
            }
        }
    }
}
