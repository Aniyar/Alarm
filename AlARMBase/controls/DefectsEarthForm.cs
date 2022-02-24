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
    public partial class DefectsEarthForm : MetroFramework.Forms.MetroForm
    {
        public DefectsEarth defectsEarth;
        private BindingSource existsSource;
        public DialogResult result = DialogResult.Cancel;
        public DefectsEarthForm()
        {
            InitializeComponent();
            catalogListBox1.SetDataSource(MainTrackStructureService.GetCatalog(MainTrackStructureConst.MtoDefectsEarth));
        }
        public void SetForm(DefectsEarth obj)
        {
            Text = "Изменение записи";
            coordControl.SetValue(obj.Start_Km, obj.Start_M, obj.Final_Km, obj.Final_M);
            catalogListBox1.CurrentId = obj.Type_id;
        }

        internal void SetExistingSource(BindingSource bindingSource)
        {
            existsSource = bindingSource;
        }
        public void ClearForm()
        {
            coordControl.Clear();
            catalogListBox1.Clear();
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (coordControl.CorrectFilled == false || catalogListBox1.CurrentId < 0)
            {
                MetroFramework.MetroMessageBox.Show(this, alerts.insert_error + " " + alerts.check_fields_filling, alerts.inserting, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            defectsEarth = new DefectsEarth {
                Start_Km = coordControl.StartKm,
                Start_M = coordControl.StartM,
                Final_Km = coordControl.FinalKm,
                Final_M = coordControl.FinalM,
                Type = catalogListBox1.CurrentValue,
                Type_id = catalogListBox1.CurrentId
            };
            result = DialogResult.OK;
            Close();
        }
    }
}
