using ALARm.Core;
using ALARm.Services;
using System;
using System.Windows.Forms;

namespace ALARm.controls
{
    public partial class ArtificialConstructionForm : MetroFramework.Forms.MetroForm
    {
        public ArtificialConstruction ArtificialConstruction;
        public DialogResult Result = DialogResult.Cancel;
        public ArtificialConstructionForm()
        {
            InitializeComponent();
            catalogListBox.SetDataSource(MainTrackStructureService.GetCatalog(MainTrackStructureConst.MtoArtificialConstruction));
        }
        public void SetArtificialConstructionForm(ArtificialConstruction obj)
        {
            Text = "Изменение записи";
            coordControl.SetValue(obj.Km, obj.Meter);
            tbLen.Text = obj.Len.ToString();
            catalogListBox.CurrentId = obj.Type_Id;
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            int len = -1;

            if (coordControl.CorrectFilled == false || catalogListBox.CurrentId == -1 || !Int32.TryParse(tbLen.Text, out len))
            {
                MetroFramework.MetroMessageBox.Show(this, alerts.insert_error + " " + alerts.check_fields_filling, alerts.inserting, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            ArtificialConstruction = new ArtificialConstruction
            {
                Km = coordControl.StartKm,
                Meter = coordControl.StartM,
                Len = len,
                Type_Id = catalogListBox.CurrentId,
                Type = catalogListBox.CurrentValue
            };
            Result = DialogResult.OK;
            Close();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            Result = DialogResult.Cancel;
            Close();
        }
    }
}
