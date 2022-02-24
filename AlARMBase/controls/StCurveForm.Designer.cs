namespace ALARm.controls
{
    partial class StCurveForm
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
            this.LevelCoord = new ALARm.controls.CoordControl();
            this.MainCoord = new ALARm.controls.CoordControl();
            this.mpRadius = new MetroFramework.Controls.MetroPanel();
            this.tbRadius = new MetroFramework.Controls.MetroTextBox();
            this.mlFinalM = new MetroFramework.Controls.MetroLabel();
            this.mpWear = new MetroFramework.Controls.MetroPanel();
            this.tbWear = new MetroFramework.Controls.MetroTextBox();
            this.metroLabel2 = new MetroFramework.Controls.MetroLabel();
            this.catalogWidth = new ALARm.controls.CatalogListBox();
            this.metroPanel2.SuspendLayout();
            this.mpRadius.SuspendLayout();
            this.mpWear.SuspendLayout();
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
            this.metroPanel2.Location = new System.Drawing.Point(20, 420);
            this.metroPanel2.MinimumSize = new System.Drawing.Size(0, 38);
            this.metroPanel2.Name = "metroPanel2";
            this.metroPanel2.Size = new System.Drawing.Size(531, 38);
            this.metroPanel2.TabIndex = 30;
            this.metroPanel2.VerticalScrollbarBarColor = true;
            this.metroPanel2.VerticalScrollbarHighlightOnWheel = false;
            this.metroPanel2.VerticalScrollbarSize = 10;
            // 
            // btnSave
            // 
            this.btnSave.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnSave.ForeColor = System.Drawing.SystemColors.Control;
            this.btnSave.Location = new System.Drawing.Point(327, 0);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(102, 38);
            this.btnSave.TabIndex = 6;
            this.btnSave.Text = global::ALARm.alerts.btn_save;
            this.btnSave.UseSelectable = true;
            this.btnSave.Click += new System.EventHandler(this.BtnSave_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnCancel.ForeColor = System.Drawing.SystemColors.Control;
            this.btnCancel.Location = new System.Drawing.Point(429, 0);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(102, 38);
            this.btnCancel.TabIndex = 7;
            this.btnCancel.Text = global::ALARm.alerts.btn_cancel;
            this.btnCancel.UseSelectable = true;
            this.btnCancel.Click += new System.EventHandler(this.BtnCancel_Click);
            // 
            // LevelCoord
            // 
            this.LevelCoord.AutoSize = true;
            this.LevelCoord.Dock = System.Windows.Forms.DockStyle.Top;
            this.LevelCoord.FinalCoordsHidden = true;
            this.LevelCoord.FinalKm = 0;
            this.LevelCoord.FinalKmTitle = "";
            this.LevelCoord.FinalM = 0;
            this.LevelCoord.FinalMTitle = "";
            this.LevelCoord.Location = new System.Drawing.Point(20, 255);
            this.LevelCoord.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.LevelCoord.MetersHidden = false;
            this.LevelCoord.Name = "LevelCoord";
            this.LevelCoord.Size = new System.Drawing.Size(531, 78);
            this.LevelCoord.StartKm = 0;
            this.LevelCoord.StartKmTitle = "Переходная 1";
            this.LevelCoord.StartM = 0;
            this.LevelCoord.StartMTitle = "Переходная 2";
            this.LevelCoord.TabIndex = 3;
            this.LevelCoord.TitleWidth = 120;
            this.LevelCoord.UseSelectable = true;
            // 
            // MainCoord
            // 
            this.MainCoord.AutoSize = true;
            this.MainCoord.Dock = System.Windows.Forms.DockStyle.Top;
            this.MainCoord.FinalCoordsHidden = false;
            this.MainCoord.FinalKm = 0;
            this.MainCoord.FinalKmTitle = "Конец (км)";
            this.MainCoord.FinalM = 0;
            this.MainCoord.FinalMTitle = "Конец (м)";
            this.MainCoord.Location = new System.Drawing.Point(20, 60);
            this.MainCoord.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.MainCoord.MetersHidden = false;
            this.MainCoord.Name = "MainCoord";
            this.MainCoord.Size = new System.Drawing.Size(531, 156);
            this.MainCoord.StartKm = 0;
            this.MainCoord.StartKmTitle = "Начало (км)";
            this.MainCoord.StartM = 0;
            this.MainCoord.StartMTitle = "Начало (м)";
            this.MainCoord.TabIndex = 1;
            this.MainCoord.TitleWidth = 120;
            this.MainCoord.UseSelectable = true;
            // 
            // mpRadius
            // 
            this.mpRadius.Controls.Add(this.tbRadius);
            this.mpRadius.Controls.Add(this.mlFinalM);
            this.mpRadius.Dock = System.Windows.Forms.DockStyle.Top;
            this.mpRadius.HorizontalScrollbarBarColor = true;
            this.mpRadius.HorizontalScrollbarHighlightOnWheel = false;
            this.mpRadius.HorizontalScrollbarSize = 10;
            this.mpRadius.Location = new System.Drawing.Point(20, 216);
            this.mpRadius.Margin = new System.Windows.Forms.Padding(0);
            this.mpRadius.Name = "mpRadius";
            this.mpRadius.Padding = new System.Windows.Forms.Padding(5);
            this.mpRadius.Size = new System.Drawing.Size(531, 39);
            this.mpRadius.TabIndex = 2;
            this.mpRadius.VerticalScrollbarBarColor = true;
            this.mpRadius.VerticalScrollbarHighlightOnWheel = false;
            this.mpRadius.VerticalScrollbarSize = 10;
            // 
            // tbRadius
            // 
            // 
            // 
            // 
            this.tbRadius.CustomButton.Image = null;
            this.tbRadius.CustomButton.Location = new System.Drawing.Point(373, 1);
            this.tbRadius.CustomButton.Name = "";
            this.tbRadius.CustomButton.Size = new System.Drawing.Size(27, 27);
            this.tbRadius.CustomButton.Style = MetroFramework.MetroColorStyle.Blue;
            this.tbRadius.CustomButton.TabIndex = 1;
            this.tbRadius.CustomButton.Theme = MetroFramework.MetroThemeStyle.Light;
            this.tbRadius.CustomButton.UseSelectable = true;
            this.tbRadius.CustomButton.Visible = false;
            this.tbRadius.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbRadius.Lines = new string[0];
            this.tbRadius.Location = new System.Drawing.Point(125, 5);
            this.tbRadius.MaxLength = 32767;
            this.tbRadius.Name = "tbRadius";
            this.tbRadius.PasswordChar = '\0';
            this.tbRadius.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.tbRadius.SelectedText = "";
            this.tbRadius.SelectionLength = 0;
            this.tbRadius.SelectionStart = 0;
            this.tbRadius.ShortcutsEnabled = true;
            this.tbRadius.Size = new System.Drawing.Size(401, 29);
            this.tbRadius.TabIndex = 2;
            this.tbRadius.UseSelectable = true;
            this.tbRadius.WaterMarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
            this.tbRadius.WaterMarkFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Pixel);
            this.tbRadius.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.DigitKeyFilter);
            // 
            // mlFinalM
            // 
            this.mlFinalM.Dock = System.Windows.Forms.DockStyle.Left;
            this.mlFinalM.Location = new System.Drawing.Point(5, 5);
            this.mlFinalM.Name = "mlFinalM";
            this.mlFinalM.Size = new System.Drawing.Size(120, 29);
            this.mlFinalM.TabIndex = 1;
            this.mlFinalM.Text = "Радиус";
            this.mlFinalM.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // mpWear
            // 
            this.mpWear.Controls.Add(this.tbWear);
            this.mpWear.Controls.Add(this.metroLabel2);
            this.mpWear.Dock = System.Windows.Forms.DockStyle.Top;
            this.mpWear.HorizontalScrollbarBarColor = true;
            this.mpWear.HorizontalScrollbarHighlightOnWheel = false;
            this.mpWear.HorizontalScrollbarSize = 10;
            this.mpWear.Location = new System.Drawing.Point(20, 371);
            this.mpWear.Margin = new System.Windows.Forms.Padding(0);
            this.mpWear.Name = "mpWear";
            this.mpWear.Padding = new System.Windows.Forms.Padding(5);
            this.mpWear.Size = new System.Drawing.Size(531, 39);
            this.mpWear.TabIndex = 5;
            this.mpWear.VerticalScrollbarBarColor = true;
            this.mpWear.VerticalScrollbarHighlightOnWheel = false;
            this.mpWear.VerticalScrollbarSize = 10;
            // 
            // tbWear
            // 
            // 
            // 
            // 
            this.tbWear.CustomButton.Image = null;
            this.tbWear.CustomButton.Location = new System.Drawing.Point(373, 1);
            this.tbWear.CustomButton.Name = "";
            this.tbWear.CustomButton.Size = new System.Drawing.Size(27, 27);
            this.tbWear.CustomButton.Style = MetroFramework.MetroColorStyle.Blue;
            this.tbWear.CustomButton.TabIndex = 1;
            this.tbWear.CustomButton.Theme = MetroFramework.MetroThemeStyle.Light;
            this.tbWear.CustomButton.UseSelectable = true;
            this.tbWear.CustomButton.Visible = false;
            this.tbWear.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbWear.Lines = new string[0];
            this.tbWear.Location = new System.Drawing.Point(125, 5);
            this.tbWear.MaxLength = 32767;
            this.tbWear.Name = "tbWear";
            this.tbWear.PasswordChar = '\0';
            this.tbWear.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.tbWear.SelectedText = "";
            this.tbWear.SelectionLength = 0;
            this.tbWear.SelectionStart = 0;
            this.tbWear.ShortcutsEnabled = true;
            this.tbWear.Size = new System.Drawing.Size(401, 29);
            this.tbWear.TabIndex = 2;
            this.tbWear.UseSelectable = true;
            this.tbWear.WaterMarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
            this.tbWear.WaterMarkFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Pixel);
            this.tbWear.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.DigitKeyFilter);
            // 
            // metroLabel2
            // 
            this.metroLabel2.Dock = System.Windows.Forms.DockStyle.Left;
            this.metroLabel2.Location = new System.Drawing.Point(5, 5);
            this.metroLabel2.Name = "metroLabel2";
            this.metroLabel2.Size = new System.Drawing.Size(120, 29);
            this.metroLabel2.TabIndex = 5;
            this.metroLabel2.Text = "Износ";
            this.metroLabel2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // catalogWidth
            // 
            this.catalogWidth.CurrentId = -1;
            this.catalogWidth.Dock = System.Windows.Forms.DockStyle.Top;
            this.catalogWidth.Location = new System.Drawing.Point(20, 333);
            this.catalogWidth.Margin = new System.Windows.Forms.Padding(1);
            this.catalogWidth.MinimumSize = new System.Drawing.Size(0, 38);
            this.catalogWidth.Name = "catalogWidth";
            this.catalogWidth.Size = new System.Drawing.Size(531, 38);
            this.catalogWidth.TabIndex = 4;
            this.catalogWidth.Title = "Ширина колеи";
            this.catalogWidth.TitleWidth = 120;
            this.catalogWidth.UseSelectable = true;
            // 
            // StCurveForm
            // 
            this.AcceptButton = this.btnSave;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BorderStyle = MetroFramework.Forms.MetroFormBorderStyle.FixedSingle;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(571, 478);
            this.Controls.Add(this.mpWear);
            this.Controls.Add(this.catalogWidth);
            this.Controls.Add(this.LevelCoord);
            this.Controls.Add(this.mpRadius);
            this.Controls.Add(this.MainCoord);
            this.Controls.Add(this.metroPanel2);
            this.MaximizeBox = false;
            this.Name = "StCurveForm";
            this.Resizable = false;
            this.Text = "Добавление записи";
            this.metroPanel2.ResumeLayout(false);
            this.mpRadius.ResumeLayout(false);
            this.mpWear.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private MetroFramework.Controls.MetroPanel metroPanel2;
        private MetroFramework.Controls.MetroButton btnSave;
        private MetroFramework.Controls.MetroButton btnCancel;
        private CoordControl MainCoord;
        private CoordControl LevelCoord;
        private MetroFramework.Controls.MetroPanel mpRadius;
        private MetroFramework.Controls.MetroTextBox tbRadius;
        private MetroFramework.Controls.MetroLabel mlFinalM;
        private MetroFramework.Controls.MetroPanel mpWear;
        private MetroFramework.Controls.MetroTextBox tbWear;
        private MetroFramework.Controls.MetroLabel metroLabel2;
        private CatalogListBox catalogWidth;
    }
}