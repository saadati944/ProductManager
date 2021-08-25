using Business;
using DataLayer;
using DataLayer.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Windows.Forms;

namespace Presentation.Forms
{
    public class FrmCreateInvoice : Form
    {
        protected const string _itemNameColumnName = "ItemName";
        protected const string _stockNameColumnName = "StockName";

        protected readonly int _itemIdColumnIndex = 0;
        protected readonly int _stockColumnIndex = 1;
        protected readonly int _itemColumnIndex = 2;
        protected readonly int _quantityColumnIndex = 3;
        protected readonly int _feeColumnIndex = 4;
        protected readonly int _discountColumnIndex = 5;
        protected readonly int _TotalAfterDiscountColumnIndex = 6;
        protected readonly int _taxcolumnName = 7;
        protected readonly int _totalPriceColumnIndex = 8;
        protected readonly int _deleteBtnColumnIndex = 9;
        protected readonly string _stockIdColumnName = "StockRef";

        protected int? _originalInvoiceNumber = null;
        protected byte[] _version = null;

        protected DataTable _invoiceDataTable;
        protected DataTable _invoiceItemsDataTable;

        protected readonly Database _database;
        protected BuyInvoiceBusiness _buyInvoiceBusiness;
        protected SellInvoiceBusiness _sellInvoiceBusiness;
        protected InvoiceBusiness _invoiceBusiness;
        protected readonly Business.ItemsBusiness _itemsBusiness;
        protected readonly DataLayer.Repositories.ItemsRepository _itemsRepository;

        protected readonly Dictionary<string, int> _stockNameRefs;


        public FrmCreateInvoice() { }
        public FrmCreateInvoice(StructureMap.IContainer container)
        {
            _database = container.GetInstance<Database>();
            _buyInvoiceBusiness = container.GetInstance<BuyInvoiceBusiness>();
            _sellInvoiceBusiness = container.GetInstance<SellInvoiceBusiness>();
            _itemsBusiness = container.GetInstance<ItemsBusiness>();
            _itemsRepository = container.GetInstance<DataLayer.Repositories.ItemsRepository>();
            _stockNameRefs = new Dictionary<string, int>();
            foreach (Stock x in _database.Stocks)
                _stockNameRefs.Add(x.Name, x.Id);
        }

        protected void SetErrorProviderPadding(Control container, ErrorProvider errorProvider, int value, bool children = false)
        {
            foreach (Control x in container.Controls)
            {
                errorProvider.SetIconPadding(x, value);
                if (children)
                    SetErrorProviderPadding(x, errorProvider, value, true);
            }
        }

        protected virtual DataTable NewInvoiceDataTable() { throw new NotImplementedException(); }
        protected virtual bool CheckVersion() { throw new NotImplementedException(); }
        protected virtual DataTable NewInvoiceItemDataTable() { throw new NotImplementedException(); }

        protected void InitilizeDataTable(int? number)
        {
            _originalInvoiceNumber = number;
            if (number == null)
            {
                _invoiceDataTable = NewInvoiceDataTable();
                _invoiceItemsDataTable = NewInvoiceItemDataTable();

                DataRow row = _invoiceDataTable.NewRow();

                row[Invoice.NumberColumnName] = -1;
                row[Invoice.DateColumnName] = DateTime.Now;
                row[Invoice.TotalPriceColumnName] = 0;
                row[Invoice.PartyRefColumnName] = -1;
                row[Invoice.UserRefColumnName] = -1;

                _invoiceDataTable.Rows.Add(row);
            }
            else
            {
                _invoiceDataTable = _invoiceBusiness.GetInvoice(number.Value);
                _invoiceItemsDataTable = _invoiceBusiness.GetInvoiceItems((int)_invoiceDataTable.Rows[0][Invoice.IdColumnName]);
            }
            _invoiceDataTable.Rows[0][Invoice.UserRefColumnName] = Database.LoggedInUser.Id;

            _invoiceItemsDataTable.Columns.Add(_itemNameColumnName, typeof(string));
            _invoiceItemsDataTable.Columns.Add(_stockNameColumnName, typeof(string));
        }

        protected void EditMode(DataGridView dgv, ComboBox parties)
        {
            Party p = new Party { Id = (int)_invoiceDataTable.Rows[0][Invoice.PartyRefColumnName] };
            p.Load();
            parties.Text = p.Name;

            for (int i = 0; i < _invoiceItemsDataTable.Rows.Count; i++)
            {
                string itemname = _itemsRepository.GetItemModel((int)_invoiceItemsDataTable.Rows[i][InvoiceItem.ItemRefColumnName]).Name;
                string stockname = "";
                int stockref = (int)_invoiceItemsDataTable.Rows[i][InvoiceItem.StockRefColumnName];
                foreach (var x in _stockNameRefs)
                    if (x.Value == stockref)
                    {
                        stockname = x.Key;
                        break;
                    }


                //if ((DataGridViewComboBoxCell)dgv.Rows[i].Cells[_stockColumnIndex] == null)
                //    dgv.Rows[i].Cells[_stockColumnIndex] = new DataGridViewComboBoxCell();
                var stockcombosell = (DataGridViewComboBoxCell)dgv.Rows[i].Cells[_stockColumnIndex];
                if (!stockcombosell.Items.Contains(stockname))
                    stockcombosell.Items.Add(stockname);

                // stockcombosell.Value = stockname;
                _invoiceItemsDataTable.Rows[i][_stockNameColumnName] = stockname;

                var itemcombosell = (DataGridViewComboBoxCell)dgv.Rows[i].Cells[_itemColumnIndex];

                itemcombosell.Items.AddRange(StockItems(stockref));
                if (!itemcombosell.Items.Contains(itemname))
                    itemcombosell.Items.Add(itemname);


                // itemcombosell.Value = itemname;
                _invoiceItemsDataTable.Rows[i][_itemNameColumnName] = itemname;
            }
        }
        protected virtual string[] StockItems(int stockref)
        {
            throw new NotImplementedException();
        }
        protected void UpdateTotalPrices()
        {
            for (int i = 0; i < _invoiceItemsDataTable.Rows.Count; i++)
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
