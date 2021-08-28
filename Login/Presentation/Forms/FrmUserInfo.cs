using Framework.Interfaces;
using Framework.DataLayer.Models;
using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace Login.Presentation.Forms
{
    public partial class FrmUserInfo : Form
    {
        private readonly IDatabase _database;
        private User _user;
        public FrmUserInfo(IDatabase database)
        {
            _database = database;
            InitializeComponent();
            var defaultUser = Framework.Utilities.LoggedInUser.User;

            _user = new User
            {
                Id = defaultUser.Id,
                UserName = txtFirstName.Text = defaultUser.UserName,
                FullName = txtLastName.Text = defaultUser.FullName,
                Age = (int)(numAge.Value = defaultUser.Age),
                Gender = radMale.Checked = defaultUser.Gender,
                Password = defaultUser.Password
            };
            SetErrorProviderPadding(this, errorProvider, 10, true);
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

        private void Bind()
        {
            _user.UserName = txtFirstName.Text;
            _user.FullName = txtLastName.Text;
            _user.Age = (int)numAge.Value;
            _user.Gender = radMale.Checked;
        }

        private void radFemale_CheckedChanged(object sender, EventArgs e)
        {
            radMale.Checked = !radFemale.Checked;
        }

        private void radMale_CheckedChanged(object sender, EventArgs e)
        {
            radFemale.Checked = !radMale.Checked;
        }

        private void btnUpdateUserData_Click(object sender, EventArgs e)
        {
            grpPassword.Enabled = false;
            if (!ValidateChildren(ValidationConstraints.Enabled))
            {
                grpPassword.Enabled = true;
                return;
            }

            Bind();

            _database.Save(_user);
            Framework.Utilities.LoggedInUser.User = _user;
            grpPassword.Enabled = true;
            MessageBox.Show("اطلاعات جدید با موفقیت ثبت شد");
        }

        private void txtNewPassAgain_Validating(object sender, CancelEventArgs e)
        {
            ValidateTextLength(sender, e, 3);
            if (e.Cancel)
                return;
            if (txtNewPassAgain.Text != txtNewPass.Text)
            {
                errorProvider.SetError(txtNewPassAgain, "رمز عبور جدید همخوانی ندارد");
                e.Cancel = true;
            }
            else
            {
                errorProvider.SetError(txtNewPassAgain, null);
                e.Cancel = false;
            }
        }

        private void txtPassword_Validating(object sender, CancelEventArgs e)
        {
            if (txtPassword.Text != _user.Password)
            {
                errorProvider.SetError(txtPassword, "مز عبور اشتباه وارد شده است");
                e.Cancel = true;
            }
            else
            {
                errorProvider.SetError(txtPassword, null);
                e.Cancel = false;
            }
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

        private void btnUpdatePassword_Click(object sender, EventArgs e)
        {
            grpUserInfo.Enabled = false;
            if (!ValidateChildren(ValidationConstraints.Enabled))
            {
                grpUserInfo.Enabled = true;
                return;
            }
            _user.Password = txtNewPass.Text;
            txtNewPass.Text = txtNewPassAgain.Text = txtPassword.Text = "";
            _database.Save(_user);
            grpUserInfo.Enabled = true;
            MessageBox.Show("رمز عبور با موفقیت تغییر یافت");
        }

        private void txtPassword_MouseEnter(object sender, EventArgs e)
        {
            ((TextBox)sender).PasswordChar = '\0';
        }

        private void txtPassword_MouseLeave(object sender, EventArgs e)
        {
            ((TextBox)sender).PasswordChar = '*';
        }

        private void txtLastName_Validating(object sender, CancelEventArgs e)
        {
            ValidateTextLength(sender, e, 3);
        }

        private void txtNewPass_Validating(object sender, CancelEventArgs e)
        {
            ValidateTextLength(sender, e, 3);
        }
    }
}
