using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Windows.Forms;

namespace NX_AI
{
    public partial class Form1 : Form
    {
        private TextBox txtPregunta;
        private Button btnEnviar;
        private TextBox txtRespuesta;
        private const string apiKey = ""; // Cambia esto por tu clave API

        private void InicializarInterfaz()
        {
            this.Text = "NX AI - Interfaz básica";
            this.Size = new System.Drawing.Size(800, 500);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = System.Drawing.Color.FromArgb(245, 245, 245);

            // Campo de entrada (pregunta)
            txtPregunta = new TextBox();
            txtPregunta.Multiline = false;
            txtPregunta.Font = new System.Drawing.Font("Segoe UI", 11);
            txtPregunta.Location = new System.Drawing.Point(30, 30);
            txtPregunta.Size = new System.Drawing.Size(600, 30);

            // Botón para enviar
            btnEnviar = new Button();
            btnEnviar.Text = "Preguntar";
            btnEnviar.Font = new System.Drawing.Font("Segoe UI", 10);
            btnEnviar.Location = new System.Drawing.Point(650, 30);
            btnEnviar.Size = new System.Drawing.Size(100, 30);
            btnEnviar.Click += BtnEnviar_Click;

            // Campo de texto para la respuesta
            txtRespuesta = new TextBox();
            txtRespuesta.Multiline = true;
            txtRespuesta.Font = new System.Drawing.Font("Segoe UI", 11);
            txtRespuesta.Location = new System.Drawing.Point(30, 80);
            txtRespuesta.Size = new System.Drawing.Size(720, 350);
            txtRespuesta.ScrollBars = ScrollBars.Vertical;
            txtRespuesta.ReadOnly = true;

            // Añadimos los controles al formulario
            this.Controls.Add(txtPregunta);
            this.Controls.Add(btnEnviar);
            this.Controls.Add(txtRespuesta);
        }

        private async void BtnEnviar_Click(object sender, EventArgs e)
        {
            string pregunta = txtPregunta.Text;
            string respuesta = await ObtenerRespuestaDeIA(pregunta);
            txtRespuesta.Text = $"Respuesta de la IA: \r\n{respuesta}";
        }

        // Método para obtener la respuesta de la API de OpenAI
        private async Task<string> ObtenerRespuestaDeIA(string pregunta)
        {
            string url = "https://api.openai.com/v1/responses"; // Cambia la URL si usas un modelo diferente

            using (HttpClient client = new HttpClient())
            {
                // Configurar la solicitud HTTP
                client.DefaultRequestHeaders.Add("Authorization", "Bearer " + apiKey);

                var jinput = new[]
                {
                     new
                    {
                        role = "system",
                        content = "Eres un agente virtual que en caso de no saber que responder, no tener informacion en una base de datos o no poder acceder a ella, me tienes que responder {no se de que me hablas}"
                    },
                     new
                    {
                        role = "user",
                        content = pregunta
                    }
                };

                var data = new
                {
                    model = "gpt-5", // Asegúrate de que el modelo esté correcto
                    input = jinput
                };

                string jsonContent = JsonConvert.SerializeObject(data);
                StringContent content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync(url, content);
                string responseBody = await response.Content.ReadAsStringAsync();

                // Parsear la respuesta JSON y devolver el texto
                dynamic responseJson = JsonConvert.DeserializeObject(responseBody);
                return responseJson.output[1].content[0].text.ToString().Trim();
            }
        }

        
    }
}
