using BasicData.DataLayer;
using BasicData.DataLayer.Models;
using BasicData.Interfaces;
using Framework.Interfaces;
using Framework.Presentation.Forms;
using System;
using System.Data;
using System.Windows.Forms;

namespace BasicData.Presentation.Forms
{
    public class FrmItemLastPrices : FrmGridView
    {
        private const string _settingsPrefix = "FrmItemLastPrices";
        private const string _priceColumnName = "Price";
        private const string _dateColumnName = "Date";
        private const string _persianDateColumnName = "PersianDate";

        private Item _item;
        private DataTable _dataTable;
        private readonly IItemsRepository _itemsRepository;

        public FrmItemLastPrices(IDatabase database, IItemsRepository itemsRepository, ISettingsBusiness settings, IFormFactory formFactory) : base(database, settings, formFactory)
        {
            _itemsRepository = itemsRepository;
        }
        public void SetItemId(int id)
        {
            _item = _itemsRepository.GetItemModel(id);
            SetTitle("لیست قیمت های " + _item.Name);

            _columnSettings.Add(new ColumnSelectInfo { SettingsKey = _settingsPrefix + _persianDateColumnName, DisplayName = "تاریخ", Checked = true });
            _columnSettings.Add(new ColumnSelectInfo { SettingsKey = _settingsPrefix + _priceColumnName, DisplayName = "قیمت", Checked = true });
            LoadColumnSettings();
            UpdateData();
        }

        public override void UpdateData()
        {
            dataGridView.DataSource = null;
            dataGridView.Columns.Clear();
            GenerateTable();

            dataGridView.AutoGenerateColumns = false;

            dataGridView.Columns.Add(_persianDateColumnName, "تاریخ");
            dataGridView.Columns[_persianDateColumnName].Visible = IsVIsibleColumn(_settingsPrefix + _persianDateColumnName);
            dataGridView.Columns[_persianDateColumnName].DataPropertyName = _persianDateColumnName;

            dataGridView.Columns.Add(_priceColumnName, "قیمت");
            dataGridView.Columns[_priceColumnName].Visible = IsVIsibleColumn(_settingsPrefix + _priceColumnName);
            dataGridView.Columns[_priceColumnName].DataPropertyName = _priceColumnName;

            dataGridView.DataSource = _dataTable;
        }

        private void GenerateTable()
        {
            _dataTable = _database.GetAllDataset<ItemPrice>(null, null, "ItemRef=" + _item.Id, "Date DESC").Tables[0];

            if (!_dataTable.Columns.Contains(_persianDateColumnName))
                _dataTable.Columns.Add(_persianDateColumnName);

            foreach (DataRow row in _dataTable.Rows)
                row[_persianDateColumnName] = Framework.Utilities.PersianDate.PersianDateStringFromDateTime((DateTime)row[_dateColumnName]);

            DataRow drow = _dataTable.NewRow();
            drow[_persianDateColumnName] = "قیمت پیشفرض";
            drow[_priceColumnName] = _item.Price;
            _dataTable.Rows.Add(drow);
        }
    }
}
