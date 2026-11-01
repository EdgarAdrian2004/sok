using SOK.Application.Common;
using SOK.Domain.Enums;
using Xunit;

namespace SOK.UI.Tests.Application;

public sealed class TelemetryCatalogTests
{
    [Fact]
    public void Catalogo_TieneAlMenosTreintaClaves()
    {
        var catalog = TelemetryCatalog.CreateDefinitions();

        Assert.True(catalog.Count >= 30, $"El catálogo solo tiene {catalog.Count} claves.");
    }

    [Fact]
    public void Catalogo_LosIdentificadoresSonUnicos()
    {
        var catalog = TelemetryCatalog.CreateDefinitions();

        var duplicates = catalog
            .GroupBy(key => key.Id, StringComparer.Ordinal)
            .Where(group => group.Count() > 1)
            .Select(group => group.Key)
            .ToList();

        Assert.Empty(duplicates);
    }

    [Fact]
    public void Catalogo_TodasLasCategoriasTienenAlMenosUnaClave()
    {
        var catalog = TelemetryCatalog.CreateDefinitions();

        foreach (var category in Enum.GetValues<TelemetryCategory>())
        {
            Assert.Contains(catalog, key => key.Category == category);
        }
    }

    [Fact]
    public void Catalogo_LasClavesTienenRutaValorYDescripcion()
    {
        var catalog = TelemetryCatalog.CreateDefinitions();

        foreach (var key in catalog)
        {
            Assert.False(string.IsNullOrWhiteSpace(key.RegistryPath), $"{key.Id} no tiene ruta.");
            Assert.False(string.IsNullOrWhiteSpace(key.ValueName), $"{key.Id} no tiene nombre de valor.");
            Assert.False(string.IsNullOrWhiteSpace(key.DisplayName), $"{key.Id} no tiene nombre visible.");
            Assert.False(string.IsNullOrWhiteSpace(key.Description), $"{key.Id} no tiene descripción.");
            Assert.StartsWith("HK", key.RegistryPath);
        }
    }

    [Fact]
    public void Catalogo_LosValoresHabilitadoYDeshabilitadoDifieren()
    {
        var catalog = TelemetryCatalog.CreateDefinitions();

        foreach (var key in catalog)
        {
            Assert.NotEqual(key.DisabledValue, key.EnabledValue, StringComparer.OrdinalIgnoreCase);
        }
    }

    [Fact]
    public void Catalogo_LosValoresDwordSonNumericos()
    {
        var catalog = TelemetryCatalog.CreateDefinitions();

        foreach (var key in catalog.Where(key => key.ValueKind == TelemetryValueKind.Dword))
        {
            Assert.True(int.TryParse(key.DisabledValue, out _), $"{key.Id} tiene un valor deshabilitado no numérico.");
            Assert.True(int.TryParse(key.EnabledValue, out _), $"{key.Id} tiene un valor habilitado no numérico.");
        }
    }

    [Fact]
    public void Catalogo_CadaLlamadaDevuelveInstanciasNuevas()
    {
        var first = TelemetryCatalog.CreateDefinitions();
        var second = TelemetryCatalog.CreateDefinitions();

        Assert.NotSame(first[0], second[0]);

        first[0].Status = TelemetryStatus.Deshabilitado;
        Assert.Equal(TelemetryStatus.Desconocido, second[0].Status);
    }
}
