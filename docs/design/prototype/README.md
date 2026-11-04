# Prototipo visual de SOK

Prototipo navegable de la interfaz de **SOK** construido con HTML, CSS y JavaScript sin
dependencias, sin compilación y sin servidor. Su función es fijar la dirección visual antes de
traducirla a WPF y servir como referencia verificable de estados, jerarquía y accesibilidad.

> **Estado:** prototipo de diseño (v1). No ejecuta acciones reales sobre Windows y todos los
> valores de CPU, memoria y respaldos son de ejemplo.

---

## 1. Por qué HTML y no Figma

El archivo de Figma se conserva como referencia histórica, pero la cuenta gratuita limita la
cantidad de páginas y de prototipos interactivos. El prototipo en HTML cubre lo que faltaba:

| Necesidad | Figma (plan gratuito) | Prototipo HTML |
| --- | --- | --- |
| Pantallas ilimitadas | Limitado | Sin límite |
| Estados hover / focus / disabled | Parcial | Reales |
| Tema claro y oscuro | Dos archivos | Un solo archivo con tokens |
| Navegación entre pantallas | Solo en modo presentación | Enlaces reales |
| Verificación de contraste | Manual | Calculable en el navegador |
| Traducción a XAML | Manual | Tabla de equivalencias directa |

---

## 2. Cómo abrirlo

1. Abrir `sok/docs/design/prototype/index.html` con doble clic.
2. El prototipo funciona desde `file://`: no hay `fetch()`, módulos ES ni rutas absolutas.
3. Opcional, para probar en varios dispositivos: `python -m http.server 8765` dentro de esta
   carpeta y abrir `http://127.0.0.1:8765/`.

Navegadores objetivo: Microsoft Edge y Google Chrome actuales (motor Chromium). No se prueba
Firefox ni Safari porque la aplicación final es WPF sobre Windows.

### Atajos

| Atajo | Acción |
| --- | --- |
| `Tab` / `Shift` + `Tab` | Recorrer controles |
| `Enter` / `Espacio` | Activar el control enfocado |
| `Ctrl` + `K` | Enfocar la búsqueda de secciones |
| `?` | Abrir el diálogo de atajos |
| `Esc` | Cerrar diálogo o limpiar la búsqueda |

---

## 3. Estructura de archivos

```
prototype/
├── index.html              Portada con el índice de pantallas
├── screens/
│   ├── dashboard.html      Panel de control
│   ├── tutorial-welcome.html
│   ├── tutorial-step02.html   «Panel de control» (rediseñado)
│   ├── tutorial-final.html
│   ├── profiles.html
│   └── backups.html
├── tokens.css              Variables de color, tipografía, espacio, radios y sombras
├── components.css          Biblioteca de componentes (BEM)
├── layout.css              Reset, shell, rejillas y responsive
├── motion.css              Animaciones y respeto por «reduced motion»
├── shell.js                Sprite de iconos, barra lateral y cabecera
├── app.js                  Tema, diálogos, avisos, búsqueda y atajos
└── README.md               Este documento
```

### Orden de carga

`tokens.css` → `components.css` → `layout.css` → `motion.css` → `shell.js` → `app.js`.

`shell.js` calcula su ruta base a partir de su propio `<script src>`, por lo que los enlaces
funcionan igual en `file://` y en `http://`. Las pantallas solo declaran
`<aside id="sidebar-mount">` y `<header id="header-mount">`; el resto se inyecta.

---

## 4. Temas

- El tema **oscuro es el predeterminado**, igual que en el archivo de Figma.
- El botón de la cabecera alterna claro/oscuro y guarda la elección en `localStorage`
  (`sok-theme`).
- Un script en línea en el `<head>` aplica el tema antes de pintar para evitar el destello
  blanco al cambiar de pantalla.
- `tokens.css` incluye además un bloque `prefers-contrast: more` y
  `forced-colors: active` para alto contraste y modo de contraste forzado de Windows.
- La barra lateral permanece oscura en ambos temas: es la decisión de diseño que mantiene
  continuidad con Figma y con el patrón de navegación de Windows 11.

---

## 5. Decisiones de diseño

| Tema | Decisión | Motivo |
| --- | --- | --- |
| Tipografía | Segoe UI Variable / Segoe UI | Tipografía nativa de Windows 11; elimina Inter del prototipo |
| Iconos | Trazo de 24×24, `stroke-width` 1,75 | Coherente con Fluent System Icons y con WPF `Path` |
| Geometría | Radios 8 / 12 / 16 / 24 px | Escala Fluent; se traduce a `CornerRadius` |
| Elevación | Tres niveles de sombra | Separa superficie, tarjeta y diálogo sin bordes duros |
| Foco | Anillo de 2 px con `outline-offset` | Visible sobre cualquier superficie, requisito WCAG 2.4.11 |
| Movimiento | 120–240 ms, `ease-standard` | Rápido y sobrio; se anula con `prefers-reduced-motion` |
| Densidad | Filas de 52 px; botones de 32, 40 y 48 px | Cómodo para uso con ratón y con teclado |

### Corrección del paso «Panel de control»

El paso 2 del tutorial era el punto más débil del diseño anterior. El rediseño resuelve:

1. **Estados de paso accesibles.** Cada paso usa `aria-current="step"` y el texto
   «(completado)» va en una clase visualmente oculta, no en un color.
2. **Mapa de la pantalla.** Una ilustración numerada asocia cada zona con su descripción; los
   números son el mismo recurso nemotécnico en ambos lugares.
3. **Barra de acciones fija.** «Anterior» a la izquierda y «Siguiente» a la derecha, siempre
   visibles, con una pista de teclado en el extremo opuesto.
4. **Progreso explícito.** «Paso 2 de 3», barra con `role="progressbar"` y su `aria-valuenow`.

---

## 6. Accesibilidad

Lista verificada en el prototipo:

- [x] Contraste mínimo 4,5:1 en texto y 3:1 en componentes, verificado sobre los pares
      semánticos de `docs/design/design-tokens.json`.
- [x] Enlace «Saltar al contenido» como primer elemento enfocable.
- [x] Un solo `<h1>` por pantalla; jerarquía `h1 → h2 → h3` sin saltos.
- [x] Regiones semánticas: `header`, `nav`, `main`, `aside`, `footer`.
- [x] Diálogos con `role="dialog"`, `aria-modal`, foco inicial, trampa de foco y `Esc`.
- [x] Fondo inerte (`inert`) mientras hay un diálogo abierto.
- [x] Avisos en `aria-live="polite"`; los de error usan `role="alert"`.
- [x] Iconos decorativos con `aria-hidden="true"`; los informativos llevan etiqueta textual.
- [x] Estados no dependientes solo del color: se acompañan de texto o icono.
- [x] Objetivos táctiles de al menos 32×32 px (40×40 en acciones principales).
- [x] `prefers-reduced-motion` desactiva animaciones y transiciones.
- [x] Tablas con `<caption>`, `scope="col"` y encabezados reales.
- [x] Formularios con `<label>` asociado y texto de ayuda.

Pendiente de comprobar con lector de pantalla real (Narrator) durante la fase WPF.

---

## 7. Componentes incluidos

| Componente | Clase principal | Uso |
| --- | --- | --- |
| Botón | `.btn` + `.btn--primary/secondary/ghost/danger` | Acciones y navegación |
| Tarjeta | `.card` | Contenedor base con cabecera, cuerpo y pie |
| Métrica | `.metric`, `.metric-strip` | CPU, memoria, disco, procesos |
| Anillo de progreso | `.ring` | Salud del sistema |
| Gráfica de barras | `.chart` | Rendimiento en 30 minutos |
| Stepper | `.stepper` | Progreso del tutorial |
| Barra de acciones | `.action-bar` | Navegación fija del tutorial |
| InfoBar | `.infobar--info/success/warning/danger` | Avisos en contexto |
| Insignia | `.badge--accent/success/warning/danger/neutral` | Estado |
| Diálogo | `.dialog` | Confirmaciones y formularios |
| Aviso flotante | `.toast` | Resultado de una acción |
| Tabla | `.table` | Historial de respaldos |
| Interruptor | `.toggle` | Preferencias booleanas |
| Campo | `.field`, `.input`, `.select`, `.textarea` | Formularios |
| Estado vacío | `.empty-state` | Listas sin contenido |
| Línea de actividad | `.activity` | Historial reciente |
| Tarjeta de perfil | `.profile-card` | Perfiles Gamer / Ofimático |

---

## 8. Traducción a WPF

La correspondencia es directa porque los nombres de token se conservan.

### Colores

| Token CSS | Clave en `Theme.xaml` | Claro | Oscuro |
| --- | --- | --- | --- |
| `--color-bg-canvas` | `BackgroundBrush` | `#F8FAFC` | `#0B1120` |
| `--color-surface-default` | `CardBrush` | `#FFFFFF` | `#111827` |
| `--color-sidebar-bg` | `SidebarBrush` | `#0F172A` | `#020617` |
| `--color-accent-default` | `AccentBrush` | `#2563EB` | `#60A5FA` |
| `--color-text-primary` | `TextPrimaryBrush` | `#0F172A` | `#F8FAFC` |
| `--color-text-secondary` | `TextSecondaryBrush` | `#475569` | `#CBD5E1` |
| `--color-border-subtle` | `BorderBrush` | `#E2E8F0` | `#1E293B` |
| `--color-success-solid` | `SuccessBrush` | `#047857` | `#10B981` |
| `--color-warning-solid` | `WarningBrush` | `#B45309` | `#F59E0B` |
| `--color-danger-solid` | `DangerBrush` (nuevo) | `#BE123C` | `#F43F5E` |

> Los valores actuales de `SuccessColor` (`#16A34A`) y `WarningColor` (`#D97706`) en
> `Theme.xaml` se conservan hasta la fase de traducción; allí se alinean con los tokens y se
> agrega el diccionario de tema oscuro.

### Tipografía y geometría

| CSS | XAML |
| --- | --- |
| `--font-sans` | `FontFamily="Segoe UI Variable Text, Segoe UI"` |
| `--text-display` (32 px) | `Style x:Key="DisplayTitle"` (nuevo) |
| `--text-h1` (28 px) | `Style x:Key="PageTitle"` (hoy 24 px) |
| `--text-h2` (24 px) | `Style x:Key="PageTitle"` con `FontSize="24"` |
| `--text-h4` (16 px) | `Style x:Key="SectionTitle"` |
| `--text-body` (14 px) | `Style x:Key="BodyText"` (hoy 13 px) |
| `--text-caption` (12 px) | `Style x:Key="SmallText"` (hoy 11 px) |
| `--radius-sm/md/lg/xl` | `CornerRadius="6 / 8 / 12 / 16"` |
| `--shadow-e1` | `DropShadowEffect BlurRadius="3" ShadowDepth="1" Opacity="0.10"` |
| `--shadow-e2` | `DropShadowEffect BlurRadius="12" ShadowDepth="2" Opacity="0.14"` |
| `--space-4…--space-48` | `Thickness` y `Margin` equivalentes en DIP |

### Componentes

| Prototipo | Destino WPF |
| --- | --- |
| `.btn--primary` | `PrimaryButton` (existente) |
| `.btn--secondary` | `SecondaryButton` (nuevo) |
| `.btn--ghost` | `GhostButton` (nuevo) |
| `.card` | `CardBorder` (existente) |
| `.nav__item` | `NavListBoxItem` (existente) |
| `.stepper` | `StepperItem` + `ItemsControl` (nuevo) |
| `.infobar` | `InfoBar` propio sobre `Border` (nuevo) |
| `.dialog` | `ContentDialog` propio o `Window` modal (nuevo) |
| `.toast` | `ToastHost` con `DispatcherTimer` (nuevo) |
| `.toggle` | `ToggleSwitch` con plantilla propia (nuevo) |
| `.table` | `DataGrid` con estilos de columna (nuevo) |
| `.ring` | `Path` con `ArcSegment` o `ProgressRing` (nuevo) |
| `.chart` | `Polyline`/`Rectangle` en `Canvas` o `LiveCharts` (evaluar) |

> Los recursos nuevos se agregan a `Theme.xaml` en la fase de traducción, sin modificar los
> existentes, para no romper los estilos actuales de `MainWindow` y `TutorialView`.

---

## 9. Fuera de alcance

- **Telemetría, Servicios y Memoria:** aparecen en la navegación como elementos deshabilitados
  con la etiqueta `WPF`, porque su diseño depende de datos reales que aún no existen.
- **Acciones reales:** ningún botón modifica el sistema; los resultados se simulan con avisos.
- **Persistencia:** solo se guarda el tema. El resto del estado se reinicia al recargar.

---

## 10. Cómo extender el prototipo

1. Agregar la pantalla en `screens/` copiando el encabezado de otra (tema, hojas de estilo,
   `data-page`, `data-title`, `data-section`).
2. Registrar la ruta en el arreglo `NAV` de `shell.js` si debe aparecer en la barra lateral.
3. Reutilizar componentes existentes antes de crear clases nuevas.
4. Si se necesita un token nuevo, agregarlo primero en `docs/design/design-tokens.json`, luego
   en `tokens.css` y por último en `Theme.xaml`.
5. Verificar contraste, foco visible y navegación completa con teclado antes de dar por cerrada
   la pantalla.
