using ALARm.Core;
using ALARm.Services;
using System;
using System.Windows.Forms;

namespace ALARm.controls
{
    public partial class DistanceBetweenTracksForm : MetroFramework.Forms.MetroForm
    {
        public DistanceBetweenTracks distanceBetweenTracks;
        private BindingSource existsCrossties;
        public DialogResult Result = DialogResult.Cancel;
        public DistanceBetweenTracksForm(Int64 parentID)
        {
            InitializeComponent();
            admListBox1.SetDataSource(AdmStructureService.GetUnits(AdmStructureConst.AdmTrack, parentID));
            admListBox1.DisplayMember = "Code";
            admListBox2.SetDataSource(AdmStructureService.GetUnits(AdmStructureConst.AdmTrack, parentID));
            admListBox2.DisplayMember = "Code";
        }
        public void SetForm(DistanceBetweenTracks obj)
        {
            Text = "Изменение записи";
            coordControl.SetValue(obj.Start_Km, obj.Start_M, obj.Final_Km, obj.Final_M);
            admListBox1.CurrentId = obj.Left_adm_track_id;
            admListBox2.CurrentId = obj.Right_adm_track_id;
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            Result = DialogResult.Cancel;
            Close();
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            int leftm = 0, rightm = 0;

            if (admListBox1.CurrentId < 0 || admListBox2.CurrentId < 0 || !int.TryParse(tbLeftM.Text, out leftm) || !int.TryParse(tbRightM.Text, out rightm) || (coordControl.CorrectFilled == false))
            {
                MetroFramework.MetroMessageBox.Show(this, alerts.insert_error + " " + alerts.check_fields_filling, alerts.inserting, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            distanceBetweenTracks = new DistanceBetweenTracks
            {
                Start_Km = coordControl.StartKm,
                Start_M = coordControl.StartM,
                Final_Km = coordControl.FinalKm,
                Final_M = coordControl.FinalM,
                Left_m = leftm,
                Left_adm_track_id = admListBox1.CurrentId,
                Left_track = admListBox1.CurrentCode,
                Right_m = rightm,
                Right_adm_track_id = admListBox2.CurrentId,
                Right_track = admListBox2.CurrentCode
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
