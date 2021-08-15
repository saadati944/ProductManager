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




        public override InvoiceType GetInvoiceType()
        {
            return InvoiceType.Selling;
        }

       

        public override string[] Columns()
        {
            return new string[] { NumberColumnName, PartyRefColumnName, UserRefColumnName, DateColumnName, TotalPriceColumnName, StockRefColumnName};
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
            Number = GetField(row, NumberColumnName, Number);
            PartyRef = GetField(row, PartyRefColumnName, PartyRef);
            UserRef = GetField(row, UserRefColumnName, UserRef);
            Date = GetField(row, DateColumnName, Date);
            TotalPrice = GetField(row, TotalPriceColumnName, TotalPrice);
            StockRef = GetField(row, StockRefColumnName, StockRef);
        }

        public override string TableName()
        {
            return _tableName;
        }

        
    }
}
