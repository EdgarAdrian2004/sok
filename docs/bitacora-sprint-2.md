# Bitácora — Sprint 2

**Sprint:** Telemetría y Privacidad (16 SP)  
**Rama:** `feature/sprint-2-telemetria-privacidad`  
**Objetivo:** permitir que el usuario consulte el estado real de las telemetrías de Windows, las deshabilite de forma individual y aplique una configuración de privacidad máxima, siempre con un respaldo del registro previo a cualquier escritura.

## Historias de usuario

| HU | Descripción | SP | Estado |
| --- | --- | --- | --- |
| HU-05 | Listado de telemetrías con estado (al menos 30 claves de registro organizadas por categoría) | 5 | ✅ Implementado (46 claves en 11 categorías) |
| HU-06 | Interruptor para deshabilitar telemetría individualmente | 8 | ✅ Implementado |
| HU-07 | Botón «Privacidad máxima» que aplica todas las telemetrías de una sola vez | 3 | ✅ Implementado |

## Tareas técnicas completadas

- [x] Enums `TelemetryCategory`, `TelemetryStatus` y `TelemetryValueKind`.
- [x] Entidades `TelemetryKey` (definición + estado observado) y `TelemetryValue` (lectura tipada del registro).
- [x] `TelemetryCatalog` con 46 claves en 11 categorías y sus valores habilitado/deshabilitado.
- [x] Contratos `ITelemetryRegistryService` e `ITelemetryManager`.
- [x] `TelemetryManager`: respaldo → escritura → verificación → bitácora.
- [x] `TelemetryRegistryService` con `Microsoft.Win32.Registry` 5.0.0 (lectura y escritura `DWORD`/`String`).
- [x] `RegistryPathHelper` para interpretar rutas `HKCU\…` y `HKLM\…`.
- [x] `CreateBackupForKeysAsync` en `RegistryBackupService`: exporta solo las claves afectadas.
- [x] `TelemetryViewModel`, `TelemetryItemViewModel` y `TelemetryCategoryGroup`.
- [x] `TelemetryView` con tarjetas por categoría, filtro, resumen y badges de reinicio.
- [x] Estilos `SokToggleSwitch`, `WarningBadge` y `BadgeText` en `Theme.xaml`.
- [x] Registro de los nuevos servicios en el contenedor de dependencias.
- [x] 21 pruebas nuevas (catálogo, gestor, registro real, respaldo real y ViewModel).

## Decisiones de diseño

| Decisión | Justificación |
| --- | --- |
| Catálogo estático en código, no en base de datos | El estado real vive en el registro de Windows; duplicarlo en SQLite crearía una fuente de verdad paralela que se desincroniza. |
| Auditoría en la tabla `Logs` existente (`LogModule.Telemetria`) | El DDL ya preveía el módulo; evita tablas y repositorios redundantes. |
| `Microsoft.Win32.Registry` en lugar de `reg.exe` para leer y escribir | Permite comparar tipos y valores con precisión; `reg.exe` se mantiene solo para exportar los respaldos. |
| Un único respaldo en «Privacidad máxima» | Evita decenas de exportaciones parciales y deja un solo manifiesto verificable para la restauración futura. |
| Abortar si una clave existente no se puede exportar | La regla de oro exige respaldar antes de modificar; continuar sin respaldo la incumpliría. |
| Registrar las claves inexistentes como «no requiere respaldo» | En equipos con Windows 10 u 11 hay claves que no existen; no son un error y no deben bloquear la operación. |

## Desviación respecto del plan inicial

El plan original de 23 SP proponía las tablas `TelemetryKeys` y `TelemetryActionLogs`. Se descartaron por las razones indicadas arriba. El alcance funcional de las tres historias se mantiene; el cambio es de persistencia y quedó documentado en el `PLAN_MAESTRO` v1.9.

## Verificación

- `dotnet build SOK.sln -c Debug`: 0 errores, 0 advertencias.
- `dotnet build SOK.sln -c Release`: 0 errores, 0 advertencias.
- `dotnet test SOK.sln -c Debug`: 55 pruebas correctas (34 del Sprint 1 + 21 nuevas).
- `dotnet test SOK.sln -c Release`: 55 pruebas correctas, 0 fallidas (salida TRX en `artifacts/test-results/resultados.trx`).
- Estado leído en la máquina de validación: «38 de 46 claves deshabilitadas».
- Prueba de integración del registro sobre una clave temporal en `HKCU\Software\SOK\Tests` (sin elevación y sin tocar la configuración del equipo).
- Prueba de integración del respaldo: genera el `.reg`, el manifiesto y el SHA-256 recomputable; una clave inexistente queda registrada sin archivo.
- Aplicación ejecutada en Windows 10 22H2 (10.0.19045) con elevación UAC.
- Captura del módulo: `docs/capturas/sprint-2-telemetria.png`.

## Incidencias y correcciones

| Incidencia | Corrección |
| --- | --- |
| El título «Telemetría y privacidad» aparecía duplicado y recortado, porque `MainWindow` ya lo muestra en la cabecera | Se eliminó el título de la vista y se conservó la descripción del respaldo. |
| La vista arrancaba desplazada unos 40 px al entrar al módulo | `TelemetryView` vuelve al inicio del desplazamiento en el evento `Loaded`, con prioridad `DispatcherPriority.Loaded`. |
| El pie del menú seguía indicando «Sprint 1» | Se actualizó a «Sprint 2». |
| La captura visual fallaba por UIPI: la aplicación es `requireAdministrator` y el shell de trabajo no estaba elevado | Se ejecutó un script de PowerShell elevado que cierra la instancia previa, compila en Release, lanza SOK, navega con UI Automation y captura con `PrintWindow`. |

## Commits del sprint

| Commit | Tipo | Contenido |
| --- | --- | --- |
| `850459b` | `feat(domain)` | Entidades y enums de telemetría. |
| `c4aafba` | `feat(application)` | Catálogo, contratos y orquestador. |
| `a2569b8` | `feat(infrastructure)` | Servicio de registro y respaldo por claves. |
| `a574b1b` | `feat(ui)` | Vista, ViewModels, estilos y DI. |
| `4f0332f` | `test(telemetry)` | 19 pruebas de catálogo, gestor, registro y ViewModel. |
| `812476d` | `fix(ui)` | Título duplicado, desplazamiento inicial y versión del pie. |
| `c100b83` | `test(backups)` | 2 pruebas de integración del respaldo por claves. |

**Total:** 31 archivos modificados, 2 703 líneas agregadas y 9 eliminadas.

## Cierre del sprint

- [x] Validar la aplicación en Windows 10 22H2.
- [x] Revisar visualmente la interfaz y corregir el título duplicado y el desplazamiento.
- [x] Actualizar el `PLAN_MAESTRO` a la versión 1.9 con el Sprint 2 completado.

**Estado:** Sprint 2 cerrado el 01/11/2026. Siguiente iteración: Sprint 3 (Gestión de Servicios, HU-08 a HU-10).
