using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using HomeworkManager.Models;
using HomeworkManager.Services;

namespace HomeworkManager.Forms.Pages
{
    public class DashboardPage : Panel
    {
        private readonly HomeworkService _service;
        private readonly MainShell _shell;

        public DashboardPage(HomeworkService service, MainShell shell)
        {
            _service = service;
            _shell = shell;
            this.Padding = new Padding(20);
            this.BackColor = MainShell.ThemeBg;
            Build();
        }

        private void Build()
        {
            this.Controls.Clear();
            var all = _service.GetAll();
            int total = all.Count;
            int done = all.Count(h => h.IsCompleted);
            int overdue = all.Count(h => !h.IsCompleted && h.DueDate.Date < DateTime.Today);
            int today = all.Count(h => !h.IsCompleted && h.DueDate.Date == DateTime.Today);
            int week = all.Count(h => !h.IsCompleted && h.DueDate.Date > DateTime.Today && h.DueDate.Date <= DateTime.Today.AddDays(7));
            double rate = total == 0 ? 0 : (double)done / total * 100;

            var todayList = all.Where(h => !h.IsCompleted && h.DueDate.Date == DateTime.Today).ToList();
            var soonList = all.Where(h => !h.IsCompleted && h.DueDate.Date > DateTime.Today && h.DueDate.Date <= DateTime.Today.AddDays(7)).OrderBy(h => h.DueDate).ToList();

            var tbl = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 5,
                BackColor = Color.Transparent
            };
            int soonH = 36 + 28 + Math.Max(soonList.Count, 1) * 30 + 20;
            int todayH = 36 + 28 + Math.Max(todayList.Count, 1) * 30 + 20;
            tbl.RowStyles.Add(new RowStyle(SizeType.Absolute, soonH));
            tbl.RowStyles.Add(new RowStyle(SizeType.Absolute, 12));
            tbl.RowStyles.Add(new RowStyle(SizeType.Absolute, todayH));
            tbl.RowStyles.Add(new RowStyle(SizeType.Absolute, 12));
            tbl.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            // ── 7天內到期 ─────────────────────────────────────────────
            Panel pnlSoon;
            if (soonList.Count == 0)
            {
                pnlSoon = BuildSection($"🟡 7天內即將到期（0 筆）", null, "近7天沒有即將到期的作業 👍");
            }
            else
            {
                var dgvSoon = MakeMiniGrid();
                dgvSoon.DataSource = soonList;
                pnlSoon = BuildSection($"🟡 7天內即將到期（{soonList.Count} 筆）", dgvSoon, null);
            }
            tbl.Controls.Add(pnlSoon, 0, 0);
            tbl.Controls.Add(new Panel { BackColor = Color.Transparent }, 0, 1);

            // ── 今日到期 ──────────────────────────────────────────────
            Panel pnlToday;
            if (todayList.Count == 0)
            {
                pnlToday = BuildSection("🔴 今日到期（0 筆）", null, "今天沒有到期作業 🎉");
            }
            else
            {
                var dgvToday = MakeMiniGrid();
                dgvToday.DataSource = todayList;
                pnlToday = BuildSection($"🔴 今日到期（{todayList.Count} 筆）", dgvToday, null);
            }
            tbl.Controls.Add(pnlToday, 0, 2);
            tbl.Controls.Add(new Panel { BackColor = Color.Transparent }, 0, 3);

            // ── 完成率 + 卡片 ─────────────────────────────────────────
            var pnlBottom = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                BackColor = Color.Transparent
            };
            pnlBottom.RowStyles.Add(new RowStyle(SizeType.Absolute, 100));
            pnlBottom.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            var pnlProg = new Panel { Dock = DockStyle.Fill, BackColor = MainShell.ThemePanel, Padding = new Padding(14, 8, 14, 8) };
            var lblProgTitle = new Label { Text = "📈 完成率", Font = new Font("微軟正黑體", 11F, FontStyle.Bold), ForeColor = MainShell.ThemeFore, Location = new Point(14, 8), Size = new Size(200, 28) };
            var lblProgVal = new Label { Text = string.Format("{0:F1}%  （{1} / {2} 筆已完成）", rate, done, total), Font = new Font("微軟正黑體", 10F), ForeColor = MainShell.ThemeFore, Location = new Point(14, 38), Size = new Size(400, 24) };
            var pbPanel = new Panel { Location = new Point(0, 66), Height = 22, Anchor = AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Top, BackColor = Color.Transparent };
            var pb = new ProgressBar { Minimum = 0, Maximum = 100, Value = (int)rate, Dock = DockStyle.Fill, Style = ProgressBarStyle.Continuous };
            pbPanel.Controls.Add(pb);
            pnlProg.Controls.Add(lblProgTitle);
            pnlProg.Controls.Add(lblProgVal);
            pnlProg.Controls.Add(pbPanel);
            pnlProg.Resize += (s, e) => pbPanel.Width = pnlProg.Width;
            pnlBottom.Controls.Add(pnlProg, 0, 0);

            var pnlCards = new FlowLayoutPanel { Dock = DockStyle.Fill, FlowDirection = FlowDirection.LeftToRight, WrapContents = false, BackColor = Color.Transparent, Padding = new Padding(0, 10, 0, 0) };
            pnlCards.Controls.Add(MakeCard("📚 總作業數", total.ToString(), MainShell.ThemeAccent));
            pnlCards.Controls.Add(MakeCard("✅ 已完成", done.ToString(), Color.FromArgb(34, 160, 90)));
            pnlCards.Controls.Add(MakeCard("⛔ 已逾期", overdue.ToString(), Color.FromArgb(210, 55, 55)));
            pnlCards.Controls.Add(MakeCard("🔴 今天到期", today.ToString(), Color.FromArgb(220, 140, 20)));
            pnlCards.Controls.Add(MakeCard("🟡 7天內到期", week.ToString(), Color.FromArgb(160, 100, 20)));
            pnlBottom.Controls.Add(pnlCards, 0, 1);

            tbl.Controls.Add(pnlBottom, 0, 4);
            this.Controls.Add(tbl);
        }

        private Panel BuildSection(string title, DataGridView dgv, string emptyText)
        {
            var tbl = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 2,
                BackColor = MainShell.ThemePanel
            };
            tbl.RowStyles.Add(new RowStyle(SizeType.Absolute, 36));
            tbl.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
            tbl.Controls.Add(new Label { Text = title, Font = new Font("微軟正黑體", 11F, FontStyle.Bold), ForeColor = MainShell.ThemeFore, Dock = DockStyle.Fill, Padding = new Padding(14, 6, 0, 0) }, 0, 0);

            if (dgv != null)
                tbl.Controls.Add(dgv, 0, 1);
            else
                tbl.Controls.Add(new Label { Text = emptyText, ForeColor = MainShell.ThemeMuted, Font = new Font("微軟正黑體", 10F), Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft, Padding = new Padding(14, 0, 0, 0) }, 0, 1);

            var pnl = new Panel { Dock = DockStyle.Fill, BackColor = MainShell.ThemePanel };
            pnl.Controls.Add(tbl);
            return pnl;
        }

        private Panel MakeCard(string title, string value, Color accent)
        {
            var pnl = new Panel { Width = 170, Height = 110, Margin = new Padding(0, 0, 16, 0), BackColor = MainShell.ThemePanel };
            pnl.Controls.Add(new Panel { Dock = DockStyle.Top, Height = 5, BackColor = accent });
            pnl.Controls.Add(new Label { Text = value, Font = new Font("微軟正黑體", 26F, FontStyle.Bold), ForeColor = accent, TextAlign = ContentAlignment.MiddleCenter, Dock = DockStyle.Fill });
            pnl.Controls.Add(new Label { Text = title, Font = new Font("微軟正黑體", 9.5F), ForeColor = MainShell.ThemeMuted, TextAlign = ContentAlignment.MiddleCenter, Dock = DockStyle.Bottom, Height = 28 });
            return pnl;
        }

        private DataGridView MakeMiniGrid()
        {
            var dgv = new DataGridView
            {
                Dock = DockStyle.Fill,
                AutoGenerateColumns = false,
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                ReadOnly = true,
                RowHeadersVisible = false,
                SelectionMode = DataGridViewSelectionMode.CellSelect,
                MultiSelect = false,
                BackgroundColor = MainShell.ThemePanel,
                BorderStyle = BorderStyle.None,
                GridColor = Color.FromArgb(230, 233, 240),
                Font = new Font("微軟正黑體", 9.5F),
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                EnableHeadersVisualStyles = false,
                ColumnHeadersHeight = 28,
                RowTemplate = { Height = 28 },
                TabStop = false
            };
            dgv.ColumnHeadersDefaultCellStyle.BackColor = MainShell.ThemeAccent;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("微軟正黑體", 9.5F, FontStyle.Bold);
            dgv.DefaultCellStyle.BackColor = MainShell.ThemePanel;
            dgv.DefaultCellStyle.ForeColor = MainShell.ThemeFore;

            dgv.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "課程", DataPropertyName = "CourseName", FillWeight = 25 });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "標題", DataPropertyName = "Title", FillWeight = 35 });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "截止日期", DataPropertyName = "DueDate", FillWeight = 20, DefaultCellStyle = new DataGridViewCellStyle { Format = "yyyy/MM/dd" } });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "提醒", DataPropertyName = "ReminderText", FillWeight = 20 });

            dgv.DataBindingComplete += (s, e) => { dgv.CurrentCell = null; dgv.ClearSelection(); };
            dgv.GotFocus += (s, e) => { dgv.CurrentCell = null; dgv.ClearSelection(); };
            return dgv;
        }
    }
}