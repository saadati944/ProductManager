using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Drawing;

namespace Tappe.Forms
{
    public class FrmInvoicesGridView : FrmGridView
    {
        public enum InvoiceFormType
        {
            BuyingInvoices,
            SellingInvoices,
            All
        }
        private readonly bool _showSells = false;
        private readonly bool _showBuys = false;

        private readonly FrmMain _frmMain;

        private readonly Data.Repositories.BuyInvoicesRepository _buyInvoicesRepository;
        private readonly Data.Repositories.SellInvoicesRepository _sellInvoicesRepository;

        private readonly Business.BuyInvoiceBusiness _buyInvoiceBusiness;
        private readonly Business.SellInvoiceBusiness _sellInvoiceBusiness;

        private readonly Business.Permissions _permissions;
        private readonly ContextMenuStrip _contextMenu;

        //private readonly Color _sellInvoiceColor = Color.FromArgb(213, 255, 204);
        //private readonly Color _buyInvoiceColor = Color.FromArgb(255, 212, 204);
        private const string _sellInvoiceColumnText = "فروش";
        private const string _buyInvoiceColumnText = "خرید";

        private const string _settingsPrefix = "FrmInvoicesGridView";
        private const string _typeColumnName = "Type";
        private const string _numberColumnName = "Number";
        //private const string _dateColumnName = "Date";
        private const string _persianDateColumnName = "PersianDate";
        private const string _TotalPriceColumnName = "TotalPrice";
        private const string _stockColumnName = "Stock";

        public FrmInvoicesGridView(InvoiceFormType formType, FrmMain frmMain) : base()
        {
            _frmMain = frmMain;

            _buyInvoicesRepository = container.Create<Data.Repositories.BuyInvoicesRepository>();
            _sellInvoicesRepository = container.Create<Data.Repositories.SellInvoicesRepository>();

            _buyInvoiceBusiness = container.Create<Business.BuyInvoiceBusiness>();
            _sellInvoiceBusiness = container.Create<Business.SellInvoiceBusiness>();

            _permissions = container.Create<Business.Permissions>();

            if (formType == InvoiceFormType.All || formType == InvoiceFormType.SellingInvoices)
            {
                _showSells = true;
                _sellInvoicesRepository.Update();
            }
            if (formType == InvoiceFormType.All || formType == InvoiceFormType.BuyingInvoices)
            {
                _showBuys = true;
                _buyInvoicesRepository.Update();
            }

            ContextMenuStrip contextMenu = new ContextMenuStrip();
            contextMenu.Items.Add(new ToolStripMenuItem("حذف", null, RemoveMenueItem_Click));
            contextMenu.Items.Add(new ToolStripMenuItem("ویرایش", null, EditMenueItem_Click));
            _contextMenu = contextMenu;

            _buyInvoicesRepository.DataChanged += Repositories_DataChanged;
            _sellInvoicesRepository.DataChanged += Repositories_DataChanged;

            _columnSettings.Add(new ColumnSelectInfo { SettingsKey = _settingsPrefix + _typeColumnName, DisplayName = "نوع فاکتور", Checked = true });
            _columnSettings.Add(new ColumnSelectInfo { SettingsKey = _settingsPrefix + _numberColumnName, DisplayName = "شماره فاکتور", Checked = true });
            _columnSettings.Add(new ColumnSelectInfo { SettingsKey = _settingsPrefix + _persianDateColumnName, DisplayName = "تاریخ", Checked = true });
            _columnSettings.Add(new ColumnSelectInfo { SettingsKey = _settingsPrefix + _TotalPriceColumnName, DisplayName = "مبلغ کل", Checked = true });
            _columnSettings.Add(new ColumnSelectInfo { SettingsKey = _settingsPrefix + _stockColumnName, DisplayName = "انبار", Checked = true });

            LoadColumnSettings();


            dataGridView.AutoGenerateColumns = false;
            dataGridView.CellContextMenuStripNeeded += DataGridView_CellContextMenuStripNeeded;
            dataGridView.CellDoubleClick += DataGridView_CellDoubleClick;
            switch (formType)
            {
                case InvoiceFormType.BuyingInvoices:
                    Text = "فاکتور های خرید";
                    break;
                case InvoiceFormType.SellingInvoices:
                    Text = "فاکتور های فروش";
                    break;
                case InvoiceFormType.All:
                    Text = "فاکتور های خرید و فروش";
                    break;
            }
            UpdateData();
        }
        private void RemoveMenueItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("آیا مطمئن هستید که میخواهید این مورد را حذف کنید ؟", "تایید برای حذف", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                return;

            var invoicetype = (string)dataGridView.CurrentRow.Cells[_typeColumnName].Value == _sellInvoiceColumnText ? InvoiceFormType.SellingInvoices : InvoiceFormType.BuyingInvoices;
            var invoicenumber = (int)dataGridView.CurrentRow.Cells[_numberColumnName].Value;

            var res = invoicetype == InvoiceFormType.BuyingInvoices ? _buyInvoiceBusiness.RemoveInvoice(invoicenumber) : _sellInvoiceBusiness.RemoveInvoice(invoicenumber);

            if (!res)
            {
                MessageBox.Show("در هنگام حذف فاکتور خطایی رخ داده است");
                return;
            }

            if (invoicetype == InvoiceFormType.BuyingInvoices)
                _buyInvoicesRepository.Update();
            else
                _sellInvoicesRepository.Update();
        }

        private void EditMenueItem_Click(object sender, EventArgs e)
        {
            var invoicetype = (string)dataGridView.CurrentRow.Cells[_typeColumnName].Value == _sellInvoiceColumnText ? InvoiceFormType.SellingInvoices : InvoiceFormType.BuyingInvoices;
            var invoicenumber = (int)dataGridView.CurrentRow.Cells[_numberColumnName].Value;
            
            if(invoicetype == InvoiceFormType.BuyingInvoices)
            {
                if (_buyInvoiceBusiness.LockInvoiceNumber(invoicenumber))
                {
                    new FrmCreateBuyInvoice(invoicenumber).ShowDialog();
                    _buyInvoiceBusiness.UnlockInvoiceNumber(invoicenumber);
                    _buyInvoicesRepository.Update();
                }
                else
                    MessageBox.Show("امکان ویرایش فاکتور وجود ندارد");

            }
            else
            {
                if (_sellInvoiceBusiness.LockInvoiceNumber(invoicenumber))
                {
                    new FrmCreateSellInvoice(invoicenumber).ShowDialog();
                    _sellInvoiceBusiness.UnlockInvoiceNumber(invoicenumber);
                    _sellInvoicesRepository.Update();
                }
                else
                    MessageBox.Show("امکان ویرایش فاکتور وجود ندارد");
            }
        }

        private void Repositories_DataChanged()
        {
            UpdateData();
        }

        private void DataGridView_CellContextMenuStripNeeded(object sender, System.Windows.Forms.DataGridViewCellContextMenuStripNeededEventArgs e)
        {
            if (e.RowIndex == -1)
            {
                ShowCustomizeWindow();
                return;
            }
            
            // TODO : create a permission for removing invoices
            //if (e.ColumnIndex == -1 || !_permissions.GetLoggedInUserPermission(Business.Permissions.))
            //    return;

            dataGridView.ClearSelection();
            dataGridView.Rows[e.RowIndex].Selected = true;
            dataGridView.CurrentCell = dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex];
            e.ContextMenuStrip = _contextMenu;
        }
        private void DataGridView_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1)
                return;
            dataGridView.Rows[e.RowIndex].Selected = true;
            InvoiceFormType formtype = (string)dataGridView.Rows[e.RowIndex].Cells[_typeColumnName].Value == _sellInvoiceColumnText ? InvoiceFormType.SellingInvoices : InvoiceFormType.BuyingInvoices;
            var frm = new FrmShowInvoice(formtype, (int)dataGridView.Rows[e.RowIndex].Cells[_numberColumnName].Value);

            _frmMain.ShowChildForm(frm);
        }

        public override void UpdateData()
        {
            dataGridView.DataSource = null;

            dataGridView.Columns.Clear();
            dataGridView.Columns.Add(_typeColumnName, "نوع فاکتور");
            dataGridView.Columns[_typeColumnName].Visible = IsVIsibleColumn(_settingsPrefix + _typeColumnName);
            dataGridView.Columns[_typeColumnName].DataPropertyName = _typeColumnName;

            dataGridView.Columns.Add(_numberColumnName, "شماره فاکتور");
            dataGridView.Columns[_numberColumnName].Visible = IsVIsibleColumn(_settingsPrefix + _numberColumnName);
            dataGridView.Columns[_numberColumnName].DataPropertyName = _numberColumnName;

            dataGridView.Columns.Add(_persianDateColumnName, "تاریخ");
            dataGridView.Columns[_persianDateColumnName].Visible = IsVIsibleColumn(_settingsPrefix + _persianDateColumnName);
            dataGridView.Columns[_persianDateColumnName].DataPropertyName = _persianDateColumnName;

            dataGridView.Columns.Add(_TotalPriceColumnName, "مبلغ کل");
            dataGridView.Columns[_TotalPriceColumnName].Visible = IsVIsibleColumn(_settingsPrefix + _TotalPriceColumnName);
            dataGridView.Columns[_TotalPriceColumnName].DataPropertyName = _TotalPriceColumnName;

            dataGridView.Columns.Add(_stockColumnName, "انبار");
            dataGridView.Columns[_stockColumnName].Visible = IsVIsibleColumn(_settingsPrefix + _stockColumnName);
            dataGridView.Columns[_stockColumnName].DataPropertyName = _stockColumnName;

            dataGridView.DataSource = CreateTable();
        }
        private DataTable CreateTable()
        {
            var table = _sellInvoicesRepository.DataTable.Clone();
            if (_showSells)
                foreach (DataRow x in _sellInvoicesRepository.DataTable.Rows)
                    table.Rows.Add(x.ItemArray);
            if (_showBuys)
                foreach (DataRow x in _buyInvoicesRepository.DataTable.Rows)
                    table.Rows.Add(x.ItemArray);
            return table;
        }
    }
}
