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

        public Form1()
        {
            InitializeComponent();
            ActualizarBarraEstado();

            // Suscribir eventos
            richTextBox1.SelectionChanged += richTextBox1_SelectionChanged;
            richTextBox1.TextChanged += richTextBox1_TextChanged;
        }

        // --- LÓGICA DE EMOJIS (IMÁGENES) ---
        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            // Desactivamos temporalmente para evitar que el reemplazo dispare el evento otra vez
            richTextBox1.TextChanged -= richTextBox1_TextChanged;

            int cursorOriginal = richTextBox1.SelectionStart;
            bool huboCambio = false;

            // Diccionario que vincula el texto con la imagen en Recursos
            var mapaImagenes = new Dictionary<string, Image>
            {
                { ":)", Properties.Resources.cara_feliz },
                { ":(", Properties.Resources.emoji_triste },
                { ":D", Properties.Resources.emoji_risa },
                { "<3", Properties.Resources.emoji_corazon },
            };

            foreach (var emoji in mapaImagenes)
            {
                int index = 0;
                while ((index = richTextBox1.Find(emoji.Key, index, RichTextBoxFinds.None)) != -1)
                {
                    richTextBox1.Select(index, emoji.Key.Length);

                    // Copiamos imagen al portapapeles y la pegamos en el RichTextBox
                    Clipboard.SetImage(emoji.Value);
                    richTextBox1.Paste();

                    huboCambio = true;
                }
            }

            if (huboCambio)
            {
                richTextBox1.SelectionStart = cursorOriginal;
                Clipboard.Clear(); // Limpiamos el portapapeles
            }

            richTextBox1.TextChanged += richTextBox1_TextChanged;
        }

        // --- BARRA DE ESTADO ---
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

        // --- ARCHIVO: ABRIR ---
        private void abrirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            ofd.Filter = "Documento Enriquecido|*.rtf|Archivo de texto|*.txt";

            if (ofd.ShowDialog() == DialogResult.OK)
            {
                archivoActual = ofd.FileName;
                if (Path.GetExtension(archivoActual).ToLower() == ".rtf")
                    richTextBox1.LoadFile(archivoActual);
                else
                    richTextBox1.Text = File.ReadAllText(archivoActual);

                this.Text = "Bloc de notas - " + Path.GetFileName(archivoActual);
            }
        }

        // --- ARCHIVO: GUARDAR ---
        private void guardarToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(archivoActual))
                EjecutarGuardarComo();
            else
                GuardarArchivoReal(archivoActual);
        }

        private void guardarComoToolStripMenuItem_Click(object sender, EventArgs e)
        {
            EjecutarGuardarComo();
        }

        private void EjecutarGuardarComo()
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Documento Enriquecido|*.rtf|Archivo de texto|*.txt";
            sfd.AddExtension = true;

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                archivoActual = sfd.FileName;
                GuardarArchivoReal(archivoActual);
                this.Text = "Bloc de notas - " + Path.GetFileName(archivoActual);
            }
        }

        private void GuardarArchivoReal(string ruta)
        {
            // Si es RTF guardamos con imágenes, si es TXT solo texto plano
            if (Path.GetExtension(ruta).ToLower() == ".rtf")
                richTextBox1.SaveFile(ruta, RichTextBoxStreamType.RichText);
            else
                File.WriteAllText(ruta, richTextBox1.Text);
        }

        // --- OTROS EVENTOS Y BOTONES ---
        private void salirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void toolStripButton1_Click(object sender, EventArgs e) => abrirToolStripMenuItem_Click(sender, e);
        private void toolStripButton2_Click(object sender, EventArgs e) => guardarToolStripMenuItem_Click_1(sender, e);
        private void toolStripButton3_Click(object sender, EventArgs e) => EjecutarGuardarComo();

        private void buscarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form2 f = new Form2(richTextBox1);
            f.Show();
        }

        private void toolStripButton4_Click(object sender, EventArgs e) => buscarToolStripMenuItem_Click(sender, e);

        // Métodos vacíos para evitar errores de diseño (CS1061)
        private void toolStripStatusLabel1_Click(object sender, EventArgs e) { }
        private void Form1_Load(object sender, EventArgs e) { }
        private void toolStripProgressBar1_Click(object sender, EventArgs e) { }
    }
}