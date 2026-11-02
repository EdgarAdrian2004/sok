---
order: 6
title: "Respaldos y restauración"
module: Backups
action: OpenBackups
---

# Respaldos y restauración

El módulo **Respaldos** concentra los puntos de restauración que SOK ha
creado y permite devolver el sistema a un estado anterior.

## Tipos de respaldo

| Tipo | Contenido |
| --- | --- |
| Registro | Claves del registro exportadas como archivos `.reg`. |
| Servicios | Estado y tipo de inicio de los servicios afectados. |
| Completo | Registro y servicios en un mismo punto de restauración. |

Cada respaldo se guarda con un archivo de manifiesto y un hash SHA-256 que
permite comprobar que no fue alterado.

## Integridad de los respaldos

La lista muestra una de estas leyendas:

- **Íntegro (SHA-256 verificado)**: el archivo existe y su hash coincide.
- **Hash alterado**: el archivo cambió después de crearse; no se restaurará.
- **Archivo no encontrado**: el archivo ya no está en la ruta registrada.

SOK solo habilita el botón **Restaurar** cuando el respaldo es íntegro.

## Proceso de restauración

1. Seleccione un respaldo y pulse **Restaurar**.
2. Lea la confirmación y acepte.
3. SOK verifica el hash del manifiesto y de cada archivo `.reg`.
4. Se importan las claves de registro y se devuelve cada servicio a su tipo
   de inicio anterior.
5. El resultado queda registrado en la bitácora como **Revertido**.

> La restauración no cambia el estado de ejecución de los servicios ni la
> lista de standby de memoria. Tampoco reinicia Windows: si se modificaron
> claves de telemetría que lo requieren, reinicie el equipo para completar el
> cambio.
