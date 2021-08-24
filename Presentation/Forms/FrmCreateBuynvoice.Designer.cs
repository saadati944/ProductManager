namespace Presentation.Forms
{
    partial class FrmCreateBuyInvoice
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.panel2 = new System.Windows.Forms.Panel();
            this.btnCancele = new System.Windows.Forms.Button();
            this.btnSave = new System.Windows.Forms.Button();
            this.lblTotalPrice = new System.Windows.Forms.Label();
            this.lblTotalPriceLable = new System.Windows.Forms.Label();
            this.pnlControls = new System.Windows.Forms.Panel();
            this.radCustomeNumber = new System.Windows.Forms.RadioButton();
            this.radAutoNumber = new System.Windows.Forms.RadioButton();
            this.txtDate = new System.Windows.Forms.MaskedTextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.numInvoiceNumber = new System.Windows.Forms.NumericUpDown();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.txtBuyer = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.cmbParties = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.lblTitle = new System.Windows.Forms.Label();
            this.itemsGridView = new System.Windows.Forms.DataGridView();
            this.Id = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.StockName = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.ItemName = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.Delete = new System.Windows.Forms.DataGridViewButtonColumn();
            this.dataGridViewTextBoxColumn1 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataGridViewTextBoxColumn2 = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.errorProviderHeader = new System.Windows.Forms.ErrorProvider(this.components);
            this.errorProviderItems = new System.Windows.Forms.ErrorProvider(this.components);
            this.panel2.SuspendLayout();
            this.pnlControls.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numInvoiceNumber)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.itemsGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProviderHeader)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProviderItems)).BeginInit();
            this.SuspendLayout();
            // 
            // panel2
            // 
            this.panel2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(192)))), ((int)(((byte)(255)))), ((int)(((byte)(192)))));
            this.panel2.Controls.Add(this.btnCancele);
            this.panel2.Controls.Add(this.btnSave);
            this.panel2.Controls.Add(this.lblTotalPrice);
            this.panel2.Controls.Add(this.lblTotalPriceLable);
            this.panel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel2.Location = new System.Drawing.Point(0, 496);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(867, 53);
            this.panel2.TabIndex = 7;
            // 
            // btnCancele
            // 
            this.btnCancele.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.btnCancele.FlatAppearance.BorderSize = 0;
            this.btnCancele.FlatAppearance.MouseDownBackColor = System.Drawing.Color.MediumSeaGreen;
            this.btnCancele.FlatAppearance.MouseOverBackColor = System.Drawing.Color.MediumSeaGreen;
            this.btnCancele.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnCancele.Location = new System.Drawing.Point(24, 15);
            this.btnCancele.Name = "btnCancele";
            this.btnCancele.Size = new System.Drawing.Size(200, 23);
            this.btnCancele.TabIndex = 1;
            this.btnCancele.Text = "انصراف";
            this.btnCancele.UseVisualStyleBackColor = false;
            this.btnCancele.Click += new System.EventHandler(this.btnCancele_Click);
            // 
            // btnSave
            // 
            this.btnSave.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.btnSave.FlatAppearance.BorderSize = 0;
            this.btnSave.FlatAppearance.MouseDownBackColor = System.Drawing.Color.MediumSeaGreen;
            this.btnSave.FlatAppearance.MouseOverBackColor = System.Drawing.Color.MediumSeaGreen;
            this.btnSave.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btnSave.Location = new System.Drawing.Point(230, 15);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(200, 23);
            this.btnSave.TabIndex = 0;
            this.btnSave.Text = "ثبت نهایی";
            this.btnSave.UseVisualStyleBackColor = false;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // lblTotalPrice
            // 
            this.lblTotalPrice.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblTotalPrice.Location = new System.Drawing.Point(529, 20);
            this.lblTotalPrice.Name = "lblTotalPrice";
            this.lblTotalPrice.Size = new System.Drawing.Size(182, 18);
            this.lblTotalPrice.TabIndex = 5;
            this.lblTotalPrice.Text = "0";
            // 
            // lblTotalPriceLable
            // 
            this.lblTotalPriceLable.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblTotalPriceLable.AutoSize = true;
            this.lblTotalPriceLable.Location = new System.Drawing.Point(751, 20);
            this.lblTotalPriceLable.Name = "lblTotalPriceLable";
            this.lblTotalPriceLable.Size = new System.Drawing.Size(104, 13);
            this.lblTotalPriceLable.TabIndex = 4;
            this.lblTotalPriceLable.Text = "مجموع قیمت فاکتور :";
            // 
            // pnlControls
            // 
            this.pnlControls.Controls.Add(this.radCustomeNumber);
            this.pnlControls.Controls.Add(this.radAutoNumber);
            this.pnlControls.Controls.Add(this.txtDate);
            this.pnlControls.Controls.Add(this.label7);
            this.pnlControls.Controls.Add(this.numInvoiceNumber);
            this.pnlControls.Controls.Add(this.textBox2);
            this.pnlControls.Controls.Add(this.label6);
            this.pnlControls.Controls.Add(this.label5);
            this.pnlControls.Controls.Add(this.txtBuyer);
            this.pnlControls.Controls.Add(this.label4);
            this.pnlControls.Controls.Add(this.cmbParties);
            this.pnlControls.Controls.Add(this.label2);
            this.pnlControls.Controls.Add(this.lblTitle);
            this.pnlControls.Dock = System.Windows.Forms.DockStyle.Top;
            this.pnlControls.Location = new System.Drawing.Point(0, 0);
            this.pnlControls.Name = "pnlControls";
            this.pnlControls.Size = new System.Drawing.Size(867, 126);
            this.pnlControls.TabIndex = 8;
            // 
            // radCustomeNumber
            // 
            this.radCustomeNumber.AutoSize = true;
            this.radCustomeNumber.Location = new System.Drawing.Point(191, 53);
            this.radCustomeNumber.Name = "radCustomeNumber";
            this.radCustomeNumber.Size = new System.Drawing.Size(14, 13);
            this.radCustomeNumber.TabIndex = 12;
            this.radCustomeNumber.TabStop = true;
            this.radCustomeNumber.UseVisualStyleBackColor = true;
            this.radCustomeNumber.CheckedChanged += new System.EventHandler(this.radCustomeNumber_CheckedChanged);
            // 
            // radAutoNumber
            // 
            this.radAutoNumber.AutoSize = true;
            this.radAutoNumber.Location = new System.Drawing.Point(151, 31);
            this.radAutoNumber.Name = "radAutoNumber";
            this.radAutoNumber.Size = new System.Drawing.Size(54, 17);
            this.radAutoNumber.TabIndex = 11;
            this.radAutoNumber.TabStop = true;
            this.radAutoNumber.Text = "خودکار";
            this.radAutoNumber.UseVisualStyleBackColor = true;
            // 
            // txtDate
            // 
            this.txtDate.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtDate.Location = new System.Drawing.Point(35, 95);
            this.txtDate.Mask = "0000/00/00";
            this.txtDate.Name = "txtDate";
            this.txtDate.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.txtDate.Size = new System.Drawing.Size(177, 14);
            this.txtDate.TabIndex = 2;
            this.txtDate.Text = "00000000";
            this.txtDate.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.txtDate.Leave += new System.EventHandler(this.txtDate_Leave);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(226, 53);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(73, 13);
            this.label7.TabIndex = 10;
            this.label7.Text = "شماره فاکتور :";
            // 
            // numInvoiceNumber
            // 
            this.numInvoiceNumber.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.numInvoiceNumber.Location = new System.Drawing.Point(35, 54);
            this.numInvoiceNumber.Maximum = new decimal(new int[] {
            1000000000,
            0,
            0,
            0});
            this.numInvoiceNumber.Name = "numInvoiceNumber";
            this.numInvoiceNumber.Size = new System.Drawing.Size(153, 17);
            this.numInvoiceNumber.TabIndex = 4;
            this.numInvoiceNumber.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // textBox2
            // 
            this.textBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox2.Location = new System.Drawing.Point(12, 129);
            this.textBox2.MaxLength = 300;
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(762, 21);
            this.textBox2.TabIndex = 8;
            this.textBox2.Visible = false;
            // 
            // label6
            // 
            this.label6.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(780, 132);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(39, 13);
            this.label6.TabIndex = 7;
            this.label6.Text = "آدرس :";
            this.label6.Visible = false;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(264, 95);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(35, 13);
            this.label5.TabIndex = 6;
            this.label5.Text = "تاریخ :";
            // 
            // txtBuyer
            // 
            this.txtBuyer.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.txtBuyer.Enabled = false;
            this.txtBuyer.Location = new System.Drawing.Point(328, 95);
            this.txtBuyer.Name = "txtBuyer";
            this.txtBuyer.Size = new System.Drawing.Size(172, 14);
            this.txtBuyer.TabIndex = 1;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(506, 95);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(41, 13);
            this.label4.TabIndex = 3;
            this.label4.Text = "توسط :";
            // 
            // cmbParties
            // 
            this.cmbParties.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cmbParties.AutoCompleteMode = System.Windows.Forms.AutoCompleteMode.SuggestAppend;
            this.cmbParties.AutoCompleteSource = System.Windows.Forms.AutoCompleteSource.ListItems;
            this.cmbParties.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.cmbParties.FormattingEnabled = true;
            this.cmbParties.Location = new System.Drawing.Point(576, 92);
            this.cmbParties.Name = "cmbParties";
            this.cmbParties.Size = new System.Drawing.Size(198, 21);
            this.cmbParties.TabIndex = 0;
            this.cmbParties.Validating += new System.ComponentModel.CancelEventHandler(this.cmbParties_Validating);
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(780, 95);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(58, 13);
            this.label2.TabIndex = 1;
            this.label2.Text = "تامین کننده";
            // 
            // lblTitle
            // 
            this.lblTitle.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.lblTitle.Font = new System.Drawing.Font("Tahoma", 20F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(178)));
            this.lblTitle.Location = new System.Drawing.Point(553, 9);
            this.lblTitle.Name = "lblTitle";
            this.lblTitle.Size = new System.Drawing.Size(291, 33);
            this.lblTitle.TabIndex = 0;
            this.lblTitle.Text = "فاکتور خرید";
            // 
            // itemsGridView
            // 
            this.itemsGridView.AllowUserToResizeRows = false;
            this.itemsGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.itemsGridView.BackgroundColor = System.Drawing.Color.White;
            this.itemsGridView.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.itemsGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.Id,
            this.StockName,
            this.ItemName,
            this.Delete});
            this.itemsGridView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.itemsGridView.Location = new System.Drawing.Point(0, 126);
            this.itemsGridView.Name = "itemsGridView";
            this.itemsGridView.Size = new System.Drawing.Size(867, 370);
            this.itemsGridView.TabIndex = 0;
            this.itemsGridView.DataError += new System.Windows.Forms.DataGridViewDataErrorEventHandler(this.itemsGridView_DataError);
            this.itemsGridView.Validating += new System.ComponentModel.CancelEventHandler(this.itemsGridView_Validating);
            // 
            // Id
            // 
            this.Id.HeaderText = "Id";
            this.Id.Name = "Id";
            this.Id.ReadOnly = true;
            this.Id.Visible = false;
            // 
            // StockName
            // 
            this.StockName.FillWeight = 150F;
            this.StockName.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.StockName.HeaderText = "انبار";
            this.StockName.Name = "StockName";
            // 
            // ItemName
            // 
            this.ItemName.FillWeight = 200F;
            this.ItemName.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.ItemName.HeaderText = "نام محصول";
            this.ItemName.Name = "ItemName";
            // 
            // Delete
            // 
            this.Delete.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.ColumnHeader;
            this.Delete.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Delete.HeaderText = "عملیات";
            this.Delete.Name = "Delete";
            this.Delete.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.Delete.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            this.Delete.Width = 66;
            // 
            // dataGridViewTextBoxColumn1
            // 
            this.dataGridViewTextBoxColumn1.HeaderText = "Id";
            this.dataGridViewTextBoxColumn1.Name = "dataGridViewTextBoxColumn1";
            this.dataGridViewTextBoxColumn1.ReadOnly = true;
            this.dataGridViewTextBoxColumn1.Visible = false;
            // 
            // dataGridViewTextBoxColumn2
            // 
            this.dataGridViewTextBoxColumn2.FillWeight = 200F;
            this.dataGridViewTextBoxColumn2.HeaderText = "نام محصول";
            this.dataGridViewTextBoxColumn2.Name = "dataGridViewTextBoxColumn2";
            this.dataGridViewTextBoxColumn2.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.dataGridViewTextBoxColumn2.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.NotSortable;
            this.dataGridViewTextBoxColumn2.Width = 761;
            // 
            // errorProviderHeader
            // 
            this.errorProviderHeader.ContainerControl = this;
            this.errorProviderHeader.RightToLeft = true;
            // 
            // errorProviderItems
            // 
            this.errorProviderItems.ContainerControl = this;
            this.errorProviderItems.RightToLeft = true;
            // 
            // FrmCreateBuyInvoice
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoValidate = System.Windows.Forms.AutoValidate.Disable;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(220)))), ((int)(((byte)(255)))), ((int)(((byte)(220)))));
            this.ClientSize = new System.Drawing.Size(867, 549);
            this.Controls.Add(this.itemsGridView);
            this.Controls.Add(this.pnlControls);
            this.Controls.Add(this.panel2);
            this.Font = new System.Drawing.Font("Tahoma", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(178)));
            this.KeyPreview = true;
            this.MinimumSize = new System.Drawing.Size(883, 588);
            this.Name = "FrmCreateBuyInvoice";
            this.RightToLeft = System.Windows.Forms.RightToLeft.Yes;
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "فاکتور خرید";
            this.Load += new System.EventHandler(this.FrmCreateBuyInvoice_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.FrmCreateBuyInvoice_KeyDown);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.pnlControls.ResumeLayout(false);
            this.pnlControls.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.numInvoiceNumber)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.itemsGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProviderHeader)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.errorProviderItems)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Button btnCancele;
        private System.Windows.Forms.Button btnSave;
        private System.Windows.Forms.Label lblTotalPrice;
        private System.Windows.Forms.Label lblTotalPriceLable;
        private System.Windows.Forms.Panel pnlControls;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.ComboBox cmbParties;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label lblTitle;
        private System.Windows.Forms.TextBox txtBuyer;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.NumericUpDown numInvoiceNumber;
        private System.Windows.Forms.MaskedTextBox txtDate;
        private System.Windows.Forms.DataGridView itemsGridView;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn1;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataGridViewTextBoxColumn2;
        private System.Windows.Forms.ErrorProvider errorProviderHeader;
        private System.Windows.Forms.ErrorProvider errorProviderItems;
        private System.Windows.Forms.DataGridViewTextBoxColumn Id;
        private System.Windows.Forms.DataGridViewComboBoxColumn StockName;
        private System.Windows.Forms.DataGridViewComboBoxColumn ItemName;
        private System.Windows.Forms.DataGridViewButtonColumn Delete;
        private System.Windows.Forms.RadioButton radCustomeNumber;
        private System.Windows.Forms.RadioButton radAutoNumber;
    }
}