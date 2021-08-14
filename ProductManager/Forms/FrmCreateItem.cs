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
    public partial class FrmCreateItem : Form
    {
        private readonly Business.ItemsBusiness _itemsBusiness;
        private readonly Data.Repositories.MeasurementUnitsRepository _measurementUnitsRepository;
        private Data.Models.Item _item;

        public FrmCreateItem(int itemId = -1)
        {
            InitializeComponent();
            _itemsBusiness = container.Create<Business.ItemsBusiness>();
            _measurementUnitsRepository = container.Create<Data.Repositories.MeasurementUnitsRepository>();
            _measurementUnitsRepository.Update();
            cmbMeasurementUnits.Items.AddRange(_measurementUnitsRepository.MeasurementUnits.ToArray());

            
            if (itemId != -1)
            {
                _item = _itemsBusiness.GetItem(itemId);
                _item.Include();
                for (int i = 0; i < cmbMeasurementUnits.Items.Count; i++)
                    if (((Data.Models.MeasurementUnit)cmbMeasurementUnits.Items[i]).Id == _item.MeasurementUnitRef)
                    {
                        cmbMeasurementUnits.SelectedIndex = i;
                        _item.MeasurementUnit = (Data.Models.MeasurementUnit)cmbMeasurementUnits.Items[i];
                    }
                lblTitle.Text = Text = "تغییر محصول";
                txtCreator.Text = _item.Creator.ToString();
            }
            else
            {
                _item = new Data.Models.Item();
                _item.CreatorRef = Program.LoggedInUser.Id;
                txtCreator.Text = Program.LoggedInUser.ToString();
            }
            bindingSource.DataSource = new Data.Models.Item
            {
                Id = _item.Id,
                Name = _item.Name,
                Description = _item.Description,
                Price = _item.Price,
                MeasurementUnitRef = _item.MeasurementUnitRef,
                CreatorRef = _item.CreatorRef,
                MeasurementUnit = _item.MeasurementUnit,
                Creator = _item.Creator
            };
        }

        private void btnCancele_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!ValidateChildren(ValidationConstraints.Enabled))
                return;
            _item = (Data.Models.Item)bindingSource.DataSource;
            _item.MeasurementUnitRef = _item.MeasurementUnit.Id;
            _itemsBusiness.SaveItem(_item);
            Close();
        }


        private void txtItemName_Validating(object sender, CancelEventArgs e)
        {
            if (txtItemName.Text != _item.Name && (String.IsNullOrEmpty(txtItemName.Text) || _itemsBusiness.IsItemNameExists(txtItemName.Text)))
            {
                e.Cancel = true;
                if(String.IsNullOrEmpty(txtItemName.Text))
                    errorProvider.SetError(txtItemName, "نام کالا اجباری میباشد");
                else
                    errorProvider.SetError(txtItemName, "این نام قبلا ثبت شده است");
            }
            else
            {
                e.Cancel = false;
                errorProvider.SetError(txtItemName, null);
            }
        }

        private void cmbMeasurementUnits_Validating(object sender, CancelEventArgs e)
        {
            if(cmbMeasurementUnits.SelectedIndex == -1)
            {
                e.Cancel = true;
                errorProvider.SetError(cmbMeasurementUnits, "یکی از موارد را انتخاب کنید");
            }
            else
            {
                e.Cancel = false;
                errorProvider.SetError(cmbMeasurementUnits, null);
            }
        }

        private void FrmCreateItem_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                btnCancele_Click(null, null);
        }
    }
}
