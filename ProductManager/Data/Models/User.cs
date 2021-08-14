using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tappe.Data.Models
{
    public class User : Model
    {
        private const string _tableName = "Users";
        private const string _firstnameColumnName = "Firstname";
        private const string _lastnameColumnName = "Lastname";
        private const string _passwordnameColumnName = "Password";
        private const string _agenameColumnName = "Age";
        private const string _gendernameColumnName = "Gender";

        public int Age { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public string Password { get; set; }

        public string FullName
        {
            get
            {
                return String.Format("{0} {1}", FirstName, LastName);
            }
        }

        // 1 : male, 0 : female
        public bool Gender { get; set; }

        public override string[] Columns()
        {
            return new string[] { _firstnameColumnName, _lastnameColumnName, _passwordnameColumnName, _agenameColumnName, _gendernameColumnName };
        }

        public override string[] GetValues()
        {
            return new string[] { FirstName, LastName, Password, Age.ToString(), Gender.ToString() };
        }


        public override void MapToModel(DataRow row)
        {
            base.MapToModel(row);
            FirstName = GetField(row, _firstnameColumnName, FirstName);
            LastName = GetField(row, _lastnameColumnName, LastName);
            Password = GetField(row, _passwordnameColumnName, Password);
            Age = GetField(row, _agenameColumnName, Age);
            Gender = GetField(row, _gendernameColumnName, Gender);
        }

        public override string TableName()
        {
            return _tableName;
        }

        public override string ToString()
        {
            return String.Format("{0}  {1}", Gender ? "آقای" : "خانم", FullName);
        }
    }
}
