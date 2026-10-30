namespace SOK.Domain.Enums;

/// <summary>
/// Módulo que genera una entrada de la bitácora. Coincide con los valores
/// permitidos por la columna Logs.Module.
/// </summary>
public enum LogModule
{
    Acceso,
    Dashboard,
    Telemetria,
    Servicios,
    Memoria,
    Respaldos,
    Perfiles
}
