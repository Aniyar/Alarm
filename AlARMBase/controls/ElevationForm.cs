using ALARm.Core;
using ALARm.Services;
using System;
using System.Windows.Forms;

namespace ALARm.controls
{
    public partial class ElevationForm : MetroFramework.Forms.MetroForm
    {
        public Elevation Elevation;
        private BindingSource exists;
        public DialogResult Result = DialogResult.Cancel;
        public ElevationForm()
        {
            InitializeComponent();
            catalogListBox.SetDataSource(MainTrackStructureService.GetCatalog(MainTrackStructureConst.MtoElevation));
        }

        public void SetElevationForm(Elevation obj)
        {
            Text = "Изменение записи";
            coordControl.SetValue(obj.Start_Km, obj.Start_M, obj.Final_Km, obj.Final_M);
            catalogListBox.CurrentId = obj.Level_Id;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (catalogListBox.CurrentId == -1 || coordControl.CorrectFilled == false)
            {
                MetroFramework.MetroMessageBox.Show(this, alerts.insert_error + " " + alerts.check_fields_filling, alerts.inserting, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Elevation = new Elevation
            {
                Start_Km = coordControl.StartKm,
                Start_M = coordControl.StartM,
                Final_Km = coordControl.FinalKm,
                Final_M = coordControl.FinalM,
                Level_Id = catalogListBox.CurrentId,
                Side = catalogListBox.CurrentValue
            };
            Result = DialogResult.OK;
            Close();
        }

        public void SetExistingSource(BindingSource bindingSource)
        {
            exists = bindingSource;
        }
    }
}
