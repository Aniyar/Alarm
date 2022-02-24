using ALARm.Core;
using ALARm.Services;
using System;
using System.Windows.Forms;

namespace ALARm.controls
{
    public partial class ProfileObjectForm : MetroFramework.Forms.MetroForm
    {
        public ProfileObject profileObject;
        private BindingSource existsSource;
        public DialogResult Result = DialogResult.Cancel;
        public ProfileObjectForm()
        {
            InitializeComponent();
            catObject.SetDataSource(MainTrackStructureService.GetCatalog(MainTrackStructureConst.MtoProfileObject));
            catSide.SetDataSource(MainTrackStructureService.GetCatalog(MainTrackStructureConst.CatSide));
        }

        public void SetProfileObject(ProfileObject obj)
        {
            Text = "Изменение записи";
            coordControl.SetValue(obj.Km, obj.Meter);
            catObject.CurrentId = obj.Object_id;
            catSide.CurrentId = obj.Side_id;
        }

        internal void SetExistingSource(BindingSource bindingSource)
        {
            existsSource = bindingSource;
        }

        public void ClearForm()
        {
            coordControl.Clear();
            catObject.Clear();
            catSide.Clear();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if ((catObject.CurrentId == -1) || (coordControl.StartKm < 0) || (coordControl.StartM < 0) || (catSide.CurrentId == -1))
            {
                MetroFramework.MetroMessageBox.Show(this, alerts.insert_error + " " + alerts.check_fields_filling, alerts.inserting, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            profileObject = new ProfileObject {
                Km = coordControl.StartKm,
                Meter = coordControl.StartM,
                Side_id = catSide.CurrentId,
                Side = catSide.CurrentValue,
                Object_id = catObject.CurrentId,
                Object_type = catObject.CurrentValue
            };
            Result = DialogResult.OK;
            Close();
        }
    }
}
