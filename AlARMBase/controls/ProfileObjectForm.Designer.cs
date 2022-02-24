namespace ALARm.controls
{
    partial class ProfileObjectForm
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
            this.catSide = new ALARm.controls.CatalogListBox();
            this.catObject = new ALARm.controls.CatalogListBox();
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
            this.metroPanel2.HorizontalScrollbarSize = 10;
            this.metroPanel2.Location = new System.Drawing.Point(20, 180);
            this.metroPanel2.Name = "metroPanel2";
            this.metroPanel2.Size = new System.Drawing.Size(317, 31);
            this.metroPanel2.TabIndex = 12;
            this.metroPanel2.VerticalScrollbarBarColor = true;
            this.metroPanel2.VerticalScrollbarHighlightOnWheel = false;
            this.metroPanel2.VerticalScrollbarSize = 10;
            // 
            // btnSave
            // 
            this.btnSave.Dock = System.Windows.Forms.DockStyle.Right;
            this.btnSave.ForeColor = System.Drawing.SystemColors.Control;
            this.btnSave.Location = new System.Drawing.Point(113, 0);
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
            this.btnCancel.Location = new System.Drawing.Point(215, 0);
            this.btnCancel.Margin = new System.Windows.Forms.Padding(7, 6, 7, 6);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(102, 31);
            this.btnCancel.TabIndex = 3;
            this.btnCancel.Text = global::ALARm.alerts.btn_cancel;
            this.btnCancel.UseSelectable = true;
            // 
            // catSide
            // 
            this.catSide.CurrentId = -1;
            this.catSide.Dock = System.Windows.Forms.DockStyle.Top;
            this.catSide.Location = new System.Drawing.Point(20, 177);
            this.catSide.Margin = new System.Windows.Forms.Padding(1);
            this.catSide.Name = "catSide";
            this.catSide.Size = new System.Drawing.Size(317, 39);
            this.catSide.TabIndex = 11;
            this.catSide.Title = "Сторона";
            this.catSide.TitleWidth = 100;
            this.catSide.UseSelectable = true;
            this.catSide.Visible = false;
            // 
            // catObject
            // 
            this.catObject.CurrentId = -1;
            this.catObject.Dock = System.Windows.Forms.DockStyle.Top;
            this.catObject.Location = new System.Drawing.Point(20, 138);
            this.catObject.Margin = new System.Windows.Forms.Padding(1);
            this.catObject.Name = "catObject";
            this.catObject.Size = new System.Drawing.Size(317, 39);
            this.catObject.TabIndex = 10;
            this.catObject.Title = "Тип объекта";
            this.catObject.TitleWidth = 100;
            this.catObject.UseSelectable = true;
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
            this.coordControl.Location = new System.Drawing.Point(20, 60);
            this.coordControl.MetersHidden = false;
            this.coordControl.Name = "coordControl";
            this.coordControl.Size = new System.Drawing.Size(317, 78);
            this.coordControl.StartKm = 0;
            this.coordControl.StartKmTitle = "Км";
            this.coordControl.StartM = 0;
            this.coordControl.StartMTitle = "М";
            this.coordControl.TabIndex = 9;
            this.coordControl.TitleWidth = 110;
            this.coordControl.UseSelectable = true;
            // 
            // ProfileObjectForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(357, 231);
            this.Controls.Add(this.metroPanel2);
            this.Controls.Add(this.catSide);
            this.Controls.Add(this.catObject);
            this.Controls.Add(this.coordControl);
            this.Name = "ProfileObjectForm";
            this.Text = "Добавление записи";
            this.metroPanel2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private CoordControl coordControl;
        private CatalogListBox catObject;
        private CatalogListBox catSide;
        private MetroFramework.Controls.MetroPanel metroPanel2;
        private MetroFramework.Controls.MetroButton btnSave;
        private MetroFramework.Controls.MetroButton btnCancel;
    }
}