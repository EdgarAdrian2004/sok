# Bitácora — Sprint 1

**Sprint:** Núcleo + Dashboard + Respaldos (23 SP)  
**Rama:** `feature/sprint-1-nucleo-dashboard-backup`  
**Objetivo:** sentar las bases de SOK con dominio, persistencia SQLite, navegación WPF, panel de control con WMI, bitácora y respaldos completos.

## Historias de usuario

| HU | Descripción | SP | Estado |
| --- | --- | --- | --- |
| HU-02 | Elevación UAC mediante `app.manifest` | 2 | ✅ Implementado |
| HU-03 | Ventana principal de dos columnas, navegación y vistas base | 3 | ✅ Implementado |
| HU-01 | Panel de control con CPU, RAM, disco y datos del equipo | 5 | ✅ Implementado |
| HU-04 | Bitácora de acciones en SQLite | 5 | ✅ Implementado |
| HU-16 | Respaldo completo de registro y servicios con SHA-256 | 8 | ✅ Implementado |

## Tareas técnicas completadas

- [x] Entidades, enums y contratos del dominio.
- [x] `SokDbContext` con DDL embebido y verificado.
- [x] Repositorios ADO.NET: perfiles, ajustes, respaldos, logs y configuración.
- [x] `SystemInfoProvider` con WMI (`Win32_OperatingSystem`, `Win32_Processor`, `Win32_LogicalDisk`).
- [x] `RegistryBackupService` con `reg.exe` y manifiesto SHA-256.
- [x] `ServicesBackupService` con WMI y exportación JSON.
- [x] `BackupManager` con manifiesto, hash y registro en SQLite.
- [x] `LogService` y registro de acciones del panel.
- [x] `NavigationService` propio y 7 vistas.
- [x] `DashboardView` con actualización cada 3 segundos.
- [x] Proyecto de pruebas `SOK.UI.Tests` con xUnit + Moq.
- [x] Scripts `build.ps1`, `test.ps1` y `package.ps1`.

## Verificación

- `dotnet build SOK.sln`: 0 errores, 0 advertencias.
- `dotnet test SOK.sln`: 34 pruebas correctas (30 unitarias + 4 de integración con Windows).
- Base de datos creada en `%LOCALAPPDATA%\SOK\sok.db`.
- Respaldos en `%LOCALAPPDATA%\SOK\Backups`.
- Aplicación ejecutada en Windows 10 22H2 (10.0.19045): ventana «SOK — System Optimization Key» con elevación UAC.
- Captura del panel de control: `docs/capturas/sprint-1-dashboard.png`.

## Cierre del sprint

- [x] Validar la aplicación en Windows 10 22H2.
- [x] Revisar visualmente la interfaz y ajustar estilos (texto de disco sin recortes).
- [x] Actualizar el `PLAN_MAESTRO` a la versión 1.6 con el Sprint 1 completado.

**Estado:** Sprint 1 cerrado el 30/10/2026. Siguiente iteración: B.4/B.6 (diagramas UML) y B.8 (integración continua) antes del Sprint 2.
