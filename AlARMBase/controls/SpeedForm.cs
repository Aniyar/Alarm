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

    public partial class SpeedForm : MetroFramework.Forms.MetroForm
    {
        private bool isTemp = false;
        public Speed speed;
        private BindingSource existsSource;
        public DialogResult result = DialogResult.Cancel;
        public SpeedForm()
        {
            InitializeComponent();
        }
        public void SetSpeedForm(TempSpeed obj)
        {
            Text = "Изменение записи";
            coordControl.SetValue(obj.Start_Km, obj.Start_M, obj.Final_Km, obj.Final_M);
            passenger.Text = obj.Passenger.ToString();
            freight.Text = obj.Freight.ToString();
            emptyfreight.Text = obj.Empty_Freight.ToString();
            catalogListBox.CurrentId = obj.Reason_Id;
        }
        public void SetSpeedForm(Speed obj)
        {
            Text = "Изменение записи";
            coordControl.SetValue(obj.Start_Km, obj.Start_M, obj.Final_Km, obj.Final_M);
            passenger.Text = obj.Passenger.ToString();
            freight.Text = obj.Freight.ToString();
            emptyfreight.Text = obj.Empty_Freight.ToString();
            tbSapsan.Text = obj.Sapsan.ToString();
            tbLastochka.Text = obj.Lastochka.ToString();
        }
        public void SetTempStatus()
        {
            isTemp = true;
            catalogListBox.Visible = true;
            catalogListBox.SetDataSource(MainTrackStructureService.GetCatalog(MainTrackStructureConst.MtoTempSpeed));
            metroPanel4.Visible = false;
            metroPanel5.Visible = false;
        }

        internal void SetExistingSource(BindingSource bindingSource)
        {
            existsSource = bindingSource;
        }
        public void ClearForm()
        {
            coordControl.Clear();
            catalogListBox.Clear();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if ((catalogListBox.CurrentId == -1 && isTemp) || coordControl.CorrectFilled == false
                || passenger.Text.Equals(string.Empty) || freight.Text.Equals(string.Empty) || emptyfreight.Text.Equals(string.Empty) || 
                ((tbSapsan.Text.Equals(string.Empty) || tbLastochka.Text.Equals(string.Empty)) && !isTemp))
            {
                MetroFramework.MetroMessageBox.Show(this, alerts.insert_error + " " + alerts.check_fields_filling, alerts.inserting, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (isTemp)
            {
                speed = new TempSpeed();
                (speed as TempSpeed).Reason = catalogListBox.CurrentValue;
                (speed as TempSpeed).Reason_Id = catalogListBox.CurrentId;
            }
            else
            {
                speed = new Speed();
                speed.Sapsan = int.Parse(tbSapsan.Text);
                speed.Lastochka = int.Parse(tbLastochka.Text);
            }

            speed.Start_Km = coordControl.StartKm;
            speed.Start_M = coordControl.StartM;
            speed.Final_Km = coordControl.FinalKm;
            speed.Final_M = coordControl.FinalM;
            speed.Passenger = int.Parse(passenger.Text);
            speed.Freight = int.Parse(freight.Text);
            speed.Empty_Freight = int.Parse(emptyfreight.Text);
            result = DialogResult.OK;
            Close();
        }
        private void DigitKeyFilter(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }
    }
}
