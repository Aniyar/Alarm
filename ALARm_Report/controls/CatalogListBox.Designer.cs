namespace ALARm.controls
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
            this.lbReport = new System.Windows.Forms.ListBox();
            this.reportBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.lbTitle = new MetroFramework.Controls.MetroLabel();
            this.metroPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.reportBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // metroPanel1
            // 
            this.metroPanel1.Controls.Add(this.lbReport);
            this.metroPanel1.Controls.Add(this.lbTitle);
            this.metroPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.metroPanel1.HorizontalScrollbarBarColor = true;
            this.metroPanel1.HorizontalScrollbarHighlightOnWheel = false;
            this.metroPanel1.HorizontalScrollbarSize = 10;
            this.metroPanel1.Location = new System.Drawing.Point(0, 0);
            this.metroPanel1.Name = "metroPanel1";
            this.metroPanel1.Padding = new System.Windows.Forms.Padding(5);
            this.metroPanel1.Size = new System.Drawing.Size(310, 294);
            this.metroPanel1.TabIndex = 1;
            this.metroPanel1.VerticalScrollbarBarColor = true;
            this.metroPanel1.VerticalScrollbarHighlightOnWheel = false;
            this.metroPanel1.VerticalScrollbarSize = 10;
            // 
            // lbReport
            // 
            this.lbReport.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lbReport.DataSource = this.reportBindingSource;
            this.lbReport.DisplayMember = "Name";
            this.lbReport.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lbReport.FormattingEnabled = true;
            this.lbReport.Location = new System.Drawing.Point(5, 34);
            this.lbReport.Name = "lbReport";
            this.lbReport.Size = new System.Drawing.Size(300, 255);
            this.lbReport.TabIndex = 6;
            this.lbReport.ValueMember = "ID";
            // 
            // reportBindingSource
            // 
            this.reportBindingSource.DataSource = typeof(ALARm.Core.Report.ReportTemplate);
            // 
            // lbTitle
            // 
            this.lbTitle.BackColor = System.Drawing.SystemColors.Control;
            this.lbTitle.Dock = System.Windows.Forms.DockStyle.Top;
            this.lbTitle.Location = new System.Drawing.Point(5, 5);
            this.lbTitle.Margin = new System.Windows.Forms.Padding(0);
            this.lbTitle.Name = "lbTitle";
            this.lbTitle.Size = new System.Drawing.Size(300, 29);
            this.lbTitle.TabIndex = 4;
            this.lbTitle.Text = "Ведомости";
            this.lbTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.lbTitle.Theme = MetroFramework.MetroThemeStyle.Light;
            this.lbTitle.UseCustomBackColor = true;
            // 
            // CatalogListBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.metroPanel1);
            this.Margin = new System.Windows.Forms.Padding(2);
            this.Name = "CatalogListBox";
            this.Size = new System.Drawing.Size(310, 294);
            this.metroPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.reportBindingSource)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private MetroFramework.Controls.MetroPanel metroPanel1;
        private MetroFramework.Controls.MetroLabel lbTitle;
        private System.Windows.Forms.BindingSource reportBindingSource;
        private System.Windows.Forms.ListBox lbReport;
    }
}
