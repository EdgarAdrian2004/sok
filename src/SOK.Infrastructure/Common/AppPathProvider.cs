using SOK.Application.Abstractions;
using SOK.Application.Common;

namespace SOK.Infrastructure.Common;

/// <summary>
/// Implementación de las rutas de SOK sobre LocalApplicationData.
/// </summary>
public sealed class AppPathProvider : IAppPathProvider
{
    public string AppDataDirectory => SokPaths.AppDataDirectory;

    public string DatabasePath => SokPaths.DatabasePath;

    public string BackupDirectory => SokPaths.BackupDirectory;
}
