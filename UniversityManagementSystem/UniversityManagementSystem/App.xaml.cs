using Microsoft.Extensions.DependencyInjection;
using System.Windows;
using UniversityManagementSystem.Services;
using UniversityManagementSystem.ViewModels;

namespace UniversityManagementSystem
{
    public partial class App : System.Windows.Application
    { 
        public static IServiceProvider ServiceProvider { get; private set; }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);

            ServiceProvider = serviceCollection.BuildServiceProvider();

            var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            ServiceRegistry.RegisterServices(services);

            services.AddSingleton<MainWindow>();
            services.AddSingleton<MainWindowViewModel>();
        }
    }
}
