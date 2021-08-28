using Framework.DataLayer;
using Framework.DataLayer.Models;
using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

namespace Login.Presentation.Forms
{
    public partial class FrmLogin : Form
    {
        private readonly Interfaces.IUsersBusiness _usersBusiness;
        private readonly Framework.Interfaces.IFormFactory _formFactory;

        public FrmLogin(Interfaces.IUsersBusiness usersBusiness, Framework.Interfaces.IFormFactory formFactory)
        {
            InitializeComponent();
            _formFactory = formFactory;
            _usersBusiness = usersBusiness;
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

            var user = _usersBusiness.Login(txtUserName.Text, txtPassword.Text);
            if(user == null)
            {
                MessageBox.Show("نام کاربری یا رمز عبور اشتباه است");
                return;
            }

            txtUserName.Text = txtPassword.Text = "";

            Framework.Utilities.LoggedInUser.User = user;
            FrmMain mainform = _formFactory.CreateForm<FrmMain>();
            mainform.Shown += delegate (object se, EventArgs ev) { Hide(); };
            mainform.FormClosed += delegate (object se, FormClosedEventArgs ev)
            {
                Show();
            };
            mainform.Show();
        }

        private void txtPassword_Validating(object sender, CancelEventArgs e)
        {
            if(String.IsNullOrEmpty(txtPassword.Text))
            {
                errorProvider.SetError(txtPassword, "رمز عبور را وارد کنید");
                e.Cancel = true;
            }
            else
            {
                errorProvider.SetError(txtPassword, null);
                e.Cancel = false;
            }
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void FrmLogin_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                btnClose_Click(null, null);
        }

        private void txtUserName_Validating(object sender, CancelEventArgs e)
        {
            if (String.IsNullOrEmpty(txtUserName.Text))
            {
                errorProvider.SetError(txtUserName, "نام کاربری را وارد نمایید");
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
