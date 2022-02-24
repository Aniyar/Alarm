using ALARm.Core;
using ALARm.Services;
using System;
using System.Windows.Forms;

namespace ALARm.controls
{
    public partial class RailsSectionsForm : MetroFramework.Forms.MetroForm
    {
        public RailsSections RailsSections;
        private BindingSource existsSource;
        public DialogResult Result = DialogResult.Cancel;
        public RailsSectionsForm()
        {
            InitializeComponent();
            catalogListBox.SetDataSource(MainTrackStructureService.GetCatalog(MainTrackStructureConst.MtoRailSection));
        }
        public void SetRailsSectionsForm(RailsSections obj)
        {
            Text = "Изменение записи";
            coordControl.SetValue(obj.Start_Km, obj.Start_M, obj.Final_Km, obj.Final_M);
            catalogListBox.CurrentId = obj.Type_Id;
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
            if ((catalogListBox.CurrentId == -1) || (coordControl.CorrectFilled == false))
            {
                MetroFramework.MetroMessageBox.Show(this, alerts.insert_error + " " + alerts.check_fields_filling, alerts.inserting, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            RailsSections = new RailsSections
            {
                Start_Km = coordControl.StartKm,
                Start_M = coordControl.StartM,
                Final_Km = coordControl.FinalKm,
                Final_M = coordControl.FinalM,
                Type_Id = catalogListBox.CurrentId,
                Type = catalogListBox.CurrentValue
            };
            Result = DialogResult.OK;
            Close();
        }
    }
}
