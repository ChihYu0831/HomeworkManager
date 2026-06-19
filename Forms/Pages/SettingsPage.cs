using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using HomeworkManager.Services;

namespace HomeworkManager.Forms.Pages
{
    public class SettingsPage : Panel
    {
        private readonly HomeworkService _service;
        private readonly MainShell _shell;

        public SettingsPage(HomeworkService service, MainShell shell)
        {
            _service = service;
            _shell   = shell;
            this.Padding   = new Padding(24);
            this.BackColor = MainShell.ThemeBg;
            Build();
        }

        private void Build()
        {
            // ── 外觀 ──────────────────────────────────────────────────
            var grpAppear = MakeGroup("🎨 外觀設定");
            grpAppear.Dock   = DockStyle.Top;
            grpAppear.Height = 100;

            var btnDark = MakeBtn(_shell.IsDark ? "☀️ 切換為淺色模式" : "🌙 切換為深色模式",
                Color.FromArgb(50, 50, 60), new Point(16, 36));
            btnDark.Click += (s, e) =>
            {
                _shell.ToggleDarkMode(!_shell.IsDark);
            };
            grpAppear.Controls.Add(btnDark);
            grpAppear.Controls.Add(new Label { Text = "切換深色 / 淺色介面主題", Location = new Point(160, 46), AutoSize = true, ForeColor = MainShell.ThemeMuted, Font = new Font("微軟正黑體", 10F) });
            this.Controls.Add(grpAppear);

            // ── 資料管理 ─────────────────────────────────────────────
            var grpData = MakeGroup("💾 資料管理");
            grpData.Dock   = DockStyle.Top;
            grpData.Height = 160;

            var btnExport = MakeBtn("📤 匯出 CSV", Color.FromArgb(0, 123, 255), new Point(16, 36));
            btnExport.Click += BtnExport_Click;
            grpData.Controls.Add(btnExport);
            grpData.Controls.Add(new Label { Text = "將所有作業資料匯出為 CSV 檔案", Location = new Point(160, 46), AutoSize = true, ForeColor = MainShell.ThemeMuted, Font = new Font("微軟正黑體", 10F) });

            var btnBackup = MakeBtn("📁 備份資料", Color.FromArgb(40, 167, 69), new Point(16, 90));
            btnBackup.Click += BtnBackup_Click;
            grpData.Controls.Add(btnBackup);
            grpData.Controls.Add(new Label { Text = "將 JSON 資料檔複製備份至指定位置", Location = new Point(160, 100), AutoSize = true, ForeColor = MainShell.ThemeMuted, Font = new Font("微軟正黑體", 10F) });

            this.Controls.Add(grpData);

            // ── 危險區 ────────────────────────────────────────────────
            var grpDanger = MakeGroup("⚠️ 危險操作");
            grpDanger.Dock   = DockStyle.Top;
            grpDanger.Height = 100;

            var btnClear = MakeBtn("🗑️ 清除所有資料", Color.FromArgb(210, 55, 55), new Point(16, 36));
            btnClear.Click += BtnClearAll_Click;
            grpDanger.Controls.Add(btnClear);
            grpDanger.Controls.Add(new Label { Text = "永久刪除所有作業資料，無法復原！", Location = new Point(176, 46), AutoSize = true, ForeColor = Color.FromArgb(210, 55, 55), Font = new Font("微軟正黑體", 10F, FontStyle.Bold) });

            this.Controls.Add(grpDanger);

            // ── 關於 ─────────────────────────────────────────────────
            var grpAbout = MakeGroup("ℹ️ 關於");
            grpAbout.Dock   = DockStyle.Top;
            grpAbout.Height = 100;
            grpAbout.Controls.Add(new Label
            {
                Text      = "學生作業管理系統  Student Homework Management System\n版本 2.0  |  .NET Framework 4.8  |  WinForms",
                Location  = new Point(16, 32),
                AutoSize  = true,
                Font      = new Font("微軟正黑體", 10F),
                ForeColor = MainShell.ThemeMuted
            });
            this.Controls.Add(grpAbout);
        }

        private GroupBox MakeGroup(string title) => new GroupBox
        {
            Text = title, Font = new Font("微軟正黑體", 10F, FontStyle.Bold),
            ForeColor = MainShell.ThemeFore, BackColor = MainShell.ThemePanel,
            Padding = new Padding(8), Margin = new Padding(0, 0, 0, 14)
        };

        private Button MakeBtn(string text, Color bg, Point loc)
        {
            var btn = new Button { Text = text, Location = loc, Size = new Size(140, 34), FlatStyle = FlatStyle.Flat, BackColor = bg, ForeColor = Color.White, Font = new Font("微軟正黑體", 10F, FontStyle.Bold), Cursor = Cursors.Hand };
            btn.FlatAppearance.BorderSize = 0;
            return btn;
        }

        private void BtnExport_Click(object sender, EventArgs e)
        {
            using (var dlg = new SaveFileDialog { Filter = "CSV 檔案|*.csv", FileName = $"作業清單_{DateTime.Today:yyyyMMdd}.csv" })
            {
                if (dlg.ShowDialog() != DialogResult.OK) return;
                var sb = new StringBuilder();
                sb.AppendLine("課程名稱,作業標題,作業內容,截止日期,狀態");
                foreach (var hw in _service.GetAll())
                    sb.AppendLine($"\"{hw.CourseName}\",\"{hw.Title}\",\"{hw.Content}\",{hw.DueDate:yyyy/MM/dd},{hw.StatusText}");
                File.WriteAllText(dlg.FileName, sb.ToString(), Encoding.UTF8);
                MessageBox.Show("✅ 已成功匯出 CSV！", "匯出成功", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void BtnBackup_Click(object sender, EventArgs e)
        {
            string src = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "HomeworkManager", "homeworks.json");
            if (!File.Exists(src)) { MessageBox.Show("找不到資料檔案。", "錯誤", MessageBoxButtons.OK, MessageBoxIcon.Error); return; }
            using (var dlg = new SaveFileDialog { Filter = "JSON 檔案|*.json", FileName = $"homeworks_backup_{DateTime.Today:yyyyMMdd}.json" })
            {
                if (dlg.ShowDialog() != DialogResult.OK) return;
                File.Copy(src, dlg.FileName, true);
                MessageBox.Show("✅ 備份成功！", "備份", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }

        private void BtnClearAll_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("確定要刪除所有資料嗎？此操作無法復原！", "警告", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                foreach (var hw in _service.GetAll().ToList())
                    _service.Delete(hw.Id);
                MessageBox.Show("已清除所有資料。", "完成", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }
}
