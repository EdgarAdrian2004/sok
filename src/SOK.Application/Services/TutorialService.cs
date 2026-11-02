using System.Globalization;
using System.Reflection;
using SOK.Application.Abstractions;
using SOK.Application.Models;

namespace SOK.Application.Services;

/// <summary>
/// Carga los pasos del tutorial desde los recursos incrustados en el
/// ensamblado. Cada archivo Markdown puede declarar un encabezado con los
/// metadatos <c>order</c>, <c>title</c>, <c>module</c> y <c>action</c>.
/// </summary>
public sealed class TutorialService : ITutorialService
{
    /// <summary>Prefijo lógico de los recursos del tutorial.</summary>
    public const string ResourcePrefix = "SOK.Application.Tutorial.";

    private readonly Lazy<IReadOnlyList<TutorialStep>> _steps;

    public TutorialService()
    {
        _steps = new Lazy<IReadOnlyList<TutorialStep>>(
            LoadSteps,
            LazyThreadSafetyMode.ExecutionAndPublication);
    }

    public Task<IReadOnlyList<TutorialStep>> GetStepsAsync(CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();
        return Task.FromResult(_steps.Value);
    }

    private static IReadOnlyList<TutorialStep> LoadSteps()
    {
        var assembly = typeof(TutorialService).Assembly;
        var resources = assembly
            .GetManifestResourceNames()
            .Where(name => name.StartsWith(ResourcePrefix, StringComparison.Ordinal) &&
                           name.EndsWith(".md", StringComparison.OrdinalIgnoreCase))
            .OrderBy(name => name, StringComparer.Ordinal)
            .ToList();

        var steps = new List<TutorialStep>(resources.Count);

        foreach (var resourceName in resources)
        {
            var raw = ReadResource(assembly, resourceName);
            var (metadata, content) = ParseFrontMatter(raw);

            var order = ParseOrder(metadata, resourceName);
            var title = GetValue(metadata, "title")
                        ?? ExtractTitle(content)
                        ?? Path.GetFileNameWithoutExtension(resourceName);

            steps.Add(new TutorialStep(
                order,
                title,
                content.Trim(),
                GetValue(metadata, "module"),
                GetValue(metadata, "action")));
        }

        return steps
            .OrderBy(step => step.Order)
            .ThenBy(step => step.Title, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    private static string ReadResource(Assembly assembly, string resourceName)
    {
        using var stream = assembly.GetManifestResourceStream(resourceName)
                           ?? throw new InvalidOperationException(
                               $"No se encontró el recurso del tutorial «{resourceName}».");
        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }

    private static (IReadOnlyDictionary<string, string> Metadata, string Content) ParseFrontMatter(
        string raw)
    {
        var metadata = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        var normalized = raw.Replace("\r\n", "\n", StringComparison.Ordinal);

        if (!normalized.StartsWith("---\n", StringComparison.Ordinal))
        {
            return (metadata, normalized);
        }

        var end = normalized.IndexOf("\n---", 4, StringComparison.Ordinal);
        if (end < 0)
        {
            return (metadata, normalized);
        }

        var header = normalized.Substring(4, end - 4);
        foreach (var line in header.Split('\n'))
        {
            var separator = line.IndexOf(':');
            if (separator <= 0)
            {
                continue;
            }

            var key = line[..separator].Trim();
            var value = line[(separator + 1)..].Trim().Trim('"', '\'');
            if (key.Length > 0)
            {
                metadata[key] = value;
            }
        }

        var contentStart = end + 4;
        var content = contentStart >= normalized.Length
            ? string.Empty
            : normalized[contentStart..].TrimStart('\n');

        return (metadata, content);
    }

    private static int ParseOrder(IReadOnlyDictionary<string, string> metadata, string resourceName)
    {
        if (GetValue(metadata, "order") is { } value &&
            int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var order))
        {
            return order;
        }

        var fileName = Path.GetFileNameWithoutExtension(resourceName);
        var separator = fileName.IndexOf('-');
        var prefix = separator > 0 ? fileName[..separator] : fileName;

        return int.TryParse(prefix, NumberStyles.Integer, CultureInfo.InvariantCulture, out var parsed)
            ? parsed
            : int.MaxValue;
    }

    private static string? GetValue(IReadOnlyDictionary<string, string> metadata, string key) =>
        metadata.TryGetValue(key, out var value) && !string.IsNullOrWhiteSpace(value)
            ? value
            : null;

    private static string? ExtractTitle(string content)
    {
        foreach (var line in content.Split('\n'))
        {
            var trimmed = line.Trim();
            if (trimmed.StartsWith("# ", StringComparison.Ordinal))
            {
                return trimmed[2..].Trim();
            }
        }

        return null;
    }
}
