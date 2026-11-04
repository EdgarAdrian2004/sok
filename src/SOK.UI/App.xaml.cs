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
using SOK.UI.Services;
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
            services.AddSingleton<IRegistryRestoreService, RegistryRestoreService>();
            services.AddSingleton<ITelemetryRegistryService, TelemetryRegistryService>();
            services.AddSingleton<IServiceControlService, ServiceControlService>();
            services.AddSingleton<IMemoryProvider, MemoryProvider>();
            services.AddSingleton<IMemoryReleaseService, MemoryReleaseService>();
            services.AddSingleton(TimeProvider.System);
            services.AddSingleton<ILogService, LogService>();
            services.AddSingleton<IBackupManager, BackupManager>();
            services.AddSingleton<ITelemetryManager, TelemetryManager>();
            services.AddSingleton<IServiceManager, ServiceManager>();
            services.AddSingleton<IMemoryManager, MemoryManager>();
            services.AddSingleton<IProfileManager, ProfileManager>();
            services.AddSingleton<IRestoreManager, RestoreManager>();
            services.AddSingleton<ITutorialService, TutorialService>();

            services.AddSingleton<IFileDialogService, FileDialogService>();
            services.AddSingleton<IUserDialogService, UserDialogService>();
            services.AddSingleton<ThemeService>();

            services.AddSingleton<INavigationService, NavigationService>();
            services.AddSingleton<MainViewModel>();
            services.AddSingleton<DashboardViewModel>();
            services.AddSingleton<TelemetryViewModel>();
            services.AddSingleton<ServicesViewModel>();
            services.AddSingleton<MemoryViewModel>();
            services.AddSingleton<ProfilesViewModel>();
            services.AddSingleton<BackupsViewModel>();
            services.AddSingleton<TutorialViewModel>();

            Ioc.Default.ConfigureServices(services.BuildServiceProvider());

            Ioc.Default.GetRequiredService<ThemeService>().ApplySaved();

            var window = new MainWindow
            {
                DataContext = Ioc.Default.GetRequiredService<MainViewModel>()
            };

            MainWindow = window;
            window.Show();

            ShowTutorialOnFirstRun();
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

    /// <summary>
    /// Abre el tutorial automáticamente la primera vez que se ejecuta la
    /// aplicación. La marca se guarda al completar o saltar la guía.
    /// </summary>
    private static void ShowTutorialOnFirstRun()
    {
        try
        {
            var settings = Ioc.Default.GetRequiredService<ISettingsRepository>();
            var completed = settings
                .GetAsync(TutorialViewModel.TutorialCompletedKey)
                .GetAwaiter()
                .GetResult();

            if (!string.Equals(completed?.Value, "true", StringComparison.OrdinalIgnoreCase))
            {
                Ioc.Default
                    .GetRequiredService<INavigationService>()
                    .RequestNavigation("Tutorial");
            }
        }
        catch
        {
            // Si la configuración no se puede leer, la aplicación sigue
            // funcionando y el tutorial queda disponible en el menú.
        }
    }
}
