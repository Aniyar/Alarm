using ALARm.Core;
using System;
using System.Windows.Forms;

namespace ALARm.controls
{
    public partial class NonExistsKm : MetroFramework.Forms.MetroForm
    {
        public NonExtKm Nexkm;
        private BindingSource existsNexkm;
        public DialogResult Result = DialogResult.Cancel;
        public NonExistsKm()
        {
            InitializeComponent();
        }
        public NonExistsKm(NonExtKm obj)
        {
            Text = "Изменение записи";
            InitializeComponent();
            mtbKM.Text = obj.Km.ToString();
        }

        public void ClearForm()
        {
            mtbKM.Text = string.Empty;
        }

        public void SetExistingsSource(BindingSource bs)
        {
            existsNexkm = bs;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (mtbKM.Text.Equals(string.Empty))
            {
                MetroFramework.MetroMessageBox.Show(this, alerts.insert_error + " " + alerts.check_fields_filling, alerts.inserting, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Nexkm = new NonExtKm { Km = int.Parse(mtbKM.Text) };
            Result = DialogResult.OK;
            Close();
        }

        private void DigitalKeyFilter(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }
    }
}
