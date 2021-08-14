using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Tappe.Data;
using Tappe.Data.Models;

namespace Tappe.Forms
{
    public partial class FrmAddStock : Form
    {
        private readonly Database _database;
        private readonly List<Stock> _stocks;
        private readonly Stock _stock;

        public FrmAddStock()
        {
            InitializeComponent();

            _database = container.Create<Database>();

            _stocks = _database.Stocks.ToList();
            lstStocks.Items.AddRange(_stocks.ToArray());

            _stock = new Stock();
            txtName.DataBindings.Add("Text", _stock, "Name");
        }

        private void txtName_Validating(object sender, CancelEventArgs e)
        {
            foreach (Stock x in _stocks)
                if (x.Name == txtName.Text)
                {
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
