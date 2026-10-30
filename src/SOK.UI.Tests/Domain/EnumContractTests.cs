using SOK.Domain.Enums;
using Xunit;

namespace SOK.UI.Tests.Domain;

/// <summary>
/// Verifica que los nombres de los enums coincidan exactamente con los
/// valores admitidos por los CHECK del esquema SQLite.
/// </summary>
public sealed class EnumContractTests
{
    [Theory]
    [InlineData(ProfileType.Gamer, "Gamer")]
    [InlineData(ProfileType.Ofimatico, "Ofimatico")]
    [InlineData(ProfileType.Personalizado, "Personalizado")]
    public void ProfileType_CoincideConElDDL(ProfileType value, string expected)
    {
        Assert.Equal(expected, value.ToString());
    }

    [Theory]
    [InlineData(ProfileModule.Telemetria, "Telemetria")]
    [InlineData(ProfileModule.Servicios, "Servicios")]
    [InlineData(ProfileModule.Memoria, "Memoria")]
    public void ProfileModule_CoincideConElDDL(ProfileModule value, string expected)
    {
        Assert.Equal(expected, value.ToString());
    }

    [Theory]
    [InlineData(ProfileAction.Deshabilitar, "Deshabilitar")]
    [InlineData(ProfileAction.Habilitar, "Habilitar")]
    [InlineData(ProfileAction.Valor, "Valor")]
    [InlineData(ProfileAction.Umbral, "Umbral")]
    public void ProfileAction_CoincideConElDDL(ProfileAction value, string expected)
    {
        Assert.Equal(expected, value.ToString());
    }

    [Theory]
    [InlineData(BackupType.Registro, "Registro")]
    [InlineData(BackupType.Servicios, "Servicios")]
    [InlineData(BackupType.Completo, "Completo")]
    public void BackupType_CoincideConElDDL(BackupType value, string expected)
    {
        Assert.Equal(expected, value.ToString());
    }

    [Theory]
    [InlineData(LogModule.Acceso, "Acceso")]
    [InlineData(LogModule.Dashboard, "Dashboard")]
    [InlineData(LogModule.Telemetria, "Telemetria")]
    [InlineData(LogModule.Servicios, "Servicios")]
    [InlineData(LogModule.Memoria, "Memoria")]
    [InlineData(LogModule.Respaldos, "Respaldos")]
    [InlineData(LogModule.Perfiles, "Perfiles")]
    public void LogModule_CoincideConElDDL(LogModule value, string expected)
    {
        Assert.Equal(expected, value.ToString());
    }

    [Theory]
    [InlineData(ActionResult.Exitoso, "Exitoso")]
    [InlineData(ActionResult.Fallido, "Fallido")]
    [InlineData(ActionResult.Revertido, "Revertido")]
    public void ActionResult_CoincideConElDDL(ActionResult value, string expected)
    {
        Assert.Equal(expected, value.ToString());
    }
}
