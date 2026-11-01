namespace SOK.Application.Abstractions;

/// <summary>
/// Exporta las claves de registro relevantes para SOK y devuelve la ruta
/// del manifiesto que describe los archivos generados.
/// </summary>
public interface IRegistryBackupService
{
    Task<string> CreateBackupAsync(string destinationDirectory, CancellationToken cancellationToken = default);

    /// <summary>
    /// Exporta únicamente las claves indicadas y devuelve la ruta del
    /// manifiesto con los archivos generados y sus hashes SHA-256.
    /// Las claves que no existen en el equipo se registran en el manifiesto
    /// como no aplicables, sin abortar la operación.
    /// </summary>
    Task<string> CreateBackupForKeysAsync(
        IReadOnlyCollection<string> registryKeys,
        string destinationDirectory,
        CancellationToken cancellationToken = default);
}
