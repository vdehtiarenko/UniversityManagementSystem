using System.Collections.ObjectModel;
using System.Windows.Input;
using UniversityManagementSystem.Infrastructure.Commands;
using UniversityManagementSystem.ViewModels.Base;
using UniversityManagementSystem.Views.Windows;
using UniversityManagementSystem.DTO;
using System.Media;
using UniversityManagementSystem.Helpers;
using System.Windows;
using UniversityManagementSystem.DAL.Entities;
using UniversityManagementSystem.Services.ServicesInterfaces;

namespace UniversityManagementSystem.ViewModels
{
    public class MainWindowViewModel : ViewModel
    {
        private readonly ICourseService _courseService;
        private readonly IGroupService _groupService;
        private readonly ITeacherService _teacherService;
        private readonly IStudentService _studentService;

        public ObservableCollection<Course> Courses { get; set; } = new ObservableCollection<Course>();
        public ObservableCollection<Group> Groups { get; set; } = new ObservableCollection<Group>();
        public ObservableCollection<Student> Students { get; set; } = new ObservableCollection<Student>();

        public NotificationHelper NotificationHelper { get; set; } = new NotificationHelper();

        #region Commands

        #region OpenCreateGroupWindowCommand - Command to open the group creation window

        public ICommand OpenCreateGroupWindowCommand { get; }

        private bool CanOpenCreateGroupWindowCommandExecuted(object p) => true;

        private void OnOpenCreateGroupWindowCommandExecuted(object p)
        {
            if (SelectedCourse == null)
            {

                NotificationHelper.ShowWarning("Course is not selected! Select a course before creating a group.");
            }
            else
            {
                var groupCreateWindowViewModel = new GroupCreateWindowViewModel(SelectedCourse, _groupService, _teacherService);
                groupCreateWindowViewModel.GroupCreated += UpdateGroupList;

                var groupCreateWindow = new GroupCreateWindow
                {
                    DataContext = groupCreateWindowViewModel
                };

                groupCreateWindow.ShowDialog();
            }
        }

        #endregion

        #region OpenCreateStudentWindowCommand - Command to open student creation window

        public ICommand OpenCreateStudentWindowCommand { get; }

        private bool CanOpenCreateStudentWindowCommand(object p) => true;

        private void OnOpenCreateStudentWindowCommandExecuted(object p)
        {
            if (SelectedGroup == null)
            {
                NotificationHelper.ShowWarning("The group is not selected! Select a group before adding a student.");
            }
            else
            {
                var studentCreateWindowViewModel = new StudentCreateWindowViewModel(SelectedGroup, _studentService, _groupService);
                studentCreateWindowViewModel.StudentCreated += UpdateStudentList;

                var studentCreateWindow = new StudentCreateWindow
                {
                    DataContext = studentCreateWindowViewModel
                };

                studentCreateWindow.ShowDialog();
            }
        }

        #endregion

        #region OpenExportGroupsOfStudentsWindowCommand - A command that opens a window for exporting a list of students to various files

        public ICommand OpenExportGroupsOfStudentsWindowCommand { get; }

        private bool CanOpenExportGroupsOfStudentsWindowCommandExecuted(object p) => true;

        private void OnOpenExportGroupsOfStudentsWindowCommand(object p)
        {
            if (SelectedGroup == null)
            {
                NotificationHelper.ShowWarning("The group is not selected! Select a group before exporting students.");
            }
            else
            {
                var textDataModel = new TextDataModel(SelectedCourse, SelectedGroup, Students.ToList());

                var exportGroupsOfStudentsWindowViewModel = new ExportGroupsOfStudentsWindowViewModel(textDataModel, _studentService);

                var exportGroupsOfStudentsWinow = new ExportGroupsOfStudentsWindow
                {
                    DataContext = exportGroupsOfStudentsWindowViewModel
                };

                exportGroupsOfStudentsWinow.ShowDialog();
            }
        }

        #endregion

        #region OpenTeacherManagementWindowCommand - The command that opens the window for managing teachers

        public ICommand OpenTeacherManagementWindowCommand { get; }

        private bool CanOpenTeacherManagementWindowCommandExecuted(object p) => true;

        private void OnOpenTeacherManagementWindowCommandExecuted(object p)
        {
            var teacherManagementWindowViewModel = new TeacherManagementWindowViewModel(_teacherService, _courseService);

            var teacherManagementWindow = new TeacherManagementWindow
            {
                DataContext = teacherManagementWindowViewModel
            };

            teacherManagementWindow.ShowDialog();
        }

        #endregion 

        #region DeleteGroupCommand - Command to delete a group

        public ICommand DeleteGroupCommand { get; }

        private bool CanDeleteGroupCommandExecuted(object p) => true;

        private void OnDeleteGroupCommandExecuted(object p)
        {

            if (p is Group group)
            {
                SelectedGroup = group;

                if (Students.Count > 0)
                {
                    NotificationHelper.ShowWarning("The group could not be deleted because it contains at least one student.");
                }
                else
                {
                    var confirmationWindowViewModel = new ConfirmationWindowViewModel($"Do you want to delete the group Group name: {SelectedGroup.Name}");
                    confirmationWindowViewModel.ConfirmationChoice += OnConfirmationChoiceForGroup;

                    var confirmationWindow = new ConfirmationWindow
                    {
                        DataContext = confirmationWindowViewModel
                    };

                    confirmationWindow.ShowDialog();
                }
            }       
        }

        #endregion

        #region DeleteStudentCommand - Command to delete a student

        public ICommand DeleteStudentCommand { get; }

        private bool CanDeleteStudentCommandExecuted(object p)
        {
            return p != null;
        }

        private void OnDeleteStudentCommandExecuted(object p)
        {
            if (p is Student student)
            {
                SelectedStudent = student;
            }

            var confirmationWindowViewModel = new ConfirmationWindowViewModel($"Do you want to delete the Student: {SelectedStudent.FirstName} {SelectedStudent.LastName}");
            confirmationWindowViewModel.ConfirmationChoice += OnConfirmationChoiceForStudent;

            var confirmationWindow = new ConfirmationWindow
            {
                DataContext = confirmationWindowViewModel
            };

            confirmationWindow.ShowDialog();
        }

        #endregion

        #region EditGroupCommand - Command to edit a group

        public ICommand EditGroupCommand { get; }

        private bool CanEditGroupCommandExecuted(object p)
        {
            return p != null;
        }

        private void OnEditGroupCommandExecuted(object p)
        {
            if (p is Group group)
            {
                SelectedGroup = group;
            }

            var groupEditWindowViewModel = new GroupEditWindowViewModel(SelectedGroup, _groupService, _teacherService);
            groupEditWindowViewModel.GroupEdited += UpdateGroupList;

            var groupEditWindow = new GroupEditWindow
            {
                DataContext = groupEditWindowViewModel
            };

            groupEditWindow.ShowDialog();
        }

        #endregion

        #region EditStudentCommand - Command to edit a student

        public ICommand EditStudentCommand { get; }

        private bool CanEditStudentCommandExecuted(object p)
        {
            return p != null;
        }

        private void OnEditStudentCommandExecuted(object p)
        {
            if (p is Student student)
            {
                SelectedStudent = student;
            }

            var studentEditWindowViewModel = new StudentEditWindowViewModel(SelectedStudent, _studentService);

            studentEditWindowViewModel.StudentEdited += UpdateStudentList;

            var studentEditWindow = new StudentEditWindow
            {
                DataContext = studentEditWindowViewModel
            };

            studentEditWindow.ShowDialog();
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

        #region SelectedTabIndex : int Index of the selected Tab Item

        private int _selectedTabIndex;

        public int SelectedTabIndex
        {
            get => _selectedTabIndex;
            set => Set(ref _selectedTabIndex, value);
        }

        #endregion

        #region SelectedCourse: Course Select course from list

        private Course _selectedCourse;

        public Course SelectedCourse
        {
            get => _selectedCourse;
            set
            {
                if (Set(ref _selectedCourse, value))
                {
                    if (SelectedCourse != null)
                    {
                        var groups = _groupService.GetCourseGroups(SelectedCourse.CourseId);

                        Groups.Clear();
                        Students.Clear();

                        foreach (var group in groups)
                        {
                            Groups.Add(group);
                        }

                        SelectedTabIndex = 1;
                    }
                }
            }
        }

        #endregion

        #region SelectedGroup: Group Select group from list

        private Group _selectedGroup;

        public Group SelectedGroup
        {
            get => _selectedGroup;
            set
            {
                if (Set(ref _selectedGroup, value))
                {
                    if (SelectedGroup != null)
                    {
                        var students = _groupService.GetStudentsByGroup(SelectedGroup.GroupId);

                        Students.Clear();

                        foreach (var student in students)
                        {
                            Students.Add(student);
                        }
                    }
                }
            }
        }

        #endregion

        #region SelectedStudent: Student Select student from list

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

        private void UpdateGroupList()
        {
            var updatedGroups = _groupService.GetCourseGroups(SelectedCourse.CourseId);

            Groups.Clear();
            foreach (var group in updatedGroups)
            {
                Groups.Add(group);
            }

            OnPropertyChanged(nameof(Groups));
        }

        private void UpdateStudentList()
        {
            var updatedStudents = _groupService.GetStudentsByGroup(SelectedGroup.GroupId);

            Students.Clear();
            foreach (var student in updatedStudents)
            {
                Students.Add(student);
            }

            OnPropertyChanged(nameof(Students));
        }

        private void UpdateCourseList()
        {
            var updatedCourses = _courseService.GetAllCourses();

            Courses.Clear();
            foreach (var course in updatedCourses)
            {
                Courses.Add(course);
            }

            OnPropertyChanged(nameof(Courses));
        }

        private void OnConfirmationChoiceForGroup()
        {
            _groupService.DeleteGroup(SelectedGroup.GroupId);

            NotificationHelper.ShowSuccess("The group was successfully deleted.");

            UpdateGroupList();
        }

        private void OnConfirmationChoiceForStudent()
        {
            _studentService.DeleteStudent(SelectedStudent.StudentId);

            NotificationHelper.ShowSuccess("The student was successfully deleted.");

            UpdateStudentList();
        }

        private void PlayWarningSound()
        {
            SystemSounds.Beep.Play();
        }

        public MainWindowViewModel(ICourseService courseService, IGroupService groupService, IStudentService studentService, ITeacherService teacherService)
        {
            #region Commands

            OpenCreateGroupWindowCommand = new LambdaCommand(OnOpenCreateGroupWindowCommandExecuted, CanOpenCreateGroupWindowCommandExecuted);
            OpenCreateStudentWindowCommand = new LambdaCommand(OnOpenCreateStudentWindowCommandExecuted, CanOpenCreateStudentWindowCommand);
            OpenTeacherManagementWindowCommand = new LambdaCommand(OnOpenTeacherManagementWindowCommandExecuted, CanOpenTeacherManagementWindowCommandExecuted);
            OpenExportGroupsOfStudentsWindowCommand = new LambdaCommand(OnOpenExportGroupsOfStudentsWindowCommand, CanOpenExportGroupsOfStudentsWindowCommandExecuted);

            EditGroupCommand = new LambdaCommand(OnEditGroupCommandExecuted, CanEditGroupCommandExecuted);
            EditStudentCommand = new LambdaCommand(OnEditStudentCommandExecuted, CanEditStudentCommandExecuted);
            DeleteGroupCommand = new LambdaCommand(OnDeleteGroupCommandExecuted, CanDeleteGroupCommandExecuted);
            DeleteStudentCommand = new LambdaCommand(OnDeleteStudentCommandExecuted, CanDeleteStudentCommandExecuted);
            CloseWindowCommand = new LambdaCommand(OnCloseWindowCommandExecuted, CanCloseWindowCommandExecuted);

            #endregion

            _courseService = courseService;
            _groupService = groupService;
            _teacherService = teacherService;
            _studentService = studentService;

            var courses = _courseService.GetAllCourses();

            foreach (var course in courses)
            {
                Courses.Add(course);
            }

            SelectedTabIndex = 0;
        }
    }
}
