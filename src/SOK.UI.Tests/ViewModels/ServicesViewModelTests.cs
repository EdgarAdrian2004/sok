using Moq;
using SOK.Application.Abstractions;
using SOK.Application.DTOs;
using SOK.Domain.Entities;
using SOK.Domain.Enums;
using SOK.UI.ViewModels;
using Xunit;

namespace SOK.UI.Tests.ViewModels;

public sealed class ServicesViewModelTests
{
    [Fact]
    public async Task CargaInicial_AgrupaPorCategoriaYCuentaSoloServiciosInstalados()
    {
        var manager = CreateManager(
            CreateEntry("a", ServiceCategory.Telemetria, ServiceStatus.Habilitado),
            CreateEntry("b", ServiceCategory.Telemetria, ServiceStatus.Deshabilitado),
            CreateEntry("c", ServiceCategory.Obsoleto, ServiceStatus.Habilitado),
            CreateEntry("d", ServiceCategory.Obsoleto, ServiceStatus.NoAplicable));

        var viewModel = new ServicesViewModel(manager.Object);
        await viewModel.InitializationTask;

        Assert.Equal(2, viewModel.Groups.Count);
        Assert.Equal("1 de 3 servicios deshabilitados.", viewModel.SummaryText);
        Assert.Equal(3, viewModel.Categories.Count);
        Assert.Contains("Catálogo de servicios cargado correctamente.", viewModel.StatusMessage);
    }

    [Fact]
    public async Task Filtro_MuestraSoloLaCategoriaSeleccionada()
    {
        var manager = CreateManager(
            CreateEntry("a", ServiceCategory.Telemetria, ServiceStatus.Habilitado),
            CreateEntry("c", ServiceCategory.Obsoleto, ServiceStatus.Habilitado));

        var viewModel = new ServicesViewModel(manager.Object);
        await viewModel.InitializationTask;

        viewModel.SelectedCategory = "Servicios obsoletos";

        Assert.Single(viewModel.Groups);
        Assert.Equal("Servicios obsoletos", viewModel.Groups[0].Name);
    }

    [Fact]
    public async Task Toggle_Exitoso_ActualizaEstadoYResumen()
    {
        var entry = CreateEntry("a", ServiceCategory.Telemetria, ServiceStatus.Habilitado);
        var manager = CreateManager(entry);
        manager
            .Setup(m => m.SetDisabledAsync("a", true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ServiceOperationResult.Completed(
                1,
                0,
                @"C:\backups\services-backup.json",
                Array.Empty<string>(),
                "«Servicio a» se deshabilitó correctamente."));

        var viewModel = new ServicesViewModel(manager.Object);
        await viewModel.InitializationTask;

        var item = viewModel.Groups[0].Items[0];
        await viewModel.OnItemToggleAsync(item, disabled: true);

        Assert.True(item.IsDisabled);
        Assert.Equal(ServiceStatus.Deshabilitado, item.Status);
        Assert.Equal(ServiceStartMode.Deshabilitado, item.Entry.StartMode);
        Assert.Equal("1 de 1 servicios deshabilitados.", viewModel.SummaryText);
        Assert.Contains("se deshabilitó", viewModel.StatusMessage);
        Assert.Contains("services-backup.json", viewModel.LastBackupText);
    }

    [Fact]
    public async Task Toggle_Fallido_RevierteElInterruptor()
    {
        var entry = CreateEntry("a", ServiceCategory.Telemetria, ServiceStatus.Habilitado);
        var manager = CreateManager(entry);
        manager
            .Setup(m => m.SetDisabledAsync("a", true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ServiceOperationResult.Completed(
                0,
                1,
                @"C:\backups\services-backup.json",
                new[] { "Servicio a: acceso denegado." },
                "No se pudo cambiar «Servicio a»: acceso denegado."));

        var viewModel = new ServicesViewModel(manager.Object);
        await viewModel.InitializationTask;

        var item = viewModel.Groups[0].Items[0];
        await viewModel.OnItemToggleAsync(item, disabled: true);

        Assert.False(item.IsDisabled);
        Assert.Equal(ServiceStatus.Habilitado, item.Status);
        Assert.Contains("No se pudo cambiar", viewModel.StatusMessage);
        Assert.Equal("0 de 1 servicios deshabilitados.", viewModel.SummaryText);
    }

    [Fact]
    public async Task ServicioNoInstalado_NoPermiteCambiarElInterruptor()
    {
        var manager = CreateManager(
            CreateEntry("a", ServiceCategory.Obsoleto, ServiceStatus.NoAplicable));

        var viewModel = new ServicesViewModel(manager.Object);
        await viewModel.InitializationTask;

        var item = viewModel.Groups[0].Items[0];

        Assert.False(item.CanToggle);
        Assert.Equal("No instalado en este equipo", item.StatusText);
        Assert.Equal("Sin servicios del catálogo disponibles en este equipo.", viewModel.SummaryText);
    }

    [Fact]
    public async Task AplicarPerfilGamer_InvocaAlGestorYRecargaElCatalogo()
    {
        var manager = CreateManager(
            CreateEntry("a", ServiceCategory.Telemetria, ServiceStatus.Habilitado));
        manager
            .Setup(m => m.ApplyProfileAsync(ServiceProfile.Gamer, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ServiceOperationResult.Completed(
                1,
                0,
                @"C:\backups\services-backup.json",
                Array.Empty<string>(),
                "Perfil Gamer aplicado: 1 servicios deshabilitados."));

        var viewModel = new ServicesViewModel(manager.Object);
        await viewModel.InitializationTask;

        await viewModel.ApplyGamerProfileCommand.ExecuteAsync(null);

        manager.Verify(
            m => m.ApplyProfileAsync(ServiceProfile.Gamer, It.IsAny<CancellationToken>()),
            Times.Once);
        manager.Verify(
            m => m.GetCatalogAsync(It.IsAny<CancellationToken>()),
            Times.Exactly(2));
        Assert.Contains("Perfil Gamer aplicado", viewModel.StatusMessage);
    }

    [Fact]
    public async Task AplicarPerfilOfimatico_ReportaFallosParciales()
    {
        var manager = CreateManager(
            CreateEntry("a", ServiceCategory.Xbox, ServiceStatus.Habilitado));
        manager
            .Setup(m => m.ApplyProfileAsync(ServiceProfile.Ofimatico, It.IsAny<CancellationToken>()))
            .ReturnsAsync(ServiceOperationResult.Completed(
                0,
                1,
                @"C:\backups\services-backup.json",
                new[] { "Servicio a: acceso denegado." },
                "Perfil Ofimático aplicado parcialmente: 0 correctos y 1 con error."));

        var viewModel = new ServicesViewModel(manager.Object);
        await viewModel.InitializationTask;

        await viewModel.ApplyOfimaticoProfileCommand.ExecuteAsync(null);

        Assert.Contains("aplicado parcialmente", viewModel.StatusMessage);
        Assert.Contains("acceso denegado", viewModel.StatusMessage);
    }

    private static Mock<IServiceManager> CreateManager(params ServiceEntry[] entries)
    {
        var manager = new Mock<IServiceManager>();
        manager
            .Setup(m => m.GetCatalogAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(entries.ToList());
        return manager;
    }

    private static ServiceEntry CreateEntry(
        string id,
        ServiceCategory category,
        ServiceStatus status)
    {
        var startMode = status switch
        {
            ServiceStatus.Deshabilitado => ServiceStartMode.Deshabilitado,
            ServiceStatus.NoAplicable => ServiceStartMode.Desconocido,
            ServiceStatus.Desconocido => ServiceStartMode.Desconocido,
            _ => ServiceStartMode.Manual
        };

        return new ServiceEntry
        {
            Id = id,
            DisplayName = $"Servicio {id}",
            Description = $"Descripción del servicio {id}.",
            Category = category,
            DefaultStartMode = ServiceStartMode.Manual,
            Exists = status != ServiceStatus.NoAplicable,
            StartMode = startMode,
            State = status == ServiceStatus.NoAplicable
                ? ServiceState.Desconocido
                : ServiceState.Detenido
        };
    }
}
