using SOK.Application.Common;
using SOK.Domain.Enums;
using Xunit;

namespace SOK.UI.Tests.Application;

/// <summary>
/// Verifica el contrato del catálogo estático de servicios: tamaño,
/// unicidad, categorías y, sobre todo, que los perfiles sean conservadores
/// con la usabilidad del equipo.
/// </summary>
public sealed class ServiceCatalogTests
{
    private static readonly string[] BaseComun =
    {
        "DiagTrack", "dmwappushservice", "WerSvc", "Fax", "RpcLocator",
        "SysMain", "TrkWks", "MapsBroker", "lfsvc", "WalletService",
        "PhoneSvc", "SDRSVC", "WMPNetworkSvc", "SSDPSRV", "upnphost", "lmhosts"
    };

    private static readonly string[] NuncaEnPerfiles =
    {
        "WSearch", "SharedAccess", "bthserv", "WbioSrvc", "RemoteRegistry",
        "RemoteAccess", "seclogon", "stisvc", "Spooler"
    };

    [Fact]
    public void CreateDefinitions_Devuelve30ServiciosConIdentificadoresUnicos()
    {
        var catalog = ServiceCatalog.CreateDefinitions();

        Assert.Equal(30, catalog.Count);
        Assert.Equal(catalog.Count, catalog.Select(entry => entry.Id).Distinct(StringComparer.OrdinalIgnoreCase).Count());
        Assert.All(catalog, entry =>
        {
            Assert.False(string.IsNullOrWhiteSpace(entry.Id));
            Assert.False(string.IsNullOrWhiteSpace(entry.DisplayName));
            Assert.False(string.IsNullOrWhiteSpace(entry.Description));
        });
    }

    [Fact]
    public void CreateDefinitions_CubreLasDiezCategorias()
    {
        var catalog = ServiceCatalog.CreateDefinitions();

        var categories = catalog.Select(entry => entry.Category).Distinct().ToList();

        Assert.Equal(10, categories.Count);
        Assert.Contains(ServiceCategory.Telemetria, categories);
        Assert.Contains(ServiceCategory.Obsoleto, categories);
        Assert.Contains(ServiceCategory.Rendimiento, categories);
        Assert.Contains(ServiceCategory.Privacidad, categories);
        Assert.Contains(ServiceCategory.Red, categories);
        Assert.Contains(ServiceCategory.Xbox, categories);
        Assert.Contains(ServiceCategory.Bluetooth, categories);
        Assert.Contains(ServiceCategory.Biometria, categories);
        Assert.Contains(ServiceCategory.Seguridad, categories);
        Assert.Contains(ServiceCategory.Perifericos, categories);
    }

    [Fact]
    public void Perfiles_CompartenLaBaseComun()
    {
        var catalog = ServiceCatalog.CreateDefinitions();

        foreach (var id in BaseComun)
        {
            var entry = catalog.Single(item => item.Id == id);

            Assert.True(entry.DisabledInGamer, $"{id} debería deshabilitarse en Gamer.");
            Assert.True(entry.DisabledInOfimatico, $"{id} debería deshabilitarse en Ofimático.");
        }
    }

    [Fact]
    public void PerfilGamer_SoloAgregaPrintNotify()
    {
        var catalog = ServiceCatalog.CreateDefinitions();

        var soloGamer = catalog
            .Where(entry => entry.DisabledInGamer && !entry.DisabledInOfimatico)
            .Select(entry => entry.Id)
            .ToList();

        Assert.Equal(new[] { "PrintNotify" }, soloGamer);
    }

    [Fact]
    public void PerfilOfimatico_SoloAgregaLosCuatroServiciosDeXbox()
    {
        var catalog = ServiceCatalog.CreateDefinitions();

        var soloOfimatico = catalog
            .Where(entry => entry.DisabledInOfimatico && !entry.DisabledInGamer)
            .Select(entry => entry.Id)
            .OrderBy(id => id, StringComparer.Ordinal)
            .ToList();

        Assert.Equal(
            new[] { "XblAuthManager", "XblGameSave", "XboxGipSvc", "XboxNetApiSvc" },
            soloOfimatico);
    }

    [Theory]
    [InlineData("WSearch")]
    [InlineData("SharedAccess")]
    [InlineData("bthserv")]
    [InlineData("WbioSrvc")]
    [InlineData("RemoteRegistry")]
    [InlineData("RemoteAccess")]
    [InlineData("seclogon")]
    [InlineData("stisvc")]
    [InlineData("Spooler")]
    public void ServiciosDeAltoRiesgo_NuncaSeDeshabilitanPorPerfil(string id)
    {
        var entry = ServiceCatalog.CreateDefinitions().Single(item => item.Id == id);

        Assert.False(entry.DisabledInGamer);
        Assert.False(entry.DisabledInOfimatico);
    }

    [Fact]
    public void NuncaEnPerfiles_CoincideConElCriterioDelPlan()
    {
        var catalog = ServiceCatalog.CreateDefinitions();

        var excluidos = catalog
            .Where(entry => !entry.DisabledInGamer && !entry.DisabledInOfimatico)
            .Select(entry => entry.Id)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        Assert.Equal(
            NuncaEnPerfiles.OrderBy(id => id, StringComparer.Ordinal).ToArray(),
            excluidos.OrderBy(id => id, StringComparer.Ordinal).ToArray());
    }

    [Fact]
    public void ServiciosDeAltoRiesgo_EstanMarcadosComoTales()
    {
        var catalog = ServiceCatalog.CreateDefinitions();

        var deAltoRiesgo = catalog
            .Where(entry => entry.HighRisk)
            .Select(entry => entry.Id)
            .OrderBy(id => id, StringComparer.Ordinal)
            .ToArray();

        Assert.Equal(
            new[] { "RemoteAccess", "SharedAccess", "Spooler", "WSearch", "WbioSrvc", "bthserv" },
            deAltoRiesgo);
    }

    [Fact]
    public void CreateDefinitions_DevuelveInstanciasNuevasEnCadaLlamada()
    {
        var first = ServiceCatalog.CreateDefinitions();
        var second = ServiceCatalog.CreateDefinitions();

        Assert.NotSame(first[0], second[0]);
    }
}
