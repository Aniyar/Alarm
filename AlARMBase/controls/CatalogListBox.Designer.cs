﻿namespace ALARm.controls
{
    partial class CatalogListBox
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
            this.catalogBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.lbTitle = new MetroFramework.Controls.MetroLabel();
            this.metroPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.catalogBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // metroPanel1
            // 
            this.metroPanel1.Controls.Add(this.cmbAdmUnit);
            this.metroPanel1.Controls.Add(this.lbTitle);
            this.metroPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.metroPanel1.HorizontalScrollbarBarColor = true;
            this.metroPanel1.HorizontalScrollbarHighlightOnWheel = false;
            this.metroPanel1.HorizontalScrollbarSize = 15;
            this.metroPanel1.Location = new System.Drawing.Point(0, 0);
            this.metroPanel1.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.metroPanel1.Name = "metroPanel1";
            this.metroPanel1.Padding = new System.Windows.Forms.Padding(8);
            this.metroPanel1.Size = new System.Drawing.Size(465, 60);
            this.metroPanel1.TabIndex = 1;
            this.metroPanel1.VerticalScrollbarBarColor = true;
            this.metroPanel1.VerticalScrollbarHighlightOnWheel = false;
            this.metroPanel1.VerticalScrollbarSize = 15;
            // 
            // cmbAdmUnit
            // 
            this.cmbAdmUnit.DataSource = this.catalogBindingSource;
            this.cmbAdmUnit.DisplayMember = "Name";
            this.cmbAdmUnit.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cmbAdmUnit.FormattingEnabled = true;
            this.cmbAdmUnit.ItemHeight = 23;
            this.cmbAdmUnit.Location = new System.Drawing.Point(158, 8);
            this.cmbAdmUnit.Margin = new System.Windows.Forms.Padding(0);
            this.cmbAdmUnit.Name = "cmbAdmUnit";
            this.cmbAdmUnit.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.cmbAdmUnit.Size = new System.Drawing.Size(299, 29);
            this.cmbAdmUnit.TabIndex = 5;
            this.cmbAdmUnit.UseSelectable = true;
            this.cmbAdmUnit.ValueMember = "Id";
            // 
            // catalogBindingSource
            // 
            this.catalogBindingSource.DataSource = typeof(ALARm.Core.Catalog);
            // 
            // lbTitle
            // 
            this.lbTitle.Dock = System.Windows.Forms.DockStyle.Left;
            this.lbTitle.Location = new System.Drawing.Point(8, 8);
            this.lbTitle.Margin = new System.Windows.Forms.Padding(0);
            this.lbTitle.Name = "lbTitle";
            this.lbTitle.Size = new System.Drawing.Size(150, 44);
            this.lbTitle.TabIndex = 4;
            this.lbTitle.Text = "Тип";
            this.lbTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // CatalogListBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.metroPanel1);
            this.Name = "CatalogListBox";
            this.Size = new System.Drawing.Size(465, 60);
            this.metroPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.catalogBindingSource)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private MetroFramework.Controls.MetroPanel metroPanel1;
        private MetroFramework.Controls.MetroComboBox cmbAdmUnit;
        private MetroFramework.Controls.MetroLabel lbTitle;
        private System.Windows.Forms.BindingSource catalogBindingSource;
    }
}
