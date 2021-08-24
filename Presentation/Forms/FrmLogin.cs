using DataLayer;
using DataLayer.Models;
using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

namespace Presentation.Forms
{
    public partial class FrmLogin : Form
    {
        private Database _database;
        private User _user;

        public FrmLogin()
        {
            InitializeComponent();
            _database = Utilities.IOC.Container.GetInstance<Database>();
            SetErrorProviderPadding(this, errorProvider, 10);
        }
        private void SetErrorProviderPadding(Control container, ErrorProvider errorProvider, int value, bool children = false)
        {
            foreach (Control x in container.Controls)
            {
                errorProvider.SetIconPadding(x, value);
                if (children)
                    SetErrorProviderPadding(x, errorProvider, value, true);
            }
        }

        private void FrmLogin_FormClosing(object sender, FormClosingEventArgs e)
        {
            Application.Exit();
        }

        private void btnEnter_Click(object sender, EventArgs e)
        {
            if (!ValidateChildren())
                return;

            txtUserName.Text = txtPassword.Text = "";
            //var user = (User)cmbUsers.SelectedItem;

            Database.LoggedInUser = _user;
            FrmMain mainform = Utilities.IOC.Container.GetInstance<FrmMain>();
            mainform.Shown += delegate (object se, EventArgs ev) { Hide(); };
            mainform.FormClosed += delegate (object se, FormClosedEventArgs ev)
            {
                Show();
            };
            mainform.Show();
        }

        private void txtPassword_Validating(object sender, CancelEventArgs e)
        {
            if (_user == null)
            {
                e.Cancel = true;
                return;
            }
            if (txtPassword.Text != _user.Password)
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

        private void txtUserName_Validating(object sender, CancelEventArgs e)
        {
            _user = _database.Users.FirstOrDefault(x => x.FirstName == txtUserName.Text);
            if (_user == null)
            {
                errorProvider.SetError(txtUserName, "نام کاربری نا معتبر");
                e.Cancel = true;
            }
            else
            {
                errorProvider.SetError(txtUserName, null);
                e.Cancel = false;
            }
        }
    }
}
