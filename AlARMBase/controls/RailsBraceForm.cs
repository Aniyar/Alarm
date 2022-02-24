using ALARm.Core;
using ALARm.Services;
using System;
using System.Windows.Forms;

namespace ALARm.controls
{
    public partial class RailsBraceForm : MetroFramework.Forms.MetroForm
    {
        public RailsBrace RailsBrace;
        private BindingSource existsSource;
        public DialogResult Result = DialogResult.Cancel;
        public RailsBraceForm()
        {
            InitializeComponent();
            catalogListBox.SetDataSource(MainTrackStructureService.GetCatalog(MainTrackStructureConst.MtoRailsBrace));
        }
        public void SetRailsBraceForm(RailsBrace obj)
        {
            Text = "Изменение записи";
            coordControl.SetValue(obj.Start_Km, obj.Start_M, obj.Final_Km, obj.Final_M);
            catalogListBox.CurrentId = obj.Brace_Type_Id;
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

            RailsBrace = new RailsBrace
            {
                Start_Km = coordControl.StartKm,
                Start_M = coordControl.StartM,
                Final_Km = coordControl.FinalKm,
                Final_M = coordControl.FinalM,
                Brace_Type_Id = catalogListBox.CurrentId,
                Brace_Type = catalogListBox.CurrentValue
            };
            Result = DialogResult.OK;
            Close();
        }
    }
}
