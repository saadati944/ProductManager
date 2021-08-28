using System;
using System.Drawing;
using System.Windows.Forms;

namespace BuyAndSell.Presentation.CustomControls
{
    public class NumericUpDownColumn : DataGridViewColumn
    {
        public NumericUpDownColumn()
            : base(new NumericUpDownCell())
        {
        }

        public override DataGridViewCell CellTemplate
        {
            get { return base.CellTemplate; }
            set
            {
                if (value != null && !value.GetType().IsAssignableFrom(typeof(NumericUpDownCell)))
                {
                    throw new InvalidCastException("Must be a NumericUpDownCell");
                }
                base.CellTemplate = value;
            }
        }
    }

    public class NumericUpDownCell : DataGridViewTextBoxCell
    {
        private readonly decimal min;
        private readonly decimal max;

        public NumericUpDownCell()
            : base()
        {
            this.min = 0;
            this.max = 100000000000000000;
            Style.Format = "F4";
        }
        public NumericUpDownCell(decimal min, decimal max)
            : base()
        {
            this.min = min;
            this.max = max;
            Style.Format = "F4";
        }

        public override void InitializeEditingControl(int rowIndex, object initialFormattedValue, DataGridViewCellStyle dataGridViewCellStyle)
        {
            base.InitializeEditingControl(rowIndex, initialFormattedValue, dataGridViewCellStyle);
            NumericUpDownEditingControl ctl = DataGridView.EditingControl as NumericUpDownEditingControl;
            ctl.Minimum = this.min;
            ctl.Maximum = this.max;
            ctl.DecimalPlaces = 4;
            ctl.Value = Convert.ToDecimal(this.Value);
        }

        public override Type EditType
        {
            get { return typeof(NumericUpDownEditingControl); }
        }

        public override Type ValueType
        {
            get { return typeof(Decimal); }
        }

        public override object DefaultNewRowValue
        {
            get { return null; } //未編集の新規行に余計な初期値が出ないようにする
        }
    }

    public class NumericUpDownEditingControl : NumericUpDown, IDataGridViewEditingControl
    {
        private DataGridView dataGridViewControl;
        private bool valueIsChanged = false;
        private int rowIndexNum;

        public NumericUpDownEditingControl()
            : base()
        {
            this.DecimalPlaces = 0;
        }

        public DataGridView EditingControlDataGridView
        {
            get { return dataGridViewControl; }
            set { dataGridViewControl = value; }
        }

        public object EditingControlFormattedValue
        {
            get { return this.Value.ToString("F4"); }
            set { this.Value = Decimal.Parse(value.ToString()); }
        }
        public int EditingControlRowIndex
        {
            get { return rowIndexNum; }
            set { rowIndexNum = value; }
        }
        public bool EditingControlValueChanged
        {
            get { return valueIsChanged; }
            set { valueIsChanged = value; }
        }

        public Cursor EditingPanelCursor
        {
            get { return base.Cursor; }
        }

        public bool RepositionEditingControlOnValueChange
        {
            get { return false; }
        }

        public void ApplyCellStyleToEditingControl(DataGridViewCellStyle dataGridViewCellStyle)
        {
            this.Font = dataGridViewCellStyle.Font;
            this.ForeColor = dataGridViewCellStyle.ForeColor;
            this.BackColor = dataGridViewCellStyle.BackColor;
        }

        public bool EditingControlWantsInputKey(Keys keyData, bool dataGridViewWantsInputKey)
        {
            return (keyData == Keys.Left || keyData == Keys.Right ||
                keyData == Keys.Up || keyData == Keys.Down ||
                keyData == Keys.Home || keyData == Keys.End ||
                keyData == Keys.PageDown || keyData == Keys.PageUp);
        }

        public object GetEditingControlFormattedValue(DataGridViewDataErrorContexts context)
        {
            return this.Value.ToString();
        }

        public void PrepareEditingControlForEdit(bool selectAll)
        {
            this.Select(0, this.Text.Length);
        }

        protected override void OnValueChanged(EventArgs e)
        {
            valueIsChanged = true;
            this.EditingControlDataGridView.NotifyCurrentCellDirty(true);
            base.OnValueChanged(e);
        }
    }
    //public class NumericColumn : DataGridViewColumn
    //{
    //    public NumericColumn() : base(new NumericCell())
    //    {
    //    }

    //    public override DataGridViewCell CellTemplate
    //    {
    //        get
    //        {
    //            return base.CellTemplate;
    //        }
    //        set
    //        {
    //            if (value != null &&
    //                !value.GetType().IsAssignableFrom(typeof(NumericCell)))
    //            {
    //                throw new InvalidCastException("Must be a NumericCell");
    //            }
    //            base.CellTemplate = value;
    //        }
    //    }
    //}

    //public class NumericCell : DataGridViewTextBoxCell
    //{

    //    public NumericCell()
    //        : base()
    //    {
    //    }

    //    public override void InitializeEditingControl(int rowIndex, object
    //        initialFormattedValue, DataGridViewCellStyle dataGridViewCellStyle)
    //    {
    //        base.InitializeEditingControl(rowIndex, initialFormattedValue,
    //            dataGridViewCellStyle);
    //        NumericUpDownEditingControl ctl =
    //            DataGridView.EditingControl as NumericUpDownEditingControl;
    //        if (this.Value == null)
    //        {
    //            ctl.Value = (Decimal)this.DefaultNewRowValue;
    //        }
    //        else
    //        {
    //            ctl.Value = (Decimal)this.Value;
    //        }
    //    }

    //    public override Type EditType
    //    {
    //        get
    //        {
    //            return typeof(NumericUpDownEditingControl);
    //        }
    //    }

    //    public override Type ValueType
    //    {
    //        get
    //        {
    //            return typeof(Decimal);
    //        }
    //    }

    //    public override object DefaultNewRowValue
    //    {
    //        get
    //        {
    //            return Decimal.Zero;
    //        }
    //    }
    //}

    //class NumericUpDownEditingControl : NumericUpDown, IDataGridViewEditingControl
    //{
    //    DataGridView dataGridView;
    //    private bool valueChanged = false;
    //    int rowIndex;

    //    public NumericUpDownEditingControl()
    //    {
    //        this.Increment = 1;
    //        this.Minimum = 0;
    //        this.Maximum = 1000000000000;
    //        this.DecimalPlaces = 4;
    //    }

    //    public object EditingControlFormattedValue
    //    {
    //        get
    //        {
    //            return this.Value;
    //        }
    //        set
    //        {
    //            if (value is Decimal)
    //            {
    //                MessageBox.Show("asdfasdfasdf");
    //                this.Value = (Decimal) value;
    //            }
    //            if (value is String)
    //            {
    //                try
    //                {
    //                    this.Value = Decimal.Parse((String)value);
    //                }
    //                catch
    //                {
    //                    this.Value = 0;
    //                }
    //            }
    //        }
    //    }

    //    public object GetEditingControlFormattedValue(
    //        DataGridViewDataErrorContexts context)
    //    {
    //        return EditingControlFormattedValue;
    //    }

    //    public void ApplyCellStyleToEditingControl(
    //        DataGridViewCellStyle dataGridViewCellStyle)
    //    {
    //        this.Font = dataGridViewCellStyle.Font;
    //        this.ForeColor = dataGridViewCellStyle.ForeColor;
    //        this.BackColor = dataGridViewCellStyle.BackColor;
    //    }

    //    public int EditingControlRowIndex
    //    {
    //        get
    //        {
    //            return rowIndex;
    //        }
    //        set
    //        {
    //            rowIndex = value;
    //        }
    //    }

    //    public bool EditingControlWantsInputKey(
    //        Keys key, bool dataGridViewWantsInputKey)
    //    {
    //        switch (key & Keys.KeyCode)
    //        {
    //            case Keys.Up:
    //            case Keys.Down:
    //            case Keys.Home:
    //            case Keys.End:
    //                return true;
    //            default:
    //                return !dataGridViewWantsInputKey;
    //        }
    //    }

    //    public void PrepareEditingControlForEdit(bool selectAll)
    //    {
    //    }

    //    public bool RepositionEditingControlOnValueChange
    //    {
    //        get
    //        {
    //            return false;
    //        }
    //    }

    //    public DataGridView EditingControlDataGridView
    //    {
    //        get
    //        {
    //            return dataGridView;
    //        }
    //        set
    //        {
    //            dataGridView = value;
    //        }
    //    }

    //    public bool EditingControlValueChanged
    //    {
    //        get
    //        {
    //            return valueChanged;
    //        }
    //        set
    //        {
    //            valueChanged = value;
    //        }
    //    }

    //    public Cursor EditingPanelCursor
    //    {
    //        get
    //        {
    //            return base.Cursor;
    //        }
    //    }

    //    protected override void OnValueChanged(EventArgs eventargs)
    //    {
    //        valueChanged = true;
    //        this.EditingControlDataGridView.NotifyCurrentCellDirty(true);
    //        base.OnValueChanged(eventargs);
    //    }
    //}
}
