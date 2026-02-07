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
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            // Aquí puedes añadir lógica para saber si el archivo fue modificado
        }

        private void guardarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Este es el primer botón de guardar (puedes dejarlo o borrarlo)
            MessageBox.Show("Hola");
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
                textBox1.Text = File.ReadAllText(archivoActual);
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
                File.WriteAllText(archivoActual, textBox1.Text);
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
                File.WriteAllText(archivoActual, textBox1.Text);
                this.Text = "Bloc de notas - " + Path.GetFileName(archivoActual);
            }
        }

        private void statusStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void toolStripButton4_Click(object sender, EventArgs e)
        {
            
            Form2 f = new Form2();
            f.ShowDialog();
        }
    }
}