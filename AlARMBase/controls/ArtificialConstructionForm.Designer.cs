namespace ALARm.controls
{
    partial class ArtificialConstructionForm
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
            this.catalogListBox = new ALARm.controls.CatalogListBox();
            this.coordControl = new ALARm.controls.CoordControl();
            this.mpLen = new MetroFramework.Controls.MetroPanel();
            this.tbLen = new MetroFramework.Controls.MetroTextBox();
            this.mlTraffic = new MetroFramework.Controls.MetroLabel();
            this.metroPanel2.SuspendLayout();
            this.mpLen.SuspendLayout();
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
            this.metroPanel2.Location = new System.Drawing.Point(13, 226);
            this.metroPanel2.Name = "metroPanel2";
            this.metroPanel2.Size = new System.Drawing.Size(331, 31);
            this.metroPanel2.TabIndex = 10;
            this.metroPanel2.VerticalScrollbarBarColor = true;
            this.metroPanel2.VerticalScrollbarHighlightOnWheel = false;
            this.metroPanel2.VerticalScrollbarSize = 10;
            // 
            // btnSave
            // 
            this.btnSave.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnSave.ForeColor = System.Drawing.SystemColors.Control;
            this.btnSave.Location = new System.Drawing.Point(132, 0);
            this.btnSave.Margin = new System.Windows.Forms.Padding(7, 6, 7, 6);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(98, 31);
            this.btnSave.TabIndex = 4;
            this.btnSave.Text = global::ALARm.alerts.btn_save;
            this.btnSave.UseSelectable = true;
            this.btnSave.Click += new System.EventHandler(this.BtnSave_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnCancel.ForeColor = System.Drawing.SystemColors.Control;
            this.btnCancel.Location = new System.Drawing.Point(230, 0);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(7, 6, 7, 6);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(101, 31);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = global::ALARm.alerts.btn_cancel;
            this.btnCancel.UseSelectable = true;
            this.btnCancel.Click += new System.EventHandler(this.BtnCancel_Click);
            // 
            // catalogListBox
            // 
            this.catalogListBox.CurrentId = -1;
            this.catalogListBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.catalogListBox.Location = new System.Drawing.Point(13, 177);
            this.catalogListBox.Margin = new System.Windows.Forms.Padding(1);
            this.catalogListBox.Name = "catalogListBox";
            this.catalogListBox.Size = new System.Drawing.Size(331, 39);
            this.catalogListBox.TabIndex = 3;
            this.catalogListBox.Title = "Тип";
            this.catalogListBox.TitleWidth = 110;
            this.catalogListBox.UseSelectable = true;
            // 
            // coordControl
            // 
            this.coordControl.AutoSize = true;
            this.coordControl.Dock = System.Windows.Forms.DockStyle.Top;
            this.coordControl.FinalCoordsHidden = true;
            this.coordControl.FinalKm = 0;
            this.coordControl.FinalKmTitle = "Конец (км)";
            this.coordControl.FinalM = 0;
            this.coordControl.FinalMTitle = "Конец (м)";
            this.coordControl.Location = new System.Drawing.Point(13, 60);
            this.coordControl.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.coordControl.MetersHidden = false;
            this.coordControl.Name = "coordControl";
            this.coordControl.Size = new System.Drawing.Size(331, 78);
            this.coordControl.StartKm = 0;
            this.coordControl.StartKmTitle = "Км";
            this.coordControl.StartM = 0;
            this.coordControl.StartMTitle = "Метр";
            this.coordControl.TabIndex = 1;
            this.coordControl.TitleWidth = 110;
            this.coordControl.UseSelectable = true;
            // 
            // mpLen
            // 
            this.mpLen.Controls.Add(this.tbLen);
            this.mpLen.Controls.Add(this.mlTraffic);
            this.mpLen.Dock = System.Windows.Forms.DockStyle.Top;
            this.mpLen.HorizontalScrollbarBarColor = true;
            this.mpLen.HorizontalScrollbarHighlightOnWheel = false;
            this.mpLen.HorizontalScrollbarSize = 10;
            this.mpLen.Location = new System.Drawing.Point(13, 138);
            this.mpLen.Margin = new System.Windows.Forms.Padding(0);
            this.mpLen.Name = "mpLen";
            this.mpLen.Padding = new System.Windows.Forms.Padding(5);
            this.mpLen.Size = new System.Drawing.Size(331, 39);
            this.mpLen.TabIndex = 2;
            this.mpLen.VerticalScrollbarBarColor = true;
            this.mpLen.VerticalScrollbarHighlightOnWheel = false;
            this.mpLen.VerticalScrollbarSize = 10;
            // 
            // tbLen
            // 
            // 
            // 
            // 
            this.tbLen.CustomButton.Image = null;
            this.tbLen.CustomButton.Location = new System.Drawing.Point(183, 1);
            this.tbLen.CustomButton.Name = "";
            this.tbLen.CustomButton.Size = new System.Drawing.Size(27, 27);
            this.tbLen.CustomButton.Style = MetroFramework.MetroColorStyle.Blue;
            this.tbLen.CustomButton.TabIndex = 1;
            this.tbLen.CustomButton.Theme = MetroFramework.MetroThemeStyle.Light;
            this.tbLen.CustomButton.UseSelectable = true;
            this.tbLen.CustomButton.Visible = false;
            this.tbLen.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbLen.Lines = new string[0];
            this.tbLen.Location = new System.Drawing.Point(115, 5);
            this.tbLen.MaxLength = 32767;
            this.tbLen.Name = "tbLen";
            this.tbLen.PasswordChar = '\0';
            this.tbLen.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.tbLen.SelectedText = "";
            this.tbLen.SelectionLength = 0;
            this.tbLen.SelectionStart = 0;
            this.tbLen.ShortcutsEnabled = true;
            this.tbLen.Size = new System.Drawing.Size(211, 29);
            this.tbLen.TabIndex = 2;
            this.tbLen.UseSelectable = true;
            this.tbLen.WaterMarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
            this.tbLen.WaterMarkFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Pixel);
            // 
            // mlTraffic
            // 
            this.mlTraffic.Dock = System.Windows.Forms.DockStyle.Left;
            this.mlTraffic.Location = new System.Drawing.Point(5, 5);
            this.mlTraffic.Name = "mlTraffic";
            this.mlTraffic.Size = new System.Drawing.Size(110, 29);
            this.mlTraffic.TabIndex = 2;
            this.mlTraffic.Text = "Длина";
            this.mlTraffic.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ArtificialConstructionForm
            // 
            this.AcceptButton = this.btnSave;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(357, 270);
            this.Controls.Add(this.metroPanel2);
            this.Controls.Add(this.catalogListBox);
            this.Controls.Add(this.mpLen);
            this.Controls.Add(this.coordControl);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximizeBox = false;
            this.Name = "ArtificialConstructionForm";
            this.Padding = new System.Windows.Forms.Padding(13, 60, 13, 13);
            this.Text = "Добавление записи";
            this.metroPanel2.ResumeLayout(false);
            this.mpLen.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MetroFramework.Controls.MetroPanel metroPanel2;
        private MetroFramework.Controls.MetroButton btnSave;
        private MetroFramework.Controls.MetroButton btnCancel;
        private CatalogListBox catalogListBox;
        private CoordControl coordControl;
        private MetroFramework.Controls.MetroPanel mpLen;
        private MetroFramework.Controls.MetroTextBox tbLen;
        private MetroFramework.Controls.MetroLabel mlTraffic;
    }
}