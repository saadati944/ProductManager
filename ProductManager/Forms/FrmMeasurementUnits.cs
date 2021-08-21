using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tappe.Forms
{
    public partial class FrmMeasurementUnits : Form
    {
        private readonly Data.Database _database;
        private Data.Models.MeasurementUnit _measurementUnit;

        public FrmMeasurementUnits(Data.Database database)
        {
            InitializeComponent();
            _database = database;
            _measurementUnit = new Data.Models.MeasurementUnit();
            changeMode(false);
            updateList();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            bool inedit = _measurementUnit.Id != -1;
            _measurementUnit.Name = txtName.Text;
            _database.Save(_measurementUnit);
            _measurementUnit = new Data.Models.MeasurementUnit();
            if (inedit)
                btnCancel_Click(null, null);
            updateList();
        }


        private void changeMode(bool editMode)
        {
            if (editMode)
            {
                lblStatus.Text = String.Format("اصلاح '{0}'", _measurementUnit.Name);
                txtName.Text = _measurementUnit.Name;
                btnCancel.Visible = true;
                btnAdd.Text = "ثبت";
                return;
            }
            lblStatus.Text = "افزودن مورد جدید";
            txtName.Text = "";
            btnCancel.Visible = false;
            btnAdd.Text = "افزودن";
        }
        private void updateList()
        {
            lstUnits.Items.Clear();
            lstUnits.Items.AddRange(_database.MeasurementUnits.ToArray());
        }

        private void lstUnits_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (lstUnits.SelectedIndex == -1)
                return;
            _measurementUnit = (Data.Models.MeasurementUnit)lstUnits.SelectedItem;
            changeMode(true);
        }

        private void lstUnits_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (lstUnits.SelectedIndex == -1)
                return;

            if (MessageBox.Show("آیا از حذف این مورد اطمینان دارید ؟", "", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                _database.Delete((Data.Models.MeasurementUnit)lstUnits.SelectedItem);
                _measurementUnit = new Data.Models.MeasurementUnit();
                btnCancel_Click(null, null);
                updateList();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            lstUnits.SelectedIndex = -1;
            lstUnits.SelectedItem = null;
            _measurementUnit = new Data.Models.MeasurementUnit();
            changeMode(false);
        }

        private void FrmMeasurementUnits_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                Close();
        }
    }
}
