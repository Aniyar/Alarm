namespace ALARm.controls
{
    partial class MetroSpinEdit
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
            this.valueTextBox = new MetroFramework.Controls.MetroTextBox();
            this.metroPanel1 = new MetroFramework.Controls.MetroPanel();
            this.downBtn = new MetroFramework.Controls.MetroButton();
            this.upBtn = new MetroFramework.Controls.MetroButton();
            this.metroPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // valueTextBox
            // 
            // 
            // 
            // 
            this.valueTextBox.CustomButton.Image = null;
            this.valueTextBox.CustomButton.Location = new System.Drawing.Point(239, 2);
            this.valueTextBox.CustomButton.Name = "";
            this.valueTextBox.CustomButton.Size = new System.Drawing.Size(33, 33);
            this.valueTextBox.CustomButton.Style = MetroFramework.MetroColorStyle.Blue;
            this.valueTextBox.CustomButton.TabIndex = 1;
            this.valueTextBox.CustomButton.Theme = MetroFramework.MetroThemeStyle.Light;
            this.valueTextBox.CustomButton.UseSelectable = true;
            this.valueTextBox.CustomButton.Visible = false;
            this.valueTextBox.Dock = System.Windows.Forms.DockStyle.Fill;
            this.valueTextBox.Lines = new string[] {
        "1520"};
            this.valueTextBox.Location = new System.Drawing.Point(0, 0);
            this.valueTextBox.MaxLength = 32767;
            this.valueTextBox.Name = "valueTextBox";
            this.valueTextBox.PasswordChar = '\0';
            this.valueTextBox.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.valueTextBox.SelectedText = "";
            this.valueTextBox.SelectionLength = 0;
            this.valueTextBox.SelectionStart = 0;
            this.valueTextBox.ShortcutsEnabled = true;
            this.valueTextBox.Size = new System.Drawing.Size(275, 38);
            this.valueTextBox.TabIndex = 0;
            this.valueTextBox.Text = "1520";
            this.valueTextBox.UseSelectable = true;
            this.valueTextBox.WaterMarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
            this.valueTextBox.WaterMarkFont = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Pixel);
            
            this.valueTextBox.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.DigitKeyFilter);
            // 
            // metroPanel1
            // 
            this.metroPanel1.BackgroundImage = global::ALARm.Properties.Resources.add;
            this.metroPanel1.Controls.Add(this.downBtn);
            this.metroPanel1.Controls.Add(this.upBtn);
            this.metroPanel1.Dock = System.Windows.Forms.DockStyle.Right;
            this.metroPanel1.HorizontalScrollbarBarColor = true;
            this.metroPanel1.HorizontalScrollbarHighlightOnWheel = false;
            this.metroPanel1.HorizontalScrollbarSize = 10;
            this.metroPanel1.Location = new System.Drawing.Point(247, 0);
            this.metroPanel1.Name = "metroPanel1";
            this.metroPanel1.Size = new System.Drawing.Size(28, 38);
            this.metroPanel1.TabIndex = 1;
            this.metroPanel1.VerticalScrollbarBarColor = true;
            this.metroPanel1.VerticalScrollbarHighlightOnWheel = false;
            this.metroPanel1.VerticalScrollbarSize = 10;
            // 
            // downBtn
            // 
            this.downBtn.Dock = System.Windows.Forms.DockStyle.Top;
            this.downBtn.Location = new System.Drawing.Point(0, 19);
            this.downBtn.Name = "downBtn";
            this.downBtn.Size = new System.Drawing.Size(28, 19);
            this.downBtn.TabIndex = 3;
            this.downBtn.Text = "▼";
            this.downBtn.UseSelectable = true;
            this.downBtn.Click += new System.EventHandler(this.downBtn_Click);
            // 
            // upBtn
            // 
            this.upBtn.Dock = System.Windows.Forms.DockStyle.Top;
            this.upBtn.Location = new System.Drawing.Point(0, 0);
            this.upBtn.Name = "upBtn";
            this.upBtn.Size = new System.Drawing.Size(28, 19);
            this.upBtn.TabIndex = 2;
            this.upBtn.Text = "▲";
            this.upBtn.UseSelectable = true;
            this.upBtn.Click += new System.EventHandler(this.upBtn_Click);
            // 
            // MetroSpinEdit
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.metroPanel1);
            this.Controls.Add(this.valueTextBox);
            this.Name = "MetroSpinEdit";
            this.Size = new System.Drawing.Size(275, 38);
            this.metroPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private MetroFramework.Controls.MetroTextBox valueTextBox;
        private MetroFramework.Controls.MetroPanel metroPanel1;
        private MetroFramework.Controls.MetroButton downBtn;
        private MetroFramework.Controls.MetroButton upBtn;
    }
}
