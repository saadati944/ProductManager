using System.Data;

namespace Framework.DataLayer.Models
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
