# Diseño de la base de datos — Sistema SOK

**Sección V.17 del informe TAP**
Archivos: `SOK_DDL.sql` (fuente autoritativa) · `ER_SOK.dbml` (dbdiagram.io) · `ER_SOK.puml` / `ER_SOK.png`
Versión: 1.0 — 04/10/2026 — **DDL verificado con SQLite 3.50.4**

---

## 1. Decisiones previas

Antes de modelar se define qué información vive en cada almacenamiento:

| Información | Dónde se guarda | Justificación |
| --- | --- | --- |
| Perfiles y sus ajustes | **SQLite** | Estado mutable del usuario, debe sobrevivir a los reinicios |
| Metadatos de respaldos (ruta + hash) | **SQLite** | El archivo vive en disco; la BD garantiza trazabilidad e integridad |
| Bitácora de acciones | **SQLite** | Requerimiento explícito (HU-04), consulta ordenada por fecha |
| Configuración de la app (umbral, perfil activo) | **SQLite** | Preferencias persistentes |
| Archivos de respaldo (`.reg`, `.json`) | **Sistema de archivos** | Binarios/Texto que debe ser re-aplicable por Windows |
| Perfil exportado (HU-15) | **Archivo JSON** | Formato intercambiable entre equipos |
| Catálogos de telemetrías (30+ claves) y servicios (20+) | **Código** (`SOK.Domain`) | Datos de configuración versionados con el programa; cambiarlos exige revisión de código, no de datos |

**Motor elegido:** SQLite embebido. Sin servidor, un único archivo, transacciones ACID y experiencia previa del perfil del egresado en el ciclo IV.

## 2. Diseño conceptual — V.17.1

### 2.1 Entidades

| Entidad | Descripción | Atributos principales |
| --- | --- | --- |
| **Profile** | Perfil de optimización (Gamer, Ofimático, Personalizado) | `Name`, `Type`, `IsDefault`, fechas |
| **ProfileSetting** | Ajuste individual que compone un perfil | `Module`, `TargetKey`, `Action`, `Value` |
| **Backup** | Metadato de un respaldo generado | `BackupType`, `FilePath`, `HashSha256`, `CreatedAt` |
| **Log** | Registro de una acción ejecutada | `Timestamp`, `Module`, `Action`, `Result`, `Detail` |
| **Setting** | Par clave-valor de configuración de la aplicación | `SettingKey`, `SettingValue` |

### 2.2 Relaciones y cardinalidades

| Relación | Cardinalidad | Regla de negocio |
| --- | --- | --- |
| Profile — ProfileSetting | **1 : N** | Un perfil contiene varios ajustes; un ajuste pertenece a un solo perfil. Al eliminar un perfil se eliminan sus ajustes (cascada). |
| Backup — Log | **1 : N** | Un respaldo puede estar asociado a varias acciones (crear + restaurar); una acción referencia como máximo un respaldo. Si el respaldo se elimina, el log conserva el registro con `BackupId = NULL`. |

Las entidades **Setting** y **Log** no presentan relaciones obligatorias adicionales: `Log.BackupId` es la única clave foránea opcional del modelo.

Diagrama: `ER_SOK.png` (espejo editable: `ER_SOK.dbml` para dbdiagram.io).

## 3. Diseño lógico — V.17.2

### 3.1 Mapeo entidad → tabla

| Tabla | Clave primaria | Claves foráneas | Índices |
| --- | --- | --- | --- |
| `Profiles` | `Id` (INTEGER, autoincremento) | — | UNIQUE sobre `Name` |
| `ProfileSettings` | `Id` | `ProfileId → Profiles.Id` (CASCADE) | `IX_ProfileSettings_ProfileId`; UNIQUE (`ProfileId`,`Module`,`TargetKey`) |
| `Backups` | `Id` | — | `IX_Backups_CreatedAt`, `IX_Backups_Hash` |
| `Logs` | `Id` | `BackupId → Backups.Id` (SET NULL) | `IX_Logs_Timestamp`, `IX_Logs_Module` |
| `Settings` | `SettingKey` (TEXT) | — | — (la PK ya es índice) |

### 3.2 Verificación de normalización (3FN)

| Tabla | Análisis |
| --- | --- |
| `Profiles` | Todos los atributos dependen de `Id`. No hay dependencias transitivas: el tipo de perfil se almacena como valor y **no** se deriva de otra columna. |
| `ProfileSettings` | Dependencia funcional completa: (`ProfileId`, `Module`, `TargetKey`) → `Action`, `Value`. Sin grupos repetitivos ni dependencias transitivas. |
| `Backups` | Todos los atributos dependen de `Id`. El hash no determina ningún otro atributo, por lo que se indexa pero no se declara UNIQUE (permitiría detectar duplicados sin prohibirlos). |
| `Logs` | `Id` → resto de columnas. `BackupId` solo referencia, no deriva datos del respaldo (no se copia ruta ni hash), evitando así la dependencia transitiva `Log → Backup → FilePath`. |
| `Settings` | `SettingKey` → `SettingValue`, `UpdatedAt`. Relación clave-valor sin dependencias transitivas. |

**Conclusión:** las cinco tablas cumplen la Tercera Forma Normal.

### 3.3 Decisiones de modelado

1. **Booleanos como `INTEGER 0/1`** — SQLite no tiene tipo booleano; se refuerza con `CHECK (... IN (0,1))`.
2. **Fechas como `TEXT` en formato ISO-8601** (`YYYY-MM-DD HH:MM:SS`) generado por `datetime('now','localtime')`: ordenables lexicográficamente y legibles.
3. **Enumeraciones como `TEXT` con `CHECK`** — en lugar de tablas de dominio, porque el dominio es fijo y acotado; los valores se almacenan sin acentos (`Ofimatico`, `Telemetria`) para evitar problemas de codificación, presentándose acentuados en la interfaz.
4. **Claves de Windows como `TEXT` sin normalizar** — una tabla catálogo añadiría complejidad sin beneficio, dado que los catálogos viven en el código.
5. **Bitácora append-only** — `Logs` solo recibe inserciones; la edición está prohibida por diseño.

## 4. Diseño físico — V.17.3

### 4.1 Tipos de datos utilizados

| Tipo SQLite | Uso |
| --- | --- |
| `INTEGER PRIMARY KEY AUTOINCREMENT` | Identificadores (alias de `rowid`) |
| `TEXT` | Cadenas, fechas, claves de registro, rutas, hashes |
| `TEXT` + `CHECK` | Enumeraciones |
| `INTEGER` + `CHECK (0,1)` | Booleanos |

### 4.2 PRAGMAS obligatorios (gotcha de implementación)

```sql
PRAGMA foreign_keys = ON;   -- SQLite NO valida FK por defecto
PRAGMA journal_mode = WAL;  -- concurrencia lectura/escritura
```

> SQLite **no** hace cumplir las claves foráneas salvo que se declare `PRAGMA foreign_keys = ON` **en cada conexión**. Es el error más común al pasar de un SGBD cliente-servidor a SQLite desde .NET.

### 4.3 Verificación ejecutada (04/10/2026)

El script `SOK_DDL.sql` fue ejecutado contra SQLite 3.50.4 con los siguientes resultados:

| Prueba | Resultado |
| --- | --- |
| Creación de 5 tablas y 5 índices | ✅ |
| CRUD básico (insertar perfil, ajuste, respaldo, log) | ✅ |
| Rechazo de `Type` fuera de `CHECK` | ✅ |
| Rechazo de `Module` fuera de `CHECK` en `Logs` | ✅ |
| Rechazo de clave foránea inexistente | ✅ |
| `ON DELETE CASCADE` en `ProfileSettings` | ✅ |
| `ON DELETE SET NULL` en `Logs.BackupId` | ✅ |
| Restricción `UNIQUE` en `Profiles.Name` | ✅ |
| `PRAGMA foreign_keys = 1` | ✅ |
| `PRAGMA journal_mode = wal` (archivo) | ✅ |
| `PRAGMA integrity_check = ok` | ✅ |
| `PRAGMA foreign_key_check` sin violaciones | ✅ |
| Inserción de 10 001 registros en `Logs` | ✅ 0,12 s |
| Plan de consulta por `Timestamp` usa `IX_Logs_Timestamp` | ✅ |

**Conclusión:** el diseño físico está verificado y es apto para implementarse en la capa `SOK.Infrastructure.Persistence`.

## 5. Puntos de atención para la implementación

1. Abrir la conexión SQLite con `foreign_keys=ON` en **cada** sesión (una sola conexión global en la app).
2. El archivo de BD debe ubicarse en `%LOCALAPPDATA%\SOK\sok.db` (no en `Program Files`, que requiere elevación para escribir).
3. Con `journal_mode=WAL` se generan archivos auxiliares `-wal` y `-shm`; incluirlos en la limpieza/desinstalación.
4. Todas las escrituras de optimización deben ocurrir **después** del respaldo exitoso (regla de oro).
5. Usar transacciones en operaciones masivas (UC-08 «Privacidad máxima») para permitir revertir de forma atómica.
