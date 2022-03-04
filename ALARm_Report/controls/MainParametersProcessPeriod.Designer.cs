namespace ALARm_Report.controls
{
    partial class MainParametersProcessPeriod
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
            this.components = new System.ComponentModel.Container();
            this.ProcessDataComboBox = new MetroFramework.Controls.MetroComboBox();
            this.mainParametersProcessBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.MainPanel = new MetroFramework.Controls.MetroPanel();
            this.ButtonsPanel = new MetroFramework.Controls.MetroPanel();
            this.CancelButton = new MetroFramework.Controls.MetroButton();
            this.ChooseButton = new MetroFramework.Controls.MetroButton();
            ((System.ComponentModel.ISupportInitialize)(this.mainParametersProcessBindingSource)).BeginInit();
            this.MainPanel.SuspendLayout();
            this.ButtonsPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // ProcessDataComboBox
            // 
            this.ProcessDataComboBox.DataSource = this.mainParametersProcessBindingSource;
            this.ProcessDataComboBox.DisplayMember = "Date_Vrem";
            this.ProcessDataComboBox.FormattingEnabled = true;
            this.ProcessDataComboBox.ItemHeight = 23;
            this.ProcessDataComboBox.Location = new System.Drawing.Point(24, 33);
            this.ProcessDataComboBox.Name = "ProcessDataComboBox";
            this.ProcessDataComboBox.Size = new System.Drawing.Size(204, 29);
            this.ProcessDataComboBox.TabIndex = 0;
            this.ProcessDataComboBox.UseSelectable = true;
            this.ProcessDataComboBox.ValueMember = "Id";
            this.ProcessDataComboBox.SelectedIndexChanged += new System.EventHandler(this.ProcessDataComboBox_SelectedIndexChanged);
            // 
            // mainParametersProcessBindingSource
            // 
            this.mainParametersProcessBindingSource.DataSource = typeof(ALARm.Core.MainParametersProcess);
            // 
            // MainPanel
            // 
            this.MainPanel.Controls.Add(this.ButtonsPanel);
            this.MainPanel.Controls.Add(this.ProcessDataComboBox);
            this.MainPanel.HorizontalScrollbarBarColor = true;
            this.MainPanel.HorizontalScrollbarHighlightOnWheel = false;
            this.MainPanel.HorizontalScrollbarSize = 10;
            this.MainPanel.Location = new System.Drawing.Point(-1, 0);
            this.MainPanel.Name = "MainPanel";
            this.MainPanel.Size = new System.Drawing.Size(259, 130);
            this.MainPanel.TabIndex = 1;
            this.MainPanel.VerticalScrollbarBarColor = true;
            this.MainPanel.VerticalScrollbarHighlightOnWheel = false;
            this.MainPanel.VerticalScrollbarSize = 10;
            // 
            // ButtonsPanel
            // 
            this.ButtonsPanel.Controls.Add(this.CancelButton);
            this.ButtonsPanel.Controls.Add(this.ChooseButton);
            this.ButtonsPanel.HorizontalScrollbarBarColor = true;
            this.ButtonsPanel.HorizontalScrollbarHighlightOnWheel = false;
            this.ButtonsPanel.HorizontalScrollbarSize = 10;
            this.ButtonsPanel.Location = new System.Drawing.Point(24, 83);
            this.ButtonsPanel.Name = "ButtonsPanel";
            this.ButtonsPanel.Size = new System.Drawing.Size(204, 32);
            this.ButtonsPanel.TabIndex = 2;
            this.ButtonsPanel.VerticalScrollbarBarColor = true;
            this.ButtonsPanel.VerticalScrollbarHighlightOnWheel = false;
            this.ButtonsPanel.VerticalScrollbarSize = 10;
            // 
            // CancelButton
            // 
            this.CancelButton.Location = new System.Drawing.Point(126, 3);
            this.CancelButton.Name = "CancelButton";
            this.CancelButton.Size = new System.Drawing.Size(75, 26);
            this.CancelButton.TabIndex = 3;
            this.CancelButton.Text = "Отмена";
            this.CancelButton.UseSelectable = true;
            this.CancelButton.Click += new System.EventHandler(this.CancelButton_Click);
            // 
            // ChooseButton
            // 
            this.ChooseButton.Location = new System.Drawing.Point(45, 3);
            this.ChooseButton.Name = "ChooseButton";
            this.ChooseButton.Size = new System.Drawing.Size(75, 26);
            this.ChooseButton.TabIndex = 2;
            this.ChooseButton.Text = "Выбрать";
            this.ChooseButton.UseSelectable = true;
            this.ChooseButton.Click += new System.EventHandler(this.ChooseButton_Click);
            // 
            // MainParametersProcessPeriod
            // 
            this.AcceptButton = this.ChooseButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(256, 130);
            this.Controls.Add(this.MainPanel);
            this.DisplayHeader = false;
            this.Movable = false;
            this.Name = "MainParametersProcessPeriod";
            this.Padding = new System.Windows.Forms.Padding(20, 30, 20, 20);
            this.Resizable = false;
            ((System.ComponentModel.ISupportInitialize)(this.mainParametersProcessBindingSource)).EndInit();
            this.MainPanel.ResumeLayout(false);
            this.ButtonsPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private MetroFramework.Controls.MetroComboBox ProcessDataComboBox;
        private MetroFramework.Controls.MetroPanel MainPanel;
        private MetroFramework.Controls.MetroPanel ButtonsPanel;
#pragma warning disable CS0108 // 'MainParametersProcessPeriod.CancelButton' hides inherited member 'Form.CancelButton'. Use the new keyword if hiding was intended.
        private MetroFramework.Controls.MetroButton CancelButton;
#pragma warning restore CS0108 // 'MainParametersProcessPeriod.CancelButton' hides inherited member 'Form.CancelButton'. Use the new keyword if hiding was intended.
        private MetroFramework.Controls.MetroButton ChooseButton;
        private System.Windows.Forms.BindingSource mainParametersProcessBindingSource;
    }
}