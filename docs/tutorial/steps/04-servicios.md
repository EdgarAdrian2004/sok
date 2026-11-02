---
order: 4
title: "Gestión de servicios"
module: Services
action: OpenServices
---

# Gestión de servicios

El módulo **Servicios** muestra los servicios de Windows que SOK conoce y
permite deshabilitar los que no son esenciales para el uso diario.

## Cómo leer la lista

Los servicios se agrupan por categoría y muestran su nombre, su descripción,
su tipo de inicio y su estado actual. Use el selector de categoría para
concentrarse en un grupo determinado.

## Cambios individuales

El interruptor de cada servicio detiene el servicio y cambia su tipo de
inicio a **Deshabilitado**, o lo devuelve a su tipo de inicio original. Cada
cambio genera un respaldo en JSON antes de modificar el sistema.

## Perfiles de servicios

Los botones **Aplicar perfil Gamer** y **Aplicar perfil Ofimático** actúan
sobre todos los servicios que correspondan al perfil:

- **Gamer**: prioriza el rendimiento y deshabilita una lista más amplia de
  servicios no esenciales.
- **Ofimático**: prioriza la estabilidad y conserva servicios de impresión y
  aplicaciones nativas.

## Servicios de alto riesgo

Algunos servicios afectan funciones sensibles, como la impresión, el soporte
remoto o la seguridad del equipo. SOK los identifica con una etiqueta de
riesgo y no los incluye en los perfiles automáticos: solo pueden cambiarse de
forma manual y con confirmación del usuario.

> Si deshabilita un servicio y alguna función deja de funcionar, vuelva a
> habilitarlo o restaure el respaldo correspondiente.
