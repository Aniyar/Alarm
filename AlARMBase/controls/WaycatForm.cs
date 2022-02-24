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
    public partial class WaycatForm : MetroFramework.Forms.MetroForm
    {
        public Waycat waycat;
        private BindingSource existsSource;
        public DialogResult result = DialogResult.Cancel;
        public WaycatForm()
        {
            InitializeComponent();
            catalogListBox.SetDataSource(MainTrackStructureService.GetCatalog(MainTrackStructureConst.MtoWaycat));
        }
        public void SetWaycatForm(Waycat obj)
        {
            Text = "Изменение записи";
            coordControl.SetValue(obj.Start_Km, obj.Start_M, obj.Final_Km, obj.Final_M);
            catalogListBox.CurrentId = obj.Type_id;
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
            if (coordControl.CorrectFilled == false)
            {
                MetroFramework.MetroMessageBox.Show(this, alerts.insert_error + " " + alerts.check_fields_filling, alerts.inserting, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            waycat = new Waycat
            {
                Start_Km = coordControl.StartKm,
                Start_M = coordControl.StartM,
                Final_Km = coordControl.FinalKm,
                Final_M = coordControl.FinalM,
                Type_id = catalogListBox.CurrentId,
                Type = catalogListBox.CurrentValue
            };
            result = DialogResult.OK;
            Close();
        }
    }
}
