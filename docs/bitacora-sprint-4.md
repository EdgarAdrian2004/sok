# Bitácora — Sprint 4

**Sprint:** Liberación de Memoria (18 SP)  
**Rama:** `feature/sprint-4-memoria`  
**Objetivo:** medir la memoria RAM en tiempo real, liberar el working set de los procesos con un clic y ejecutar la liberación de forma automática cuando el uso supera un umbral configurable, dejando constancia en la bitácora.

## Historias de usuario

| HU | Descripción | SP | Estado |
| --- | --- | --- | --- |
| HU-11 | Medidor en tiempo real de RAM | 5 | ✅ Implementado |
| HU-12 | Liberar memoria con un clic (`EmptyWorkingSet`) | 8 | ✅ Implementado |
| HU-13 | Liberación automática por umbral (70 %–95 %) | 5 | ✅ Implementado |

## Tareas técnicas completadas

- [x] DTOs `MemorySnapshot`, `MemoryReleaseResult` y `MemoryAutoReleaseOptions`.
- [x] Contratos `IMemoryProvider`, `IMemoryReleaseService` e `IMemoryManager`.
- [x] `MemoryManager`: medición, liberación manual, umbral automático, intervalo mínimo y persistencia.
- [x] `NativeMemoryMethods` con `GlobalMemoryStatusEx`, `EnumProcesses`, `OpenProcess`, `EmptyWorkingSet` y `CloseHandle` (P/Invoke).
- [x] `MemoryProvider`: lectura de memoria física con `GlobalMemoryStatusEx`.
- [x] `MemoryReleaseService`: recorrido de procesos, liberación de working sets y estimación de memoria recuperada.
- [x] `MemoryViewModel`: medidor cada tres segundos, comando de liberación, configuración del umbral y guardado en `Settings`.
- [x] `MemoryView`: tarjeta del medidor, barra de progreso, acciones, estado de la última operación y configuración automática.
- [x] Registro en el contenedor de dependencias y navegación al módulo «Memoria RAM».
- [x] Pie de la ventana actualizado a «v0.1 · Sprint 4».
- [x] 31 pruebas nuevas (17 casos del orquestador, 4 de integración y 10 del ViewModel).

## Decisiones de diseño

| Decisión | Justificación |
| --- | --- |
| Medición con `GlobalMemoryStatusEx` | Es la API oficial para el estado global de memoria física y evita depender de WMI, que ya se usa en otros módulos con más latencia. |
| Liberación con `EmptyWorkingSet` sobre todos los procesos accesibles | Es el mecanismo estándar para devolver páginas al sistema. Los procesos protegidos o sin permisos se contabilizan como fallidos y no interrumpen la operación. |
| Sin respaldo previo | La liberación de memoria no modifica configuración persistente del sistema: solo solicita a Windows que vacíe working sets. La regla de oro se mantiene para telemetría, servicios y respaldos. |
| Umbral por defecto del 80 % | Coincide con el valor sembrado por el DDL (`MemoryReleaseThreshold`). El rango configurable se limita a 70 %–95 %. |
| Intervalo mínimo de un minuto entre liberaciones automáticas | Evita que el medidor dispare una liberación en cada actualización de tres segundos mientras la RAM sigue por encima del umbral. |
| Claves `Memory.AutoReleaseEnabled` y `Memory.AutoReleaseThreshold` | Separan la preferencia del usuario de la clave heredada del DDL. Si la clave nueva no existe, se lee `MemoryReleaseThreshold` como valor de partida. |
| Medición cada tres segundos | Es la misma frecuencia del panel de control y evita lecturas innecesarias. |
| `FreedBytesEstimate` como variación de memoria disponible | `EmptyWorkingSet` no devuelve los bytes liberados; se informa la diferencia de memoria física disponible antes y después, que es una estimación verificable. |

## Desviación respecto del plan inicial

El plan preveía `CachedBytes` y `ModifiedBytes` dentro de `MemorySnapshot` a partir de `GlobalMemoryStatusEx`. Esa API no expone memoria en caché ni páginas modificadas, por lo que el DTO final conserva solo los valores que la llamada nativa entrega de forma directa: total, disponible, porcentaje de uso y hora de captura. Incorporar esos campos habría requerido `GetPerformanceInfo` u otra API y no aportaba al criterio de aceptación de la HU-11 (GB usados/libres, porcentaje y barra). El ajuste no cambia el alcance del sprint.

## Verificación

- `dotnet build SOK.sln -c Debug`: 0 errores, 0 advertencias.
- `dotnet build SOK.sln -c Release`: 0 errores, 0 advertencias.
- `dotnet test SOK.sln -c Debug`: 127 pruebas correctas (96 anteriores + 31 nuevas).
- `dotnet test SOK.sln -c Release`: 127 pruebas correctas, 0 fallidas.
- Prueba de integración del medidor: `MemoryProvider` devuelve total mayor que cero, disponible dentro del total, porcentaje entre 0 y 100 y hora de captura reciente.
- Prueba de integración de la liberación: `MemoryReleaseService` recorre los procesos del equipo y devuelve un resultado coherente sin lanzar excepciones.
- Validación funcional con la base SQLite real en un directorio temporal:
  - Memoria total: 17 086 664 704 bytes (15,9 GB); disponible inicial: 11 420 270 592 bytes; uso inicial: 33,0 %.
  - Liberación manual: 115 de 223 procesos correctos, 108 sin acceso y 237,0 MB estimados.
  - Disponible después: 11 661 504 512 bytes; uso después: 31,0 %.
  - Configuración inicial: liberación desactivada y umbral 80 % (leído de `MemoryReleaseThreshold`).
  - Configuración guardada: liberación activada y umbral 70 %; ambas claves nuevas quedaron en `Settings`.
  - Liberación automática simulada con una instantánea al 99 %: se ejecutó, devolvió éxito y registró «Liberación automática» en `Logs`.
- Validación real desde la interfaz:
  - «Liberar ahora» sobre 222 procesos: 207 correctos, 15 sin acceso y 399,1 MB estimados.
  - Entrada en `Logs` con módulo `Memoria`, acción «Liberar memoria», resultado `Exitoso` y detalle «207 de 222 procesos liberados; 15 sin acceso; 399.1 MB estimados.».
  - El medidor mostró 24 % de uso, 4,0 / 15,9 GB en uso y 11,9 GB disponibles.
  - La configuración automática se mostró desactivada con umbral del 80 %.
- Aplicación ejecutada en Windows 10 22H2 (10.0.19045) con elevación UAC.
- Captura del módulo: `docs/capturas/sprint-4-memoria.png`.

## Incidencias y correcciones

| Incidencia | Corrección |
| --- | --- |
| `EntryPointNotFoundException` al invocar `OpenProcess`, `EmptyWorkingSet` y `CloseHandle` | Los envoltorios administrados tenían nombres distintos a los exports nativos. Se agregó `EntryPoint` explícito en los tres `DllImport`; la prueba de integración detectó el fallo antes de la validación visual. |
| El mensaje de estado se sobrescribía en cada tick del medidor | Se conserva el mensaje de éxito en la primera lectura y en las actualizaciones manuales, y se evita sobrescribir el resultado de una liberación automática. |
| La captura visual requiere elevación por UIPI | Se reutilizó el script elevado del Sprint 3: cierra instancias previas, compila en Release, lanza SOK, navega con UI Automation, pulsa «Liberar ahora» y captura con `PrintWindow`. |

## Commits del sprint

| Commit | Tipo | Contenido |
| --- | --- | --- |
| `60b8dc6` | `feat(application)` | DTOs de memoria. |
| `5381b9c` | `feat(application)` | Contratos y orquestador de memoria. |
| `24bebaa` | `feat(infrastructure)` | P/Invoke y proveedores de memoria. |
| `d91ead1` | `feat(ui)` | Vista, ViewModel, DI y navegación. |
| `eeab99e` | `fix(infrastructure)` | Entry points de las llamadas nativas. |
| `e81f179` | `test(memory)` | 31 pruebas de medición, liberación y umbral. |
| `eb58cb5` | `fix(ui)` | Mensaje de estado del medidor. |
| Este commit | `docs` | Plan maestro v1.11, README, bitácora y captura. |

## Cierre del sprint

- [x] Validar la aplicación en Windows 10 22H2.
- [x] Revisar visualmente la interfaz del módulo de memoria.
- [x] Validar una liberación real con registro en `Logs`.
- [x] Validar la liberación automática con una instantánea simulada.
- [x] Actualizar el `PLAN_MAESTRO` a la versión 1.11 con el Sprint 4 completado.

**Estado:** Sprint 4 cerrado el 01/11/2026. Siguiente iteración: Sprint 5 (Perfiles + Restauración + Tutorial, HU-14, HU-15, HU-17 y HU-18).
