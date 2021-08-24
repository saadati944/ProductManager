using System;
using System.Data;

namespace DataLayer.Models
{
    public abstract class Model
    {
        public const string IdColumnName = "Id";

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
                Id = GetField(row, "Id", Id);
            //if (Id == -1)
            //    Id = (int)row["Id"];
        }
        public virtual void Include() { }

        public static T GetField<T>(DataRow row, string col, T def)
        {
            if (row[col] is DBNull)
                return def;
            T val = row.Field<T>(col);
            return val == null ? def : val;
        }
    }
}