using SOK.Application.Models;

namespace SOK.Application.Abstractions;

/// <summary>
/// Fachada del tutorial integrado: entrega los pasos redactados en Markdown
/// y ordenados según su metadato <c>order</c>.
/// </summary>
public interface ITutorialService
{
    /// <summary>Devuelve los pasos del tutorial en orden de presentación.</summary>
    Task<IReadOnlyList<TutorialStep>> GetStepsAsync(CancellationToken cancellationToken = default);
}
