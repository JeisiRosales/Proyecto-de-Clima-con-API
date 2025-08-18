# PROYECTO PERSONAL
## Proyecto de Clima con API de OpenWeather

En este proyecto se integra **Postman, SQL Server, Power BI y una aplicación web (HTML/JS)** utilizando datos de la API de OpenWeather.  
El objetivo es aprender el flujo completo de **consumo, almacenamiento, análisis y visualización de datos**.

---

## 🚀 Tecnologías utilizadas
- **Postman** → para probar y validar la API.  
- **SQL Server** → almacenamiento de datos históricos de clima.  
- **Power BI** → creación de dashboards interactivos.  
- **HTML, CSS y JavaScript** → app web para mostrar datos en tiempo real.  
- **OpenWeather API** → fuente de datos del clima.

---

## 🔗 Flujo del Proyecto
1. **Postman**:  
   - Probar la API de OpenWeather con una ciudad determinada.  
   - Validar parámetros y respuesta en formato JSON.  

2. **SQL Server**:  
   - Crear tabla `Clima` con campos (`Fecha`, `Ciudad`, `Temperatura`, `Humedad`).  
   - Insertar registros diarios con datos obtenidos de la API.  

3. **Power BI**:  
   - Conectarse a SQL Server.  
   - Generar gráficos de tendencias de temperatura y humedad.  

4. **Web (HTML/JS)**:  
   - Interfaz sencilla con un campo de búsqueda de ciudad.  
   - Uso de `fetch()` para consultar la API y mostrar los datos en pantalla.  
