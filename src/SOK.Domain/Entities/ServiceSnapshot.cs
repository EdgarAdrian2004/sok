using SOK.Domain.Enums;

namespace SOK.Domain.Entities;

/// <summary>
/// Lectura del estado de un servicio de Windows: nombre interno, nombre
/// visible, tipo de inicio y estado de ejecución.
/// </summary>
public sealed record ServiceSnapshot(
    string Name,
    string DisplayName,
    ServiceStartMode StartMode,
    ServiceState State);
