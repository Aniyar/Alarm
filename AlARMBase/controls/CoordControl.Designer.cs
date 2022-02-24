namespace ALARm.controls
{
    partial class CoordControl
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
            this.mpStartKm = new MetroFramework.Controls.MetroPanel();
            this.tbStartKm = new MetroFramework.Controls.MetroTextBox();
            this.mlStartKm = new MetroFramework.Controls.MetroLabel();
            this.mpStartM = new MetroFramework.Controls.MetroPanel();
            this.tbStartM = new MetroFramework.Controls.MetroTextBox();
            this.mlStartM = new MetroFramework.Controls.MetroLabel();
            this.mpFinalKm = new MetroFramework.Controls.MetroPanel();
            this.tbFinalKm = new MetroFramework.Controls.MetroTextBox();
            this.mlFinalKm = new MetroFramework.Controls.MetroLabel();
            this.mpFinalM = new MetroFramework.Controls.MetroPanel();
            this.tbFinalM = new MetroFramework.Controls.MetroTextBox();
            this.mlFinalM = new MetroFramework.Controls.MetroLabel();
            this.mpStartKm.SuspendLayout();
            this.mpStartM.SuspendLayout();
            this.mpFinalKm.SuspendLayout();
            this.mpFinalM.SuspendLayout();
            this.SuspendLayout();
            // 
            // mpStartKm
            // 
            this.mpStartKm.Controls.Add(this.tbStartKm);
            this.mpStartKm.Controls.Add(this.mlStartKm);
            this.mpStartKm.Dock = System.Windows.Forms.DockStyle.Top;
            this.mpStartKm.HorizontalScrollbarBarColor = true;
            this.mpStartKm.HorizontalScrollbarHighlightOnWheel = false;
            this.mpStartKm.HorizontalScrollbarSize = 10;
            this.mpStartKm.Location = new System.Drawing.Point(0, 0);
            this.mpStartKm.Margin = new System.Windows.Forms.Padding(0);
            this.mpStartKm.Name = "mpStartKm";
            this.mpStartKm.Padding = new System.Windows.Forms.Padding(5);
            this.mpStartKm.Size = new System.Drawing.Size(310, 39);
            this.mpStartKm.TabIndex = 0;
            this.mpStartKm.VerticalScrollbarBarColor = true;
            this.mpStartKm.VerticalScrollbarHighlightOnWheel = false;
            this.mpStartKm.VerticalScrollbarSize = 10;
            // 
            // tbStartKm
            // 
            // 
            // 
            // 
            this.tbStartKm.CustomButton.Image = null;
            this.tbStartKm.CustomButton.Location = new System.Drawing.Point(162, 1);
            this.tbStartKm.CustomButton.Name = "";
            this.tbStartKm.CustomButton.Size = new System.Drawing.Size(27, 27);
            this.tbStartKm.CustomButton.Style = MetroFramework.MetroColorStyle.Blue;
            this.tbStartKm.CustomButton.TabIndex = 1;
            this.tbStartKm.CustomButton.Theme = MetroFramework.MetroThemeStyle.Light;
            this.tbStartKm.CustomButton.UseSelectable = true;
            this.tbStartKm.CustomButton.Visible = false;
            this.tbStartKm.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbStartKm.Lines = new string[0];
            this.tbStartKm.Location = new System.Drawing.Point(115, 5);
            this.tbStartKm.MaxLength = 32767;
            this.tbStartKm.Name = "tbStartKm";
            this.tbStartKm.PasswordChar = '\0';
            this.tbStartKm.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.tbStartKm.SelectedText = "";
            this.tbStartKm.SelectionLength = 0;
            this.tbStartKm.SelectionStart = 0;
            this.tbStartKm.ShortcutsEnabled = true;
            this.tbStartKm.Size = new System.Drawing.Size(190, 29);
            this.tbStartKm.TabIndex = 2;
            this.tbStartKm.UseSelectable = true;
            this.tbStartKm.WaterMarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
            this.tbStartKm.WaterMarkFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Pixel);
            this.tbStartKm.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.DigitKeyFilter);
            this.tbStartKm.KeyUp += new System.Windows.Forms.KeyEventHandler(this.TbStartKm_KeyUp);
            // 
            // mlStartKm
            // 
            this.mlStartKm.Dock = System.Windows.Forms.DockStyle.Left;
            this.mlStartKm.Location = new System.Drawing.Point(5, 5);
            this.mlStartKm.Name = "mlStartKm";
            this.mlStartKm.Size = new System.Drawing.Size(110, 29);
            this.mlStartKm.TabIndex = 1;
            this.mlStartKm.Text = "Начало (км)";
            this.mlStartKm.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // mpStartM
            // 
            this.mpStartM.Controls.Add(this.tbStartM);
            this.mpStartM.Controls.Add(this.mlStartM);
            this.mpStartM.Dock = System.Windows.Forms.DockStyle.Top;
            this.mpStartM.HorizontalScrollbarBarColor = true;
            this.mpStartM.HorizontalScrollbarHighlightOnWheel = false;
            this.mpStartM.HorizontalScrollbarSize = 10;
            this.mpStartM.Location = new System.Drawing.Point(0, 39);
            this.mpStartM.Margin = new System.Windows.Forms.Padding(0);
            this.mpStartM.Name = "mpStartM";
            this.mpStartM.Padding = new System.Windows.Forms.Padding(5);
            this.mpStartM.Size = new System.Drawing.Size(310, 39);
            this.mpStartM.TabIndex = 2;
            this.mpStartM.VerticalScrollbarBarColor = true;
            this.mpStartM.VerticalScrollbarHighlightOnWheel = false;
            this.mpStartM.VerticalScrollbarSize = 10;
            // 
            // tbStartM
            // 
            // 
            // 
            // 
            this.tbStartM.CustomButton.Image = null;
            this.tbStartM.CustomButton.Location = new System.Drawing.Point(162, 1);
            this.tbStartM.CustomButton.Name = "";
            this.tbStartM.CustomButton.Size = new System.Drawing.Size(27, 27);
            this.tbStartM.CustomButton.Style = MetroFramework.MetroColorStyle.Blue;
            this.tbStartM.CustomButton.TabIndex = 1;
            this.tbStartM.CustomButton.Theme = MetroFramework.MetroThemeStyle.Light;
            this.tbStartM.CustomButton.UseSelectable = true;
            this.tbStartM.CustomButton.Visible = false;
            this.tbStartM.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbStartM.Lines = new string[0];
            this.tbStartM.Location = new System.Drawing.Point(115, 5);
            this.tbStartM.MaxLength = 32767;
            this.tbStartM.Name = "tbStartM";
            this.tbStartM.PasswordChar = '\0';
            this.tbStartM.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.tbStartM.SelectedText = "";
            this.tbStartM.SelectionLength = 0;
            this.tbStartM.SelectionStart = 0;
            this.tbStartM.ShortcutsEnabled = true;
            this.tbStartM.Size = new System.Drawing.Size(190, 29);
            this.tbStartM.TabIndex = 2;
            this.tbStartM.UseSelectable = true;
            this.tbStartM.WaterMarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
            this.tbStartM.WaterMarkFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Pixel);
            this.tbStartM.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.DigitKeyFilter);
            this.tbStartM.KeyUp += new System.Windows.Forms.KeyEventHandler(this.TbStartM_KeyUp);
            // 
            // mlStartM
            // 
            this.mlStartM.Dock = System.Windows.Forms.DockStyle.Left;
            this.mlStartM.Location = new System.Drawing.Point(5, 5);
            this.mlStartM.Name = "mlStartM";
            this.mlStartM.Size = new System.Drawing.Size(110, 29);
            this.mlStartM.TabIndex = 1;
            this.mlStartM.Text = "Начало (м)";
            this.mlStartM.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // mpFinalKm
            // 
            this.mpFinalKm.Controls.Add(this.tbFinalKm);
            this.mpFinalKm.Controls.Add(this.mlFinalKm);
            this.mpFinalKm.Dock = System.Windows.Forms.DockStyle.Top;
            this.mpFinalKm.HorizontalScrollbarBarColor = true;
            this.mpFinalKm.HorizontalScrollbarHighlightOnWheel = false;
            this.mpFinalKm.HorizontalScrollbarSize = 10;
            this.mpFinalKm.Location = new System.Drawing.Point(0, 78);
            this.mpFinalKm.Margin = new System.Windows.Forms.Padding(0);
            this.mpFinalKm.Name = "mpFinalKm";
            this.mpFinalKm.Padding = new System.Windows.Forms.Padding(5);
            this.mpFinalKm.Size = new System.Drawing.Size(310, 39);
            this.mpFinalKm.TabIndex = 3;
            this.mpFinalKm.VerticalScrollbarBarColor = true;
            this.mpFinalKm.VerticalScrollbarHighlightOnWheel = false;
            this.mpFinalKm.VerticalScrollbarSize = 10;
            // 
            // tbFinalKm
            // 
            // 
            // 
            // 
            this.tbFinalKm.CustomButton.Image = null;
            this.tbFinalKm.CustomButton.Location = new System.Drawing.Point(162, 1);
            this.tbFinalKm.CustomButton.Name = "";
            this.tbFinalKm.CustomButton.Size = new System.Drawing.Size(27, 27);
            this.tbFinalKm.CustomButton.Style = MetroFramework.MetroColorStyle.Blue;
            this.tbFinalKm.CustomButton.TabIndex = 1;
            this.tbFinalKm.CustomButton.Theme = MetroFramework.MetroThemeStyle.Light;
            this.tbFinalKm.CustomButton.UseSelectable = true;
            this.tbFinalKm.CustomButton.Visible = false;
            this.tbFinalKm.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbFinalKm.Lines = new string[0];
            this.tbFinalKm.Location = new System.Drawing.Point(115, 5);
            this.tbFinalKm.Margin = new System.Windows.Forms.Padding(0);
            this.tbFinalKm.MaxLength = 32767;
            this.tbFinalKm.Name = "tbFinalKm";
            this.tbFinalKm.PasswordChar = '\0';
            this.tbFinalKm.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.tbFinalKm.SelectedText = "";
            this.tbFinalKm.SelectionLength = 0;
            this.tbFinalKm.SelectionStart = 0;
            this.tbFinalKm.ShortcutsEnabled = true;
            this.tbFinalKm.Size = new System.Drawing.Size(190, 29);
            this.tbFinalKm.TabIndex = 2;
            this.tbFinalKm.UseSelectable = true;
            this.tbFinalKm.WaterMarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
            this.tbFinalKm.WaterMarkFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Pixel);
            this.tbFinalKm.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.DigitKeyFilter);
            this.tbFinalKm.KeyUp += new System.Windows.Forms.KeyEventHandler(this.TbFinalKm_KeyUp);
            // 
            // mlFinalKm
            // 
            this.mlFinalKm.Dock = System.Windows.Forms.DockStyle.Left;
            this.mlFinalKm.Location = new System.Drawing.Point(5, 5);
            this.mlFinalKm.Name = "mlFinalKm";
            this.mlFinalKm.Size = new System.Drawing.Size(110, 29);
            this.mlFinalKm.TabIndex = 1;
            this.mlFinalKm.Text = "Конец (км)";
            this.mlFinalKm.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // mpFinalM
            // 
            this.mpFinalM.Controls.Add(this.tbFinalM);
            this.mpFinalM.Controls.Add(this.mlFinalM);
            this.mpFinalM.Dock = System.Windows.Forms.DockStyle.Top;
            this.mpFinalM.HorizontalScrollbarBarColor = true;
            this.mpFinalM.HorizontalScrollbarHighlightOnWheel = false;
            this.mpFinalM.HorizontalScrollbarSize = 10;
            this.mpFinalM.Location = new System.Drawing.Point(0, 117);
            this.mpFinalM.Margin = new System.Windows.Forms.Padding(0);
            this.mpFinalM.Name = "mpFinalM";
            this.mpFinalM.Padding = new System.Windows.Forms.Padding(5);
            this.mpFinalM.Size = new System.Drawing.Size(310, 39);
            this.mpFinalM.TabIndex = 4;
            this.mpFinalM.VerticalScrollbarBarColor = true;
            this.mpFinalM.VerticalScrollbarHighlightOnWheel = false;
            this.mpFinalM.VerticalScrollbarSize = 10;
            // 
            // tbFinalM
            // 
            // 
            // 
            // 
            this.tbFinalM.CustomButton.Image = null;
            this.tbFinalM.CustomButton.Location = new System.Drawing.Point(162, 1);
            this.tbFinalM.CustomButton.Name = "";
            this.tbFinalM.CustomButton.Size = new System.Drawing.Size(27, 27);
            this.tbFinalM.CustomButton.Style = MetroFramework.MetroColorStyle.Blue;
            this.tbFinalM.CustomButton.TabIndex = 1;
            this.tbFinalM.CustomButton.Theme = MetroFramework.MetroThemeStyle.Light;
            this.tbFinalM.CustomButton.UseSelectable = true;
            this.tbFinalM.CustomButton.Visible = false;
            this.tbFinalM.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tbFinalM.Lines = new string[0];
            this.tbFinalM.Location = new System.Drawing.Point(115, 5);
            this.tbFinalM.MaxLength = 32767;
            this.tbFinalM.Name = "tbFinalM";
            this.tbFinalM.PasswordChar = '\0';
            this.tbFinalM.ScrollBars = System.Windows.Forms.ScrollBars.None;
            this.tbFinalM.SelectedText = "";
            this.tbFinalM.SelectionLength = 0;
            this.tbFinalM.SelectionStart = 0;
            this.tbFinalM.ShortcutsEnabled = true;
            this.tbFinalM.Size = new System.Drawing.Size(190, 29);
            this.tbFinalM.TabIndex = 2;
            this.tbFinalM.UseSelectable = true;
            this.tbFinalM.WaterMarkColor = System.Drawing.Color.FromArgb(((int)(((byte)(109)))), ((int)(((byte)(109)))), ((int)(((byte)(109)))));
            this.tbFinalM.WaterMarkFont = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Italic, System.Drawing.GraphicsUnit.Pixel);
            this.tbFinalM.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.DigitKeyFilter);
            this.tbFinalM.KeyUp += new System.Windows.Forms.KeyEventHandler(this.TbFinalM_KeyUp);
            // 
            // mlFinalM
            // 
            this.mlFinalM.Dock = System.Windows.Forms.DockStyle.Left;
            this.mlFinalM.Location = new System.Drawing.Point(5, 5);
            this.mlFinalM.Name = "mlFinalM";
            this.mlFinalM.Size = new System.Drawing.Size(110, 29);
            this.mlFinalM.TabIndex = 1;
            this.mlFinalM.Text = "Конец (м)";
            this.mlFinalM.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // CoordControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoSize = true;
            this.Controls.Add(this.mpFinalM);
            this.Controls.Add(this.mpFinalKm);
            this.Controls.Add(this.mpStartM);
            this.Controls.Add(this.mpStartKm);
            this.Name = "CoordControl";
            this.Size = new System.Drawing.Size(310, 156);
            this.mpStartKm.ResumeLayout(false);
            this.mpStartM.ResumeLayout(false);
            this.mpFinalKm.ResumeLayout(false);
            this.mpFinalM.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private MetroFramework.Controls.MetroPanel mpStartKm;
        private MetroFramework.Controls.MetroTextBox tbStartKm;
        private MetroFramework.Controls.MetroLabel mlStartKm;
        private MetroFramework.Controls.MetroPanel mpStartM;
        private MetroFramework.Controls.MetroTextBox tbStartM;
        private MetroFramework.Controls.MetroLabel mlStartM;
        private MetroFramework.Controls.MetroPanel mpFinalKm;
        private MetroFramework.Controls.MetroTextBox tbFinalKm;
        private MetroFramework.Controls.MetroLabel mlFinalKm;
        private MetroFramework.Controls.MetroPanel mpFinalM;
        private MetroFramework.Controls.MetroTextBox tbFinalM;
        private MetroFramework.Controls.MetroLabel mlFinalM;
    }
}
