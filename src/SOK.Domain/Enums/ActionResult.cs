namespace SOK.Domain.Enums;

/// <summary>
/// Resultado de una acción registrada en la bitácora. Coincide con los
/// valores permitidos por la columna Logs.Result.
/// </summary>
public enum ActionResult
{
    Exitoso,
    Fallido,
    Revertido
}
