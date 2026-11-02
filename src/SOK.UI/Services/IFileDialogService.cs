namespace SOK.UI.Services;

/// <summary>
/// Abstracción de los diálogos de archivo de Windows para que los
/// ViewModels se puedan probar sin abrir ventanas reales.
/// </summary>
public interface IFileDialogService
{
    /// <summary>Muestra el diálogo de guardar y devuelve la ruta elegida.</summary>
    string? ShowSaveFileDialog(string title, string filter, string defaultFileName);

    /// <summary>Muestra el diálogo de abrir y devuelve la ruta elegida.</summary>
    string? ShowOpenFileDialog(string title, string filter);
}
