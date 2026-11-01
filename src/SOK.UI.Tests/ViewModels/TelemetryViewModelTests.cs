using Moq;
using SOK.Application.Abstractions;
using SOK.Application.DTOs;
using SOK.Domain.Entities;
using SOK.Domain.Enums;
using SOK.UI.ViewModels;
using Xunit;

namespace SOK.UI.Tests.ViewModels;

public sealed class TelemetryViewModelTests
{
    [Fact]
    public async Task CargaInicial_AgrupaLasClavesPorCategoria()
    {
        var manager = CreateManager(
            CreateKey("a", TelemetryCategory.Sistema, TelemetryStatus.Habilitado),
            CreateKey("b", TelemetryCategory.Sistema, TelemetryStatus.Deshabilitado),
            CreateKey("c", TelemetryCategory.Publicidad, TelemetryStatus.Habilitado));

        var viewModel = new TelemetryViewModel(manager.Object);
        await viewModel.InitializationTask;

        Assert.Equal(2, viewModel.Groups.Count);
        Assert.Contains("1 de 3", viewModel.SummaryText);
        Assert.Equal(3, viewModel.Categories.Count);
        Assert.Contains("Catálogo de telemetría cargado correctamente.", viewModel.StatusMessage);
    }

    [Fact]
    public async Task Filtro_MuestraSoloLaCategoriaSeleccionada()
    {
        var manager = CreateManager(
            CreateKey("a", TelemetryCategory.Sistema, TelemetryStatus.Habilitado),
            CreateKey("c", TelemetryCategory.Publicidad, TelemetryStatus.Habilitado));

        var viewModel = new TelemetryViewModel(manager.Object);
        await viewModel.InitializationTask;

        viewModel.SelectedCategory = "Publicidad";

        Assert.Single(viewModel.Groups);
        Assert.Equal("Publicidad", viewModel.Groups[0].Name);
    }

    [Fact]
    public async Task Toggle_Exitoso_ActualizaEstadoYResumen()
    {
        var key = CreateKey("a", TelemetryCategory.Sistema, TelemetryStatus.Habilitado);
        var manager = CreateManager(key);
        manager
            .Setup(m => m.SetDisabledAsync("a", true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(TelemetryOperationResult.Completed(
                1,
                0,
                @"C:\backups\manifest.json",
                Array.Empty<string>(),
                "«Clave a» se deshabilitó correctamente."));

        var viewModel = new TelemetryViewModel(manager.Object);
        await viewModel.InitializationTask;

        var item = viewModel.Groups[0].Items[0];
        await viewModel.OnItemToggleAsync(item, disabled: true);

        Assert.True(item.IsDisabled);
        Assert.Equal(TelemetryStatus.Deshabilitado, item.Status);
        Assert.Contains("1 de 1", viewModel.SummaryText);
        Assert.Contains("se deshabilitó", viewModel.StatusMessage);
        Assert.Contains("manifest.json", viewModel.LastBackupText);
    }

    [Fact]
    public async Task Toggle_Fallido_RevierteElInterruptor()
    {
        var key = CreateKey("a", TelemetryCategory.Sistema, TelemetryStatus.Habilitado);
        var manager = CreateManager(key);
        manager
            .Setup(m => m.SetDisabledAsync("a", true, It.IsAny<CancellationToken>()))
            .ReturnsAsync(TelemetryOperationResult.Completed(
                0,
                1,
                @"C:\backups\manifest.json",
                new[] { "Clave a: acceso denegado." },
                "No se pudo cambiar «Clave a»: acceso denegado."));

        var viewModel = new TelemetryViewModel(manager.Object);
        await viewModel.InitializationTask;

        var item = viewModel.Groups[0].Items[0];
        await viewModel.OnItemToggleAsync(item, disabled: true);

        Assert.False(item.IsDisabled);
        Assert.Equal(TelemetryStatus.Habilitado, item.Status);
        Assert.Contains("No se pudo cambiar", viewModel.StatusMessage);
        Assert.Contains("0 de 1", viewModel.SummaryText);
    }

    [Fact]
    public async Task PrivacidadMaxima_InvocaAlGestorYRecargaElCatalogo()
    {
        var key = CreateKey("a", TelemetryCategory.Sistema, TelemetryStatus.Habilitado);
        var manager = CreateManager(key);
        manager
            .Setup(m => m.ApplyMaxPrivacyAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(TelemetryOperationResult.Completed(
                1,
                0,
                @"C:\backups\manifest.json",
                Array.Empty<string>(),
                "Privacidad máxima aplicada: 1 claves deshabilitadas."));

        var viewModel = new TelemetryViewModel(manager.Object);
        await viewModel.InitializationTask;

        await viewModel.ApplyMaxPrivacyCommand.ExecuteAsync(null);

        manager.Verify(m => m.ApplyMaxPrivacyAsync(It.IsAny<CancellationToken>()), Times.Once);
        manager.Verify(m => m.GetCatalogAsync(It.IsAny<CancellationToken>()), Times.Exactly(2));
        Assert.Contains("Privacidad máxima aplicada", viewModel.StatusMessage);
    }

    private static Mock<ITelemetryManager> CreateManager(params TelemetryKey[] keys)
    {
        var manager = new Mock<ITelemetryManager>();
        manager
            .Setup(m => m.GetCatalogAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(keys.ToList());
        return manager;
    }

    private static TelemetryKey CreateKey(
        string id,
        TelemetryCategory category,
        TelemetryStatus status) =>
        new()
        {
            Id = id,
            Category = category,
            DisplayName = $"Clave {id}",
            Description = $"Descripción de la clave {id}.",
            RegistryPath = @"HKCU\Software\SOK\Tests",
            ValueName = id,
            ValueKind = TelemetryValueKind.Dword,
            DisabledValue = "0",
            EnabledValue = "1",
            Status = status
        };
}
