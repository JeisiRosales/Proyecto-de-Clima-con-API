
USE ClimaDB;
GO

-- Tabla de localizaciones
CREATE TABLE Localizaciones (
    idLocalizacion INT IDENTITY(1,1) PRIMARY KEY,
    nombreCiudad NVARCHAR(100),
    pais NVARCHAR(50),
    latitud DECIMAL(9,6),
    longitud DECIMAL(9,6)
);

-- Tabla de registros de clima
CREATE TABLE DatosClima (
    idDato INT IDENTITY(1,1) PRIMARY KEY,
    idLocalizacion INT FOREIGN KEY REFERENCES Localizaciones(idLocalizacion),
    fechaHoraConsulta DATETIME NOT NULL, -- Fecha y hora de la consulta
    temperatura DECIMAL(5,2),
    humedad INT,
    descripcion NVARCHAR(100),
    velocidadViento DECIMAL(5,2),
    presion DECIMAL(7,2),
    visibilidad DECIMAL(7,2)
);
