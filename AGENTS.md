# AGENTS.md

## Proyecto

SOK (System Optimization Key): aplicación de escritorio Windows (C# .NET 8 + WPF + MVVM) que unifica herramientas de optimización del sistema operativo. Proyecto único de titulación (TAP) de Edgar Adrian Flores Herrera, egresado del IESTP "Trujillo", carrera Diseño y Programación Web.

**El `PLAN_MAESTRO_PROYECTO_SOK.md` es la fuente única de verdad.** Todo cambio de alcance, stack, arquitectura o plan debe reflejarse ahí con nueva versión.

## Contexto crítico

- Proyecto de un solo desarrollador. Todo el código, documentación y diseño lo realiza Edgar con asistencia de agentes de IA.
- El proyecto NO es un repo de código convencional: incluye documentos académicos (TAP), sílabos, encuestas y material de referencia conviviendo con el código.
- El directorio raíz mezcla entregables académicos (`.md`, `.docx`, `.xlsx`) con el código fuente. Los documentos `.md` del TAP son borradores, no documentación técnica para desarrolladores.
- La estructura de código planificada aún no existe (fases B y C pendientes). No crear código sin antes actualizar el PLAN_MAESTRO.

## Restricciones del proyecto

- **Stack fijo (no cambiar sin aprobación explícita):** C# .NET 8 + WPF + MVVM (CommunityToolkit.Mvvm) + SQLite + PowerShell 7 + Win32 P/Invoke. Alternativas ya evaluadas y descartadas: Rust/C++ y Electron/WebView2.
- **Arquitectura de 4 capas:** SOK.Domain (sin dependencias) → SOK.Application → SOK.Infrastructure → SOK.UI. Regla de oro: UI→Infrastructure según el tipo de archivo.
- **Regla de oro de SOK:** cualquier acción que modifique el sistema DEBE generar un respaldo primero.
- **Commits atómicos** con conventional commits: `feat:`, `fix:`, `docs:`, `refactor:`, `test:`, `chore:`.
- **Idioma de documentación académica:** español, tercera persona, tono formal.

## Documentación académica (formato IESTP Trujillo 2025)

Cuando se genere el informe final (.docx):
- Fuente Arial, 11 pt cuerpo, 14 pt títulos de apartado (mayúsculas), 12 pt subtítulos.
- Interlineado 1.5, márgenes 2.54 cm, justificado, sangría al inicio de cada párrafo.
- Numeración romana minúscula hasta Introducción, arábiga desde apartado I.
- Citas y referencias en Norma APA 7ª edición.
- La Introducción se redacta al FINAL del trabajo completo (es un resumen ejecutivo en pasado).

## Validaciones ya cerradas (no volver a cuestionar)

- Diagnóstico: 5-8 sept 2026, 9 docentes, modalidad mixta (presencial + digital), 10 preguntas.
- "Se traba/congela": 7/9 (77,8 %), no 8/9.
- Hardware verificado in situ: Lenovo, Intel Core i7-6500U, 8 GB RAM, SSD; 25 laptops.
- Fuente oficial I.E.: Guía de Colegios Perú (Padrón MINEDU al 5/01/2026) — https://miguiadecolegios.org/colegio/0388082/82081-sagrado-corazon-de-jesus-pedro-galvez-cajamarca
- Padrón de 9 encuestados verificado en sección 3.3 del PLAN_MAESTRO.

## Convenciones de trabajo con agentes de IA

- Los borradores se trabajan en Markdown primero; el paso a `.docx` lo hace Edgar manualmente.
- No proponer cambios al stack ni a la estructura del informe sin aprobación.
- No inventar datos: si algo no está en el PLAN_MAESTRO ni en los archivos del proyecto, preguntar.
- Estructura del informe TAP (orden obligatorio): Portada → I. Área Problemática → II. Justificación → III. Objetivos → IV. Referente Teórico → V. Desarrollo del Trabajo (V.11-V.20) → VI. Resultados → Conclusiones → Recomendaciones → Referencias (APA) → Anexos.
