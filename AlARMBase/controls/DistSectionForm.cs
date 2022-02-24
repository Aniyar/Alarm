using ALARm.Core;
using ALARm.Services;
using System;
using System.Windows.Forms;

namespace ALARm.controls
{
    public partial class DistanceForm : MetroFramework.Forms.MetroForm
    {
        public object Section;
        private BindingSource existsSections;
        public DialogResult Result = DialogResult.Cancel;
        public DistanceForm()
        {
            InitializeComponent();
            if (this is PdbForm)
                admPdbListBox.Visible = true;
        }
        public void SetDistanceForm(DistSection obj)
        {
            Text = "Изменение записи";
            admRoadListBox.CurrentValue = obj.Road;
            admDistanceListBox.CurrentValue = obj.Distance;
            coordControl.SetValue(obj.Start_Km, obj.Start_M, obj.Final_Km, obj.Final_M);
        }
        public void SetPdbForm(PdbSection obj)
        {
            Text = "Изменение записи";
            admRoadListBox.CurrentValue = obj.Road;
            admDistanceListBox.CurrentValue = obj.Distance;
            admPdbListBox.CurrentValue = obj.Pdb;
            coordControl.SetValue(obj.Start_Km, obj.Start_M, obj.Final_Km, obj.Final_M);
        }

        public void ClearForm()
        {
            coordControl.Clear();
            admDistanceListBox.Clear();
            admRoadListBox.Clear();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (this is PdbForm)
            {
                if (admRoadListBox.CurrentId == -1 || admDistanceListBox.CurrentId == -1 || admPdbListBox.CurrentId == -1  || coordControl.CorrectFilled == false)
                {
                    MetroFramework.MetroMessageBox.Show(this, alerts.insert_error + " " + alerts.check_fields_filling, alerts.inserting, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            } else
            {
                if (admRoadListBox.CurrentId == -1 || admDistanceListBox.CurrentId == -1 || coordControl.CorrectFilled == false)
                {
                    MetroFramework.MetroMessageBox.Show(this, alerts.insert_error + " " + alerts.check_fields_filling, alerts.inserting, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }           

            if (this is DistanceForm)
                Section = new DistSection 
                {
                    Start_Km = coordControl.StartKm,
                    Start_M = coordControl.StartM,
                    Final_Km = coordControl.FinalKm,
                    Final_M = coordControl.FinalM,
                    DistanceId = admDistanceListBox.CurrentId,
                    Distance = admDistanceListBox.CurrentValue,
                    Road = admRoadListBox.CurrentValue
                };
            if (this is PdbForm)
                Section = new PdbSection
                {
                    Start_Km = coordControl.StartKm,
                    Start_M = coordControl.StartM,
                    Final_Km = coordControl.FinalKm,
                    Final_M = coordControl.FinalM,
                    DistanceId = admDistanceListBox.CurrentId,
                    Distance = admDistanceListBox.CurrentValue,
                    Pdb = admPdbListBox.CurrentValue,
                    PdbId = admPdbListBox.CurrentId,
                    Road = admRoadListBox.CurrentValue
                };
            Result = DialogResult.OK;
            Close();
        }

        internal void LoadRoads(BindingSource bindingSource)
        {
            admRoadListBox.SetDataSource(bindingSource);
        }

       
        private void admRoadListBox_SelectionChanged(object sender, EventArgs e)
        {
            admDistanceListBox.SetDataSource(AdmStructureService.GetDistancesOfRoad(admRoadListBox.CurrentId));
        }
        public void SetExistingSectionsSource(BindingSource bindingSource)
        {
            
            existsSections = bindingSource;
        }

        private void admDistanceListBox_SelectionChanged(object sender, EventArgs e)
        {
            if (this is PdbForm)
            {
                admPdbListBox.SetDataSource(AdmStructureService.GetPartsOfDistance(admDistanceListBox.CurrentId));
            }
        }
    }
}
