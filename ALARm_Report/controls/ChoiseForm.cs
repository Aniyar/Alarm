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
    public partial class ChoiseForm : MetroFramework.Forms.MetroForm
    {
        public DialogResult dialogResult = DialogResult.Cancel;
        public double wear = -1;
        //public List<AdmTrack> admTracks = new List<AdmTrack>();
        public List<long> admTracksIDs = new List<long>();
        private int Mode = 0;

        internal void SetTripsDataSource(object distanceId, ReportPeriod period)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Вид формы
        /// </summary>
        /// <param name="mode"> Виды формы: 0 - проезд и пути, 1 - износ.</param>
        public ChoiseForm(int mode)
        {
            InitializeComponent();
            switch (mode)
            {
                case 0:
                    this.Height = 237;
                    Mode = mode;
                    break;
                case 1:
                    metroPanel1.Visible = false;
                    cbTrips.Visible = false;
                    metroPanel4.Visible = true;
                    this.Height = 168;
                    Mode = mode;
                    break;
                default:
                    Mode = -1;
                    Close();
                    return;
            }
        }

        public void SetTripsDataSource(long distanceId, ReportPeriod period,long trackId = -1 )
        {
            //tripsBindingSource.DataSource = RdStructureService.GetTripsOnDistance(distanceId, period);

            admTrackBindingSource.DataSource = RdStructureService.GetTracks(distanceId, period,trackId);
            int height = 15 + admTrackBindingSource.Count * 22 + 5 * 2;
            metroPanel1.Height = height;
            this.Height = 185 + height;
        }

        private void mbAccept_Click(object sender, EventArgs e)
        {
            switch (Mode)
            {
                case 0:
                    (admTrackBindingSource.DataSource as List<AdmTrack>).Where(a => a.Accept == true).ToList().ForEach(a => admTracksIDs.Add(a.Id));

                    if (admTracksIDs.Count < 1)
                    {
                        MetroFramework.MetroMessageBox.Show(this, "ErrNo5", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        break;
                    }

                    dialogResult = DialogResult.OK;
                    Close();
                    return;
                case 1:
                    if (!double.TryParse(tbWear.Text, out wear))
                    {
                        MetroFramework.MetroMessageBox.Show(this, "ErrNo5", "", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        break;
                    }

                    dialogResult = DialogResult.OK;
                    Close();
                    return;
            }
        }

        private void mbCancel_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void cbTrips_SelectedValueChanged(object sender, EventArgs e)
        {
            if (tripsBindingSource.Count > 0 && !cbTrips.SelectedValue.Equals(null))
            {
                admTrackBindingSource.DataSource = RdStructureService.GetTracksOnTrip((long)cbTrips.SelectedValue);
                int height = 15 + admTrackBindingSource.Count * 22 + 5 * 2;
                metroPanel1.Height = height;
                this.Height = 185 + height;
            }
        }

        private void metroGrid1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
