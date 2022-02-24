using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Forms;
using ALARm.Core;

namespace ALARm.controls
{
    public partial class SwitchListBox : MetroFramework.Controls.MetroUserControl
    {
        public Int64 CurrentId {
            get {
                var switchunit = (Switch)cmbSwitchUnit.SelectedItem;
                Int64 neutral = -1;
                return switchunit?.Id ?? neutral;
            }
            set {
                for (int i = 0; i < cmbSwitchUnit.Items.Count; ++i)
                {
                    if (((Switch)cmbSwitchUnit.Items[i]).Id == value)
                    {
                        cmbSwitchUnit.SelectedIndex = i;
                        return;
                    }
                }
            }
        }

        public string CurrentValue {
            get {
                var switchunit = (Switch)cmbSwitchUnit.SelectedItem;
                return switchunit?.Num ?? string.Empty;
            }
            set {
                for (int i = 0; i < cmbSwitchUnit.Items.Count; ++i)
                {
                    if (((Switch)cmbSwitchUnit.Items[i]).Num == value)
                    {
                        cmbSwitchUnit.SelectedIndex = i;
                        return;
                    }
                }
            }
        }

        [
        Category("Appearance"),
        Description("SwitchUnitLabel")
        ]

        public string Title {
            get => lbTitle.Text;
            set => lbTitle.Text = value;
        }

        [
        Category("Appearance"),
        Description("SwitchUnitLabel")
        ]
        public int TitleWidth {
            get {
                return lbTitle.Width;
            }
            set {
                lbTitle.Width = value;

            }
        }

        internal void Clear()
        {
            cmbSwitchUnit.Items.Clear();
        }

        public SwitchListBox()
        {
            InitializeComponent();
        }

        public void SetDataSource(object bs)
        {
            switchBindingSource.DataSource = bs;
            cmbSwitchUnit.SelectedIndex = -1;
        }

        public event EventHandler SelectionChanged;

        private void CmbSwitchUnit_SelectedIndexChanged(object sender, EventArgs e)
        {
            SelectionChanged?.Invoke(this, e);
        }
    }
}
