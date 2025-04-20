using System.Media;
using UniversityManagementSystem.ViewModels;
using UniversityManagementSystem.Views.Windows;

namespace UniversityManagementSystem.Helpers
{
    public class NotificationHelper
    {
        public void ShowWarning(string message)
        {
            var warningNotificationWindowViewModel = new WarningNotificationWindowViewModel(message);

            var warningNotificationWindow = new WarningNotificationWindow
            {
                DataContext = warningNotificationWindowViewModel
            };

            PlayWarningSound();

            warningNotificationWindow.ShowDialog();
        }

        public void ShowSuccess(string message)
        {
            var successNotificationWindowViewModel = new SuccessNotificationWindowViewModel(message);

            var successNotificationWindow = new SuccessNotificationWindow
            {
                DataContext = successNotificationWindowViewModel
            };

            PlaySuccessSound();

            successNotificationWindow.ShowDialog();
        }

        private void PlayWarningSound()
        {
            SystemSounds.Beep.Play(); 
        }

        private void PlaySuccessSound()
        {
            SystemSounds.Asterisk.Play();
        }


        public NotificationHelper() { }
    }
}
