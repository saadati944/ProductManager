using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data;
using Tappe.Data;
using Tappe.Data.Models;
using Tappe.Business;
using Tappe.Data.Repositories;

namespace Tappe.Forms
{
    public class FrmCreateInvoice : Form
    {
        protected const string _itemNameColumnName = "ItemName";

        protected readonly int _idColumnIndex = 0;
        protected readonly int _stockColumnIndex = 1;
        protected readonly int _itemColumnIndex = 2;
        protected readonly int _quantityColumnIndex = 3;
        protected readonly int _feeColumnIndex = 4;
        protected readonly int _discountColumnIndex = 5;
        protected readonly int _TotalAfterDiscountColumnIndex = 6;
        protected readonly int _taxcolumnName = 7;
        protected readonly int _totalPriceColumnIndex = 8;
        protected readonly int _deleteBtnColumnIndex = 9;

        protected int _originalInvoiceNumber = -1;
        protected int _lockedInvoiceNumber = -1;

        protected DataTable _invoiceDataTable;
        protected DataTable _invoiceItemsDataTable;

        protected readonly Database _database;
        protected BuyInvoiceBusiness _buyInvoiceBusiness;
        protected SellInvoiceBusiness _sellInvoiceBusiness;
        protected InvoiceBusiness _invoiceBusiness;
        protected readonly ItemsBusiness _itemsBusiness;

        protected readonly Dictionary<string, int> _stockNameRefs;


        public FrmCreateInvoice()
        {
            _database = container.Create<Database>();
            _buyInvoiceBusiness = container.Create<BuyInvoiceBusiness>();
            _sellInvoiceBusiness = container.Create<SellInvoiceBusiness>();
            _itemsBusiness = container.Create<ItemsBusiness>();
            _stockNameRefs = new Dictionary<string, int>();
            foreach (Stock x in _database.Stocks)
                _stockNameRefs.Add(x.Name, x.Id);
        }
        protected void SetInvoiceItemsStockRef(DataGridView itemsGridView)
        {
            for (int i = 0; i < _invoiceItemsDataTable.Rows.Count; i++)
            {
                int stockref = _stockNameRefs[(string)itemsGridView.Rows[i].Cells[_stockColumnIndex].Value];
                _invoiceItemsDataTable.Rows[i][Invoice.StockRefColumnName] = stockref;
            }
        }

        protected virtual DataTable NewInvoiceDataTable() { throw new NotImplementedException(); }
        protected virtual DataTable NewInvoiceItemDataTable() { throw new NotImplementedException(); }

        protected void InitilizeDataTable(int number)
        {
            _originalInvoiceNumber = number;
            if (number == -1)
            {
                _invoiceDataTable = NewInvoiceDataTable();
                _invoiceItemsDataTable = NewInvoiceItemDataTable();

                DataRow row = _invoiceDataTable.NewRow();

                row[Invoice.NumberColumnName] = 0;
                row[Invoice.DateColumnName] = DateTime.Now;
                row[Invoice.TotalPriceColumnName] = 0;
                row[Invoice.StockRefColumnName] = -1;
                row[Invoice.PartyRefColumnName] = -1;
                row[Invoice.UserRefColumnName] = -1;

                _invoiceDataTable.Rows.Add(row);
            }
            else
            {
                _invoiceDataTable = _invoiceBusiness.GetInvoice(number);
                _invoiceItemsDataTable = _invoiceBusiness.GetInvoiceItems((int)_invoiceDataTable.Rows[0][Invoice.IdColumnName]);
            }
            _invoiceDataTable.Rows[0][Invoice.UserRefColumnName] = Program.LoggedInUser.Id;
            _invoiceItemsDataTable.Columns.Add(_itemNameColumnName, typeof(string));
        }

        protected void EditMode()
        {
            for(int i=0; i< _invoiceItemsDataTable.Rows.Count; i++)
                _invoiceItemsDataTable.Rows[i][_itemNameColumnName] = _itemsBusiness.GetItemModel((int)_invoiceItemsDataTable.Rows[i][InvoiceItem.ItemRefColumnName]);
        }

        protected void UpdateTotalPrices()
        {
            for(int i=0; i<_invoiceItemsDataTable.Rows.Count; i++)
                ItemsGridView_CellEndEdit(null, new DataGridViewCellEventArgs(_quantityColumnIndex, i));
        }

        protected virtual void ItemsGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            throw new NotImplementedException();
        }

        protected void DateToString(object sender, ConvertEventArgs cevent)
        {
            if (cevent.DesiredType != typeof(string)) return;
            cevent.Value = PersianDate.PersianDateStringFromDateTime((DateTime)cevent.Value);
        }

        protected void StringToDate(object sender, ConvertEventArgs cevent)
        {
            if (cevent.DesiredType != typeof(DateTime)) return;

            DateTime dt;
            if (!PersianDate.DateTimeFromPersianDateString((string)cevent.Value, out dt))
                return;

            cevent.Value = dt;
        }
    }
}
