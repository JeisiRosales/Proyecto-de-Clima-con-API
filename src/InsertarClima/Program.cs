using System;
using System.Data.SqlClient;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq; // Instalar con: dotnet add package Newtonsoft.Json
using System.Text.Json;

class Program
{
    static async Task Main(string[] args)
    {
        //Valores de coexion
        string connectionString = "Server=JEISI;Database=ClimaDB;Trusted_Connection=True;";
        string apiKey = "CCS6THRQ44UK724JLCDG29YZ8";
        List<string> ciudades = new List<string>()
        {
            "Amazonas, VE",
            "Anzoategui, VE",
            "Apure, VE",
            "Aragua, VE",
            "Barinas, VE",
            "Bolivar, VE",
            "Carabobo, VE",
            "Cojedes, VE",
            "Delta Amacuro, VE",
            "Falcon, VE",
            "Guarico, VE",
            "Lara, VE",
            "Merida, VE",
            "Miranda, VE",
            "Monagas, VE",
            "Nueva Esparta, VE",
            "Portuguesa, VE",
            "Sucre, VE",
            "Tachira, VE",
            "Trujillo, VE",
            "Vargas, VE",
            "Yaracuy, VE",
            "Zulia, VE",
            "Caracas, VE"
        };

        foreach (string ciudad in ciudades) 
        {
            // Endpoint de la API
            string url = $"https://weather.visualcrossing.com/VisualCrossingWebServices/rest/services/timeline/{ciudad}/today?unitGroup=metric&key={apiKey}&include=hours";

            using (HttpClient client = new HttpClient())
            {
                // Llamada a la API
                HttpResponseMessage response = await client.GetAsync(url);
                string json = await response.Content.ReadAsStringAsync();

                // Parsear JSON
                JObject data = JObject.Parse(json);

                // Tomamos la hora actual y consultamos los datos
                // hora actual
                int horaActual = DateTime.Now.Hour;
                var horasDelDia = data["days"][0]["hours"];
                var hora = horasDelDia[horaActual];

                // guardamos los datos 
                decimal temp = (decimal)hora["temp"];
                int humedad = (int)hora["humidity"];
                string desc = (string)hora["conditions"];
                decimal viento = (decimal)hora["windspeed"];
                decimal presion = (decimal)hora["pressure"];
                decimal visibilidad = (decimal)hora["visibility"];
                decimal latitud = (decimal)data["latitude"];
                decimal longitud = (decimal)data["longitude"];

                // Insertar en SQL Server
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    // Verificar si la ciudad ya existe en Localizaciones
                    SqlCommand cmdCheck = new SqlCommand("SELECT idLocalizacion FROM Localizaciones WHERE nombreCiudad=@ciudad", conn);
                    cmdCheck.Parameters.AddWithValue("@ciudad", ciudad.Split(',')[0]);
                    var idLocalizacionObj = cmdCheck.ExecuteScalar();
                    int idLocalizacion;

                    if (idLocalizacionObj == null)
                    {
                        SqlCommand cmdInsertLoc = new SqlCommand(
                            "INSERT INTO Localizaciones (nombreCiudad, pais, latitud, longitud) VALUES (@ciudad, @pais, @latitud, @longitud); SELECT SCOPE_IDENTITY();", conn);
                        cmdInsertLoc.Parameters.AddWithValue("@ciudad", ciudad.Split(',')[0]);
                        cmdInsertLoc.Parameters.AddWithValue("@pais", ciudad.Split(',')[1]);
                        cmdInsertLoc.Parameters.AddWithValue("@latitud", latitud);
                        cmdInsertLoc.Parameters.AddWithValue("@longitud", longitud);
                        idLocalizacion = Convert.ToInt32(cmdInsertLoc.ExecuteScalar());
                    }
                    else
                    {
                        idLocalizacion = Convert.ToInt32(idLocalizacionObj);
                    }

                    // Insertar datos del clima
                    SqlCommand cmdInsert = new SqlCommand(
                        "INSERT INTO DatosClima (idLocalizacion, fechaHoraConsulta, temperatura, humedad, descripcion, velocidadViento, presion, visibilidad) " +
                        "VALUES (@idLoc, @fechaHora, @temp, @humedad, @desc, @viento, @presion, @visibilidad)", conn);

                    cmdInsert.Parameters.AddWithValue("@idLoc", idLocalizacion);
                    cmdInsert.Parameters.AddWithValue("@fechaHora", DateTime.Now);
                    cmdInsert.Parameters.AddWithValue("@temp", temp);
                    cmdInsert.Parameters.AddWithValue("@humedad", humedad);
                    cmdInsert.Parameters.AddWithValue("@desc", desc);
                    cmdInsert.Parameters.AddWithValue("@viento", viento);
                    cmdInsert.Parameters.AddWithValue("@presion", presion);
                    cmdInsert.Parameters.AddWithValue("@visibilidad", visibilidad);

                    cmdInsert.ExecuteNonQuery();
                }

                // generar un .json con los datos actuales  
                string dataDir = @"C:\Users\Jeisi Rosales\Desktop\Proyecto-de-Clima-con-API\docs\data";
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string queryActual = @"
                        SELECT 
                            l.nombreCiudad,
                            l.pais,
                            c.fechaHoraConsulta,
                            c.temperatura,
                            c.humedad,
                            c.velocidadViento,
                            c.presion
                        FROM DatosClima c
                        JOIN Localizaciones l ON c.idLocalizacion = l.idLocalizacion
                        WHERE c.fechaHoraConsulta IN (
                            SELECT MAX(fechaHoraConsulta)
                            FROM DatosClima
                            GROUP BY idLocalizacion
                        )
                        ORDER BY l.nombreCiudad;
                    ";

                    SqlCommand cmd = new SqlCommand(queryActual, conn);
                    SqlDataReader reader = cmd.ExecuteReader();

                    var listaActual = new List<object>();
                    while (reader.Read())
                    {
                        listaActual.Add(new
                        {
                            Ciudad = reader["nombreCiudad"].ToString(),
                            Pais = reader["pais"].ToString(),
                            FechaHora = Convert.ToDateTime(reader["fechaHoraConsulta"]),
                            Temperatura = Convert.ToDecimal(reader["temperatura"]),
                            Humedad = Convert.ToInt32(reader["humedad"]),
                            Viento = Convert.ToDecimal(reader["velocidadViento"]),
                            Presion = Convert.ToDecimal(reader["presion"])
                        });
                    }
                    reader.Close();

                    string jsonActual = JsonSerializer.Serialize(listaActual, new JsonSerializerOptions { WriteIndented = true });
                    File.WriteAllText(Path.Combine(dataDir, "clima_actual.json"), jsonActual);
                }

                // genera un .json con un historico de datos de 7 dias
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();

                    string queryHistorico = @"
                        SELECT 
                            l.nombreCiudad,
                            l.pais,
                            c.fechaHoraConsulta,
                            c.temperatura,
                            c.humedad,
                            c.velocidadViento,
                            c.presion
                        FROM DatosClima c
                        JOIN Localizaciones l ON c.idLocalizacion = l.idLocalizacion
                        WHERE c.fechaHoraConsulta >= DATEADD(DAY, -7, GETDATE())
                        ORDER BY l.nombreCiudad, c.fechaHoraConsulta;
                    ";

                    SqlCommand cmdHist = new SqlCommand(queryHistorico, conn);
                    SqlDataReader readerHist = cmdHist.ExecuteReader();

                    var listaHist = new List<object>();
                    while (readerHist.Read())
                    {
                        listaHist.Add(new
                        {
                            Ciudad = readerHist["nombreCiudad"].ToString(),
                            Pais = readerHist["pais"].ToString(),
                            FechaHora = Convert.ToDateTime(readerHist["fechaHoraConsulta"]),
                            Temperatura = Convert.ToDecimal(readerHist["temperatura"]),
                            Humedad = Convert.ToInt32(readerHist["humedad"]),
                            Viento = Convert.ToDecimal(readerHist["velocidadViento"]),
                            Presion = Convert.ToDecimal(readerHist["presion"])
                        });
                    }
                    readerHist.Close();

                    string jsonHistorico = JsonSerializer.Serialize(listaHist, new JsonSerializerOptions { WriteIndented = true });
                    File.WriteAllText(Path.Combine(dataDir, "clima_historico.json"), jsonHistorico);
                }
            }
        }
    }
}
