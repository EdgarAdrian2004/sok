using SOK.Application.DTOs;

namespace SOK.Application.Abstractions;

/// <summary>
/// Restaura claves del registro de Windows a partir del manifiesto que
/// genera <c>RegistryBackupService</c>. Verifica el hash de cada archivo
/// <c>.reg</c> antes de importarlo.
/// </summary>
public interface IRegistryRestoreService
{
    /// <summary>
    /// Importa los archivos <c>.reg</c> listados en el manifiesto y
    /// devuelve cuántos se restauraron, cuántos se omitieron y los fallos.
    /// </summary>
    Task<RegistryRestoreOutcome> RestoreFromManifestAsync(
        string registryManifestPath,
        CancellationToken cancellationToken = default);
}
