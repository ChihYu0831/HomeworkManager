using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using HomeworkManager.Models;
using HomeworkManager.Services;

namespace HomeworkManager.Forms.Pages
{
    public class HomeworkPage : Panel
    {
        private readonly HomeworkService _service;
        private readonly MainShell _shell;

        private ComboBox cmbCourse, cmbStatus;
        private TextBox txtTitle, txtContent, txtSearch;
        private DateTimePicker dtpDue;
        private DataGridView dgv;
        private Label lblStats;
        private string _selectedId = null;
        private bool _isLoading = false;

        private static readonly string[] Courses = {
            "羽球入門","作業系統概論","組合語言與計算機組織",
            "程式設計（二）","視窗程式設計（二）","演算法概論","機率與統計","醫療資訊學概論"
        };

        public HomeworkPage(HomeworkService service, MainShell shell)
        {
            _service = service;
            _shell = shell;
            this.BackColor = MainShell.ThemeBg;
            Build();
            RefreshGrid(_service.GetAll());
        }

        private void Build()
        {
            // 整體用 TableLayoutPanel：上方固定區 + 下方填滿的清單
            var tbl = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                BackColor = Color.Transparent,
                Padding = new Padding(16)
            };
            tbl.RowStyles.Add(new RowStyle(SizeType.Absolute, 340)); // 輸入+搜尋
            tbl.RowStyles.Add(new RowStyle(SizeType.Percent, 100));  // 清單

            // ── 上方：輸入 + 搜尋 ────────────────────────────────────
            var pnlTop = new Panel { Dock = DockStyle.Fill, BackColor = Color.Transparent };

            // 作業資料 GroupBox
            var grpInput = MakeGroup("✏️ 作業資料");
            grpInput.Dock = DockStyle.Top;
            grpInput.Height = 258;

            // 列1：課程 / 標題
            grpInput.Controls.Add(MakeLabel("課程名稱：", new Point(12, 36)));
            cmbCourse = new ComboBox { Location = new Point(90, 34), Size = new Size(220, 28), DropDownStyle = ComboBoxStyle.DropDown, BackColor = MainShell.ThemePanel, ForeColor = MainShell.ThemeFore };
            cmbCourse.Items.AddRange(Courses);
            grpInput.Controls.Add(cmbCourse);

            grpInput.Controls.Add(MakeLabel("作業標題：", new Point(326, 36)));
            txtTitle = new TextBox { Location = new Point(404, 34), Size = new Size(460, 28), BackColor = MainShell.ThemePanel, ForeColor = MainShell.ThemeFore };
            grpInput.Controls.Add(txtTitle);

            // 列2：截止日期 / 狀態
            grpInput.Controls.Add(MakeLabel("截止日期：", new Point(12, 78)));
            dtpDue = new DateTimePicker { Location = new Point(90, 76), Size = new Size(170, 28), Format = DateTimePickerFormat.Short, MinDate = DateTime.Today };
            grpInput.Controls.Add(dtpDue);

            grpInput.Controls.Add(MakeLabel("狀　　態：", new Point(278, 78)));
            cmbStatus = new ComboBox { Location = new Point(356, 76), Size = new Size(130, 28), DropDownStyle = ComboBoxStyle.DropDownList, BackColor = MainShell.ThemePanel, ForeColor = MainShell.ThemeFore };
            cmbStatus.Items.AddRange(new object[] { "未完成", "已完成" });
            cmbStatus.SelectedIndex = 0;
            grpInput.Controls.Add(cmbStatus);

            // 列3：作業內容
            grpInput.Controls.Add(MakeLabel("作業內容：", new Point(12, 120)));
            txtContent = new TextBox { Location = new Point(90, 118), Size = new Size(780, 70), Multiline = true, ScrollBars = ScrollBars.Vertical, BackColor = MainShell.ThemePanel, ForeColor = MainShell.ThemeFore };
            grpInput.Controls.Add(txtContent);

            // 列4：按鈕
            var pnlBtn = new Panel { Location = new Point(90, 198), Size = new Size(650, 38), BackColor = Color.Transparent };
            pnlBtn.Controls.Add(MakeBtnAt("➕ 新增", Color.FromArgb(40, 167, 69), new Point(0, 2), BtnAdd_Click));
            pnlBtn.Controls.Add(MakeBtnAt("✏️ 修改", Color.FromArgb(0, 123, 255), new Point(118, 2), BtnUpdate_Click));
            pnlBtn.Controls.Add(MakeBtnAt("🗑️ 刪除", Color.FromArgb(210, 55, 55), new Point(236, 2), BtnDelete_Click));
            pnlBtn.Controls.Add(MakeBtnAt("✅ 標記完成", Color.FromArgb(102, 16, 242), new Point(354, 2), BtnComplete_Click));
            pnlBtn.Controls.Add(MakeBtnAt("🔄 清除", Color.FromArgb(108, 117, 125), new Point(472, 2), BtnClear_Click));
            grpInput.Controls.Add(pnlBtn);

            pnlTop.Controls.Add(grpInput);

            // 搜尋 GroupBox
            var grpSearch = MakeGroup("🔍 搜尋");
            grpSearch.Dock = DockStyle.Top;
            grpSearch.Height = 64;
            grpSearch.Controls.Add(MakeLabel("搜尋：", new Point(12, 28)));
            txtSearch = new TextBox { Location = new Point(70, 26), Size = new Size(300, 28), BackColor = MainShell.ThemePanel, ForeColor = MainShell.ThemeFore };
            txtSearch.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) DoSearch(); };
            grpSearch.Controls.Add(txtSearch);
            grpSearch.Controls.Add(MakeBtnAt("搜尋", Color.FromArgb(0, 123, 255), new Point(380, 20), (s, e) => DoSearch()));
            grpSearch.Controls.Add(MakeBtnAt("顯示全部", Color.FromArgb(108, 117, 125), new Point(498, 20), (s, e) => { txtSearch.Clear(); RefreshGrid(_service.GetAll()); }));
            pnlTop.Controls.Add(grpSearch);

            tbl.Controls.Add(pnlTop, 0, 0);

            // ── 下方：作業清單 ────────────────────────────────────────
            var grpGrid = MakeGroup("📋 作業清單");
            grpGrid.Dock = DockStyle.Fill;

            dgv = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoGenerateColumns = false,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                MultiSelect = false,
                BackgroundColor = MainShell.ThemePanel,
                BorderStyle = BorderStyle.None,
                GridColor = Color.FromArgb(220, 225, 235),
                Font = new Font("微軟正黑體", 9.5F),
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None,
                EnableHeadersVisualStyles = false,
                ColumnHeadersHeight = 34,
                RowTemplate = { Height = 30 }
            };
            dgv.ColumnHeadersDefaultCellStyle.BackColor = MainShell.ThemeAccent;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("微軟正黑體", 10F, FontStyle.Bold);

            dgv.Columns.Add(new DataGridViewTextBoxColumn { Name = "colId", Visible = false, DataPropertyName = "Id" });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "課程", DataPropertyName = "CourseName", Width = 160 });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "標題", DataPropertyName = "Title", Width = 180 });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "內容", DataPropertyName = "Content", Width = 200 });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "截止日期", DataPropertyName = "DueDate", Width = 100, DefaultCellStyle = new DataGridViewCellStyle { Format = "yyyy/MM/dd" } });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "狀態", DataPropertyName = "StatusText", Width = 80 });
            var colR = new DataGridViewTextBoxColumn { HeaderText = "提醒", DataPropertyName = "ReminderText" };
            colR.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            dgv.Columns.Add(colR);

            dgv.SelectionChanged += Dgv_SelectionChanged;
            dgv.CellFormatting += Dgv_CellFormatting;
            dgv.DataBindingComplete += (s, e) =>
            {
                dgv.CurrentCell = null;
                dgv.ClearSelection();
                ClearInputs();
            };

            lblStats = new Label { Dock = DockStyle.Bottom, Height = 26, ForeColor = MainShell.ThemeMuted, Font = new Font("微軟正黑體", 9F), Padding = new Padding(4, 4, 0, 0) };

            grpGrid.Controls.Add(dgv);
            grpGrid.Controls.Add(lblStats);
            tbl.Controls.Add(grpGrid, 0, 1);

            this.Controls.Add(tbl);
        }

        private GroupBox MakeGroup(string t) => new GroupBox { Text = t, Font = new Font("微軟正黑體", 10F, FontStyle.Bold), ForeColor = MainShell.ThemeFore, BackColor = MainShell.ThemePanel, Padding = new Padding(6) };
        private Label MakeLabel(string t, Point p) => new Label { Text = t, Location = p, Size = new Size(78, 24), TextAlign = ContentAlignment.MiddleRight, ForeColor = MainShell.ThemeFore, Font = new Font("微軟正黑體", 10F) };
        private Button MakeBtnAt(string text, Color bg, Point loc, EventHandler click)
        {
            var b = new Button { Text = text, Location = loc, Size = new Size(110, 32), FlatStyle = FlatStyle.Flat, BackColor = bg, ForeColor = Color.White, Font = new Font("微軟正黑體", 10F, FontStyle.Bold), Cursor = Cursors.Hand };
            b.FlatAppearance.BorderSize = 0; b.Click += click; return b;
        }

        private void RefreshGrid(List<Homework> list)
        {
            _isLoading = true;
            dgv.DataSource = null;
            dgv.DataSource = list;
            dgv.DefaultCellStyle.BackColor = MainShell.ThemePanel;
            dgv.DefaultCellStyle.ForeColor = MainShell.ThemeFore;
            dgv.BackgroundColor = MainShell.ThemePanel;
            int total = list.Count, done = list.Count(h => h.IsCompleted), overdue = list.Count(h => !h.IsCompleted && h.DueDate.Date < DateTime.Today);
            lblStats.Text = $"共 {total} 筆  |  已完成：{done}  |  未完成：{total - done}  |  已逾期：{overdue}";
            _selectedId = null; dgv.CurrentCell = null; dgv.ClearSelection();
            _isLoading = false;
        }

        private void DoSearch() => RefreshGrid(_service.Search(txtSearch.Text.Trim()));

        private void Dgv_SelectionChanged(object sender, EventArgs e)
        {
            if (_isLoading) return;
            if (dgv.CurrentRow?.DataBoundItem is Homework hw)
            {
                _selectedId = hw.Id;
                cmbCourse.Text = hw.CourseName;
                txtTitle.Text = hw.Title;
                txtContent.Text = hw.Content;
                dtpDue.MinDate = new DateTime(2000, 1, 1);
                dtpDue.Value = hw.DueDate.Date;
                dtpDue.MinDate = DateTime.Today;
                cmbStatus.SelectedIndex = hw.IsCompleted ? 1 : 0;
            }
        }

        private void Dgv_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var row = dgv.Rows[e.RowIndex];
            if (row.DataBoundItem is Homework hw)
            {
                bool dark = _shell.IsDark;
                Color bg = hw.IsCompleted ? (dark ? Color.FromArgb(30, 70, 40) : Color.FromArgb(225, 255, 225))
                         : hw.DueDate.Date < DateTime.Today ? (dark ? Color.FromArgb(90, 30, 30) : Color.FromArgb(255, 218, 218))
                         : hw.DueDate.Date == DateTime.Today ? (dark ? Color.FromArgb(80, 70, 20) : Color.FromArgb(255, 245, 195))
                         : (hw.DueDate.Date - DateTime.Today).Days <= 3 ? (dark ? Color.FromArgb(75, 55, 15) : Color.FromArgb(255, 238, 210))
                         : MainShell.ThemePanel;
                row.DefaultCellStyle.BackColor = bg;
                row.DefaultCellStyle.ForeColor = MainShell.ThemeFore;
                row.DefaultCellStyle.SelectionBackColor = Color.FromArgb(80, 130, 220);
                row.DefaultCellStyle.SelectionForeColor = Color.White;
            }
        }

        private void BtnAdd_Click(object sender, EventArgs e)
        {
            if (!Val()) return;
            _service.Add(new Homework { CourseName = cmbCourse.Text.Trim(), Title = txtTitle.Text.Trim(), Content = txtContent.Text.Trim(), DueDate = dtpDue.Value.Date.AddHours(23).AddMinutes(59).AddSeconds(59), IsCompleted = cmbStatus.SelectedIndex == 1 });
            RefreshGrid(_service.GetAll()); ClearInputs();
            MessageBox.Show("✅ 作業已新增！", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void BtnUpdate_Click(object sender, EventArgs e)
        {
            if (_selectedId == null) { MessageBox.Show("請先選取要修改的作業。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            if (!Val()) return;
            _service.Update(new Homework { Id = _selectedId, CourseName = cmbCourse.Text.Trim(), Title = txtTitle.Text.Trim(), Content = txtContent.Text.Trim(), DueDate = dtpDue.Value.Date.AddHours(23).AddMinutes(59).AddSeconds(59), IsCompleted = cmbStatus.SelectedIndex == 1 });
            RefreshGrid(_service.GetAll()); ClearInputs();
            MessageBox.Show("✏️ 作業已修改！", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void BtnDelete_Click(object sender, EventArgs e)
        {
            if (_selectedId == null) { MessageBox.Show("請先選取要刪除的作業。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            if (MessageBox.Show("確定要刪除？", "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            { _service.Delete(_selectedId); RefreshGrid(_service.GetAll()); ClearInputs(); }
        }
        private void BtnComplete_Click(object sender, EventArgs e)
        {
            if (_selectedId == null) { MessageBox.Show("請先選取作業。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning); return; }
            _service.MarkCompleted(_selectedId); RefreshGrid(_service.GetAll()); ClearInputs();
            MessageBox.Show("✅ 已標記為完成！", "成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void BtnClear_Click(object sender, EventArgs e) => ClearInputs();

        private bool Val()
        {
            if (string.IsNullOrWhiteSpace(cmbCourse.Text)) { MessageBox.Show("請填寫課程名稱。", "驗證", MessageBoxButtons.OK, MessageBoxIcon.Warning); return false; }
            if (string.IsNullOrWhiteSpace(txtTitle.Text)) { MessageBox.Show("請填寫作業標題。", "驗證", MessageBoxButtons.OK, MessageBoxIcon.Warning); return false; }
            return true;
        }
        private void ClearInputs()
        {
            _isLoading = true;
            cmbCourse.Text = ""; txtTitle.Clear(); txtContent.Clear();
            dtpDue.Value = DateTime.Today; cmbStatus.SelectedIndex = 0;
            _selectedId = null; dgv.CurrentCell = null; dgv.ClearSelection();
            _isLoading = false;
        }
    }
}