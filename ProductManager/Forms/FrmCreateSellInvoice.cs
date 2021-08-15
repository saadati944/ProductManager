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
using Tappe.Data.Repositories;

namespace Tappe.Forms
{
    public partial class FrmCreateSellInvoice : Form
    {
        private readonly int _idColumnIndex = 0;
        private readonly int _itemColumnIndex = 1;
        private readonly int _quantityColumnIndex = 2;
        private readonly int _feeColumnIndex = 3;
        private readonly int _discountColumnIndex = 4;
        private readonly int _TotalAfterDiscountColumnIndex = 5;
        private readonly int _taxcolumnName = 6;
        private readonly int _totalPriceColumnIndex = 7;
        private readonly int _deleteBtnColumnIndex = 8;


        private readonly Database _database;
        private readonly Business.SellInvoiceBusiness _sellInvoiceBusiness;
        private readonly Business.ItemsBusiness _itemsBusiness;
        private readonly Data.Models.SellInvoice _sellInvoice = new Data.Models.SellInvoice();

        public FrmCreateSellInvoice()
        {
            InitializeComponent();
            _database = container.Create<Database>();
            _itemsBusiness = container.Create<Business.ItemsBusiness>();
            _sellInvoiceBusiness = container.Create<Business.SellInvoiceBusiness>();


            txtSeller.Text = Program.LoggedInUser.ToString();
            _sellInvoice.UserRef = Program.LoggedInUser.Id;
            _sellInvoice.Date = DateTime.Now;
            _sellInvoice.Number = _sellInvoiceBusiness.GetLastInvoiceNumber() + 1;

            Binding b = new Binding ("Text", _sellInvoice, "Date");
            b.Format += new ConvertEventHandler(DateToString);
            b.Parse += new ConvertEventHandler(StringToDate);
            txtDate.DataBindings.Add(b);
            numInvoiceNumber.DataBindings.Add("Value", _sellInvoice, "Number");
            lblTotalPrice.DataBindings.Add("Text", _sellInvoice, "TotalPrice");

            cmbParties.Items.AddRange(_database.Parties.ToArray());
            cmbParties.SelectedIndex = 0;
            cmbStocks.Items.AddRange(_database.Stocks.ToArray());
            cmbStocks_SelectedIndexChanged(null, null);

            for (int i=0; i<6; i++)
                itemsGridView.Columns.Insert(2, new CustomControls.NumericUpDownColumn());

            ((DataGridViewComboBoxColumn)itemsGridView.Columns[_itemColumnIndex]).Items.AddRange(_itemsBusiness.Items.Select(x => x.Name).ToArray());

            itemsGridView.Columns[_TotalAfterDiscountColumnIndex].ReadOnly = true;
            itemsGridView.Columns[_totalPriceColumnIndex].ReadOnly = true;

            itemsGridView.Columns[_quantityColumnIndex].Name = "مقدار";
            itemsGridView.Columns[_feeColumnIndex].Name = "فی";
            itemsGridView.Columns[_discountColumnIndex].Name = "تخفیف";
            itemsGridView.Columns[_TotalAfterDiscountColumnIndex].Name = "مبلغ پس از کسر تخفیف";
            itemsGridView.Columns[_taxcolumnName].Name = "عوارض و مالیات";
            itemsGridView.Columns[_totalPriceColumnIndex].Name = "مبلغ کل با احتساب تخفیف و عوارض و مالیات";

            itemsGridView.CellBeginEdit += ItemsGridView_CellBeginEdit;
            itemsGridView.CellEndEdit += ItemsGridView_CellEndEdit;
            itemsGridView.CellClick += ItemsGridView_CellClick;
        }

        private void ItemsGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == _deleteBtnColumnIndex)
            {
                if(e.RowIndex != itemsGridView.Rows.Count-1)
                    itemsGridView.Rows.RemoveAt(e.RowIndex);
                return;
            }
        }

        private void ItemsGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == _quantityColumnIndex)
            {
                if(itemsGridView.Rows[e.RowIndex].Cells[_quantityColumnIndex].Value is decimal)
                    itemsGridView.Rows[e.RowIndex].Cells[_quantityColumnIndex].Value = (int)(decimal)itemsGridView.Rows[e.RowIndex].Cells[_quantityColumnIndex].Value;

                ValidateRow(e.RowIndex);
            }
            if (e.ColumnIndex == _itemColumnIndex)
            {
                if (itemsGridView.Rows[e.RowIndex].Cells[_itemColumnIndex].Value == null)
                    return;
                ((DataGridViewButtonCell)itemsGridView.Rows[e.RowIndex].Cells[_deleteBtnColumnIndex]).Value = "حذف";
                var item = _itemsBusiness.GetItemModel((string)itemsGridView.Rows[e.RowIndex].Cells[_itemColumnIndex].Value);
                itemsGridView.Rows[e.RowIndex].Cells[_idColumnIndex].Value = item.Id;
                itemsGridView.Rows[e.RowIndex].Cells[_feeColumnIndex].Value = _itemsBusiness.GetItemPrice(item.Id, _sellInvoice.Date).Price;
                itemsGridView.Rows[e.RowIndex].Cells[_quantityColumnIndex].Value = (int) 0;
                itemsGridView.Rows[e.RowIndex].Cells[_discountColumnIndex].Value = (decimal)0;
                itemsGridView.Rows[e.RowIndex].Cells[_taxcolumnName].Value = (decimal)0;
                itemsGridView.Rows[e.RowIndex].Cells[_totalPriceColumnIndex].Value = (decimal)0;
                itemsGridView.Rows[e.RowIndex].Cells[_TotalAfterDiscountColumnIndex].Value = (decimal)0;
            }
            else
            {
                itemsGridView.Rows[e.RowIndex].Cells[_TotalAfterDiscountColumnIndex].Value =
                    (int)itemsGridView.Rows[e.RowIndex].Cells[_quantityColumnIndex].Value
                    * (decimal)itemsGridView.Rows[e.RowIndex].Cells[_feeColumnIndex].Value
                    - (decimal)itemsGridView.Rows[e.RowIndex].Cells[_discountColumnIndex].Value;

                itemsGridView.Rows[e.RowIndex].Cells[_totalPriceColumnIndex].Value =
                    (int)itemsGridView.Rows[e.RowIndex].Cells[_quantityColumnIndex].Value
                    * (decimal)itemsGridView.Rows[e.RowIndex].Cells[_feeColumnIndex].Value
                    - (decimal)itemsGridView.Rows[e.RowIndex].Cells[_discountColumnIndex].Value
                    + (decimal)itemsGridView.Rows[e.RowIndex].Cells[_taxcolumnName].Value;
                CalculateTotalPrice();
            }
            
        }

        private bool ValidateRow(int rowind)
        {
            if (Business.ItemsBusiness.GetItemQuantity((int)itemsGridView.Rows[rowind].Cells[_idColumnIndex].Value, _sellInvoice.StockRef) < (int)itemsGridView.Rows[rowind].Cells[_quantityColumnIndex].Value)
            {
                itemsGridView.Rows[rowind].Cells[_quantityColumnIndex].ErrorText = "موجودی ناکافی";
                return false;
            }

            itemsGridView.Rows[rowind].Cells[_quantityColumnIndex].ErrorText = null;
            return true;
        }

        private void ItemsGridView_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (e.RowIndex == itemsGridView.Rows.Count - 1 && e.ColumnIndex != _itemColumnIndex)
                e.Cancel = true;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!ValidateChildren(ValidationConstraints.Enabled))
                return;

            _sellInvoice.PartyRef = GetPartyRef();

            CalculateTotalPrice();

            List<Data.Models.InvoiceItem> invoiceItems = new List<Data.Models.InvoiceItem>();
            for(int i=0; i<itemsGridView.Rows.Count - 1; i++)
            {
                invoiceItems.Add(new Data.Models.SellInvoiceItem
                {
                    ItemRef = (int) itemsGridView.Rows[i].Cells[_idColumnIndex].Value,
                    Fee = (decimal)itemsGridView.Rows[i].Cells[_feeColumnIndex].Value,
                    Quantity = (int)itemsGridView.Rows[i].Cells[_quantityColumnIndex].Value,
                    Discount = (decimal)itemsGridView.Rows[i].Cells[_discountColumnIndex].Value,
                    Tax = (decimal)itemsGridView.Rows[i].Cells[_taxcolumnName].Value
                });
            }

            _sellInvoice.InvoiceItems = invoiceItems;
            bool result = _sellInvoiceBusiness.SaveInvoice(_sellInvoice);
            if(!result)
                MessageBox.Show("هنگام ذخیره کردن فاکتور خطایی رخ داده است !!!");
            else
                Close();
        }

        private int GetPartyRef()
        {
            if(cmbParties.SelectedIndex != -1 && ((Data.Models.Party)cmbParties.SelectedItem).Name == cmbParties.Text)
                return ((Data.Models.Party)cmbParties.SelectedItem).Id;
            
            var party = new Data.Models.Party { Name = cmbParties.Text };
            party.Save();
            return party.Id;
        }


        private void btnCancele_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void itemsGridView_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            e.Cancel = true;
        }

        private void CalculateTotalPrice()
        {
            decimal totalprice = 0;
            for (int i = 0; i < itemsGridView.Rows.Count - 1; i++)
            {
                totalprice += (decimal) itemsGridView.Rows[i].Cells[_totalPriceColumnIndex].Value;
            }

            _sellInvoice.TotalPrice = totalprice;
            lblTotalPrice.Text = totalprice.ToString();
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

        private void numInvoiceNumber_Validating(object sender, CancelEventArgs e)
        {
            if (!_sellInvoiceBusiness.IsInvoiceNumberValid((int)numInvoiceNumber.Value))
            {
                errorProvider.SetError(numInvoiceNumber, "شماره فاکتور قبلا ثبت شده است");
                e.Cancel = true;
            }
            else
            {
                errorProvider.SetError(numInvoiceNumber, null);
                e.Cancel = false;
            }
        }

        private void itemsGridView_Validating(object sender, CancelEventArgs e)
        {
            if (itemsGridView.Rows.Count == 1)
            {
                e.Cancel = true;
                errorProvider.SetError(itemsGridView, "فاکتور خالی میباشد");
            }
            else
            {
                e.Cancel = false;
                errorProvider.SetError(itemsGridView, null);
                for (int i = 0; i < itemsGridView.Rows.Count - 1; i++)
                    if (!ValidateRow(i))
                        e.Cancel = true;
            }
        }

        private void cmbStocks_Validating(object sender, CancelEventArgs e)
        {
            if (cmbStocks.SelectedIndex == -1)
            {
                e.Cancel = true;
                errorProvider.SetError(cmbStocks, "یک انبار را انتخاب کنید");
            }
            else
            {
                e.Cancel = false;
                errorProvider.SetError(cmbStocks, null);
            }
        }

        private void cmbStocks_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbStocks.SelectedIndex == -1)
                cmbStocks.SelectedIndex = 0;
            _sellInvoice.StockRef = ((Data.Models.Stock)cmbStocks.SelectedItem).Id;
            for (int i = 0; i < itemsGridView.Rows.Count - 1; i++)
                ValidateRow(i);
        }

        private void FrmCreateSellInvoice_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                btnCancele_Click(null, null);
        }
    }
}
