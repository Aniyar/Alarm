using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ALARm.Core;
using ALARm.Services;

namespace ALARm_Report.controls
{
    public partial class MainParametersProcessPeriod : MetroFramework.Forms.MetroForm
    {
        public DialogResult Result = DialogResult.Cancel;
        public long mainProcessId = -1;

        public MainParametersProcessPeriod()
        {
            InitializeComponent();
            mainParametersProcessBindingSource.DataSource = RdStructureService.GetMainParametersProcess();
            if (ProcessDataComboBox.Items.Count < 1)
                ChooseButton.Hide();
            else
                ProcessDataComboBox.SelectedIndex = -1;
        }

        private void ChooseButton_Click(object sender, EventArgs e)
        {
            Result = DialogResult.OK;
            Close();
        }

        private void ProcessDataComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {
                mainProcessId = ((MainParametersProcess)ProcessDataComboBox.SelectedItem).Id;
            }
            catch
            {
                mainProcessId = -1;
            }
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
