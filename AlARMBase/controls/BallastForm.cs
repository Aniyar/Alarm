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
    public partial class BallastForm : MetroFramework.Forms.MetroForm
    {
        public BallastType ballast;
        private BindingSource existsSource;
        public DialogResult result = DialogResult.Cancel;
        public BallastForm()
        {
            InitializeComponent();
        }
        public void SetForm(BallastType obj)
        {
            Text = "Изменение записи";
            coordControl.SetValue(obj.Start_Km, obj.Start_M, obj.Final_Km, obj.Final_M);
            tbBallast.Text = obj.Ballast.ToString();
        }

        internal void SetExistingSource(BindingSource bindingSource)
        {
            existsSource = bindingSource;
        }
        public void ClearForm()
        {
            coordControl.Clear();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            int ball = 0;

            if (coordControl.CorrectFilled == false || !Int32.TryParse(tbBallast.Text, out ball))
            {
                MetroFramework.MetroMessageBox.Show(this, alerts.insert_error + " " + alerts.check_fields_filling, alerts.inserting, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            ballast = new BallastType {
                Start_Km = coordControl.StartKm,
                Start_M = coordControl.StartM,
                Final_Km = coordControl.FinalKm,
                Final_M = coordControl.FinalM,
                Ballast = ball
            };
            result = DialogResult.OK;
            Close();
        }
    }
}
