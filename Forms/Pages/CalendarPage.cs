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
            tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 400));
            tbl.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            tbl.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            // ── 左欄 ─────────────────────────────────────────────────
            var pnlLeft = new Panel { Dock = DockStyle.Fill, BackColor = MainShell.ThemePanel, Padding = new Padding(16, 12, 16, 16) };

            // 月曆（Dock=Top，最先加入 = 最上面）
            cal = new MonthCalendar
            {
                MaxSelectionCount = 1,
                ShowToday = true,
                Font = new Font("微軟正黑體", 11F),
                Dock = DockStyle.Top,
                Height = 260
            };
            cal.DateSelected += Cal_DateSelected;

            // 月曆標題（Dock=Top，第二個加入 = 在月曆下面）
            var lblCalTitle = new Label
            {
                Text = "📅 月曆",
                Font = new Font("微軟正黑體", 11F, FontStyle.Bold),
                ForeColor = MainShell.ThemeFore,
                Dock = DockStyle.Top,
                Height = 32
            };

            // 圖例（Dock=Top，最後加入 = 在最下面）
            var pnlLegend = new Panel { Dock = DockStyle.Top, Height = 114, BackColor = Color.Transparent, Padding = new Padding(0, 8, 0, 0) };
            var legendTitle = new Label { Text = "顏色說明", Font = new Font("微軟正黑體", 10F, FontStyle.Bold), ForeColor = MainShell.ThemeFore, Location = new Point(0, 0), Size = new Size(300, 24) };
            pnlLegend.Controls.Add(legendTitle);
            int ly = 26;
            foreach (var (icon, text, color) in new (string, string, Color)[] {
                ("🟡", "今天到期",      Color.FromArgb(180, 130, 0)),
                ("🔴", "已逾期",        Color.FromArgb(210, 55, 55)),
                ("✅", "已完成",        Color.FromArgb(34, 160, 90)),
                ("●",  "粗體 = 有作業", MainShell.ThemeMuted)
            })
            {
                pnlLegend.Controls.Add(new Label { Text = $"  {icon}  {text}", Font = new Font("微軟正黑體", 9.5F), ForeColor = color, Location = new Point(0, ly), Size = new Size(300, 22) });
                ly += 22;
            }

            pnlLeft.Controls.Add(cal);
            pnlLeft.Controls.Add(lblCalTitle);
            pnlLeft.Controls.Add(pnlLegend);
            tbl.Controls.Add(pnlLeft, 0, 0);

            // ── 右欄 ─────────────────────────────────────────────────
            var pnlRight = new Panel { Dock = DockStyle.Fill, BackColor = MainShell.ThemePanel, Padding = new Padding(16) };

            lblDayTitle = new Label
            {
                Text = "👆 請點選左側日期查看作業",
                Font = new Font("微軟正黑體", 13F, FontStyle.Bold),
                ForeColor = MainShell.ThemeFore,
                Dock = DockStyle.Top,
                Height = 42
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

            // 顏色依「今天」計算，不是點選的日期
            dgvDay.CellFormatting += DgvDay_CellFormatting;
            dgvDay.DataBindingComplete += (s, e) => { dgvDay.CurrentCell = null; dgvDay.ClearSelection(); };

            pnlRight.Controls.Add(dgvDay);
            pnlRight.Controls.Add(lblDayTitle);
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
                // 顏色依今天（DateTime.Today）計算，跟點選哪天無關
                Color bg;
                if (hw.IsCompleted)
                    bg = Color.FromArgb(225, 255, 225);
                else if (hw.DueDate.Date < DateTime.Today)
                    bg = Color.FromArgb(255, 218, 218);   // 逾期：紅
                else if (hw.DueDate.Date == DateTime.Today)
                    bg = Color.FromArgb(255, 245, 195);   // 今天：黃
                else if ((hw.DueDate.Date - DateTime.Today).Days <= 3)
                    bg = Color.FromArgb(255, 235, 200);   // 3天內：淡橘
                else
                    bg = MainShell.ThemePanel;             // 正常：白

                row.DefaultCellStyle.BackColor = bg;
                row.DefaultCellStyle.ForeColor = MainShell.ThemeFore;
                row.DefaultCellStyle.SelectionBackColor = Color.FromArgb(80, 130, 220);
                row.DefaultCellStyle.SelectionForeColor = Color.White;
            }
        }
    }
}