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
    public partial class FrmAddItemPrice : Form
    {
        private readonly Business.ItemsBusiness _itemsBusiness;
        private Data.Models.ItemPrice _itemPrice = new Data.Models.ItemPrice();

        public FrmAddItemPrice(Business.ItemsBusiness itemsBusiness)
        {
            InitializeComponent();
            _itemsBusiness = itemsBusiness;
            cmbItems.Items.AddRange(_itemsBusiness.Items.ToArray());
            _itemPrice.Date = DateTime.Now;

            Binding b = new Binding("Text", _itemPrice, "Date");
            b.Format += new ConvertEventHandler(DateToString);
            b.Parse += new ConvertEventHandler(StringToDate);
            txtDate.DataBindings.Add(b);
            numPrice.DataBindings.Add("Value", _itemPrice, "Price");
        }

        private void FrmAddItemPrice_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                btnCancele_Click(null, null);
        }

        private void btnCancele_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void cmbItems_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(cmbItems.SelectedIndex != -1)
                _itemPrice.ItemRef = ((Data.Models.Item)cmbItems.SelectedItem).Id;
        }

        private void DateToString(object sender, ConvertEventArgs cevent)
        {
            if (cevent.DesiredType != typeof(string)) return;
            cevent.Value = Data.Models.PersianDate.PersianDateStringFromDateTime((DateTime)cevent.Value);
        }

        private void StringToDate(object sender, ConvertEventArgs cevent)
        {
            if (cevent.DesiredType != typeof(DateTime)) return;

            DateTime dt;
            if (!Data.Models.PersianDate.DateTimeFromPersianDateString((string)cevent.Value, out dt))
                return;

            cevent.Value = dt;
        }

        private void txtDate_Validating(object sender, CancelEventArgs e)
        {
            DateTime dt;
            if (!Data.Models.PersianDate.DateTimeFromPersianDateString(txtDate.Text, out dt))
            {
                errorProvider.SetError(txtDate, "تاریخ ورودی معتبر نمیباشد");
                e.Cancel = true;
            }
            else
            {
                errorProvider.SetError(txtDate, null);
                e.Cancel = false;
            }
        }

        private void cmbItems_Validating(object sender, CancelEventArgs e)
        {
            if (cmbItems.SelectedIndex == -1)
            {
                errorProvider.SetError(cmbItems, "یکی از محصولات را انتخاب کنید");
                e.Cancel = true;
            }
            else
            {
                errorProvider.SetError(cmbItems, null);
                e.Cancel = false;
            }

        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!ValidateChildren())
                return;

            Data.Models.ItemPrice price = _itemsBusiness.GetItemPrice(_itemPrice.ItemRef, _itemPrice.Date);
            if (price.Date == _itemPrice.Date)
            {
                price.Price = _itemPrice.Price;
                price.Save();
            }
            else
                _itemPrice.Save();

            Close();
        }
    }
}
