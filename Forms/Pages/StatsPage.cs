using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using HomeworkManager.Services;
using HomeworkManager.Models;

namespace HomeworkManager.Forms.Pages
{
    public class StatsPage : Panel
    {
        private readonly HomeworkService _service;
        private readonly MainShell _shell;

        public StatsPage(HomeworkService service, MainShell shell)
        {
            _service = service;
            _shell   = shell;
            this.Padding   = new Padding(24);
            this.BackColor = MainShell.ThemeBg;
            Build();
        }

        private void Build()
        {
            var all   = _service.GetAll();
            int total = all.Count;
            int done  = all.Count(h => h.IsCompleted);
            int undone = total - done;

            // ── 完成率圓形圖（手繪）──────────────────────────────────
            var grpPie = MakeGroup("📊 完成率");
            grpPie.Dock   = DockStyle.Top;
            grpPie.Height = 220;

            var picPie = new PictureBox { Location = new Point(16, 32), Size = new Size(180, 160), BackColor = Color.Transparent };
            picPie.Paint += (s, e) => DrawPie(e.Graphics, picPie.Size, done, undone);

            var lblDone   = MakeStatLabel($"✅ 已完成：{done} 筆",   Color.FromArgb(34, 160, 90),  new Point(210, 50));
            var lblUndone = MakeStatLabel($"🔲 未完成：{undone} 筆", MainShell.ThemeAccent,        new Point(210, 90));
            var lblRate   = MakeStatLabel($"完成率：{(total == 0 ? 0 : done * 100.0 / total):F1}%", MainShell.ThemeFore, new Point(210, 130));
            lblRate.Font  = new Font("微軟正黑體", 14F, FontStyle.Bold);

            grpPie.Controls.Add(picPie);
            grpPie.Controls.Add(lblDone);
            grpPie.Controls.Add(lblUndone);
            grpPie.Controls.Add(lblRate);
            this.Controls.Add(grpPie);

            // ── 各課程作業數量長條圖 ────────────────────────────────
            var grpBar = MakeGroup("📚 各課程作業數量");
            grpBar.Dock   = DockStyle.Fill;

            var groups = all.GroupBy(h => h.CourseName)
                            .Select(g => new { Course = g.Key, Total = g.Count(), Done = g.Count(h => h.IsCompleted) })
                            .OrderByDescending(g => g.Total)
                            .ToList();

            var picBar = new PictureBox { Dock = DockStyle.Fill, BackColor = Color.Transparent };
            picBar.Paint += (s, e) => DrawBars(e.Graphics, picBar.Size, groups.Select(g => (g.Course, g.Total, g.Done)).ToList());

            grpBar.Controls.Add(picBar);
            this.Controls.Add(grpBar);
        }

        private void DrawPie(Graphics g, Size size, int done, int undone)
        {
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            int total = done + undone;
            var rect  = new Rectangle(10, 10, size.Width - 20, size.Height - 20);

            if (total == 0)
            {
                g.FillEllipse(new SolidBrush(Color.FromArgb(200, 200, 200)), rect);
                return;
            }

            float doneAngle = 360f * done / total;
            g.FillPie(new SolidBrush(Color.FromArgb(34, 160, 90)), rect, -90, doneAngle);
            g.FillPie(new SolidBrush(MainShell.ThemeAccent),        rect, -90 + doneAngle, 360 - doneAngle);
            g.DrawEllipse(new Pen(MainShell.ThemePanel, 3), rect);
        }

        private void DrawBars(Graphics g, Size size, List<(string course, int total, int done)> data)
        {
            if (data.Count == 0) return;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;

            int maxVal   = data.Max(d => d.total);
            int barH     = Math.Min(36, (size.Height - 40) / Math.Max(data.Count, 1));
            int labelW   = 180;
            int chartW   = size.Width - labelW - 80;
            int startY   = 20;
            var fontLbl  = new Font("微軟正黑體", 9F);
            var fontNum  = new Font("微軟正黑體", 9F, FontStyle.Bold);
            var brushFg  = new SolidBrush(MainShell.ThemeFore);

            for (int i = 0; i < data.Count; i++)
            {
                var (course, total, done) = data[i];
                int y = startY + i * (barH + 10);

                // label
                g.DrawString(course, fontLbl, brushFg, new RectangleF(0, y + 4, labelW - 8, barH), new StringFormat { Alignment = StringAlignment.Far });

                // bg bar
                var bgRect = new RectangleF(labelW, y, chartW, barH);
                g.FillRectangle(new SolidBrush(Color.FromArgb(220, 225, 235)), bgRect);

                // total bar
                float totalW = maxVal == 0 ? 0 : chartW * total / maxVal;
                g.FillRectangle(new SolidBrush(Color.FromArgb(180, MainShell.ThemeAccent)), new RectangleF(labelW, y, totalW, barH));

                // done bar
                float doneW = maxVal == 0 ? 0 : chartW * done / maxVal;
                g.FillRectangle(new SolidBrush(Color.FromArgb(34, 160, 90)), new RectangleF(labelW, y, doneW, barH));

                // number
                g.DrawString($"{done}/{total}", fontNum, brushFg, labelW + totalW + 6, y + 8);
            }

            // legend
            g.FillRectangle(new SolidBrush(Color.FromArgb(34, 160, 90)), labelW, size.Height - 22, 14, 14);
            g.DrawString("已完成", fontLbl, brushFg, labelW + 18, size.Height - 22);
            g.FillRectangle(new SolidBrush(Color.FromArgb(180, MainShell.ThemeAccent)), labelW + 80, size.Height - 22, 14, 14);
            g.DrawString("未完成", fontLbl, brushFg, labelW + 98, size.Height - 22);
        }

        private GroupBox MakeGroup(string title) => new GroupBox
        {
            Text = title, Font = new Font("微軟正黑體", 10F, FontStyle.Bold),
            ForeColor = MainShell.ThemeFore, BackColor = MainShell.ThemePanel, Padding = new Padding(8)
        };

        private Label MakeStatLabel(string text, Color color, Point loc) => new Label
        {
            Text = text, ForeColor = color, Location = loc, AutoSize = true,
            Font = new Font("微軟正黑體", 11F, FontStyle.Bold)
        };
    }
}
