using System.Windows.Input;
using System.Windows;
using UniversityManagementSystem.ViewModels.Base;
using UniversityManagementSystem.Infrastructure.Commands;
using UniversityManagementSystem.Views.Windows;
using UniversityManagementSystem.Helpers;
using UniversityManagementSystem.DAL.Entities;
using UniversityManagementSystem.Services.ServicesInterfaces;

namespace UniversityManagementSystem.ViewModels
{
    internal class TeacherEditWindowViewModel : ViewModel
    {
        private readonly ITeacherService _teacherService;

        public NotificationHelper NotificationHelper { get; set; } = new NotificationHelper();

        #region Commands

        #region EditTeacherCommand - Command to edit existing teacher

        public ICommand EditTeacherCommand { get; }

        private bool CanEditTeacherCommandExecuted(object p) => true;

        private void OnEditTeacherCommandExecuted(object p)
        {
            if (string.IsNullOrEmpty(FirstName))
            {
                NotificationHelper.ShowWarning("Teacher could not be edited. The 'First name' field is required.");
            }
            else if (string.IsNullOrEmpty(LastName))
            {
                NotificationHelper.ShowWarning("Teacher could not be edited. The 'Last name' field is required.");
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

        #region TeacherEdited Event - Event triggered when a teacher is edited

        public event Action TeacherEdited;

        private void OnTeacherEdited()
        {
            TeacherEdited?.Invoke();
        }

        #endregion

        #endregion

        #region FirstName: string First name of teacher who will be edited 

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

        #region LastName: string Last name of teacher who will be edited

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

        #region SelectedTeacher: Teacher Select teacher for editing

        private Teacher _selectedTeacher;

        public Teacher SelectedTeacher
        {
            get => _selectedTeacher;
            set
            {
                Set(ref _selectedTeacher, value);
            }
        }

        #endregion

        private void OnConfirmationChoice()
        {
            SelectedTeacher.FirstName = FirstName;
            SelectedTeacher.LastName = LastName;

            var editedTeacher = _teacherService.UpdateTeacher(SelectedTeacher.TeacherId, SelectedTeacher.FirstName, SelectedTeacher.LastName);

            if (editedTeacher != null)
            {
                TeacherEdited();
            }

            NotificationHelper.ShowSuccess("The teacher was successfully edited.");

            var window = System.Windows.Application.Current.Windows.OfType<Window>().FirstOrDefault(w => w.DataContext == this);
            window?.Close();
        }

        public TeacherEditWindowViewModel(Teacher selectedTeacher, ITeacherService teacherService)
        {
            #region Commands

            EditTeacherCommand = new LambdaCommand(OnEditTeacherCommandExecuted, CanEditTeacherCommandExecuted);
            CloseWindowCommand = new LambdaCommand(OnCloseWindowCommandExecuted, CanCloseWindowCommandExecuted);

            #endregion

            _teacherService = teacherService;

            SelectedTeacher = selectedTeacher;

            FirstName = selectedTeacher.FirstName;
            LastName = selectedTeacher.LastName;
        }
    }
}
