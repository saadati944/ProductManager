using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.DataLayer.Models
{
    public class PermissionViewModel
    {
        public string Key { get; set; }
        public string DisplayName { get; set; }
        public bool Permitted { get; set; }
    }
}
