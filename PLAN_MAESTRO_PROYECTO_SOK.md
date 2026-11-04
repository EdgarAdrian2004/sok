# PLAN MAESTRO DEL PROYECTO SOK

## System Optimization Key — Trabajo de Aplicación Profesional

> **Documento de referencia única** para el desarrollo del TAP de Edgar Adrian Flores Herrera.
> Generado el: 29 de septiembre de 2026 — Actualizado: 3 de noviembre de 2026
> Versión: 1.13 (fase de diseño visual: tokens, prototipo navegable y tema oscuro)
> Estado: En desarrollo (Bloque A 5/6; Bloque B 8/8 ✅; Bloque C — Sprints 1 a 5 completados; Bloque C.5 — diseño visual en curso; siguiente: Bloque D)

---

## TABLA DE CONTENIDOS

1. [Resumen Ejecutivo](#1-resumen-ejecutivo)
2. [Contexto Personal](#2-contexto-personal)
3. [Contexto Institucional](#3-contexto-institucional)
4. [Marco Metodológico (IESTP Trujillo 2025)](#4-marco-metodológico-iestp-trujillo-2025)
5. [Análisis del Perfil del Egresado](#5-análisis-del-perfil-del-egresado)
6. [Stack Tecnológico de SOK](#6-stack-tecnológico-de-sok)
7. [Arquitectura del Sistema](#7-arquitectura-del-sistema)
8. [Estructura del Proyecto (Carpetas)](#8-estructura-del-proyecto-carpetas)
9. [Plan de Trabajo por Bloques](#9-plan-de-trabajo-por-bloques)
   - [Bloque A — Fundamentación](#bloque-a--fundamentación)
   - [Bloque B — Diseño](#bloque-b--diseño)
   - [Bloque C — Implementación por Sprints](#bloque-c--implementación-por-sprints)
   - [Bloque C.5 — Diseño Visual y Tema Oscuro](#bloque-c5--diseño-visual-y-tema-oscuro)
   - [Bloque D — Validación y Cierre](#bloque-d--validación-y-cierre)
10. [Mapa de Entregables](#10-mapa-de-entregables)
11. [Brechas de Conocimiento a Cubrir](#11-brechas-de-conocimiento-a-cubrir)
12. [Ruta de Aprendizaje Sugerida](#12-ruta-de-aprendizaje-sugerida)
13. [Próximos Pasos Inmediatos](#13-próximos-pasos-inmediatos)
14. [Glosario Técnico](#14-glosario-técnico)
15. [Notas para Trabajar con Otros Agentes de IA](#15-notas-para-trabajar-con-otros-agentes-de-ia)

---

## 1. RESUMEN EJECUTIVO

**SOK (System Optimization Key)** es un programa de escritorio para Windows que unifica en una sola aplicación funcionalidades actualmente dispersas en cinco herramientas independientes: O&O ShutUp10 (telemetría), Mem Reduct (memoria RAM), ISLC (standby list), SSDFresh (SSD) y WinScript (tweaks generales).

El proyecto surge de una necesidad real detectada en la **I.E. N.° 82081 "Sagrado Corazón de Jesús"** de Pedro Gálvez, Cajamarca, donde 9 docentes encuestados reportan problemas recurrentes de rendimiento en laptops Lenovo con hardware competente (Intel Core i7, 8 GB RAM, SSD), evidenciando que el problema no es de hardware sino de configuración del sistema operativo.

**Características clave del proyecto:**

- **Tipo de deliverable:** Programa de escritorio Windows + informe TAP (Trabajo de Aplicación Profesional)
- **Stack tecnológico:** C# .NET 8 + WPF + MVVM + SQLite + PowerShell + Win32 API (P/Invoke)
- **Metodología:** Scrum con 5 sprints de 2 semanas cada uno
- **Duración estimada:** 12 semanas (3 meses)
- **Alcance MVP:** 3 módulos funcionales (Telemetría, Servicios, Memoria) + núcleo + perfiles + tutorial
- **Autor:** Edgar Adrian Flores Herrera (en solitario)
- **Institución:** IESTP "Trujillo" — Programa de Diseño y Programación Web
- **Asesor:** Por definir

---

## 2. CONTEXTO PERSONAL

### 2.1 Datos del autor

| Campo | Valor |
| --- | --- |
| **Nombre completo** | Edgar Adrian Flores Herrera |
| **Institución de origen** | IESTP "Trujillo" (Perú, La Libertad, Trujillo) |
| **Carrera técnica** | Diseño y Programación Web |
| **Estado académico** | Egresado (sin título técnico aún) |
| **Convalidación actual** | Universidad Privada del Norte (UPN) |
| **Modalidad de titulación** | Trabajo de Aplicación Profesional (TAP) |
| **Rol en el proyecto** | Único desarrollador e investigador |
| **Sistema operativo** | Windows 10 |
| **IDE instalado** | Visual Studio 2022 |
| **Cuenta GitHub** | Sí (conocimiento básico de Git) |

### 2.2 Motivación

Edgar siente que los conocimientos del instituto no fueron del todo sólidos, especialmente en programación de escritorio Windows y bajo nivel. Por ello busca:

1. Construir algo bien hecho, no improvisado
2. Documentar todo correctamente desde el inicio
3. Tener claridad en cada paso antes de avanzar
4. Obtener su título técnico con un proyecto que aplique lo aprendido

### 2.3 Apoyo externo

Edgar está trabajando con una persona que lo ayuda. Por ello necesita un documento completo que permita que otra persona (o un agente de IA diferente) entienda todo el contexto del proyecto sin perder el hilo.

---

## 3. CONTEXTO INSTITUCIONAL

### 3.1 IESTP "Trujillo"

- **Página oficial:** <https://iesptrujillo.edu.pe/index.html>
- **Programa:** Diseño y Programación Web (Plan 2022-2024)
- **Total de sílabos:** 37 (6 ciclos)
- **Total de créditos:** ~109
- **Total de horas:** ~2 838

### 3.2 I.E. Sagrado Corazón de Jesús (lugar del problema real)

| Campo | Valor |
| --- | --- |
| **Nombre** | I.E. N.° 82081 "Sagrado Corazón de Jesús" |
| **Nivel** | Primario |
| **Código modular** | 0388082 |
| **Dirección** | Jr. José Gálvez 530 |
| **Distrito** | Pedro Gálvez |
| **Provincia** | San Marcos |
| **Departamento** | Cajamarca |
| **UGEL** | UGEL San Marcos |
| **Director** | Prof. Hugo Eduardo Abanto Rojas |
| **Turno** | Mañana-Tarde |
| **Alumnos** | 468 |
| **Docentes** | 23 |
| **Secciones** | 18 |
| **Coordenadas** | -7.3357, -78.1698 |
| **Fuente oficial** | Guía de Colegios Perú — <https://miguiadecolegios.org/colegio/0388082/82081-sagrado-corazon-de-jesus-pedro-galvez-cajamarca> (datos del Padrón MINEDU al 5 de enero de 2026; verificada por Edgar el 30/09/2026) |

### 3.3 Diagnóstico aplicado (datos reales)

| Dato | Valor |
| --- | --- |
| **Fechas del diagnóstico** | Del 5 al 8 de septiembre de 2026 |
| **Tipo de instrumento** | Encuesta estructurada de modalidad mixta (presencial y digital) |
| **Preguntas del instrumento** | 10 (cerradas y abiertas) |
| **Docentes encuestados** | 9 (39% de la plana) |
| **Perfiles de los encuestados** | 1°, 3°, 4° (x2), 5° (x2), 6° (x3) |
| **Archivo fuente de respuestas** | `Diagnóstico de Equipamiento Tecnológico - I.E. _Sagrado Corazón de Jesús_ (Respuestas) (1).md` |

**Padrón completo de encuestados (verificado contra el archivo de respuestas):**

| # | Docente | Grado |
| --- | --- | --- |
| 1 | Álvaro Flores Lezama | 1.° |
| 2 | Tania Soledad Tirado Torres | 3.° |
| 3 | Rosario de Pilar Herrera Torres | 4.° |
| 4 | Miriam Olortegui | 4.° |
| 5 | Rita Marín Jiménez | 5.° |
| 6 | Luciano Ordoñez Montoya | 5.° |
| 7 | Ronald Orlando Araujo Pita | 6.° |
| 8 | Nelly Muñoz Medina | 6.° |
| 9 | Duany Medalith Mendoza Tapia | 6.° |

### 3.4 Hardware de las laptops (verificación in situ)

| Componente | Especificación |
| --- | --- |
| **Marca** | Lenovo |
| **Cantidad** | 25 laptops |
| **Procesador** | Intel Core i7-6500U @ 2.50 GHz |
| **Memoria RAM** | 8 GB |
| **Almacenamiento** | 2 unidades ~450 GB c/u (1 HDD + 1 SSD) |

### 3.5 Resultados de la encuesta (porcentajes verificados)

| Problema reportado | Frecuencia "A veces" | Frecuencia "Siempre" | Total |
| --- | --- | --- | --- |
| Se traba, se congela o demora en encender | 6 | 1 | **7/9 (77,8 %)** |
| Se calienta con uso continuo | 8 | 1 | **9/9 (100 %)** |
| No se pueden abrir apps básicas al mismo tiempo | 5 | 1 | **6/9 (66,7 %)** |
| Internet lento o inestable | 4 | 5 | **9/9 (100 %)** |
| Sin conocimiento técnico explícito | — | — | **4/9 (44,4 %)** |

### 3.6 Testimonios literales de docentes (para citar en el informe)

- **Rosario del Pilar Herrera Torres** (4° grado, madre de Edgar): *"Es muy lento"*
- **Rita Marín Jiménez** (5° grado): *"Son cada vez más lentas"*
- **Luciano Ordoñez Montoya** (5° grado): *"Laptos antiguas no soportan programas más actualizados"*
- **Duany Medalith Mendoza Tapia** (6° grado): *"Son lentas y de muy bajo Windows"*
- **Nelly Muñoz Medina** (6° grado): *"Ya son antiguas"*
- **Ronald Orlando Araujo Pita** (6° grado): *"Solamente que el servicio de internet es lento"*

---

## 4. MARCO METODOLÓGICO (IESTP TRUJILLO 2025)

### 4.1 Estructura oficial del informe TAP

Según la "Guía para la Presentación de Informes de Trabajo Aplicación Profesional (Titulación)" del IESTP Trujillo (2025), el informe debe seguir esta estructura:

text

1. Portada

2. Dedicatoria (opcional)

3. Agradecimientos (opcional)

4. Índice

5. Lista de tablas

6. Lista de figuras

7. Introducción

8. I. Área Problemática

9. II. Justificación

10. III. Objetivos

3.1 Objetivo general

3.2 Objetivos específicos

11. IV. Referente Teórico

12. V. Desarrollo del Trabajo

5.1-5.10 Datos de la empresa/institución

5.11 Análisis y diseño

5.12 Denominación del sistema

5.13 Definición del sistema

5.14 Beneficios del sistema

5.15 Restricciones

5.16 Criterios de desempeño

5.17 Diseño de base de datos

5.18 Requerimientos funcionales y no funcionales

5.19 Construcción e implementación (sprints)

5.20+ Interfaces gráficas de cada módulo

13. VI. Resultados

14. Conclusiones

15. Recomendaciones

16. Referencias bibliográficas (Norma APA)

17. Anexos

### 4.2 Formato obligatorio (Guía IESTP 2025)

| Elemento | Especificación |
| --- | --- |
| **Fuente** | Arial |
| **Tamaño cuerpo** | 11 pt |
| **Tamaño títulos apartados** (I, II, III) | 14 pt, negrita, MAYÚSCULAS |
| **Tamaño subtítulos** (3.1, 3.2, etc.) | 12 pt, negrita |
| **Alineación cuerpo** | Justificado |
| **Interlineado** | 1.5 |
| **Sangría** | En cada inicio de párrafo |
| **Márgenes** | 2.54 cm en los 4 lados |
| **Numeración** | Romana minúscula hasta Introducción (i, ii, iii), arábiga desde apartado I (1, 2, 3) |
| **Ubicación número de página** | Esquina inferior derecha |
| **Citas y referencias** | Norma APA (7ª edición) |
| **Tablas** | Formato APA (3 líneas horizontales: superior, debajo del header, inferior) |
| **Figuras** | Numeradas consecutivamente, con título y fuente |

### 4.3 Reglas de contenido (de la guía)

- **Introducción**: Se redacta al final (es un resumen ejecutivo del trabajo completo, incluye conclusiones más importantes). Va en tiempo pasado.
- **Conclusiones**: Una conclusión por cada objetivo específico, extensión mínima de un párrafo c/u. No debe introducir información nueva.
- **Recomendaciones**: Tres tipos — metodológicas, académicas, prácticas.

---

## 5. ANÁLISIS DEL PERFIL DEL EGRESADO

### 5.1 Competencias técnicas que Edgar YA domina (según los 37 sílabos)

#### Programación Web Frontend

- HTML5, CSS3, JavaScript (ES6+)
- Bootstrap, W3.CSS, Flexbox, CSS Grid
- jQuery, Angular, Vue.js
- Preprocesadores: Pug, Sass, LESS, Stylus
- APIs Web: Canvas, WebStorage, IndexedDB, REST
- Accesibilidad: W3C, WCAG, ARIA

#### Programación Web Backend

- PHP (POO, funciones, arreglos)
- ASP.NET Core MVC (C#) — stack más fuerte
- Arquitectura cliente-servidor, REST, JSON

#### Bases de Datos

- MySQL, SQL Server, Oracle, PostgreSQL, SQLite
- Modelado ER, normalización (1FN-3FN)
- SQL: DDL, DML, TCL, vistas, procedimientos almacenados (Transact SQL)

#### Programación General / Algorítmica

- **C# (intermedio-alto)** — stack más sólido
- JavaScript, PHP, Java (Android), Python (básico), C/C++ (IoT básico)

#### Mobile

- Android nativo: Java + Android Studio + SQLite
- Xamarin.Forms: C# + XAML + .NET ⭐ (transferencia directa a WPF)

#### Cloud / DevOps

- Microsoft Azure (VMs, Functions, Cognitive Services)
- Git/GitHub (ramas, merges, conflictos)
- Power BI

#### Otros

- IA: IBM Watson, OpenCV, chatbots
- Videojuegos: Unity + C# (2D/3D)
- IoT: Arduino, Raspberry Pi, sensores, C/C++ + Python
- UML: Rational Rose (casos de uso, clases, secuencia, estados, componentes)
- Metodologías: Scrum, XP, Kanban

### 5.2 Perfil resultante

> **Desarrollador fullstack con énfasis Microsoft (.NET/C#/SQL Server/Azure)** y capacidades multiplataforma (Xamarin), con experiencia móvil y nociones de IA/IoT.

---

## 6. STACK TECNOLÓGICO DE SOK

### 6.1 Decisión del stack

Dado el perfil del egresado, el stack más coherente es **C# .NET 8 + WPF + PowerShell + Win32 API**, porque:

1. ✅ Maximiza lo que Edgar ya sabe (C#, XAML de Xamarin, SQL, Git, Visual Studio)
2. ✅ Único punto de entrada nuevo: Win32 API + PowerShell + Registry/WMI/SCM
3. ✅ Alternativa descartada (Rust/C++ puro): mejor rendimiento pero Edgar no tiene base suficiente
4. ✅ Alternativa descartada (Electron/WebView2): Edgar sabe JS pero la app necesita permisos de administrador y llamadas a Win32

### 6.2 Stack detallado por capa

| Capa | Tecnología | Versión |
| --- | --- | --- |
| **UI / Presentación** | WPF + XAML + CommunityToolkit.Mvvm | .NET 8 |
| **Lógica / Aplicación** | C# / .NET 8 (class library) | .NET 8 |
| **Orquestación** | PowerShell 7 + SDK embebido | 7.x |
| **Bajo nivel** | P/Invoke Win32 API (kernel32, psapi, advapi32) | — |
| **Datos** | SQLite + ADO.NET puro (`Microsoft.Data.Sqlite`) | SQLite 3.x |
| **Sistema** | Registry, WMI, SCM, ETW, Task Scheduler | Win10/11 |
| **Pruebas** | xUnit + Moq | — |
| **Empaquetado** | Inno Setup | — |
| **Versionado** | Git + GitHub | — |

### 6.3 Justificación de cada tecnología

| Tecnología | Por qué se eligió |
| --- | --- |
| **C# / .NET 8** | Stack más fuerte de Edgar (ASP.NET Core MVC, Xamarin, Unity) |
| **WPF + XAML** | Edgar ya domina XAML por Xamarin.Forms; transferencia ~80% |
| **MVVM Toolkit** | Patrón estándar para WPF; separa UI de lógica |
| **SQLite** | Edgar ya lo maneja del Ciclo IV; ideal para apps de escritorio |
| **PowerShell embebido** | Reproduce exactamente el modelo mental de WinScript |
| **P/Invoke Win32** | Único modo de acceder a `EmptyWorkingSet` (Mem Reduct) y otras APIs de bajo nivel |
| **Inno Setup** | Genera instalador .exe con elevación UAC automática |

---

## 7. ARQUITECTURA DEL SISTEMA

### 7.1 Arquitectura por capas (patrón MVVM + capas)

text

┌──────────────────────────────────────────────────────────┐

│ CAPA UI (Presentation) │

│ SOK.UI (WPF + XAML + CommunityToolkit.Mvvm) │

│ ├── Views: MainWindow, DashboardView, ModuleViews │

│ ├── ViewModels: por cada View │

│ └── Controles personalizados (medidores, switches) │

├──────────────────────────────────────────────────────────┤

│ CAPA APLICACIÓN (Use Cases) │

│ SOK.Application │

│ ├── Services: IBackupService, IProfileService │

│ ├── DTOs │

│ └── Interfaces (contratos) │

├──────────────────────────────────────────────────────────┤

│ CAPA DOMINIO (Business Logic) │

│ SOK.Domain │

│ ├── Entities: Profile, ProfileSetting, Backup, Log, Setting │

│ ├── Enums: ProfileType, ProfileModule, ProfileAction, │

│ │ BackupType, LogModule, ActionResult │

│ └── Rules: lógica de negocio pura (sin dependencias) │

├──────────────────────────────────────────────────────────┤

│ CAPA INFRAESTRUCTURA (System Access) │

│ SOK.Infrastructure │

│ ├── Windows: RegistryService, ServiceManager, │

│ │ MemoryOptimizer, SystemInfoProvider │

│ ├── Persistence: SokDbContext + repositorios (ADO.NET puro) │

│ ├── PowerShell: PowerShellRunner │

│ └── Backup: RegistryBackup, ServicesBackup │

└──────────────────────────────────────────────────────────┘

### 7.2 Flujo de una acción de optimización (ejemplo)

text

[Usuario clic "Deshabilitar Telemetría"]

↓

TelemetryViewModel.OptimizeCommand

↓

IOptimizationService.ExecuteAsync(action)

↓

1. IBackupService.CreateBackup(action) ← SIEMPRE primero

↓

2. IRegistryService.DisableTelemetryKeys()

↓

3. IServiceController.StopService("DiagTrack")

↓

4. IPersistenceService.LogAction(action, result)

↓

5. UI actualiza estado con binding

### 7.3 Patrones de diseño a usar

| Patrón | Uso en SOK |
| --- | --- |
| **MVVM** | Separación UI/lógica (CommunityToolkit.Mvvm) |
| **Repository** | Abstracción de SQLite |
| **Factory** | Crear módulos dinámicamente |
| **Singleton** | Para el BackupService |
| **Command** | Acciones deshacer/rehacer |
| **Observer** | Notificaciones entre módulos |

### 7.4 Decisiones técnicas del Sprint 1 (cerradas)

| Tema | Decisión | Justificación |
| --- | --- | --- |
| Persistencia | **ADO.NET puro** con `Microsoft.Data.Sqlite` 8.0.31 | Evita una dependencia adicional y hace explícito el acceso a datos para el TAP |
| Pruebas | **xUnit + Moq** | xUnit es el estándar de .NET y Moq permite aislar servicios sin tocar SQLite ni Windows |
| Inyección de dependencias | **`Ioc.Default`** de CommunityToolkit.Mvvm | Ya incluido en el paquete; suficiente para el tamaño de SOK |
| Navegación | **`NavigationService` propio** con diccionario de vistas | CommunityToolkit no incluye navegación; evita añadir Prism u otra librería |
| Base de datos | `SokDbContext` con el **DDL verificado** de `docs/db/SOK_DDL.sql` | El diseño físico ya fue probado contra SQLite 3.50.4 |
| Ubicación de la BD | `%LOCALAPPDATA%\SOK\sok.db` | `Program Files` requiere elevación para escritura; `LocalAppData` es el lugar correcto |
| Proyecto de pruebas | `src/SOK.UI.Tests/` con subcarpetas `Domain/`, `Application/`, `Infrastructure/` | Un solo proyecto xUnit que referencia las cuatro capas |
| Estrategia de ramas | `main` + `feature/sprint-N-*` por sprint; PR al cierre | Commits atómicos y revisión por sprint |

**Orden de ejecución confirmado:** Sprint 1 → B.4 (clases UML) + B.6 (secuencia UML) → B.8 (CI) → Sprints 2–5 → Bloque D → A.6 (Introducción) + V.12–V.16.

**Nota:** B.4 y B.6 se elaboran después del Sprint 1 para que los diagramas reflejen las clases y dependencias reales, evitando retrabajo.

---

## 8. ESTRUCTURA DEL PROYECTO (CARPETAS)

text

SOK/ ← solución Visual Studio

│

├── SOK.sln ← solución (VS)

├── README.md ← presentación del repo

├── .gitignore

├── docs/ ← documentación del proyecto

│ ├── manual-usuario.md ← manual que verá el usuario final

│ ├── arquitectura.md ← diseño técnico

│ ├── api-interna.md ← contratos entre capas

│ ├── design/ ← sistema de diseño

│ │ ├── design-tokens.json ← 451 tokens (179 globales, 72 por tema y 128 de componentes)

│ │ ├── README.md ← importación a Figma, nomenclatura y reglas de accesibilidad

│ │ └── prototype/ ← prototipo navegable HTML/CSS/JS (6 pantallas)

│ └── bitacora-tareas.md ← Scrum backlog

│

├── src/ ← CÓDIGO FUENTE

│ ├── SOK.Domain/ ← capa dominio (sin dependencias)

│ │ ├── Entities/

│ │ ├── Enums/

│ │ ├── Interfaces/

│ │ └── Services/

│ │

│ ├── SOK.Application/ ← casos de uso

│ │ ├── DTOs/

│ │ ├── Services/

│ │ └── Interfaces/

│ │

│ ├── SOK.Infrastructure/ ← acceso al sistema

│ │ ├── Windows/

│ │ │ ├── RegistryService.cs

│ │ │ ├── ServiceManager.cs

│ │ │ ├── MemoryOptimizer.cs ← P/Invoke kernel32/psapi

│ │ │ ├── SystemInfoProvider.cs ← WMI

│ │ │ └── NativeMethods.cs ← declaraciones P/Invoke

│ │ ├── Persistence/

│ │ │ ├── SQLiteDatabase.cs

│ │ │ └── Repositories/

│ │ ├── Backup/

│ │ │ ├── RegistryBackupService.cs

│ │ │ └── ServicesBackupService.cs

│ │ └── PowerShell/

│ │ └── PowerShellRunner.cs

│ │

│ ├── SOK.UI/ ← WPF (presentación)

│ │ ├── App.xaml

│ │ ├── Views/

│ │ │ ├── MainWindow.xaml

│ │ │ ├── DashboardView.xaml

│ │ │ ├── TelemetryModuleView.xaml

│ │ │ ├── ServicesModuleView.xaml

│ │ │ ├── MemoryModuleView.xaml

│ │ │ ├── ProfileSelectionView.xaml

│ │ │ ├── BackupRestoreView.xaml

│ │ │ └── TutorialView.xaml

│ │ ├── ViewModels/

│ │ ├── Controls/ ← controles custom

│ │ ├── Converters/

│ │ ├── Styles/ ← tema visual

│ │ └── Assets/ ← íconos, imágenes

│ │

│ └── SOK.UI.Tests/ ← pruebas unitarias

│ ├── Domain/

│ ├── Application/

│ └── Infrastructure/

│

├── scripts/ ← scripts auxiliares

│ ├── build.ps1 ← compilar

│ ├── test.ps1 ← correr tests

│ └── package.ps1 ← generar instalador

│

└── installer/ ← empaquetado

└── sok-installer.iss ← Inno Setup script

> **Regla de oro:** Si no sabes dónde va un archivo nuevo, pregúntate:
>
> - ¿Es UI? → `SOK.UI`
> - ¿Es lógica de negocio pura? → `SOK.Domain`
> - ¿Habla con Windows/SQLite? → `SOK.Infrastructure`
> - ¿Es un caso de uso? → `SOK.Application`

---

## 9. PLAN DE TRABAJO POR BLOQUES

### BLOQUE A — FUNDAMENTACIÓN

**Objetivo:** Establecer el marco conceptual, justificación y planificación del proyecto.
**Duración estimada:** 2 semanas
**Salida esperada:** Secciones I, II, III, IV + V.18 + V.19 del informe + bitácora Scrum iniciada.

| Paso | Acción | Documento que alimenta | Estado |
| --- | --- | --- | --- |
| A.1 | Redactar Área Problemática con caso real San Marcos | Sección I | ✅ Hecho |
| A.2 | Redactar Justificación (tecnológica, operativa, social, económica, académica, ambiental) | Sección II | ✅ Hecho |
| A.3 | Redactar Objetivos (general + 5 específicos) | Sección III | ✅ Hecho |
| A.4 | Construir Product Backlog (6 epics + 18 historias + Sprint 1) | Sección V.19 | ✅ Hecho |
| A.5 | Redactar Referente Teórico (Win32, Registry, WMI, MVVM, etc.) | Sección IV | ✅ Hecho (borrador MD) |
| A.6 | Redactar Introducción (al final del Bloque A) | Preliminares | ⏳ Pendiente |

### BLOQUE B — DISEÑO

**Objetivo:** Diseñar el sistema (UML + BD) y crear el repositorio GitHub + solución Visual Studio vacía.
**Duración estimada:** 2 semanas
**Salida esperada:** Secciones V.11-V.18 del informe + repo GitHub creado.

| Paso | Acción | Documento que alimenta | Estado |
| --- | --- | --- | --- |
| B.1 | Crear repo GitHub `EdgarAdrian2004/sok` + estructura de carpetas | — (código) | ✅ Hecho (público) |
| B.2 | Crear solución VS 2022 con 4 capas (Domain, Application, Infrastructure, UI) | — (código) | ✅ Hecho (compila sin errores) |
| B.3 | Diseñar BD SQLite (tablas: Profiles, Backups, Logs, Settings) | Sección V.17 | ✅ Hecho (DDL verificado) |
| B.4 | Elaborar diagrama UML de clases | Sección V.12 | ✅ Hecho (6 diagramas: paquetes + 5 capas) |
| B.5 | Elaborar diagrama UML de casos de uso | Sección V.11 | ✅ Hecho (general + 8 sub-diagramas, 18/18 HU) |
| B.6 | Elaborar diagrama UML de secuencia (flujo optimizar) | Sección V.13 | ✅ Hecho (respaldo completo + actualización del panel) |
| B.7 | Refinar Requerimientos RF y RNF | Sección V.18 | ✅ Ya hecho |
| B.8 | Configurar CI en GitHub Actions (que compile en cada push) | — (código) | ✅ Hecho (run #1 exitoso) |

> **Nota de secuencia:** B.4 y B.6 se elaboran después del Sprint 1, cuando existan clases, repositorios, servicios y ViewModels reales. B.8 se configura después de B.4/B.6.

#### Detalle de B.4 — Diagrama UML de clases (V.12)

El diagrama se dividió en seis archivos para que cada uno sea legible al insertarse en el informe. Un diagrama único de toda la solución resultó de 4096 px de ancho, imposible de leer en una página A4.

| Archivo | Sección | Contenido | Dimensiones |
| --- | --- | --- | --- |
| `DiagramaPaquetes_SOK.puml` | V.12.0 | Paquetes y regla de dependencia entre capas | 1791 × 941 |
| `DiagramaClases_Dominio.puml` | V.12.1 | Entidades, enums y contratos de repositorio | 2052 × 1179 |
| `DiagramaClases_Aplicacion.puml` | V.12.2 | Contratos de servicio, `BackupManager` y `LogService` | 1703 × 1364 |
| `DiagramaClases_Persistencia.puml` | V.12.3 | `SokDbContext` y los cinco repositorios ADO.NET | 1285 × 1273 |
| `DiagramaClases_Windows.puml` | V.12.4 | WMI, respaldo de registro y de servicios | 1437 × 603 |
| `DiagramaClases_Presentacion.puml` | V.12.5 | `App`, navegación, ViewModels y vistas | 1915 × 940 |

Los diagramas se derivaron del grafo inicial de Graphify (438 nodos, 991 aristas) y se contrastaron con los archivos `.csproj`. El análisis completo está en `docs/uml/mapa-conceptual-graphify.md`. Al cierre del Sprint 5 el grafo se regeneró sobre el commit `8d48a95`: **1503 nodos, 4116 aristas y 74 comunidades**, con 88 % de relaciones extraídas del AST.

#### Detalle de B.6 — Diagrama UML de secuencia (V.13)

| Archivo | Sección | Flujo modelado | Trazabilidad |
| --- | --- | --- | --- |
| `Secuencia_RespaldoCompleto.puml` | V.13.1 | Creación del respaldo completo antes de optimizar | HU-16 |
| `Secuencia_ActualizarDashboard.puml` | V.13.2 | Actualización automática (cada 3 s) y manual del panel | HU-01, HU-04 |

Ambos diagramas usan los nombres reales de clases y métodos del Sprint 1. El detalle de bajo nivel del registro y de los servicios se dejó en V.12.4 para no saturar la secuencia; así el diagrama de respaldo conserva 2050 × 1581 px y sigue siendo imprimible.

#### Detalle de B.8 — Integración continua (`.github/workflows/ci.yml`)

| Aspecto | Decisión |
| --- | --- |
| Disparadores | Push a `main`, pull request hacia `main` y ejecución manual (`workflow_dispatch`). |
| Runner | `windows-latest`; es obligatorio porque `SOK.UI` apunta a `net8.0-windows` y las pruebas de integración usan WMI y `reg.exe`. |
| Pasos | Descarga del código → SDK .NET 8 (con caché NuGet) → `dotnet restore` → `dotnet build -c Release` → `dotnet test -c Release` → resumen en el job summary → publicación del TRX como artefacto. |
| Concurrencia | `ci-${{ github.ref }}` con cancelación: un push nuevo cancela la ejecución anterior de la misma rama. |
| Permisos | `contents: read` (mínimo necesario). |
| Retención del artefacto | 14 días. |

**Verificación:** ejecución n.º 1 ([run 41125212927](https://github.com/EdgarAdrian2004/sok/actions/runs/41125212927)) sobre el commit `5d304c4`: exitosa en 129 segundos, con los ocho pasos en verde (restauración, compilación en Release y 34/34 pruebas). Las pruebas de integración se ejecutan sin elevación porque solo leen información del sistema y exportan claves `HKCU` a un directorio temporal.

### BLOQUE C — IMPLEMENTACIÓN POR SPRINTS

**Objetivo:** Construir el programa SOK funcional mediante 5 sprints.
**Duración estimada:** 10 semanas (5 sprints × 2 semanas)
**Salida esperada:** Programa funcional + Sección V.19 completa.

| Sprint | Módulo | Historias | Semanas | Story points | Documento |
| --- | --- | --- | --- | --- | --- |
| **Sprint 1** | Núcleo + Dashboard + Backup | HU-01, 02, 03, 04, 16 | 2 | 23 | V.19 Sprint 1 — ✅ Completado |
| **Sprint 2** | Telemetría y Privacidad | HU-05, 06, 07 | 2 | 16 | V.19 Sprint 2 — ✅ Completado |
| **Sprint 3** | Gestión de Servicios | HU-08, 09, 10 | 2 | 13 | V.19 Sprint 3 — ✅ Completado |
| **Sprint 4** | Liberación de Memoria | HU-11, 12, 13 | 2 | 18 | V.19 Sprint 4 — ✅ Completado |
| **Sprint 5** | Perfiles + Restore + Tutorial | HU-14, 15, 17, 18 | 2 | 21 | V.19 Sprint 5 — ✅ Completado |

**Estado actual:** Sprint 1 completado (rama `feature/sprint-1-nucleo-dashboard-backup`, 2026-10-30), Sprint 2 completado (rama `feature/sprint-2-telemetria-privacidad`, 2026-11-01), Sprint 3 completado (rama `feature/sprint-3-servicios`, 2026-11-01), Sprint 4 completado (rama `feature/sprint-4-memoria`, 2026-11-01) y Sprint 5 completado (rama `feature/sprint-5-perfiles-restauracion-tutorial`, 2026-11-02). El Bloque C queda cerrado; el siguiente paso es el Bloque D (validación en campo, instalador y cierre del informe).

#### Detalle Sprint 1 — Núcleo y Dashboard (23 SP)

| HU | Tareas técnicas | SP |
| --- | --- | --- |
| HU-02 | Configurar app.manifest con requireAdministrator. Probar elevación en Windows 10. Documentar comportamiento. | 2 |
| HU-03 | Crear MainWindow.xaml con layout de 2 columnas. Implementar NavigationService. Crear vistas vacías para cada módulo. Aplicar estilos visuales básicos. | 3 |
| HU-01 | Implementar SystemInfoProvider con WMI. Crear DashboardView con medidores. Implementar data binding con MVVM. Probar actualización en tiempo real. | 5 |
| HU-04 | Crear esquema de tabla Logs en SQLite. Implementar LogRepository. Crear LogService en Application. Integrar registro en acciones del dashboard. | 5 |
| HU-16 | Diseñar esquema de tabla Backups. Implementar RegistryBackupService. Implementar ServicesBackupService. Crear BackupManager orquestador. Generar hash SHA-256 de cada respaldo. | 8 |

**Definición de Hecho (DoD) del Sprint 1:**

- ✅ Código versionado en Git con commits atómicos
- ✅ Pruebas unitarias pasan (xUnit): 34/34 (30 unitarias + 4 de integración)
- ✅ Código revisado
- ✅ Funcionalidad validada en Windows 10 22H2 (10.0.19045)
- ✅ Bitácora del sprint actualizada

**Evidencia de verificación (30/10/2026):**

| Criterio | Resultado |
| --- | --- |
| `dotnet build SOK.sln` | 0 errores, 0 advertencias |
| `dotnet test SOK.sln` | 34 pruebas correctas, 0 fallidas |
| Arranque de la aplicación | Ventana «SOK — System Optimization Key» visible y elevada |
| Panel de control | CPU, RAM, disco y datos del equipo actualizados cada 3 s |
| Base de datos | `%LOCALAPPDATA%\SOK\sok.db` creada con el DDL verificado |
| Respaldos | `%LOCALAPPDATA%\SOK\Backups` con registro, servicios, manifiesto y SHA-256 |
| Captura de pantalla | `docs/capturas/sprint-1-dashboard.png` |

#### Detalle Sprint 2 — Telemetría y Privacidad (16 SP)

| HU | Descripción | SP |
| --- | --- | --- |
| HU-05 | Listado de telemetrías con estado (al menos 30 claves de registry organizadas por categoría) | 5 |
| HU-06 | Toggle para deshabilitar telemetría individualmente | 8 |
| HU-07 | Botón "Privacidad máxima" que aplica todas las telemetrías de una sola vez | 3 |

**Claves de telemetría a cubrir (estudio previo):**

- DiagTrack (Servicio de seguimiento de diagnóstico)
- Cortana y SearchUI
- Windows Defender telemetría
- Advertising ID
- Telemetría de Office (si aplica)
- Configuración de privacidad (ubicación, cámara, micrófono, contactos)
- Telemetría de Edge
- Windows Error Reporting
- Telemetría de OneDrive

**Implementación final (2026-11-01):** el catálogo se amplió a **46 claves en 11 categorías** (Sistema, Cortana, Defender, Publicidad, Privacidad, Edge, Reporte de errores, OneDrive, Sugerencias, Actividad y Comentarios). Las claves se declaran en `SOK.Application/Common/TelemetryCatalog.cs` con su ruta de registro, nombre del valor, tipo (`DWORD` o `String`), valor habilitado, valor deshabilitado y banderas `RequiresAdmin` y `RequiresRestart`. El estado no se almacena: se lee del registro en cada consulta y se clasifica como `Habilitado`, `Deshabilitado`, `NoAplicable` (la clave no existe en el equipo) o `Desconocido`.

**Decisión de diseño (desviación justificada del plan original de 23 SP):** el plan inicial proponía las tablas `TelemetryKeys` y `TelemetryActionLogs`. Se descartaron porque el catálogo es estático y el estado real vive en el registro de Windows: duplicarlo en SQLite introduciría una fuente de verdad paralela que se desincroniza con el sistema. Las acciones se auditan en la tabla `Logs` existente con el módulo `LogModule.Telemetria` (ya previsto en el DDL), y los respaldos en la tabla `Backups`. No se agregaron tablas ni repositorios nuevos.

**Definición de Hecho (DoD) del Sprint 2:**

- ✅ Código versionado en Git con commits atómicos
- ✅ Pruebas unitarias y de integración pasan (xUnit): 55/55 (34 del Sprint 1 + 21 nuevas)
- ✅ Código revisado
- ✅ Funcionalidad validada en Windows 10 22H2 (10.0.19045)
- ✅ Bitácora del sprint actualizada

**Evidencia de verificación (01/11/2026):**

| Criterio | Resultado |
| --- | --- |
| `dotnet build SOK.sln` | 0 errores, 0 advertencias (Debug y Release) |
| `dotnet test SOK.sln` | 55 pruebas correctas, 0 fallidas |
| Catálogo | 46 claves en 11 categorías; identificadores únicos y valores coherentes |
| Estado leído del equipo | «38 de 46 claves deshabilitadas» en la máquina de validación |
| Respaldo previo | Un `.reg` por clave afectada + `registry-manifest.json` con SHA-256; las claves inexistentes se registran como «no requiere respaldo» |
| Privacidad máxima | Un único respaldo para todas las claves pendientes y un registro en la bitácora |
| Fallo de exportación | Aborta la operación antes de escribir (regla de oro) |
| Bitácora | Acciones registradas en `Logs` con `LogModule.Telemetria`, resultado y `BackupId` |
| Captura de pantalla | `docs/capturas/sprint-2-telemetria.png` |

**Limitaciones conocidas:**

- La restauración de un respaldo de telemetría desde la interfaz se incorporó en el Sprint 5 (HU-17); el manifiesto y los `.reg` se verifican con SHA-256 y se importan desde el módulo **Respaldos**.
- Los cambios marcados con «Requiere reinicio» (servicios `DiagTrack` y `dmwappushservice`, Cortana, SmartScreen) surten efecto tras reiniciar el equipo o el Explorador.
- La validación visual requirió un script de PowerShell elevado porque la aplicación se ejecuta con `requireAdministrator` y la interfaz no puede inspeccionarse desde un proceso sin elevar (UIPI).

#### Detalle Sprint 3 — Gestión de Servicios (13 SP)

| HU | Descripción | SP |
| --- | --- | --- |
| HU-08 | Listado de servicios con estado (al menos 20 servicios no esenciales) | 5 |
| HU-09 | Detener y deshabilitar servicios individualmente | 5 |
| HU-10 | Perfiles de servicios (Gamer/Ofimático) | 3 |

**Decisión técnica (01/11/2026):** lectura con **WMI** (`Win32_Service`) por consistencia con `ServicesBackupService`, y escritura con **`ServiceController`** (`Stop`/`Start`) más **`ChangeServiceConfig`** de `advapi32.dll` vía P/Invoke para el tipo de inicio. Se descarta `sc.exe` para evitar el análisis de su salida de consola. El catálogo es **estático en código** (`ServiceCatalog`), igual que el de telemetría: los servicios son componentes conocidos de Windows y no requieren una tabla en SQLite. `IServicesBackupService` se amplía con `CreateBackupForServicesAsync` para respaldar únicamente los servicios afectados.

**Catálogo final (30 servicios, 10 categorías):**

| Servicio | Nombre visible | Categoría | Modo al habilitar | Gamer | Ofimático | Riesgo |
| --- | --- | --- | --- | --- | --- | --- |
| `DiagTrack` | Experiencias de usuario conectado y telemetría | Telemetría | Automático | ✅ | ✅ | Bajo |
| `dmwappushservice` | Enrutamiento de mensajes WAP push | Telemetría | Manual | ✅ | ✅ | Bajo |
| `WerSvc` | Informe de errores de Windows | Telemetría | Manual | ✅ | ✅ | Bajo |
| `Fax` | Fax | Obsoleto | Manual | ✅ | ✅ | Bajo |
| `RpcLocator` | Localizador de llamadas a procedimiento remoto | Obsoleto | Manual | ✅ | ✅ | Bajo |
| `PrintNotify` | Notificaciones de impresión | Obsoleto | Manual | ✅ | ❌ | Bajo |
| `WalletService` | Servicio de cartera | Obsoleto | Manual | ✅ | ✅ | Bajo |
| `PhoneSvc` | Servicio de telefonía | Obsoleto | Manual | ✅ | ✅ | Bajo |
| `SDRSVC` | Copia de seguridad de Windows | Obsoleto | Manual | ✅ | ✅ | Bajo |
| `WMPNetworkSvc` | Uso compartido multimedia de Windows Media Player | Obsoleto | Manual | ✅ | ✅ | Bajo |
| `SysMain` | SysMain (Superfetch) | Rendimiento | Automático | ✅ | ✅ | Medio |
| `TrkWks` | Seguimiento de vínculos distribuidos | Rendimiento | Automático | ✅ | ✅ | Bajo |
| `MapsBroker` | Administrador de mapas descargados | Rendimiento | Automático | ✅ | ✅ | Bajo |
| `WSearch` | Windows Search | Rendimiento | Automático | ❌ | ❌ | **Alto** |
| `lfsvc` | Servicio de geolocalización | Privacidad | Manual | ✅ | ✅ | Bajo |
| `SSDPSRV` | Detección SSDP | Red | Manual | ✅ | ✅ | Bajo |
| `upnphost` | Host de dispositivos UPnP | Red | Manual | ✅ | ✅ | Bajo |
| `lmhosts` | Compatibilidad con NetBIOS sobre TCP/IP | Red | Manual | ✅ | ✅ | Bajo |
| `SharedAccess` | Uso compartido de conexión a Internet | Red | Manual | ❌ | ❌ | **Alto** |
| `XboxGipSvc` | Entrada de juegos de Xbox | Xbox | Manual | ❌ | ✅ | Bajo |
| `XboxNetApiSvc` | Red de Xbox Live | Xbox | Manual | ❌ | ✅ | Bajo |
| `XblAuthManager` | Administrador de autenticación de Xbox Live | Xbox | Manual | ❌ | ✅ | Bajo |
| `XblGameSave` | Guardado de juegos de Xbox Live | Xbox | Manual | ❌ | ✅ | Bajo |
| `bthserv` | Compatibilidad con Bluetooth | Bluetooth | Manual | ❌ | ❌ | **Alto** |
| `WbioSrvc` | Servicio biométrico de Windows | Biometría | Manual | ❌ | ❌ | **Alto** |
| `RemoteRegistry` | Registro remoto | Seguridad | Manual | ❌ | ❌ | Medio |
| `RemoteAccess` | Acceso remoto | Seguridad | Manual | ❌ | ❌ | **Alto** |
| `seclogon` | Inicio de sesión secundario | Seguridad | Manual | ❌ | ❌ | Medio |
| `stisvc` | Adquisición de imágenes (escáneres) | Periféricos | Manual | ❌ | ❌ | Medio |
| `Spooler` | Cola de impresión | Periféricos | Automático | ❌ | ❌ | **Alto** |

**Criterio de los perfiles (conservador en usabilidad):** la columna «Gamer» u «Ofimático» indica que el perfil **deshabilita** el servicio. Los servicios marcados ❌ en ambos perfiles se listan para que el usuario pueda deshabilitarlos manualmente, pero **ningún perfil los toca**. Se mantienen siempre disponibles Bluetooth, Windows Search, Windows Hello, impresión, soporte remoto y las apps nativas de Windows.

- **Base común (ambos perfiles):** `DiagTrack`, `dmwappushservice`, `WerSvc`, `Fax`, `RpcLocator`, `SysMain`, `TrkWks`, `MapsBroker`, `lfsvc`, `WalletService`, `PhoneSvc`, `SDRSVC`, `WMPNetworkSvc`, `SSDPSRV`, `upnphost`, `lmhosts`.
- **Solo Gamer:** `PrintNotify`.
- **Solo Ofimático:** `XboxGipSvc`, `XboxNetApiSvc`, `XblAuthManager`, `XblGameSave`.
- **Nunca en perfiles (solo manual):** `WSearch`, `SharedAccess`, `bthserv`, `WbioSrvc`, `RemoteRegistry`, `RemoteAccess`, `seclogon`, `stisvc`, `Spooler`.

**Definición de Hecho (DoD) del Sprint 3:**

- ✅ Código versionado en Git con commits atómicos
- ✅ Pruebas unitarias y de integración pasan (xUnit)
- ✅ Código revisado
- ✅ Funcionalidad validada en Windows 10 22H2
- ✅ Bitácora del sprint actualizada

**Verificación del Sprint 3:** 0 errores y 0 advertencias en Debug y Release; **96/96 pruebas correctas** (41 nuevas); validación real sobre `PrintNotify` (deshabilitar y volver a habilitar) con dos respaldos JSON y dos entradas en `Logs`; captura en `docs/capturas/sprint-3-servicios.png`; detalle en `docs/bitacora-sprint-3.md`.

#### Detalle Sprint 4 — Liberación de Memoria (18 SP)

| HU | Descripción | SP |
| --- | --- | --- |
| HU-11 | Medidor en tiempo real de RAM | 5 |
| HU-12 | Liberar memoria con un clic (EmptyWorkingSet) | 8 |
| HU-13 | Liberación automática por umbral (70%-95%) | 5 |

**APIs Win32 usadas:**

- `GlobalMemoryStatusEx` (kernel32.dll) — memoria física total y disponible.
- `EnumProcesses` (psapi.dll) — lista de procesos activos.
- `OpenProcess` (kernel32.dll) — handle de proceso con `PROCESS_SET_QUOTA` y `PROCESS_QUERY_INFORMATION`.
- `EmptyWorkingSet` (psapi.dll) — liberación del working set de un proceso.
- `CloseHandle` (kernel32.dll) — cierre de los handles abiertos.

**Implementación final (01/11/2026):** el módulo se organiza en las cuatro capas. En `SOK.Application/DTOs` se declaran `MemorySnapshot` (total, disponible, porcentaje de uso, hora de captura y bytes en uso calculados), `MemoryReleaseResult` (éxito, procesos intentados, correctos, fallidos, estimación de bytes liberados y mensaje, con la fábrica `Failed`) y `MemoryAutoReleaseOptions` (activación, umbral y normalización al rango 70 %–95 %). Los contratos `IMemoryProvider`, `IMemoryReleaseService` e `IMemoryManager` viven en `SOK.Application/Abstractions`; `MemoryManager` orquesta la medición, la liberación manual, la liberación automática, la persistencia del umbral y el registro en `Logs` con el módulo `Memoria`. En `SOK.Infrastructure/Windows`, `NativeMemoryMethods` concentra las firmas P/Invoke con `EntryPoint` explícito, `MemoryProvider` lee la memoria física y `MemoryReleaseService` recorre los procesos, abre cada handle, ejecuta `EmptyWorkingSet` y calcula la diferencia de memoria disponible antes y después. En la UI, `MemoryViewModel` actualiza el medidor cada tres segundos con un `DispatcherTimer`, expone los comandos `RefreshNowCommand` y `ReleaseNowCommand`, muestra el último resultado y guarda el interruptor y el umbral en `Settings`; `MemoryView` presenta la tarjeta del medidor, la barra de uso, las acciones y la configuración automática. El ViewModel se registra como singleton y la vista se resuelve desde `NavigationService` con la clave `Memory`.

**Decisiones de diseño:**

| Decisión | Justificación |
| --- | --- |
| Medición con `GlobalMemoryStatusEx` | Es la API oficial del estado global de memoria física y evita la latencia de WMI, ya usada en el panel de control. |
| Liberación con `EmptyWorkingSet` sobre todos los procesos accesibles | Es el mecanismo estándar para devolver páginas al sistema. Los procesos protegidos o sin permisos se cuentan como fallidos y no interrumpen la operación. |
| Sin respaldo previo | La liberación no modifica configuración persistente: solo solicita a Windows vaciar working sets. La regla de oro se mantiene para telemetría, servicios y respaldos. |
| Umbral por defecto del 80 % | Coincide con el valor sembrado por el DDL (`MemoryReleaseThreshold`) y el rango configurable se limita a 70 %–95 %. |
| Intervalo mínimo de un minuto entre liberaciones automáticas | Evita que el medidor dispare una liberación en cada actualización de tres segundos mientras la RAM sigue por encima del umbral. El reloj se inyecta con `TimeProvider` para poder probarlo. |
| Claves `Memory.AutoReleaseEnabled` y `Memory.AutoReleaseThreshold` | Separan la preferencia del usuario de la clave heredada del DDL. Si la clave nueva no existe, se lee `MemoryReleaseThreshold` como valor de partida. |
| `FreedBytesEstimate` por diferencia de memoria disponible | `EmptyWorkingSet` no devuelve los bytes liberados; la diferencia antes/después es una estimación verificable. |

**Desviación respecto del plan inicial:** el plan preveía `CachedBytes` y `ModifiedBytes` dentro de `MemorySnapshot` a partir de `GlobalMemoryStatusEx`. Esa API no expone memoria en caché ni páginas modificadas, por lo que el DTO final conserva solo los valores que la llamada nativa entrega directamente. Incorporar esos campos habría requerido `GetPerformanceInfo` u otra API y no aportaba al criterio de aceptación de la HU-11 (GB usados/libres, porcentaje y barra). El ajuste no cambia el alcance del sprint.

**Definición de Hecho (DoD) del Sprint 4:**

- ✅ Código versionado en Git con commits atómicos
- ✅ Pruebas unitarias y de integración pasan (xUnit): 127/127 (96 anteriores + 31 nuevas)
- ✅ Código revisado
- ✅ Funcionalidad validada en Windows 10 22H2 (10.0.19045)
- ✅ Bitácora del sprint actualizada

**Evidencia de verificación (01/11/2026):**

| Criterio | Resultado |
| --- | --- |
| `dotnet build SOK.sln` | 0 errores, 0 advertencias (Debug y Release) |
| `dotnet test SOK.sln` | 127/127 pruebas correctas (96 anteriores + 31 nuevas) |
| Medición real | 15,9 GB totales; 33,0 % de uso inicial y 31,0 % después de la liberación |
| Liberación manual (arnés) | 115 de 223 procesos, 108 sin acceso y 237,0 MB estimados |
| Liberación desde la interfaz | 207 de 222 procesos, 15 sin acceso y 399,1 MB estimados |
| Bitácora | Entrada en `Logs` con módulo `Memoria`, acción «Liberar memoria», resultado `Exitoso` y detalle de procesos y MB |
| Umbral automático | Configuración guardada (activado, 70 %) y liberación simulada al 99 % registrada como «Liberación automática» |
| Captura de pantalla | `docs/capturas/sprint-4-memoria.png` |

**Limitaciones conocidas:**

- La liberación es «best effort»: los procesos protegidos, elevados o sin permisos se contabilizan como fallidos y no interrumpen la operación.
- `FreedBytesEstimate` es una estimación por diferencia de memoria disponible; no representa los bytes exactos devueltos por cada proceso.
- No se vacía la lista de standby ni el archivo de paginación: requeriría APIs no documentadas (`NtSetSystemInformation`) y queda fuera del alcance del sprint.
- La liberación automática se evalúa mientras SOK está en ejecución y el módulo de memoria se abrió al menos una vez, porque el temporizador vive en `MemoryViewModel`, registrado como singleton.
- La clave heredada `MemoryReleaseThreshold` solo se usa como valor de partida; las preferencias nuevas se guardan en `Memory.AutoReleaseEnabled` y `Memory.AutoReleaseThreshold`.

#### Detalle Sprint 5 — Perfiles + Restauración + Tutorial (21 SP) — ✅ Completado

| HU | Descripción | SP |
| --- | --- | --- |
| HU-14 | Selección de perfil al iniciar (Gamer/Ofimático/Personalizado) | 5 |
| HU-17 | Restaurar sistema a punto anterior desde backup | 8 |
| HU-18 | Tutorial integrado con guía paso a paso | 5 |
| HU-15 | Exportar/importar perfil JSON | 3 |

**Decisiones de implementación:**

- Los perfiles base **Gamer** y **Ofimático** no se siembran con ajustes en el DDL. `ProfileCatalog` genera sus ajustes en código a partir de `TelemetryCatalog` y `ServiceCatalog`, de modo que el catálogo de claves y servicios siga teniendo una sola fuente de verdad y no se dupliquen datos en la base.
- La interfaz separa **Activar** de **Aplicar**. Activar solo persiste `Settings.ActiveProfileId` y no modifica Windows; aplicar ejecuta los ajustes de telemetría, servicios y memoria con confirmación previa y un respaldo por módulo afectado.
- Los perfiles personalizados solo se incorporan mediante **importación JSON**. Todo perfil importado se registra como `ProfileType.Personalizado`; si el nombre ya existe, se renombra automáticamente con el sufijo `(importado)` o `(importado N)`. No se incluye un editor manual de perfiles en este sprint.
- La **restauración incluye la telemetría**: el manifiesto del respaldo ya contenía la ruta del archivo `.reg`; `RestoreManager` verifica el SHA-256 del manifiesto y de cada `.reg`, ejecuta `reg import` y devuelve los servicios a su tipo de inicio anterior. La bitácora usa `ActionResult.Revertido`.
- El **tutorial** es híbrido: el contenido se redacta en Markdown (`docs/tutorial/steps/*.md`) y se incrusta como recurso del ensamblado; la interfaz lo convierte a `FlowDocument` con Markdig y lo muestra en `FlowDocumentScrollViewer`. Así el texto se reutiliza en el informe TAP y no se duplica en XAML.
- `ITelemetryManager` y `IServiceManager` incorporan `ApplyDisabledAsync`, que aplica un conjunto explícito de claves o servicios con un único respaldo por módulo. `ProfileManager` los usa para aplicar un perfil sin generar un respaldo por cada elemento.

**HU-14 — Selección de perfil.** Se implementan `IProfileManager` y `ProfileManager` con `GetAllProfilesAsync`, `GetActiveProfileAsync`, `SetActiveProfileAsync`, `ApplyProfileAsync` y `ApplyActiveProfileAsync`. Los perfiles base sin ajustes se completan en el primer acceso a partir de los catálogos. La aplicación de un perfil agrupa los ajustes por módulo, delega en `ApplyDisabledAsync` de telemetría y servicios, y persiste la configuración de memoria (`Memory.AutoReleaseEnabled` y `Memory.AutoReleaseThreshold`). El umbral es 75 % para Gamer y 85 % para Ofimático. La UI incorpora `ProfilesViewModel`, `ProfileItemViewModel` y `ProfilesView`, con insignia de perfil activo, confirmación antes de aplicar y rutas de los respaldos visibles.

**HU-15 — Exportar e importar perfiles.** `ProfileExportDto` define un esquema versionado (`SchemaVersion = 1`) con nombre, tipo, descripción y ajustes. La exportación escribe JSON legible; la importación valida la versión, el nombre, los módulos, las acciones, las claves de telemetría, los nombres de servicio y los ajustes de memoria antes de escribir en la base. Los errores se informan con `InvalidDataException` y no dejan registros parciales. La UI agrega los diálogos de guardar y abrir archivo mediante `IFileDialogService`.

**HU-17 — Restauración.** `IRestoreManager` y `RestoreManager` listan los respaldos con su estado de integridad (`BackupStatus`), verifican la existencia del archivo y comparan su SHA-256 con el valor guardado. Si el hash no coincide, la restauración se rechaza sin tocar el sistema. `RegistryRestoreService` importa cada archivo `.reg` del manifiesto con `reg.exe` después de verificar su hash individual. `BackupsViewModel` y `BackupsView` muestran tipo, fecha, descripción e integridad, y solo habilitan **Restaurar** cuando el respaldo es íntegro.

**HU-18 — Tutorial.** `TutorialService` carga los ocho pasos incrustados, lee el encabezado de metadatos (`order`, `title`, `module`, `action`) y los ordena. `TutorialViewModel` controla el índice, los comandos Anterior/Siguiente, la apertura del módulo relacionado y el cierre. `MarkdownFlowDocumentBuilder` convierte encabezados, párrafos, énfasis, enlaces, listas, tablas, citas y bloques de código en un `FlowDocument` con los estilos de SOK. El tutorial se abre automáticamente en la primera ejecución y la clave `TutorialCompleted` evita repetirlo; después queda disponible en el menú lateral.

**Limitaciones documentadas:**

- La restauración devuelve las claves de registro y el tipo de inicio de los servicios, pero no el estado de ejecución de los servicios ni la lista de standby de memoria.
- Los cambios de telemetría pueden requerir reiniciar Windows para surtir efecto completo; la interfaz lo advierte en el mensaje de resultado.
- Los perfiles personalizados no se editan manualmente en esta versión: solo se crean por importación JSON.
- La liberación automática de memoria se evalúa mientras SOK está en ejecución y el módulo de memoria se ha abierto al menos una vez en la sesión.
- El tutorial automático solo aparece la primera vez; las siguientes ejecuciones lo dejan en el menú lateral.

**Evidencia de verificación (02/11/2026):**

| Criterio | Resultado |
| --- | --- |
| `dotnet build SOK.sln -c Debug` | 0 errores, 0 advertencias |
| `dotnet build SOK.sln -c Release` | 0 errores, 0 advertencias |
| `dotnet test SOK.sln` | 167/167 pruebas correctas (40 nuevas) |
| Perfiles | Activación, aplicación, exportación e importación JSON cubiertas con pruebas unitarias |
| Restauración | Verificación de hash, rechazo de respaldos alterados, restauración de registro y servicios, bitácora `Revertido` |
| Tutorial | 8 pasos cargados, navegación, persistencia de `TutorialCompleted` y renderizado Markdown |
| Graphify | 1503 nodos, 4116 aristas, 74 comunidades (88 % extraído) sobre el commit `8d48a95` |

**Commits principales:** `a89b512` (gestor de perfiles), `3e092ea` (UI de perfiles), `15ad677` (pruebas de perfiles), `8e5f274` (restauración), `7088d4e` (UI de respaldos), `aa9a57d` (pruebas de restauración), `e16a5a8` (servicio de tutorial), `bab6a5b` (UI del tutorial) y `8d48a95` (pruebas del tutorial).

### BLOQUE C.5 — DISEÑO VISUAL Y TEMA OSCURO

**Objetivo:** Definir y validar la interfaz antes de traducirla a WPF, e incorporar el tema
oscuro que el alcance v1.12 no contemplaba.
**Duración estimada:** 5 días de prototipo (cerrado) + 4-6 días de traducción a WPF.
**Salida esperada:** sistema de tokens, prototipo navegable, `Theme.xaml` actualizado y las
vistas de la interfaz adaptadas, sin cambios en la lógica de negocio.

| Paso | Acción | Entregable | Estado |
| --- | --- | --- | --- |
| C.5.1 | Auditar la interfaz actual contra el archivo de Figma | Diagnóstico de brechas visuales | ✅ Hecho |
| C.5.2 | Definir tokens semánticos para tema claro y oscuro | `docs/design/design-tokens.json` | ✅ Hecho |
| C.5.3 | Construir el prototipo navegable | `docs/design/prototype/` | ✅ Hecho |
| C.5.4 | Verificar contraste, foco y navegación por teclado | Lista de accesibilidad del prototipo | ✅ Hecho |
| C.5.5 | Traducir tokens y componentes a XAML | `src/SOK.UI/Styles/Theme.xaml` | ⏳ Pendiente |
| C.5.6 | Adaptar `MainWindow` y las vistas al nuevo tema | `src/SOK.UI/Views/` | ⏳ Pendiente |
| C.5.7 | Agregar selector de tema persistente | `Settings.Theme` + preferencia de usuario | ⏳ Pendiente |
| C.5.8 | Verificar compilación, 167 pruebas y capturas | `docs/capturas/` | ⏳ Pendiente |

#### Por qué un prototipo HTML y no Figma

El archivo de Figma (`6vNaxrG0ToILbdADADKp5f`) se conserva como referencia, pero la cuenta
gratuita limita la cantidad de páginas y de prototipos interactivos. El prototipo en HTML, CSS
y JavaScript sin dependencias cubre lo que faltaba: estados reales de hover, foco y deshabilitado,
navegación entre pantallas, tema claro/oscuro con un solo juego de tokens y verificación de
contraste en el propio navegador. Se abre con doble clic desde `index.html`; no requiere
compilación ni servidor.

#### Inventario de tokens

`design-tokens.json` contiene **451 tokens**: 179 globales (paletas, tipografía, espacio, radios,
sombras, movimiento), 72 semánticos por tema (claro y oscuro) y 128 de componentes. Los nombres
se conservan entre CSS y XAML para que la traducción sea una correspondencia directa:

| Grupo | Ejemplo de token | Destino en `Theme.xaml` |
| --- | --- | --- |
| Superficies | `color.bg.canvas`, `color.surface.default` | `BackgroundBrush`, `CardBrush` |
| Texto | `color.text.primary`, `color.text.secondary` | `TextPrimaryBrush`, `TextSecondaryBrush` |
| Bordes | `color.border.subtle` | `BorderBrush` |
| Acento | `color.accent.default`, `color.accent.hover` | `AccentBrush`, `AccentHoverBrush` |
| Estado | `color.success.fg`, `color.warning.fg`, `color.danger.fg` | `SuccessBrush`, `WarningBrush`, `DangerBrush` (nueva) |
| Navegación | `color.sidebar.bg`, `color.sidebar.hover` | `SidebarBrush`, `SidebarHoverBrush` |
| Tipografía | `typography.h1`, `typography.body`, `typography.caption` | `PageTitle`, `BodyText`, `SmallText` |
| Geometría | `radius.xl`, `shadow.elevation1` | `CardBorder.CornerRadius`, `DropShadowEffect` |

#### Decisiones visuales cerradas

- **Tema oscuro por defecto**, igual que el archivo de Figma; el tema claro queda a un clic y la
  elección se persiste.
- **Segoe UI Variable / Segoe UI** reemplaza a Inter: es la tipografía nativa de Windows 11 y
  evita cargar fuentes externas en WPF.
- **Iconografía de trazo 24 × 24** con `stroke-width` 1,75, coherente con Fluent System Icons y
  traducible a `Path` de WPF.
- **Geometría Fluent**: radios de 6, 8, 12, 16 y 24 px; tres niveles de elevación.
- **Foco visible** de 2 px con separación de 2 px, requisito WCAG 2.4.11.
- **La barra lateral permanece oscura en ambos temas**, decisión que mantiene continuidad con
  Figma y con el patrón de navegación de Windows 11.

#### Alcance del prototipo

Seis pantallas: panel de control, tutorial (bienvenida, panel de control y cierre), perfiles y
respaldos. Telemetría, servicios y memoria aparecen deshabilitados en la navegación con la
etiqueta «WPF»: se rediseñarán durante la traducción, cuando se trabaje con los ViewModels y los
datos reales. El prototipo no ejecuta acciones sobre Windows; los resultados se simulan con
avisos y diálogos de confirmación.

**Rediseño del paso «Panel de control».** Era el punto más débil del tutorial anterior. El nuevo
paso usa estados accesibles (`aria-current="step"` y texto «(completado)» oculto visualmente), un
mapa numerado que asocia cada zona de la interfaz con su descripción, progreso explícito
(«Paso 2 de 3» con `role="progressbar"`) y una barra de acciones fija con «Anterior» a la
izquierda y «Siguiente» a la derecha.

#### Restricciones de la traducción a WPF

- No se modifica la lógica de negocio: las **167 pruebas** deben seguir en verde.
- Los recursos existentes de `Theme.xaml` se conservan y se amplían; no se renombran claves en uso.
- La traducción se hace en commits atómicos (`style:` y `refactor(ui):`).
- Cada pantalla traducida se valida con captura en `docs/capturas/` y con `dotnet build` en Debug
  y Release.
- La regla de oro se mantiene intacta: ningún cambio visual toca la ruta de respaldo.

**Commits del prototipo:** `6bfbceb` (sistema de estilos), `15152b8` (navegación e
interacciones), `127e840` (seis pantallas) y `4d15c16` (documentación).

### BLOQUE D — VALIDACIÓN Y CIERRE

**Objetivo:** Validar el programa en campo, generar instalador y cerrar el informe TAP.
**Duración estimada:** 2-3 semanas
**Salida esperada:** Sección VI + Conclusiones + Recomendaciones + Informe final + instalador.

| Paso | Acción | Documento que alimenta | Estado |
| --- | --- | --- | --- |
| D.1 | Instalar SOK en 2-3 laptops de los profesores de San Marcos | — (pruebas) | ⏳ Pendiente |
| D.2 | Medir antes/después (RAM, servicios activos, tiempo de boot) | Sección VI | ⏳ Pendiente |
| D.3 | Entrevistar a profesores (satisfacción, facilidad de uso) | Sección VI | ⏳ Pendiente |
| D.4 | Generar instalador con Inno Setup | — (código) | ⏳ Pendiente |
| D.5 | Redactar Conclusiones (1 por objetivo específico) | Conclusiones | ⏳ Pendiente |
| D.6 | Redactar Recomendaciones (metodológicas, académicas, prácticas) | Recomendaciones | ⏳ Pendiente |
| D.7 | Revisión final con Normas APA + entregable impreso | — (cierre) | ⏳ Pendiente |

---

## 10. MAPA DE ENTREGABLES

### 10.1 Documentos Word generados (para integrar al TAP)

| Archivo | Secciones incluidas | Ubicación | Estado |
| --- | --- | --- | --- |
| `TAP_SOK_Bloque_A.docx` | I. Área Problemática + II. Justificación + III. Objetivos + V.18 Requerimientos | `/home/z/my-project/download/` | ✅ Generado |
| `TAP_SOK_V19_ProductBacklog.docx` | V.19 Construcción (Epics + HU + Backlog + Sprint 1) | `/home/z/my-project/download/` | ✅ Generado |

### 10.2 Recursos de sílabos en Markdown

| Archivo | Contenido | Ubicación |
| --- | --- | --- |
| `silabos_md/` | 37 sílabos en .md organizados por ciclo | `/home/z/my-project/download/` |
| `silabos_md/TODOS_LOS_SILABOS_CONSOLIDADO.md` | Todos los sílabos en un solo archivo | `/home/z/my-project/download/` |
| `silabos_md/README.md` | Índice + guía de uso | `/home/z/my-project/download/` |
| `silabos_md.zip` | ZIP con todo empaquetado | `/home/z/my-project/download/` |

### 10.3 Documentos pendientes de generar

| Documento | Sección del TAP | Bloque |
| --- | --- | --- |
| `TAP_SOK_BloqueA_ReferenteTeorico.docx` (borrador MD listo: `TAP_SOK_BloqueA_ReferenteTeorico.md`) | IV. Referente Teórico | A |
| `TAP_SOK_Introduccion.docx` | Preliminares (Introducción) | A |
| `TAP_SOK_BloqueB_Diseno.docx` | V.11-V.17 (UML + BD) | B |
| `TAP_SOK_Sprint2.docx` | V.19 Sprint 2 | C |
| `TAP_SOK_Sprint3.docx` | V.19 Sprint 3 | C |
| `TAP_SOK_Sprint4.docx` | V.19 Sprint 4 | C |
| `TAP_SOK_Sprint5.docx` | V.19 Sprint 5 | C |
| `TAP_SOK_Resultados.docx` | VI. Resultados | D |
| `TAP_SOK_Conclusiones_Recomendaciones.docx` | Conclusiones + Recomendaciones | D |
| `TAP_SOK_Referencias.docx` | Referencias bibliográficas | D |
| `TAP_SOK_Anexos.docx` | Anexos | D |

### 10.4 Pendientes de código (Bloque B y C)

| Artefacto | Bloque | Estado |
| --- | --- | --- |
| Repositorio GitHub `EdgarAdrian2004/sok` (público) | B | ✅ Hecho |
| Solución VS 2022 con 4 capas — `sok/SOK.sln` | B | ✅ Hecho (compila) |
| Esquema SQLite (DDL) — `docs/db/SOK_DDL.sql` | B | ✅ Hecho (verificado) |
| Diseño de BD (conceptual/lógico/físico) — `docs/db/diseno-base-datos.md` | B | ✅ Hecho |
| Diagrama ER — `docs/db/ER_SOK.png` (+ espejo `.dbml`) | B | ✅ Hecho |
| Diagrama UML casos de uso (general + 8 sub-diagramas) — `docs/uml/` | B | ✅ Hecho (18/18 HU) |
| Descripción de casos de uso — `docs/uml/descripcion-casos-uso.md` | B | ✅ Hecho |
| Diagrama UML de clases (paquetes + 5 capas) — `docs/uml/Diagrama*.puml` | B | ✅ Hecho (V.12) |
| Diagrama UML de secuencia (respaldo + panel) — `docs/uml/Secuencia*.puml` | B | ✅ Hecho (V.13) |
| Mapa conceptual del código (Graphify) — `docs/uml/mapa-conceptual-graphify.md` | B | ✅ Hecho (1503 nodos al cierre del Sprint 5) |
| Configurar CI en GitHub Actions — `.github/workflows/ci.yml` | B | ✅ Hecho (compila y prueba en cada push) |
| Implementación Sprint 1 (núcleo + dashboard + backup) | C | ✅ Hecho |
| Implementación Sprint 2 (telemetría) | C | ✅ Hecho |
| Implementación Sprint 3 (servicios) | C | ✅ Hecho |
| Implementación Sprint 4 (memoria) | C | ✅ Hecho |
| Implementación Sprint 5 (perfiles + restauración + tutorial) | C | ✅ Hecho |
| Instalador Inno Setup | D | ⏳ Pendiente |

---

## 11. BRECHAS DE CONOCIMIENTO A CUBRIR

El plan de estudios de Edgar es 100% web/móvil/cloud. **No hay ni un solo curso** sobre programación de escritorio Windows a bajo nivel. Las brechas críticas son:

| # | Brecha | Por qué es crítica | Dónde se aplica |
| --- | --- | --- | --- |
| 1 | **Win32 API / P/Invoke** (kernel32, psapi, ntdll, advapi32) | Necesaria para ISLC, Mem Reduct | Sprint 4 |
| 2 | **PowerShell scripting y runspaces** | WinScript es 100% PowerShell | Sprint 2 (futuro) |
| 3 | **Windows Management Instrumentation (WMI)** | Consulta de hardware, servicios, procesos | Sprint 1 |
| 4 | **Windows Registry** (HKLM, HKCU, HKCR) | O&O ShutUp10 y SSDFresh modifican docenas de claves | Sprint 2 |
| 5 | **Servicios de Windows** (SCM) | Habilitar/deshabilitar telemetría | Sprint 3 |
| 6 | **Group Policy Objects (GPO)** | O&O ShutUp10 trabaja con políticas | Sprint 2 |
| 7 | **Task Scheduler API** | Programar tareas de optimización automática | Sprint 4 (futuro) |
| 8 | **Memoria RAM a bajo nivel** (standby list, working set, page file) | Núcleo de ISLC y Mem Reduct | Sprint 4 |
| 9 | **Storage / SSD APIs** (TRIM, SMART) | Núcleo de SSDFresh | MVP-2 (futuro) |
| 10 | **Telemetría Windows** (DiagTrack, ETW, Cortana) | Núcleo de O&O ShutUp10 | Sprint 2 |
| 11 | **UAC / Elevación de privilegios** | Toda la app requiere admin | Sprint 1 |
| 12 | **WPF / WinUI 3 / WinForms** (UI de escritorio) | Edgar conoce XAML → falta profundizar | Sprint 1 |
| 13 | **Empaquetado MSI / MSIX / Inno Setup** | Distribución instalable | Bloque D |
| 14 | **Code signing (Authenticode)** | Evitar SmartScreen warnings | Post-TAP |
| 15 | **ETW (Event Tracing for Windows)** | Monitoreo de rendimiento en tiempo real | Sprint 4 |
| 16 | **Sysinternals** (ProcMon, RegMon, RAMMap) | Diagnóstico y referencia | Sprint 1-4 |

---

## 12. RUTA DE APRENDIZAJE SUGERIDA

Para cubrir las brechas, Edgar debe seguir este orden (aproximadamente 6-8 semanas enfocadas):

### Semana 1 — WPF + MVVM (3-4 días)

- Tutorial oficial de Microsoft: "Get started with WPF"
- CommunityToolkit.Mvvm documentation
- Ejercicio: crear app WPF simple con data binding
- **Recurso:** Ya sabes XAML por Xamarin.Forms, esto es transferencia rápida (~80%)

### Semana 2 — Win32 API con P/Invoke en C# (5 días)

- pinvoke.net (referencia de firmas)
- Microsoft Docs: Platform Invoke (P/Invoke)
- Ejercicio: llamar a `EmptyWorkingSet` en un proceso
- **Recurso:** "Pro .NET Memory" by Konrad Kokosa (capítulos sobre Win32)

### Semana 3 — PowerShell 7 + SDK embebido (3-4 días)

- Microsoft Learn: "PowerShell 7 SDK"
- Ejercicio: ejecutar script PS desde C#
- **Recurso:** Mark Russinovich blog (Windows Internals)

### Semana 4 — Windows Registry + WMI (5 días)

- Microsoft Docs: Windows Registry
- Microsoft Docs: Windows Management Instrumentation
- Ejercicio: leer y modificar claves de registry
- Ejercicio: consultar `Win32_DiskDrive`, `Win32_Service`, `Win32_OperatingSystem`

### Semana 5 — Service Control Manager (SCM) + Task Scheduler (3-4 días)

- `System.ServiceProcess` namespace
- Microsoft Docs: Task Scheduler API
- Ejercicio: detener/deshabilitar servicio

### Semana 6 — GPO + ETW (3-4 días)

- Microsoft Docs: Group Policy
- `System.Diagnostics.Tracing`
- Ejercicio: suscribirse a eventos ETW

### Semana 7-8 — Proyecto integrador

- Replicar primero O&O ShutUp10 (más fácil, solo registry) → luego SSDFresh → Mem Reduct → ISLC → WinScript

---

## 13. PRÓXIMOS PASOS INMEDIATOS

### Paso inmediato 1: Referente Teórico (Sección IV) — ✅ COMPLETADO (30/09/2026)

Borrador generado en Markdown: `TAP_SOK_BloqueA_ReferenteTeorico.md` (17 subsecciones, pendiente de revisión, formalización APA y paso a .docx). Cubre:

- Sistema operativo Windows 10/11
- API Win32 y P/Invoke
- Windows Registry
- Windows Management Instrumentation (WMI)
- Service Control Manager (SCM)
- Memoria RAM, working set, standby list
- Telemetría en sistemas operativos
- SSD y TRIM
- C# y .NET 8
- WPF y patrón MVVM
- SQLite
- Metodología Scrum
- UML
- Normas APA

**Extensión estimada:** 10-15 páginas al formato final (borrador actual: 17 subsecciones)

### Paso inmediato 2: Introducción

Después del Referente Teórico, se redacta la Introducción como cierre del Bloque A. Es un resumen ejecutivo del trabajo completo, incluye las conclusiones más importantes.

### Paso inmediato 3: Bloque B — Diseño + Setup técnico

**Hecho:** repo GitHub (B.1), solución Visual Studio (B.2), diseño de BD (B.3), diagramas de clases (B.4), casos de uso (B.5), diagramas de secuencia (B.6) e integración continua (B.8). **Bloque B completo (8/8).**

### Paso inmediato 4: Bloque C — Implementación

**Hecho:** Sprint 1 (núcleo, Dashboard, bitácora y respaldos), Sprint 2 (telemetría y privacidad), Sprint 3 (gestión de servicios), Sprint 4 (liberación de memoria) y Sprint 5 (perfiles, restauración y tutorial) completados y validados en Windows 10 22H2.

**Siguiente:** Bloque C.5 (diseño visual y tema oscuro) y luego Bloque D (validación en campo, instalador y cierre del informe), sobre las mismas reglas de trabajo: respaldo previo, commits atómicos y verificación con `dotnet build` + `dotnet test`.

### Paso inmediato 5: Bloque C.5 — Diseño visual y tema oscuro

**Hecho:** sistema de 451 tokens (`docs/design/design-tokens.json`), prototipo navegable de seis pantallas (`docs/design/prototype/`) y verificación de accesibilidad (contraste WCAG AA, foco visible, navegación por teclado, `prefers-reduced-motion`). El prototipo se abre desde `index.html` sin compilación ni servidor.

**Pendiente:** traducir los tokens y componentes a `Theme.xaml`, adaptar `MainWindow` y las vistas, agregar el selector de tema persistente y verificar las 167 pruebas con capturas nuevas. Estimación: 4-6 días.

### Paso inmediato 6: Bloque D — Validación y cierre

Pruebas en campo (I.E. San Marcos), conclusiones, recomendaciones, informe final.

---

## 14. GLOSARIO TÉCNICO

| Término | Definición |
| --- | --- |
| **API Win32** | Conjunto de funciones nativas de Windows para interactuar con el sistema operativo a bajo nivel. |
| **P/Invoke** | Mecanismo de .NET para llamar a funciones nativas (C/C++) desde código C#. |
| **Windows Registry** | Base de datos jerárquica donde Windows almacena configuraciones del sistema y aplicaciones. |
| **WMI** | Windows Management Instrumentation. Infraestructura para consultar y administrar componentes de Windows. |
| **SCM** | Service Control Manager. Componente de Windows que administra los servicios del sistema. |
| **Working Set** | Conjunto de páginas de memoria RAM que un proceso tiene cargadas físicamente. |
| **Standby List** | Lista de páginas de memoria RAM que fueron usadas pero ya no, y que Windows mantiene cacheadas. |
| **Telemetría** | Recolección de datos de uso del sistema por parte del sistema operativo, enviados a Microsoft. |
| **TRIM** | Comando que informa al SSD qué bloques ya no están en uso, mejorando su rendimiento. |
| **UAC** | User Account Control. Mecanismo de Windows que requiere confirmación para tareas administrativas. |
| **MVVM** | Model-View-ViewModel. Patrón arquitectónico para separar UI de lógica de negocio. |
| **WPF** | Windows Presentation Foundation. Framework de Microsoft para aplicaciones de escritorio Windows. |
| **XAML** | Lenguaje de marcado declarativo para definir interfaces de usuario (WPF, Xamarin, WinUI). |
| **SQLite** | Sistema de gestión de bases de datos relacional embebido, sin servidor. |
| **Scrum** | Metodología ágil para gestión de proyectos de software, basada en sprints iterativos. |
| **Story points** | Unidad de medida relativa para estimar el esfuerzo de una historia de usuario en Scrum. |
| **MoSCoW** | Método de priorización: Must have, Should have, Could have, Won't have. |
| **Definition of Done (DoD)** | Criterios que deben cumplirse para considerar una historia de usuario como terminada. |
| **Product Backlog** | Lista priorizada de todas las historias de usuario del proyecto. |
| **Sprint Backlog** | Subconjunto del product backlog seleccionado para un sprint específico. |
| **Epic** | Historia grande que agrupa varias historias de usuario relacionadas. |
| **Inno Setup** | Herramienta gratuita para crear instaladores de aplicaciones Windows. |
| **Git** | Sistema de control de versiones distribuido. |
| **GitHub** | Plataforma de hosting de repositorios Git con herramientas colaborativas. |

---

## 15. NOTAS PARA TRABAJAR CON OTROS AGENTES DE IA

Si Edgar va a trabajar con otra persona o con otro agente de IA, este documento es la fuente única de verdad. Instrucciones:

### 15.1 Cómo usar este documento con un agente de IA

**Prompt sugerido para iniciar una nueva sesión:**

text

Hola, soy Edgar Adrian Flores Herrera, egresado del IESTP Trujillo (Perú)

en Diseño y Programación Web. Estoy desarrollando mi Trabajo de Aplicación

Profesional (TAP) para obtener el título técnico. El proyecto se llama

SOK (System Optimization Key), un programa de escritorio Windows para

optimizar el sistema operativo.

Adjunto el "PLAN\_MAESTRO\_SOK.md" que contiene TODO el contexto del proyecto:

- Mi perfil como egresado

- El caso real (I.E. N°82081 Sagrado Corazón de Jesús en Cajamarca)

- El stack tecnológico elegido

- La arquitectura del sistema

- El plan de trabajo en bloques A-B-C-D

- Los entregables ya generados

- Las brechas de conocimiento a cubrir

- El glosario técnico

Lee primero el PLAN\_MAESTRO\_SOK.md completo y luego ayúdame con:

[INSERTAR TAREA ESPECÍFICA AQUÍ]

### 15.2 Reglas críticas para cualquier agente que ayude

1. **Respetar el stack tecnológico definido** (C# .NET 8 + WPF + MVVM + SQLite + PowerShell + Win32 P/Invoke). No proponer cambios a menos que Edgar lo solicite explícitamente.
2. **Respetar la estructura del informe** según la guía IESTP Trujillo 2025 (Arial 11, interlineado 1.5, márgenes 2.54 cm, Normas APA, numeración romana hasta Introducción + arábiga desde apartado I).
3. **No inventar datos**. Si un dato no está en este documento o en los entregables, preguntar a Edgar.
4. **Respetar el orden metodológico**: Problema → Objetivos → Diseño → Implementación → Resultados. No empezar a codificar antes de tener el diseño cerrado.
5. **Documentar en paralelo al código**: por cada sprint, actualizar la sección V.19 del informe con capturas, código y resultados.
6. **Usar Git desde el primer commit**: commits atómicos, mensajes descriptivos siguiendo conventional commits (feat:, fix:, docs:, refactor:, test:, chore:).
7. **Generar respaldo antes de cualquier optimización**: esta es la regla de oro del programa SOK. Cualquier módulo que modifique el sistema DEBE generar un respaldo primero.
8. **Tono del informe**: académico formal, en tercera persona, en pasado (porque describe un trabajo ya realizado). Vocabulario técnico preciso pero accesible.

### 15.3 Validaciones realizadas y pendientes

**Confirmadas por Edgar (30/09/2026):**

- ✅ **Fecha del diagnóstico:** del 5 al 8 de septiembre de 2026 (según marcas de tiempo del archivo de respuestas).
- ✅ **Los 9 docentes encuestados:** padrón verificado contra el archivo de respuestas (ver sección 3.3).
- ✅ **Composición de la muestra (1°, 3°, 4°x2, 5°x2, 6°x3):** correcta.
- ✅ **Hardware (i7-6500U, 8 GB RAM, SSD):** verificado personalmente por Edgar en la I.E.
- ✅ **Cantidad de laptops:** 25 unidades confirmadas por Edgar.
- ✅ **Porcentaje "se traba/congela":** corregido a 7/9 (77,8 %) según respuestas reales.
- ✅ **Fuente oficial de datos de la I.E.:** Guía de Colegios Perú — <https://miguiadecolegios.org/colegio/0388082/82081-sagrado-corazon-de-jesus-pedro-galvez-cajamarca> (datos del Padrón MINEDU al 5/01/2026; verificada el 30/09/2026). Los datos de la página coinciden con el informe: 468 alumnos, 18 secciones, 23 docentes, director Abanto Rojas Hugo Eduardo, código 0388082.
- ✅ **Modalidad de aplicación de la encuesta:** mixta (presencial y digital), confirmada por Edgar.
- ✅ **Número de preguntas del instrumento:** 10 campos de pregunta (son todas las preguntas del formulario; la marca de tiempo no cuenta como pregunta).

**Pendiente:**

- ⏳ **Formalización en Norma APA (7.ª edición):** cuando el informe esté completo, pasar todas las citas y referencias al formato APA definitivo (incluida la cita de la fuente oficial de la I.E.).

### 15.4 Recursos externos recomendados

| Recurso | Para qué sirve |
| --- | --- |
| [Microsoft Docs - WPF](https://learn.microsoft.com/en-us/dotnet/desktop/wpf/) | Documentación oficial WPF |
| [pinvoke.net](https://www.pinvoke.net/) | Referencia de firmas P/Invoke para Win32 |
| [Microsoft Docs - WMI](https://learn.microsoft.com/en-us/windows/win32/wmisdk/wmi-start-page) | Documentación WMI |
| [Microsoft Docs - Windows Registry](https://learn.microsoft.com/en-us/windows/win32/sysinfo/registry) | Documentación Registry |
| [Scrum Guides](https://scrumguides.org/scrum-guide.html) | Guía oficial de Scrum |
| [APA Style](https://apastyle.apa.org/) | Normas APA oficiales |
| [O&O ShutUp10](https://www.oo-software.com/en/shutup10) | Herramienta de referencia |
| [Mem Reduct](https://github.com/henrypp/memreduct) | Open source, sirve de inspiración |
| [Inno Setup](https://jrsoftware.org/isinfo.php) | Generador de instaladores |

---

## HISTORIAL DE VERSIONES

| Versión | Fecha | Cambios |
| --- | --- | --- |
| 1.0 | 29/09/2026 | Versión inicial. Bloque A completado (excepto Referente Teórico e Introducción). |
| 1.1 | 30/09/2026 | Corrección de datos según archivo de respuestas real: fechas de diagnóstico (5-8 sep 2026), frecuencia "se traba" corregida a 7/9 (77,8 %), padrón completo de los 9 docentes, testimonios ampliados, encuesta (modalidad mixta, 10 preguntas) y fuente oficial de la I.E. confirmadas, validaciones de la sección 15.3 actualizadas. **Nuevo entregable:** borrador del Referente Teórico (Sección IV) en Markdown. |
| 1.2 | 04/10/2026 | **Bloque B (diseño):** diagrama de casos de uso en PlantUML (19 casos concretos + 1 abstracto, trazabilidad 18/18 HU) con descripciones; diseño de base de datos conceptual, lógico y físico (5 tablas, 5 índices, 3FN) con DDL verificado contra SQLite 3.50.4; diagrama entidad-relación (PNG + DBML para dbdiagram.io). Nueva carpeta `docs/` con `uml/` y `db/`. Pasos B.3 y B.5 completados. |
| 1.3 | 04/10/2026 | **Sub-diagramas de casos de uso:** el diagrama unificado se complementa con 8 sub-diagramas por módulo (Acceso, Panel de control, Telemetría, Servicios, Memoria, Respaldos, Perfiles y Casos transversales), conservando sin cambios el diagrama general. Se aplica la estrategia de **Opción A** para relaciones entre módulos: los casos externos se representan con fondo gris y una nota que indica el módulo de procedencia. Cada sub-diagrama cubre sus HU y se genera en PNG. `docs/uml/descripcion-casos-uso.md` se actualiza con el índice de los 9 diagramas y la regla de representación (v1.1 del documento). |
| 1.4 | 05/10/2026 | **Bloque B — infraestructura (B.1 + B.2).** Se crea el repositorio público **`EdgarAdrian2004/sok`** (el nombre previsto en el plan, `edgarflores/sok`, se ajustó porque las credenciales de GitHub disponibles corresponden a `EdgarAdrian2004`); la carpeta de trabajo se reorganiza en la subcarpeta `sok/`, dejando fuera del repositorio los sílabos, el diagnóstico del I.E. y los ejemplos de referencia. Se inicializa la solución `SOK.sln` con las cuatro capas (`SOK.Domain` → `SOK.Application` → `SOK.Infrastructure` → `SOK.UI`), dependencias unidireccionales, `.gitignore` y `README.md`. Paquetes NuGet alineados con el stack .NET 8: `Microsoft.Data.Sqlite` 8.0.31, `System.Management` 8.0.0 y `CommunityToolkit.Mvvm` 8.4.2. Compilación verificada: 0 errores y 0 advertencias. Nota técnica: en `App.xaml.cs` se califica `System.Windows.Application`, pues dentro del espacio de nombres `SOK.*` el identificador `Application` resuelve a la capa `SOK.Application`. |
| 1.5 | 30/10/2026 | **Decisiones técnicas del Sprint 1.** Se cierran: persistencia con **ADO.NET puro** (`Microsoft.Data.Sqlite` 8.0.31, sin Dapper), pruebas **xUnit + Moq**, inyección de dependencias con **`Ioc.Default`** de CommunityToolkit, **`NavigationService` propio**, `SokDbContext` con el DDL verificado de `docs/db/SOK_DDL.sql`, base de datos en **`%LOCALAPPDATA%\SOK\sok.db`**, proyecto de pruebas en `src/SOK.UI.Tests/` y estrategia de ramas `main` + `feature/sprint-N-*`. Se actualiza la arquitectura (entidades `ProfileSetting`, `Log`, `Setting`; enums `ProfileModule`, `ProfileAction`, `BackupType`, `LogModule`, `ActionResult`) y se confirma el orden: Sprint 1 → B.4/B.6 → B.8 → Sprints 2–5 → Bloque D → A.6 + V.12–V.16. |
| 1.6 | 30/10/2026 | **Sprint 1 completado.** Se implementan las cuatro capas: dominio (entidades, enums y contratos), aplicación (`SystemSnapshot`, `LogService`, `BackupManager`, rutas y SHA-256), infraestructura (`SokDbContext`, cinco repositorios ADO.NET, `SystemInfoProvider` con WMI, respaldos de registro y servicios) y UI WPF (manifiesto UAC, tema, `MainWindow` de dos columnas, `NavigationService` propio, siete vistas, `DashboardViewModel` y arranque con `Ioc.Default`). Se agrega el proyecto de pruebas `src/SOK.UI.Tests` (xUnit + Moq) con 34 pruebas y los scripts `build.ps1`, `test.ps1` y `package.ps1`. Verificación: `dotnet build` con 0 errores y 0 advertencias, 34/34 pruebas correctas y arranque validado en Windows 10 22H2. Evidencia visual en `docs/capturas/sprint-1-dashboard.png`. |
| 1.7 | 30/10/2026 | **Diagramas UML sobre el código real (B.4 y B.6).** Se ejecuta `graphify extract . --code-only` sobre el commit `693eda1` (438 nodos, 991 aristas y 17 comunidades; 86 % extraído del AST). Con ese insumo se elaboran seis diagramas de clases (V.12.0 paquetes, V.12.1 dominio, V.12.2 aplicación, V.12.3 persistencia, V.12.4 servicios de Windows y V.12.5 presentación) y dos de secuencia (V.13.1 respaldo completo y V.13.2 actualización del panel). Se agrega `docs/uml/mapa-conceptual-graphify.md` con el análisis del grafo. Se documenta que un diagrama único de la solución resultaba ilegible (4096 px de ancho), por lo que la sección V.12 se divide en seis archivos. |
| 1.8 | 30/10/2026 | **Bloque B completo (B.8: integración continua).** Se agrega `.github/workflows/ci.yml`: en cada push y pull request hacia `main` instala el SDK de .NET 8 con caché NuGet, restaura, compila `SOK.sln` en `Release`, ejecuta las 34 pruebas con salida TRX, escribe el resumen en el job summary y publica el archivo de resultados como artefacto (14 días). El runner es `windows-latest` porque `SOK.UI` apunta a `net8.0-windows` y las pruebas de integración usan WMI y `reg.exe`. Primera ejecución ([run #1](https://github.com/EdgarAdrian2004/sok/actions/runs/41125212927)) exitosa en 129 segundos. El README incorpora la insignia de estado y la sección del flujo. Con esto, el Bloque B queda en 8/8 y el siguiente paso es el Sprint 2 (Telemetría y Privacidad). |
| 1.9 | 01/11/2026 | **Sprint 2 completado (Telemetría y Privacidad, 16 SP).** Se implementan las tres historias: catálogo estático de **46 claves en 11 categorías** con estado leído en vivo del registro (`TelemetryCatalog`, `ITelemetryRegistryService`, `TelemetryRegistryService` con `Microsoft.Win32.Registry` 5.0.0), cambio individual con interruptor (`TelemetryManager.SetDisabledAsync`) y privacidad máxima con un único respaldo (`ApplyMaxPrivacyAsync`). `IRegistryBackupService` se amplía con `CreateBackupForKeysAsync`, que exporta solo las claves afectadas, calcula el SHA-256 del manifiesto, registra las claves inexistentes como «no requiere respaldo» y aborta la operación si una clave existente no se puede exportar. La UI incorpora `TelemetryViewModel`, `TelemetryItemViewModel`, `TelemetryCategoryGroup`, la vista con tarjetas por categoría, filtro, resumen, badges de reinicio y los estilos `SokToggleSwitch`, `WarningBadge` y `BadgeText`. **Decisión de diseño:** se descartan las tablas `TelemetryKeys` y `TelemetryActionLogs` del plan inicial; el estado se lee del registro y la auditoría reutiliza la tabla `Logs` con `LogModule.Telemetria`. Verificación: 0 errores y 0 advertencias en Debug y Release, **55/55 pruebas correctas** (21 nuevas) y validación visual en Windows 10 22H2 con captura en `docs/capturas/sprint-2-telemetria.png`. |
| 1.10 | 01/11/2026 | **Sprint 3 completado (Gestión de Servicios, 13 SP).** Se implementan las tres historias: catálogo estático de **30 servicios en 10 categorías** con estado leído del SCM, cambio individual con interruptor y perfiles Gamer/Ofimático. La lectura usa **WMI** (`Win32_Service`) y la escritura **`ServiceController`** más **`ChangeServiceConfig`** (`advapi32.dll`, P/Invoke), descartando `sc.exe`. `IServicesBackupService` se amplía con `CreateBackupForServicesAsync`, que exporta solo los servicios afectados y aborta si ninguno existe. La UI incorpora `ServicesViewModel`, `ServiceItemViewModel`, `ServiceCategoryGroup`, la vista con tarjetas por categoría, filtro, resumen, badges de riesgo y los estilos `DangerBadge`/`DangerBadgeText`. **Criterio de perfiles conservador en usabilidad:** ningún perfil deshabilita Bluetooth, Windows Search, Windows Hello, impresión, soporte remoto ni las apps nativas de Windows; los servicios de alto riesgo se listan para cambio manual, nunca automático. Verificación: 0 errores y 0 advertencias en Debug y Release, **96/96 pruebas correctas** (41 nuevas), validación real de deshabilitar y habilitar `PrintNotify` con dos respaldos y dos entradas en `Logs`, y captura en `docs/capturas/sprint-3-servicios.png`. |
| 1.11 | 01/11/2026 | **Sprint 4 completado (Liberación de Memoria, 18 SP).** Se implementan las tres historias: medidor de RAM en tiempo real con `GlobalMemoryStatusEx`, liberación manual con `EmptyWorkingSet` sobre todos los procesos accesibles y liberación automática por umbral configurable (70 %–95 %, valor por defecto 80 %, intervalo mínimo de un minuto). La infraestructura usa P/Invoke (`EnumProcesses`, `OpenProcess`, `EmptyWorkingSet`, `CloseHandle`) con `EntryPoint` explícito; `MemoryManager` persiste la preferencia en `Settings` (`Memory.AutoReleaseEnabled` y `Memory.AutoReleaseThreshold`) con lectura de respaldo de la clave heredada `MemoryReleaseThreshold` y audita en `Logs` con el módulo `Memoria`. **Sin respaldo previo:** la liberación no modifica configuración persistente. **Desviación justificada:** se omiten `CachedBytes` y `ModifiedBytes` porque `GlobalMemoryStatusEx` no los expone. La UI incorpora `MemoryViewModel` (medición cada tres segundos, comandos, guardado del umbral) y `MemoryView`. Verificación: 0 errores y 0 advertencias en Debug y Release, **127/127 pruebas correctas** (31 nuevas), liberación real de 207 de 222 procesos con 399,1 MB estimados, liberación automática simulada registrada en `Logs` y captura en `docs/capturas/sprint-4-memoria.png`. |
| 1.12 | 02/11/2026 | **Sprint 5 completado (Perfiles + Restauración + Tutorial, 21 SP).** Se implementan las cuatro historias: perfiles base Gamer y Ofimático generados desde los catálogos (`ProfileCatalog`), activación persistente en `Settings.ActiveProfileId`, aplicación con respaldo por módulo, exportación e importación JSON versionada, restauración con verificación SHA-256 del manifiesto y de cada `.reg`, importación con `reg.exe` y devolución de servicios a su tipo de inicio anterior, y tutorial de ocho pasos en Markdown incrustado y renderizado a `FlowDocument` con Markdig. `ITelemetryManager` y `IServiceManager` incorporan `ApplyDisabledAsync` para aplicar un conjunto explícito con un único respaldo. La UI suma `ProfilesView`, `BackupsView` y `TutorialView` con sus ViewModels, insignias de perfil activo e integridad, confirmaciones y navegación desde el tutorial. El tutorial se abre en la primera ejecución y `TutorialCompleted` evita repetirlo. Verificación: 0 errores y 0 advertencias en Debug y Release, **167/167 pruebas correctas** (40 nuevas) y grafo de Graphify regenerado con 1503 nodos, 4116 aristas y 74 comunidades sobre el commit `8d48a95`. |
| 1.13 | 03/11/2026 | **Fase de diseño visual y tema oscuro (Bloque C.5).** Se define el sistema de diseño en `docs/design/design-tokens.json`: **451 tokens** (179 globales, 72 semánticos por tema y 128 de componentes) para tema claro, oscuro, alto contraste y contraste forzado. Se construye un **prototipo navegable de seis pantallas** en HTML, CSS y JavaScript sin dependencias (`docs/design/prototype/`), que se abre desde `index.html` sin compilación ni servidor y sirve de referencia para la traducción a WPF. El prototipo reemplaza Inter por Segoe UI, adopta iconografía de trazo de 24 × 24 y geometría Fluent, y **rediseña el paso «Panel de control» del tutorial** con estados de paso accesibles, mapa numerado de la interfaz y barra de acciones fija. Se documentan las decisiones, la lista de accesibilidad verificada y la correspondencia entre tokens CSS y `Theme.xaml` en `docs/design/prototype/README.md`. **Alcance nuevo:** el tema oscuro, que la v1.12 no contemplaba, pasa a formar parte del producto. La lógica de negocio no se modifica y las 167 pruebas siguen siendo el criterio de no regresión. |

---

## FIN DEL DOCUMENTO

Este documento es la fuente única de verdad del proyecto SOK. Cualquier cambio en el alcance, stack, arquitectura o plan debe quedar reflejado aquí en una nueva versión.

**Próxima actualización sugerida:** al cerrar el Bloque C.5 (traducción del prototipo a WPF y tema oscuro) y, después, al cerrar el Bloque D (validación en campo, instalador y cierre del informe TAP).

**Contacto del autor:** Edgar Adrian Flores Herrera (egresado IESTP Trujillo).
