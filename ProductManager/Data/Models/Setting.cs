using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tappe.Data.Models
{
    //TODO: use userid to isolate settings per user
    class Setting : Model
    {
        private const string _nameColumnName = "Name";
        private const string _valueColumnName = "Value";
        private const string _tableName = "Settings";

        public string Name { get; set; }
        public string Value { get; set; }

        public override string[] Columns()
        {
            return new string[] { _nameColumnName, _valueColumnName };
        }

        public override string[] GetValues()
        {
            return new string[] { Name, Value };
        }
        public override void MapToModel(System.Data.DataRow row)
        {
            base.MapToModel(row);
            Name = Field(row, _nameColumnName, Name);
            Value = Field(row, _valueColumnName, Value);
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
