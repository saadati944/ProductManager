using Framework.DataLayer.Models;
using System.Collections.Generic;
using System.Linq;

namespace Framework.Business
{


    public class PermissionsBusiness : Interfaces.IPermissionsBusiness
    {
        private readonly Interfaces.IDatabase _database;

        // create/add permissions
        public const string CreateSellInvoicePermission = "CreateSellInvoicePermission";
        public const string EditSellInvoicePermission = "EditSellInvoicePermission";
        public const string RemoveSellInvoicePermission = "RemoveSellInvoicePermission";
        public const string CreateBuyInvoicePermission = "CreateBuyInvoicePermission";
        public const string EditBuyInvoicePermission = "EditBuyInvoicePermission";
        public const string RemoveBuyInvoicePermission = "RemoveBuyInvoicePermission";

        public const string CreateItemPermission = "CreateItemPermission";
        public const string EditItemPermission = "CreateItemPermission";
        public const string RemoveItemPermission = "CreateItemPermission";

        public const string CreateStockPermission = "CreateStockPermission";
        public const string CreateMeasurementUnitPermission = "CreateMeasurementUnitPermission";
        public const string AddItemPricePermission = "AddItemPricePermission";

        // view permissions
        public const string ViewInventoryPermission = "ViewInventoryPermission";
        public const string ViewItemsPermission = "ViewItemsPermission";
        public const string ViewSellInvoicesPermission = "ViewSellInvoicesPermission";
        public const string ViewBuyInvoicesPermission = "ViewBuyInvoicesPermission";
        public const string ViewItemsPriceListPermission = "ViewItemsPriceListPermission";

        public const string Permission = "";

        public PermissionsBusiness(Interfaces.IDatabase database)
        {
            _database = database;
        }

        public List<PermissionViewModel> GetAllPermissions(int userref)
        {
            List<PermissionViewModel> ps = new List<PermissionViewModel>();
            ps.Add(new PermissionViewModel { Key = CreateSellInvoicePermission, DisplayName = "ثبت فاکتور فروش" });
            ps.Add(new PermissionViewModel { Key = EditSellInvoicePermission, DisplayName = "ویرایش فاکتور فروش" });
            ps.Add(new PermissionViewModel { Key = RemoveSellInvoicePermission, DisplayName = "حذف فاکتور فروش" });

            ps.Add(new PermissionViewModel { Key = CreateBuyInvoicePermission, DisplayName = "ثبت فاکتور خرید" });
            ps.Add(new PermissionViewModel { Key = EditBuyInvoicePermission, DisplayName = "ویرایش فاکتور خرید" });
            ps.Add(new PermissionViewModel { Key = RemoveBuyInvoicePermission, DisplayName = "حذف فاکتور خرید" });

            ps.Add(new PermissionViewModel { Key = CreateItemPermission, DisplayName = "افزودن محصولات" });
            ps.Add(new PermissionViewModel { Key = EditItemPermission, DisplayName = "ویرایش محصولات" });
            ps.Add(new PermissionViewModel { Key = RemoveItemPermission, DisplayName = "حذف محصولات" });
            ps.Add(new PermissionViewModel { Key = CreateStockPermission, DisplayName = "مدیریت انبار ها" });

            ps.Add(new PermissionViewModel { Key = CreateMeasurementUnitPermission, DisplayName = "مدیریت واحد های اندازه گیری" });

            ps.Add(new PermissionViewModel { Key = AddItemPricePermission, DisplayName = "ثبت قیمت برای محصولات" });
            ps.Add(new PermissionViewModel { Key = ViewInventoryPermission, DisplayName = "مشاهده موجودی محصولات" });
            ps.Add(new PermissionViewModel { Key = ViewItemsPermission, DisplayName = "مشاهده لیست محصولات" });
            ps.Add(new PermissionViewModel { Key = ViewSellInvoicesPermission, DisplayName = "مشاهده فاکتور های فروش" });
            ps.Add(new PermissionViewModel { Key = ViewBuyInvoicesPermission, DisplayName = "مشاهده فاکتور های خرید" });
            ps.Add(new PermissionViewModel { Key = ViewItemsPriceListPermission, DisplayName = "مشاهده لیست قیمت محصولات" });

            foreach (var x in ps)
                x.Permitted = GetPermission(x.Key, userref);

            return ps;
        }


        public bool GetLoggedInUserPermission(string name)
        {
            return Utilities.LoggedInUser.User.Id == 1 || GetPermission(name, Utilities.LoggedInUser.User.Id);
        }

        public void SetPermission(string name, int userRef, bool value)
        {
            SetPermission(name, value.ToString(), userRef);
        }
        public bool GetPermission(string name, int userRef)
        {
            return GetPermission(name, userRef, null) == "True";
        }

        private string GetPermission(string name, int userRef, string def)
        {
            var s = GetByName(name, userRef);
            return s == null ? def : s.Value;
        }
        private void SetPermission(string name, string value, int userRef)
        {
            var s = GetByName(name, userRef);
            if (s == null)
                s = new Permission();
            s.Name = name;
            s.Value = value;
            s.UserRef = userRef;
            _database.Save(s);
        }
        private Permission GetByName(string name, int userRef)
        {
            return _database.GetAll<Permission>(null, null, "UserRef = " + userRef + " AND Name = '" + name.Replace("''", "'") + "'").FirstOrDefault();
        }
    }
}
