namespace ALARm.controls
{
    partial class DistanceForm
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
            this.coordControl = new ALARm.controls.CoordControl();
            this.admPdbListBox = new ALARm.controls.AdmListBox();
            this.admDistanceListBox = new ALARm.controls.AdmListBox();
            this.admRoadListBox = new ALARm.controls.AdmListBox();
            this.metroPanel2 = new MetroFramework.Controls.MetroPanel();
            this.btnCancel = new MetroFramework.Controls.MetroButton();
            this.btnSave = new MetroFramework.Controls.MetroButton();
            this.metroPanel2.SuspendLayout();
            this.SuspendLayout();
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
            this.coordControl.Location = new System.Drawing.Point(20, 177);
            this.coordControl.MetersHidden = false;
            this.coordControl.Name = "coordControl";
            this.coordControl.Size = new System.Drawing.Size(417, 156);
            this.coordControl.StartKm = 0;
            this.coordControl.StartKmTitle = "Начало (км)";
            this.coordControl.StartM = 0;
            this.coordControl.StartMTitle = "Начало (м)";
            this.coordControl.TabIndex = 8;
            this.coordControl.TitleWidth = 110;
            this.coordControl.UseSelectable = true;
            // 
            // admPdbListBox
            // 
            this.admPdbListBox.AutoSize = true;
            this.admPdbListBox.CurrentId = -1;
            this.admPdbListBox.CurrentValue = "";
            this.admPdbListBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.admPdbListBox.Location = new System.Drawing.Point(20, 138);
            this.admPdbListBox.MinimumSize = new System.Drawing.Size(0, 39);
            this.admPdbListBox.Name = "admPdbListBox";
            this.admPdbListBox.Size = new System.Drawing.Size(417, 39);
            this.admPdbListBox.TabIndex = 7;
            this.admPdbListBox.Title = "Подразделение";
            this.admPdbListBox.UseSelectable = true;
            this.admPdbListBox.Visible = false;
            // 
            // admDistanceListBox
            // 
            this.admDistanceListBox.AutoSize = true;
            this.admDistanceListBox.CurrentId = -1;
            this.admDistanceListBox.CurrentValue = "";
            this.admDistanceListBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.admDistanceListBox.Location = new System.Drawing.Point(20, 99);
            this.admDistanceListBox.MinimumSize = new System.Drawing.Size(0, 39);
            this.admDistanceListBox.Name = "admDistanceListBox";
            this.admDistanceListBox.Size = new System.Drawing.Size(417, 39);
            this.admDistanceListBox.TabIndex = 5;
            this.admDistanceListBox.Title = "Дистанция";
            this.admDistanceListBox.UseSelectable = true;
            this.admDistanceListBox.SelectionChanged += new System.EventHandler(this.admDistanceListBox_SelectionChanged);
            // 
            // admRoadListBox
            // 
            this.admRoadListBox.AutoSize = true;
            this.admRoadListBox.CurrentId = -1;
            this.admRoadListBox.CurrentValue = "";
            this.admRoadListBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.admRoadListBox.Location = new System.Drawing.Point(20, 60);
            this.admRoadListBox.MinimumSize = new System.Drawing.Size(0, 39);
            this.admRoadListBox.Name = "admRoadListBox";
            this.admRoadListBox.Size = new System.Drawing.Size(417, 39);
            this.admRoadListBox.TabIndex = 4;
            this.admRoadListBox.Title = "Дорога";
            this.admRoadListBox.UseSelectable = true;
            this.admRoadListBox.SelectionChanged += new System.EventHandler(this.admRoadListBox_SelectionChanged);
            // 
            // metroPanel2
            // 
            this.metroPanel2.Controls.Add(this.btnCancel);
            this.metroPanel2.Controls.Add(this.btnSave);
            this.metroPanel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.metroPanel2.HorizontalScrollbarBarColor = true;
            this.metroPanel2.HorizontalScrollbarHighlightOnWheel = false;
            this.metroPanel2.HorizontalScrollbarSize = 10;
            this.metroPanel2.Location = new System.Drawing.Point(20, 333);
            this.metroPanel2.Name = "metroPanel2";
            this.metroPanel2.Size = new System.Drawing.Size(417, 39);
            this.metroPanel2.TabIndex = 9;
            this.metroPanel2.VerticalScrollbarBarColor = true;
            this.metroPanel2.VerticalScrollbarHighlightOnWheel = false;
            this.metroPanel2.VerticalScrollbarSize = 10;
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.ForeColor = System.Drawing.SystemColors.Control;
            this.btnCancel.Location = new System.Drawing.Point(315, 3);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(102, 29);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = global::ALARm.alerts.btn_cancel;
            this.btnCancel.UseSelectable = true;
            // 
            // btnSave
            // 
            this.btnSave.ForeColor = System.Drawing.SystemColors.Control;
            this.btnSave.Location = new System.Drawing.Point(207, 3);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(102, 29);
            this.btnSave.TabIndex = 2;
            this.btnSave.Text = global::ALARm.alerts.btn_save;
            this.btnSave.UseSelectable = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // DistanceForm
            // 
            this.AcceptButton = this.btnSave;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BorderStyle = MetroFramework.Forms.MetroFormBorderStyle.FixedSingle;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(457, 399);
            this.Controls.Add(this.metroPanel2);
            this.Controls.Add(this.coordControl);
            this.Controls.Add(this.admPdbListBox);
            this.Controls.Add(this.admDistanceListBox);
            this.Controls.Add(this.admRoadListBox);
            this.MaximizeBox = false;
            this.Name = "DistanceForm";
            this.Resizable = false;
            this.Text = "Участок дистанции";
            this.metroPanel2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private AdmListBox admRoadListBox;
        private AdmListBox admDistanceListBox;
        private AdmListBox admPdbListBox;
        private CoordControl coordControl;
        private MetroFramework.Controls.MetroPanel metroPanel2;
        private MetroFramework.Controls.MetroButton btnCancel;
        private MetroFramework.Controls.MetroButton btnSave;
    }
}