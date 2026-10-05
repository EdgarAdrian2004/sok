# Casos de uso — Sistema SOK

**Sección V.11 (Análisis y diseño) del informe TAP**
Diagramas: `CasosDeUso_SOK.puml` / `.png` (general) **+ 8 sub-diagramas por módulo** — índice en la sección 2
Fuente: Product Backlog (18 HU) — `PLAN_MAESTRO_PROYECTO_SOK.md`
Versión: 1.1 — 04/10/2026

---

## 1. Actores

| Actor | Tipo | Descripción |
| --- | --- | --- |
| **Docente** | Principal | Usuario final de la I.E. N.° 82081. No posee conocimientos técnicos avanzados (44,4 % de los encuestados declaró no tenerlos), por lo que interactúa mediante interfaces guiadas. |
| **Sistema Windows** | Secundario | Proporciona los recursos sobre los que actúa SOK: claves del registro, servicios (SCM), datos de hardware (WMI) y estado de la memoria. |

## 2. Índice de diagramas

El sistema se modela con **un diagrama general**, que conserva el contexto completo del sistema, y **8 sub-diagramas** —uno por módulo— para facilitar su explicación sección por sección en el informe.

| Diagrama | Módulo | Casos de uso | HU cubiertas | Archivos |
| --- | --- | --- | --- | --- |
| **General** | Sistema completo | UC-01 a UC-19 + UC-ABS | 18/18 | `CasosDeUso_SOK.puml` / `.png` |
| 1/8 | Acceso | UC-01, UC-02 | HU-02, HU-03, HU-14 | `CasosDeUso_Acceso.puml` / `.png` |
| 2/8 | Panel de control | UC-03, UC-04, UC-05 | HU-01, HU-03, HU-04, HU-18 | `CasosDeUso_Dashboard.puml` / `.png` |
| 3/8 | Telemetría y privacidad | UC-06, UC-07, UC-08 | HU-05, HU-06, HU-07 | `CasosDeUso_Telemetria.puml` / `.png` |
| 4/8 | Gestión de servicios | UC-09, UC-10, UC-11 | HU-08, HU-09, HU-10 | `CasosDeUso_Servicios.puml` / `.png` |
| 5/8 | Memoria RAM | UC-12, UC-13, UC-14 | HU-11, HU-12, HU-13 | `CasosDeUso_Memoria.puml` / `.png` |
| 6/8 | Respaldos | UC-15, UC-16 | HU-16, HU-17 | `CasosDeUso_Respaldos.puml` / `.png` |
| 7/8 | Perfiles | UC-17, UC-18 | HU-14, HU-15 | `CasosDeUso_Perfiles.puml` / `.png` |
| 8/8 | Casos transversales | UC-19, UC-ABS | HU-04 | `CasosDeUso_Transversales.puml` / `.png` |

**Regla de representación de los casos externos.** Cuando un caso de uso de un módulo se relaciona con uno de otro módulo (por ejemplo, UC-07 → UC-15), el sub-diagrama lo muestra con **fondo gris** y una nota que indica *«pertenece al módulo …»*. Así cada sub-diagrama queda autoexplicativo sin perder el vínculo con el resto del sistema.

| Sub-diagrama | Casos externos mostrados (en gris) | Relación representada |
| --- | --- | --- |
| 3/8 Telemetría | UC-15, UC-ABS | `<<include>>` (respaldo previo) y generalización |
| 4/8 Servicios | UC-15, UC-ABS | `<<include>>` (respaldo previo) y generalización |
| 5/8 Memoria | UC-ABS | Generalización — *sin* respaldo, por excepción justificada |
| 7/8 Perfiles | UC-02 | `<<include>>` desde el módulo Acceso |
| 8/8 Transversales | UC-07, UC-08, UC-10, UC-11, UC-13 | Generalización hacia UC-ABS |

## 3. Trazabilidad HU → Casos de uso

Cobertura verificada: **18/18 historias de usuario**.

| HU | Historia de usuario | Caso(s) de uso |
| --- | --- | --- |
| HU-01 | Dashboard con datos de WMI y medidores | UC-03 |
| HU-02 | Elevación de administrador (app.manifest / UAC) | UC-01 |
| HU-03 | MainWindow, layout y NavigationService (shell) | UC-01, UC-03 *(transversal)* |
| HU-04 | Tabla Logs en SQLite + LogRepository + LogService | UC-19, UC-04 |
| HU-05 | Listado de telemetrías con estado (30+ claves) | UC-06 |
| HU-06 | Interruptor de telemetría individual | UC-07 |
| HU-07 | Botón «Privacidad máxima» | UC-08 |
| HU-08 | Listado de servicios con estado (20+) | UC-09 |
| HU-09 | Detener y deshabilitar un servicio | UC-10 |
| HU-10 | Perfiles de servicios (Gamer / Ofimático) | UC-11 |
| HU-11 | Medidor de RAM en tiempo real | UC-12 |
| HU-12 | Liberar memoria con un clic (EmptyWorkingSet) | UC-13 |
| HU-13 | Liberación automática por umbral (70 %–95 %) | UC-14 |
| HU-14 | Selección de perfil al iniciar | UC-02, UC-17 |
| HU-15 | Exportar / importar perfil (JSON) | UC-18 |
| HU-16 | Crear respaldo (registro / servicios, SHA-256) | UC-15 |
| HU-17 | Restaurar sistema desde un respaldo | UC-16 |
| HU-18 | Tutorial integrado paso a paso | UC-05 |

**Nota sobre HU-03:** la navegación entre módulos no constituye un caso de uso por sí misma (es un medio, no un objetivo del usuario); se modela como soporte transversal del shell de la aplicación, cubierto por UC-01 y utilizado por todos los casos.

**Nota sobre creación de perfiles:** el backlog no contempla un caso de creación manual de perfiles; los perfiles base se siembran en la base de datos y los perfiles nuevos se incorporan mediante importación (HU-15 / UC-18).

## 4. Descripción de los casos de uso

| ID | Caso de uso | Actor | Precondición | Flujo principal | Postcondición |
| --- | --- | --- | --- | --- | --- |
| UC-01 | Iniciar SOK con elevación de administrador | Docente | Windows 10; credenciales de administrador disponibles | 1) Docente ejecuta SOK → 2) Windows solicita confirmación UAC → 3) Docente confirma → 4) SOK abre la base SQLite y carga configuración → 5) incluye UC-02 | Aplicación lista con el panel de control visible |
| UC-02 | Seleccionar perfil de optimización al iniciar | Docente | UC-01 en ejecución; existe al menos un perfil | 1) SOK lee `Profiles` → 2) muestra perfiles disponibles y marca el activo → 3) Docente acepta o cambia → 4) se actualiza `Settings.ActiveProfileId` | Perfil activo establecido para la sesión |
| UC-03 | Ver estado del sistema (CPU, RAM, disco, servicios) | Docente | UC-01 | 1) SOK consulta WMI (`Win32_OperatingSystem`, `Win32_Processor`, memoria, discos) → 2) presenta medidores en el panel → 3) actualiza periódicamente | Indicadores visibles y actualizados |
| UC-04 | Consultar historial de acciones | Docente | Existir al menos un registro en `Logs` | 1) SOK lee `Logs` ordenado por `Timestamp` → 2) Docente filtra por módulo o fecha → 3) examina resultado y detalle | Historial revisado |
| UC-05 | Consultar tutorial integrado | Docente | UC-01 | 1) Docente abre la guía → 2) navega los pasos → 3) cierra la guía | Procedimiento comprendido por el usuario |
| UC-06 | Ver telemetrías y su estado | Docente | UC-01 | 1) SOK lee el estado real de 30+ claves de registro y servicios → 2) lista por categoría con interruptor de estado | Estado de privacidad visible |
| UC-07 | Deshabilitar telemetría individualmente | Docente | UC-06 en pantalla | 1) Docente alterna el interruptor → 2) **incluye UC-15** (respaldo de la clave) → 3) SOK modifica el registro o detiene el servicio → 4) **incluye UC-19** (bitácora) → 5) la interfaz refleja el nuevo estado | Telemetría desactivada y respaldada |
| UC-08 | Aplicar privacidad máxima | Docente | UC-06 | 1) Docente pulsa la acción → 2) **incluye UC-15** (respaldo conjunto) → 3) SOK aplica todas las telemetrías del catálogo → 4) **incluye UC-19** → 5) confirma resultado por elemento | Privacidad máxima aplicada de forma masiva |
| UC-09 | Ver servicios y su estado | Docente | UC-01 | 1) SOK consulta el SCM (`Win32_Service`) → 2) lista 20+ servicios no esenciales con su tipo de inicio | Estado de servicios visible |
| UC-10 | Detener o deshabilitar un servicio | Docente | UC-09 en pantalla | 1) Docente selecciona el servicio → 2) **incluye UC-15** (respaldo del tipo de inicio) → 3) SOK detiene y deshabilita → 4) **incluye UC-19** | Servicio modificado y respaldado |
| UC-11 | Aplicar perfil de servicios (Gamer / Ofimático) | Docente | UC-09 | 1) Docente elige el perfil → 2) **incluye UC-15** → 3) SOK aplica el conjunto de servicios del perfil → 4) **incluye UC-19** | Perfil de servicios aplicado |
| UC-12 | Ver medidor de memoria en tiempo real | Docente | UC-01 | 1) SOK llama `GlobalMemoryStatusEx` → 2) muestra porcentaje y tendencia → 3) actualiza en intervalo configurado | Uso de RAM visible |
| UC-13 | Liberar memoria con un clic | Docente | UC-12 en pantalla; uso de RAM sobre el umbral recomendado | 1) Docente pulsa «Liberar» → 2) SOK enumera procesos (`EnumProcesses`) y vacía su working set (`EmptyWorkingSet`) → 3) **incluye UC-19** (solo bitácora: no altera configuración persistente) → 4) muestra memoria liberada | Memoria RAM liberada |
| UC-14 | Configurar liberación automática por umbral | Docente | UC-01 | 1) Docente define el umbral (70 %–95 %) → 2) SOK persiste el valor en `Settings` → 3) **incluye UC-19** | Umbral guardado; la liberación se dispara por el servicio en segundo plano |
| UC-15 | Crear respaldo | Sistema (automático; también invocable) | Acción de optimización detectada | 1) SOK identifica claves/servicios a modificar → 2) exporta el valor previo (`.reg` o JSON) → 3) calcula hash SHA-256 → 4) registra ruta e integridad en `Backups` | Respaldo reversible disponible |
| UC-16 | Restaurar sistema desde un respaldo | Docente | Existir respaldos registrados en `Backups` | 1) SOK lista respaldos con fecha, tipo e integridad → 2) Docente selecciona uno → 3) SOK verifica el hash → 4) aplica la restauración → 5) **incluye UC-19** | Sistema devuelto al punto respaldado |
| UC-17 | Ver perfiles disponibles y activar uno | Docente | UC-01 | 1) SOK lee `Profiles` → 2) Docente revisa nombre, tipo y descripción → 3) activa el perfil elegido | Perfil activo actualizado |
| UC-18 | Exportar o importar perfil (JSON) | Docente | Para exportar: perfil existente. Para importar: archivo JSON válido | 1) Docente elige exportar o importar → 2) exportar: SOK serializa el perfil a JSON; importar: SOK valida y persiste el archivo → 3) **incluye UC-19** | Perfil compartido o incorporado |
| UC-19 | Registrar acción en la bitácora | Sistema (automático) | Ninguna | 1) SOK construye el registro (módulo, acción, elemento, resultado, detalle, respaldo asociado) → 2) inserta la fila en `Logs` → 3) si falla la escritura, notifica en la interfaz | Trazabilidad de la acción garantizada |
| UC-ABS | *«abstract»* Ejecutar acción de optimización | — | — | Caso de uso abstracto: agrupa las acciones que **modifican configuración persistente** de Windows. Incluye UC-15 (crear respaldo) y UC-19 (bitácora). | — |

## 5. Relaciones del diagrama

| Relación | Casos | Significado |
| --- | --- | --- |
| `<<include>>` | UC-01 → UC-02 | Al iniciar siempre se resuelve el perfil activo |
| `<<include>>` | UC-07, UC-08, UC-10, UC-11 → UC-15 | **Regla de oro:** toda modificación de configuración persistente genera respaldo previo |
| `<<include>>` | UC-ABS → UC-19 | Toda acción de optimización queda registrada |
| Generalización | UC-07, UC-08, UC-10, UC-11, UC-13 → UC-ABS | Reutilizan el comportamiento base del caso abstracto |
| Asociación | Sistema Windows → UC-03, UC-15 | SOK consulta datos (WMI) y extrae claves/servicios a respaldar |

**Justificación de la excepción en UC-13 (liberar memoria):** `EmptyWorkingSet` modifica únicamente el working set en ejecución; no altera claves de registro, servicios ni cualquier otra configuración persistente del sistema. Por ello no requiere respaldo previo y solo se registra en la bitácora. Esta distinción es coherente con la regla de oro del proyecto, formulada para acciones que **modifican el sistema** de forma permanente.

## 6. Criterios de aceptación de esta fase

- [x] 18/18 HU trazadas a casos de uso (sección 3)
- [x] Cada caso de uso con actor, precondición, flujo y postcondición (sección 4)
- [x] Diagrama general generado en PlantUML y exportado a PNG
- [x] 8 sub-diagramas por módulo generados y exportados a PNG
- [x] Casos externos representados con fondo gris y nota de procedencia
- [x] Regla de oro (respaldo previo) modelada explícitamente con `<<include>>`
- [x] Excepción de memoria justificada técnicamente
- [x] Casos redundantes eliminados (navegación y creación manual de perfiles)
