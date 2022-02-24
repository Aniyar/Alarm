using ALARm.Core;
using ALARm.Services;
using MetroFramework.Controls;
using System;
using System.Linq;
using System.Windows.Forms;

namespace ALARm.controls
{
    public partial class AdmUniForm : MetroFramework.Forms.MetroForm
    {
        private bool isStationObject;
        private bool directionMode = false;
        public object Admunit;
        public DialogResult Result = DialogResult.Cancel;
        public void ChangeMetroLabelText(string text) {
            lbName.Text = text;
        }
        public void SetStationObjectStatus(int objectType)
        {
            isStationObject = true;
            catalogListBox.Show();
            catalogListBox.SetDataSource(AdmStructureService.GetCatalog(objectType));
            switch(objectType)
            {
                case AdmStructureConst.AdmStation:
                    Admunit = new Station();
                    break;
                case AdmStructureConst.AdmStationObject:
                    lbCode.Visible = false;
                    tbCode.Visible = false;
                    Admunit = new StationObject();
                    break;
                case AdmStructureConst.AdmPark:
                    lbCode.Visible = false;
                    tbCode.Visible = false;
                    Admunit = new Park();
                    break;
                case AdmStructureConst.AdmStationTrack:
                    lbName.Visible = false;
                    tbName.Visible = false;
                    Admunit = new StationTrack();
                    break;
                case AdmStructureConst.AdmParkTrack:
                    lbName.Visible = false;
                    tbName.Visible = false;
                    Admunit = new StationTrack();
                    break;
            }
        }
        public void SetDirectionMode()
        {
            directionMode = true;
        }
        public AdmUniForm()
        {
            InitializeComponent();
            tbCode.Text = string.Empty;
            tbName.Text = string.Empty;
        }

        public AdmUniForm(AdmUnit obj)
        {
            InitializeComponent();
            Text = "Изменение записи";
            tbCode.Text = obj.Code;
            try
            {
                if (String.IsNullOrEmpty(obj.Chief_fullname))
                    tbName.Text = obj.Name;
                else
                    tbName.Text = obj.Chief_fullname;
            }
            catch
            {
                
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (((tbCode.Text == string.Empty) && tbCode.Visible) ||
                ((tbName.Text == string.Empty) && tbName.Visible) ||
                (isStationObject && (catalogListBox.CurrentId == -1)))
            {
                MetroFramework.MetroMessageBox.Show(this, alerts.filling_error, alerts.inserting, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!isStationObject)
            {
                Admunit = new AdmUnit { Code = tbCode.Text, Name = tbName.Text, Chief_fullname = tbName.Text };
                var tbAbbr = Controls.Find("tbAbbr",true);
                if (tbAbbr.Count()>0)
                {

                    (Admunit as AdmUnit).Abbr = (tbAbbr.FirstOrDefault() as MetroTextBox).Text;
                }
            }

            else
            {
                ((StationObject)Admunit).Type_id = catalogListBox.CurrentId;
                ((StationObject)Admunit).Code = tbCode.Text;
                ((StationObject)Admunit).Name = tbName.Text;
                ((StationObject)Admunit).Object_type = catalogListBox.CurrentValue;
            }
            Result = DialogResult.OK;
            
            Close();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            Result = DialogResult.Cancel;
            Close();
        }

        internal void HideNameControl()
        {
            lbName.Visible = false;
            tbName.Visible = false;
        }

        private void tbCode_KeyPress(object sender, KeyPressEventArgs e)
        {
           // e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        private void tbCode_KeyUp(object sender, KeyEventArgs e)
        {
            if (directionMode)
            {
                int codeDir;
                int.TryParse(tbCode.Text, out codeDir);
                string name = AdmStructureService.GetDirectionName(codeDir.ToString());
                if (!string.IsNullOrEmpty(name))
                    tbName.Text = name;
            }
        }
    }
}
