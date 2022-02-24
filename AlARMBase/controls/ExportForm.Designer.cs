namespace ALARm.controls
{
    partial class ExportForm
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
            this.metroPanel3 = new MetroFramework.Controls.MetroPanel();
            this.btnCancel = new MetroFramework.Controls.MetroButton();
            this.btnSave = new MetroFramework.Controls.MetroButton();
            this.lbRoad = new ALARm.controls.AdmListBox();
            this.metroPanel6 = new MetroFramework.Controls.MetroPanel();
            this.mdtDate = new MetroFramework.Controls.MetroDateTime();
            this.metroLabel5 = new MetroFramework.Controls.MetroLabel();
            this.metroPanel3.SuspendLayout();
            this.metroPanel6.SuspendLayout();
            this.SuspendLayout();
            // 
            // metroPanel3
            // 
            this.metroPanel3.Controls.Add(this.btnCancel);
            this.metroPanel3.Controls.Add(this.btnSave);
            this.metroPanel3.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.metroPanel3.HorizontalScrollbarBarColor = true;
            this.metroPanel3.HorizontalScrollbarHighlightOnWheel = false;
            this.metroPanel3.HorizontalScrollbarSize = 10;
            this.metroPanel3.Location = new System.Drawing.Point(20, 152);
            this.metroPanel3.Margin = new System.Windows.Forms.Padding(3, 20, 3, 3);
            this.metroPanel3.MinimumSize = new System.Drawing.Size(0, 37);
            this.metroPanel3.Name = "metroPanel3";
            this.metroPanel3.Padding = new System.Windows.Forms.Padding(3);
            this.metroPanel3.Size = new System.Drawing.Size(338, 37);
            this.metroPanel3.TabIndex = 10;
            this.metroPanel3.VerticalScrollbarBarColor = true;
            this.metroPanel3.VerticalScrollbarHighlightOnWheel = false;
            this.metroPanel3.VerticalScrollbarSize = 10;
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(252, 6);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "Отмена";
            this.btnCancel.UseSelectable = true;
            this.btnCancel.Click += new System.EventHandler(this.BtnCancel_Click);
            // 
            // btnSave
            // 
            this.btnSave.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.btnSave.Location = new System.Drawing.Point(171, 6);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 4;
            this.btnSave.Text = "Сохранить";
            this.btnSave.UseSelectable = true;
            this.btnSave.Click += new System.EventHandler(this.BtnSave_Click);
            // 
            // lbRoad
            // 
            this.lbRoad.AutoSize = true;
            this.lbRoad.CurrentCode = "";
            this.lbRoad.CurrentId = ((long)(-1));
            this.lbRoad.CurrentValue = "";
            this.lbRoad.DisplayMember = "Name";
            this.lbRoad.Dock = System.Windows.Forms.DockStyle.Top;
            this.lbRoad.Location = new System.Drawing.Point(20, 60);
            this.lbRoad.MinimumSize = new System.Drawing.Size(338, 39);
            this.lbRoad.Name = "lbRoad";
            this.lbRoad.Size = new System.Drawing.Size(338, 39);
            this.lbRoad.TabIndex = 11;
            this.lbRoad.Title = "Дорога";
            this.lbRoad.TitleWidth = 110;
            this.lbRoad.UseSelectable = true;
            // 
            // metroPanel6
            // 
            this.metroPanel6.Controls.Add(this.mdtDate);
            this.metroPanel6.Controls.Add(this.metroLabel5);
            this.metroPanel6.Dock = System.Windows.Forms.DockStyle.Top;
            this.metroPanel6.HorizontalScrollbarBarColor = true;
            this.metroPanel6.HorizontalScrollbarHighlightOnWheel = false;
            this.metroPanel6.HorizontalScrollbarSize = 10;
            this.metroPanel6.Location = new System.Drawing.Point(20, 99);
            this.metroPanel6.Name = "metroPanel6";
            this.metroPanel6.Size = new System.Drawing.Size(338, 37);
            this.metroPanel6.TabIndex = 14;
            this.metroPanel6.VerticalScrollbarBarColor = true;
            this.metroPanel6.VerticalScrollbarHighlightOnWheel = false;
            this.metroPanel6.VerticalScrollbarSize = 10;
            // 
            // mdtDate
            // 
            this.mdtDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.mdtDate.Location = new System.Drawing.Point(118, 4);
            this.mdtDate.MinimumSize = new System.Drawing.Size(0, 29);
            this.mdtDate.Name = "mdtDate";
            this.mdtDate.Size = new System.Drawing.Size(212, 29);
            this.mdtDate.TabIndex = 4;
            // 
            // metroLabel5
            // 
            this.metroLabel5.AutoSize = true;
            this.metroLabel5.Location = new System.Drawing.Point(39, 8);
            this.metroLabel5.Name = "metroLabel5";
            this.metroLabel5.Size = new System.Drawing.Size(37, 19);
            this.metroLabel5.TabIndex = 3;
            this.metroLabel5.Text = "Дата";
            // 
            // ExportForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(378, 209);
            this.Controls.Add(this.metroPanel6);
            this.Controls.Add(this.lbRoad);
            this.Controls.Add(this.metroPanel3);
            this.Name = "ExportForm";
            this.Text = "Экспорт";
            this.metroPanel3.ResumeLayout(false);
            this.metroPanel6.ResumeLayout(false);
            this.metroPanel6.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MetroFramework.Controls.MetroPanel metroPanel3;
        private MetroFramework.Controls.MetroButton btnCancel;
        private MetroFramework.Controls.MetroButton btnSave;
        private AdmListBox lbRoad;
        private MetroFramework.Controls.MetroPanel metroPanel6;
        private MetroFramework.Controls.MetroDateTime mdtDate;
        private MetroFramework.Controls.MetroLabel metroLabel5;
    }
}