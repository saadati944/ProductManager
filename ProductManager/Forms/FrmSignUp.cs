﻿using System;
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
    public partial class FrmSignUp : Form
    {
        private readonly Data.Models.User _user = new Data.Models.User();
        public FrmSignUp()
        {
            InitializeComponent();
            bindingSource.DataSource = _user;
            radMale.Checked = true;
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

            Data.Models.User u = (Data.Models.User) bindingSource.DataSource;
            u.Save();
            MessageBox.Show("ثبت نام با موفقیت به پایان رسید");
            Close();
        }

        private void radMale_CheckedChanged(object sender, EventArgs e)
        {
            ((Data.Models.User)bindingSource.DataSource).Gender = radMale.Checked;
        }

        private void txtFirstName_Validating(object sender, CancelEventArgs e)
        {
            ValidateTextLength(sender, e, 3);
        }
        private void ValidateTextLength(object sender, CancelEventArgs e, int minLen)
        {
            if(((Control)sender).Text.Length < minLen)
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
