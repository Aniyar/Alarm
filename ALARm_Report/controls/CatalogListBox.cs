using System;
using System.ComponentModel;
using System.Windows.Forms;
using ALARm.Core;
using ALARm.Core.Report;

namespace ALARm.controls
{
    public partial class CatalogListBox : MetroFramework.Controls.MetroUserControl
    {
        public int CurrentId
        {
            get
            {
                var catalog = lbReport.SelectedItem as ReportTemplate;
                return catalog != null ? catalog.ID : -1;
            }
        }
        public ReportTemplate CurrentValue
        {
            get
            {
                return lbReport.SelectedItem as ReportTemplate;
                
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
            lbReport.Items.Clear();
        }

        public CatalogListBox()
        {
            InitializeComponent();
        }

        public void SetDataSource(Object bs)
        {
            reportBindingSource.DataSource = bs;
            lbReport.SelectedIndex = -1;
        }
        public BindingSource GetDataSource()
        {
            //   return periodBindingSource;
            return reportBindingSource;
        }
    }
}
