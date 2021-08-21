using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tappe.Forms
{
    public partial class FrmLogin : Form
    {
        public FrmLogin()
        {
            InitializeComponent();
            UpdateUsersList();
        }

        private void UpdateUsersList()
        {
            cmbUsers.Items.Clear();
            cmbUsers.Items.AddRange(Program.Container.GetInstance<Data.Database>().Users.ToArray());
            cmbUsers.SelectedIndex = 0;
        }

        private void FrmLogin_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }

        private void btnEnter_Click(object sender, EventArgs e)
        {
            if (!ValidateChildren())
                return;

            txtPassword.Text = "";
            var user = (Data.Models.User)cmbUsers.SelectedItem;

            Program.LoggedInUser = user;
            FrmMain mainform = new FrmMain();
            mainform.Shown += delegate (object se, EventArgs ev) { Hide(); };
            mainform.FormClosed += delegate (object se, FormClosedEventArgs ev)
            {
                Show();
                UpdateUsersList();
            };
            mainform.Show();
        }

        private void cmbUsers_Validating(object sender, CancelEventArgs e)
        {
            if(cmbUsers.SelectedIndex == -1)
            {
                errorProvider.SetError(cmbUsers, "یکی از کاربران را انتخاب نمایید");
                e.Cancel = true;
            }
            else
            {
                errorProvider.SetError(cmbUsers, null);
                e.Cancel = false;
            }
        }

        private void txtPassword_Validating(object sender, CancelEventArgs e)
        {
            if(cmbUsers.SelectedIndex == -1)
            {
                e.Cancel = true;
                return;
            }
            if (txtPassword.Text != ((Data.Models.User)cmbUsers.SelectedItem).Password)
            {
                errorProvider.SetError(txtPassword, "رمز عبور اشتباه میباشد");
                e.Cancel = true;
            }
            else
            {
                errorProvider.SetError(txtPassword, null);
                e.Cancel = false;
            }
        }

        private void btnSignUp_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void FrmLogin_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                btnEnter_Click(null, null);
        }
    }
}
