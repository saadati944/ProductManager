using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tappe.Data;
using Tappe.Data.Models;

namespace Tappe.Business
{
    public class Settings
    {
        private readonly Database _database;

        public Settings()
        {
            _database = container.Create<Database>();
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
            _database.Save(s);
        }

        private Setting GetByName(string name)
        {
            try
            {
                return _database.GetAll<Setting>(null, null, "Name='" + name.Replace("''", "''") + "'").First();
            }
            catch { }
            return null;
        }
    }
}
