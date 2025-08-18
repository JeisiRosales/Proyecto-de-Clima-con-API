using System;
using System.Data.SqlClient;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq; // Instalar con: dotnet add package Newtonsoft.Json

class Program
{
    static async Task Main(string[] args)
    {
        //Valores de coexion
        string connectionString = "Server=JEISI;Database=ClimaDB;Trusted_Connection=True;";
        string apiKey = "CCS6THRQ44UK724JLCDG29YZ8";
        string ciudad = "Caracas,VE";

        // Endpoint de la API
        string url = $"https://weather.visualcrossing.com/VisualCrossingWebServices/rest/services/timeline/{ciudad}/today?unitGroup=metric&key={apiKey}&include=hours";

        using (HttpClient client = new HttpClient())
        {
            // Llamada a la API
            HttpResponseMessage response = await client.GetAsync(url);
            string json = await response.Content.ReadAsStringAsync();

            // Parsear JSON
            JObject data = JObject.Parse(json);

            // Tomamos la primera hora del día como prueba
            var hora = data["days"][0]["hours"][0];
            decimal temp = (decimal)hora["temp"];
            int humedad = (int)hora["humidity"];
            string desc = (string)hora["conditions"];
            decimal viento = (decimal)hora["windspeed"];
            decimal presion = (decimal)hora["pressure"];
            decimal visibilidad = (decimal)hora["visibility"];

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
                        "INSERT INTO Localizaciones (nombreCiudad, pais) VALUES (@ciudad, @pais); SELECT SCOPE_IDENTITY();", conn);
                    cmdInsertLoc.Parameters.AddWithValue("@ciudad", ciudad.Split(',')[0]);
                    cmdInsertLoc.Parameters.AddWithValue("@pais", ciudad.Split(',')[1]);
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
        }
    }
}
