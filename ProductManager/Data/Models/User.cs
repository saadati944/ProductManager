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

        public const string FirstnameColumnName = "Firstname";
        public const string LastnameColumnName = "Lastname";
        public const string PasswordnameColumnName = "Password";
        public const string AgenameColumnName = "Age";
        public const string GendernameColumnName = "Gender";

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
            return new string[] { FirstnameColumnName, LastnameColumnName, PasswordnameColumnName, AgenameColumnName, GendernameColumnName };
        }

        public override string[] GetValues()
        {
            return new string[] { FirstName, LastName, Password, Age.ToString(), Gender.ToString() };
        }


        public override void MapToModel(DataRow row)
        {
            base.MapToModel(row);
            FirstName = GetField(row, FirstnameColumnName, FirstName);
            LastName = GetField(row, LastnameColumnName, LastName);
            Password = GetField(row, PasswordnameColumnName, Password);
            Age = GetField(row, AgenameColumnName, Age);
            Gender = GetField(row, GendernameColumnName, Gender);
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
