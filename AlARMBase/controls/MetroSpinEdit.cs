using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace ALARm.controls
{
    public partial class MetroSpinEdit : MetroFramework.Controls.MetroUserControl
    {
        private int defaultValue;
        private int interval = 1;
        [
        Category("Flash"),
        Description("any integer value")
        ]
        public int DefaultValue
        {
            get => defaultValue;
            set
            {
                defaultValue = value;
                valueTextBox.Text = DefaultValue.ToString();
                Invalidate();
            }
        }
        [
       Category("Flash"),
       Description("any integer value")
       ]
        public int Interval
        {
            get => interval;
            set
            {
                interval = value;

                Invalidate();
            }
        }
        public int GetValue()
        {
            return int.Parse(valueTextBox.Text);
        }
        public void SetValue(int value)
        {
            valueTextBox.Text = value.ToString();
        }
        public MetroSpinEdit()
        {
            InitializeComponent();
        }

        private void upBtn_Click(object sender, EventArgs e)
        {
            valueTextBox.Text = (int.Parse(valueTextBox.Text) + interval).ToString();
        }

        private void downBtn_Click(object sender, EventArgs e)
        {
            valueTextBox.Text = (int.Parse(valueTextBox.Text) - interval).ToString();
        }
        private void DigitKeyFilter(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }
    }
}
