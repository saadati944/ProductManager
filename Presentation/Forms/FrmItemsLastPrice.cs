using Business;
using Business.Repositories;
using DataLayer;
using DataLayer.Models;
using System;
using System.Data;

namespace Presentation.Forms
{
    public class FrmItemsLastPrice : FrmGridView
    {
        private const string _settingsPrefix = "FrmItemsLastPrice";

        private const string _itemIdColumnName = "Id";
        private const string _itemNameColumnName = "Item";
        private const string _dateColumnName = "Date";
        private const string _priceColumnName = "Price";

        private readonly ItemsBusiness _itemsBusiness;
        private readonly ItemsRepository _itemsRepository;
        private DataTable _dataTable;
        private readonly FrmMain _frmMain;

        private StructureMap.IContainer container;

        public FrmItemsLastPrice(FrmMain frmMain, StructureMap.IContainer container) : base(container.GetInstance<Database>(), container.GetInstance<Settings>())
        {
            this.container = container;
            _frmMain = frmMain;
            SetTitle("لیست آخرین قیمت محصولات");
            dataGridView.AutoGenerateColumns = false;
            _itemsBusiness = container.GetInstance<ItemsBusiness>();
            _itemsRepository = _itemsBusiness.ItemsRepository;


            _itemsRepository.Update();

            _columnSettings.Add(new ColumnSelectInfo { SettingsKey = _settingsPrefix + _itemNameColumnName, DisplayName = "نام محصول", Checked = true });
            _columnSettings.Add(new ColumnSelectInfo { SettingsKey = _settingsPrefix + _dateColumnName, DisplayName = "تاریخ آخرین قیمت", Checked = true });
            _columnSettings.Add(new ColumnSelectInfo { SettingsKey = _settingsPrefix + _priceColumnName, DisplayName = "آخرین قیمت", Checked = true });
            LoadColumnSettings();

            dataGridView.CellDoubleClick += DataGridView_CellDoubleClick;

            _itemsRepository.DataChanged += UpdateData;
            UpdateData();
        }

        private void DataGridView_CellDoubleClick(object sender, System.Windows.Forms.DataGridViewCellEventArgs e)
        {
            if (e.RowIndex == -1)
                return;
            _frmMain.ShowChildForm(new FrmItemLastPrices(int.Parse((string)dataGridView.Rows[e.RowIndex].Cells[_itemIdColumnName].Value), container));
        }

        public override void UpdateData()
        {
            dataGridView.DataSource = null;
            dataGridView.Columns.Clear();
            GenerateTable();

            dataGridView.Columns.Add(_itemIdColumnName, null);
            dataGridView.Columns[_itemIdColumnName].Visible = false;
            dataGridView.Columns[_itemIdColumnName].DataPropertyName = _itemIdColumnName;

            dataGridView.Columns.Add(_itemNameColumnName, "نام محصول");
            dataGridView.Columns[_itemNameColumnName].Visible = IsVIsibleColumn(_settingsPrefix + _itemNameColumnName);
            dataGridView.Columns[_itemNameColumnName].DataPropertyName = _itemNameColumnName;

            dataGridView.Columns.Add(_dateColumnName, "تاریخ آخرین قیمت");
            dataGridView.Columns[_dateColumnName].Visible = IsVIsibleColumn(_settingsPrefix + _dateColumnName);
            dataGridView.Columns[_dateColumnName].DataPropertyName = _dateColumnName;

            dataGridView.Columns.Add(_priceColumnName, "آخرین قیمت");
            dataGridView.Columns[_priceColumnName].Visible = IsVIsibleColumn(_settingsPrefix + _priceColumnName);
            dataGridView.Columns[_priceColumnName].DataPropertyName = _priceColumnName;

            dataGridView.DataSource = _dataTable;
        }

        private void GenerateTable()
        {
            if (_dataTable == null)
                _dataTable = new DataTable();
            else
                _dataTable.Clear();

            if (!_dataTable.Columns.Contains(_itemIdColumnName))
                _dataTable.Columns.Add(_itemIdColumnName);
            if (!_dataTable.Columns.Contains(_itemNameColumnName))
                _dataTable.Columns.Add(_itemNameColumnName);
            if (!_dataTable.Columns.Contains(_dateColumnName))
                _dataTable.Columns.Add(_dateColumnName);
            if (!_dataTable.Columns.Contains(_priceColumnName))
                _dataTable.Columns.Add(_priceColumnName);

            foreach (var item in _itemsRepository.Items)
            {
                DataRow dr = _dataTable.NewRow();
                dr[_itemNameColumnName] = item;
                dr[_itemIdColumnName] = item.Id;
                var lastprice = _itemsBusiness.GetItemPrice(item.Id, DateTime.MaxValue);
                dr[_dateColumnName] = PersianDate.PersianDateStringFromDateTime(lastprice.Date == DateTime.MaxValue ? DateTime.Now : lastprice.Date);
                dr[_priceColumnName] = lastprice.Price;
                _dataTable.Rows.Add(dr);
            }

        }
    }
}
