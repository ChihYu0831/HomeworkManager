using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using HomeworkManager.Services;
using HomeworkManager.Models;

namespace HomeworkManager.Forms.Pages
{
    public class CalendarPage : Panel
    {
        private readonly HomeworkService _service;
        private readonly MainShell _shell;
        private MonthCalendar cal;
        private DataGridView dgvDay;
        private Label lblDayTitle;

        public CalendarPage(HomeworkService service, MainShell shell)
        {
            _service = service;
            _shell = shell;
            this.BackColor = MainShell.ThemeBg;
            Build();
            HighlightDueDates();
        }

        private void Build()
        {
            // 主分割：左欄固定寬 / 右欄填滿
            var tblMain = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                BackColor = Color.Transparent,
                Padding = new Padding(16)
            };
            tblMain.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 380));
            tblMain.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            tblMain.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            // ── 左欄：TableLayoutPanel 分三列 ────────────────────────
            var tblLeft = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 3,
                BackColor = MainShell.ThemePanel,
                Padding = new Padding(16)
            };
            tblLeft.RowStyles.Add(new RowStyle(SizeType.Absolute, 34));   // 列0：標題
            tblLeft.RowStyles.Add(new RowStyle(SizeType.Absolute, 240));  // 列1：月曆
            tblLeft.RowStyles.Add(new RowStyle(SizeType.Percent, 100));   // 列2：圖例

            // 列0：標題
            var lblCalTitle = new Label
            {
                Text = "📅 月曆",
                Font = new Font("微軟正黑體", 11F, FontStyle.Bold),
                ForeColor = MainShell.ThemeFore,
                Dock = DockStyle.Fill,
                TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            };

            // 列1：月曆
            cal = new MonthCalendar
            {
                MaxSelectionCount = 1,
                ShowToday = true,
                Font = new Font("微軟正黑體", 11F),
                Dock = DockStyle.Fill,
                CalendarDimensions = new Size(1, 1),
                TitleBackColor = Color.FromArgb(28, 55, 110),
                TitleForeColor = Color.White,
                TrailingForeColor = Color.FromArgb(180, 180, 180),
                ForeColor = Color.FromArgb(30, 30, 30)
            };
            cal.DateSelected += Cal_DateSelected;

            // 列2：圖例
            var tblLegend = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 5,
                BackColor = Color.Transparent,
                Padding = new Padding(0, 10, 0, 0)
            };
            tblLegend.RowStyles.Add(new RowStyle(SizeType.Absolute, 28));
            tblLegend.RowStyles.Add(new RowStyle(SizeType.Absolute, 24));
            tblLegend.RowStyles.Add(new RowStyle(SizeType.Absolute, 24));
            tblLegend.RowStyles.Add(new RowStyle(SizeType.Absolute, 24));
            tblLegend.RowStyles.Add(new RowStyle(SizeType.Absolute, 24));

            tblLegend.Controls.Add(new Label { Text = "顏色說明", Font = new Font("微軟正黑體", 10F, FontStyle.Bold), ForeColor = MainShell.ThemeFore, Dock = DockStyle.Fill }, 0, 0);

            int row = 1;
            foreach (var (icon, text, color) in new (string, string, Color)[] {
                ("🟡", "今天到期",        Color.FromArgb(180, 130, 0)),
                ("🔴", "已逾期",          Color.FromArgb(210, 55, 55)),
                ("✅", "已完成",          Color.FromArgb(34, 160, 90)),
                ("●",  "粗體 = 有作業",   Color.FromArgb(120, 60, 200))
            })
            {
                tblLegend.Controls.Add(new Label { Text = $"  {icon}  {text}", Font = new Font("微軟正黑體", 9.5F), ForeColor = color, Dock = DockStyle.Fill, TextAlign = System.Drawing.ContentAlignment.MiddleLeft }, 0, row++);
            }

            tblLeft.Controls.Add(lblCalTitle, 0, 0);
            tblLeft.Controls.Add(cal, 0, 1);
            tblLeft.Controls.Add(tblLegend, 0, 2);
            tblMain.Controls.Add(tblLeft, 0, 0);

            // ── 右欄：標題 + DataGridView ─────────────────────────────
            var tblRight = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                BackColor = MainShell.ThemePanel,
                Padding = new Padding(16)
            };
            tblRight.RowStyles.Add(new RowStyle(SizeType.Absolute, 46));
            tblRight.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            lblDayTitle = new Label
            {
                Text = "👆 請點選左側日期查看作業",
                Font = new Font("微軟正黑體", 13F, FontStyle.Bold),
                ForeColor = MainShell.ThemeFore,
                Dock = DockStyle.Fill,
                TextAlign = System.Drawing.ContentAlignment.MiddleLeft
            };

            dgvDay = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoGenerateColumns = false,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                BackgroundColor = MainShell.ThemePanel,
                BorderStyle = BorderStyle.None,
                GridColor = Color.FromArgb(220, 225, 235),
                Font = new Font("微軟正黑體", 10.5F),
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                EnableHeadersVisualStyles = false,
                ColumnHeadersHeight = 34,
                RowTemplate = { Height = 34 }
            };
            dgvDay.ColumnHeadersDefaultCellStyle.BackColor = MainShell.ThemeAccent;
            dgvDay.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgvDay.ColumnHeadersDefaultCellStyle.Font = new Font("微軟正黑體", 10F, FontStyle.Bold);
            dgvDay.DefaultCellStyle.BackColor = MainShell.ThemePanel;
            dgvDay.DefaultCellStyle.ForeColor = MainShell.ThemeFore;

            dgvDay.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "提醒", DataPropertyName = "ReminderText", FillWeight = 20 });
            dgvDay.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "課程", DataPropertyName = "CourseName", FillWeight = 28 });
            dgvDay.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "作業標題", DataPropertyName = "Title", FillWeight = 34 });
            dgvDay.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "狀態", DataPropertyName = "StatusText", FillWeight = 18 });

            dgvDay.CellFormatting += DgvDay_CellFormatting;
            dgvDay.DataBindingComplete += (s, e) => { dgvDay.CurrentCell = null; dgvDay.ClearSelection(); };

            tblRight.Controls.Add(lblDayTitle, 0, 0);
            tblRight.Controls.Add(dgvDay, 0, 1);
            tblMain.Controls.Add(tblRight, 1, 0);

            this.Controls.Add(tblMain);
        }

        private void HighlightDueDates()
        {
            var dates = _service.GetAll()
                .Where(h => !h.IsCompleted)
                .Select(h => h.DueDate.Date)
                .Distinct().ToArray();
            cal.BoldedDates = dates;
            cal.UpdateBoldedDates();
        }

        private void Cal_DateSelected(object sender, DateRangeEventArgs e)
        {
            var selected = e.Start.Date;
            var list = _service.GetAll()
                .Where(h => h.DueDate.Date == selected)
                .OrderBy(h => h.IsCompleted).ToList();
            lblDayTitle.Text = $"📅 {selected:yyyy年MM月dd日}  共 {list.Count} 筆作業";
            dgvDay.DataSource = null;
            dgvDay.DataSource = list;
        }

        private void DgvDay_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var row = dgvDay.Rows[e.RowIndex];
            if (row.DataBoundItem is Homework hw)
            {
                bool dark = _shell.IsDark;
                Color bg;
                if (hw.IsCompleted)
                    bg = dark ? Color.FromArgb(30, 70, 40) : Color.FromArgb(225, 255, 225);
                else if (hw.DueDate.Date < DateTime.Today)
                    bg = dark ? Color.FromArgb(90, 30, 30) : Color.FromArgb(255, 218, 218);
                else if (hw.DueDate.Date == DateTime.Today)
                    bg = dark ? Color.FromArgb(80, 70, 20) : Color.FromArgb(255, 245, 195);
                else if ((hw.DueDate.Date - DateTime.Today).Days <= 3)
                    bg = dark ? Color.FromArgb(75, 55, 15) : Color.FromArgb(255, 235, 200);
                else
                    bg = MainShell.ThemePanel;

                row.DefaultCellStyle.BackColor = bg;
                row.DefaultCellStyle.ForeColor = dark ? Color.FromArgb(220, 220, 220) : MainShell.ThemeFore;
                row.DefaultCellStyle.SelectionBackColor = Color.FromArgb(80, 130, 220);
                row.DefaultCellStyle.SelectionForeColor = Color.White;
            }
        }
    }
}