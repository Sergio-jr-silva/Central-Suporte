using System;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using MySql.Data.MySqlClient;

namespace Central_Suporte
{
    public partial class CentralSuporte : Form
    {
        private string connectionString = "server=localhost;database=centralsuporte_db;uid=root;pwd=3477;";

        public CentralSuporte()
        {
            InitializeComponent();
        }

        private void btnFechar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btnMinimizar_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        private void CentralSuporte_Load(object sender, EventArgs e)
        {
            CarregarChamados();

            dataGridView1.CellPainting += Tabela_CellPainting;
            dataGridView1.CellContentClick += dataGridView1_CellContentClick;
        }

        private void CarregarChamados()
        {
            using (MySqlConnection conn = new MySqlConnection(connectionString))
            {
                try
                {
                    conn.Open();
                    string query = "SELECT id, titulo, prioridade, data, descricao FROM chamados";
                    MySqlDataAdapter adapter = new MySqlDataAdapter(query, conn);
                    DataTable dt = new DataTable();
                    adapter.Fill(dt);

                    dataGridView1.AutoGenerateColumns = false;
                    dataGridView1.Columns.Clear();

                    dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
                    {
                        HeaderText = "ID",
                        DataPropertyName = "id",
                        Name = "id"
                    });

                    dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
                    {
                        HeaderText = "Título",
                        DataPropertyName = "titulo",
                        Name = "titulo"
                    });

                    dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
                    {
                        HeaderText = "Prioridade",
                        DataPropertyName = "prioridade",
                        Name = "prioridade"
                    });

                    dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
                    {
                        HeaderText = "Data",
                        DataPropertyName = "data",
                        Name = "data"
                    });

                    dataGridView1.Columns.Add(new DataGridViewTextBoxColumn
                    {
                        HeaderText = "Descrição",
                        DataPropertyName = "descricao",
                        Name = "descricao"
                    });

                    // Botão "Ver" para abrir modal
                    var btnDescricao = new DataGridViewButtonColumn
                    {
                        HeaderText = "Resumo",
                        Name = "btnDescricao",
                        Text = "🔍 Ver",
                        UseColumnTextForButtonValue = true

                    };
                    dataGridView1.Columns.Add(btnDescricao);

                    dataGridView1.DataSource = dt;

                    // Oculta a coluna descrição na grid (só usamos o valor internamente)
                    dataGridView1.Columns["descricao"].Visible = false;
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Erro ao carregar chamados: " + ex.Message);
                }
            }
        }

        private void Tabela_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            var grid = (DataGridView)sender;
            if (!grid.Columns.Contains("prioridade")) return;

            int colIndex = grid.Columns["prioridade"].Index;

            if (e.RowIndex >= 0 && e.ColumnIndex == colIndex)
            {
                e.Handled = true;
                e.PaintBackground(e.CellBounds, true);

                string texto = e.FormattedValue?.ToString() ?? "";
                Color corFundo;
                Color corTexto = Color.White;

                switch (texto)
                {
                    case "Alta":
                        corFundo = Color.FromArgb(220, 53, 69); // vermelho
                        break;
                    case "Média":
                        corFundo = Color.FromArgb(255, 193, 7); // amarelo/laranja
                        corTexto = Color.Black;
                        break;
                    case "Baixa":
                        corFundo = Color.FromArgb(40, 167, 69); // verde
                        break;
                    default:
                        corFundo = Color.Gray;
                        break;
                }

                using (SolidBrush b = new SolidBrush(corFundo))
                {
                    Rectangle rect = new Rectangle(e.CellBounds.X + 10, e.CellBounds.Y + 5,
                        e.CellBounds.Width - 20, e.CellBounds.Height - 10);
                    e.Graphics.FillRectangle(b, rect);

                    using (Font fonte = new Font("Segoe UI", 9, FontStyle.Bold))
                    using (Brush brushTexto = new SolidBrush(corTexto))
                    using (StringFormat sf = new StringFormat
                    {
                        Alignment = StringAlignment.Center,
                        LineAlignment = StringAlignment.Center
                    })
                    {
                        e.Graphics.DrawString(texto.ToUpper(), fonte, brushTexto, rect, sf);
                    }
                }

                // Desenha a borda padrão
                ControlPaint.DrawBorder(e.Graphics, e.CellBounds, Color.LightGray, ButtonBorderStyle.Solid);
            }
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {
            if (dataGridView1.Columns[e.ColumnIndex].Name == "btnDescricao" && e.RowIndex >= 0)
            {
                string titulo = dataGridView1.Rows[e.RowIndex].Cells["titulo"].Value?.ToString() ?? "Detalhes";
                string descricao = dataGridView1.Rows[e.RowIndex].Cells["descricao"].Value?.ToString() ?? "";

                using (Form modal = new Form())
                {
                    modal.Text = $"Resumo - {titulo}";
                    modal.Size = new Size(400, 300);
                    modal.StartPosition = FormStartPosition.CenterParent;
                    modal.FormBorderStyle = FormBorderStyle.FixedDialog;
                    modal.MaximizeBox = false;
                    modal.MinimizeBox = false;

                    Label lbl = new Label()
                    {
                        Text = descricao,
                        AutoSize = false,
                        Font = new Font("Segoe UI", 10),
                        Padding = new Padding(10),
                        Dock = DockStyle.Fill
                    };

                    modal.Controls.Add(lbl);
                    modal.ShowDialog();
                }
            }
        }

        private void label3_Click(object sender, EventArgs e)
        {

        }

        private void iconButton2_Click(object sender, EventArgs e)
        {
            TabPags.SelectedTab = LimpezaManual;
        }

        private void iconButton1_Click(object sender, EventArgs e)
        {
            TabPags.SelectedTab = Chamados;
        }

        private void iconButton3_Click(object sender, EventArgs e)
        {
            TabPags.SelectedTab = LimpezaAuto;
        }

        private void iconButton4_Click(object sender, EventArgs e)
        {
            TabPags.SelectedTab = verificarRed;
        }

        private void iconButton5_Click(object sender, EventArgs e)
        {
            TabPags.SelectedTab = Status;
        }

        private void label15_Click(object sender, EventArgs e)
        {

        }
    }
}
