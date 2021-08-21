using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tappe.Forms
{
    public class FrmItemsLastPrice : FrmGridView
    {
        private const string _settingsPrefix = "FrmItemsLastPrice";

        private const string _itemIdColumnName = "Id";
        private const string _itemNameColumnName = "Item";
        private const string _dateColumnName = "Date";
        private const string _priceColumnName = "Price";

        private readonly Business.ItemsBusiness _itemsBusiness;
        private readonly Data.Repositories.ItemsRepository _itemsRepository;
        private DataTable _dataTable;
        private readonly FrmMain _frmMain;

        private StructureMap.IContainer container;

        public FrmItemsLastPrice(FrmMain frmMain, StructureMap.IContainer container) : base(container.GetInstance<Data.Database>(), container.GetInstance<Business.Settings>())
        {
            this.container = container;
            _frmMain = frmMain;
            SetTitle("لیست آخرین قیمت محصولات");
            dataGridView.AutoGenerateColumns = false;
            _itemsBusiness = container.GetInstance<Business.ItemsBusiness>();
            _itemsRepository = _itemsBusiness.ItemsRepository;

            
            _itemsRepository.Update();

            _columnSettings.Add(new ColumnSelectInfo { SettingsKey = _settingsPrefix + _itemNameColumnName, DisplayName = "نام محصول" });
            _columnSettings.Add(new ColumnSelectInfo { SettingsKey = _settingsPrefix + _dateColumnName, DisplayName = "تاریخ آخرین قیمت" });
            _columnSettings.Add(new ColumnSelectInfo { SettingsKey = _settingsPrefix + _priceColumnName, DisplayName = "آخرین قیمت" });
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
                dr[_dateColumnName] = Data.Models.PersianDate.PersianDateStringFromDateTime(lastprice.Date == DateTime.MaxValue ? DateTime.Now : lastprice.Date);
                dr[_priceColumnName] = lastprice.Price;
                _dataTable.Rows.Add(dr);
            }
            
        }
    }
}
