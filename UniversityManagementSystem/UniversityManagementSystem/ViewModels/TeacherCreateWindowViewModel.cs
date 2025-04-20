using System.Windows.Input;
using System.Windows;
using UniversityManagementSystem.Infrastructure.Commands;
using UniversityManagementSystem.ViewModels.Base;
using UniversityManagementSystem.DAL;
using UniversityManagementSystem.Services;
using UniversityManagementSystem.Views.Windows;
using UniversityManagementSystem.Helpers;
using UniversityManagementSystem.Services.ServicesInterfaces;

namespace UniversityManagementSystem.ViewModels
{
    internal class TeacherCreateWindowViewModel : ViewModel
    {
        private readonly ITeacherService _teacherService;

        public NotificationHelper NotificationHelper { get; set; } = new NotificationHelper();

        #region Commands

        #region CreateTeacherCommand - Сommand for creating teacher

        public ICommand CreateTeacherCommand { get; }

        private bool CanCreateTeacherCommandExecuted(object p) => true;

        private void OnCreateTeacherCommandExecuted(object p)
        {
            if (string.IsNullOrEmpty(FirstName))
            {
                NotificationHelper.ShowWarning("Teacher could not be created. The 'First name' field is required.");
            }
            else if (string.IsNullOrEmpty(LastName))
            {
                NotificationHelper.ShowWarning("Teacher could not be created. The 'Last name' field is required.");
            }
            else
            {
                var confirmationWindowViewModel = new ConfirmationWindowViewModel("Do you want add a new Teacher with the specified parameters ?");
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

        #region FirstName: string First name of teacher who will be created 

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

        #region LastName: string Last name of teacher who will be created 

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

        #region TeacherCreated Event - Event triggered when a teacher is created

        public event Action TeacherCreated;

        private void OnTeacherCreated()
        {
            TeacherCreated?.Invoke();
        }
        #endregion

        #endregion 

        private void OnConfirmationChoice()
        {
            var createdTeacher = _teacherService.Create(FirstName, LastName);

            if (createdTeacher != null)
            {
                OnTeacherCreated();
            }

            NotificationHelper.ShowSuccess("The teacher was successfully created.");

            var window = System.Windows.Application.Current.Windows.OfType<Window>().FirstOrDefault(w => w.DataContext == this);
            window?.Close();
        }

        public TeacherCreateWindowViewModel(ITeacherService teacherService)
        {
            #region Commands

            CreateTeacherCommand = new LambdaCommand(OnCreateTeacherCommandExecuted, CanCreateTeacherCommandExecuted);
            CloseWindowCommand = new LambdaCommand(OnCloseWindowCommandExecuted, CanCloseWindowCommandExecuted);

            #endregion

            _teacherService = teacherService;
        }
    }
}
