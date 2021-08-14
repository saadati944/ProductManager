using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tappe.Data.Models
{
    public class Item : Model
    {
        private const string _nameColumnName = "Name";
        private const string _descriptionColumnName = "Description";
        private const string _creatorRefColumnName = "CreatorRef";
        private const string _priceColumnName = "Price";
        private const string _measurementUnitRefColumnName = "MeasurementUnitRef";
        private const string _tableName = "Items";

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
            Name = Field(row, _nameColumnName, Name);
            Description = Field(row, _descriptionColumnName, Description);
            CreatorRef = Field(row, _creatorRefColumnName, CreatorRef);
            Price = Field(row, _priceColumnName, Price);
            MeasurementUnitRef = Field(row, _measurementUnitRefColumnName, MeasurementUnitRef);
        }

        public override string[] Columns()
        {
            return new string[]{ _nameColumnName, _descriptionColumnName, _creatorRefColumnName, _priceColumnName, _measurementUnitRefColumnName };
        }

        public override string[] GetValues()
        {
            return new string[] { Name, Description, CreatorRef.ToString(), Price.ToString(), MeasurementUnitRef.ToString()};
        }

        public override void Include()
        {
            Creator = new User { Id = CreatorRef };
            MeasurementUnit = new MeasurementUnit { Id = MeasurementUnitRef };
            Creator.Load();
            MeasurementUnit.Load();
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
