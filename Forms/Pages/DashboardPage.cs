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
            _shell   = shell;
            this.Padding = new Padding(24);
            Build();
        }

        private void Build()
        {
            this.Controls.Clear();
            this.BackColor = MainShell.ThemeBg;

            var all      = _service.GetAll();
            int total    = all.Count;
            int done     = all.Count(h => h.IsCompleted);
            int overdue  = all.Count(h => !h.IsCompleted && h.DueDate.Date < DateTime.Today);
            int today    = all.Count(h => !h.IsCompleted && h.DueDate.Date == DateTime.Today);
            int week     = all.Count(h => !h.IsCompleted && h.DueDate.Date > DateTime.Today && h.DueDate.Date <= DateTime.Today.AddDays(7));
            double rate  = total == 0 ? 0 : (double)done / total * 100;

            // ── 統計卡片區 ─────────────────────────────────────────────
            var pnlCards = new FlowLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 130,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                BackColor = Color.Transparent,
                Padding = new Padding(0, 0, 0, 12)
            };

            pnlCards.Controls.Add(MakeCard("📚 總作業數",  total.ToString(),    MainShell.ThemeAccent));
            pnlCards.Controls.Add(MakeCard("✅ 已完成",    done.ToString(),     Color.FromArgb(34, 160, 90)));
            pnlCards.Controls.Add(MakeCard("⛔ 已逾期",    overdue.ToString(),  Color.FromArgb(210, 55, 55)));
            pnlCards.Controls.Add(MakeCard("🔴 今天到期",  today.ToString(),    Color.FromArgb(220, 140, 20)));
            pnlCards.Controls.Add(MakeCard("🟡 7天內到期", week.ToString(),     Color.FromArgb(180, 110, 30)));

            this.Controls.Add(pnlCards);

            // ── 完成率 ProgressBar ─────────────────────────────────────
            var pnlProg = MakeSection("📈 完成率");
            var lblRate = new Label
            {
                Text      = $"{rate:F1}%  （{done} / {total} 筆已完成）",
                Font      = new Font("微軟正黑體", 11F, FontStyle.Bold),
                ForeColor = MainShell.ThemeFore,
                AutoSize  = true,
                Location  = new Point(16, 32)
            };
            var pb = new ProgressBar
            {
                Minimum  = 0,
                Maximum  = 100,
                Value    = (int)rate,
                Location = new Point(16, 62),
                Size     = new Size(700, 28),
                Style    = ProgressBarStyle.Continuous
            };
            pnlProg.Controls.Add(lblRate);
            pnlProg.Controls.Add(pb);
            this.Controls.Add(pnlProg);

            // ── 今日到期清單 ───────────────────────────────────────────
            var todayList = all.Where(h => !h.IsCompleted && h.DueDate.Date == DateTime.Today).ToList();
            var pnlToday  = MakeSection($"🔴 今日到期（{todayList.Count} 筆）");
            if (todayList.Count == 0)
            {
                pnlToday.Controls.Add(new Label { Text = "今天沒有到期作業 🎉", ForeColor = MainShell.ThemeMuted, Location = new Point(16, 36), AutoSize = true, Font = new Font("微軟正黑體", 10F) });
            }
            else
            {
                var dgv = MakeMiniGrid();
                dgv.DataSource = todayList;
                pnlToday.Controls.Add(dgv);
            }
            this.Controls.Add(pnlToday);

            // ── 即將到期清單 ───────────────────────────────────────────
            var soonList = all.Where(h => !h.IsCompleted && h.DueDate.Date > DateTime.Today && h.DueDate.Date <= DateTime.Today.AddDays(7)).OrderBy(h => h.DueDate).ToList();
            var pnlSoon  = MakeSection($"🟡 7天內即將到期（{soonList.Count} 筆）");
            if (soonList.Count == 0)
            {
                pnlSoon.Controls.Add(new Label { Text = "近7天沒有即將到期的作業 👍", ForeColor = MainShell.ThemeMuted, Location = new Point(16, 36), AutoSize = true, Font = new Font("微軟正黑體", 10F) });
            }
            else
            {
                var dgv = MakeMiniGrid();
                dgv.DataSource = soonList;
                pnlSoon.Controls.Add(dgv);
            }
            this.Controls.Add(pnlSoon);
        }

        private Panel MakeCard(string title, string value, Color accent)
        {
            var pnl = new Panel
            {
                Width     = 170,
                Height    = 108,
                Margin    = new Padding(0, 0, 16, 0),
                BackColor = MainShell.ThemePanel
            };
            // top accent bar
            var bar = new Panel { Dock = DockStyle.Top, Height = 5, BackColor = accent };
            var lblVal = new Label
            {
                Text      = value,
                Font      = new Font("微軟正黑體", 26F, FontStyle.Bold),
                ForeColor = accent,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock      = DockStyle.Fill
            };
            var lblTitle = new Label
            {
                Text      = title,
                Font      = new Font("微軟正黑體", 9.5F),
                ForeColor = MainShell.ThemeMuted,
                TextAlign = ContentAlignment.MiddleCenter,
                Dock      = DockStyle.Bottom,
                Height    = 28
            };
            pnl.Controls.Add(lblVal);
            pnl.Controls.Add(lblTitle);
            pnl.Controls.Add(bar);
            return pnl;
        }

        private Panel MakeSection(string title)
        {
            var pnl = new Panel
            {
                Dock      = DockStyle.Top,
                Height    = 160,
                BackColor = MainShell.ThemePanel,
                Padding   = new Padding(0, 0, 0, 12),
                Margin    = new Padding(0, 0, 0, 14)
            };
            var lbl = new Label
            {
                Text      = title,
                Font      = new Font("微軟正黑體", 11F, FontStyle.Bold),
                ForeColor = MainShell.ThemeFore,
                Dock      = DockStyle.Top,
                Height    = 34,
                Padding   = new Padding(14, 6, 0, 0)
            };
            var sep = new Panel { Dock = DockStyle.Top, Height = 1, BackColor = Color.FromArgb(220, 225, 235) };
            pnl.Controls.Add(sep);
            pnl.Controls.Add(lbl);
            return pnl;
        }

        private DataGridView MakeMiniGrid()
        {
            var dgv = new DataGridView
            {
                Dock                  = DockStyle.Fill,
                AutoGenerateColumns   = false,
                AllowUserToAddRows    = false,
                AllowUserToDeleteRows = false,
                ReadOnly              = true,
                RowHeadersVisible     = false,
                SelectionMode         = DataGridViewSelectionMode.FullRowSelect,
                BackgroundColor       = MainShell.ThemePanel,
                BorderStyle           = BorderStyle.None,
                GridColor             = Color.FromArgb(230, 233, 240),
                Font                  = new Font("微軟正黑體", 9.5F),
                AutoSizeColumnsMode   = DataGridViewAutoSizeColumnsMode.Fill,
                EnableHeadersVisualStyles = false,
                ColumnHeadersHeight   = 30
            };
            dgv.ColumnHeadersDefaultCellStyle.BackColor = MainShell.ThemeAccent;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            dgv.ColumnHeadersDefaultCellStyle.Font      = new Font("微軟正黑體", 10F, FontStyle.Bold);
            dgv.DefaultCellStyle.BackColor = MainShell.ThemePanel;
            dgv.DefaultCellStyle.ForeColor = MainShell.ThemeFore;

            dgv.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "課程", DataPropertyName = "CourseName", FillWeight = 25 });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "標題", DataPropertyName = "Title",      FillWeight = 35 });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "截止日期", DataPropertyName = "DueDate", FillWeight = 20, DefaultCellStyle = new DataGridViewCellStyle { Format = "yyyy/MM/dd" } });
            dgv.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "提醒", DataPropertyName = "ReminderText", FillWeight = 20 });
            return dgv;
        }
    }
}
