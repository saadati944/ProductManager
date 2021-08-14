using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace Tappe.Data.Models
{
    public abstract class Model
    {
        private int _id = -1;
        public int Id
        {
            get
            {
                return _id;
            }
            set
            {
                _id = value;
            }
        }

        public bool Included { get { return true; } }

        public abstract string[] Columns();
        public abstract string TableName();
        public abstract string[] GetValues();
        public virtual void MapToModel(DataRow row)
        {
            if (Id == -1)
                Id = Field(row, "Id", Id);
            //if (Id == -1)
            //    Id = (int)row["Id"];
        }
        public virtual void Include() { }

        protected T Field<T>(DataRow row, string col, T def)
        {
            T val = row.Field<T>(col);
            return val == null ? def : val;
        }
    }
}