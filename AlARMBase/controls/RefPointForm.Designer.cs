namespace ALARm.controls
{
    partial class RefPointForm
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
            this.tbAbscoord = new MetroFramework.Controls.MetroTextBox();
            this.metroLabel4 = new MetroFramework.Controls.MetroLabel();
            this.coordControl1 = new ALARm.controls.CoordControl();
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
            this.metroPanel2.Location = new System.Drawing.Point(13, 187);
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
            this.mpFinalM.Controls.Add(this.tbAbscoord);
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
            // tbAbscoord
            // 
            // 
            // 
            // 
            this.tbAbscoord.CustomButton.Image = null;
            this.tbAbscoord.CustomButton.Location = new System.Drawing.Point(223, 1);
            this.tbAbscoord.CustomButton.Name = "";
            this.tbAbscoord.CustomButton.Size = new System.Drawing.Size(27, 27);
            this.tbAbscoord.CustomButton.Style = MetroFramework.MetroColorStyle.Blue;
            this.tbAbscoord.CustomButton.TabIndex = 1;
            this.tbAbscoord.CustomButton.Theme = MetroFramework.MetroThemeStyle.Light;
            this.tbAbscoord.CustomButton.UseSelectable = true;
            this.tbAbscoord.CustomButton.Visible = false;
            this.tbAbscoord.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbAbscoord.Lines = new string[0];
            this.tbAbscoord.Location = new System.Drawing.Point(145, 5);
            this.tbAbscoord.MaxLength = 32767;
            this.tbAbscoord.Name = "tbAbscoord";
            this.tbAbscoord.PasswordChar = '\0';
            this.tbAbscoord.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.tbAbscoord.SelectedText = "";
            this.tbAbscoord.SelectionLength = 0;
            this.tbAbscoord.SelectionStart = 0;
            this.tbAbscoord.ShortcutsEnabled = true;
            this.tbAbscoord.Size = new System.Drawing.Size(251, 29);
            this.tbAbscoord.TabIndex = 2;
            this.tbAbscoord.UseSelectable = true;
            this.tbAbscoord.WaterMarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
            this.tbAbscoord.WaterMarkFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Pixel);
            // 
            // metroLabel4
            // 
            this.metroLabel4.Dock = System.Windows.Forms.DockStyle.Left;
            this.metroLabel4.Location = new System.Drawing.Point(5, 5);
            this.metroLabel4.Name = "metroLabel4";
            this.metroLabel4.Size = new System.Drawing.Size(140, 29);
            this.metroLabel4.TabIndex = 1;
            this.metroLabel4.Text = "Отметка, м";
            this.metroLabel4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // coordControl1
            // 
            this.coordControl1.AutoSize = true;
            this.coordControl1.Dock = System.Windows.Forms.DockStyle.Top;
            this.coordControl1.FinalCoordsHidden = true;
            this.coordControl1.FinalKm = 0;
            this.coordControl1.FinalKmTitle = "Конец (км)";
            this.coordControl1.FinalM = 0;
            this.coordControl1.FinalMTitle = "Конец (м)";
            this.coordControl1.Location = new System.Drawing.Point(13, 60);
            this.coordControl1.MetersHidden = false;
            this.coordControl1.Name = "coordControl1";
            this.coordControl1.Size = new System.Drawing.Size(401, 78);
            this.coordControl1.StartKm = 0;
            this.coordControl1.StartKmTitle = "Км";
            this.coordControl1.StartM = 0;
            this.coordControl1.StartMTitle = "Метр";
            this.coordControl1.TabIndex = 21;
            this.coordControl1.TitleWidth = 110;
            this.coordControl1.UseSelectable = true;
            // 
            // RefPointForm
            // 
            this.AcceptButton = this.btnSave;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(427, 231);
            this.Controls.Add(this.mpFinalM);
            this.Controls.Add(this.coordControl1);
            this.Controls.Add(this.metroPanel2);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximizeBox = false;
            this.Name = "RefPointForm";
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
        private MetroFramework.Controls.MetroPanel mpFinalM;
        private MetroFramework.Controls.MetroTextBox tbAbscoord;
        private MetroFramework.Controls.MetroLabel metroLabel4;
        private CoordControl coordControl1;
    }
}