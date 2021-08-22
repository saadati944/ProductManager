using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DataLayer;
using DataLayer.Models;
using Business.Repositories;

namespace Presentation.Forms
{
    public partial class FrmCreateItem : Form
    {
        private readonly Business.ItemsBusiness _itemsBusiness;
        private readonly MeasurementUnitsRepository _measurementUnitsRepository;
        private DataTable _dataTable;

        public FrmCreateItem(StructureMap.IContainer container, int itemId = -1)
        {
            InitializeComponent();
            _itemsBusiness = container.GetInstance<Business.ItemsBusiness>();
            _measurementUnitsRepository = container.GetInstance<MeasurementUnitsRepository>();
            _measurementUnitsRepository.Update();
            cmbMeasurementUnits.Items.AddRange(_measurementUnitsRepository.MeasurementUnits.ToArray());

            CreateDataTable(itemId);

            var defdata = _dataTable.NewRow();
            defdata.ItemArray = _dataTable.Rows[0].ItemArray;
            _dataTable.Rows.Add(defdata);

            BindDataTable();
            errorProvider.DataSource = _dataTable;
            SetErrorProviderPadding(this, errorProvider, 10);
        }
        private void SetErrorProviderPadding(Control container, ErrorProvider errorProvider, int value, bool children = false)
        {
            foreach (Control x in container.Controls)
            {
                errorProvider.SetIconPadding(x, value);
                if (children)
                    SetErrorProviderPadding(x, errorProvider, value, true);
            }
        }
        private void CreateDataTable(int itemId)
        {
            if (itemId != -1)
            {
                _dataTable = _itemsBusiness.GetItem(itemId);
                DataRow row = _dataTable.Rows[0];
                for (int i = 0; i < cmbMeasurementUnits.Items.Count; i++)
                    if (((MeasurementUnit)cmbMeasurementUnits.Items[i]).Id == (int)row[Item.MeasurementUnitRefColumnName])
                    {
                        cmbMeasurementUnits.SelectedIndex = i;
                        break;
                    }
                lblTitle.Text = Text = "تغییر محصول";
                
            }
            else
            {
                _dataTable = _itemsBusiness.NewTable();
                DataRow row = _dataTable.NewRow();
                row[Item.CreatorRefColumnName] = Database.LoggedInUser.Id;
                row[Item.CreatorColumnName] = Database.LoggedInUser;
                row[Item.NameColumnName] = row[Item.DescriptionColumnName] = "";
                row[Item.PriceColumnName] = 0;
                row[Item.MeasurementUnitRefColumnName] = ((MeasurementUnit)cmbMeasurementUnits.Items[0]).Id;
                cmbMeasurementUnits.SelectedIndex = 0;
                _dataTable.Rows.Add(row);
            }
            txtCreator.Text = _dataTable.Rows[0][Item.CreatorColumnName].ToString();
        }
        
        private void BindDataTable()
        {
            txtItemName.DataBindings.Add("Text", _dataTable, Item.NameColumnName, false, DataSourceUpdateMode.OnPropertyChanged);
            txtDescription.DataBindings.Add("Text", _dataTable, Item.DescriptionColumnName, false, DataSourceUpdateMode.OnPropertyChanged);
            numDefaultPrice.DataBindings.Add("Value", _dataTable, Item.PriceColumnName, false, DataSourceUpdateMode.OnPropertyChanged);
            cmbMeasurementUnits.DataBindings.Add("SelectedItem", _dataTable, Item.MeasurementUnitColumnName, false, DataSourceUpdateMode.OnPropertyChanged);
        }

        private bool ValidateDatatable()
        {
            DataRow row = _dataTable.Rows[0];

            row[Item.MeasurementUnitRefColumnName] = cmbMeasurementUnits.SelectedIndex == -1 ? -1 : ((MeasurementUnit)cmbMeasurementUnits.SelectedItem).Id;

            row.ClearErrors();

            bool result = true;

            if (row[Item.NameColumnName] is DBNull)
            {
                row.SetColumnError(Item.NameColumnName, "نام کالا اجباری میباشد");
                result = false;
            }
            else
            {
                string name = (string)row[Item.NameColumnName];
                if (name.Length == 0 || name != (string)_dataTable.Rows[1][Item.NameColumnName] && _itemsBusiness.IsItemNameExists(name))
                {
                    row.SetColumnError(Item.NameColumnName, "این نام قبلا ثبت شده است");
                    result = false;
                }
            }
            if (row[Item.MeasurementUnitRefColumnName] is DBNull || (int)row[Item.MeasurementUnitRefColumnName] == -1)
            {
                row.SetColumnError(Item.MeasurementUnitColumnName, "یکی از واحد های اندازه گیری را انتخاب نمایید");
                result = false;
            }

            return result;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (!ValidateDatatable())
                return;

            _itemsBusiness.SaveItem(_dataTable);
            Close();
        }

        private void btnCancele_Click(object sender, EventArgs e)
        {
            Close();
        }
        private void FrmCreateItem_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Escape)
                btnCancele_Click(null, null);
        }

    }
}
