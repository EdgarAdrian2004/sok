# PLAN MAESTRO DEL PROYECTO SOK

## System Optimization Key — Trabajo de Aplicación Profesional

> **Documento de referencia única** para el desarrollo del TAP de Edgar Adrian Flores Herrera.
> Generado el: 29 de septiembre de 2026 — Actualizado: 4 de octubre de 2026
> Versión: 1.3 (datos corregidos + diseño de BD y casos de uso con sub-diagramas)
> Estado: En desarrollo (Bloque A completo salvo Introducción; Bloque B en curso)

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
| **Datos** | SQLite + Dapper | SQLite 3.x |
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

│ ├── Entities: Profile, Backup, OptimizationAction │

│ ├── Enums: ProfileType, ModuleType, OptimizationResult │

│ └── Rules: lógica de negocio pura (sin dependencias) │

├──────────────────────────────────────────────────────────┤

│ CAPA INFRAESTRUCTURA (System Access) │

│ SOK.Infrastructure │

│ ├── Windows: RegistryService, ServiceManager, │

│ │ MemoryOptimizer, SystemInfoProvider │

│ ├── Persistence: SQLiteDatabase (SQLite + Dapper) │

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
| B.1 | Crear repo GitHub `edgarflores/sok` + estructura de carpetas | — (código) | ⏳ Pendiente |
| B.2 | Crear solución VS 2022 con 4 capas (Domain, Application, Infrastructure, UI) | — (código) | ⏳ Pendiente |
| B.3 | Diseñar BD SQLite (tablas: Profiles, Backups, Logs, Settings) | Sección V.17 | ✅ Hecho (DDL verificado) |
| B.4 | Elaborar diagrama UML de clases | Sección V.11 | ⏳ Pendiente |
| B.5 | Elaborar diagrama UML de casos de uso | Sección V.11 | ✅ Hecho (general + 8 sub-diagramas, 18/18 HU) |
| B.6 | Elaborar diagrama UML de secuencia (flujo optimizar) | Sección V.11 | ⏳ Pendiente |
| B.7 | Refinar Requerimientos RF y RNF | Sección V.18 | ✅ Ya hecho |
| B.8 | Configurar CI en GitHub Actions (que compile en cada push) | — (código) | ⏳ Pendiente |

### BLOQUE C — IMPLEMENTACIÓN POR SPRINTS

**Objetivo:** Construir el programa SOK funcional mediante 5 sprints.
**Duración estimada:** 10 semanas (5 sprints × 2 semanas)
**Salida esperada:** Programa funcional + Sección V.19 completa.

| Sprint | Módulo | Historias | Semanas | Story points | Documento |
| --- | --- | --- | --- | --- | --- |
| **Sprint 1** | Núcleo + Dashboard + Backup | HU-01, 02, 03, 04, 16 | 2 | 23 | V.19 Sprint 1 |
| **Sprint 2** | Telemetría y Privacidad | HU-05, 06, 07 | 2 | 16 | V.19 Sprint 2 |
| **Sprint 3** | Gestión de Servicios | HU-08, 09, 10 | 2 | 13 | V.19 Sprint 3 |
| **Sprint 4** | Liberación de Memoria | HU-11, 12, 13 | 2 | 18 | V.19 Sprint 4 |
| **Sprint 5** | Perfiles + Restore + Tutorial | HU-14, 15, 17, 18 | 2 | 21 | V.19 Sprint 5 |

#### Detalle Sprint 1 — Núcleo y Dashboard (23 SP)

| HU | Tareas técnicas | SP |
| --- | --- | --- |
| HU-02 | Configurar app.manifest con requireAdministrator. Probar elevación en Windows 10. Documentar comportamiento. | 2 |
| HU-03 | Crear MainWindow.xaml con layout de 2 columnas. Implementar NavigationService. Crear vistas vacías para cada módulo. Aplicar estilos visuales básicos. | 3 |
| HU-01 | Implementar SystemInfoProvider con WMI. Crear DashboardView con medidores. Implementar data binding con MVVM. Probar actualización en tiempo real. | 5 |
| HU-04 | Crear esquema de tabla Logs en SQLite. Implementar LogRepository. Crear LogService en Application. Integrar registro en acciones del dashboard. | 5 |
| HU-16 | Diseñar esquema de tabla Backups. Implementar RegistryBackupService. Implementar ServicesBackupService. Crear BackupManager orquestador. Generar hash SHA-256 de cada respaldo. | 8 |

**Definición de Hecho (DoD) del Sprint 1:**

- Código versionado en Git con commits atómicos
- Pruebas unitarias pasan (xUnit)
- Código revisado
- Funcionalidad validada en Windows 10
- Bitácora del sprint actualizada

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

#### Detalle Sprint 3 — Gestión de Servicios (13 SP)

| HU | Descripción | SP |
| --- | --- | --- |
| HU-08 | Listado de servicios con estado (al menos 20 servicios no esenciales) | 5 |
| HU-09 | Detener y deshabilitar servicios individualmente | 5 |
| HU-10 | Perfiles de servicios (Gamer/Ofimático) | 3 |

**Servicios a catalogar (estudio previo):**

- DiagTrack, dmwappushservice (telemetría)
- Fax, PrintNotify (no usados típicamente)
- RemoteRegistry, RemoteAccess (seguridad)
- SysMain/Superfetch (depende del caso)
- WSearch (indexación, útil pero consume recursos)
- Xbox services (para perfil ofimático)
- Biometric services (si no se usan)
- Bluetooth services (si no se usan)
- Otros servicios de telemetría de Windows 10/11

#### Detalle Sprint 4 — Liberación de Memoria (18 SP)

| HU | Descripción | SP |
| --- | --- | --- |
| HU-11 | Medidor en tiempo real de RAM | 5 |
| HU-12 | Liberar memoria con un clic (EmptyWorkingSet) | 8 |
| HU-13 | Liberación automática por umbral (70%-95%) | 5 |

**APIs Win32 a usar:**

- `EmptyWorkingSet` (psapi.dll) — libera working set de un proceso
- `EnumProcesses` (psapi.dll) — lista procesos activos
- `OpenProcess` (kernel32.dll) — obtiene handle de un proceso
- `GlobalMemoryStatusEx` (kernel32.dll) — info de memoria global

#### Detalle Sprint 5 — Perfiles + Restore + Tutorial (21 SP)

| HU | Descripción | SP |
| --- | --- | --- |
| HU-14 | Selección de perfil al iniciar (Gamer/Ofimático/Personalizado) | 5 |
| HU-17 | Restaurar sistema a punto anterior desde backup | 8 |
| HU-18 | Tutorial integrado con guía paso a paso | 5 |
| HU-15 | Exportar/importar perfil JSON | 3 |

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
| Repositorio GitHub `edgarflores/sok` | B | ⏳ Pendiente |
| Solución VS 2022 con 4 capas | B | ⏳ Pendiente |
| Esquema SQLite (DDL) — `docs/db/SOK_DDL.sql` | B | ✅ Hecho (verificado) |
| Diseño de BD (conceptual/lógico/físico) — `docs/db/diseno-base-datos.md` | B | ✅ Hecho |
| Diagrama ER — `docs/db/ER_SOK.png` (+ espejo `.dbml`) | B | ✅ Hecho |
| Diagrama UML casos de uso (general + 8 sub-diagramas) — `docs/uml/` | B | ✅ Hecho (18/18 HU) |
| Descripción de casos de uso — `docs/uml/descripcion-casos-uso.md` | B | ✅ Hecho |
| Diagrama UML de clases | B | ⏳ Pendiente |
| Diagrama UML de secuencia | B | ⏳ Pendiente |
| Configurar CI en GitHub Actions | B | ⏳ Pendiente |
| Implementación Sprint 1 (núcleo + dashboard + backup) | C | ⏳ Pendiente |
| Implementación Sprint 2 (telemetría) | C | ⏳ Pendiente |
| Implementación Sprint 3 (servicios) | C | ⏳ Pendiente |
| Implementación Sprint 4 (memoria) | C | ⏳ Pendiente |
| Implementación Sprint 5 (perfiles + tutorial) | C | ⏳ Pendiente |
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
| 13 | **Empaquetado MSI / MSIX / Inno Setup** | Distribución instalable | Sprint 5 |
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

Crear el repo GitHub + solución Visual Studio + diagramas UML + diseño BD.

### Paso inmediato 4: Bloque C — Implementación

5 sprints de 2 semanas cada uno.

### Paso inmediato 5: Bloque D — Validación y cierre

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

---

## FIN DEL DOCUMENTO

Este documento es la fuente única de verdad del proyecto SOK. Cualquier cambio en el alcance, stack, arquitectura o plan debe quedar reflejado aquí en una nueva versión.

**Próxima actualización sugerida:** al cierre del Bloque A completo (después de Referente Teórico e Introducción).

**Contacto del autor:** Edgar Adrian Flores Herrera (egresado IESTP Trujillo).
