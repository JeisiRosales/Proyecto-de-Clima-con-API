# 🌦️ Análisis y visualización de datos climáticos en Venezuela

Sistema que consume datos meteorológicos desde la **Visual Crossing Weather API**, los procesa con un script en **C#**, los almacena en **SQL Server**, y los presenta mediante **Power BI** y una **aplicación web (GitHub Pages)** para consulta pública.

### ¿Por qué este proyecto?
Este proyecto nace como una práctica integral para fortalecer mis habilidades como **analista de datos**.  El objetivo es demostrar el ciclo completo de trabajo con datos: desde la **obtención (API)**, pasando por la **transformación y almacenamiento (SQL Server + C#)**, hasta la **visualización y análisis (Power BI + Web)**.  

Elegí este caso de estudio porque el clima es un tipo de dato **dinámico, cambiante y con múltiples dimensiones**, lo que permite aplicar conceptos de calidad de datos, modelado, visualización y comunicación de resultados, simulando un escenario real de trabajo en una empresa.

## 🚀 Características y tecnologías utilizadas

- 🌐 **Consumo de API**: integración con Visual Crossing Weather API usando **Postman** para obtener datos meteorológicos.  
- 💾 **Almacenamiento en SQL Server**: base de datos `ClimaDB` con tablas para localizaciones y registros climáticos.  
- ⚙️ **Script en C#**: conecta a la API, guarda datos en SQL y genera archivos JSON para la web.  
- 📊 **Dashboard en Power BI**: informes interactivos para explorar tendencias y métricas.  
- 💻 **Aplicación Web (GitHub Pages)**: visualización pública de datos climáticos con gráficos (Chart.js).  
- 🔒 **Buenas prácticas de seguridad**: uso de placeholders para API keys y credenciales de base de datos.  

## 🏗️ Arquitectura del proyecto

El flujo de datos sigue este recorrido:

API Visual Crossing → Script C# → Base de Datos SQL Server → Power BI + JSON Export → Web (GitHub Pages)

```text
                                                    ┌─────────────────────┐
                                                    │  Visual Crossing    │
                                                    │     Weather API     │
                                                    └─────────┬───────────┘
                                                              │
                                                              ▼
                                                       ┌────────────┐
                                                       │  Script C# │
                                                       │ (consume   │
                                                       │   API)     │
                                                       └─────┬──────┘
                                                             │
                                             ┌───────────────┴───────────────┐
                                             ▼                               ▼
                                     ┌───────────────┐                ┌───────────────┐
                                     │  SQL Server   │                │ Archivos JSON │
                                     │   ClimaDB     │                │   (/docs)     │
                                     └───────┬───────┘                └───────┬───────┘
                                             │                               │
                                             ▼                               ▼
                                     ┌───────────────┐                ┌───────────────┐
                                     │   Power BI    │                │   Web App     │
                                     │ Dashboard     │                │ (GitHub Pages)│
                                     └───────────────┘                └───────────────┘

```
## 🔧 Requisitos

Para poder ejecutar este proyecto en tu entorno local necesitas:

- **.NET SDK 6.0 o superior** → para compilar y ejecutar el script en C#  
- **SQL Server 2019+ y SQL Server Management Studio (SSMS)** → para crear y gestionar la base de datos `ClimaDB`  
- **Power BI Desktop** → para abrir y explorar el reporte interactivo  
- **Git** → para clonar el repositorio
- **Visual Studio 2022** → para ejecutar el script de C#
- **Navegador web moderno** (Chrome, Edge, Firefox, etc.) → para visualizar la aplicación web  

👉 Recursos externos necesarios:
- Cuenta gratuita en [Visual Crossing Weather API](https://www.visualcrossing.com/weather-api) para obtener tu **API Key**.

## 📂 Estructura del repositorio

El repositorio está organizado de la siguiente manera:

- /docs → Contiene la aplicación web y los archivos JSON generados
   - /data → Archivos clima_actual.json y clima_historico.json
- /postman → Colección de requests para pruebas en Postman
- /sql → Scripts SQL para crear la base de datos y tablas
- /src → Código fuente en C# (Program.cs y solución .NET)
- /powerbi → Archivo .pbix con el dashboard de Power BI
- .github/workflows → Workflows opcionales para automatización con GitHub Actions
- README.md → Documentación principal del proyecto
- LICENSE → Licencia del proyecto

## ⚡ Instalación y configuración rápida

1. Clona el repositorio
2. Configura tus credenciales
   - Reemplaza los valores con tu API Key y tus credenciales de SQL Server en el scrpit de C#.
3. Crea la base de datos
   - Abre ClimaDB_Script.sql en SSMS.
   - Ejecútalo para crear la base de datos y tablas necesarias.
4. Ejecuta el script C#
   - Esto consumirá la API, cargará los datos en ClimaDB y generará los archivos JSON en /docs/data.
5. **Accede a la aplicación web**
   - La aplicación está publicada directamente en **GitHub Pages** y puede verse en el siguiente enlace:  
     👉 [Demo en vivo](https://JeisiRosales.github.io/proyecto-clima/)  

   *(Opcional para desarrolladores)*  
   Si deseas probar la web localmente, abre `/docs/index.html` en tu navegador o usa un servidor local
6. Explora el reporte en Power BI
   - Abre /powerbi/ReporteClima.pbix en Power BI Desktop.
   - Actualiza la conexión a tu SQL Server si es necesario.
