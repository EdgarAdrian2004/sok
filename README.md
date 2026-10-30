# SOK — System Optimization Key

[![CI](https://github.com/EdgarAdrian2004/sok/actions/workflows/ci.yml/badge.svg)](https://github.com/EdgarAdrian2004/sok/actions/workflows/ci.yml)

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

## Integración continua

El workflow `.github/workflows/ci.yml` se ejecuta en cada push y en cada pull request dirigido a `main`:

1. Instala el SDK de .NET 8.
2. Restaura los paquetes de la solución.
3. Compila `SOK.sln` en configuración `Release`.
4. Ejecuta las 34 pruebas (30 unitarias + 4 de integración) y publica el resumen y el archivo TRX como artefacto.

## Documentación

- `PLAN_MAESTRO_PROYECTO_SOK.md` — fuente única de verdad del proyecto
- `docs/uml/` — diagramas de casos de uso (general + 8 sub-diagramas), diagramas de clases (paquetes + 5 capas) y diagramas de secuencia (respaldo completo y actualización del panel)
- `docs/uml/mapa-conceptual-graphify.md` — mapa de entidades y dependencias reales del código (438 nodos y 991 aristas)
- `docs/db/` — diseño de base de datos: DDL verificado, modelo entidad-relación y documento de diseño
- `docs/capturas/` — evidencia visual de la aplicación en ejecución
