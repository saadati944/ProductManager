using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tappe.Data.Models
{
    public class SellInvoice : Invoice
    {
        private const string _tableName = "SellInvoices";
        private const string _numberColumnName = "Number";
        private const string _partyRefColumnName = "PartyRef";
        private const string _userRefColumnName = "UserRef";
        private const string _dateColumnName = "Date";
        private const string _totalPriceColumnName = "TotalPrice";
        private const string _stockRefColumnName = "StockRef";

        //public int Number { get; set; }
        //public int PartyRef { get; set; }
        //public Party Party { get; set; }
        //public int UserRef { get; set; }
        //public User User { get; set; }
        //public DateTime Date { get; set; }
        //public decimal TotalPrice { get; set; }

        //public IEnumerable<SellInvoiceItem> InvoiceItems { get; set; }

        public override InvoiceType GetInvoiceType()
        {
            return InvoiceType.Selling;
        }

       

        public override string[] Columns()
        {
            return new string[] { _numberColumnName, _partyRefColumnName, _userRefColumnName, _dateColumnName, _totalPriceColumnName, _stockRefColumnName};
        }

        public override string[] GetValues()
        {
            return new string[] { Number.ToString(), PartyRef.ToString(), UserRef.ToString(), Date.ToString("yyyy-MM-dd"), TotalPrice.ToString(), StockRef.ToString()};
        }

        public override void Include()
        {
            User = new User { Id = UserRef };
            Party = new Party { Id = PartyRef };
            Stock = new Stock { Id = StockRef };

            User.Load();
            Party.Load();
            Stock.Load();
        }

        public override void MapToModel(DataRow row)
        {
            base.MapToModel(row);
            Number = GetField(row, _numberColumnName, Number);
            PartyRef = GetField(row, _partyRefColumnName, PartyRef);
            UserRef = GetField(row, _userRefColumnName, UserRef);
            Date = GetField(row, _dateColumnName, Date);
            TotalPrice = GetField(row, _totalPriceColumnName, TotalPrice);
            StockRef = GetField(row, _stockRefColumnName, StockRef);
        }

        public override string TableName()
        {
            return _tableName;
        }

        
    }
}
