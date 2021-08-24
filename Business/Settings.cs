using DataLayer;
using DataLayer.Models;
using System;
using System.Linq;

namespace Business
{
    public class Settings
    {
        private readonly Database _database;

        public Settings(Database database)
        {
            _database = database;
        }

        public string GetSetting(string name, string def = null)
        {
            var s = GetByName(name);
            return s == null ? def : s.Value;
        }
        public void SetSetting(string name, string value)
        {
            var s = GetByName(name);
            if (s == null)
                s = new Setting();
            s.Name = name;
            s.Value = value;
            s.UserRef = Database.LoggedInUser.Id;
            _database.Save(s);
        }

        private Setting GetByName(string name)
        {
            try
            {
                return _database.GetAll<Setting>(null, null, String.Format("Name='{0}' AND UserRef={1}", name.Replace("''", "''"), Database.LoggedInUser.Id)).First();
            }
            catch { }
            return null;
        }
    }
}
