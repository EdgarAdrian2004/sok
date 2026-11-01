using System.Text.Json;
using Microsoft.Win32;
using SOK.Infrastructure.Windows;
using Xunit;

namespace SOK.UI.Tests.Infrastructure;

/// <summary>
/// Pruebas de integración del respaldo por claves. Crean una clave temporal
/// bajo HKCU, de modo que no se modifica la configuración real del equipo ni
/// se requiere elevación.
/// </summary>
public sealed class RegistryBackupIntegrationTests : IDisposable
{
    private const string TestRoot = @"Software\SOK\BackupTests";

    private readonly string _tempRoot = Path.Combine(
        Path.GetTempPath(),
        "sok-registry-backup-" + Guid.NewGuid().ToString("N"));

    [Fact]
    public async Task CreateBackupForKeysAsync_ExportaLaClaveYGeneraManifiestoConHash()
    {
        var keyName = $@"{TestRoot}\{Guid.NewGuid():N}";
        var fullPath = $@"HKCU\{keyName}";

        using (var key = Registry.CurrentUser.CreateSubKey(keyName, writable: true))
        {
            key.SetValue("ValorDePrueba", 1, RegistryValueKind.DWord);
        }

        try
        {
            var service = new RegistryBackupService();

            var manifestPath = await service.CreateBackupForKeysAsync(
                new[] { fullPath },
                _tempRoot);

            Assert.True(File.Exists(manifestPath));

            var registryDirectory = Path.Combine(_tempRoot, "registry");
            var exportedFiles = Directory.GetFiles(registryDirectory, "*.reg");
            Assert.Single(exportedFiles);
            Assert.True(new FileInfo(exportedFiles[0]).Length > 0);

            using var document = JsonDocument.Parse(await File.ReadAllTextAsync(manifestPath));
            var entry = Assert.Single(document.RootElement.EnumerateArray().ToList());

            Assert.Equal(fullPath, entry.GetProperty("Key").GetString());
            Assert.False(string.IsNullOrWhiteSpace(entry.GetProperty("FileName").GetString()));

            var hash = entry.GetProperty("HashSha256").GetString();
            Assert.NotNull(hash);
            Assert.Equal(64, hash!.Length);

            var fileName = entry.GetProperty("FileName").GetString()!;
            var recomputed = await SOK.Application.Common.Sha256Helper.ComputeFileHashAsync(
                Path.Combine(registryDirectory, fileName));
            Assert.Equal(recomputed, hash);
        }
        finally
        {
            Registry.CurrentUser.DeleteSubKeyTree(keyName, throwOnMissingSubKey: false);
        }
    }

    [Fact]
    public async Task CreateBackupForKeysAsync_ClaveInexistente_SeRegistraSinRespaldo()
    {
        var missingPath = $@"HKCU\{TestRoot}\{Guid.NewGuid():N}";

        var service = new RegistryBackupService();

        var manifestPath = await service.CreateBackupForKeysAsync(
            new[] { missingPath },
            _tempRoot);

        Assert.True(File.Exists(manifestPath));

        using var document = JsonDocument.Parse(await File.ReadAllTextAsync(manifestPath));
        var entry = Assert.Single(document.RootElement.EnumerateArray().ToList());

        Assert.Equal(missingPath, entry.GetProperty("Key").GetString());
        Assert.Equal(JsonValueKind.Null, entry.GetProperty("FileName").ValueKind);
        Assert.Equal(JsonValueKind.Null, entry.GetProperty("HashSha256").ValueKind);
        Assert.Contains(
            "no requiere respaldo",
            entry.GetProperty("Error").GetString(),
            StringComparison.OrdinalIgnoreCase);

        var registryDirectory = Path.Combine(_tempRoot, "registry");
        Assert.Empty(Directory.GetFiles(registryDirectory, "*.reg"));
    }

    public void Dispose()
    {
        if (Directory.Exists(_tempRoot))
        {
            Directory.Delete(_tempRoot, recursive: true);
        }
    }
}
