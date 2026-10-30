using SOK.Domain.Enums;

namespace SOK.Domain.Entities;

/// <summary>
/// Ajuste individual que compone un perfil de optimización.
/// </summary>
public sealed class ProfileSetting
{
    public int Id { get; set; }

    public int ProfileId { get; set; }

    public ProfileModule Module { get; set; }

    public string TargetKey { get; set; } = string.Empty;

    public ProfileAction Action { get; set; }

    public string? Value { get; set; }
}
