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
using static ALARm.Core.MainTrackStructureConst;

namespace ALARm.controls
{
    public partial class SwitchForm : MetroFramework.Forms.MetroForm
    {
        public Switch @switch;
        private BindingSource existsSource;
        public DialogResult result = DialogResult.Cancel;
        public SwitchForm(long road_id)
        {
            InitializeComponent();
            cbMark.SetDataSource(MainTrackStructureService.GetCatalog(MainTrackStructureConst.MtoSwitchMark));
            cbMark.SetDisplayMember("Mark");
            cbSide.SetDataSource(MainTrackStructureService.GetCatalog(MainTrackStructureConst.CatSide));
            cbPoint.SetDataSource(MainTrackStructureService.GetCatalog(MainTrackStructureConst.MtoSwitchPoint));
            cbDir.SetDataSource(MainTrackStructureService.GetCatalog(MainTrackStructureConst.MtoSwitchDir));
            cbStation.SetDataSource(AdmStructureService.GetUnitsOfRoad(AdmStructureConst.AdmStation, road_id));            
        }
        public void SetSwitchForm(Switch obj, long road_id)
        {
            Text = "Изменение записи";
            coordControl.SetValue(obj.Km, obj.Meter);
            cbMark.CurrentId = obj.Mark_Id;
            cbSide.CurrentId = (int)obj.Side_Id;
            cbDir.CurrentId = (int)obj.Dir_Id;
            num.Text = obj.Num;
            cbPoint.CurrentId = obj.Point_Id;
            cbStation.CurrentId = obj.Station_Id;
        }
        internal void SetExistingSource(BindingSource bindingSource)
        {
            existsSource = bindingSource;
        }
        public void ClearForm()
        {
            cbMark.Clear();
            cbSide.Clear();
            cbPoint.Clear();
            cbDir.Clear();
            cbStation.Clear();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (cbMark.CurrentId == -1 || cbSide.CurrentId == -1 || cbPoint.CurrentId == -1 || cbStation.CurrentId == -1 ||
                num.Text.Equals(String.Empty) || coordControl.StartKm.Equals(String.Empty) || coordControl.StartM.Equals(String.Empty))
            {
                MetroFramework.MetroMessageBox.Show(this, alerts.insert_error + " " + alerts.check_fields_filling, alerts.inserting, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            @switch = new Switch
            {
                Km = coordControl.StartKm,
                Meter = coordControl.StartM,
                Num = num.Text,
                Side = cbSide.CurrentValue,
                Side_Id = (Side)cbSide.CurrentId,
                Point = cbPoint.CurrentValue,
                Point_Id = cbPoint.CurrentId,
                Dir = cbDir.CurrentValue,
                Dir_Id = (SwitchDirection)cbDir.CurrentId,
                Mark = cbMark.CurrentMarkValue,
                Mark_Id = cbMark.CurrentId,
                Station = cbStation.CurrentValue,
                Station_Id = cbStation.CurrentId
            };
            result = DialogResult.OK;
            Close();
        }
    }
}
