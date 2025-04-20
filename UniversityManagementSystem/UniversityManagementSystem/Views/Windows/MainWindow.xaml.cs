using System.Windows;
using UniversityManagementSystem.ViewModels;

namespace UniversityManagementSystem
{
    public partial class MainWindow : Window
    {
        public MainWindow(MainWindowViewModel viewModel)
        {
            InitializeComponent();
            DataContext = viewModel;
        }
    }
}
