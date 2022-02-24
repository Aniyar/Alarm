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
    public partial class CommunicationForm : MetroFramework.Forms.MetroForm
    {
        public Communication communication;
        private BindingSource existsSource;
        public DialogResult result = DialogResult.Cancel;
        public CommunicationForm()
        {
            InitializeComponent();
            catalogListBox1.SetDataSource(MainTrackStructureService.GetCatalog(MainTrackStructureConst.MtoCommunication));
        }
        public void SetForm(Communication obj)
        {
            Text = "Изменение записи";
            coordControl.SetValue(obj.Km, obj.Meter);
            catalogListBox1.CurrentId = obj.Object_id;
        }

        internal void SetExistingSource(BindingSource bindingSource)
        {
            existsSource = bindingSource;
        }
        public void ClearForm()
        {
            coordControl.Clear();
            catalogListBox1.Clear();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (coordControl.CorrectFilled == false || catalogListBox1.CurrentId < 0)
            {
                MetroFramework.MetroMessageBox.Show(this, alerts.insert_error + " " + alerts.check_fields_filling, alerts.inserting, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            communication = new Communication {
                Km = coordControl.StartKm,
                Meter = coordControl.StartM,
                Object_id = catalogListBox1.CurrentId,
                Object = catalogListBox1.CurrentValue,                
            };
            result = DialogResult.OK;
            Close();
        }
    }
}
