using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using HomeworkManager.Models;
using HomeworkManager.Services;

namespace HomeworkManager.Forms
{
    public partial class MainForm : Form
    {
        private readonly HomeworkService _service;
        private string _selectedId = null;
        private bool _isLoading = false;
        private bool _isDarkMode = false;

        // ── 色票 ──────────────────────────────────────────────────────
        // 淺色
        private static readonly Color LightBg = Color.FromArgb(245, 247, 250);
        private static readonly Color LightPanel = Color.White;
        private static readonly Color LightFore = Color.Black;
        private static readonly Color LightLabel = Color.FromArgb(30, 80, 160);
        private static readonly Color LightStats = Color.FromArgb(80, 80, 80);
        private static readonly Color LightGridHdr = Color.FromArgb(30, 80, 160);
        private static readonly Color LightGridHdrFg = Color.White;
        private static readonly Color LightGridBg = Color.White;
        private static readonly Color LightGridLine = Color.FromArgb(220, 225, 235);
        // 深色
        private static readonly Color DarkBg = Color.FromArgb(30, 30, 30);
        private static readonly Color DarkPanel = Color.FromArgb(45, 45, 48);
        private static readonly Color DarkFore = Color.FromArgb(220, 220, 220);
        private static readonly Color DarkLabel = Color.FromArgb(100, 160, 255);
        private static readonly Color DarkStats = Color.FromArgb(180, 180, 180);
        private static readonly Color DarkGridHdr = Color.FromArgb(60, 60, 65);
        private static readonly Color DarkGridHdrFg = Color.FromArgb(220, 220, 220);
        private static readonly Color DarkGridBg = Color.FromArgb(45, 45, 48);
        private static readonly Color DarkGridLine = Color.FromArgb(70, 70, 75);

        public MainForm()
        {
            InitializeComponent();
            if (this.DesignMode) return;

            _service = new HomeworkService();
            dtpDueDate.MinDate = DateTime.Today;
            WireEvents();
            SetupGrid();
            RefreshGrid(_service.GetAll());

            this.Load += (s, e) => {
                dgvHomework.CurrentCell = null;
                dgvHomework.ClearSelection();
                ClearInputs();

                var todayDue = _service.GetAll()
                    .Where(h => !h.IsCompleted && h.DueDate.Date == DateTime.Today)
                    .ToList();
                if (todayDue.Count > 0)
                {
                    string list = string.Join("\n", todayDue.Select(h => $"・{h.CourseName}　{h.Title}"));
                    MessageBox.Show(
                        $"⚠️ 今天有 {todayDue.Count} 筆作業即將到期：\n\n{list}",
                        "今日到期提醒",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Warning);
                }
            };
        }

        // ── Events ────────────────────────────────────────────────────
        private void WireEvents()
        {
            this.btnAdd.Click += BtnAdd_Click;
            this.btnUpdate.Click += BtnUpdate_Click;
            this.btnDelete.Click += BtnDelete_Click;
            this.btnComplete.Click += BtnComplete_Click;
            this.btnClear.Click += BtnClear_Click;
            this.btnSearch.Click += BtnSearch_Click;
            this.btnShowAll.Click += BtnShowAll_Click;
            this.btnDarkMode.Click += BtnDarkMode_Click;
            this.txtSearch.KeyDown += TxtSearch_KeyDown;
            this.dgvHomework.SelectionChanged += DgvHomework_SelectionChanged;
            this.dgvHomework.CellFormatting += DgvHomework_CellFormatting;
        }

        // ── Grid Setup ────────────────────────────────────────────────
        private void SetupGrid()
        {
            dgvHomework.Columns.Clear();
            dgvHomework.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.None;

            dgvHomework.Columns.Add(new DataGridViewTextBoxColumn { Name = "colId", HeaderText = "ID", DataPropertyName = "Id", Visible = false });
            dgvHomework.Columns.Add(new DataGridViewTextBoxColumn { Name = "colCourse", HeaderText = "課程", DataPropertyName = "CourseName", Width = 160 });
            dgvHomework.Columns.Add(new DataGridViewTextBoxColumn { Name = "colTitle", HeaderText = "標題", DataPropertyName = "Title", Width = 160 });
            dgvHomework.Columns.Add(new DataGridViewTextBoxColumn { Name = "colContent", HeaderText = "內容", DataPropertyName = "Content", Width = 200 });
            dgvHomework.Columns.Add(new DataGridViewTextBoxColumn
            {
                Name = "colDue",
                HeaderText = "截止日期",
                DataPropertyName = "DueDate",
                Width = 110,
                DefaultCellStyle = new DataGridViewCellStyle { Format = "yyyy/MM/dd" }
            });
            dgvHomework.Columns.Add(new DataGridViewTextBoxColumn { Name = "colStatus", HeaderText = "狀態", DataPropertyName = "StatusText", Width = 80 });
            var colReminder = new DataGridViewTextBoxColumn { Name = "colReminder", HeaderText = "提醒", DataPropertyName = "ReminderText" };
            colReminder.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            dgvHomework.Columns.Add(colReminder);
        }

        private void RefreshGrid(List<Homework> list)
        {
            _isLoading = true;
            dgvHomework.DataSource = null;
            dgvHomework.DataSource = list;
            UpdateStats(list);
            _selectedId = null;
            dgvHomework.ClearSelection();
            _isLoading = false;
        }

        private void UpdateStats(List<Homework> list)
        {
            int total = list.Count;
            int done = list.Count(h => h.IsCompleted);
            int overdue = list.Count(h => !h.IsCompleted && h.DueDate.Date < DateTime.Today);
            int dueToday = list.Count(h => !h.IsCompleted && h.DueDate.Date == DateTime.Today);
            lblStats.Text = $"共 {total} 筆  |  已完成：{done}  |  未完成：{total - done}  |  今日到期：{dueToday}  |  已逾期：{overdue}";
        }

        // ── Dark Mode ─────────────────────────────────────────────────
        private void BtnDarkMode_Click(object sender, EventArgs e)
        {
            _isDarkMode = !_isDarkMode;
            ApplyTheme();
        }

        private void ApplyTheme()
        {
            Color bg = _isDarkMode ? DarkBg : LightBg;
            Color panel = _isDarkMode ? DarkPanel : LightPanel;
            Color fore = _isDarkMode ? DarkFore : LightFore;
            Color label = _isDarkMode ? DarkLabel : LightLabel;
            Color stats = _isDarkMode ? DarkStats : LightStats;
            Color gridHdr = _isDarkMode ? DarkGridHdr : LightGridHdr;
            Color gridHdrFg = _isDarkMode ? DarkGridHdrFg : LightGridHdrFg;
            Color gridBg = _isDarkMode ? DarkGridBg : LightGridBg;
            Color gridLine = _isDarkMode ? DarkGridLine : LightGridLine;

            // Form
            this.BackColor = bg;

            // 標題
            lblTitle.ForeColor = label;

            // GroupBox
            foreach (var grp in new[] { grpInput, grpSearch, grpGrid })
            {
                grp.BackColor = panel;
                grp.ForeColor = fore;
            }

            // Labels
            foreach (var lbl in new Control[] { lblCourse, lblHwTitle, lblDueDate, lblContent, lblStatus, lblSearch })
                lbl.ForeColor = fore;

            lblStats.ForeColor = stats;

            // TextBox
            foreach (var txt in new[] { txtTitle, txtContent, txtSearch })
            {
                txt.BackColor = _isDarkMode ? Color.FromArgb(60, 60, 65) : Color.White;
                txt.ForeColor = fore;
            }

            // ComboBox
            foreach (var cmb in new[] { cmbCourse, cmbStatus })
            {
                cmb.BackColor = _isDarkMode ? Color.FromArgb(60, 60, 65) : Color.White;
                cmb.ForeColor = fore;
            }

            // DateTimePicker
            dtpDueDate.CalendarMonthBackground = panel;
            dtpDueDate.CalendarForeColor = fore;
            dtpDueDate.CalendarTitleBackColor = gridHdr;
            dtpDueDate.CalendarTitleForeColor = gridHdrFg;

            // Panel (buttons background)
            pnlButtons.BackColor = panel;

            // 深色模式按鈕文字
            btnDarkMode.Text = _isDarkMode ? "☀️ 淺色模式" : "🌙 深色模式";

            // DataGridView
            dgvHomework.BackgroundColor = gridBg;
            dgvHomework.GridColor = gridLine;
            dgvHomework.DefaultCellStyle.BackColor = gridBg;
            dgvHomework.DefaultCellStyle.ForeColor = fore;
            dgvHomework.ColumnHeadersDefaultCellStyle.BackColor = gridHdr;
            dgvHomework.ColumnHeadersDefaultCellStyle.ForeColor = gridHdrFg;
            dgvHomework.ColumnHeadersDefaultCellStyle.Font =
                new Font("微軟正黑體", 10F, FontStyle.Bold);

            // 強制重繪資料列顏色
            dgvHomework.Invalidate();
        }

        // ── CellFormatting ────────────────────────────────────────────
        private void DgvHomework_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var row = dgvHomework.Rows[e.RowIndex];
            if (row.DataBoundItem is Homework hw)
            {
                Color bg;
                if (_isDarkMode)
                {
                    if (hw.IsCompleted)
                        bg = Color.FromArgb(30, 70, 40);
                    else if (hw.DueDate.Date < DateTime.Today)
                        bg = Color.FromArgb(90, 30, 30);
                    else if (hw.DueDate.Date == DateTime.Today)
                        bg = Color.FromArgb(80, 70, 20);
                    else if ((hw.DueDate.Date - DateTime.Today).Days <= 3)
                        bg = Color.FromArgb(80, 55, 20);
                    else
                        bg = DarkGridBg;
                }
                else
                {
                    if (hw.IsCompleted)
                        bg = Color.FromArgb(230, 255, 230);
                    else if (hw.DueDate.Date < DateTime.Today)
                        bg = Color.FromArgb(255, 220, 220);
                    else if (hw.DueDate.Date == DateTime.Today)
                        bg = Color.FromArgb(255, 245, 200);
                    else if ((hw.DueDate.Date - DateTime.Today).Days <= 3)
                        bg = Color.FromArgb(255, 240, 210);
                    else
                        bg = Color.White;
                }

                Color fg = _isDarkMode ? DarkFore : Color.Black;
                row.DefaultCellStyle.BackColor = bg;
                row.DefaultCellStyle.ForeColor = fg;
                row.DefaultCellStyle.SelectionBackColor = Color.FromArgb(80, 120, 200);
                row.DefaultCellStyle.SelectionForeColor = Color.White;
            }
        }

        // ── SelectionChanged ──────────────────────────────────────────
        private void DgvHomework_SelectionChanged(object sender, EventArgs e)
        {
            if (_isLoading) return;
            if (dgvHomework.CurrentRow?.DataBoundItem is Homework hw)
            {
                _selectedId = hw.Id;
                cmbCourse.Text = hw.CourseName;
                txtTitle.Text = hw.Title;
                txtContent.Text = hw.Content;
                dtpDueDate.MinDate = new DateTime(2000, 1, 1);
                dtpDueDate.MaxDate = new DateTime(2100, 12, 31);
                dtpDueDate.Value = hw.DueDate.Date;
                dtpDueDate.MinDate = DateTime.Today;
                cmbStatus.SelectedIndex = hw.IsCompleted ? 1 : 0;
            }
        }

        // ── Button handlers ───────────────────────────────────────────
        private void BtnAdd_Click(object sender, EventArgs e)
        {
            if (!ValidateInput()) return;
            var hw = new Homework
            {
                CourseName = cmbCourse.Text.Trim(),
                Title = txtTitle.Text.Trim(),
                Content = txtContent.Text.Trim(),
                DueDate = dtpDueDate.Value.Date.AddHours(23).AddMinutes(59).AddSeconds(59),
                IsCompleted = cmbStatus.SelectedIndex == 1
            };
            _service.Add(hw);
            RefreshGrid(_service.GetAll());
            ClearInputs();
            MessageBox.Show("✅ 作業已新增！", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnUpdate_Click(object sender, EventArgs e)
        {
            if (_selectedId == null) { MessageBox.Show("請先在清單中選取要修改的作業。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            if (!ValidateInput()) return;
            var hw = new Homework
            {
                Id = _selectedId,
                CourseName = cmbCourse.Text.Trim(),
                Title = txtTitle.Text.Trim(),
                Content = txtContent.Text.Trim(),
                DueDate = dtpDueDate.Value.Date.AddHours(23).AddMinutes(59).AddSeconds(59),
                IsCompleted = cmbStatus.SelectedIndex == 1
            };
            _service.Update(hw);
            RefreshGrid(_service.GetAll());
            ClearInputs();
            MessageBox.Show("✏️ 作業已修改！", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (_selectedId == null) { MessageBox.Show("請先在清單中選取要刪除的作業。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            if (MessageBox.Show("確定要刪除這筆作業嗎？", "確認刪除", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                _service.Delete(_selectedId);
                RefreshGrid(_service.GetAll());
                ClearInputs();
            }
        }

        private void BtnComplete_Click(object sender, EventArgs e)
        {
            if (_selectedId == null) { MessageBox.Show("請先在清單中選取要標記完成的作業。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            _service.MarkCompleted(_selectedId);
            RefreshGrid(_service.GetAll());
            ClearInputs();
            MessageBox.Show("✅ 已標記為完成！", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnClear_Click(object sender, EventArgs e) => ClearInputs();

        private void BtnSearch_Click(object sender, EventArgs e) => RefreshGrid(_service.Search(txtSearch.Text.Trim()));

        private void BtnShowAll_Click(object sender, EventArgs e) { txtSearch.Clear(); RefreshGrid(_service.GetAll()); }

        private void TxtSearch_KeyDown(object sender, KeyEventArgs e) { if (e.KeyCode == Keys.Enter) BtnSearch_Click(sender, e); }

        // ── Helpers ───────────────────────────────────────────────────
        private bool ValidateInput()
        {
            if (string.IsNullOrWhiteSpace(cmbCourse.Text)) { MessageBox.Show("請填寫課程名稱。", "驗證失敗", MessageBoxButtons.OK, MessageBoxIcon.Warning); cmbCourse.Focus(); return false; }
            if (string.IsNullOrWhiteSpace(txtTitle.Text)) { MessageBox.Show("請填寫作業標題。", "驗證失敗", MessageBoxButtons.OK, MessageBoxIcon.Warning); txtTitle.Focus(); return false; }
            return true;
        }

        private void ClearInputs()
        {
            _isLoading = true;
            cmbCourse.Text = "";
            txtTitle.Clear();
            txtContent.Clear();
            dtpDueDate.Value = DateTime.Today;
            cmbStatus.SelectedIndex = 0;
            _selectedId = null;
            dgvHomework.CurrentCell = null;
            dgvHomework.ClearSelection();
            _isLoading = false;
        }
    }
}