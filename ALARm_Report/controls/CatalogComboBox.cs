using System;
using System.ComponentModel;
using System.Windows.Forms;
using ALARm.Core;

namespace ALARm.controls
{
    public partial class CatalogComboBox : MetroFramework.Controls.MetroUserControl
    {


        public int CurrentId
        {
            get
            {
                var catalog = cmbAdmUnit.SelectedItem as Catalog;
                return catalog != null ? catalog.Id : -1;
            }
        }
        public string CurrentValue
        {
            get
            {
                var catalog = cmbAdmUnit.SelectedItem as Catalog;
                return catalog != null ? catalog.Name : String.Empty;
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
            cmbAdmUnit.Items.Clear();
        }

        public CatalogComboBox()
        {
            InitializeComponent();
        }

        public void SetDataSource(Object bs)
        {
            catalogBindingSource.DataSource = bs;
            cmbAdmUnit.SelectedIndex = -1;
        }
        public BindingSource GetDataSource()
        {
            //   return periodBindingSource;
            return catalogBindingSource;
        }
        public event EventHandler SelectionChanged;

        private void cmbAdmUnit_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectionChanged?.Invoke(this, e);
        }
    }
}
