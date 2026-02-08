using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Bloc_de_notas
{
    public partial class Form2 : Form
    {
        // Variable para guardar la referencia al cuadro de texto del Form1
        RichTextBox cuadroTextoPrincipal;

        // Modificamos el constructor para recibir el RichTextBox
        public Form2(RichTextBox rtb)
        {
            InitializeComponent();
            cuadroTextoPrincipal = rtb;

            // Valores por defecto para que no truene
            radioButton2.Checked = true; // Buscar hacia abajo por defecto
        }

        // --- BOTÓN BUSCAR SIGUIENTE ---
        private void button1_Click(object sender, EventArgs e)
        {
            string palabra = richTextBox1.Text;
            if (string.IsNullOrEmpty(palabra)) return;

            // Configuramos las opciones de búsqueda
            RichTextBoxFinds opciones = RichTextBoxFinds.None;

            if (checkBox1.Checked) // "Coincidir mayúsculas y minúsculas"
                opciones |= RichTextBoxFinds.MatchCase;

            if (radioButton1.Checked) // "Hacia arriba"
                opciones |= RichTextBoxFinds.Reverse;

            int resultado = -1;
            int inicioBusqueda = 0;

            if (radioButton2.Checked) // BUSCAR HACIA ABAJO
            {
                // Empezamos a buscar justo después de donde está el cursor actualmente
                inicioBusqueda = cuadroTextoPrincipal.SelectionStart + cuadroTextoPrincipal.SelectionLength;
                resultado = cuadroTextoPrincipal.Find(palabra, inicioBusqueda, opciones);
            }
            else // BUSCAR HACIA ARRIBA
            {
                // Buscamos desde el inicio hasta la posición actual del cursor
                inicioBusqueda = cuadroTextoPrincipal.SelectionStart;
                resultado = cuadroTextoPrincipal.Find(palabra, 0, inicioBusqueda, opciones);
            }

            if (resultado != -1)
            {

                cuadroTextoPrincipal.Focus();
            }
            else
            {
                MessageBox.Show("No se encontró: " + palabra, "Buscador", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        // --- BOTÓN CANCELAR / CERRAR ---
        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void textBox1_TextChanged(object sender, EventArgs e) { }
        private void radioButton1_CheckedChanged(object sender, EventArgs e) { }
        private void radioButton2_CheckedChanged(object sender, EventArgs e) { }
        private void checkBox1_CheckedChanged(object sender, EventArgs e) { }
        private void checkBox2_CheckedChanged(object sender, EventArgs e) { }
    }
}