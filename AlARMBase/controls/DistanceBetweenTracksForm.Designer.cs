namespace ALARm.controls
{
    partial class DistanceBetweenTracksForm
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
            this.metroPanel3 = new MetroFramework.Controls.MetroPanel();
            this.tbLeftM = new MetroFramework.Controls.MetroTextBox();
            this.metroLabel2 = new MetroFramework.Controls.MetroLabel();
            this.metroPanel1 = new MetroFramework.Controls.MetroPanel();
            this.tbRightM = new MetroFramework.Controls.MetroTextBox();
            this.metroLabel1 = new MetroFramework.Controls.MetroLabel();
            this.admListBox1 = new ALARm.controls.AdmListBox();
            this.admListBox2 = new ALARm.controls.AdmListBox();
            this.metroPanel2.SuspendLayout();
            this.metroPanel3.SuspendLayout();
            this.metroPanel1.SuspendLayout();
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
            this.metroPanel2.Location = new System.Drawing.Point(13, 378);
            this.metroPanel2.Name = "metroPanel2";
            this.metroPanel2.Size = new System.Drawing.Size(297, 31);
            this.metroPanel2.TabIndex = 4;
            this.metroPanel2.VerticalScrollbarBarColor = true;
            this.metroPanel2.VerticalScrollbarHighlightOnWheel = false;
            this.metroPanel2.VerticalScrollbarSize = 10;
            // 
            // btnSave
            // 
            this.btnSave.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnSave.ForeColor = System.Drawing.SystemColors.Control;
            this.btnSave.Location = new System.Drawing.Point(93, 0);
            this.btnSave.Margin = new System.Windows.Forms.Padding(7, 6, 7, 6);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(102, 31);
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
            this.btnCancel.Location = new System.Drawing.Point(195, 0);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(7, 6, 7, 6);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(102, 31);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = global::ALARm.alerts.btn_cancel;
            this.btnCancel.UseSelectable = true;
            this.btnCancel.Click += new System.EventHandler(this.BtnCancel_Click);
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
            this.coordControl.Size = new System.Drawing.Size(297, 156);
            this.coordControl.StartKm = 0;
            this.coordControl.StartKmTitle = "Начало (км)";
            this.coordControl.StartM = 0;
            this.coordControl.StartMTitle = "Начало (м)";
            this.coordControl.TabIndex = 0;
            this.coordControl.TitleWidth = 110;
            this.coordControl.UseSelectable = true;
            // 
            // metroPanel3
            // 
            this.metroPanel3.Controls.Add(this.tbLeftM);
            this.metroPanel3.Controls.Add(this.metroLabel2);
            this.metroPanel3.Dock = System.Windows.Forms.DockStyle.Top;
            this.metroPanel3.HorizontalScrollbarBarColor = true;
            this.metroPanel3.HorizontalScrollbarHighlightOnWheel = false;
            this.metroPanel3.HorizontalScrollbarSize = 10;
            this.metroPanel3.Location = new System.Drawing.Point(13, 216);
            this.metroPanel3.Margin = new System.Windows.Forms.Padding(0);
            this.metroPanel3.Name = "metroPanel3";
            this.metroPanel3.Padding = new System.Windows.Forms.Padding(5);
            this.metroPanel3.Size = new System.Drawing.Size(297, 39);
            this.metroPanel3.TabIndex = 25;
            this.metroPanel3.VerticalScrollbarBarColor = true;
            this.metroPanel3.VerticalScrollbarHighlightOnWheel = false;
            this.metroPanel3.VerticalScrollbarSize = 10;
            // 
            // tbLeftM
            // 
            // 
            // 
            // 
            this.tbLeftM.CustomButton.Image = null;
            this.tbLeftM.CustomButton.Location = new System.Drawing.Point(147, 1);
            this.tbLeftM.CustomButton.Name = "";
            this.tbLeftM.CustomButton.Size = new System.Drawing.Size(27, 27);
            this.tbLeftM.CustomButton.Style = MetroFramework.MetroColorStyle.Blue;
            this.tbLeftM.CustomButton.TabIndex = 1;
            this.tbLeftM.CustomButton.Theme = MetroFramework.MetroThemeStyle.Light;
            this.tbLeftM.CustomButton.UseSelectable = true;
            this.tbLeftM.CustomButton.Visible = false;
            this.tbLeftM.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbLeftM.Lines = new string[0];
            this.tbLeftM.Location = new System.Drawing.Point(117, 5);
            this.tbLeftM.MaxLength = 32767;
            this.tbLeftM.Name = "tbLeftM";
            this.tbLeftM.PasswordChar = '\0';
            this.tbLeftM.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.tbLeftM.SelectedText = "";
            this.tbLeftM.SelectionLength = 0;
            this.tbLeftM.SelectionStart = 0;
            this.tbLeftM.ShortcutsEnabled = true;
            this.tbLeftM.Size = new System.Drawing.Size(175, 29);
            this.tbLeftM.TabIndex = 2;
            this.tbLeftM.UseSelectable = true;
            this.tbLeftM.WaterMarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
            this.tbLeftM.WaterMarkFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Pixel);
            // 
            // metroLabel2
            // 
            this.metroLabel2.Dock = System.Windows.Forms.DockStyle.Left;
            this.metroLabel2.Location = new System.Drawing.Point(5, 5);
            this.metroLabel2.Name = "metroLabel2";
            this.metroLabel2.Size = new System.Drawing.Size(112, 29);
            this.metroLabel2.TabIndex = 1;
            this.metroLabel2.Text = "Мп.лев.";
            this.metroLabel2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // metroPanel1
            // 
            this.metroPanel1.Controls.Add(this.tbRightM);
            this.metroPanel1.Controls.Add(this.metroLabel1);
            this.metroPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.metroPanel1.HorizontalScrollbarBarColor = true;
            this.metroPanel1.HorizontalScrollbarHighlightOnWheel = false;
            this.metroPanel1.HorizontalScrollbarSize = 10;
            this.metroPanel1.Location = new System.Drawing.Point(13, 294);
            this.metroPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.metroPanel1.Name = "metroPanel1";
            this.metroPanel1.Padding = new System.Windows.Forms.Padding(5);
            this.metroPanel1.Size = new System.Drawing.Size(297, 39);
            this.metroPanel1.TabIndex = 26;
            this.metroPanel1.VerticalScrollbarBarColor = true;
            this.metroPanel1.VerticalScrollbarHighlightOnWheel = false;
            this.metroPanel1.VerticalScrollbarSize = 10;
            // 
            // tbRightM
            // 
            // 
            // 
            // 
            this.tbRightM.CustomButton.Image = null;
            this.tbRightM.CustomButton.Location = new System.Drawing.Point(147, 1);
            this.tbRightM.CustomButton.Name = "";
            this.tbRightM.CustomButton.Size = new System.Drawing.Size(27, 27);
            this.tbRightM.CustomButton.Style = MetroFramework.MetroColorStyle.Blue;
            this.tbRightM.CustomButton.TabIndex = 1;
            this.tbRightM.CustomButton.Theme = MetroFramework.MetroThemeStyle.Light;
            this.tbRightM.CustomButton.UseSelectable = true;
            this.tbRightM.CustomButton.Visible = false;
            this.tbRightM.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbRightM.Lines = new string[0];
            this.tbRightM.Location = new System.Drawing.Point(117, 5);
            this.tbRightM.MaxLength = 32767;
            this.tbRightM.Name = "tbRightM";
            this.tbRightM.PasswordChar = '\0';
            this.tbRightM.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.tbRightM.SelectedText = "";
            this.tbRightM.SelectionLength = 0;
            this.tbRightM.SelectionStart = 0;
            this.tbRightM.ShortcutsEnabled = true;
            this.tbRightM.Size = new System.Drawing.Size(175, 29);
            this.tbRightM.TabIndex = 2;
            this.tbRightM.UseSelectable = true;
            this.tbRightM.WaterMarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
            this.tbRightM.WaterMarkFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Pixel);
            // 
            // metroLabel1
            // 
            this.metroLabel1.Dock = System.Windows.Forms.DockStyle.Left;
            this.metroLabel1.Location = new System.Drawing.Point(5, 5);
            this.metroLabel1.Name = "metroLabel1";
            this.metroLabel1.Size = new System.Drawing.Size(112, 29);
            this.metroLabel1.TabIndex = 1;
            this.metroLabel1.Text = "Мп.прав.";
            this.metroLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // admListBox1
            // 
            this.admListBox1.AutoSize = true;
            this.admListBox1.CurrentCode = "";
            this.admListBox1.CurrentId = ((long)(-1));
            this.admListBox1.CurrentValue = "";
            this.admListBox1.DisplayMember = "Name";
            this.admListBox1.Dock = System.Windows.Forms.DockStyle.Top;
            this.admListBox1.Location = new System.Drawing.Point(13, 255);
            this.admListBox1.MinimumSize = new System.Drawing.Size(0, 39);
            this.admListBox1.Name = "admListBox1";
            this.admListBox1.Size = new System.Drawing.Size(297, 39);
            this.admListBox1.TabIndex = 28;
            this.admListBox1.Title = "№ пути лев.";
            this.admListBox1.TitleWidth = 110;
            this.admListBox1.UseSelectable = true;
            // 
            // admListBox2
            // 
            this.admListBox2.AutoSize = true;
            this.admListBox2.CurrentCode = "";
            this.admListBox2.CurrentId = ((long)(-1));
            this.admListBox2.CurrentValue = "";
            this.admListBox2.DisplayMember = "Name";
            this.admListBox2.Dock = System.Windows.Forms.DockStyle.Top;
            this.admListBox2.Location = new System.Drawing.Point(13, 333);
            this.admListBox2.MinimumSize = new System.Drawing.Size(0, 39);
            this.admListBox2.Name = "admListBox2";
            this.admListBox2.Size = new System.Drawing.Size(297, 39);
            this.admListBox2.TabIndex = 29;
            this.admListBox2.Title = "№ пути прав.";
            this.admListBox2.TitleWidth = 110;
            this.admListBox2.UseSelectable = true;
            // 
            // DistanceBetweenTracksForm
            // 
            this.AcceptButton = this.btnSave;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(323, 422);
            this.Controls.Add(this.metroPanel2);
            this.Controls.Add(this.admListBox2);
            this.Controls.Add(this.metroPanel1);
            this.Controls.Add(this.admListBox1);
            this.Controls.Add(this.metroPanel3);
            this.Controls.Add(this.coordControl);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "DistanceBetweenTracksForm";
            this.Padding = new System.Windows.Forms.Padding(13, 60, 13, 13);
            this.Text = "Добавление записи";
            this.metroPanel2.ResumeLayout(false);
            this.metroPanel3.ResumeLayout(false);
            this.metroPanel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private CoordControl coordControl;
        private MetroFramework.Controls.MetroPanel metroPanel2;
        private MetroFramework.Controls.MetroButton btnSave;
        private MetroFramework.Controls.MetroButton btnCancel;
        private MetroFramework.Controls.MetroPanel metroPanel3;
        private MetroFramework.Controls.MetroTextBox tbLeftM;
        private MetroFramework.Controls.MetroLabel metroLabel2;
        private MetroFramework.Controls.MetroPanel metroPanel1;
        private MetroFramework.Controls.MetroTextBox tbRightM;
        private MetroFramework.Controls.MetroLabel metroLabel1;
        private AdmListBox admListBox1;
        private AdmListBox admListBox2;
    }
}