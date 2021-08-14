using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tappe.Data;

namespace Tappe.Forms
{
    public partial class FrmUserInfo : Form 
    {
        private Data.Models.User _user;
        public FrmUserInfo()
        {
            InitializeComponent();
            var defaultUser = Program.LoggedInUser;

            //bindingSource.DataSource = new Data.Models.User
            //{
            //    Id = defaultUser.Id,
            //    FirstName = defaultUser.FirstName,
            //    LastName = defaultUser.LastName,
            //    Age = defaultUser.Age,
            //    Gender = defaultUser.Gender,
            //    Password = defaultUser.Password
            //};

            //txtFirstName.DataBindings.Add("Text", _user, "FirstName");
            //txtLastName.DataBindings.Add("Text", _user, "LastName");
            //numAge.DataBindings.Add("Value", _user, "Age");
            //radMale.DataBindings.Add("Checked", _user, "Gender");

            _user = new Data.Models.User
            {
                Id = defaultUser.Id,
                FirstName = txtFirstName.Text = defaultUser.FirstName,
                LastName = txtLastName.Text = defaultUser.LastName,
                Age = (int)(numAge.Value = defaultUser.Age),
                Gender = radMale.Checked = defaultUser.Gender,
                Password = defaultUser.Password
            };
        }

        private void Bind()
        {
            _user.FirstName = txtFirstName.Text;
            _user.LastName = txtLastName.Text;
            _user.Age = (int) numAge.Value;
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

            _user.Save();
            Program.LoggedInUser = _user;
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
            _user.Save();
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
