using SOK.Domain.Enums;

namespace SOK.UI.ViewModels;

/// <summary>
/// Nombres en español de las categorías de servicios que se muestran en la
/// interfaz.
/// </summary>
public static class ServiceCategoryNames
{
    public static string GetName(ServiceCategory category) =>
        category switch
        {
            ServiceCategory.Telemetria => "Telemetría",
            ServiceCategory.Obsoleto => "Servicios obsoletos",
            ServiceCategory.Rendimiento => "Rendimiento",
            ServiceCategory.Privacidad => "Privacidad",
            ServiceCategory.Red => "Red local",
            ServiceCategory.Xbox => "Xbox",
            ServiceCategory.Bluetooth => "Bluetooth",
            ServiceCategory.Biometria => "Biometría",
            ServiceCategory.Seguridad => "Seguridad",
            ServiceCategory.Perifericos => "Periféricos",
            _ => category.ToString()
        };
}
