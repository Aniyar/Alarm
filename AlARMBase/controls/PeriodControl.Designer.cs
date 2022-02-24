namespace ALARm.controls
{
    partial class PeriodControl
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

        #region Component Designer generated code

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
            this.mgPeriod = new MetroFramework.Controls.MetroGrid();
            this.startDateDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.finalDateDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.periodBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.tsSectionPeriod = new System.Windows.Forms.ToolStrip();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.btnAdd = new System.Windows.Forms.ToolStripButton();
            this.btnRemove = new System.Windows.Forms.ToolStripButton();
            this.btnEdit = new System.Windows.Forms.ToolStripButton();
            ((System.ComponentModel.ISupportInitialize)(this.mgPeriod)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.periodBindingSource)).BeginInit();
            this.tsSectionPeriod.SuspendLayout();
            this.SuspendLayout();
            // 
            // mgPeriod
            // 
            this.mgPeriod.AllowUserToAddRows = false;
            this.mgPeriod.AllowUserToResizeRows = false;
            this.mgPeriod.AutoGenerateColumns = false;
            this.mgPeriod.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.mgPeriod.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.mgPeriod.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.mgPeriod.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(170)))), ((int)(((byte)(173)))));
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(201)))), ((int)(((byte)(206)))));
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(17)))), ((int)(((byte)(17)))));
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.mgPeriod.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.mgPeriod.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.mgPeriod.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.startDateDataGridViewTextBoxColumn,
            this.finalDateDataGridViewTextBoxColumn});
            this.mgPeriod.DataSource = this.periodBindingSource;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(136)))), ((int)(((byte)(136)))), ((int)(((byte)(136)))));
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(201)))), ((int)(((byte)(206)))));
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(17)))), ((int)(((byte)(17)))));
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.mgPeriod.DefaultCellStyle = dataGridViewCellStyle2;
            this.mgPeriod.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mgPeriod.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.mgPeriod.EnableHeadersVisualStyles = false;
            this.mgPeriod.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.mgPeriod.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.mgPeriod.Location = new System.Drawing.Point(0, 31);
            this.mgPeriod.Name = "mgPeriod";
            this.mgPeriod.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(170)))), ((int)(((byte)(173)))));
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(201)))), ((int)(((byte)(206)))));
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(17)))), ((int)(((byte)(17)))));
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.mgPeriod.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.mgPeriod.RowHeadersVisible = false;
            this.mgPeriod.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.mgPeriod.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.mgPeriod.Size = new System.Drawing.Size(260, 119);
            this.mgPeriod.Style = MetroFramework.MetroColorStyle.Teal;
            this.mgPeriod.TabIndex = 6;
            this.mgPeriod.SelectionChanged += new System.EventHandler(this.mgPeriod_SelectionChanged);
            // 
            // startDateDataGridViewTextBoxColumn
            // 
            this.startDateDataGridViewTextBoxColumn.DataPropertyName = "Start_Date";
            this.startDateDataGridViewTextBoxColumn.HeaderText = "Дата начала";
            this.startDateDataGridViewTextBoxColumn.Name = "startDateDataGridViewTextBoxColumn";
            this.startDateDataGridViewTextBoxColumn.Width = 110;
            // 
            // finalDateDataGridViewTextBoxColumn
            // 
            this.finalDateDataGridViewTextBoxColumn.DataPropertyName = "Final_Date";
            this.finalDateDataGridViewTextBoxColumn.HeaderText = "Дата конца";
            this.finalDateDataGridViewTextBoxColumn.Name = "finalDateDataGridViewTextBoxColumn";
            this.finalDateDataGridViewTextBoxColumn.Width = 110;
            // 
            // periodBindingSource
            // 
            this.periodBindingSource.DataSource = typeof(ALARm.Core.Period);
            // 
            // tsSectionPeriod
            // 
            this.tsSectionPeriod.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.tsSectionPeriod.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel1,
            this.toolStripSeparator2,
            this.btnAdd,
            this.btnRemove,
            this.btnEdit});
            this.tsSectionPeriod.Location = new System.Drawing.Point(0, 0);
            this.tsSectionPeriod.Name = "tsSectionPeriod";
            this.tsSectionPeriod.Size = new System.Drawing.Size(260, 31);
            this.tsSectionPeriod.TabIndex = 5;
            this.tsSectionPeriod.Text = "toolStrip2";
            this.tsSectionPeriod.ItemClicked += new System.Windows.Forms.ToolStripItemClickedEventHandler(this.tsSectionPeriod_ItemClicked);
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(101, 28);
            this.toolStripLabel1.Text = "Период действия";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 31);
            // 
            // btnAdd
            // 
            this.btnAdd.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnAdd.Image = global::ALARm.Properties.Resources.add;
            this.btnAdd.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnAdd.Name = "btnAdd";
            this.btnAdd.Size = new System.Drawing.Size(28, 28);
            this.btnAdd.Text = "добавить";
            this.btnAdd.Click += new System.EventHandler(this.btnAdd_Click);
            // 
            // btnRemove
            // 
            this.btnRemove.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnRemove.Image = global::ALARm.Properties.Resources.remove;
            this.btnRemove.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnRemove.Name = "btnRemove";
            this.btnRemove.Size = new System.Drawing.Size(28, 28);
            this.btnRemove.Text = "удалить";
            this.btnRemove.Click += new System.EventHandler(this.btnRemove_Click);
            // 
            // btnEdit
            // 
            this.btnEdit.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnEdit.Image = global::ALARm.Properties.Resources.edit;
            this.btnEdit.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnEdit.Name = "btnEdit";
            this.btnEdit.Size = new System.Drawing.Size(28, 28);
            this.btnEdit.Text = "редактировать";
            this.btnEdit.Click += new System.EventHandler(this.btnEdit_Click);
            // 
            // PeriodControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.mgPeriod);
            this.Controls.Add(this.tsSectionPeriod);
            this.Name = "PeriodControl";
            this.Size = new System.Drawing.Size(260, 150);
            ((System.ComponentModel.ISupportInitialize)(this.mgPeriod)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.periodBindingSource)).EndInit();
            this.tsSectionPeriod.ResumeLayout(false);
            this.tsSectionPeriod.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MetroFramework.Controls.MetroGrid mgPeriod;
        private System.Windows.Forms.ToolStrip tsSectionPeriod;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton btnAdd;
        private System.Windows.Forms.ToolStripButton btnRemove;
        private System.Windows.Forms.BindingSource periodBindingSource;
        private System.Windows.Forms.DataGridViewTextBoxColumn startDateDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn finalDateDataGridViewTextBoxColumn;
        private System.Windows.Forms.ToolStripButton btnEdit;
    }
}
