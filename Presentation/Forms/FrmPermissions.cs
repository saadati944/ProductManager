using Business;
using DataLayer;
using DataLayer.Models;
using System;
using System.Windows.Forms;

namespace Presentation.Forms
{
    public partial class FrmPermissions : Form
    {
        private readonly Permissions _permissions;
        private int _userref;

        public FrmPermissions(Permissions permissions, Database database)
        {
            InitializeComponent();
            _permissions = permissions;
            foreach (var x in database.Users)
                if (x.Id != Database.LoggedInUser.Id)
                    cmbUsers.Items.Add(x);
        }

        private void AddCheckboxes()
        {
            SuspendLayout();
            foreach (var x in _permissions.GetAllPermissions(_userref))
            {
                AddCheckBox(x);
            }
            ResumeLayout();
        }
        private void AddCheckBox(PermissionViewModel pvm)
        {
            CheckBox chbox = new CheckBox();
            chbox.Text = pvm.DisplayName;
            chbox.Checked = pvm.Permitted;
            chbox.CheckedChanged += Chbox_CheckedChanged;
            chbox.Tag = pvm.Key;
            chbox.FlatStyle = FlatStyle.Flat;
            chbox.Font = Font;
            chbox.Dock = DockStyle.Top;
            chbox.Height = 25;
            chbox.RightToLeft = RightToLeft.Yes;
            pnlCheckboxes.Controls.Add(chbox);
            chbox.SendToBack();
        }

        private void Chbox_CheckedChanged(object sender, EventArgs e)
        {
            var chbox = (CheckBox)sender;
            _permissions.SetPermission((string)chbox.Tag, _userref, chbox.Checked);
        }

        private void cmbUsers_SelectedIndexChanged(object sender, EventArgs e)
        {
            pnlCheckboxes.Controls.Clear();
            if (cmbUsers.SelectedIndex == -1)
            {
                pnlCheckboxes.Enabled = false;
                return;
            }
            else
            {
                pnlCheckboxes.Enabled = true;
                _userref = ((User)cmbUsers.SelectedItem).Id;
                AddCheckboxes();
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
