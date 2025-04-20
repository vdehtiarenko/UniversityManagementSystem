using UniversityManagementSystem.Services;
using UniversityManagementSystem.ViewModels.Base;
using UniversityManagementSystem.DAL;
using UniversityManagementSystem.Infrastructure.Commands;
using System.Windows.Input;
using UniversityManagementSystem.Views.Windows;
using System.Windows;
using UniversityManagementSystem.Helpers;
using UniversityManagementSystem.DAL.Entities;
using UniversityManagementSystem.Services.ServicesInterfaces;

namespace UniversityManagementSystem.ViewModels
{
    internal class StudentEditWindowViewModel : ViewModel
    {
        private readonly IStudentService _studentService;

        public NotificationHelper NotificationHelper { get; set; } = new NotificationHelper();

        #region Commands

        #region EditStudentCommand - Command to edit existing student

        public ICommand EditStudentCommand { get; }

        private bool CanEditStudentCommandExecuted(object p) => true;

        private void OnEditStudentCommandExecuted(object p)
        {
            if (string.IsNullOrEmpty(FirstName))
            {
                NotificationHelper.ShowWarning("Student could not be edited. The 'First name' field is required.");
            }
            else if (string.IsNullOrEmpty(LastName))
            {
                NotificationHelper.ShowWarning("Student could not be edited. The 'Last name' field is required.");
            }
            else
            {
                var confirmationWindowViewModel = new ConfirmationWindowViewModel("Do you want to save all your changes ?");
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

        #endregion

        #region Events

        #region StudentEdited Event - Event triggered when a student is edited

        public event Action StudentEdited;

        private void OnStudentEdited()
        {
            StudentEdited?.Invoke();
        }

        #endregion

        #endregion

        #region FirstName: string First name of student who will be edited 

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

        #region LastName: string Last name of student who will be edited

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

        #region SelectedStudent: Student Select student for editing

        private Student _selectedStudent;

        public Student SelectedStudent 
        {
            get => _selectedStudent; 
            set
            {
                Set(ref _selectedStudent, value);
            }
        }

        #endregion

        private void OnConfirmationChoice()
        {
            SelectedStudent.FirstName = FirstName;
            SelectedStudent.LastName = LastName;

            var editedStudent = _studentService.UpdateStudent(SelectedStudent.StudentId, SelectedStudent.FirstName, SelectedStudent.LastName);

            if (editedStudent != null)
            {
                StudentEdited();
            }

            NotificationHelper.ShowSuccess("The student was successfully edited.");

            var window = System.Windows.Application.Current.Windows.OfType<Window>().FirstOrDefault(w => w.DataContext == this);
            window?.Close();
        }

        public StudentEditWindowViewModel(Student selectedStudent, IStudentService studentService)
        {
            #region Commands

            EditStudentCommand = new LambdaCommand(OnEditStudentCommandExecuted, CanEditStudentCommandExecuted);
            CloseWindowCommand = new LambdaCommand(OnCloseWindowCommandExecuted, CanCloseWindowCommandExecuted);

            #endregion 

            _studentService = studentService;

            SelectedStudent = selectedStudent;

            FirstName = SelectedStudent.FirstName;
            LastName = SelectedStudent.LastName;
        }

    }
}
