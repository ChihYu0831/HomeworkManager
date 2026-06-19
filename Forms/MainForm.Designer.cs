namespace HomeworkManager.Forms
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();

            // ── Controls ─────────────────────────────────────────────
            this.lblTitle = new System.Windows.Forms.Label();
            this.grpInput = new System.Windows.Forms.GroupBox();
            this.lblCourse = new System.Windows.Forms.Label();
            this.cmbCourse = new System.Windows.Forms.ComboBox();
            this.lblHwTitle = new System.Windows.Forms.Label();
            this.txtTitle = new System.Windows.Forms.TextBox();
            this.lblContent = new System.Windows.Forms.Label();
            this.txtContent = new System.Windows.Forms.TextBox();
            this.lblDueDate = new System.Windows.Forms.Label();
            this.dtpDueDate = new System.Windows.Forms.DateTimePicker();
            this.lblStatus = new System.Windows.Forms.Label();
            this.cmbStatus = new System.Windows.Forms.ComboBox();
            this.pnlButtons = new System.Windows.Forms.Panel();
            this.btnAdd = new System.Windows.Forms.Button();
            this.btnUpdate = new System.Windows.Forms.Button();
            this.btnDelete = new System.Windows.Forms.Button();
            this.btnComplete = new System.Windows.Forms.Button();
            this.btnClear = new System.Windows.Forms.Button();
            this.grpSearch = new System.Windows.Forms.GroupBox();
            this.lblSearch = new System.Windows.Forms.Label();
            this.txtSearch = new System.Windows.Forms.TextBox();
            this.btnSearch = new System.Windows.Forms.Button();
            this.btnShowAll = new System.Windows.Forms.Button();
            this.grpGrid = new System.Windows.Forms.GroupBox();
            this.dgvHomework = new System.Windows.Forms.DataGridView();
            this.lblStats = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);

            this.grpInput.SuspendLayout();
            this.pnlButtons.SuspendLayout();
            this.grpSearch.SuspendLayout();
            this.grpGrid.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgvHomework)).BeginInit();
            this.SuspendLayout();

            // ── Form ─────────────────────────────────────────────────
            this.Text = "學生作業管理系統  Student Homework Management System";
            this.Size = new System.Drawing.Size(980, 740);
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.MinimumSize = new System.Drawing.Size(900, 680);
            this.Font = new System.Drawing.Font("微軟正黑體", 10F);
            this.BackColor = System.Drawing.Color.FromArgb(245, 247, 250);

            // ── lblTitle ──────────────────────────────────────────────
            this.lblTitle.Text = "📚 學生作業管理系統";
            this.lblTitle.Font = new System.Drawing.Font("微軟正黑體", 18F, System.Drawing.FontStyle.Bold);
            this.lblTitle.ForeColor = System.Drawing.Color.FromArgb(30, 80, 160);
            this.lblTitle.Location = new System.Drawing.Point(16, 12);
            this.lblTitle.Size = new System.Drawing.Size(500, 38);
            this.lblTitle.AutoSize = false;

            // ── grpInput ─────────────────────────────────────────────
            this.grpInput.Text = "輸入區";
            this.grpInput.Font = new System.Drawing.Font("微軟正黑體", 10F, System.Drawing.FontStyle.Bold);
            this.grpInput.Location = new System.Drawing.Point(12, 56);
            this.grpInput.Size = new System.Drawing.Size(944, 200);
            this.grpInput.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this.grpInput.BackColor = System.Drawing.Color.White;

            // Course
            this.lblCourse.Text = "課程名稱：";
            this.lblCourse.Location = new System.Drawing.Point(16, 32);
            this.lblCourse.Size = new System.Drawing.Size(80, 24);
            this.lblCourse.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

            this.cmbCourse.Location = new System.Drawing.Point(100, 30);
            this.cmbCourse.Size = new System.Drawing.Size(200, 28);
            this.cmbCourse.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDown;
            this.cmbCourse.Items.AddRange(new object[] { "數學", "英文", "程式設計", "資料結構", "作業系統", "網路概論" });

            // Title
            this.lblHwTitle.Text = "作業標題：";
            this.lblHwTitle.Location = new System.Drawing.Point(320, 32);
            this.lblHwTitle.Size = new System.Drawing.Size(80, 24);
            this.lblHwTitle.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

            this.txtTitle.Location = new System.Drawing.Point(404, 30);
            this.txtTitle.Size = new System.Drawing.Size(280, 28);

            // Due date
            this.lblDueDate.Text = "截止日期：";
            this.lblDueDate.Location = new System.Drawing.Point(700, 32);
            this.lblDueDate.Size = new System.Drawing.Size(80, 24);
            this.lblDueDate.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

            this.dtpDueDate.Location = new System.Drawing.Point(784, 30);
            this.dtpDueDate.Size = new System.Drawing.Size(148, 28);
            this.dtpDueDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;

            // Content
            this.lblContent.Text = "作業內容：";
            this.lblContent.Location = new System.Drawing.Point(16, 72);
            this.lblContent.Size = new System.Drawing.Size(80, 24);
            this.lblContent.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

            this.txtContent.Location = new System.Drawing.Point(100, 70);
            this.txtContent.Size = new System.Drawing.Size(700, 70);
            this.txtContent.Multiline = true;
            this.txtContent.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;

            // Status
            this.lblStatus.Text = "狀　　態：";
            this.lblStatus.Location = new System.Drawing.Point(816, 72);
            this.lblStatus.Size = new System.Drawing.Size(80, 24);
            this.lblStatus.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

            this.cmbStatus.Location = new System.Drawing.Point(816, 98);
            this.cmbStatus.Size = new System.Drawing.Size(116, 28);
            this.cmbStatus.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbStatus.Items.AddRange(new object[] { "未完成", "已完成" });
            this.cmbStatus.SelectedIndex = 0;

            // ── pnlButtons ───────────────────────────────────────────
            this.pnlButtons.Location = new System.Drawing.Point(100, 152);
            this.pnlButtons.Size = new System.Drawing.Size(700, 40);

            // btnAdd
            this.btnAdd.Text = "➕ 新增";
            this.btnAdd.Size = new System.Drawing.Size(110, 34);
            this.btnAdd.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnAdd.BackColor = System.Drawing.Color.FromArgb(40, 167, 69);
            this.btnAdd.ForeColor = System.Drawing.Color.White;
            this.btnAdd.Font = new System.Drawing.Font("微軟正黑體", 10F, System.Drawing.FontStyle.Bold);
            this.btnAdd.FlatAppearance.BorderSize = 0;
            this.btnAdd.Cursor = System.Windows.Forms.Cursors.Hand;

            // btnUpdate
            this.btnUpdate.Text = "✏️ 修改";
            this.btnUpdate.Size = new System.Drawing.Size(110, 34);
            this.btnUpdate.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnUpdate.BackColor = System.Drawing.Color.FromArgb(0, 123, 255);
            this.btnUpdate.ForeColor = System.Drawing.Color.White;
            this.btnUpdate.Font = new System.Drawing.Font("微軟正黑體", 10F, System.Drawing.FontStyle.Bold);
            this.btnUpdate.FlatAppearance.BorderSize = 0;
            this.btnUpdate.Cursor = System.Windows.Forms.Cursors.Hand;

            // btnDelete
            this.btnDelete.Text = "🗑️ 刪除";
            this.btnDelete.Size = new System.Drawing.Size(110, 34);
            this.btnDelete.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnDelete.BackColor = System.Drawing.Color.FromArgb(220, 53, 69);
            this.btnDelete.ForeColor = System.Drawing.Color.White;
            this.btnDelete.Font = new System.Drawing.Font("微軟正黑體", 10F, System.Drawing.FontStyle.Bold);
            this.btnDelete.FlatAppearance.BorderSize = 0;
            this.btnDelete.Cursor = System.Windows.Forms.Cursors.Hand;

            // btnComplete
            this.btnComplete.Text = "✅ 標記完成";
            this.btnComplete.Size = new System.Drawing.Size(110, 34);
            this.btnComplete.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnComplete.BackColor = System.Drawing.Color.FromArgb(102, 16, 242);
            this.btnComplete.ForeColor = System.Drawing.Color.White;
            this.btnComplete.Font = new System.Drawing.Font("微軟正黑體", 10F, System.Drawing.FontStyle.Bold);
            this.btnComplete.FlatAppearance.BorderSize = 0;
            this.btnComplete.Cursor = System.Windows.Forms.Cursors.Hand;

            // btnClear
            this.btnClear.Text = "🔄 清除";
            this.btnClear.Size = new System.Drawing.Size(110, 34);
            this.btnClear.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnClear.BackColor = System.Drawing.Color.FromArgb(108, 117, 125);
            this.btnClear.ForeColor = System.Drawing.Color.White;
            this.btnClear.Font = new System.Drawing.Font("微軟正黑體", 10F, System.Drawing.FontStyle.Bold);
            this.btnClear.FlatAppearance.BorderSize = 0;
            this.btnClear.Cursor = System.Windows.Forms.Cursors.Hand;

            this.btnAdd.Location = new System.Drawing.Point(0, 3);
            this.btnUpdate.Location = new System.Drawing.Point(118, 3);
            this.btnDelete.Location = new System.Drawing.Point(236, 3);
            this.btnComplete.Location = new System.Drawing.Point(354, 3);
            this.btnClear.Location = new System.Drawing.Point(488, 3);

            this.pnlButtons.Controls.AddRange(new System.Windows.Forms.Control[]{
                this.btnAdd, this.btnUpdate, this.btnDelete, this.btnComplete, this.btnClear });

            this.grpInput.Controls.AddRange(new System.Windows.Forms.Control[]{
                this.lblCourse, this.cmbCourse,
                this.lblHwTitle, this.txtTitle,
                this.lblDueDate, this.dtpDueDate,
                this.lblContent, this.txtContent,
                this.lblStatus, this.cmbStatus,
                this.pnlButtons });

            // ── grpSearch ─────────────────────────────────────────────
            this.grpSearch.Text = "搜尋區";
            this.grpSearch.Font = new System.Drawing.Font("微軟正黑體", 10F, System.Drawing.FontStyle.Bold);
            this.grpSearch.Location = new System.Drawing.Point(12, 268);
            this.grpSearch.Size = new System.Drawing.Size(944, 62);
            this.grpSearch.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this.grpSearch.BackColor = System.Drawing.Color.White;

            this.lblSearch.Text = "搜尋：";
            this.lblSearch.Location = new System.Drawing.Point(16, 24);
            this.lblSearch.Size = new System.Drawing.Size(55, 24);
            this.lblSearch.TextAlign = System.Drawing.ContentAlignment.MiddleRight;

            this.txtSearch.Location = new System.Drawing.Point(74, 22);
            this.txtSearch.Size = new System.Drawing.Size(300, 28);

            this.btnSearch.Text = "🔍 搜尋";
            this.btnSearch.Location = new System.Drawing.Point(382, 20);
            this.btnSearch.Size = new System.Drawing.Size(100, 32);
            this.btnSearch.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSearch.BackColor = System.Drawing.Color.FromArgb(0, 123, 255);
            this.btnSearch.ForeColor = System.Drawing.Color.White;
            this.btnSearch.Font = new System.Drawing.Font("微軟正黑體", 10F, System.Drawing.FontStyle.Bold);
            this.btnSearch.FlatAppearance.BorderSize = 0;
            this.btnSearch.Cursor = System.Windows.Forms.Cursors.Hand;

            this.btnShowAll.Text = "📋 顯示全部";
            this.btnShowAll.Location = new System.Drawing.Point(490, 20);
            this.btnShowAll.Size = new System.Drawing.Size(110, 32);
            this.btnShowAll.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnShowAll.BackColor = System.Drawing.Color.FromArgb(108, 117, 125);
            this.btnShowAll.ForeColor = System.Drawing.Color.White;
            this.btnShowAll.Font = new System.Drawing.Font("微軟正黑體", 10F, System.Drawing.FontStyle.Bold);
            this.btnShowAll.FlatAppearance.BorderSize = 0;
            this.btnShowAll.Cursor = System.Windows.Forms.Cursors.Hand;

            this.grpSearch.Controls.AddRange(new System.Windows.Forms.Control[]{
                this.lblSearch, this.txtSearch, this.btnSearch, this.btnShowAll });

            // ── grpGrid ───────────────────────────────────────────────
            this.grpGrid.Text = "作業清單";
            this.grpGrid.Font = new System.Drawing.Font("微軟正黑體", 10F, System.Drawing.FontStyle.Bold);
            this.grpGrid.Location = new System.Drawing.Point(12, 340);
            this.grpGrid.Size = new System.Drawing.Size(944, 350);
            this.grpGrid.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this.grpGrid.BackColor = System.Drawing.Color.White;

            // DataGridView
            this.dgvHomework.Location = new System.Drawing.Point(10, 26);
            this.dgvHomework.Size = new System.Drawing.Size(922, 282);
            this.dgvHomework.Anchor = System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this.dgvHomework.AllowUserToAddRows = false;
            this.dgvHomework.AllowUserToDeleteRows = false;
            this.dgvHomework.ReadOnly = true;
            this.dgvHomework.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.dgvHomework.MultiSelect = false;
            this.dgvHomework.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dgvHomework.RowHeadersVisible = false;
            this.dgvHomework.BackgroundColor = System.Drawing.Color.White;
            this.dgvHomework.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.dgvHomework.Font = new System.Drawing.Font("微軟正黑體", 9.5F);
            this.dgvHomework.RowTemplate.Height = 30;
            this.dgvHomework.ColumnHeadersHeight = 34;
            this.dgvHomework.ColumnHeadersDefaultCellStyle.Font =
                new System.Drawing.Font("微軟正黑體", 10F, System.Drawing.FontStyle.Bold);
            this.dgvHomework.ColumnHeadersDefaultCellStyle.BackColor = System.Drawing.Color.FromArgb(30, 80, 160);
            this.dgvHomework.ColumnHeadersDefaultCellStyle.ForeColor = System.Drawing.Color.White;
            this.dgvHomework.EnableHeadersVisualStyles = false;
            this.dgvHomework.GridColor = System.Drawing.Color.FromArgb(220, 225, 235);

            this.grpGrid.Controls.Add(this.dgvHomework);

            // lblStats
            this.lblStats.Location = new System.Drawing.Point(12, 698);
            this.lblStats.Size = new System.Drawing.Size(944, 28);
            this.lblStats.Anchor = System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left | System.Windows.Forms.AnchorStyles.Right;
            this.lblStats.Font = new System.Drawing.Font("微軟正黑體", 9.5F);
            this.lblStats.ForeColor = System.Drawing.Color.FromArgb(80, 80, 80);

            // ── Add to Form ───────────────────────────────────────────
            this.Controls.AddRange(new System.Windows.Forms.Control[]{
                this.lblTitle, this.grpInput, this.grpSearch, this.grpGrid, this.lblStats });

            this.grpInput.ResumeLayout(false);
            this.pnlButtons.ResumeLayout(false);
            this.grpSearch.ResumeLayout(false);
            this.grpGrid.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.dgvHomework)).EndInit();
            this.ResumeLayout(false);
        }

        // ── Field declarations ────────────────────────────────────────
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.GroupBox grpInput;
        private System.Windows.Forms.Label lblCourse;
        private System.Windows.Forms.ComboBox cmbCourse;
        private System.Windows.Forms.Label lblHwTitle;
        private System.Windows.Forms.TextBox txtTitle;
        private System.Windows.Forms.Label lblContent;
        private System.Windows.Forms.TextBox txtContent;
        private System.Windows.Forms.Label lblDueDate;
        private System.Windows.Forms.DateTimePicker dtpDueDate;
        private System.Windows.Forms.Label lblStatus;
        private System.Windows.Forms.ComboBox cmbStatus;
        private System.Windows.Forms.Panel pnlButtons;
        private System.Windows.Forms.Button btnAdd;
        private System.Windows.Forms.Button btnUpdate;
        private System.Windows.Forms.Button btnDelete;
        private System.Windows.Forms.Button btnComplete;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.GroupBox grpSearch;
        private System.Windows.Forms.Label lblSearch;
        private System.Windows.Forms.TextBox txtSearch;
        private System.Windows.Forms.Button btnSearch;
        private System.Windows.Forms.Button btnShowAll;
        private System.Windows.Forms.GroupBox grpGrid;
        private System.Windows.Forms.DataGridView dgvHomework;
        private System.Windows.Forms.Label lblStats;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}