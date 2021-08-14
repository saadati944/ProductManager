using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tappe.Data.Models
{
    class Permission : Model
    {
        private const string _tableName = "Permissions";
        private const string _nameColumnName = "Name";
        private const string _valueColumnName = "Value";
        private const string _userRefColumnName = "UserRef";

        public string Name { get; set; }
        public string Value { get; set; }
        public int UserRef { get; set; }
        public User User { get; set; }

        public new bool Included
        {
            get
            {
                return User != null;
            }
        }
        public override void Include()
        {
            User = new User { Id = UserRef };

            User.Load();
        }

        public override string[] Columns()
        {
            return new string[] { _userRefColumnName, _nameColumnName, _valueColumnName };
        }

        public override string[] GetValues()
        {
            return new string[] { UserRef.ToString(), Name, Value };
        }
        public override void MapToModel(System.Data.DataRow row)
        {
            base.MapToModel(row);
            Name = Field(row, _nameColumnName, Name);
            Value = Field(row, _valueColumnName, Value);
            UserRef = Field(row, _userRefColumnName, UserRef);
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
