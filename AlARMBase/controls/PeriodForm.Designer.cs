namespace ALARm.controls
{
    partial class PeriodForm
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
            this.metroLabel1 = new MetroFramework.Controls.MetroLabel();
            this.metroLabel2 = new MetroFramework.Controls.MetroLabel();
            this.mdtStartDate = new MetroFramework.Controls.MetroDateTime();
            this.mdtFinalDate = new MetroFramework.Controls.MetroDateTime();
            this.btnCancel = new MetroFramework.Controls.MetroButton();
            this.btnSave = new MetroFramework.Controls.MetroButton();
            this.SuspendLayout();
            // 
            // metroLabel1
            // 
            this.metroLabel1.AutoSize = true;
            this.metroLabel1.Location = new System.Drawing.Point(51, 69);
            this.metroLabel1.Name = "metroLabel1";
            this.metroLabel1.Size = new System.Drawing.Size(84, 19);
            this.metroLabel1.TabIndex = 0;
            this.metroLabel1.Text = "Дата начала";
            // 
            // metroLabel2
            // 
            this.metroLabel2.AutoSize = true;
            this.metroLabel2.Location = new System.Drawing.Point(51, 102);
            this.metroLabel2.Name = "metroLabel2";
            this.metroLabel2.Size = new System.Drawing.Size(78, 19);
            this.metroLabel2.TabIndex = 1;
            this.metroLabel2.Text = "Дата конца";
            // 
            // mdtStartDate
            // 
            this.mdtStartDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.mdtStartDate.Location = new System.Drawing.Point(151, 64);
            this.mdtStartDate.MinimumSize = new System.Drawing.Size(0, 29);
            this.mdtStartDate.Name = "mdtStartDate";
            this.mdtStartDate.Size = new System.Drawing.Size(200, 29);
            this.mdtStartDate.TabIndex = 2;
            // 
            // mdtFinalDate
            // 
            this.mdtFinalDate.Format = System.Windows.Forms.DateTimePickerFormat.Short;
            this.mdtFinalDate.Location = new System.Drawing.Point(151, 97);
            this.mdtFinalDate.MinimumSize = new System.Drawing.Size(0, 29);
            this.mdtFinalDate.Name = "mdtFinalDate";
            this.mdtFinalDate.Size = new System.Drawing.Size(200, 29);
            this.mdtFinalDate.TabIndex = 3;
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(276, 138);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 7;
            this.btnCancel.Text = "Отмена";
            this.btnCancel.UseSelectable = true;
            this.btnCancel.Click += new System.EventHandler(this.btnCancel_Click);
            // 
            // btnSave
            // 
            this.btnSave.Location = new System.Drawing.Point(195, 138);
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(75, 23);
            this.btnSave.TabIndex = 6;
            this.btnSave.Text = "Сохранить";
            this.btnSave.UseSelectable = true;
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // PeriodForm
            // 
            this.AcceptButton = this.btnSave;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(378, 183);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.mdtFinalDate);
            this.Controls.Add(this.mdtStartDate);
            this.Controls.Add(this.metroLabel2);
            this.Controls.Add(this.metroLabel1);
            this.KeyPreview = true;
            this.MaximizeBox = false;
            this.Name = "PeriodForm";
            this.Resizable = false;
            this.Text = "Добавление записи";
            //this.Shown += new System.EventHandler(this.PeriodsForm_Shown);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private MetroFramework.Controls.MetroLabel metroLabel1;
        private MetroFramework.Controls.MetroLabel metroLabel2;
        private MetroFramework.Controls.MetroDateTime mdtStartDate;
        private MetroFramework.Controls.MetroDateTime mdtFinalDate;
        private MetroFramework.Controls.MetroButton btnCancel;
        private MetroFramework.Controls.MetroButton btnSave;
    }
}