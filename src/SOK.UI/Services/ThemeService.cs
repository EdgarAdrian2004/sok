using System.Windows;
using Microsoft.Extensions.DependencyInjection;
using SOK.Domain.Abstractions;

namespace SOK.UI.Services;

/// <summary>
/// Administra el tema visual (claro/oscuro) de la aplicación y lo persiste
/// en la configuración.
/// </summary>
public sealed class ThemeService : CommunityToolkit.Mvvm.ComponentModel.ObservableObject
{
    private readonly ISettingsRepository _settings;
    private const string SettingKey = "Theme";

    public ThemeService(ISettingsRepository settings)
    {
        _settings = settings ?? throw new ArgumentNullException(nameof(settings));
    }

    private string _currentTheme = "Dark";

    public string CurrentTheme
    {
        get => _currentTheme;
        private set
        {
            if (SetProperty(ref _currentTheme, value))
            {
                OnPropertyChanged(nameof(IsDark));
                OnPropertyChanged(nameof(ToggleLabel));
            }
        }
    }

    public bool IsDark => string.Equals(CurrentTheme, "Dark", StringComparison.OrdinalIgnoreCase);

    public string ToggleLabel => IsDark ? "Tema claro" : "Tema oscuro";

    /// <summary>
    /// Aplica el tema guardado (o el predeterminado oscuro) sin persistir.
    /// </summary>
    public void ApplySaved()
    {
        try
        {
            var setting = _settings.GetAsync(SettingKey).GetAwaiter().GetResult();
            var theme = setting?.Value;
            if (string.IsNullOrWhiteSpace(theme))
            {
                theme = "Dark";
            }
            Apply(theme, persist: false);
        }
        catch
        {
            Apply("Dark", persist: false);
        }
    }

    public void Apply(string theme, bool persist = true)
    {
        theme = theme.Equals("Light", StringComparison.OrdinalIgnoreCase) ? "Light" : "Dark";

        var dictionaries = System.Windows.Application.Current.Resources.MergedDictionaries;
        var toRemove = dictionaries
            .Where(d => d.Source is not null &&
                        d.Source.OriginalString.Contains("/Themes/", StringComparison.OrdinalIgnoreCase))
            .ToList();
        foreach (var dictionary in toRemove)
        {
            dictionaries.Remove(dictionary);
        }

        var uri = new Uri($"Styles/Themes/{theme}.xaml", UriKind.Relative);
        dictionaries.Add(new System.Windows.ResourceDictionary { Source = uri });

        CurrentTheme = theme;

        if (persist)
        {
            try
            {
                _settings.SetAsync(SettingKey, theme).GetAwaiter().GetResult();
            }
            catch
            {
                // La persistencia no debe interrumpir el cambio visual.
            }
        }
    }

    public void Toggle() => Apply(IsDark ? "Light" : "Dark");
}
