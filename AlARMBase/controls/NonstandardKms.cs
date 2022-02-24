using ALARm.Core;
using System;
using System.Windows.Forms;

namespace ALARm.controls
{
    public partial class NonstandardKms: MetroFramework.Forms.MetroForm
    {
        public NonstandardKm Nskm;
        private BindingSource existsNskms;
        public DialogResult Result = DialogResult.Cancel;
        public NonstandardKms()
        {
            InitializeComponent();
        }
        public NonstandardKms(NonstandardKm obj)
        {
            Text = "Изменение записи";
            InitializeComponent();
            mtbKM.Text = obj.Km.ToString();
            mtbLength.Text = obj.Len.ToString();
        }

        public void ClearForm()
        {
            mtbKM.Text = string.Empty;
            mtbLength.Text = string.Empty;
        }
        public void SetExistingsSource(BindingSource bs)
        {
            existsNskms = bs;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (mtbKM.Text.Equals(string.Empty) || mtbLength.Text.Equals(string.Empty))
            {
                MetroFramework.MetroMessageBox.Show(this, alerts.insert_error + " " + alerts.check_fields_filling, alerts.inserting, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Nskm = new NonstandardKm { Km = int.Parse(mtbKM.Text), Len = int.Parse(mtbLength.Text) };
            Result = DialogResult.OK;
            Close();
        }

        private void DigitalKeyFilter(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }
    }
}
