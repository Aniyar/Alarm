using ALARm.Core;
using ALARm.Services;
using System;
using System.Windows.Forms;

namespace ALARm.controls
{
    public partial class DimensionForm : MetroFramework.Forms.MetroForm
    {
        public Dimension dimension;
        public DialogResult Result = DialogResult.Cancel;
        public DimensionForm()
        {
            InitializeComponent();
            catalogListBox.SetDataSource(MainTrackStructureService.GetCatalog(MainTrackStructureConst.MtoDimension));
        }
        public void SetForm(Dimension obj)
        {
            Text = "Изменение записи";
            coordControl.SetValue(obj.Start_Km, obj.Start_M, obj.Final_Km, obj.Final_M);
            catalogListBox.CurrentId = obj.Type_id;
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

            dimension = new Dimension
            {
                Start_Km = coordControl.StartKm,
                Start_M = coordControl.StartM,
                Final_Km = coordControl.FinalKm,
                Final_M = coordControl.FinalM,
                Type_id = catalogListBox.CurrentId,
                Type = catalogListBox.CurrentValue
            };
            Result = DialogResult.OK;
            Close();
        }
    }
}
