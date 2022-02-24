using System;
using System.Windows.Forms;
using ALARm.Core;
using ALARm.Services;
using ALARm.controls;
using MetroFramework;
using ALARm;
using System.IO;
using System.Xml.Linq;
using System.Collections.Generic;
using ALARm.Core.Report;
using System.Linq;
using static ALARm.Core.MainTrackStructureConst;
using System.Drawing;

namespace ALARMBase
{
    public partial class MainForm : MetroFramework.Forms.MetroForm
    {
        public MainForm()
        {
            InitializeComponent();
            Build();
        }

        private void Build()
        {
            ucRoads.Build("ДОРОГИ", AdmStructureConst.AdmRoad);
            ucNods.Build("НОД", AdmStructureConst.AdmNod);
            ucDistances.Build("ДИСТАНЦИИ", AdmStructureConst.AdmDistance);
            ucPchu.Build("ПЧУ", AdmStructureConst.AdmPchu);
            ucPD.Build("ПД", AdmStructureConst.AdmPd);
            ucPDB.Build("ПДБ", AdmStructureConst.AdmPdb);
            ucDirections.Build("Направления", AdmStructureConst.AdmDirection);
            ucTracks.Build("Пути", AdmStructureConst.AdmTrack);
            scStation.Build("Станции", AdmStructureConst.AdmStation);
            scPark.Build("Парки", AdmStructureConst.AdmPark);
            scStationObject.Build("Обекты ст. путей", AdmStructureConst.AdmStationObject);
            scStationTracks.Build("Станционные пути", AdmStructureConst.AdmStationTrack);

            pcSectionPeriod.Build(MainTrackStructureConst.MtoDistSection);
            pcCrosstiePeriod.Build(MainTrackStructureConst.MtoCrossTie);
            pcNonstandartKmPeriod.Build(MainTrackStructureConst.MtoNonStandard);
            pcTrackClassPeriod.Build(MainTrackStructureConst.MtoTrackClass);
            pcRailsBracesPeriod.Build(MainTrackStructureConst.MtoRailsBrace);
            pcNormaPeriod.Build(MainTrackStructureConst.MtoNormaWidth);
            pcSpeedPeriod.Build(MainTrackStructureConst.MtoSpeed);
            pcTempSpeedPeriod.Build(MainTrackStructureConst.MtoTempSpeed);
            pcPdbSectionPeriod.Build(MainTrackStructureConst.MtoPdbSection);
            pcStationSectionPeriod.Build(MainTrackStructureConst.MtoStationSection);
            pcCurvePeriod.Build(MainTrackStructureConst.MtoCurve);
            pcStraighteningThreadPeriod.Build(MainTrackStructureConst.MtoStraighteningThread);
            pcArtificialConstructionPeriod.Build(MainTrackStructureConst.MtoArtificialConstruction);
            pcSwitchPeriod.Build(MainTrackStructureConst.MtoSwitch);
            pcElevationPeriod.Build(MainTrackStructureConst.MtoElevation);
            pcRailsSectionsPeriod.Build(MainTrackStructureConst.MtoRailSection);
            pcLongRailsPeriod.Build(MainTrackStructureConst.MtoLongRails);
            pcNonExistKm.Build(MainTrackStructureConst.MtoNonExtKm);
            pcProfileObjectPeriod.Build(MainTrackStructureConst.MtoProfileObject);
            pcChamJoint.Build(MainTrackStructureConst.MtoChamJoint);
            pcTraffic.Build(MainTrackStructureConst.MtoTraffic);
            pcWaycat.Build(MainTrackStructureConst.MtoWaycat);
            pcRefPoint.Build(MainTrackStructureConst.MtoRefPoint);
            pcProfmarks.Build(MainTrackStructureConst.MtoProfmarks);
            pcRfid.Build(MainTrackStructureConst.MtoRFID);
            pcCheckSection.Build(MainTrackStructureConst.MtoCheckSection);
            pcDistanceBetweenTracksPeriod.Build(MainTrackStructureConst.MtoDistanceBetweenTracks);
            pcDefectsEarthPeriod.Build(MainTrackStructureConst.MtoDefectsEarth);
            pcCoordinateGNSSPeriod.Build(MainTrackStructureConst.MtoCoordinateGNSS);
            pcCommunicationPeriod.Build(MainTrackStructureConst.MtoCommunication);
            pcDeep.Build(MainTrackStructureConst.MtoDeep);
            pcBallastType.Build(MainTrackStructureConst.MtoBallastType);
            pcDimension.Build(MainTrackStructureConst.MtoDimension);
        }

        private void RemoveMainTrackObject(BindingSource bs, int mtoType)
        {
            if (MetroMessageBox.Show(this, alerts.remove_ask, "Удаление...", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                try
                {
                    var obj = bs.Current as MainTrackObject;
                    if (obj != null)
                    {
                        if (MainTrackStructureService.DeleteMtoObject(obj.Id, mtoType))
                        {
                            bs.RemoveCurrent();
                        }
                    }
                }
                catch (Exception ex)
                {
                    MetroMessageBox.Show(this, ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            ucRoads.SetDataSource(AdmStructureService.GetUnits(AdmStructureConst.AdmRoad, -1));

            //ucDirections.SetDataSource(AdmStructureService.GetUnits(AdmStructureConst.AdmDirection, -1));
        }

        private void ucRoads_UnitSelectionChanged(object sender, EventArgs e)
        {
            CleanTables(AdmStructureConst.AdmRoad);
            if (ucRoads.GetDataSource().Count > 0)
            {
                ucNods.ParentId = ucRoads.CurrentUnitId;
                ucNods.SetDataSource(AdmStructureService.GetUnits(ucNods.AdmLevel, ucNods.ParentId));
                ucDirections.ParentId = ucRoads.CurrentUnitId;
                ucDirections.SetDataSource(AdmStructureService.GetUnits(ucDirections.AdmLevel, ucDirections.ParentId));
            }
        }

        private void ucNods_UnitSelectionChanged(object sender, EventArgs e)
        {
            CleanTables(AdmStructureConst.AdmNod);
            if (ucNods.GetDataSource().Count > 0)
            {
                ucDistances.ParentId = ucNods.CurrentUnitId;
                ucDistances.SetDataSource(AdmStructureService.GetUnits(ucDistances.AdmLevel, ucDistances.ParentId));
                scStation.parentId = ucNods.CurrentUnitId;
                scStation.SetDataSource(AdmStructureService.GetUnits(scStation.AdmLevel, scStation.parentId));
            }
        }

        private void ucDistances_UnitSelectionChanged(object sender, EventArgs e)
        {
            CleanTables(AdmStructureConst.AdmDistance);
            if (ucNods.GetDataSource().Count > 0)
            {
                ucPchu.ParentId = ucDistances.CurrentUnitId;
                ucPchu.SetDataSource(AdmStructureService.GetUnits(ucPchu.AdmLevel, ucPchu.ParentId));
            }
        }

        private void ucPchu_UnitSelectionChanged(object sender, EventArgs e)
        {
            CleanTables(AdmStructureConst.AdmPchu);
            if (ucNods.GetDataSource().Count > 0)
            {
                ucPD.ParentId = ucPchu.CurrentUnitId;
                ucPD.SetDataSource(AdmStructureService.GetUnits(ucPD.AdmLevel, ucPD.ParentId));
            }
        }

        private void CleanTables(int unitLevel)
        {
            if (unitLevel == 0)
            {
                ucNods.DataSourceClear();
                ucDirections.DataSourceClear();
            }               
            if (unitLevel <= 1)
            {
                ucDistances.DataSourceClear();
                scStation.DataSourceClear();
                scPark.DataSourceClear();
                scStationObject.DataSourceClear();
                scStationTracks.DataSourceClear();
            }
            if (unitLevel <= 2)
                ucPchu.DataSourceClear();
            if (unitLevel <= 3)
                ucPD.DataSourceClear();
            if (unitLevel <= 4)
                ucPDB.DataSourceClear();
            if (unitLevel == 6 || unitLevel == 0)
            {
                ucTracks.DataSourceClear();
            }
            if (unitLevel == 7 || unitLevel == 6 || unitLevel == 0 || unitLevel == 11)
            {
                distSectionBindingSource.Clear();
                crossTieBindingSource.Clear();
                nonstandardKmBindingSource.Clear();
                railsBraceBindingSource.Clear();
                trackClassBindingSource.Clear();
                normaWidthBindingSource.Clear();
                speedBindingSource.Clear();
                tempSpeedBindingSource.Clear();
                elevationBindingSource.Clear();
                pdbSectionBindingSource.Clear();                
                curveBindingSource.Clear();
                elCurveBindingSource.Clear();
                stCurveBindingSource.Clear();
                straighteningThreadBindingSource.Clear();
                artificialConstructionBindingSource.Clear();
                switchBindingSource.Clear();
                railsSectionsBindingSource.Clear();
                longRailsBindingSource.Clear();
                nonExistKmBindingSource.Clear();
                profileObjectBindingSource.Clear();
                chamJointBindingSource.Clear();
                refPointBindingSource.Clear();
                profmarksBindingSource.Clear();
                waycatBindingSource.Clear();
                mtoTrafficBindingSource.Clear();
                repairProjectBindingSource.Clear();
                rfidBindingSource.Clear();
                checkSectionBindingSource.Clear();
                distanceBetweenTracksBindingSource.Clear();
                defectsEarthBindingSource.Clear();
                coordinateGNSSBindingSource.Clear();
                communicationBindingSource.Clear();
                stationSectionBindingSource.Clear();
                deepBindingSource.Clear();
                ballastTypeBindingSource.Clear();
                dimensionBindingSource.Clear();
            }
            if (unitLevel == 8)
            {
                scPark.DataSourceClear();
                scStationObject.DataSourceClear();
                scStationTracks.DataSourceClear();
            }
            if (unitLevel == 9)
            {
                scStationObject.DataSourceClear();
                scStationTracks.DataSourceClear();
            }
        }

        private void ucPD_UnitSelectionChanged(object sender, EventArgs e)
        {
            CleanTables(AdmStructureConst.AdmPd);
            if (ucNods.GetDataSource().Count > 0)
            {
                ucPDB.ParentId = ucPD.CurrentUnitId;
                ucPDB.SetDataSource(AdmStructureService.GetUnits(ucPDB.AdmLevel, ucPDB.ParentId));
            }
        }

        private void ucDirections_UnitSelectionChanged(object sender, EventArgs e)
        {
            CleanTables(AdmStructureConst.AdmDirection);
            if (ucDirections.GetDataSource().Count > 0)
            {
                ucTracks.ParentId = ucDirections.CurrentUnitId;
                ucTracks.SetDataSource(AdmStructureService.GetUnits(ucTracks.AdmLevel, ucTracks.ParentId));
            }
        }

        private void UcTracks_UnitSelectionChanged(object sender, EventArgs e)
        {
            CleanTables(AdmStructureConst.AdmTrack);
            if (ucTracks.GetDataSource().Count > 0)
            {
                switch(metroTabControl1.SelectedTab.Name)
                {
                    case "mtpSections":
                        SetParentTrack(pcSectionPeriod, MainTrackStructureConst.MtoDistSection);
                        break;
                    case "mtpPdbSections":
                        SetParentTrack(pcPdbSectionPeriod, MainTrackStructureConst.MtoPdbSection);
                        break;
                    case "mtpStationSections":
                        SetParentTrack(pcStationSectionPeriod, MainTrackStructureConst.MtoStationSection);
                        break;
                    case "mtpCheckSection":
                        SetParentTrack(pcCheckSection, MainTrackStructureConst.MtoCheckSection);
                        break;
                    case "mtpRfid":
                        SetParentTrack(pcRfid, MainTrackStructureConst.MtoRFID);
                        break;
                    case "mtpRepairProject":
                        repairProjectBindingSource.Clear();
                        repairProjectBindingSource.DataSource = MainTrackStructureService.GetMtoObjects(ucTracks.CurrentUnitId, MainTrackStructureConst.MtoRepairProject);
                        break;
                    case "mtpTraffic":
                        SetParentTrack(pcTraffic, MainTrackStructureConst.MtoTraffic);
                        break;
                    case "mtpWaycat":
                        SetParentTrack(pcWaycat, MainTrackStructureConst.MtoWaycat);
                        break;
                    case "mtpRefPoint":
                        SetParentTrack(pcRefPoint, MainTrackStructureConst.MtoRefPoint);
                        break;
                    case "mtpProfmarks":
                        SetParentTrack(pcProfmarks, MainTrackStructureConst.MtoProfmarks);
                        break;
                    case "mtpChamJoint":
                        SetParentTrack(pcChamJoint, MainTrackStructureConst.MtoChamJoint);
                        break;
                    case "mtpProfileObject":
                        SetParentTrack(pcProfileObjectPeriod, MainTrackStructureConst.MtoProfileObject);
                        break;
                    case "mtpNonExistKm":
                        SetParentTrack(pcNonExistKm, MainTrackStructureConst.MtoNonExtKm);
                        break;
                    case "mtbLongRails":
                        SetParentTrack(pcLongRailsPeriod, MainTrackStructureConst.MtoLongRails);
                        break;
                    case "mtpRailsSections":
                        SetParentTrack(pcRailsSectionsPeriod, MainTrackStructureConst.MtoRailSection);
                        break;
                    case "mtpCrosstie":
                        SetParentTrack(pcCrosstiePeriod, MainTrackStructureConst.MtoCrossTie);
                        break;
                    case "mtpNonStandart":
                        SetParentTrack(pcNonstandartKmPeriod, MainTrackStructureConst.MtoNonStandard);
                        break;
                    case "mtpTrackClass":
                        SetParentTrack(pcTrackClassPeriod, MainTrackStructureConst.MtoTrackClass);
                        break;
                    case "mtpBraces":
                        SetParentTrack(pcRailsBracesPeriod, MainTrackStructureConst.MtoRailsBrace);
                        break;
                    case "mtpTrackWidth":
                        SetParentTrack(pcNormaPeriod, MainTrackStructureConst.MtoNormaWidth);
                        break;
                    case "mtpRestrictions":
                        SetParentTrack(pcTempSpeedPeriod, MainTrackStructureConst.MtoTempSpeed);
                        break;
                    case "mtpElevations":
                        SetParentTrack(pcElevationPeriod, MainTrackStructureConst.MtoElevation);
                        break;
                    case "mtpSpeeds":
                        SetParentTrack(pcSpeedPeriod, MainTrackStructureConst.MtoSpeed);
                        break;
                    case "mtpCurves":
                        SetParentTrack(pcCurvePeriod, MainTrackStructureConst.MtoCurve);
                        break;
                    case "mtpArtificialConstructions":
                        SetParentTrack(pcArtificialConstructionPeriod, MainTrackStructureConst.MtoArtificialConstruction);
                        break;
                    case "mtpSwitch":
                        SetParentTrack(pcSwitchPeriod, MainTrackStructureConst.MtoSwitch);
                        break;
                    case "mtpStraighteningThread":
                        SetParentTrack(pcStraighteningThreadPeriod, MainTrackStructureConst.MtoStraighteningThread);
                        break;
                    case "mtpCommunication":
                        SetParentTrack(pcCommunicationPeriod, MainTrackStructureConst.MtoCommunication);
                        break;
                    case "mtpCoordinateGNSS":
                        SetParentTrack(pcCoordinateGNSSPeriod, MainTrackStructureConst.MtoCoordinateGNSS);
                        break;
                    case "mtpDefectsEarth":
                        SetParentTrack(pcDefectsEarthPeriod, MainTrackStructureConst.MtoDefectsEarth);
                        break;
                    case "mtpDistanceBetweenTracks":
                        SetParentTrack(pcDistanceBetweenTracksPeriod, MainTrackStructureConst.MtoDistanceBetweenTracks);
                        break;
                    case "mtpDeep":
                        SetParentTrack(pcDeep, MainTrackStructureConst.MtoDeep);
                        break;
                    case "mtpBallast":
                        SetParentTrack(pcBallastType, MainTrackStructureConst.MtoBallastType);
                        break;
                    case "mtpDimension":
                        SetParentTrack(pcDimension, MainTrackStructureConst.MtoDimension);
                        break;
                }
            }
        }

        private void SetParentTrack(PeriodControl period, int mtoObject)
        {
            if (msbShowStationTrackObjects.Checked == true)
            {
                if (AdmStructureService.GetStationTrack(scStationTracks.currentUnitId, scStationTracks.AdmLevel) != null)
                {
                    period.TrackId = AdmStructureService.GetStationTrack(scStationTracks.currentUnitId, scStationTracks.AdmLevel).Adm_track_id;
                    period.SetDataSource(MainTrackStructureService.GetPeriods(period.TrackId, mtoObject));
                }
            }
            else
            {
                period.TrackId = ucTracks.CurrentUnitId;
                period.SetDataSource(MainTrackStructureService.GetPeriods(ucTracks.CurrentUnitId, mtoObject));
            }           
        }

        private void tsbAddDistSection(object sender, EventArgs e)
        {
            if (pcSectionPeriod.GetDataSource().Count <= 0)
            {
                MetroMessageBox.Show(this, alerts.before_insert_period, alerts.inserting, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            using (var distSectionForm = new DistanceForm())
            {
                distSectionForm.LoadRoads(ucRoads.GetDataSource());
                distSectionForm.SetExistingSectionsSource(msSections.DataSource as BindingSource);
                distSectionForm.ShowDialog();
                if (distSectionForm.Result == DialogResult.Cancel)
                    return;
                var section = distSectionForm.Section as DistSection;
                section.Period_Id = pcSectionPeriod.CurrentId;
                MainTrackStructureService.InsertObject(section, MainTrackStructureConst.MtoDistSection);
                if (section.Id > -1)
                {
                    distSectionBindingSource.Add(section);
                    distSectionBindingSource.MoveLast();
                }
                else
                {
                    MessageHelper.ShowError(this, alerts.insert_error + " " + alerts.check_fields_filling);
                }
            }
        }

        private void tsbEditDistSection_Click(object sender, EventArgs e)
        {
            if (pcSectionPeriod.GetDataSource().Count <= 0)
            {
                MetroMessageBox.Show(this, alerts.before_insert_period, alerts.editing, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                var obj = distSectionBindingSource.Current as DistSection;
                using (var distSectionForm = new DistanceForm())
                {
                    distSectionForm.LoadRoads(ucRoads.GetDataSource());
                    distSectionForm.SetExistingSectionsSource(msSections.DataSource as BindingSource);
                    distSectionForm.SetDistanceForm(obj);
                    distSectionForm.ShowDialog();
                    if (distSectionForm.Result == DialogResult.Cancel)
                        return;
                    var section = distSectionForm.Section as DistSection;
                    Int64 distanceId = obj.DistanceId;
                    int s_km = obj.Start_Km, s_m = obj.Start_M, f_km = obj.Final_Km, f_m = obj.Final_M;
                    string distance = obj.Distance, road = obj.Road;
                    obj.Start_Km = section.Start_Km;
                    obj.Start_M = section.Start_M;
                    obj.Final_Km = section.Final_Km;
                    obj.Final_M = section.Final_M;
                    obj.DistanceId = section.DistanceId;
                    obj.Distance = section.Distance;
                    obj.Road = section.Road;

                    if (MainTrackStructureService.UpdateMtoObject(obj, MainTrackStructureConst.MtoDistSection))
                    {
                        distSectionBindingSource.EndEdit();
                        distSectionBindingSource.ResetCurrentItem();
                    }
                    else
                    {
                        obj.Start_Km = s_km;
                        obj.Start_M = s_m;
                        obj.Final_Km = f_km;
                        obj.Final_M = f_m;
                        obj.DistanceId = distanceId;
                        obj.Distance = distance;
                        obj.Road = road;
                        distSectionBindingSource.EndEdit();
                        distSectionBindingSource.ResetCurrentItem();
                        MetroMessageBox.Show(this, alerts.edit_error + " " + alerts.check_fields_filling, alerts.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch
            {
                MetroMessageBox.Show(this, alerts.edit_error, alerts.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void pcSectionPeriod_PeriodSelectionChanged(object sender, EventArgs e)
        {
            PeriodSelectionChanged(distSectionBindingSource, MainTrackStructureConst.MtoDistSection, pcSectionPeriod);
        }

        private void tsbRemoveDistSection_Click(object sender, EventArgs e)
        {
            RemoveMainTrackObject(distSectionBindingSource, MainTrackStructureConst.MtoDistSection);
        }

        private void tsbAddDimension_Click(object sender, EventArgs e)
        {
            if (pcDimension.GetDataSource().Count <= 0)
            {
                MetroMessageBox.Show(this, alerts.before_insert_period, alerts.inserting, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            using (var dimensionForm = new DimensionForm())
            {
                dimensionForm.ShowDialog();
                if (dimensionForm.Result == DialogResult.Cancel)
                    return;
                var dimension = dimensionForm.dimension;
                dimension.Period_Id = pcDimension.CurrentId;
                MainTrackStructureService.InsertObject(dimension, MainTrackStructureConst.MtoDimension);
                if (dimension.Id > -1)
                {
                    dimensionBindingSource.Add(dimension);
                    dimensionBindingSource.MoveLast();
                }
                else
                {
                    MessageHelper.ShowError(this, alerts.insert_error + " " + alerts.check_fields_filling);
                }
            }
        }

        private void tsbEditDimension_Click(object sender, EventArgs e)
        {
            if (pcDimension.GetDataSource().Count <= 0)
            {
                MetroMessageBox.Show(this, alerts.before_insert_period, alerts.editing, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                var obj = dimensionBindingSource.Current as Dimension;
                using (var dimensionForm = new DimensionForm())
                {
                    dimensionForm.SetForm(obj);
                    dimensionForm.ShowDialog();
                    if (dimensionForm.Result == DialogResult.Cancel)
                        return;
                    var dimension = dimensionForm.dimension;
                    int s_km = obj.Start_Km, s_m = obj.Start_M, f_km = obj.Final_Km, f_m = obj.Final_M, type_id = dimension.Type_id;
                    string type = obj.Type;
                    obj.Start_Km = dimension.Start_Km;
                    obj.Start_M = dimension.Start_M;
                    obj.Final_Km = dimension.Final_Km;
                    obj.Final_M = dimension.Final_M;
                    obj.Type_id = dimension.Type_id;
                    obj.Type = dimension.Type;

                    if (MainTrackStructureService.UpdateMtoObject(obj, MainTrackStructureConst.MtoDistSection))
                    {
                        dimensionBindingSource.EndEdit();
                        dimensionBindingSource.ResetCurrentItem();
                    }
                    else
                    {
                        obj.Start_Km = s_km;
                        obj.Start_M = s_m;
                        obj.Final_Km = f_km;
                        obj.Final_M = f_m;
                        obj.Type = type;
                        obj.Type_id = type_id;
                        dimensionBindingSource.EndEdit();
                        dimensionBindingSource.ResetCurrentItem();
                        MetroMessageBox.Show(this, alerts.edit_error + " " + alerts.check_fields_filling, alerts.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch
            {
                MetroMessageBox.Show(this, alerts.edit_error, alerts.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void pcDimensionPeriod_PeriodSelectionChanged(object sender, EventArgs e)
        {
            dimensionBindingSource.Clear();
            if (pcDimension.GetDataSource().Count > 0)
            {
                dimensionBindingSource.DataSource = MainTrackStructureService.GetMtoObjects(pcDimension.CurrentId, MainTrackStructureConst.MtoDimension);
            }
        }

        private void tsbDeleteDimension_Click(object sender, EventArgs e)
        {
            RemoveMainTrackObject(dimensionBindingSource, MainTrackStructureConst.MtoDimension);
        }

        private void tsbAddCrosstie_Click(object sender, EventArgs e)
        {
            if (pcCrosstiePeriod.GetDataSource().Count <= 0)
            {
                MetroMessageBox.Show(this, alerts.before_insert_period, alerts.inserting, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            using (var crosstieForm = new CrosstieForm())
            {
                crosstieForm.SetExistingCrosstiesSource(msCrossties.DataSource as BindingSource);
                crosstieForm.ShowDialog();
                if (crosstieForm.Result == DialogResult.Cancel)
                    return;
                var crosstie = crosstieForm.Crosstie;
                crosstie.Period_Id = pcCrosstiePeriod.CurrentId;
                MainTrackStructureService.InsertObject(crosstie, MainTrackStructureConst.MtoCrossTie);
                if (crosstie.Id > -1)
                {
                    crossTieBindingSource.Add(crosstie);
                    crossTieBindingSource.MoveLast();
                }
                else
                {
                    MessageHelper.ShowError(this, alerts.insert_error + " " + alerts.check_fields_filling);
                }
            }
        }

        private void tsbEditCrosstie_Click(object sender, EventArgs e)
        {
            if (pcCrosstiePeriod.GetDataSource().Count <= 0)
            {
                MetroMessageBox.Show(this, alerts.before_insert_period, alerts.editing, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                var obj = crossTieBindingSource.Current as CrossTie;
                using (var crosstieForm = new CrosstieForm())
                {
                    crosstieForm.SetExistingCrosstiesSource(msCrossties.DataSource as BindingSource);
                    crosstieForm.SetCrosstieForm(obj);
                    crosstieForm.ShowDialog();
                    if (crosstieForm.Result == DialogResult.Cancel)
                        return;
                    var crosstie = crosstieForm.Crosstie;
                    int s_km = obj.Start_Km, s_m = obj.Start_M, f_km = obj.Final_Km, f_m = obj.Final_M, typeId = obj.Crosstie_type_id;
                    string type = obj.CrossTie_type;
                    obj.Start_Km = crosstie.Start_Km;
                    obj.Start_M = crosstie.Start_M;
                    obj.Final_Km = crosstie.Final_Km;
                    obj.Final_M = crosstie.Final_M;
                    obj.CrossTie_type = crosstie.CrossTie_type;
                    obj.Crosstie_type_id = crosstie.Crosstie_type_id;

                    if (MainTrackStructureService.UpdateMtoObject(obj, MainTrackStructureConst.MtoCrossTie))
                    {
                        crossTieBindingSource.EndEdit();
                        crossTieBindingSource.ResetCurrentItem();
                    }
                    else
                    {
                        obj.Start_Km = s_km;
                        obj.Start_M = s_m;
                        obj.Final_Km = f_km;
                        obj.Final_M = f_m;
                        obj.CrossTie_type = type;
                        obj.Crosstie_type_id = typeId;
                        crossTieBindingSource.EndEdit();
                        crossTieBindingSource.ResetCurrentItem();
                        MetroMessageBox.Show(this, alerts.edit_error + " " + alerts.check_fields_filling, alerts.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch
            {
                MetroMessageBox.Show(this, alerts.edit_error, alerts.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void pcCrosstiePeriod_PeriodSelectionChanged(object sender, EventArgs e)
        {
            crossTieBindingSource.Clear();
            if (pcCrosstiePeriod.GetDataSource().Count > 0)
            {
                crossTieBindingSource.DataSource = MainTrackStructureService.GetMtoObjects(pcCrosstiePeriod.CurrentId, MainTrackStructureConst.MtoCrossTie);
            }
        }

        private void tsbDeleteCrosstie_Click(object sender, EventArgs e)
        {
            RemoveMainTrackObject(crossTieBindingSource, MainTrackStructureConst.MtoCrossTie);
        }

        private void pcNonExistKm_PeriodSelectionChanged(object sender, EventArgs e)
        {
            nonExistKmBindingSource.Clear();
            if (pcNonExistKm.GetDataSource().Count > 0)
            {
                nonExistKmBindingSource.DataSource = MainTrackStructureService.GetMtoObjects(pcNonExistKm.CurrentId, MainTrackStructureConst.MtoNonExtKm);
            }
        }

        private void tsbAddNonExistKm_Click(object sender, EventArgs e)
        {
            if (pcNonExistKm.GetDataSource().Count <= 0)
            {
                MetroMessageBox.Show(this, alerts.before_insert_period, alerts.inserting, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            using (var nonExistKmForm = new NonExistsKm())
            {
                nonExistKmForm.SetExistingsSource(mgNonExistKm.DataSource as BindingSource);
                nonExistKmForm.ShowDialog();
                if (nonExistKmForm.Result == DialogResult.Cancel)
                    return;
                var nexkm = nonExistKmForm.Nexkm;
                nexkm.Period_Id = pcNonExistKm.CurrentId;
                MainTrackStructureService.InsertObject(nexkm, MainTrackStructureConst.MtoNonExtKm);
                if (nexkm.Id > -1)
                {
                    nonExistKmBindingSource.Add(nexkm);
                    nonExistKmBindingSource.MoveLast();
                }
                else
                {
                    MessageHelper.ShowError(this, alerts.insert_error + " " + alerts.check_fields_filling);
                }
            }
        }

        private void tsbEditNonExistKm_Click(object sender, EventArgs e)
        {
            if (pcNonExistKm.GetDataSource().Count <= 0)
            {
                MetroMessageBox.Show(this, alerts.before_insert_period, alerts.editing, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                var obj = nonExistKmBindingSource.Current as NonExtKm;
                using (var nonExistKmForm = new NonExistsKm(obj))
                {
                    nonExistKmForm.SetExistingsSource(mgNonExistKm.DataSource as BindingSource);
                    nonExistKmForm.ShowDialog();
                    if (nonExistKmForm.Result == DialogResult.Cancel)
                        return;
                    var nexkm = nonExistKmForm.Nexkm;
                    int km = obj.Km;
                    obj.Km = nexkm.Km;

                    if (MainTrackStructureService.UpdateMtoObject(obj, MainTrackStructureConst.MtoNonExtKm))
                    {
                        nonExistKmBindingSource.EndEdit();
                        nonExistKmBindingSource.ResetCurrentItem();
                    }
                    else
                    {
                        obj.Km = km;
                        nonExistKmBindingSource.EndEdit();
                        nonExistKmBindingSource.ResetCurrentItem();
                        MetroMessageBox.Show(this, alerts.edit_error + " " + alerts.check_fields_filling, alerts.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch
            {
                MetroMessageBox.Show(this, alerts.edit_error, alerts.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void pcNonstandartKm_PeriodSelectionChanged(object sender, EventArgs e)
        {
            nonstandardKmBindingSource.Clear();
            if (pcNonstandartKmPeriod.GetDataSource().Count > 0)
            {
                nonstandardKmBindingSource.DataSource = MainTrackStructureService.GetMtoObjects(pcNonstandartKmPeriod.CurrentId, MainTrackStructureConst.MtoNonStandard);
            }
        }

        private void tsbAddNonStandardKm_Click(object sender, EventArgs e)
        {
            if (pcNonstandartKmPeriod.GetDataSource().Count <= 0)
            {
                MetroMessageBox.Show(this, alerts.before_insert_period, alerts.inserting, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            using (var nonStandardKmForm = new NonstandardKms())
            {
                nonStandardKmForm.SetExistingsSource(mgNonstandartKm.DataSource as BindingSource);
                nonStandardKmForm.ShowDialog();
                if (nonStandardKmForm.Result == DialogResult.Cancel)
                    return;
                var nskm = nonStandardKmForm.Nskm;
                nskm.Period_Id = pcNonstandartKmPeriod.CurrentId;
                MainTrackStructureService.InsertObject(nskm, MainTrackStructureConst.MtoNonStandard);
                if (nskm.Id > -1)
                {
                    nonstandardKmBindingSource.Add(nskm);
                    nonstandardKmBindingSource.MoveLast();
                }
                else
                {
                    MessageHelper.ShowError(this, alerts.insert_error + " " + alerts.check_fields_filling);
                }
            }
        }

        private void tsbEditNonStandartKm_Click(object sender, EventArgs e)
        {
            if (pcNonstandartKmPeriod.GetDataSource().Count <= 0)
            {
                MetroMessageBox.Show(this, alerts.before_insert_period, alerts.editing, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                var obj = nonstandardKmBindingSource.Current as NonstandardKm;
                using (var nonStandardKmForm = new NonstandardKms(obj))
                {
                    nonStandardKmForm.SetExistingsSource(mgNonstandartKm.DataSource as BindingSource);
                    nonStandardKmForm.ShowDialog();
                    if (nonStandardKmForm.Result == DialogResult.Cancel)
                        return;
                    var nskm = nonStandardKmForm.Nskm;
                    int km = obj.Km, len = obj.Len;
                    obj.Km = nskm.Km;
                    obj.Len = nskm.Len;
                 
                    if (MainTrackStructureService.UpdateMtoObject(obj, MainTrackStructureConst.MtoNonStandard))
                    {
                        nonstandardKmBindingSource.EndEdit();
                        nonstandardKmBindingSource.ResetCurrentItem();
                    }
                    else
                    {
                        obj.Km = km;
                        obj.Len = len;
                        nonstandardKmBindingSource.EndEdit();
                        nonstandardKmBindingSource.ResetCurrentItem();
                        MetroMessageBox.Show(this, alerts.edit_error + " " + alerts.check_fields_filling, alerts.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch
            {
                MetroMessageBox.Show(this, alerts.edit_error, alerts.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void pcTrackClass_PeriodSelectionChanged(object sender, EventArgs e)
        {
            PeriodSelectionChanged(trackClassBindingSource, MainTrackStructureConst.MtoTrackClass, pcTrackClassPeriod);
        }

        private void tsbRemoveTrackClass_Click(object sender, EventArgs e)
        {
            RemoveMainTrackObject(trackClassBindingSource, MainTrackStructureConst.MtoTrackClass);
        }

        private void tsbAddTrackClass_Click(object sender, EventArgs e)
        {
            if (pcTrackClassPeriod.GetDataSource().Count <= 0)
            {
                MessageHelper.ShowWarning(this, alerts.inserting);
                return;
            }
            using (var trackclassForm = new TrackClassForm())
            {
                trackclassForm.SetExistingSource(mgTrackClass.DataSource as BindingSource);
                trackclassForm.ShowDialog();
                if (trackclassForm.result == DialogResult.Cancel)
                    return;
                var trackClass = trackclassForm.trackclass;
                trackClass.Period_Id = pcTrackClassPeriod.CurrentId;
                MainTrackStructureService.InsertObject(trackClass, MainTrackStructureConst.MtoTrackClass);
                if (trackClass.Id > -1)
                {
                    trackClassBindingSource.Add(trackClass);
                    trackClassBindingSource.MoveLast();
                }
                else
                {
                    MessageHelper.ShowError(this, alerts.insert_error + " " + alerts.check_fields_filling);
                }
            }
        }

        private void tsbEditTrackClass_Clcik(object sender, EventArgs e)
        {
            if (pcTrackClassPeriod.GetDataSource().Count <= 0)
            {
                MetroMessageBox.Show(this, alerts.before_insert_period, alerts.editing, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                var obj = trackClassBindingSource.Current as TrackClass;
                using (var trackclassForm = new TrackClassForm())
                {
                    trackclassForm.SetExistingSource(mgTrackClass.DataSource as BindingSource);
                    trackclassForm.SetTrackClassForm(obj);
                    trackclassForm.ShowDialog();
                    if (trackclassForm.result == DialogResult.Cancel)
                        return;
                    var trackClass = trackclassForm.trackclass;
                    int s_km = obj.Start_Km, s_m = obj.Start_M, f_km = obj.Final_Km, f_m = obj.Final_M, classId = obj.Class_Id;
                    string type = obj.Class_Type;
                    obj.Start_Km = trackClass.Start_Km;
                    obj.Start_M = trackClass.Start_M;
                    obj.Final_Km = trackClass.Final_Km;
                    obj.Final_M = trackClass.Final_M;
                    obj.Class_Id = trackClass.Class_Id;
                    obj.Class_Type = trackClass.Class_Type;

                    if (MainTrackStructureService.UpdateMtoObject(obj, MainTrackStructureConst.MtoTrackClass))
                    {
                        trackClassBindingSource.EndEdit();
                        trackClassBindingSource.ResetCurrentItem();
                    }
                    else
                    {
                        obj.Start_Km = s_km;
                        obj.Start_M = s_m;
                        obj.Final_Km = f_km;
                        obj.Final_M = f_m;
                        obj.Class_Id = classId;
                        obj.Class_Type = type;
                        trackClassBindingSource.EndEdit();
                        trackClassBindingSource.ResetCurrentItem();
                        MetroMessageBox.Show(this, alerts.edit_error + " " + alerts.check_fields_filling, alerts.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch
            {
                MetroMessageBox.Show(this, alerts.edit_error, alerts.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void tsbAddRailsBraceClick(object sender, EventArgs e)
        {
            if (pcRailsBracesPeriod.GetDataSource().Count <= 0)
            {
                MessageHelper.ShowWarning(this, alerts.inserting);
                return;
            }
            using (var railsBraceForm = new RailsBraceForm())
            {

                railsBraceForm.SetExistingSource(mgRailsBraces.DataSource as BindingSource);
                railsBraceForm.ShowDialog();
                if (railsBraceForm.Result == DialogResult.Cancel)
                    return;
                var railsBrace = railsBraceForm.RailsBrace;
                railsBrace.Period_Id = pcRailsBracesPeriod.CurrentId;
                MainTrackStructureService.InsertObject(railsBrace, MainTrackStructureConst.MtoRailsBrace);
                if (railsBrace.Id > -1)
                {
                    railsBraceBindingSource.Add(railsBrace);
                    railsBraceBindingSource.MoveLast();
                }
                else
                {
                    MessageHelper.ShowError(this, alerts.insert_error + " " + alerts.check_fields_filling);
                }
            }
        }

        private void tsbEditRailsBrace_Click(object sender, EventArgs e)
        {
            if (pcRailsBracesPeriod.GetDataSource().Count <= 0)
            {
                MetroMessageBox.Show(this, alerts.before_insert_period, alerts.editing, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                var obj = railsBraceBindingSource.Current as RailsBrace;
                using (var railsBraceForm = new RailsBraceForm())
                {
                    railsBraceForm.SetExistingSource(mgRailsBraces.DataSource as BindingSource);
                    railsBraceForm.SetRailsBraceForm(obj);
                    railsBraceForm.ShowDialog();
                    if (railsBraceForm.Result == DialogResult.Cancel)
                        return;
                    var railsBrace = railsBraceForm.RailsBrace;
                    int s_km = obj.Start_Km, s_m = obj.Start_M, f_km = obj.Final_Km, f_m = obj.Final_M, braceId = obj.Brace_Type_Id;
                    string type = obj.Brace_Type;
                    obj.Start_Km = railsBrace.Start_Km;
                    obj.Start_M = railsBrace.Start_M;
                    obj.Final_Km = railsBrace.Final_Km;
                    obj.Final_M = railsBrace.Final_M;
                    obj.Brace_Type_Id = railsBrace.Brace_Type_Id;
                    obj.Brace_Type = railsBrace.Brace_Type;

                    if (MainTrackStructureService.UpdateMtoObject(obj, MainTrackStructureConst.MtoRailsBrace))
                    {
                        railsBraceBindingSource.EndEdit();
                        railsBraceBindingSource.ResetCurrentItem();
                    }
                    else
                    {
                        obj.Start_Km = s_km;
                        obj.Start_M = s_m;
                        obj.Final_Km = f_km;
                        obj.Final_M = f_m;
                        obj.Brace_Type_Id = braceId;
                        obj.Brace_Type = type;
                        railsBraceBindingSource.EndEdit();
                        railsBraceBindingSource.ResetCurrentItem();
                        MetroMessageBox.Show(this, alerts.edit_error + " " + alerts.check_fields_filling, alerts.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch
            {
                MetroMessageBox.Show(this, alerts.edit_error, alerts.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void tsbRemoveRailsBrace_Click(object sender, EventArgs e)
        {
            RemoveMainTrackObject(railsBraceBindingSource, MainTrackStructureConst.MtoRailsBrace);
        }

        private void pcRailsBracesPeriod_PeriodSelectionChanged(object sender, EventArgs e)
        {
            PeriodSelectionChanged(railsBraceBindingSource, MainTrackStructureConst.MtoRailsBrace, pcRailsBracesPeriod);
        }

        private void tsbAddRailSection_Click(object sender, EventArgs e)
        {
            if (pcRailsSectionsPeriod.GetDataSource().Count <= 0)
            {
                MessageHelper.ShowWarning(this, alerts.inserting);
                return;
            }
            using (var railSectionForm = new RailsSectionsForm())
            {
                railSectionForm.SetExistingSource(mgRailsSections.DataSource as BindingSource);
                railSectionForm.ShowDialog();
                if (railSectionForm.Result == DialogResult.Cancel)
                    return;
                var railSection = railSectionForm.RailsSections;
                railSection.Period_Id = pcRailsSectionsPeriod.CurrentId;
                MainTrackStructureService.InsertObject(railSection, MainTrackStructureConst.MtoRailSection);
                if (railSection.Id > -1)
                {
                    railsSectionsBindingSource.Add(railSection);
                    railsSectionsBindingSource.MoveLast();
                }
                else
                {
                    MessageHelper.ShowError(this, alerts.insert_error + " " + alerts.check_fields_filling);
                }
            }
        }

        private void tsbEditRailSection_Click(object sender, EventArgs e)
        {
            if (pcRailsSectionsPeriod.GetDataSource().Count <= 0)
            {
                MetroMessageBox.Show(this, alerts.before_insert_period, alerts.editing, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                var obj = railsSectionsBindingSource.Current as RailsSections;
                using (var railSectionForm = new RailsSectionsForm())
                {
                    railSectionForm.SetExistingSource(mgRailsSections.DataSource as BindingSource);
                    railSectionForm.SetRailsSectionsForm(obj);
                    railSectionForm.ShowDialog();
                    if (railSectionForm.Result == DialogResult.Cancel)
                        return;
                    var railSection = railSectionForm.RailsSections;
                    int s_km = obj.Start_Km, s_m = obj.Start_M, f_km = obj.Final_Km, f_m = obj.Final_M, typeId = obj.Type_Id;
                    string type = obj.Type;
                    obj.Start_Km = railSection.Start_Km;
                    obj.Start_M = railSection.Start_M;
                    obj.Final_Km = railSection.Final_Km;
                    obj.Final_M = railSection.Final_M;
                    obj.Type_Id = railSection.Type_Id;
                    obj.Type = railSection.Type;

                    if (MainTrackStructureService.UpdateMtoObject(obj, MainTrackStructureConst.MtoRailSection))
                    {
                        railsSectionsBindingSource.EndEdit();
                        railsSectionsBindingSource.ResetCurrentItem();
                    }
                    else
                    {
                        obj.Start_Km = s_km;
                        obj.Start_M = s_m;
                        obj.Final_Km = f_km;
                        obj.Final_M = f_m;
                        obj.Type_Id = typeId;
                        obj.Type = type;
                        railsSectionsBindingSource.EndEdit();
                        railsSectionsBindingSource.ResetCurrentItem();
                        MetroMessageBox.Show(this, alerts.edit_error + " " + alerts.check_fields_filling, alerts.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch
            {
                MetroMessageBox.Show(this, alerts.edit_error, alerts.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void tsbRemoveRailSection_Click(object sender, EventArgs e)
        {
            RemoveMainTrackObject(railsSectionsBindingSource, MainTrackStructureConst.MtoRailSection);
        }

        private void pcRailsSectionsPeriod_PeriodSelectionChanged(object sender, EventArgs e)
        {
            PeriodSelectionChanged(railsSectionsBindingSource, MainTrackStructureConst.MtoRailSection, pcRailsSectionsPeriod);
        }

        private void tsbAddLongRails_Click(object sender, EventArgs e)
        {
            if (pcLongRailsPeriod.GetDataSource().Count <= 0)
            {
                MessageHelper.ShowWarning(this, alerts.inserting);
                return;
            }
            using (var longRailsForm = new LongRailsForm())
            {
                longRailsForm.SetExistingSource(mgLongRails.DataSource as BindingSource);
                longRailsForm.ShowDialog();
                if (longRailsForm.Result == DialogResult.Cancel)
                    return;
                var longRails = longRailsForm.LongRails;
                longRails.Period_Id = pcLongRailsPeriod.CurrentId;
                MainTrackStructureService.InsertObject(longRails, MainTrackStructureConst.MtoLongRails);
                if (longRails.Id > -1)
                {
                    longRailsBindingSource.Add(longRails);
                    longRailsBindingSource.MoveLast();
                }
                else
                {
                    MessageHelper.ShowError(this, alerts.insert_error + " " + alerts.check_fields_filling);
                }
            }
        }

        private void tsbEditLongRails_Click(object sender, EventArgs e)
        {
            if (pcLongRailsPeriod.GetDataSource().Count <= 0)
            {
                MetroMessageBox.Show(this, alerts.before_insert_period, alerts.editing, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                var obj = longRailsBindingSource.Current as LongRails;
                using (var longRailsForm = new LongRailsForm())
                {
                    longRailsForm.SetExistingSource(mgLongRails.DataSource as BindingSource);
                    longRailsForm.SetLongRailsForm(obj);
                    longRailsForm.ShowDialog();
                    if (longRailsForm.Result == DialogResult.Cancel)
                        return;
                    var longRails = longRailsForm.LongRails;
                    int s_km = obj.Start_Km, s_m = obj.Start_M, f_km = obj.Final_Km, f_m = obj.Final_M, typeid = obj.Type_id;
                    string type = obj.Type;
                    obj.Start_Km = longRails.Start_Km;
                    obj.Start_M = longRails.Start_M;
                    obj.Final_Km = longRails.Final_Km;
                    obj.Final_M = longRails.Final_M;
                    obj.Type_id = longRails.Type_id;
                    obj.Type = longRails.Type;

                    if (MainTrackStructureService.UpdateMtoObject(obj, MainTrackStructureConst.MtoLongRails))
                    {
                        longRailsBindingSource.EndEdit();
                        longRailsBindingSource.ResetCurrentItem();
                    }
                    else
                    {
                        obj.Start_Km = s_km;
                        obj.Start_M = s_m;
                        obj.Final_Km = f_km;
                        obj.Final_M = f_m;
                        obj.Type_id = typeid;
                        obj.Type = type;
                        longRailsBindingSource.EndEdit();
                        longRailsBindingSource.ResetCurrentItem();
                        MetroMessageBox.Show(this, alerts.edit_error + " " + alerts.check_fields_filling, alerts.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch
            {
                MetroMessageBox.Show(this, alerts.edit_error, alerts.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void tsbRemoveLongRails_Click(object sender, EventArgs e)
        {
            RemoveMainTrackObject(longRailsBindingSource, MainTrackStructureConst.MtoLongRails);
        }

        private void pcLongRailsPeriod_PeriodSelectionChanged(object sender, EventArgs e)
        {
            PeriodSelectionChanged(longRailsBindingSource, MainTrackStructureConst.MtoLongRails, pcLongRailsPeriod);
        }

        private void tsbRemoveNormaWidth_Click(object sender, EventArgs e)
        {
            RemoveMainTrackObject(normaWidthBindingSource, MainTrackStructureConst.MtoNormaWidth);
        }

        private void tsbAddNormaWidth_Click(object sender, EventArgs e)
        {
            if (pcNormaPeriod.GetDataSource().Count <= 0)
            {
                MessageHelper.ShowWarning(this, alerts.inserting);
                return;
            }
            using (var normaWidthForm = new NormaWidthForm())
            {

                normaWidthForm.SetExistingSource(mgNormaWidth.DataSource as BindingSource);
                normaWidthForm.ShowDialog();
                if (normaWidthForm.Result == DialogResult.Cancel)
                    return;
                var norma = normaWidthForm.Norma;
                norma.Period_Id = pcNormaPeriod.CurrentId;
                MainTrackStructureService.InsertObject(norma, MainTrackStructureConst.MtoNormaWidth);
                if (norma.Id > -1)
                {
                    normaWidthBindingSource.Add(norma);
                    nonstandardKmBindingSource.MoveLast();
                }
                else
                {
                    MessageHelper.ShowError(this, alerts.insert_error + " " + alerts.check_fields_filling);
                }
            }
        }

        private void tsbEditNormaWidth_Click(object sender, EventArgs e)
        {
            if (pcNormaPeriod.GetDataSource().Count <= 0)
            {
                MetroMessageBox.Show(this, alerts.before_insert_period, alerts.editing, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                var obj = normaWidthBindingSource.Current as NormaWidth;
                using (var normaWidthForm = new NormaWidthForm(obj))
                {
                    normaWidthForm.SetExistingSource(mgNormaWidth.DataSource as BindingSource);
                    normaWidthForm.ShowDialog();
                    if (normaWidthForm.Result == DialogResult.Cancel)
                        return;
                    var norma = normaWidthForm.Norma;
                    int s_km = obj.Start_Km, s_m = obj.Start_M, f_km = obj.Final_Km, f_m = obj.Final_M, norma_width = obj.Norma_Width;
                    obj.Start_Km = norma.Start_Km;
                    obj.Start_M = norma.Start_M;
                    obj.Final_Km = norma.Final_Km;
                    obj.Final_M = norma.Final_M;
                    obj.Norma_Width = norma.Norma_Width;

                    if (MainTrackStructureService.UpdateMtoObject(obj, MainTrackStructureConst.MtoNormaWidth))
                    {
                        normaWidthBindingSource.EndEdit();
                        normaWidthBindingSource.ResetCurrentItem();
                    }
                    else
                    {
                        obj.Start_Km = s_km;
                        obj.Start_M = s_m;
                        obj.Final_Km = f_km;
                        obj.Final_M = f_m;
                        obj.Norma_Width = norma_width;
                        normaWidthBindingSource.EndEdit();
                        normaWidthBindingSource.ResetCurrentItem();
                        MetroMessageBox.Show(this, alerts.edit_error + " " + alerts.check_fields_filling, alerts.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch
            {
                MetroMessageBox.Show(this, alerts.edit_error, alerts.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void pcNormaPeriod_PeriodSelectionChanged(object sender, EventArgs e)
        {
            PeriodSelectionChanged(normaWidthBindingSource, MainTrackStructureConst.MtoNormaWidth, pcNormaPeriod);
        }

        private void tsbAddSpeed_Click(object sender, EventArgs e)
        {
            if (pcSpeedPeriod.GetDataSource().Count <= 0)
            {
                MessageHelper.ShowWarning(this, alerts.inserting);
                return;
            }
            using (var speedForm = new SpeedForm())
            {
                speedForm.SetExistingSource(mgSpeed.DataSource as BindingSource);
                speedForm.ShowDialog();
                if (speedForm.result == DialogResult.Cancel)
                    return;
                var speed = speedForm.speed;
                speed.Period_Id = pcSpeedPeriod.CurrentId;
                MainTrackStructureService.InsertObject(speed, MainTrackStructureConst.MtoSpeed);
                if (speed.Id > -1)
                {
                    speedBindingSource.Add(speed);
                    speedBindingSource.MoveLast();
                }
                else
                {
                    MessageHelper.ShowError(this, alerts.insert_error + " " + alerts.check_fields_filling);
                }
            }
        }

        private void tsbEditSpeed_Click(object sender, EventArgs e)
        {
            if (pcSpeedPeriod.GetDataSource().Count <= 0)
            {
                MetroMessageBox.Show(this, alerts.before_insert_period, alerts.editing, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                var obj = speedBindingSource.Current as Speed;
                using (var speedForm = new SpeedForm())
                {
                    speedForm.SetExistingSource(mgSpeed.DataSource as BindingSource);
                    speedForm.SetSpeedForm(obj);
                    speedForm.ShowDialog();
                    if (speedForm.result == DialogResult.Cancel)
                        return;
                    var speed = speedForm.speed;
                    int s_km = obj.Start_Km, s_m = obj.Start_M, f_km = obj.Final_Km, f_m = obj.Final_M, pass = obj.Passenger, fre = obj.Freight, em_fre = obj.Empty_Freight, saps = obj.Sapsan, lasto = obj.Lastochka;
                    obj.Start_Km = speed.Start_Km;
                    obj.Start_M = speed.Start_M;
                    obj.Final_Km = speed.Final_Km;
                    obj.Final_M = speed.Final_M;
                    obj.Passenger = speed.Passenger;
                    obj.Freight = speed.Freight;
                    obj.Empty_Freight = speed.Empty_Freight;
                    obj.Sapsan = speed.Sapsan;
                    obj.Lastochka = speed.Lastochka;

                    if (MainTrackStructureService.UpdateMtoObject(obj, MainTrackStructureConst.MtoSpeed))
                    {
                        speedBindingSource.EndEdit();
                        speedBindingSource.ResetCurrentItem();
                    }
                    else
                    {
                        obj.Start_Km = s_km;
                        obj.Start_M = s_m;
                        obj.Final_Km = f_km;
                        obj.Final_M = f_m;
                        obj.Passenger = pass;
                        obj.Freight = fre;
                        obj.Empty_Freight = em_fre;
                        obj.Sapsan = saps;
                        obj.Lastochka = lasto;
                        speedBindingSource.EndEdit();
                        speedBindingSource.ResetCurrentItem();
                        MetroMessageBox.Show(this, alerts.edit_error + " " + alerts.check_fields_filling, alerts.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch
            {
                MetroMessageBox.Show(this, alerts.edit_error, alerts.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void pcSpeedPeriod_PeriodSelectionChanged(object sender, EventArgs e)
        {
            PeriodSelectionChanged(speedBindingSource, MainTrackStructureConst.MtoSpeed, pcSpeedPeriod);
        }

        private void pcTempSpeedPeriod_SelectionChanged(object sender, EventArgs e)
        {
            PeriodSelectionChanged(tempSpeedBindingSource, MainTrackStructureConst.MtoTempSpeed, pcTempSpeedPeriod);
        }

        private void tsbRemoveSpeed_Click(object sender, EventArgs e)
        {
            RemoveMainTrackObject(speedBindingSource, MainTrackStructureConst.MtoSpeed);
        }

        private void tsbAddTempSpeed_Click(object sender, EventArgs e)
        {
            if (pcTempSpeedPeriod.GetDataSource().Count <= 0)
            {
                MessageHelper.ShowWarning(this, alerts.inserting);
                return;
            }
            using (var speedForm = new SpeedForm())
            {
                speedForm.SetTempStatus();
                speedForm.SetExistingSource(mgTempSpeed.DataSource as BindingSource);
                speedForm.ShowDialog();
                if (speedForm.result == DialogResult.Cancel)
                    return;
                var speed = speedForm.speed;
                speed.Period_Id = pcTempSpeedPeriod.CurrentId;
                MainTrackStructureService.InsertObject(speed, MainTrackStructureConst.MtoTempSpeed);
                if (speed.Id > -1)
                {
                    tempSpeedBindingSource.Add(speed);
                    tempSpeedBindingSource.MoveLast();
                }
                else
                {
                    MessageHelper.ShowError(this, alerts.insert_error + " " + alerts.check_fields_filling);
                }
            }
        }

        private void tsbEditTempSpeed_Click(object sender, EventArgs e)
        {
            if (pcTempSpeedPeriod.GetDataSource().Count <= 0)
            {
                MetroMessageBox.Show(this, alerts.before_insert_period, alerts.editing, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                var obj = tempSpeedBindingSource.Current as TempSpeed;
                using (var speedForm = new SpeedForm())
                {
                    speedForm.SetTempStatus();
                    speedForm.SetExistingSource(mgTempSpeed.DataSource as BindingSource);
                    speedForm.SetSpeedForm(obj);
                    speedForm.ShowDialog();
                    if (speedForm.result == DialogResult.Cancel)
                        return;
                    var speed = speedForm.speed;
                    int s_km = obj.Start_Km, s_m = obj.Start_M, f_km = obj.Final_Km, f_m = obj.Final_M, pass = obj.Passenger, fre = obj.Freight, em_fre = obj.Empty_Freight, reasonId = obj.Reason_Id;
                    string reason = obj.Reason;
                    obj.Start_Km = speed.Start_Km;
                    obj.Start_M = speed.Start_M;
                    obj.Final_Km = speed.Final_Km;
                    obj.Final_M = speed.Final_M;
                    obj.Passenger = speed.Passenger;
                    obj.Freight = speed.Freight;
                    obj.Empty_Freight = speed.Empty_Freight;
                    obj.Reason_Id = ((TempSpeed)speed).Reason_Id;
                    obj.Reason = ((TempSpeed)speed).Reason;

                    if (MainTrackStructureService.UpdateMtoObject(obj, MainTrackStructureConst.MtoTempSpeed))
                    {
                        tempSpeedBindingSource.EndEdit();
                        tempSpeedBindingSource.ResetCurrentItem();
                    }
                    else
                    {
                        obj.Start_Km = s_km;
                        obj.Start_M = s_m;
                        obj.Final_Km = f_km;
                        obj.Final_M = f_m;
                        obj.Passenger = pass;
                        obj.Freight = fre;
                        obj.Empty_Freight = em_fre;
                        obj.Reason_Id = reasonId;
                        obj.Reason = reason;
                        tempSpeedBindingSource.EndEdit();
                        tempSpeedBindingSource.ResetCurrentItem();
                        MetroMessageBox.Show(this, alerts.edit_error + " " + alerts.check_fields_filling, alerts.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch
            {
                MetroMessageBox.Show(this, alerts.edit_error, alerts.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void tsbRemoveTempSpeed_Click(object sender, EventArgs e)
        {
            RemoveMainTrackObject(tempSpeedBindingSource, MainTrackStructureConst.MtoTempSpeed);
        }

        private void pcElevationPeriod_PeriodSelectionChanged(object sender, EventArgs e)
        {
            PeriodSelectionChanged(elevationBindingSource, MainTrackStructureConst.MtoElevation, pcElevationPeriod);
        }

        private void PeriodSelectionChanged(BindingSource bindingSource, int mtoObject, PeriodControl period)
        {
            bindingSource.Clear();
            if (period.GetDataSource().Count > 0)
            {
                bindingSource.DataSource = MainTrackStructureService.GetMtoObjects(period.CurrentId, mtoObject);
            }
        }

        private void tsbRemoveElevation_Click(object sender, EventArgs e)
        {
            RemoveMainTrackObject(elevationBindingSource, MainTrackStructureConst.MtoElevation);
        }

        private void tsbAddElevation_Click(object sender, EventArgs e)
        {
            if (pcElevationPeriod.GetDataSource().Count <= 0)
            {
                MessageHelper.ShowWarning(this, alerts.inserting);
                return;
            }
            using (var elevationForm = new ElevationForm())
            {
                elevationForm.SetExistingSource(mgElevation.DataSource as BindingSource);
                elevationForm.ShowDialog();
                if (elevationForm.Result == DialogResult.Cancel)
                    return;
                var elevation = elevationForm.Elevation;
                elevation.Period_Id = pcElevationPeriod.CurrentId;
                MainTrackStructureService.InsertObject(elevation, MainTrackStructureConst.MtoElevation);
                if (elevation.Id > -1)
                {
                    elevationBindingSource.Add(elevation);
                    elevationBindingSource.MoveLast();
                }
                else
                {
                    MessageHelper.ShowError(this, alerts.insert_error + " " + alerts.check_fields_filling);
                }
            }
        }

        private void tsbEditElevation_Click(object sender, EventArgs e)
        {
            if (pcElevationPeriod.GetDataSource().Count <= 0)
            {
                MetroMessageBox.Show(this, alerts.before_insert_period, alerts.editing, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                var obj = elevationBindingSource.Current as Elevation;
                using (var elevationForm = new ElevationForm())
                {
                    elevationForm.SetExistingSource(mgElevation.DataSource as BindingSource);
                    elevationForm.SetElevationForm(obj);
                    elevationForm.ShowDialog();
                    if (elevationForm.Result == DialogResult.Cancel)
                        return;
                    var elevation = elevationForm.Elevation;
                    int s_km = obj.Start_Km, s_m = obj.Start_M, f_km = obj.Final_Km, f_m = obj.Final_M, lvlId = obj.Level_Id;
                    string side = obj.Side;
                    obj.Start_Km = elevation.Start_Km;
                    obj.Start_M = elevation.Start_M;
                    obj.Final_Km = elevation.Final_Km;
                    obj.Final_M = elevation.Final_M;
                    obj.Level_Id = elevation.Level_Id;
                    obj.Side = elevation.Side;

                    if (MainTrackStructureService.UpdateMtoObject(obj, MainTrackStructureConst.MtoElevation))
                    {
                        elevationBindingSource.EndEdit();
                        elevationBindingSource.ResetCurrentItem();
                    }
                    else
                    {
                        obj.Start_Km = s_km;
                        obj.Start_M = s_m;
                        obj.Final_Km = f_km;
                        obj.Final_M = f_m;
                        obj.Level_Id = lvlId;
                        obj.Side = side;
                        elevationBindingSource.EndEdit();
                        elevationBindingSource.ResetCurrentItem();
                        MetroMessageBox.Show(this, alerts.edit_error + " " + alerts.check_fields_filling, alerts.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch
            {
                MetroMessageBox.Show(this, alerts.edit_error, alerts.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void tsbAddPdbSection_Click(object sender, EventArgs e)
        {
            if (pcPdbSectionPeriod.GetDataSource().Count <= 0)
            {
                MetroMessageBox.Show(this, alerts.before_insert_period, alerts.inserting, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            using (var pdbSectionForm = new PdbForm())
            {
                pdbSectionForm.LoadRoads(ucRoads.GetDataSource());
                pdbSectionForm.SetExistingSectionsSource(mgPdbSections.DataSource as BindingSource);
                pdbSectionForm.ShowDialog();
                if (pdbSectionForm.Result == DialogResult.Cancel)
                    return;

                (pdbSectionForm.Section as PdbSection).Period_Id = pcPdbSectionPeriod.CurrentId;
                MainTrackStructureService.InsertObject(pdbSectionForm.Section as PdbSection, MainTrackStructureConst.MtoPdbSection);
                if ((pdbSectionForm.Section as PdbSection).Id > -1)
                {
                    pdbSectionBindingSource.Add(pdbSectionForm.Section as PdbSection);
                    pdbSectionBindingSource.MoveLast();
                }
                else
                {
                    MessageHelper.ShowError(this, alerts.insert_error + " " + alerts.check_fields_filling);
                }
            }
        }

        private void tsbEditPdbSection_Click(object sender, EventArgs e)
        {
            if (pcPdbSectionPeriod.GetDataSource().Count <= 0)
            {
                MetroMessageBox.Show(this, alerts.before_insert_period, alerts.editing, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                var obj = pdbSectionBindingSource.Current as PdbSection;
                using (var pdbSectionForm = new PdbForm())
                {
                    pdbSectionForm.LoadRoads(ucRoads.GetDataSource());
                    pdbSectionForm.SetExistingSectionsSource(mgPdbSections.DataSource as BindingSource);
                    pdbSectionForm.SetPdbForm(obj);
                    pdbSectionForm.ShowDialog();
                    if (pdbSectionForm.Result == DialogResult.Cancel)
                        return;
                    var section = pdbSectionForm.Section as PdbSection;
                    Int64 distanceId = obj.DistanceId, pdbId = obj.PdbId;
                    int s_km = obj.Start_Km, s_m = obj.Start_M, f_km = obj.Final_Km, f_m = obj.Final_M;
                    string distance = obj.Distance, road = obj.Road, pdb = obj.Pdb;
                    obj.Start_Km = section.Start_Km;
                    obj.Start_M = section.Start_M;
                    obj.Final_Km = section.Final_Km;
                    obj.Final_M = section.Final_M;
                    obj.DistanceId = section.DistanceId;
                    obj.Distance = section.Distance;
                    obj.Road = section.Road;
                    obj.PdbId = section.PdbId;
                    obj.Pdb = section.Pdb;

                    if (MainTrackStructureService.UpdateMtoObject(obj, MainTrackStructureConst.MtoPdbSection))
                    {
                        pdbSectionBindingSource.EndEdit();
                        pdbSectionBindingSource.ResetCurrentItem();
                    }
                    else
                    {
                        obj.Start_Km = s_km;
                        obj.Start_M = s_m;
                        obj.Final_Km = f_km;
                        obj.Final_M = f_m;
                        obj.DistanceId = distanceId;
                        obj.Distance = distance;
                        obj.Road = road;
                        obj.PdbId = pdbId;
                        obj.Pdb = pdb;
                        pdbSectionBindingSource.EndEdit();
                        pdbSectionBindingSource.ResetCurrentItem();
                        MetroMessageBox.Show(this, alerts.edit_error + " " + alerts.check_fields_filling, alerts.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch
            {
                MetroMessageBox.Show(this, alerts.edit_error, alerts.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void pcPdbSectionPeriod_PeriodSelectionChanged(object sender, EventArgs e)
        {
            PeriodSelectionChanged(pdbSectionBindingSource, MainTrackStructureConst.MtoPdbSection, pcPdbSectionPeriod);

            string lastPDB = String.Empty;
            List<Color> colors = new List<Color> { Color.Aquamarine, Color.LightGoldenrodYellow, Color.Gold, Color.White, Color.LightGreen};
            int i = 0;
            foreach (DataGridViewRow row in mgPdbSections.Rows)
            {
                if (row.Cells["Pdb"].Value.ToString().Equals(lastPDB))
                {
                    row.DefaultCellStyle.BackColor = colors[i];
                }
                else
                {
                    lastPDB = row.Cells["Pdb"].Value.ToString();
                    i = (i + 1) % 5;
                    row.DefaultCellStyle.BackColor = colors[i];
                }
            }
        }

        private void tsbRemovePdbSection(object sender, EventArgs e)
        {
            RemoveMainTrackObject(pdbSectionBindingSource, MainTrackStructureConst.MtoPdbSection);
        }

        private void msbShowMainTrackObjects_Click(object sender, EventArgs e)
        {
            msbShowStationTrackObjects.Checked = false;
            msbShowMainTrackObjects.Checked = true;
            scStation.Hide();
            scPark.Hide();
            scStationObject.Hide();
            scStationTracks.Hide();
            ucStationsDirections.Hide();
            metroPanel34.Hide();
            metroPanel23.Show();
            ucDistances.Show();
            ucPchu.Show();
            ucPD.Show();
            ucPDB.Show();
            ucDirections.Show();
            ucTracks.Show();
            CleanTables(AdmStructureConst.AdmTrack);
        }

        private void msbShowStationTrackObjects_Click(object sender, EventArgs e)
        {
            msbShowStationTrackObjects.Checked = true;
            msbShowMainTrackObjects.Checked = false;
            scStationObject.Show();
            scPark.Show();
            scStation.Show();
            scStationTracks.Show();
            ucStationsDirections.Show();
            metroPanel34.Show();
            metroPanel23.Hide();
            ucDistances.Hide();
            ucPchu.Hide();
            ucPD.Hide();
            ucPDB.Hide();
            ucDirections.Hide();
            ucTracks.Hide();
            CleanTables(AdmStructureConst.AdmTrack);
        }

        private void pcStationSectionPeriod_PeriodSelectionChanged(object sender, EventArgs e)
        {
            PeriodSelectionChanged(stationSectionBindingSource, MainTrackStructureConst.MtoStationSection, pcStationSectionPeriod);
        }

        private void tsbAddStationSection_Click(object sender, EventArgs e)
        {
            if (pcStationSectionPeriod.GetDataSource().Count <= 0)
            {
                MetroMessageBox.Show(this, alerts.before_insert_period, alerts.inserting, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            using (var stationSectionForm = new StationSectionForm())
            {
                stationSectionForm.LoadRoads(ucRoads.GetDataSource());
                stationSectionForm.SetExistingSectionsSource(mgStationSections.DataSource as BindingSource);
                stationSectionForm.ShowDialog();
                if (stationSectionForm.result == DialogResult.Cancel)
                    return;
                var section = stationSectionForm.section as StationSection;

                section.Period_Id = pcStationSectionPeriod.CurrentId;
                MainTrackStructureService.InsertObject(section, MainTrackStructureConst.MtoStationSection);
                if (section.Id > -1)
                {
                    stationSectionBindingSource.Add(section);
                    stationSectionBindingSource.MoveLast();
                }
                else
                {
                    MessageHelper.ShowError(this, alerts.insert_error + " " + alerts.check_fields_filling);
                }
            }
        }

        private void tsbEditStatonSection_CLick(object sender, EventArgs e)
        {
            if (pcStationSectionPeriod.GetDataSource().Count <= 0)
            {
                MetroMessageBox.Show(this, alerts.before_insert_period, alerts.editing, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                var obj = stationSectionBindingSource.Current as StationSection;
                using (var stationSectionForm = new StationSectionForm())
                {
                    stationSectionForm.LoadRoads(ucRoads.GetDataSource());
                    stationSectionForm.SetExistingSectionsSource(mgStationSections.DataSource as BindingSource);
                    stationSectionForm.SetStationSectionForm(obj);
                    stationSectionForm.ShowDialog();
                    if (stationSectionForm.result == DialogResult.Cancel)
                        return;
                    var section = stationSectionForm.section as StationSection;
                    Int64 nodId = obj.Nod_Id, stId = obj.Station_Id;
                    int s_km = obj.Start_Km, s_m = obj.Start_M, f_km = obj.Final_Km, f_m = obj.Final_M, akm = obj.Axis_Km, am = obj.Axis_M, poId = obj.Point_Id;
                    string nod = obj.Nod, station = obj.Station, road = obj.Road, point = obj.Point;
                    obj.Start_Km = section.Start_Km;
                    obj.Start_M = section.Start_M;
                    obj.Final_Km = section.Final_Km;
                    obj.Final_M = section.Final_M;
                    obj.Axis_Km = section.Axis_Km;
                    obj.Axis_M = section.Axis_M;
                    obj.Nod = section.Nod;
                    obj.Nod_Id = section.Nod_Id;
                    obj.Station = section.Station;
                    obj.Station_Id = section.Station_Id;
                    obj.Road = section.Road;
                    obj.Point = section.Point;
                    obj.Point_Id = section.Point_Id;

                    if (MainTrackStructureService.UpdateMtoObject(obj, MainTrackStructureConst.MtoStationSection))
                    {
                        stationSectionBindingSource.EndEdit();
                        stationSectionBindingSource.ResetCurrentItem();
                    }
                    else
                    {
                        obj.Start_Km = s_km;
                        obj.Start_M = s_m;
                        obj.Final_Km = f_km;
                        obj.Final_M = f_m;
                        obj.Axis_Km = akm;
                        obj.Axis_M = am;
                        obj.Nod = nod;
                        obj.Nod_Id = nodId;
                        obj.Station = station;
                        obj.Station_Id = stId;
                        obj.Road = road;
                        obj.Point = point;
                        obj.Point_Id = poId;
                        stationSectionBindingSource.EndEdit();
                        stationSectionBindingSource.ResetCurrentItem();
                        MetroMessageBox.Show(this, alerts.edit_error + " " + alerts.check_fields_filling, alerts.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch
            {
                MetroMessageBox.Show(this, alerts.edit_error, alerts.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void tsbRemoveStationSection_Click(object sender, EventArgs e)
        {
            RemoveMainTrackObject(stationSectionBindingSource, MainTrackStructureConst.MtoStationSection);
        }

        private void pcCurveControl_PeriodSelectionChanged(object sender, EventArgs e)
        {
            PeriodSelectionChanged(curveBindingSource, MainTrackStructureConst.MtoCurve, pcCurvePeriod);
        }

        private void mgCurve_SelectionChanged(object sender, EventArgs e)
        {
            elCurveBindingSource.Clear();
            stCurveBindingSource.Clear();
            if (curveBindingSource.Count > 0)
            {
                elCurveBindingSource.DataSource = MainTrackStructureService.GetMtoObjects((curveBindingSource.Current as Curve).Id, MainTrackStructureConst.MtoElCurve);
                stCurveBindingSource.DataSource = MainTrackStructureService.GetMtoObjects((curveBindingSource.Current as Curve).Id, MainTrackStructureConst.MtoStCurve);
            }
        }

        private void tsbAddCurve_Click(object sender, EventArgs e)
        {
            if (pcCurvePeriod.GetDataSource().Count <= 0)
            {
                MetroMessageBox.Show(this, alerts.before_insert_period, alerts.inserting, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            using (var curveForm = new CurveForm())
            {
                curveForm.SetExistingSectionsSource(curveBindingSource.DataSource as BindingSource);
                curveForm.ShowDialog();
                if (curveForm.Result == DialogResult.Cancel)
                    return;
                var curve = curveForm.Curve as Curve;

                curve.Period_Id = pcCurvePeriod.CurrentId;
                MainTrackStructureService.InsertObject(curve, MainTrackStructureConst.MtoCurve);
                if (curve.Id > -1)
                {
                    curveBindingSource.Add(curve);
                    curveBindingSource.MoveLast();
                }
                else
                {
                    MessageHelper.ShowError(this, alerts.insert_error + " " + alerts.check_fields_filling);
                }
            }
        }

        private void tsbEditCurve_Click(object sender, EventArgs e)
        {
            if (pcCurvePeriod.GetDataSource().Count <= 0)
            {
                MetroMessageBox.Show(this, alerts.before_insert_period, alerts.editing, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                var obj = curveBindingSource.Current as Curve;
                using (var curveForm = new CurveForm())
                {
                    curveForm.SetExistingSectionsSource(curveBindingSource.DataSource as BindingSource);
                    curveForm.SetCurveForm(obj);
                    curveForm.ShowDialog();
                    if (curveForm.Result == DialogResult.Cancel)
                        return;
                    var curve = curveForm.Curve as Curve;
                    int s_km = obj.Start_Km, s_m = obj.Start_M, f_km = obj.Final_Km, f_m = obj.Final_M, sideId = obj.Side_id;
                    string side = obj.Side;
                    obj.Start_Km = curve.Start_Km;
                    obj.Start_M = curve.Start_M;
                    obj.Final_Km = curve.Final_Km;
                    obj.Final_M = curve.Final_M;
                    obj.Side = curve.Side;
                    obj.Side_id = curve.Side_id;

                    if (MainTrackStructureService.UpdateMtoObject(obj, MainTrackStructureConst.MtoCurve))
                    {
                        curveBindingSource.EndEdit();
                        curveBindingSource.ResetCurrentItem();
                    }
                    else
                    {
                        obj.Start_Km = s_km;
                        obj.Start_M = s_m;
                        obj.Final_Km = f_km;
                        obj.Final_M = f_m;
                        obj.Side = side;
                        obj.Side_id = sideId;
                        curveBindingSource.EndEdit();
                        curveBindingSource.ResetCurrentItem();
                        MetroMessageBox.Show(this, alerts.edit_error + " " + alerts.check_fields_filling, alerts.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch
            {
                MetroMessageBox.Show(this, alerts.edit_error, alerts.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void tsbRevoveCurve_Click(object sender, EventArgs e)
        {
            RemoveMainTrackObject(curveBindingSource, MainTrackStructureConst.MtoCurve);
        }

        private void tsbAddElCurve_Click(object sender, EventArgs e)
        {
            if (curveBindingSource.Count <= 0)
            {
                MetroMessageBox.Show(this, alerts.before_insert, alerts.inserting, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            using (var elCurveForm = new ElCurveForm())
            {
                elCurveForm.SetExistingSectionsSource(elCurveBindingSource.DataSource as BindingSource);
                elCurveForm.SetElCurveForm(curveBindingSource.Current as Curve);
                elCurveForm.ShowDialog();
                if (elCurveForm.Result == DialogResult.Cancel)
                    return;
                var elCurve = elCurveForm.Curve as ElCurve;

                elCurve.Period_Id = (curveBindingSource.Current as Curve).Id;
                MainTrackStructureService.InsertObject(elCurve, MainTrackStructureConst.MtoElCurve);
                if (elCurve.Id > -1)
                {
                    elCurveBindingSource.Add(elCurve);
                    elCurveBindingSource.MoveLast();
                }
                else
                {
                    MessageHelper.ShowError(this, alerts.insert_error + " " + alerts.check_fields_filling);
                }
            }
        }

        private void tsbEditElCurve_Click(object sender, EventArgs e)
        {
            if (curveBindingSource.Count <= 0)
            {
                MetroMessageBox.Show(this, alerts.before_insert, alerts.editing, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                var obj = elCurveBindingSource.Current as ElCurve;
                using (var elCurveForm = new ElCurveForm())
                {
                    elCurveForm.SetExistingSectionsSource(elCurveBindingSource.DataSource as BindingSource);
                    elCurveForm.SetElCurveForm(obj);
                    elCurveForm.ShowDialog();
                    if (elCurveForm.Result == DialogResult.Cancel)
                        return;
                    var elCurve = elCurveForm.Curve as ElCurve;
                    int s_km = obj.Start_Km, s_m = obj.Start_M, f_km = obj.Final_Km, f_m = obj.Final_M, tran1 = obj.Transition_2, tran2 = obj.Transition_2;
                    float lvl = obj.Lvl;
                    obj.Start_Km = elCurve.Start_Km;
                    obj.Start_M = elCurve.Start_M;
                    obj.Final_Km = elCurve.Final_Km;
                    obj.Final_M = elCurve.Final_M;
                    obj.Transition_1 = elCurve.Transition_1;
                    obj.Transition_2 = elCurve.Transition_2;
                    obj.Lvl = elCurve.Lvl;

                    if (MainTrackStructureService.UpdateMtoObject(obj, MainTrackStructureConst.MtoElCurve))
                    {
                        elCurveBindingSource.EndEdit();
                        elCurveBindingSource.ResetCurrentItem();
                    }
                    else
                    {
                        obj.Start_Km = s_km;
                        obj.Start_M = s_m;
                        obj.Final_Km = f_km;
                        obj.Final_M = f_m;
                        obj.Transition_1 = tran1;
                        obj.Transition_2 = tran2;
                        obj.Lvl = lvl;
                        elCurveBindingSource.EndEdit();
                        elCurveBindingSource.ResetCurrentItem();
                        MetroMessageBox.Show(this, alerts.edit_error + " " + alerts.check_fields_filling, alerts.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch
            {
                MetroMessageBox.Show(this, alerts.edit_error, alerts.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void tsbRemoveElCurve_Click(object sender, EventArgs e)
        {
            RemoveMainTrackObject(elCurveBindingSource, MainTrackStructureConst.MtoElCurve);
        }

        private void tsbAddStCurve_Click(object sender, EventArgs e)
        {
            if (curveBindingSource.Count <= 0)
            {
                MetroMessageBox.Show(this, alerts.before_insert, alerts.inserting, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            using (var stCurveForm = new StCurveForm())
            {
                stCurveForm.SetStCurveForm(curveBindingSource.Current as Curve);
                stCurveForm.ShowDialog();
                if (stCurveForm.Result == DialogResult.Cancel)
                    return;
                var stCurve = stCurveForm.StCurve as StCurve;

                stCurve.Period_Id = (curveBindingSource.Current as Curve).Id;
                MainTrackStructureService.InsertObject(stCurve, MainTrackStructureConst.MtoStCurve);
                if (stCurve.Id > 0)
                {
                    stCurveBindingSource.Add(stCurve);
                    stCurveBindingSource.MoveLast();
                }
                else
                {
                    MessageHelper.ShowError(this, alerts.insert_error + " " + alerts.check_fields_filling);
                }
            }
        }

        private void tsbEditStCurve_Click(object sender, EventArgs e)
        {
            if (curveBindingSource.Count <= 0)
            {
                MetroMessageBox.Show(this, alerts.before_insert, alerts.editing, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                var obj = stCurveBindingSource.Current as StCurve;
                using (var stCurveForm = new StCurveForm())
                {
                    stCurveForm.SetExistingSectionsSource(stCurveBindingSource.DataSource as BindingSource);
                    stCurveForm.SetStCurveForm(obj);
                    stCurveForm.ShowDialog();
                    if (stCurveForm.Result == DialogResult.Cancel)
                        return;
                    var stCurve = stCurveForm.StCurve as StCurve;
                    int s_km = obj.Start_Km, s_m = obj.Start_M, f_km = obj.Final_Km, f_m = obj.Final_M, tran1 = obj.Transition_1, tran2 = obj.Transition_2, width = obj.Width;
                    double radius = obj.Radius;
                    double wear = obj.Wear;
                    obj.Start_Km = stCurve.Start_Km;
                    obj.Start_M = stCurve.Start_M;
                    obj.Final_Km = stCurve.Final_Km;
                    obj.Final_M = stCurve.Final_M;
                    obj.Transition_1 = stCurve.Transition_1;
                    obj.Transition_2 = stCurve.Transition_2;
                    obj.Radius = stCurve.Radius;
                    obj.Wear = stCurve.Wear;
                    obj.Width = stCurve.Width;

                    if (MainTrackStructureService.UpdateMtoObject(obj, MainTrackStructureConst.MtoStCurve))
                    {
                        stCurveBindingSource.EndEdit();
                        stCurveBindingSource.ResetCurrentItem();
                    }
                    else
                    {
                        obj.Start_Km = s_km;
                        obj.Start_M = s_m;
                        obj.Final_Km = f_km;
                        obj.Final_M = f_m;
                        obj.Transition_1 = tran1;
                        obj.Transition_2 = tran2;
                        obj.Radius = radius;
                        obj.Wear = wear;
                        obj.Width = width;
                        stCurveBindingSource.EndEdit();
                        stCurveBindingSource.ResetCurrentItem();
                        MetroMessageBox.Show(this, alerts.edit_error + " " + alerts.check_fields_filling, alerts.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch
            {
                MetroMessageBox.Show(this, alerts.edit_error, alerts.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void tsbRemoveStCurve_Click(object sender, EventArgs e)
        {
            RemoveMainTrackObject(stCurveBindingSource, MainTrackStructureConst.MtoStCurve);
        }

        private void tsbAddStraighteningThread_Click(object sender, EventArgs e)
        {
            if (pcStraighteningThreadPeriod.GetDataSource().Count <= 0)
            {
                MessageHelper.ShowWarning(this, alerts.inserting);
                return;
            }
            using (var straighteningThreadForm = new StraighteningThreadForm())
            {
                straighteningThreadForm.SetExistingSource(mgStraighteningThread.DataSource as BindingSource);
                straighteningThreadForm.ShowDialog();
                if (straighteningThreadForm.result == DialogResult.Cancel)
                    return;
                var straighteningThread = straighteningThreadForm.straighteningThread;
                straighteningThread.Period_Id = pcStraighteningThreadPeriod.CurrentId;
                MainTrackStructureService.InsertObject(straighteningThread, MainTrackStructureConst.MtoStraighteningThread);
                if (straighteningThread.Id > -1)
                {
                    straighteningThreadBindingSource.Add(straighteningThread);
                    straighteningThreadBindingSource.MoveLast();
                }
                else
                {
                    MessageHelper.ShowError(this, alerts.insert_error + " " + alerts.check_fields_filling);
                }
            }
        }

        private void tsbEditStraighteningThread_Click(object sender, EventArgs e)
        {
            if (pcStraighteningThreadPeriod.GetDataSource().Count <= 0)
            {
                MetroMessageBox.Show(this, alerts.before_insert_period, alerts.editing, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                var obj = straighteningThreadBindingSource.Current as StraighteningThread;
                using (var straighteningThreadForm = new StraighteningThreadForm())
                {
                    straighteningThreadForm.SetExistingSource(mgStraighteningThread.DataSource as BindingSource);
                    straighteningThreadForm.SetStraighteningThreadForm(obj);
                    straighteningThreadForm.ShowDialog();
                    if (straighteningThreadForm.result == DialogResult.Cancel)
                        return;
                    var straighteningThread = straighteningThreadForm.straighteningThread as StraighteningThread;
                    int s_km = obj.Start_Km, s_m = obj.Start_M, f_km = obj.Final_Km, f_m = obj.Final_M, sideId = obj.Side_Id;
                    string side = obj.Side;
                    obj.Start_Km = straighteningThread.Start_Km;
                    obj.Start_M = straighteningThread.Start_M;
                    obj.Final_Km = straighteningThread.Final_Km;
                    obj.Final_M = straighteningThread.Final_M;
                    obj.Side_Id = straighteningThread.Side_Id;
                    obj.Side = straighteningThread.Side;

                    if (MainTrackStructureService.UpdateMtoObject(obj, MainTrackStructureConst.MtoStraighteningThread))
                    {
                        straighteningThreadBindingSource.EndEdit();
                        straighteningThreadBindingSource.ResetCurrentItem();
                    }
                    else
                    {
                        obj.Start_Km = s_km;
                        obj.Start_M = s_m;
                        obj.Final_Km = f_km;
                        obj.Final_M = f_m;
                        obj.Side_Id = sideId;
                        obj.Side = side;
                        straighteningThreadBindingSource.EndEdit();
                        straighteningThreadBindingSource.ResetCurrentItem();
                        MetroMessageBox.Show(this, alerts.edit_error + " " + alerts.check_fields_filling, alerts.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch
            {
                MetroMessageBox.Show(this, alerts.edit_error, alerts.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void tsbRemoveStraighteningThread_Click(object sender, EventArgs e)
        {
            RemoveMainTrackObject(straighteningThreadBindingSource, MainTrackStructureConst.MtoStraighteningThread);
        }

        private void pcStraighteningThreadPeriod_PeriodSelectionChanged(object sender, EventArgs e)
        {
            PeriodSelectionChanged(straighteningThreadBindingSource, MainTrackStructureConst.MtoStraighteningThread, pcStraighteningThreadPeriod);
        }

        private void pcArtificialConstructionPeriod_PeriodSelectionChanged(object sender, EventArgs e)
        {
            PeriodSelectionChanged(artificialConstructionBindingSource, MainTrackStructureConst.MtoArtificialConstruction, pcArtificialConstructionPeriod);
        }

        private void tsbAddArtificialConstruction_Click(object sender, EventArgs e)
        {
            if (pcArtificialConstructionPeriod.GetDataSource().Count <= 0)
            {
                MessageHelper.ShowWarning(this, alerts.inserting);
                return;
            }
            using (var artificialConstructionForm = new ArtificialConstructionForm())
            {
                artificialConstructionForm.ShowDialog();
                if (artificialConstructionForm.Result == DialogResult.Cancel)
                    return;
                var artificialConstruction = artificialConstructionForm.ArtificialConstruction;
                artificialConstruction.Period_Id = pcArtificialConstructionPeriod.CurrentId;
                MainTrackStructureService.InsertObject(artificialConstruction, MainTrackStructureConst.MtoArtificialConstruction);
                if (artificialConstruction.Id > -1)
                {
                    artificialConstructionBindingSource.Add(artificialConstruction);
                    artificialConstructionBindingSource.MoveLast();
                }
                else
                {
                    MessageHelper.ShowError(this, alerts.insert_error + " " + alerts.check_fields_filling);
                }
            }
        }

        private void tsbEditArtificialConstruction_Click(object sender, EventArgs e)
        {
            if (pcArtificialConstructionPeriod.GetDataSource().Count <= 0)
            {
                MetroMessageBox.Show(this, alerts.before_insert_period, alerts.editing, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                var obj = artificialConstructionBindingSource.Current as ArtificialConstruction;
                using (var artificialConstructionForm = new ArtificialConstructionForm())
                {
                    artificialConstructionForm.SetArtificialConstructionForm(obj);
                    artificialConstructionForm.ShowDialog();
                    if (artificialConstructionForm.Result == DialogResult.Cancel)
                        return;
                    var artificialConstruction = artificialConstructionForm.ArtificialConstruction as ArtificialConstruction;
                    int km = obj.Km, metr = obj.Meter, len = obj.Len, typeId = obj.Type_Id;
                    string type = obj.Type;
                    obj.Km = artificialConstruction.Km;
                    obj.Meter = artificialConstruction.Meter;
                    obj.Len = artificialConstruction.Len;
                    obj.Type_Id = artificialConstruction.Type_Id;
                    obj.Type = artificialConstruction.Type;

                    if (MainTrackStructureService.UpdateMtoObject(obj, MainTrackStructureConst.MtoArtificialConstruction))
                    {
                        artificialConstructionBindingSource.EndEdit();
                        artificialConstructionBindingSource.ResetCurrentItem();
                    }
                    else
                    {
                        obj.Km = km;
                        obj.Meter = metr;
                        obj.Len = len;
                        obj.Type_Id = typeId;
                        obj.Type = type;
                        artificialConstructionBindingSource.EndEdit();
                        artificialConstructionBindingSource.ResetCurrentItem();
                        MetroMessageBox.Show(this, alerts.edit_error + " " + alerts.check_fields_filling, alerts.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch
            {
                MetroMessageBox.Show(this, alerts.edit_error, alerts.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void tsbRemoveArtificialConstruction_Click(object sender, EventArgs e)
        {
            RemoveMainTrackObject(artificialConstructionBindingSource, MainTrackStructureConst.MtoArtificialConstruction);
        }

        private void pcSwitchPeriod_PeriodSelectionChanged(object sender, EventArgs e)
        {
            PeriodSelectionChanged(switchBindingSource, MainTrackStructureConst.MtoSwitch, pcSwitchPeriod);
        }

        private void tsbAddSwitch_Click(object sender, EventArgs e)
        {
            if (pcSwitchPeriod.GetDataSource().Count <= 0)
            {
                MessageHelper.ShowWarning(this, alerts.inserting);
                return;
            }
            using (var switchForm = new SwitchForm(ucRoads.CurrentUnitId))
            {

                switchForm.SetExistingSource(mgSwitch.DataSource as BindingSource);
                switchForm.ShowDialog();
                if (switchForm.result == DialogResult.Cancel)
                    return;
                var @switch = switchForm.@switch;
                @switch.Period_Id = pcSwitchPeriod.CurrentId;
                MainTrackStructureService.InsertObject(@switch, MainTrackStructureConst.MtoSwitch);
                if (@switch.Id > -1)
                {
                    switchBindingSource.Add(@switch);
                    switchBindingSource.MoveLast();
                }
                else
                {
                    MessageHelper.ShowError(this, alerts.insert_error + " " + alerts.check_fields_filling);
                }
            }
        }

        private void tsbEditSwitch_Click(object sender,EventArgs e)
        {
            if (pcSwitchPeriod.GetDataSource().Count <= 0)
            {
                MetroMessageBox.Show(this, alerts.before_insert_period, alerts.editing, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                var obj = switchBindingSource.Current as Switch;
                using (var switchForm = new SwitchForm(ucRoads.CurrentUnitId))
                {
                    
                    switchForm.SetExistingSource(mgSwitch.DataSource as BindingSource);
                    switchForm.SetSwitchForm(obj, ucRoads.CurrentUnitId);
                    switchForm.ShowDialog();
                    if (switchForm.result == DialogResult.Cancel)
                        return;
                    var @switch = switchForm.@switch as Switch;
                    Int64 stationId = obj.Station_Id;
                    int km = obj.Km, m = obj.Meter, fkm = obj.Final_Km, fm = obj.Final_M, sideId = (int)obj.Side_Id, pointId = obj.Point_Id, dirId = (int)obj.Dir_Id, markId = obj.Mark_Id; 
                    string side = obj.Side, point = obj.Point, dir = obj.Dir, num = obj.Num, mark = obj.Mark, stat = obj.Station;
                    obj.Km = @switch.Km;
                    obj.Meter = @switch.Meter;
                    obj.Final_Km = @switch.Final_Km;
                    obj.Final_M = @switch.Final_M;
                    obj.Side_Id = @switch.Side_Id;
                    obj.Side = @switch.Side;
                    obj.Point_Id = @switch.Point_Id;
                    obj.Point = @switch.Point;
                    obj.Dir_Id = @switch.Dir_Id;
                    obj.Dir = @switch.Dir;
                    obj.Mark_Id = @switch.Mark_Id;
                    obj.Mark = @switch.Mark;
                    obj.Num = @switch.Num;
                    obj.Station = @switch.Station;
                    obj.Station_Id = @switch.Station_Id;

                    if (MainTrackStructureService.UpdateMtoObject(obj, MainTrackStructureConst.MtoSwitch))
                    {
                        switchBindingSource.EndEdit();
                        switchBindingSource.ResetCurrentItem();
                    }
                    else
                    {
                        obj.Km = km;
                        obj.Meter = m;
                        obj.Final_Km = fkm;
                        obj.Final_M = fm;
                        obj.Side_Id = (Side)sideId;
                        obj.Side = side;
                        obj.Mark_Id = markId;
                        obj.Mark = mark;
                        obj.Dir_Id = (SwitchDirection)dirId;
                        obj.Dir = dir;
                        obj.Point_Id = pointId;
                        obj.Point = point;
                        obj.Num = num;
                        obj.Station_Id = stationId;
                        obj.Station = stat;
                        switchBindingSource.EndEdit();
                        switchBindingSource.ResetCurrentItem();
                        MetroMessageBox.Show(this, alerts.edit_error + " " + alerts.check_fields_filling, alerts.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch
            {
                MetroMessageBox.Show(this, alerts.edit_error, alerts.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void tsbRemoveSwitch_Click(object sender, EventArgs e)
        {
            RemoveMainTrackObject(switchBindingSource, MainTrackStructureConst.MtoSwitch);
        }

        private void tsbNonStandardKmRemove_Click(object sender, EventArgs e)
        {
            RemoveMainTrackObject(nonstandardKmBindingSource, MainTrackStructureConst.MtoNonStandard);
        }

        private void tsbRemoveNonExistKm_Click(object sender, EventArgs e)
        {
            RemoveMainTrackObject(nonExistKmBindingSource, MainTrackStructureConst.MtoNonExtKm);
        }

        private void tsbAddProfileObject_Click(object sender, EventArgs e)
        {
            if (pcProfileObjectPeriod.GetDataSource().Count <= 0)
            {
                MessageHelper.ShowWarning(this, alerts.inserting);
                return;
            }
            using (var profileObjectForm = new ProfileObjectForm())
            {
                profileObjectForm.SetExistingSource(mgProfileObject.DataSource as BindingSource);
                profileObjectForm.ShowDialog();
                if (profileObjectForm.Result == DialogResult.Cancel)
                    return;
                var profileObject = profileObjectForm.profileObject;
                profileObject.Period_Id = pcProfileObjectPeriod.CurrentId;
                MainTrackStructureService.InsertObject(profileObject, MainTrackStructureConst.MtoProfileObject);
                if (profileObject.Id > -1)
                {
                    profileObjectBindingSource.Add(profileObject);
                    profileObjectBindingSource.MoveLast();
                }
                else
                {
                    MessageHelper.ShowError(this, alerts.insert_error + " " + alerts.check_fields_filling);
                }
            }
        }

        private void tsbEditProfileObject_Click(object sender, EventArgs e)
        {
            if (pcProfileObjectPeriod.GetDataSource().Count <= 0)
            {
                MetroMessageBox.Show(this, alerts.before_insert_period, alerts.editing, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                var obj = profileObjectBindingSource.Current as ProfileObject;
                using (var profileObjectForm = new ProfileObjectForm())
                {
                    profileObjectForm.SetExistingSource(mgProfileObject.DataSource as BindingSource);
                    profileObjectForm.SetProfileObject(obj);
                    profileObjectForm.ShowDialog();
                    if (profileObjectForm.Result == DialogResult.Cancel)
                        return;
                    var profileObject = profileObjectForm.profileObject;
                    int km = obj.Km, m = obj.Meter, sideId = obj.Side_id, objectId = obj.Object_id;
                    string side = obj.Side, objectType = obj.Object_type;
                    obj.Km = profileObject.Km;
                    obj.Meter = profileObject.Meter;
                    obj.Side = profileObject.Side;
                    obj.Side_id = profileObject.Side_id;
                    obj.Object_id = profileObject.Object_id;
                    obj.Object_type = profileObject.Object_type;

                    if (MainTrackStructureService.UpdateMtoObject(obj, MainTrackStructureConst.MtoProfileObject))
                    {
                        profileObjectBindingSource.EndEdit();
                        profileObjectBindingSource.ResetCurrentItem();
                    }
                    else
                    {
                        obj.Km = km;
                        obj.Meter = m;
                        obj.Side = side;
                        obj.Side_id = sideId;
                        obj.Object_id = objectId;
                        obj.Object_type = objectType;
                        profileObjectBindingSource.EndEdit();
                        profileObjectBindingSource.ResetCurrentItem();
                        MetroMessageBox.Show(this, alerts.edit_error + " " + alerts.check_fields_filling, alerts.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch
            {
                MetroMessageBox.Show(this, alerts.edit_error, alerts.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void tsbRemoveProfileObject_Click(object sender, EventArgs e)
        {
            RemoveMainTrackObject(profileObjectBindingSource, MainTrackStructureConst.MtoProfileObject);
        }

        private void pcProfileObjectPeriod_PeriodSelectionChanged(object sender, EventArgs e)
        {
            PeriodSelectionChanged(profileObjectBindingSource, MainTrackStructureConst.MtoProfileObject, pcProfileObjectPeriod);
        }

        private void tsbAddCommunication_Click(object sender, EventArgs e)
        {
            if (pcCommunicationPeriod.GetDataSource().Count <= 0)
            {
                MessageHelper.ShowWarning(this, alerts.before_insert_period);
                return;
            }
            using (var communicationForm = new CommunicationForm())
            {
                communicationForm.ShowDialog();
                if (communicationForm.result == DialogResult.Cancel)
                    return;
                var communicationObject = communicationForm.communication;
                communicationObject.Period_Id = pcCommunicationPeriod.CurrentId;
                MainTrackStructureService.InsertObject(communicationObject, MainTrackStructureConst.MtoCommunication);
                if (communicationObject.Id > -1)
                {
                    communicationBindingSource.Add(communicationObject);
                    communicationBindingSource.MoveLast();
                }
                else
                {
                    MessageHelper.ShowError(this, alerts.insert_error + " " + alerts.check_fields_filling);
                }
            }
        }

        private void tsbEditCommunication_Click(object sender, EventArgs e)
        {
            if (pcCommunicationPeriod.GetDataSource().Count <= 0)
            {
                MetroMessageBox.Show(this, alerts.before_insert_period, alerts.editing, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                var obj = communicationBindingSource.Current as Communication;
                using (var communicationForm = new CommunicationForm())
                {
                    communicationForm.SetForm(obj);
                    communicationForm.ShowDialog();
                    if (communicationForm.result == DialogResult.Cancel)
                        return;
                    var commun = communicationForm.communication;
                    int km = obj.Km, m = obj.Meter, objectId = obj.Object_id;
                    string object_ = obj.Object;
                    obj.Km = commun.Km;
                    obj.Meter = commun.Meter;
                    obj.Object_id = commun.Object_id;
                    obj.Object = commun.Object;

                    if (MainTrackStructureService.UpdateMtoObject(obj, MainTrackStructureConst.MtoCommunication))
                    {
                        communicationBindingSource.EndEdit();
                        communicationBindingSource.ResetCurrentItem();
                    }
                    else
                    {
                        obj.Km = km;
                        obj.Meter = m;
                        obj.Object_id = objectId;
                        obj.Object = object_;
                        communicationBindingSource.EndEdit();
                        communicationBindingSource.ResetCurrentItem();
                        MetroMessageBox.Show(this, alerts.edit_error + " " + alerts.check_fields_filling, alerts.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch
            {
                MetroMessageBox.Show(this, alerts.edit_error, alerts.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void tsbRemoveCommunication_Click(object sender, EventArgs e)
        {
            RemoveMainTrackObject(communicationBindingSource, MainTrackStructureConst.MtoCommunication);
        }

        private void pcCommunicationPeriod_PeriodSelectionChanged(object sender, EventArgs e)
        {
            PeriodSelectionChanged(communicationBindingSource, MainTrackStructureConst.MtoCommunication, pcCommunicationPeriod);
        }

        private void tsbAddCoordinateGNSS_Click(object sender, EventArgs e)
        {
            if (pcCoordinateGNSSPeriod.GetDataSource().Count <= 0)
            {
                MessageHelper.ShowWarning(this, alerts.before_insert_period);
                return;
            }
            using (var coordinateGNSSForm = new CoordinateGNSSForm())
            {
                coordinateGNSSForm.ShowDialog();
                if (coordinateGNSSForm.result == DialogResult.Cancel)
                    return;
                var coordObject = coordinateGNSSForm.coordinateGNSS;
                coordObject.Period_Id = pcCoordinateGNSSPeriod.CurrentId;
                MainTrackStructureService.InsertObject(coordObject, MainTrackStructureConst.MtoCoordinateGNSS);
                if (coordObject.Id > -1)
                {
                    coordinateGNSSBindingSource.Add(coordObject);
                    coordinateGNSSBindingSource.MoveLast();
                }
                else
                {
                    MessageHelper.ShowError(this, alerts.insert_error + " " + alerts.check_fields_filling);
                }
            }
        }

        private void tsbEditCoordinateGNSS_Click(object sender, EventArgs e)
        {
            if (pcCoordinateGNSSPeriod.GetDataSource().Count <= 0)
            {
                MetroMessageBox.Show(this, alerts.before_insert_period, alerts.editing, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                var obj = coordinateGNSSBindingSource.Current as CoordinateGNSS;
                using (var coordinateGNSSForm = new CoordinateGNSSForm())
                {
                    coordinateGNSSForm.SetForm(obj);
                    coordinateGNSSForm.ShowDialog();
                    if (coordinateGNSSForm.result == DialogResult.Cancel)
                        return;
                    var coord = coordinateGNSSForm.coordinateGNSS;
                    int km = obj.Km, m = obj.Meter;
                    double lati = obj.Latitude, longti = obj.Longtitude, alti = obj.Altitude, excoord = obj.Exact_coordinate, exhei = obj.Exact_height;
                    obj.Km = coord.Km;
                    obj.Meter = coord.Meter;
                    obj.Latitude = coord.Latitude;
                    obj.Longtitude = coord.Longtitude;
                    obj.Altitude = coord.Altitude;
                    obj.Exact_height = coord.Exact_height;
                    obj.Exact_coordinate = coord.Exact_coordinate;

                    if (MainTrackStructureService.UpdateMtoObject(obj, MainTrackStructureConst.MtoCoordinateGNSS))
                    {
                        coordinateGNSSBindingSource.EndEdit();
                        coordinateGNSSBindingSource.ResetCurrentItem();
                    }
                    else
                    {
                        obj.Km = km;
                        obj.Meter = m;
                        obj.Latitude = lati;
                        obj.Longtitude = longti;
                        obj.Altitude = alti;
                        obj.Exact_height = exhei;
                        obj.Exact_coordinate = excoord;
                        coordinateGNSSBindingSource.EndEdit();
                        coordinateGNSSBindingSource.ResetCurrentItem();
                        MetroMessageBox.Show(this, alerts.edit_error + " " + alerts.check_fields_filling, alerts.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch
            {
                MetroMessageBox.Show(this, alerts.edit_error, alerts.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void tsbRemoveCoordinateGNSS_Click(object sender, EventArgs e)
        {
            RemoveMainTrackObject(coordinateGNSSBindingSource, MainTrackStructureConst.MtoCoordinateGNSS);
        }

        private void pcCoordinateGNSSPeriod_PeriodSelectionChanged(object sender, EventArgs e)
        {
            PeriodSelectionChanged(coordinateGNSSBindingSource, MainTrackStructureConst.MtoCoordinateGNSS, pcCoordinateGNSSPeriod);
        }

        private void tsbAddDefectsEarth_Click(object sender, EventArgs e)
        {
            if (pcDefectsEarthPeriod.GetDataSource().Count <= 0)
            {
                MessageHelper.ShowWarning(this, alerts.before_insert_period);
                return;
            }
            using (var defectsEarthForm = new DefectsEarthForm())
            {
                defectsEarthForm.ShowDialog();
                if (defectsEarthForm.result == DialogResult.Cancel)
                    return;
                var defectsEarth = defectsEarthForm.defectsEarth;
                defectsEarth.Period_Id = pcDefectsEarthPeriod.CurrentId;
                MainTrackStructureService.InsertObject(defectsEarth, MainTrackStructureConst.MtoDefectsEarth);
                if (defectsEarth.Id > -1)
                {
                    defectsEarthBindingSource.Add(defectsEarth);
                    defectsEarthBindingSource.MoveLast();
                }
                else
                {
                    MessageHelper.ShowError(this, alerts.insert_error + " " + alerts.check_fields_filling);
                }
            }
        }

        private void tsbEditDefectsEarth_Click(object sender, EventArgs e)
        {
            if (pcDefectsEarthPeriod.GetDataSource().Count <= 0)
            {
                MetroMessageBox.Show(this, alerts.before_insert_period, alerts.editing, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                var obj = defectsEarthBindingSource.Current as DefectsEarth;
                using (var defectsEarthForm = new DefectsEarthForm())
                {
                    defectsEarthForm.SetForm(obj);
                    defectsEarthForm.ShowDialog();
                    if (defectsEarthForm.result == DialogResult.Cancel)
                        return;
                    var defearth = defectsEarthForm.defectsEarth;
                    int skm = obj.Start_Km, sm = obj.Start_M, fkm = obj.Final_Km, fm = obj.Final_M, typeid = obj.Type_id;
                    string type = obj.Type;
                    obj.Start_Km = defearth.Start_Km;
                    obj.Start_M = defearth.Start_M;
                    obj.Final_Km = defearth.Final_Km;
                    obj.Final_M = defearth.Final_M;
                    obj.Type_id = defearth.Type_id;
                    obj.Type = defearth.Type;

                    if (MainTrackStructureService.UpdateMtoObject(obj, MainTrackStructureConst.MtoDefectsEarth))
                    {
                        defectsEarthBindingSource.EndEdit();
                        defectsEarthBindingSource.ResetCurrentItem();
                    }
                    else
                    {
                        obj.Start_Km = skm;
                        obj.Start_M = sm;
                        obj.Final_Km = fkm;
                        obj.Final_M = fm;
                        obj.Type_id = typeid;
                        obj.Type = type;
                        defectsEarthBindingSource.EndEdit();
                        defectsEarthBindingSource.ResetCurrentItem();
                        MetroMessageBox.Show(this, alerts.edit_error + " " + alerts.check_fields_filling, alerts.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch
            {
                MetroMessageBox.Show(this, alerts.edit_error, alerts.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void tsbRemoveDefectsEarth_Click(object sender, EventArgs e)
        {
            RemoveMainTrackObject(defectsEarthBindingSource, MainTrackStructureConst.MtoDefectsEarth);
        }

        private void pcDefectsEarthPeriod_PeriodSelectionChanged(object sender, EventArgs e)
        {
            PeriodSelectionChanged(defectsEarthBindingSource, MainTrackStructureConst.MtoDefectsEarth, pcDefectsEarthPeriod);
        }

        private void tsbAddDistanceBetweenTracks_Click(object sender, EventArgs e)
        {
            if (pcDistanceBetweenTracksPeriod.GetDataSource().Count <= 0)
            {
                MessageHelper.ShowWarning(this, alerts.before_insert_period);
                return;
            }
            using (var distanceBetweenTracksForm = new DistanceBetweenTracksForm(ucDirections.CurrentUnitId))
            {
                distanceBetweenTracksForm.ShowDialog();
                if (distanceBetweenTracksForm.Result == DialogResult.Cancel)
                    return;
                var distanceBetweenTracks = distanceBetweenTracksForm.distanceBetweenTracks;
                distanceBetweenTracks.Period_Id = pcDistanceBetweenTracksPeriod.CurrentId;
                MainTrackStructureService.InsertObject(distanceBetweenTracks, MainTrackStructureConst.MtoDistanceBetweenTracks);
                if (distanceBetweenTracks.Id > -1)
                {
                    distanceBetweenTracksBindingSource.Add(distanceBetweenTracks);
                    distanceBetweenTracksBindingSource.MoveLast();
                }
                else
                {
                    MessageHelper.ShowError(this, alerts.insert_error + " " + alerts.check_fields_filling);
                }
            }
        }

        private void tsbEditDistanceBetweenTracks_Click(object sender, EventArgs e)
        {
            if (pcDistanceBetweenTracksPeriod.GetDataSource().Count <= 0)
            {
                MetroMessageBox.Show(this, alerts.before_insert_period, alerts.editing, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                var obj = distanceBetweenTracksBindingSource.Current as DistanceBetweenTracks;
                using (var distanceBetweenTracksForm = new DistanceBetweenTracksForm(ucDirections.CurrentUnitId))
                {
                    distanceBetweenTracksForm.SetForm(obj);
                    distanceBetweenTracksForm.ShowDialog();
                    if (distanceBetweenTracksForm.Result == DialogResult.Cancel)
                        return;
                    var dist = distanceBetweenTracksForm.distanceBetweenTracks;
                    Int64 leftid = obj.Left_adm_track_id, rightid = obj.Right_adm_track_id;
                    int skm = obj.Start_Km, sm = obj.Start_M, fkm = obj.Final_Km, fm = obj.Final_M, leftm = obj.Left_m, rightm = obj.Right_m;
                    string left = obj.Left_track, right = obj.Right_track;
                    obj.Start_Km = dist.Start_Km;
                    obj.Start_M = dist.Start_M;
                    obj.Final_Km = dist.Final_Km;
                    obj.Final_M = dist.Final_M;
                    obj.Left_adm_track_id = dist.Left_adm_track_id;
                    obj.Left_m = dist.Left_m;
                    obj.Left_track = dist.Left_track;
                    obj.Right_adm_track_id = dist.Right_adm_track_id;
                    obj.Right_m = dist.Right_m;
                    obj.Right_track = dist.Right_track;

                    if (MainTrackStructureService.UpdateMtoObject(obj, MainTrackStructureConst.MtoDistanceBetweenTracks))
                    {
                        distanceBetweenTracksBindingSource.EndEdit();
                        distanceBetweenTracksBindingSource.ResetCurrentItem();
                    }
                    else
                    {
                        obj.Start_Km = skm;
                        obj.Start_M = sm;
                        obj.Final_Km = fkm;
                        obj.Final_M = fm;
                        obj.Left_adm_track_id = leftid;
                        obj.Left_m = leftm;
                        obj.Left_track = left;
                        obj.Right_adm_track_id = rightid;
                        obj.Right_m = rightm;
                        obj.Right_track = right;
                        distanceBetweenTracksBindingSource.EndEdit();
                        distanceBetweenTracksBindingSource.ResetCurrentItem();
                        MetroMessageBox.Show(this, alerts.edit_error + " " + alerts.check_fields_filling, alerts.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch
            {
                MetroMessageBox.Show(this, alerts.edit_error, alerts.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void tsbRemoveDistanceBetweenTracks_Click(object sender, EventArgs e)
        {
            RemoveMainTrackObject(distanceBetweenTracksBindingSource, MainTrackStructureConst.MtoDistanceBetweenTracks);
        }

        private void pcDistanceBetweenTracksPeriod_PeriodSelectionChanged(object sender, EventArgs e)
        {
            PeriodSelectionChanged(distanceBetweenTracksBindingSource, MainTrackStructureConst.MtoDistanceBetweenTracks, pcDistanceBetweenTracksPeriod);
        }

        private void tsbAddChamJoint_Click(object sender, EventArgs e)
        {
            if (pcChamJoint.GetDataSource().Count <= 0)
            {
                MessageHelper.ShowWarning(this, alerts.inserting);
                return;
            }
            using (var chamJointForm = new ChamJointForm())
            {
                chamJointForm.SetExistingSource(chamJointBindingSource.DataSource as BindingSource);
                chamJointForm.ShowDialog();
                if (chamJointForm.result == DialogResult.Cancel)
                    return;
                var joint = chamJointForm.chamJoint;
                joint.Period_Id = pcChamJoint.CurrentId;
                MainTrackStructureService.InsertObject(joint, MainTrackStructureConst.MtoChamJoint);
                if (joint.Id > -1)
                {
                    chamJointBindingSource.Add(joint);
                    chamJointBindingSource.MoveLast();
                }
                else
                {
                    MessageHelper.ShowError(this, alerts.insert_error + " " + alerts.check_fields_filling);
                }
            }
        }

        private void tsbEditChamJoint_Click(object sender, EventArgs e)
        {
            if (pcChamJoint.GetDataSource().Count <= 0)
            {
                MetroMessageBox.Show(this, alerts.before_insert_period, alerts.editing, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                var obj = chamJointBindingSource.Current as ChamJoint;
                using (var chamJointForm = new ChamJointForm())
                {
                    chamJointForm.SetExistingSource(chamJointBindingSource.DataSource as BindingSource);
                    chamJointForm.SetChamJointForm(obj);
                    chamJointForm.ShowDialog();
                    if (chamJointForm.result == DialogResult.Cancel)
                        return;
                    var joint = chamJointForm.chamJoint;
                    int startkm = obj.Start_Km, startm = obj.Start_M, finalkm = obj.Final_Km, finalm = obj.Final_M, typeid = obj.Type_id;
                    string type = obj.Type;
                    obj.Start_Km = joint.Start_Km;
                    obj.Start_M = joint.Start_M;
                    obj.Final_Km = joint.Final_Km;
                    obj.Final_M = joint.Final_M;
                    obj.Type = joint.Type;
                    obj.Type_id = joint.Type_id;

                    if (MainTrackStructureService.UpdateMtoObject(obj, MainTrackStructureConst.MtoChamJoint))
                    {
                        chamJointBindingSource.EndEdit();
                        chamJointBindingSource.ResetCurrentItem();
                    }
                    else
                    {
                        obj.Start_Km = startkm;
                        obj.Start_M = startm;
                        obj.Final_Km = finalkm;
                        obj.Final_M = finalm;
                        obj.Type = type;
                        obj.Type_id = typeid;
                        chamJointBindingSource.EndEdit();
                        chamJointBindingSource.ResetCurrentItem();
                        MetroMessageBox.Show(this, alerts.edit_error + " " + alerts.check_fields_filling, alerts.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch
            {
                MetroMessageBox.Show(this, alerts.edit_error, alerts.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void tsbDeleteChamJoint_Click(object sender, EventArgs e)
        {
            RemoveMainTrackObject(chamJointBindingSource, MainTrackStructureConst.MtoChamJoint);
        }

        private void pcChamJoint_PeriodSelectionChanged(object sender, EventArgs e)
        {
            PeriodSelectionChanged(chamJointBindingSource, MainTrackStructureConst.MtoChamJoint, pcChamJoint);
        }

        private void tsbAddCheckSection_Click(object sender, EventArgs e)
        {
            if (pcCheckSection.GetDataSource().Count <= 0)
            {
                MessageHelper.ShowWarning(this, alerts.inserting);
                return;
            }
            using (var checkSectionForm = new CheckSectionForm())
            {
                checkSectionForm.SetExistingSource(checkSectionBindingSource.DataSource as BindingSource);
                checkSectionForm.ShowDialog();
                if (checkSectionForm.result == DialogResult.Cancel)
                    return;
                var checkSection = checkSectionForm.checkSection;
                checkSection.Period_Id = pcCheckSection.CurrentId;
                MainTrackStructureService.InsertObject(checkSection, MainTrackStructureConst.MtoCheckSection);
                if (checkSection.Id > -1)
                {
                    checkSectionBindingSource.Add(checkSection);
                    checkSectionBindingSource.MoveLast();
                }
                else
                {
                    MessageHelper.ShowError(this, alerts.insert_error + " " + alerts.check_fields_filling);
                }
            }
        }

        private void tsbEditCheckSection_Click(object sender, EventArgs e)
        {
            if (pcCheckSection.GetDataSource().Count <= 0)
            {
                MetroMessageBox.Show(this, alerts.before_insert_period, alerts.editing, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                var obj = checkSectionBindingSource.Current as CheckSection;
                using (var checkSectionForm = new CheckSectionForm())
                {
                    checkSectionForm.SetExistingSource(checkSectionBindingSource.DataSource as BindingSource);
                    checkSectionForm.SetCheckSectionForm(obj);
                    checkSectionForm.ShowDialog();
                    if (checkSectionForm.result == DialogResult.Cancel)
                        return;
                    var checkSection = checkSectionForm.checkSection;
                    int startkm = obj.Start_Km, startm = obj.Start_M, finalkm = obj.Final_Km, finalm = obj.Final_M;
                    double avgl = obj.Avg_level, avgw = obj.Avg_width, skol = obj.Sko_level, skow = obj.Sko_width;
                    obj.Start_Km = checkSection.Start_Km;
                    obj.Start_M = checkSection.Start_M;
                    obj.Final_Km = checkSection.Final_Km;
                    obj.Final_M = checkSection.Final_M;
                    obj.Avg_level = checkSection.Avg_level;
                    obj.Avg_width = checkSection.Avg_width;
                    obj.Sko_level = checkSection.Sko_level;
                    obj.Sko_width = checkSection.Sko_width;

                    if (MainTrackStructureService.UpdateMtoObject(obj, MainTrackStructureConst.MtoCheckSection))
                    {
                        checkSectionBindingSource.EndEdit();
                        checkSectionBindingSource.ResetCurrentItem();
                    }
                    else
                    {
                        obj.Start_Km = startkm;
                        obj.Start_M = startm;
                        obj.Final_Km = finalkm;
                        obj.Final_M = finalm;
                        obj.Avg_level = avgl;
                        obj.Avg_width = avgw;
                        obj.Sko_level = skol;
                        obj.Sko_width = skow;
                        checkSectionBindingSource.EndEdit();
                        checkSectionBindingSource.ResetCurrentItem();
                        MetroMessageBox.Show(this, alerts.edit_error + " " + alerts.check_fields_filling, alerts.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch
            {
                MetroMessageBox.Show(this, alerts.edit_error, alerts.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void tsbDeleteCheckSection_Click(object sender, EventArgs e)
        {
            RemoveMainTrackObject(checkSectionBindingSource, MainTrackStructureConst.MtoCheckSection);
        }

        private void pcCheckSection_PeriodSelectionChanged(object sender, EventArgs e)
        {
            PeriodSelectionChanged(checkSectionBindingSource, MainTrackStructureConst.MtoCheckSection, pcCheckSection);
        }

        private void tsbAddProfmarks_Click(object sender, EventArgs e)
        {
            if (pcProfmarks.GetDataSource().Count <= 0)
            {
                MessageHelper.ShowWarning(this, alerts.inserting);
                return;
            }
            using (var profmarksForm = new ProfmarksForm())
            {
                profmarksForm.SetExistingSource(profmarksBindingSource.DataSource as BindingSource);
                profmarksForm.ShowDialog();
                if (profmarksForm.result == DialogResult.Cancel)
                    return;
                var profobj = profmarksForm.profmark;
                profobj.Period_Id = pcProfmarks.CurrentId;
                MainTrackStructureService.InsertObject(profobj, MainTrackStructureConst.MtoProfmarks);
                if (profobj.Id > -1)
                {
                    profmarksBindingSource.Add(profobj);
                    profmarksBindingSource.MoveLast();
                }
                else
                {
                    MessageHelper.ShowError(this, alerts.insert_error + " " + alerts.check_fields_filling);
                }
            }
        }

        private void tsbEditProfmarks_Click(object sender, EventArgs e)
        {
            if (pcProfmarks.GetDataSource().Count <= 0)
            {
                MetroMessageBox.Show(this, alerts.before_insert_period, alerts.editing, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                var obj = profmarksBindingSource.Current as Profmarks;
                using (var profmarksForm = new ProfmarksForm())
                {
                    profmarksForm.SetExistingSource(profmarksBindingSource.DataSource as BindingSource);
                    profmarksForm.SetProfmarksForm(obj);
                    profmarksForm.ShowDialog();
                    if (profmarksForm.result == DialogResult.Cancel)
                        return;
                    var profobj = profmarksForm.profmark;
                    int km = obj.Km, meter = obj.Meter;
                    double profil = obj.Profil;
                    obj.Km = profobj.Km;
                    obj.Meter = profobj.Meter;
                    obj.Profil = profobj.Profil;

                    if (MainTrackStructureService.UpdateMtoObject(obj, MainTrackStructureConst.MtoProfmarks))
                    {
                        profmarksBindingSource.EndEdit();
                        profmarksBindingSource.ResetCurrentItem();
                    }
                    else
                    {
                        obj.Km = km;
                        obj.Meter = meter;
                        obj.Profil = profil;
                        profmarksBindingSource.EndEdit();
                        profmarksBindingSource.ResetCurrentItem();
                        MetroMessageBox.Show(this, alerts.edit_error + " " + alerts.check_fields_filling, alerts.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch
            {
                MetroMessageBox.Show(this, alerts.edit_error, alerts.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void tsbDeleteProfmarks_Click(object sender, EventArgs e)
        {
            RemoveMainTrackObject(profmarksBindingSource, MainTrackStructureConst.MtoProfmarks);
        }

        private void pcProfmarks_PeriodSelectionChanged(object sender, EventArgs e)
        {
            PeriodSelectionChanged(profmarksBindingSource, MainTrackStructureConst.MtoProfmarks, pcProfmarks);
        }

        private void tsbAddRefPoint_Click(object sender, EventArgs e)
        {
            if (pcRefPoint.GetDataSource().Count <= 0)
            {
                MessageHelper.ShowWarning(this, alerts.inserting);
                return;
            }
            using (var refPointForm = new RefPointForm())
            {
                refPointForm.SetExistingSource(refPointBindingSource.DataSource as BindingSource);
                refPointForm.ShowDialog();
                if (refPointForm.result == DialogResult.Cancel)
                    return;
                var refobj = refPointForm.point;
                refobj.Period_Id = pcRefPoint.CurrentId;
                MainTrackStructureService.InsertObject(refobj, MainTrackStructureConst.MtoRefPoint);
                if (refobj.Id > -1)
                {
                    refPointBindingSource.Add(refobj);
                    refPointBindingSource.MoveLast();
                }
                else
                {
                    MessageHelper.ShowError(this, alerts.insert_error + " " + alerts.check_fields_filling);
                }
            }
        }

        private void tsbEditRefPoint_Click(object sender, EventArgs e)
        {
            if (pcRefPoint.GetDataSource().Count <= 0)
            {
                MetroMessageBox.Show(this, alerts.before_insert_period, alerts.editing, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                var obj = refPointBindingSource.Current as RefPoint;
                using (var refPointForm = new RefPointForm())
                {
                    refPointForm.SetExistingSource(refPointBindingSource.DataSource as BindingSource);
                    refPointForm.SetRefPointForm(obj);
                    refPointForm.ShowDialog();
                    if (refPointForm.result == DialogResult.Cancel)
                        return;
                    var refobj = refPointForm.point;
                    int km = obj.Km, meter = obj.Meter;
                    double mark = obj.Mark;
                    obj.Km = refobj.Km;
                    obj.Meter = refobj.Meter;
                    obj.Mark = refobj.Mark;

                    if (MainTrackStructureService.UpdateMtoObject(obj, MainTrackStructureConst.MtoRefPoint))
                    {
                        refPointBindingSource.EndEdit();
                        refPointBindingSource.ResetCurrentItem();
                    }
                    else
                    {
                        obj.Km = km;
                        obj.Meter = meter;
                        obj.Mark = mark;
                        refPointBindingSource.EndEdit();
                        refPointBindingSource.ResetCurrentItem();
                        MetroMessageBox.Show(this, alerts.edit_error + " " + alerts.check_fields_filling, alerts.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch
            {
                MetroMessageBox.Show(this, alerts.edit_error, alerts.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void tsbDeleteRefPoint_Click(object sender, EventArgs e)
        {
            RemoveMainTrackObject(refPointBindingSource, MainTrackStructureConst.MtoRefPoint);
        }

        private void pcRefPoint_PeriodSelectionChanged(object sender, EventArgs e)
        {
            PeriodSelectionChanged(refPointBindingSource, MainTrackStructureConst.MtoRefPoint, pcRefPoint);
        }

        private void tsbAddRfid_Click(object sender, EventArgs e)
        {
            if (pcRfid.GetDataSource().Count <= 0)
            {
                MessageHelper.ShowWarning(this, alerts.inserting);
                return;
            }
            using (var rfidForm = new RfidForm())
            {
                rfidForm.SetExistingSource(rfidBindingSource.DataSource as BindingSource);
                rfidForm.ShowDialog();
                if (rfidForm.result == DialogResult.Cancel)
                    return;
                var refobj = rfidForm.point;
                refobj.Period_Id = pcRfid.CurrentId;
                MainTrackStructureService.InsertObject(refobj, MainTrackStructureConst.MtoRFID);
                if (refobj.Id > -1)
                {
                    rfidBindingSource.Add(refobj);
                    rfidBindingSource.MoveLast();
                }
                else
                {
                    MessageHelper.ShowError(this, alerts.insert_error + " " + alerts.check_fields_filling);
                }
            }
        }

        private void tsbEditRfid_Click(object sender, EventArgs e)
        {
            if (pcRfid.GetDataSource().Count <= 0)
            {
                MetroMessageBox.Show(this, alerts.before_insert_period, alerts.editing, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                var obj = rfidBindingSource.Current as Rfid;
                using (var rfidForm = new RfidForm())
                {
                    rfidForm.SetExistingSource(rfidBindingSource.DataSource as BindingSource);
                    rfidForm.SetRfidForm(obj);
                    rfidForm.ShowDialog();
                    if (rfidForm.result == DialogResult.Cancel)
                        return;
                    var rfid = rfidForm.point;
                    string mark = obj.Mark;
                    int km = obj.Km, meter = obj.Meter;
                    obj.Mark = rfid.Mark;
                    obj.Km = rfid.Km;
                    obj.Meter = rfid.Meter;

                    if (MainTrackStructureService.UpdateMtoObject(obj, MainTrackStructureConst.MtoRFID))
                    {
                        rfidBindingSource.EndEdit();
                        rfidBindingSource.ResetCurrentItem();
                    }
                    else
                    {
                        obj.Mark = mark;
                        obj.Km = km;
                        obj.Meter = meter;
                        rfidBindingSource.EndEdit();
                        rfidBindingSource.ResetCurrentItem();
                        MetroMessageBox.Show(this, alerts.edit_error + " " + alerts.check_fields_filling, alerts.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch
            {
                MetroMessageBox.Show(this, alerts.edit_error, alerts.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void tsbDeleteRfid_Click(object sender, EventArgs e)
        {
            RemoveMainTrackObject(rfidBindingSource, MainTrackStructureConst.MtoRFID);
        }

        private void pcRfid_PeriodSelectionChanged(object sender, EventArgs e)
        {
            PeriodSelectionChanged(rfidBindingSource, MainTrackStructureConst.MtoRFID, pcRfid);
        }

        private void tsbAddTraffic_Click(object sender, EventArgs e)
        {
            if (pcTraffic.GetDataSource().Count <= 0)
            {
                MessageHelper.ShowWarning(this, alerts.inserting);
                return;
            }
            using (var trafficForm = new TrafficForm())
            {
                trafficForm.SetExistingSource(mtoTrafficBindingSource.DataSource as BindingSource);
                trafficForm.ShowDialog();
                if (trafficForm.result == DialogResult.Cancel)
                    return;
                var trafficobj = trafficForm.traffic;
                trafficobj.Period_Id = pcTraffic.CurrentId;
                MainTrackStructureService.InsertObject(trafficobj, MainTrackStructureConst.MtoTraffic);
                if (trafficobj.Id > -1)
                {
                    mtoTrafficBindingSource.Add(trafficobj);
                    mtoTrafficBindingSource.MoveLast();
                }
                else
                {
                    MessageHelper.ShowError(this, alerts.insert_error + " " + alerts.check_fields_filling);
                }
            }
        }

        private void tsbEditTraffic_Click(object sender, EventArgs e)
        {
            if (pcTraffic.GetDataSource().Count <= 0)
            {
                MetroMessageBox.Show(this, alerts.before_insert_period, alerts.editing, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                var obj = mtoTrafficBindingSource.Current as MtoTraffic;
                using (var trafficForm = new TrafficForm())
                {
                    trafficForm.SetExistingSource(mtoTrafficBindingSource.DataSource as BindingSource);
                    trafficForm.SetTrafficForm(obj);
                    trafficForm.ShowDialog();
                    if (trafficForm.result == DialogResult.Cancel)
                        return;
                    var trafficobj = trafficForm.traffic;
                    int startkm = obj.Start_Km, startm = obj.Start_M, finalkm = obj.Final_Km, finalm = obj.Final_M, traffic = obj.Traffic;
                    obj.Start_Km = trafficobj.Start_Km;
                    obj.Start_M = trafficobj.Start_M;
                    obj.Final_Km = trafficobj.Final_Km;
                    obj.Final_M = trafficobj.Final_M;
                    obj.Traffic = trafficobj.Traffic;


                    if (MainTrackStructureService.UpdateMtoObject(obj, MainTrackStructureConst.MtoTraffic))
                    {
                        mtoTrafficBindingSource.EndEdit();
                        mtoTrafficBindingSource.ResetCurrentItem();
                    }
                    else
                    {
                        obj.Start_Km = startkm;
                        obj.Start_M = startm;
                        obj.Final_Km = finalkm;
                        obj.Final_M = finalm;
                        obj.Traffic = traffic;
                        mtoTrafficBindingSource.EndEdit();
                        mtoTrafficBindingSource.ResetCurrentItem();
                        MetroMessageBox.Show(this, alerts.edit_error + " " + alerts.check_fields_filling, alerts.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch
            {
                MetroMessageBox.Show(this, alerts.edit_error, alerts.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void tsbDeleteTraffic_Click(object sender, EventArgs e)
        {
            RemoveMainTrackObject(mtoTrafficBindingSource, MainTrackStructureConst.MtoTraffic);
        }

        private void pcTraffic_PeriodSelectionChanged(object sender, EventArgs e)
        {
            PeriodSelectionChanged(mtoTrafficBindingSource, MainTrackStructureConst.MtoTraffic, pcTraffic);
        }

        private void tsbAddDeep_Click(object sender, EventArgs e)
        {
            if (pcDeep.GetDataSource().Count <= 0)
            {
                MetroMessageBox.Show(this, alerts.before_insert_period, alerts.editing, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            using (var deepForm = new DeepForm())
            {
                deepForm.ShowDialog();
                if (deepForm.result == DialogResult.Cancel)
                    return;
                var deepobj = deepForm.deep;
                deepobj.Period_Id = pcDeep.CurrentId;
                MainTrackStructureService.InsertObject(deepobj, MainTrackStructureConst.MtoDeep);
                if (deepobj.Id > -1)
                {
                    deepBindingSource.Add(deepobj);
                    deepBindingSource.MoveLast();
                }
                else
                {
                    MessageHelper.ShowError(this, alerts.insert_error + " " + alerts.check_fields_filling);
                }
            }
        }

        private void tsbEditDeep_Click(object sender, EventArgs e)
        {
            if (pcDeep.GetDataSource().Count <= 0)
            {
                MetroMessageBox.Show(this, alerts.empty_table, alerts.editing, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                var obj = deepBindingSource.Current as Deep;
                using (var deepForm = new DeepForm())
                {
                    deepForm.SetForm(obj);
                    deepForm.ShowDialog();
                    if (deepForm.result == DialogResult.Cancel)
                        return;
                    var deepobj = deepForm.deep;
                    int startkm = obj.Start_Km, startm = obj.Start_M, finalkm = obj.Final_Km, finalm = obj.Final_M;
                    obj.Start_Km = deepobj.Start_Km;
                    obj.Start_M = deepobj.Start_M;
                    obj.Final_Km = deepobj.Final_Km;
                    obj.Final_M = deepobj.Final_M;


                    if (MainTrackStructureService.UpdateMtoObject(obj, MainTrackStructureConst.MtoDeep))
                    {
                        deepBindingSource.EndEdit();
                        deepBindingSource.ResetCurrentItem();
                    }
                    else
                    {
                        obj.Start_Km = startkm;
                        obj.Start_M = startm;
                        obj.Final_Km = finalkm;
                        obj.Final_M = finalm;
                        deepBindingSource.EndEdit();
                        deepBindingSource.ResetCurrentItem();
                        MetroMessageBox.Show(this, alerts.edit_error + " " + alerts.check_fields_filling, alerts.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch
            {
                MetroMessageBox.Show(this, alerts.edit_error, alerts.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void tsbDeleteDeep_Click(object sender, EventArgs e)
        {
            RemoveMainTrackObject(deepBindingSource, MainTrackStructureConst.MtoDeep);
        }

        private void pcDeep_PeriodSelectionChanged(object sender, EventArgs e)
        {
            PeriodSelectionChanged(deepBindingSource, MainTrackStructureConst.MtoDeep, pcDeep);
        }

        private void tsbAddBallast_Click(object sender, EventArgs e)
        {
            if (pcBallastType.GetDataSource().Count <= 0)
            {
                MetroMessageBox.Show(this, alerts.before_insert_period, alerts.editing, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            using (var ballastForm = new BallastForm())
            {
                ballastForm.ShowDialog();
                if (ballastForm.result == DialogResult.Cancel)
                    return;
                var ballobj = ballastForm.ballast;
                ballobj.Period_Id = pcBallastType.CurrentId;
                MainTrackStructureService.InsertObject(ballobj, MainTrackStructureConst.MtoBallastType);
                if (ballobj.Id > -1)
                {
                    ballastTypeBindingSource.Add(ballobj);
                    ballastTypeBindingSource.MoveLast();
                }
                else
                {
                    MessageHelper.ShowError(this, alerts.insert_error + " " + alerts.check_fields_filling);
                }
            }
        }

        private void tsbEditBallast_Click(object sender, EventArgs e)
        {
            if (pcBallastType.GetDataSource().Count <= 0)
            {
                MetroMessageBox.Show(this, alerts.empty_table, alerts.editing, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                var obj = ballastTypeBindingSource.Current as BallastType;
                using (var ballastForm = new BallastForm())
                {
                    ballastForm.SetForm(obj);
                    ballastForm.ShowDialog();
                    if (ballastForm.result == DialogResult.Cancel)
                        return;
                    var ballobj = ballastForm.ballast;
                    int startkm = obj.Start_Km, startm = obj.Start_M, finalkm = obj.Final_Km, finalm = obj.Final_M, ball = obj.Ballast;
                    obj.Start_Km = ballobj.Start_Km;
                    obj.Start_M = ballobj.Start_M;
                    obj.Final_Km = ballobj.Final_Km;
                    obj.Final_M = ballobj.Final_M;
                    obj.Ballast = ballobj.Ballast;


                    if (MainTrackStructureService.UpdateMtoObject(obj, MainTrackStructureConst.MtoBallastType))
                    {
                        ballastTypeBindingSource.EndEdit();
                        ballastTypeBindingSource.ResetCurrentItem();
                    }
                    else
                    {
                        obj.Start_Km = startkm;
                        obj.Start_M = startm;
                        obj.Final_Km = finalkm;
                        obj.Final_M = finalm;
                        obj.Ballast = ball;
                        ballastTypeBindingSource.EndEdit();
                        ballastTypeBindingSource.ResetCurrentItem();
                        MetroMessageBox.Show(this, alerts.edit_error + " " + alerts.check_fields_filling, alerts.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch
            {
                MetroMessageBox.Show(this, alerts.edit_error, alerts.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void tsbDeleteBallast_Click(object sender, EventArgs e)
        {
            RemoveMainTrackObject(ballastTypeBindingSource, MainTrackStructureConst.MtoBallastType);
        }

        private void pcBallast_PeriodSelectionChanged(object sender, EventArgs e)
        {
            PeriodSelectionChanged(ballastTypeBindingSource, MainTrackStructureConst.MtoBallastType, pcBallastType);
        }

        private void tsbAddWaycat_Click(object sender, EventArgs e)
        {
            if (pcWaycat.GetDataSource().Count <= 0)
            {
                MessageHelper.ShowWarning(this, alerts.inserting);
                return;
            }
            using (var waycatForm = new WaycatForm())
            {
                waycatForm.SetExistingSource(waycatBindingSource.DataSource as BindingSource);
                waycatForm.ShowDialog();
                if (waycatForm.result == DialogResult.Cancel)
                    return;
                var waycatobj = waycatForm.waycat;
                waycatobj.Period_Id = pcWaycat.CurrentId;
                MainTrackStructureService.InsertObject(waycatobj, MainTrackStructureConst.MtoWaycat);
                if (waycatobj.Id > -1)
                {
                    waycatBindingSource.Add(waycatobj);
                    waycatBindingSource.MoveLast();
                }
                else
                {
                    MessageHelper.ShowError(this, alerts.insert_error + " " + alerts.check_fields_filling);
                }
            }
        }

        private void tsbEditWaycat_Click(object sender, EventArgs e)
        {
            if (pcWaycat.GetDataSource().Count <= 0)
            {
                MetroMessageBox.Show(this, alerts.before_insert_period, alerts.editing, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                var obj = waycatBindingSource.Current as Waycat;
                using (var waycatForm = new WaycatForm())
                {
                    waycatForm.SetExistingSource(waycatBindingSource.DataSource as BindingSource);
                    waycatForm.SetWaycatForm(obj);
                    waycatForm.ShowDialog();
                    if (waycatForm.result == DialogResult.Cancel)
                        return;
                    var waycatobj = waycatForm.waycat;
                    int startkm = obj.Start_Km, startm = obj.Start_M, finalkm = obj.Final_Km, finalm = obj.Final_M, typeid = obj.Type_id;
                    string type = obj.Type;
                    obj.Start_Km = waycatobj.Start_Km;
                    obj.Start_M = waycatobj.Start_M;
                    obj.Final_Km = waycatobj.Final_Km;
                    obj.Final_M = waycatobj.Final_M;
                    obj.Type_id = waycatobj.Type_id;
                    obj.Type = waycatobj.Type;

                    if (MainTrackStructureService.UpdateMtoObject(obj, MainTrackStructureConst.MtoWaycat))
                    {
                        waycatBindingSource.EndEdit();
                        waycatBindingSource.ResetCurrentItem();
                    }
                    else
                    {
                        obj.Start_Km = startkm;
                        obj.Start_M = startm;
                        obj.Final_Km = finalkm;
                        obj.Final_M = finalm;
                        obj.Type_id = typeid;
                        obj.Type = type;
                        waycatBindingSource.EndEdit();
                        waycatBindingSource.ResetCurrentItem();
                        MetroMessageBox.Show(this, alerts.edit_error + " " + alerts.check_fields_filling, alerts.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch
            {
                MetroMessageBox.Show(this, alerts.edit_error, alerts.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void tsbDeleteWaycat_Click(object sender, EventArgs e)
        {
            RemoveMainTrackObject(waycatBindingSource, MainTrackStructureConst.MtoWaycat);
        }

        private void pcWaycat_PeriodSelectionChanged(object sender, EventArgs e)
        {
            PeriodSelectionChanged(waycatBindingSource, MainTrackStructureConst.MtoWaycat, pcWaycat);
        }

        private void tsbAddRepairProject_Click(object sender, EventArgs e)
        {
            if (ucTracks.GetDataSource().Count <= 0)
            {
                MessageHelper.ShowWarning(this, alerts.before_insert_track);
                return;
            }
            using (var repairProjectForm = new RepairProjectForm())
            {
                repairProjectForm.ShowDialog();
                if (repairProjectForm.result == DialogResult.Cancel)
                    return;
                var repairobj = repairProjectForm.repairProject;
                repairobj.Adm_track_id = ucTracks.CurrentUnitId;
                MainTrackStructureService.InsertObject(repairobj, MainTrackStructureConst.MtoRepairProject);
                if (repairobj.Id > -1)
                {
                    repairProjectBindingSource.Add(repairobj);
                    repairProjectBindingSource.MoveLast();
                }
                else
                {
                    MessageHelper.ShowError(this, alerts.insert_error + " " + alerts.check_fields_filling);
                }
            }
        }

        private void tsbEditRepairProject_Click(object sender, EventArgs e)
        {
            if (ucTracks.GetDataSource().Count <= 0)
            {
                MetroMessageBox.Show(this, alerts.before_insert_track, alerts.editing, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                var obj = repairProjectBindingSource.Current as RepairProject;
                using (var repairProjectForm = new RepairProjectForm())
                {
                    repairProjectForm.SetRepairProjectForm(obj);
                    repairProjectForm.ShowDialog();
                    if (repairProjectForm.result == DialogResult.Cancel)
                        return;
                    var repairobj = repairProjectForm.repairProject;
                    int startkm = obj.Start_Km, startm = obj.Start_M, finalkm = obj.Final_Km, finalm = obj.Final_M, typeid = obj.Type_id, acceptid = obj.Accept_id, speed = obj.Speed;
                    string type = obj.Type, accept = obj.Accept;
                    DateTime dateTime = obj.Repair_date;
                    obj.Start_Km = repairobj.Start_Km;
                    obj.Start_M = repairobj.Start_M;
                    obj.Final_Km = repairobj.Final_Km;
                    obj.Final_M = repairobj.Final_M;
                    obj.Type = repairobj.Type;
                    obj.Type_id = repairobj.Type_id;
                    obj.Accept_id = repairobj.Accept_id;
                    obj.Accept = repairobj.Accept;
                    obj.Repair_date = repairobj.Repair_date;
                    obj.Speed = repairobj.Speed;

                    if (MainTrackStructureService.UpdateMtoObject(obj, MainTrackStructureConst.MtoRepairProject))
                    {
                        repairProjectBindingSource.EndEdit();
                        repairProjectBindingSource.ResetCurrentItem();
                    }
                    else
                    {
                        obj.Start_Km = startkm;
                        obj.Start_M = startm;
                        obj.Final_Km = finalkm;
                        obj.Final_M = finalm;
                        obj.Type = type;
                        obj.Type_id = typeid;
                        obj.Accept = accept;
                        obj.Accept_id = acceptid;
                        obj.Repair_date = dateTime;
                        obj.Speed = speed;
                        repairProjectBindingSource.EndEdit();
                        repairProjectBindingSource.ResetCurrentItem();
                        MetroMessageBox.Show(this, alerts.edit_error + " " + alerts.check_fields_filling, alerts.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            catch
            {
                MetroMessageBox.Show(this, alerts.edit_error, alerts.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void tsbDeleteRepairProject_Click(object sender, EventArgs e)
        {
            RemoveMainTrackObject(repairProjectBindingSource, MainTrackStructureConst.MtoRepairProject);
        }

        private void generateDirectionList_Click(object sender, EventArgs e)
        {
            try
            {
                using (var periodGenerate = new PeriodGenerateDirListForm())
                {
                    periodGenerate.ShowDialog();
                    if (periodGenerate.Result == DialogResult.Cancel)
                        return;
                    DateTime dirListDate = periodGenerate.Period.Start_Date;

                    string dirName = System.IO.Directory.GetCurrentDirectory() + "\\DirList\\" + ucDirections.GetDirectionName().TrimEnd(' ') + "_" + dirListDate.ToShortDateString();
                    DirectoryInfo dirInfo = new DirectoryInfo(dirName);

                    if (dirInfo.Exists)
                    {
                        foreach (FileInfo fileInfo in dirInfo.GetFiles())
                            fileInfo.Delete();
                    }
                    else
                        dirInfo.Create();

                    using (var textWirter = new StreamWriter(dirName + "\\Putlist.txt", false, System.Text.Encoding.GetEncoding(1251)))
                    {
                        textWirter.WriteLine(DateTime.Today.ToString("dd.MM.yyyy"));
                        textWirter.WriteLine(DateTime.Now.ToString("HH:mm:ss"));
                        textWirter.WriteLine(ucDirections.GetDirectionName().TrimEnd(' '));
                        textWirter.WriteLine(dirListDate.ToShortDateString());
                    }

                    foreach (int trackID in ucTracks.GetTrackID())
                        if (!MainTrackStructureService.GenerateDirectionList(ucDirections.GetDirectionName().TrimEnd(' '), trackID, dirName, dirListDate))
                            MetroMessageBox.Show(this, alerts.generate_list_error);
                }
            }
            catch
            {
                MetroMessageBox.Show(this, alerts.generate_list_error);
            }
        }

        private void importData_Click(object sender, EventArgs e)
        {

        }

        private void scStation_UnitSelectionChanged(object sender, EventArgs e)
        {
            CleanTables(AdmStructureConst.AdmStation);
            if (scStation.GetDataSource().Count > 0)
            {
                scPark.parentId = scStation.currentUnitId;
                scPark.SetDataSource(AdmStructureService.GetUnits(scPark.AdmLevel, scPark.parentId));
                scStationTracks.Title = "Станционные пути";
                scStationTracks.AdmLevel = AdmStructureConst.AdmStationTrack;
                scStationTracks.parentId = scStation.currentUnitId;
                scStationTracks.SetDataSource(AdmStructureService.GetUnits(scStationTracks.AdmLevel, scStationTracks.parentId));
                ucStationsDirections.SetDataSource(AdmStructureService.GetStationsDirection(scStation.currentUnitId, ucRoads.CurrentUnitId));
            }
        }

        private void scPark_UnitSelectionChanged(object sender, EventArgs e)
        {
            CleanTables(AdmStructureConst.AdmPark);
            if (scPark.GetDataSource().Count > 0)
            {
                scStation.ClearSelection();
                scStationObject.parentId = scPark.currentUnitId;
                scStationObject.SetDataSource(AdmStructureService.GetUnits(scStationObject.AdmLevel, scStationObject.parentId));
                scStationTracks.Title = "Парковые пути";
                scStationTracks.AdmLevel = AdmStructureConst.AdmParkTrack;
                scStationTracks.parentId = scPark.currentUnitId;
                scStationTracks.SetDataSource(AdmStructureService.GetUnits(scStationTracks.AdmLevel, scStationTracks.parentId));
            }
        }

        private void exportCSV_Click(object sender, EventArgs e)
        {
            using (var exportForm = new ExportForm())
            {
                exportForm.ShowDialog();
                if (exportForm.Result == DialogResult.Cancel)
                    return;

                this.Enabled = false;
                if (!ImportAndExport.Export(progressBar, exportForm.admRoadId, exportForm.Period.Start_Date))
                    MetroMessageBox.Show(this, alerts.export_error);
                else
                    MetroMessageBox.Show(this, alerts.success);
                progressBar.Value = 0;
                this.Enabled = true;
            }
        }

        private void exportEKASUI_Click(object sender, EventArgs e)
        {
            using (var mainParametersProcessPeriodForm = new MainParametersProcessPeriod())
            {
                mainParametersProcessPeriodForm.ShowDialog();
                if (mainParametersProcessPeriodForm.Result == DialogResult.Cancel)
                    return;

                string dirName = System.IO.Directory.GetCurrentDirectory() + @"\export_ALARm\EKASUI";
                //string carNumber = "24543";
                DirectoryInfo dirInfo = new DirectoryInfo(dirName);
                Random r = new Random();
                if (!dirInfo.Exists)
                    dirInfo.Create();
                var trip = RdStructureService.GetTrip(mainParametersProcessPeriodForm.trip_id);
                
                    trip.Car = "24543";

                try
                {
                    XDocument gParamaters = new XDocument();
                    XElement gOutFile = new XElement("OutFile",
                        new XAttribute("nsiver", DateTime.Now.ToString("dd.MM.yyyy")));



                    XDocument xdDeviation = new XDocument();
                    XElement xeOutFile = new XElement("OutFile",
                        new XAttribute("nsiver", DateTime.Now.ToString("dd.MM.yyyy")));

                    //MainParametersProcess mainParametersProcess = RdStructureService.GetMainParametersProcess(mainParametersProcessPeriodForm.mainProcessId);
                    //List<Trips> trips = RdStructureService.GetTrips(mainParametersProcessPeriodForm.mainProcessId) as List<Trips>;
                    
                    List<S3> s3list = RdStructureService.GetS3(mainParametersProcessPeriodForm.trip_id) as List<S3>;
                    
                    bool s3Exists = s3list.Count > 0 ? true : false;

                    if (s3Exists)
                    {
                        XElement xeHeader = new XElement("header",
                        new XAttribute("Manufacture", ""),
                        new XAttribute("PackageID", trip.Car.Trim() + s3list[0].Put),
                        new XAttribute("PackageNum", "1"),
                        new XAttribute("SiteID", s3list[0].Put),
                        new XAttribute("carID", trip.Car.Trim()),
                        new XAttribute("soft", "ALARmDK"),
                        new XAttribute("runDate", trip.Trip_date.ToString("dd.MM.yyyy")),
                        new XAttribute("decodeDate", trip.Trip_date.ToString("dd.MM.yyyy")),
                        new XAttribute("decoder", trip.Chief.Trim()),
                        new XAttribute("pathType", "1"),
                        new XAttribute("pathID", ""),
                        new XAttribute("pathText", ""));
                        xeOutFile.Add(xeHeader);

                        foreach (S3 s3 in s3list)
                        {
                            string defectID = String.Empty;
                            switch (s3.Ots)
                            {
                                case "Р":
                                case "Р3м":
                                case "Р м":
                                    defectID = "090004000303";
                                    break;
                                case "Р.н":
                                case "Рн3м":
                                case "Рн м":
                                    defectID = "090004015377";
                                    break;
                                case "Уш":
                                    defectID = "090004012176";
                                    break;
                                case "Суж":
                                    defectID = "090004012174";
                                    break;
                                case "ОШК":
                                    defectID = "090004000294";
                                    break;
                                case "УУ":
                                    defectID = "090004015317";
                                    break;
                                case "Пр.п":
                                case "Пр.п3м":
                                case "Пр.п м":
                                    defectID = "090004012681";
                                    break;
                                case "Пр.л":
                                case "Пр.л3м":
                                case "Пр.л м":
                                    defectID = "090004012166";
                                    break;
                                case "У":
                                case "У3м":
                                case "У м":
                                    defectID = "090004012169";
                                    break;
                                case "П":
                                case "П3м":
                                case "П м":
                                    defectID = "090004012168";
                                    break;
                                //устр.кривых
                                case "Анп":
                                    defectID = "090004007837";
                                    break;
                                case "Укл":
                                    defectID = "090004012154";
                                    break;
                                case "Пси":
                                    defectID = "090004007838";
                                    break;
                                //сочетания
                                case "Ш10":
                                    defectID = "090004007820";
                                    break;
                            }

                            var sector = MainTrackStructureService.GetSector(s3.Track_id, s3.Km, trip.Trip_date);
                            var station_section = MainTrackStructureService.GetMtoObjectsByCoord(trip.Trip_date, s3.Km, MainTrackStructureConst.MtoStationSection, s3.Track_id) as List<StationSection>;
                            var pdb_section = MainTrackStructureService.GetMtoObjectsByCoord(trip.Trip_date, s3.Km, MainTrackStructureConst.MtoPdbSection, s3.Track_id) as List<PdbSection>;
                            XElement gInsident = new XElement("Incident",
                                new XAttribute("REC", s3list.IndexOf(s3)),
                                new XAttribute("kod_otstup", !defectID.Equals(String.Empty) ? defectID : "n"),
                                new XAttribute("kod_napr", s3.Directcode),
                                new XAttribute("kod_st", station_section.Count > 0 ? station_section[0].Station : sector),
                                new XAttribute("nzhs", pdb_section.Count>0 ? pdb_section[0].Nod : "n"),
                                new XAttribute("npch", s3.Pch),
                                new XAttribute("nput", s3.Put),
                                new XAttribute("date", trip.Trip_date.ToString("dd.MM.yyyy")),
                                new XAttribute("time", trip.Trip_date.ToString("hh.mm.ss")),
                                new XAttribute("temp_vozd", r.Next(25,26)),
                                new XAttribute("temp_rels", r.Next(25, 26)+20),
                                new XAttribute("nomer_mdk", trip.Car),
                                new XAttribute("avtor", trip.Chief.Trim()),
                                new XAttribute("km", s3.Km),
                                new XAttribute("pk", s3.Meter / 100+1),
                                new XAttribute("metr", s3.Meter % 100),
                                new XAttribute("dlina_ots", s3.Len),
                                new XAttribute("velich_ots", s3.Otkl),
                                new XAttribute("stepen_ots", s3.Ots),
                                new XAttribute("kol_ots", s3.Kol),
                                new XAttribute("speed_ogr", s3.Ovp != -1 ? s3.Ovp : 0)

                                ); 
                            XElement xeIncident = new XElement("Incident",
                                new XAttribute("recID", trip.Trip_date.ToString("yyyyMMddHHmmss")),
                                new XAttribute("time", trip.Trip_date.ToString("dd.MM.yyyy HH.mm.ss")),
                                new XAttribute("startKM", s3.Km),
                                new XAttribute("startM", s3.Meter),
                                //new XAttribute("span", "n"),
                                new XAttribute("defectText", s3.Ots),
                                new XAttribute("defectID", !defectID.Equals(String.Empty) ? defectID : "n"),
                                new XAttribute("Degree", s3.Typ),
                                new XAttribute("Length", s3.Len),
                                new XAttribute("Amplitude", s3.Otkl),
                                new XAttribute("Absolute", s3.Otkl),
                                new XAttribute("Count", s3.Kol),
                                new XAttribute("SpeedLimit", s3.Ovp != -1 ? s3.Ovp : 0),
                                new XAttribute("Ignore", 0),
                                new XAttribute("Specialization", s3.Tip_poezdki),
                                new XAttribute("Lat", "n"),
                                new XAttribute("Lon", "n"));
                            gOutFile.Add(gInsident);
                            xeOutFile.Add(xeIncident);
                        }
                        
                        xdDeviation.Add(xeOutFile);
                        gParamaters.Add(gOutFile);
                        gParamaters.Save(System.IO.Directory.GetCurrentDirectory() + $"\\export_ALARm\\EKASUI\\G24543_{trip.Trip_date.ToString("dd.MM.yyyy")}_0.xml");
                        xdDeviation.Save(System.IO.Directory.GetCurrentDirectory() + "\\export_ALARm\\EKASUI\\Deviation.xml");
                    }
                }
                catch
                {
                    MetroMessageBox.Show(this, "Ошибка при экспорте \"Отступления по геометрии рельсовой колеи\"", "Экспорт ЕК АСУИ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                try
                {
                    XDocument xdXML = new XDocument();
                    XElement xeOutFile = new XElement("OutFile",
                        new XAttribute("nsiver", DateTime.Now.ToString("dd.MM.yyyy")));
                    List<Bedemost> bedList = RdStructureService.GetBedemost(mainParametersProcessPeriodForm.trip_id) as List<Bedemost>;

                    if (bedList.Count > 0)
                    {
                        XElement xeHeader = new XElement("header",
                        new XAttribute("Manufacture", ""),
                        new XAttribute("PackageID",  trip.Car.Trim() + bedList[0].Put),
                        new XAttribute("PackageNum", "1"),
                        new XAttribute("SiteID", bedList[0].Put),
                        new XAttribute("carID", trip.Car.Trim()),
                        new XAttribute("soft", "ALARmDK"),
                        new XAttribute("runDate", trip.Trip_date.ToString("dd.MM.yyyy")),
                        new XAttribute("decodeDate", trip.Trip_date.ToString("dd.MM.yyyy")),
                        new XAttribute("decoder", trip.Chief.Trim()),
                        new XAttribute("pathType", "1"),
                        new XAttribute("pathID", ""),
                        new XAttribute("pathText", ""));
                        xeOutFile.Add(xeHeader);

                        foreach (Bedemost bedemost in bedList)
                        {
                            XElement xePathPoints = new XElement("PathPoints",
                                new XAttribute("KM", bedemost.Kmtrue),
                                new XAttribute("DLINA_PROV", bedemost.Lkm),
                                new XAttribute("B_M", "0"),
                                new XAttribute("Specialization", bedemost.Tip_poezdki),
                                new XAttribute("BALL", bedemost.Ball),
                                new XAttribute("OZEN", bedemost.Otsenka));

                            xeOutFile.Add(xePathPoints);
                        }

                        xdXML.Add(xeOutFile);
                        xdXML.Save(System.IO.Directory.GetCurrentDirectory() + "\\export_ALARm\\EKASUI\\kmBall.xml");
                    }
                }
                catch
                {
                    MetroMessageBox.Show(this, "Ошибка при экспорте \"Покилометровая балловая оценка\"", "Экспорт ЕК АСУИ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                try
                {
                    XDocument xdXML = new XDocument();
                    XElement xeOutFile = new XElement("OutFile",
                        new XAttribute("nsiver", DateTime.Now.ToString("dd.MM.yyyy")));

                    List<Curve> curveList = RdStructureService.GetCurves(mainParametersProcessPeriodForm.trip_id) as List<Curve>;
                    List<Bedemost> bedList = RdStructureService.GetBedemost(mainParametersProcessPeriodForm.trip_id) as List<Bedemost>;
                    bool bedExists = bedList.Count > 0 ? true : false;
                    bool curveExists = curveList.Count > 0 ? true : false;

                    if ( bedExists && curveExists)
                    {
                        int skm = 0, sm = 0, fkm = 0, fm = 0, len = 0;
                        skm = curveList.OrderBy(tmp => tmp.Start_Km * 1000 + tmp.Start_M).First().Start_Km;
                        sm = curveList.OrderBy(tmp => tmp.Start_Km * 1000 + tmp.Start_M).First().Start_M;
                        fkm = curveList.OrderByDescending(tmp => tmp.Final_Km * 1000 + tmp.Final_M).First().Final_Km;
                        fm = curveList.OrderByDescending(tmp => tmp.Final_Km * 1000 + tmp.Final_M).First().Final_M;
                        len = skm * 1000 + sm - fkm * 1000 - fm;

                        XElement xeHeader = new XElement("header",
                        new XAttribute("Manufacture", ""),
                        new XAttribute("PackageID", trip.Car.Trim() + bedList[0].Put),
                        new XAttribute("PackageNum","1" ),
                        new XAttribute("SiteID",bedList[0].Put),
                        new XAttribute("carID", trip.Car.Trim()),
                        new XAttribute("soft", "ALARmDK"),
                        new XAttribute("runDate", trip.Trip_date.ToString("dd.MM.yyyy")),
                        new XAttribute("decodeDate", trip.Trip_date.ToString("dd.MM.yyyy")),
                        new XAttribute("decoder", trip.Chief.Trim()),
                        new XAttribute("Specialization", bedList[0].Tip_poezdki),
                        new XAttribute("pathType", "1"),
                        new XAttribute("pathID", ""),
                        new XAttribute("pathText", ""),
                        new XAttribute("Dlina_prov", len),
                        new XAttribute("s_KM", skm),
                        new XAttribute("s_M", sm),
                        new XAttribute("e_KM", fkm),
                        new XAttribute("e_M", fm));
                        xeOutFile.Add(xeHeader);

                        foreach (Curve curve in curveList.OrderBy(tmp => tmp.Start_Km * 1000 + tmp.Start_M))
                        {
                            curve.Straightenings =
                                (MainTrackStructureService.GetCurves(curve.Id, MainTrackStructureConst.MtoStCurve) as List<StCurve>).OrderBy(radius => radius.Start_Km * 1000 + radius.Start_M).ToList();
                            curve.Elevations =
                                (MainTrackStructureService.GetCurves(curve.Id, MainTrackStructureConst.MtoElCurve) as List<ElCurve>).OrderBy(radius => radius.Start_Km * 1000 + radius.Start_M).ToList();
                            float Lvl = 0;

                            if (curve.Straightenings.Count < 1)
                                continue;

                            XElement xeCurve = new XElement("curve",
                                new XAttribute("Number_crv", curve.Id),
                                new XAttribute("CurveID", curve.Id),
                                new XAttribute("s_KM", curve.Start_Km),
                                new XAttribute("s_M", curve.Start_M),
                                new XAttribute("e_KM", curve.Final_Km),
                                new XAttribute("e_M", curve.Final_M),
                                new XAttribute("Radius", Convert.ToInt32(curve.Straightenings.Max(s => s.Radius))),
                                new XAttribute("Napr", curve.Side_id - 1),
                                new XAttribute("SpeedLimit_pass", curve.Passspeed),
                                new XAttribute("SpeedLimit_gruz", curve.Freightspeed));

                           

                            xeOutFile.Add(xeCurve);
                        }

                        xdXML.Add(xeOutFile);
                        xdXML.Save(System.IO.Directory.GetCurrentDirectory() + "\\export_ALARm\\EKASUI\\kmBall.xml");
                    }
                }
                catch
                {
                    MetroMessageBox.Show(this, "Ошибка при экспорте \"Боковой износ в кривых участках пути\"", "Экспорт ЕК АСУИ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }

                try
                {
                    XDocument xdXML = new XDocument();
                    XElement xeOutFile = new XElement("OutFile",
                        new XAttribute("nsiver", DateTime.Now.ToString("dd.MM.yyyy")));

                    List<Gaps> gapsList = RdStructureService.GetGaps(mainParametersProcessPeriodForm.trip_id) as List<Gaps>;
                    List<Bedemost> bedList = RdStructureService.GetBedemost(mainParametersProcessPeriodForm.trip_id) as List<Bedemost>;
                    
                    bool bedExists = bedList.Count > 0 ? true : false;
                    bool gapsExists = gapsList.Count > 0 ? true : false;

                    if (bedExists && gapsExists)
                    {
                        XElement xeHeader = new XElement("header",
                        new XAttribute("Manufacture", ""),
                        new XAttribute("PackageID", trip.Car.Trim() + bedList[0].Put),
                        new XAttribute("PackageNum", "1"),
                        new XAttribute("SiteID", bedList[0].Put),
                        new XAttribute("carID", trip.Car.Trim()),
                        new XAttribute("soft", "ALARmDK"),
                        new XAttribute("runDate", trip.Trip_date.ToString("dd.MM.yyyy")),
                        new XAttribute("decodeDate", trip.Trip_date.ToString("dd.MM.yyyy")),
                        new XAttribute("decoder", trip.Chief.Trim()),
                        new XAttribute("pathType", "1"),
                        new XAttribute("pathID", ""),
                        new XAttribute("pathText", ""));
                        xeOutFile.Add(xeHeader);

                        XElement xeGapsList = new XElement("ZazorSet");
                        foreach (Gaps gaps in gapsList)
                        {
                            XElement xeGaps = new XElement("Zazor",
                                new XAttribute("Specialization", "n"),
                                new XAttribute("abs_meter", "n"),
                                new XAttribute("km", gaps.Km),
                                new XAttribute("m", gaps.Meter),
                                new XAttribute("Threadhread", "n"),
                                new XAttribute("Temperature", "n"),
                                new XAttribute("sizeWidth", "n"),
                                new XAttribute("SpeedLimit", "n"),
                                new XAttribute("SpeedOrder", "n"),
                                new XAttribute("Overstep", "n"));
                            xeGapsList.Add(xeGaps);
                        }

                        xeOutFile.Add(xeGapsList);
                        xdXML.Add(xeOutFile);
                        xdXML.Save(System.IO.Directory.GetCurrentDirectory() + "\\export_ALARm\\EKASUI\\kmBall.xml");
                    }
                }
                catch
                {
                    MetroMessageBox.Show(this, "Ошибка при экспорте \"Измеренные стыковые зазоры\"", "Экспорт ЕК АСУИ", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private void tsbAcceptAllRepairProject_Click(object sender, EventArgs e)
        {
            if (repairProjectBindingSource.Count <= 0 || ucTracks.GetDataSource().Count <= 0)
            {
                MetroMessageBox.Show(this, alerts.before_insert, alerts.editing, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            try
            {
                if (!MainTrackStructureService.SetAcceptRepairProject(ucTracks.CurrentUnitId))
                {
                    MetroMessageBox.Show(this, alerts.edit_error, alerts.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            catch
            {
                MetroMessageBox.Show(this, alerts.edit_error, alerts.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void tsbShowAllAcceptRepairProject_Click(object sender, EventArgs e)
        {
            repairProjectBindingSource.DataSource = MainTrackStructureService.GetAcceptRepairProject(ucTracks.CurrentUnitId);
        }

        private void scStationTracks_UnitSelectionChanged(object sender, EventArgs e)
        {
            CleanTables(AdmStructureConst.AdmStationTrack);
            if (scStationTracks.GetDataSource().Count > 0)
            {
                switch (metroTabControl1.SelectedTab.Name)
                {
                    case "mtpSections":
                        SetParentTrack(pcSectionPeriod, MainTrackStructureConst.MtoDistSection);
                        break;
                    case "mtpPdbSections":
                        SetParentTrack(pcPdbSectionPeriod, MainTrackStructureConst.MtoPdbSection);
                        break;
                    case "mtpStationSections":
                        SetParentTrack(pcStationSectionPeriod, MainTrackStructureConst.MtoStationSection);
                        break;
                    case "mtpCheckSection":
                        SetParentTrack(pcCheckSection, MainTrackStructureConst.MtoCheckSection);
                        break;
                    case "mtpRfid":
                        SetParentTrack(pcRfid, MainTrackStructureConst.MtoRFID);
                        break;
                    case "mtpRepairProject":
                        repairProjectBindingSource.Clear();
                        repairProjectBindingSource.DataSource = MainTrackStructureService.GetMtoObjects(ucTracks.CurrentUnitId, MainTrackStructureConst.MtoRepairProject);
                        break;
                    case "mtpTraffic":
                        SetParentTrack(pcTraffic, MainTrackStructureConst.MtoTraffic);
                        break;
                    case "mtpWaycat":
                        SetParentTrack(pcWaycat, MainTrackStructureConst.MtoWaycat);
                        break;
                    case "mtpRefPoint":
                        SetParentTrack(pcRefPoint, MainTrackStructureConst.MtoRefPoint);
                        break;
                    case "mtpProfmarks":
                        SetParentTrack(pcProfmarks, MainTrackStructureConst.MtoProfmarks);
                        break;
                    case "mtpChamJoint":
                        SetParentTrack(pcChamJoint, MainTrackStructureConst.MtoChamJoint);
                        break;
                    case "mtpProfileObject":
                        SetParentTrack(pcProfileObjectPeriod, MainTrackStructureConst.MtoProfileObject);
                        break;
                    case "mtpNonExistKm":
                        SetParentTrack(pcNonExistKm, MainTrackStructureConst.MtoNonExtKm);
                        break;
                    case "mtbLongRails":
                        SetParentTrack(pcLongRailsPeriod, MainTrackStructureConst.MtoLongRails);
                        break;
                    case "mtpRailsSections":
                        SetParentTrack(pcRailsSectionsPeriod, MainTrackStructureConst.MtoRailSection);
                        break;
                    case "mtpCrosstie":
                        SetParentTrack(pcCrosstiePeriod, MainTrackStructureConst.MtoCrossTie);
                        break;
                    case "mtpNonStandart":
                        SetParentTrack(pcNonstandartKmPeriod, MainTrackStructureConst.MtoNonStandard);
                        break;
                    case "mtpTrackClass":
                        SetParentTrack(pcTrackClassPeriod, MainTrackStructureConst.MtoTrackClass);
                        break;
                    case "mtpBraces":
                        SetParentTrack(pcRailsBracesPeriod, MainTrackStructureConst.MtoRailsBrace);
                        break;
                    case "mtpTrackWidth":
                        SetParentTrack(pcNormaPeriod, MainTrackStructureConst.MtoNormaWidth);
                        break;
                    case "mtpRestrictions":
                        SetParentTrack(pcTempSpeedPeriod, MainTrackStructureConst.MtoTempSpeed);
                        break;
                    case "mtpElevations":
                        SetParentTrack(pcElevationPeriod, MainTrackStructureConst.MtoElevation);
                        break;
                    case "mtpSpeeds":
                        SetParentTrack(pcSpeedPeriod, MainTrackStructureConst.MtoSpeed);
                        break;
                    case "mtpCurves":
                        SetParentTrack(pcCurvePeriod, MainTrackStructureConst.MtoCurve);
                        break;
                    case "mtpArtificialConstructions":
                        SetParentTrack(pcArtificialConstructionPeriod, MainTrackStructureConst.MtoArtificialConstruction);
                        break;
                    case "mtpSwitch":
                        SetParentTrack(pcSwitchPeriod, MainTrackStructureConst.MtoSwitch);
                        break;
                    case "mtpStraighteningThread":
                        SetParentTrack(pcStraighteningThreadPeriod, MainTrackStructureConst.MtoStraighteningThread);
                        break;
                    case "mtpCommunication":
                        SetParentTrack(pcCommunicationPeriod, MainTrackStructureConst.MtoCommunication);
                        break;
                    case "mtpCoordinateGNSS":
                        SetParentTrack(pcCoordinateGNSSPeriod, MainTrackStructureConst.MtoCoordinateGNSS);
                        break;
                    case "mtpDefectsEarth":
                        SetParentTrack(pcDefectsEarthPeriod, MainTrackStructureConst.MtoDefectsEarth);
                        break;
                    case "mtpDistanceBetweenTracks":
                        SetParentTrack(pcDistanceBetweenTracksPeriod, MainTrackStructureConst.MtoDistanceBetweenTracks);
                        break;
                    case "mtpDeep":
                        SetParentTrack(pcDeep, MainTrackStructureConst.MtoDeep);
                        break;
                    case "mtpBallast":
                        SetParentTrack(pcBallastType, MainTrackStructureConst.MtoBallastType);
                        break;
                    case "mtpDimension":
                        SetParentTrack(pcDimension, MainTrackStructureConst.MtoDimension);
                        break;
                }
            }
        }

        private void metroTabControl1_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (metroTabControl1.SelectedTab.Name)
            {
                case "mtpSections":
                    SetParentTrack(pcSectionPeriod, MainTrackStructureConst.MtoDistSection);
                    break;
                case "mtpPdbSections":
                    SetParentTrack(pcPdbSectionPeriod, MainTrackStructureConst.MtoPdbSection);
                    break;
                case "mtpStationSections":
                    SetParentTrack(pcStationSectionPeriod, MainTrackStructureConst.MtoStationSection);
                    break;
                case "mtpCheckSection":
                    SetParentTrack(pcCheckSection, MainTrackStructureConst.MtoCheckSection);
                    break;
                case "mtpRfid":
                    SetParentTrack(pcRfid, MainTrackStructureConst.MtoRFID);
                    break;
                case "mtpRepairProject":
                    repairProjectBindingSource.Clear();
                    repairProjectBindingSource.DataSource = MainTrackStructureService.GetMtoObjects(ucTracks.CurrentUnitId, MainTrackStructureConst.MtoRepairProject);
                    break;
                case "mtpTraffic":
                    SetParentTrack(pcTraffic, MainTrackStructureConst.MtoTraffic);
                    break;
                case "mtpWaycat":
                    SetParentTrack(pcWaycat, MainTrackStructureConst.MtoWaycat);
                    break;
                case "mtpRefPoint":
                    SetParentTrack(pcRefPoint, MainTrackStructureConst.MtoRefPoint);
                    break;
                case "mtpProfmarks":
                    SetParentTrack(pcProfmarks, MainTrackStructureConst.MtoProfmarks);
                    break;
                case "mtpChamJoint":
                    SetParentTrack(pcChamJoint, MainTrackStructureConst.MtoChamJoint);
                    break;
                case "mtpProfileObject":
                    SetParentTrack(pcProfileObjectPeriod, MainTrackStructureConst.MtoProfileObject);
                    break;
                case "mtpNonExistKm":
                    SetParentTrack(pcNonExistKm, MainTrackStructureConst.MtoNonExtKm);
                    break;
                case "mtbLongRails":
                    SetParentTrack(pcLongRailsPeriod, MainTrackStructureConst.MtoLongRails);
                    break;
                case "mtpRailsSections":
                    SetParentTrack(pcRailsSectionsPeriod, MainTrackStructureConst.MtoRailSection);
                    break;
                case "mtpCrosstie":
                    SetParentTrack(pcCrosstiePeriod, MainTrackStructureConst.MtoCrossTie);
                    break;
                case "mtpNonStandart":
                    SetParentTrack(pcNonstandartKmPeriod, MainTrackStructureConst.MtoNonStandard);
                    break;
                case "mtpTrackClass":
                    SetParentTrack(pcTrackClassPeriod, MainTrackStructureConst.MtoTrackClass);
                    break;
                case "mtpBraces":
                    SetParentTrack(pcRailsBracesPeriod, MainTrackStructureConst.MtoRailsBrace);
                    break;
                case "mtpTrackWidth":
                    SetParentTrack(pcNormaPeriod, MainTrackStructureConst.MtoNormaWidth);
                    break;
                case "mtpRestrictions":
                    SetParentTrack(pcTempSpeedPeriod, MainTrackStructureConst.MtoTempSpeed);
                    break;
                case "mtpElevations":
                    SetParentTrack(pcElevationPeriod, MainTrackStructureConst.MtoElevation);
                    break;
                case "mtpSpeeds":
                    SetParentTrack(pcSpeedPeriod, MainTrackStructureConst.MtoSpeed);
                    break;
                case "mtpCurves":
                    SetParentTrack(pcCurvePeriod, MainTrackStructureConst.MtoCurve);
                    break;
                case "mtpArtificialConstructions":
                    SetParentTrack(pcArtificialConstructionPeriod, MainTrackStructureConst.MtoArtificialConstruction);
                    break;
                case "mtpSwitch":
                    SetParentTrack(pcSwitchPeriod, MainTrackStructureConst.MtoSwitch);
                    break;
                case "mtpStraighteningThread":
                    SetParentTrack(pcStraighteningThreadPeriod, MainTrackStructureConst.MtoStraighteningThread);
                    break;
                case "mtpCommunication":
                    SetParentTrack(pcCommunicationPeriod, MainTrackStructureConst.MtoCommunication);
                    break;
                case "mtpCoordinateGNSS":
                    SetParentTrack(pcCoordinateGNSSPeriod, MainTrackStructureConst.MtoCoordinateGNSS);
                    break;
                case "mtpDefectsEarth":
                    SetParentTrack(pcDefectsEarthPeriod, MainTrackStructureConst.MtoDefectsEarth);
                    break;
                case "mtpDistanceBetweenTracks":
                    SetParentTrack(pcDistanceBetweenTracksPeriod, MainTrackStructureConst.MtoDistanceBetweenTracks);
                    break;
                case "mtpDeep":
                    SetParentTrack(pcDeep, MainTrackStructureConst.MtoDeep);
                    break;
                case "mtpBallast":
                    SetParentTrack(pcBallastType, MainTrackStructureConst.MtoBallastType);
                    break;
                case "mtpDimension":
                    SetParentTrack(pcDimension, MainTrackStructureConst.MtoDimension);
                    break;
            }
        }

        private void фрагментыToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {

        }

        private void импортXMLToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            folderBrowserDialog.ShowNewFolderButton = false;
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(folderBrowserDialog.SelectedPath);
                FileInfo[] fileInfo = directoryInfo.GetFiles("*.xml", SearchOption.TopDirectoryOnly);
                if (fileInfo.Length == 0)
                {
                    MessageBox.Show("Файлы для импорта отсутствуют", "Импорт XML", MessageBoxButtons.OK);
                }
                else
                {
                    using (var importForm = new ImportForm())
                    {
                        importForm.ShowDialog();
                        if (importForm.Result == DialogResult.Cancel)
                            return;

                        var road = (AdmRoad)AdmStructureService.Insert(importForm.admRoad);
                        if (road.Id < 1)
                        {
                            MetroMessageBox.Show(this, alerts.insert_error, alerts.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        importForm.admNod.Parent_Id = road.Id;
                        var nod = (AdmNod)AdmStructureService.Insert(importForm.admNod);
                        if (nod.Id < 1)
                        {
                            MetroMessageBox.Show(this, alerts.insert_error, alerts.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                            return;
                        }

                        this.Enabled = false;
                        if (!ImportAndExport.Import(fileInfo, progressBar, 0, road.Id, nod.Id, importForm.Period))
                            MetroMessageBox.Show(this, alerts.import_error);
                        else
                            MetroMessageBox.Show(this, alerts.success);
                        progressBar.Value = 0;
                        this.Enabled = true;
                        ucRoads.SetDataSource(AdmStructureService.GetUnits(AdmStructureConst.AdmRoad, -1));
                        ucRoads.Refresh();
                    }
                }
            }
        }

        private void импортCSVToolStripMenuItem_Click(object sender, EventArgs e)
        {
            FolderBrowserDialog folderBrowserDialog = new FolderBrowserDialog();
            folderBrowserDialog.ShowNewFolderButton = false;
            if (folderBrowserDialog.ShowDialog() == DialogResult.OK)
            {
                DirectoryInfo directoryInfo = new DirectoryInfo(folderBrowserDialog.SelectedPath);
                FileInfo[] fileInfo = directoryInfo.GetFiles("*.csv", SearchOption.TopDirectoryOnly);
                if (fileInfo.Length == 0)
                {
                    MessageBox.Show("Файлы для импорта отсутствуют", "Импорт CSV", MessageBoxButtons.OK);
                }
                else
                {
                    this.Enabled = false;
                    if (!ImportAndExport.Import(fileInfo, progressBar, 1))
                        MetroMessageBox.Show(this, alerts.import_error);
                    else
                        MetroMessageBox.Show(this, alerts.success);
                    progressBar.Value = 0;
                    this.Enabled = true;
                    ucRoads.SetDataSource(AdmStructureService.GetUnits(AdmStructureConst.AdmRoad, -1));
                    ucRoads.Refresh();
                }
            }
        }

        private void menuStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            timer1.Enabled = false;
            RdStructureService.RunRvoDataInsert();
            timer1.Enabled = true;

        }

        private void записьВидеоДанныхToolStripMenuItem_Click(object sender, EventArgs e)
        {
            timer1.Enabled = true;
        }

        private void остановитьЗаписьВидеоданныхToolStripMenuItem_Click(object sender, EventArgs e)
        {
            timer1.Enabled = false;
        }

        private void metroGrid6_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}
