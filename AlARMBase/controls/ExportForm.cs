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
    public partial class ExportForm : MetroFramework.Forms.MetroForm
    {
        public long admRoadId = -1;
        public Period Period;
        public DialogResult Result = DialogResult.Cancel;

        public ExportForm()
        {
            InitializeComponent();
            mdtDate.Value = DateTime.Now;
            List<AdmUnit> admRoads = (List<AdmUnit>) AdmStructureService.GetUnits(AdmStructureConst.AdmRoad, -1);
            admRoads.ForEach(a => a.Name = a.Name + " (" + a.Code + ")");
            admRoads.Add(new AdmUnit { Id = -1, Name = "Все" });
            admRoads.OrderBy(a => a.Id);
            lbRoad.SetDataSource(admRoads);
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            admRoadId = lbRoad.CurrentId;

            Period = new Period {
                Start_Date = mdtDate.Value.Date,
                Final_Date = mdtDate.Value.Date
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
