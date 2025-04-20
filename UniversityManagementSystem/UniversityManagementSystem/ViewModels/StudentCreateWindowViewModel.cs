using System.Windows;
using System.Windows.Input;
using UniversityManagementSystem.DAL;
using UniversityManagementSystem.FileProcessors;
using UniversityManagementSystem.Helpers;
using UniversityManagementSystem.Infrastructure.Commands;
using UniversityManagementSystem.Services;
using UniversityManagementSystem.Services.ServicesInterfaces;
using UniversityManagementSystem.ViewModels.Base;
using UniversityManagementSystem.Views.Windows;

namespace UniversityManagementSystem.ViewModels
{
    internal class StudentCreateWindowViewModel : ViewModel
    {
        private readonly IStudentService _studentService;
        private readonly IGroupService _groupService;

        public NotificationHelper NotificationHelper { get; set; } = new NotificationHelper();

        #region Commands

        #region CreateStudentCommand - Сommand for creating student

        public ICommand CreateStudentCommand { get; }

        private bool CanCreateStudentCommandExecuted(object p) => true;

        private void OnCreateStudentCommandExecuted(object p)
        {
            if (string.IsNullOrEmpty(FirstName))
            {
                NotificationHelper.ShowWarning("Student could not be created. The 'First name' field is required.");
            }
            else if (string.IsNullOrEmpty(LastName))
            {
                NotificationHelper.ShowWarning("Student could not be created. The 'Last name' field is required.");
            }
            else
            {
                var confirmationWindowViewModel = new ConfirmationWindowViewModel("Do you want add a new student with the specified parameters ?");
                confirmationWindowViewModel.ConfirmationChoice += OnConfirmationChoice;

                var confirmationWindow = new ConfirmationWindow
                {
                    DataContext = confirmationWindowViewModel
                };

                confirmationWindow.ShowDialog();
            }
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

        #region ImportFromCsvFileCommand - Command to import students from a csv file 

        public ICommand ImportFromCsvFileCommand { get; }

        private bool CanImportFromCsvFileCommandExecuted(object p)
        {
            return true;
        }

        private void OnImportFromCsvFileCommandExecuted(object p)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "CSV Files (*.csv)|*.csv|All Files (*.*)|*.*",
                Title = "Select CSV File"
            };

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                string selectedFilePath = openFileDialog.FileName;

                _groupService.ClearStudentsFromGroup(SelectedGroup.GroupId);

                CsvProcessor csvProcessor = new CsvProcessor(_studentService);

                csvProcessor.ImportFromCsv(selectedFilePath, SelectedGroup);
            }

            OnStudentCreated();

            var window = System.Windows.Application.Current.Windows.OfType<Window>().FirstOrDefault(w => w.DataContext == this);
            window?.Close();
        }

        #endregion

        #endregion

        #region SelectedGroup: Group Select group from list

        private Group _selectedGroup;

        public Group SelectedGroup
        {
            get => _selectedGroup;
            set
            {
                Set(ref _selectedGroup, value);
            }
        }

        #endregion

        #region FirstName: string First name of student who will be created 

        private string _firstName;

        public string FirstName
        {
            get => _firstName;
            set
            {
                Set(ref _firstName, value);
            }
        }

        #endregion

        #region LastName: string Last name of student who will be created 

        private string _lastName;

        public string LastName
        {
            get => _lastName;
            set
            {
                Set(ref _lastName, value);
            }
        }

        #endregion

        #region Events

        #region StudentCreated Event - Event triggered when a student is created

        public event Action StudentCreated;

        private void OnStudentCreated()
        {
            StudentCreated?.Invoke();
        }

        #endregion

        #endregion

        private void OnConfirmationChoice()
        {
            var createdStudent = _studentService.GetOrCreate(FirstName, LastName, SelectedGroup.GroupId);

            if (createdStudent != null)
            {
                OnStudentCreated();
            }

            NotificationHelper.ShowSuccess("The student was successfully created.");

            var window = System.Windows.Application.Current.Windows.OfType<Window>().FirstOrDefault(w => w.DataContext == this);
            window?.Close();
        }

        public StudentCreateWindowViewModel(Group selectedGroup, IStudentService studentService, IGroupService groupService)
        {
            #region Commands

            CreateStudentCommand = new LambdaCommand(OnCreateStudentCommandExecuted, CanCreateStudentCommandExecuted);
            CloseWindowCommand = new LambdaCommand(OnCloseWindowCommandExecuted, CanCloseWindowCommandExecuted);
            ImportFromCsvFileCommand = new LambdaCommand(OnImportFromCsvFileCommandExecuted, CanImportFromCsvFileCommandExecuted);

            #endregion

            _studentService = studentService;
            _groupService = groupService;

            SelectedGroup = selectedGroup;
        }
    }
}
