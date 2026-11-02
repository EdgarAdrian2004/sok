# Bitácora — Sprint 5

**Sprint:** Perfiles + Restauración + Tutorial (21 SP)  
**Rama:** `feature/sprint-5-perfiles-restauracion-tutorial`  
**Objetivo:** incorporar perfiles de optimización reutilizables, restaurar el sistema a un punto anterior desde un respaldo íntegro y ofrecer una guía paso a paso para usuarios sin conocimientos técnicos.

## Historias de usuario

| HU | Descripción | SP | Estado |
| --- | --- | --- | --- |
| HU-14 | Selección de perfil al iniciar (Gamer / Ofimático / Personalizado) | 5 | ✅ Implementado |
| HU-15 | Exportar e importar perfil en JSON | 3 | ✅ Implementado |
| HU-17 | Restaurar el sistema a un punto anterior desde un respaldo | 8 | ✅ Implementado |
| HU-18 | Tutorial integrado con guía paso a paso | 5 | ✅ Implementado |

## Tareas técnicas completadas

### Perfiles (HU-14 y HU-15)

- [x] `ProfileCatalog`: genera los ajustes de los perfiles Gamer y Ofimático desde `TelemetryCatalog` y `ServiceCatalog`.
- [x] Contrato `IProfileManager` y orquestador `ProfileManager` con listado, perfil activo, activación, aplicación, exportación e importación.
- [x] `ApplyDisabledAsync` en `ITelemetryManager` e `IServiceManager` para aplicar un conjunto explícito de cambios con un único respaldo por módulo.
- [x] `ProfileExportDto` versionado (`SchemaVersion = 1`) y `ProfileOperationResult` con rutas de respaldo y errores.
- [x] Validación de módulos, acciones, claves de telemetría, nombres de servicio y ajustes de memoria antes de escribir en la base.
- [x] Renombrado automático de perfiles importados: `(importado)`, `(importado 2)`, etc.
- [x] `ProfilesViewModel`, `ProfileItemViewModel` y `ProfilesView` con insignia de perfil activo, confirmación antes de aplicar y acciones de exportación e importación.
- [x] `IFileDialogService` / `FileDialogService` para los diálogos de guardar y abrir archivo.
- [x] `IUserDialogService` / `UserDialogService` para confirmaciones y mensajes de resultado.

### Restauración (HU-17)

- [x] DTOs `BackupStatus`, `RestoreResult` y `RegistryRestoreOutcome`.
- [x] Contratos `IRestoreManager` e `IRegistryRestoreService`.
- [x] `RestoreManager`: lista los respaldos con su estado de integridad, verifica el SHA-256 del manifiesto y rechaza los archivos alterados.
- [x] `RegistryRestoreService`: importa cada archivo `.reg` con `reg.exe` después de comprobar su hash individual.
- [x] Restauración del tipo de inicio de los servicios (`Auto`, `Manual`, `Disabled`) y verificación posterior del cambio.
- [x] Registro en `Logs` con módulo `Respaldos`, acción «Restaurar sistema» y resultado `ActionResult.Revertido`.
- [x] `BackupsViewModel`, `BackupItemViewModel` y `BackupsView` con tipo, fecha, descripción, integridad y botón **Restaurar** habilitado solo para respaldos íntegros.

### Tutorial (HU-18)

- [x] Modelo `TutorialStep` y contrato `ITutorialService`.
- [x] `TutorialService`: carga los pasos incrustados en el ensamblado, lee el encabezado de metadatos (`order`, `title`, `module`, `action`) y los ordena.
- [x] Ocho pasos en Markdown en `docs/tutorial/steps/`: bienvenida, panel de control, telemetría, servicios, memoria, respaldos, perfiles y cierre.
- [x] `MarkdownFlowDocumentBuilder`: convierte encabezados, párrafos, énfasis, enlaces, listas, tablas, citas y bloques de código en un `FlowDocument` con los estilos de SOK.
- [x] `TutorialViewModel`: índice del paso actual, comandos Anterior / Siguiente / Ir al módulo / Saltar / Finalizar y persistencia de `TutorialCompleted`.
- [x] `TutorialView`: índice lateral, contenido en `FlowDocumentScrollViewer` y barra de acciones.
- [x] `INavigationService.NavigationRequested` y `RequestNavigation`; `MainViewModel` actualiza el menú cuando un ViewModel solicita navegar.
- [x] Apertura automática del tutorial en la primera ejecución desde `App.xaml.cs`.
- [x] 40 pruebas nuevas: 13 de perfiles, 11 de restauración y 16 de tutorial y renderizador.

## Decisiones de diseño

| Decisión | Justificación |
| --- | --- |
| Perfiles base generados en código por `ProfileCatalog` | Evita duplicar en el DDL las claves de telemetría y los servicios. Los catálogos siguen siendo la única fuente de verdad y los ajustes se completan en el primer acceso. |
| Separar **Activar** de **Aplicar** | Activar solo persiste `Settings.ActiveProfileId` y no modifica Windows; aplicar ejecuta los ajustes con confirmación y respaldos. Así el usuario puede preparar el perfil sin alterar el equipo. |
| Perfiles personalizados solo por importación JSON | El plan del sprint excluyó el editor manual. Todo perfil importado se registra como `ProfileType.Personalizado` y se renombra si el nombre ya existe. |
| Un respaldo por módulo al aplicar un perfil | `ApplyDisabledAsync` agrupa las claves de telemetría y los servicios en una sola operación por módulo, en lugar de generar un respaldo por elemento. |
| Umbral de memoria 75 % para Gamer y 85 % para Ofimático | Gamer prioriza la recuperación de recursos; Ofimático prioriza la estabilidad y evita liberaciones demasiado frecuentes. Ambos activan la liberación automática. |
| Restauración con verificación SHA-256 previa | Un respaldo alterado o incompleto no debe aplicarse. El manifiesto y cada archivo `.reg` se comprueban antes de ejecutar `reg import`. |
| Restauración limitada al tipo de inicio de los servicios | Devolver el estado de ejecución de cada servicio requeriría detener o iniciar procesos con efectos secundarios; el plan solo exige recuperar la configuración persistente. |
| Tutorial híbrido Markdown + `FlowDocument` | El texto se redacta una sola vez en Markdown, se reutiliza en el informe TAP y se renderiza en WPF con Markdig. Se evita duplicar el contenido en XAML y se conserva el formato enriquecido. |
| Renderizado con el AST de Markdig | Se recorre el árbol de Markdig y se construyen elementos WPF nativos. No se agrega Markdig.Wpf, que está sin mantenimiento reciente. |
| El tutorial se abre solo en la primera ejecución | La clave `TutorialCompleted` se guarda al finalizar o al saltar. Después, la guía queda disponible en el menú lateral sin interrumpir el trabajo. |

## Desviaciones y limitaciones

- El plan estimaba 21 commits atómicos; el trabajo se agrupó en **9 commits de código** por capa e historia, más el commit de documentación. Cada commit compila y deja la solución en verde.
- Los perfiles personalizados no se editan manualmente en esta versión: solo se crean por importación JSON.
- La restauración devuelve las claves de registro y el tipo de inicio de los servicios, pero no el estado de ejecución de los servicios ni la lista de standby de memoria.
- Los cambios de telemetría pueden requerir reiniciar Windows para surtir efecto completo; la interfaz lo advierte en el mensaje de resultado.
- La liberación automática de memoria se evalúa mientras SOK está en ejecución y el módulo de memoria se ha abierto al menos una vez en la sesión.
- La captura visual del sprint requiere elevación UAC. Se obtuvo con la aplicación ejecutada sin elevación mediante el compatibility layer `RunAsInvoker`; las operaciones que modifican el sistema se validaron con las pruebas automatizadas y no se ejecutaron sobre el equipo de desarrollo.

## Verificación

- `dotnet build SOK.sln -c Debug`: 0 errores, 0 advertencias.
- `dotnet build SOK.sln -c Release`: 0 errores, 0 advertencias.
- `dotnet test SOK.sln -c Debug`: **167 pruebas correctas**, 0 fallidas (127 anteriores + 40 nuevas).
- `dotnet test SOK.sln -c Release`: **167 pruebas correctas**, 0 fallidas.
- Distribución de las pruebas nuevas:
  - Perfiles: 8 casos de `ProfileManager` y 5 del ViewModel.
  - Restauración: 5 casos de `RestoreManager` y 6 del ViewModel.
  - Tutorial: 4 casos de `TutorialService`, 7 del ViewModel y 5 del renderizador Markdown.
- Cobertura funcional de los criterios de aceptación:
  - Perfil activo persistido en `Settings.ActiveProfileId` y visible con insignia en la lista.
  - Exportación de un perfil a JSON válido e importación como perfil `Personalizado` con renombrado automático.
  - Rechazo de JSON inválido sin dejar registros parciales.
  - Listado de respaldos con estado de integridad y rechazo de respaldos alterados.
  - Restauración de registro y servicios con bitácora `Revertido`.
  - Carga de los ocho pasos del tutorial, navegación entre pasos, apertura del módulo relacionado y persistencia de `TutorialCompleted`.
  - Renderizado de encabezados, énfasis, enlaces, listas, tablas, citas y código en `FlowDocument`.
- Grafo de Graphify regenerado sobre el commit `8d48a95`: **1503 nodos, 4116 aristas y 74 comunidades**, con 88 % de relaciones extraídas del AST.
- Integración continua: [ejecución 41394997190](https://github.com/EdgarAdrian2004/sok/actions/runs/41394997190) exitosa en 94 segundos (compilación Release y 167 pruebas).
- Validación funcional de la interfaz con UI Automation sobre la compilación Release:
  - Primera ejecución: el tutorial se abrió automáticamente en el paso 1 de 8, con el índice completo de pasos y los botones Anterior, Siguiente y Saltar tutorial.
  - Al pulsar **Saltar tutorial**, la aplicación guardó `TutorialCompleted = "true"`, navegó al Panel de control y mostró el estado del sistema.
  - Segunda ejecución: el tutorial no reapareció y la aplicación abrió directamente el Panel de control.
  - Módulo Perfiles: perfil activo «Ofimático», perfiles Gamer y Ofimático cargados, con acciones Activar, Aplicar, Exportar e Importar perfil JSON.
  - Módulo Respaldos: «2 de 2 respaldos listos para restaurar», ambos de tipo Servicios, con fecha 01/11/2026 y estado «Íntegro (SHA-256 verificado)».
- Captura de la interfaz: `docs/capturas/sprint-5-perfiles-tutorial.png` (tutorial, perfiles y respaldos).

## Incidencias y correcciones

| Incidencia | Corrección |
| --- | --- |
| Ambigüedad `CS0104` entre `Block`, `Table`, `TableCell`, `TableRow` e `Inline` de WPF y de Markdig | Se agregaron alias `MdBlock`, `MdTable`, `MdTableCell`, `MdTableRow`, `MdInline` y `WpfTable` para separar los dos modelos. |
| Markdig informaba tres columnas para una tabla Markdown de dos columnas | `ColumnDefinitions` incluye una definición adicional. El renderizador calcula el número de columnas a partir del máximo de celdas por fila y usa las definiciones solo como respaldo. |
| Las pruebas del renderizador fallaban por afinidad de hilo de WPF | Las aserciones se ejecutan dentro de un hilo STA con `RunInSta` y las excepciones se propagan con `ExceptionDispatchInfo`. |
| Las pruebas no encontraban el texto de la negrita, la cursiva y el enlace | Markdig anida los `Run` y `Hyperlink` dentro de un `Span`; se agregó un recorrido recursivo de inlines. |
| Moq devolvía `null` en métodos que retornan `Task<IReadOnlyList<T>>` sin configuración | `ProfileManager` y `RestoreManager` usan `?? Array.Empty<T>()` para tolerar listas nulas. |
| Los elementos `- [ ]` del paso de cierre se mostraban como texto literal | El paso 08 se redactó con viñetas simples, ya que el renderizador no implementa casillas interactivas. |

## Commits del sprint

| Commit | Tipo | Contenido |
| --- | --- | --- |
| `a89b512` | `feat(app)` | Catálogo de perfiles, contratos, orquestador y aplicación masiva por módulo. |
| `3e092ea` | `feat(ui)` | Módulo de perfiles, diálogos y registro en el contenedor de dependencias. |
| `15ad677` | `test` | 13 pruebas de perfiles y su ViewModel. |
| `8e5f274` | `feat(app)` | Restauración con verificación SHA-256 e importación de registro. |
| `7088d4e` | `feat(ui)` | Módulo de respaldos con integridad y restauración. |
| `aa9a57d` | `test` | 11 pruebas de restauración y su ViewModel. |
| `e16a5a8` | `feat(app)` | Servicio de tutorial y ocho pasos en Markdown. |
| `bab6a5b` | `feat(ui)` | Tutorial con renderizado Markdown, navegación y apertura inicial. |
| `8d48a95` | `test` | 16 pruebas del tutorial y del renderizador. |
| Este commit | `docs` | Plan maestro v1.12, README, bitácora y captura. |

## Cierre del sprint

- [x] Implementar las cuatro historias de usuario del Sprint 5.
- [x] Verificar la solución en Debug y Release sin errores ni advertencias.
- [x] Ejecutar las 167 pruebas en Debug y Release.
- [x] Regenerar el grafo de Graphify y registrar sus métricas.
- [x] Actualizar el `PLAN_MAESTRO` a la versión 1.12 con el Sprint 5 completado.
- [x] Actualizar el README con los módulos, las pruebas y la bitácora.
- [x] Generar la captura de la interfaz del sprint.
- [x] Crear el pull request y obtener la integración continua en verde.

**Estado:** Sprint 5 cerrado el 02/11/2026. El Bloque C queda completo; la siguiente iteración es el Bloque D (validación en campo, instalador y cierre del informe TAP).
