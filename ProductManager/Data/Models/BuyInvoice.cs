using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tappe.Data.Models
{
    public class BuyInvoice : Invoice
    {
        private const string _tableName = "BuyInvoices";



        public override InvoiceType GetInvoiceType()
        {
            return InvoiceType.Buying;
        }


        public override string[] Columns()
        {
            return new string[] { NumberColumnName, PartyRefColumnName, UserRefColumnName, DateColumnName, TotalPriceColumnName};
        }

        public override string[] GetValues()
        {
            return new string[] { Number.ToString(), PartyRef.ToString(), UserRef.ToString(), Date.ToString("yyyy-MM-dd"), TotalPrice.ToString() };
        }

        public override void Include()
        {
            User = new User { Id = UserRef };
            Party = new Party { Id = PartyRef };

            User.Load();
            Party.Load();
        }

        public override void MapToModel(DataRow row)
        {
            base.MapToModel(row);
            Number = GetField(row, NumberColumnName, Number);
            PartyRef = GetField(row, PartyRefColumnName, PartyRef);
            UserRef = GetField(row, UserRefColumnName, UserRef);
            Date = GetField(row, DateColumnName, Date);
            TotalPrice = GetField(row, TotalPriceColumnName, TotalPrice);
        }

        public override string TableName()
        {
            return _tableName;
        }
    }
}
