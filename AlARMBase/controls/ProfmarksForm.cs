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
    public partial class ProfmarksForm : MetroFramework.Forms.MetroForm
    {
        public Profmarks profmark;
        private BindingSource existsSource;
        public DialogResult result = DialogResult.Cancel;
        public ProfmarksForm()
        {
            InitializeComponent();
        }
        public void SetProfmarksForm(Profmarks obj)
        {
            Text = "Изменение записи";
            coordControl.SetValue(obj.Km, obj.Meter);
            tbProfil.Text = obj.Profil.ToString();
        }
        internal void SetExistingSource(BindingSource bindingSource)
        {
            existsSource = bindingSource;
        }
        public void ClearForm()
        {
            coordControl.Clear();
            tbProfil.Text = String.Empty;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            double profil = 0.0;

            if (coordControl.StartKm.Equals(String.Empty) || coordControl.StartM.Equals(String.Empty) || !double.TryParse(tbProfil.Text, out profil))
            {
                MetroFramework.MetroMessageBox.Show(this, alerts.insert_error + " " + alerts.check_fields_filling, alerts.inserting, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            profmark = new Profmarks
            {
                Km = coordControl.StartKm,
                Meter = coordControl.StartM,
                Profil = profil
            };
            result = DialogResult.OK;
            Close();
        }
    }
}
