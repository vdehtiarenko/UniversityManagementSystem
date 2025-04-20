using System.Windows;
using System.Windows.Input;
using UniversityManagementSystem.Infrastructure.Commands;
using UniversityManagementSystem.ViewModels.Base;

namespace UniversityManagementSystem.ViewModels
{
    internal class WarningNotificationWindowViewModel : ViewModel
    {
        #region Commands

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

        #region WarningMessage: string Warning message that will be displayed on the window

        private string _warningMessage;

        public string WarningMessage
        {
            get => _warningMessage;
            set
            {
                Set(ref _warningMessage, value);
            }
        }

        #endregion

        public WarningNotificationWindowViewModel(string warningMessage)
        {
            #region Commands

            CloseWindowCommand = new LambdaCommand(OnCloseWindowCommandExecuted, CanCloseWindowCommandExecuted);

            #endregion

            WarningMessage = warningMessage;

        }
    }
}
