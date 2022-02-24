using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ALARm.Core;

namespace ALARm_Report.controls
{
    public partial class PeriodComboBox : MetroFramework.Controls.MetroUserControl
    {
        public PeriodComboBox()
        {
            InitializeComponent();
        }
        public ReportPeriod Current => cmbAdmUnit.SelectedItem as ReportPeriod;
      
        public string CurrentValue
        {
            get
            {
                var period = cmbAdmUnit.SelectedItem as ReportPeriod;
                return period != null ? period.Period : String.Empty;
            }
        }
        [
            Category("Appearance"),
            Description("CatalogLabel")
        ]
        public string Title
        {
            get
            {
                return lbTitle.Text;
            }
            set
            {
                lbTitle.Text = value;

            }
        }

        [
            Category("Appearance"),
            Description("CatalogLabel")
        ]
        public int TitleWidth
        {
            get
            {
                return lbTitle.Width;
            }
            set
            {
                lbTitle.Width = value;

            }
        }



        internal void Clear()
        {
            periodBindingSource.Clear();
        }

      

        public void SetDataSource(Object bs)
        {
            periodBindingSource.DataSource = bs;
            cmbAdmUnit.SelectedIndex = -1;
        }
        public BindingSource GetDataSource()
        {
            //   return periodBindingSource;
            return periodBindingSource;
        }
        public event EventHandler SelectionChanged;

        private void cmbAdmUnit_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectionChanged?.Invoke(this, e);
        }
    }
}
