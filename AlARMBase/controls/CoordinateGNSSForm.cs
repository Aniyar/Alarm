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
    public partial class CoordinateGNSSForm : MetroFramework.Forms.MetroForm
    {
        public CoordinateGNSS coordinateGNSS;
        private BindingSource existsSource;
        public DialogResult result = DialogResult.Cancel;
        public CoordinateGNSSForm()
        {
            InitializeComponent();
        }
        public void SetForm(CoordinateGNSS obj)
        {
            Text = "Изменение записи";
            coordControl.SetValue(obj.Km, obj.Meter);
            tbLatitude.Text = obj.Latitude.ToString();
            tbLongtitude.Text = obj.Longtitude.ToString();
            tbAltitude.Text = obj.Altitude.ToString();
            tbCoord.Text = obj.Exact_coordinate.ToString();
            tbHeight.Text = obj.Exact_height.ToString();
        }
        internal void SetExistingSource(BindingSource bindingSource)
        {
            existsSource = bindingSource;
        }
        public void ClearForm()
        {
            coordControl.Clear();
            tbLatitude.Text = String.Empty;
            tbLongtitude.Text = String.Empty;
            tbAltitude.Text = String.Empty;
            tbCoord.Text = String.Empty;
            tbHeight.Text = String.Empty;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            double latitude = 0.0, longtitude = 0.0, altitude = 0.0, coord = 0.0, height = 0.0;

            if (coordControl.CorrectFilled == false || !double.TryParse(tbLatitude.Text, out latitude) || !double.TryParse(tbLongtitude.Text, out longtitude) || !double.TryParse(tbAltitude.Text, out altitude)
                || !double.TryParse(tbCoord.Text, out coord) || !double.TryParse(tbHeight.Text, out height))
            {
                MetroFramework.MetroMessageBox.Show(this, alerts.insert_error + " " + alerts.check_fields_filling, alerts.inserting, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            coordinateGNSS = new CoordinateGNSS
            {
                Km = coordControl.StartKm,
                Meter = coordControl.StartM,
                Latitude = latitude,
                Longtitude = longtitude,
                Altitude = altitude,
                Exact_coordinate = coord,
                Exact_height = height
            };
            result = DialogResult.OK;
            Close();
        }
    }
}
