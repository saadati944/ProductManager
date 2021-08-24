using DataLayer;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Presentation.Forms
{
    public abstract partial class FrmGridView : Form
    {
        protected readonly Database _database;
        protected readonly Business.Settings _settings;
        protected readonly List<ColumnSelectInfo> _columnSettings = new List<ColumnSelectInfo>();

        public FrmGridView(Database database, Business.Settings settings)
        {
            TopLevel = false;
            InitializeComponent();
            _database = database;
            _settings = settings;
        }


        protected void ShowCustomizeWindow()
        {
            var frmSelectColumns = new FrmSelectColumns(_columnSettings);
            btnCustomize.Visible = dataGridView.Visible = false;
            frmSelectColumns.Left = 30;
            frmSelectColumns.Width = pnlGrid.Width - 60;
            frmSelectColumns.Top = 30;
            frmSelectColumns.Height = pnlGrid.Height - 60;
            frmSelectColumns.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            frmSelectColumns.FormClosed += FrmSelectColumns_FormClosed;
            pnlGrid.Controls.Add(frmSelectColumns);
            pnlGrid.Resize += frmSelectColumns.FrmSelectColumns_ResizeEnd;
            frmSelectColumns.Show();
            frmSelectColumns.FrmSelectColumns_ResizeEnd(null, null);
            frmSelectColumns.BringToFront();
        }

        protected bool IsVIsibleColumn(string columnName, bool def = true)
        {
            foreach (var x in _columnSettings)
                if (x.SettingsKey == columnName)
                    return x.Checked;
            return def;
        }

        protected void LoadColumnSettings()
        {
            foreach (var x in _columnSettings)
                x.Checked = _settings.GetSetting(x.SettingsKey, x.Checked.ToString()) == "True";
        }

        private void FrmSelectColumns_FormClosed(object sender, FormClosedEventArgs e)
        {
            btnCustomize.Visible = dataGridView.Visible = true;
            UpdateData();
        }

        private void FrmGridView_LocationChanged(object sender, EventArgs e)
        {
            CheckDimensions();
        }

        private void FrmGridView_SizeChanged(object sender, EventArgs e)
        {
            CheckDimensions();
        }

        public void SetTitle(string title)
        {
            Text = title;
        }

        public void CheckDimensions()
        {
            if (WindowState == FormWindowState.Maximized)
                return;

            if (Width > Parent.Width)
                Width = Parent.Width;
            if (Height > Parent.Height)
                Height = Parent.Height;

            if (Left < 0)
                Left = 0;
            else if (Left > Parent.Width - Width)
                Left = Parent.Width - Width;

            if (Top < 0)
                Top = 0;
            else if (Top > Parent.Height - Height)
                Top = Parent.Height - Height;
        }
        public abstract void UpdateData();

        private void FrmGridView_Activated(object sender, EventArgs e)
        {
            BringToFront();
        }

        private void FrmGridView_Load(object sender, EventArgs e)
        {
            BringToFront();
        }

        private void FrmGridView_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                Close();
        }


        private void btnCustomize_Click(object sender, EventArgs e)
        {
            ShowCustomizeWindow();
        }
    }
}
