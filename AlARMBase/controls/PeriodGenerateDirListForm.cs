using System;
using ALARm.Core;
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
    public partial class PeriodGenerateDirListForm : MetroFramework.Forms.MetroForm
    {
        public DialogResult Result = DialogResult.Cancel;
        public Period Period;
        public PeriodGenerateDirListForm()
        {
            InitializeComponent();
            mdtStartDate.Value = DateTime.Now;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            Period = new Period { Start_Date = mdtStartDate.Value.Date };
            Result = DialogResult.OK;
            Close();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            Result = DialogResult.Cancel;
            Close();
        }
    }
}
