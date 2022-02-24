namespace ALARm.controls
{
    partial class rdVideoObjectsControl
    {
        /// <summary> 
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором компонентов

        /// <summary> 
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.dataObjects = new System.Windows.Forms.DataGridView();
            this.idDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.oidDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.fnumDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.kmDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.ptDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.mtrDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.xDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.yDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.wDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.hDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.prbDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.msDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.fileidDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.bsRdVideoObjects = new System.Windows.Forms.BindingSource(this.components);
            this.listObjects = new System.Windows.Forms.CheckedListBox();
            this.checkObjects = new MetroFramework.Controls.MetroCheckBox();
            this.checkKm = new MetroFramework.Controls.MetroCheckBox();
            this.startKm = new MetroFramework.Controls.MetroTextBox();
            this.finalKm = new MetroFramework.Controls.MetroTextBox();
            this.metroLabel1 = new MetroFramework.Controls.MetroLabel();
            this.metroLabel2 = new MetroFramework.Controls.MetroLabel();
            ((System.ComponentModel.ISupportInitialize)(this.dataObjects)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bsRdVideoObjects)).BeginInit();
            this.SuspendLayout();
            // 
            // dataObjects
            // 
            this.dataObjects.AllowUserToAddRows = false;
            this.dataObjects.AllowUserToDeleteRows = false;
            this.dataObjects.AutoGenerateColumns = false;
            this.dataObjects.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            this.dataObjects.BackgroundColor = System.Drawing.SystemColors.ButtonHighlight;
            this.dataObjects.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataObjects.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.idDataGridViewTextBoxColumn,
            this.oidDataGridViewTextBoxColumn,
            this.fnumDataGridViewTextBoxColumn,
            this.kmDataGridViewTextBoxColumn,
            this.ptDataGridViewTextBoxColumn,
            this.mtrDataGridViewTextBoxColumn,
            this.xDataGridViewTextBoxColumn,
            this.yDataGridViewTextBoxColumn,
            this.wDataGridViewTextBoxColumn,
            this.hDataGridViewTextBoxColumn,
            this.prbDataGridViewTextBoxColumn,
            this.msDataGridViewTextBoxColumn,
            this.fileidDataGridViewTextBoxColumn});
            this.dataObjects.DataSource = this.bsRdVideoObjects;
            this.dataObjects.Location = new System.Drawing.Point(3, 26);
            this.dataObjects.MultiSelect = false;
            this.dataObjects.Name = "dataObjects";
            this.dataObjects.ReadOnly = true;
            this.dataObjects.Size = new System.Drawing.Size(494, 171);
            this.dataObjects.TabIndex = 5;
            // 
            // idDataGridViewTextBoxColumn
            // 
            this.idDataGridViewTextBoxColumn.DataPropertyName = "Id";
            this.idDataGridViewTextBoxColumn.HeaderText = "Id";
            this.idDataGridViewTextBoxColumn.Name = "idDataGridViewTextBoxColumn";
            this.idDataGridViewTextBoxColumn.ReadOnly = true;
            this.idDataGridViewTextBoxColumn.Visible = false;
            // 
            // oidDataGridViewTextBoxColumn
            // 
            this.oidDataGridViewTextBoxColumn.DataPropertyName = "Oid";
            this.oidDataGridViewTextBoxColumn.HeaderText = "Oid";
            this.oidDataGridViewTextBoxColumn.Name = "oidDataGridViewTextBoxColumn";
            this.oidDataGridViewTextBoxColumn.ReadOnly = true;
            this.oidDataGridViewTextBoxColumn.Visible = false;
            // 
            // fnumDataGridViewTextBoxColumn
            // 
            this.fnumDataGridViewTextBoxColumn.DataPropertyName = "Fnum";
            this.fnumDataGridViewTextBoxColumn.HeaderText = "Fnum";
            this.fnumDataGridViewTextBoxColumn.Name = "fnumDataGridViewTextBoxColumn";
            this.fnumDataGridViewTextBoxColumn.ReadOnly = true;
            this.fnumDataGridViewTextBoxColumn.Visible = false;
            // 
            // kmDataGridViewTextBoxColumn
            // 
            this.kmDataGridViewTextBoxColumn.DataPropertyName = "Km";
            this.kmDataGridViewTextBoxColumn.HeaderText = "Километр";
            this.kmDataGridViewTextBoxColumn.Name = "kmDataGridViewTextBoxColumn";
            this.kmDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // ptDataGridViewTextBoxColumn
            // 
            this.ptDataGridViewTextBoxColumn.DataPropertyName = "Pt";
            this.ptDataGridViewTextBoxColumn.HeaderText = "Пикет";
            this.ptDataGridViewTextBoxColumn.Name = "ptDataGridViewTextBoxColumn";
            this.ptDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // mtrDataGridViewTextBoxColumn
            // 
            this.mtrDataGridViewTextBoxColumn.DataPropertyName = "Mtr";
            this.mtrDataGridViewTextBoxColumn.HeaderText = "Метр";
            this.mtrDataGridViewTextBoxColumn.Name = "mtrDataGridViewTextBoxColumn";
            this.mtrDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // xDataGridViewTextBoxColumn
            // 
            this.xDataGridViewTextBoxColumn.DataPropertyName = "X";
            this.xDataGridViewTextBoxColumn.HeaderText = "X";
            this.xDataGridViewTextBoxColumn.Name = "xDataGridViewTextBoxColumn";
            this.xDataGridViewTextBoxColumn.ReadOnly = true;
            this.xDataGridViewTextBoxColumn.Visible = false;
            // 
            // yDataGridViewTextBoxColumn
            // 
            this.yDataGridViewTextBoxColumn.DataPropertyName = "Y";
            this.yDataGridViewTextBoxColumn.HeaderText = "Y";
            this.yDataGridViewTextBoxColumn.Name = "yDataGridViewTextBoxColumn";
            this.yDataGridViewTextBoxColumn.ReadOnly = true;
            this.yDataGridViewTextBoxColumn.Visible = false;
            // 
            // wDataGridViewTextBoxColumn
            // 
            this.wDataGridViewTextBoxColumn.DataPropertyName = "W";
            this.wDataGridViewTextBoxColumn.HeaderText = "W";
            this.wDataGridViewTextBoxColumn.Name = "wDataGridViewTextBoxColumn";
            this.wDataGridViewTextBoxColumn.ReadOnly = true;
            this.wDataGridViewTextBoxColumn.Visible = false;
            // 
            // hDataGridViewTextBoxColumn
            // 
            this.hDataGridViewTextBoxColumn.DataPropertyName = "H";
            this.hDataGridViewTextBoxColumn.HeaderText = "H";
            this.hDataGridViewTextBoxColumn.Name = "hDataGridViewTextBoxColumn";
            this.hDataGridViewTextBoxColumn.ReadOnly = true;
            this.hDataGridViewTextBoxColumn.Visible = false;
            // 
            // prbDataGridViewTextBoxColumn
            // 
            this.prbDataGridViewTextBoxColumn.DataPropertyName = "Prb";
            this.prbDataGridViewTextBoxColumn.HeaderText = "Prb";
            this.prbDataGridViewTextBoxColumn.Name = "prbDataGridViewTextBoxColumn";
            this.prbDataGridViewTextBoxColumn.ReadOnly = true;
            this.prbDataGridViewTextBoxColumn.Visible = false;
            // 
            // msDataGridViewTextBoxColumn
            // 
            this.msDataGridViewTextBoxColumn.DataPropertyName = "Ms";
            this.msDataGridViewTextBoxColumn.HeaderText = "Ms";
            this.msDataGridViewTextBoxColumn.Name = "msDataGridViewTextBoxColumn";
            this.msDataGridViewTextBoxColumn.ReadOnly = true;
            this.msDataGridViewTextBoxColumn.Visible = false;
            // 
            // fileidDataGridViewTextBoxColumn
            // 
            this.fileidDataGridViewTextBoxColumn.DataPropertyName = "fileid";
            this.fileidDataGridViewTextBoxColumn.HeaderText = "fileid";
            this.fileidDataGridViewTextBoxColumn.Name = "fileidDataGridViewTextBoxColumn";
            this.fileidDataGridViewTextBoxColumn.ReadOnly = true;
            this.fileidDataGridViewTextBoxColumn.Visible = false;
            // 
            // bsRdVideoObjects
            // 
            this.bsRdVideoObjects.AllowNew = true;
            this.bsRdVideoObjects.DataSource = typeof(ALARm.Core.VideoObject);
            // 
            // listObjects
            // 
            this.listObjects.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.listObjects.FormattingEnabled = true;
            this.listObjects.Location = new System.Drawing.Point(3, 226);
            this.listObjects.Name = "listObjects";
            this.listObjects.Size = new System.Drawing.Size(374, 167);
            this.listObjects.TabIndex = 8;
            this.listObjects.ItemCheck += new System.Windows.Forms.ItemCheckEventHandler(this.listObjects_filter);
            // 
            // checkObjects
            // 
            this.checkObjects.AutoSize = true;
            this.checkObjects.Location = new System.Drawing.Point(3, 203);
            this.checkObjects.Name = "checkObjects";
            this.checkObjects.Size = new System.Drawing.Size(72, 15);
            this.checkObjects.TabIndex = 12;
            this.checkObjects.Text = "Объекты";
            this.checkObjects.UseSelectable = true;
            // 
            // checkKm
            // 
            this.checkKm.AutoSize = true;
            this.checkKm.Location = new System.Drawing.Point(383, 203);
            this.checkKm.Name = "checkKm";
            this.checkKm.Size = new System.Drawing.Size(94, 15);
            this.checkKm.TabIndex = 13;
            this.checkKm.Text = "Промежуток";
            this.checkKm.UseSelectable = true;
            // 
            // startKm
            // 
            this.startKm.BackColor = System.Drawing.SystemColors.Control;
            // 
            // 
            // 
            this.startKm.CustomButton.Image = null;
            this.startKm.CustomButton.Location = new System.Drawing.Point(92, 1);
            this.startKm.CustomButton.Name = "";
            this.startKm.CustomButton.Size = new System.Drawing.Size(21, 21);
            this.startKm.CustomButton.Style = MetroFramework.MetroColorStyle.Blue;
            this.startKm.CustomButton.TabIndex = 1;
            this.startKm.CustomButton.Theme = MetroFramework.MetroThemeStyle.Light;
            this.startKm.CustomButton.UseSelectable = true;
            this.startKm.CustomButton.Visible = false;
            this.startKm.ForeColor = System.Drawing.SystemColors.ControlText;
            this.startKm.Lines = new string[0];
            this.startKm.Location = new System.Drawing.Point(383, 243);
            this.startKm.MaxLength = 32767;
            this.startKm.Name = "startKm";
            this.startKm.PasswordChar = '\0';
            this.startKm.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.startKm.SelectedText = "";
            this.startKm.SelectionLength = 0;
            this.startKm.SelectionStart = 0;
            this.startKm.ShortcutsEnabled = true;
            this.startKm.Size = new System.Drawing.Size(114, 23);
            this.startKm.TabIndex = 14;
            this.startKm.UseSelectable = true;
            this.startKm.WaterMarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
            this.startKm.WaterMarkFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Pixel);
            this.startKm.TextChanged += new System.EventHandler(this.startFinalKm_filter);
            // 
            // finalKm
            // 
            this.finalKm.BackColor = System.Drawing.SystemColors.Control;
            // 
            // 
            // 
            this.finalKm.CustomButton.Image = null;
            this.finalKm.CustomButton.Location = new System.Drawing.Point(92, 1);
            this.finalKm.CustomButton.Name = "";
            this.finalKm.CustomButton.Size = new System.Drawing.Size(21, 21);
            this.finalKm.CustomButton.Style = MetroFramework.MetroColorStyle.Blue;
            this.finalKm.CustomButton.TabIndex = 1;
            this.finalKm.CustomButton.Theme = MetroFramework.MetroThemeStyle.Light;
            this.finalKm.CustomButton.UseSelectable = true;
            this.finalKm.CustomButton.Visible = false;
            this.finalKm.Lines = new string[0];
            this.finalKm.Location = new System.Drawing.Point(383, 285);
            this.finalKm.MaxLength = 32767;
            this.finalKm.Name = "finalKm";
            this.finalKm.PasswordChar = '\0';
            this.finalKm.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.finalKm.SelectedText = "";
            this.finalKm.SelectionLength = 0;
            this.finalKm.SelectionStart = 0;
            this.finalKm.ShortcutsEnabled = true;
            this.finalKm.Size = new System.Drawing.Size(114, 23);
            this.finalKm.TabIndex = 15;
            this.finalKm.UseSelectable = true;
            this.finalKm.WaterMarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
            this.finalKm.WaterMarkFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Pixel);
            this.finalKm.TextChanged += new System.EventHandler(this.startFinalKm_filter);
            // 
            // metroLabel1
            // 
            this.metroLabel1.AutoSize = true;
            this.metroLabel1.FontSize = MetroFramework.MetroLabelSize.Small;
            this.metroLabel1.Location = new System.Drawing.Point(385, 226);
            this.metroLabel1.Name = "metroLabel1";
            this.metroLabel1.Size = new System.Drawing.Size(68, 15);
            this.metroLabel1.TabIndex = 16;
            this.metroLabel1.Text = "Начало (км)";
            // 
            // metroLabel2
            // 
            this.metroLabel2.AutoSize = true;
            this.metroLabel2.FontSize = MetroFramework.MetroLabelSize.Small;
            this.metroLabel2.Location = new System.Drawing.Point(382, 268);
            this.metroLabel2.Name = "metroLabel2";
            this.metroLabel2.Size = new System.Drawing.Size(62, 15);
            this.metroLabel2.TabIndex = 17;
            this.metroLabel2.Text = "Конец (км)";
            // 
            // rdVideoObjectsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.metroLabel2);
            this.Controls.Add(this.metroLabel1);
            this.Controls.Add(this.finalKm);
            this.Controls.Add(this.startKm);
            this.Controls.Add(this.checkKm);
            this.Controls.Add(this.checkObjects);
            this.Controls.Add(this.listObjects);
            this.Controls.Add(this.dataObjects);
            this.Name = "rdVideoObjectsControl";
            this.Size = new System.Drawing.Size(500, 411);
            ((System.ComponentModel.ISupportInitialize)(this.dataObjects)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bsRdVideoObjects)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.CheckedListBox listObjects;
        private System.Windows.Forms.DataGridView dataObjects;
        private System.Windows.Forms.BindingSource bsRdVideoObjects;
        private System.Windows.Forms.DataGridViewTextBoxColumn idDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn oidDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn fnumDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn kmDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn ptDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn mtrDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn xDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn yDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn wDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn hDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn prbDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn msDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn fileidDataGridViewTextBoxColumn;
        private MetroFramework.Controls.MetroCheckBox checkObjects;
        private MetroFramework.Controls.MetroCheckBox checkKm;
        private MetroFramework.Controls.MetroTextBox startKm;
        private MetroFramework.Controls.MetroTextBox finalKm;
        private MetroFramework.Controls.MetroLabel metroLabel1;
        private MetroFramework.Controls.MetroLabel metroLabel2;
    }
}
