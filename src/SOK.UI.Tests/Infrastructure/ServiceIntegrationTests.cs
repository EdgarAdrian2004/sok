using SOK.Domain.Enums;
using SOK.Infrastructure.Windows;
using Xunit;

namespace SOK.UI.Tests.Infrastructure;

/// <summary>
/// Pruebas de integración con el Service Control Manager. Solo leen el
/// estado del equipo y generan respaldos, por lo que no requieren
/// elevación ni modifican servicios reales.
/// </summary>
public sealed class ServiceIntegrationTests : IDisposable
{
    private const string ServicioConocido = "Dnscache";
    private const string SegundoServicio = "Spooler";

    private readonly string _tempDirectory = Path.Combine(
        Path.GetTempPath(),
        "sok-service-integration-" + Guid.NewGuid().ToString("N"));

    [Fact]
    public async Task GetAllAsync_DevuelveLosServiciosDelEquipo()
    {
        var service = new ServiceControlService();

        var services = await service.GetAllAsync();

        Assert.NotEmpty(services);
        Assert.All(services, snapshot => Assert.False(string.IsNullOrWhiteSpace(snapshot.Name)));
        Assert.Contains(
            services,
            snapshot => string.Equals(snapshot.Name, ServicioConocido, StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task GetAsync_DevuelveElServicioSolicitado()
    {
        var service = new ServiceControlService();

        var snapshot = await service.GetAsync(ServicioConocido);

        Assert.NotNull(snapshot);
        Assert.Equal(ServicioConocido, snapshot!.Name, ignoreCase: true);
        Assert.NotEqual(ServiceStartMode.Desconocido, snapshot.StartMode);
        Assert.NotEqual(ServiceState.Desconocido, snapshot.State);
    }

    [Fact]
    public async Task GetAsync_DevuelveNullCuandoElServicioNoExiste()
    {
        var service = new ServiceControlService();

        var snapshot = await service.GetAsync("SOK_ServicioInexistente_" + Guid.NewGuid().ToString("N"));

        Assert.Null(snapshot);
    }

    [Fact]
    public async Task CreateBackupForServicesAsync_ExportaSoloLosServiciosIndicados()
    {
        var service = new ServicesBackupService();

        var filePath = await service.CreateBackupForServicesAsync(
            new[] { ServicioConocido, SegundoServicio },
            _tempDirectory);

        Assert.True(File.Exists(filePath));

        var json = await File.ReadAllTextAsync(filePath);
        Assert.Contains($"\"Name\": \"{ServicioConocido}\"", json);
        Assert.Contains($"\"Name\": \"{SegundoServicio}\"", json);
        Assert.DoesNotContain("Winmgmt", json);
    }

    [Fact]
    public async Task CreateBackupForServicesAsync_SinServiciosExistentes_LanzaInvalidOperationException()
    {
        var service = new ServicesBackupService();

        await Assert.ThrowsAsync<InvalidOperationException>(
            () => service.CreateBackupForServicesAsync(
                new[] { "SOK_ServicioInexistente_" + Guid.NewGuid().ToString("N") },
                _tempDirectory));
    }

    [Fact]
    public async Task CreateBackupForServicesAsync_SinNombres_LanzaArgumentException()
    {
        var service = new ServicesBackupService();

        await Assert.ThrowsAsync<ArgumentException>(
            () => service.CreateBackupForServicesAsync(Array.Empty<string>(), _tempDirectory));
    }

    public void Dispose()
    {
        if (Directory.Exists(_tempDirectory))
        {
            Directory.Delete(_tempDirectory, recursive: true);
        }
    }
}
