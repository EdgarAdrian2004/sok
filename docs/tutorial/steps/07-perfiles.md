---
order: 7
title: "Perfiles de optimización"
module: Profiles
action: OpenProfiles
---

# Perfiles de optimización

Un **perfil** agrupa ajustes de telemetría, servicios y memoria para
aplicarlos de una sola vez. SOK incluye dos perfiles base y permite incorporar
perfiles personalizados mediante un archivo JSON.

## Perfiles incluidos

- **Gamer**: deshabilita telemetrías y servicios no esenciales, incluida la
  notificación de impresión, y configura la liberación automática de memoria
  al 75 %.
- **Ofimático**: deshabilita telemetrías y servicios no esenciales sin tocar
  impresión ni Xbox, y configura la liberación automática de memoria al 85 %.

## Activar y aplicar

- **Activar** marca el perfil como activo para la sesión y lo guarda en la
  configuración. No modifica el sistema.
- **Aplicar** ejecuta los ajustes del perfil sobre Windows. Antes de hacerlo,
  SOK solicita confirmación y crea un respaldo de las claves de registro y de
  los servicios afectados.

El perfil activo se muestra con una insignia verde en la lista.

## Exportar e importar

El botón **Exportar** guarda el perfil seleccionado en un archivo JSON legible.
El botón **Importar perfil JSON** incorpora un perfil externo como perfil
**Personalizado**. Si el nombre ya existe, SOK lo renombra automáticamente con
el sufijo **(importado)**.

> En esta versión los perfiles personalizados solo se crean por importación
> JSON; no existe un editor manual de perfiles.

## Buenas prácticas

Exporte sus perfiles antes de modificar el equipo y conserve los archivos en
una carpeta segura. Así podrá reutilizarlos en otro equipo o restaurarlos
después de una reinstalación.
