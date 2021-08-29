namespace Framework.DataLayer.Models
{
    public class Setting : Model
    {
        private const string _tableName = "Settings";

        public const string NameColumnName = "Name";
        public const string ValueColumnName = "Value";
        public const string UserRefColumnName = "UserRef";

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

            Load(User);
        }

        public override string[] Columns()
        {
            return new string[] { NameColumnName, ValueColumnName, UserRefColumnName };
        }

        public override string[] GetValues()
        {
            return new string[] { Name, Value, UserRef.ToString() };
        }
        public override void MapToModel(System.Data.DataRow row)
        {
            base.MapToModel(row);
            Name = GetField(row, NameColumnName, Name);
            Value = GetField(row, ValueColumnName, Value);
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
