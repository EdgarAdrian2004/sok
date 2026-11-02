---
order: 5
title: "Liberación de memoria RAM"
module: Memory
action: OpenMemory
---

# Liberación de memoria RAM

El módulo **Memoria RAM** mide el uso de memoria física y permite liberar el
working set de los procesos activos.

## Medidor en tiempo real

El medidor se actualiza cada tres segundos y muestra el porcentaje de memoria
utilizada, la cantidad usada respecto del total y la memoria disponible. El
botón **Actualizar** fuerza una lectura inmediata.

## Liberación manual

El botón **Liberar memoria** solicita a los procesos activos que devuelvan al
sistema la memoria que no están utilizando. Al finalizar, SOK informa cuántos
procesos respondieron y una estimación de la memoria liberada.

Esta acción **no genera respaldo**, porque no modifica ninguna configuración
permanente del equipo: solo cambia el estado transitorio de los procesos.

## Liberación automática

Active el interruptor **Liberación automática** y ajuste el umbral entre
**70 % y 95 %**. Cuando el uso de RAM alcance o supere ese umbral, SOK
liberará memoria automáticamente, con un intervalo mínimo entre liberaciones
para evitar acciones repetidas.

> La liberación automática se evalúa mientras SOK está en ejecución y el
> módulo de memoria se ha abierto al menos una vez en la sesión.

## Interpretación del resultado

La estimación de memoria liberada puede variar entre mediciones. Algunos
procesos vuelven a solicitar memoria poco después; por eso el indicador puede
subir nuevamente. Lo importante es que el equipo recupere capacidad de
respuesta sin cerrar aplicaciones.
