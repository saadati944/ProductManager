using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLayer.Models
{
    public class Stock : Model
    {
        private const string _tableName = "Stocks";
        public const string NameColumnName = "Name";

        public string Name { get; set; }

        public override string[] Columns()
        {
            return new string[] { NameColumnName };
        }

        public override string[] GetValues()
        {
            return new string[] { Name };
        }

        public override void MapToModel(DataRow row)
        {
            base.MapToModel(row);
            Name = GetField(row, NameColumnName, Name);
        }

        public override string TableName()
        {
            return _tableName;
        }
        public override string ToString()
        {
            return Name;
        }
    }
}
