namespace ALARm.controls
{
    partial class CheckSectionForm
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
            this.tbAvgLvl = new MetroFramework.Controls.MetroTextBox();
            this.metroLabel2 = new MetroFramework.Controls.MetroLabel();
            this.metroPanel1 = new MetroFramework.Controls.MetroPanel();
            this.tbSkoLvl = new MetroFramework.Controls.MetroTextBox();
            this.metroLabel1 = new MetroFramework.Controls.MetroLabel();
            this.metroPanel4 = new MetroFramework.Controls.MetroPanel();
            this.tbAvgW = new MetroFramework.Controls.MetroTextBox();
            this.metroLabel3 = new MetroFramework.Controls.MetroLabel();
            this.metroPanel5 = new MetroFramework.Controls.MetroPanel();
            this.tbSkoW = new MetroFramework.Controls.MetroTextBox();
            this.metroLabel4 = new MetroFramework.Controls.MetroLabel();
            this.metroPanel2.SuspendLayout();
            this.metroPanel3.SuspendLayout();
            this.metroPanel1.SuspendLayout();
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
            this.metroPanel2.Location = new System.Drawing.Point(13, 382);
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
            // metroPanel3
            // 
            this.metroPanel3.Controls.Add(this.tbAvgLvl);
            this.metroPanel3.Controls.Add(this.metroLabel2);
            this.metroPanel3.Dock = System.Windows.Forms.DockStyle.Top;
            this.metroPanel3.HorizontalScrollbarBarColor = true;
            this.metroPanel3.HorizontalScrollbarHighlightOnWheel = false;
            this.metroPanel3.HorizontalScrollbarSize = 10;
            this.metroPanel3.Location = new System.Drawing.Point(13, 216);
            this.metroPanel3.Margin = new System.Windows.Forms.Padding(0);
            this.metroPanel3.Name = "metroPanel3";
            this.metroPanel3.Padding = new System.Windows.Forms.Padding(5);
            this.metroPanel3.Size = new System.Drawing.Size(331, 39);
            this.metroPanel3.TabIndex = 24;
            this.metroPanel3.VerticalScrollbarBarColor = true;
            this.metroPanel3.VerticalScrollbarHighlightOnWheel = false;
            this.metroPanel3.VerticalScrollbarSize = 10;
            // 
            // tbAvgLvl
            // 
            // 
            // 
            // 
            this.tbAvgLvl.CustomButton.Image = null;
            this.tbAvgLvl.CustomButton.Location = new System.Drawing.Point(153, 1);
            this.tbAvgLvl.CustomButton.Name = "";
            this.tbAvgLvl.CustomButton.Size = new System.Drawing.Size(27, 27);
            this.tbAvgLvl.CustomButton.Style = MetroFramework.MetroColorStyle.Blue;
            this.tbAvgLvl.CustomButton.TabIndex = 1;
            this.tbAvgLvl.CustomButton.Theme = MetroFramework.MetroThemeStyle.Light;
            this.tbAvgLvl.CustomButton.UseSelectable = true;
            this.tbAvgLvl.CustomButton.Visible = false;
            this.tbAvgLvl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbAvgLvl.Lines = new string[0];
            this.tbAvgLvl.Location = new System.Drawing.Point(145, 5);
            this.tbAvgLvl.MaxLength = 32767;
            this.tbAvgLvl.Name = "tbAvgLvl";
            this.tbAvgLvl.PasswordChar = '\0';
            this.tbAvgLvl.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.tbAvgLvl.SelectedText = "";
            this.tbAvgLvl.SelectionLength = 0;
            this.tbAvgLvl.SelectionStart = 0;
            this.tbAvgLvl.ShortcutsEnabled = true;
            this.tbAvgLvl.Size = new System.Drawing.Size(181, 29);
            this.tbAvgLvl.TabIndex = 2;
            this.tbAvgLvl.UseSelectable = true;
            this.tbAvgLvl.WaterMarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
            this.tbAvgLvl.WaterMarkFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Pixel);
            // 
            // metroLabel2
            // 
            this.metroLabel2.Dock = System.Windows.Forms.DockStyle.Left;
            this.metroLabel2.Location = new System.Drawing.Point(5, 5);
            this.metroLabel2.Name = "metroLabel2";
            this.metroLabel2.Size = new System.Drawing.Size(140, 29);
            this.metroLabel2.TabIndex = 1;
            this.metroLabel2.Text = "Уровень(МО)";
            this.metroLabel2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // metroPanel1
            // 
            this.metroPanel1.Controls.Add(this.tbSkoLvl);
            this.metroPanel1.Controls.Add(this.metroLabel1);
            this.metroPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.metroPanel1.HorizontalScrollbarBarColor = true;
            this.metroPanel1.HorizontalScrollbarHighlightOnWheel = false;
            this.metroPanel1.HorizontalScrollbarSize = 10;
            this.metroPanel1.Location = new System.Drawing.Point(13, 255);
            this.metroPanel1.Margin = new System.Windows.Forms.Padding(0);
            this.metroPanel1.Name = "metroPanel1";
            this.metroPanel1.Padding = new System.Windows.Forms.Padding(5);
            this.metroPanel1.Size = new System.Drawing.Size(331, 39);
            this.metroPanel1.TabIndex = 25;
            this.metroPanel1.VerticalScrollbarBarColor = true;
            this.metroPanel1.VerticalScrollbarHighlightOnWheel = false;
            this.metroPanel1.VerticalScrollbarSize = 10;
            // 
            // tbSkoLvl
            // 
            // 
            // 
            // 
            this.tbSkoLvl.CustomButton.Image = null;
            this.tbSkoLvl.CustomButton.Location = new System.Drawing.Point(153, 1);
            this.tbSkoLvl.CustomButton.Name = "";
            this.tbSkoLvl.CustomButton.Size = new System.Drawing.Size(27, 27);
            this.tbSkoLvl.CustomButton.Style = MetroFramework.MetroColorStyle.Blue;
            this.tbSkoLvl.CustomButton.TabIndex = 1;
            this.tbSkoLvl.CustomButton.Theme = MetroFramework.MetroThemeStyle.Light;
            this.tbSkoLvl.CustomButton.UseSelectable = true;
            this.tbSkoLvl.CustomButton.Visible = false;
            this.tbSkoLvl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbSkoLvl.Lines = new string[0];
            this.tbSkoLvl.Location = new System.Drawing.Point(145, 5);
            this.tbSkoLvl.MaxLength = 32767;
            this.tbSkoLvl.Name = "tbSkoLvl";
            this.tbSkoLvl.PasswordChar = '\0';
            this.tbSkoLvl.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.tbSkoLvl.SelectedText = "";
            this.tbSkoLvl.SelectionLength = 0;
            this.tbSkoLvl.SelectionStart = 0;
            this.tbSkoLvl.ShortcutsEnabled = true;
            this.tbSkoLvl.Size = new System.Drawing.Size(181, 29);
            this.tbSkoLvl.TabIndex = 2;
            this.tbSkoLvl.UseSelectable = true;
            this.tbSkoLvl.WaterMarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
            this.tbSkoLvl.WaterMarkFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Pixel);
            // 
            // metroLabel1
            // 
            this.metroLabel1.Dock = System.Windows.Forms.DockStyle.Left;
            this.metroLabel1.Location = new System.Drawing.Point(5, 5);
            this.metroLabel1.Name = "metroLabel1";
            this.metroLabel1.Size = new System.Drawing.Size(140, 29);
            this.metroLabel1.TabIndex = 1;
            this.metroLabel1.Text = "Уровень(СКО)";
            this.metroLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // metroPanel4
            // 
            this.metroPanel4.Controls.Add(this.tbAvgW);
            this.metroPanel4.Controls.Add(this.metroLabel3);
            this.metroPanel4.Dock = System.Windows.Forms.DockStyle.Top;
            this.metroPanel4.HorizontalScrollbarBarColor = true;
            this.metroPanel4.HorizontalScrollbarHighlightOnWheel = false;
            this.metroPanel4.HorizontalScrollbarSize = 10;
            this.metroPanel4.Location = new System.Drawing.Point(13, 294);
            this.metroPanel4.Margin = new System.Windows.Forms.Padding(0);
            this.metroPanel4.Name = "metroPanel4";
            this.metroPanel4.Padding = new System.Windows.Forms.Padding(5);
            this.metroPanel4.Size = new System.Drawing.Size(331, 39);
            this.metroPanel4.TabIndex = 26;
            this.metroPanel4.VerticalScrollbarBarColor = true;
            this.metroPanel4.VerticalScrollbarHighlightOnWheel = false;
            this.metroPanel4.VerticalScrollbarSize = 10;
            // 
            // tbAvgW
            // 
            // 
            // 
            // 
            this.tbAvgW.CustomButton.Image = null;
            this.tbAvgW.CustomButton.Location = new System.Drawing.Point(153, 1);
            this.tbAvgW.CustomButton.Name = "";
            this.tbAvgW.CustomButton.Size = new System.Drawing.Size(27, 27);
            this.tbAvgW.CustomButton.Style = MetroFramework.MetroColorStyle.Blue;
            this.tbAvgW.CustomButton.TabIndex = 1;
            this.tbAvgW.CustomButton.Theme = MetroFramework.MetroThemeStyle.Light;
            this.tbAvgW.CustomButton.UseSelectable = true;
            this.tbAvgW.CustomButton.Visible = false;
            this.tbAvgW.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbAvgW.Lines = new string[0];
            this.tbAvgW.Location = new System.Drawing.Point(145, 5);
            this.tbAvgW.MaxLength = 32767;
            this.tbAvgW.Name = "tbAvgW";
            this.tbAvgW.PasswordChar = '\0';
            this.tbAvgW.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.tbAvgW.SelectedText = "";
            this.tbAvgW.SelectionLength = 0;
            this.tbAvgW.SelectionStart = 0;
            this.tbAvgW.ShortcutsEnabled = true;
            this.tbAvgW.Size = new System.Drawing.Size(181, 29);
            this.tbAvgW.TabIndex = 2;
            this.tbAvgW.UseSelectable = true;
            this.tbAvgW.WaterMarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
            this.tbAvgW.WaterMarkFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Pixel);
            // 
            // metroLabel3
            // 
            this.metroLabel3.Dock = System.Windows.Forms.DockStyle.Left;
            this.metroLabel3.Location = new System.Drawing.Point(5, 5);
            this.metroLabel3.Name = "metroLabel3";
            this.metroLabel3.Size = new System.Drawing.Size(140, 29);
            this.metroLabel3.TabIndex = 1;
            this.metroLabel3.Text = "Шаблон(МО)";
            this.metroLabel3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // metroPanel5
            // 
            this.metroPanel5.Controls.Add(this.tbSkoW);
            this.metroPanel5.Controls.Add(this.metroLabel4);
            this.metroPanel5.Dock = System.Windows.Forms.DockStyle.Top;
            this.metroPanel5.HorizontalScrollbarBarColor = true;
            this.metroPanel5.HorizontalScrollbarHighlightOnWheel = false;
            this.metroPanel5.HorizontalScrollbarSize = 10;
            this.metroPanel5.Location = new System.Drawing.Point(13, 333);
            this.metroPanel5.Margin = new System.Windows.Forms.Padding(0);
            this.metroPanel5.Name = "metroPanel5";
            this.metroPanel5.Padding = new System.Windows.Forms.Padding(5);
            this.metroPanel5.Size = new System.Drawing.Size(331, 39);
            this.metroPanel5.TabIndex = 27;
            this.metroPanel5.VerticalScrollbarBarColor = true;
            this.metroPanel5.VerticalScrollbarHighlightOnWheel = false;
            this.metroPanel5.VerticalScrollbarSize = 10;
            // 
            // tbSkoW
            // 
            // 
            // 
            // 
            this.tbSkoW.CustomButton.Image = null;
            this.tbSkoW.CustomButton.Location = new System.Drawing.Point(153, 1);
            this.tbSkoW.CustomButton.Name = "";
            this.tbSkoW.CustomButton.Size = new System.Drawing.Size(27, 27);
            this.tbSkoW.CustomButton.Style = MetroFramework.MetroColorStyle.Blue;
            this.tbSkoW.CustomButton.TabIndex = 1;
            this.tbSkoW.CustomButton.Theme = MetroFramework.MetroThemeStyle.Light;
            this.tbSkoW.CustomButton.UseSelectable = true;
            this.tbSkoW.CustomButton.Visible = false;
            this.tbSkoW.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbSkoW.Lines = new string[0];
            this.tbSkoW.Location = new System.Drawing.Point(145, 5);
            this.tbSkoW.MaxLength = 32767;
            this.tbSkoW.Name = "tbSkoW";
            this.tbSkoW.PasswordChar = '\0';
            this.tbSkoW.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.tbSkoW.SelectedText = "";
            this.tbSkoW.SelectionLength = 0;
            this.tbSkoW.SelectionStart = 0;
            this.tbSkoW.ShortcutsEnabled = true;
            this.tbSkoW.Size = new System.Drawing.Size(181, 29);
            this.tbSkoW.TabIndex = 2;
            this.tbSkoW.UseSelectable = true;
            this.tbSkoW.WaterMarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
            this.tbSkoW.WaterMarkFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Pixel);
            // 
            // metroLabel4
            // 
            this.metroLabel4.Dock = System.Windows.Forms.DockStyle.Left;
            this.metroLabel4.Location = new System.Drawing.Point(5, 5);
            this.metroLabel4.Name = "metroLabel4";
            this.metroLabel4.Size = new System.Drawing.Size(140, 29);
            this.metroLabel4.TabIndex = 1;
            this.metroLabel4.Text = "Шаблон(СКО)";
            this.metroLabel4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // CheckSectionForm
            // 
            this.AcceptButton = this.btnSave;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(357, 426);
            this.Controls.Add(this.metroPanel5);
            this.Controls.Add(this.metroPanel4);
            this.Controls.Add(this.metroPanel1);
            this.Controls.Add(this.metroPanel3);
            this.Controls.Add(this.metroPanel2);
            this.Controls.Add(this.coordControl);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximizeBox = false;
            this.Name = "CheckSectionForm";
            this.Padding = new System.Windows.Forms.Padding(13, 60, 13, 13);
            this.Text = "Добавление записи";
            this.metroPanel2.ResumeLayout(false);
            this.metroPanel3.ResumeLayout(false);
            this.metroPanel1.ResumeLayout(false);
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
        private MetroFramework.Controls.MetroPanel metroPanel3;
        private MetroFramework.Controls.MetroTextBox tbAvgLvl;
        private MetroFramework.Controls.MetroLabel metroLabel2;
        private MetroFramework.Controls.MetroPanel metroPanel1;
        private MetroFramework.Controls.MetroTextBox tbSkoLvl;
        private MetroFramework.Controls.MetroLabel metroLabel1;
        private MetroFramework.Controls.MetroPanel metroPanel4;
        private MetroFramework.Controls.MetroTextBox tbAvgW;
        private MetroFramework.Controls.MetroLabel metroLabel3;
        private MetroFramework.Controls.MetroPanel metroPanel5;
        private MetroFramework.Controls.MetroTextBox tbSkoW;
        private MetroFramework.Controls.MetroLabel metroLabel4;
    }
}