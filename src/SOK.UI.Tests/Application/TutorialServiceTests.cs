using SOK.Application.Services;
using Xunit;

namespace SOK.UI.Tests.Application;

public sealed class TutorialServiceTests
{
    [Fact]
    public async Task GetStepsAsync_DevuelveLosOchoPasosEnOrden()
    {
        var service = new TutorialService();

        var steps = await service.GetStepsAsync();

        Assert.Equal(8, steps.Count);
        Assert.Equal(Enumerable.Range(1, 8), steps.Select(step => step.Order));
    }

    [Fact]
    public async Task GetStepsAsync_LeeElTituloYLosMetadatosDelEncabezado()
    {
        var service = new TutorialService();

        var steps = await service.GetStepsAsync();

        Assert.Equal("Bienvenida a SOK", steps[0].Title);
        Assert.Equal("Panel de control", steps[1].Title);
        Assert.Equal("Dashboard", steps[1].ModuleKey);
        Assert.Equal("OpenDashboard", steps[1].ActionName);
        Assert.Null(steps[0].ModuleKey);
        Assert.Equal("Cierre y próximos pasos", steps[7].Title);
    }

    [Fact]
    public async Task GetStepsAsync_NoIncluyeElEncabezadoEnElContenido()
    {
        var service = new TutorialService();

        var steps = await service.GetStepsAsync();

        Assert.All(steps, step =>
        {
            Assert.False(step.ContentMarkdown.StartsWith("---", StringComparison.Ordinal));
            Assert.False(string.IsNullOrWhiteSpace(step.ContentMarkdown));
        });

        Assert.StartsWith("# Bienvenida a SOK", steps[0].ContentMarkdown);
    }

    [Fact]
    public async Task GetStepsAsync_SoloUsaModulosConocidos()
    {
        var conocidos = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            "Dashboard",
            "Telemetry",
            "Services",
            "Memory",
            "Backups",
            "Profiles"
        };

        var service = new TutorialService();
        var steps = await service.GetStepsAsync();

        Assert.All(
            steps.Where(step => step.ModuleKey is not null),
            step => Assert.Contains(step.ModuleKey!, conocidos));
    }
}
