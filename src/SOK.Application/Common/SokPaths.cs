namespace SOK.Application.Common;

/// <summary>
/// Rutas por defecto de SOK. La base de datos y los respaldos se guardan
/// en LocalApplicationData para no requerir escritura en Program Files.
/// </summary>
public static class SokPaths
{
    public static string AppDataDirectory =>
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "SOK");

    public static string DatabasePath => Path.Combine(AppDataDirectory, "sok.db");

    public static string BackupDirectory => Path.Combine(AppDataDirectory, "Backups");
}
