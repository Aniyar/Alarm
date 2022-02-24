using ALARm.Core;
using ALARm.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ALARm.controls
{
    public partial class CheckSectionForm : MetroFramework.Forms.MetroForm
    {
        public CheckSection checkSection;
        private BindingSource existsSource;
        public DialogResult result = DialogResult.Cancel;
        public CheckSectionForm()
        {
            InitializeComponent();
        }
        public void SetCheckSectionForm(CheckSection obj)
        {
            Text = "Изменение записи";
            coordControl.SetValue(obj.Start_Km, obj.Start_M, obj.Final_Km, obj.Final_M);
            tbAvgLvl.Text = obj.Avg_level.ToString();
            tbAvgW.Text = obj.Avg_width.ToString();
            tbSkoLvl.Text = obj.Sko_level.ToString();
            tbSkoW.Text = obj.Sko_width.ToString();
        }

        internal void SetExistingSource(BindingSource bindingSource)
        {
            existsSource = bindingSource;
        }
        public void ClearForm()
        {
            coordControl.Clear();
            tbAvgLvl.Text = String.Empty;
            tbAvgW.Text = String.Empty;
            tbSkoLvl.Text = String.Empty;
            tbSkoW.Text = String.Empty;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            double avgl = 0.0, avgw = 0.0, skol = 0.0, skow = 0.0;

            if (coordControl.CorrectFilled == false || !double.TryParse(tbAvgLvl.Text, out avgl) || !double.TryParse(tbAvgW.Text, out avgw) ||
                !double.TryParse(tbSkoW.Text, out skow) || !double.TryParse(tbSkoLvl.Text, out skol))
            {
                MetroFramework.MetroMessageBox.Show(this, alerts.insert_error + " " + alerts.check_fields_filling, alerts.inserting, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            checkSection = new CheckSection {
                Start_Km = coordControl.StartKm,
                Start_M = coordControl.StartM,
                Final_Km = coordControl.FinalKm,
                Final_M = coordControl.FinalM,
                Avg_level = avgl,
                Avg_width = avgw,
                Sko_level = skol,
                Sko_width = skow
            };
            result = DialogResult.OK;
            Close();
        }
    }
}
