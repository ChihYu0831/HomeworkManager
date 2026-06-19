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
            var tbl = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 2,
                RowCount = 1,
                BackColor = Color.Transparent,
                Padding = new Padding(16),
                CellBorderStyle = TableLayoutPanelCellBorderStyle.None
            };
            tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 360)); // 左：月曆
            tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));  // 右：清單
            tbl.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            // ── 左欄：月曆 + 圖例 ─────────────────────────────────────
            var pnlLeft = new Panel { Dock = DockStyle.Fill, BackColor = MainShell.ThemePanel, Padding = new Padding(16) };

            var lblCalTitle = new Label
            {
                Text = "📅 月曆",
                Font = new Font("微軟正黑體", 12F, FontStyle.Bold),
                ForeColor = MainShell.ThemeFore,
                Dock = DockStyle.Top,
                Height = 36
            };
            pnlLeft.Controls.Add(lblCalTitle);

            cal = new MonthCalendar
            {
                Dock = DockStyle.Top,
                MaxSelectionCount = 1,
                ShowToday = true,
                Font = new Font("微軟正黑體", 10F),
                Height = 220
            };
            cal.DateSelected += Cal_DateSelected;
            pnlLeft.Controls.Add(cal);

            var pnlLegend = new Panel { Dock = DockStyle.Top, Height = 120, BackColor = Color.Transparent, Padding = new Padding(0, 12, 0, 0) };
            int ly = 12;
            foreach (var (icon, text, color) in new (string, string, Color)[] {
                ("●", "粗體日期 = 有作業到期", MainShell.ThemeFore),
                ("🔴", "今天到期",             Color.FromArgb(220, 140, 20)),
                ("⛔", "已逾期",               Color.FromArgb(210, 55, 55)),
                ("✅", "已完成",               Color.FromArgb(34, 160, 90))
            })
            {
                pnlLegend.Controls.Add(new Label
                {
                    Text = $"  {icon}  {text}",
                    Font = new Font("微軟正黑體", 10F),
                    ForeColor = color,
                    Location = new Point(0, ly),
                    Size = new Size(320, 24)
                });
                ly += 26;
            }
            pnlLeft.Controls.Add(pnlLegend);
            tbl.Controls.Add(pnlLeft, 0, 0);

            // ── 右欄：作業清單 ────────────────────────────────────────
            var pnlRight = new Panel { Dock = DockStyle.Fill, BackColor = MainShell.ThemePanel, Padding = new Padding(16) };

            lblDayTitle = new Label
            {
                Text = "👆 請點選左側日期查看作業",
                Font = new Font("微軟正黑體", 13F, FontStyle.Bold),
                ForeColor = MainShell.ThemeFore,
                Dock = DockStyle.Top,
                Height = 42
            };
            pnlRight.Controls.Add(lblDayTitle);

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

            pnlRight.Controls.Add(dgvDay);
            tbl.Controls.Add(pnlRight, 1, 0);

            this.Controls.Add(tbl);
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
                Color bg = hw.IsCompleted ? Color.FromArgb(225, 255, 225)
                         : hw.DueDate.Date < DateTime.Today ? Color.FromArgb(255, 218, 218)
                         : Color.FromArgb(255, 245, 195);
                row.DefaultCellStyle.BackColor = bg;
                row.DefaultCellStyle.ForeColor = MainShell.ThemeFore;
            }
        }
    }
}