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
using Tappe.Data.Repositories;
using StructureMap;

namespace Tappe.Forms
{
    public partial class FrmMain : Form
    {
        private readonly StructureMap.IContainer container;

        private readonly Data.Repositories.ItemsRepository _itemsRepository;
        private readonly Data.Repositories.MeasurementUnitsRepository _measurementUnitsRepository;
        private readonly Data.Repositories.SellInvoicesRepository _sellInvoicesRepository;
        private readonly Data.Repositories.BuyInvoicesRepository _buyInvoicesRepository;

        private readonly BuyInvoiceBusiness _buyInvoiceBusiness;
        private readonly SellInvoiceBusiness _sellInvoiceBusiness;

        private readonly Permissions _permissions;
        private readonly List<FrmGridView> _gridViewForms = new List<FrmGridView>();

        private int childFormLeft = 0;
        private int childFormTop = 0;
        public FrmMain(ItemsRepository itemsRepository, MeasurementUnitsRepository measurementUnitsRepository, SellInvoicesRepository sellInvoicesRepository, BuyInvoicesRepository buyInvoicesRepository, BuyInvoiceBusiness buyInvoiceBusiness, SellInvoiceBusiness sellInvoiceBusiness, Permissions permissions)
        {
            container = Program.Container;
            InitializeComponent();
            _itemsRepository = itemsRepository;
            _measurementUnitsRepository = measurementUnitsRepository;
            _measurementUnitsRepository.Update();

            _buyInvoiceBusiness = buyInvoiceBusiness;
            _sellInvoiceBusiness = sellInvoiceBusiness;
            _sellInvoicesRepository = _sellInvoiceBusiness.SellInvoicesRepository;
            _buyInvoicesRepository = _buyInvoiceBusiness.BuyInvoicesRepository;
            _permissions = permissions;

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
            var frm = container.GetInstance<FrmItemsGridView>();
            ShowChildForm(frm);
        }
        private void AddInvoiceGridViewForm(FrmInvoicesGridView.InvoiceFormType formtype)
        {
            var frm = new FrmInvoicesGridView(formtype, this, container);
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
            var frm = container.GetInstance<FrmInventoryGridView>();
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
            container.GetInstance<FrmCreateItem>().ShowDialog();
            UpdateItemsRepo();
        }

        private void btnAddBuyingInvoice_Click(object sender, EventArgs e)
        {
            int invoiceNumber = _buyInvoiceBusiness.GetLockedInvoiceNumber();
            try
            {
                new FrmCreateBuyInvoice(invoiceNumber, container).ShowDialog();
                UpdateInvoicesRepo();
            }
            finally
            {
                _buyInvoiceBusiness.UnlockInvoiceNumber(invoiceNumber);
            }
        }

        private void btnAddSellingInvoice_Click(object sender, EventArgs e)
        {
            int invoiceNumber = _sellInvoiceBusiness.GetLockedInvoiceNumber();
            try
            {
                new FrmCreateSellInvoice(invoiceNumber, container).ShowDialog();
                UpdateInvoicesRepo();
            }
            finally
            {
                _sellInvoiceBusiness.UnlockInvoiceNumber(invoiceNumber);
            }
        }

        private void btnMeasurementUnits_Click(object sender, EventArgs e)
        {
            container.GetInstance<FrmMeasurementUnits>().ShowDialog();
            UpdateMeasurementUnitsRepo();
        }

        private void pnlForms_Resize(object sender, EventArgs e)
        {
            foreach (var x in _gridViewForms)
                x.CheckDimensions();
        }

        private void btnStocks_Click(object sender, EventArgs e)
        {
            container.GetInstance <FrmAddStock>().ShowDialog();
            UpdateItemsRepo();
        }

        private void btnPricesList_Click(object sender, EventArgs e)
        {
            ShowChildForm(new FrmItemsLastPrice(this, container));
        }

        private void btnAddItemPrice_Click(object sender, EventArgs e)
        {
            container.GetInstance < FrmAddItemPrice>().ShowDialog();
            UpdateMeasurementUnitsRepo();
        }

        private void btnLogOut_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnUserInfo_Click(object sender, EventArgs e)
        {
            container.GetInstance < FrmUserInfo >().ShowDialog();
            UserUpdated();
        }

        private void btnPermissions_Click(object sender, EventArgs e)
        {
            container.GetInstance < FrmPermissions>().ShowDialog();
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            SuspendLayout();
            CheckPermissions();
            ResumeLayout();
        }

        private void btnAddUser_Click(object sender, EventArgs e)
        {
            container.GetInstance < FrmSignUp>().ShowDialog();
        }

        private void FrmMain_KeyDown(object sender, KeyEventArgs e)
        {
            if(e.KeyCode == Keys.Escape)
                btnLogOut_Click(null, null);
        }
    }
}
