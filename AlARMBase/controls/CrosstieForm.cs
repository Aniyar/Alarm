using ALARm.Core;
using ALARm.Services;
using System;
using System.Windows.Forms;

namespace ALARm.controls
{
    public partial class CrosstieForm : MetroFramework.Forms.MetroForm
    {
        public CrossTie Crosstie;
        private BindingSource existsCrossties;
        public DialogResult Result = DialogResult.Cancel;
        public CrosstieForm()
        {
            InitializeComponent();
            catalogListBox.SetDataSource(MainTrackStructureService.GetCatalog(MainTrackStructureConst.MtoCrossTie));
        }
        public void SetCrosstieForm(CrossTie obj)
        {
            Text = "Изменение записи";
            coordControl.SetValue(obj.Start_Km, obj.Start_M, obj.Final_Km, obj.Final_M);
            catalogListBox.CurrentId = obj.Crosstie_type_id;
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            Result = DialogResult.Cancel;
            Close();
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if ((catalogListBox.CurrentId == -1) || (coordControl.CorrectFilled == false))
            {
                MetroFramework.MetroMessageBox.Show(this, alerts.insert_error + " " + alerts.check_fields_filling, alerts.inserting, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Crosstie = new CrossTie
            {
                Start_Km = coordControl.StartKm,
                Start_M = coordControl.StartM,
                Final_Km = coordControl.FinalKm,
                Final_M = coordControl.FinalM,
                Crosstie_type_id = catalogListBox.CurrentId,
                CrossTie_type = catalogListBox.CurrentValue
            };
            Result = DialogResult.OK;
            Close();
        }

        public void SetExistingCrosstiesSource(BindingSource bindingSource)
        {
            existsCrossties = bindingSource;
        }
    }
}
