using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using UniversityManagementSystem.DAL.Entities;
using UniversityManagementSystem.Helpers;
using UniversityManagementSystem.Infrastructure.Commands;
using UniversityManagementSystem.ViewModels.Base;
using UniversityManagementSystem.Views.Windows;
using UniversityManagementSystem.Services.ServicesInterfaces;

namespace UniversityManagementSystem.ViewModels
{
    internal class GroupEditWindowViewModel : ViewModel
    {
        private readonly IGroupService _groupService;
        private readonly ITeacherService _teacherService;

        public ObservableCollection<Teacher> Teachers { get; set; } = new ObservableCollection<Teacher>();
        public ObservableCollection<Teacher> SelectedTeachers { get; set; } = new ObservableCollection<Teacher>();

        public NotificationHelper NotificationHelper { get; set; } = new NotificationHelper();

        #region Commands

        #region EditGroupCommand - Command to edit existing group

        public ICommand EditGroupCommand { get; }

        private bool CanEditGroupCommandExecuted(object p) => true;


        private void OnEditGroupCommandExecuted(object p)
        {
            if (SelectedGroup.Name == null)
            {
                NotificationHelper.ShowWarning("The group cannot be edited. The 'Name' field cannot be empty.");
            }
            else if (SelectedTeachers.Count == 0)
            {
                NotificationHelper.ShowWarning("You cannot edit a group. A group must have at least one teacher.");
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
            };
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

        #region GroupEdited Event - Event triggered when a group is edited

        public event Action GroupEdited;

        private void OnGroupEdited()
        {
            GroupEdited?.Invoke();
        }

        #endregion

        #endregion

        #region SelectedGroup: Group Select group for editing

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

        #region Name: string Name of the selected group

        private string _name;

        public string Name 
        {
            get => _name;
            set
            {
                Set(ref _name, value);
            }
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

            SelectedGroup.Name = Name;

            var editedGroup = _groupService.UpdateGroup(SelectedGroup.GroupId, SelectedGroup.Name, selectedTeachersId);

            if (editedGroup != null)
            {
                GroupEdited();
            }

            NotificationHelper.ShowSuccess("The group has been successfully edited.");

            var window = System.Windows.Application.Current.Windows.OfType<Window>().FirstOrDefault(w => w.DataContext == this);
            window?.Close();
        }

        public GroupEditWindowViewModel(Group selectedGroup, IGroupService groupService, ITeacherService teacherService)
        {
            #region Commands

            AddSelectedTeacherCommand = new LambdaCommand(OnAddSelectedTeacherCommand, CanAddSelectedTeacherCommandExecuted);
            RemoveSelectedTeacherCommand = new LambdaCommand(OnRemoveSelectedTeacherCommandExecuted, CanRemoveSelectedTeacherCommandExecuted);
            EditGroupCommand = new LambdaCommand(OnEditGroupCommandExecuted, CanEditGroupCommandExecuted);
            CloseWindowCommand = new LambdaCommand(OnCloseWindowCommandExecuted, CanCloseWindowCommandExecuted);

            #endregion

            _groupService = groupService;
            _teacherService = teacherService;

            SelectedGroup = selectedGroup;

            Name = SelectedGroup.Name;

            var teachers = _teacherService.GetAllTeachers();

            foreach (var teacher in teachers)
            {
                Teachers.Add(teacher);
            }

            var selectedTeachers = _groupService.GetTeachersByGroup(SelectedGroup.GroupId);

            foreach (var teacher in selectedTeachers)
            {
                SelectedTeachers.Add(teacher);
            }
        }
    }
}
