using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tappe.Business;

namespace Tappe.Forms
{
    public partial class FrmMain : Form
    {
        private readonly Data.Repositories.ItemsRepository _itemsRepository;
        private readonly Data.Repositories.MeasurementUnitsRepository _measurementUnitsRepository;
        private readonly Data.Repositories.SellInvoicesRepository _sellInvoicesRepository;
        private readonly Data.Repositories.BuyInvoicesRepository _buyInvoicesRepository;
        private readonly Permissions _permissions;
        private readonly List<FrmGridView> _gridViewForms = new List<FrmGridView>();

        private int childFormLeft = 0;
        private int childFormTop = 0;
        public FrmMain()
        {
            container.Reset();
            InitializeComponent();
            _itemsRepository = container.Create<Data.Repositories.ItemsRepository>();
            _measurementUnitsRepository = container.Create<Data.Repositories.MeasurementUnitsRepository>();
            _measurementUnitsRepository.Update();
            _sellInvoicesRepository = container.Create<Data.Repositories.SellInvoicesRepository>();
            _buyInvoicesRepository = container.Create<Data.Repositories.BuyInvoicesRepository>();
            _permissions = container.Create<Permissions>();

            UserUpdated();
        }

        private void CheckPermissions()
        {
            if(Program.LoggedInUser.Id == 1)
                return;
            btnPermissions.Visible = false;
            btnAddUser.Visible = false;

            btnInventory.Visible = _permissions.GetLoggedInUserPermission(Permissions.ViewInventoryPermission);
            btnProducts.Visible = _permissions.GetLoggedInUserPermission(Permissions.ViewItemsPermission);

            bool viewsell = _permissions.GetLoggedInUserPermission(Permissions.ViewSellInvoicesPermission);
            bool viewbuy = _permissions.GetLoggedInUserPermission(Permissions.ViewBuyInvoicesPermission);
            btnAllInvoices.Visible = viewbuy && viewsell;
            btnInvoicesSelling.Visible = viewsell;
            btnInvoicesBuying.Visible = viewbuy;

            btnPricesList.Visible = _permissions.GetLoggedInUserPermission(Permissions.ViewItemsPriceListPermission);

            btnAddProduct.Visible = _permissions.GetLoggedInUserPermission(Permissions.CreateEditRemoveItemPermission);
            btnAddSellingInvoice.Visible = _permissions.GetLoggedInUserPermission(Permissions.CreateSellInvoicePermission);
            btnAddBuyingInvoice.Visible = _permissions.GetLoggedInUserPermission(Permissions.CreateBuyInvoicePermission);
            btnAddItemPrice.Visible = _permissions.GetLoggedInUserPermission(Permissions.AddItemPricePermission);

            btnMeasurementUnits.Visible = _permissions.GetLoggedInUserPermission(Permissions.CreateMeasurementUnitPermission);
            btnStocks.Visible = _permissions.GetLoggedInUserPermission(Permissions.CreateStockPermission);

            lblTools.Visible = btnAddProduct.Visible || btnAddSellingInvoice.Visible || btnAddBuyingInvoice.Visible || btnAddItemPrice.Visible;
            lblOthers.Visible = btnMeasurementUnits.Visible || btnStocks.Visible;
        }

        private void UserUpdated()
        {
            lblStatus.Text = "کاربر جاری : " + Program.LoggedInUser.FullName;
        }

        private void AddItemsGridViewForm()
        {
            var frm = new FrmItemsGridView(_itemsRepository);
            ShowChildForm(frm);
        }
        private void AddInvoiceGridViewForm(FrmInvoicesGridView.InvoiceFormType formtype)
        {
            var frm = new FrmInvoicesGridView(formtype, this);
            ShowChildForm(frm);
        }
        public void ShowChildForm(FrmGridView frm)
        {
            pnlForms.Controls.Add(frm);
            _gridViewForms.Add(frm);
            frm.FormClosing += delegate (object sender, FormClosingEventArgs e) { _gridViewForms.Remove((FrmGridView)sender); };

            childFormLeft += 20;
            if (childFormLeft + frm.Width >= pnlForms.Width)
                childFormLeft = 0;

            childFormTop += 20;
            if (childFormTop + frm.Height >= pnlForms.Height)
                childFormTop = 0;

            frm.Left = childFormLeft;
            frm.Top = childFormTop;

            frm.Show();
        }
        private void UpdateItemsRepo()
        {
            _itemsRepository.Update();
        }
        private void UpdateInvoicesRepo()
        {
            //TODO: split invoice updating
            _sellInvoicesRepository.Update();
            _buyInvoicesRepository.Update();
        }
        private void UpdateMeasurementUnitsRepo()
        {
            _measurementUnitsRepository.Update();
        }

        private void btnInventory_Click(object sender, EventArgs e)
        {
            var frm = new FrmInventoryGridView();
            ShowChildForm(frm);
        }

        private void btnProducts_Click(object sender, EventArgs e)
        {
            UpdateItemsRepo();
            AddItemsGridViewForm();
        }

        private void btnInvoicesBuying_Click(object sender, EventArgs e)
        {
            UpdateInvoicesRepo();
            AddInvoiceGridViewForm(FrmInvoicesGridView.InvoiceFormType.BuyingInvoices);
        }

        private void btnInvoicesSelling_Click(object sender, EventArgs e)
        {
            UpdateInvoicesRepo();
            AddInvoiceGridViewForm(FrmInvoicesGridView.InvoiceFormType.SellingInvoices);
        }
        private void btnAllInvoices_Click(object sender, EventArgs e)
        {
            UpdateInvoicesRepo();
            AddInvoiceGridViewForm(FrmInvoicesGridView.InvoiceFormType.All);
        }
        private void btnAddProduct_Click(object sender, EventArgs e)
        {
            new FrmCreateItem().ShowDialog();
            UpdateItemsRepo();
        }

        private void btnAddBuyingInvoice_Click(object sender, EventArgs e)
        {
            try
            {
                new FrmCreateBuyInvoice().ShowDialog();
                UpdateInvoicesRepo();
            }
            catch { }
        }

        private void btnAddSellingInvoice_Click(object sender, EventArgs e)
        {
            try
            {
                new FrmCreateSellInvoice().ShowDialog();
                UpdateInvoicesRepo();
            }
            catch { }
        }

        private void btnMeasurementUnits_Click(object sender, EventArgs e)
        {
            new FrmMeasurementUnits().ShowDialog();
            UpdateMeasurementUnitsRepo();
        }

        private void pnlForms_Resize(object sender, EventArgs e)
        {
            foreach (var x in _gridViewForms)
                x.CheckDimensions();
        }

        private void btnStocks_Click(object sender, EventArgs e)
        {
            new FrmAddStock().ShowDialog();
            UpdateItemsRepo();
        }

        private void btnPricesList_Click(object sender, EventArgs e)
        {
            ShowChildForm(new FrmItemsLastPrice(this));
        }

        private void btnAddItemPrice_Click(object sender, EventArgs e)
        {
            new FrmAddItemPrice().ShowDialog();
            UpdateMeasurementUnitsRepo();
        }

        private void btnLogOut_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnUserInfo_Click(object sender, EventArgs e)
        {
            new FrmUserInfo().ShowDialog();
            UserUpdated();
        }

        private void btnPermissions_Click(object sender, EventArgs e)
        {
            new FrmPermissions().ShowDialog();
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            SuspendLayout();
            CheckPermissions();
            ResumeLayout();
        }

        private void btnAddUser_Click(object sender, EventArgs e)
        {
            new FrmSignUp().ShowDialog();
        }
    }
}
