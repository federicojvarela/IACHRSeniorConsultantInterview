# Ejercicio de Entrevista para Consultor Senior IACHR

## Introducción

Este proyecto está diseñado como un ejercicio para evaluar las habilidades de arquitectura, diseño y desarrollo de candidatos a posiciones de consultor senior. El sistema implementa un procesador básico de documentos con catálogos que presenta oportunidades de mejora significativas en términos de arquitectura y rendimiento.

## Estructura del Proyecto

El proyecto sigue una arquitectura de capas:

- **Core**: Contiene las entidades, interfaces y servicios del dominio.
- **Infrastructure**: Implementa los repositorios y servicios definidos en Core.
- **WebApi**: Proporciona una API REST para interactuar con el sistema.

## Desafíos para el Candidato

El sistema actual tiene limitaciones importantes que el candidato debe identificar y resolver:

### 1. Procesamiento de Documentos

Actualmente, el sistema procesa los documentos de manera sincrónica, lo que genera:
- Bloqueo del hilo de ejecución durante el procesamiento
- Tiempos de respuesta largos en la API
- Potenciales problemas de escalabilidad bajo carga

Se espera que el candidato:
- **Transforme** el sistema para permitir procesamiento asincrónico
- **Diseñe** una arquitectura que permita escalabilidad horizontal
- **Proponga e implemente** patrones arquitectónicos que mejoren la separación de responsabilidades

> **Nota:** Valoramos especialmente soluciones que demuestren conocimiento de arquitecturas modernas y desacopladas. Las implementaciones que consideren aspectos como resiliencia, observabilidad y mantenibilidad serán evaluadas positivamente.

### 2. Gestión de Catálogos

Los catálogos son consultados directamente desde el archivo JSON cada vez que se solicitan, causando:
- Tiempos de respuesta inconsistentes
- Carga innecesaria del sistema de archivos
- Uso ineficiente de recursos

Se espera que el candidato:
- **Diseñe e implemente** una capa de caché eficiente
- **Defina** estrategias adecuadas de invalidación de caché
- **Considere** aspectos como la consistencia de datos y la gestión de memoria

## Criterios de Evaluación

Se evaluará principalmente:

- **Diseño arquitectónico**: Capacidad para identificar y aplicar patrones de diseño y arquitectura adecuados para resolver los problemas planteados
- **Calidad del código**: Limpieza, mantenibilidad, seguimiento de principios SOLID
- **Visión técnica**: Capacidad para ver más allá de la solución inmediata y proponer mejoras que añadan valor a largo plazo
- **Toma de decisiones**: Justificación clara de las decisiones técnicas tomadas y consideración de alternativas
- **Conocimiento de tecnologías modernas**: Uso apropiado de características avanzadas del lenguaje y frameworks

No buscamos simplemente que el código "funcione", sino que demuestre un pensamiento arquitectónico maduro que anticipe futuras necesidades y cambios.

## Cómo Ejecutar el Proyecto

1. Clonar el repositorio
2. Restaurar las dependencias
3. Ejecutar el proyecto WebApi
4. Acceder a Swagger para probar la API: `https://localhost:5001/swagger`

## Entregables Esperados

1. Código fuente con las mejoras implementadas
2. Documentación breve que explique:
   - Patrones arquitectónicos aplicados
   - Justificación de las decisiones técnicas
   - Consideraciones de escalabilidad y mantenibilidad
   - Posibles mejoras adicionales que implementaría con más tiempo