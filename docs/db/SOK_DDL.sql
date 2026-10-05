-- ============================================================
--  SOK (System Optimization Key)
--  Esquema de base de datos SQLite - DISENO FISICO (V.17.3)
--  Autor: Edgar Adrian Flores Herrera
--  IESTP Trujillo - TAP
--  Version: 1.0 - 04/10/2026
--  Verificado: si (prueba de ejecucion con SQLite 3.50.4)
-- ============================================================

-- ------------------------------------------------------------
-- PRAGMAS DE LA SESION (deben ejecutarse en cada conexion)
-- OJO: SQLite NO valida claves foraneas salvo que se declare
-- PRAGMA foreign_keys = ON; es el error mas comun en .NET.
-- ------------------------------------------------------------
PRAGMA foreign_keys = ON;
PRAGMA journal_mode = WAL;

-- ------------------------------------------------------------
-- TABLA: Profiles
-- Perfiles de optimizacion (Gamer, Ofimatico, Personalizado).
-- ------------------------------------------------------------
CREATE TABLE IF NOT EXISTS Profiles (
    Id          INTEGER PRIMARY KEY AUTOINCREMENT,
    Name        TEXT    NOT NULL UNIQUE,
    Type        TEXT    NOT NULL CHECK (Type IN ('Gamer', 'Ofimatico', 'Personalizado')),
    Description TEXT,
    IsDefault   INTEGER NOT NULL DEFAULT 0 CHECK (IsDefault IN (0, 1)),
    CreatedAt   TEXT    NOT NULL DEFAULT (datetime('now', 'localtime')),
    UpdatedAt   TEXT    NOT NULL DEFAULT (datetime('now', 'localtime'))
);

-- ------------------------------------------------------------
-- TABLA: ProfileSettings
-- Acciones que componen un perfil (1 perfil : N ajustes).
-- Module/TargetKey replican lo que SOK aplicara en Windows.
-- ------------------------------------------------------------
CREATE TABLE IF NOT EXISTS ProfileSettings (
    Id        INTEGER PRIMARY KEY AUTOINCREMENT,
    ProfileId INTEGER NOT NULL,
    Module    TEXT    NOT NULL CHECK (Module IN ('Telemetria', 'Servicios', 'Memoria')),
    TargetKey TEXT    NOT NULL,
    Action    TEXT    NOT NULL CHECK (Action IN ('Deshabilitar', 'Habilitar', 'Valor', 'Umbral')),
    Value     TEXT,
    FOREIGN KEY (ProfileId) REFERENCES Profiles (Id) ON DELETE CASCADE,
    UNIQUE (ProfileId, Module, TargetKey)
);

-- ------------------------------------------------------------
-- TABLA: Backups
-- Metadatos de los respaldos. El archivo (.reg / .json) vive en
-- el sistema de archivos; aqui se guarda ruta + hash SHA-256.
-- ------------------------------------------------------------
CREATE TABLE IF NOT EXISTS Backups (
    Id          INTEGER PRIMARY KEY AUTOINCREMENT,
    Name        TEXT    NOT NULL,
    BackupType  TEXT    NOT NULL CHECK (BackupType IN ('Registro', 'Servicios', 'Completo')),
    FilePath    TEXT    NOT NULL,
    HashSha256  TEXT    NOT NULL,
    Description TEXT,
    CreatedAt   TEXT    NOT NULL DEFAULT (datetime('now', 'localtime'))
);

-- ------------------------------------------------------------
-- TABLA: Logs
-- Bitacora de acciones (HU-04). Registro append-only.
-- ------------------------------------------------------------
CREATE TABLE IF NOT EXISTS Logs (
    Id        INTEGER PRIMARY KEY AUTOINCREMENT,
    Timestamp TEXT    NOT NULL DEFAULT (datetime('now', 'localtime')),
    Module    TEXT    NOT NULL CHECK (Module IN ('Acceso', 'Dashboard', 'Telemetria',
                                                 'Servicios', 'Memoria', 'Respaldos', 'Perfiles')),
    Action    TEXT    NOT NULL,
    TargetKey TEXT,
    Result    TEXT    NOT NULL CHECK (Result IN ('Exitoso', 'Fallido', 'Revertido')),
    Detail    TEXT,
    BackupId  INTEGER,
    FOREIGN KEY (BackupId) REFERENCES Backups (Id) ON DELETE SET NULL
);

-- ------------------------------------------------------------
-- TABLA: Settings
-- Pares clave/valor de la configuracion de la aplicacion
-- (perfil activo, umbral de memoria, inicio con Windows, etc.).
-- ------------------------------------------------------------
CREATE TABLE IF NOT EXISTS Settings (
    SettingKey   TEXT PRIMARY KEY,
    SettingValue TEXT NOT NULL,
    UpdatedAt    TEXT    NOT NULL DEFAULT (datetime('now', 'localtime'))
);

-- ------------------------------------------------------------
-- INDICES
-- ------------------------------------------------------------
CREATE INDEX IF NOT EXISTS IX_ProfileSettings_ProfileId ON ProfileSettings (ProfileId);
CREATE INDEX IF NOT EXISTS IX_Backups_CreatedAt         ON Backups (CreatedAt);
CREATE INDEX IF NOT EXISTS IX_Backups_Hash              ON Backups (HashSha256);
CREATE INDEX IF NOT EXISTS IX_Logs_Timestamp            ON Logs (Timestamp);
CREATE INDEX IF NOT EXISTS IX_Logs_Module               ON Logs (Module);

-- ------------------------------------------------------------
-- DATOS INICIALES (opcionales: perfiles predefinidos)
-- ------------------------------------------------------------
INSERT INTO Profiles (Name, Type, Description, IsDefault)
VALUES ('Ofimatico', 'Ofimatico', 'Perfil para uso administrativo y docente: prioriza estabilidad y consumo moderado de recursos.', 1)
ON CONFLICT (Name) DO NOTHING;

INSERT INTO Profiles (Name, Type, Description, IsDefault)
VALUES ('Gamer', 'Gamer', 'Perfil que libera la mayor cantidad de recursos: deshabilita telemetrias y servicios no esenciales.', 0)
ON CONFLICT (Name) DO NOTHING;

INSERT INTO Settings (SettingKey, SettingValue)
VALUES ('MemoryReleaseThreshold', '80')
ON CONFLICT (SettingKey) DO NOTHING;

INSERT INTO Settings (SettingKey, SettingValue)
VALUES ('ActiveProfileId', '1')
ON CONFLICT (SettingKey) DO NOTHING;
