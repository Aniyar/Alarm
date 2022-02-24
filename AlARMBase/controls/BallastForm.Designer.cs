namespace ALARm.controls
{
    partial class BallastForm
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
            this.coordControl = new ALARm.controls.CoordControl();
            this.mpStartKm = new MetroFramework.Controls.MetroPanel();
            this.tbBallast = new MetroFramework.Controls.MetroTextBox();
            this.mlTraffic = new MetroFramework.Controls.MetroLabel();
            this.metroPanel2.SuspendLayout();
            this.mpStartKm.SuspendLayout();
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
            this.metroPanel2.Location = new System.Drawing.Point(13, 271);
            this.metroPanel2.Name = "metroPanel2";
            this.metroPanel2.Size = new System.Drawing.Size(331, 31);
            this.metroPanel2.TabIndex = 7;
            this.metroPanel2.VerticalScrollbarBarColor = true;
            this.metroPanel2.VerticalScrollbarHighlightOnWheel = false;
            this.metroPanel2.VerticalScrollbarSize = 10;
            // 
            // btnSave
            // 
            this.btnSave.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnSave.ForeColor = System.Drawing.SystemColors.Control;
            this.btnSave.Location = new System.Drawing.Point(127, 0);
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
            this.btnCancel.Location = new System.Drawing.Point(229, 0);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(7, 6, 7, 6);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(102, 31);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = global::ALARm.alerts.btn_cancel;
            this.btnCancel.UseSelectable = true;
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
            this.coordControl.Location = new System.Drawing.Point(13, 60);
            this.coordControl.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.coordControl.MetersHidden = false;
            this.coordControl.Name = "coordControl";
            this.coordControl.Size = new System.Drawing.Size(331, 156);
            this.coordControl.StartKm = 0;
            this.coordControl.StartKmTitle = "Начало (км)";
            this.coordControl.StartM = 0;
            this.coordControl.StartMTitle = "Начало (м)";
            this.coordControl.TabIndex = 5;
            this.coordControl.TitleWidth = 110;
            this.coordControl.UseSelectable = true;
            // 
            // mpStartKm
            // 
            this.mpStartKm.Controls.Add(this.tbBallast);
            this.mpStartKm.Controls.Add(this.mlTraffic);
            this.mpStartKm.Dock = System.Windows.Forms.DockStyle.Top;
            this.mpStartKm.HorizontalScrollbarBarColor = true;
            this.mpStartKm.HorizontalScrollbarHighlightOnWheel = false;
            this.mpStartKm.HorizontalScrollbarSize = 10;
            this.mpStartKm.Location = new System.Drawing.Point(13, 216);
            this.mpStartKm.Margin = new System.Windows.Forms.Padding(0);
            this.mpStartKm.Name = "mpStartKm";
            this.mpStartKm.Padding = new System.Windows.Forms.Padding(5);
            this.mpStartKm.Size = new System.Drawing.Size(331, 39);
            this.mpStartKm.TabIndex = 8;
            this.mpStartKm.VerticalScrollbarBarColor = true;
            this.mpStartKm.VerticalScrollbarHighlightOnWheel = false;
            this.mpStartKm.VerticalScrollbarSize = 10;
            // 
            // tbBallast
            // 
            // 
            // 
            // 
            this.tbBallast.CustomButton.Image = null;
            this.tbBallast.CustomButton.Location = new System.Drawing.Point(183, 1);
            this.tbBallast.CustomButton.Name = "";
            this.tbBallast.CustomButton.Size = new System.Drawing.Size(27, 27);
            this.tbBallast.CustomButton.Style = MetroFramework.MetroColorStyle.Blue;
            this.tbBallast.CustomButton.TabIndex = 1;
            this.tbBallast.CustomButton.Theme = MetroFramework.MetroThemeStyle.Light;
            this.tbBallast.CustomButton.UseSelectable = true;
            this.tbBallast.CustomButton.Visible = false;
            this.tbBallast.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbBallast.Lines = new string[0];
            this.tbBallast.Location = new System.Drawing.Point(115, 5);
            this.tbBallast.MaxLength = 32767;
            this.tbBallast.Name = "tbBallast";
            this.tbBallast.PasswordChar = '\0';
            this.tbBallast.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.tbBallast.SelectedText = "";
            this.tbBallast.SelectionLength = 0;
            this.tbBallast.SelectionStart = 0;
            this.tbBallast.ShortcutsEnabled = true;
            this.tbBallast.Size = new System.Drawing.Size(211, 29);
            this.tbBallast.TabIndex = 2;
            this.tbBallast.UseSelectable = true;
            this.tbBallast.WaterMarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
            this.tbBallast.WaterMarkFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Pixel);
            // 
            // mlTraffic
            // 
            this.mlTraffic.Dock = System.Windows.Forms.DockStyle.Left;
            this.mlTraffic.Location = new System.Drawing.Point(5, 5);
            this.mlTraffic.Name = "mlTraffic";
            this.mlTraffic.Size = new System.Drawing.Size(110, 29);
            this.mlTraffic.TabIndex = 1;
            this.mlTraffic.Text = "Род балласта";
            this.mlTraffic.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // BallastForm
            // 
            this.AcceptButton = this.btnSave;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(357, 315);
            this.Controls.Add(this.mpStartKm);
            this.Controls.Add(this.metroPanel2);
            this.Controls.Add(this.coordControl);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximizeBox = false;
            this.Name = "BallastForm";
            this.Padding = new System.Windows.Forms.Padding(13, 60, 13, 13);
            this.Text = "Добавление записи";
            this.metroPanel2.ResumeLayout(false);
            this.mpStartKm.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MetroFramework.Controls.MetroPanel metroPanel2;
        private MetroFramework.Controls.MetroButton btnSave;
        private MetroFramework.Controls.MetroButton btnCancel;
        private CoordControl coordControl;
        private MetroFramework.Controls.MetroPanel mpStartKm;
        private MetroFramework.Controls.MetroTextBox tbBallast;
        private MetroFramework.Controls.MetroLabel mlTraffic;
    }
}