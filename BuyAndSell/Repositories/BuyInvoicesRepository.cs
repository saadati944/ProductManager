using Framework.Interfaces;
using Framework.DataLayer.Models;
using BuyAndSell.DataLayer.Models;
using BuyAndSell.Interfaces;
using BasicData.DataLayer.Models;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;

namespace BuyAndSell.Repositories
{
    public class BuyInvoicesRepository : IBuyInvoicesRepository
    {
        private DataTable _dataTable = null;
        private readonly IDatabase _database;
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

        public BuyInvoicesRepository(IDatabase database)
        {
            _database = database;
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
                _dataAdapter = _database.GetDataAdapter<BuyInvoice>(_connection);
            }
            if (_dataTable == null)
                _dataTable = new DataTable();
            else
                _dataTable.Clear();

            _dataAdapter.Fill(_dataTable);

            if (!_dataTable.Columns.Contains(_userColumnName))
                _dataTable.Columns.Add(_userColumnName, typeof(User));
            if (!_dataTable.Columns.Contains(_partyColumnName))
                _dataTable.Columns.Add(_partyColumnName, typeof(Party));
            if (!_dataTable.Columns.Contains(_typeColumnName))
                _dataTable.Columns.Add(_typeColumnName, typeof(string));
            if (!_dataTable.Columns.Contains(_stockColumnName))
                _dataTable.Columns.Add(_stockColumnName, typeof(Stock));
            if (!_dataTable.Columns.Contains(_persianDateColumnName))
                _dataTable.Columns.Add(_persianDateColumnName, typeof(string));

            for (int i = 0; i < _dataTable.Rows.Count; i++)
            {
                DataRow dr = _dataTable.Rows[i];

                Party party = new Party { Id = (int)dr[_partyRefColumnName] };
                _database.Load(party);
                dr[_partyColumnName] = party;

                User user = new User { Id = (int)dr[_userRefColumnName] };
                _database.Load(user);
                dr[_userColumnName] = user;

                dr[_persianDateColumnName] = PersianDate.PersianDateStringFromDateTime((DateTime)dr[_dateColumnName]);

                dr[_typeColumnName] = "خرید";
            }


            if (DataChanged != null)
                DataChanged();
        }
        public DataTable NewInvoiceDataTable()
        {
            return DataTable.Clone();
        }
        public DataTable NewInvoiceItemDataTable()
        {
            return _database.GetAllDataset<BuyInvoiceItem>(null, null, "1=0", null, 0).Tables[0];
        }

        public Invoice GetInvoiceModel(int number)
        {
            BuyInvoice invoice = new BuyInvoice();
            invoice.MapToModel(GetInvoice(number).Rows[0]);
            return invoice;
        }

        public DataTable GetInvoice(int number)
        {
            DataTable table = NewInvoiceDataTable();
            for (int i = 0; i < DataTable.Rows.Count; i++)
                if (DataTable.Rows[i].Field<int>(_numberColumnName) == number)
                {
                    DataRow row = table.NewRow();
                    row.ItemArray = DataTable.Rows[i].ItemArray;
                    table.Rows.Add(row);
                    break;
                }
            return table;
        }

        public IEnumerable<InvoiceItem> GetInvoiceItemsModel(int invoiceid, SqlConnection connection = null, SqlTransaction transaction = null)
        {
            return _database.GetAll<BuyInvoiceItem>(connection, transaction, "BuyInvoiceRef=" + invoiceid);
        }

        public DataTable GetInvoiceItems(int invoiceid, SqlConnection connection = null, SqlTransaction transaction = null)
        {
            return _database.GetAllDataset<BuyInvoiceItem>(connection, transaction, "BuyInvoiceRef=" + invoiceid).Tables[0];
        }
    }

}
