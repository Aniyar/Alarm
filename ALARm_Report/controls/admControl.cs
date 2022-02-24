using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ALARm.Services;
using ALARm.Core;
using ALARm.Core.Report;

namespace ALARm.controls
{
    public partial class admControl : MetroFramework.Controls.MetroUserControl
    {
        public long currentUnitId = -1;
        public long parentId = -1;
        public int admLevel = -1;

        public admControl()
        {
            InitializeComponent();
        }

        internal void Build(string caption, int admLvl)
        {
            metroLabel1.Text = caption;
            admLevel = admLvl;
        }

        public void dataSourceClear()
        {
            parentId = -1;
            currentUnitId = -1;
            listBox1.DataSource = null as DataTable;
        }

        public int getDataCount()
        {
            return listBox1.Items.Count;
        }
        
        public void setDataList(long parentId)
        {
            listBox1.DataSource = AdmStructureService.GetDistancesRoad( parentId);
            listBox1.DisplayMember = "NAME";
            listBox1.ValueMember = "ID";
        }

        public event EventHandler UnitSelectionChanged;

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.Items.Count <= 0)
                return;
            if (listBox1.DataSource == null)
                return;
            currentUnitId = ((AdmUnit) listBox1.SelectedItem).Id;
            UnitSelectionChanged?.Invoke(this, e);
        }
    }
}
