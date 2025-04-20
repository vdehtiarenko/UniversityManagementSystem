using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using UniversityManagementSystem.DAL.Entities;
using UniversityManagementSystem.Helpers;
using UniversityManagementSystem.Infrastructure.Commands;
using UniversityManagementSystem.ViewModels.Base;
using UniversityManagementSystem.Views.Windows;
using UniversityManagementSystem.Services.ServicesInterfaces;
using UniversityManagementSystem.DTO;

namespace UniversityManagementSystem.ViewModels
{
    internal class AddingGroupToTeacherWindowViewModel : ViewModel
    {
        private readonly ITeacherService _teacherService;

        public ObservableCollection<GroupWithCourseNameModel> Groups { get; set; } = new ObservableCollection<GroupWithCourseNameModel>();
        public ObservableCollection<GroupWithCourseNameModel> SelectedGroups { get; set; } = new ObservableCollection<GroupWithCourseNameModel>();

        public NotificationHelper NotificationHelper { get; set; } = new NotificationHelper();

        #region Commands 

        #region AddGroupToTeacherCommand - Command to add a group to a teacher

        public ICommand AddGroupToTeacherCommand { get; }

        private bool CanAddGroupToTeacherCommandExecuted(object p) => true;

        private void OnAddGroupToTeacherCommandExecuted(object p)
        {
            if (SelectedGroups.Count < 1)
            {
                NotificationHelper.ShowWarning("No groups are selected! Select at least one group for the teacher.");
            }
            else
            {
                var confirmationWindowViewModel = new ConfirmationWindowViewModel("Do you want add group to teacher ?");
                confirmationWindowViewModel.ConfirmationChoice += OnConfirmationChoice;

                var confirmationWindow = new ConfirmationWindow
                {
                    DataContext = confirmationWindowViewModel
                };

                confirmationWindow.ShowDialog();
            }
        }

        #endregion

        #region AddSelectedGroupCommand - Command to add the selected group to the list

        public ICommand AddSelectedGroupCommand { get; }

        private bool CanAddSelectedGroupCommandExecuted(object p)
        {
            return p != null;
        }

        private void OnAddSelectedGroupCommandCommand(object p)
        {
            if (p is GroupWithCourseNameModel group)
            {
                bool isDuplicate = SelectedGroups.Any(g =>
                    g.Group.CourseId == group.Group.CourseId &&
                    g.Group.GroupId == group.Group.GroupId);

                if (!isDuplicate)
                {
                    SelectedGroups.Add(group);
                }
            }
        }


        #endregion

        #region RemoveSelectedGroupCommand - Command to remove a selected group from the list 

        public ICommand RemoveSelectedGroupCommand { get; }

        private bool CanRemoveSelectedGroupCommandExecuted(object p)
        {
            return p != null;
        }

        private void OnRemoveSelectedGroupCommandExecuted(object p)
        {
            var group = p as GroupWithCourseNameModel;

            if (group != null)
            {
                var groupsToRemove = SelectedGroups.FirstOrDefault(g => g.Group.GroupId == group.Group.GroupId && g.Group.CourseId == group.Group.CourseId);

                if (groupsToRemove != null)
                {
                    SelectedGroups.Remove(groupsToRemove);
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

        #region GroupAdded Event - Event triggered when a group added to a teacher

        public event Action GroupAdded;

        private void OnGroupAdded()
        {
            GroupAdded?.Invoke();
        }

        #endregion

        #endregion

        #region SelectedTeacher: Teacher Teacher selected for group assignment

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
                        var groups = _teacherService.GetGroupsInWhichTheTeacherTeach(SelectedTeacher.TeacherId);

                        Groups.Clear();

                        foreach (var group in Groups)
                        {
                            Groups.Add(group);
                        }

                        OnPropertyChanged(nameof(Groups));
                    }
                }
            }
        }

        #endregion

        private void OnConfirmationChoice()
        {
            var newGroupCourseTeachers = new List<GroupCourseTeacher>();

            foreach (var group in SelectedGroups)
            {
                var groupCourseTeacher = new GroupCourseTeacher(group.Group.GroupId, group.Group.CourseId, SelectedTeacher.TeacherId);

                newGroupCourseTeachers.Add(groupCourseTeacher);
            }

            _teacherService.UpdateTeacherGroups(SelectedTeacher.TeacherId, newGroupCourseTeachers);

            OnGroupAdded();

            NotificationHelper.ShowSuccess("The groups were successfully assigned to the teacher.");

            var window = System.Windows.Application.Current.Windows.OfType<Window>().FirstOrDefault(w => w.DataContext == this);
            window?.Close();
        }

        public AddingGroupToTeacherWindowViewModel(Teacher selectedTeacher, ITeacherService teacherService, ICourseService courseService)
        {
            #region Command

            AddGroupToTeacherCommand = new LambdaCommand(OnAddGroupToTeacherCommandExecuted, CanAddGroupToTeacherCommandExecuted);
            AddSelectedGroupCommand = new LambdaCommand(OnAddSelectedGroupCommandCommand, CanAddSelectedGroupCommandExecuted);
            RemoveSelectedGroupCommand = new LambdaCommand(OnRemoveSelectedGroupCommandExecuted, CanRemoveSelectedGroupCommandExecuted);
            CloseWindowCommand = new LambdaCommand(OnCloseWindowCommandExecuted, CanCloseWindowCommandExecuted);

            #endregion

            _teacherService = teacherService;

            SelectedTeacher = selectedTeacher;

            var groups = _teacherService.GetAllGroups();

            foreach (var group in groups)
            {
                var groupWithCourseNameModel = new GroupWithCourseNameModel(group, courseService);
                Groups.Add(groupWithCourseNameModel);
            }

            var selectedGroups = _teacherService.GetGroupsInWhichTheTeacherTeach(SelectedTeacher.TeacherId);

            foreach (var group in selectedGroups)
            {
                var groupWithCourseNameModel = new GroupWithCourseNameModel(group, courseService);
                SelectedGroups.Add(groupWithCourseNameModel);
            }
        }
    }
}
