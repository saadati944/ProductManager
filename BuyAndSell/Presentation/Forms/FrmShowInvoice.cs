using BuyAndSell.DataLayer.Models;
using BuyAndSell.Interfaces;
using Framework.DataLayer;
using Framework.Interfaces;
using Framework.Presentation.Forms;
using System;
using System.Text;
using System.Windows.Forms;

namespace BuyAndSell.Presentation.Forms
{
    class FrmShowInvoice : FrmGridView
    {
        private FrmInvoicesGridView.InvoiceFormType _formType;
        private int _number;

        private Label lblInformation;

        private const string _settingsPrefix = "FrmShowInvoiceGridView";
        private const string _itemColumnName = "Item";
        private const string _quantityColumnName = "Quantity";
        private const string _feeColumnName = "Fee";
        private const string _discountColumnName = "Discount";
        private const string _taxColumnName = "Tax";
        private const string _totalPriceColumnName = "TotalPrice";
        private const string _stockColumnName = "Stock";

        private ISellInvoiceBusiness _sellInvoiceBusiness;
        private IBuyInvoiceBusiness _buyInvoiceBusiness;

        public FrmShowInvoice(IDatabase database, ISettingsBusiness settingsBusiness, IFormFactory formFactory, ISellInvoiceBusiness sellInvoiceBusiness, IBuyInvoiceBusiness buyInvoiceBusiness) :
            base(database, settingsBusiness, formFactory)
        {
            _buyInvoiceBusiness = buyInvoiceBusiness;
            _sellInvoiceBusiness = sellInvoiceBusiness;
        }

        public void SetUpForm(FrmInvoicesGridView.InvoiceFormType formtype, int number)
        {
            _number = number;
            _formType = formtype;

            lblInformation = new Label();
            Controls.Add(lblInformation);

            _columnSettings.Add(new ColumnSelectInfo { SettingsKey = _settingsPrefix + _itemColumnName, DisplayName = "محصول", Checked = true });
            _columnSettings.Add(new ColumnSelectInfo { SettingsKey = _settingsPrefix + _quantityColumnName, DisplayName = "مقدار", Checked = true });
            _columnSettings.Add(new ColumnSelectInfo { SettingsKey = _settingsPrefix + _feeColumnName, DisplayName = "فی", Checked = true });
            _columnSettings.Add(new ColumnSelectInfo { SettingsKey = _settingsPrefix + _discountColumnName, DisplayName = "تخفیف", Checked = true });
            _columnSettings.Add(new ColumnSelectInfo { SettingsKey = _settingsPrefix + _taxColumnName, DisplayName = "مالیات و عوارض", Checked = true });
            _columnSettings.Add(new ColumnSelectInfo { SettingsKey = _settingsPrefix + _totalPriceColumnName, DisplayName = "مبلغ کل", Checked = true });
            _columnSettings.Add(new ColumnSelectInfo { SettingsKey = _settingsPrefix + _stockColumnName, DisplayName = "انبار", Checked = true });
            LoadColumnSettings();

            lblInformation.Dock = DockStyle.Top;
            lblInformation.AutoSize = false;
            lblInformation.Font = new System.Drawing.Font("Tahoma", 12);
            lblInformation.Height = 120;
            lblInformation.SendToBack();
            lblInformation.RightToLeft = RightToLeft.Yes;

            UpdateData();
        }

        public override void UpdateData()
        {
            bool selling = _formType == FrmInvoicesGridView.InvoiceFormType.SellingInvoices;
            dataGridView.Rows.Clear();

            dataGridView.Columns.Clear();
            dataGridView.Columns.Add(_itemColumnName, "محصول");
            dataGridView.Columns[_itemColumnName].Visible = IsVIsibleColumn(_settingsPrefix + _itemColumnName);
            dataGridView.Columns.Add(_quantityColumnName, "مقدار");
            dataGridView.Columns[_quantityColumnName].Visible = IsVIsibleColumn(_settingsPrefix + _quantityColumnName);
            dataGridView.Columns.Add(_feeColumnName, "فی");
            dataGridView.Columns[_feeColumnName].Visible = IsVIsibleColumn(_settingsPrefix + _feeColumnName);
            dataGridView.Columns.Add(_discountColumnName, "تخفیف");
            dataGridView.Columns[_discountColumnName].Visible = IsVIsibleColumn(_settingsPrefix + _discountColumnName);
            dataGridView.Columns.Add(_taxColumnName, "مالیات و عوارض");
            dataGridView.Columns[_taxColumnName].Visible = IsVIsibleColumn(_settingsPrefix + _taxColumnName);
            dataGridView.Columns.Add(_totalPriceColumnName, "مبلغ کل");
            dataGridView.Columns[_totalPriceColumnName].Visible = IsVIsibleColumn(_settingsPrefix + _totalPriceColumnName);
            dataGridView.Columns.Add(_stockColumnName, "انبار");
            dataGridView.Columns[_stockColumnName].Visible = IsVIsibleColumn(_settingsPrefix + _stockColumnName);

            Invoice invoice;
            if (selling)
                invoice = _sellInvoiceBusiness.FullLoadSellInvoice(_number);
            else
                invoice = _buyInvoiceBusiness.FullLoadBuyInvoice(_number);
            foreach (var x in invoice.InvoiceItems)
            {
                if (!x.Included)
                    x.Include();
                dataGridView.Rows.Add(x.Item.Name, x.Quantity, x.Fee, x.Discount, x.Tax, x.Fee * x.Quantity - x.Discount + x.Tax, x.Stock.Name);
            }

            SetTitle(String.Format("فاکتور {0} شماره {1}", selling ? "فروش" : "خرید", invoice.Number));

            StringBuilder information = new StringBuilder();
            information.Append("نوع فاکتور          : ");
            information.Append(selling ? "فروش" : "خرید");
            information.Append("\nشماره فاکتور     : ");
            information.Append(invoice.Number);
            information.Append("\nصادر شده توسط : ");
            information.Append(invoice.User.FullName);
            information.Append("\nتاریخ                 : ");
            information.Append(invoice.GetPersianDate().ToString());
            information.Append("\nقیمت کل           : ");
            information.Append(invoice.TotalPrice);

            lblInformation.Text = information.ToString();
        }
    }
}
