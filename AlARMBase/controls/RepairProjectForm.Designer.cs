namespace ALARm.controls
{
    partial class RepairProjectForm
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
            this.metroPanel2 = new MetroFramework.Controls.MetroPanel();
            this.btnSave = new MetroFramework.Controls.MetroButton();
            this.btnCancel = new MetroFramework.Controls.MetroButton();
            this.metroPanel3 = new MetroFramework.Controls.MetroPanel();
            this.mdtDate = new MetroFramework.Controls.MetroDateTime();
            this.metroLabel2 = new MetroFramework.Controls.MetroLabel();
            this.mpFinalM = new MetroFramework.Controls.MetroPanel();
            this.metroLabel4 = new MetroFramework.Controls.MetroLabel();
            this.speedControl = new System.Windows.Forms.NumericUpDown();
            this.cbAccept = new ALARm.controls.CatalogListBox();
            this.cbType = new ALARm.controls.CatalogListBox();
            this.coordControl = new ALARm.controls.CoordControl();
            this.metroPanel2.SuspendLayout();
            this.metroPanel3.SuspendLayout();
            this.mpFinalM.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.speedControl)).BeginInit();
            this.SuspendLayout();
            // 
            // metroPanel2
            // 
            this.metroPanel2.Controls.Add(this.btnSave);
            this.metroPanel2.Controls.Add(this.btnCancel);
            this.metroPanel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.metroPanel2.HorizontalScrollbarBarColor = true;
            this.metroPanel2.HorizontalScrollbarHighlightOnWheel = false;
            this.metroPanel2.HorizontalScrollbarSize = 10;
            this.metroPanel2.Location = new System.Drawing.Point(13, 389);
            this.metroPanel2.Name = "metroPanel2";
            this.metroPanel2.Size = new System.Drawing.Size(401, 31);
            this.metroPanel2.TabIndex = 13;
            this.metroPanel2.VerticalScrollbarBarColor = true;
            this.metroPanel2.VerticalScrollbarHighlightOnWheel = false;
            this.metroPanel2.VerticalScrollbarSize = 10;
            // 
            // btnSave
            // 
            this.btnSave.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnSave.ForeColor = System.Drawing.SystemColors.Control;
            this.btnSave.Location = new System.Drawing.Point(197, 0);
            this.btnSave.Margin = new System.Windows.Forms.Padding(7, 6, 7, 6);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(102, 31);
            this.btnSave.TabIndex = 4;
            this.btnSave.Text = global::ALARm.alerts.btn_save;
            this.btnSave.UseSelectable = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnCancel.ForeColor = System.Drawing.SystemColors.Control;
            this.btnCancel.Location = new System.Drawing.Point(299, 0);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(7, 6, 7, 6);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(102, 31);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = global::ALARm.alerts.btn_cancel;
            this.btnCancel.UseSelectable = true;
            // 
            // metroPanel3
            // 
            this.metroPanel3.Controls.Add(this.mdtDate);
            this.metroPanel3.Controls.Add(this.metroLabel2);
            this.metroPanel3.Dock = System.Windows.Forms.DockStyle.Top;
            this.metroPanel3.HorizontalScrollbarBarColor = true;
            this.metroPanel3.HorizontalScrollbarHighlightOnWheel = false;
            this.metroPanel3.HorizontalScrollbarSize = 10;
            this.metroPanel3.Location = new System.Drawing.Point(13, 294);
            this.metroPanel3.Margin = new System.Windows.Forms.Padding(0);
            this.metroPanel3.Name = "metroPanel3";
            this.metroPanel3.Padding = new System.Windows.Forms.Padding(5);
            this.metroPanel3.Size = new System.Drawing.Size(401, 39);
            this.metroPanel3.TabIndex = 23;
            this.metroPanel3.VerticalScrollbarBarColor = true;
            this.metroPanel3.VerticalScrollbarHighlightOnWheel = false;
            this.metroPanel3.VerticalScrollbarSize = 10;
            // 
            // mdtDate
            // 
            this.mdtDate.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mdtDate.Location = new System.Drawing.Point(145, 5);
            this.mdtDate.MinimumSize = new System.Drawing.Size(0, 29);
            this.mdtDate.Name = "mdtDate";
            this.mdtDate.Size = new System.Drawing.Size(251, 29);
            this.mdtDate.TabIndex = 2;
            // 
            // metroLabel2
            // 
            this.metroLabel2.Dock = System.Windows.Forms.DockStyle.Left;
            this.metroLabel2.Location = new System.Drawing.Point(5, 5);
            this.metroLabel2.Name = "metroLabel2";
            this.metroLabel2.Size = new System.Drawing.Size(140, 29);
            this.metroLabel2.TabIndex = 1;
            this.metroLabel2.Text = "Дата";
            this.metroLabel2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // mpFinalM
            // 
            this.mpFinalM.Controls.Add(this.speedControl);
            this.mpFinalM.Controls.Add(this.metroLabel4);
            this.mpFinalM.Dock = System.Windows.Forms.DockStyle.Top;
            this.mpFinalM.HorizontalScrollbarBarColor = true;
            this.mpFinalM.HorizontalScrollbarHighlightOnWheel = false;
            this.mpFinalM.HorizontalScrollbarSize = 10;
            this.mpFinalM.Location = new System.Drawing.Point(13, 333);
            this.mpFinalM.Margin = new System.Windows.Forms.Padding(0);
            this.mpFinalM.Name = "mpFinalM";
            this.mpFinalM.Padding = new System.Windows.Forms.Padding(5);
            this.mpFinalM.Size = new System.Drawing.Size(401, 39);
            this.mpFinalM.TabIndex = 24;
            this.mpFinalM.VerticalScrollbarBarColor = true;
            this.mpFinalM.VerticalScrollbarHighlightOnWheel = false;
            this.mpFinalM.VerticalScrollbarSize = 10;
            // 
            // metroLabel4
            // 
            this.metroLabel4.Dock = System.Windows.Forms.DockStyle.Left;
            this.metroLabel4.Location = new System.Drawing.Point(5, 5);
            this.metroLabel4.Name = "metroLabel4";
            this.metroLabel4.Size = new System.Drawing.Size(140, 29);
            this.metroLabel4.TabIndex = 1;
            this.metroLabel4.Text = "Скорость";
            this.metroLabel4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // speedControl
            // 
            this.speedControl.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.speedControl.Increment = new decimal(new int[] {
            5,
            0,
            0,
            0});
            this.speedControl.Location = new System.Drawing.Point(145, 6);
            this.speedControl.Maximum = new decimal(new int[] {
            180,
            0,
            0,
            0});
            this.speedControl.Name = "speedControl";
            this.speedControl.Size = new System.Drawing.Size(251, 26);
            this.speedControl.TabIndex = 2;
            // 
            // cbAccept
            // 
            this.cbAccept.CurrentId = -1;
            this.cbAccept.Dock = System.Windows.Forms.DockStyle.Top;
            this.cbAccept.Location = new System.Drawing.Point(13, 255);
            this.cbAccept.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.cbAccept.Name = "cbAccept";
            this.cbAccept.Size = new System.Drawing.Size(401, 39);
            this.cbAccept.TabIndex = 22;
            this.cbAccept.Title = "Статус принятия";
            this.cbAccept.TitleWidth = 140;
            this.cbAccept.UseSelectable = true;
            // 
            // cbType
            // 
            this.cbType.CurrentId = -1;
            this.cbType.Dock = System.Windows.Forms.DockStyle.Top;
            this.cbType.Location = new System.Drawing.Point(13, 216);
            this.cbType.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.cbType.Name = "cbType";
            this.cbType.Size = new System.Drawing.Size(401, 39);
            this.cbType.TabIndex = 17;
            this.cbType.Title = "Тип ремонта";
            this.cbType.TitleWidth = 140;
            this.cbType.UseSelectable = true;
            // 
            // coordControl
            // 
            this.coordControl.AutoSize = true;
            this.coordControl.Dock = System.Windows.Forms.DockStyle.Top;
            this.coordControl.FinalCoordsHidden = false;
            this.coordControl.FinalKm = 0;
            this.coordControl.FinalKmTitle = "Конец (км)";
            this.coordControl.FinalM = 0;
            this.coordControl.FinalMTitle = "Конец (м)";
            this.coordControl.Location = new System.Drawing.Point(13, 60);
            this.coordControl.MetersHidden = false;
            this.coordControl.Name = "coordControl";
            this.coordControl.Size = new System.Drawing.Size(401, 156);
            this.coordControl.StartKm = 0;
            this.coordControl.StartKmTitle = "Начало (км)";
            this.coordControl.StartM = 0;
            this.coordControl.StartMTitle = "Начало (м)";
            this.coordControl.TabIndex = 16;
            this.coordControl.TitleWidth = 140;
            this.coordControl.UseSelectable = true;
            // 
            // RepairProjectForm
            // 
            this.AcceptButton = this.btnSave;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(427, 433);
            this.Controls.Add(this.mpFinalM);
            this.Controls.Add(this.metroPanel3);
            this.Controls.Add(this.cbAccept);
            this.Controls.Add(this.cbType);
            this.Controls.Add(this.coordControl);
            this.Controls.Add(this.metroPanel2);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximizeBox = false;
            this.Name = "RepairProjectForm";
            this.Padding = new System.Windows.Forms.Padding(13, 60, 13, 13);
            this.Text = "Добавление записи";
            this.metroPanel2.ResumeLayout(false);
            this.metroPanel3.ResumeLayout(false);
            this.mpFinalM.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.speedControl)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MetroFramework.Controls.MetroPanel metroPanel2;
        private MetroFramework.Controls.MetroButton btnSave;
        private MetroFramework.Controls.MetroButton btnCancel;
        private CoordControl coordControl;
        private CatalogListBox cbType;
        private CatalogListBox cbAccept;
        private MetroFramework.Controls.MetroPanel metroPanel3;
        private MetroFramework.Controls.MetroDateTime mdtDate;
        private MetroFramework.Controls.MetroLabel metroLabel2;
        private MetroFramework.Controls.MetroPanel mpFinalM;
        private System.Windows.Forms.NumericUpDown speedControl;
        private MetroFramework.Controls.MetroLabel metroLabel4;
    }
}