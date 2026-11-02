using SOK.Application.Common;
using SOK.Domain.Entities;
using SOK.Domain.Enums;

namespace SOK.UI.ViewModels;

/// <summary>
/// Representación de un perfil en la interfaz. Mantiene el perfil de
/// dominio y expone textos listos para el enlace de datos.
/// </summary>
public sealed class ProfileItemViewModel
{
    public ProfileItemViewModel(Profile profile, bool isActive)
    {
        ArgumentNullException.ThrowIfNull(profile);

        Id = profile.Id;
        Name = profile.Name;
        Type = profile.Type;
        TypeText = profile.Type switch
        {
            ProfileType.Gamer => "Gamer",
            ProfileType.Ofimatico => "Ofimático",
            _ => "Personalizado"
        };
        Description = string.IsNullOrWhiteSpace(profile.Description)
            ? ProfileCatalog.GetCriterion(profile.Type)
            : profile.Description;
        IsActive = isActive;
    }

    public int Id { get; }

    public string Name { get; }

    public ProfileType Type { get; }

    public string TypeText { get; }

    public string Description { get; }

    public bool IsActive { get; }

    public string ActiveText => IsActive ? "Perfil activo" : string.Empty;

    public string ActivateButtonText => IsActive ? "Activo" : "Activar";
}
