namespace ALARm.controls
{
    partial class AdmListBox
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.metroPanel1 = new MetroFramework.Controls.MetroPanel();
            this.cmbAdmUnit = new MetroFramework.Controls.MetroComboBox();
            this.admUnitBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.lbTitle = new MetroFramework.Controls.MetroLabel();
            this.metroPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.admUnitBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // metroPanel1
            // 
            this.metroPanel1.Controls.Add(this.cmbAdmUnit);
            this.metroPanel1.Controls.Add(this.lbTitle);
            this.metroPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.metroPanel1.HorizontalScrollbarBarColor = true;
            this.metroPanel1.HorizontalScrollbarHighlightOnWheel = false;
            this.metroPanel1.HorizontalScrollbarSize = 10;
            this.metroPanel1.Location = new System.Drawing.Point(0, 0);
            this.metroPanel1.Name = "metroPanel1";
            this.metroPanel1.Padding = new System.Windows.Forms.Padding(5, 5, 5, 5);
            this.metroPanel1.Size = new System.Drawing.Size(310, 39);
            this.metroPanel1.TabIndex = 0;
            this.metroPanel1.VerticalScrollbarBarColor = true;
            this.metroPanel1.VerticalScrollbarHighlightOnWheel = false;
            this.metroPanel1.VerticalScrollbarSize = 10;
            // 
            // cmbAdmUnit
            // 
            this.cmbAdmUnit.DataSource = this.admUnitBindingSource;
            this.cmbAdmUnit.DisplayMember = "Name";
            this.cmbAdmUnit.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cmbAdmUnit.FormattingEnabled = true;
            this.cmbAdmUnit.ItemHeight = 23;
            this.cmbAdmUnit.Location = new System.Drawing.Point(115, 5);
            this.cmbAdmUnit.Name = "cmbAdmUnit";
            this.cmbAdmUnit.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.cmbAdmUnit.Size = new System.Drawing.Size(190, 29);
            this.cmbAdmUnit.TabIndex = 5;
            this.cmbAdmUnit.UseSelectable = true;
            this.cmbAdmUnit.ValueMember = "Id";
            this.cmbAdmUnit.SelectedIndexChanged += new System.EventHandler(this.CmbAdmUnit_SelectedIndexChanged);
            // 
            // admUnitBindingSource
            // 
            this.admUnitBindingSource.DataSource = typeof(ALARm.Core.AdmUnit);
            // 
            // lbTitle
            // 
            this.lbTitle.Dock = System.Windows.Forms.DockStyle.Left;
            this.lbTitle.Location = new System.Drawing.Point(5, 5);
            this.lbTitle.Margin = new System.Windows.Forms.Padding(0);
            this.lbTitle.Name = "lbTitle";
            this.lbTitle.Size = new System.Drawing.Size(110, 29);
            this.lbTitle.TabIndex = 4;
            this.lbTitle.Text = "AdmUnit";
            this.lbTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // AdmListBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.metroPanel1);
            this.Name = "AdmListBox";
            this.Size = new System.Drawing.Size(310, 39);
            this.metroPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.admUnitBindingSource)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private MetroFramework.Controls.MetroPanel metroPanel1;
        private MetroFramework.Controls.MetroComboBox cmbAdmUnit;
        private System.Windows.Forms.BindingSource admUnitBindingSource;
        private MetroFramework.Controls.MetroLabel lbTitle;
    }
}
