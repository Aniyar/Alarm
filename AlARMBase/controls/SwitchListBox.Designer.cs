namespace ALARm.controls
{
    partial class SwitchListBox
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
            this.metroPanel1 = new MetroFramework.Controls.MetroPanel();
            this.cmbSwitchUnit = new MetroFramework.Controls.MetroComboBox();
            this.switchBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.lbTitle = new MetroFramework.Controls.MetroLabel();
            this.metroPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.switchBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // metroPanel1
            // 
            this.metroPanel1.Controls.Add(this.cmbSwitchUnit);
            this.metroPanel1.Controls.Add(this.lbTitle);
            this.metroPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.metroPanel1.HorizontalScrollbarBarColor = true;
            this.metroPanel1.HorizontalScrollbarHighlightOnWheel = false;
            this.metroPanel1.HorizontalScrollbarSize = 10;
            this.metroPanel1.Location = new System.Drawing.Point(0, 0);
            this.metroPanel1.Name = "metroPanel1";
            this.metroPanel1.Padding = new System.Windows.Forms.Padding(5);
            this.metroPanel1.Size = new System.Drawing.Size(310, 39);
            this.metroPanel1.TabIndex = 1;
            this.metroPanel1.VerticalScrollbarBarColor = true;
            this.metroPanel1.VerticalScrollbarHighlightOnWheel = false;
            this.metroPanel1.VerticalScrollbarSize = 10;
            // 
            // cmbSwitchUnit
            // 
            this.cmbSwitchUnit.DataSource = this.switchBindingSource;
            this.cmbSwitchUnit.DisplayMember = "Num";
            this.cmbSwitchUnit.Dock = System.Windows.Forms.DockStyle.Fill;
            this.cmbSwitchUnit.FormattingEnabled = true;
            this.cmbSwitchUnit.ItemHeight = 23;
            this.cmbSwitchUnit.Location = new System.Drawing.Point(115, 5);
            this.cmbSwitchUnit.Name = "cmbSwitchUnit";
            this.cmbSwitchUnit.RightToLeft = System.Windows.Forms.RightToLeft.No;
            this.cmbSwitchUnit.Size = new System.Drawing.Size(190, 29);
            this.cmbSwitchUnit.TabIndex = 5;
            this.cmbSwitchUnit.UseSelectable = true;
            this.cmbSwitchUnit.ValueMember = "Num";
            this.cmbSwitchUnit.SelectedIndexChanged += new System.EventHandler(this.CmbSwitchUnit_SelectedIndexChanged);
            // 
            // switchBindingSource
            // 
            this.switchBindingSource.DataSource = typeof(ALARm.Core.Switch);
            // 
            // lbTitle
            // 
            this.lbTitle.Dock = System.Windows.Forms.DockStyle.Left;
            this.lbTitle.Location = new System.Drawing.Point(5, 5);
            this.lbTitle.Margin = new System.Windows.Forms.Padding(0);
            this.lbTitle.Name = "lbTitle";
            this.lbTitle.Size = new System.Drawing.Size(110, 29);
            this.lbTitle.TabIndex = 4;
            this.lbTitle.Text = "SwitchUnit";
            this.lbTitle.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // SwitchListBox
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.metroPanel1);
            this.Name = "SwitchListBox";
            this.Size = new System.Drawing.Size(310, 39);
            this.metroPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.switchBindingSource)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private MetroFramework.Controls.MetroPanel metroPanel1;
        private MetroFramework.Controls.MetroComboBox cmbSwitchUnit;
        private MetroFramework.Controls.MetroLabel lbTitle;
        private System.Windows.Forms.BindingSource switchBindingSource;
    }
}
