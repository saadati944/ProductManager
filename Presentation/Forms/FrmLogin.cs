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
        private Business.UsersBusiness _usersBusiness;

        public FrmLogin(Business.UsersBusiness usersBusiness)
        {
            InitializeComponent();
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
            var user = _usersBusiness.Login(txtUserName.Text, txtPassword.Text);
            if(user == null)
            {
                MessageBox.Show("نام کاربری یا رمز عبور اشتباه است");
                return;
            }

            txtUserName.Text = txtPassword.Text = "";

            Database.LoggedInUser = user;
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
            
        }
    }
}
