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
        // Diccionario para guardar el texto plano de cada pestaña
        private Dictionary<TabPage, string> _textoPlano = new Dictionary<TabPage, string>();

        public Form1()
        {
            InitializeComponent();
            tabControl1.TabPages.Clear();

            toolStripComboBox1.Items.Clear();

            for (int i = 8; i <= 72; i += 2)
            {
                toolStripComboBox1.Items.Add(i.ToString());
            }

            toolStripComboBox1.Text = "12";
            CrearNuevaPestaña("Sin título");
        }

        private RichTextBox GetEditorActual()
        {
            if (tabControl1.SelectedTab != null && tabControl1.SelectedTab.Controls.Count > 0)
            {
                return tabControl1.SelectedTab.Controls[0] as RichTextBox;
            }
            return null;
        }

        private void CrearNuevaPestaña(string titulo, string rutaArchivo = null)
        {
            TabPage nuevaPagina = new TabPage(titulo);
            nuevaPagina.Tag = rutaArchivo;

            RichTextBox nuevoRTB = new RichTextBox();
            nuevoRTB.Dock = DockStyle.Fill;
            nuevoRTB.BorderStyle = BorderStyle.None;
            nuevoRTB.AcceptsTab = true;

            nuevoRTB.TextChanged += richTextBox1_TextChanged;
            nuevoRTB.SelectionChanged += richTextBox1_SelectionChanged;

            nuevaPagina.Controls.Add(nuevoRTB);
            tabControl1.TabPages.Add(nuevaPagina);
            tabControl1.SelectedTab = nuevaPagina;
        }

        private void toolStripButton7_Click(object sender, EventArgs e)
        {
            CrearNuevaPestaña("Sin título");
        }

        private void toolStripButton5_Click(object sender, EventArgs e)
        {
            ColorDialog cd = new ColorDialog();
            if (cd.ShowDialog() == DialogResult.OK)
            {
                RichTextBox editor = GetEditorActual();
                if (editor != null) editor.SelectionColor = cd.Color;
            }
        }

        private void toolStripButton6_Click(object sender, EventArgs e)
        {
            ColorDialog cd = new ColorDialog();
            if (cd.ShowDialog() == DialogResult.OK)
            {
                RichTextBox editor = GetEditorActual();
                if (editor != null) editor.SelectionBackColor = cd.Color;
            }
        }

        private void toolStripComboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            AplicarTamañoLetra();
        }

        private void toolStripComboBox1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                AplicarTamañoLetra();
                e.SuppressKeyPress = true;
            }
        }

        private void AplicarTamañoLetra()
        {
            RichTextBox editor = GetEditorActual();
            if (editor == null || string.IsNullOrEmpty(toolStripComboBox1.Text)) return;

            if (float.TryParse(toolStripComboBox1.Text, out float nuevoTamaño))
            {
                if (nuevoTamaño < 1) nuevoTamaño = 8;
                if (nuevoTamaño > 100) nuevoTamaño = 100;

                Font fuenteReferencia = editor.SelectionFont ?? editor.Font;
                Font nuevaFont = new Font(fuenteReferencia.FontFamily, nuevoTamaño, fuenteReferencia.Style);
                editor.SelectionFont = nuevaFont;
                editor.Focus();
            }
        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            RichTextBox txt = sender as RichTextBox;
            if (txt == null) return;

            // Guardamos el texto plano ANTES de hacer cualquier reemplazo visual
            TabPage paginaActual = tabControl1.TabPages.Cast<TabPage>()
                .FirstOrDefault(p => p.Controls.Contains(txt));

            if (paginaActual != null)
                _textoPlano[paginaActual] = txt.Text;

            txt.TextChanged -= richTextBox1_TextChanged;

            int cursorOriginal = txt.SelectionStart;
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
                while ((index = txt.Find(emoji.Key, index, RichTextBoxFinds.None)) != -1)
                {
                    txt.Select(index, emoji.Key.Length);
                    Clipboard.SetImage(emoji.Value);
                    txt.Paste();
                    huboCambio = true;
                }
            }

            if (huboCambio)
            {
                txt.SelectionStart = cursorOriginal;
                Clipboard.Clear();
            }

            txt.TextChanged += richTextBox1_TextChanged;
        }

        private void abrirToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog { Filter = "Archivos soportados|*.txt" };
            if (ofd.ShowDialog() == DialogResult.OK)
            {
                CrearNuevaPestaña(Path.GetFileName(ofd.FileName), ofd.FileName);
                RichTextBox editor = GetEditorActual();

                if (Path.GetExtension(ofd.FileName).ToLower() == ".txt")
                    editor.LoadFile(ofd.FileName);
                else
                    editor.Text = File.ReadAllText(ofd.FileName);
            }
        }

        private void guardarToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            RichTextBox editor = GetEditorActual();
            if (editor == null) return;

            string ruta = tabControl1.SelectedTab.Tag as string;

            if (string.IsNullOrEmpty(ruta))
            {
                SaveFileDialog sfd = new SaveFileDialog { Filter = "Archivo de texto|*.txt" };
                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    ruta = sfd.FileName;
                    tabControl1.SelectedTab.Tag = ruta;
                    tabControl1.SelectedTab.Text = Path.GetFileName(ruta);
                }
                else return;
            }

            // Usamos el texto plano guardado, no el contenido del RTB
            string contenidoAGuardar = _textoPlano.ContainsKey(tabControl1.SelectedTab)
                ? _textoPlano[tabControl1.SelectedTab]
                : editor.Text;

            File.WriteAllText(ruta, contenidoAGuardar);
        }

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

        private void toolStripButton1_Click(object sender, EventArgs e) => abrirToolStripMenuItem_Click(sender, e);
        private void toolStripButton2_Click(object sender, EventArgs e) => guardarToolStripMenuItem_Click_1(sender, e);
        private void toolStripButton3_Click(object sender, EventArgs e) { tabControl1.SelectedTab.Tag = null; guardarToolStripMenuItem_Click_1(sender, e); }
        private void salirToolStripMenuItem_Click(object sender, EventArgs e) => Application.Exit();
        private void toolStripButton4_Click(object sender, EventArgs e) { Form2 f = new Form2(GetEditorActual()); f.Show(); }
        private void buscarToolStripMenuItem_Click(object sender, EventArgs e) => toolStripButton4_Click(sender, e);

        private void toolStripStatusLabel1_Click(object sender, EventArgs e) { }
        private void Form1_Load(object sender, EventArgs e) { }
        private void toolStripProgressBar1_Click(object sender, EventArgs e) { }
        private void toolStripComboBox1_Click(object sender, EventArgs e) { }
        private void tabPage1_Click(object sender, EventArgs e) { }

        private void toolStripButton8_Click(object sender, EventArgs e)
        {
            if (tabControl1.TabPages.Count > 0)
            {
                DialogResult res = MessageBox.Show("¿Seguro que quieres cerrar esta pestaña?", "Cerrar", MessageBoxButtons.YesNo);
                if (res == DialogResult.Yes)
                {
                    // Limpiamos el diccionario al cerrar la pestaña
                    _textoPlano.Remove(tabControl1.SelectedTab);
                    tabControl1.TabPages.Remove(tabControl1.SelectedTab);
                }
            }

            if (tabControl1.TabPages.Count == 0)
            {
                CrearNuevaPestaña("Sin título");
            }
        }

        private void toolStripComboBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }
    }
}