using ALARm.Core;
using ALARm.Services;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace ALARm.controls
{
    public partial class CurveForm : MetroFramework.Forms.MetroForm
    {
        public object Curve;
        private BindingSource existsSections;
        public DialogResult Result = DialogResult.Cancel;
        public CurveForm()
        {
            InitializeComponent();
        }
        public void SetCurveForm(Curve obj)
        {
            Text = "Изменение записи";
            catalogListSide.CurrentId = obj.Side_id;
            MainCoord.SetValue(obj.Start_Km, obj.Start_M, obj.Final_Km, obj.Final_M);

        }
        public void SetElCurveForm(Curve obj)
        {
            MainCoord.SetValue(obj.Start_Km, obj.Start_M, obj.Final_Km, obj.Final_M);
        }
        public void SetElCurveForm(ElCurve obj)
        {
            Text = "Изменение записи";
            MainCoord.SetValue(obj.Start_Km, obj.Start_M, obj.Final_Km, obj.Final_M);
            LevelCoord.SetValue(obj.Transition_1, obj.Transition_2);
            tbLevel.Text = obj.Lvl.ToString();
        }

        public void SetExistingSectionsSource(BindingSource bindingSource)
        {
            catalogListSide.SetDataSource(MainTrackStructureService.GetCatalog(MainTrackStructureConst.CatSide));
            existsSections = bindingSource;
            if (this is ElCurveForm)
            {
                catalogListSide.Visible = false;
                LevelCoord.Visible = true;
                mpLevel.Visible = true;
                Height = 400;
            }
        }

        private void BtnSave_Click(object sender, EventArgs e)
        {
            if (this is ElCurveForm)
            {
                if (MainCoord.CorrectFilled == false || LevelCoord.CorrectFilled == false || tbLevel.Text == string.Empty)
                {
                    MetroFramework.MetroMessageBox.Show(this, alerts.insert_error + " " + alerts.check_fields_filling, alerts.inserting, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            } else
            {
                if (MainCoord.CorrectFilled == false || catalogListSide.CurrentId == -1)
                {
                    MetroFramework.MetroMessageBox.Show(this, alerts.insert_error + " " + alerts.check_fields_filling, alerts.inserting, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            if (this is ElCurveForm) {
                Curve = new ElCurve
                {
                    Start_Km = MainCoord.StartKm,
                    Start_M = MainCoord.StartM,
                    Final_Km = MainCoord.FinalKm,
                    Final_M = MainCoord.FinalM,
                    Transition_1 = LevelCoord.StartKm,
                    Transition_2 = LevelCoord.StartM,
                    Lvl = int.Parse(tbLevel.Text)
                };
            } else
            {
                Curve = new Curve
                {
                    Side = catalogListSide.CurrentValue,
                    Side_id = catalogListSide.CurrentId,
                    Start_Km = MainCoord.StartKm,
                    Start_M = MainCoord.StartM,
                    Final_Km = MainCoord.FinalKm,
                    Final_M = MainCoord.FinalM,
                };
            }

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
