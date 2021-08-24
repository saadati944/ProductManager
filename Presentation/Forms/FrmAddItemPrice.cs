using DataLayer;
using DataLayer.Models;
using System;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

namespace Presentation.Forms
{
    public partial class FrmAddItemPrice : Form
    {
        private readonly Business.ItemsBusiness _itemsBusiness;
        private ItemPrice _itemPrice = new ItemPrice();

        public FrmAddItemPrice(Business.ItemsBusiness itemsBusiness)
        {
            InitializeComponent();
            SetErrorProviderPadding(this, errorProvider, 10);
            _itemsBusiness = itemsBusiness;
            cmbItems.Items.AddRange(_itemsBusiness.Items.ToArray());
            _itemPrice.Date = DateTime.Now;

            Binding b = new Binding("Text", _itemPrice, "Date");
            b.Format += new ConvertEventHandler(DateToString);
            b.Parse += new ConvertEventHandler(StringToDate);
            txtDate.DataBindings.Add(b);
            numPrice.DataBindings.Add("Value", _itemPrice, "Price");
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
            if (cmbItems.SelectedIndex != -1)
                _itemPrice.ItemRef = ((Item)cmbItems.SelectedItem).Id;
        }

        private void DateToString(object sender, ConvertEventArgs cevent)
        {
            if (cevent.DesiredType != typeof(string)) return;
            cevent.Value = PersianDate.PersianDateStringFromDateTime((DateTime)cevent.Value);
        }

        private void StringToDate(object sender, ConvertEventArgs cevent)
        {
            if (cevent.DesiredType != typeof(DateTime)) return;

            DateTime dt;
            if (!PersianDate.DateTimeFromPersianDateString((string)cevent.Value, out dt))
                return;

            cevent.Value = dt;
        }

        private void txtDate_Validating(object sender, CancelEventArgs e)
        {
            DateTime dt;
            if (!PersianDate.DateTimeFromPersianDateString(txtDate.Text, out dt))
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
            errorProvider.Clear();
            if (!ValidateChildren())
                return;

            ItemPrice price = _itemsBusiness.GetItemPrice(_itemPrice.ItemRef, _itemPrice.Date);
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
