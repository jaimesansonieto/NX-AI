using System;
using System.Drawing;
using System.Windows.Forms;

namespace NX_AI
{
    public partial class Form1 : Form
    {
        private TextBox txtPregunta;
        private Button btnEnviar;
        private RichTextBox txtRespuesta;
        private IAService iaService;

        public Form1()
        {
            // Solo un constructor aquí
            InitializeComponent(); // <-- obligatorio porque existe el Designer
            iaService = new IAService();
            InicializarInterfaz(); // <-- tu interfaz personalizada
        }

        private void InicializarInterfaz()
        {
            // Puedes usar tus propios controles
            this.Text = "NX AI - Interfaz básica";
            this.Size = new Size(800, 500);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(245, 245, 245);

            txtPregunta = new TextBox
            {
                Multiline = false,
                Font = new Font("Segoe UI", 11, FontStyle.Italic),
                Location = new Point(30, 30),
                Size = new Size(600, 30)
            };

            btnEnviar = new Button
            {
                Text = "Preguntar",
                Font = new Font("Segoe UI", 10),
                Location = new Point(650, 30),
                Size = new Size(100, 30)
            };
            btnEnviar.Click += BtnEnviar_Click;

            txtRespuesta = new RichTextBox
            {
                Font = new Font("Segoe UI", 11),
                Location = new Point(30, 80),
                Size = new Size(720, 350),
                ReadOnly = true,
                ScrollBars = RichTextBoxScrollBars.Vertical
            };

            this.Controls.Add(txtPregunta);
            this.Controls.Add(btnEnviar);
            this.Controls.Add(txtRespuesta);
            this.AcceptButton = btnEnviar;
        }

        private async void BtnEnviar_Click(object sender, EventArgs e)
        {
            btnEnviar.Enabled = false;
            string pregunta = txtPregunta.Text;
            string respuesta = await iaService.ObtenerRespuestaDeIA(pregunta);

            txtPregunta.Clear();

            txtRespuesta.SelectionFont = new Font(txtRespuesta.Font, FontStyle.Italic);
            txtRespuesta.AppendText(pregunta + "\r\n");

            txtRespuesta.SelectionFont = new Font(txtRespuesta.Font, FontStyle.Regular);
            txtRespuesta.AppendText("Respuesta de la IA:\r\n");

            txtRespuesta.SelectionFont = new Font(txtRespuesta.Font, FontStyle.Bold);
            txtRespuesta.AppendText(respuesta + "\r\n\r\n");

            btnEnviar.Enabled = true;
        }
    }
}
