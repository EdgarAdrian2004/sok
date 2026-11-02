namespace SOK.Application.DTOs;

/// <summary>
/// Representación serializable de un perfil para exportar o importar en
/// JSON. Los enums se guardan como texto para que el archivo sea legible y
/// estable entre versiones.
/// </summary>
public sealed record ProfileExportDto(
    int SchemaVersion,
    string Name,
    string Type,
    string? Description,
    IReadOnlyList<ProfileSettingExportDto> Settings)
{
    public const int CurrentSchemaVersion = 1;
}

/// <summary>Ajuste individual dentro de un perfil exportado.</summary>
public sealed record ProfileSettingExportDto(
    string Module,
    string TargetKey,
    string Action,
    string? Value);
