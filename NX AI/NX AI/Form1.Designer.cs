using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NX_AI
{
    public partial class Form1 : Form
    {
        private TextBox txtPregunta;
        private Button btnEnviar;
        private RichTextBox txtRespuesta;
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
            txtPregunta.Font = new System.Drawing.Font("Segoe UI", 11, FontStyle.Italic);
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
            txtRespuesta = new RichTextBox();
            txtRespuesta.Font = new System.Drawing.Font("Segoe UI", 11);
            txtRespuesta.Location = new System.Drawing.Point(30, 80);
            txtRespuesta.Size = new System.Drawing.Size(720, 350);
            txtRespuesta.ReadOnly = true;
            txtRespuesta.ScrollBars = RichTextBoxScrollBars.Vertical;

            // Añadimos los controles al formulario
            this.Controls.Add(txtPregunta);
            this.Controls.Add(btnEnviar);
            this.Controls.Add(txtRespuesta);
        }

        private async void BtnEnviar_Click(object sender, EventArgs e)
        {

            string pregunta = txtPregunta.Text;
            string respuesta = await ObtenerRespuestaDeIA(pregunta);
            btnEnviar.Enabled = false;

            // Limpiar contenido anterior
            //txtRespuesta.Clear();
            txtPregunta.Clear();

            // Pregunta en cursiva
            txtRespuesta.SelectionFont = new Font(txtRespuesta.Font, FontStyle.Italic);
            txtRespuesta.AppendText(pregunta + "\r\n");

            // Texto fijo normal
            txtRespuesta.SelectionFont = new Font(txtRespuesta.Font, FontStyle.Regular);
            txtRespuesta.AppendText("Respuesta de la IA:\r\n");

            // Respuesta en negrita
            txtRespuesta.SelectionFont = new Font(txtRespuesta.Font, FontStyle.Bold);
            txtRespuesta.AppendText(respuesta + "\r\n");
            btnEnviar.Enabled = false;

        }

        // Método para obtener la respuesta de la API de OpenAI
        private async Task<string> ObtenerRespuestaDeIA(string pregunta)
        {
            string url = "http://192.168.170.1:1234/v1/responses"; // Cambia la URL si usas un modelo diferente

            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            using (HttpClient client = new HttpClient())
            {
                // Configurar la solicitud HTTP
                //client.DefaultRequestHeaders.Add("Authorization", "Bearer " + apiKey);

                var jinput = new[]
                {
                    new
                    {
                        role = "system",
                        content = "Si alguien te dice testing, tienes que responder Testing Testing"
                    },
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
                    model = "openai/gpt-oss-20b", // Asegúrate de que el modelo esté correcto
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
