using System;
using System.Drawing;
using System.Windows.Forms;
using HomeworkManager.Forms.Pages;
using HomeworkManager.Services;

namespace HomeworkManager.Forms
{
    public partial class MainShell : Form
    {
        private readonly HomeworkService _service;
        private Panel _currentPage;
        private bool _isDark = false;

        // ── Theme colours ─────────────────────────────────────────────
        public static Color ThemeBg = Color.FromArgb(245, 247, 250);
        public static Color ThemePanel = Color.White;
        public static Color ThemeFore = Color.FromArgb(30, 30, 30);
        public static Color ThemeSidebar = Color.FromArgb(28, 55, 110);
        public static Color ThemeAccent = Color.FromArgb(52, 120, 246);
        public static Color ThemeMuted = Color.FromArgb(100, 110, 130);

        public bool IsDark => _isDark;

        public MainShell()
        {
            InitializeComponent();
            _service = new HomeworkService();
            BuildSidebar();

            this.Load += (s, e) => NavigateTo("dashboard");
        }

        // ── Sidebar ───────────────────────────────────────────────────
        private void BuildSidebar()
        {
            string[] icons = { "🏠", "📋", "📅", "📊", "⚙️" };
            string[] labels = { "首頁", "作業管理", "行事曆", "統計分析", "設定" };
            string[] keys = { "dashboard", "homework", "calendar", "stats", "settings" };

            for (int i = 0; i < labels.Length; i++)
            {
                var btn = new Button
                {
                    Text = $"  {icons[i]}  {labels[i]}",
                    Tag = keys[i],
                    FlatStyle = FlatStyle.Flat,
                    TextAlign = ContentAlignment.MiddleLeft,
                    Font = new Font("微軟正黑體", 11F),
                    ForeColor = Color.White,
                    BackColor = Color.Transparent,
                    Dock = DockStyle.Top,
                    Height = 52,
                    Cursor = Cursors.Hand,
                    Padding = new Padding(14, 0, 0, 0)
                };
                btn.FlatAppearance.BorderSize = 0;
                btn.FlatAppearance.MouseOverBackColor = Color.FromArgb(60, 100, 200);
                btn.Click += NavBtn_Click;
                pnlSidebar.Controls.Add(btn);
            }
            foreach (Control c in pnlSidebar.Controls)
                pnlSidebar.Controls.SetChildIndex(c, 0);
        }

        private void NavBtn_Click(object sender, EventArgs e)
            => NavigateTo(((Button)sender).Tag.ToString());

        public void NavigateTo(string key)
        {
            Panel page;
            switch (key)
            {
                case "homework": page = new HomeworkPage(_service, this); break;
                case "calendar": page = new CalendarPage(_service, this); break;
                case "stats": page = new StatsPage(_service, this); break;
                case "settings": page = new SettingsPage(_service, this); break;
                default: page = new DashboardPage(_service, this); break;
            }

            pnlContent.Controls.Clear();
            page.Dock = DockStyle.Fill;
            pnlContent.Controls.Add(page);
            _currentPage = page;

            // highlight active button
            foreach (Control c in pnlSidebar.Controls)
                if (c is Button b)
                    b.BackColor = b.Tag?.ToString() == key
                        ? Color.FromArgb(60, 100, 200)
                        : Color.Transparent;

            lblPageTitle.Text = GetPageTitle(key);
        }

        private string GetPageTitle(string key)
        {
            switch (key)
            {
                case "homework": return "作業管理";
                case "calendar": return "行事曆";
                case "stats": return "統計分析";
                case "settings": return "設定";
                default: return "首頁 Dashboard";
            }
        }

        // ── Dark mode (called from SettingsPage) ──────────────────────
        public void ToggleDarkMode(bool dark)
        {
            _isDark = dark;
            ThemeBg = dark ? Color.FromArgb(25, 25, 28) : Color.FromArgb(245, 247, 250);
            ThemePanel = dark ? Color.FromArgb(40, 40, 45) : Color.White;
            ThemeFore = dark ? Color.FromArgb(220, 220, 220) : Color.FromArgb(30, 30, 30);
            ThemeSidebar = dark ? Color.FromArgb(18, 30, 55) : Color.FromArgb(28, 55, 110);
            ThemeAccent = dark ? Color.FromArgb(70, 140, 255) : Color.FromArgb(52, 120, 246);
            ThemeMuted = dark ? Color.FromArgb(150, 155, 165) : Color.FromArgb(100, 110, 130);

            pnlSidebar.BackColor = ThemeSidebar;
            pnlTopBar.BackColor = ThemePanel;
            pnlContent.BackColor = ThemeBg;
            this.BackColor = ThemeBg;
            lblPageTitle.ForeColor = ThemeFore;
            lblAppName.ForeColor = Color.White;

            // refresh current page
            string currentKey = "dashboard";
            foreach (Control c in pnlSidebar.Controls)
                if (c is Button b && b.BackColor != Color.Transparent)
                    currentKey = b.Tag?.ToString() ?? "dashboard";
            NavigateTo(currentKey);
        }

        private void CheckTodayDue()
        {
            var list = _service.GetAll();
            var due = list.FindAll(h => !h.IsCompleted && h.DueDate.Date == DateTime.Today);
            if (due.Count > 0)
            {
                string items = string.Join("\n", due.ConvertAll(h => $"・{h.CourseName}　{h.Title}"));
                MessageBox.Show($"⚠️ 今天有 {due.Count} 筆作業到期：\n\n{items}",
                    "今日到期提醒", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
}