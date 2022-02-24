using ALARm.Core;
using ALARm.Services;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ALARm.controls
{
    public partial class StCurveForm : MetroFramework.Forms.MetroForm
    {
        public object StCurve;
        private BindingSource existsSections;
        public DialogResult Result = DialogResult.Cancel;
        public StCurveForm()
        {
            InitializeComponent();
            catalogWidth.SetDataSource(new List<Catalog>() { new Catalog { Id = 1520, Name = "1520" }, new Catalog { Id = 1524, Name = "1524" }, new Catalog { Id = 1530, Name = "1530" }, new Catalog { Id = 1535, Name = "1535" }, new Catalog { Id = 1540, Name = "1540" } });
        }
        public void SetStCurveForm(Curve obj)
        {
            MainCoord.SetValue(obj.Start_Km, obj.Start_M, obj.Final_Km, obj.Final_M);
            catalogWidth.SetDataSource(new List<Catalog>() { new Catalog { Id = 1520, Name = "1520" }, new Catalog { Id = 1524, Name = "1524" }, new Catalog { Id = 1530, Name = "1530" }, new Catalog { Id = 1535, Name = "1535" }, new Catalog { Id = 1540, Name = "1540" } });
        }
        public void SetStCurveForm(StCurve obj)
        {
            Text = "Изменение записи";
            MainCoord.SetValue(obj.Start_Km, obj.Start_M, obj.Final_Km, obj.Final_M);
            tbRadius.Text = obj.Radius.ToString();
            LevelCoord.SetValue(obj.Transition_1, obj.Transition_2);
            catalogWidth.CurrentId = obj.Width;
            tbWear.Text = obj.Wear.ToString();
        }

        public void SetExistingSectionsSource(BindingSource bindingSource)
        {
            existsSections = bindingSource;
            catalogWidth.SetDataSource(new List<Catalog>() { new Catalog { Id = 1520, Name = "1520" }, new Catalog { Id = 1524, Name = "1524" }, new Catalog { Id = 1530, Name = "1530" }, new Catalog { Id = 1535, Name = "1535" }, new Catalog { Id = 1540, Name = "1540" } });
        }

        private void DigitKeyFilter(object sender, KeyPressEventArgs e)
        {
            e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (MainCoord.CorrectFilled == false || LevelCoord.CorrectFilled == false || tbRadius.Text == string.Empty || tbWear.Text == string.Empty || catalogWidth.CurrentId == -1)
            {
                MetroFramework.MetroMessageBox.Show(this, alerts.insert_error + " " + alerts.check_fields_filling, alerts.inserting, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            StCurve = new StCurve
            {
                Start_Km = MainCoord.StartKm,
                Start_M = MainCoord.StartM,
                Final_Km = MainCoord.FinalKm,
                Final_M = MainCoord.FinalM,
                Transition_1 = LevelCoord.StartKm,
                Transition_2 = LevelCoord.StartM,
                Radius = int.Parse(tbRadius.Text),
                Wear = int.Parse(tbWear.Text),
                Width = catalogWidth.CurrentId
            };

            Result = DialogResult.OK;
            Close();
        }

        private void BtnCancel_Click(object sender, EventArgs e)
        {
            Result = DialogResult.Cancel;
            Close();
        }
    }
}
