using ALARm.Core;
using ALARm.Services;
using System;
using System.Windows.Forms;

namespace ALARm.controls
{
    public partial class ImportForm : MetroFramework.Forms.MetroForm
    {
        public AdmRoad admRoad;
        public AdmNod admNod;
        public Period Period;
        public DialogResult Result = DialogResult.Cancel;

        public ImportForm()
        {
            InitializeComponent();
            mdtStartDate.Value = DateTime.Now;
            mdtFinalDate.Value = mdtStartDate.Value.AddDays(1);
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (tbCode.Text == string.Empty || tbName.Text == string.Empty || tbCodeNOD.Text == string.Empty || tbNameNOD.Text == string.Empty)
            {
                MetroFramework.MetroMessageBox.Show(this, alerts.filling_error, alerts.inserting, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (mdtFinalDate.Value.Date <= mdtStartDate.Value.Date)
            {
                MetroFramework.MetroMessageBox.Show(this, alerts.date_overlaps, alerts.inserting, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            admRoad = new AdmRoad {
                Code = tbCode.Text,
                Name = tbName.Text
            };
            admNod = new AdmNod {
                Code = tbCodeNOD.Text,
                Name = tbNameNOD.Text
            };
            Period = new Period { 
                Start_Date = mdtStartDate.Value.Date, 
                Final_Date = mdtFinalDate.Value.Date 
            };

            Result = DialogResult.OK;
            Close();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            Result = DialogResult.Cancel;
            Close();
        }

        private void tbCode_KeyPress(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }
    }
}
