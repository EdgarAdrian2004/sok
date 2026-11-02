using SOK.Application.DTOs;
using SOK.Domain.Entities;

namespace SOK.Application.Abstractions;

/// <summary>
/// Fachada del módulo de perfiles: consulta y activa perfiles, aplica sus
/// ajustes sobre Windows y los exporta o importa en JSON.
/// </summary>
public interface IProfileManager
{
    /// <summary>
    /// Devuelve todos los perfiles. Los perfiles base sin ajustes se
    /// completan a partir de los catálogos de telemetría y servicios.
    /// </summary>
    Task<IReadOnlyList<Profile>> GetAllProfilesAsync(CancellationToken cancellationToken = default);

    /// <summary>Devuelve el perfil activo según <c>Settings.ActiveProfileId</c>.</summary>
    Task<Profile?> GetActiveProfileAsync(CancellationToken cancellationToken = default);

    /// <summary>Marca un perfil como activo y lo persiste en <c>Settings</c>.</summary>
    Task SetActiveProfileAsync(int profileId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Aplica los ajustes del perfil indicado: telemetría y servicios con
    /// respaldo previo, y configuración de memoria sin respaldo porque no
    /// altera el sistema operativo.
    /// </summary>
    Task<ProfileOperationResult> ApplyProfileAsync(
        int profileId,
        CancellationToken cancellationToken = default);

    /// <summary>Aplica el perfil activo.</summary>
    Task<ProfileOperationResult> ApplyActiveProfileAsync(CancellationToken cancellationToken = default);

    /// <summary>Exporta un perfil a un archivo JSON y devuelve la ruta escrita.</summary>
    Task<string> ExportProfileAsync(
        int profileId,
        string filePath,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Importa un perfil desde JSON. El perfil se crea como
    /// <c>Personalizado</c> y recibe un nombre único si el original ya existe.
    /// </summary>
    Task<Profile> ImportProfileAsync(string filePath, CancellationToken cancellationToken = default);
}
