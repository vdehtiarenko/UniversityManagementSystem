using System.Windows;
using System.Windows.Input;
using UniversityManagementSystem.Infrastructure.Commands;
using UniversityManagementSystem.ViewModels.Base;

namespace UniversityManagementSystem.ViewModels
{
    internal class SuccessNotificationWindowViewModel : ViewModel
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

        #region SuccessMessage: string Successg message that will be displayed on the window

        private string _successMessage;

        public string SuccessMessage
        {
            get => _successMessage;
            set
            {
                Set(ref _successMessage, value);
            }
        }

        #endregion

        public SuccessNotificationWindowViewModel(string successMessage)
        {
            #region Commands

            CloseWindowCommand = new LambdaCommand(OnCloseWindowCommandExecuted, CanCloseWindowCommandExecuted);

            #endregion

            SuccessMessage = successMessage;
        }
    }
}
