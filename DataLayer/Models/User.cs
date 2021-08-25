using System;
using System.Data;

namespace DataLayer.Models
{
    public class User : Model
    {
        private const string _tableName = "Users";

        public const string UsernameColumnName = "Username";
        public const string FullnameColumnName = "Fullname";
        public const string PasswordnameColumnName = "Password";
        public const string AgenameColumnName = "Age";
        public const string GendernameColumnName = "Gender";

        public int Age { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string Password { get; set; }


        // 1 : male, 0 : female
        public bool Gender { get; set; }

        public override string[] Columns()
        {
            return new string[] { UsernameColumnName, FullnameColumnName, PasswordnameColumnName, AgenameColumnName, GendernameColumnName };
        }

        public override string[] GetValues()
        {
            return new string[] { UserName, FullName, Password, Age.ToString(), Gender.ToString() };
        }


        public override void MapToModel(DataRow row)
        {
            base.MapToModel(row);
            UserName = GetField(row, UsernameColumnName, UserName);
            FullName = GetField(row, FullnameColumnName, FullName);
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
