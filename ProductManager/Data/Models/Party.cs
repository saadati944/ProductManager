using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tappe.Data.Models
{
    public class Party : Model
    {
        private const string _nameColumnName = "Name";
        private const string _tableName = "Parties";
        public string Name { get; set; }

        public override string[] Columns()
        {
            return new string[] { _nameColumnName };
        }

        public override string[] GetValues()
        {
            return new string[] { Name };
        }
        public override void MapToModel(System.Data.DataRow row)
        {
            base.MapToModel(row);
            Name = Field(row, _nameColumnName, Name);
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
