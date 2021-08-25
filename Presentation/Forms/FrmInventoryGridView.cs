using DataLayer;
using DataLayer.Repositories;

namespace Presentation.Forms
{
    public class FrmInventoryGridView : FrmGridView
    {
        private readonly MeasurementUnitsRepository _measurementUnitsRepository;
        private readonly StockSummariesRepository _stockSummariesRepository;

        private const string _settingsPrefix = "FrmInventoryGridView";
        private const string _itemNameColumnName = "Item";
        private const string _stockNameColumnName = "Stock";
        private const string _quantityColumnName = "Quantity";
        private const string _measurementUnitColumnName = "MeasurementUnit";
        public FrmInventoryGridView(Database database, Business.Settings settings, MeasurementUnitsRepository measurementUnitsRepository, StockSummariesRepository stockSummariesRepository) : base(database, settings)
        {
            _stockSummariesRepository = stockSummariesRepository;
            _measurementUnitsRepository = measurementUnitsRepository;
            _measurementUnitsRepository.DataChanged += RepositoriesChanged;


            _measurementUnitsRepository.Update();
            _stockSummariesRepository.DataChanged += RepositoriesChanged;

            dataGridView.AutoGenerateColumns = false;

            _columnSettings.Add(new ColumnSelectInfo { SettingsKey = _settingsPrefix + _itemNameColumnName, DisplayName = "نام محصول", Checked = true });
            _columnSettings.Add(new ColumnSelectInfo { SettingsKey = _settingsPrefix + _stockNameColumnName, DisplayName = "انبار", Checked = true });
            _columnSettings.Add(new ColumnSelectInfo { SettingsKey = _settingsPrefix + _quantityColumnName, DisplayName = "مقدار", Checked = true });
            _columnSettings.Add(new ColumnSelectInfo { SettingsKey = _settingsPrefix + _measurementUnitColumnName, DisplayName = "واحد اندازه گیری", Checked = true });
            LoadColumnSettings();

            _stockSummariesRepository.Update();

            SetTitle("موجودی");
        }

        private void RepositoriesChanged()
        {
            UpdateData();
        }

        public override void UpdateData()
        {
            dataGridView.DataSource = null;

            dataGridView.Columns.Clear();

            dataGridView.Columns.Add(_itemNameColumnName, "نام محصول");
            dataGridView.Columns[_itemNameColumnName].Visible = IsVIsibleColumn(_settingsPrefix + _itemNameColumnName);
            dataGridView.Columns[_itemNameColumnName].DataPropertyName = _itemNameColumnName;

            dataGridView.Columns.Add(_stockNameColumnName, "انبار");
            dataGridView.Columns[_stockNameColumnName].Visible = IsVIsibleColumn(_settingsPrefix + _stockNameColumnName);
            dataGridView.Columns[_stockNameColumnName].DataPropertyName = _stockNameColumnName;

            dataGridView.Columns.Add(_quantityColumnName, "مقدار");
            dataGridView.Columns[_quantityColumnName].Visible = IsVIsibleColumn(_settingsPrefix + _quantityColumnName);
            dataGridView.Columns[_quantityColumnName].DataPropertyName = _quantityColumnName;

            dataGridView.Columns.Add(_measurementUnitColumnName, "واحد");
            dataGridView.Columns[_measurementUnitColumnName].Visible = IsVIsibleColumn(_settingsPrefix + _measurementUnitColumnName);
            dataGridView.Columns[_measurementUnitColumnName].DataPropertyName = _measurementUnitColumnName;

            dataGridView.DataSource = _stockSummariesRepository.DataTable;
        }
    }
}
