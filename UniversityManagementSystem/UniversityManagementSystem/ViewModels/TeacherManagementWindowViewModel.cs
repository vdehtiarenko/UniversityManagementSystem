using System.Collections.ObjectModel;
using System.Windows.Input;
using UniversityManagementSystem.ViewModels.Base;
using UniversityManagementSystem.Infrastructure.Commands;
using UniversityManagementSystem.Views.Windows;
using UniversityManagementSystem.DAL.Entities;
using UniversityManagementSystem.Services.ServicesInterfaces;
using UniversityManagementSystem.Helpers;
using UniversityManagementSystem.DTO;


namespace UniversityManagementSystem.ViewModels
{
    internal class TeacherManagementWindowViewModel : ViewModel
    {
        private readonly ITeacherService _teacherService;
        private readonly ICourseService _courseService;

        public ObservableCollection<Teacher> Teachers { get; set; } = new ObservableCollection<Teacher>();
        public ObservableCollection<GroupWithCourseNameModel> GroupsInWhichTheTeacherTeach { get; set; } = new ObservableCollection<GroupWithCourseNameModel>();

        public NotificationHelper NotificationHelper { get; set; } = new NotificationHelper();

        #region Command 

        #region OpenCreateTeacherWindowCommand - Command to open teacher creation window

        public ICommand OpenCreateTeacherWindowCommand { get; }

        private bool CanOpenCreateTeacherWindowCommandExecuted(object p) => true;

        private void OnOpenCreateTeacherWindowCommandExecuted(object p)
        {
            var teacherCreateWindowViewModel = new TeacherCreateWindowViewModel(_teacherService);
            teacherCreateWindowViewModel.TeacherCreated += UpdateTeachers;

            var teacherCreateWindow = new TeacherCreateWindow
            {
                DataContext = teacherCreateWindowViewModel
            };

            teacherCreateWindow.ShowDialog();
        }

        #endregion

        #region OpenAddGroupToTeacherWindowCommand - Command to open 

        public ICommand OpenAddGroupToTeacherWindowCommand { get; }

        private bool CanOpenAddGroupToTeacherWindowCommandExecuted(object p) => true;

        private void OnOpenAddGroupToTeacherWindowCommandExecuted(object p)
        {
            if (SelectedTeacher ==  null)
            {
                NotificationHelper.ShowWarning("No teacher selected! Select a teacher before adding a group.");
            }
            else
            {
                var addingGroupToTeacherWindowViewModel = new AddingGroupToTeacherWindowViewModel(SelectedTeacher, _teacherService, _courseService);
                addingGroupToTeacherWindowViewModel.GroupAdded += UpdateGroupsInWhichTheTeacherTeach;

                var addingGroupToTeacherWindow = new AddingGroupToTeacherWindow
                {
                    DataContext = addingGroupToTeacherWindowViewModel
                };

                addingGroupToTeacherWindow.ShowDialog();
            }
        }

        #endregion

        #region EditTeacherCommand - Command to edit a teacher

        public ICommand EditTeacherCommand { get; }

        private bool CanEditTeacherCommandExecuted(object p)
        {
            return p != null;
        }

        private void OnEditTeacherCommandExecuted(object p)
        {
            if (p is Teacher teacher)
            {
                SelectedTeacher = teacher;
            }

            var teacherEditWindowViewModel = new TeacherEditWindowViewModel(SelectedTeacher, _teacherService);
            teacherEditWindowViewModel.TeacherEdited += UpdateTeachers;

            var teacherEditWindow = new TeacherEditWindow
            {
                DataContext = teacherEditWindowViewModel
            };

            teacherEditWindow.ShowDialog();
        }

        #endregion

        #region DeleteStudentCommand - Command to delete a teacher

        public ICommand DeleteTeacherCommand { get; }

        private bool CanDeleteTeacherCommandExecuted(object p)
        {
            return p != null;
        }

        private void OnDeleteTeacherCommandExecuted(object p)
        {
            if (p is Teacher teacher)
            {
                SelectedTeacher = teacher;
            }

            var confirmationWindowViewModel = new ConfirmationWindowViewModel($"Do you want to delete the Teacher: {SelectedTeacher.FirstName} {SelectedTeacher.LastName}");
            confirmationWindowViewModel.ConfirmationChoice += OnConfirmationChoice;

            var confirmationWindow = new ConfirmationWindow
            {
                DataContext = confirmationWindowViewModel
            };

            confirmationWindow.ShowDialog();
        }

        #endregion

        #endregion

        #region SelectedTeacher: Teacher Select teacher from the list

        private Teacher _selectedTeacher;

        public Teacher SelectedTeacher
        {
            get => _selectedTeacher;
            set
            {
                if (Set(ref _selectedTeacher, value))
                {
                    if (SelectedTeacher != null)
                    {
                        var groupsInWhichTheTeacherTeach = _teacherService.GetGroupsInWhichTheTeacherTeach(SelectedTeacher.TeacherId);

                        GroupsInWhichTheTeacherTeach.Clear();

                        foreach (var group in groupsInWhichTheTeacherTeach)
                        {
                            var groupWithCourseNameModel = new GroupWithCourseNameModel(group, _courseService);
                            GroupsInWhichTheTeacherTeach.Add(groupWithCourseNameModel);
                        }

                        OnPropertyChanged(nameof(GroupsInWhichTheTeacherTeach));
                    }
                }
            }
        }

        #endregion

        private void OnConfirmationChoice()
        {
            _teacherService.DeleteTeacher(SelectedTeacher.TeacherId);

            UpdateTeachers();
        }

        public void UpdateTeachers()
        {
            var teachers = _teacherService.GetAllTeachers();

            Teachers.Clear();

            foreach (var teacher in teachers)
            {
                Teachers.Add(teacher);
            }

            OnPropertyChanged(nameof(Teachers));
        }

        public void UpdateGroupsInWhichTheTeacherTeach()
        {
            var groupsInWhichTheTeacherTeach = _teacherService.GetGroupsInWhichTheTeacherTeach(SelectedTeacher.TeacherId);

            GroupsInWhichTheTeacherTeach.Clear();

            foreach (var group in groupsInWhichTheTeacherTeach)
            {
                var groupWithCourseNameModel = new GroupWithCourseNameModel(group, _courseService);
                GroupsInWhichTheTeacherTeach.Add(groupWithCourseNameModel);
            }

            OnPropertyChanged(nameof(GroupsInWhichTheTeacherTeach));
        }

        public TeacherManagementWindowViewModel(ITeacherService teacherService, ICourseService courseService)
        {
            #region Commands

            OpenCreateTeacherWindowCommand = new LambdaCommand(OnOpenCreateTeacherWindowCommandExecuted, CanOpenCreateTeacherWindowCommandExecuted);
            EditTeacherCommand = new LambdaCommand(OnEditTeacherCommandExecuted, CanEditTeacherCommandExecuted);
            DeleteTeacherCommand = new LambdaCommand(OnDeleteTeacherCommandExecuted, CanDeleteTeacherCommandExecuted);
            OpenAddGroupToTeacherWindowCommand = new LambdaCommand(OnOpenAddGroupToTeacherWindowCommandExecuted, CanOpenAddGroupToTeacherWindowCommandExecuted);

            #endregion

            _teacherService = teacherService;
            _courseService = courseService;

            var teachers = _teacherService.GetAllTeachers();

            foreach (var teacher in teachers)
            {
                Teachers.Add(teacher);
            }
        }
    }
}
