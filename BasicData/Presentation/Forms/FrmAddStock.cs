using BasicData.DataLayer;
using BasicData.DataLayer.Models;
using BasicData.Interfaces;
using Framework.Interfaces;
using Framework.Presentation.Forms;
using System.Collections.Generic;
using System;
using System.Data;
using System.Linq;
using System.Windows.Forms;
using System.ComponentModel;

namespace BasicData.Presentation.Forms
{
    public partial class FrmAddStock : Form
    {
        private readonly IDatabase _database;
        private readonly List<Stock> _stocks;
        private readonly Stock _stock;

        public FrmAddStock(IDatabase database)
        {
            InitializeComponent();

            _database = database;

            _stocks = _database.GetAll<Stock>().ToList();
            lstStocks.Items.AddRange(_stocks.ToArray());

            _stock = new Stock();
            txtName.DataBindings.Add("Text", _stock, "Name");
        }

        private void txtName_Validating(object sender, CancelEventArgs e)
        {
            if (String.IsNullOrEmpty(txtName.Text))
            {
                errorProvider.SetIconPadding(txtName, 10);
                errorProvider.SetError(txtName, "نامی را انتخاب کنید");
                e.Cancel = true;
                return;
            }
            else
                foreach (Stock x in _stocks)
                    if (x.Name == txtName.Text)
                    {
                        errorProvider.SetIconPadding(txtName, 10);
                        errorProvider.SetError(txtName, "این نام قبلا ثبت شده است");
                        e.Cancel = true;
                        return;
                    }

            errorProvider.SetError(txtName, null);
            e.Cancel = false;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            errorProvider.Clear();
            if (!ValidateChildren())
                return;

            _database.Save(_stock);
            var copy = new Stock { Name = _stock.Name };

            _stocks.Add(copy);
            lstStocks.Items.Add(copy);

            txtName.Text = "";
        }

        private void FrmAddStock_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                btnClose_Click(null, null);
        }
    }
}
