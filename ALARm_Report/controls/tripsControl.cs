using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ALARm.Core;
using ALARm.Services;
using ALARm.Core.Report;

namespace ALARm.controls
{
    public partial class tripsControl : MetroFramework.Controls.MetroUserControl
    {
        public long currentId = -1;
        public long parentId = -1;
        public List<long> parentIDs = new List<long> { -1 };
        public long admDirectionId = -1;
        public int rdLvl = -1;

        public tripsControl()
        {
            InitializeComponent();
            dataTrips.AutoGenerateColumns = false;
        }

        internal void Build(string caption)
        {
            checkAll.Text = caption;

            if (this is tripsFilesControl)
            {
                metroLabel1.Hide();
                dataTrips.Columns.Add("description", "Описание");
                dataTrips.Columns["description"].DataPropertyName = "description";
                dataTrips.Columns["description"].ReadOnly = true;
                dataTrips.Columns["description"].MinimumWidth = 258;
                /*dataTrips.Columns.Add("file_name", "file_name");
                dataTrips.Columns["file_name"].DataPropertyName = "file_name";
                dataTrips.Columns["file_name"].ReadOnly = true;
                dataTrips.Columns.Add("threat_id", "threat_id");
                dataTrips.Columns["threat_id"].DataPropertyName = "threat_id";
                dataTrips.Columns["threat_id"].ReadOnly = true;*/
                rdLvl = 1;
            }
            else
            {
                metroLabel1.Text = caption;
                checkAll.Hide();
                dataTrips.Columns["checkTrips"].Visible = false;
                dataTrips.Columns.Add("trip_date", "Дата поездки");
                dataTrips.Columns["trip_date"].DataPropertyName = "trip_date";
                dataTrips.Columns["trip_date"].ReadOnly = true;
                dataTrips.Columns["trip_date"].MinimumWidth = 258;
                rdLvl = 0;
            }
        }
        
        public List<long> GetCheckedItemsIDs()
        {
            return ((List<TripFiles>)bindingSource1.DataSource).Where(tmp => tmp.Checked_Status == true).Select(tmp => tmp.Id).ToList<long>();
        }

        public object GetDataSource()
        {
            return bindingSource1.DataSource;
        }

        public void dataSourceClear()
        {
            parentId = -1;
            admDirectionId = -1;
            checkAll.CheckedChanged -= checkAll_Checked;
            checkAll.Checked = false;
            checkAll.CheckedChanged += checkAll_Checked;

            if (this is tripsFilesControl)
            {
                List<TripFiles> trips = new List<TripFiles>();
                dataTrips.DataSource = trips; 
            }
            else
            {
                List<Trips> trips = new List<Trips>();
                dataTrips.DataSource = trips;
            }
        }

        public int getDataCount()
        {
            return dataTrips.Rows.Count;
        }

        private void checkAll_Checked(object sender, EventArgs e)
        {
            if (dataTrips.Rows.Count <= 0)
            {
                if (checkAll.Checked == false)
                    return;
                else
                {
                    checkAll.Checked = false;
                    return;
                }
            }
            if (checkAll.Checked == true)
            {
                if (this is tripsFilesControl)
                {
                    foreach (TripFiles items in dataTrips.DataSource as List<TripFiles>)
                    {
                        items.Checked_Status = true;
                    }
                }
                /*else
                {
                    foreach (Trips items in bindingSource1.DataSource as List<Trips>)
                    {
                        items.Checked_Status = true;
                    }
                }*/
                dataTrips.Refresh();
                UnitSelectionChanged?.Invoke(this, e);
            }
            else
            {
                if (this is tripsFilesControl)
                {
                    foreach (TripFiles items in dataTrips.DataSource as List<TripFiles>)
                    {
                        items.Checked_Status = false;
                    }
                }
                /*else
                {
                    foreach (Trips items in bindingSource1.DataSource as List<Trips>)
                    {
                        items.Checked_Status = false;
                    }
                }*/
                dataTrips.Refresh();
                UnitSelectionChanged?.Invoke(this, e);
            }
        }

        public void setDataPeriod(long getId)
        {
            checkAll.CheckedChanged -= checkAll_Checked;
            checkAll.Checked = false;
            checkAll.CheckedChanged += checkAll_Checked;
            if (bindingSource1.Count > 0)
                dataTrips.DataSource = ((List<TripFiles>)bindingSource1.DataSource).Where(tmp => tmp.Trip_id == getId).ToList();
        }

        public void setDataPeriod(List<long> directionIDs)
        {
            bindingSource1.DataSource = RdStructureService.GetTrips(directionIDs, rdLvl);
            dataTrips.DataSource = bindingSource1;
        }

        private void dataTrips_SelectionChanged(object sender, EventArgs e)
        {
            if (dataTrips.Rows.Count <= 0)
                return;
            if (dataTrips.DataSource == null)
                return;

            if (this is tripsFilesControl)
            {
                currentId = ((TripFiles)bindingSource1.Current).Id; 
            }
            else
            {
                currentId = ((Trips)bindingSource1.Current).Id;
            }

            UnitSelectionChanged?.Invoke(this, e);
        }

        public event EventHandler UnitSelectionChanged;
    }
}
