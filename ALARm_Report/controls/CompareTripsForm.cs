using ALARm;
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

namespace ALARm_Report.controls
{
    public partial class CompareTripsForm : MetroFramework.Forms.MetroForm
    {
        public DialogResult dialogResult = DialogResult.Cancel;
        public long firstTripId = -1;
        public long secondTripId = -1;
        public List<long> firstAdmTracksIDs = new List<long>();
        public List<long> secondAdmTracksIDs = new List<long>();

        public CompareTripsForm()
        {
            InitializeComponent();
        }

        public void SetTripsDataSource(long distanceId, ReportPeriod period)
        {
            tripsBindingSource.DataSource = RdStructureService.GetTripsOnDistance(distanceId, period);
            tripsBindingSource1.DataSource = RdStructureService.GetTripsOnDistance(distanceId, period);
        }

        private void mbAccept_Click(object sender, EventArgs e)
        {
            firstTripId = (cbTrips.SelectedItem as Trips).Id;
            secondTripId = (cbTrips1.SelectedItem as Trips).Id;
            (admTrackBindingSource.DataSource as List<AdmTrack>).Where(a => a.Accept == true).ToList().ForEach(a => firstAdmTracksIDs.Add(a.Id));
            (admTrackBindingSource1.DataSource as List<AdmTrack>).Where(a => a.Accept == true).ToList().ForEach(a => secondAdmTracksIDs.Add(a.Id));

            if (firstAdmTracksIDs.Count < 1 || secondAdmTracksIDs.Count < 1 || firstTripId == -1 || secondTripId == -1)
            {
                MetroFramework.MetroMessageBox.Show(this, "ErrNo5", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            else
            {
                dialogResult = DialogResult.OK;
                Close();
            }
        }
        private void mbCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void cbTrips_SelectedValueChanged(object sender, EventArgs e)
        {
            if (tripsBindingSource.Count > 0 && !(cbTrips.SelectedValue is null))
            {
                admTrackBindingSource.DataSource = RdStructureService.GetTracksOnTrip((long)cbTrips.SelectedValue);
                int height = 29 + 15 + admTrackBindingSource.Count * 22;
                metroPanel2.Height = height;
                this.Height = 140 + height;
            }
        }

        private void cbTrips1_SelectedValueChanged(object sender, EventArgs e)
        {
            if (tripsBindingSource1.Count > 0 && !(cbTrips1.SelectedValue is null))
            {
                admTrackBindingSource1.DataSource = RdStructureService.GetTracksOnTrip((long)cbTrips1.SelectedValue);
                int height = 29 + 15 + admTrackBindingSource1.Count * 22;
                metroPanel3.Height = height;
                this.Height = 140 + height;
            }
        }
    }
}
