using Moq;
using SOK.Application.Abstractions;
using SOK.Application.DTOs;
using SOK.Domain.Entities;
using SOK.Domain.Enums;
using SOK.UI.Services;
using SOK.UI.ViewModels;
using Xunit;

namespace SOK.UI.Tests.ViewModels;

public sealed class ProfilesViewModelTests
{
    [Fact]
    public async Task LoadAsync_MuestraLosPerfilesYElActivo()
    {
        var manager = new Mock<IProfileManager>();
        manager
            .Setup(service => service.GetAllProfilesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Profile>
            {
                new() { Id = 1, Name = "Ofimatico", Type = ProfileType.Ofimatico, Description = "Estable" },
                new() { Id = 2, Name = "Gamer", Type = ProfileType.Gamer, Description = "Rendimiento" }
            });
        manager
            .Setup(service => service.GetActiveProfileAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Profile { Id = 2, Name = "Gamer", Type = ProfileType.Gamer });

        var viewModel = CreateViewModel(manager);
        await viewModel.InitializationTask;

        Assert.Equal(2, viewModel.Profiles.Count);
        Assert.Contains("Gamer", viewModel.ActiveProfileText);
        Assert.NotNull(viewModel.SelectedProfile);
        Assert.Equal(2, viewModel.SelectedProfile!.Id);
        Assert.True(viewModel.SelectedProfile.IsActive);
    }

    [Fact]
    public async Task ActivateProfileCommand_PersisteElPerfilSeleccionado()
    {
        var manager = CreateManagerWithProfiles();
        var viewModel = CreateViewModel(manager);
        await viewModel.InitializationTask;

        var item = viewModel.Profiles.First(profile => profile.Id == 2);
        await viewModel.ActivateProfileCommand.ExecuteAsync(item);

        manager.Verify(
            service => service.SetActiveProfileAsync(2, It.IsAny<CancellationToken>()),
            Times.Once);

        Assert.Contains("Gamer", viewModel.StatusMessage);
    }

    [Fact]
    public async Task ApplyProfileCommand_ConfirmaYReportaLosRespaldos()
    {
        var manager = CreateManagerWithProfiles();
        manager
            .Setup(service => service.ApplyProfileAsync(2, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ProfileOperationResult(
                true,
                10,
                0,
                5,
                0,
                true,
                Array.Empty<string>(),
                new[] { @"C:\backups\telemetria", @"C:\backups\servicios" },
                "Perfil «Gamer» aplicado correctamente."));

        var dialogs = new Mock<IUserDialogService>();
        dialogs
            .Setup(dialog => dialog.Confirm(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(true);

        var viewModel = CreateViewModel(manager, dialogs: dialogs);
        await viewModel.InitializationTask;

        var item = viewModel.Profiles.First(profile => profile.Id == 2);
        await viewModel.ApplyProfileCommand.ExecuteAsync(item);

        dialogs.Verify(
            dialog => dialog.Confirm(It.IsAny<string>(), It.IsAny<string>()),
            Times.Once);

        Assert.Contains("aplicado correctamente", viewModel.StatusMessage);
        Assert.Contains("telemetria", viewModel.LastBackupText);
    }

    [Fact]
    public async Task ApplyProfileCommand_Cancelado_NoAplicaElPerfil()
    {
        var manager = CreateManagerWithProfiles();
        var dialogs = new Mock<IUserDialogService>();
        dialogs
            .Setup(dialog => dialog.Confirm(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(false);

        var viewModel = CreateViewModel(manager, dialogs: dialogs);
        await viewModel.InitializationTask;

        var item = viewModel.Profiles.First(profile => profile.Id == 2);
        await viewModel.ApplyProfileCommand.ExecuteAsync(item);

        manager.Verify(
            service => service.ApplyProfileAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task ImportProfileCommand_IncorporaElPerfilYLoSelecciona()
    {
        var allProfiles = new List<Profile>
        {
            new() { Id = 1, Name = "Ofimatico", Type = ProfileType.Ofimatico },
            new() { Id = 2, Name = "Gamer", Type = ProfileType.Gamer }
        };

        var imported = new Profile
        {
            Id = 3,
            Name = "Gamer (importado)",
            Type = ProfileType.Personalizado
        };

        var manager = new Mock<IProfileManager>();
        manager
            .Setup(service => service.GetAllProfilesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => allProfiles);
        manager
            .Setup(service => service.GetActiveProfileAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => allProfiles[0]);
        manager
            .Setup(service => service.ImportProfileAsync(
                @"C:\perfiles\gamer.json",
                It.IsAny<CancellationToken>()))
            .Callback(() => allProfiles.Add(imported))
            .ReturnsAsync(imported);

        var files = new Mock<IFileDialogService>();
        files
            .Setup(dialog => dialog.ShowOpenFileDialog(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(@"C:\perfiles\gamer.json");

        var viewModel = CreateViewModel(manager, files: files);
        await viewModel.InitializationTask;

        await viewModel.ImportProfileCommand.ExecuteAsync(null);

        manager.Verify(
            service => service.ImportProfileAsync(
                @"C:\perfiles\gamer.json",
                It.IsAny<CancellationToken>()),
            Times.Once);

        Assert.Equal(3, viewModel.SelectedProfile?.Id);
        Assert.Contains("importado", viewModel.StatusMessage);
    }

    private static Mock<IProfileManager> CreateManagerWithProfiles()
    {
        var manager = new Mock<IProfileManager>();
        manager
            .Setup(service => service.GetAllProfilesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Profile>
            {
                new() { Id = 1, Name = "Ofimatico", Type = ProfileType.Ofimatico },
                new() { Id = 2, Name = "Gamer", Type = ProfileType.Gamer }
            });
        manager
            .Setup(service => service.GetActiveProfileAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new Profile { Id = 1, Name = "Ofimatico", Type = ProfileType.Ofimatico });

        return manager;
    }

    private static ProfilesViewModel CreateViewModel(
        Mock<IProfileManager> manager,
        Mock<IFileDialogService>? files = null,
        Mock<IUserDialogService>? dialogs = null) =>
        new(
            manager.Object,
            (files ?? new Mock<IFileDialogService>()).Object,
            (dialogs ?? new Mock<IUserDialogService>()).Object);
}
