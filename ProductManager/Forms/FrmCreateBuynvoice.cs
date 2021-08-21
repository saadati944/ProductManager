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
using Tappe.Data.Models;
using Tappe.Data.Repositories;

namespace Tappe.Forms
{
    public partial class FrmCreateBuyInvoice : FrmCreateInvoice
    {
        private int number;
        public FrmCreateBuyInvoice(int invoiceNumber, StructureMap.IContainer container, int number = -1) : base(container)
        {
            this.number = number;
            InitializeComponent();

            _invoiceBusiness = _buyInvoiceBusiness;

            itemsGridView.AutoGenerateColumns = false;

            InitilizeDataTable(number);

            cmbParties.Items.AddRange(_database.Parties.ToArray());

            
            txtBuyer.Text = Program.LoggedInUser.ToString();


            SuspendLayout();
            for (int i=0; i<6; i++)
                itemsGridView.Columns.Insert(3, new CustomControls.NumericUpDownColumn());

            ((DataGridViewComboBoxColumn)itemsGridView.Columns[_stockColumnIndex]).Items.AddRange(_database.Stocks.Select(x => x.Name).ToArray());

            errorProviderHeader.DataSource = _invoiceDataTable;
            errorProviderItems.DataSource = _invoiceItemsDataTable;

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

            ResumeLayout();

            cmbParties.Text = cmbParties.Items.Count == 0 ? "" : cmbParties.Items[0].ToString();

            itemsGridView.Columns[_itemIdColumnIndex].Visible = false;

            itemsGridView.Columns.Add(_stockIdColumnName, "StockId");
            itemsGridView.Columns[_stockIdColumnName].Visible = false;

            BindDataTable();

            _invoiceDataTable.Rows[0][Invoice.NumberColumnName] = invoiceNumber;
            _lockedInvoiceNumber = invoiceNumber;

            if (_originalInvoiceNumber != -1)
                lblTitle.Text = "ویرایش فاکتور خرید";
            SetErrorProviderPadding(pnlControls, errorProviderHeader, 10);
        }

        protected override DataTable NewInvoiceDataTable()
        {
            return _buyInvoiceBusiness.BuyInvoicesRepository.NewInvoiceDataTable();
        }
        protected override DataTable NewInvoiceItemDataTable()
        {
            return _buyInvoiceBusiness.BuyInvoicesRepository.NewInvoiceItemDataTable();
        }

        private void BindDataTable()
        {
            foreach (Control x in pnlControls.Controls)
                x.DataBindings.Clear();
            lblTotalPrice.DataBindings.Clear();

            //header bindings
            numInvoiceNumber.DataBindings.Add("Value", _invoiceDataTable, Invoice.NumberColumnName, false, DataSourceUpdateMode.OnPropertyChanged);
            lblTotalPrice.DataBindings.Add("Text", _invoiceDataTable, Invoice.TotalPriceColumnName, false, DataSourceUpdateMode.OnPropertyChanged);

            // cmbParties.DataBindings.Add("SelectedIndex", _invoiceDataTable, Invoice.PartyRefColumnName, false, DataSourceUpdateMode.Never);

            Binding b = new Binding("Text", _invoiceDataTable, Invoice.DateColumnName, false, DataSourceUpdateMode.OnPropertyChanged);
            b.Format += new ConvertEventHandler(DateToString);
            b.Parse += new ConvertEventHandler(StringToDate);
            txtDate.DataBindings.Add(b);


            //items bindings
            if (itemsGridView.DataSource != null)
                return;
            itemsGridView.DataSource = _invoiceItemsDataTable;
            itemsGridView.Columns[_itemIdColumnIndex].DataPropertyName = InvoiceItem.ItemRefColumnName;
            itemsGridView.Columns[_stockIdColumnName].DataPropertyName = InvoiceItem.StockRefColumnName;
            itemsGridView.Columns[_quantityColumnIndex].DataPropertyName = InvoiceItem.QuantityColumnName;
            itemsGridView.Columns[_feeColumnIndex].DataPropertyName = InvoiceItem.FeeColumnName;
            itemsGridView.Columns[_discountColumnIndex].DataPropertyName = InvoiceItem.DiscountColumnName;
            itemsGridView.Columns[_taxcolumnName].DataPropertyName = InvoiceItem.TaxColumnName;
            itemsGridView.Columns[_itemColumnIndex].DataPropertyName = _itemNameColumnName;
            itemsGridView.Columns[_stockColumnIndex].DataPropertyName = _stockNameColumnName;
        }
 
        private void ItemsGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == _deleteBtnColumnIndex)
            {
                if (e.RowIndex != itemsGridView.Rows.Count - 1 && e.RowIndex > -1)
                {
                    _invoiceItemsDataTable.Rows.RemoveAt(e.RowIndex);
                    CalculateTotalPrice();
                }
                return;
            }
        }

        protected override void ItemsGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == _quantityColumnIndex)
            {
                if(itemsGridView.Rows[e.RowIndex].Cells[_quantityColumnIndex].Value is decimal)
                    itemsGridView.Rows[e.RowIndex].Cells[_quantityColumnIndex].Value = (int)(decimal)itemsGridView.Rows[e.RowIndex].Cells[_quantityColumnIndex].Value;

            }
            if (e.ColumnIndex == _stockColumnIndex)
            {
                ((DataGridViewComboBoxCell)itemsGridView.Rows[e.RowIndex].Cells[_itemColumnIndex]).Items.Clear();
                itemsGridView.Rows[e.RowIndex].Cells[_itemIdColumnIndex].Value =
                itemsGridView.Rows[e.RowIndex].Cells[_feeColumnIndex].Value =
                itemsGridView.Rows[e.RowIndex].Cells[_quantityColumnIndex].Value =
                itemsGridView.Rows[e.RowIndex].Cells[_discountColumnIndex].Value =
                itemsGridView.Rows[e.RowIndex].Cells[_taxcolumnName].Value =
                itemsGridView.Rows[e.RowIndex].Cells[_totalPriceColumnIndex].Value =
                itemsGridView.Rows[e.RowIndex].Cells[_TotalAfterDiscountColumnIndex].Value = (decimal)0;

                if (!(itemsGridView.Rows[e.RowIndex].Cells[_stockColumnIndex].Value is DBNull))
                {
                    int stockref = _stockNameRefs[(string)itemsGridView.Rows[e.RowIndex].Cells[_stockColumnIndex].Value];
                    ((DataGridViewComboBoxCell)itemsGridView.Rows[e.RowIndex].Cells[_itemColumnIndex]).Items.AddRange(StockItems(stockref));
                    itemsGridView.Rows[e.RowIndex].Cells[_stockIdColumnName].Value = stockref;
                }
                else
                    itemsGridView.Rows[e.RowIndex].Cells[_stockIdColumnName].Value = -1;
            }
            else if (e.ColumnIndex == _itemColumnIndex)
            {
                if (itemsGridView.Rows[e.RowIndex].Cells[_itemColumnIndex].Value is DBNull)
                {
                    itemsGridView.Rows[e.RowIndex].Cells[_itemIdColumnIndex].Value = -1;
                    return;
                }
                ((DataGridViewButtonCell)itemsGridView.Rows[e.RowIndex].Cells[_deleteBtnColumnIndex]).Value = "حذف";
                var item = _itemsBusiness.GetItemModel((string)itemsGridView.Rows[e.RowIndex].Cells[_itemColumnIndex].Value);
                itemsGridView.Rows[e.RowIndex].Cells[_itemIdColumnIndex].Value = item.Id;
                itemsGridView.Rows[e.RowIndex].Cells[_feeColumnIndex].Value = _itemsBusiness.GetItemPrice(item.Id, (DateTime)_invoiceDataTable.Rows[0][Invoice.DateColumnName]).Price;
                itemsGridView.Rows[e.RowIndex].Cells[_quantityColumnIndex].Value = (int) 0;
                itemsGridView.Rows[e.RowIndex].Cells[_discountColumnIndex].Value =
                itemsGridView.Rows[e.RowIndex].Cells[_taxcolumnName].Value =
                itemsGridView.Rows[e.RowIndex].Cells[_totalPriceColumnIndex].Value =
                itemsGridView.Rows[e.RowIndex].Cells[_TotalAfterDiscountColumnIndex].Value = (decimal)0;
            }
            else
            {
                itemsGridView.Rows[e.RowIndex].Cells[_TotalAfterDiscountColumnIndex].Value =
                    (int)itemsGridView.Rows[e.RowIndex].Cells[_quantityColumnIndex].Value
                    * (decimal)itemsGridView.Rows[e.RowIndex].Cells[_feeColumnIndex].Value
                    - (decimal)itemsGridView.Rows[e.RowIndex].Cells[_discountColumnIndex].Value;

                itemsGridView.Rows[e.RowIndex].Cells[_totalPriceColumnIndex].Value =
                    (decimal)itemsGridView.Rows[e.RowIndex].Cells[_TotalAfterDiscountColumnIndex].Value
                    + (decimal)itemsGridView.Rows[e.RowIndex].Cells[_taxcolumnName].Value;
                CalculateTotalPrice();
            }
            if (((DataGridViewButtonCell)itemsGridView.Rows[e.RowIndex].Cells[_deleteBtnColumnIndex]).Value == null)
                ((DataGridViewButtonCell)itemsGridView.Rows[e.RowIndex].Cells[_deleteBtnColumnIndex]).Value = "حذف";
        }

        protected override string[] StockItems(int stockref)
        {
            return _itemsBusiness.Items.Select(x => x.Name).ToArray();
        }

        private void ItemsGridView_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (e.RowIndex == itemsGridView.Rows.Count - 1 && e.ColumnIndex != _stockColumnIndex
                || (e.ColumnIndex != _itemColumnIndex && e.ColumnIndex != _stockColumnIndex && itemsGridView.Rows[e.RowIndex].Cells[e.ColumnIndex].Value is DBNull))
                e.Cancel = true;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            errorProviderHeader.Clear();
            errorProviderItems.Clear();

            _invoiceDataTable.Rows[0][Invoice.PartyRefColumnName] = GetPartyRef();
            if (!Business.InvoiceBusiness.ValidateInvoiceDataTable(_invoiceDataTable, _buyInvoiceBusiness, _originalInvoiceNumber == -1 ? _lockedInvoiceNumber : _originalInvoiceNumber)
                | !Business.InvoiceBusiness.ValidateInvoiceItemsDataTable(_invoiceItemsDataTable) | !ValidateChildren(ValidationConstraints.Enabled))
            {
                BindDataTable();
                return;
            }

            CalculateTotalPrice();


            bool result = _originalInvoiceNumber == -1 ? _buyInvoiceBusiness.SaveInvoice(_invoiceDataTable, _invoiceItemsDataTable) : _buyInvoiceBusiness.EditInvoice(_originalInvoiceNumber, _invoiceDataTable, _invoiceItemsDataTable);
            if (!result)
                MessageBox.Show("هنگام ذخیره کردن فاکتور خطایی رخ داده است !!!");
            else
                Close();
        }

        private int GetPartyRef()
        {
            if (cmbParties.SelectedIndex != -1 && ((Party)cmbParties.SelectedItem).Name == cmbParties.Text)
                return ((Party)cmbParties.SelectedItem).Id;
            
            var party = new Party { Name = cmbParties.Text };
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
                if (itemsGridView.Rows[i].Cells[_totalPriceColumnIndex].Value == null)
                    continue;
                totalprice += (decimal) itemsGridView.Rows[i].Cells[_totalPriceColumnIndex].Value;
                ((DataGridViewButtonCell)itemsGridView.Rows[i].Cells[_deleteBtnColumnIndex]).Value = "حذف";
            }

            lblTotalPrice.Text = totalprice.ToString();
        }

        private void FrmCreateBuyInvoice_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                btnCancele_Click(null, null);
        }

        private void txtDate_Leave(object sender, EventArgs e)
        {
            DateTime dt;
            if (!PersianDate.DateTimeFromPersianDateString(txtDate.Text, out dt))
                txtDate.Text = PersianDate.PersianDateStringFromDateTime((DateTime)_invoiceDataTable.Rows[0][Invoice.DateColumnName]);
        }

        private void cmbParties_Validating(object sender, CancelEventArgs e)
        {
            if (String.IsNullOrEmpty(cmbParties.Text))
            {
                e.Cancel = true;
                errorProviderHeader.SetError(cmbParties, "نام تامین کننده را وارد یا انتخاب نمایید");
            }
            else
            {
                e.Cancel = false;
                errorProviderHeader.SetError(cmbParties, null);
            }
        }

        private void FrmCreateBuyInvoice_Load(object sender, EventArgs e)
        {
            if (number != -1)
            {
                EditMode(itemsGridView, cmbParties);
                UpdateTotalPrices();
            }
        }

        private void itemsGridView_Validating(object sender, CancelEventArgs e)
        {
            if(itemsGridView.Rows.Count == 1)
            {
                e.Cancel = true;
                errorProviderHeader.SetError(lblTotalPriceLable, "موردی را برای خرید انتخاب کنید");
            }
            else
            {
                e.Cancel = false;
                errorProviderHeader.SetError(lblTotalPriceLable, null);
            }
        }
    }
}
