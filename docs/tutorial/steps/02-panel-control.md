---
order: 2
title: "Panel de control"
module: Dashboard
action: OpenDashboard
---

# Panel de control

El **Panel de control** es la primera pantalla de SOK y muestra el estado
general del equipo en tiempo real.

## Indicadores disponibles

| Indicador | Qué muestra |
| --- | --- |
| CPU | Porcentaje de uso del procesador. |
| Memoria RAM | Memoria utilizada respecto del total instalado. |
| Disco | Espacio libre respecto de la capacidad total. |

Además, el panel presenta el nombre del sistema operativo, el procesador, el
nombre del equipo y el tiempo transcurrido desde el último encendido.

## Actualización de los datos

Los indicadores se actualizan automáticamente cada tres segundos. El botón
**Actualizar ahora** fuerza una lectura inmediata y registra la consulta en
la bitácora.

## Respaldo completo

El botón **Crear respaldo completo** guarda el estado de las claves de
registro y de los servicios que SOK administra. Este respaldo aparece luego
en el módulo **Respaldos** y puede restaurarse si fuera necesario.

> Recomendación: cree un respaldo completo antes de aplicar cambios masivos,
> aunque SOK ya genera respaldos automáticos en cada operación.
