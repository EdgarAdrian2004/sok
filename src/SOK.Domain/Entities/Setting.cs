namespace SOK.Domain.Entities;

/// <summary>
/// Par clave/valor de configuración de la aplicación.
/// </summary>
public sealed class Setting
{
    public string Key { get; set; } = string.Empty;

    public string Value { get; set; } = string.Empty;

    public DateTime UpdatedAt { get; set; } = DateTime.Now;
}
