namespace ALARm.controls
{
    partial class tripsControl
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
            this.dataTrips = new System.Windows.Forms.DataGridView();
            this.checkAll = new MetroFramework.Controls.MetroCheckBox();
            this.bindingSource1 = new System.Windows.Forms.BindingSource(this.components);
            this.checkTrips = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.metroLabel1 = new MetroFramework.Controls.MetroLabel();
            ((System.ComponentModel.ISupportInitialize)(this.dataTrips)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).BeginInit();
            this.SuspendLayout();
            // 
            // dataTrips
            // 
            this.dataTrips.AllowUserToAddRows = false;
            this.dataTrips.AllowUserToDeleteRows = false;
            this.dataTrips.BackgroundColor = System.Drawing.SystemColors.ButtonHighlight;
            this.dataTrips.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataTrips.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.checkTrips});
            this.dataTrips.Location = new System.Drawing.Point(0, 26);
            this.dataTrips.MultiSelect = false;
            this.dataTrips.Name = "dataTrips";
            this.dataTrips.RowHeadersVisible = false;
            this.dataTrips.RowHeadersWidth = 20;
            this.dataTrips.Size = new System.Drawing.Size(300, 171);
            this.dataTrips.TabIndex = 4;
            this.dataTrips.SelectionChanged += new System.EventHandler(this.dataTrips_SelectionChanged);
            // 
            // checkAll
            // 
            this.checkAll.AutoSize = true;
            this.checkAll.Location = new System.Drawing.Point(3, 5);
            this.checkAll.Name = "checkAll";
            this.checkAll.Size = new System.Drawing.Size(54, 15);
            this.checkAll.TabIndex = 5;
            this.checkAll.Text = "check";
            this.checkAll.UseSelectable = true;
            this.checkAll.CheckedChanged += new System.EventHandler(this.checkAll_Checked);
            // 
            // checkTrips
            // 
            this.checkTrips.DataPropertyName = "Checked_Status";
            this.checkTrips.Frozen = true;
            this.checkTrips.HeaderText = "";
            this.checkTrips.MinimumWidth = 20;
            this.checkTrips.Name = "checkTrips";
            this.checkTrips.Width = 20;
            // 
            // metroLabel1
            // 
            this.metroLabel1.AutoSize = true;
            this.metroLabel1.Location = new System.Drawing.Point(3, 5);
            this.metroLabel1.Name = "metroLabel1";
            this.metroLabel1.Size = new System.Drawing.Size(0, 0);
            this.metroLabel1.TabIndex = 6;
            // 
            // tripsControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.metroLabel1);
            this.Controls.Add(this.checkAll);
            this.Controls.Add(this.dataTrips);
            this.Name = "tripsControl";
            this.Size = new System.Drawing.Size(300, 197);
            ((System.ComponentModel.ISupportInitialize)(this.dataTrips)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.bindingSource1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.DataGridView dataTrips;
        private MetroFramework.Controls.MetroCheckBox checkAll;
        private System.Windows.Forms.BindingSource bindingSource1;
        private System.Windows.Forms.DataGridViewCheckBoxColumn checkTrips;
        private MetroFramework.Controls.MetroLabel metroLabel1;
    }
}
