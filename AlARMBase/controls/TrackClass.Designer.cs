namespace ALARm.controls
{
    partial class TrackClassForm
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
            this.catalogListBox = new ALARm.controls.CatalogListBox();
            this.coordControl = new ALARm.controls.CoordControl();
            this.metroPanel2.SuspendLayout();
            this.SuspendLayout();
            // 
            // metroPanel2
            // 
            this.metroPanel2.Controls.Add(this.btnSave);
            this.metroPanel2.Controls.Add(this.btnCancel);
            this.metroPanel2.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.metroPanel2.HorizontalScrollbarBarColor = true;
            this.metroPanel2.HorizontalScrollbarHighlightOnWheel = false;
            this.metroPanel2.HorizontalScrollbarSize = 15;
            this.metroPanel2.Location = new System.Drawing.Point(20, 403);
            this.metroPanel2.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.metroPanel2.Name = "metroPanel2";
            this.metroPanel2.Size = new System.Drawing.Size(496, 48);
            this.metroPanel2.TabIndex = 7;
            this.metroPanel2.VerticalScrollbarBarColor = true;
            this.metroPanel2.VerticalScrollbarHighlightOnWheel = false;
            this.metroPanel2.VerticalScrollbarSize = 15;
            // 
            // btnSave
            // 
            this.btnSave.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnSave.ForeColor = System.Drawing.SystemColors.Control;
            this.btnSave.Location = new System.Drawing.Point(190, 0);
            this.btnSave.Margin = new System.Windows.Forms.Padding(10, 9, 10, 9);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(153, 48);
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
            this.btnCancel.Location = new System.Drawing.Point(343, 0);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(10, 9, 10, 9);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(153, 48);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = global::ALARm.alerts.btn_cancel;
            this.btnCancel.UseSelectable = true;
            // 
            // catalogListBox
            // 
            this.catalogListBox.Dock = System.Windows.Forms.DockStyle.Top;
            this.catalogListBox.Location = new System.Drawing.Point(20, 320);
            this.catalogListBox.Margin = new System.Windows.Forms.Padding(2);
            this.catalogListBox.Name = "catalogListBox";
            this.catalogListBox.Size = new System.Drawing.Size(496, 60);
            this.catalogListBox.TabIndex = 6;
            this.catalogListBox.Title = "Класс";
            this.catalogListBox.UseSelectable = true;
            // 
            // coordControl
            // 
            this.coordControl.AutoSize = true;
            this.coordControl.Dock = System.Windows.Forms.DockStyle.Top;
            this.coordControl.FinalKm = 0;
            this.coordControl.FinalM = 0;
            this.coordControl.Location = new System.Drawing.Point(20, 80);
            this.coordControl.Margin = new System.Windows.Forms.Padding(6, 8, 6, 8);
            this.coordControl.MetersHidden = false;
            this.coordControl.Name = "coordControl";
            this.coordControl.Size = new System.Drawing.Size(496, 240);
            this.coordControl.StartKm = 0;
            this.coordControl.StartM = 0;
            this.coordControl.TabIndex = 5;
            this.coordControl.UseSelectable = true;
            // 
            // TrackClassForm
            // 
            this.AcceptButton = this.btnSave;
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(536, 471);
            this.Controls.Add(this.metroPanel2);
            this.Controls.Add(this.catalogListBox);
            this.Controls.Add(this.coordControl);
            this.MaximizeBox = false;
            this.Name = "TrackClassForm";
            this.Padding = new System.Windows.Forms.Padding(20, 80, 20, 20);
            this.Text = alerts.inserting;
            this.metroPanel2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MetroFramework.Controls.MetroPanel metroPanel2;
        private MetroFramework.Controls.MetroButton btnSave;
        private MetroFramework.Controls.MetroButton btnCancel;
        private CatalogListBox catalogListBox;
        private CoordControl coordControl;
    }
}