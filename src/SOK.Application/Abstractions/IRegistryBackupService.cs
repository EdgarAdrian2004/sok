namespace SOK.Application.Abstractions;

/// <summary>
/// Exporta las claves de registro relevantes para SOK y devuelve la ruta
/// del manifiesto que describe los archivos generados.
/// </summary>
public interface IRegistryBackupService
{
    Task<string> CreateBackupAsync(string destinationDirectory, CancellationToken cancellationToken = default);
}
