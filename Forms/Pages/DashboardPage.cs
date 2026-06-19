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

            // 主 TableLayoutPanel：5列
            var tbl = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                ColumnCount = 1,
                RowCount = 5,
                BackColor = Color.Transparent
            };

            int todayH = 60 + Math.Max(todayList.Count, 1) * 32;
            int soonH = 60 + Math.Max(soonList.Count, 1) * 32;
            int progH = 100;
            int cardH = 130;

            tbl.RowStyles.Add(new RowStyle(SizeType.Absolute, soonH));   // 0: 7天內
            tbl.RowStyles.Add(new RowStyle(SizeType.Absolute, todayH));  // 1: 今日到期
            tbl.RowStyles.Add(new RowStyle(SizeType.Absolute, progH));   // 2: 完成率
            tbl.RowStyles.Add(new RowStyle(SizeType.Absolute, cardH));   // 3: 卡片
            tbl.RowStyles.Add(new RowStyle(SizeType.Percent, 100));      // 4: 空白

            // ── 列0：7天內到期 ────────────────────────────────────────
            var pnlSoon = MakeSection($"🟡 7天內即將到期（{soonList.Count} 筆）");
            if (soonList.Count == 0)
                pnlSoon.Controls.Add(new Label { Text = "近7天沒有即將到期的作業 👍", ForeColor = MainShell.ThemeMuted, Dock = DockStyle.Fill, Font = new Font("微軟正黑體", 10F), TextAlign = ContentAlignment.MiddleLeft, Padding = new Padding(14, 0, 0, 0) });
            else
            {
                var dgv = MakeMiniGrid();
                dgv.DataSource = soonList;
                pnlSoon.Controls.Add(dgv);
            }
            tbl.Controls.Add(pnlSoon, 0, 0);

            // ── 列1：今日到期 ─────────────────────────────────────────
            var pnlToday = MakeSection($"🔴 今日到期（{todayList.Count} 筆）");
            if (todayList.Count == 0)
                pnlToday.Controls.Add(new Label { Text = "今天沒有到期作業 🎉", ForeColor = MainShell.ThemeMuted, Dock = DockStyle.Fill, Font = new Font("微軟正黑體", 10F), TextAlign = ContentAlignment.MiddleLeft, Padding = new Padding(14, 0, 0, 0) });
            else
            {
                var dgv = MakeMiniGrid();
                dgv.DataSource = todayList;
                pnlToday.Controls.Add(dgv);
            }
            tbl.Controls.Add(pnlToday, 0, 1);

            // ── 列2：完成率 ───────────────────────────────────────────
            var pnlProg = MakeSection("📈 完成率");
            var lblRate = new Label
            {
                Text = $"{rate:F1}%  （{done} / {total} 筆已完成）",
                Font = new Font("微軟正黑體", 11F, FontStyle.Bold),
                ForeColor = MainShell.ThemeFore,
                Location = new Point(14, 34),
                AutoSize = true
            };
            var pb = new ProgressBar
            {
                Minimum = 0,
                Maximum = 100,
                Value = (int)rate,
                Location = new Point(14, 62),
                Size = new Size(700, 22),
                Style = ProgressBarStyle.Continuous
            };
            pnlProg.Controls.Add(lblRate);
            pnlProg.Controls.Add(pb);
            tbl.Controls.Add(pnlProg, 0, 2);

            // ── 列3：統計卡片 ─────────────────────────────────────────
            var pnlCards = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                BackColor = Color.Transparent,
                Padding = new Padding(0, 8, 0, 0)
            };
            pnlCards.Controls.Add(MakeCard("📚 總作業數", total.ToString(), MainShell.ThemeAccent));
            pnlCards.Controls.Add(MakeCard("✅ 已完成", done.ToString(), Color.FromArgb(34, 160, 90)));
            pnlCards.Controls.Add(MakeCard("⛔ 已逾期", overdue.ToString(), Color.FromArgb(210, 55, 55)));
            pnlCards.Controls.Add(MakeCard("🔴 今天到期", today.ToString(), Color.FromArgb(220, 140, 20)));
            pnlCards.Controls.Add(MakeCard("🟡 7天內到期", week.ToString(), Color.FromArgb(160, 100, 20)));
            tbl.Controls.Add(pnlCards, 0, 3);

            // 列4：空白
            tbl.Controls.Add(new Panel { Dock = DockStyle.Fill, BackColor = Color.Transparent }, 0, 4);

            this.Controls.Add(tbl);
        }

        private Panel MakeSection(string title)
        {
            var pnl = new Panel { Dock = DockStyle.Fill, BackColor = MainShell.ThemePanel, Margin = new Padding(0, 0, 0, 10) };
            var lbl = new Label
            {
                Text = title,
                Font = new Font("微軟正黑體", 11F, FontStyle.Bold),
                ForeColor = MainShell.ThemeFore,
                Dock = DockStyle.Top,
                Height = 36,
                Padding = new Padding(14, 8, 0, 0)
            };
            pnl.Controls.Add(lbl);
            return pnl;
        }

        private Panel MakeCard(string title, string value, Color accent)
        {
            var pnl = new Panel { Width = 170, Height = 110, Margin = new Padding(0, 0, 16, 0), BackColor = MainShell.ThemePanel };
            var bar = new Panel { Dock = DockStyle.Top, Height = 5, BackColor = accent };
            var lblVal = new Label { Text = value, Font = new Font("微軟正黑體", 26F, FontStyle.Bold), ForeColor = accent, TextAlign = ContentAlignment.MiddleCenter, Dock = DockStyle.Fill };
            var lblTit = new Label { Text = title, Font = new Font("微軟正黑體", 9.5F), ForeColor = MainShell.ThemeMuted, TextAlign = ContentAlignment.MiddleCenter, Dock = DockStyle.Bottom, Height = 28 };
            pnl.Controls.Add(lblVal);
            pnl.Controls.Add(lblTit);
            pnl.Controls.Add(bar);
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
                ColumnHeadersHeight = 30,
                RowTemplate = { Height = 28 },
                TabStop = false
            };
            dgv.ColumnHeadersDefaultCellStyle.BackColor = MainShell.ThemeAccent;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("微軟正黑體", 10F, FontStyle.Bold);
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