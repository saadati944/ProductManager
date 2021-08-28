using Framework.DataLayer.Models;
using System.Collections.Generic;

namespace Framework.Interfaces
{
    public interface IPermissionsBusiness
    {
        List<PermissionViewModel> GetAllPermissions(int userref);
        bool GetLoggedInUserPermission(string name);
        void SetPermission(string name, int userRef, bool value);
        bool GetPermission(string name, int userRef);

    }
}
