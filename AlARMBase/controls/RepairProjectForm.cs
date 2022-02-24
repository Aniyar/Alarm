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
    public partial class RepairProjectForm : MetroFramework.Forms.MetroForm
    {
        public RepairProject repairProject;
        public DialogResult result = DialogResult.Cancel;
        public RepairProjectForm()
        {
            InitializeComponent();
            cbType.SetDataSource(MainTrackStructureService.GetCatalog(MainTrackStructureConst.MtoRepairProject));
            cbAccept.SetDataSource(MainTrackStructureService.GetCatalog(MainTrackStructureConst.MtoAcceptType));
        }
        public void SetRepairProjectForm(RepairProject obj)
        {
            Text = "Изменение записи";
            coordControl.SetValue(obj.Start_Km, obj.Start_M, obj.Final_Km, obj.Final_M);
            cbType.CurrentId = obj.Type_id;
            cbAccept.CurrentId = obj.Accept_id;
            mdtDate.Value = obj.Repair_date.Date;
            speedControl.Value = obj.Speed;
        }
        public void ClearForm()
        {
            cbType.Clear();
            cbAccept.Clear();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (cbType.CurrentId == -1 || coordControl.CorrectFilled == false)
            {
                MetroFramework.MetroMessageBox.Show(this, alerts.insert_error + " " + alerts.check_fields_filling, alerts.inserting, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            repairProject = new RepairProject {
                Start_Km = coordControl.StartKm,
                Start_M = coordControl.StartM,
                Final_Km = coordControl.FinalKm,
                Final_M = coordControl.FinalM,
                Type = cbType.CurrentValue,
                Type_id = cbType.CurrentId,
                Accept = cbAccept.CurrentValue,
                Accept_id = cbAccept.CurrentId,
                Repair_date = mdtDate.Value.Date,
                Speed = (int)speedControl.Value
            };
            result = DialogResult.OK;
            Close();
        }
    }
}
