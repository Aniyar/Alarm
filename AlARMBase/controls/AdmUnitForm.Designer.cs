namespace ALARm.controls
{
    partial class AdmUniForm
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
            this.catalogListBox = new ALARm.controls.CatalogListBox();
            this.metroPanel1.SuspendLayout();
            this.metroPanel2.SuspendLayout();
            this.metroPanel3.SuspendLayout();
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
            this.tbCode.KeyUp += new System.Windows.Forms.KeyEventHandler(this.tbCode_KeyUp);
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
            this.metroPanel1.Location = new System.Drawing.Point(20, 60);
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
            this.metroPanel2.Location = new System.Drawing.Point(20, 97);
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
            this.metroPanel3.Location = new System.Drawing.Point(20, 228);
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
            // catalogListBox
            // 
            this.catalogListBox.CurrentId = -1;
            this.catalogListBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.catalogListBox.Location = new System.Drawing.Point(20, 134);
            this.catalogListBox.Margin = new System.Windows.Forms.Padding(2, 2, 2, 2);
            this.catalogListBox.Name = "catalogListBox";
            this.catalogListBox.Size = new System.Drawing.Size(330, 39);
            this.catalogListBox.TabIndex = 8;
            this.catalogListBox.Title = "Тип";
            this.catalogListBox.TitleWidth = 100;
            this.catalogListBox.UseSelectable = true;
            this.catalogListBox.Visible = false;
            // 
            // AdmUniForm
            // 
            this.AcceptButton = this.btnSave;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BorderStyle = MetroFramework.Forms.MetroFormBorderStyle.FixedSingle;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(370, 285);
            this.Controls.Add(this.metroPanel3);
            this.Controls.Add(this.catalogListBox);
            this.Controls.Add(this.metroPanel2);
            this.Controls.Add(this.metroPanel1);
            this.ForeColor = System.Drawing.SystemColors.ControlText;
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.Name = "AdmUniForm";
            this.Resizable = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Style = MetroFramework.MetroColorStyle.Default;
            this.Text = "Добавление записи";
            this.metroPanel1.ResumeLayout(false);
            this.metroPanel2.ResumeLayout(false);
            this.metroPanel3.ResumeLayout(false);
            this.ResumeLayout(false);

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
        private CatalogListBox catalogListBox;
        private MetroFramework.Controls.MetroPanel metroPanel3;
    }
}