using System.Windows.Input;
using UniversityManagementSystem.ViewModels.Base;
using UniversityManagementSystem.Infrastructure.Commands;
using System.Windows;
using System.Collections.ObjectModel;
using UniversityManagementSystem.Views.Windows;
using UniversityManagementSystem.Helpers;
using UniversityManagementSystem.DAL.Entities;
using UniversityManagementSystem.Services.ServicesInterfaces;


namespace UniversityManagementSystem.ViewModels
{
    internal class GroupCreateWindowViewModel : ViewModel
    {
        private readonly IGroupService _groupService;
        private readonly ITeacherService _teacherService;

        public ObservableCollection<Teacher> Teachers { get; set; } = new ObservableCollection<Teacher>();
        public ObservableCollection<Teacher> SelectedTeachers { get; set; } = new ObservableCollection<Teacher>();

        public NotificationHelper NotificationHelper { get; set; } = new NotificationHelper();

        #region Commands

        #region CreateGroupCommand - Сommand for creating group

        public ICommand CreateGroupCommand { get; }

        private bool CanCreateGroupCommandExecuted(object p) => true;

        private void OnCreateGroupCommandExecuted(object p)
        {
            if (string.IsNullOrEmpty(GroupName))
            {
                NotificationHelper.ShowWarning("The group cannot be created because the 'Name' field is not filled in.");
            }
            else if (SelectedTeachers.Count < 1)
            {
                NotificationHelper.ShowWarning("The group cannot be created because no teacher has been assigned.");
            }
            else
            {
                var confirmationWindowViewModel = new ConfirmationWindowViewModel("Do you want add a new group with the specified parameters ?");
                confirmationWindowViewModel.ConfirmationChoice += OnConfirmationChoice;

                var confirmationWindow = new ConfirmationWindow
                {
                    DataContext = confirmationWindowViewModel
                };

                confirmationWindow.ShowDialog();
            } 
        }

        #endregion

        #region AddSelectedTeacherCommand - Command to assign a teacher to the group's teacher list

        public ICommand AddSelectedTeacherCommand { get; }

        private bool CanAddSelectedTeacherCommandExecuted(object p)
        {
            return p != null;
        }

        private void OnAddSelectedTeacherCommand(object p)
        {
            var teacher = p as Teacher;

            if (teacher != null)
            {
                if (!SelectedTeachers.Any(t => t.TeacherId == teacher.TeacherId))
                {
                    SelectedTeachers.Add(teacher);
                }
            }
        }

        #endregion

        #region RemoveSelectedTeacherCommand - Command to remove a teacher from the group's teacher list

        public ICommand RemoveSelectedTeacherCommand { get; }

        private bool CanRemoveSelectedTeacherCommandExecuted(object p)
        {
            return p != null;
        }

        private void OnRemoveSelectedTeacherCommandExecuted(object p)
        {
            var teacher = p as Teacher;

            if (teacher != null)
            {
                var teacherToRemove = SelectedTeachers.FirstOrDefault(t => t.TeacherId == teacher.TeacherId);

                if (teacherToRemove != null)
                {
                    SelectedTeachers.Remove(teacherToRemove);
                }
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

        #region GroupCreated Event - Event triggered when a group is created

        public event Action GroupCreated;

        private void OnGroupCreated()
        {
            GroupCreated?.Invoke();
        }

        #endregion

        #endregion

        #region SelectedCourse: Course Select course from list

        private Course _selectedCourse;

        public Course Selectedcourse 
        {
            get => _selectedCourse;
            set => Set(ref _selectedCourse, value);
        }

        #endregion

        #region GroupName: string Name of the group to be created

        private string _groupName;

        public string GroupName
        {
            get => _groupName;
            set => Set(ref _groupName, value);
        }

        #endregion

        #region SelectedTeacher: Teacher Selected teacher to be assigned to the group

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
            var selectedTeachersId = new List<Guid>();

            foreach (var selectedTeacher in SelectedTeachers)
            {
                selectedTeachersId.Add(selectedTeacher.TeacherId);
            }

            var createdGroup = _groupService.GetOrCreate(GroupName, Selectedcourse.CourseId, selectedTeachersId);

            if (createdGroup != null)
            {
                OnGroupCreated();              
            }

            NotificationHelper.ShowSuccess("The group was successfully established.");

            var window = System.Windows.Application.Current.Windows.OfType<Window>().FirstOrDefault(w => w.DataContext == this);
            window?.Close();
        }

        public GroupCreateWindowViewModel(Course selectedCourse, IGroupService groupService, ITeacherService teacherService)
        {
            #region Commands

            CreateGroupCommand = new LambdaCommand(OnCreateGroupCommandExecuted, CanCreateGroupCommandExecuted);
            AddSelectedTeacherCommand = new LambdaCommand(OnAddSelectedTeacherCommand, CanAddSelectedTeacherCommandExecuted);
            RemoveSelectedTeacherCommand = new LambdaCommand(OnRemoveSelectedTeacherCommandExecuted, CanRemoveSelectedTeacherCommandExecuted);
            CloseWindowCommand = new LambdaCommand(OnCloseWindowCommandExecuted, CanCloseWindowCommandExecuted);

            #endregion

            _groupService = groupService;
            _teacherService = teacherService;

            var teachers = _teacherService.GetAllTeachers();

            foreach (var teacher in teachers)
            {
                Teachers.Add(teacher);
            }
            
            Selectedcourse = selectedCourse;
        }
    }
}
