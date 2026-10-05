# SOK — System Optimization Key

Aplicación de escritorio Windows (C# .NET 8 + WPF + MVVM) que unifica las herramientas de optimización del sistema operativo en una sola interfaz, pensada para equipos con recursos limitados.

Proyecto de titulación (TAP) — **Edgar Adrian Flores Herrera**
IESTP "Trujillo" — Diseño y Programación Web

## Módulos

| Módulo | Descripción |
| --- | --- |
| Panel de control | Estado del sistema en tiempo real (CPU, RAM, disco, servicios) |
| Telemetría y privacidad | Desactivación de telemetrías de Windows y «Privacidad máxima» |
| Servicios | Gestión de servicios no esenciales y perfiles Gamer / Ofimático |
| Memoria RAM | Medidor en tiempo real, liberación manual y liberación por umbral |
| Respaldos | Respaldos de registro y servicios con integridad SHA-256 y restauración |
| Perfiles | Perfiles de optimización con exportación / importación en JSON |
| Tutorial | Guía paso a paso para usuarios sin conocimientos técnicos |

**Regla de oro:** toda acción que modifique la configuración persistente del sistema genera un respaldo primero.

## Arquitectura

Cuatro capas con dependencias unidireccionales:

```
SOK.UI → SOK.Infrastructure → SOK.Application → SOK.Domain
```

| Capa | Responsabilidad |
| --- | --- |
| `SOK.Domain` | Entidades, enumeraciones e interfaces. Sin dependencias externas. |
| `SOK.Application` | Servicios y lógica de aplicación. |
| `SOK.Infrastructure` | Persistencia SQLite, servicios de Windows, Win32 P/Invoke. |
| `SOK.UI` | Interfaz WPF con patrón MVVM. |

## Stack tecnológico

- C# .NET 8 + WPF
- MVVM con CommunityToolkit.Mvvm
- SQLite (Microsoft.Data.Sqlite)
- PowerShell 7
- Win32 P/Invoke

## Requisitos

- Windows 10 o 11
- .NET 8 SDK
- Visual Studio 2022

## Estructura del repositorio

```
src/        código de las cuatro capas
docs/       diagramas UML y diseño de base de datos
*.md        documentación del proyecto
```

## Documentación

- `PLAN_MAESTRO_PROYECTO_SOK.md` — fuente única de verdad del proyecto
- `docs/uml/` — diagramas de casos de uso (general + 8 sub-diagramas) y descripciones
- `docs/db/` — diseño de base de datos: DDL verificado, modelo entidad-relación y documento de diseño
