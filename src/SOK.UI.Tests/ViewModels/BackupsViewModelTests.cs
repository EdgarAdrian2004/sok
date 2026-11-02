using Moq;
using SOK.Application.Abstractions;
using SOK.Application.DTOs;
using SOK.UI.Services;
using SOK.UI.ViewModels;
using Xunit;

namespace SOK.UI.Tests.ViewModels;

public sealed class BackupsViewModelTests
{
    [Fact]
    public async Task LoadAsync_MuestraElResumenYSeleccionaElPrimerRestaurable()
    {
        var manager = new Mock<IRestoreManager>();
        manager
            .Setup(service => service.GetBackupsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<BackupStatus>
            {
                Status(1, "Alterado", exists: true, matches: false),
                Status(2, "Válido", exists: true, matches: true),
                Status(3, "Inexistente", exists: false, matches: false)
            });

        var viewModel = CreateViewModel(manager);
        await viewModel.InitializationTask;

        Assert.Equal(3, viewModel.Backups.Count);
        Assert.Equal("1 de 3 respaldos listos para restaurar.", viewModel.SummaryText);
        Assert.Equal(2, viewModel.SelectedBackup?.Id);
        Assert.True(viewModel.SelectedBackup!.CanRestore);
        Assert.False(viewModel.Backups[0].CanRestore);
        Assert.Equal("No restaurable", viewModel.Backups[0].RestoreButtonText);
    }

    [Fact]
    public async Task LoadAsync_SinRespaldos_MuestraUnMensajeInformativo()
    {
        var manager = new Mock<IRestoreManager>();
        manager
            .Setup(service => service.GetBackupsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(Array.Empty<BackupStatus>());

        var viewModel = CreateViewModel(manager);
        await viewModel.InitializationTask;

        Assert.Empty(viewModel.Backups);
        Assert.Contains("Todavía no hay respaldos", viewModel.StatusMessage);
        Assert.Equal("Sin respaldos registrados.", viewModel.SummaryText);
    }

    [Fact]
    public async Task RestoreCommand_ConfirmaYReportaElResultado()
    {
        var manager = CreateManagerWithBackups();
        manager
            .Setup(service => service.RestoreAsync(2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new RestoreResult(
                true,
                4,
                2,
                Array.Empty<string>(),
                "Respaldo «Válido» restaurado correctamente."));

        var dialogs = new Mock<IUserDialogService>();
        dialogs
            .Setup(dialog => dialog.Confirm(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(true);

        var viewModel = CreateViewModel(manager, dialogs);
        await viewModel.InitializationTask;

        var item = viewModel.Backups.First(backup => backup.Id == 2);
        await viewModel.RestoreCommand.ExecuteAsync(item);

        dialogs.Verify(
            dialog => dialog.Confirm(It.IsAny<string>(), It.IsAny<string>()),
            Times.Once);

        dialogs.Verify(
            dialog => dialog.ShowInfo(
                "Restauración completada",
                It.Is<string>(message =>
                    message.Contains("Claves de registro restauradas: 4") &&
                    message.Contains("Servicios restaurados: 2"))),
            Times.Once);

        Assert.Contains("restaurado correctamente", viewModel.StatusMessage);
    }

    [Fact]
    public async Task RestoreCommand_Cancelado_NoRestauraElRespaldo()
    {
        var manager = CreateManagerWithBackups();
        var dialogs = new Mock<IUserDialogService>();
        dialogs
            .Setup(dialog => dialog.Confirm(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(false);

        var viewModel = CreateViewModel(manager, dialogs);
        await viewModel.InitializationTask;

        var item = viewModel.Backups.First(backup => backup.Id == 2);
        await viewModel.RestoreCommand.ExecuteAsync(item);

        manager.Verify(
            service => service.RestoreAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task RestoreCommand_RespaldoNoRestaurable_MuestraErrorYNoRestaura()
    {
        var manager = CreateManagerWithBackups();
        var dialogs = new Mock<IUserDialogService>();

        var viewModel = CreateViewModel(manager, dialogs);
        await viewModel.InitializationTask;

        var item = viewModel.Backups.First(backup => backup.Id == 1);
        await viewModel.RestoreCommand.ExecuteAsync(item);

        dialogs.Verify(
            dialog => dialog.ShowError(
                "Respaldo no restaurable",
                It.IsAny<string>()),
            Times.Once);

        manager.Verify(
            service => service.RestoreAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task RestoreCommand_FalloParcial_MuestraElErrorYLosFallos()
    {
        var manager = CreateManagerWithBackups();
        manager
            .Setup(service => service.RestoreAsync(2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new RestoreResult(
                false,
                1,
                0,
                new[] { "DiagTrack: acceso denegado." },
                "Restauración parcial de «Válido»: 1 con error."));

        var dialogs = new Mock<IUserDialogService>();
        dialogs
            .Setup(dialog => dialog.Confirm(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(true);

        var viewModel = CreateViewModel(manager, dialogs);
        await viewModel.InitializationTask;

        var item = viewModel.Backups.First(backup => backup.Id == 2);
        await viewModel.RestoreCommand.ExecuteAsync(item);

        dialogs.Verify(
            dialog => dialog.ShowError("Restauración parcial", It.IsAny<string>()),
            Times.Once);

        Assert.Contains("acceso denegado", viewModel.StatusMessage);
    }

    private static Mock<IRestoreManager> CreateManagerWithBackups()
    {
        var manager = new Mock<IRestoreManager>();
        manager
            .Setup(service => service.GetBackupsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<BackupStatus>
            {
                Status(1, "Alterado", exists: true, matches: false),
                Status(2, "Válido", exists: true, matches: true)
            });

        return manager;
    }

    private static BackupStatus Status(int id, string name, bool exists, bool matches) =>
        new(
            id,
            name,
            "Completo",
            $@"C:\backups\{name}.json",
            "Respaldo de prueba",
            new DateTime(2026, 11, 2, 10, 30, 0),
            exists,
            matches);

    private static BackupsViewModel CreateViewModel(
        Mock<IRestoreManager> manager,
        Mock<IUserDialogService>? dialogs = null) =>
        new(
            manager.Object,
            (dialogs ?? new Mock<IUserDialogService>()).Object);
}
