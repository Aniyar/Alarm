namespace ALARm.controls
{
    partial class CoordinateGNSSForm
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
            this.mpFinalM = new MetroFramework.Controls.MetroPanel();
            this.tbLatitude = new MetroFramework.Controls.MetroTextBox();
            this.metroLabel4 = new MetroFramework.Controls.MetroLabel();
            this.coordControl = new ALARm.controls.CoordControl();
            this.metroPanel1 = new MetroFramework.Controls.MetroPanel();
            this.tbLongtitude = new MetroFramework.Controls.MetroTextBox();
            this.metroLabel1 = new MetroFramework.Controls.MetroLabel();
            this.metroPanel3 = new MetroFramework.Controls.MetroPanel();
            this.tbAltitude = new MetroFramework.Controls.MetroTextBox();
            this.metroLabel2 = new MetroFramework.Controls.MetroLabel();
            this.metroPanel4 = new MetroFramework.Controls.MetroPanel();
            this.tbCoord = new MetroFramework.Controls.MetroTextBox();
            this.metroLabel3 = new MetroFramework.Controls.MetroLabel();
            this.metroPanel5 = new MetroFramework.Controls.MetroPanel();
            this.tbHeight = new MetroFramework.Controls.MetroTextBox();
            this.metroLabel5 = new MetroFramework.Controls.MetroLabel();
            this.metroPanel2.SuspendLayout();
            this.mpFinalM.SuspendLayout();
            this.metroPanel1.SuspendLayout();
            this.metroPanel3.SuspendLayout();
            this.metroPanel4.SuspendLayout();
            this.metroPanel5.SuspendLayout();
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
            this.metroPanel2.Location = new System.Drawing.Point(13, 342);
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
            // mpFinalM
            // 
            this.mpFinalM.Controls.Add(this.tbLatitude);
            this.mpFinalM.Controls.Add(this.metroLabel4);
            this.mpFinalM.Dock = System.Windows.Forms.DockStyle.Top;
            this.mpFinalM.HorizontalScrollbarBarColor = true;
            this.mpFinalM.HorizontalScrollbarHighlightOnWheel = false;
            this.mpFinalM.HorizontalScrollbarSize = 10;
            this.mpFinalM.Location = new System.Drawing.Point(13, 138);
            this.mpFinalM.Margin = new System.Windows.Forms.Padding(0);
            this.mpFinalM.Name = "mpFinalM";
            this.mpFinalM.Padding = new System.Windows.Forms.Padding(5);
            this.mpFinalM.Size = new System.Drawing.Size(401, 39);
            this.mpFinalM.TabIndex = 20;
            this.mpFinalM.VerticalScrollbarBarColor = true;
            this.mpFinalM.VerticalScrollbarHighlightOnWheel = false;
            this.mpFinalM.VerticalScrollbarSize = 10;
            // 
            // tbLatitude
            // 
            // 
            // 
            // 
            this.tbLatitude.CustomButton.Image = null;
            this.tbLatitude.CustomButton.Location = new System.Drawing.Point(223, 1);
            this.tbLatitude.CustomButton.Name = "";
            this.tbLatitude.CustomButton.Size = new System.Drawing.Size(27, 27);
            this.tbLatitude.CustomButton.Style = MetroFramework.MetroColorStyle.Blue;
            this.tbLatitude.CustomButton.TabIndex = 1;
            this.tbLatitude.CustomButton.Theme = MetroFramework.MetroThemeStyle.Light;
            this.tbLatitude.CustomButton.UseSelectable = true;
            this.tbLatitude.CustomButton.Visible = false;
            this.tbLatitude.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbLatitude.Lines = new string[0];
            this.tbLatitude.Location = new System.Drawing.Point(145, 5);
            this.tbLatitude.MaxLength = 32767;
            this.tbLatitude.Name = "tbLatitude";
            this.tbLatitude.PasswordChar = '\0';
            this.tbLatitude.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.tbLatitude.SelectedText = "";
            this.tbLatitude.SelectionLength = 0;
            this.tbLatitude.SelectionStart = 0;
            this.tbLatitude.ShortcutsEnabled = true;
            this.tbLatitude.Size = new System.Drawing.Size(251, 29);
            this.tbLatitude.TabIndex = 2;
            this.tbLatitude.UseSelectable = true;
            this.tbLatitude.WaterMarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
            this.tbLatitude.WaterMarkFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Pixel);
            // 
            // metroLabel4
            // 
            this.metroLabel4.Dock = System.Windows.Forms.DockStyle.Left;
            this.metroLabel4.Location = new System.Drawing.Point(5, 5);
            this.metroLabel4.Name = "metroLabel4";
            this.metroLabel4.Size = new System.Drawing.Size(140, 29);
            this.metroLabel4.TabIndex = 1;
            this.metroLabel4.Text = "Широта";
            this.metroLabel4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
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
            this.coordControl.MetersHidden = false;
            this.coordControl.Name = "coordControl";
            this.coordControl.Size = new System.Drawing.Size(401, 78);
            this.coordControl.StartKm = 0;
            this.coordControl.StartKmTitle = "Км";
            this.coordControl.StartM = 0;
            this.coordControl.StartMTitle = "М";
            this.coordControl.TabIndex = 16;
            this.coordControl.TitleWidth = 140;
            this.coordControl.UseSelectable = true;
            // 
            // metroPanel1
            // 
            this.metroPanel1.Controls.Add(this.tbLongtitude);
            this.metroPanel1.Controls.Add(this.metroLabel1);
            this.metroPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.metroPanel1.HorizontalScrollbarBarColor = true;
            this.metroPanel1.HorizontalScrollbarHighlightOnWheel = false;
            this.metroPanel1.HorizontalScrollbarSize = 10;
            this.metroPanel1.Location = new System.Drawing.Point(13, 177);
            this.metroPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.metroPanel1.Name = "metroPanel1";
            this.metroPanel1.Padding = new System.Windows.Forms.Padding(5);
            this.metroPanel1.Size = new System.Drawing.Size(401, 39);
            this.metroPanel1.TabIndex = 21;
            this.metroPanel1.VerticalScrollbarBarColor = true;
            this.metroPanel1.VerticalScrollbarHighlightOnWheel = false;
            this.metroPanel1.VerticalScrollbarSize = 10;
            // 
            // tbLongtitude
            // 
            // 
            // 
            // 
            this.tbLongtitude.CustomButton.Image = null;
            this.tbLongtitude.CustomButton.Location = new System.Drawing.Point(223, 1);
            this.tbLongtitude.CustomButton.Name = "";
            this.tbLongtitude.CustomButton.Size = new System.Drawing.Size(27, 27);
            this.tbLongtitude.CustomButton.Style = MetroFramework.MetroColorStyle.Blue;
            this.tbLongtitude.CustomButton.TabIndex = 1;
            this.tbLongtitude.CustomButton.Theme = MetroFramework.MetroThemeStyle.Light;
            this.tbLongtitude.CustomButton.UseSelectable = true;
            this.tbLongtitude.CustomButton.Visible = false;
            this.tbLongtitude.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbLongtitude.Lines = new string[0];
            this.tbLongtitude.Location = new System.Drawing.Point(145, 5);
            this.tbLongtitude.MaxLength = 32767;
            this.tbLongtitude.Name = "tbLongtitude";
            this.tbLongtitude.PasswordChar = '\0';
            this.tbLongtitude.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.tbLongtitude.SelectedText = "";
            this.tbLongtitude.SelectionLength = 0;
            this.tbLongtitude.SelectionStart = 0;
            this.tbLongtitude.ShortcutsEnabled = true;
            this.tbLongtitude.Size = new System.Drawing.Size(251, 29);
            this.tbLongtitude.TabIndex = 2;
            this.tbLongtitude.UseSelectable = true;
            this.tbLongtitude.WaterMarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
            this.tbLongtitude.WaterMarkFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Pixel);
            // 
            // metroLabel1
            // 
            this.metroLabel1.Dock = System.Windows.Forms.DockStyle.Left;
            this.metroLabel1.Location = new System.Drawing.Point(5, 5);
            this.metroLabel1.Name = "metroLabel1";
            this.metroLabel1.Size = new System.Drawing.Size(140, 29);
            this.metroLabel1.TabIndex = 1;
            this.metroLabel1.Text = "Долгота";
            this.metroLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // metroPanel3
            // 
            this.metroPanel3.Controls.Add(this.tbAltitude);
            this.metroPanel3.Controls.Add(this.metroLabel2);
            this.metroPanel3.Dock = System.Windows.Forms.DockStyle.Top;
            this.metroPanel3.HorizontalScrollbarBarColor = true;
            this.metroPanel3.HorizontalScrollbarHighlightOnWheel = false;
            this.metroPanel3.HorizontalScrollbarSize = 10;
            this.metroPanel3.Location = new System.Drawing.Point(13, 216);
            this.metroPanel3.Margin = new System.Windows.Forms.Padding(0);
            this.metroPanel3.Name = "metroPanel3";
            this.metroPanel3.Padding = new System.Windows.Forms.Padding(5);
            this.metroPanel3.Size = new System.Drawing.Size(401, 39);
            this.metroPanel3.TabIndex = 22;
            this.metroPanel3.VerticalScrollbarBarColor = true;
            this.metroPanel3.VerticalScrollbarHighlightOnWheel = false;
            this.metroPanel3.VerticalScrollbarSize = 10;
            // 
            // tbAltitude
            // 
            // 
            // 
            // 
            this.tbAltitude.CustomButton.Image = null;
            this.tbAltitude.CustomButton.Location = new System.Drawing.Point(223, 1);
            this.tbAltitude.CustomButton.Name = "";
            this.tbAltitude.CustomButton.Size = new System.Drawing.Size(27, 27);
            this.tbAltitude.CustomButton.Style = MetroFramework.MetroColorStyle.Blue;
            this.tbAltitude.CustomButton.TabIndex = 1;
            this.tbAltitude.CustomButton.Theme = MetroFramework.MetroThemeStyle.Light;
            this.tbAltitude.CustomButton.UseSelectable = true;
            this.tbAltitude.CustomButton.Visible = false;
            this.tbAltitude.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbAltitude.Lines = new string[0];
            this.tbAltitude.Location = new System.Drawing.Point(145, 5);
            this.tbAltitude.MaxLength = 32767;
            this.tbAltitude.Name = "tbAltitude";
            this.tbAltitude.PasswordChar = '\0';
            this.tbAltitude.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.tbAltitude.SelectedText = "";
            this.tbAltitude.SelectionLength = 0;
            this.tbAltitude.SelectionStart = 0;
            this.tbAltitude.ShortcutsEnabled = true;
            this.tbAltitude.Size = new System.Drawing.Size(251, 29);
            this.tbAltitude.TabIndex = 2;
            this.tbAltitude.UseSelectable = true;
            this.tbAltitude.WaterMarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
            this.tbAltitude.WaterMarkFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Pixel);
            // 
            // metroLabel2
            // 
            this.metroLabel2.Dock = System.Windows.Forms.DockStyle.Left;
            this.metroLabel2.Location = new System.Drawing.Point(5, 5);
            this.metroLabel2.Name = "metroLabel2";
            this.metroLabel2.Size = new System.Drawing.Size(140, 29);
            this.metroLabel2.TabIndex = 1;
            this.metroLabel2.Text = "Высота";
            this.metroLabel2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // metroPanel4
            // 
            this.metroPanel4.Controls.Add(this.tbCoord);
            this.metroPanel4.Controls.Add(this.metroLabel3);
            this.metroPanel4.Dock = System.Windows.Forms.DockStyle.Top;
            this.metroPanel4.HorizontalScrollbarBarColor = true;
            this.metroPanel4.HorizontalScrollbarHighlightOnWheel = false;
            this.metroPanel4.HorizontalScrollbarSize = 10;
            this.metroPanel4.Location = new System.Drawing.Point(13, 255);
            this.metroPanel4.Margin = new System.Windows.Forms.Padding(0);
            this.metroPanel4.Name = "metroPanel4";
            this.metroPanel4.Padding = new System.Windows.Forms.Padding(5);
            this.metroPanel4.Size = new System.Drawing.Size(401, 39);
            this.metroPanel4.TabIndex = 23;
            this.metroPanel4.VerticalScrollbarBarColor = true;
            this.metroPanel4.VerticalScrollbarHighlightOnWheel = false;
            this.metroPanel4.VerticalScrollbarSize = 10;
            // 
            // tbCoord
            // 
            // 
            // 
            // 
            this.tbCoord.CustomButton.Image = null;
            this.tbCoord.CustomButton.Location = new System.Drawing.Point(223, 1);
            this.tbCoord.CustomButton.Name = "";
            this.tbCoord.CustomButton.Size = new System.Drawing.Size(27, 27);
            this.tbCoord.CustomButton.Style = MetroFramework.MetroColorStyle.Blue;
            this.tbCoord.CustomButton.TabIndex = 1;
            this.tbCoord.CustomButton.Theme = MetroFramework.MetroThemeStyle.Light;
            this.tbCoord.CustomButton.UseSelectable = true;
            this.tbCoord.CustomButton.Visible = false;
            this.tbCoord.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbCoord.Lines = new string[0];
            this.tbCoord.Location = new System.Drawing.Point(145, 5);
            this.tbCoord.MaxLength = 32767;
            this.tbCoord.Name = "tbCoord";
            this.tbCoord.PasswordChar = '\0';
            this.tbCoord.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.tbCoord.SelectedText = "";
            this.tbCoord.SelectionLength = 0;
            this.tbCoord.SelectionStart = 0;
            this.tbCoord.ShortcutsEnabled = true;
            this.tbCoord.Size = new System.Drawing.Size(251, 29);
            this.tbCoord.TabIndex = 2;
            this.tbCoord.UseSelectable = true;
            this.tbCoord.WaterMarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
            this.tbCoord.WaterMarkFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Pixel);
            // 
            // metroLabel3
            // 
            this.metroLabel3.Dock = System.Windows.Forms.DockStyle.Left;
            this.metroLabel3.Location = new System.Drawing.Point(5, 5);
            this.metroLabel3.Name = "metroLabel3";
            this.metroLabel3.Size = new System.Drawing.Size(140, 29);
            this.metroLabel3.TabIndex = 1;
            this.metroLabel3.Text = "Точн.Коорд.";
            this.metroLabel3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // metroPanel5
            // 
            this.metroPanel5.Controls.Add(this.tbHeight);
            this.metroPanel5.Controls.Add(this.metroLabel5);
            this.metroPanel5.Dock = System.Windows.Forms.DockStyle.Top;
            this.metroPanel5.HorizontalScrollbarBarColor = true;
            this.metroPanel5.HorizontalScrollbarHighlightOnWheel = false;
            this.metroPanel5.HorizontalScrollbarSize = 10;
            this.metroPanel5.Location = new System.Drawing.Point(13, 294);
            this.metroPanel5.Margin = new System.Windows.Forms.Padding(0);
            this.metroPanel5.Name = "metroPanel5";
            this.metroPanel5.Padding = new System.Windows.Forms.Padding(5);
            this.metroPanel5.Size = new System.Drawing.Size(401, 39);
            this.metroPanel5.TabIndex = 24;
            this.metroPanel5.VerticalScrollbarBarColor = true;
            this.metroPanel5.VerticalScrollbarHighlightOnWheel = false;
            this.metroPanel5.VerticalScrollbarSize = 10;
            // 
            // tbHeight
            // 
            // 
            // 
            // 
            this.tbHeight.CustomButton.Image = null;
            this.tbHeight.CustomButton.Location = new System.Drawing.Point(223, 1);
            this.tbHeight.CustomButton.Name = "";
            this.tbHeight.CustomButton.Size = new System.Drawing.Size(27, 27);
            this.tbHeight.CustomButton.Style = MetroFramework.MetroColorStyle.Blue;
            this.tbHeight.CustomButton.TabIndex = 1;
            this.tbHeight.CustomButton.Theme = MetroFramework.MetroThemeStyle.Light;
            this.tbHeight.CustomButton.UseSelectable = true;
            this.tbHeight.CustomButton.Visible = false;
            this.tbHeight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbHeight.Lines = new string[0];
            this.tbHeight.Location = new System.Drawing.Point(145, 5);
            this.tbHeight.MaxLength = 32767;
            this.tbHeight.Name = "tbHeight";
            this.tbHeight.PasswordChar = '\0';
            this.tbHeight.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.tbHeight.SelectedText = "";
            this.tbHeight.SelectionLength = 0;
            this.tbHeight.SelectionStart = 0;
            this.tbHeight.ShortcutsEnabled = true;
            this.tbHeight.Size = new System.Drawing.Size(251, 29);
            this.tbHeight.TabIndex = 2;
            this.tbHeight.UseSelectable = true;
            this.tbHeight.WaterMarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
            this.tbHeight.WaterMarkFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Pixel);
            // 
            // metroLabel5
            // 
            this.metroLabel5.Dock = System.Windows.Forms.DockStyle.Left;
            this.metroLabel5.Location = new System.Drawing.Point(5, 5);
            this.metroLabel5.Name = "metroLabel5";
            this.metroLabel5.Size = new System.Drawing.Size(140, 29);
            this.metroLabel5.TabIndex = 1;
            this.metroLabel5.Text = "Точн.Выс.";
            this.metroLabel5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // CoordinateGNSSForm
            // 
            this.AcceptButton = this.btnSave;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(427, 386);
            this.Controls.Add(this.metroPanel5);
            this.Controls.Add(this.metroPanel4);
            this.Controls.Add(this.metroPanel3);
            this.Controls.Add(this.metroPanel1);
            this.Controls.Add(this.mpFinalM);
            this.Controls.Add(this.coordControl);
            this.Controls.Add(this.metroPanel2);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximizeBox = false;
            this.Name = "CoordinateGNSSForm";
            this.Padding = new System.Windows.Forms.Padding(13, 60, 13, 13);
            this.Text = "Добавление записи";
            this.metroPanel2.ResumeLayout(false);
            this.mpFinalM.ResumeLayout(false);
            this.metroPanel1.ResumeLayout(false);
            this.metroPanel3.ResumeLayout(false);
            this.metroPanel4.ResumeLayout(false);
            this.metroPanel5.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MetroFramework.Controls.MetroPanel metroPanel2;
        private MetroFramework.Controls.MetroButton btnSave;
        private MetroFramework.Controls.MetroButton btnCancel;
        private CoordControl coordControl;
        private MetroFramework.Controls.MetroPanel mpFinalM;
        private MetroFramework.Controls.MetroLabel metroLabel4;
        private MetroFramework.Controls.MetroTextBox tbLatitude;
        private MetroFramework.Controls.MetroPanel metroPanel1;
        private MetroFramework.Controls.MetroTextBox tbLongtitude;
        private MetroFramework.Controls.MetroLabel metroLabel1;
        private MetroFramework.Controls.MetroPanel metroPanel3;
        private MetroFramework.Controls.MetroTextBox tbAltitude;
        private MetroFramework.Controls.MetroLabel metroLabel2;
        private MetroFramework.Controls.MetroPanel metroPanel4;
        private MetroFramework.Controls.MetroTextBox tbCoord;
        private MetroFramework.Controls.MetroLabel metroLabel3;
        private MetroFramework.Controls.MetroPanel metroPanel5;
        private MetroFramework.Controls.MetroTextBox tbHeight;
        private MetroFramework.Controls.MetroLabel metroLabel5;
    }
}