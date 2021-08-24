namespace DataLayer.Models
{
    public class Party : Model
    {
        private const string _tableName = "Parties";

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
        public override void MapToModel(System.Data.DataRow row)
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
