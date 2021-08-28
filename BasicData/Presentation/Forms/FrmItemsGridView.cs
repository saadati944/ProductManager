using BasicData.DataLayer;
using BasicData.DataLayer.Models;
using BasicData.Interfaces;
using Framework.Interfaces;
using Framework.Presentation.Forms;
using System;
using System.Windows.Forms;

namespace BasicData.Presentation.Forms
{
    public class FrmItemsGridView : FrmGridView
    {
        private const string _settingsPrefix = "FrmItemsGridView";
        private const string _idColumnName = "Id";
        private const string _nameColumnName = "Name";
        private const string _descColumnName = "Description";
        private const string _measurementUnitColumnName = "MeasurementUnit";
        private const string _priceColumnName = "Price";
        private const string _creatorColumnName = "Creator";

        private const string _title = "محصولات/خدمات";
        private readonly ContextMenuStrip _contextMenu;
        private readonly IItemsRepository _itemsRepository;
        private readonly IPermissionsBusiness _permissions;

        public FrmItemsGridView(IDatabase database, ISettingsBusiness settings, IItemsRepository itemsRepository, IPermissionsBusiness permissions, IFormFactory formFactory) : base(database, settings, formFactory)
        {
            _itemsRepository = itemsRepository;
            _permissions = permissions;
            SetTitle(_title);

            _itemsRepository.Update();

            ContextMenuStrip contextMenu = new ContextMenuStrip();
            if (_permissions.GetLoggedInUserPermission(Framework.Business.PermissionsBusiness.RemoveItemPermission))
                contextMenu.Items.Add(new ToolStripMenuItem("حذف", null, RemoveMenueItem_Click));
            if (_permissions.GetLoggedInUserPermission(Framework.Business.PermissionsBusiness.EditItemPermission))
                contextMenu.Items.Add(new ToolStripMenuItem("تغییر", null, EditMenueItem_Click));
            _contextMenu = contextMenu;

            _itemsRepository.DataChanged += Repository_DataChanged;

            // default settings
            _columnSettings.Add(new ColumnSelectInfo { SettingsKey = _settingsPrefix + _idColumnName, DisplayName = "شماره", Checked = true });
            _columnSettings.Add(new ColumnSelectInfo { SettingsKey = _settingsPrefix + _nameColumnName, DisplayName = "نام", Checked = true });
            _columnSettings.Add(new ColumnSelectInfo { SettingsKey = _settingsPrefix + _descColumnName, DisplayName = "توضیحات", Checked = true });
            _columnSettings.Add(new ColumnSelectInfo { SettingsKey = _settingsPrefix + _measurementUnitColumnName, DisplayName = "واحد اندازه گیری", Checked = true });
            _columnSettings.Add(new ColumnSelectInfo { SettingsKey = _settingsPrefix + _priceColumnName, DisplayName = "قیمت", Checked = true });
            _columnSettings.Add(new ColumnSelectInfo { SettingsKey = _settingsPrefix + _creatorColumnName, DisplayName = "سازنده", Checked = true });
            LoadColumnSettings();

            dataGridView.CellContextMenuStripNeeded += DataGridView_CellContextMenuStripNeeded;
            dataGridView.CellDoubleClick += DataGridView_CellDoubleClick;
            dataGridView.AutoGenerateColumns = false;
            dataGridView.EditMode = DataGridViewEditMode.EditProgrammatically;
            UpdateData();
        }


        private void DataGridView_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1)
            {
                ShowCustomizeWindow();
                return;
            }
            if (e.ColumnIndex == -1)
                return;

            if (!_permissions.GetLoggedInUserPermission(Framework.Business.PermissionsBusiness.EditItemPermission))
                return;

            dataGridView.ClearSelection();
            dataGridView.Rows[e.RowIndex].Selected = true;
            dataGridView.CurrentCell = dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex];
            var frmcreateitem = _formFactory.CreateForm<FrmCreateItem>();
            frmcreateitem.SetItemId((int)dataGridView.CurrentRow.Cells[_idColumnName].Value);
            frmcreateitem.ShowDialog();
            _itemsRepository.Update();
        }

        private void Repository_DataChanged()
        {
            UpdateData();
        }

        private void DataGridView_CellContextMenuStripNeeded(object sender, DataGridViewCellContextMenuStripNeededEventArgs e)
        {
            if (e.RowIndex == -1 || e.ColumnIndex == -1)
                return;

            dataGridView.ClearSelection();
            dataGridView.Rows[e.RowIndex].Selected = true;
            dataGridView.CurrentCell = dataGridView.Rows[e.RowIndex].Cells[e.ColumnIndex];
            e.ContextMenuStrip = _contextMenu;
        }

        public override void UpdateData()
        {
            dataGridView.DataSource = null;
            dataGridView.Columns.Clear();

            dataGridView.Columns.Add(_idColumnName, "شماره");
            dataGridView.Columns[_idColumnName].Visible = IsVIsibleColumn(_settingsPrefix + _idColumnName);
            dataGridView.Columns[_idColumnName].DataPropertyName = _idColumnName;

            dataGridView.Columns.Add(_nameColumnName, "نام");
            dataGridView.Columns[_nameColumnName].Visible = IsVIsibleColumn(_settingsPrefix + _nameColumnName);
            dataGridView.Columns[_nameColumnName].DataPropertyName = _nameColumnName;

            dataGridView.Columns.Add(_descColumnName, "توضیحات");
            dataGridView.Columns[_descColumnName].Visible = IsVIsibleColumn(_settingsPrefix + _descColumnName);
            dataGridView.Columns[_descColumnName].DataPropertyName = _descColumnName;

            dataGridView.Columns.Add(_measurementUnitColumnName, "واحد اندازه گیری");
            dataGridView.Columns[_measurementUnitColumnName].Visible = IsVIsibleColumn(_settingsPrefix + _measurementUnitColumnName);
            dataGridView.Columns[_measurementUnitColumnName].DataPropertyName = _measurementUnitColumnName;

            dataGridView.Columns.Add(_priceColumnName, "قیمت");
            dataGridView.Columns[_priceColumnName].Visible = IsVIsibleColumn(_settingsPrefix + _priceColumnName);
            dataGridView.Columns[_priceColumnName].DataPropertyName = _priceColumnName;

            dataGridView.Columns.Add(_creatorColumnName, "سازنده");
            dataGridView.Columns[_creatorColumnName].Visible = IsVIsibleColumn(_settingsPrefix + _creatorColumnName);
            dataGridView.Columns[_creatorColumnName].DataPropertyName = _creatorColumnName;

            dataGridView.DataSource = _itemsRepository.DataTable;
        }

        private void EditMenueItem_Click(object sender, EventArgs e)
        {
            var frmcreateitem = _formFactory.CreateForm<FrmCreateItem>();
            frmcreateitem.SetItemId((int)dataGridView.CurrentRow.Cells[_idColumnName].Value);
            frmcreateitem.ShowDialog();
            _itemsRepository.Update();
        }

        private void RemoveMenueItem_Click(object sender, EventArgs e)
        {
            if (MessageBox.Show("آیا مطمئن هستید که میخواهید این مورد را حذف کنید ؟", "تایید برای حذف", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
                return;

            var item = new Item { Id = (int)dataGridView.CurrentRow.Cells[_idColumnName].Value };
            _database.Delete(item);

            _itemsRepository.Update();
        }
    }
}
