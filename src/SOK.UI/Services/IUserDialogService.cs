namespace SOK.UI.Services;

/// <summary>
/// Abstracción de los cuadros de confirmación e información para que los
/// ViewModels se puedan probar sin abrir ventanas reales.
/// </summary>
public interface IUserDialogService
{
    /// <summary>Muestra una confirmación y devuelve <c>true</c> si el usuario acepta.</summary>
    bool Confirm(string title, string message);

    /// <summary>Muestra un aviso informativo.</summary>
    void ShowInfo(string title, string message);

    /// <summary>Muestra un aviso de error.</summary>
    void ShowError(string title, string message);
}
