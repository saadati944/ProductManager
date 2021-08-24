using System.Data;

namespace DataLayer.Models
{
    public abstract class VersionableModel : Model
    {
        public const string VersionColumnName = "Version";
        public byte[] Version { private set; get; }
        public override void MapToModel(DataRow row)
        {
            base.MapToModel(row);
            Version = GetField(row, VersionColumnName, Version);
        }
    }
}
