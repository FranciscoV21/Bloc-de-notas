using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Bloc_de_notas
{
    public partial class Form1 : Form
    {
        string archivoActual = null;

        private readonly Dictionary<string, string> mapaEmojis = new Dictionary<string, string>
        {
            { ":)", "🙂" },
            { ":(", "☹️" },
            { ":D", "😀" },
            { ";)", "😉" },
            { "<3", "❤️" },
            { "(y)", "👍" }
        };
        public Form1()
        {
            InitializeComponent();
            // Configuración inicial de la barra de estado
            ActualizarBarraEstado();

            // Suscribir eventos por código para asegurar que funcionen
            richTextBox1.SelectionChanged += richTextBox1_SelectionChanged;
            richTextBox1.TextChanged += richTextBox1_TextChanged;
        }
        private void richTextBox1_SelectionChanged(object sender, EventArgs e)
        {
            ActualizarBarraEstado();
        }

        private void ActualizarBarraEstado()
        {
            int index = richTextBox1.SelectionStart;
            int linea = richTextBox1.GetLineFromCharIndex(index) + 1;
            int columna = index - richTextBox1.GetFirstCharIndexFromLine(linea - 1) + 1;

            toolStripStatusLabel1.Text = $"Línea: {linea}";
            toolStripStatusLabel2.Text = $"Columna: {columna}";
        }


        private void toolStripStatusLabel1_Click(object sender, EventArgs e)
        {
        }

        private void Form1_Load(object sender, EventArgs e)
        {
        }

        // --- FUNCIÓN ABRIR ---
        private void abrirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Documento de texto|*.txt";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                archivoActual = ofd.FileName;
                richTextBox1.Text = File.ReadAllText(archivoActual);
                this.Text = "Bloc de notas - " + Path.GetFileName(archivoActual);
            }
        }

        // --- FUNCIÓN GUARDAR ---
        private void guardarToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(archivoActual))
            {
                EjecutarGuardarComo();
            }
            else
            {
                File.WriteAllText(archivoActual, richTextBox1.Text);
            }
        }

        // --- FUNCIÓN GUARDAR COMO ---
        private void guardarComoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EjecutarGuardarComo();
        }

        // --- FUNCIÓN SALIR ---
        private void salirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        // --- MÉTODO AUXILIAR PARA GUARDAR COMO ---
        private void EjecutarGuardarComo()
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Documento de texto|*.txt";
            sfd.AddExtension = true;

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                archivoActual = sfd.FileName;
                File.WriteAllText(archivoActual, richTextBox1.Text);
                this.Text = "Bloc de notas - " + Path.GetFileName(archivoActual);
            }
        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            
            Form2 f = new Form2(richTextBox1);
            f.Show();
        }

        private void toolStripProgressBar1_Click(object sender, EventArgs e)
        {

        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            // Desactivamos el evento temporalmente para evitar recursividad infinita
            richTextBox1.TextChanged -= richTextBox1_TextChanged;

            int cursorPosition = richTextBox1.SelectionStart;
            bool cambioRealizado = false;

            foreach (var emoji in mapaEmojis)
            {
                if (richTextBox1.Text.Contains(emoji.Key))
                {
                    // Reemplazamos el texto por el emoji
                    richTextBox1.Text = richTextBox1.Text.Replace(emoji.Key, emoji.Value);
                    cambioRealizado = true;
                }
            }

            // Si hubo cambio, el cursor se mueve al inicio, hay que regresarlo
            if (cambioRealizado)
            {
                richTextBox1.SelectionStart = cursorPosition;
            }

            // Reactivamos el evento
            richTextBox1.TextChanged += richTextBox1_TextChanged;
        }

        private void toolStripButton1_Click(object sender, EventArgs e)
        {
            abrirToolStripMenuItem_Click(sender, e);
        }

        private void toolStripButton2_Click(object sender, EventArgs e)
        {
            guardarToolStripMenuItem_Click_1(sender,e);
        }

        private void toolStripButton3_Click(object sender, EventArgs e)
        {
            EjecutarGuardarComo();
        }

        private void buscarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form2 f = new Form2(richTextBox1);
            f.Show();
        }
    }
}