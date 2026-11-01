namespace SOK.Application.Abstractions;

/// <summary>
/// Exporta el estado de los servicios de Windows y devuelve la ruta del
/// archivo JSON generado.
/// </summary>
public interface IServicesBackupService
{
    Task<string> CreateBackupAsync(string destinationDirectory, CancellationToken cancellationToken = default);

    /// <summary>
    /// Exporta únicamente los servicios indicados y devuelve la ruta del
    /// archivo JSON generado. Los servicios que no existen en el equipo se
    /// omiten del archivo.
    /// </summary>
    Task<string> CreateBackupForServicesAsync(
        IReadOnlyCollection<string> serviceNames,
        string destinationDirectory,
        CancellationToken cancellationToken = default);
}
