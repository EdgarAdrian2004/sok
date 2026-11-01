using SOK.Domain.Enums;

namespace SOK.Domain.Entities;

/// <summary>
/// Valor leído del registro de Windows junto con su tipo. Se usa para
/// comparar el estado actual con los valores esperados del catálogo.
/// </summary>
public sealed record TelemetryValue(TelemetryValueKind Kind, string Value);
