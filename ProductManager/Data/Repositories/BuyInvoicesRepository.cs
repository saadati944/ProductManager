using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.SqlClient;

namespace Tappe.Data.Repositories
{
    public class BuyInvoicesRepository : IRepository
    {
        private DataTable _dataTable = null;
        private readonly Database _database;
        private SqlConnection _connection;
        private SqlDataAdapter _dataAdapter;

        private const string _numberColumnName = "Number";
        private const string _partyRefColumnName = "PartyRef";
        private const string _partyColumnName = "Party";
        private const string _userRefColumnName = "UserRef";
        private const string _userColumnName = "User";
        private const string _dateColumnName = "Date";
        private const string _persianDateColumnName = "PersianDate";
        private const string _totalPriceColumnName = "TotalPrice";
        private const string _typeColumnName = "Type";
        private const string _stockRefColumnName = "StockRef";
        private const string _stockColumnName = "Stock";

        public event DataChangeHandler DataChanged;

        public BuyInvoicesRepository()
        {
            _database = container.Create<Database>();
        }

        public DataTable DataTable
        {
            get
            {
                if (_dataTable == null)
                    Update();
                return _dataTable;
            }
        }

        public void Update()
        {
            if (_connection == null)
            {
                _connection = _database.GetConnection();
                _dataAdapter = _database.GetDataAdapter<Models.BuyInvoice>(_connection);
            }
            if (_dataTable == null)
                _dataTable = new DataTable();
            else
                _dataTable.Clear();

            _dataAdapter.Fill(_dataTable);

            if (!_dataTable.Columns.Contains(_userColumnName))
                _dataTable.Columns.Add(_userColumnName);
            if (!_dataTable.Columns.Contains(_partyColumnName))
                _dataTable.Columns.Add(_partyColumnName);
            if(!_dataTable.Columns.Contains(_typeColumnName))
                _dataTable.Columns.Add(_typeColumnName);
            if (!_dataTable.Columns.Contains(_stockColumnName))
                _dataTable.Columns.Add(_stockColumnName);
            if (!_dataTable.Columns.Contains(_persianDateColumnName))
                _dataTable.Columns.Add(_persianDateColumnName);

            for (int i = 0; i < _dataTable.Rows.Count; i++)
            {
                DataRow dr = _dataTable.Rows[i];

                Models.Party party = new Models.Party { Id = (int)dr[_partyRefColumnName] };
                party.Load();
                dr[_partyColumnName] = party;

                Models.User user = new Models.User { Id = (int)dr[_userRefColumnName] };
                user.Load();
                dr[_userColumnName] = user;

                Models.Stock stock = new Models.Stock { Id = (int)dr[_stockRefColumnName] };
                stock.Load();
                dr[_stockColumnName] = stock;

                dr[_persianDateColumnName] = Models.PersianDate.PersianDateStringFromDateTime((DateTime)dr[_dateColumnName]);

                dr[_typeColumnName] = "خرید";
            }


            if (DataChanged != null)
                DataChanged();
        }
    }

    /*
     class InvoicesRepository
    {
        private readonly Business.BuyInvoiceBusiness _buyInvoiceBusiness;
        private readonly Business.SellInvoiceBusiness _sellInvoiceBusiness;

        private List<Models.SellInvoice> _sellInvoices = null;
        private List<Models.SellInvoiceItem> _sellInvoiceItems = null;
        private List<Models.BuyInvoice> _buyInvoices = null;
        private List<Models.BuyInvoiceItem> _buyInvoiceItems = null;

        public IEnumerable<Models.SellInvoice> SellInvoices
        {
            get
            {
                if (_sellInvoices == null)
                    Update();
                return _sellInvoices;
            }
        }
        public IEnumerable<Models.SellInvoiceItem> SellInvoiceItems
        {
            get
            {
                if (_sellInvoiceItems == null)
                    Update();
                return _sellInvoiceItems;
            }
        }
        public IEnumerable<Models.BuyInvoice> BuyInvoices
        {
            get
            {
                if (_buyInvoices == null)
                    Update();
                return _buyInvoices;
            }
        }
        public IEnumerable<Models.BuyInvoiceItem> BuyInvoiceItems
        {
            get
            {
                if (_buyInvoiceItems == null)
                    Update();
                return _buyInvoiceItems;
            }
        }

        public delegate void DataChangeHandler();
        public event DataChangeHandler DataChanged;

        public InvoicesRepository()
        {
            _buyInvoiceBusiness = container.Create<Business.BuyInvoiceBusiness>();
            _sellInvoiceBusiness = container.Create<Business.SellInvoiceBusiness>();
        }

        public void Update()
        {
            _sellInvoices = _sellInvoiceBusiness.Invoices.ToList();
            _sellInvoiceItems = _sellInvoiceBusiness.InvoiceItems.ToList();
            _buyInvoices = _buyInvoiceBusiness.Invoices.ToList();
            _buyInvoiceItems = _buyInvoiceBusiness.InvoiceItems.ToList();
            if (DataChanged != null)
                DataChanged.Invoke();
        }
    }
     */
}
