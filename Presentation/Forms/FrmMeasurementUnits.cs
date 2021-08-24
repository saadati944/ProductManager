using DataLayer;
using DataLayer.Models;
using System;
using System.Linq;
using System.Windows.Forms;

namespace Presentation.Forms
{
    public partial class FrmMeasurementUnits : Form
    {
        private readonly Database _database;
        private MeasurementUnit _measurementUnit;

        public FrmMeasurementUnits(Database database)
        {
            InitializeComponent();
            _database = database;
            _measurementUnit = new MeasurementUnit();
            changeMode(false);
            updateList();
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            //TODO: input validation
            bool inedit = _measurementUnit.Id != -1;
            _measurementUnit.Name = txtName.Text;
            _database.Save(_measurementUnit);
            _measurementUnit = new MeasurementUnit();
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
            _measurementUnit = (MeasurementUnit)lstUnits.SelectedItem;
            changeMode(true);
        }

        private void lstUnits_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (lstUnits.SelectedIndex == -1)
                return;

            if (MessageBox.Show("آیا از حذف این مورد اطمینان دارید ؟", "", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                _database.Delete((MeasurementUnit)lstUnits.SelectedItem);
                _measurementUnit = new MeasurementUnit();
                btnCancel_Click(null, null);
                updateList();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            lstUnits.SelectedIndex = -1;
            lstUnits.SelectedItem = null;
            _measurementUnit = new MeasurementUnit();
            changeMode(false);
        }

        private void FrmMeasurementUnits_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                Close();
        }
    }
}
