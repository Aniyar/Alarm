using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using ALARm.Core;

namespace ALARm.controls
{
    public partial class AdmListBox : MetroFramework.Controls.MetroUserControl
    {
        public Int64 CurrentId {
            get {
                var admunit = (AdmUnit)cmbAdmUnit.SelectedItem;
                Int64 neutral = -1;
                return admunit?.Id ?? neutral;
            }
            set {
                for (int i = 0; i < cmbAdmUnit.Items.Count; ++i)
                {
                    if (((AdmUnit)cmbAdmUnit.Items[i]).Id == value)
                    {
                        cmbAdmUnit.SelectedIndex = i;
                        return;
                    }
                }
            }
        }

        public string CurrentValue {
            get {
                var admunit = (AdmUnit)cmbAdmUnit.SelectedItem;
                return admunit?.Name ?? string.Empty;
            }
            set {
                for (int i = 0; i < cmbAdmUnit.Items.Count; ++i)
                {
                    if (((AdmUnit)cmbAdmUnit.Items[i]).Name == value)
                    {
                        cmbAdmUnit.SelectedIndex = i;
                        return;
                    }
                }
            }
        }

        public string CurrentCode {
            get {
                var admunit = (AdmUnit)cmbAdmUnit.SelectedItem;
                return admunit?.Code ?? string.Empty;
            }
            set {
                for (int i = 0; i < cmbAdmUnit.Items.Count; ++i)
                {
                    if (((AdmUnit)cmbAdmUnit.Items[i]).Code == value)
                    {
                        cmbAdmUnit.SelectedIndex = i;
                        return;
                    }
                }
            }
        }

        [
        Category("Appearance"),
        Description("AdmUnitLabel")
        ]

        public string Title
        {
            get => lbTitle.Text;
            set => lbTitle.Text = value;
        }

        [
        Category("Appearance"),
        Description("AdmUnitLabel")
        ]
        public int TitleWidth {
            get {
                return lbTitle.Width;
            }
            set {
                lbTitle.Width = value;

            }
        }

        [
            Category("Appearance"),
            Description("AdmUnitLabel")
        ]

        public string DisplayMember {
            get => cmbAdmUnit.DisplayMember;
            set => cmbAdmUnit.DisplayMember = value;
        }

        internal void Clear()
        {
            cmbAdmUnit.Items.Clear();
        }

        public AdmListBox()
        {
            InitializeComponent();
        }

        public void SetDataSource(object bs)
        {
            admUnitBindingSource.DataSource = bs;
            cmbAdmUnit.SelectedIndex = -1;
        }

        public BindingSource GetDataSource()
        {
            //return periodBindingSource;
            return admUnitBindingSource;
        }

        public event EventHandler SelectionChanged;

        private void CmbAdmUnit_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectionChanged?.Invoke(this, e);
        }
    }
}
