using Framework.DataLayer.Models;
using System;
using System.Linq;

namespace Framework.Business
{
    public class SettingsBusiness : Interfaces.ISettingsBusiness
    {
        private readonly Interfaces.IDatabase _database;

        public SettingsBusiness(Interfaces.IDatabase database)
        {
            _database = database;
        }

        public string GetSetting(string name, string defaultValue = null)
        {
            var s = GetByName(name);
            return s == null ? defaultValue : s.Value;
        }
        public void SetSetting(string name, string value)
        {
            var s = GetByName(name);
            if (s == null)
                s = new Setting();
            s.Name = name;
            s.Value = value;
            s.UserRef = Utilities.LoggedInUser.User.Id;
            _database.Save(s);
        }

        private Setting GetByName(string name)
        {
            return _database.GetAll<Setting>(null, null, String.Format("Name='{0}' AND UserRef={1}", name.Replace("''", "''"), Utilities.LoggedInUser.User.Id)).FirstOrDefault();
        }
    }
}
