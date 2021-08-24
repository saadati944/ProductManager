using DataLayer;
using DataLayer.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;

namespace Presentation.Forms
{
    public partial class FrmAddStock : Form
    {
        private readonly Database _database;
        private readonly List<Stock> _stocks;
        private readonly Stock _stock;

        public FrmAddStock(Database database)
        {
            InitializeComponent();

            _database = database;

            _stocks = _database.Stocks.ToList();
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

            _stock.Save();
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
