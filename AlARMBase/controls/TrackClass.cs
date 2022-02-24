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
    public partial class TrackClassForm : MetroFramework.Forms.MetroForm
    {
        public TrackClass trackclass;
        private BindingSource existsSource;
        public DialogResult result = DialogResult.Cancel;
        public TrackClassForm()
        {
            InitializeComponent();
            catalogListBox.SetDataSource(MainTrackStructureService.GetCatalog(MainTrackStructureConst.MtoTrackClass));
        }
        public void SetTrackClassForm(TrackClass obj)
        {
            Text = "Изменение записи";
            coordControl.SetValue(obj.Start_Km, obj.Start_M, obj.Final_Km, obj.Final_M);
            catalogListBox.CurrentId = obj.Class_Id;
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
            if (catalogListBox.CurrentId == -1 || coordControl.CorrectFilled == false)
            {
                MetroFramework.MetroMessageBox.Show(this, alerts.insert_error + " " + alerts.check_fields_filling, alerts.inserting, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            trackclass = new TrackClass
            {
                Start_Km = coordControl.StartKm,
                Start_M = coordControl.StartM,
                Final_Km = coordControl.FinalKm,
                Final_M = coordControl.FinalM,
                Class_Id = catalogListBox.CurrentId,
                Class_Type = catalogListBox.CurrentValue
            };
            result = DialogResult.OK;
            Close();
        }
    }
}
