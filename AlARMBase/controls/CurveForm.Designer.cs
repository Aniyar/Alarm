namespace ALARm.controls
{
    partial class CurveForm
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
            this.catalogListSide = new ALARm.controls.CatalogListBox();
            this.mpLevel = new MetroFramework.Controls.MetroPanel();
            this.tbLevel = new MetroFramework.Controls.MetroTextBox();
            this.metroLabel1 = new MetroFramework.Controls.MetroLabel();
            this.metroPanel2.SuspendLayout();
            this.mpLevel.SuspendLayout();
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
            this.metroPanel2.Location = new System.Drawing.Point(20, 377);
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
            this.btnSave.TabIndex = 5;
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
            this.btnCancel.TabIndex = 4;
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
            this.LevelCoord.Location = new System.Drawing.Point(20, 254);
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
            this.LevelCoord.Visible = false;
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
            this.MainCoord.Location = new System.Drawing.Point(20, 98);
            this.MainCoord.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.MainCoord.MetersHidden = false;
            this.MainCoord.Name = "MainCoord";
            this.MainCoord.Size = new System.Drawing.Size(531, 156);
            this.MainCoord.StartKm = 0;
            this.MainCoord.StartKmTitle = "Начало (км)";
            this.MainCoord.StartM = 0;
            this.MainCoord.StartMTitle = "Начало (м)";
            this.MainCoord.TabIndex = 2;
            this.MainCoord.TitleWidth = 120;
            this.MainCoord.UseSelectable = true;
            // 
            // catalogListSide
            // 
            this.catalogListSide.CurrentId = -1;
            this.catalogListSide.Dock = System.Windows.Forms.DockStyle.Top;
            this.catalogListSide.Location = new System.Drawing.Point(20, 60);
            this.catalogListSide.Margin = new System.Windows.Forms.Padding(1);
            this.catalogListSide.MinimumSize = new System.Drawing.Size(0, 38);
            this.catalogListSide.Name = "catalogListSide";
            this.catalogListSide.Size = new System.Drawing.Size(531, 38);
            this.catalogListSide.TabIndex = 1;
            this.catalogListSide.Title = "Сторона";
            this.catalogListSide.TitleWidth = 120;
            this.catalogListSide.UseSelectable = true;
            // 
            // mpLevel
            // 
            this.mpLevel.Controls.Add(this.tbLevel);
            this.mpLevel.Controls.Add(this.metroLabel1);
            this.mpLevel.Dock = System.Windows.Forms.DockStyle.Top;
            this.mpLevel.HorizontalScrollbarBarColor = true;
            this.mpLevel.HorizontalScrollbarHighlightOnWheel = false;
            this.mpLevel.HorizontalScrollbarSize = 10;
            this.mpLevel.Location = new System.Drawing.Point(20, 332);
            this.mpLevel.Margin = new System.Windows.Forms.Padding(0);
            this.mpLevel.Name = "mpLevel";
            this.mpLevel.Padding = new System.Windows.Forms.Padding(5);
            this.mpLevel.Size = new System.Drawing.Size(531, 39);
            this.mpLevel.TabIndex = 4;
            this.mpLevel.VerticalScrollbarBarColor = true;
            this.mpLevel.VerticalScrollbarHighlightOnWheel = false;
            this.mpLevel.VerticalScrollbarSize = 10;
            this.mpLevel.Visible = false;
            // 
            // tbLevel
            // 
            // 
            // 
            // 
            this.tbLevel.CustomButton.Image = null;
            this.tbLevel.CustomButton.Location = new System.Drawing.Point(373, 1);
            this.tbLevel.CustomButton.Name = "";
            this.tbLevel.CustomButton.Size = new System.Drawing.Size(27, 27);
            this.tbLevel.CustomButton.Style = MetroFramework.MetroColorStyle.Blue;
            this.tbLevel.CustomButton.TabIndex = 1;
            this.tbLevel.CustomButton.Theme = MetroFramework.MetroThemeStyle.Light;
            this.tbLevel.CustomButton.UseSelectable = true;
            this.tbLevel.CustomButton.Visible = false;
            this.tbLevel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbLevel.Lines = new string[0];
            this.tbLevel.Location = new System.Drawing.Point(125, 5);
            this.tbLevel.MaxLength = 32767;
            this.tbLevel.Name = "tbLevel";
            this.tbLevel.PasswordChar = '\0';
            this.tbLevel.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.tbLevel.SelectedText = "";
            this.tbLevel.SelectionLength = 0;
            this.tbLevel.SelectionStart = 0;
            this.tbLevel.ShortcutsEnabled = true;
            this.tbLevel.Size = new System.Drawing.Size(401, 29);
            this.tbLevel.TabIndex = 2;
            this.tbLevel.UseSelectable = true;
            this.tbLevel.WaterMarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
            this.tbLevel.WaterMarkFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Pixel);
            // 
            // metroLabel1
            // 
            this.metroLabel1.Dock = System.Windows.Forms.DockStyle.Left;
            this.metroLabel1.Location = new System.Drawing.Point(5, 5);
            this.metroLabel1.Name = "metroLabel1";
            this.metroLabel1.Size = new System.Drawing.Size(120, 29);
            this.metroLabel1.TabIndex = 1;
            this.metroLabel1.Text = "Уровень";
            this.metroLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // CurveForm
            // 
            this.AcceptButton = this.btnSave;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.BorderStyle = MetroFramework.Forms.MetroFormBorderStyle.FixedSingle;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(571, 435);
            this.Controls.Add(this.mpLevel);
            this.Controls.Add(this.LevelCoord);
            this.Controls.Add(this.MainCoord);
            this.Controls.Add(this.metroPanel2);
            this.Controls.Add(this.catalogListSide);
            this.MaximizeBox = false;
            this.Name = "CurveForm";
            this.Resizable = false;
            this.Text = "Добавление записи";
            this.metroPanel2.ResumeLayout(false);
            this.mpLevel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private MetroFramework.Controls.MetroPanel metroPanel2;
        private MetroFramework.Controls.MetroButton btnSave;
        private MetroFramework.Controls.MetroButton btnCancel;
        private CatalogListBox catalogListSide;
        private CoordControl MainCoord;
        private CoordControl LevelCoord;
        private MetroFramework.Controls.MetroPanel mpLevel;
        private MetroFramework.Controls.MetroTextBox tbLevel;
        private MetroFramework.Controls.MetroLabel metroLabel1;
    }
}