namespace SOK.Application.Abstractions;

/// <summary>
/// Rutas de almacenamiento de SOK. Se abstrae para poder usar directorios
/// temporales en las pruebas unitarias.
/// </summary>
public interface IAppPathProvider
{
    string AppDataDirectory { get; }

    string DatabasePath { get; }

    string BackupDirectory { get; }
}
