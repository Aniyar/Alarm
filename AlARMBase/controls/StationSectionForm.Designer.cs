namespace ALARm.controls
{
    partial class StationSectionForm
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
            this.mpFinalM = new MetroFramework.Controls.MetroPanel();
            this.tbAxisM = new MetroFramework.Controls.MetroTextBox();
            this.metroLabel4 = new MetroFramework.Controls.MetroLabel();
            this.mpFinalKm = new MetroFramework.Controls.MetroPanel();
            this.tbAxisKm = new MetroFramework.Controls.MetroTextBox();
            this.metroLabel3 = new MetroFramework.Controls.MetroLabel();
            this.metroPanel2 = new MetroFramework.Controls.MetroPanel();
            this.btnSave = new MetroFramework.Controls.MetroButton();
            this.btnCancel = new MetroFramework.Controls.MetroButton();
            this.catalogListPoint = new ALARm.controls.CatalogListBox();
            this.coordControl = new ALARm.controls.CoordControl();
            this.admStationListBox = new ALARm.controls.AdmListBox();
            this.admNodListBox = new ALARm.controls.AdmListBox();
            this.admRoadListBox = new ALARm.controls.AdmListBox();
            this.directionListBox = new ALARm.controls.AdmListBox();
            this.metroPanel1 = new MetroFramework.Controls.MetroPanel();
            this.mpFinalM.SuspendLayout();
            this.mpFinalKm.SuspendLayout();
            this.metroPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // mpFinalM
            // 
            this.mpFinalM.Controls.Add(this.tbAxisM);
            this.mpFinalM.Controls.Add(this.metroLabel4);
            this.mpFinalM.Dock = System.Windows.Forms.DockStyle.Top;
            this.mpFinalM.HorizontalScrollbarBarColor = true;
            this.mpFinalM.HorizontalScrollbarHighlightOnWheel = false;
            this.mpFinalM.HorizontalScrollbarSize = 10;
            this.mpFinalM.Location = new System.Drawing.Point(20, 201);
            this.mpFinalM.Margin = new System.Windows.Forms.Padding(0);
            this.mpFinalM.Name = "mpFinalM";
            this.mpFinalM.Padding = new System.Windows.Forms.Padding(5);
            this.mpFinalM.Size = new System.Drawing.Size(531, 39);
            this.mpFinalM.TabIndex = 16;
            this.mpFinalM.VerticalScrollbarBarColor = true;
            this.mpFinalM.VerticalScrollbarHighlightOnWheel = false;
            this.mpFinalM.VerticalScrollbarSize = 10;
            // 
            // tbAxisM
            // 
            // 
            // 
            // 
            this.tbAxisM.CustomButton.Image = null;
            this.tbAxisM.CustomButton.Location = new System.Drawing.Point(383, 1);
            this.tbAxisM.CustomButton.Name = "";
            this.tbAxisM.CustomButton.Size = new System.Drawing.Size(27, 27);
            this.tbAxisM.CustomButton.Style = MetroFramework.MetroColorStyle.Blue;
            this.tbAxisM.CustomButton.TabIndex = 1;
            this.tbAxisM.CustomButton.Theme = MetroFramework.MetroThemeStyle.Light;
            this.tbAxisM.CustomButton.UseSelectable = true;
            this.tbAxisM.CustomButton.Visible = false;
            this.tbAxisM.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbAxisM.Lines = new string[0];
            this.tbAxisM.Location = new System.Drawing.Point(115, 5);
            this.tbAxisM.MaxLength = 32767;
            this.tbAxisM.Name = "tbAxisM";
            this.tbAxisM.PasswordChar = '\0';
            this.tbAxisM.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.tbAxisM.SelectedText = "";
            this.tbAxisM.SelectionLength = 0;
            this.tbAxisM.SelectionStart = 0;
            this.tbAxisM.ShortcutsEnabled = true;
            this.tbAxisM.Size = new System.Drawing.Size(411, 29);
            this.tbAxisM.TabIndex = 2;
            this.tbAxisM.UseSelectable = true;
            this.tbAxisM.WaterMarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
            this.tbAxisM.WaterMarkFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Pixel);
            // 
            // metroLabel4
            // 
            this.metroLabel4.Dock = System.Windows.Forms.DockStyle.Left;
            this.metroLabel4.Location = new System.Drawing.Point(5, 5);
            this.metroLabel4.Name = "metroLabel4";
            this.metroLabel4.Size = new System.Drawing.Size(110, 29);
            this.metroLabel4.TabIndex = 1;
            this.metroLabel4.Text = "Ось (м)";
            this.metroLabel4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // mpFinalKm
            // 
            this.mpFinalKm.Controls.Add(this.tbAxisKm);
            this.mpFinalKm.Controls.Add(this.metroLabel3);
            this.mpFinalKm.Dock = System.Windows.Forms.DockStyle.Top;
            this.mpFinalKm.HorizontalScrollbarBarColor = true;
            this.mpFinalKm.HorizontalScrollbarHighlightOnWheel = false;
            this.mpFinalKm.HorizontalScrollbarSize = 10;
            this.mpFinalKm.Location = new System.Drawing.Point(20, 162);
            this.mpFinalKm.Margin = new System.Windows.Forms.Padding(0);
            this.mpFinalKm.Name = "mpFinalKm";
            this.mpFinalKm.Padding = new System.Windows.Forms.Padding(5);
            this.mpFinalKm.Size = new System.Drawing.Size(531, 39);
            this.mpFinalKm.TabIndex = 15;
            this.mpFinalKm.VerticalScrollbarBarColor = true;
            this.mpFinalKm.VerticalScrollbarHighlightOnWheel = false;
            this.mpFinalKm.VerticalScrollbarSize = 10;
            // 
            // tbAxisKm
            // 
            // 
            // 
            // 
            this.tbAxisKm.CustomButton.Image = null;
            this.tbAxisKm.CustomButton.Location = new System.Drawing.Point(383, 1);
            this.tbAxisKm.CustomButton.Name = "";
            this.tbAxisKm.CustomButton.Size = new System.Drawing.Size(27, 27);
            this.tbAxisKm.CustomButton.Style = MetroFramework.MetroColorStyle.Blue;
            this.tbAxisKm.CustomButton.TabIndex = 1;
            this.tbAxisKm.CustomButton.Theme = MetroFramework.MetroThemeStyle.Light;
            this.tbAxisKm.CustomButton.UseSelectable = true;
            this.tbAxisKm.CustomButton.Visible = false;
            this.tbAxisKm.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbAxisKm.Lines = new string[0];
            this.tbAxisKm.Location = new System.Drawing.Point(115, 5);
            this.tbAxisKm.Margin = new System.Windows.Forms.Padding(0);
            this.tbAxisKm.MaxLength = 32767;
            this.tbAxisKm.Name = "tbAxisKm";
            this.tbAxisKm.PasswordChar = '\0';
            this.tbAxisKm.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.tbAxisKm.SelectedText = "";
            this.tbAxisKm.SelectionLength = 0;
            this.tbAxisKm.SelectionStart = 0;
            this.tbAxisKm.ShortcutsEnabled = true;
            this.tbAxisKm.Size = new System.Drawing.Size(411, 29);
            this.tbAxisKm.TabIndex = 2;
            this.tbAxisKm.UseSelectable = true;
            this.tbAxisKm.WaterMarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
            this.tbAxisKm.WaterMarkFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Pixel);
            // 
            // metroLabel3
            // 
            this.metroLabel3.Dock = System.Windows.Forms.DockStyle.Left;
            this.metroLabel3.Location = new System.Drawing.Point(5, 5);
            this.metroLabel3.Name = "metroLabel3";
            this.metroLabel3.Size = new System.Drawing.Size(110, 29);
            this.metroLabel3.TabIndex = 1;
            this.metroLabel3.Text = "Ось (км)";
            this.metroLabel3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // metroPanel2
            // 
            this.metroPanel2.Controls.Add(this.btnSave);
            this.metroPanel2.Controls.Add(this.btnCancel);
            this.metroPanel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.metroPanel2.HorizontalScrollbarBarColor = true;
            this.metroPanel2.HorizontalScrollbarHighlightOnWheel = false;
            this.metroPanel2.HorizontalScrollbarSize = 15;
            this.metroPanel2.Location = new System.Drawing.Point(20, 540);
            this.metroPanel2.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.metroPanel2.Name = "metroPanel2";
            this.metroPanel2.Size = new System.Drawing.Size(531, 28);
            this.metroPanel2.TabIndex = 21;
            this.metroPanel2.VerticalScrollbarBarColor = true;
            this.metroPanel2.VerticalScrollbarHighlightOnWheel = false;
            this.metroPanel2.VerticalScrollbarSize = 15;
            // 
            // btnSave
            // 
            this.btnSave.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnSave.ForeColor = System.Drawing.SystemColors.Control;
            this.btnSave.Location = new System.Drawing.Point(225, 0);
            this.btnSave.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(153, 28);
            this.btnSave.TabIndex = 5;
            this.btnSave.Text = global::ALARm.alerts.btn_save;
            this.btnSave.UseSelectable = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnCancel.ForeColor = System.Drawing.SystemColors.Control;
            this.btnCancel.Location = new System.Drawing.Point(378, 0);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(153, 28);
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = global::ALARm.alerts.btn_cancel;
            this.btnCancel.UseSelectable = true;
            // 
            // catalogListPoint
            // 
            this.catalogListPoint.CurrentId = -1;
            this.catalogListPoint.Dock = System.Windows.Forms.DockStyle.Top;
            this.catalogListPoint.Location = new System.Drawing.Point(20, 396);
            this.catalogListPoint.Margin = new System.Windows.Forms.Padding(1);
            this.catalogListPoint.Name = "catalogListPoint";
            this.catalogListPoint.Size = new System.Drawing.Size(531, 34);
            this.catalogListPoint.TabIndex = 18;
            this.catalogListPoint.Title = "Отображать как";
            this.catalogListPoint.TitleWidth = 165;
            this.catalogListPoint.UseSelectable = true;
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
            this.coordControl.Location = new System.Drawing.Point(20, 240);
            this.coordControl.Margin = new System.Windows.Forms.Padding(6, 8, 6, 8);
            this.coordControl.MetersHidden = false;
            this.coordControl.Name = "coordControl";
            this.coordControl.Size = new System.Drawing.Size(531, 156);
            this.coordControl.StartKm = 0;
            this.coordControl.StartKmTitle = "Начало (км)";
            this.coordControl.StartM = 0;
            this.coordControl.StartMTitle = "Начало (м)";
            this.coordControl.TabIndex = 13;
            this.coordControl.TitleWidth = 110;
            this.coordControl.UseSelectable = true;
            // 
            // admStationListBox
            // 
            this.admStationListBox.AutoSize = true;
            this.admStationListBox.CurrentCode = "";
            this.admStationListBox.CurrentId = ((long)(-1));
            this.admStationListBox.CurrentValue = "";
            this.admStationListBox.DisplayMember = "Name";
            this.admStationListBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.admStationListBox.Location = new System.Drawing.Point(20, 128);
            this.admStationListBox.Margin = new System.Windows.Forms.Padding(0);
            this.admStationListBox.MinimumSize = new System.Drawing.Size(0, 34);
            this.admStationListBox.Name = "admStationListBox";
            this.admStationListBox.Size = new System.Drawing.Size(531, 34);
            this.admStationListBox.TabIndex = 12;
            this.admStationListBox.Title = "Станция";
            this.admStationListBox.TitleWidth = 110;
            this.admStationListBox.UseSelectable = true;
            this.admStationListBox.SelectionChanged += new System.EventHandler(this.admStationListBox_SelectionChanged);
            // 
            // admNodListBox
            // 
            this.admNodListBox.AutoSize = true;
            this.admNodListBox.CurrentCode = "";
            this.admNodListBox.CurrentId = ((long)(-1));
            this.admNodListBox.CurrentValue = "";
            this.admNodListBox.DisplayMember = "Name";
            this.admNodListBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.admNodListBox.Location = new System.Drawing.Point(20, 94);
            this.admNodListBox.Margin = new System.Windows.Forms.Padding(6, 8, 6, 8);
            this.admNodListBox.MinimumSize = new System.Drawing.Size(0, 34);
            this.admNodListBox.Name = "admNodListBox";
            this.admNodListBox.Size = new System.Drawing.Size(531, 34);
            this.admNodListBox.TabIndex = 11;
            this.admNodListBox.Title = "Отделение";
            this.admNodListBox.TitleWidth = 110;
            this.admNodListBox.UseSelectable = true;
            this.admNodListBox.SelectionChanged += new System.EventHandler(this.admNodListBox_SelectionChanged);
            // 
            // admRoadListBox
            // 
            this.admRoadListBox.AutoSize = true;
            this.admRoadListBox.CurrentCode = "";
            this.admRoadListBox.CurrentId = ((long)(-1));
            this.admRoadListBox.CurrentValue = "";
            this.admRoadListBox.DisplayMember = "Name";
            this.admRoadListBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.admRoadListBox.Location = new System.Drawing.Point(20, 60);
            this.admRoadListBox.Margin = new System.Windows.Forms.Padding(6, 8, 6, 8);
            this.admRoadListBox.MinimumSize = new System.Drawing.Size(0, 34);
            this.admRoadListBox.Name = "admRoadListBox";
            this.admRoadListBox.Size = new System.Drawing.Size(531, 34);
            this.admRoadListBox.TabIndex = 10;
            this.admRoadListBox.Title = "Дорога";
            this.admRoadListBox.TitleWidth = 110;
            this.admRoadListBox.UseSelectable = true;
            this.admRoadListBox.SelectionChanged += new System.EventHandler(this.admRoadListBox_SelectionChanged);
            // 
            // directionListBox
            // 
            this.directionListBox.AutoSize = true;
            this.directionListBox.CurrentCode = "";
            this.directionListBox.CurrentId = ((long)(-1));
            this.directionListBox.CurrentValue = "";
            this.directionListBox.DisplayMember = "Name";
            this.directionListBox.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.directionListBox.Location = new System.Drawing.Point(20, 496);
            this.directionListBox.Margin = new System.Windows.Forms.Padding(0);
            this.directionListBox.MinimumSize = new System.Drawing.Size(0, 34);
            this.directionListBox.Name = "directionListBox";
            this.directionListBox.Size = new System.Drawing.Size(531, 34);
            this.directionListBox.TabIndex = 22;
            this.directionListBox.Title = "Направления";
            this.directionListBox.TitleWidth = 110;
            this.directionListBox.UseSelectable = true;
            // 
            // metroPanel1
            // 
            this.metroPanel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.metroPanel1.HorizontalScrollbarBarColor = true;
            this.metroPanel1.HorizontalScrollbarHighlightOnWheel = false;
            this.metroPanel1.HorizontalScrollbarSize = 10;
            this.metroPanel1.Location = new System.Drawing.Point(20, 530);
            this.metroPanel1.MaximumSize = new System.Drawing.Size(10000, 10);
            this.metroPanel1.Name = "metroPanel1";
            this.metroPanel1.Size = new System.Drawing.Size(531, 10);
            this.metroPanel1.TabIndex = 23;
            this.metroPanel1.VerticalScrollbarBarColor = true;
            this.metroPanel1.VerticalScrollbarHighlightOnWheel = false;
            this.metroPanel1.VerticalScrollbarSize = 10;
            // 
            // StationSectionForm
            // 
            this.AcceptButton = this.btnSave;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BorderStyle = MetroFramework.Forms.MetroFormBorderStyle.FixedSingle;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(571, 588);
            this.Controls.Add(this.directionListBox);
            this.Controls.Add(this.metroPanel1);
            this.Controls.Add(this.metroPanel2);
            this.Controls.Add(this.catalogListPoint);
            this.Controls.Add(this.coordControl);
            this.Controls.Add(this.mpFinalM);
            this.Controls.Add(this.mpFinalKm);
            this.Controls.Add(this.admStationListBox);
            this.Controls.Add(this.admNodListBox);
            this.Controls.Add(this.admRoadListBox);
            this.MaximizeBox = false;
            this.Name = "StationSectionForm";
            this.Resizable = false;
            this.Text = "Добавление записи";
            this.mpFinalM.ResumeLayout(false);
            this.mpFinalKm.ResumeLayout(false);
            this.metroPanel2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private CoordControl coordControl;
        private AdmListBox admStationListBox;
        private AdmListBox admNodListBox;
        private AdmListBox admRoadListBox;
        private MetroFramework.Controls.MetroPanel mpFinalM;
        private MetroFramework.Controls.MetroTextBox tbAxisM;
        private MetroFramework.Controls.MetroLabel metroLabel4;
        private MetroFramework.Controls.MetroPanel mpFinalKm;
        private MetroFramework.Controls.MetroTextBox tbAxisKm;
        private MetroFramework.Controls.MetroLabel metroLabel3;
        private CatalogListBox catalogListPoint;
        private MetroFramework.Controls.MetroPanel metroPanel2;
        private MetroFramework.Controls.MetroButton btnSave;
        private MetroFramework.Controls.MetroButton btnCancel;
        private AdmListBox directionListBox;
        private MetroFramework.Controls.MetroPanel metroPanel1;
    }
}