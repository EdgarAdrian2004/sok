namespace SOK.Domain.Enums;

/// <summary>
/// Acción que aplica un ajuste de perfil. Coincide con los valores
/// permitidos por la columna ProfileSettings.Action.
/// </summary>
public enum ProfileAction
{
    Deshabilitar,
    Habilitar,
    Valor,
    Umbral
}
