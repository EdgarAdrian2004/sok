using Moq;
using SOK.Application.Abstractions;
using SOK.Application.Models;
using SOK.Domain.Abstractions;
using SOK.UI.Navigation;
using SOK.UI.ViewModels;
using Xunit;

namespace SOK.UI.Tests.ViewModels;

public sealed class TutorialViewModelTests
{
    [Fact]
    public async Task LoadAsync_CargaLosPasosYSeUbicaEnElPrimero()
    {
        var viewModel = CreateViewModel();
        await viewModel.InitializationTask;

        Assert.Equal(3, viewModel.Steps.Count);
        Assert.Equal(0, viewModel.CurrentIndex);
        Assert.Equal("Paso 1 de 3", viewModel.ProgressText);
        Assert.False(viewModel.CanGoPrevious);
        Assert.True(viewModel.CanGoNext);
        Assert.False(viewModel.IsLastStep);
        Assert.True(viewModel.IsNotLastStep);
        Assert.Equal("Paso uno", viewModel.CurrentStep?.Title);
    }

    [Fact]
    public async Task NextCommand_AvanzaYPreviousCommand_Retrocede()
    {
        var viewModel = CreateViewModel();
        await viewModel.InitializationTask;

        viewModel.NextCommand.Execute(null);
        Assert.Equal(1, viewModel.CurrentIndex);
        Assert.Equal("Paso 2 de 3", viewModel.ProgressText);
        Assert.True(viewModel.CanGoPrevious);

        viewModel.NextCommand.Execute(null);
        Assert.Equal(2, viewModel.CurrentIndex);
        Assert.True(viewModel.IsLastStep);
        Assert.False(viewModel.CanGoNext);

        viewModel.PreviousCommand.Execute(null);
        Assert.Equal(1, viewModel.CurrentIndex);
        Assert.False(viewModel.IsLastStep);
    }

    [Fact]
    public async Task GoToModuleCommand_SolicitaLaNavegacionDelPasoActual()
    {
        var navigation = new Mock<INavigationService>();
        var viewModel = CreateViewModel(navigation: navigation);
        await viewModel.InitializationTask;

        viewModel.NextCommand.Execute(null);
        viewModel.GoToModuleCommand.Execute(null);

        navigation.Verify(
            service => service.RequestNavigation("Dashboard"),
            Times.Once);

        Assert.Equal("Ir al panel de control", viewModel.ModuleButtonText);
    }

    [Fact]
    public async Task GoToModuleCommand_SinModulo_NoNavega()
    {
        var navigation = new Mock<INavigationService>();
        var viewModel = CreateViewModel(navigation: navigation);
        await viewModel.InitializationTask;

        viewModel.GoToModuleCommand.Execute(null);

        navigation.Verify(
            service => service.RequestNavigation(It.IsAny<string>()),
            Times.Never);
    }

    [Fact]
    public async Task CompleteCommand_GuardaLaMarcaYNavegaAlPanel()
    {
        var settings = new Mock<ISettingsRepository>();
        var navigation = new Mock<INavigationService>();
        var viewModel = CreateViewModel(settings: settings, navigation: navigation);
        await viewModel.InitializationTask;

        await viewModel.CompleteCommand.ExecuteAsync(null);

        settings.Verify(
            repository => repository.SetAsync(
                TutorialViewModel.TutorialCompletedKey,
                "true",
                It.IsAny<CancellationToken>()),
            Times.Once);

        navigation.Verify(
            service => service.RequestNavigation("Dashboard"),
            Times.Once);

        Assert.Contains("completado", viewModel.StatusMessage);
    }

    [Fact]
    public async Task SkipCommand_GuardaLaMarcaYNavegaAlPanel()
    {
        var settings = new Mock<ISettingsRepository>();
        var navigation = new Mock<INavigationService>();
        var viewModel = CreateViewModel(settings: settings, navigation: navigation);
        await viewModel.InitializationTask;

        await viewModel.SkipCommand.ExecuteAsync(null);

        settings.Verify(
            repository => repository.SetAsync(
                TutorialViewModel.TutorialCompletedKey,
                "true",
                It.IsAny<CancellationToken>()),
            Times.Once);

        navigation.Verify(
            service => service.RequestNavigation("Dashboard"),
            Times.Once);

        Assert.Contains("omitido", viewModel.StatusMessage);
    }

    [Fact]
    public async Task CompleteCommand_SiFallaElGuardado_MuestraElErrorYNoNavega()
    {
        var settings = new Mock<ISettingsRepository>();
        settings
            .Setup(repository => repository.SetAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("Base de datos bloqueada."));

        var navigation = new Mock<INavigationService>();
        var viewModel = CreateViewModel(settings: settings, navigation: navigation);
        await viewModel.InitializationTask;

        await viewModel.CompleteCommand.ExecuteAsync(null);

        navigation.Verify(
            service => service.RequestNavigation(It.IsAny<string>()),
            Times.Never);

        Assert.Contains("Base de datos bloqueada", viewModel.StatusMessage);
    }

    private static TutorialViewModel CreateViewModel(
        Mock<ISettingsRepository>? settings = null,
        Mock<INavigationService>? navigation = null)
    {
        var tutorial = new Mock<ITutorialService>();
        tutorial
            .Setup(service => service.GetStepsAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<TutorialStep>
            {
                new(1, "Paso uno", "# Paso uno", null, null),
                new(2, "Paso dos", "# Paso dos", "Dashboard", "OpenDashboard"),
                new(3, "Paso tres", "# Paso tres", "Telemetry", "OpenTelemetry")
            });

        return new TutorialViewModel(
            tutorial.Object,
            (settings ?? new Mock<ISettingsRepository>()).Object,
            (navigation ?? new Mock<INavigationService>()).Object);
    }
}
