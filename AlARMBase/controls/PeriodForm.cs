using ALARm.Core;
using System;
using System.Windows.Forms;

namespace ALARm.controls
{
    public partial class PeriodForm : MetroFramework.Forms.MetroForm
    {
        public DialogResult Result = DialogResult.Cancel;
        public Period Period;
        public PeriodForm()
        {
            InitializeComponent();
            mdtStartDate.Value = DateTime.Now;
            mdtFinalDate.Value = mdtStartDate.Value.AddDays(1);
        }
        public PeriodForm(Period period)
        {
            InitializeComponent();
            Text = "Изменение записи";
            mdtStartDate.Value = period.Start_Date;
            mdtFinalDate.Value = period.Final_Date;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
          
            if (mdtFinalDate.Value.Date<= mdtStartDate.Value.Date)
            {
                MetroFramework.MetroMessageBox.Show(this, alerts.date_overlaps, alerts.inserting, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Period = new Period { Start_Date = mdtStartDate.Value.Date, Final_Date = mdtFinalDate.Value.Date };
            Result = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Result = DialogResult.Cancel;
            Close();
        }
    }
}
