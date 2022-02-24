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
    public partial class  RfidForm : MetroFramework.Forms.MetroForm
    {
        public Rfid point;
        private BindingSource existsSource;
        public DialogResult result = DialogResult.Cancel;
        public RfidForm()
        {
            InitializeComponent();
        }
        public void SetRfidForm(Rfid obj)
        {
            Text = "Изменение записи";
            coordControl.SetValue(obj.Km, obj.Meter);
            tbName.Text = obj.Mark;
        }
        internal void SetExistingSource(BindingSource bindingSource)
        {
            existsSource = bindingSource;
        }
        public void ClearForm()
        {
            coordControl.Clear();
            tbName.Text = String.Empty;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {

            if (tbName.Text.Equals(String.Empty) || coordControl.StartKm.Equals(String.Empty) || coordControl.StartM.Equals(String.Empty))
            {
                MetroFramework.MetroMessageBox.Show(this, alerts.insert_error + " " + alerts.check_fields_filling, alerts.inserting, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            point = new Rfid {
                Km = coordControl.StartKm,
                Meter = coordControl.StartM,
                Mark = tbName.Text
            };
            result = DialogResult.OK;
            Close();
        }
    }
}
