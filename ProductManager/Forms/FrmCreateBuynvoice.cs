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
using Tappe.Data.Models;
using Tappe.Data.Repositories;

namespace Tappe.Forms
{
    public partial class FrmCreateBuyInvoice : FrmCreateInvoice
    {

        public FrmCreateBuyInvoice(int invoiceNumber, int number = -1)
        {
            InitializeComponent();

            _invoiceBusiness = _buyInvoiceBusiness;

            itemsGridView.AutoGenerateColumns = false;

            InitilizeDataTable(number);
            

            cmbParties.Items.AddRange(_database.Parties.ToArray());
            cmbParties.SelectedIndex = 0;
            cmbStocks.Items.AddRange(_database.Stocks.ToArray());
            cmbStocks_SelectedIndexChanged(null, null);

            txtBuyer.Text = Program.LoggedInUser.ToString();

            SuspendLayout();
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

            ResumeLayout();

            cmbParties.Text = cmbParties.Items.Count != 0 ? "" : cmbParties.Items[0].ToString();
            BindDataTable();

            if (number != -1)
                EditMode();

            itemsGridView.Columns[_idColumnIndex].Visible = false;
            _invoiceDataTable.Rows[0][Invoice.NumberColumnName] = invoiceNumber;
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
            //header bindings
            numInvoiceNumber.DataBindings.Add("Value", _invoiceDataTable, Invoice.NumberColumnName, false, DataSourceUpdateMode.OnPropertyChanged);
            lblTotalPrice.DataBindings.Add("Text", _invoiceDataTable, Invoice.TotalPriceColumnName, false, DataSourceUpdateMode.OnPropertyChanged);

            cmbParties.DataBindings.Add("SelectedIndex", _invoiceDataTable, Invoice.PartyRefColumnName, false, DataSourceUpdateMode.Never);

            Binding b = new Binding("Text", _invoiceDataTable, Invoice.DateColumnName, false, DataSourceUpdateMode.OnPropertyChanged);
            b.Format += new ConvertEventHandler(DateToString);
            b.Parse += new ConvertEventHandler(StringToDate);
            txtDate.DataBindings.Add(b);

            b = new Binding("SelectedIndex", _invoiceDataTable, Invoice.StockRefColumnName, false, DataSourceUpdateMode.OnPropertyChanged);
            b.Format += new ConvertEventHandler(StockIdToSelectedIndex);
            b.Parse += new ConvertEventHandler(SelectedIndexToStockId);
            cmbStocks.DataBindings.Add(b);

            //items bindings
            itemsGridView.DataSource = _invoiceItemsDataTable;
            itemsGridView.Columns[_idColumnIndex].DataPropertyName = InvoiceItem.ItemRefColumnName;
            itemsGridView.Columns[_quantityColumnIndex].DataPropertyName = InvoiceItem.QuantityColumnName;
            itemsGridView.Columns[_feeColumnIndex].DataPropertyName = InvoiceItem.FeeColumnName;
            itemsGridView.Columns[_discountColumnIndex].DataPropertyName = InvoiceItem.DiscountColumnName;
            itemsGridView.Columns[_taxcolumnName].DataPropertyName = InvoiceItem.TaxColumnName;
            itemsGridView.Columns[_itemColumnIndex].DataPropertyName = _itemNameColumnName;
        }
 
        private void ItemsGridView_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex == _deleteBtnColumnIndex)
            {
                if (e.RowIndex != itemsGridView.Rows.Count - 1)
                {
                    itemsGridView.Rows.RemoveAt(e.RowIndex);
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
            if (e.ColumnIndex == _itemColumnIndex)
            {
                if (itemsGridView.Rows[e.RowIndex].Cells[_itemColumnIndex].Value == null)
                    return;
                ((DataGridViewButtonCell)itemsGridView.Rows[e.RowIndex].Cells[_deleteBtnColumnIndex]).Value = "حذف";
                var item = _itemsBusiness.GetItemModel((string)itemsGridView.Rows[e.RowIndex].Cells[_itemColumnIndex].Value);
                itemsGridView.Rows[e.RowIndex].Cells[_idColumnIndex].Value = item.Id;
                itemsGridView.Rows[e.RowIndex].Cells[_feeColumnIndex].Value = _itemsBusiness.GetItemPrice(item.Id, (DateTime)_invoiceDataTable.Rows[0][Invoice.DateColumnName]).Price;
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
                    (decimal)itemsGridView.Rows[e.RowIndex].Cells[_TotalAfterDiscountColumnIndex].Value
                    + (decimal)itemsGridView.Rows[e.RowIndex].Cells[_taxcolumnName].Value;
                CalculateTotalPrice();
            }
        }


        private void ItemsGridView_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            if (e.RowIndex == itemsGridView.Rows.Count - 1 && e.ColumnIndex != _itemColumnIndex)
                e.Cancel = true;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            _invoiceDataTable.Rows[0][Invoice.PartyRefColumnName] = GetPartyRef();
            if( !Business.InvoiceBusiness.ValidateInvoiceDataTable(_invoiceDataTable, _buyInvoiceBusiness, _originalInvoiceNumber) 
                | !Business.InvoiceBusiness.ValidateInvoiceItemsDataTable(_invoiceItemsDataTable) || !ValidateChildren(ValidationConstraints.Enabled))
            {
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
            if(cmbParties.SelectedIndex != -1 && ((Party)cmbParties.SelectedItem).Name == cmbParties.Text)
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
            }

            lblTotalPrice.Text = totalprice.ToString();
        }

        private void SelectedIndexToStockId(object sender, ConvertEventArgs cevent)
        {
            if (cevent.DesiredType != typeof(int)) return;
            cevent.Value = ((int)cevent.Value) == -1 ? -1 : ((Stock)cmbStocks.SelectedItem).Id;
        }

        private void StockIdToSelectedIndex(object sender, ConvertEventArgs cevent)
        {
            if (cevent.DesiredType != typeof(int)) return;
            for(int i=0; i<cmbStocks.Items.Count; i++)
            {
            Stock x = (Stock) cmbStocks.Items[i];
                if(x.Id == (int)cevent.Value)
                {
                    cevent.Value = i;
                    return;
                }
            }
            cevent.Value = -1;
        }

        private void cmbStocks_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cmbStocks.SelectedIndex == -1)
                cmbStocks.SelectedIndex = 0;

            _invoiceDataTable.Rows[0][Invoice.StockRefColumnName] = ((Stock)cmbStocks.SelectedItem).Id;
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
                errorProvider.SetError(cmbParties, "نام تامین کننده را وارد یا انتخاب نمایید");
            }
            else
            {
                e.Cancel = false;
                errorProvider.SetError(cmbParties, null);
            }
        }

        private void FrmCreateBuyInvoice_Load(object sender, EventArgs e)
        {
            UpdateTotalPrices();
        }
    }
}
