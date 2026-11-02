namespace SOK.Application.Models;

/// <summary>
/// Paso del tutorial integrado. El contenido se redacta en Markdown y la
/// interfaz lo convierte en un documento WPF. La clave de módulo permite
/// ofrecer un botón para abrir el módulo correspondiente.
/// </summary>
public sealed record TutorialStep(
    int Order,
    string Title,
    string ContentMarkdown,
    string? ModuleKey,
    string? ActionName);
