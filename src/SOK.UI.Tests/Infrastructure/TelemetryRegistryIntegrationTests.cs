using Microsoft.Win32;
using SOK.Domain.Enums;
using SOK.Infrastructure.Windows;
using Xunit;

namespace SOK.UI.Tests.Infrastructure;

/// <summary>
/// Pruebas de integración del acceso al registro de Windows. Usan una
/// clave temporal bajo HKCU para no modificar la configuración del equipo
/// y no requieren elevación.
/// </summary>
public sealed class TelemetryRegistryIntegrationTests
{
    private const string TestRoot = @"Software\SOK\Tests";

    [Fact]
    public async Task EscribeYLeeValoresDwordYString()
    {
        var keyName = $@"{TestRoot}\{Guid.NewGuid():N}";
        var fullPath = $@"HKCU\{keyName}";

        try
        {
            var service = new TelemetryRegistryService();

            await service.WriteValueAsync(fullPath, "Numero", "1", TelemetryValueKind.Dword);
            var number = await service.ReadValueAsync(fullPath, "Numero");

            Assert.NotNull(number);
            Assert.Equal(TelemetryValueKind.Dword, number!.Kind);
            Assert.Equal("1", number.Value);

            await service.WriteValueAsync(fullPath, "Numero", "0", TelemetryValueKind.Dword);
            var updated = await service.ReadValueAsync(fullPath, "Numero");
            Assert.Equal("0", updated!.Value);

            await service.WriteValueAsync(fullPath, "Texto", "Deny", TelemetryValueKind.String);
            var text = await service.ReadValueAsync(fullPath, "Texto");

            Assert.NotNull(text);
            Assert.Equal(TelemetryValueKind.String, text!.Kind);
            Assert.Equal("Deny", text.Value);
        }
        finally
        {
            Registry.CurrentUser.DeleteSubKeyTree(keyName, throwOnMissingSubKey: false);
        }
    }

    [Fact]
    public async Task ReadValueAsync_DevuelveNullCuandoNoExiste()
    {
        var service = new TelemetryRegistryService();

        var missingValue = await service.ReadValueAsync(
            $@"HKCU\{TestRoot}\{Guid.NewGuid():N}",
            "Valor");

        Assert.Null(missingValue);
    }
}
