using System.Windows.Forms;
using MetroFramework.Controls;

namespace ALARm.controls
{
    partial class admControl
    {
        /// <summary> 
        /// Обязательная переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором компонентов

        /// <summary> 
        /// Требуемый метод для поддержки конструктора — не изменяйте 
        /// содержимое этого метода с помощью редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.topPanel = new MetroFramework.Controls.MetroPanel();
            this.metroLabel1 = new MetroFramework.Controls.MetroLabel();
            this.fillPanel = new MetroFramework.Controls.MetroPanel();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.admDistanceBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.metroPanel1 = new MetroFramework.Controls.MetroPanel();
            this.bodyPanel = new MetroFramework.Controls.MetroPanel();
            this.topPanel.SuspendLayout();
            this.fillPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.admDistanceBindingSource)).BeginInit();
            this.bodyPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // topPanel
            // 
            this.topPanel.Controls.Add(this.metroLabel1);
            this.topPanel.Dock = System.Windows.Forms.DockStyle.Top;
            this.topPanel.HorizontalScrollbarBarColor = true;
            this.topPanel.HorizontalScrollbarHighlightOnWheel = false;
            this.topPanel.HorizontalScrollbarSize = 10;
            this.topPanel.Location = new System.Drawing.Point(0, 0);
            this.topPanel.Margin = new System.Windows.Forms.Padding(3, 3, 3, 25);
            this.topPanel.Name = "topPanel";
            this.topPanel.Padding = new System.Windows.Forms.Padding(0, 0, 0, 7);
            this.topPanel.Size = new System.Drawing.Size(150, 23);
            this.topPanel.TabIndex = 0;
            this.topPanel.VerticalScrollbarBarColor = true;
            this.topPanel.VerticalScrollbarHighlightOnWheel = false;
            this.topPanel.VerticalScrollbarSize = 10;
            // 
            // metroLabel1
            // 
            this.metroLabel1.AutoSize = true;
            this.metroLabel1.Location = new System.Drawing.Point(0, 3);
            this.metroLabel1.Name = "metroLabel1";
            this.metroLabel1.Size = new System.Drawing.Size(0, 0);
            this.metroLabel1.TabIndex = 2;
            // 
            // fillPanel
            // 
            this.fillPanel.BackColor = System.Drawing.SystemColors.ControlDark;
            this.fillPanel.Controls.Add(this.listBox1);
            this.fillPanel.Controls.Add(this.metroPanel1);
            this.fillPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.fillPanel.HorizontalScrollbarBarColor = true;
            this.fillPanel.HorizontalScrollbarHighlightOnWheel = false;
            this.fillPanel.HorizontalScrollbarSize = 10;
            this.fillPanel.Location = new System.Drawing.Point(0, 0);
            this.fillPanel.Margin = new System.Windows.Forms.Padding(3, 25, 3, 3);
            this.fillPanel.Name = "fillPanel";
            this.fillPanel.Padding = new System.Windows.Forms.Padding(0, 25, 0, 0);
            this.fillPanel.Size = new System.Drawing.Size(150, 150);
            this.fillPanel.TabIndex = 1;
            this.fillPanel.VerticalScrollbarBarColor = true;
            this.fillPanel.VerticalScrollbarHighlightOnWheel = false;
            this.fillPanel.VerticalScrollbarSize = 10;
            // 
            // listBox1
            // 
            this.listBox1.DataSource = this.admDistanceBindingSource;
            this.listBox1.DisplayMember = "Name";
            this.listBox1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listBox1.FormattingEnabled = true;
            this.listBox1.Location = new System.Drawing.Point(0, 28);
            this.listBox1.Name = "listBox1";
            this.listBox1.Size = new System.Drawing.Size(150, 122);
            this.listBox1.TabIndex = 3;
            this.listBox1.ValueMember = "Id";
            this.listBox1.SelectedIndexChanged += new System.EventHandler(this.listBox1_SelectedIndexChanged);
            // 
            // admDistanceBindingSource
            // 
            this.admDistanceBindingSource.DataSource = typeof(ALARm.Core.AdmDistance);
            // 
            // metroPanel1
            // 
            this.metroPanel1.BackColor = System.Drawing.SystemColors.ScrollBar;
            this.metroPanel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.metroPanel1.ForeColor = System.Drawing.Color.Coral;
            this.metroPanel1.HorizontalScrollbarBarColor = true;
            this.metroPanel1.HorizontalScrollbarHighlightOnWheel = false;
            this.metroPanel1.HorizontalScrollbarSize = 10;
            this.metroPanel1.Location = new System.Drawing.Point(0, 25);
            this.metroPanel1.Name = "metroPanel1";
            this.metroPanel1.Size = new System.Drawing.Size(150, 3);
            this.metroPanel1.TabIndex = 2;
            this.metroPanel1.UseCustomBackColor = true;
            this.metroPanel1.VerticalScrollbarBarColor = true;
            this.metroPanel1.VerticalScrollbarHighlightOnWheel = false;
            this.metroPanel1.VerticalScrollbarSize = 10;
            // 
            // bodyPanel
            // 
            this.bodyPanel.Controls.Add(this.topPanel);
            this.bodyPanel.Controls.Add(this.fillPanel);
            this.bodyPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.bodyPanel.HorizontalScrollbarBarColor = true;
            this.bodyPanel.HorizontalScrollbarHighlightOnWheel = false;
            this.bodyPanel.HorizontalScrollbarSize = 10;
            this.bodyPanel.Location = new System.Drawing.Point(0, 0);
            this.bodyPanel.Name = "bodyPanel";
            this.bodyPanel.Size = new System.Drawing.Size(150, 150);
            this.bodyPanel.TabIndex = 0;
            this.bodyPanel.VerticalScrollbarBarColor = true;
            this.bodyPanel.VerticalScrollbarHighlightOnWheel = false;
            this.bodyPanel.VerticalScrollbarSize = 10;
            // 
            // admControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.bodyPanel);
            this.Name = "admControl";
            this.topPanel.ResumeLayout(false);
            this.topPanel.PerformLayout();
            this.fillPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.admDistanceBindingSource)).EndInit();
            this.bodyPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private MetroFramework.Controls.MetroPanel topPanel;
        private MetroFramework.Controls.MetroPanel fillPanel;
        private MetroFramework.Controls.MetroPanel bodyPanel;
        private MetroPanel metroPanel1;
        private MetroLabel metroLabel1;
        private ListBox listBox1;
        private BindingSource admDistanceBindingSource;
    }
}
