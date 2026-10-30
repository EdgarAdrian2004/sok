using System.Globalization;

namespace SOK.Infrastructure.Persistence;

/// <summary>
/// Conversiones entre los tipos de .NET y los valores de texto que usa
/// el esquema SQLite de SOK.
/// </summary>
internal static class SqliteMapping
{
    private const string TimestampFormat = "yyyy-MM-dd HH:mm:ss";

    public static string ToDb(DateTime value) =>
        value.ToString(TimestampFormat, CultureInfo.InvariantCulture);

    public static DateTime FromDb(string value)
    {
        if (DateTime.TryParseExact(value, TimestampFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsed))
        {
            return parsed;
        }

        return DateTime.Parse(value, CultureInfo.InvariantCulture, DateTimeStyles.None);
    }

    public static string? NullableString(string? value) =>
        string.IsNullOrWhiteSpace(value) ? null : value;
}
