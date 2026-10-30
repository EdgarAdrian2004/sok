namespace SOK.Application.Abstractions;

/// <summary>
/// Exporta el estado de los servicios de Windows y devuelve la ruta del
/// archivo JSON generado.
/// </summary>
public interface IServicesBackupService
{
    Task<string> CreateBackupAsync(string destinationDirectory, CancellationToken cancellationToken = default);
}
