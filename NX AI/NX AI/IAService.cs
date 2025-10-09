using System;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace NX_AI
{
    public class IAService
    {
        private const string apiUrl = "http://192.168.170.1:1234/v1/responses";
        private string reglas; // Almacenará las reglas cargadas

        // 🔹 Constructor: carga las reglas al crear la instancia
        public IAService()
        {
            reglas = CargarReglas();
        }

        // 🔹 Función que lee el archivo de reglas
        private string CargarReglas()
        {
            string rutaArchivo = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "reglas.txt");

            if (File.Exists(rutaArchivo))
            {
                return File.ReadAllText(rutaArchivo);
            }
            else
            {
                return "Eres un asistente virtual. No se encontró el archivo de reglas.";
            }
        }

        // 🔹 Método principal para obtener respuesta
        public async Task<string> ObtenerRespuestaDeIA(string pregunta)
        {
            using (HttpClient client = new HttpClient())
            {
                var jinput = new[]
                {
                    new
                    {
                        role = "system",
                        content = reglas// 👈 usamos el texto leído desde reglas.txt
                    },
                    new
                    {
                        role = "user",
                        content = pregunta
                    }
                };

                var data = new
                {
                    model = "openai/gpt-oss-20b",
                    input = jinput
                };

                string jsonContent = JsonConvert.SerializeObject(data);
                StringContent content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync(apiUrl, content);
                string responseBody = await response.Content.ReadAsStringAsync();

                dynamic responseJson = JsonConvert.DeserializeObject(responseBody);
                return responseJson.output[1].content[0].text.ToString().Trim();
            }
        }
    }
}
