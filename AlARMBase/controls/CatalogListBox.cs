using System;
using System.ComponentModel;
using System.Windows.Forms;
using ALARm.Core;

namespace ALARm.controls
{
    public partial class CatalogListBox : MetroFramework.Controls.MetroUserControl
    {
        public int CurrentId
        {
            get
            {
                var catalog = cmbAdmUnit.SelectedItem as Catalog;
                return catalog != null ? catalog.Id : -1;
            }
            set {
                for (int i = 0; i < cmbAdmUnit.Items.Count; ++i)
                {
                    if (((Catalog)cmbAdmUnit.Items[i]).Id == value)
                    {
                        cmbAdmUnit.SelectedIndex = i;
                        return;
                    }
                }
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
        public string CurrentMarkValue 
        {
            get {
                var catalog = cmbAdmUnit.SelectedItem as Catalog;
                return catalog != null ? catalog.Mark : String.Empty;
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

        public CatalogListBox()
        {
            InitializeComponent();
        }

        public void SetDataSource(Object bs)
        {
            catalogBindingSource.DataSource = bs;
            cmbAdmUnit.SelectedIndex = 0;
        }
        public void SetDisplayMember(string displayMember)
        {
            cmbAdmUnit.DisplayMember = displayMember;
        }
        public BindingSource GetDataSource()
        {
            //   return periodBindingSource;
            return catalogBindingSource;
        }
    }
}
