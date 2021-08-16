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
        protected readonly int _idColumnIndex = 0;
        protected readonly int _itemColumnIndex = 1;
        protected readonly int _quantityColumnIndex = 2;
        protected readonly int _feeColumnIndex = 3;
        protected readonly int _discountColumnIndex = 4;
        protected readonly int _TotalAfterDiscountColumnIndex = 5;
        protected readonly int _taxcolumnName = 6;
        protected readonly int _totalPriceColumnIndex = 7;
        protected readonly int _deleteBtnColumnIndex = 8;

        protected DataTable _invoiceDataTable;
        protected DataTable _invoiceItemsDataTable;

        protected readonly Database _database;
        protected BuyInvoiceBusiness _buyInvoiceBusiness;
        protected SellInvoiceBusiness _sellInvoiceBusiness;
        protected InvoiceBusiness _invoiceBusiness;
        protected readonly ItemsBusiness _itemsBusiness;


        public FrmCreateInvoice()
        {
            _database = container.Create<Database>();
            _buyInvoiceBusiness = container.Create<BuyInvoiceBusiness>();
            _sellInvoiceBusiness = container.Create<SellInvoiceBusiness>();
            _itemsBusiness = container.Create<ItemsBusiness>();
        }

        protected virtual DataTable NewInvoiceDataTable() { throw new NotImplementedException(); }
        protected virtual DataTable NewInvoiceItemDataTable() { throw new NotImplementedException(); }

        protected void InitilizeDataTable(int number)
        {
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
        }

    }
}
