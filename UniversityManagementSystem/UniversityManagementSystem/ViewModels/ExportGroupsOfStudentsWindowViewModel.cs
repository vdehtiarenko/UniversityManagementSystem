using UniversityManagementSystem.ViewModels.Base;
using System.Windows.Input;
using UniversityManagementSystem.FileProcessors;
using UniversityManagementSystem.Infrastructure.Commands;
using System.Windows;
using UniversityManagementSystem.Helpers;
using UniversityManagementSystem.DTO;
using UniversityManagementSystem.Services.ServicesInterfaces;

namespace UniversityManagementSystem.ViewModels
{
    internal class ExportGroupsOfStudentsWindowViewModel : ViewModel
    {
        public NotificationHelper NotificationHelper { get; set; } = new NotificationHelper();

        private readonly IStudentService _studentService;

        #region Command

        #region ExportToDocxFileCommand - Command for exporting a list of students to a docx file

        public ICommand ExportToDocxFileCommand {  get; }

        private bool CanExportToDocxFileCommandExecuted(object p)
        {
            return TextDataModel != null;
        }

        private void OnExportToDocxFileCommandExecuted(object p)
        {
            FolderBrowserDialog folderDialog = new FolderBrowserDialog();

            if (folderDialog.ShowDialog() == DialogResult.OK)
            {
                string selectedPath = folderDialog.SelectedPath;

                DocxProcessor docxProcessor = new DocxProcessor();

                docxProcessor.ExportToDocx(TextDataModel, selectedPath);
            }

            NotificationHelper.ShowSuccess("The students were successfully saved to a DOCX file.");
        }

        #endregion

        #region ExportToPdfFileCommand - Command for exporting a list of students to a pdf file

        public ICommand ExportToPdfFileCommand { get; }

        private bool CanExportToPdfFileCommandExecuted(object p)
        {
            return TextDataModel != null;
        }

        private void OnExportToPdfFileCommandExecuted(object p)
        {
            FolderBrowserDialog folderDialog = new FolderBrowserDialog();

            if (folderDialog.ShowDialog() == DialogResult.OK)
            {
                string selectedPath = folderDialog.SelectedPath;

                PdfProcessor pdfProcessor = new PdfProcessor();

                pdfProcessor.ExportToPdf(TextDataModel, selectedPath);
            }

            NotificationHelper.ShowSuccess("The students were successfully saved to a PDF file.");
        }

        #endregion

        #region ExportToCsvFileCommandd - Command for exporting a list of students to a csv file

        public ICommand ExportToCsvFileCommand { get; }

        private bool CanExportToCsvFileCommandExecuted(object p)
        {
            return TextDataModel != null;
        }

        private void OnExportToCsvFileCommandExecuted(object p)
        {
            FolderBrowserDialog folderDialog = new FolderBrowserDialog();

            if (folderDialog.ShowDialog() == DialogResult.OK)
            {
                string selectedPath = folderDialog.SelectedPath;

                CsvProcessor csvProcessor = new CsvProcessor(_studentService);

                csvProcessor.ExportToCsv(TextDataModel, selectedPath);
            }

            NotificationHelper.ShowSuccess("The students were successfully saved to a CSV file.");
        }

        #endregion

        #region CloseWindowCommand - Command for closing window

        public ICommand CloseWindowCommand { get; }

        private bool CanCloseWindowCommandExecuted(object p) => true;

        private void OnCloseWindowCommandExecuted(object p)
        {
            var window = System.Windows.Application.Current.Windows.OfType<Window>().FirstOrDefault(w => w.DataContext == this);
            window?.Close();
        }

        #endregion 

        #endregion

        #region TextDataModel: TextDataModel A module that contains data for export to various types of files

        private TextDataModel _textDataModel;

        public TextDataModel TextDataModel 
        {
            get => _textDataModel; 
            set
            {
                Set(ref _textDataModel, value);  
            }
        }

        #endregion

        public ExportGroupsOfStudentsWindowViewModel(TextDataModel textDataModel, IStudentService studentService)
        {
            #region Commands

            ExportToDocxFileCommand = new LambdaCommand(OnExportToDocxFileCommandExecuted, CanExportToDocxFileCommandExecuted);
            ExportToPdfFileCommand = new LambdaCommand(OnExportToPdfFileCommandExecuted, CanExportToPdfFileCommandExecuted);
            ExportToCsvFileCommand = new LambdaCommand(OnExportToCsvFileCommandExecuted, CanExportToCsvFileCommandExecuted);
            CloseWindowCommand = new LambdaCommand(OnCloseWindowCommandExecuted, CanCloseWindowCommandExecuted);

            #endregion

            _studentService = studentService;

            TextDataModel = textDataModel;
        }
    }
}
