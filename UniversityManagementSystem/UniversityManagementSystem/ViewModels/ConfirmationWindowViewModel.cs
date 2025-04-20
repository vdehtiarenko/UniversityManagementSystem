using System.Windows;
using System.Windows.Input;
using UniversityManagementSystem.Infrastructure.Commands;
using UniversityManagementSystem.ViewModels.Base;

namespace UniversityManagementSystem.ViewModels
{
    internal class ConfirmationWindowViewModel : ViewModel
    {
        #region Commands

        #region SelectionConfirmationCommand - Command to confirm the selection 

        public ICommand SelectionConfirmationCommand { get; }

        private bool CanSelectionConfirmationCommandExecuted(object p) => true;

        private void OnSelectionConfirmationCommandExecuted(object p)
        {
            OnConfirmationChoice();

            var window = System.Windows.Application.Current.Windows.OfType<Window>().FirstOrDefault(w => w.DataContext == this);
            window?.Close();
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

        #region ConfirmationChoice Event - Event that notifies about the choice

        public event Action ConfirmationChoice;

        private void OnConfirmationChoice()
        {
            ConfirmationChoice?.Invoke();
        }

        #endregion

        #endregion

        #region Message: string Message that will be displayed on the window

        private string _message;

        public  string Message 
        {
            get => _message;
            set
            {
                Set(ref _message, value); 
            }
        }

        #endregion

        public ConfirmationWindowViewModel(string message) 
        {
            #region Commands

            SelectionConfirmationCommand = new LambdaCommand(OnSelectionConfirmationCommandExecuted, CanSelectionConfirmationCommandExecuted);
            CloseWindowCommand = new LambdaCommand(OnCloseWindowCommandExecuted, CanCloseWindowCommandExecuted);
            
            #endregion

            Message = message;  
        }
    }
}
