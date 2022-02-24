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
    public partial class  RefPointForm : MetroFramework.Forms.MetroForm
    {
        public RefPoint point;
        private BindingSource existsSource;
        public DialogResult result = DialogResult.Cancel;
        public RefPointForm()
        {
            InitializeComponent();
        }
        public void SetRefPointForm(RefPoint obj)
        {
            Text = "Изменение записи";
            tbAbscoord.Text = obj.Mark.ToString();
            coordControl1.SetValue(obj.Km, obj.Meter);
        }
        internal void SetExistingSource(BindingSource bindingSource)
        {
            existsSource = bindingSource;
        }
        public void ClearForm()
        {
            tbAbscoord.Text = String.Empty;
            coordControl1.Clear();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            double mark = 0;

            if (!double.TryParse(tbAbscoord.Text, out mark) || !coordControl1.CorrectFilled)
            {
                MetroFramework.MetroMessageBox.Show(this, alerts.insert_error + " " + alerts.check_fields_filling, alerts.inserting, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            point = new RefPoint {
                Km = coordControl1.StartKm,
                Meter = coordControl1.StartM,
                Mark = mark
            };
            result = DialogResult.OK;
            Close();
        }
    }
}
