namespace ALARm.controls
{
    partial class AdmUnitControls
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
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.UnitName = new System.Windows.Forms.ToolStripLabel();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.AddBtn = new System.Windows.Forms.ToolStripButton();
            this.DeleteBtn = new System.Windows.Forms.ToolStripButton();
            this.EditBtn = new System.Windows.Forms.ToolStripButton();
            this.metrogrid = new MetroFramework.Controls.MetroGrid();
            this.TrackBorder = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.codeDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.nameDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.admUnitBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.metrogrid)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.admUnitBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // toolStrip1
            // 
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.UnitName,
            this.toolStripSeparator1,
            this.AddBtn,
            this.DeleteBtn,
            this.EditBtn});
            this.toolStrip1.Location = new System.Drawing.Point(0, 0);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(262, 25);
            this.toolStrip1.TabIndex = 0;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // UnitName
            // 
            this.UnitName.Name = "UnitName";
            this.UnitName.Size = new System.Drawing.Size(0, 22);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // AddBtn
            // 
            this.AddBtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.AddBtn.Image = global::ALARm.Properties.Resources.add;
            this.AddBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.AddBtn.Name = "AddBtn";
            this.AddBtn.Size = new System.Drawing.Size(23, 22);
            this.AddBtn.Text = "добавить";
            this.AddBtn.Click += new System.EventHandler(this.AddBtn_Click);
            // 
            // DeleteBtn
            // 
            this.DeleteBtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.DeleteBtn.Image = global::ALARm.Properties.Resources.remove;
            this.DeleteBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.DeleteBtn.Name = "DeleteBtn";
            this.DeleteBtn.Size = new System.Drawing.Size(23, 22);
            this.DeleteBtn.Text = "удалить";
            this.DeleteBtn.Click += new System.EventHandler(this.DeleteBtn_Click);
            // 
            // EditBtn
            // 
            this.EditBtn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.EditBtn.Image = global::ALARm.Properties.Resources.edit;
            this.EditBtn.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.EditBtn.Name = "EditBtn";
            this.EditBtn.Size = new System.Drawing.Size(23, 22);
            this.EditBtn.Text = "редактировать";
            this.EditBtn.Click += new System.EventHandler(this.EditBtn_Click);
            // 
            // metrogrid
            // 
            this.metrogrid.AllowUserToAddRows = false;
            this.metrogrid.AllowUserToResizeRows = false;
            this.metrogrid.AutoGenerateColumns = false;
            this.metrogrid.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.metrogrid.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.metrogrid.CellBorderStyle = System.Windows.Forms.DataGridViewCellBorderStyle.None;
            this.metrogrid.ColumnHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(170)))), ((int)(((byte)(173)))));
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            dataGridViewCellStyle1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(201)))), ((int)(((byte)(206)))));
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(17)))), ((int)(((byte)(17)))));
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.metrogrid.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.metrogrid.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.metrogrid.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.codeDataGridViewTextBoxColumn,
            this.nameDataGridViewTextBoxColumn,
            this.TrackBorder});
            this.metrogrid.Cursor = System.Windows.Forms.Cursors.Hand;
            this.metrogrid.DataSource = this.admUnitBindingSource;
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle2.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            dataGridViewCellStyle2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(136)))), ((int)(((byte)(136)))), ((int)(((byte)(136)))));
            dataGridViewCellStyle2.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(201)))), ((int)(((byte)(206)))));
            dataGridViewCellStyle2.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(17)))), ((int)(((byte)(17)))));
            dataGridViewCellStyle2.WrapMode = System.Windows.Forms.DataGridViewTriState.False;
            this.metrogrid.DefaultCellStyle = dataGridViewCellStyle2;
            this.metrogrid.Dock = System.Windows.Forms.DockStyle.Fill;
            this.metrogrid.EditMode = System.Windows.Forms.DataGridViewEditMode.EditProgrammatically;
            this.metrogrid.EnableHeadersVisualStyles = false;
            this.metrogrid.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            this.metrogrid.GridColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.metrogrid.Location = new System.Drawing.Point(0, 25);
            this.metrogrid.MultiSelect = false;
            this.metrogrid.Name = "metrogrid";
            this.metrogrid.RowHeadersBorderStyle = System.Windows.Forms.DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle3.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(170)))), ((int)(((byte)(173)))));
            dataGridViewCellStyle3.Font = new System.Drawing.Font("Segoe UI", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Pixel);
            dataGridViewCellStyle3.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            dataGridViewCellStyle3.SelectionBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(201)))), ((int)(((byte)(206)))));
            dataGridViewCellStyle3.SelectionForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(17)))), ((int)(((byte)(17)))), ((int)(((byte)(17)))));
            dataGridViewCellStyle3.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.metrogrid.RowHeadersDefaultCellStyle = dataGridViewCellStyle3;
            this.metrogrid.RowHeadersVisible = false;
            this.metrogrid.RowHeadersWidthSizeMode = System.Windows.Forms.DataGridViewRowHeadersWidthSizeMode.DisableResizing;
            this.metrogrid.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
            this.metrogrid.Size = new System.Drawing.Size(262, 125);
            this.metrogrid.Style = MetroFramework.MetroColorStyle.Teal;
            this.metrogrid.TabIndex = 1;
            this.metrogrid.SelectionChanged += new System.EventHandler(this.MetroGrid1_SelectionChanged);
            // 
            // TrackBorder
            // 
            this.TrackBorder.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.TrackBorder.DataPropertyName = "Border";
            this.TrackBorder.HeaderText = "Граница";
            this.TrackBorder.Name = "TrackBorder";
            this.TrackBorder.Visible = false;
            this.TrackBorder.Width = 75;
            // 
            // codeDataGridViewTextBoxColumn
            // 
            this.codeDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.DisplayedCells;
            this.codeDataGridViewTextBoxColumn.DataPropertyName = "Code";
            this.codeDataGridViewTextBoxColumn.HeaderText = "Код";
            this.codeDataGridViewTextBoxColumn.Name = "codeDataGridViewTextBoxColumn";
            this.codeDataGridViewTextBoxColumn.Width = 49;
            // 
            // nameDataGridViewTextBoxColumn
            // 
            this.nameDataGridViewTextBoxColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.nameDataGridViewTextBoxColumn.DataPropertyName = "Name";
            this.nameDataGridViewTextBoxColumn.HeaderText = "Название";
            this.nameDataGridViewTextBoxColumn.Name = "nameDataGridViewTextBoxColumn";
            // 
            // admUnitBindingSource
            // 
            this.admUnitBindingSource.DataSource = typeof(ALARm.Core.AdmUnit);
            // 
            // AdmUnitControls
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.metrogrid);
            this.Controls.Add(this.toolStrip1);
            this.MinimumSize = new System.Drawing.Size(218, 150);
            this.Name = "AdmUnitControls";
            this.Size = new System.Drawing.Size(262, 150);
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.metrogrid)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.admUnitBindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripLabel UnitName;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton AddBtn;
        private System.Windows.Forms.ToolStripButton DeleteBtn;
        private System.Windows.Forms.BindingSource admUnitBindingSource;
        private MetroFramework.Controls.MetroGrid metrogrid;
        private System.Windows.Forms.ToolStripButton EditBtn;
        private System.Windows.Forms.DataGridViewTextBoxColumn codeDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn nameDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn TrackBorder;
    }
}
