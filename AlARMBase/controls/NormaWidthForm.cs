using ALARm.Core;
using System;
using System.Windows.Forms;

namespace ALARm.controls
{
    public partial class NormaWidthForm : MetroFramework.Forms.MetroForm
    {
        public NormaWidth Norma;
        private BindingSource existsSource;
        public DialogResult Result = DialogResult.Cancel;

        public NormaWidthForm()
        {
            InitializeComponent();
        }

        public NormaWidthForm(NormaWidth obj)
        {
            Text = "Изменение записи";
            InitializeComponent();
            coordControl.SetValue(obj.Start_Km, obj.Start_M, obj.Final_Km, obj.Final_M);
            msNorma.SetValue(obj.Norma_Width);
        }

        internal void SetExistingSource(BindingSource bindingSource)
        {
            existsSource = bindingSource;
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            if (coordControl.CorrectFilled == false || msNorma.GetValue() < 0)
            {
                MetroFramework.MetroMessageBox.Show(this, alerts.insert_error + " " + alerts.check_fields_filling, alerts.inserting, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            Norma = new NormaWidth
            {
                Start_Km = coordControl.StartKm,
                Start_M = coordControl.StartM,
                Final_Km = coordControl.FinalKm,
                Final_M = coordControl.FinalM,
                Norma_Width = msNorma.GetValue()
            };
            Result = DialogResult.OK;
            Close();
        }

        
    }
}
