using Framework.Interfaces;
using Framework.DataLayer.Models;
using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Login.Presentation.Forms
{
    public partial class FrmSignUp : Form
    {
        private readonly IDatabase _database;
        private readonly User _user = new User();
        
        public FrmSignUp(IDatabase database)
        {
            _database = database;

            InitializeComponent();
            bindingSource.DataSource = _user;
            radMale.Checked = true;
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
        private void txtPassword_MouseLeave(object sender, EventArgs e)
        {
            ((TextBox)sender).PasswordChar = '*';
        }

        private void txtPassword_MouseEnter(object sender, EventArgs e)
        {
            ((TextBox)sender).PasswordChar = '\0';
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!ValidateChildren())
                return;

            User u = (User)bindingSource.DataSource;
            _database.Save(u);
            MessageBox.Show("ثبت نام با موفقیت به پایان رسید");
            Close();
        }

        private void radMale_CheckedChanged(object sender, EventArgs e)
        {
            ((User)bindingSource.DataSource).Gender = radMale.Checked;
        }

        private void txtFirstName_Validating(object sender, CancelEventArgs e)
        {
            ValidateTextLength(sender, e, 3);
        }
        private void ValidateTextLength(object sender, CancelEventArgs e, int minLen)
        {
            if (((Control)sender).Text.Length < minLen)
            {
                errorProvider.SetError((Control)sender, "حداقل طول مجاز برای این فیلد 3 کاراکر میباشد");
                e.Cancel = true;
            }
            else
            {
                errorProvider.SetError((Control)sender, null);
                e.Cancel = false;
            }
        }

        private void txtLastName_Validating(object sender, CancelEventArgs e)
        {
            ValidateTextLength(sender, e, 3);
        }

        private void txtPassword_Validating(object sender, CancelEventArgs e)
        {
            ValidateTextLength(sender, e, 3);
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
