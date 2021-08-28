using System.Data;
using Framework.DataLayer.Models;

namespace BasicData.DataLayer.Models
{
    public class Item : VersionableModel
    {
        private const string _tableName = "Items";

        public const string NameColumnName = "Name";
        public const string DescriptionColumnName = "Description";
        public const string CreatorRefColumnName = "CreatorRef";
        public const string CreatorColumnName = "Creator";
        public const string PriceColumnName = "Price";
        public const string MeasurementUnitRefColumnName = "MeasurementUnitRef";
        public const string MeasurementUnitColumnName = "MeasurementUnit";

        public string Name { get; set; }
        public string Description { get; set; }

        // FK to People.Id
        public int CreatorRef { get; set; }
        public User Creator { get; set; }

        // FK to MeasurementUnits.Id
        public int MeasurementUnitRef { get; set; }

        public MeasurementUnit MeasurementUnit { get; set; }

        public decimal Price { get; set; }

        public new bool Included
        {
            get
            {
                return Creator != null && MeasurementUnit != null;
            }
        }

        public override void MapToModel(DataRow row)
        {
            base.MapToModel(row);
            Name = GetField(row, NameColumnName, Name);
            Description = GetField(row, DescriptionColumnName, Description);
            CreatorRef = GetField(row, CreatorRefColumnName, CreatorRef);
            Price = GetField(row, PriceColumnName, Price);
            MeasurementUnitRef = GetField(row, MeasurementUnitRefColumnName, MeasurementUnitRef);
        }

        public override string[] Columns()
        {
            return new string[] { NameColumnName, DescriptionColumnName, CreatorRefColumnName, PriceColumnName, MeasurementUnitRefColumnName };
        }

        public override string[] GetValues()
        {
            return new string[] { Name, Description, CreatorRef.ToString(), Price.ToString(), MeasurementUnitRef.ToString() };
        }

        public override void Include()
        {
            //Creator = new User { Id = CreatorRef };
            //MeasurementUnit = new MeasurementUnit { Id = MeasurementUnitRef };
            //Creator.Load();
            //MeasurementUnit.Load();
        }

        public override string TableName()
        {
            return _tableName;
        }

        public override string ToString()
        {
            return Name;
            // return $"{Name} _ {(Description.Length < 30 ? Description : Description.Substring(0, 30))}";
        }
    }
}
