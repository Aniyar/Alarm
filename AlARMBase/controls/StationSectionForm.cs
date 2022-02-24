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
    public partial class StationSectionForm : MetroFramework.Forms.MetroForm
    {
        public object section;
        private BindingSource existsSections;
        public DialogResult result = DialogResult.Cancel;
        public StationSectionForm()
        {
            InitializeComponent();
            catalogListPoint.SetDataSource(MainTrackStructureService.GetCatalog(MainTrackStructureConst.MtoStationSection));
        }
        public void SetStationSectionForm(StationSection obj)
        {
            Text = "Изменение записи";
            admRoadListBox.CurrentValue = obj.Road;
            admNodListBox.CurrentValue = obj.Nod;
            admStationListBox.CurrentValue = obj.Station;
            coordControl.SetValue(obj.Start_Km, obj.Start_M, obj.Final_Km, obj.Final_M);
            tbAxisKm.Text = obj.Axis_Km.ToString();
            tbAxisM.Text = obj.Axis_M.ToString();
            catalogListPoint.CurrentId = obj.Point_Id;
        }
        public void ClearForm()
        {
            coordControl.Clear();
            admNodListBox.Clear();
            admRoadListBox.Clear();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {

            if (admRoadListBox.CurrentId == -1 || admNodListBox.CurrentId == -1 || admStationListBox.CurrentId == -1 || catalogListPoint.CurrentId == -1 ||
                tbAxisKm.Text.Equals(String.Empty) || tbAxisM.Equals(String.Empty) || coordControl.CorrectFilled == false)
            {
                MetroFramework.MetroMessageBox.Show(this, alerts.insert_error + " " + alerts.check_fields_filling, alerts.inserting, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            section = new StationSection
            {
                Start_Km = coordControl.StartKm,
                Start_M = coordControl.StartM,
                Final_Km = coordControl.FinalKm,
                Final_M = coordControl.FinalM,
                Nod_Id = admNodListBox.CurrentId,
                Nod = admNodListBox.CurrentValue,
                Station = admStationListBox.CurrentValue,
                Station_Id = admStationListBox.CurrentId,
                Road = admRoadListBox.CurrentValue,
                Axis_Km = int.Parse(tbAxisKm.Text),
                Axis_M = int.Parse(tbAxisM.Text),
                Point = catalogListPoint.CurrentValue,
                Point_Id = catalogListPoint.CurrentId
            };
            result = DialogResult.OK;
            Close();
        }

        internal void LoadRoads(BindingSource bindingSource)
        {
            admRoadListBox.SetDataSource(bindingSource);
        }

        private void admRoadListBox_SelectionChanged(object sender, EventArgs e)
        {
            admNodListBox.SetDataSource(AdmStructureService.GetUnits(AdmStructureConst.AdmNod, admRoadListBox.CurrentId));
        }

        public void SetExistingSectionsSource(BindingSource bindingSource)
        {
            existsSections = bindingSource;
        }

        private void admNodListBox_SelectionChanged(object sender, EventArgs e)
        {
            admStationListBox.SetDataSource(AdmStructureService.GetUnits(AdmStructureConst.AdmStation, admNodListBox.CurrentId));
        }

        private void admStationListBox_SelectionChanged(object sender, EventArgs e)
        {
            directionListBox.SetDataSource(AdmStructureService.GetStationsDirection(admStationListBox.CurrentId, admRoadListBox.CurrentId));
        }
    }
}
