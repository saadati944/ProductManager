using System.Data;

namespace DataLayer.Models
{
    public class InvoiceLock : Model
    {
        private const string _tableName = "InvoiceLocks";

        public const string InvoiceTypeColumnName = "InvoiceType";
        public const string InvoiceNumberColumnName = "InvoiceNumber";

        private bool _invoiceType;
        public Invoice.InvoiceType InvoiceType
        {
            get
            {
                return _invoiceType ? Invoice.InvoiceType.Selling : Invoice.InvoiceType.Buying;
            }
            set
            {
                _invoiceType = value == Invoice.InvoiceType.Selling;
            }
        }
        public int InvoiceNumber { get; set; }

        public override void MapToModel(DataRow row)
        {
            base.MapToModel(row);
            _invoiceType = GetField(row, InvoiceTypeColumnName, _invoiceType);
            InvoiceNumber = GetField(row, InvoiceNumberColumnName, InvoiceNumber);
        }


        public override string[] Columns()
        {
            return new string[] { InvoiceTypeColumnName, InvoiceNumberColumnName };
        }

        public override string[] GetValues()
        {
            return new string[] { _invoiceType.ToString(), InvoiceNumber.ToString() };
        }

        public override string TableName()
        {
            return _tableName;
        }
    }
}
