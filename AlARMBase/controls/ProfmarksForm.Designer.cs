namespace ALARm.controls
{
    partial class ProfmarksForm
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
            this.tbProfil = new MetroFramework.Controls.MetroTextBox();
            this.metroLabel4 = new MetroFramework.Controls.MetroLabel();
            this.coordControl = new ALARm.controls.CoordControl();
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
            this.metroPanel2.Location = new System.Drawing.Point(13, 182);
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
            this.mpFinalM.Controls.Add(this.tbProfil);
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
            // tbProfil
            // 
            // 
            // 
            // 
            this.tbProfil.CustomButton.Image = null;
            this.tbProfil.CustomButton.Location = new System.Drawing.Point(223, 1);
            this.tbProfil.CustomButton.Name = "";
            this.tbProfil.CustomButton.Size = new System.Drawing.Size(27, 27);
            this.tbProfil.CustomButton.Style = MetroFramework.MetroColorStyle.Blue;
            this.tbProfil.CustomButton.TabIndex = 1;
            this.tbProfil.CustomButton.Theme = MetroFramework.MetroThemeStyle.Light;
            this.tbProfil.CustomButton.UseSelectable = true;
            this.tbProfil.CustomButton.Visible = false;
            this.tbProfil.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbProfil.Lines = new string[0];
            this.tbProfil.Location = new System.Drawing.Point(145, 5);
            this.tbProfil.MaxLength = 32767;
            this.tbProfil.Name = "tbProfil";
            this.tbProfil.PasswordChar = '\0';
            this.tbProfil.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.tbProfil.SelectedText = "";
            this.tbProfil.SelectionLength = 0;
            this.tbProfil.SelectionStart = 0;
            this.tbProfil.ShortcutsEnabled = true;
            this.tbProfil.Size = new System.Drawing.Size(251, 29);
            this.tbProfil.TabIndex = 2;
            this.tbProfil.UseSelectable = true;
            this.tbProfil.WaterMarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
            this.tbProfil.WaterMarkFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Pixel);
            // 
            // metroLabel4
            // 
            this.metroLabel4.Dock = System.Windows.Forms.DockStyle.Left;
            this.metroLabel4.Location = new System.Drawing.Point(5, 5);
            this.metroLabel4.Name = "metroLabel4";
            this.metroLabel4.Size = new System.Drawing.Size(140, 29);
            this.metroLabel4.TabIndex = 1;
            this.metroLabel4.Text = "Отметка (м)";
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
            // ProfmarksForm
            // 
            this.AcceptButton = this.btnSave;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(427, 226);
            this.Controls.Add(this.mpFinalM);
            this.Controls.Add(this.coordControl);
            this.Controls.Add(this.metroPanel2);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximizeBox = false;
            this.Name = "ProfmarksForm";
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
        private MetroFramework.Controls.MetroPanel mpFinalM;
        private MetroFramework.Controls.MetroLabel metroLabel4;
        private MetroFramework.Controls.MetroTextBox tbProfil;
    }
}