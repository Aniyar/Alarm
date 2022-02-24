namespace ALARm.controls
{
    partial class SpeedForm
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
            this.mpFinalM = new MetroFramework.Controls.MetroPanel();
            this.passenger = new MetroFramework.Controls.MetroTextBox();
            this.metroLabel4 = new MetroFramework.Controls.MetroLabel();
            this.metroPanel1 = new MetroFramework.Controls.MetroPanel();
            this.freight = new MetroFramework.Controls.MetroTextBox();
            this.metroLabel1 = new MetroFramework.Controls.MetroLabel();
            this.metroPanel3 = new MetroFramework.Controls.MetroPanel();
            this.emptyfreight = new MetroFramework.Controls.MetroTextBox();
            this.metroLabel2 = new MetroFramework.Controls.MetroLabel();
            this.metroPanel2 = new MetroFramework.Controls.MetroPanel();
            this.btnSave = new MetroFramework.Controls.MetroButton();
            this.btnCancel = new MetroFramework.Controls.MetroButton();
            this.catalogListBox = new ALARm.controls.CatalogListBox();
            this.coordControl = new ALARm.controls.CoordControl();
            this.metroPanel4 = new MetroFramework.Controls.MetroPanel();
            this.tbSapsan = new MetroFramework.Controls.MetroTextBox();
            this.metroLabel3 = new MetroFramework.Controls.MetroLabel();
            this.metroPanel5 = new MetroFramework.Controls.MetroPanel();
            this.tbLastochka = new MetroFramework.Controls.MetroTextBox();
            this.metroLabel5 = new MetroFramework.Controls.MetroLabel();
            this.mpFinalM.SuspendLayout();
            this.metroPanel1.SuspendLayout();
            this.metroPanel3.SuspendLayout();
            this.metroPanel2.SuspendLayout();
            this.metroPanel4.SuspendLayout();
            this.metroPanel5.SuspendLayout();
            this.SuspendLayout();
            // 
            // mpFinalM
            // 
            this.mpFinalM.Controls.Add(this.passenger);
            this.mpFinalM.Controls.Add(this.metroLabel4);
            this.mpFinalM.Dock = System.Windows.Forms.DockStyle.Top;
            this.mpFinalM.HorizontalScrollbarBarColor = true;
            this.mpFinalM.HorizontalScrollbarHighlightOnWheel = false;
            this.mpFinalM.HorizontalScrollbarSize = 10;
            this.mpFinalM.Location = new System.Drawing.Point(13, 216);
            this.mpFinalM.Margin = new System.Windows.Forms.Padding(0);
            this.mpFinalM.Name = "mpFinalM";
            this.mpFinalM.Padding = new System.Windows.Forms.Padding(5);
            this.mpFinalM.Size = new System.Drawing.Size(331, 39);
            this.mpFinalM.TabIndex = 14;
            this.mpFinalM.VerticalScrollbarBarColor = true;
            this.mpFinalM.VerticalScrollbarHighlightOnWheel = false;
            this.mpFinalM.VerticalScrollbarSize = 10;
            // 
            // passenger
            // 
            // 
            // 
            // 
            this.passenger.CustomButton.Image = null;
            this.passenger.CustomButton.Location = new System.Drawing.Point(183, 1);
            this.passenger.CustomButton.Name = "";
            this.passenger.CustomButton.Size = new System.Drawing.Size(27, 27);
            this.passenger.CustomButton.Style = MetroFramework.MetroColorStyle.Blue;
            this.passenger.CustomButton.TabIndex = 1;
            this.passenger.CustomButton.Theme = MetroFramework.MetroThemeStyle.Light;
            this.passenger.CustomButton.UseSelectable = true;
            this.passenger.CustomButton.Visible = false;
            this.passenger.Dock = System.Windows.Forms.DockStyle.Fill;
            this.passenger.Lines = new string[0];
            this.passenger.Location = new System.Drawing.Point(115, 5);
            this.passenger.MaxLength = 32767;
            this.passenger.Name = "passenger";
            this.passenger.PasswordChar = '\0';
            this.passenger.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.passenger.SelectedText = "";
            this.passenger.SelectionLength = 0;
            this.passenger.SelectionStart = 0;
            this.passenger.ShortcutsEnabled = true;
            this.passenger.Size = new System.Drawing.Size(211, 29);
            this.passenger.TabIndex = 2;
            this.passenger.UseSelectable = true;
            this.passenger.WaterMarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
            this.passenger.WaterMarkFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Pixel);
            this.passenger.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.DigitKeyFilter);
            // 
            // metroLabel4
            // 
            this.metroLabel4.Dock = System.Windows.Forms.DockStyle.Left;
            this.metroLabel4.Location = new System.Drawing.Point(5, 5);
            this.metroLabel4.Name = "metroLabel4";
            this.metroLabel4.Size = new System.Drawing.Size(110, 29);
            this.metroLabel4.TabIndex = 1;
            this.metroLabel4.Text = "Пассажирская";
            this.metroLabel4.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // metroPanel1
            // 
            this.metroPanel1.Controls.Add(this.freight);
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
            this.metroPanel1.TabIndex = 15;
            this.metroPanel1.VerticalScrollbarBarColor = true;
            this.metroPanel1.VerticalScrollbarHighlightOnWheel = false;
            this.metroPanel1.VerticalScrollbarSize = 10;
            // 
            // freight
            // 
            // 
            // 
            // 
            this.freight.CustomButton.Image = null;
            this.freight.CustomButton.Location = new System.Drawing.Point(183, 1);
            this.freight.CustomButton.Name = "";
            this.freight.CustomButton.Size = new System.Drawing.Size(27, 27);
            this.freight.CustomButton.Style = MetroFramework.MetroColorStyle.Blue;
            this.freight.CustomButton.TabIndex = 1;
            this.freight.CustomButton.Theme = MetroFramework.MetroThemeStyle.Light;
            this.freight.CustomButton.UseSelectable = true;
            this.freight.CustomButton.Visible = false;
            this.freight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.freight.Lines = new string[0];
            this.freight.Location = new System.Drawing.Point(115, 5);
            this.freight.MaxLength = 32767;
            this.freight.Name = "freight";
            this.freight.PasswordChar = '\0';
            this.freight.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.freight.SelectedText = "";
            this.freight.SelectionLength = 0;
            this.freight.SelectionStart = 0;
            this.freight.ShortcutsEnabled = true;
            this.freight.Size = new System.Drawing.Size(211, 29);
            this.freight.TabIndex = 2;
            this.freight.UseSelectable = true;
            this.freight.WaterMarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
            this.freight.WaterMarkFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Pixel);
            this.freight.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.DigitKeyFilter);
            // 
            // metroLabel1
            // 
            this.metroLabel1.Dock = System.Windows.Forms.DockStyle.Left;
            this.metroLabel1.Location = new System.Drawing.Point(5, 5);
            this.metroLabel1.Name = "metroLabel1";
            this.metroLabel1.Size = new System.Drawing.Size(110, 29);
            this.metroLabel1.TabIndex = 1;
            this.metroLabel1.Text = "Грузовая";
            this.metroLabel1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // metroPanel3
            // 
            this.metroPanel3.Controls.Add(this.emptyfreight);
            this.metroPanel3.Controls.Add(this.metroLabel2);
            this.metroPanel3.Dock = System.Windows.Forms.DockStyle.Top;
            this.metroPanel3.HorizontalScrollbarBarColor = true;
            this.metroPanel3.HorizontalScrollbarHighlightOnWheel = false;
            this.metroPanel3.HorizontalScrollbarSize = 10;
            this.metroPanel3.Location = new System.Drawing.Point(13, 294);
            this.metroPanel3.Margin = new System.Windows.Forms.Padding(0);
            this.metroPanel3.Name = "metroPanel3";
            this.metroPanel3.Padding = new System.Windows.Forms.Padding(5);
            this.metroPanel3.Size = new System.Drawing.Size(331, 40);
            this.metroPanel3.TabIndex = 16;
            this.metroPanel3.VerticalScrollbarBarColor = true;
            this.metroPanel3.VerticalScrollbarHighlightOnWheel = false;
            this.metroPanel3.VerticalScrollbarSize = 10;
            // 
            // emptyfreight
            // 
            // 
            // 
            // 
            this.emptyfreight.CustomButton.Image = null;
            this.emptyfreight.CustomButton.Location = new System.Drawing.Point(183, 1);
            this.emptyfreight.CustomButton.Name = "";
            this.emptyfreight.CustomButton.Size = new System.Drawing.Size(27, 27);
            this.emptyfreight.CustomButton.Style = MetroFramework.MetroColorStyle.Blue;
            this.emptyfreight.CustomButton.TabIndex = 1;
            this.emptyfreight.CustomButton.Theme = MetroFramework.MetroThemeStyle.Light;
            this.emptyfreight.CustomButton.UseSelectable = true;
            this.emptyfreight.CustomButton.Visible = false;
            this.emptyfreight.Dock = System.Windows.Forms.DockStyle.Fill;
            this.emptyfreight.Lines = new string[0];
            this.emptyfreight.Location = new System.Drawing.Point(115, 5);
            this.emptyfreight.MaximumSize = new System.Drawing.Size(0, 29);
            this.emptyfreight.MaxLength = 32767;
            this.emptyfreight.Name = "emptyfreight";
            this.emptyfreight.PasswordChar = '\0';
            this.emptyfreight.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.emptyfreight.SelectedText = "";
            this.emptyfreight.SelectionLength = 0;
            this.emptyfreight.SelectionStart = 0;
            this.emptyfreight.ShortcutsEnabled = true;
            this.emptyfreight.Size = new System.Drawing.Size(211, 29);
            this.emptyfreight.TabIndex = 2;
            this.emptyfreight.UseSelectable = true;
            this.emptyfreight.WaterMarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
            this.emptyfreight.WaterMarkFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Pixel);
            this.emptyfreight.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.DigitKeyFilter);
            // 
            // metroLabel2
            // 
            this.metroLabel2.Dock = System.Windows.Forms.DockStyle.Left;
            this.metroLabel2.Location = new System.Drawing.Point(5, 5);
            this.metroLabel2.Name = "metroLabel2";
            this.metroLabel2.Size = new System.Drawing.Size(110, 30);
            this.metroLabel2.TabIndex = 1;
            this.metroLabel2.Text = "Грузовая прж.";
            this.metroLabel2.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.metroLabel2.WrapToLine = true;
            // 
            // metroPanel2
            // 
            this.metroPanel2.Controls.Add(this.btnSave);
            this.metroPanel2.Controls.Add(this.btnCancel);
            this.metroPanel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.metroPanel2.HorizontalScrollbarBarColor = true;
            this.metroPanel2.HorizontalScrollbarHighlightOnWheel = false;
            this.metroPanel2.HorizontalScrollbarSize = 10;
            this.metroPanel2.Location = new System.Drawing.Point(13, 418);
            this.metroPanel2.Name = "metroPanel2";
            this.metroPanel2.Size = new System.Drawing.Size(331, 31);
            this.metroPanel2.TabIndex = 18;
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
            this.btnSave.TabIndex = 3;
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
            this.btnCancel.TabIndex = 4;
            this.btnCancel.Text = global::ALARm.alerts.btn_cancel;
            this.btnCancel.UseSelectable = true;
            // 
            // catalogListBox
            // 
            this.catalogListBox.CurrentId = -1;
            this.catalogListBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.catalogListBox.Location = new System.Drawing.Point(13, 414);
            this.catalogListBox.Margin = new System.Windows.Forms.Padding(1);
            this.catalogListBox.Name = "catalogListBox";
            this.catalogListBox.Size = new System.Drawing.Size(331, 39);
            this.catalogListBox.TabIndex = 17;
            this.catalogListBox.Title = "Тип";
            this.catalogListBox.TitleWidth = 100;
            this.catalogListBox.UseSelectable = true;
            this.catalogListBox.Visible = false;
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
            this.coordControl.TabIndex = 11;
            this.coordControl.TitleWidth = 110;
            this.coordControl.UseSelectable = true;
            // 
            // metroPanel4
            // 
            this.metroPanel4.Controls.Add(this.tbSapsan);
            this.metroPanel4.Controls.Add(this.metroLabel3);
            this.metroPanel4.Dock = System.Windows.Forms.DockStyle.Top;
            this.metroPanel4.HorizontalScrollbarBarColor = true;
            this.metroPanel4.HorizontalScrollbarHighlightOnWheel = false;
            this.metroPanel4.HorizontalScrollbarSize = 10;
            this.metroPanel4.Location = new System.Drawing.Point(13, 334);
            this.metroPanel4.Margin = new System.Windows.Forms.Padding(0);
            this.metroPanel4.Name = "metroPanel4";
            this.metroPanel4.Padding = new System.Windows.Forms.Padding(5);
            this.metroPanel4.Size = new System.Drawing.Size(331, 40);
            this.metroPanel4.TabIndex = 19;
            this.metroPanel4.VerticalScrollbarBarColor = true;
            this.metroPanel4.VerticalScrollbarHighlightOnWheel = false;
            this.metroPanel4.VerticalScrollbarSize = 10;
            // 
            // tbSapsan
            // 
            // 
            // 
            // 
            this.tbSapsan.CustomButton.Image = null;
            this.tbSapsan.CustomButton.Location = new System.Drawing.Point(183, 1);
            this.tbSapsan.CustomButton.Name = "";
            this.tbSapsan.CustomButton.Size = new System.Drawing.Size(27, 27);
            this.tbSapsan.CustomButton.Style = MetroFramework.MetroColorStyle.Blue;
            this.tbSapsan.CustomButton.TabIndex = 1;
            this.tbSapsan.CustomButton.Theme = MetroFramework.MetroThemeStyle.Light;
            this.tbSapsan.CustomButton.UseSelectable = true;
            this.tbSapsan.CustomButton.Visible = false;
            this.tbSapsan.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbSapsan.Lines = new string[0];
            this.tbSapsan.Location = new System.Drawing.Point(115, 5);
            this.tbSapsan.MaximumSize = new System.Drawing.Size(0, 29);
            this.tbSapsan.MaxLength = 32767;
            this.tbSapsan.Name = "tbSapsan";
            this.tbSapsan.PasswordChar = '\0';
            this.tbSapsan.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.tbSapsan.SelectedText = "";
            this.tbSapsan.SelectionLength = 0;
            this.tbSapsan.SelectionStart = 0;
            this.tbSapsan.ShortcutsEnabled = true;
            this.tbSapsan.Size = new System.Drawing.Size(211, 29);
            this.tbSapsan.TabIndex = 2;
            this.tbSapsan.UseSelectable = true;
            this.tbSapsan.WaterMarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
            this.tbSapsan.WaterMarkFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Pixel);
            this.tbSapsan.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.DigitKeyFilter);
            // 
            // metroLabel3
            // 
            this.metroLabel3.Dock = System.Windows.Forms.DockStyle.Left;
            this.metroLabel3.Location = new System.Drawing.Point(5, 5);
            this.metroLabel3.Name = "metroLabel3";
            this.metroLabel3.Size = new System.Drawing.Size(110, 30);
            this.metroLabel3.TabIndex = 1;
            this.metroLabel3.Text = "Сапсан";
            this.metroLabel3.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.metroLabel3.WrapToLine = true;
            // 
            // metroPanel5
            // 
            this.metroPanel5.Controls.Add(this.tbLastochka);
            this.metroPanel5.Controls.Add(this.metroLabel5);
            this.metroPanel5.Dock = System.Windows.Forms.DockStyle.Top;
            this.metroPanel5.HorizontalScrollbarBarColor = true;
            this.metroPanel5.HorizontalScrollbarHighlightOnWheel = false;
            this.metroPanel5.HorizontalScrollbarSize = 10;
            this.metroPanel5.Location = new System.Drawing.Point(13, 374);
            this.metroPanel5.Margin = new System.Windows.Forms.Padding(0);
            this.metroPanel5.Name = "metroPanel5";
            this.metroPanel5.Padding = new System.Windows.Forms.Padding(5);
            this.metroPanel5.Size = new System.Drawing.Size(331, 40);
            this.metroPanel5.TabIndex = 20;
            this.metroPanel5.VerticalScrollbarBarColor = true;
            this.metroPanel5.VerticalScrollbarHighlightOnWheel = false;
            this.metroPanel5.VerticalScrollbarSize = 10;
            // 
            // tbLastochka
            // 
            // 
            // 
            // 
            this.tbLastochka.CustomButton.Image = null;
            this.tbLastochka.CustomButton.Location = new System.Drawing.Point(183, 1);
            this.tbLastochka.CustomButton.Name = "";
            this.tbLastochka.CustomButton.Size = new System.Drawing.Size(27, 27);
            this.tbLastochka.CustomButton.Style = MetroFramework.MetroColorStyle.Blue;
            this.tbLastochka.CustomButton.TabIndex = 1;
            this.tbLastochka.CustomButton.Theme = MetroFramework.MetroThemeStyle.Light;
            this.tbLastochka.CustomButton.UseSelectable = true;
            this.tbLastochka.CustomButton.Visible = false;
            this.tbLastochka.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbLastochka.Lines = new string[0];
            this.tbLastochka.Location = new System.Drawing.Point(115, 5);
            this.tbLastochka.MaximumSize = new System.Drawing.Size(0, 29);
            this.tbLastochka.MaxLength = 32767;
            this.tbLastochka.Name = "tbLastochka";
            this.tbLastochka.PasswordChar = '\0';
            this.tbLastochka.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.tbLastochka.SelectedText = "";
            this.tbLastochka.SelectionLength = 0;
            this.tbLastochka.SelectionStart = 0;
            this.tbLastochka.ShortcutsEnabled = true;
            this.tbLastochka.Size = new System.Drawing.Size(211, 29);
            this.tbLastochka.TabIndex = 2;
            this.tbLastochka.UseSelectable = true;
            this.tbLastochka.WaterMarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
            this.tbLastochka.WaterMarkFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Pixel);
            this.tbLastochka.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.DigitKeyFilter);
            // 
            // metroLabel5
            // 
            this.metroLabel5.Dock = System.Windows.Forms.DockStyle.Left;
            this.metroLabel5.Location = new System.Drawing.Point(5, 5);
            this.metroLabel5.Name = "metroLabel5";
            this.metroLabel5.Size = new System.Drawing.Size(110, 30);
            this.metroLabel5.TabIndex = 1;
            this.metroLabel5.Text = "Ласточка";
            this.metroLabel5.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            this.metroLabel5.WrapToLine = true;
            // 
            // SpeedForm
            // 
            this.AcceptButton = this.btnSave;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(357, 462);
            this.Controls.Add(this.metroPanel2);
            this.Controls.Add(this.catalogListBox);
            this.Controls.Add(this.metroPanel5);
            this.Controls.Add(this.metroPanel4);
            this.Controls.Add(this.metroPanel3);
            this.Controls.Add(this.metroPanel1);
            this.Controls.Add(this.mpFinalM);
            this.Controls.Add(this.coordControl);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximizeBox = false;
            this.Name = "SpeedForm";
            this.Padding = new System.Windows.Forms.Padding(13, 60, 13, 13);
            this.Text = "Добавление записи";
            this.mpFinalM.ResumeLayout(false);
            this.metroPanel1.ResumeLayout(false);
            this.metroPanel3.ResumeLayout(false);
            this.metroPanel2.ResumeLayout(false);
            this.metroPanel4.ResumeLayout(false);
            this.metroPanel5.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private CoordControl coordControl;
        private MetroFramework.Controls.MetroPanel mpFinalM;
        private MetroFramework.Controls.MetroTextBox passenger;
        private MetroFramework.Controls.MetroLabel metroLabel4;
        private MetroFramework.Controls.MetroPanel metroPanel1;
        private MetroFramework.Controls.MetroTextBox freight;
        private MetroFramework.Controls.MetroLabel metroLabel1;
        private MetroFramework.Controls.MetroPanel metroPanel3;
        private MetroFramework.Controls.MetroTextBox emptyfreight;
        private MetroFramework.Controls.MetroLabel metroLabel2;
        private CatalogListBox catalogListBox;
        private MetroFramework.Controls.MetroPanel metroPanel2;
        private MetroFramework.Controls.MetroButton btnSave;
        private MetroFramework.Controls.MetroButton btnCancel;
        private MetroFramework.Controls.MetroPanel metroPanel4;
        private MetroFramework.Controls.MetroTextBox tbSapsan;
        private MetroFramework.Controls.MetroLabel metroLabel3;
        private MetroFramework.Controls.MetroPanel metroPanel5;
        private MetroFramework.Controls.MetroTextBox tbLastochka;
        private MetroFramework.Controls.MetroLabel metroLabel5;
    }
}