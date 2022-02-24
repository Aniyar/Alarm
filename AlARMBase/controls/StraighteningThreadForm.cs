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
    public partial class StraighteningThreadForm : MetroFramework.Forms.MetroForm
    {

        public StraighteningThread straighteningThread;
        private BindingSource existsSource;
        public DialogResult result = DialogResult.Cancel;
        public StraighteningThreadForm()
        {
            InitializeComponent();
            catalogListBox.SetDataSource(MainTrackStructureService.GetCatalog(MainTrackStructureConst.CatSide));
        }
        public void SetStraighteningThreadForm(StraighteningThread obj)
        {
            Text = "Изменение записи";
            coordControl.SetValue(obj.Start_Km, obj.Start_M, obj.Final_Km, obj.Final_M);
            catalogListBox.CurrentId = obj.Side_Id;
        }
        internal void SetExistingSource(BindingSource bindingSource)
        {
            existsSource = bindingSource;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (coordControl.CorrectFilled == false || catalogListBox.CurrentId == -1)
            {
                MetroFramework.MetroMessageBox.Show(this, alerts.insert_error + " " + alerts.check_fields_filling, alerts.inserting, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            straighteningThread = new StraighteningThread
            {
                Start_Km = coordControl.StartKm,
                Start_M = coordControl.StartM,
                Final_Km = coordControl.FinalKm,
                Final_M = coordControl.FinalM,
                Side_Id = catalogListBox.CurrentId,
                Side = catalogListBox.CurrentValue
            };
            result = DialogResult.OK;
            Close();
        }
    }
}
