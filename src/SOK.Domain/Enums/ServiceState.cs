namespace SOK.Domain.Enums;

/// <summary>
/// Estado de ejecución observado de un servicio de Windows.
/// </summary>
public enum ServiceState
{
    EnEjecucion,
    Detenido,
    Pausado,
    Iniciando,
    Deteniendo,
    Desconocido
}
