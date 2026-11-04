# Sistema de diseño visual de SOK

**Archivo de tokens:** [`design-tokens.json`](design-tokens.json)
**Prototipo navegable:** [`prototype/index.html`](prototype/index.html) · [guía](prototype/README.md)

- **Versión:** 1.0
- **Fecha:** 03/11/2026
- **Alcance:** especificación para Figma y posterior traducción a WPF.

---

## 1. Qué contiene este paquete

`design-tokens.json` está en formato compatible con **Tokens Studio for Figma** y con Style Dictionary. Define cuatro sets:

| Set | Contenido | Tokens |
| --- | --- | --- |
| `global` | Escalas de color, tipografía, espaciado, radios, bordes, sombras, iconos y layout. | 179 |
| `light` | Colores semánticos del tema claro. | 72 |
| `dark` | Colores semánticos del tema oscuro. | 72 |
| `components` | Medidas y colores de componentes: botón, tarjeta, badge, input, navegación, stepper, progreso, toggle, tabla, empty state, tooltip y diálogo. | 128 |

Cada tema resuelve **379 tokens**. Las referencias entre tokens usan la sintaxis `{grupo.token}` y ya fueron validadas: no hay alias rotos.

### 1.1 Decisiones visuales

- **Marca:** se conserva el azul `#2563EB` y la barra lateral oscura `#0F172A`, pero se amplía la paleta con escalas completas.
- **Jerarquía:** el texto secundario pasa de `#64748B` a `#475569` para superar WCAG AA con margen (7.24:1 sobre el fondo).
- **Profundidad:** tres niveles de sombra; las tarjetas dejan de ser planas.
- **Espaciado:** escala basada en múltiplos de 4, de 0 a 64.
- **Radios:** 8 px para controles, 12 px para elementos internos y 16 px para tarjetas.
- **Tipografía:** escala de 11 a 40 px con estilos nombrados (`display`, `h1`–`h4`, `body`, `caption`, `overline`, `metric`).
- **Modo oscuro:** paleta completa con contraste verificado. **Su implementación en WPF requiere actualizar el plan maestro a v1.13**, porque el alcance v1.12 solo contempla el tema claro.

---

## 2. Importar en Figma con Tokens Studio

### 2.1 Requisitos

1. Cuenta de Figma (el plan gratuito es suficiente para diseñar).
2. Plugin **Tokens Studio for Figma** instalado desde el menú de plugins de Figma.
3. El archivo `docs/design/design-tokens.json` descargado en tu equipo.

### 2.2 Pasos

1. En Figma, crea un archivo nuevo llamado **SOK — Sistema de diseño**.
2. Ejecuta el plugin: `Plugins → Tokens Studio for Figma`.
3. En el panel del plugin, abre la opción de importar/cargar tokens y selecciona `design-tokens.json`. La etiqueta exacta cambia según la versión (`Import`, `Load from file` o `Tools → Import`).
4. Verifica que aparezcan los sets `global`, `light`, `dark` y `components`.
5. Abre el panel **Themes** y activa **SOK / Light**. Deben quedar habilitados `global`, `light` y `components`; `dark` debe estar deshabilitado.
6. Si tu versión del plugin no reconoce el bloque `$themes`, habilita manualmente los sets `global`, `light` y `components` en el panel de sets; el resultado es equivalente.
7. En el panel de estilos y variables del plugin, aplica los tokens como:
   - **Color styles / variables** para `color.*`.
   - **Text styles** para `typography.*`.
   - **Effect styles** para `shadow.elevation1`, `elevation2` y `elevation3`.
   - **Number/string variables** para `spacing.*`, `radius.*`, `borderWidth.*` y `sizing`.
8. Cuando termines el tema claro, activa **SOK / Dark** y repite la aplicación de estilos. Los nombres son los mismos; cambian los valores.
9. Guarda el archivo y compártelo con permiso de lectura.

> **Nota:** Figma no admite tipografías ni sombras compuestas como variables nativas. Tokens Studio las convierte en *text styles* y *effect styles*. Las opciones exactas de exportación a variables dependen de la versión y del plan del plugin.

### 2.3 Estructura recomendada del archivo

| Página | Contenido |
| --- | --- |
| `00 · Fundamentos` | Paleta, tipografía, espaciado, radios, sombras y estados. |
| `01 · Componentes` | Biblioteca de componentes con variantes y estados. |
| `02 · Pantallas` | Las pantallas prioritarias, en claro y oscuro. |
| `03 · Assets` | Iconos SVG e ilustraciones exportables. |

---

## 3. Convención de nombres en Figma

| Elemento | Convención | Ejemplo |
| --- | --- | --- |
| Pantalla | `screen/<pantalla>/<tema>` | `screen/dashboard/light` |
| Componente | `SOK/<Componente>/<Variante>` | `SOK/Button/Primary` |
| Icono | `icon/<nombre>` | `icon/shield-check` |
| Ilustración | `illustration/<nombre>` | `illustration/tutorial-welcome` |
| Tarjeta métrica | `SOK/MetricCard` | `SOK/MetricCard` |
| Estado de componente | Propiedad `state` | `default`, `hover`, `pressed`, `focus`, `disabled` |

Usa **Auto Layout** en todos los componentes y frames de pantalla. Los valores de padding y gap deben salir de `spacing.*`; los radios, de `radius.*`.

---

## 4. Pantallas prioritarias

Diseña primero en tema claro y luego duplica el frame cambiando el tema a oscuro.

| # | Pantalla | Frame | Contenido mínimo |
| --- | --- | --- | --- |
| 1 | **MainWindow + Sidebar** | 1200 × 760 | Logo, ocho ítems de navegación con icono, indicador activo, versión, header y área de contenido. |
| 2 | **Dashboard** | 1200 × 760 | Métrica principal destacada (CPU o RAM), dos métricas secundarias, información del equipo, tarjeta de respaldo y acciones con jerarquía. |
| 3 | **Tutorial — bienvenida** | 1200 × 760 | Ilustración, texto de bienvenida, stepper lateral y CTA principal. |
| 4 | **Tutorial — paso con módulo** | 1200 × 760 | Paso 02 «Panel de control» con captura o diagrama, contenido Markdown y botonera fija. |
| 5 | **Tutorial — paso final** | 1200 × 760 | Resumen, paso completado y CTA «Finalizar». |
| 6 | **Perfiles** | 1200 × 760 | Tarjetas Gamer y Ofimático, perfil activo, badges, acciones primaria/secundaria y estado vacío de perfiles importados. |
| 7 | **Respaldos** | 1200 × 760 | Lista de respaldos, integridad verificada, acciones de restauración y nota de alcance. |

**No rediseñar en Figma:** Telemetría, Servicios y Memoria. Son vistas con `DataGrid` y lógica densa; se estilizarán directamente en XAML después de estabilizar el tema.

### 4.1 Medidas base

| Elemento | Valor |
| --- | --- |
| Ventana | 1200 × 760; mínimo 960 × 600 |
| Sidebar | 248 px |
| Header | 64 px |
| Padding del contenido | 32 px |
| Separación entre tarjetas | 24 px |
| Padding interno de tarjeta | 24 px |
| Radio de tarjeta | 16 px |
| Altura de botón | 40 px |
| Altura de ítem de navegación | 44 px |

---

## 5. Componentes que debe contener la biblioteca

| Componente | Variantes mínimas |
| --- | --- |
| `SOK/Button` | Primary, Secondary, Ghost, Danger × default, hover, pressed, focus, disabled |
| `SOK/IconButton` | Ghost, Secondary × default, hover, pressed, disabled |
| `SOK/Card` | Default, Interactive, Selected |
| `SOK/MetricCard` | Default, Highlight, Warning |
| `SOK/Badge` | Neutral, Accent, Success, Warning, Danger, Info |
| `SOK/Input` | Default, Focus, Error, Disabled |
| `SOK/Toggle` | Off, On, Disabled |
| `SOK/Progress` | Determinado, Indeterminado |
| `SOK/SidebarItem` | Default, Hover, Active |
| `SOK/StepperItem` | Upcoming, Active, Completed |
| `SOK/EmptyState` | Con acción, sin acción |
| `SOK/Tooltip` | Arriba, abajo, izquierda, derecha |
| `SOK/Dialog` | Confirmación, Información, Peligro |
| `SOK/Table` | Header, Row, Row hover, Row selected |

Cada componente debe documentar sus tokens aplicados en una nota o en el panel de descripción.

---

## 6. Iconografía

- **Estilo:** outline, trazo de 1.5 px, esquinas redondeadas.
- **Tamaño base:** 24 × 24; variantes de 16, 20, 32 y 48.
- **Bibliotecas sugeridas:** Material Symbols Rounded (outline) o Phosphor Icons.
- **Color:** heredado del texto (`currentColor`); no incluir relleno de fondo.

Iconos necesarios:

`dashboard`, `telemetry`, `services`, `memory`, `profiles`, `backups`, `tutorial`, `settings`, `refresh`, `shield-check`, `shield-alert`, `check`, `check-circle`, `alert-triangle`, `x-circle`, `info`, `chevron-right`, `chevron-left`, `chevron-down`, `play`, `pause`, `download`, `upload`, `restore`, `trash`, `search`, `filter`, `monitor`, `cpu`, `memory-stick`, `hard-drive`, `clock`, `user`, `lock`, `external-link`, `sun`, `moon`, `menu`, `close`, `plus`, `more-horizontal`.

Al exportar: selecciona el componente, `Export → SVG`, marca **Outline stroke** y desactiva el fondo. Nombra el archivo igual que el icono (`shield-check.svg`).

---

## 7. Qué enviar de vuelta

Cuando el archivo esté listo, comparte:

1. **File key** de la URL: `https://www.figma.com/design/<FILE_KEY>/...`
2. **Node IDs** de las pantallas y de la página de iconos (`node-id=1234:5678`).
3. Confirmación del tema que debe usarse como referencia principal (claro u oscuro).

Con eso se pueden leer la estructura y los estilos con el MCP de Figma, y descargar los SVG directamente a `src/SOK.UI/Resources/`.

---

## 8. Traducción posterior a WPF

| Token | Clave actual o nueva en `Theme.xaml` |
| --- | --- |
| `color.bg.canvas` | `BackgroundBrush` |
| `color.surface.default` | `CardBrush` |
| `color.text.primary` | `TextPrimaryBrush` |
| `color.text.secondary` | `TextSecondaryBrush` |
| `color.border.subtle` | `BorderBrush` |
| `color.accent.default` | `AccentBrush` |
| `color.accent.hover` | `AccentHoverBrush` |
| `color.sidebar.bg` | `SidebarBrush` |
| `color.sidebar.hover` | `SidebarHoverBrush` |
| `color.success.fg` | `SuccessBrush` |
| `color.warning.fg` | `WarningBrush` |
| `color.danger.fg` | `DangerBrush` (nueva) |
| `typography.h1` | `PageTitle` |
| `typography.h4` | `SectionTitle` |
| `typography.body` | `BodyText` |
| `typography.metricS` | `MetricValue` |
| `typography.caption` | `SmallText` |
| `radius.xl` | `CardBorder.CornerRadius` |
| `radius.md` | Radio de botones e inputs |
| `shadow.elevation1` | `DropShadowEffect` de tarjetas |

La traducción se hará en commits atómicos (`style:` y `refactor(ui):`), sin tocar lógica de negocio y manteniendo las **167 pruebas** en verde.

---

## 9. Reglas de accesibilidad

- No usar `color.text.tertiary` para textos menores a 12 px sobre `color.bg.canvas`; su contraste es 4.55:1, justo en el límite.
- No colocar texto blanco sobre `color.warning.solid` en tema claro: usar `color.warning.onSolid`.
- Los estados no deben comunicarse solo por color; acompañar con icono o texto.
- Todo control interactivo debe tener estado `focus` visible con anillo de 2 px y separación de 2 px.
- Mantener el tamaño mínimo de texto en 11 px y el área táctil mínima en 40 × 40 px.

---

## 10. Fuera de alcance de esta fase

- Cambios de lógica, persistencia o respaldos.
- Rediseño de las tablas de Telemetría, Servicios y Memoria.
- Animaciones complejas o transiciones entre vistas.
- Instalador y validación en campo (Bloque D).

El modo oscuro se diseñará ahora, pero **no se implementará en WPF hasta registrar el cambio de alcance en el plan maestro v1.13**.
