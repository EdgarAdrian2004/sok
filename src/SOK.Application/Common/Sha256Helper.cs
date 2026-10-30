using System.Security.Cryptography;

namespace SOK.Application.Common;

/// <summary>
/// Cálculo de hashes SHA-256 para verificar la integridad de los respaldos.
/// </summary>
public static class Sha256Helper
{
    public static async Task<string> ComputeFileHashAsync(string filePath, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(filePath);

        await using var stream = File.OpenRead(filePath);
        var hash = await SHA256.HashDataAsync(stream, cancellationToken);
        return Convert.ToHexString(hash).ToLowerInvariant();
    }
}
