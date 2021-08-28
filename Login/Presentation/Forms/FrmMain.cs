using BasicData.Presentation.Forms;
using BuyAndSell.Presentation.Forms;
using BasicData.Interfaces;
using BuyAndSell.Interfaces;
using Framework.Interfaces;
using Framework.Business;
using Framework.DataLayer;
using Framework.Presentation.Forms;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Login.Presentation.Forms
{
    public partial class FrmMain : Form
    {
        private readonly IFormFactory _formFactory;
        private readonly IPermissionsBusiness _permissions;

        private readonly IItemsRepository _itemsRepository;
        private readonly IMeasurementUnitsRepository _measurementUnitsRepository;
        private readonly ISellInvoicesRepository _sellInvoicesRepository;
        private readonly IBuyInvoicesRepository _buyInvoicesRepository;


        private readonly List<FrmGridView> _gridViewForms = new List<FrmGridView>();

        private int childFormLeft = 0;
        private int childFormTop = 0;

        public FrmMain(IFormFactory formFactory, IPermissionsBusiness permissions, IItemsRepository itemsRepository, IMeasurementUnitsRepository measurementUnitsRepository,
            ISellInvoicesRepository sellInvoicesRepository, IBuyInvoicesRepository buyInvoicesRepository)
        {
            _itemsRepository = itemsRepository;
            _measurementUnitsRepository = measurementUnitsRepository;
            _sellInvoicesRepository = sellInvoicesRepository;
            _buyInvoicesRepository = buyInvoicesRepository;

            InitializeComponent();

            _permissions = permissions;
            _formFactory = formFactory;

            Framework.Utilities.FormFactory.OnFormAdded += FormFactory_OnFormAdded;

            UserUpdated();
        }

        private void FormFactory_OnFormAdded(Form frm)
        {
            ShowChildForm((FrmGridView)frm);
        }

        private void CheckPermissions()
        {
            if (Framework.Utilities.LoggedInUser.User.Id == 1)
                return;
            btnPermissions.Visible = false;
            btnAddUser.Visible = false;

            btnInventory.Visible = _permissions.GetLoggedInUserPermission(PermissionsBusiness.ViewInventoryPermission);
            btnProducts.Visible = _permissions.GetLoggedInUserPermission(PermissionsBusiness.ViewItemsPermission);

            bool viewsell = _permissions.GetLoggedInUserPermission(PermissionsBusiness.ViewSellInvoicesPermission);
            bool viewbuy = _permissions.GetLoggedInUserPermission(PermissionsBusiness.ViewBuyInvoicesPermission);
            btnAllInvoices.Visible = viewbuy && viewsell;
            btnInvoicesSelling.Visible = viewsell;
            btnInvoicesBuying.Visible = viewbuy;

            btnPricesList.Visible = _permissions.GetLoggedInUserPermission(PermissionsBusiness.ViewItemsPriceListPermission);

            btnAddProduct.Visible = _permissions.GetLoggedInUserPermission(PermissionsBusiness.CreateItemPermission);
            btnAddSellingInvoice.Visible = _permissions.GetLoggedInUserPermission(PermissionsBusiness.CreateSellInvoicePermission);
            btnAddBuyingInvoice.Visible = _permissions.GetLoggedInUserPermission(PermissionsBusiness.CreateBuyInvoicePermission);
            btnAddItemPrice.Visible = _permissions.GetLoggedInUserPermission(PermissionsBusiness.AddItemPricePermission);

            btnMeasurementUnits.Visible = _permissions.GetLoggedInUserPermission(PermissionsBusiness.CreateMeasurementUnitPermission);
            btnStocks.Visible = _permissions.GetLoggedInUserPermission(PermissionsBusiness.CreateStockPermission);

            lblTools.Visible = btnAddProduct.Visible || btnAddSellingInvoice.Visible || btnAddBuyingInvoice.Visible || btnAddItemPrice.Visible;
            lblOthers.Visible = btnMeasurementUnits.Visible || btnStocks.Visible;
        }

        private void UserUpdated()
        {
            lblStatus.Text = "کاربر جاری : " + Framework.Utilities.LoggedInUser.User.FullName;
        }

        private void AddItemsGridViewForm()
        {
            var frm = _formFactory.CreateForm<FrmItemsGridView>();
            ShowChildForm(frm);
        }
        private void AddInvoiceGridViewForm(FrmInvoicesGridView.InvoiceFormType formtype)
        {
            var frm = _formFactory.CreateForm<FrmInvoicesGridView>();
            frm.SetFormType(formtype);
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
            var frm = _formFactory.CreateForm<FrmInventoryGridView>();
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
            _formFactory.CreateForm<FrmCreateItem>().ShowDialog();
            UpdateItemsRepo();
        }

        private void btnAddBuyingInvoice_Click(object sender, EventArgs e)
        {
            _formFactory.CreateForm<FrmCreateBuyInvoice>().ShowDialog();
            UpdateInvoicesRepo();
        }

        private void btnAddSellingInvoice_Click(object sender, EventArgs e)
        {
            _formFactory.CreateForm<FrmCreateSellInvoice>().ShowDialog();
            UpdateInvoicesRepo();
        }

        private void btnMeasurementUnits_Click(object sender, EventArgs e)
        {
            _formFactory.CreateForm<FrmMeasurementUnits>().ShowDialog();
            UpdateMeasurementUnitsRepo();
        }

        private void pnlForms_Resize(object sender, EventArgs e)
        {
            foreach (var x in _gridViewForms)
                x.CheckDimensions();
        }

        private void btnStocks_Click(object sender, EventArgs e)
        {
            _formFactory.CreateForm<FrmAddStock>().ShowDialog();
            UpdateItemsRepo();
        }

        private void btnPricesList_Click(object sender, EventArgs e)
        {
            var frm = _formFactory.CreateForm<FrmItemsLastPrice>();
            ShowChildForm(frm);
        }

        private void btnAddItemPrice_Click(object sender, EventArgs e)
        {
            _formFactory.CreateForm<FrmAddItemPrice>().ShowDialog();
            UpdateMeasurementUnitsRepo();
        }

        private void btnLogOut_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnUserInfo_Click(object sender, EventArgs e)
        {
            _formFactory.CreateForm<FrmUserInfo>().ShowDialog();
            UserUpdated();
        }

        private void btnPermissions_Click(object sender, EventArgs e)
        {
            _formFactory.CreateForm<FrmPermissions>().ShowDialog();
        }

        private void FrmMain_Load(object sender, EventArgs e)
        {
            SuspendLayout();
            CheckPermissions();
            ResumeLayout();
        }

        private void btnAddUser_Click(object sender, EventArgs e)
        {
            _formFactory.CreateForm<FrmSignUp>().ShowDialog();
        }

        private void FrmMain_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                btnLogOut_Click(null, null);
        }
    }
}
