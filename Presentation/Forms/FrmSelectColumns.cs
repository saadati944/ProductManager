using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

namespace Presentation.Forms
{
    public partial class FrmSelectColumns : Form
    {
        private readonly Business.Settings _settings;
        private List<ColumnSelectInfo> _columns;

        public List<ColumnSelectInfo> Columns { get { return _columns; } }

        public FrmSelectColumns(List<ColumnSelectInfo> columns)
        {
            InitializeComponent();
            _settings = Utilities.IOC.Container.GetInstance<Business.Settings>();
            _columns = columns;
            TopLevel = false;
            AddCheckboxes();
        }
        private void AddCheckboxes()
        {
            SuspendLayout();
            foreach (var x in Columns)
                AddCheckBox(x);
            FrmSelectColumns_ResizeEnd(null, null);
            ResumeLayout();
        }

        private void AddCheckBox(ColumnSelectInfo c)
        {
            CheckBox chbox = new CheckBox();
            chbox.Text = c.DisplayName;
            chbox.Checked = c.Checked;
            chbox.DataBindings.Add("Checked", c, "Checked");
            chbox.CheckedChanged += Chbox_CheckedChanged;
            chbox.FlatStyle = FlatStyle.Flat;
            chbox.Dock = DockStyle.Top;
            chbox.Height = 25;
            chbox.RightToLeft = RightToLeft.Yes;
            chbox.TextAlign = ContentAlignment.MiddleCenter;
            pnlCheckBoxes.Controls.Add(chbox);
            chbox.BringToFront();
        }


        private void Chbox_CheckedChanged(object sender, EventArgs e)
        {
            foreach (var x in Columns)
                if (x.DisplayName == ((CheckBox)sender).Text)
                    continue;
                else if (x.Checked)
                    return;

            ((CheckBox)sender).Checked = true;
        }

        public void FrmSelectColumns_ResizeEnd(object sender, EventArgs e)
        {
            int pad = (Width - 150) / 2;
            Padding padding = new Padding(pad, 0, pad, 0);
            foreach (Control x in pnlCheckBoxes.Controls)
                x.Padding = padding;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            foreach (var x in Columns)
                _settings.SetSetting(x.SettingsKey, x.Checked.ToString());
            Close();
        }
    }
    public class ColumnSelectInfo
    {
        public string SettingsKey { get; set; }
        public string DisplayName { get; set; }
        public bool Checked { get; set; }
    }
}
