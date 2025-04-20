using System.IO;
using PdfSharp.Pdf;
using PdfSharp.Drawing;
using UniversityManagementSystem.DTO;

namespace UniversityManagementSystem.FileProcessors
{
    public class PdfProcessor
    {
        public void ExportToPdf(TextDataModel dataModel, string outputDirectory)
        {
            if (dataModel == null)
                throw new ArgumentException("DataModel cannot be null.");

            if (string.IsNullOrWhiteSpace(outputDirectory))
                throw new ArgumentException("Output directory cannot be empty.");

            string fileName = $"{dataModel.Course.Name}_{dataModel.Group.Name}.pdf";
            string filePath = Path.Combine(outputDirectory, fileName);

            PdfDocument document = new PdfDocument();
            document.Info.Title = $"{dataModel.Course.Name} - {dataModel.Group.Name}";

            PdfPage page = document.AddPage();
            XGraphics gfx = XGraphics.FromPdfPage(page);

            XFont font = new XFont("Arial", 12);

            double yPosition = 40;

            if (dataModel.Students != null && dataModel.Students.Count > 0)
            {
                gfx.DrawString("Students List:", font, XBrushes.Black, new XPoint(40, yPosition));
                yPosition += 20;

                for (int i = 0; i < dataModel.Students.Count; i++)
                {
                    string studentInfo = $"{i + 1}. {dataModel.Students[i].FirstName} {dataModel.Students[i].LastName}";
                    gfx.DrawString(studentInfo, font, XBrushes.Black, new XPoint(40, yPosition));
                    yPosition += 20;
                }
            }
            else
            {
                gfx.DrawString("No students found in this group.", font, XBrushes.Black, new XPoint(40, yPosition));
            }

            document.Save(filePath);
        }
    }
}
