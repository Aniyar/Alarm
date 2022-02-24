using System;
using System.ComponentModel;
using System.Windows.Forms;

namespace ALARm.controls
{
    public partial class CoordControl : MetroFramework.Controls.MetroUserControl
    {
        private bool metersHidden;
        private bool finalCoordsHidden;
        private int titleWidth = 110;
        [
        Category("Coord"),
        Description("from 1 to 999999")
        ]
        public int StartKm
        {
            get; set;
        }
        [
        Category("Coord"),
        Description("from 1 to 999999")
        ]
        public int StartM
        {
            get; set;
        }
        [
        Category("Coord"),
        Description("from 1 to 999999")
        ]
        public int FinalKm
        {
            get; set;
        }
        [
        Category("Coord"),
        Description("from 1 to 999999")
        ]
        public int FinalM
        {
            get; set;
        }
        [
        Category("Coord"),
        Description("True or False")
        ]
        public bool MetersHidden
        {
            get {
                return metersHidden;
            }
            set {
                metersHidden = value;
                if (finalCoordsHidden)
                {
                    if (metersHidden)
                    {
                        mpStartM.Visible = false;
                        mpFinalM.Visible = false;
                        mpFinalKm.Visible = false;
                    }
                    else
                    {
                        mpStartM.Visible = true;
                        mpFinalKm.Visible = false;
                        mpFinalM.Visible = false;
                    }
                }
                else
                {
                    if (metersHidden)
                    {
                        mpStartM.Visible = false;
                        mpFinalM.Visible = false;
                        mpFinalKm.Visible = true;
                    }
                    else
                    {
                        mpStartM.Visible = true;
                        mpFinalM.Visible = true;
                        mpFinalKm.Visible = true;
                    }
                }
            }
        }
        [
       Category("Coord"),
       Description("True or False")
       ]
        public bool FinalCoordsHidden
        {
            get => finalCoordsHidden;
            set
            {
                finalCoordsHidden = value;
                if (metersHidden)
                {
                    if (finalCoordsHidden)
                    {
                        mpFinalKm.Visible = false;
                        mpFinalM.Visible = false;
                        mpStartM.Visible = false;
                    }
                    else
                    {
                        mpFinalKm.Visible = true;
                        mpStartM.Visible = false;
                        mpFinalM.Visible = false;
                    }
                }
                else
                {
                    if (finalCoordsHidden)
                    {
                        mpFinalKm.Visible = false;
                        mpFinalM.Visible = false;
                        mpStartM.Visible = true;
                    }
                    else
                    {
                        mpFinalKm.Visible = true;
                        mpFinalM.Visible = true;
                        mpStartM.Visible = true;
                    }
                }
            }
        }
        [
        Category("Titles"),
        Description("Tile finalkm label")
        ]
        public string StartKmTitle
        {
            get => mlStartKm.Text;
            set => mlStartKm.Text = value;
        }

        [
        Category("Titles"),
        Description("Tile finalmm label")
        ]
        public string StartMTitle
        {
            get => mlStartM.Text;
            set => mlStartM.Text = value;
        }
        [
        Category("Titles"),
        Description("Tile startkm label")
        ]
        public string FinalKmTitle
        {
            get => mlFinalKm.Text;
            set => mlFinalKm.Text = value;
        }

      
        [
        Category("Titles"),
        Description("Tile finalmm label")
        ]
        public string FinalMTitle
        {
            get => mlFinalM.Text;
            set => mlFinalM.Text = value;
        }
        [
        Category("Titles"),
        Description("Tiеle width")
        ]
        public int TitleWidth
        {
            get => titleWidth;
            set
            {
                titleWidth = value;
                mlFinalM.Width = titleWidth;
                mlFinalKm.Width = titleWidth;
                mlStartM.Width = titleWidth;
                mlStartKm.Width = titleWidth;
            }
        }

        public bool CorrectFilled {
            get {
                if (!finalCoordsHidden && !metersHidden && (tbStartKm.Text.Equals(string.Empty) || tbStartM.Text.Equals(string.Empty) || tbFinalKm.Text.Equals(string.Empty) || tbFinalM.Text.Equals(string.Empty) ||
                    (StartKm * 1000 + StartM >= FinalKm * 1000 + FinalM) || (StartKm < 0 || StartM < 0 || FinalKm < 0 || FinalM < 0)))
                    return false;
                else if (finalCoordsHidden && !metersHidden && (tbStartKm.Text.Equals(string.Empty) || tbStartM.Text.Equals(string.Empty) || (StartKm < 0 || StartM < 0)))
                    return false;
                else if (!finalCoordsHidden && metersHidden && (tbStartKm.Text.Equals(string.Empty) || tbFinalKm.Text.Equals(string.Empty) || (StartKm >= FinalKm) || (StartKm < 0 || FinalKm < 0)))
                    return false;
                else if (finalCoordsHidden && metersHidden && (tbStartKm.Text.Equals(string.Empty) || StartKm < 0))
                    return false;
                return true;
            }
        }

        public CoordControl()
        {
            InitializeComponent();
            if (finalCoordsHidden)
            {
                mpFinalKm.Visible = false;
                mpFinalM.Visible = false;
            }
        }

        public void Clear() {
            tbStartKm.Text = string.Empty;
            tbStartM.Text = string.Empty;
            tbFinalKm.Text = string.Empty;
            tbFinalM.Text = string.Empty;
        }

        public void SetValue(int startKm, int startM)
        {
            tbStartKm.Text = startKm.ToString();
            StartKm = startKm;
            tbStartM.Text = startM.ToString();
            StartM = startM;
        }

        public void SetValue(int startKm, int startM, int finalKm, int finalM)
        {
            tbStartKm.Text = startKm.ToString();
            StartKm = startKm;
            tbStartM.Text = startM.ToString();
            StartM = startM;
            tbFinalKm.Text = finalKm.ToString();
            FinalKm = finalKm;
            tbFinalM.Text = finalM.ToString();
            FinalM = finalM;
        }

        private void DigitKeyFilter(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        private void TbStartKm_KeyUp(object sender, KeyEventArgs e)
        {
            int.TryParse(tbStartKm.Text, out var km);
            StartKm = km;
        }

        private void TbStartM_KeyUp(object sender, KeyEventArgs e)
        {
            int m;
            int.TryParse(tbStartM.Text, out m);
            StartM = m;
        }

        private void TbFinalKm_KeyUp(object sender, KeyEventArgs e)
        {
            int km;
            int.TryParse(tbFinalKm.Text, out km);
            FinalKm = km;
        }

        private void TbFinalM_KeyUp(object sender, KeyEventArgs e)
        {
            int.TryParse(tbFinalM.Text, out var m);
            FinalM = m;
        }
    }
}
