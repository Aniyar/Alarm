namespace ALARm_Report.controls
{
    partial class ChoiseForm
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle3 = new System.Windows.Forms.DataGridViewCellStyle();
            this.metroPanel1 = new MetroFramework.Controls.MetroPanel();
            this.metroGrid1 = new MetroFramework.Controls.MetroGrid();
            this.admTrackBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.metroPanel2 = new MetroFramework.Controls.MetroPanel();
            this.mbAccept = new MetroFramework.Controls.MetroButton();
            this.metroPanel3 = new MetroFramework.Controls.MetroPanel();
            this.mbCancel = new MetroFramework.Controls.MetroButton();
            this.cbTrips = new MetroFramework.Controls.MetroComboBox();
            this.tripsBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.metroPanel4 = new MetroFramework.Controls.MetroPanel();
            this.tbWear = new MetroFramework.Controls.MetroTextBox();
            this.mlWear = new MetroFramework.Controls.MetroLabel();
            this.codeDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.identityDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.Direction = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.acceptDataGridViewCheckBoxColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.metroPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.metroGrid1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.admTrackBindingSource)).BeginInit();
            this.metroPanel2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.tripsBindingSource)).BeginInit();
            this.metroPanel4.SuspendLayout();
            this.SuspendLayout();
            // 
            // metroPanel1
            // 
            this.metroPanel1.Controls.Add(this.metroGrid1);
            this.metroPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.metroPanel1.HorizontalScrollbarBarColor = true;
            this.metroPanel1.HorizontalScrollbarHighlightOnWheel = false;
            this.metroPanel1.HorizontalScrollbarSize = 10;
            this.metroPanel1.Location = new System.Drawing.Point(20, 89);
            this.metroPanel1.Name = "metroPanel1";
            this.metroPanel1.Padding = new System.Windows.Forms.Padding(0, 5, 0, 5);
            this.metroPanel1.Size = new System.Drawing.Size(529, 79);
            this.metroPanel1.TabIndex = 0;
            this.metroPanel1.VerticalScrollbarBarColor = true;
            this.metroPanel1.VerticalScrollbarHighlightOnWheel = false;
            this.metroPanel1.VerticalScrollbarSize = 10;
            // 
            // metroGrid1
            // 
            this.metroGrid1.AllowUserToAddRows = false;
            this.metroGrid1.AllowUserToDeleteRows = false;
            this.metroGrid1.AllowUserToResizeRows = false;
            this.metroGrid1.AutoGenerateColumns = false;
            this.metroGrid1.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.metroGrid1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.metroGrid1.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.metroGrid1.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(174)))), ((int)(((byte)(219)))));
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(198)))), ((int)(((byte)(247)))));
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(17)))), ((int)(((byte)(17)))));
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.metroGrid1.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.metroGrid1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.metroGrid1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.codeDataGridViewTextBoxColumn,
            this.identityDataGridViewTextBoxColumn,
            this.Direction,
            this.acceptDataGridViewCheckBoxColumn});
            this.metroGrid1.DataSource = this.admTrackBindingSource;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(136)))), ((int)(((byte)(136)))), ((int)(((byte)(136)))));
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(198)))), ((int)(((byte)(247)))));
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(17)))), ((int)(((byte)(17)))));
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.metroGrid1.DefaultCellStyle = dataGridViewCellStyle2;
            this.metroGrid1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.metroGrid1.EnableHeadersVisualStyles = false;
            this.metroGrid1.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.metroGrid1.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.metroGrid1.Location = new System.Drawing.Point(0, 5);
            this.metroGrid1.MultiSelect = false;
            this.metroGrid1.Name = "metroGrid1";
            this.metroGrid1.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(174)))), ((int)(((byte)(219)))));
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(198)))), ((int)(((byte)(247)))));
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(17)))), ((int)(((byte)(17)))));
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.metroGrid1.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.metroGrid1.RowHeadersVisible = false;
            this.metroGrid1.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.metroGrid1.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.metroGrid1.Size = new System.Drawing.Size(529, 69);
            this.metroGrid1.TabIndex = 2;
            this.metroGrid1.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.metroGrid1_CellContentClick);
            // 
            // admTrackBindingSource
            // 
            this.admTrackBindingSource.DataSource = typeof(ALARm.Core.AdmTrack);
            // 
            // metroPanel2
            // 
            this.metroPanel2.Controls.Add(this.mbAccept);
            this.metroPanel2.Controls.Add(this.metroPanel3);
            this.metroPanel2.Controls.Add(this.mbCancel);
            this.metroPanel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.metroPanel2.HorizontalScrollbarBarColor = true;
            this.metroPanel2.HorizontalScrollbarHighlightOnWheel = false;
            this.metroPanel2.HorizontalScrollbarSize = 10;
            this.metroPanel2.Location = new System.Drawing.Point(20, 224);
            this.metroPanel2.MaximumSize = new System.Drawing.Size(1000, 31);
            this.metroPanel2.MinimumSize = new System.Drawing.Size(200, 31);
            this.metroPanel2.Name = "metroPanel2";
            this.metroPanel2.Size = new System.Drawing.Size(529, 31);
            this.metroPanel2.TabIndex = 1;
            this.metroPanel2.VerticalScrollbarBarColor = true;
            this.metroPanel2.VerticalScrollbarHighlightOnWheel = false;
            this.metroPanel2.VerticalScrollbarSize = 10;
            // 
            // mbAccept
            // 
            this.mbAccept.Dock = System.Windows.Forms.DockStyle.Right;
            this.mbAccept.Location = new System.Drawing.Point(332, 0);
            this.mbAccept.MinimumSize = new System.Drawing.Size(93, 31);
            this.mbAccept.Name = "mbAccept";
            this.mbAccept.Size = new System.Drawing.Size(93, 31);
            this.mbAccept.TabIndex = 3;
            this.mbAccept.Text = "Принять";
            this.mbAccept.UseSelectable = true;
            this.mbAccept.Click += new System.EventHandler(this.mbAccept_Click);
            // 
            // metroPanel3
            // 
            this.metroPanel3.Dock = System.Windows.Forms.DockStyle.Right;
            this.metroPanel3.HorizontalScrollbarBarColor = true;
            this.metroPanel3.HorizontalScrollbarHighlightOnWheel = false;
            this.metroPanel3.HorizontalScrollbarSize = 10;
            this.metroPanel3.Location = new System.Drawing.Point(425, 0);
            this.metroPanel3.MaximumSize = new System.Drawing.Size(11, 31);
            this.metroPanel3.MinimumSize = new System.Drawing.Size(11, 31);
            this.metroPanel3.Name = "metroPanel3";
            this.metroPanel3.Size = new System.Drawing.Size(11, 31);
            this.metroPanel3.TabIndex = 4;
            this.metroPanel3.VerticalScrollbarBarColor = true;
            this.metroPanel3.VerticalScrollbarHighlightOnWheel = false;
            this.metroPanel3.VerticalScrollbarSize = 10;
            // 
            // mbCancel
            // 
            this.mbCancel.Dock = System.Windows.Forms.DockStyle.Right;
            this.mbCancel.Location = new System.Drawing.Point(436, 0);
            this.mbCancel.MinimumSize = new System.Drawing.Size(93, 31);
            this.mbCancel.Name = "mbCancel";
            this.mbCancel.Size = new System.Drawing.Size(93, 31);
            this.mbCancel.TabIndex = 2;
            this.mbCancel.Text = "Отмена";
            this.mbCancel.UseSelectable = true;
            this.mbCancel.Click += new System.EventHandler(this.mbCancel_Click);
            // 
            // cbTrips
            // 
            this.cbTrips.DataSource = this.tripsBindingSource;
            this.cbTrips.DisplayMember = "TripInfo";
            this.cbTrips.Dock = System.Windows.Forms.DockStyle.Top;
            this.cbTrips.FormattingEnabled = true;
            this.cbTrips.ItemHeight = 23;
            this.cbTrips.Location = new System.Drawing.Point(20, 60);
            this.cbTrips.Name = "cbTrips";
            this.cbTrips.Size = new System.Drawing.Size(529, 29);
            this.cbTrips.TabIndex = 2;
            this.cbTrips.UseSelectable = true;
            this.cbTrips.ValueMember = "Id";
            this.cbTrips.Visible = false;
            this.cbTrips.SelectedValueChanged += new System.EventHandler(this.cbTrips_SelectedValueChanged);
            // 
            // tripsBindingSource
            // 
            this.tripsBindingSource.DataSource = typeof(ALARm.Core.Trips);
            // 
            // metroPanel4
            // 
            this.metroPanel4.Controls.Add(this.tbWear);
            this.metroPanel4.Controls.Add(this.mlWear);
            this.metroPanel4.Dock = System.Windows.Forms.DockStyle.Top;
            this.metroPanel4.HorizontalScrollbarBarColor = true;
            this.metroPanel4.HorizontalScrollbarHighlightOnWheel = false;
            this.metroPanel4.HorizontalScrollbarSize = 10;
            this.metroPanel4.Location = new System.Drawing.Point(20, 168);
            this.metroPanel4.Name = "metroPanel4";
            this.metroPanel4.Padding = new System.Windows.Forms.Padding(0, 5, 0, 5);
            this.metroPanel4.Size = new System.Drawing.Size(529, 39);
            this.metroPanel4.TabIndex = 3;
            this.metroPanel4.VerticalScrollbarBarColor = true;
            this.metroPanel4.VerticalScrollbarHighlightOnWheel = false;
            this.metroPanel4.VerticalScrollbarSize = 10;
            this.metroPanel4.Visible = false;
            // 
            // tbWear
            // 
            // 
            // 
            // 
            this.tbWear.CustomButton.Image = null;
            this.tbWear.CustomButton.Location = new System.Drawing.Point(361, 1);
            this.tbWear.CustomButton.Name = "";
            this.tbWear.CustomButton.Size = new System.Drawing.Size(27, 27);
            this.tbWear.CustomButton.Style = MetroFramework.MetroColorStyle.Blue;
            this.tbWear.CustomButton.TabIndex = 1;
            this.tbWear.CustomButton.Theme = MetroFramework.MetroThemeStyle.Light;
            this.tbWear.CustomButton.UseSelectable = true;
            this.tbWear.CustomButton.Visible = false;
            this.tbWear.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbWear.Lines = new string[0];
            this.tbWear.Location = new System.Drawing.Point(140, 5);
            this.tbWear.MaxLength = 32767;
            this.tbWear.Name = "tbWear";
            this.tbWear.PasswordChar = '\0';
            this.tbWear.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.tbWear.SelectedText = "";
            this.tbWear.SelectionLength = 0;
            this.tbWear.SelectionStart = 0;
            this.tbWear.ShortcutsEnabled = true;
            this.tbWear.Size = new System.Drawing.Size(389, 29);
            this.tbWear.TabIndex = 3;
            this.tbWear.UseSelectable = true;
            this.tbWear.WaterMarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
            this.tbWear.WaterMarkFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Pixel);
            // 
            // mlWear
            // 
            this.mlWear.Dock = System.Windows.Forms.DockStyle.Left;
            this.mlWear.Location = new System.Drawing.Point(0, 5);
            this.mlWear.MaximumSize = new System.Drawing.Size(140, 29);
            this.mlWear.MinimumSize = new System.Drawing.Size(140, 29);
            this.mlWear.Name = "mlWear";
            this.mlWear.Size = new System.Drawing.Size(140, 29);
            this.mlWear.TabIndex = 2;
            this.mlWear.Text = "Износ";
            this.mlWear.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // codeDataGridViewTextBoxColumn
            // 
            this.codeDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.codeDataGridViewTextBoxColumn.DataPropertyName = "Code";
            this.codeDataGridViewTextBoxColumn.FillWeight = 10.50008F;
            this.codeDataGridViewTextBoxColumn.HeaderText = "Путь";
            this.codeDataGridViewTextBoxColumn.Name = "codeDataGridViewTextBoxColumn";
            this.codeDataGridViewTextBoxColumn.ReadOnly = true;
            // 
            // identityDataGridViewTextBoxColumn
            // 
            this.identityDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.identityDataGridViewTextBoxColumn.DataPropertyName = "Identity";
            this.identityDataGridViewTextBoxColumn.FillWeight = 94.92815F;
            this.identityDataGridViewTextBoxColumn.HeaderText = "Тип";
            this.identityDataGridViewTextBoxColumn.Name = "identityDataGridViewTextBoxColumn";
            this.identityDataGridViewTextBoxColumn.ReadOnly = true;
            this.identityDataGridViewTextBoxColumn.Width = 170;
            // 
            // Direction
            // 
            this.Direction.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.Direction.DataPropertyName = "Direction";
            this.Direction.HeaderText = "Направление";
            this.Direction.Name = "Direction";
            this.Direction.ReadOnly = true;
            this.Direction.Width = 170;
            // 
            // acceptDataGridViewCheckBoxColumn
            // 
            this.acceptDataGridViewCheckBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.None;
            this.acceptDataGridViewCheckBoxColumn.DataPropertyName = "Accept";
            this.acceptDataGridViewCheckBoxColumn.FillWeight = 0.4112077F;
            this.acceptDataGridViewCheckBoxColumn.HeaderText = "Выбор";
            this.acceptDataGridViewCheckBoxColumn.Name = "acceptDataGridViewCheckBoxColumn";
            this.acceptDataGridViewCheckBoxColumn.Width = 70;
            // 
            // ChoiseForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(569, 275);
            this.Controls.Add(this.metroPanel4);
            this.Controls.Add(this.metroPanel1);
            this.Controls.Add(this.cbTrips);
            this.Controls.Add(this.metroPanel2);
            this.Name = "ChoiseForm";
            this.Text = "Выбор пути";
            this.metroPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.metroGrid1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.admTrackBindingSource)).EndInit();
            this.metroPanel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.tripsBindingSource)).EndInit();
            this.metroPanel4.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private MetroFramework.Controls.MetroPanel metroPanel1;
        private MetroFramework.Controls.MetroPanel metroPanel2;
        private MetroFramework.Controls.MetroButton mbAccept;
        private MetroFramework.Controls.MetroPanel metroPanel3;
        private MetroFramework.Controls.MetroButton mbCancel;
        private MetroFramework.Controls.MetroComboBox cbTrips;
        private MetroFramework.Controls.MetroPanel metroPanel4;
        private MetroFramework.Controls.MetroLabel mlWear;
        private MetroFramework.Controls.MetroTextBox tbWear;
        private MetroFramework.Controls.MetroGrid metroGrid1;
        private System.Windows.Forms.BindingSource admTrackBindingSource;
        private System.Windows.Forms.BindingSource tripsBindingSource;
        private System.Windows.Forms.DataGridViewTextBoxColumn codeDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn identityDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn Direction;
        private System.Windows.Forms.DataGridViewCheckBoxColumn acceptDataGridViewCheckBoxColumn;
    }
}