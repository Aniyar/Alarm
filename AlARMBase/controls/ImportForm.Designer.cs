namespace ALARm.controls
{
    partial class ImportForm
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
            this.lbCode = new MetroFramework.Controls.MetroLabel();
            this.tbCode = new MetroFramework.Controls.MetroTextBox();
            this.btnSave = new MetroFramework.Controls.MetroButton();
            this.btnCancel = new MetroFramework.Controls.MetroButton();
            this.metroPanel1 = new MetroFramework.Controls.MetroPanel();
            this.metroPanel2 = new MetroFramework.Controls.MetroPanel();
            this.tbName = new MetroFramework.Controls.MetroTextBox();
            this.lbName = new MetroFramework.Controls.MetroLabel();
            this.metroPanel3 = new MetroFramework.Controls.MetroPanel();
            this.metroLabel1 = new MetroFramework.Controls.MetroLabel();
            this.metroLabel2 = new MetroFramework.Controls.MetroLabel();
            this.metroPanel4 = new MetroFramework.Controls.MetroPanel();
            this.tbCodeNOD = new MetroFramework.Controls.MetroTextBox();
            this.metroLabel3 = new MetroFramework.Controls.MetroLabel();
            this.metroPanel5 = new MetroFramework.Controls.MetroPanel();
            this.tbNameNOD = new MetroFramework.Controls.MetroTextBox();
            this.metroLabel4 = new MetroFramework.Controls.MetroLabel();
            this.metroPanel6 = new MetroFramework.Controls.MetroPanel();
            this.mdtStartDate = new MetroFramework.Controls.MetroDateTime();
            this.metroLabel5 = new MetroFramework.Controls.MetroLabel();
            this.metroPanel7 = new MetroFramework.Controls.MetroPanel();
            this.mdtFinalDate = new MetroFramework.Controls.MetroDateTime();
            this.metroLabel6 = new MetroFramework.Controls.MetroLabel();
            this.metroPanel1.SuspendLayout();
            this.metroPanel2.SuspendLayout();
            this.metroPanel3.SuspendLayout();
            this.metroPanel4.SuspendLayout();
            this.metroPanel5.SuspendLayout();
            this.metroPanel6.SuspendLayout();
            this.metroPanel7.SuspendLayout();
            this.SuspendLayout();
            // 
            // lbCode
            // 
            this.lbCode.Dock = System.Windows.Forms.DockStyle.Left;
            this.lbCode.Location = new System.Drawing.Point(3, 3);
            this.lbCode.MinimumSize = new System.Drawing.Size(102, 23);
            this.lbCode.Name = "lbCode";
            this.lbCode.Size = new System.Drawing.Size(102, 31);
            this.lbCode.TabIndex = 0;
            this.lbCode.Text = "Код";
            this.lbCode.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // tbCode
            // 
            // 
            // 
            // 
            this.tbCode.CustomButton.Image = null;
            this.tbCode.CustomButton.Location = new System.Drawing.Point(192, 1);
            this.tbCode.CustomButton.Name = "";
            this.tbCode.CustomButton.Size = new System.Drawing.Size(29, 29);
            this.tbCode.CustomButton.Style = MetroFramework.MetroColorStyle.Blue;
            this.tbCode.CustomButton.TabIndex = 1;
            this.tbCode.CustomButton.Theme = MetroFramework.MetroThemeStyle.Light;
            this.tbCode.CustomButton.UseSelectable = true;
            this.tbCode.CustomButton.Visible = false;
            this.tbCode.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbCode.Lines = new string[0];
            this.tbCode.Location = new System.Drawing.Point(105, 3);
            this.tbCode.Margin = new System.Windows.Forms.Padding(3, 0, 3, 3);
            this.tbCode.MaxLength = 32767;
            this.tbCode.Name = "tbCode";
            this.tbCode.PasswordChar = '\0';
            this.tbCode.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.tbCode.SelectedText = "";
            this.tbCode.SelectionLength = 0;
            this.tbCode.SelectionStart = 0;
            this.tbCode.ShortcutsEnabled = true;
            this.tbCode.Size = new System.Drawing.Size(222, 31);
            this.tbCode.TabIndex = 1;
            this.tbCode.UseSelectable = true;
            this.tbCode.WaterMarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
            this.tbCode.WaterMarkFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Pixel);
            this.tbCode.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbCode_KeyPress);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(171, 6);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 4;
            this.btnSave.Text = "Сохранить";
            this.btnSave.UseSelectable = true;
            this.btnSave.Click += new System.EventHandler(this.BtnSave_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(252, 6);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 5;
            this.btnCancel.Text = "Отмена";
            this.btnCancel.UseSelectable = true;
            this.btnCancel.Click += new System.EventHandler(this.BtnCancel_Click);
            // 
            // metroPanel1
            // 
            this.metroPanel1.Controls.Add(this.tbCode);
            this.metroPanel1.Controls.Add(this.lbCode);
            this.metroPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.metroPanel1.HorizontalScrollbarBarColor = true;
            this.metroPanel1.HorizontalScrollbarHighlightOnWheel = false;
            this.metroPanel1.HorizontalScrollbarSize = 10;
            this.metroPanel1.Location = new System.Drawing.Point(20, 79);
            this.metroPanel1.Margin = new System.Windows.Forms.Padding(3, 20, 3, 3);
            this.metroPanel1.MinimumSize = new System.Drawing.Size(0, 37);
            this.metroPanel1.Name = "metroPanel1";
            this.metroPanel1.Padding = new System.Windows.Forms.Padding(3);
            this.metroPanel1.Size = new System.Drawing.Size(330, 37);
            this.metroPanel1.TabIndex = 6;
            this.metroPanel1.VerticalScrollbarBarColor = true;
            this.metroPanel1.VerticalScrollbarHighlightOnWheel = false;
            this.metroPanel1.VerticalScrollbarSize = 10;
            // 
            // metroPanel2
            // 
            this.metroPanel2.Controls.Add(this.tbName);
            this.metroPanel2.Controls.Add(this.lbName);
            this.metroPanel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.metroPanel2.HorizontalScrollbarBarColor = true;
            this.metroPanel2.HorizontalScrollbarHighlightOnWheel = false;
            this.metroPanel2.HorizontalScrollbarSize = 10;
            this.metroPanel2.Location = new System.Drawing.Point(20, 116);
            this.metroPanel2.Margin = new System.Windows.Forms.Padding(3, 20, 3, 3);
            this.metroPanel2.MinimumSize = new System.Drawing.Size(0, 37);
            this.metroPanel2.Name = "metroPanel2";
            this.metroPanel2.Padding = new System.Windows.Forms.Padding(3);
            this.metroPanel2.Size = new System.Drawing.Size(330, 37);
            this.metroPanel2.TabIndex = 7;
            this.metroPanel2.VerticalScrollbarBarColor = true;
            this.metroPanel2.VerticalScrollbarHighlightOnWheel = false;
            this.metroPanel2.VerticalScrollbarSize = 10;
            // 
            // tbName
            // 
            // 
            // 
            // 
            this.tbName.CustomButton.Image = null;
            this.tbName.CustomButton.Location = new System.Drawing.Point(192, 1);
            this.tbName.CustomButton.Name = "";
            this.tbName.CustomButton.Size = new System.Drawing.Size(29, 29);
            this.tbName.CustomButton.Style = MetroFramework.MetroColorStyle.Blue;
            this.tbName.CustomButton.TabIndex = 1;
            this.tbName.CustomButton.Theme = MetroFramework.MetroThemeStyle.Light;
            this.tbName.CustomButton.UseSelectable = true;
            this.tbName.CustomButton.Visible = false;
            this.tbName.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbName.Lines = new string[0];
            this.tbName.Location = new System.Drawing.Point(105, 3);
            this.tbName.Margin = new System.Windows.Forms.Padding(3, 0, 3, 3);
            this.tbName.MaxLength = 32767;
            this.tbName.Name = "tbName";
            this.tbName.PasswordChar = '\0';
            this.tbName.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.tbName.SelectedText = "";
            this.tbName.SelectionLength = 0;
            this.tbName.SelectionStart = 0;
            this.tbName.ShortcutsEnabled = true;
            this.tbName.Size = new System.Drawing.Size(222, 31);
            this.tbName.TabIndex = 1;
            this.tbName.UseSelectable = true;
            this.tbName.WaterMarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
            this.tbName.WaterMarkFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Pixel);
            // 
            // lbName
            // 
            this.lbName.Dock = System.Windows.Forms.DockStyle.Left;
            this.lbName.Location = new System.Drawing.Point(3, 3);
            this.lbName.MinimumSize = new System.Drawing.Size(102, 23);
            this.lbName.Name = "lbName";
            this.lbName.Size = new System.Drawing.Size(102, 31);
            this.lbName.TabIndex = 0;
            this.lbName.Text = "Название";
            this.lbName.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // metroPanel3
            // 
            this.metroPanel3.Controls.Add(this.btnCancel);
            this.metroPanel3.Controls.Add(this.btnSave);
            this.metroPanel3.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.metroPanel3.HorizontalScrollbarBarColor = true;
            this.metroPanel3.HorizontalScrollbarHighlightOnWheel = false;
            this.metroPanel3.HorizontalScrollbarSize = 10;
            this.metroPanel3.Location = new System.Drawing.Point(20, 333);
            this.metroPanel3.Margin = new System.Windows.Forms.Padding(3, 20, 3, 3);
            this.metroPanel3.MinimumSize = new System.Drawing.Size(0, 37);
            this.metroPanel3.Name = "metroPanel3";
            this.metroPanel3.Padding = new System.Windows.Forms.Padding(3);
            this.metroPanel3.Size = new System.Drawing.Size(330, 37);
            this.metroPanel3.TabIndex = 9;
            this.metroPanel3.VerticalScrollbarBarColor = true;
            this.metroPanel3.VerticalScrollbarHighlightOnWheel = false;
            this.metroPanel3.VerticalScrollbarSize = 10;
            // 
            // metroLabel1
            // 
            this.metroLabel1.AutoSize = true;
            this.metroLabel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.metroLabel1.Location = new System.Drawing.Point(20, 60);
            this.metroLabel1.Name = "metroLabel1";
            this.metroLabel1.Size = new System.Drawing.Size(53, 19);
            this.metroLabel1.TabIndex = 10;
            this.metroLabel1.Text = "Дорога";
            this.metroLabel1.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // metroLabel2
            // 
            this.metroLabel2.AutoSize = true;
            this.metroLabel2.Dock = System.Windows.Forms.DockStyle.Top;
            this.metroLabel2.Location = new System.Drawing.Point(20, 153);
            this.metroLabel2.Name = "metroLabel2";
            this.metroLabel2.Size = new System.Drawing.Size(38, 19);
            this.metroLabel2.TabIndex = 11;
            this.metroLabel2.Text = "НОД";
            // 
            // metroPanel4
            // 
            this.metroPanel4.Controls.Add(this.tbCodeNOD);
            this.metroPanel4.Controls.Add(this.metroLabel3);
            this.metroPanel4.Dock = System.Windows.Forms.DockStyle.Top;
            this.metroPanel4.HorizontalScrollbarBarColor = true;
            this.metroPanel4.HorizontalScrollbarHighlightOnWheel = false;
            this.metroPanel4.HorizontalScrollbarSize = 10;
            this.metroPanel4.Location = new System.Drawing.Point(20, 172);
            this.metroPanel4.Margin = new System.Windows.Forms.Padding(3, 20, 3, 3);
            this.metroPanel4.MinimumSize = new System.Drawing.Size(0, 37);
            this.metroPanel4.Name = "metroPanel4";
            this.metroPanel4.Padding = new System.Windows.Forms.Padding(3);
            this.metroPanel4.Size = new System.Drawing.Size(330, 37);
            this.metroPanel4.TabIndex = 12;
            this.metroPanel4.VerticalScrollbarBarColor = true;
            this.metroPanel4.VerticalScrollbarHighlightOnWheel = false;
            this.metroPanel4.VerticalScrollbarSize = 10;
            // 
            // tbCodeNOD
            // 
            // 
            // 
            // 
            this.tbCodeNOD.CustomButton.Image = null;
            this.tbCodeNOD.CustomButton.Location = new System.Drawing.Point(192, 1);
            this.tbCodeNOD.CustomButton.Name = "";
            this.tbCodeNOD.CustomButton.Size = new System.Drawing.Size(29, 29);
            this.tbCodeNOD.CustomButton.Style = MetroFramework.MetroColorStyle.Blue;
            this.tbCodeNOD.CustomButton.TabIndex = 1;
            this.tbCodeNOD.CustomButton.Theme = MetroFramework.MetroThemeStyle.Light;
            this.tbCodeNOD.CustomButton.UseSelectable = true;
            this.tbCodeNOD.CustomButton.Visible = false;
            this.tbCodeNOD.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbCodeNOD.Lines = new string[0];
            this.tbCodeNOD.Location = new System.Drawing.Point(105, 3);
            this.tbCodeNOD.Margin = new System.Windows.Forms.Padding(3, 0, 3, 3);
            this.tbCodeNOD.MaxLength = 32767;
            this.tbCodeNOD.Name = "tbCodeNOD";
            this.tbCodeNOD.PasswordChar = '\0';
            this.tbCodeNOD.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.tbCodeNOD.SelectedText = "";
            this.tbCodeNOD.SelectionLength = 0;
            this.tbCodeNOD.SelectionStart = 0;
            this.tbCodeNOD.ShortcutsEnabled = true;
            this.tbCodeNOD.Size = new System.Drawing.Size(222, 31);
            this.tbCodeNOD.TabIndex = 1;
            this.tbCodeNOD.UseSelectable = true;
            this.tbCodeNOD.WaterMarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
            this.tbCodeNOD.WaterMarkFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Pixel);
            this.tbCodeNOD.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.tbCode_KeyPress);
            // 
            // metroLabel3
            // 
            this.metroLabel3.Dock = System.Windows.Forms.DockStyle.Left;
            this.metroLabel3.Location = new System.Drawing.Point(3, 3);
            this.metroLabel3.MinimumSize = new System.Drawing.Size(102, 23);
            this.metroLabel3.Name = "metroLabel3";
            this.metroLabel3.Size = new System.Drawing.Size(102, 31);
            this.metroLabel3.TabIndex = 0;
            this.metroLabel3.Text = "Код";
            this.metroLabel3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // metroPanel5
            // 
            this.metroPanel5.Controls.Add(this.tbNameNOD);
            this.metroPanel5.Controls.Add(this.metroLabel4);
            this.metroPanel5.Dock = System.Windows.Forms.DockStyle.Top;
            this.metroPanel5.HorizontalScrollbarBarColor = true;
            this.metroPanel5.HorizontalScrollbarHighlightOnWheel = false;
            this.metroPanel5.HorizontalScrollbarSize = 10;
            this.metroPanel5.Location = new System.Drawing.Point(20, 209);
            this.metroPanel5.Margin = new System.Windows.Forms.Padding(3, 20, 3, 3);
            this.metroPanel5.MinimumSize = new System.Drawing.Size(0, 37);
            this.metroPanel5.Name = "metroPanel5";
            this.metroPanel5.Padding = new System.Windows.Forms.Padding(3);
            this.metroPanel5.Size = new System.Drawing.Size(330, 37);
            this.metroPanel5.TabIndex = 8;
            this.metroPanel5.VerticalScrollbarBarColor = true;
            this.metroPanel5.VerticalScrollbarHighlightOnWheel = false;
            this.metroPanel5.VerticalScrollbarSize = 10;
            // 
            // tbNameNOD
            // 
            // 
            // 
            // 
            this.tbNameNOD.CustomButton.Image = null;
            this.tbNameNOD.CustomButton.Location = new System.Drawing.Point(192, 1);
            this.tbNameNOD.CustomButton.Name = "";
            this.tbNameNOD.CustomButton.Size = new System.Drawing.Size(29, 29);
            this.tbNameNOD.CustomButton.Style = MetroFramework.MetroColorStyle.Blue;
            this.tbNameNOD.CustomButton.TabIndex = 1;
            this.tbNameNOD.CustomButton.Theme = MetroFramework.MetroThemeStyle.Light;
            this.tbNameNOD.CustomButton.UseSelectable = true;
            this.tbNameNOD.CustomButton.Visible = false;
            this.tbNameNOD.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbNameNOD.Lines = new string[0];
            this.tbNameNOD.Location = new System.Drawing.Point(105, 3);
            this.tbNameNOD.Margin = new System.Windows.Forms.Padding(3, 0, 3, 3);
            this.tbNameNOD.MaxLength = 32767;
            this.tbNameNOD.Name = "tbNameNOD";
            this.tbNameNOD.PasswordChar = '\0';
            this.tbNameNOD.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.tbNameNOD.SelectedText = "";
            this.tbNameNOD.SelectionLength = 0;
            this.tbNameNOD.SelectionStart = 0;
            this.tbNameNOD.ShortcutsEnabled = true;
            this.tbNameNOD.Size = new System.Drawing.Size(222, 31);
            this.tbNameNOD.TabIndex = 1;
            this.tbNameNOD.UseSelectable = true;
            this.tbNameNOD.WaterMarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
            this.tbNameNOD.WaterMarkFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Pixel);
            // 
            // metroLabel4
            // 
            this.metroLabel4.Dock = System.Windows.Forms.DockStyle.Left;
            this.metroLabel4.Location = new System.Drawing.Point(3, 3);
            this.metroLabel4.MinimumSize = new System.Drawing.Size(102, 23);
            this.metroLabel4.Name = "metroLabel4";
            this.metroLabel4.Size = new System.Drawing.Size(102, 31);
            this.metroLabel4.TabIndex = 0;
            this.metroLabel4.Text = "Название";
            this.metroLabel4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // metroPanel6
            // 
            this.metroPanel6.Controls.Add(this.mdtStartDate);
            this.metroPanel6.Controls.Add(this.metroLabel5);
            this.metroPanel6.Dock = System.Windows.Forms.DockStyle.Top;
            this.metroPanel6.HorizontalScrollbarBarColor = true;
            this.metroPanel6.HorizontalScrollbarHighlightOnWheel = false;
            this.metroPanel6.HorizontalScrollbarSize = 10;
            this.metroPanel6.Location = new System.Drawing.Point(20, 246);
            this.metroPanel6.Name = "metroPanel6";
            this.metroPanel6.Size = new System.Drawing.Size(330, 37);
            this.metroPanel6.TabIndex = 13;
            this.metroPanel6.VerticalScrollbarBarColor = true;
            this.metroPanel6.VerticalScrollbarHighlightOnWheel = false;
            this.metroPanel6.VerticalScrollbarSize = 10;
            // 
            // mdtStartDate
            // 
            this.mdtStartDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.mdtStartDate.Location = new System.Drawing.Point(115, 4);
            this.mdtStartDate.MinimumSize = new System.Drawing.Size(0, 29);
            this.mdtStartDate.Name = "mdtStartDate";
            this.mdtStartDate.Size = new System.Drawing.Size(200, 29);
            this.mdtStartDate.TabIndex = 4;
            // 
            // metroLabel5
            // 
            this.metroLabel5.AutoSize = true;
            this.metroLabel5.Location = new System.Drawing.Point(15, 9);
            this.metroLabel5.Name = "metroLabel5";
            this.metroLabel5.Size = new System.Drawing.Size(84, 19);
            this.metroLabel5.TabIndex = 3;
            this.metroLabel5.Text = "Дата начала";
            // 
            // metroPanel7
            // 
            this.metroPanel7.Controls.Add(this.mdtFinalDate);
            this.metroPanel7.Controls.Add(this.metroLabel6);
            this.metroPanel7.Dock = System.Windows.Forms.DockStyle.Top;
            this.metroPanel7.HorizontalScrollbarBarColor = true;
            this.metroPanel7.HorizontalScrollbarHighlightOnWheel = false;
            this.metroPanel7.HorizontalScrollbarSize = 10;
            this.metroPanel7.Location = new System.Drawing.Point(20, 283);
            this.metroPanel7.Name = "metroPanel7";
            this.metroPanel7.Size = new System.Drawing.Size(330, 37);
            this.metroPanel7.TabIndex = 14;
            this.metroPanel7.VerticalScrollbarBarColor = true;
            this.metroPanel7.VerticalScrollbarHighlightOnWheel = false;
            this.metroPanel7.VerticalScrollbarSize = 10;
            // 
            // mdtFinalDate
            // 
            this.mdtFinalDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.mdtFinalDate.Location = new System.Drawing.Point(115, 4);
            this.mdtFinalDate.MinimumSize = new System.Drawing.Size(0, 29);
            this.mdtFinalDate.Name = "mdtFinalDate";
            this.mdtFinalDate.Size = new System.Drawing.Size(200, 29);
            this.mdtFinalDate.TabIndex = 5;
            // 
            // metroLabel6
            // 
            this.metroLabel6.AutoSize = true;
            this.metroLabel6.Location = new System.Drawing.Point(15, 9);
            this.metroLabel6.Name = "metroLabel6";
            this.metroLabel6.Size = new System.Drawing.Size(78, 19);
            this.metroLabel6.TabIndex = 4;
            this.metroLabel6.Text = "Дата конца";
            // 
            // ImportForm
            // 
            this.AcceptButton = this.btnSave;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = MetroFramework.Forms.MetroFormBorderStyle.FixedSingle;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(370, 390);
            this.Controls.Add(this.metroPanel7);
            this.Controls.Add(this.metroPanel3);
            this.Controls.Add(this.metroPanel6);
            this.Controls.Add(this.metroPanel5);
            this.Controls.Add(this.metroPanel4);
            this.Controls.Add(this.metroLabel2);
            this.Controls.Add(this.metroPanel2);
            this.Controls.Add(this.metroPanel1);
            this.Controls.Add(this.metroLabel1);
            this.ForeColor = System.Drawing.SystemColors.ControlText;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.Name = "ImportForm";
            this.Resizable = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Style = MetroFramework.MetroColorStyle.Default;
            this.Text = "Импорт";
            this.metroPanel1.ResumeLayout(false);
            this.metroPanel2.ResumeLayout(false);
            this.metroPanel3.ResumeLayout(false);
            this.metroPanel4.ResumeLayout(false);
            this.metroPanel5.ResumeLayout(false);
            this.metroPanel6.ResumeLayout(false);
            this.metroPanel6.PerformLayout();
            this.metroPanel7.ResumeLayout(false);
            this.metroPanel7.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MetroFramework.Controls.MetroLabel lbCode;
        private MetroFramework.Controls.MetroTextBox tbCode;
        private MetroFramework.Controls.MetroButton btnSave;
        private MetroFramework.Controls.MetroButton btnCancel;
        private MetroFramework.Controls.MetroPanel metroPanel1;
        private MetroFramework.Controls.MetroPanel metroPanel2;
        private MetroFramework.Controls.MetroTextBox tbName;
        private MetroFramework.Controls.MetroLabel lbName;
        private MetroFramework.Controls.MetroPanel metroPanel3;
        private MetroFramework.Controls.MetroLabel metroLabel1;
        private MetroFramework.Controls.MetroLabel metroLabel2;
        private MetroFramework.Controls.MetroPanel metroPanel4;
        private MetroFramework.Controls.MetroTextBox tbCodeNOD;
        private MetroFramework.Controls.MetroLabel metroLabel3;
        private MetroFramework.Controls.MetroPanel metroPanel5;
        private MetroFramework.Controls.MetroTextBox tbNameNOD;
        private MetroFramework.Controls.MetroLabel metroLabel4;
        private MetroFramework.Controls.MetroPanel metroPanel6;
        private MetroFramework.Controls.MetroPanel metroPanel7;
        private MetroFramework.Controls.MetroDateTime mdtStartDate;
        private MetroFramework.Controls.MetroLabel metroLabel5;
        private MetroFramework.Controls.MetroDateTime mdtFinalDate;
        private MetroFramework.Controls.MetroLabel metroLabel6;
    }
}