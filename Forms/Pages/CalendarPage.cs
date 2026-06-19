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
        private ListBox lstDayItems;
        private Label lblDayTitle;

        public CalendarPage(HomeworkService service, MainShell shell)
        {
            _service = service;
            _shell   = shell;
            this.Padding   = new Padding(20);
            this.BackColor = MainShell.ThemeBg;
            Build();
            HighlightDueDates();
        }

        private void Build()
        {
            var pnlLeft = new Panel { Dock = DockStyle.Left, Width = 260, BackColor = MainShell.ThemePanel, Padding = new Padding(12) };

            cal = new MonthCalendar
            {
                Location        = new Point(12, 12),
                MaxSelectionCount = 1,
                ShowToday       = true,
                Font            = new Font("微軟正黑體", 9.5F)
            };
            cal.DateSelected += Cal_DateSelected;
            pnlLeft.Controls.Add(cal);

            var legend = new Label
            {
                Text      = "🔴 今天到期\n🟡 即將到期（3天內）\n⛔ 已逾期\n✅ 已完成",
                Font      = new Font("微軟正黑體", 9F),
                ForeColor = MainShell.ThemeMuted,
                Location  = new Point(12, 220),
                Size      = new Size(236, 90),
                AutoSize  = false
            };
            pnlLeft.Controls.Add(legend);

            this.Controls.Add(pnlLeft);

            var pnlRight = new Panel { Dock = DockStyle.Fill, BackColor = MainShell.ThemePanel, Padding = new Padding(16) };

            lblDayTitle = new Label
            {
                Text      = "請點選日期查看作業",
                Font      = new Font("微軟正黑體", 13F, FontStyle.Bold),
                ForeColor = MainShell.ThemeFore,
                Dock      = DockStyle.Top,
                Height    = 38
            };

            lstDayItems = new ListBox
            {
                Dock      = DockStyle.Fill,
                Font      = new Font("微軟正黑體", 10.5F),
                BackColor = MainShell.ThemePanel,
                ForeColor = MainShell.ThemeFore,
                BorderStyle = BorderStyle.None,
                ItemHeight  = 30
            };

            pnlRight.Controls.Add(lstDayItems);
            pnlRight.Controls.Add(lblDayTitle);
            this.Controls.Add(pnlRight);
        }

        private void HighlightDueDates()
        {
            // MonthCalendar 的 BoldedDates 可標記日期
            var all = _service.GetAll();
            var dates = all.Where(h => !h.IsCompleted)
                           .Select(h => h.DueDate.Date)
                           .Distinct()
                           .ToArray();
            cal.BoldedDates = dates;
            cal.UpdateBoldedDates();
        }

        private void Cal_DateSelected(object sender, DateRangeEventArgs e)
        {
            var selected = e.Start.Date;
            lblDayTitle.Text = $"📅 {selected:yyyy年MM月dd日} 的作業";

            var list = _service.GetAll()
                .Where(h => h.DueDate.Date == selected)
                .OrderBy(h => h.IsCompleted)
                .ToList();

            lstDayItems.Items.Clear();
            if (list.Count == 0)
            {
                lstDayItems.Items.Add("這天沒有作業到期 🎉");
                return;
            }

            foreach (var hw in list)
            {
                string status = hw.IsCompleted ? "✅" : (hw.DueDate.Date < DateTime.Today ? "⛔" : "🔴");
                lstDayItems.Items.Add($"  {status}  {hw.CourseName}　{hw.Title}");
            }
        }
    }
}
