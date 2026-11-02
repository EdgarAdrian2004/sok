using SOK.Application.DTOs;

namespace SOK.UI.ViewModels;

/// <summary>
/// Representación de un respaldo en la interfaz, con su estado de
/// integridad y la fecha formateada.
/// </summary>
public sealed class BackupItemViewModel
{
    public BackupItemViewModel(BackupStatus status)
    {
        ArgumentNullException.ThrowIfNull(status);

        Id = status.Id;
        Name = status.Name;
        TypeText = status.TypeText;
        DateText = status.CreatedAt.ToString("dd/MM/yyyy HH:mm:ss");
        Description = string.IsNullOrWhiteSpace(status.Description)
            ? "Sin descripción"
            : status.Description;
        FilePath = status.FilePath;
        IntegrityText = status.IntegrityText;
        CanRestore = status.CanRestore;
        IsIntegrityOk = status.FileExists && status.HashMatches;
    }

    public int Id { get; }

    public string Name { get; }

    public string TypeText { get; }

    public string DateText { get; }

    public string Description { get; }

    public string FilePath { get; }

    public string IntegrityText { get; }

    public bool CanRestore { get; }

    public bool IsIntegrityOk { get; }

    public string RestoreButtonText => CanRestore ? "Restaurar" : "No restaurable";
}
