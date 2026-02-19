using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.IO;

namespace Bloc_de_notas
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            tabControl1.TabPages.Clear();

            // Limpiamos por si acaso el diseño tiene números raros
            toolStripComboBox1.Items.Clear();

            // Llenar tamaños de letra: de 8 (chico) a 72 (grande)
            for (int i = 8; i <= 72; i += 2)
            {
                toolStripComboBox1.Items.Add(i.ToString());
            }

            toolStripComboBox1.Text = "12";
            CrearNuevaPestaña("Sin título");
        }

        // --- FUNCIÓN PARA OBTENER EL EDITOR DE LA PESTAÑA SELECCIONADA ---
        private RichTextBox GetEditorActual()
        {
            if (tabControl1.SelectedTab != null && tabControl1.SelectedTab.Controls.Count > 0)
            {
                return tabControl1.SelectedTab.Controls[0] as RichTextBox;
            }
            return null;
        }

        // --- FUNCIÓN PARA CREAR PESTAÑAS DINÁMICAMENTE ---
        private void CrearNuevaPestaña(string titulo, string rutaArchivo = null)
        {
            TabPage nuevaPagina = new TabPage(titulo);
            nuevaPagina.Tag = rutaArchivo; // Guardamos la ruta aquí para saber dónde guardar después

            RichTextBox nuevoRTB = new RichTextBox();
            nuevoRTB.Dock = DockStyle.Fill;
            nuevoRTB.BorderStyle = BorderStyle.None;
            nuevoRTB.AcceptsTab = true;

            // Suscribimos los eventos al nuevo editor
            nuevoRTB.TextChanged += richTextBox1_TextChanged;
            nuevoRTB.SelectionChanged += richTextBox1_SelectionChanged;

            nuevaPagina.Controls.Add(nuevoRTB);
            tabControl1.TabPages.Add(nuevaPagina);
            tabControl1.SelectedTab = nuevaPagina; // Enfocar la nueva pestaña
        }

        // --- BOTÓN 7: NUEVA PESTAÑA ---
        private void toolStripButton7_Click(object sender, EventArgs e)
        {
            CrearNuevaPestaña("Sin título");
        }

        // --- BOTÓN 5: COLOR DE LETRA ---
        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            ColorDialog cd = new ColorDialog();
            if (cd.ShowDialog() == DialogResult.OK)
            {
                RichTextBox editor = GetEditorActual();
                if (editor != null) editor.SelectionColor = cd.Color;
            }
        }

        // --- BOTÓN 6: COLOR DE FONDO (RESALTADO) ---
        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            ColorDialog cd = new ColorDialog();
            if (cd.ShowDialog() == DialogResult.OK)
            {
                RichTextBox editor = GetEditorActual();
                if (editor != null) editor.SelectionBackColor = cd.Color;
            }
        }

        // --- COMBOBOX: TAMAÑO DE LETRA ---
        // Asegúrate de que este evento esté vinculado a "SelectedIndexChanged" en el diseñador
        private void toolStripComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            AplicarTamañoLetra();
        }
        private void toolStripComboBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                AplicarTamañoLetra();
                e.SuppressKeyPress = true; // Evita el "beep" de Windows al dar Enter
            }
        }
        private void AplicarTamañoLetra()
        {
            RichTextBox editor = GetEditorActual();
            if (editor == null || string.IsNullOrEmpty(toolStripComboBox1.Text)) return;

            if (float.TryParse(toolStripComboBox1.Text, out float nuevoTamaño))
            {
                // Límites lógicos: Menos de 1 es invisible, más de 100 es gigante
                if (nuevoTamaño < 1) nuevoTamaño = 8;
                if (nuevoTamaño > 100) nuevoTamaño = 100;

                // Obtenemos la familia de fuente actual (ej. Arial) y el estilo (ej. Negrita)
                Font fuenteReferencia = editor.SelectionFont ?? editor.Font;

                // Creamos la nueva fuente
                Font nuevaFont = new Font(fuenteReferencia.FontFamily, nuevoTamaño, fuenteReferencia.Style);

                // APLICAR:
                editor.SelectionFont = nuevaFont;

                // Esto hace que lo que escribas después también tenga ese tamaño
                editor.Focus();
            }
        }

        // --- LÓGICA DE EMOJIS (DINÁMICA) ---
        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            RichTextBox rtb = sender as RichTextBox; // Usamos el sender para saber qué pestaña cambió
            if (rtb == null) return;

            rtb.TextChanged -= richTextBox1_TextChanged;

            int cursorOriginal = rtb.SelectionStart;
            bool huboCambio = false;

            var mapaImagenes = new Dictionary<string, Image>
            {
                { ":)", Properties.Resources.cara_feliz },
                { ":(", Properties.Resources.emoji_triste },
                { ":D", Properties.Resources.emoji_risa },
                { "<3", Properties.Resources.emoji_corazon },
            };

            foreach (var emoji in mapaImagenes)
            {
                if (emoji.Value == null) continue;
                int index = 0;
                while ((index = rtb.Find(emoji.Key, index, RichTextBoxFinds.None)) != -1)
                {
                    rtb.Select(index, emoji.Key.Length);
                    Clipboard.SetImage(emoji.Value);
                    rtb.Paste();
                    huboCambio = true;
                }
            }

            if (huboCambio)
            {
                rtb.SelectionStart = cursorOriginal;
                Clipboard.Clear();
            }

            rtb.TextChanged += richTextBox1_TextChanged;
        }

        // --- ARCHIVO: ABRIR ---
        private void abrirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog { Filter = "Archivos soportados|*.rtf;*.txt" };
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                CrearNuevaPestaña(Path.GetFileName(ofd.FileName), ofd.FileName);
                RichTextBox editor = GetEditorActual();

                if (Path.GetExtension(ofd.FileName).ToLower() == ".rtf")
                    editor.LoadFile(ofd.FileName);
                else
                    editor.Text = File.ReadAllText(ofd.FileName);
            }
        }

        // --- ARCHIVO: GUARDAR ---
        private void guardarToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            RichTextBox editor = GetEditorActual();
            if (editor == null) return;

            string ruta = tabControl1.SelectedTab.Tag as string;

            if (string.IsNullOrEmpty(ruta))
            {
                SaveFileDialog sfd = new SaveFileDialog { Filter = "Documento RTF|*.rtf|Archivo de texto|*.txt" };
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    ruta = sfd.FileName;
                    tabControl1.SelectedTab.Tag = ruta;
                    tabControl1.SelectedTab.Text = Path.GetFileName(ruta);
                }
                else return;
            }

            if (Path.GetExtension(ruta).ToLower() == ".rtf")
                editor.SaveFile(ruta, RichTextBoxStreamType.RichText);
            else
                File.WriteAllText(ruta, editor.Text);
        }

        // --- BARRA DE ESTADO ---
        private void richTextBox1_SelectionChanged(object sender, EventArgs e)
        {
            ActualizarBarraEstado();
        }

        private void ActualizarBarraEstado()
        {
            RichTextBox rtb = GetEditorActual();
            if (rtb == null) return;

            int index = rtb.SelectionStart;
            int linea = rtb.GetLineFromCharIndex(index) + 1;
            int columna = index - rtb.GetFirstCharIndexFromLine(linea - 1) + 1;

            toolStripStatusLabel1.Text = $"Línea: {linea}";
            toolStripStatusLabel2.Text = $"Columna: {columna}";
        }

        // --- OTROS EVENTOS ---
        private void toolStripButton1_Click(object sender, EventArgs e) => abrirToolStripMenuItem_Click(sender, e);
        private void toolStripButton2_Click(object sender, EventArgs e) => guardarToolStripMenuItem_Click_1(sender, e);
        private void toolStripButton3_Click(object sender, EventArgs e) { tabControl1.SelectedTab.Tag = null; guardarToolStripMenuItem_Click_1(sender, e); }
        private void salirToolStripMenuItem_Click(object sender, EventArgs e) => Application.Exit();
        private void toolStripButton4_Click(object sender, EventArgs e) { Form2 f = new Form2(GetEditorActual()); f.Show(); }
        private void buscarToolStripMenuItem_Click(object sender, EventArgs e) => toolStripButton4_Click(sender, e);

        // Métodos vacíos para que no truene el Designer
        private void toolStripStatusLabel1_Click(object sender, EventArgs e) { }
        private void Form1_Load(object sender, EventArgs e) { }
        private void toolStripProgressBar1_Click(object sender, EventArgs e) { }
        private void toolStripComboBox1_Click(object sender, EventArgs e) { }
        private void tabPage1_Click(object sender, EventArgs e) { }
        private void toolStripButton8_Click(object sender, EventArgs e)
        {
            // Verificamos que haya al menos una pestaña para cerrar
            if (tabControl1.TabPages.Count > 0)
            {
                // Confirmación opcional (puedes quitarla si prefieres que sea instantáneo)
                DialogResult res = MessageBox.Show("¿Seguro que quieres cerrar esta pestaña?", "Cerrar", MessageBoxButtons.YesNo);
                if (res == DialogResult.Yes)
                {
                    tabControl1.TabPages.Remove(tabControl1.SelectedTab);
                }
            }

            // Si cerramos todas, abrimos una nueva vacía para que el programa no quede "muerto"
            if (tabControl1.TabPages.Count == 0)
            {
                CrearNuevaPestaña("Sin título");
            }
        }

        private void toolStripComboBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            // Solo permite números y la tecla de borrar (backspace)
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }
    }
}