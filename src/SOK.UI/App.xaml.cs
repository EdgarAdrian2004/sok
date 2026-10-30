using System.Windows;
using CommunityToolkit.Mvvm.DependencyInjection;
using Microsoft.Extensions.DependencyInjection;
using SOK.Application.Abstractions;
using SOK.Application.Common;
using SOK.Application.Services;
using SOK.Domain.Abstractions;
using SOK.Infrastructure.Common;
using SOK.Infrastructure.Persistence;
using SOK.Infrastructure.Persistence.Repositories;
using SOK.Infrastructure.Windows;
using SOK.UI.Navigation;
using SOK.UI.ViewModels;

namespace SOK.UI;

/// <summary>
/// Punto de entrada de SOK. Inicializa la base de datos, registra los
/// servicios y muestra la ventana principal.
/// </summary>
public partial class App : System.Windows.Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        try
        {
            var dbContext = new SokDbContext(SokPaths.DatabasePath);
            dbContext.InitializeAsync().GetAwaiter().GetResult();

            var services = new ServiceCollection();

            services.AddSingleton(dbContext);
            services.AddSingleton<IAppPathProvider, AppPathProvider>();

            services.AddSingleton<IProfileRepository, ProfileRepository>();
            services.AddSingleton<IProfileSettingRepository, ProfileSettingRepository>();
            services.AddSingleton<IBackupRepository, BackupRepository>();
            services.AddSingleton<ILogRepository, LogRepository>();
            services.AddSingleton<ISettingsRepository, SettingsRepository>();

            services.AddSingleton<ISystemInfoProvider, SystemInfoProvider>();
            services.AddSingleton<IRegistryBackupService, RegistryBackupService>();
            services.AddSingleton<IServicesBackupService, ServicesBackupService>();
            services.AddSingleton<ILogService, LogService>();
            services.AddSingleton<IBackupManager, BackupManager>();

            services.AddSingleton<INavigationService, NavigationService>();
            services.AddSingleton<MainViewModel>();
            services.AddSingleton<DashboardViewModel>();

            Ioc.Default.ConfigureServices(services.BuildServiceProvider());

            var window = new MainWindow
            {
                DataContext = Ioc.Default.GetRequiredService<MainViewModel>()
            };

            MainWindow = window;
            window.Show();
        }
        catch (Exception ex)
        {
            MessageBox.Show(
                $"No se pudo iniciar SOK:\n\n{ex.Message}",
                "SOK — Error de inicio",
                MessageBoxButton.OK,
                MessageBoxImage.Error);

            Shutdown(1);
        }
    }
}
