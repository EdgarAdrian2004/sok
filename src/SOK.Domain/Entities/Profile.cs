using SOK.Domain.Enums;

namespace SOK.Domain.Entities;

/// <summary>
/// Perfil de optimización (Gamer, Ofimático o Personalizado).
/// </summary>
public sealed class Profile
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public ProfileType Type { get; set; }

    public string? Description { get; set; }

    public bool IsDefault { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    public List<ProfileSetting> Settings { get; set; } = new();
}
