namespace ALARm.controls
{
    partial class SwitchForm
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
            this.num = new MetroFramework.Controls.MetroTextBox();
            this.metroLabel4 = new MetroFramework.Controls.MetroLabel();
            this.cbPoint = new ALARm.controls.CatalogListBox();
            this.cbDir = new ALARm.controls.CatalogListBox();
            this.cbSide = new ALARm.controls.CatalogListBox();
            this.cbMark = new ALARm.controls.CatalogListBox();
            this.coordControl = new ALARm.controls.CoordControl();
            this.cbStation = new ALARm.controls.AdmListBox();
            this.metroPanel2.SuspendLayout();
            this.mpFinalM.SuspendLayout();
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
            this.metroPanel2.Location = new System.Drawing.Point(13, 381);
            this.metroPanel2.Name = "metroPanel2";
            this.metroPanel2.Size = new System.Drawing.Size(606, 31);
            this.metroPanel2.TabIndex = 13;
            this.metroPanel2.VerticalScrollbarBarColor = true;
            this.metroPanel2.VerticalScrollbarHighlightOnWheel = false;
            this.metroPanel2.VerticalScrollbarSize = 10;
            // 
            // btnSave
            // 
            this.btnSave.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnSave.ForeColor = System.Drawing.SystemColors.Control;
            this.btnSave.Location = new System.Drawing.Point(402, 0);
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
            this.btnCancel.Location = new System.Drawing.Point(504, 0);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(7, 6, 7, 6);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(102, 31);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = global::ALARm.alerts.btn_cancel;
            this.btnCancel.UseSelectable = true;
            // 
            // mpFinalM
            // 
            this.mpFinalM.Controls.Add(this.num);
            this.mpFinalM.Controls.Add(this.metroLabel4);
            this.mpFinalM.Dock = System.Windows.Forms.DockStyle.Top;
            this.mpFinalM.HorizontalScrollbarBarColor = true;
            this.mpFinalM.HorizontalScrollbarHighlightOnWheel = false;
            this.mpFinalM.HorizontalScrollbarSize = 10;
            this.mpFinalM.Location = new System.Drawing.Point(13, 60);
            this.mpFinalM.Margin = new System.Windows.Forms.Padding(0);
            this.mpFinalM.Name = "mpFinalM";
            this.mpFinalM.Padding = new System.Windows.Forms.Padding(5);
            this.mpFinalM.Size = new System.Drawing.Size(606, 39);
            this.mpFinalM.TabIndex = 20;
            this.mpFinalM.VerticalScrollbarBarColor = true;
            this.mpFinalM.VerticalScrollbarHighlightOnWheel = false;
            this.mpFinalM.VerticalScrollbarSize = 10;
            // 
            // num
            // 
            // 
            // 
            // 
            this.num.CustomButton.Image = null;
            this.num.CustomButton.Location = new System.Drawing.Point(428, 1);
            this.num.CustomButton.Name = "";
            this.num.CustomButton.Size = new System.Drawing.Size(27, 27);
            this.num.CustomButton.Style = MetroFramework.MetroColorStyle.Blue;
            this.num.CustomButton.TabIndex = 1;
            this.num.CustomButton.Theme = MetroFramework.MetroThemeStyle.Light;
            this.num.CustomButton.UseSelectable = true;
            this.num.CustomButton.Visible = false;
            this.num.Dock = System.Windows.Forms.DockStyle.Fill;
            this.num.Lines = new string[0];
            this.num.Location = new System.Drawing.Point(145, 5);
            this.num.MaxLength = 32767;
            this.num.Name = "num";
            this.num.PasswordChar = '\0';
            this.num.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.num.SelectedText = "";
            this.num.SelectionLength = 0;
            this.num.SelectionStart = 0;
            this.num.ShortcutsEnabled = true;
            this.num.Size = new System.Drawing.Size(456, 29);
            this.num.TabIndex = 2;
            this.num.UseSelectable = true;
            this.num.WaterMarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
            this.num.WaterMarkFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Pixel);
            // 
            // metroLabel4
            // 
            this.metroLabel4.Dock = System.Windows.Forms.DockStyle.Left;
            this.metroLabel4.Location = new System.Drawing.Point(5, 5);
            this.metroLabel4.Name = "metroLabel4";
            this.metroLabel4.Size = new System.Drawing.Size(140, 29);
            this.metroLabel4.TabIndex = 1;
            this.metroLabel4.Text = "Номер";
            this.metroLabel4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // cbPoint
            // 
            this.cbPoint.CurrentId = -1;
            this.cbPoint.Dock = System.Windows.Forms.DockStyle.Top;
            this.cbPoint.Location = new System.Drawing.Point(13, 294);
            this.cbPoint.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.cbPoint.Name = "cbPoint";
            this.cbPoint.Size = new System.Drawing.Size(606, 39);
            this.cbPoint.TabIndex = 21;
            this.cbPoint.Title = "Отображать как";
            this.cbPoint.TitleWidth = 140;
            this.cbPoint.UseSelectable = true;
            // 
            // cbDir
            // 
            this.cbDir.CurrentId = -1;
            this.cbDir.Dock = System.Windows.Forms.DockStyle.Top;
            this.cbDir.Location = new System.Drawing.Point(13, 216);
            this.cbDir.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.cbDir.Name = "cbDir";
            this.cbDir.Size = new System.Drawing.Size(606, 39);
            this.cbDir.TabIndex = 19;
            this.cbDir.Title = "Направление";
            this.cbDir.TitleWidth = 140;
            this.cbDir.UseSelectable = true;
            // 
            // cbSide
            // 
            this.cbSide.CurrentId = -1;
            this.cbSide.Dock = System.Windows.Forms.DockStyle.Top;
            this.cbSide.Location = new System.Drawing.Point(13, 255);
            this.cbSide.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.cbSide.Name = "cbSide";
            this.cbSide.Size = new System.Drawing.Size(606, 39);
            this.cbSide.TabIndex = 18;
            this.cbSide.Title = "Сторона";
            this.cbSide.TitleWidth = 140;
            this.cbSide.UseSelectable = true;
            // 
            // cbMark
            // 
            this.cbMark.CurrentId = -1;
            this.cbMark.Dock = System.Windows.Forms.DockStyle.Top;
            this.cbMark.Location = new System.Drawing.Point(13, 177);
            this.cbMark.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.cbMark.Name = "cbMark";
            this.cbMark.Size = new System.Drawing.Size(606, 39);
            this.cbMark.TabIndex = 17;
            this.cbMark.Title = "Тип";
            this.cbMark.TitleWidth = 140;
            this.cbMark.UseSelectable = true;
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
            this.coordControl.Location = new System.Drawing.Point(13, 99);
            this.coordControl.MetersHidden = false;
            this.coordControl.Name = "coordControl";
            this.coordControl.Size = new System.Drawing.Size(606, 78);
            this.coordControl.StartKm = 0;
            this.coordControl.StartKmTitle = "Км";
            this.coordControl.StartM = 0;
            this.coordControl.StartMTitle = "М";
            this.coordControl.TabIndex = 16;
            this.coordControl.TitleWidth = 140;
            this.coordControl.UseSelectable = true;
            // 
            // cbStation
            // 
            this.cbStation.AutoSize = true;
            this.cbStation.CurrentCode = "";
            this.cbStation.CurrentId = ((long)(-1));
            this.cbStation.CurrentValue = "";
            this.cbStation.DisplayMember = "Name";
            this.cbStation.Dock = System.Windows.Forms.DockStyle.Top;
            this.cbStation.Location = new System.Drawing.Point(13, 333);
            this.cbStation.MinimumSize = new System.Drawing.Size(0, 39);
            this.cbStation.Name = "cbStation";
            this.cbStation.Size = new System.Drawing.Size(606, 39);
            this.cbStation.TabIndex = 23;
            this.cbStation.Title = "Принадлежность";
            this.cbStation.TitleWidth = 140;
            this.cbStation.UseSelectable = true;
            // 
            // SwitchForm
            // 
            this.AcceptButton = this.btnSave;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(632, 425);
            this.Controls.Add(this.cbStation);
            this.Controls.Add(this.cbPoint);
            this.Controls.Add(this.cbSide);
            this.Controls.Add(this.cbDir);
            this.Controls.Add(this.cbMark);
            this.Controls.Add(this.coordControl);
            this.Controls.Add(this.mpFinalM);
            this.Controls.Add(this.metroPanel2);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximizeBox = false;
            this.Name = "SwitchForm";
            this.Padding = new System.Windows.Forms.Padding(13, 60, 13, 13);
            this.Text = "Добавление записи";
            this.metroPanel2.ResumeLayout(false);
            this.mpFinalM.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MetroFramework.Controls.MetroPanel metroPanel2;
        private MetroFramework.Controls.MetroButton btnSave;
        private MetroFramework.Controls.MetroButton btnCancel;
        private CoordControl coordControl;
        private CatalogListBox cbMark;
        private CatalogListBox cbSide;
        private CatalogListBox cbDir;
        private MetroFramework.Controls.MetroPanel mpFinalM;
        private MetroFramework.Controls.MetroTextBox num;
        private MetroFramework.Controls.MetroLabel metroLabel4;
        private CatalogListBox cbPoint;
        private AdmListBox cbStation;
    }
}