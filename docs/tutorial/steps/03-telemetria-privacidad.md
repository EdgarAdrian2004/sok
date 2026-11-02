---
order: 3
title: "Telemetría y privacidad"
module: Telemetry
action: OpenTelemetry
---

# Telemetría y privacidad

El módulo **Telemetría y privacidad** permite controlar las claves de
Windows relacionadas con el envío de datos de uso, la publicidad personalizada
y algunas funciones de búsqueda.

## Estados de cada clave

Cada elemento del catálogo muestra uno de estos estados:

- **Deshabilitado**: la clave ya tiene el valor de privacidad aplicado.
- **Habilitado**: la clave conserva el valor predeterminado de Windows.
- **No disponible**: la clave no existe en este equipo o no pudo leerse.

## Cambios individuales

Use el interruptor de cada fila para deshabilitar o habilitar una clave. Antes
de escribir en el registro, SOK crea un respaldo de las claves afectadas y
anota el resultado en la bitácora.

## Privacidad máxima

El botón **Aplicar privacidad máxima** deshabilita de una sola vez todas las
claves que todavía estén habilitadas. La operación genera un único respaldo y
un único registro en la bitácora.

> Algunos cambios de telemetría surten efecto después de reiniciar Windows.
> SOK lo indicará en el mensaje de resultado cuando corresponda.

## Buenas prácticas

- Revise el estado antes de aplicar cambios masivos.
- Si un programa o función de Windows deja de comportarse como esperaba,
  restaure el respaldo más reciente desde el módulo **Respaldos**.
