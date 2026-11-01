# Bitácora — Sprint 3

**Sprint:** Gestión de Servicios (13 SP)  
**Rama:** `feature/sprint-3-servicios`  
**Objetivo:** permitir que el usuario consulte el estado real de los servicios no esenciales de Windows, los detenga y deshabilite individualmente, y aplique perfiles Gamer u Ofimático, siempre con un respaldo previo a cualquier escritura.

## Historias de usuario

| HU | Descripción | SP | Estado |
| --- | --- | --- | --- |
| HU-08 | Listado de servicios con estado (al menos 20 servicios no esenciales) | 5 | ✅ Implementado (30 servicios en 10 categorías) |
| HU-09 | Detener y deshabilitar servicios individualmente | 5 | ✅ Implementado |
| HU-10 | Perfiles de servicios (Gamer / Ofimático) | 3 | ✅ Implementado |

## Tareas técnicas completadas

- [x] Enums `ServiceStartMode`, `ServiceState`, `ServiceStatus`, `ServiceCategory` y `ServiceProfile`.
- [x] Entidades `ServiceSnapshot` (lectura del SCM) y `ServiceEntry` (definición + estado observado).
- [x] `ServiceCatalog` con 30 servicios en 10 categorías, modo de inicio al habilitar y marca de riesgo.
- [x] Contratos `IServiceControlService`, `IServiceManager` y DTO `ServiceOperationResult`.
- [x] `ServiceManager`: respaldo → detención → cambio de inicio → verificación → bitácora.
- [x] `ServiceControlService` con lectura WMI (`Win32_Service`) y escritura `ServiceController`.
- [x] `NativeServiceMethods` con `ChangeServiceConfig` de `advapi32.dll` (P/Invoke).
- [x] `CreateBackupForServicesAsync` en `ServicesBackupService`: exporta solo los servicios afectados.
- [x] `ServiceCategoryNames`, `ServiceItemViewModel`, `ServiceCategoryGroup` y `ServicesViewModel`.
- [x] `ServicesView` con tarjetas por categoría, filtro, resumen, perfiles y badges de riesgo/reinicio.
- [x] Estilos `DangerBadge` y `DangerBadgeText` en `Theme.xaml`.
- [x] Registro de los nuevos servicios en el contenedor de dependencias y navegación.
- [x] 41 pruebas nuevas (35 unitarias + 6 de integración).

## Decisiones de diseño

| Decisión | Justificación |
| --- | --- |
| Lectura con WMI (`Win32_Service`) | Es la misma fuente que ya usa `ServicesBackupService`; evita dos lecturas distintas del mismo estado. |
| Escritura con `ServiceController` y `ChangeServiceConfig` | `ServiceController` cubre detener e iniciar; `ChangeServiceConfig` cubre el tipo de inicio, que .NET no expone desde .NET Core. Se descarta `sc.exe` para no analizar su salida de consola. |
| Catálogo estático en código | Los servicios son componentes conocidos de Windows; duplicarlos en SQLite crearía una fuente de verdad paralela. La importación JSON de la HU-15 cubrirá la extensión. |
| `DefaultStartMode` como modo de restauración | Al volver a habilitar un servicio, SOK aplica el modo que el propio catálogo define, no un valor deducido del equipo. |
| Un único respaldo por perfil | Evita decenas de exportaciones parciales y deja un solo archivo verificable. |
| Respaldo filtrado por servicios | El JSON contiene únicamente los servicios que se van a modificar; si ninguno existe, la operación aborta. |
| Servicios de alto riesgo solo manuales | Los perfiles nunca tocan Bluetooth, Windows Search, Windows Hello, impresión, soporte remoto ni aplicaciones nativas. |
| Fallo al detener como advertencia | Si el servicio no se puede detener pero el tipo de inicio se aplica, el cambio queda registrado y se informa que se detendrá al reiniciar. |

## Criterio de los perfiles

| Grupo | Servicios |
| --- | --- |
| Base común (Gamer y Ofimático) | `DiagTrack`, `dmwappushservice`, `WerSvc`, `Fax`, `RpcLocator`, `SysMain`, `TrkWks`, `MapsBroker`, `lfsvc`, `WalletService`, `PhoneSvc`, `SDRSVC`, `WMPNetworkSvc`, `SSDPSRV`, `upnphost`, `lmhosts` |
| Solo Gamer | `PrintNotify` |
| Solo Ofimático | `XboxGipSvc`, `XboxNetApiSvc`, `XblAuthManager`, `XblGameSave` |
| Nunca en perfiles (solo manual) | `WSearch`, `SharedAccess`, `bthserv`, `WbioSrvc`, `RemoteRegistry`, `RemoteAccess`, `seclogon`, `stisvc`, `Spooler` |

## Desviación respecto del plan inicial

El plan original de 13 SP enumeraba los servicios de forma genérica. El catálogo final se amplió a **30 servicios en 10 categorías** con modo de inicio, marca de riesgo y una separación explícita entre lo que aplican los perfiles y lo que solo puede cambiarse manualmente. No hubo cambios de esquema: el estado se lee en vivo del SCM y la auditoría reutiliza la tabla `Logs` con `LogModule.Servicios`. El ajuste quedó documentado en el `PLAN_MAESTRO` v1.10.

## Verificación

- `dotnet build SOK.sln -c Debug`: 0 errores, 0 advertencias.
- `dotnet build SOK.sln -c Release`: 0 errores, 0 advertencias.
- `dotnet test SOK.sln -c Debug`: 96 pruebas correctas (55 anteriores + 41 nuevas).
- `dotnet test SOK.sln -c Release`: 96 pruebas correctas, 0 fallidas (salida TRX en `artifacts/test-results/resultados.trx`).
- Estado leído en la máquina de validación: «17 de 29 servicios deshabilitados»; `RpcLocator` no existe en Windows 10, por eso el catálogo muestra 29 servicios aplicables.
- Prueba de integración del SCM: `GetAllAsync` devuelve los servicios del equipo y `GetAsync` distingue un servicio existente de uno inexistente (sin elevación).
- Prueba de integración del respaldo: el JSON contiene solo `Dnscache` y `Spooler`; una lista sin servicios existentes lanza `InvalidOperationException`.
- Validación real sobre `PrintNotify` (servicio de notificaciones de impresión, sin impacto en la impresión):
  - Antes: inicio `Manual`, estado `Detenido`, configuración `Habilitado`.
  - Deshabilitar: `Success = true`, respaldo `servicios_20261101_092419_988\services-backup.json`.
  - Después: inicio `Deshabilitado`, configuración `Deshabilitado`.
  - Habilitar: `Success = true`, respaldo `servicios_20261101_092420_523\services-backup.json`.
  - Después: inicio `Manual`, configuración `Habilitado`.
- Contenido del respaldo: únicamente `PrintNotify`, con `StartMode: Manual` y `State: Stopped`, es decir, el estado previo a la escritura.
- Bitácora de la base real: dos entradas en `Logs` con `LogModule.Servicios` («Deshabilitar servicio» y «Habilitar servicio»), resultado `Exitoso` y los identificadores de respaldo 1 y 2.
- Aplicación ejecutada en Windows 10 22H2 (10.0.19045) con elevación UAC.
- Captura del módulo: `docs/capturas/sprint-3-servicios.png`.

## Incidencias y correcciones

| Incidencia | Corrección |
| --- | --- |
| `ServiceStartMode` era ambiguo entre `SOK.Domain.Enums` y `System.ServiceProcess` | Se agregó un alias de tipo en `ServiceControlService`. |
| El resumen del encabezado se recortaba porque los controles ocupaban la misma fila | El resumen y los controles se separaron en dos filas dentro de la tarjeta. |
| El pie del menú seguía indicando «Sprint 2» | Se actualizó a «Sprint 3». |
| Tres pruebas unitarias asumían que `Fax` no existía en el estado falso y que el fallo mostraba el identificador | Se corrigieron los datos de prueba y se verificó el nombre visible en el mensaje. |
| La captura visual fallaba por UIPI: la aplicación es `requireAdministrator` y el shell de trabajo no estaba elevado | Se ejecutó un script de PowerShell elevado que cierra la instancia previa, compila en Release, lanza SOK, navega con UI Automation y captura con `PrintWindow`. |

## Commits del sprint

| Commit | Tipo | Contenido |
| --- | --- | --- |
| `fed1a0e` | `feat(services)` | Dominio, catálogo, contratos y orquestador. |
| `1452406` | `feat(infrastructure)` | Control del SCM con WMI, `ServiceController` y P/Invoke; respaldo filtrado. |
| `1f8d52a` | `feat(ui)` | Vista, ViewModels, estilos, DI y navegación. |
| `c745c28` | `test(services)` | 41 pruebas de catálogo, gestor, vista e integración. |
| Este commit | `docs` | Plan maestro v1.10, README, bitácora y captura. |

**Total:** 35 archivos modificados, 3 064 líneas agregadas y 39 eliminadas.

## Cierre del sprint

- [x] Validar la aplicación en Windows 10 22H2.
- [x] Revisar visualmente la interfaz y corregir el encabezado y el pie.
- [x] Validar una operación real de deshabilitar y habilitar con respaldo y bitácora.
- [x] Actualizar el `PLAN_MAESTRO` a la versión 1.10 con el Sprint 3 completado.

**Estado:** Sprint 3 cerrado el 01/11/2026. Siguiente iteración: Sprint 4 (Liberación de Memoria, HU-11 a HU-13).
