using Accord.Math;
using ALARm.Core;
using ALARm.Core.AdditionalParameteres;
using ALARm.Core.Report;
using ALARm.Services;
using ALARm_Report.controls;
using MetroFramework.Controls;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Xsl;
using Side = ALARm.Core.AdditionalParameteres.Side;

namespace ALARm_Report.Forms
{
    public class CrossSectionRail : Report
    {
        public override void Process(Int64 distanceId, ReportTemplate template, ReportPeriod period, MetroProgressBar progressBar)
        {
            XDocument htReport = new XDocument();
          
            using (XmlWriter writer = htReport.CreateWriter())
            {
                //Сделать выбор периода
                List<long> admTracksId = new List<long>();
                using (var choiceForm = new ChoiseForm(0))
                {
                    choiceForm.SetTripsDataSource(distanceId, period);
                    choiceForm.ShowDialog();
                    if (choiceForm.dialogResult == DialogResult.Cancel)
                        return;
                    admTracksId = choiceForm.admTracksIDs;
                }

                XDocument xdReport = new XDocument();
                XElement report = new XElement("report");

                var distance = AdmStructureService.GetUnit(AdmStructureConst.AdmDistance, distanceId) as AdmUnit;
                var road = AdmStructureService.GetRoadName(distance.Id, AdmStructureConst.AdmDistance, true);

                var mainProcesses = RdStructureService.GetMainParametersProcess(period, distance.Code);
                if (mainProcesses.Count == 0)
                {
                    MessageBox.Show(Properties.Resources.paramDataMissing);
                    return;
                }

                //var distance = AdmStructureService.GetUnit(AdmStructureConst.AdmDistance, distanceId) as AdmUnit;

                foreach (var process in mainProcesses)
                {
                    foreach(var track_id in admTracksId)
                    {
                        //var rd_curves_wear_great_13 = (RdStructureService.GetRdTables(process, 9) as List<RDCurve>).Where(r => r.Wear >= 13.0).ToList();
                        var trackName = AdmStructureService.GetTrackName(track_id);
                        var trip = RdStructureService.GetTrip(process.Id);
                        var kms = RdStructureService.GetKilometersByTrip(trip);
                        kms = kms.Where(o => o.Track_id == track_id).ToList();
                        if (kms.Count() == 0) continue;

                        var lkm = kms.Select(o => o.Number).ToList();
                        if (lkm.Count() == 0) continue;
                        var lm = kms.Select(o => o.Number).ToList();
                        if (lkm.Count() == 0) continue;

                        if (kms == null || kms.Count == 0)
                            continue;

                        var filterForm = new FilterForm();
                        var filters = new List<Filter>();

                        filters.Add(new FloatFilter() { Name = " (км)", Value = lkm.Min() });
                        filters.Add(new FloatFilter() { Name = "(метер)", Value = lm.Min() });
                        //if (rd_curves_wear_great_13.Count < 1)
                        //    continue;
                        filterForm.SetDataSource(filters);
                        if (filterForm.ShowDialog() == DialogResult.Cancel)
                            return;
                        var Blazor_prov_data = new AppData();
                        
                        Blazor_prov_data.conn = new NpgsqlConnection(Blazor_prov_data.cs);
                        Blazor_prov_data.conn.Open();
                        Blazor_prov_data.in_koridor = new BinaryReader(File.Open(Blazor_prov_data.Vnutr__profil__koridor, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
                        var data = Blazor_prov_data.in_koridor.ReadBytes(8);
                        Blazor_prov_data.in_koridor_count = BitConverter.ToSingle(data, 0);
                        Blazor_prov_data.in_koridor_size = BitConverter.ToSingle(data, 4);

                        Blazor_prov_data.in_kupe = new BinaryReader(File.Open(Blazor_prov_data.Vnutr__profil__kupe, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
                        data = Blazor_prov_data.in_kupe.ReadBytes(8);
                        Blazor_prov_data.in_kupe_count = BitConverter.ToSingle(data, 0);
                        Blazor_prov_data.in_kupe_size = BitConverter.ToSingle(data, 4);
                        Blazor_prov_data.GetBitmapAsync(700,800);

                        XElement xePages = new XElement("pages",

                            new XAttribute("version", $"{DateTime.Now} v{Assembly.GetExecutingAssembly().GetName().Version.ToString()}"),
                            new XAttribute("car", process.Car),
                            new XAttribute("chief", process.Chief),
                            new XAttribute("ps", process.Car),
                            new XAttribute("check", process.GetProcessTypeName),
                            new XAttribute("period", period.Period),
                          
                            new XAttribute("pointsleft", Blazor_prov_data.PointsLeft.ToString("0.00").Replace(",", ".")),
                            new XAttribute("pointsright", Blazor_prov_data.PointsRight.ToString("0.00").Replace(",", ".")),
                            new XAttribute("railspoints", " 61.9, 400, 25, 84.5623876, 35.6, 180, 71.25, 33.28728189, 17.31756333, 375.0000105"),
                            //X_lef
                            new XAttribute("xelem1", Blazor_prov_data.Xtest1.ToString("0.00").Replace(",", ".")),
                            new XAttribute("xelem2", Blazor_prov_data.Xtest2.ToString("0.00").Replace(",", ".")),
                            new XAttribute("xelem3", Blazor_prov_data.Xtest3.ToString("0.00").Replace(",", ".")),
                            new XAttribute("xelem3_pu_l", Blazor_prov_data.pu_l.First().ToString("0.00")),
                            new XAttribute("xelem3_npk_l", Blazor_prov_data.npk_l.First().ToString("0.00")),
                            new XAttribute("xelem3_bok_iz_l", Blazor_prov_data.bok_l.First().ToString("0.00")),
                            new XAttribute("xelem3_pu_r", Blazor_prov_data.pu_r.First().ToString("0.00")),
                            new XAttribute("xelem3_npk_r", Blazor_prov_data.npk_r.First().ToString("0.00")),
                            new XAttribute("xelem3_bok_iz_r", Blazor_prov_data.bok_r.First().ToString("0.00")),
                            new XAttribute("xelem4", Blazor_prov_data.Xtest4.ToString("0.00").Replace(",", ".")),
                            new XAttribute("x_big", Blazor_prov_data.Xtest5.ToString("0.00").Replace(",", ".")),
                            new XAttribute("xrad1", Blazor_prov_data.Xrad.ToString("0.00").Replace(",", ".")),
                            new XAttribute("bottom_x1", Blazor_prov_data.Bottom_x1.ToString("0.00").Replace(",", ".")),
                            new XAttribute("bottom_x2", Blazor_prov_data.Bottom_x1.ToString("0.00").Replace(",", ".")),
                            new XAttribute("x_per2_l", Blazor_prov_data.Perpen_x2_l.ToString("0.00").Replace(",", ".")),
                            new XAttribute("radX_l", Blazor_prov_data.RadX_l.ToString("0.00").Replace(",", ".")),
                            ////
                            new XAttribute("yelem1", Blazor_prov_data.Ytest1.ToString("0.00").Replace(",", ".")),
                            new XAttribute("yelem2", Blazor_prov_data.Ytest2.ToString("0.00").Replace(",", ".")),
                            new XAttribute("yelem3", Blazor_prov_data.Ytest3.ToString("0.00").Replace(",", ".")),
                            new XAttribute("yelem4", Blazor_prov_data.Ytest4.ToString("0.00").Replace(",", ".")),
                            new XAttribute("y_big", Blazor_prov_data.Ytest_big.ToString("0.00").Replace(",", ".")),
                            new XAttribute("yrad1", Blazor_prov_data.Ytest5.ToString("0.00").Replace(",", ".")),
                            new XAttribute("bottom_y1", Blazor_prov_data.Bottom_y1.ToString("0.00").Replace(",", ".")),
                            new XAttribute("bottom_y2", Blazor_prov_data.Bottom_y2.ToString("0.00").Replace(",", ".")),
                            new XAttribute("y_per2_l", Blazor_prov_data.Perpen_y2_l.ToString("0.00").Replace(",", ".")),
                            new XAttribute("radY_l", Blazor_prov_data.RadY_l.ToString("0.00").Replace(",", ".")),
                            ////x_right
                            new XAttribute("xelem1_r", Blazor_prov_data.Xtest1_r.ToString("0.00").Replace(",", ".")),
                            new XAttribute("xelem2_r", Blazor_prov_data.Xtest2_r.ToString("0.00").Replace(",", ".")),
                            new XAttribute("xelem3_r", Blazor_prov_data.Xtest3_r.ToString("0.00").Replace(",", ".")),
                            new XAttribute("xelem4_r", Blazor_prov_data.Xtest4_r.ToString("0.00").Replace(",", ".")),
                            new XAttribute("x_big_r", Blazor_prov_data.X_big_r.First().ToString("0.00").Replace(",", ".")),
                            new XAttribute("xrad1_r", Blazor_prov_data.Xtest5_r.ToString("0.00").Replace(",", ".")),
                            new XAttribute("bottom_x1_r", Blazor_prov_data.Bottom_x2_r.ToString("0.00").Replace(",", ".")),
                            new XAttribute("bottom_x2_r", Blazor_prov_data.Bottom_x1_r.ToString("0.00").Replace(",", ".")),
                            new XAttribute("x_per2_l_r", Blazor_prov_data.Perpen_x1_r.ToString("0.00").Replace(",", ".")),
                            new XAttribute("radX_l_r", Blazor_prov_data.RadX_r.ToString("0.00").Replace(",", ".")),
                            ////
                            new XAttribute("yelem1_r", Blazor_prov_data.Ytest1_r.ToString("0.00").Replace(",", ".")),
                            new XAttribute("yelem2_r", Blazor_prov_data.Ytest2_r.ToString("0.00").Replace(",", ".")),
                            new XAttribute("yelem3_r", Blazor_prov_data.Ytest3_r.ToString("0.00").Replace(",", ".")),
                            new XAttribute("yelem4_r", Blazor_prov_data.Ytest4_r.ToString("0.00").Replace(",", ".")),
                            new XAttribute("y_big_r", Blazor_prov_data.Ytest_big_r.ToString("0.00").Replace(",", ".")),
                            new XAttribute("yrad1_r", Blazor_prov_data.Ytest5_r.ToString("0.00").Replace(",", ".")),
                            new XAttribute("bottom_y1_r", Blazor_prov_data.Bottom_y1_r.ToString("0.00").Replace(",", ".")),
                            new XAttribute("bottom_y2_r", Blazor_prov_data.Bottom_y2_r.ToString("0.00").Replace(",", ".")),
                            new XAttribute("y_per2_l_r", Blazor_prov_data.Perpen_y1_r.ToString("0.00").Replace(",", ".")),
                            new XAttribute("radY_l_r", Blazor_prov_data.RadY_r.ToString("0.00").Replace(",", ".")),


                            new XAttribute("distance", distance.Code),
                            new XAttribute("direction", process.DirectionName + " (" + process.DirectionCode + ")"),
                            new XAttribute("track", trackName));

                        for(int i = 0; i < Blazor_prov_data.PointsLeft.Length - 1; i = i + 2)
                         {
                            if (!Double.IsNaN(Blazor_prov_data.PointsLeft[i]) && !Double.IsNaN(Blazor_prov_data.PointsLeft[i + 1]))
                            {
                                XElement PointsLeft = new XElement("pointsLeft",
                                new XAttribute("cx", Blazor_prov_data.PointsLeft[i].ToString().Replace(",", ".")),
                                new XAttribute("cy", Blazor_prov_data.PointsLeft[i + 1].ToString().Replace(",", ".")));
                                xePages.Add(PointsLeft);
                            }
                            
                        }
                        for (int j = 0; j < Blazor_prov_data.PointsRight.Length - 1; j = j + 2)
                        {
                            if (!Double.IsNaN(Blazor_prov_data.PointsRight[j]) && !Double.IsNaN(Blazor_prov_data.PointsLeft[j + 1]))
                            {
                                XElement PointsRight = new XElement("pointsRight",
                                 new XAttribute("cx_r", Blazor_prov_data.PointsRight[j].ToString().Replace(",", ".")),
                                 new XAttribute("cy_r", Blazor_prov_data.PointsRight[j + 1].ToString().Replace(",", ".")));
                                xePages.Add(PointsRight);
                            }
                        }

                        report.Add(xePages);
                    }
                }

                xdReport.Add(report);

                XslCompiledTransform transform = new XslCompiledTransform();
                transform.Load(XmlReader.Create(new StringReader(template.Xsl)));
                transform.Transform(xdReport.CreateReader(), writer);
            }
            try
            {
                htReport.Save(Path.GetTempPath() + "/report_CrossSectionRail.html");
            }
            catch
            {
                MessageBox.Show("Ошибка сохранения файлы");
            }
            finally
            {
                System.Diagnostics.Process.Start(Path.GetTempPath() + "/report_CrossSectionRail.html");
            }
        }
    }
    public class AppData
    {
        //public int CurrentFrameIndex { get; set; } = 5475;
        public int CurrentFrameIndex { get; set; } = 0;
        public int Speed { get; set; } = 100;
        public bool Processing = true;
        public int Kilometer { get; set; } = -1;
        public int Meter { get; set; } = -1;
        public int Picket { get; set; } = -1;
        public string DataLeft { get; set; }
        public string DataRight { get; set; }
        public double[] PointsLeft { get; set; }
        public double[] Railpoint { get; set; }
        public double Xtest_big { get; set; }
        public double Ytest_big { get; set; }
        public double Xtest_big_r { get; set; }
        public double Ytest_big_r { get; set; }
        public double Xtest1 { get; set; } //шейка 2 нукте лев
        public double Ytest1 { get; set; }
        public double Xtest2 { get; set; }
        public double Ytest2 { get; set; }
        public double Xtest1_r { get; set; } // шейка 2 нукте прав
        public double Ytest1_r { get; set; }
        public double Xtest2_r { get; set; }
        public double Ytest2_r { get; set; }
        public double X_r { get; set; } //orange шейка

        public double Xtest3 { get; set; } //головка 2 нукте лев
        public double Ytest3 { get; set; }
        public double Xtest4 { get; set; }
        public double Ytest4 { get; set; }
        public double Xtest3_r { get; set; } //головка 2 нукте  прав
        public double Ytest3_r { get; set; }
        public double Xtest4_r { get; set; }
        public double Ytest4_r { get; set; }
        public double Xtest5 { get; set; }
        public double Ytest5 { get; set; }
        public double Xtest5_r { get; set; }
        public double Ytest5_r { get; set; }
        public double Bottom_x1 { get; set; }
        public double Bottom_x2 { get; set; }
        public double Bottom_y1 { get; set; }
        public double Bottom_y2 { get; set; }
        public double Bottom_x1_r { get; set; }
        public double Bottom_x2_r { get; set; }
        public double Perpen_x1_r { get; set; }
        public double Perpen_y1_r { get; set; }
        public double Perpen_x2_l { get; set; }
        public double Perpen_y2_l { get; set; }
        public double RadX_l { get; set; }
        public double RadY_l { get; set; }
        public double RadX_r { get; set; }
        public double RadY_r { get; set; }
        public double Bottom_y1_r { get; set; }
        public double Bottom_y2_r { get; set; }
        public double Xrad { get; set; }

        internal int top_kupe_size;

        public double Yrad { get; set; }
        public double[] PointsRight { get; set; }
        public double[] PointsX { get; set; }
        public double[] PointsY { get; set; }
        public string NominalRailScheme { get; set; }
        public string NominalTranslateLeft { get; set; } = "-10px,-10px";
        public string NominalTranslateRight { get; set; }
        public string NominalRotateLeft { get; set; } = "0deg";
        public string NominalRotateRight { get; set; }
        public string ViewBox { get; set; }
        public string ViewBoxPoverhLeft { get; set; }
        public string ViewBoxPoverhRight { get; set; }
        public List<double> VertWearLeft { get; set; } = new List<double>();
        public List<double> VertWearRight { get; set; } = new List<double>();
        public List<double> SideWearLeft { get; set; } = new List<double>();
        public List<double> SideWearRight { get; set; } = new List<double>();
        public List<double> Wear45Left { get; set; } = new List<double>();
        public List<double> Wear45Right { get; set; } = new List<double>();
        public List<double> DownhillLeft { get; set; } = new List<double>();
        public List<double> DownhillRight { get; set; } = new List<double>();
        public List<double> Rad_arr { get; set; } = new List<double>();
        public List<double> D_arr { get; set; } = new List<double>();
        public List<double> D_arr_small { get; set; } = new List<double>();
        public List<double> fiArr { get; set; } = new List<double>(); //пу л
        public List<double> fiArr_r { get; set; } = new List<double>();//пу пр
        public List<double> pu_l { get; set; } = new List<double>(); //пу л
        public List<double> pu_r { get; set; } = new List<double>();//пу пр
        public List<double> razn_arr { get; set; } = new List<double>();//верт
        public List<double> razn_arr_r { get; set; } = new List<double>();
        public List<double> vert_l { get; set; } = new List<double>();//верт
        public List<double> vert_r { get; set; } = new List<double>();
        public List<double> npk_aArr { get; set; } = new List<double>();//нпк л
        public List<double> npk_aArr_r { get; set; } = new List<double>();//нпк л
        public List<double> npk_l { get; set; } = new List<double>();//нпк л
        public List<double> npk_r { get; set; } = new List<double>();//нпк л
        public List<double> bok_iz_Arr { get; set; } = new List<double>(); //иб л
        public List<double> bok_iz_Arr_r { get; set; } = new List<double>();//иб пр
        public List<double> bok_l { get; set; } = new List<double>(); //иб л
        public List<double> bok_r { get; set; } = new List<double>();//иб пр
        public List<double> iz_45 { get; set; } = new List<double>();
        public List<double> iz_45_r { get; set; } = new List<double>();
        public List<double> iz45_l { get; set; } = new List<double>();
        public List<double> iz45_r { get; set; } = new List<double>();
        public List<double> X_big_l { get; set; } = new List<double>();
        public List<double> X_big_r { get; set; } = new List<double>();

        public List<double> SlopeSkatingSurfaceLeft { get; set; } = new List<double>(); //НПК
        public List<double> SlopeSkatingSurfaceRight { get; set; } = new List<double>(); //НПК
        public List<int> Meters { get; set; } = new List<int>();
        public List<int> Kms { get; set; } = new List<int>();
        public string ViewBoxLeft { get; set; }
        public string ViewBoxRight { get; set; }
        public List<int> Dy = new List<int>();
        public int CalibrConstLeft { get; set; } = 0;
        public int CalibrConstRight { get; set; } = 0;
        public int CalibrConstRight1 { get; set; } = 0;
        public int ScaleCoef = 1000;
        public int WearCoef = 20;
        double s0 = 0;
        double s1 = 0;
        public int LeftWavesIndex { get; set; } = 0;
        public int RightWavesIndex { get; set; } = 0;
        public List<double> ShortWavesLeft { get; set; } = new List<double>();
        public List<double> ShortWavesRight { get; set; } = new List<double>();
        public List<double> MediumWavesLeft { get; set; } = new List<double>();
        public List<double> MediumWavesRight { get; set; } = new List<double>();
        public List<double> LongWavesLeft { get; set; } = new List<double>();
        public List<double> LongWavesRight { get; set; } = new List<double>();

        //Для записи скольз среднее
        public List<double> RollAverWrite_pu_l { get; set; } = new List<double>();
        public List<double> RollAverWrite_pu_r { get; set; } = new List<double>();

        public List<double> RollAverWrite_npk_l { get; set; } = new List<double>();
        public List<double> RollAverWrite_npk_r { get; set; } = new List<double>();
        //Для записи экспонента
        public List<double> pu_l_ra { get; set; } = new List<double>();
        public List<double> pu_r_ra { get; set; } = new List<double>();

        public List<double> npk_l_ra { get; set; } = new List<double>();
        public List<double> npk_r_ra { get; set; } = new List<double>();

        public List<double> SWavesLeft { get; set; } = new List<double>();
        public List<double> SWavesRight { get; set; } = new List<double>();
        public List<double> MWavesLeft { get; set; } = new List<double>();
        public List<double> MWavesRight { get; set; } = new List<double>();
        public List<double> LWavesLeft { get; set; } = new List<double>();
        public List<double> LWavesRight { get; set; } = new List<double>();



        public string file_id_L { get; private set; }
        public string trip_id_L { get; set; }
        public string side_id_L { get; set; }
        public string file_id_R { get; private set; }
        public string trip_id_R { get; set; }
        public string side_id_R { get; set; }
        public string Grad { get; set; }
        public static Bitmap FrameImgLeft { get; set; }
        public static Bitmap FrameImgRight { get; set; }
        public double FirstMinX { get; set; }
        public double FirstMaxX { get; set; }

        private bool QueryData = false;
        public double ExponentCoef = -1;

        public int width = 150;
        public string cs = "Host=DESKTOP-EMAFC5J;Username=postgres;Password=alhafizu;Database=Aniyar_COpy";
        //public string cs = "Host=localhost;Username=postgres;Password=alhafizu;Database=railway";
        public NpgsqlConnection conn { get; set; }

        //public string Vnutr__profil__koridor = @"C:\sntfi\ProfileProject\213_Vnutr__profil__koridor_2020_10_26_10_38_24.Profile_Calibr";
        //public string Vnutr__profil__kupe = @"C:\sntfi\ProfileProject\213_Vnutr__profil__kupe_2020_10_26_10_38_24.Profile_Calibr";

        //public string Vnutr__profil__kupe = @"F:\o59m\ShablonKORR\Profile_Calibr\242_Vnutr__profil__koridor_2021_10_18_16_43_58.Profile_Calibr";
        //public string Vnutr__profil__koridor = @"F:\o59m\ShablonKUPE\Profile_Calibr\242_Vnutr__profil__kupe_2021_10_18_16_43_58.Profile_Calibr";

        public string Vnutr__profil__kupe = @"\\DESKTOP-EMAFC5J\o59m\ShablonKORR\Profile_Calibr\242_Vnutr__profil__koridor_2021_10_18_16_43_58.Profile_Calibr";
        public string Vnutr__profil__koridor = @"\\DESKTOP-EMAFC5J\o59m\ShablonKUPE\Profile_Calibr\242_Vnutr__profil__kupe_2021_10_18_16_43_58.Profile_Calibr";


        //public string Vnutr__profil__kupe = @"F:\o59m\ShablonKORR\Profile_Calibr\228_Vnutr__profil__koridor_2021_10_16_14_29_54.Profile_Calibr";
        //public string Vnutr__profil__koridor = @"F:\o59m\ShablonKUPE\Profile_Calibr\228_Vnutr__profil__kupe_2021_10_16_14_29_54.Profile_Calibr";
        //public string Vnutr__profil__koridor = @"D:\data\158 _Vnutr__profil__koridor_2020_07_23_18_11_33.Profile_Calibr";
        //public string Vnutr__profil__kupe = @"D:\data\158 _Vnutr__profil__kupe_2020_07_23_18_11_33.Profile_Calibr";
        public BinaryReader in_koridor { get; set; }
        public BinaryReader in_kupe { get; set; }
        public BinaryReader top_koridor { get; set; }
        public BinaryReader top_kupe { get; set; }
        public string GetPoints(List<double> list)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < list.Count; i++)
            {
                sb.Append($"{list[i].ToString("0.00").Replace(",", ".")},{i} ");
            }
            return sb.ToString();
        }
        public string GetPointsAngle(List<double> list)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < list.Count; i++)
            {
                sb.Append($"{(Math.Tan(DegreeToRadian(list[i])) * ScaleCoef).ToString("0.00").Replace(",", ".")},{i} ");
            }
            return sb.ToString();
        }
        public string GetPointsWear(List<double> list)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < list.Count; i++)
            {
                sb.Append($"{(list[i] * WearCoef).ToString("0.00").Replace(",", ".")},{i} ");
            }
            return sb.ToString();
        }

        public double DegreeToRadian(double angle)
        {
            return Math.PI * angle / 180.0;
        }
        public double RadianToDegree(double radian)
        {
            return radian * 180.0 / Math.PI;
        }
        public Bitmap CurrentFrame { get; set; }

        public void CurrentProfileLeft(string filePath, int kmnum, int metr)
        {
            BinaryReader reader = in_koridor;
            {
                try
                {
                    while (!(Kilometer == kmnum && Meter == metr))
                    {
                        CurrentFrameIndex++;
                        long pos = CurrentFrameIndex * ((long)in_koridor_size) + 8;
                        reader.BaseStream.Seek(pos, SeekOrigin.Begin);
                        reader.ReadBytes(32);
                        var buff = reader.ReadBytes(4);
                        Array.Reverse(buff);
                        Kilometer = BitConverter.ToInt32(buff, 0);
                        buff = reader.ReadBytes(4);
                        Array.Reverse(buff);
                        Meter = BitConverter.ToInt32(buff, 0);
                    }
                    
                    //long ll = CurrentFrameIndex * ((long)in_koridor_size) + 8;
                    //reader.BaseStream.Seek(ll, SeekOrigin.Begin);
                    //var data = reader.ReadBytes(4);
                    //Array.Reverse(data);
                    //var U32EncoderCounter_1 = BitConverter.ToUInt32(data, 0);
                    //var Speed = reader.ReadInt32();
                    //var TimeStamp = reader.ReadDouble();
                    //var U32EncoderCounter_3 = reader.ReadInt32();
                    //var U32CadrCouter = reader.ReadInt32();
                    //var camtime = reader.ReadUInt64();
                    //data = reader.ReadBytes(4);
                    //Array.Reverse(data);
                    //Kilometer = BitConverter.ToInt32(data);
                    //data = reader.ReadBytes(4);
                    //Array.Reverse(data);
                    //Meter = BitConverter.ToInt32(data);
                    Picket = Meter / 100 + 1;


                    //Console.WriteLine($"---Профиль лев: КМ {Kilometer} М {Meter} U32EncoderCounter_1 { U32EncoderCounter_1 }");

                    List<double> arrX = new List<double>();
                    List<double> arrY = new List<double>();
                    List<double> vector = new List<double>();
                    string viewBox = String.Empty;
                    for (int i = 0; i < ((int)in_koridor_size - 40) / 8; i++)
                    {
                        double x = reader.ReadSingle();
                        double y = reader.ReadSingle();

                        if (!Double.IsNaN(x) && !Double.IsNaN(y) && x < 250)
                        {
                            arrX.Add(x);
                            arrY.Add(y);
                        }
                    }
                    FirstMinX = arrX.Min();
                    FirstMaxX = arrX.Max();

                    PointsX = arrX.ToArray();
                    PointsY = arrY.ToArray();

                    if (CurrentFrameIndex == 6833)
                    {
                        var n = 2;
                    }
                    CalcProfile(Rails.r65, arrX, arrY, Side.Left, ref vector, ref viewBox);

                    PointsLeft = vector.ToArray();
                    ViewBoxLeft = viewBox;
                    
                    Kms.Add(Kilometer);
                    Meters.Add(Meter);
                }
                catch (Exception e)
                {
                    System.Console.WriteLine(e.Message);
                    PointsLeft = Array.Empty<double>();
                    ViewBoxLeft = "0 0 0 0";
                }
            }
        }

        public void CurrentProfileRight(string filePath)
        {
            BinaryReader reader = in_kupe;
            {
                try
                {
                    long ll = CurrentFrameIndex * (long)in_kupe_size + 8;
                    reader.BaseStream.Seek(ll, SeekOrigin.Begin);
                    var encoderCounter = reader.ReadUInt64();
                    var timestamp = reader.ReadInt64();
                    var frameNumber = reader.ReadUInt64();
                    var camtime = reader.ReadUInt64();
                    var kilometer = reader.ReadInt32();
                    var meter = reader.ReadInt32();

                    //Console.WriteLine($"Профиль пр: КМ {kilometer} М {meter} encoderCounter {encoderCounter}");

                    List<double> arrX = new List<double>();
                    List<double> arrY = new List<double>();
                    List<double> vector = new List<double>();
                    string viewBox = String.Empty;
                    for (int i = 0; i < ((int)in_kupe_size - 40) / 8; i++)
                    {
                        try
                        {
                            double x = reader.ReadSingle();
                            double y = reader.ReadSingle();
                            if (!Double.IsNaN(x) && !Double.IsNaN(y) && x < 250 && y < 135)
                            {
                                arrX.Add(x);
                                arrY.Add(y);
                            }
                        }
                        catch (Exception)
                        {

                            throw;
                        }
                    }
                    CalcProfile(Rails.r65, arrX, arrY, Side.Right, ref vector, ref viewBox);

                    PointsRight = vector.ToArray();
                    ViewBoxRight = viewBox;
                }
                catch (Exception e)
                {
                    System.Console.WriteLine(e.Message);
                    PointsRight = Array.Empty<double>();
                    ViewBoxRight = "0 0 0 0";
                }
            }
        }


        int startP = -1;
        bool ctbl = false;
        bool period_l = false;
        bool period_r = false;
        double d_vert_left = 0;
        double d_vert_rigt = 0;

        public List<int> OFnum = new List<int>();
        public List<int> OKm = new List<int>();
        public List<int> OMeter = new List<int>();

        public async Task GetBitmapAsync(int number, int meter)
        {
            GC.Collect();
            try
            {
                for (int index = 0; index < 1; index++)
                {
                    CurrentFrameIndex++;
                    try
                    {
                        if (period_l == false)
                        {
                            var fname_splited = Vnutr__profil__koridor.Split('\\');
                            var fname = Vnutr__profil__koridor.Split('\\')[fname_splited.Length - 1];

                            if (conn.State == System.Data.ConnectionState.Closed)
                                conn.Open();
                            var cmd = new NpgsqlCommand();
                            cmd.Connection = conn;

                            cmd.CommandText = $@"SELECT
	                                                * 
                                                FROM
	                                                PUBLIC.trip_files 
                                                WHERE
	                                                file_name LIKE '%{fname}'";
                            NpgsqlDataReader rdr = cmd.ExecuteReader();
                            if (rdr.FieldCount > 0)
                            {
                                while (rdr.Read())
                                {
                                    file_id_L = rdr.GetInt32(0).ToString();
                                    trip_id_L = rdr.GetInt32(3).ToString();
                                    side_id_L = rdr.GetInt32(6).ToString();
                                }
                                period_l = true;
                            }
                            else
                            {
                                file_id_L = "-1";
                                trip_id_L = "-1";
                                side_id_L = "0";
                                period_l = true;
                            }
                            conn.Close();
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Не удалось определить период профиль лев" + e.Message);
                        file_id_L = "-1";
                        trip_id_L = "-1";
                        side_id_L = "0";
                        period_l = true;
                    }
                    try
                    {
                        if (period_r == false)
                        {
                            var fname_splited = Vnutr__profil__koridor.Split('\\');
                            var fname = Vnutr__profil__koridor.Split('\\')[fname_splited.Length - 1];


                            if (conn.State == System.Data.ConnectionState.Closed)
                                conn.Open();
                            var cmd = new NpgsqlCommand();
                            cmd.Connection = conn;

                            cmd.CommandText = @"SELECT * FROM public.trip_files
                                                where file_name  like '%" + fname + "'";
                            NpgsqlDataReader rdr = cmd.ExecuteReader();

                            if (rdr.FieldCount > 0)
                            {
                                while (rdr.Read())
                                {
                                    file_id_R = rdr.GetInt32(0).ToString();
                                    trip_id_R = rdr.GetInt32(3).ToString();
                                    side_id_R = rdr.GetInt32(6).ToString();
                                }
                                period_r = true;
                            }
                            else
                            {
                                file_id_R = "-1";
                                trip_id_R = "-1";
                                side_id_R = "0";
                                period_r = true;
                            }
                            conn.Close();
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Не удалось определить период профиль пр" + e.Message);
                        file_id_R = "-1";
                        trip_id_R = "-1";
                        side_id_R = "0";
                        period_r = true;
                    }
                    //запрашиваем объекты с базы координаты болтов
                    //try
                    //{
                    //    if (QueryData == false)
                    //    {
                    //        if (conn.State == System.Data.ConnectionState.Closed)
                    //            conn.Open();
                    //        var cmd = new NpgsqlCommand();
                    //        cmd.Connection = conn;
                    //        cmd.CommandText = $@"
                    //                        SELECT km, mtr FROM rd_video_objects_{trip_id_L}
                    //                        where oid in (0,1,2,5,6,7,8)
                    //                        group by km, mtr
                    //                        order by km, mtr";
                    //        NpgsqlDataReader rdr = cmd.ExecuteReader();
                    //        if (rdr.FieldCount > 0)
                    //        {
                    //            while (rdr.Read())
                    //            {
                    //                OKm.Add(rdr.GetInt32(0));
                    //                OMeter.Add(rdr.GetInt32(1));
                    //            }
                    //            QueryData = true;
                    //        }
                    //        conn.Close();
                    //    }
                    //}
                    //catch
                    //{
                    //}

                    CurrentProfileLeft(Vnutr__profil__koridor, number, meter);
                    CurrentProfileRight(Vnutr__profil__kupe);

                    var Pkt = (Meter - 1) / 100 + 1;

                    startP = startP == -1 ? Pkt : startP;

                    int startMeter = -1;
                    var pu_l_m = new List<double> { };
                    var pu_r_m = new List<double> { };
                    var vert_l_m = new List<double> { };
                    var vert_r_m = new List<double> { };
                    var bok_l_m = new List<double> { };
                    var bok_r_m = new List<double> { };
                    var npk_l_m = new List<double> { };
                    var npk_r_m = new List<double> { };
                    var iz45_l_m = new List<double> { };
                    var iz45_r_m = new List<double> { };
                    var x_big_l = new List<double> { };
                    var x_big_r = new List<double> { };


                    
                  
                }
                CurrentFrame = FrameImgLeft;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                //CurrentFrameIndex = -1;
                Processing = false;
            }
        }
        /// <summary>
        /// Возвращает индексы максимальных элементов каждого столбца
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns>индексы максимальных элементов</returns>
        private static int[] GetIndexesOfColumnsMax(int[,] matrix)
        {
            int width = matrix.Columns();
            int[] vect = new int[width];
            for (int i = 0; i < width; i++)
            {
                var column = matrix.GetColumn(i);
                vect[i] = column.IndexOf(column.Max());
            }
            return vect;
        }
        /// <summary>
        /// Возвращает индексы максимальных элементов каждого столбца
        /// </summary>
        /// <param name="matrix"></param>
        /// <returns>индексы максимальных элементов</returns>
        private static int[] GetIndexesOfColumnsMax(double[,] result)
        {
            int width = result.Columns();
            int[] vect = new int[width];
            for (int i = 0; i < width; i++)
            {
                var column = result.GetColumn(i);
                vect[i] = column.IndexOf(column.Max());
            }
            return vect;
        }

        static int[,] ConvertMatrix(int[] flat, int m, int n)
        {
            if (flat.Length != m * n)
            {
                throw new ArgumentException("Invalid length");
            }
            int[,] ret = new int[m, n];
            // BlockCopy uses byte lengths: a double is 8 bytes
            Buffer.BlockCopy(flat, 0, ret, 0, flat.Length * sizeof(Int32));
            return ret;
        }

        public string CoordinateTostring()
        {
            //GetBitmapAsync(CurrentFrameIndex);
            return $"Километр: {Kilometer} Пикет: {Picket} Метр: {Meter} Текущий кадр: ";

        }
        public string VectorToPoints(int[] vector, string side)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < vector.Length; i++)
            {
                sb.Append($"{i},{vector[i]} ");
            }
            ViewBox = $"0 {vector.Min() - 5} {vector.Length} {vector.Max() + 5}";
            return sb.ToString();
        }
        public string VectorToPoints(double[] vector, string sidePoverh, long encoderCounter_1, int km, int meter)
        {
            int xIn50 = -1;
            int xOut50 = -1;
            int xIn = -1;
            int xOut = -1;
            bool flagIn = false;
            List<double> Promezhutok = new List<double>();

            int vecMAxInd = vector.IndexOf(vector.Max());
            int vecMinInd = vector.IndexOf(vector.Min());

            xIn50 = vecMAxInd - 50 > -1 ? vecMAxInd - 50 : 0;
            xOut50 = vecMAxInd + 50 > vector.Count() - 1 ? vector.Count() - 1 : vecMAxInd + 50;

            for (int i = 0; i <= vector.Count() - 1; i++)
            {
                if (xIn50 <= i && xOut50 >= i)
                {
                    var fn = ((vector[xIn50] + vector[xOut50]) / 2 + (vector.Max() - (vector[xIn50] + vector[xOut50]) / 2) / 3);
                    if (fn > vector[i] && flagIn == false && 0.90 * vector.Max() > (vector[xIn50] + vector[xOut50]) / 2)
                    {
                        xIn = xIn == -1 ? i : xIn;
                        flagIn = true;
                        xOut = -1;
                    }

                    if (fn < vector[i] && flagIn == true)
                    {
                        xOut = xOut == -1 ? i : xOut;
                        flagIn = false;
                        //Console.WriteLine($" Max {xOut} {xIn} Gap_len:" + (xOut - xIn));
                        if (Math.Abs(xOut - xIn) <= 20)
                        {
                            
                            xIn = -1;
                        }
                        else
                        {
                            var PromCount = 0;
                            var PromezhMax = Promezhutok.Max();
                            for (int j = 0; j <= Promezhutok.Count() - 1; j++)
                            {
                                if (Promezhutok[j] > PromezhMax * 0.95)
                                {
                                    PromCount++;
                                }
                            }
                            
                        }
                    }
                    if (flagIn == true)
                    {
                        Promezhutok.Add(vector[i]);
                    }
                }
            }

            int xInmin = -1;
            int xOutmin = -1;
            bool flagInmin = false;
            Promezhutok.Clear();

            xIn50 = vecMinInd - 50 > -1 ? vecMinInd - 50 : 0;
            xOut50 = vecMinInd + 50 > vector.Count() - 1 ? vector.Count() - 1 : vecMinInd + 50;
            for (int i = 0; i <= vector.Count() - 1; i++)
            {
                if (xIn50 <= i && xOut50 >= i)
                {
                    var fn = ((vector[xIn50] + vector[xOut50]) / 2 + (vector.Min() - (vector[xIn50] + vector[xOut50]) / 2) / 3);
                    if (fn > vector[i] && flagInmin == false && 0.90 * vector.Min() > (vector[xIn50] + vector[xOut50]) / 2)
                    {
                        xInmin = xInmin == -1 ? i : xInmin;
                        flagInmin = true;
                        xOutmin = -1;
                    }
                    if (fn < vector[i] && flagInmin == true)
                    {
                        xOutmin = xOutmin == -1 ? i : xOutmin;
                        flagInmin = false;
                        //Console.WriteLine($" Min {xOutmin} {xInmin} Gap_len:" + (xOutmin - xInmin));
                        if (Math.Abs(xOutmin - xInmin) <= 20)
                        {
                            
                            xIn = -1;
                        }
                        else
                        {
                            var PromCount = 0;
                            var PromezhMin = Promezhutok.Min();
                            for (int j = 0; j <= Promezhutok.Count() - 1; j++)
                            {
                                if (Promezhutok[j] < PromezhMin * 0.95)
                                {
                                    PromCount++;
                                }
                            }
                            
                        }
                    }
                    if (flagIn == true)
                    {
                        Promezhutok.Add(vector[i]);
                    }
                }
            }

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < vector.Length; i++)
            {
                var st = $"{vector[i]}".Replace(",", ".");
                sb.Append($"{i},{st} ");
            }
            var minc = $"{vector.Min() - 5}".Replace(",", ".");
            var maxc = $"{vector.Max() + 5}".Replace(",", ".");
            //ViewBox = $"0 {minc} {vector.Length} {maxc}";

            if (sidePoverh == "Left")
            {
                ViewBoxPoverhLeft = $"0 {minc} {vector.Length} {maxc}";
            }
            else
            {
                ViewBoxPoverhRight = $"0 {minc} {vector.Length} {maxc}";
            }
            return sb.ToString();
        }

       

        public string VectorToPoints(float[] vector)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < vector.Length; i = i + 2)
            {
                sb.Append($"{vector[i]},{vector[i + 1]} ");
            }
            ViewBox = $"0 0 {vector.Max()} {vector.Max() }";
            return sb.ToString();
        }
        /// <summary>
        ///  Расчет профиля головки рельса
        /// </summary>
        /// <param name="rails">тип рельсы</param>
        /// <param name="arrX">координаты головки по оси х</param>
        /// <param name="arrY">координаты головки по оси Y</param>
        /// <param name="side">рельсовая нить</param>
        /// <param name="vector">массив координат</param>
        List<double> ARR_y = new List<double>();
        List<double> ARR_x = new List<double>();
        internal float in_koridor_count;
        internal float in_kupe_size;
        internal float in_koridor_size;
        internal float in_kupe_count;
        internal float top_koridor_count;
        internal int top_koridor_size;
        internal int top_kupe_count;
        internal int top_koridor_width;
        internal int top_koridor_height;
        internal int top_kupe_width;
        internal int top_kupe_height;

        public string is_45_r_str;
        public string is_45_l_str;
        public string LongWavesRight_str;
        public string MediumWavesRight_str;
        public string ShortWavesRight_str;
        public string LongWavesLeft_str;
        public string MediumWavesLeft_str;
        public string ShortWavesLeft_str;
        public string npk_r_str;
        public string npk_l_str;
        public string bok_iz_Arr_r_str;
        public string bok_iz_Arr_l_str;
        public string vert_r_str;
        public string vert_l_str;
        public string pu_r_str;
        public string pu_l_str;

        public void CalcProfile(Rails rails, List<double> arrX, List<double> arrY, Side side, ref List<double> vector, ref string viewBox)
        {
            double vbMinX, vbMinY, vbX, vbY, trX, trY, rotate;

            ProfileCalcParameter calcParam;
            switch (rails)
            {
                case Rails.r75:
                    calcParam = new ProfileCalcParameter(61.5, 450, 25, 85.1668859, 46, 192, 81.4, 29.3486179, 17.4545352, 425.0000933);
                    break;
                case Rails.r65:
                    calcParam = new ProfileCalcParameter(61.9, 400, 25, 84.5623876, 35.6, 180, 71.25, 33.28728189, 17.31756333, 375.0000105);
                    break;
                case Rails.r50:
                    calcParam = new ProfileCalcParameter(50.5, 350, 20, 85.0600398, 33, 152, 52.75, 29.92824734, 17.4493911, 330.0000048);
                    break;
                case Rails.r43:
                    calcParam = new ProfileCalcParameter(41.1, 350, 15, 85.176281, 30.4, 140, 56.75, 34.72074314, 16.49536762, 335.0000848);
                    break;
                default:
                    return;
            }
           

            double maxY = arrY.Max();
            arrY = arrY.Select(a => a * (-1) + maxY).ToList();

            List<double> sideX = new List<double>(), sideY = new List<double>(), headX = new List<double>(), headY = new List<double>();
            try
            {
                TryReadProfile(ref sideX, ref sideY, ref headX, ref headY, arrX.ToArray(), arrY.ToArray(), calcParam.HeadCoef, calcParam.BottomSideCoef, calcParam.TopSideCoef);
            }
            catch (Exception)
            {

                throw;
            }

            double[] arrSideX = sideX.ToArray();
            double[] arrSideY = sideY.ToArray();
            double[] arrHeadX = headX.ToArray();
            double[] arrHeadY = headY.ToArray();

            if (arrSideX.Length < 1 || arrSideY.Length < 1 || arrHeadX.Length < 1 || arrHeadY.Length < 1)
            {
                if (side == Side.Left)
                {
                    //pu_l.Add(-1);
                    //vert_l.Add(-1);
                    //bok_l.Add(-1);
                    //npk_l.Add(-1);
                    //SWavesLeft.Add(-1);
                    //MWavesLeft.Add(-1);
                    //LWavesLeft.Add(-1);

                    pu_l.Add(0.05);
                    vert_l.Add(0);
                    bok_l.Add(0);
                    npk_l.Add(3.0 / 45.0);
                    SWavesLeft.Add(0);
                    MWavesLeft.Add(0);
                    LWavesLeft.Add(0);
                    X_big_l.Add(0);
                    X_big_r.Add(0);
                }
                else
                {
                    //pu_r.Add(-1);
                    //vert_r.Add(-1);
                    //bok_r.Add(-1);
                    //npk_r.Add(-1);
                    //SWavesRight.Add(-1);
                    //MWavesRight.Add(-1);
                    //LWavesRight.Add(-1);
                    pu_r.Add(0.05);
                    vert_r.Add(0.05);
                    bok_r.Add(0.05);
                    npk_r.Add(3.0 / 45.0);
                    SWavesRight.Add(0);
                    MWavesRight.Add(0);
                    LWavesRight.Add(0);
                    X_big_l.Add(0);
                    X_big_r.Add(0);
                }
                return;
            }

            double maxX = Math.Max(arrHeadX.Max(), arrSideX.Max());
            arrHeadX = arrHeadX.Select(a => a * (-1) + maxX).ToArray();
            arrSideX = arrSideX.Select(a => a * (-1) + maxX).ToArray();

            for (int i = 0; i < arrHeadX.Length; i++)
            {
                if (side == Side.Left)
                    vector.Add(arrHeadX[i]);
                else
                    vector.Add(arrHeadX[i] * (-1) + Math.Max(arrHeadX.Max(), arrSideX.Max()));

                vector.Add(arrHeadY[i] * (-1) + Math.Max(arrHeadY.Max(), arrSideY.Max()));
            }

            for (int i = 0; i < arrSideX.Length; i++)
            {
                if (side == Side.Left)
                    vector.Add(arrSideX[i]);
                else
                    vector.Add(arrSideX[i] * (-1) + Math.Max(arrHeadX.Max(), arrSideX.Max()));

                vector.Add(arrSideY[i] * (-1) + Math.Max(arrHeadY.Max(), arrSideY.Max()));
            }

            if (arrSideY.First() < arrSideY.Last())
            {
                Array.Reverse(arrSideX);
                Array.Reverse(arrSideY);
            }

            double headDownY = arrHeadY.Min();
            double toDown = headDownY - calcParam.DownParam;
            int toDownIndex = -1;
            for (int i = 0; i < arrSideY.Length; i++)
            {
                if (arrSideY[i] < toDown)
                {
                    toDownIndex = i;
                    break;
                }
            }
            //---Табаны-----------------------------------------------------------
            List<double> sideX1 = new List<double>(), sideY1 = new List<double>();
            List<double> sideX2 = new List<double>(), sideY2 = new List<double>();
            var side_count = arrSideY.Length / 3;

            if (side == side)
            {
                var level_side_count = side_count / 3;

                try
                {
                    for (int i = 0; i <= side_count; i++)
                    {
                        if (i < level_side_count)
                        {
                            sideX1.Add(arrSideX[i]);
                            sideY1.Add(arrSideY[i]);
                        }
                        if (i > level_side_count * 2)
                        {
                            sideX2.Add(arrSideX[i]);
                            sideY2.Add(arrSideY[i]);
                        }
                    }
                }
                catch (Exception)
                {

                    throw;
                }

                //шейка 2 нукте лев
                if (side == Side.Left)
                {
                    try
                    {
                        var x1min = sideX1.Min();
                        var y1min = sideY1[sideX1.IndexOf(x1min)];

                        var x2min = sideX2.Min();
                        var y2min = sideY2[sideX2.IndexOf(x2min)];

                        Xtest1 = x1min; //green
                        Ytest1 = y1min * (-1) + Math.Max(arrHeadY.Max(), arrSideY.Max());
                        Xtest2 = x2min; //red
                        Ytest2 = y2min * (-1) + Math.Max(arrHeadY.Max(), arrSideY.Max());
                        //X_big_l = x1min; //orange
                        //Ytest1 = y1min * (-1) + Math.Max(arrHeadY.Max(), arrSideY.Max());
                        //X_big_r = x1min; //orange
                        //Ytest1 = y1min * (-1) + Math.Max(arrHeadY.Max(), arrSideY.Max());

                        X_big_l.Add(Xtest1);

                    }
                    catch (Exception)
                    {

                        throw;
                    }
                }
                //шейка 2 нукте прав
                if (side == Side.Right)
                {
                    try
                    {
                        var x1min = sideX1.Min();
                        var y1min = sideY1[sideX1.IndexOf(x1min)];

                        var x2min = sideX2.Min();
                        var y2min = sideY2[sideX2.IndexOf(x2min)];

                        var x_big_1min_l = sideX1.Min();
                        var x_big_1min_r = sideX2.Min();
                        Xtest1_r = x1min * (-1) + Math.Max(arrHeadX.Max(), arrSideX.Max());
                        Ytest1_r = y1min * (-1) + Math.Max(arrHeadY.Max(), arrSideY.Max());
                        Xtest2_r = x2min * (-1) + Math.Max(arrHeadX.Max(), arrSideX.Max());
                        Ytest2_r = y2min * (-1) + Math.Max(arrHeadY.Max(), arrSideY.Max());
                        X_big_r.Add(Xtest1_r);

                    }
                    catch (Exception)
                    {

                        throw;
                    }
                }
            }
            //----головка---------------------------------------------------------
            List<double> headX1 = new List<double>(), headY1 = new List<double>();
            List<double> headX2 = new List<double>(), headY2 = new List<double>();
            var head_count = arrHeadY.Length / 3;
            var level_head_count = head_count / 3;

            if (side == Side.Left)
            {
                try
                {
                    //Array.Reverse(arrHeadX);
                    //Array.Reverse(arrHeadY);
                    for (int i = 0; i <= head_count; i++)
                    {
                        if (i < level_head_count)
                        {
                            headX1.Add(arrHeadX[i]);
                            headY1.Add(arrHeadY[i]);
                        }
                        if (i > level_head_count * 2)
                        {
                            headX2.Add(arrHeadX[i]);
                            headY2.Add(arrHeadY[i]);
                        }
                    }
                    var x1min = headX1[headY1.IndexOf(headY1.Min())];
                    var y1min = headY1.Min();

                    var x2min = headX2[headY2.IndexOf(headY2.Min())];
                    var y2min = headY2.Min();


                    var vremX1 = new List<double> { };
                    var vremY1 = new List<double> { };
                    var vremX2 = new List<double> { };
                    var vremY2 = new List<double> { };

                    //g
                    var startI = headY1.IndexOf(headY1.Min());
                    var finalI = (headY1.Count < headY1.IndexOf(headY1.Min()) + 7 ? headY1.Count : headY1.IndexOf(headY1.Min()) + 7);

                    for (int i = startI; i < finalI; i++)
                    {
                        vremX1.Add(headX1[i]);
                        vremY1.Add(headY1[i]);
                    }
                    //r
                    var a = (0 > headY2.IndexOf(headY2.Min()) - 7 ? 0 : headY2.IndexOf(headY2.Min()) - 7);
                    var b = (0 == headY2.IndexOf(headY2.Min()) ? headY2.Count : headY2.IndexOf(headY2.Min()));

                    for (int i = a; i < b; i++)
                    {
                        vremX2.Add(headX2[i]);
                        vremY2.Add(headY2[i]);
                    }

                    //тобедегы 2 нукте
                    //Xtest3 = vremX1.Average(); //g
                    //Ytest3 = vremY1.Average() * (-1) + Math.Max(arrHeadY.Max(), arrSideY.Max());
                    //Xtest4 = vremX2.Average(); //r
                    //Ytest4 = vremY2.Average() * (-1) + Math.Max(arrHeadY.Max(), arrSideY.Max());
                }
                catch (Exception)
                {

                    throw;
                }
            }
            if (side == Side.Right)
            {
                try
                {
                    //Array.Reverse(arrHeadX);
                    //Array.Reverse(arrHeadY);
                    for (int i = 0; i <= arrHeadY.Length - 1; i++)
                    {
                        if (i > head_count * 2 && i < head_count * 2 + level_head_count)
                        {
                            headX1.Add(arrHeadX[i]);
                            headY1.Add(arrHeadY[i]);
                        }
                        if (i >= head_count * 2 + level_head_count * 2)
                        {
                            headX2.Add(arrHeadX[i]);
                            headY2.Add(arrHeadY[i]);
                        }
                    }
                    var x2min = headX1[headY1.IndexOf(headY1.Min())];
                    var y2min = headY1.Min();

                    var x1min = headX2[headY2.IndexOf(headY2.Min())];
                    var y1min = headY2.Min();

                    var vremX1 = new List<double> { };
                    var vremY1 = new List<double> { };
                    var vremX2 = new List<double> { };
                    var vremY2 = new List<double> { };

                    //g
                    for (int i = headY1.IndexOf(headY1.Min()); i < headY1.IndexOf(headY1.Min()) + 7; i++)
                    {
                        vremX1.Add(headX1[i]);
                        vremY1.Add(headY1[i]);
                    }
                    //r
                    for (int i = headY2.IndexOf(headY2.Min()); i < headY2.IndexOf(headY2.Min()) + 7; i++)
                    {
                        vremX2.Add(headX2[i]);
                        vremY2.Add(headY2[i]);
                    }

                    //тобедегы 2 нукте
                    Xtest3_r = vremX2.Average() * (-1) + Math.Max(arrHeadX.Max(), arrSideX.Max());
                    Ytest3_r = vremY2.Average() * (-1) + Math.Max(arrHeadY.Max(), arrSideY.Max());
                    Xtest4_r = vremX1.Average() * (-1) + Math.Max(arrHeadX.Max(), arrSideX.Max());
                    Ytest4_r = vremY1.Average() * (-1) + Math.Max(arrHeadY.Max(), arrSideY.Max());
                }
                catch (Exception)
                {

                    throw;
                }
            }

            double xrb = 0, yrb = 0;
            double delta = -1;
            if (side == Side.Left)
            {

                try
                {
                    GetCenterCoords(ref xrb, ref yrb, Xtest1, Ytest1, Xtest2, Ytest2, calcParam.Radius);
                }
                catch (Exception)
                {

                    throw;
                }
            }
            if (side == Side.Right)
            {
                try
                {
                    GetCenterCoords(ref xrb, ref yrb, Xtest1_r, Ytest1_r, Xtest2_r, Ytest2_r, calcParam.Radius);
                }
                catch (Exception)
                {

                    throw;
                }
            }
            if (side == Side.Left)
            {
                Xtest_big = arrSideX[arrSideY.IndexOf(arrSideY.Max())];
                Ytest_big = arrSideY.Max() * (-1) + Math.Max(arrHeadY.Max(), arrSideY.Max());

            }
            if (side == Side.Right)
            {
                Xtest_big_r = arrSideX[arrSideY.IndexOf(arrSideY.Max())] * (-1) + Math.Max(arrHeadX.Max(), arrSideX.Max());
                Ytest_big_r = arrSideY.Max() * (-1) + Math.Max(arrHeadY.Max(), arrSideY.Max());
            }
            //разница головки мак у - мин у
            double vert_elem = 0;

            //----От точки до линий------------------------------------------------
            try
            {
                //var A = Ytest4 - Ytest3;
                //var B = Xtest3 - Xtest4;
                //var C = -Xtest3 * Ytest4 + Xtest4 * Ytest3;

                //var d = Math.Abs(A * xrb + B * yrb + C) / Math.Sqrt(Math.Pow(A, 2) + Math.Pow(B, 2));

                //if (side == Side.Left)
                //{
                //    if (Math.Abs(d - 103) <= 4)
                //    {
                //        D_arr.Add(d - 103);
                //    }
                //    else
                //    {
                //        D_arr.Add(0);
                //    }
                //}
            }
            catch (Exception)
            {

                throw;
            }

            //боковой из----->>>>>>>>>>>
            double bok_elem = 0;
            if (side == Side.Left)
            {
                double down13x = 0, down13y = 0;

                var x2mincc = headX2[headY2.IndexOf(headY2.Min())];
                var y2mincc = headY2.Min();

                var golovka_x2_index = arrHeadX[arrHeadY.IndexOf(headY1.Min())];
                var golovka_y2_index = arrHeadY[arrHeadY.IndexOf(headY1.Min())];

                var prog = golovka_y2_index - 13;

                for (int i = 0; i <= arrHeadY.Count(); i++)
                {
                    try
                    {
                        if (prog >= arrHeadY[i])
                        {
                            down13x = arrHeadX[i];
                            down13y = arrHeadY[i];
                            break;
                        }
                    }
                    catch (Exception)
                    {

                        throw;
                    }
                }
                Xtest5 = down13x;
                Ytest5 = down13y * (-1) + Math.Max(arrHeadY.Max(), arrSideY.Max());
                bok_elem = arrHeadX.Max() - down13x;

                List<double> RolAver = new List<double>();
                if (bok_iz_Arr.Count() >= width)
                {
                    RolAver = bok_iz_Arr.GetRange(bok_iz_Arr.Count - width, width);
                    var e = Math.Exp(ExponentCoef * Math.Abs(bok_elem - RolAver.Average()));
                    bok_elem = RolAver.Average() + (bok_elem - RolAver.Average()) * e;
                }
                else
                {
                    var e = Math.Exp(ExponentCoef * Math.Abs(bok_elem - (bok_iz_Arr.Count() <= 0 ? bok_elem : bok_iz_Arr.Average())));
                    bok_elem = (bok_iz_Arr.Count() <= 0 ? bok_elem : bok_iz_Arr.Average()) + (bok_elem - (bok_iz_Arr.Count() <= 0 ? bok_elem : bok_iz_Arr.Average())) * e;
                }


                bok_iz_Arr.Add(bok_elem);
                bok_iz_Arr_l_str = bok_iz_Arr_l_str + $"{((bok_elem) * WearCoef).ToString("0.00").Replace(",", ".")},{CurrentFrameIndex} ";
                bok_l.Add(bok_elem);
            }
            if (side == Side.Right)
            {
                double down13x = 0, down13y = 0;
                var golovka_x2_index = arrHeadX[arrHeadY.IndexOf(headY1.Min())];
                var golovka_y2_index = arrHeadY[arrHeadY.IndexOf(headY1.Min())];

                var prog = golovka_y2_index - 13;

                for (int i = 0; i <= arrHeadY.Count(); i++)
                {
                    try
                    {
                        if (prog <= arrHeadY[i])
                        {
                            down13x = arrHeadX[i];
                            down13y = arrHeadY[i];
                            break;
                        }
                    }
                    catch (Exception)
                    {

                        throw;
                    }
                }
                Xtest5_r = down13x * (-1) + Math.Max(arrHeadX.Max(), arrSideX.Max());
                Ytest5_r = down13y * (-1) + Math.Max(arrHeadY.Max(), arrSideY.Max());
                bok_elem = arrHeadX.Max() - down13x - 0.0;


                List<double> RolAver = new List<double>();
                if (bok_iz_Arr_r.Count() >= width)
                {
                    RolAver = bok_iz_Arr_r.GetRange(bok_iz_Arr_r.Count - width, width);
                    var e = Math.Exp(ExponentCoef * Math.Abs(bok_elem - RolAver.Average()));
                    bok_elem = RolAver.Average() + (bok_elem - RolAver.Average()) * e;
                }
                else
                {
                    var e = Math.Exp(ExponentCoef * Math.Abs(bok_elem - (bok_iz_Arr_r.Count() <= 0 ? bok_elem : bok_iz_Arr_r.Average())));
                    bok_elem = (bok_iz_Arr_r.Count() <= 0 ? bok_elem : bok_iz_Arr_r.Average()) + (bok_elem - (bok_iz_Arr_r.Count() <= 0 ? bok_elem : bok_iz_Arr_r.Average())) * e;
                }

                bok_iz_Arr_r.Add(bok_elem);
                bok_iz_Arr_r_str = bok_iz_Arr_r_str + $"{((bok_elem) * WearCoef).ToString("0.00").Replace(",", ".")},{CurrentFrameIndex} ";
                bok_r.Add(bok_elem);

            }

            //---Верт износ от нижних опорных точек-------Side.Left---------------------------
            if (side == Side.Left)
            {
                var golovXmax = arrHeadX.Max();
                var sts = false;

                for (int i = 0; i <= (arrSideX.Count() - 1); i++)
                {
                    try
                    {
                        if (golovXmax - 15 < arrSideX[i] && sts == false)
                        {
                            Bottom_x1 = arrSideX[i];
                            Bottom_y1 = arrSideY[i] * (-1) + Math.Max(arrHeadY.Max(), arrSideY.Max());
                            sts = true;

                            //среднее по линии
                            var vremX1 = new List<double> { };
                            var vremY1 = new List<double> { };

                            if (i >= 5)
                            {
                                for (int ii = i - 5; ii < i; ii++)
                                {
                                    vremX1.Add(arrSideX[ii]);
                                    vremY1.Add(arrSideY[ii]);
                                }
                            }
                            else
                            {
                                if (i + 5 < arrSideX.Count())
                                {
                                    for (int ii = i; ii < i + 5; ii++)
                                    {
                                        vremX1.Add(arrSideX[ii]);
                                        vremY1.Add(arrSideY[ii]);
                                    }
                                }
                            }

                            Bottom_x1 = vremX1.Average();
                            Bottom_y1 = vremY1.Average() * (-1) + Math.Max(arrHeadY.Max(), arrSideY.Max());
                        }
                    }
                    catch (Exception)
                    {

                        throw;
                    }


                    try
                    {
                        if (Bottom_x1 + 15 < arrSideX[i] && sts == true)
                        {
                            Bottom_x2 = arrSideX[i];
                            Bottom_y2 = arrSideY[i] * (-1) + Math.Max(arrHeadY.Max(), arrSideY.Max());

                            //среднее по линии
                            var vremX2 = new List<double> { };
                            var vremY2 = new List<double> { };

                            if (i + 3 < arrSideY.Count() && i - 3 > 0)
                            {
                                for (int ii = i - 3; ii < i + 3; ii++)
                                {
                                    vremX2.Add(arrSideX[ii]);
                                    vremY2.Add(arrSideY[ii]);
                                }

                                Bottom_x2 = vremX2.Average();
                                Bottom_y2 = vremY2.Average() * (-1) + Math.Max(arrHeadY.Max(), arrSideY.Max());

                                break;
                            }

                        }
                    }
                    catch (Exception)
                    {

                        throw;
                    }

                }

                //----------------------------------------------------------------------------------------------------------------------------------------------

                List<double> DList = new List<double>();
                double aver = 0.0;

                try
                {

                    var A1 = 1 / (Bottom_x2 - Bottom_x1);
                    var B1 = -1 / (Bottom_y2 - Bottom_y1);
                    var C1 = -(Bottom_x1 / (Bottom_x2 - Bottom_x1)) + (Bottom_y1 / (Bottom_y2 - Bottom_y1));

                    if (razn_arr.Count() >= width)
                    {
                        aver = razn_arr.GetRange(razn_arr.Count - width, width).Average();
                    }
                    else
                    {
                        aver = razn_arr.Count() > 0 ? razn_arr.Average() : 0;
                    }

                    for (int i = 0; i < arrHeadY.Count() - 1; i++)
                    {
                        var Dr1 = Math.Abs(A1 * (arrHeadX[i]) + B1 * (arrHeadY[i] * (-1) + Math.Max(arrHeadY.Max(), arrSideY.Max())) + C1) / Math.Sqrt(Math.Pow(A1, 2) + Math.Pow(B1, 2));
                        DList.Add(Dr1);
                    }
                }
                catch (Exception)
                {

                    throw;
                }



                var Dmin = DList.Max() - 145.5;
                //--------------------------
                var bott_y = 0.0;

                List<double> CenterPoinArrX = new List<double>();
                List<double> CenterPoinArrY = new List<double>();



                try
                {
                    for (int i = 0; i <= (arrSideX.Count() - 1); i++)
                    {
                        var y = ((arrSideX[i] - Bottom_x1) / (Bottom_x2 - Bottom_x1)) * Bottom_y2 - ((arrSideX[i] - Bottom_x2) / (Bottom_x2 - Bottom_x1)) * Bottom_y1;
                        var s = Math.Abs(y - (arrSideY[i] * (-1) + Math.Max(arrHeadY.Max(), arrSideY.Max())));

                        double x1 = 0.0, x2 = 0.0, y1 = 0.0, y2 = 0.0, radius = 25;
                        if (i >= 40)
                        {
                            x1 = arrSideX[i - 10];
                            x2 = arrSideX[i - 40];

                            y1 = arrSideY[i - 10];
                            y2 = arrSideY[i - 40];


                            double Ar = (x1 * x1 - x2 * x2 + y1 * y1 - y2 * y2) / (y1 * 2.0 - y2 * 2.0);
                            double Br = (x2 - x1) / (y1 - y2);

                            double alpha = 1 + Br * Br;
                            double beta = 2 * Ar * Br - 2 * x1 - 2 * y1 * Br;
                            double gamma = x1 * x1 + y1 * y1 - radius * radius - 2 * y1 * Ar + Ar * Ar;

                            double D = beta * beta - 4 * alpha * gamma;

                            if (D > 0)
                            {
                                double x01 = (-beta + Math.Sqrt(D)) / (2 * alpha);
                                double x02 = (-beta - Math.Sqrt(D)) / (2 * alpha);
                                double y01 = Ar + Br * x01;
                                double y02 = Ar + Br * x02;

                                CenterPoinArrX.Add(x01);
                                CenterPoinArrY.Add(y01);
                            }
                            else
                            {
                                //Console.WriteLine($"VERT = null");
                            }
                        }

                        bott_y = arrSideY[i];
                        if (s < 0.5)
                            break;
                    }
                }
                catch (Exception)
                {

                    throw;
                }





                try
                {
                    var PointAvgX = CenterPoinArrX.Count != 0 ? CenterPoinArrX.Skip(CenterPoinArrX.Count() - 5).Take(5).Average() : 0;
                    var PointAvgY = CenterPoinArrY.Count != 0 ? CenterPoinArrY.Skip(CenterPoinArrY.Count() - 5).Take(5).Average() * (-1) + Math.Max(arrHeadY.Max(), arrSideY.Max()) : 0;

                    RadX_l = PointAvgX;
                    RadY_l = PointAvgY;

                    //астыга 18 пк тусемыз
                    var golovka_x2_l = arrHeadX[DList.IndexOf(DList.Max())];
                    var golovka_y2_l = arrHeadY[DList.IndexOf(DList.Max())] - 18;
                    var Yminus18 = 0.0;
                    var Xminus18 = 0.0;

                    for (int i = 0; i <= (arrHeadY.Count() - 1); i++)
                    {
                        if (arrHeadY[i] <= golovka_y2_l)
                        {
                            Yminus18 = arrHeadY[i];
                            Xminus18 = arrHeadX[i];
                            break;
                        }
                    }
                    //артка 20px жылжимыз
                    var Yminus20 = 0.0;
                    var Xminus20 = 0.0;
                    var inrX = arrHeadX.IndexOf(Xminus18);
                    for (int i = inrX; i >= 0; i--)
                    {
                        if (arrHeadX[i] <= Xminus18 - 20)
                        {
                            Yminus20 = arrHeadY[i];
                            Xminus20 = arrHeadX[i];
                            break;
                        }
                    }
                    Perpen_x2_l = Xminus20;
                    Perpen_y2_l = arrHeadY.Skip(arrHeadY.IndexOf(Yminus20) - 20).Take(20).Average() * (-1) + Math.Max(arrHeadY.Max(), arrSideY.Max());

                    Dmin = Math.Abs(Perpen_y2_l - RadY_l) - 130 - 2.5;
                    Dmin = /*Math.Abs((double)(A1 / B1)+0.2) > 0.05  ||*/ /*golovka_x2_l < 20 ||*/ Bottom_y2 < Bottom_y1 || (Bottom_x2 - Bottom_x1) < 8 ? (razn_arr.Count() > 0 ? aver : 0.0) : Dmin;
                }
                catch (Exception)
                {

                    throw;
                }

                if (razn_arr.Count() >= width)
                {
                    var ra = razn_arr.Skip(razn_arr.Count() - width).Take(width).Average();
                    var e = Math.Exp(ExponentCoef * 2 * Math.Abs(Dmin - ra));
                    Dmin = ra + (Dmin - ra) * e;

                }
                else
                {
                    var e = Math.Exp(ExponentCoef * 2 * Math.Abs(Dmin - (razn_arr.Count() <= 0 ? Dmin : razn_arr.Average())));
                    Dmin = (razn_arr.Count() <= 0 ? Dmin : razn_arr.Average()) + (Dmin - (razn_arr.Count() <= 0 ? Dmin : razn_arr.Average())) * e;
                }

                razn_arr.Add(Dmin);
                vert_l_str = vert_l_str + $"{((Dmin) * WearCoef).ToString("0.00").Replace(",", ".")},{CurrentFrameIndex} ";
                vert_l.Add(Dmin);

                d_vert_left = Dmin;

                //------ПУ лев--------------------------------------------------------------------------------------------------------------
                var AA = Bottom_y2 - Bottom_y1;
                var BB = Bottom_x2 - Bottom_x1;

                var fi = AA / BB;
                var koefPU = 1.0 / 40.0;

                // fi = fi - 0.23;

                if (fiArr.Count() <= width)
                {
                    s1 = s1 + fi;
                    fi = 1.0 / 20.0;
                }
                if (fiArr.Count() > width)
                {
                    fi = fi - s1 / width + 1.0 / 20.0;
                }

                //  fi = fi*Math.Exp (-Math.Abs(fi ))     + 1 / 20;
                if (fiArr.Count() >= width)
                {
                    var LS = fiArr.Skip(fiArr.Count() - width).Take(width).ToList();
                    LS.Add(fi);

                    var RolAver = LS.Average();
                    var e = Math.Exp(ExponentCoef * 10 * Math.Abs(fi - 1 / 20.0));
                    var e1 = Math.Exp(ExponentCoef * 10 * Math.Abs(RolAver - 1 / 20.0));
                    RolAver = 1.0 / 20.0 + (RolAver - 1 / 20.0) * e1;
                    fi = 1.0 / 20.0 + 0.005 + (fi - 1 / 20.0) * e;
                }
                else
                {
                    var e = Math.Exp(ExponentCoef * 20 * Math.Abs(fi - (fiArr.Count() <= 0 ? fi : fiArr.Average())));
                    fi = (fiArr.Count() <= 0 ? fi : fiArr.Average()) + (fi - (fiArr.Count() <= 0 ? fi : fiArr.Average())) * e;
                    fi = 1.0 / 20.0;
                }

                /////////////////////////////
                ///


                /////////////////////////////
                var RolAver0l = 1.0 / 20.0;
                fiArr.Add(fi);
                var width1 = 10;
                if (fiArr.Count() - width > 0)
                {
                    var LS0l = fiArr.Skip(fiArr.Count() - width1).Take(width1).ToList();
                    RolAver0l = LS0l.Average();

                }

                pu_l_str = pu_l_str + $"{(RolAver0l * WearCoef * 10).ToString("0.00").Replace(",", ".")},{CurrentFrameIndex} ";
                pu_l.Add(fi);
                var fi_npl = 3.0 / 40.0 + 1.051 * (RolAver0l - 1.0 / 20.0) + d_vert_left / 300.0; ;// + vert_l[vert_l.Count-1]/400; 

                npk_aArr.Add(fi_npl);
                npk_l.Add(fi_npl);
                npk_l_str = npk_l_str + $"{((fi_npl) * WearCoef * 10).ToString("0.00").Replace(",", ".")},{CurrentFrameIndex} ";

                //------------------------------------------------------------------------------------------------------------------------------

            }
            if (side == Side.Right)
            {
                var golovXmax = arrHeadX.Max() * (-1) + Math.Max(arrHeadX.Max(), arrSideX.Max());
                //--------------------------------------------------------------------------------------------------------------------
                var sts = false;
                for (int i = 0; i <= (arrSideX.Count() - 1); i++)
                {
                    if (golovXmax + 15 > arrSideX[i] * (-1) + Math.Max(arrHeadX.Max(), arrSideX.Max()) && sts == false)
                    {
                        Bottom_x1_r = arrSideX[i] * (-1) + Math.Max(arrHeadX.Max(), arrSideX.Max());
                        Bottom_y1_r = arrSideY[i] * (-1) + Math.Max(arrHeadY.Max(), arrSideY.Max());

                        //среднее по линии
                        var vremX1 = new List<double> { };
                        var vremY1 = new List<double> { };

                        if (i - 5 > 0)
                        {
                            for (int ii = i - 5; ii < i; ii++)
                            {
                                vremX1.Add(arrSideX[ii]);
                                vremY1.Add(arrSideY[ii]);
                            }
                        }
                        else
                        {
                            if (i + 5 < arrSideX.Count())
                            {
                                for (int ii = i; ii < i + 5; ii++)
                                {
                                    vremX1.Add(arrSideX[ii]);
                                    vremY1.Add(arrSideY[ii]);
                                }
                            }
                        }

                        Bottom_x1_r = vremX1.Average() * (-1) + Math.Max(arrHeadX.Max(), arrSideX.Max());
                        Bottom_y1_r = vremY1.Average() * (-1) + Math.Max(arrHeadY.Max(), arrSideY.Max());

                        sts = true;
                    }
                    if (Bottom_x1_r - 15 > arrSideX[i] * (-1) + Math.Max(arrHeadX.Max(), arrSideX.Max()) && sts == true)
                    {
                        Bottom_x2_r = arrSideX[i] * (-1) + Math.Max(arrHeadX.Max(), arrSideX.Max());
                        Bottom_y2_r = arrSideY[i] * (-1) + Math.Max(arrHeadY.Max(), arrSideY.Max());

                        //среднее по линии
                        var vremX2 = new List<double> { };
                        var vremY2 = new List<double> { };
                        if (i - 3 > 0 && i + 3 < arrSideX.Count())
                        {
                            for (int ii = i - 3; ii < i + 3; ii++)
                            {
                                vremX2.Add(arrSideX[ii]);
                                vremY2.Add(arrSideY[ii]);
                            }
                            Bottom_x2_r = vremX2.Average() * (-1) + Math.Max(arrHeadX.Max(), arrSideX.Max());
                            Bottom_y2_r = vremY2.Average() * (-1) + Math.Max(arrHeadY.Max(), arrSideY.Max());

                            break;
                        }
                    }
                }
                ////------------------ПУ правая------------------------------------------------------------------------------------------------------------                
                if (side == Side.Right)
                {
                    var AA = Bottom_y2_r - Bottom_y1_r;
                    var BB = Bottom_x1_r - Bottom_x2_r;

                    var fi = AA / BB;

                    if (fiArr_r.Count() <= width)
                    {
                        s0 = s0 + fi;
                        fi = 1.0 / 20.0;
                    }

                    if (fiArr_r.Count() > width)
                    {
                        fi = fi - s0 / width + 1.0 / 20;
                    }

                    if (fiArr_r.Count() >= width)
                    {
                        var LS = fiArr_r.Skip(fiArr_r.Count() - width).Take(width).ToList();
                        LS.Add(fi);

                        var RolAver = LS.Average();
                        var e = Math.Exp(ExponentCoef * 10 * Math.Abs(fi - 1 / 20.0));
                        var e1 = Math.Exp(ExponentCoef * 10 * Math.Abs(RolAver - 1 / 20.0));

                        fi = 1 / 20.0 + (fi - 1 / 20.0) * e - 0.006;
                    }

                    var RolAver0 = 1.0 / 20.0;

                    var width1 = 10;
                    fiArr_r.Add(fi);
                    if (fiArr_r.Count() - width > 0)
                    {
                        var LS0 = fiArr_r.Skip(fiArr_r.Count() - width1).Take(width1).ToList();
                        RolAver0 = LS0.Average();
                    }

                    pu_r_str = pu_r_str + $"{((RolAver0) * WearCoef * 10).ToString("0.00").Replace(",", ".")},{CurrentFrameIndex} ";
                    pu_r.Add(RolAver0);

                    var fi_npr = 3.0 / 40.0 + 1.051 * (RolAver0 - 1.0 / 20.0) + d_vert_rigt / 300.0;

                    npk_aArr_r.Add(fi_npr);
                    npk_r.Add(fi_npr);
                    npk_r_str = npk_r_str + $"{((fi_npr) * WearCoef * 10).ToString("0.00").Replace(",", ".")},{CurrentFrameIndex} ";
                }

                //--Верт Из правый------------------------------------------------------------------------------------------------------------------
                var A1 = 1 / (Bottom_x2_r - Bottom_x1_r);
                var B1 = -1 / (Bottom_y2_r - Bottom_y1_r);
                var C1 = -(Bottom_x1_r / (Bottom_x2_r - Bottom_x1_r)) + (Bottom_y1_r / (Bottom_y2_r - Bottom_y1_r));

                List<double> DList = new List<double>();
                double aver = 0.0;
                if (razn_arr_r.Count() >= width)
                {
                    aver = razn_arr_r.Skip(razn_arr_r.Count() - width).Take(width).Average();
                }
                else
                {
                    aver = razn_arr_r.Count() > 0 ? razn_arr_r.Average() : 0;
                }

                for (int i = 0; i < arrHeadY.Count() - 1; i++)
                {
                    var D = Math.Abs(A1 * (arrHeadX[i] * (-1) + Math.Max(arrHeadX.Max(), arrSideX.Max())) +
                        B1 * (arrHeadY[i] * (-1) + Math.Max(arrHeadY.Max(), arrSideY.Max())) + C1) / Math.Sqrt(Math.Pow(A1, 2) + Math.Pow(B1, 2));
                    DList.Add(D);
                }
                var Dmin = DList.Max() - 149;

                //--------------------------

                var bott_y = 0.0;

                List<double> CenterPoinArrX = new List<double>();
                List<double> CenterPoinArrY = new List<double>();

                for (int i = 0; i <= (arrSideX.Count() - 1); i++)
                {
                    var y = ((arrSideX[i] - Bottom_x1_r) / (Bottom_x2_r - Bottom_x1_r)) * Bottom_y2_r - ((arrSideX[i] - Bottom_x2_r) / (Bottom_x2_r - Bottom_x1_r)) * Bottom_y1_r;
                    var s = Math.Abs(y - (arrSideY[i] * (-1) + Math.Max(arrHeadY.Max(), arrSideY.Max())));

                    double x1 = 0.0, x2 = 0.0, y1 = 0.0, y2 = 0.0, radius = 25;
                    if (i >= 40)
                    {
                        x1 = arrSideX[i - 10];
                        x2 = arrSideX[i - 40];

                        y1 = arrSideY[i - 10];
                        y2 = arrSideY[i - 40];

                        double Ar = (x1 * x1 - x2 * x2 + y1 * y1 - y2 * y2) / (y1 * 2.0 - y2 * 2.0);
                        double Br = (x2 - x1) / (y1 - y2);

                        double alpha = 1 + Br * Br;
                        double beta = 2 * Ar * Br - 2 * x1 - 2 * y1 * Br;
                        double gamma = x1 * x1 + y1 * y1 - radius * radius - 2 * y1 * Ar + Ar * Ar;

                        double D = beta * beta - 4 * alpha * gamma;

                        if (D > 0)
                        {
                            double x01 = (-beta + Math.Sqrt(D)) / (2 * alpha);
                            double x02 = (-beta - Math.Sqrt(D)) / (2 * alpha);
                            double y01 = Ar + Br * x01;
                            double y02 = Ar + Br * x02;

                            CenterPoinArrX.Add(x01);
                            CenterPoinArrY.Add(y01);

                        }
                    }

                    bott_y = arrSideY[i];
                    if (s < 0.5)
                        break;
                }

                var PointAvgX = CenterPoinArrX.Count != 0 ? CenterPoinArrX.Skip(CenterPoinArrX.Count() - 5).Take(5).Average() * (-1) + Math.Max(arrHeadX.Max(), arrSideX.Max()) : 0;
                var PointAvgY = CenterPoinArrY.Count != 0 ? CenterPoinArrY.Skip(CenterPoinArrY.Count() - 5).Take(5).Average() * (-1) + Math.Max(arrHeadY.Max(), arrSideY.Max()) : 0;

                Dmin = (PointAvgY - (Ytest4_r + (arrHeadY[DList.IndexOf(DList.Max())] * (-1) + Math.Max(arrHeadY.Max(), arrSideY.Max()))) / 2) - 134.4;

                RadX_r = PointAvgX;
                RadY_r = PointAvgY;

                Perpen_x1_r = arrHeadX[DList.IndexOf(DList.Max())] * (-1) + Math.Max(arrHeadX.Max(), arrSideX.Max());
                Perpen_y1_r = arrHeadY[DList.IndexOf(DList.Max())] * (-1) + Math.Max(arrHeadY.Max(), arrSideY.Max());

                Dmin = /*golovXmax < Bottom_x1_r*/ /*|| Perpen_x1_r > 60*/  Bottom_y2_r < Bottom_y1_r /*|| (Bottom_x1_r - Bottom_x2_r) < 15*/ ? razn_arr_r.Count() > 0 ? aver : 0 : Dmin;
                //Dmin = Dmin < 0 ? razn_arr_r.Count() > 0 ? aver : 0 : Dmin;
                if (razn_arr_r.Count() >= width)
                {
                    var ra = razn_arr_r.Skip(razn_arr_r.Count() - width).Take(width).Average();
                    var e = Math.Exp(ExponentCoef * 2 * Math.Abs(Dmin - ra));
                    Dmin = ra + (Dmin - ra) * e;
                }
                else
                {
                    var e = Math.Exp(ExponentCoef * 2 * Math.Abs(Dmin - (razn_arr_r.Count() <= 0 ? Dmin : razn_arr_r.Average())));
                    Dmin = (razn_arr_r.Count() <= 0 ? Dmin : razn_arr_r.Average()) + (Dmin - (razn_arr_r.Count() <= 0 ? Dmin : razn_arr_r.Average())) * e;
                }
                razn_arr_r.Add(Dmin);
                vert_r_str = vert_r_str + $"{((Dmin) * WearCoef).ToString("0.00").Replace(",", ".")},{CurrentFrameIndex} ";
                vert_r.Add(Dmin);
                d_vert_rigt = Dmin;
            }

            //износ 45 град-- - бұл істеп тұр
            if (side == Side.Left)
            {
                var iz45 = Math.Sqrt(Math.Pow(vert_elem, 2) + Math.Pow(bok_elem, 2));
                //iz_45.Add(iz45);
                is_45_l_str = is_45_l_str + $"{(iz45 * WearCoef).ToString("0.00").Replace(",", ".")},{CurrentFrameIndex} ";
                iz45_l.Add(iz45);
            }
            if (side == Side.Right)
            {
                var iz45 = Math.Sqrt(Math.Pow(vert_elem, 2) + Math.Pow(bok_elem, 2));
                //iz_45_r.Add(iz45);
                is_45_r_str = is_45_r_str + $"{(iz45 * WearCoef).ToString("0.00").Replace(",", ".")},{CurrentFrameIndex} ";
                iz45_r.Add(iz45);
            }

            try
            {
                vbMinX = vector.Where((x, i) => i % 2 == 0).Min() - 10.0;
                vbX = vector.Where((x, i) => i % 2 == 0).Max() - vector.Where((x, i) => i % 2 == 0).Min() + 20.0;
                vbMinY = vector.Where((y, i) => i % 2 != 0).Min() - 10.0;
                vbY = vector.Where((y, i) => i % 2 != 0).Max() - vector.Where((y, i) => i % 2 != 0).Min() + 20.0;
                viewBox = vbMinX.ToString().Replace(",", ".") + " " + vbMinY.ToString().Replace(",", ".") + " " + vbX.ToString().Replace(",", ".") + " " + vbY.ToString().Replace(",", ".");
            }
            catch (Exception)
            {

                throw;
            }

            if (xrb < 0 && yrb < 0)
            {
                if (side == Side.Left)
                {
                    pu_l.Add(-1);
                    vert_l.Add(-1);
                    bok_l.Add(-1);
                    npk_l.Add(-1);
                    SWavesLeft.Add(-1);
                    MWavesLeft.Add(-1);
                    LWavesLeft.Add(-1);
                }
                else
                {
                    pu_r.Add(-1);
                    vert_r.Add(-1);
                    bok_r.Add(-1);
                    npk_r.Add(-1);
                    SWavesRight.Add(-1);
                    MWavesRight.Add(-1);
                    LWavesRight.Add(-1);
                }

                NominalTranslateLeft = "0,0";
                NominalTranslateRight = "0,0";
                NominalRotateLeft = "0";
                NominalRotateRight = "0";
                return;
            }

            switch (rails)
            {
                case Rails.r75:
                    if (side == Side.Left)
                    {
                        trX = xrb - (459.999949 * Math.Cos(DegreeToRadian(-delta)) - 107.2858112 * Math.Sin(DegreeToRadian(-delta)));
                        trY = yrb * (-1) - (459.999949 * Math.Sin(DegreeToRadian(-delta)) + 107.2858112 * Math.Cos(DegreeToRadian(-delta))) + Math.Max(arrHeadY.Max(), arrSideY.Max());
                        NominalTranslateLeft = trX.ToString().Replace(',', '.') + "px," + trY.ToString().Replace(',', '.') + "px";
                        NominalRotateLeft = (-delta).ToString().Replace(',', '.') + "deg";
                    }
                    else
                    {
                        trX = xrb * (-1) - (459.999949 * (-1) * Math.Cos(DegreeToRadian(delta)) - 107.2858112 * Math.Sin(DegreeToRadian(delta))) + Math.Max(arrHeadX.Max(), arrSideX.Max());
                        trY = yrb * (-1) - (459.999949 * (-1) * Math.Sin(DegreeToRadian(delta)) + 107.2858112 * Math.Cos(DegreeToRadian(delta))) + Math.Max(arrHeadY.Max(), arrSideY.Max()) + 5.0;
                        NominalTranslateRight = trX.ToString().Replace(',', '.') + "px," + trY.ToString().Replace(',', '.') + "px";
                        NominalRotateRight = delta.ToString().Replace(',', '.') + "deg";
                    }
                    break;
                case Rails.r65:
                    if (side == Side.Left)
                    {
                        //trX = xrb - (408.9999887 * Math.Cos(DegreeToRadian(-delta)) - 97.5952637 * Math.Sin(DegreeToRadian(-delta)));
                        //trY = yrb * (-1) - (408.9999887 * Math.Sin(DegreeToRadian(-delta)) + 97.5952637 * Math.Cos(DegreeToRadian(-delta))) + Math.Max(arrHeadY.Max(), arrSideY.Max());
                        trX = -10; trY = -10;
                        NominalTranslateLeft = trX.ToString().Replace(',', '.') + "px," + trY.ToString().Replace(',', '.') + "px";
                        NominalRotateLeft = (-delta).ToString().Replace(',', '.') + "deg";
                    }
                    else
                    {
                        //trX = xrb * (-1) - (408.9999887 * (-1) * Math.Cos(DegreeToRadian(delta)) - 97.5952637 * Math.Sin(DegreeToRadian(delta))) + Math.Max(arrHeadX.Max(), arrSideX.Max());
                        //trY = yrb * (-1) - (408.9999887 * (-1) * Math.Sin(DegreeToRadian(delta)) + 97.5952637 * Math.Cos(DegreeToRadian(delta))) + Math.Max(arrHeadY.Max(), arrSideY.Max()) + 5.0;
                        trX = -10; trY = -10;
                        NominalTranslateRight = trX.ToString().Replace(',', '.') + "px," + trY.ToString().Replace(',', '.') + "px";
                        NominalRotateRight = delta.ToString().Replace(',', '.') + "deg";
                    }
                    break;
                case Rails.r50:
                    if (side == Side.Left)
                    {
                        trX = xrb - (357.9999183 * Math.Cos(DegreeToRadian(-delta)) - 83.2608731 * Math.Sin(DegreeToRadian(-delta)));
                        trY = yrb * (-1) - (357.9999183 * Math.Sin(DegreeToRadian(-delta)) + 83.2608731 * Math.Cos(DegreeToRadian(-delta))) + Math.Max(arrHeadY.Max(), arrSideY.Max());
                        NominalTranslateLeft = trX.ToString().Replace(',', '.') + "px," + trY.ToString().Replace(',', '.') + "px";
                        NominalRotateLeft = (-delta).ToString().Replace(',', '.') + "deg";
                    }
                    else
                    {
                        trX = xrb * (-1) - (357.9999183 * (-1) * Math.Cos(DegreeToRadian(delta)) - 83.2608731 * Math.Sin(DegreeToRadian(delta))) + Math.Max(arrHeadX.Max(), arrSideX.Max());
                        trY = yrb * (-1) - (357.9999183 * (-1) * Math.Sin(DegreeToRadian(delta)) + 83.2608731 * Math.Cos(DegreeToRadian(delta))) + Math.Max(arrHeadY.Max(), arrSideY.Max()) + 5.0;
                        NominalTranslateRight = trX.ToString().Replace(',', '.') + "px," + trY.ToString().Replace(',', '.') + "px";
                        NominalRotateRight = delta.ToString().Replace(',', '.') + "deg";
                    }
                    break;
                case Rails.r43:
                    if (side == Side.Left)
                    {
                        trX = xrb - (357.2103467 * Math.Cos(DegreeToRadian(-delta)) - 76.7683742 * Math.Sin(DegreeToRadian(-delta)));
                        trY = yrb * (-1) - (357.2103467 * Math.Sin(DegreeToRadian(-delta)) + 76.7683742 * Math.Cos(DegreeToRadian(-delta))) + Math.Max(arrHeadY.Max(), arrSideY.Max());
                        NominalTranslateLeft = trX.ToString().Replace(',', '.') + "px," + trY.ToString().Replace(',', '.') + "px";
                        NominalRotateLeft = (-delta).ToString().Replace(',', '.') + "deg";
                    }
                    else
                    {
                        trX = xrb * (-1) - (357.2103467 * (-1) * Math.Cos(DegreeToRadian(delta)) - 76.7683742 * Math.Sin(DegreeToRadian(delta))) + Math.Max(arrHeadX.Max(), arrSideX.Max());
                        trY = yrb * (-1) - (357.2103467 * (-1) * Math.Sin(DegreeToRadian(delta)) + 76.7683742 * Math.Cos(DegreeToRadian(delta))) + Math.Max(arrHeadY.Max(), arrSideY.Max()) + 5.0;
                        NominalTranslateRight = trX.ToString().Replace(',', '.') + "px," + trY.ToString().Replace(',', '.') + "px";
                        NominalRotateRight = delta.ToString().Replace(',', '.') + "deg";
                    }
                    break;
                default:
                    break;
            }

            // GetProfileDiagram(ref schemeVector, xrb, yrb, delta, rails, side, Math.Max(arrHeadX.Max(), arrSideX.Max()), Math.Max(arrHeadY.Max(), arrSideY.Max()));
            delta = 0;
            if (side == Side.Left)
                DownhillLeft.Add(-(delta + CalibrConstLeft));
            else
                DownhillRight.Add(-(delta + CalibrConstRight1 + CalibrConstRight));

            double wear_delta = -1, wear45_delta = -1, wear_side = -1, SSS = -1;
            GetWears(ref wear_delta, ref wear45_delta, ref wear_side, xrb, yrb, arrHeadX, arrHeadY, delta, rails);
            GetSSS(ref SSS, arrHeadX, arrHeadY, rails, delta, xrb, yrb);

            if (side == Side.Left)
            {
                //  VertWearLeft.Add(wear_delta);
                //  SideWearLeft.Add(wear_side);
                //  Wear45Left.Add(wear45_delta);
                //  SlopeSkatingSurfaceLeft.Add(SSS);
            }
            else
            {
                //    VertWearRight.Add(wear_delta);
                //  SideWearRight.Add(wear_side);
                // Wear45Right.Add(wear45_delta);
                // SlopeSkatingSurfaceRight.Add(SSS);
            }
        }


        /// <summary>
        /// Читать входные данные профиля рельса (шейки и головки) с шумом и разделить на отдельные массивы координат по шейке и головке без шумов
        /// </summary>
        /// <param name="arrSideX">массив координат шейки рельса по оси X (выходная данная)</param>
        /// <param name="arrSideY">массив координат шейки рельса по оси Y (выходная данная)</param>
        /// <param name="arrHeadX">массив координат головки рельса по оси Х (выходная данная)</param>
        /// <param name="arrHeadY">массив координат головки рельса по оси Y (выходная данная)</param>
        /// <param name="arrX">массив координат профиля рельса по оси Х (входная данная)</param>
        /// <param name="arrY">массив координат профиля рельса по оси Y (входная данная)</param>
        /// <param name="coefHead">константа для головки рельса по типу рельса</param>
        /// <param name="coefSideTop">константа для шейки рельса по типу рельса</param>
        /// <param name="coefSideBot">константа для шейки рельса по типу рельса</param>
        private void TryReadProfile(ref List<double> arrSideX, ref List<double> arrSideY, ref List<double> arrHeadX, ref List<double> arrHeadY, double[] arrX, double[] arrY, double coefHead, double coefSideTop, double coefSideBot)
        {
            /*
             * Надо пересмотреть, недочеты в алгоритме (иногда берет шумы)
             */
            var tempArrX = arrX.Zip(arrY, (x, y) => new { x, y }).Where(p => p.y >= arrY.Max() - 50.0).Select(p => p.x).ToArray();
            var tempArrY = arrX.Zip(arrY, (x, y) => new { x, y }).Where(p => p.y >= arrY.Max() - 50.0).Select(p => p.y).ToArray();

            double rightX, topY;
            double[] my = new double[3];
            int rightI, x0;

            rightX = tempArrX.Max();
            rightI = Array.IndexOf(tempArrX, rightX);

            x0 = Math.Max(rightI - 75, 0);

            for (int i = 1; i <= 3; i++)
            {
                try
                {
                    my[i - 1] = tempArrY.Skip((i - 1) * 25 + x0).Take(25).Max();
                }
                catch (Exception)
                {

                    throw;
                }
            }

            topY = my.Min();

            for (int i = 0; i < Math.Min(arrX.Length, arrY.Length); i++)
            {
                try
                {
                    if (arrY[i] >= topY - coefHead && arrY[i] <= topY + 20)
                    {
                        arrHeadX.Add(arrX[i]);
                        arrHeadY.Add(arrY[i]);
                    }
                    if (arrY[i] >= topY - coefSideTop && arrY[i] <= topY - coefSideBot)
                    {
                        arrSideX.Add(arrX[i]);
                        arrSideY.Add(arrY[i]);
                    }
                }
                catch (Exception)
                {

                    throw;
                }

            }
        }

        /// <summary>
        /// Нахождение координат центра окружности и подуклонки
        /// </summary>
        /// <param name="xr0">координата окружности центра по оси Х</param>
        /// <param name="yr0">координата окружности центра по оси Y</param>
        /// <param name="delta"></param>
        /// <param name="arrSideX"></param>
        /// <param name="arrSideY"></param>
        /// <param name="toDownIndex"></param>
        /// <param name="calcParam"></param>
        private void SearchCenterPointsAndDelta2(double xr0, double yr0, ref double delta, double[] arrSideX, double[] arrSideY, int toDownIndex, ProfileCalcParameter calcParam)
        {
            try
            {
                for (int i = toDownIndex; i < arrSideX.Length - 4; i++)
                {
                    double
                        x1 = arrSideX.Get(Math.Max(i - 2, 0), i + 3).Average(),
                        y1 = arrSideY.Get(Math.Max(i - 2, 0), i + 3).Average();

                    if (GetDistanceBetweenPoints(arrSideX.First(), x1, arrSideY.First(), y1) > calcParam.DistBigRad)
                    {
                        double x2r = 0.0, y2r = 0.0;
                        x1 = arrSideX.Get(Math.Max(toDownIndex - 2, 0), (toDownIndex + 3) % arrSideX.Length).Average();
                        y1 = arrSideY.Get(Math.Max(toDownIndex - 2, 0), (toDownIndex + 3) % arrSideX.Length).Average();
                        var a1 = arrSideX.Get(Math.Max(toDownIndex - 2, 0), (toDownIndex + 3) % arrSideX.Length).Average();
                        bool isFirst = true;

                        for (int j = toDownIndex + 1; j < arrSideX.Length; j++)
                        {
                            double x2 = arrSideX.Get(j - 2, j + 3).Average(),
                                   y2 = arrSideY.Get(j - 2, j + 3).Average();
                            if (GetDistanceBetweenPoints(x1, x2, y1, y2) > 15)
                            {
                                double x0 = -1, y0 = -1;

                                if (GetCenterCoords(ref x0, ref y0, x1, y1, x2, y2, calcParam.Radius))
                                {
                                    if (isFirst)
                                    {
                                        //xr0 = x0;
                                        //yr0 = y0;
                                        isFirst = false;
                                        continue;
                                    }
                                    else if (!(IsClose(xr0, x0, 0.15, 0.1) && IsClose(yr0, y0, 0.15, 0.1)))
                                    {
                                        x2r = arrSideX.Get(j - 3, j + 2).Average();
                                        y2r = arrSideY.Get(j - 3, j + 2).Average();
                                        break;
                                    }
                                }
                            }
                        }

                        delta = calcParam.AngleG + GetAngleDiff(xr0, yr0, x2r, y2r, calcParam.Radius);
                        return;
                    }

                    for (int j = i + 1; j < arrSideX.Length - 3; j++)
                    {
                        double
                            x2 = arrSideX.Get(Math.Max(j - 2, 0), j + 3).Average(),
                            y2 = arrSideY.Get(Math.Max(j - 2, 0), j + 3).Average();


                        if (IsClose(GetDistanceBetweenPoints(x1, x2, y1, y2), calcParam.DistBigRad, 0.01, 0.1))
                        {
                            for (int k = j + 1; j < arrSideX.Length - 2; k++)
                            {
                                double
                                    x3 = arrSideX.Get(k - 2, k + 3).Average(),
                                    y3 = arrSideY.Get(k - 2, k + 3).Average();

                                //Xtest2 = x2;
                                //Ytest2 = y2;
                                if (IsClose(GetDistanceBetweenPoints(x2, x3, y2, y3), calcParam.DistLitRad, 0.01, 0.1))
                                {
                                    double xrb = 0, yrb = 0, xrl = 0, yrl = 0;
                                    GetCenterCoords(ref xrb, ref yrb, x1, y1, x2, y2, calcParam.Radius);
                                    GetCenterCoords(ref xrl, ref yrl, x2, y2, x3, y3, calcParam.LittleRadius);

                                    if (IsClose(GetDistanceBetweenPoints(xrb, xrl, yrb, yrl), calcParam.DistParam, 0.001, 0.1))
                                    {
                                        //xr0 = xrb;
                                        //yr0 = yrb;
                                        delta = calcParam.AngleG + GetAngleDiff(xr0, yr0, x2, y2, calcParam.Radius);
                                        return;
                                    }
                                }
                                else if (GetDistanceBetweenPoints(x2, x3, y2, y3) > calcParam.DistLitRad * 1.1)
                                {
                                    break;
                                }
                            }
                        }
                        else if (GetDistanceBetweenPoints(x1, x2, y1, y2) > calcParam.DistBigRad * 1.1)
                        {
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex.Message);
                xr0 = -1;
                yr0 = -1;
            }
        }

        /// <summary>
        /// Нахождение координат центра окружности и подуклонки
        /// </summary>
        /// <param name="xr0">координата окружности центра по оси Х</param>
        /// <param name="yr0">координата окружности центра по оси Y</param>
        /// <param name="delta"></param>
        /// <param name="arrSideX"></param>
        /// <param name="arrSideY"></param>
        /// <param name="toDownIndex"></param>
        /// <param name="calcParam"></param>
        private void SearchCenterPointsAndDelta(ref double xr0, ref double yr0, ref double delta, double[] arrSideX, double[] arrSideY, int toDownIndex, ProfileCalcParameter calcParam)
        {
            try
            {
                for (int i = toDownIndex; i < arrSideX.Length - 4; i++)
                {
                    double
                        x1 = arrSideX.Get(Math.Max(i - 2, 0), i + 3).Average(),
                        y1 = arrSideY.Get(Math.Max(i - 2, 0), i + 3).Average();

                    if (GetDistanceBetweenPoints(arrSideX.First(), x1, arrSideY.First(), y1) > calcParam.DistBigRad)
                    {
                        double x2r = 0.0, y2r = 0.0;
                        x1 = arrSideX.Get(Math.Max(toDownIndex - 2, 0), (toDownIndex + 3) % arrSideX.Length).Average();
                        y1 = arrSideY.Get(Math.Max(toDownIndex - 2, 0), (toDownIndex + 3) % arrSideX.Length).Average();
                        var a1 = arrSideX.Get(Math.Max(toDownIndex - 2, 0), (toDownIndex + 3) % arrSideX.Length).Average();
                        bool isFirst = true;

                        for (int j = toDownIndex + 1; j < arrSideX.Length; j++)
                        {
                            double x2 = arrSideX.Get(j - 2, j + 3).Average(),
                                   y2 = arrSideY.Get(j - 2, j + 3).Average();
                            if (GetDistanceBetweenPoints(x1, x2, y1, y2) > 15)
                            {
                                double x0 = -1, y0 = -1;

                                if (GetCenterCoords(ref x0, ref y0, x1, y1, x2, y2, calcParam.Radius))
                                {
                                    if (isFirst)
                                    {
                                        xr0 = x0;
                                        yr0 = y0;
                                        isFirst = false;
                                        continue;
                                    }
                                    else if (!(IsClose(xr0, x0, 0.15, 0.1) && IsClose(yr0, y0, 0.15, 0.1)))
                                    {
                                        x2r = arrSideX.Get(j - 3, j + 2).Average();
                                        y2r = arrSideY.Get(j - 3, j + 2).Average();
                                        break;
                                    }
                                }
                            }
                        }

                        delta = calcParam.AngleG + GetAngleDiff(xr0, yr0, x2r, y2r, calcParam.Radius);
                        return;
                    }

                    for (int j = i + 1; j < arrSideX.Length - 3; j++)
                    {
                        double
                            x2 = arrSideX.Get(Math.Max(j - 2, 0), j + 3).Average(),
                            y2 = arrSideY.Get(Math.Max(j - 2, 0), j + 3).Average();
                        if (IsClose(GetDistanceBetweenPoints(x1, x2, y1, y2), calcParam.DistBigRad, 0.01, 0.1))
                        {
                            for (int k = j + 1; j < arrSideX.Length - 2; k++)
                            {
                                double
                                    x3 = arrSideX.Get(k - 2, k + 3).Average(),
                                    y3 = arrSideY.Get(k - 2, k + 3).Average();

                                if (IsClose(GetDistanceBetweenPoints(x2, x3, y2, y3), calcParam.DistLitRad, 0.01, 0.1))
                                {
                                    double xrb = 0, yrb = 0, xrl = 0, yrl = 0;
                                    GetCenterCoords(ref xrb, ref yrb, x1, y1, x2, y2, calcParam.Radius);
                                    GetCenterCoords(ref xrl, ref yrl, x2, y2, x3, y3, calcParam.LittleRadius);

                                    if (IsClose(GetDistanceBetweenPoints(xrb, xrl, yrb, yrl), calcParam.DistParam, 0.001, 0.1))
                                    {
                                        xr0 = xrb;
                                        yr0 = yrb;
                                        delta = calcParam.AngleG + GetAngleDiff(xr0, yr0, x2, y2, calcParam.Radius);
                                        return;
                                    }
                                }
                                else if (GetDistanceBetweenPoints(x2, x3, y2, y3) > calcParam.DistLitRad * 1.1)
                                {
                                    break;
                                }
                            }
                        }
                        else if (GetDistanceBetweenPoints(x1, x2, y1, y2) > calcParam.DistBigRad * 1.1)
                        {
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex.Message);
                xr0 = -1;
                yr0 = -1;
            }
        }

        /// <summary>
        /// Найти центр окружности проходящих через 2 точки
        /// </summary>
        /// <param name="x0">координата центра окружности по оси Х (выходная данная)</param>
        /// <param name="y0">координата центра окружности по оси Y (выходная данная)</param>
        /// <param name="x1">координата первой точки по оси X (входная данная)</param>
        /// <param name="y1">координата первой точки по оси Y (входная данная)</param>
        /// <param name="x2">координата второй точки по оси Х (входная данная)</param>
        /// <param name="y2">координата второй точки по оси Y (входная данная)</param>
        /// <param name="radius">радиус окружности (входная данная)</param>
        /// <returns>true - в случае нахождения координат центра окружности, false - в противном случае</returns>
        private bool GetCenterCoords(ref double x0, ref double y0, double x1, double y1, double x2, double y2, double radius)
        {
            double A = (x1 * x1 - x2 * x2 + y1 * y1 - y2 * y2) / (y1 * 2.0 - y2 * 2.0);
            double B = (x2 - x1) / (y1 - y2);

            double alpha = 1 + B * B;
            double beta = 2 * A * B - 2 * x1 - 2 * y1 * B;
            double gamma = x1 * x1 + y1 * y1 - radius * radius - 2 * y1 * A + A * A;

            double D = beta * beta - 4 * alpha * gamma;

            if (D > 0)
            {
                double x01 = (-beta + Math.Sqrt(D)) / (2 * alpha);
                double x02 = (-beta - Math.Sqrt(D)) / (2 * alpha);
                double y01 = A + B * x01;
                double y02 = A + B * x02;

                if (x01 >= x02)
                {
                    x0 = x01;
                    y0 = y01;
                    //Xtest1 = x0;
                    //Xtest2 = y0;
                    if (radius == 400)
                    {
                        var rad_test = (Math.Sqrt(Math.Pow(x0, 2) + Math.Pow(y0, 2)));
                        if (Math.Abs(rad_test - 427) < 5)
                        {
                            Rad_arr.Add(rad_test - 427);
                        }
                    }
                    return true;
                }
                else
                {
                    x0 = x02;
                    y0 = y02;
                    if (radius == 400)
                    {
                        var rad_test = (Math.Sqrt(Math.Pow(x0, 2) + Math.Pow(y0, 2)));
                        if (Math.Abs(rad_test - 422) < 5)
                        {
                            Rad_arr.Add(rad_test - 422);
                        }
                    }
                    return true;
                }
            }
            else
            {
                x0 = -1;
                y0 = -1;
                return false;
            }
        }

        /// <summary>
        /// Нахождение расстояния между двумья точками
        /// </summary>
        /// <param name="x1">координата первой точки по оси Х</param>
        /// <param name="x2">координата первой точки по оси Y</param>
        /// <param name="y1">координата второй точки по оси Х</param>
        /// <param name="y2">координата второй точки по оси Y</param>
        /// <returns>Расстояние между двумья точками</returns>
        private double GetDistanceBetweenPoints(double x1, double x2, double y1, double y2)
        {
            return Math.Sqrt(Math.Pow(x1 - x2, 2.0) + Math.Pow(y1 - y2, 2.0));
        }

        /// <summary>
        /// Нахождения близости двух значении по абсолютной и относительной погрешностью
        /// </summary>
        /// <param name="value1">Первое значение</param>
        /// <param name="value2">Второе значение</param>
        /// <param name="rel_tol">Относительная погрешность</param>
        /// <param name="abs_tol">Абсолютная погрешность</param>
        /// <returns>true - в случае близости дух значении, false - в противном случае</returns>
        private bool IsClose(double value1, double value2, double rel_tol, double abs_tol)
        {
            return Math.Abs(value1 - value2) <= Math.Max(rel_tol * Math.Max(Math.Abs(value1), Math.Abs(value2)), abs_tol);
        }

        /// <summary>
        /// Нахождения разницы угла между точкой и прямой проходящей через центр окружности перпендикулярно к оси Х
        /// </summary>
        /// <param name="xr">координата точки по оси Х</param>
        /// <param name="yr">координата точки по оси Y</param>
        /// <param name="x2r">координата окружности по оси Х</param>
        /// <param name="y2r">координата окружности по оси Y</param>
        /// <param name="radius">радиус окружности</param>
        /// <returns>Отрицательное значение угла если точка находится в левую сторону от центра окружности, в противном случае положительное значение угла в радиан</returns>
        private double GetAngleDiff(double xr, double yr, double x2r, double y2r, double radius)
        {
            double ab = Math.Sqrt(Math.Pow(x2r - xr, 2.0) + Math.Pow(y2r - (yr - radius), 2.0));
            double alpha = ((180 / Math.PI) * Math.Asin(ab / (radius * 2))) * 2;

            double newAlpha = (180 / (2 * Math.PI)) * Math.Acos(radius / ab);

            if (x2r <= xr)
                return -alpha;
            else
                return alpha;
        }

        /// <summary>
        /// Нахождение вертикального, 45 градусного, бокового износа
        /// </summary>
        /// <param name="delta_wear">вертикальный износ (выходная данная)</param>
        /// <param name="delta_45wear">45 градусный износ (выходная данная)</param>
        /// <param name="delta_sidewear">боковой износ (выходная данная)</param>
        /// <param name="x0">координата центра окружности по оси Х</param>
        /// <param name="y0">координата центра окружности по оси Y</param>
        /// <param name="arrHeadX">массив координат головки рельса по оси Х</param>
        /// <param name="arrHeadY">массив координат головки рельса по оси Y</param>
        /// <param name="delta">подуклонка рельса</param>
        /// <param name="rails">тип рельса</param>
        private void GetWears(ref double delta_wear, ref double delta_45wear, ref double delta_sidewear, double x0, double y0, double[] arrHeadX, double[] arrHeadY, double delta, Rails rails)
        {



            double /*xc1 = -1, yc1 = -1,*/ xc2 = -1, yc2 = -1;
            switch (rails)
            {
                case Rails.r75:
                    /*xc1 = (-459.999949) * Math.Cos(delta * Math.PI / 180) - (-84.7141888) * Math.Sin(delta * Math.PI / 180) + x0;
                    yc1 = (-459.999949) * Math.Sin(delta * Math.PI / 180) + (-84.7141888) * Math.Cos(delta * Math.PI / 180) + y0;
                    xc2 = (-459.999949) * Math.Cos(delta * Math.PI / 180) - (107.2858112) * Math.Sin(delta * Math.PI / 180) + x0;
                    yc2 = (-459.999949) * Math.Sin(delta * Math.PI / 180) + (107.2858112) * Math.Cos(delta * Math.PI / 180) + y0;*/
                    xc2 = (-459.999949) * Math.Cos(delta * Math.PI / 180) - (82.2858112) * Math.Sin(delta * Math.PI / 180) + x0;
                    yc2 = (-459.999949) * Math.Sin(delta * Math.PI / 180) + (82.2858112) * Math.Cos(delta * Math.PI / 180) + y0;
                    delta_wear = arrHeadX.Zip(arrHeadY, (x, y) => GetDistanceBetweenPoints(x, xc2, y, yc2)).Min();
                    break;
                case Rails.r65:
                    /*xc1 = (-408.9999887) * Math.Cos(delta * Math.PI / 180) - (-82.4047363) * Math.Sin(delta * Math.PI / 180) + x0;
                    yc1 = (-408.9999887) * Math.Sin(delta * Math.PI / 180) + (-82.4047363) * Math.Cos(delta * Math.PI / 180) + y0;*/
                    xc2 = (-408.9999887) * Math.Cos(delta * Math.PI / 180) - (97.5952637) * Math.Sin(delta * Math.PI / 180) + x0;
                    yc2 = (-408.9999887) * Math.Sin(delta * Math.PI / 180) + (97.5952637) * Math.Cos(delta * Math.PI / 180) + y0;
                    delta_wear = arrHeadX.Zip(arrHeadY, (x, y) => GetDistanceBetweenPoints(x, xc2, y, yc2)).Min();
                    break;
                case Rails.r50:
                    /*xc1 = (-357.9999183) * Math.Cos(delta * Math.PI / 180) - (-68.7391269) * Math.Sin(delta * Math.PI / 180) + x0;
                    yc1 = (-357.9999183) * Math.Sin(delta * Math.PI / 180) + (-68.7391269) * Math.Cos(delta * Math.PI / 180) + y0;*/
                    xc2 = (-357.9999183) * Math.Cos(delta * Math.PI / 180) - (83.2608731) * Math.Sin(delta * Math.PI / 180) + x0;
                    yc2 = (-357.9999183) * Math.Sin(delta * Math.PI / 180) + (83.2608731) * Math.Cos(delta * Math.PI / 180) + y0;
                    delta_wear = arrHeadX.Zip(arrHeadY, (x, y) => GetDistanceBetweenPoints(x, xc2, y, yc2)).Min();
                    break;
                case Rails.r43:
                    /*xc1 = (-357.2103467) * Math.Cos(delta * Math.PI / 180) - (-63.2316258) * Math.Sin(delta * Math.PI / 180) + x0;
                    yc1 = (-357.2103467) * Math.Sin(delta * Math.PI / 180) + (-63.2316258) * Math.Cos(delta * Math.PI / 180) + y0;*/
                    xc2 = (-357.2103467) * Math.Cos(delta * Math.PI / 180) - (76.7683742) * Math.Sin(delta * Math.PI / 180) + x0;
                    yc2 = (-357.2103467) * Math.Sin(delta * Math.PI / 180) + (76.7683742) * Math.Cos(delta * Math.PI / 180) + y0;
                    delta_wear = arrHeadX.Zip(arrHeadY, (x, y) => GetDistanceBetweenPoints(x, xc2, y, yc2)).Min();
                    break;
                default:
                    delta_wear = -1;
                    break;
            }

            /*var distArr = arrHeadX.Zip(arrHeadY, (x, y) => GetDistanceBetweenPoints(x, xc2, y, yc2)).ToList();
            var index = distArr.IndexOf(distArr.Min());
            double y2 = arrHeadY.Get(index, (index + 3) % arrHeadY.Length).Average(), y1 = arrHeadY.Get((index - 2) % arrHeadY.Length, (index + 1) % arrHeadY.Length).Average(),
                x2 = arrHeadX.Get(index, (index + 3) % arrHeadY.Length).Average(), x1 = arrHeadX.Get((index - 2) % arrHeadY.Length, (index + 1) % arrHeadY.Length).Average();

            double A = (yc2 - yc1) / (xc2 - xc1);
            double B = (y2 - y1) / (x2 - x1);
            double xc = (A * xc1 - yc1 - B * x1 + y1) / (A - B);
            double yc = A * (xc - xc1) + yc1;

            delta_wear = Math.Sqrt(Math.Pow((xc2 - xc), 2.0) + Math.Pow((yc2 - yc), 2.0));*/

            switch (rails)
            {
                case Rails.r75:
                    /*xc1 = (-459.999949) * Math.Cos(delta * Math.PI / 180) - (95.2858112) * Math.Sin(delta * Math.PI / 180) + x0;
                    yc1 = (-459.999949) * Math.Sin(delta * Math.PI / 180) + (95.2858112) * Math.Cos(delta * Math.PI / 180) + y0;*/
                    xc2 = (-419.999949) * Math.Cos(delta * Math.PI / 180) - (135.2858112) * Math.Sin(delta * Math.PI / 180) + x0;
                    yc2 = (-419.999949) * Math.Sin(delta * Math.PI / 180) + (135.2858112) * Math.Cos(delta * Math.PI / 180) + y0;
                    delta_45wear = arrHeadX.Zip(arrHeadY, (x, y) => GetDistanceBetweenPoints(x, xc2, y, yc2)).Min();
                    break;
                case Rails.r65:
                    /*xc1 = (-408.9999887) * Math.Cos(delta * Math.PI / 180) - (85.5952637) * Math.Sin(delta * Math.PI / 180) + x0;
                    yc1 = (-408.9999887) * Math.Sin(delta * Math.PI / 180) + (85.5952637) * Math.Cos(delta * Math.PI / 180) + y0;*/
                    xc2 = (-368.9999887) * Math.Cos(delta * Math.PI / 180) - (125.5952637) * Math.Sin(delta * Math.PI / 180) + x0;
                    yc2 = (-368.9999887) * Math.Sin(delta * Math.PI / 180) + (125.5952637) * Math.Cos(delta * Math.PI / 180) + y0;
                    delta_45wear = arrHeadX.Zip(arrHeadY, (x, y) => GetDistanceBetweenPoints(x, xc2, y, yc2)).Min();
                    break;
                case Rails.r50:
                    /*xc1 = (-357.9999183) * Math.Cos(delta * Math.PI / 180) - (71.2608731) * Math.Sin(delta * Math.PI / 180) + x0;
                    yc1 = (-357.9999183) * Math.Sin(delta * Math.PI / 180) + (71.2608731) * Math.Cos(delta * Math.PI / 180) + y0;*/
                    xc2 = (-327.2415861) * Math.Cos(delta * Math.PI / 180) - (77.7121591) * Math.Sin(delta * Math.PI / 180) + x0;
                    yc2 = (-327.2415861) * Math.Sin(delta * Math.PI / 180) + (77.7121591) * Math.Cos(delta * Math.PI / 180) + y0;
                    delta_45wear = arrHeadX.Zip(arrHeadY, (x, y) => GetDistanceBetweenPoints(x, xc2, y, yc2)).Min();
                    break;
                case Rails.r43:
                    /*xc1 = (-357.2103467) * Math.Cos(delta * Math.PI / 180) - (64.7683742) * Math.Sin(delta * Math.PI / 180) + x0;
                    yc1 = (-357.2103467) * Math.Sin(delta * Math.PI / 180) + (64.7683742) * Math.Cos(delta * Math.PI / 180) + y0;*/
                    xc2 = (-317.2103467) * Math.Cos(delta * Math.PI / 180) - (104.7683742) * Math.Sin(delta * Math.PI / 180) + x0;
                    yc2 = (-317.2103467) * Math.Sin(delta * Math.PI / 180) + (104.7683742) * Math.Cos(delta * Math.PI / 180) + y0;
                    delta_45wear = arrHeadX.Zip(arrHeadY, (x, y) => GetDistanceBetweenPoints(x, xc2, y, yc2)).Min();
                    break;
                default:
                    delta_45wear = -1;
                    break;
            }

            /*distArr.Clear();
            distArr = arrHeadX.Zip(arrHeadY, (x, y) => GetDistanceBetweenPoints(x, xc2, y, yc2)).ToList();
            index = distArr.IndexOf(distArr.Min());
            y2 = arrHeadY.Get(index, (index + 3) % arrHeadY.Length).Average();
            y1 = arrHeadY.Get((index - 2) % arrHeadY.Length, (index + 1) % arrHeadY.Length).Average();
            x2 = arrHeadX.Get(index, (index + 3) % arrHeadY.Length).Average();
            x1 = arrHeadX.Get((index - 2) % arrHeadY.Length, (index + 1) % arrHeadY.Length).Average();

            A = (yc2 - yc1) / (xc2 - xc1);
            B = (y2 - y1) / (x2 - x1);
            xc = (A * xc1 - yc1 - B * x1 + y1) / (A - B);
            yc = A * (xc - xc1) + yc1;

            delta_45wear = Math.Sqrt(Math.Pow((xc2 - xc), 2.0) + Math.Pow((yc2 - yc), 2.0));
            delta_45wear = distArr.Min();*/

            switch (rails)
            {
                case Rails.r75:
                    /*xc1 = (-459.999949) * Math.Cos(delta * Math.PI / 180) - (95.2858112) * Math.Sin(delta * Math.PI / 180) + x0;
                    yc1 = (-459.999949) * Math.Sin(delta * Math.PI / 180) + (95.2858112) * Math.Cos(delta * Math.PI / 180) + y0;*/
                    xc2 = (-423.999949) * Math.Cos(delta * Math.PI / 180) - (91.6858112) * Math.Sin(delta * Math.PI / 180) + x0;
                    yc2 = (-423.999949) * Math.Sin(delta * Math.PI / 180) + (91.6858112) * Math.Cos(delta * Math.PI / 180) + y0;
                    delta_sidewear = arrHeadX.Zip(arrHeadY, (x, y) => GetDistanceBetweenPoints(x, xc2, y, yc2)).Min();
                    break;
                case Rails.r65:
                    /*xc1 = (-408.9999887) * Math.Cos(delta * Math.PI / 180) - (85.5952637) * Math.Sin(delta * Math.PI / 180) + x0;
                    yc1 = (-408.9999887) * Math.Sin(delta * Math.PI / 180) + (85.5952637) * Math.Cos(delta * Math.PI / 180) + y0;*/
                    xc2 = (-372.4999887) * Math.Cos(delta * Math.PI / 180) - (81.8952637) * Math.Sin(delta * Math.PI / 180) + x0;
                    yc2 = (-372.4999887) * Math.Sin(delta * Math.PI / 180) + (81.8952637) * Math.Cos(delta * Math.PI / 180) + y0;
                    delta_sidewear = arrHeadX.Zip(arrHeadY, (x, y) => GetDistanceBetweenPoints(x, xc2, y, yc2)).Min();
                    break;
                case Rails.r50:
                    /*xc1 = (-357.9999183) * Math.Cos(delta * Math.PI / 180) - (71.2608731) * Math.Sin(delta * Math.PI / 180) + x0;
                    yc1 = (-357.9999183) * Math.Sin(delta * Math.PI / 180) + (71.2608731) * Math.Cos(delta * Math.PI / 180) + y0;*/
                    xc2 = (-322.8999183) * Math.Cos(delta * Math.PI / 180) - (67.8608731) * Math.Sin(delta * Math.PI / 180) + x0;
                    yc2 = (-322.8999183) * Math.Sin(delta * Math.PI / 180) + (67.8608731) * Math.Cos(delta * Math.PI / 180) + y0;
                    delta_sidewear = arrHeadX.Zip(arrHeadY, (x, y) => GetDistanceBetweenPoints(x, xc2, y, yc2)).Min();
                    break;
                case Rails.r43:
                    /*xc1 = (-357.2103467) * Math.Cos(delta * Math.PI / 180) - (64.7683742) * Math.Sin(delta * Math.PI / 180) + x0;
                    yc1 = (-357.2103467) * Math.Sin(delta * Math.PI / 180) + (64.7683742) * Math.Cos(delta * Math.PI / 180) + y0;*/
                    xc2 = (-319.2103467) * Math.Cos(delta * Math.PI / 180) - (62.8683742) * Math.Sin(delta * Math.PI / 180) + x0;
                    yc2 = (-319.2103467) * Math.Sin(delta * Math.PI / 180) + (62.8683742) * Math.Cos(delta * Math.PI / 180) + y0;
                    delta_sidewear = arrHeadX.Zip(arrHeadY, (x, y) => GetDistanceBetweenPoints(x, xc2, y, yc2)).Min();
                    break;
                default:
                    delta_sidewear = -1;
                    break;
            }

            /*distArr.Clear();
            distArr = arrHeadX.Zip(arrHeadY, (x, y) => GetDistanceBetweenPoints(x, xc2, y, yc2)).ToList();
            index = distArr.IndexOf(distArr.Min());
            y2 = arrHeadY.Get(index, (index + 3) % arrHeadY.Length).Average();
            y1 = arrHeadY.Get((index - 2) % arrHeadY.Length, (index + 1) % arrHeadY.Length).Average();
            x2 = arrHeadX.Get(index, (index + 3) % arrHeadY.Length).Average();
            x1 = arrHeadX.Get((index - 2) % arrHeadY.Length, (index + 1) % arrHeadY.Length).Average();

            A = (yc2 - yc1) / (xc2 - xc1);
            B = (y2 - y1) / (x2 - x1);
            xc = (A * xc1 - yc1 - B * x1 + y1) / (A - B);
            yc = A * (xc - xc1) + yc1;

            delta_sidewear = Math.Sqrt(Math.Pow((xc2 - xc), 2.0) + Math.Pow((yc2 - yc), 2.0));
            delta_sidewear = distArr.Min();*/
        }

        /// <summary>
        /// Схема профиля рельса по предпологаемому центру и подуклонке
        /// </summary>
        /// <param name="x0">предпологаемый центр окружности по оси Х</param>
        /// <param name="y0">предпологаемый центр окружности по оси Y</param>
        /// <param name="delta">подуклонка рельса</param>
        /// <param name="rails">тип рельса</param>
        /// <param name="side">сторона рельса</param>
        /// <returns>массив точек схемы профиля рельса (на нечетных позициях - координата по оси Х, на четных - по оси Y)</returns>
        private void GetProfileDiagram(ref List<double> vector, double x0, double y0, double delta, Rails rails, Side side, double xMax, double yMax)
        {
            List<double> graphPoints = new List<double>();
            double x, y, a1, a2, rx, ry, new_rx = 0, new_ry = 0, lx, ly, new_lx, new_ly;
            double[] lx_list, ly_list;

            switch (rails)
            {
                case Rails.r75:
                    // r450 - right side
                    a1 = 4.8058428;
                    a2 = (184.8331142 + delta) % 360;
                    CirclePoints(ref graphPoints, 450, a1, a2, x0, y0);
                    // r450 - left side
                    rx = -919.999898;
                    ry = 0;
                    a1 = 4.8058428;
                    a2 = (359.9727286 + delta) % 360;
                    TurnPoints(ref new_rx, ref new_ry, rx, ry, x0, y0, delta);
                    CirclePoints(ref graphPoints, 450, a1, a2, new_rx, new_ry);
                    // r450 - right side
                    rx = 0.0000366;
                    ry = -0.3281189;
                    a1 = 5.9056276;
                    a2 = (179.9855164 + delta) % 360;
                    TurnPoints(ref new_rx, ref new_ry, rx, ry, x0, y0, delta);
                    CirclePoints(ref graphPoints, 450, a1, a2, new_rx, new_ry);
                    // r450 - left side
                    rx = -919.9999346;
                    ry = -0.3281189;
                    a1 = 5.9056276;
                    a2 = (5.9201112 + delta) % 360;
                    TurnPoints(ref new_rx, ref new_ry, rx, ry, x0, y0, delta);
                    CirclePoints(ref graphPoints, 450, a1, a2, new_rx, new_ry);
                    // lines - right side
                    lx_list = new double[] { -447.599949, -440.599949, -422.499949, -423.999949 };
                    ly_list = new double[] { 46.0858112, 53.2858112, 61.2858112, 91.6858112 };
                    LinePoints(ref graphPoints, lx_list, ly_list, delta, x0, y0);
                    // lines - left side
                    lx_list = new double[] { -472.599949, -479.599949, -497.499949, -495.999949 };
                    ly_list = new double[] { 46.0858112, 53.2858112, 61.2858112, 91.6858112 };
                    LinePoints(ref graphPoints, lx_list, ly_list, delta, x0, y0);
                    // r15 - right side
                    rx = -438.9826399;
                    ry = 90.965414;
                    a1 = 76.1672279;
                    a2 = (78.9200016 + delta) % 360;
                    TurnPoints(ref new_rx, ref new_ry, rx, ry, x0, y0, delta);
                    CirclePoints(ref graphPoints, 15, a1, a2, new_rx, new_ry);
                    // r15 - left side
                    rx = -481.0172581;
                    ry = 90.965414;
                    a1 = 76.1672279;
                    a2 = (177.2472263 + delta) % 360;
                    TurnPoints(ref new_rx, ref new_ry, rx, ry, x0, y0, delta);
                    CirclePoints(ref graphPoints, 15, a1, a2, new_rx, new_ry);
                    // r80 - right side
                    rx = -452.1631287;
                    ry = 27.3150625;
                    a1 = 10.0336928;
                    a2 = (88.4505478 + delta) % 360;
                    TurnPoints(ref new_rx, ref new_ry, rx, ry, x0, y0, delta);
                    CirclePoints(ref graphPoints, 80, a1, a2, new_rx, new_ry);
                    // r80 - left side
                    rx = -467.8367693;
                    ry = 27.3150625;
                    a1 = 10.0336928;
                    a2 = (101.583145 + delta) % 360;
                    TurnPoints(ref new_rx, ref new_ry, rx, ry, x0, y0, delta);
                    CirclePoints(ref graphPoints, 80, a1, a2, new_rx, new_ry);
                    // r500
                    rx = -459.999949;
                    ry = -392.6141788;
                    a1 = 2.291984;
                    a2 = (91.145992 + delta) % 360;
                    TurnPoints(ref new_rx, ref new_ry, rx, ry, x0, y0, delta);
                    CirclePoints(ref graphPoints, 500, a1, a2, new_rx, new_ry);
                    // r25 - right side
                    rx = -423.483336;
                    ry = -35.873994;
                    a1 = 71.1570034;
                    a2 = (255.8379913 + delta) % 360;
                    TurnPoints(ref new_rx, ref new_ry, rx, ry, x0, y0, delta);
                    CirclePoints(ref graphPoints, 25, a1, a2, new_rx, new_ry);
                    // r25 - left side
                    rx = -496.516562;
                    ry = -35.873994;
                    a1 = 71.1570034;
                    a2 = (355.3190121 + delta) % 360;
                    TurnPoints(ref new_rx, ref new_ry, rx, ry, x0, y0, delta);
                    CirclePoints(ref graphPoints, 25, a1, a2, new_rx, new_ry);
                    // lines
                    lx_list = new double[] { -429.599949, -384.999949, -384.999949, -534.999949, -534.999949, -490.399949 };
                    ly_list = new double[] { -60.1141888, -71.2141888, -84.7141888, -84.7141888, -71.2141888, -60.1141888 };
                    LinePoints(ref graphPoints, lx_list, ly_list, delta, x0, y0);
                    // lines
                    lx_list = new double[] { -459.999949, -459.999949 };
                    ly_list = new double[] { -84.7141888, 107.2858112 };
                    LinePoints(ref graphPoints, lx_list, ly_list, delta, x0, y0);
                    break;
                case Rails.r65:

                    break;
                case Rails.r50:
                    // r350 - right side
                    a1 = 4.9008146;
                    a2 = (184.9399603 + delta) % 360;
                    CirclePoints(ref graphPoints, 350, a1, a2, x0, y0);
                    /*// r350 - left side
                    rx = -715.9998366;
                    ry = 0;
                    a1 = 4.9008146;
                    a2 = (359.9608543 + delta) % 360;
                    TurnPoints(ref new_rx, ref new_ry, rx, ry, x0, y0, delta);
                    CirclePoints(ref graphPoints, 350, a1, a2, new_rx, new_ry);*/
                    // r325 - right side
                    rx = -24.9999382;
                    ry = -0.1254771;
                    a1 = 6.5369467;
                    a2 = (180.0200649 + delta) % 360;
                    TurnPoints(ref new_rx, ref new_ry, rx, ry, x0, y0, delta);
                    CirclePoints(ref graphPoints, 325, a1, a2, new_rx, new_ry);
                    /*// r325 - left side
                    rx = -690.9998984;
                    ry = -0.1254771;
                    a1 = 6.5369467;
                    a2 = (6.5168818 + delta) % 360;
                    TurnPoints(ref new_rx, ref new_ry, rx, ry, x0, y0, delta);
                    CirclePoints(ref graphPoints, 325, a1, a2, new_rx, new_ry);*/
                    // lines - right side
                    lx_list = new double[] { -347.8999183, -341.9999183, -321.9999183, -322.8999183 };
                    ly_list = new double[] { 36.7608731, 43.2608731, 50.2608731, 67.8608731 };
                    LinePoints(ref graphPoints, lx_list, ly_list, delta, x0, y0);
                    /*// lines - left side
                    lx_list = new double[] { -368.0999183, -373.9999183, -393.9999183, -393.0999183 };
                    ly_list = new double[] { 36.7608731, 43.2608731, 50.2608731, 67.8608731 };
                    LinePoints(ref graphPoints, lx_list, ly_list, delta, x0, y0);*/
                    // r15 - right side
                    rx = -337.8481878;
                    ry = 67.1055574;
                    a1 = 84.3967767;
                    a2 = (79.6369599 + delta) % 360;
                    TurnPoints(ref new_rx, ref new_ry, rx, ry, x0, y0, delta);
                    CirclePoints(ref graphPoints, 15, a1, a2, new_rx, new_ry);
                    /*// r15 - left side
                    rx = -378.1516488;
                    ry = 67.1055574;
                    a1 = 84.3967767;
                    a2 = (184.7598168 + delta) % 360;
                    TurnPoints(ref new_rx, ref new_ry, rx, ry, x0, y0, delta);
                    CirclePoints(ref graphPoints, 15, a1, a2, new_rx, new_ry);*/
                    // r80 - right side
                    rx = -350.2112765;
                    ry = 3.2914421;
                    a1 = 9.2676938;
                    a2 = (88.4160296 + delta) % 360;
                    TurnPoints(ref new_rx, ref new_ry, rx, ry, x0, y0, delta);
                    CirclePoints(ref graphPoints, 80, a1, a2, new_rx, new_ry);
                    /*// r80 - left side
                    rx = -365.7885601;
                    ry = 3.2914421;
                    a1 = 9.2676938;
                    a2 = (100.8516642 + delta) % 360;
                    TurnPoints(ref new_rx, ref new_ry, rx, ry, x0, y0, delta);
                    CirclePoints(ref graphPoints, 80, a1, a2, new_rx, new_ry);*/
                    // r500
                    rx = -357.9999183;
                    ry = -416.6391169;
                    a1 = 2.291984;
                    a2 = (91.145992 + delta) % 360;
                    TurnPoints(ref new_rx, ref new_ry, rx, ry, x0, y0, delta);
                    CirclePoints(ref graphPoints, 500, a1, a2, new_rx, new_ry);
                    // r20 - right side
                    rx = -328.7730516;
                    ry = -28.430331;
                    a1 = 71.1328461;
                    a2 = (256.0341613 + delta) % 360;
                    TurnPoints(ref new_rx, ref new_ry, rx, ry, x0, y0, delta);
                    CirclePoints(ref graphPoints, 20, a1, a2, new_rx, new_ry);
                    /*// r20 - left side
                    rx = -387.226785;
                    ry = -28.430331;
                    a1 = 71.1328461;
                    a2 = (355.0986848 + delta) % 360;
                    TurnPoints(ref new_rx, ref new_ry, rx, ry, x0, y0, delta);
                    CirclePoints(ref graphPoints, 20, a1, a2, new_rx, new_ry);*/
                    // lines
                    lx_list = new double[] { -333.5999183, -291.9999183, -291.9999183, -423.9999183, -423.9999183, -382.9999183 };
                    ly_list = new double[] { -47.8391269, -58.2391269, -68.7391269, -68.7391269, -58.2391269, -47.8391269 };
                    LinePoints(ref graphPoints, lx_list, ly_list, delta, x0, y0);
                    // lines
                    lx_list = new double[] { -357.9999183, -357.9999183 };
                    ly_list = new double[] { -68.7391269, 83.2608731 };
                    LinePoints(ref graphPoints, lx_list, ly_list, delta, x0, y0);
                    break;
                case Rails.r43:
                    // r350 - right side
                    a1 = 5.6861962;
                    a2 = (184.8237189 + delta) % 360;
                    CirclePoints(ref graphPoints, 350, a1, a2, x0, y0);
                    // r350 - left side
                    rx = -715.9998366;
                    ry = 0;
                    a1 = 5.6861962;
                    a2 = (0.8624773 + delta) % 360;
                    TurnPoints(ref new_rx, ref new_ry, rx, ry, x0, y0, delta);
                    CirclePoints(ref graphPoints, 350, a1, a2, new_rx, new_ry);
                    // r325 - right side
                    rx = -24.9999382;
                    ry = -0.1254771;
                    a1 = 6.5369467;
                    a2 = (180.0200649 + delta) % 360;
                    TurnPoints(ref new_rx, ref new_ry, rx, ry, x0, y0, delta);
                    CirclePoints(ref graphPoints, 325, a1, a2, new_rx, new_ry);
                    // r325 - left side
                    rx = -690.9998984;
                    ry = -0.1254771;
                    a1 = 6.5369467;
                    a2 = (6.5168818 + delta) % 360;
                    TurnPoints(ref new_rx, ref new_ry, rx, ry, x0, y0, delta);
                    CirclePoints(ref graphPoints, 325, a1, a2, new_rx, new_ry);
                    // lines - right side
                    lx_list = new double[] { -347.8999183, -341.9999183, -321.9999183, -322.8999183 };
                    ly_list = new double[] { 36.7608731, 43.2608731, 50.2608731, 67.8608731 };
                    LinePoints(ref graphPoints, lx_list, ly_list, delta, x0, y0);
                    // lines - left side
                    lx_list = new double[] { -368.0999183, -373.9999183, -393.9999183, -393.0999183 };
                    ly_list = new double[] { 36.7608731, 43.2608731, 50.2608731, 67.8608731 };
                    LinePoints(ref graphPoints, lx_list, ly_list, delta, x0, y0);
                    // r15 - right side
                    rx = -337.8481878;
                    ry = 67.1055574;
                    a1 = 84.3967767;
                    a2 = (79.6369599 + delta) % 360;
                    TurnPoints(ref new_rx, ref new_ry, rx, ry, x0, y0, delta);
                    CirclePoints(ref graphPoints, 15, a1, a2, new_rx, new_ry);
                    // r15 - left side
                    rx = -378.1516488;
                    ry = 67.1055574;
                    a1 = 84.3967767;
                    a2 = (184.7598168 + delta) % 360;
                    TurnPoints(ref new_rx, ref new_ry, rx, ry, x0, y0, delta);
                    CirclePoints(ref graphPoints, 15, a1, a2, new_rx, new_ry);
                    // r80 - right side
                    rx = -350.2112765;
                    ry = 3.2914421;
                    a1 = 9.2676938;
                    a2 = (88.4160296 + delta) % 360;
                    TurnPoints(ref new_rx, ref new_ry, rx, ry, x0, y0, delta);
                    CirclePoints(ref graphPoints, 80, a1, a2, new_rx, new_ry);
                    // r80 - left side
                    rx = -365.7885601;
                    ry = 3.2914421;
                    a1 = 9.2676938;
                    a2 = (100.8516642 + delta) % 360;
                    TurnPoints(ref new_rx, ref new_ry, rx, ry, x0, y0, delta);
                    CirclePoints(ref graphPoints, 80, a1, a2, new_rx, new_ry);
                    // r500
                    rx = -357.9999183;
                    ry = -416.6391169;
                    a1 = 2.291984;
                    a2 = (91.145992 + delta) % 360;
                    TurnPoints(ref new_rx, ref new_ry, rx, ry, x0, y0, delta);
                    CirclePoints(ref graphPoints, 500, a1, a2, new_rx, new_ry);
                    // r20 - right side
                    rx = -328.7730516;
                    ry = -28.430331;
                    a1 = 71.1328461;
                    a2 = (256.0341613 + delta) % 360;
                    TurnPoints(ref new_rx, ref new_ry, rx, ry, x0, y0, delta);
                    CirclePoints(ref graphPoints, 20, a1, a2, new_rx, new_ry);
                    // r20 - left side
                    rx = -387.226785;
                    ry = -28.430331;
                    a1 = 71.1328461;
                    a2 = (355.0986848 + delta) % 360;
                    TurnPoints(ref new_rx, ref new_ry, rx, ry, x0, y0, delta);
                    CirclePoints(ref graphPoints, 20, a1, a2, new_rx, new_ry);
                    // lines
                    lx_list = new double[] { -333.5999183, -291.9999183, -291.9999183, -423.9999183, -423.9999183, -382.9999183 };
                    ly_list = new double[] { -47.8391269, -58.2391269, -68.7391269, -68.7391269, -58.2391269, -47.8391269 };
                    LinePoints(ref graphPoints, lx_list, ly_list, delta, x0, y0);
                    // lines
                    lx_list = new double[] { -357.9999183, -357.9999183 };
                    ly_list = new double[] { -68.7391269, 83.2608731 };
                    LinePoints(ref graphPoints, lx_list, ly_list, delta, x0, y0);
                    break;
                default:
                    break;
            }

            if (graphPoints.Count > 0)
            {
                for (int i = 0; i < graphPoints.Count - 1; i += 2)
                {
                    if (side == Side.Left)
                        vector.Add(graphPoints[i]);
                    else
                        vector.Add(graphPoints[i] * (-1) + xMax);
                    vector.Add(graphPoints[i + 1] * (-1) + yMax);
                }
            }
        }

        /// <summary>
        /// Поворот точки на угол
        /// </summary>
        /// <param name="new_x">новая координата точки по оси Х (выходная данная)</param>
        /// <param name="new_y">новая координата точки по оси Y (выходная данная)</param>
        /// <param name="x">координата точки по оси Х</param>
        /// <param name="y">координата точки по оси Y</param>
        /// <param name="x0">координата предпологаемой окружности по оси Х</param>
        /// <param name="y0">координата предпологаемой окружности по оси Y</param>
        /// <param name="delta">угол</param>
        private void TurnPoints(ref double new_x, ref double new_y, double x, double y, double x0, double y0, double delta)
        {
            new_x = x * Math.Cos(DegreeToRadian(delta)) - y * Math.Sin(DegreeToRadian(delta)) + x0;
            new_y = x * Math.Sin(DegreeToRadian(delta)) + y * Math.Cos(DegreeToRadian(delta)) + y0;
        }

        /// <summary>
        /// Найти координаты точки окружности под определенным углом
        /// </summary>
        /// <param name="doubleList">Массив координат (выходная данная)</param>
        /// <param name="radius">радиус окружности</param>
        /// <param name="a1">угол между первыми и вторыми вершинами</param>
        /// <param name="a2">угол до второй вершины</param>
        /// <param name="x">координата центра окружнсоти по оси Х</param>
        /// <param name="y">координата центра окружнсоти по оси Y</param>
        private void CirclePoints(ref List<double> doubleList, double radius, double a1, double a2, double x, double y)
        {
            double stepX = 90.0 - RadianToDegree(Math.Acos(0.2 / radius));
            double stepY = RadianToDegree(Math.Asin(0.2 / radius));
            double step = Math.Min(stepX, stepY);
            double xn = 0, yn = 0;
            for (double a = (a2 - a1) % 360; a < a2; a += step)
            {
                xn = radius * Math.Cos(DegreeToRadian(a)) + x;
                yn = radius * Math.Sin(DegreeToRadian(a)) + y;
                doubleList.Add(xn);
                doubleList.Add(yn);
            }

            xn = radius * Math.Cos(DegreeToRadian(a2)) + x;
            yn = radius * Math.Sin(DegreeToRadian(a2)) + y;
            doubleList.Add(xn);
            doubleList.Add(yn);
        }

        /// <summary>
        /// Найти координаты линии
        /// </summary>
        /// <param name="doubleList">Массив координат (выходная данная)</param>
        /// <param name="x_list">массив координат вершин линии по оси Х</param>
        /// <param name="y_list">массив координат вершин линии по оси Y</param>
        /// <param name="delta">подуклонка рельса</param>
        /// <param name="x0">координата предпологаемой окружности по оси Х</param>
        /// <param name="y0">координата предпологаемой окружности по оси Y</param>
        private void LinePoints(ref List<double> doubleList, double[] x_list, double[] y_list, double delta, double x0, double y0)
        {
            double x, y, new_x = 0, new_y = 0;

            for (int i = 0; i < x_list.Length - 1; i++)
            {
                if (Math.Abs(x_list[i] - x_list[i + 1]) > Math.Abs(y_list[i] - y_list[i + 1]))
                {
                    for (x = Math.Min(x_list[i], x_list[i + 1]); x < Math.Max(x_list[i], x_list[i + 1]); x += 0.2)
                    {
                        y = ((y_list[i + 1] - y_list[i]) / (x_list[i + 1] - x_list[i])) * (x - x_list[i]) + y_list[i];
                        TurnPoints(ref new_x, ref new_y, x, y, x0, y0, delta);
                        doubleList.Add(new_x);
                        doubleList.Add(new_y);
                    }
                }
                else
                {
                    for (y = Math.Min(y_list[i], y_list[i + 1]); y < Math.Max(y_list[i], y_list[i + 1]); y += 0.2)
                    {
                        x = ((x_list[i + 1] - x_list[i]) / (y_list[i + 1] - y_list[i])) * (y - y_list[i]) + x_list[i];
                        TurnPoints(ref new_x, ref new_y, x, y, x0, y0, delta);
                        doubleList.Add(new_x);
                        doubleList.Add(new_y);
                    }
                }
            }
        }

        public string GetNominalRailScheme(Rails rails)
        {
            string nominalPolyline = string.Empty;

            switch (rails)
            {
                case Rails.r75:

                    break;
                case Rails.r65:
                    nominalPolyline += (-36.5).ToString().Replace(',', '.') + "," + (15.7).ToString().Replace(',', '.') + " ";
                    nominalPolyline += (-37.5).ToString().Replace(',', '.') + "," + (35.6).ToString().Replace(',', '.') + " ";
                    nominalPolyline += (-19.6).ToString().Replace(',', '.') + "," + (43.0).ToString().Replace(',', '.') + " ";
                    nominalPolyline += (-12.0).ToString().Replace(',', '.') + "," + (50.8).ToString().Replace(',', '.') + " ";
                    nominalPolyline += (-9.0).ToString().Replace(',', '.') + "," + (97.5).ToString().Replace(',', '.') + " ";
                    nominalPolyline += (-10.8).ToString().Replace(',', '.') + "," + (135.5).ToString().Replace(',', '.') + " ";
                    NominalCirclePoints(ref nominalPolyline, 25, 70.5150039, 354.6131441, -35.6895882, 133.1530019, true);
                    nominalPolyline += (-29.6).ToString().Replace(',', '.') + "," + (157.4).ToString().Replace(',', '.') + " ";
                    nominalPolyline += (-75.0).ToString().Replace(',', '.') + "," + (168.8).ToString().Replace(',', '.') + " ";
                    nominalPolyline += (-75.0).ToString().Replace(',', '.') + "," + (180.0).ToString().Replace(',', '.') + " ";
                    nominalPolyline += (75.0).ToString().Replace(',', '.') + "," + (180.0).ToString().Replace(',', '.') + " ";
                    nominalPolyline += (75.0).ToString().Replace(',', '.') + "," + (168.8).ToString().Replace(',', '.') + " ";
                    nominalPolyline += (29.6).ToString().Replace(',', '.') + "," + (157.4).ToString().Replace(',', '.') + " ";
                    NominalCirclePoints(ref nominalPolyline, 25, 70.5150039, 255.9018598, 35.6895882, 133.1530019, true);
                    nominalPolyline += (10.8).ToString().Replace(',', '.') + "," + (135.5).ToString().Replace(',', '.') + " ";
                    nominalPolyline += (9.0).ToString().Replace(',', '.') + "," + (97.5).ToString().Replace(',', '.') + " ";
                    nominalPolyline += (12.0).ToString().Replace(',', '.') + "," + (50.8).ToString().Replace(',', '.') + " ";
                    nominalPolyline += (19.6).ToString().Replace(',', '.') + "," + (43.0).ToString().Replace(',', '.') + " ";
                    nominalPolyline += (37.5).ToString().Replace(',', '.') + "," + (35.6).ToString().Replace(',', '.') + " ";
                    nominalPolyline += (36.5).ToString().Replace(',', '.') + "," + (15.7).ToString().Replace(',', '.') + " ";
                    NominalCirclePoints(ref nominalPolyline, 15, 75.6937003, 78.3299747, 21.5158753, 16.3899315, false);
                    nominalPolyline += (24.55).ToString().Replace(',', '.') + "," + (1.7).ToString().Replace(',', '.') + " ";
                    nominalPolyline += (10.0).ToString().Replace(',', '.') + "," + (0.0).ToString().Replace(',', '.') + " ";
                    nominalPolyline += (-10.0).ToString().Replace(',', '.') + "," + (0.0).ToString().Replace(',', '.') + " ";
                    nominalPolyline += (-24.55).ToString().Replace(',', '.') + "," + (1.7).ToString().Replace(',', '.') + " ";
                    NominalCirclePoints(ref nominalPolyline, 15, 75.6937003, 177.3637256, -21.5158753, 16.3899315, false);
                    break;
                case Rails.r50:
                    nominalPolyline += (-35.1).ToString().Replace(',', '.') + "," + (15.4).ToString().Replace(',', '.') + " ";
                    nominalPolyline += (-36.0).ToString().Replace(',', '.') + "," + (33.0).ToString().Replace(',', '.') + " ";
                    nominalPolyline += (-16.0).ToString().Replace(',', '.') + "," + (39.0).ToString().Replace(',', '.') + " ";
                    nominalPolyline += (-10.1).ToString().Replace(',', '.') + "," + (46.5).ToString().Replace(',', '.') + " ";
                    nominalPolyline += (-8.0).ToString().Replace(',', '.') + "," + (83.5).ToString().Replace(',', '.') + " ";
                    nominalPolyline += (-9.3).ToString().Replace(',', '.') + "," + (113.4).ToString().Replace(',', '.') + " ";
                    NominalCirclePoints(ref nominalPolyline, 20, 71.1328461, 355.0986847, -29.2268667, 111.6912041, true);
                    nominalPolyline += (-24.4).ToString().Replace(',', '.') + "," + (131.1).ToString().Replace(',', '.') + " ";
                    nominalPolyline += (-66.0).ToString().Replace(',', '.') + "," + (141.5).ToString().Replace(',', '.') + " ";
                    nominalPolyline += (-66.0).ToString().Replace(',', '.') + "," + (152.0).ToString().Replace(',', '.') + " ";
                    nominalPolyline += (66.0).ToString().Replace(',', '.') + "," + (152.0).ToString().Replace(',', '.') + " ";
                    nominalPolyline += (66.0).ToString().Replace(',', '.') + "," + (141.5).ToString().Replace(',', '.') + " ";
                    nominalPolyline += (24.4).ToString().Replace(',', '.') + "," + (131.1).ToString().Replace(',', '.') + " ";
                    NominalCirclePoints(ref nominalPolyline, 20, 71.1328461, 256.0341613, 29.2268667, 111.6912041, true);
                    nominalPolyline += (9.3).ToString().Replace(',', '.') + "," + (113.4).ToString().Replace(',', '.') + " ";
                    nominalPolyline += (8.0).ToString().Replace(',', '.') + "," + (83.5).ToString().Replace(',', '.') + " ";
                    nominalPolyline += (10.1).ToString().Replace(',', '.') + "," + (46.5).ToString().Replace(',', '.') + " ";
                    nominalPolyline += (16.0).ToString().Replace(',', '.') + "," + (39.0).ToString().Replace(',', '.') + " ";
                    nominalPolyline += (36.0).ToString().Replace(',', '.') + "," + (33.0).ToString().Replace(',', '.') + " ";
                    nominalPolyline += (35.1).ToString().Replace(',', '.') + "," + (15.4).ToString().Replace(',', '.') + " ";
                    NominalCirclePoints(ref nominalPolyline, 15, 84.3967767, 79.6369599, 20.1517305, 16.1553157, false);
                    nominalPolyline += (22.85).ToString().Replace(',', '.') + "," + (1.4).ToString().Replace(',', '.') + " ";
                    nominalPolyline += (10.0).ToString().Replace(',', '.') + "," + (0.0).ToString().Replace(',', '.') + " ";
                    nominalPolyline += (-10.0).ToString().Replace(',', '.') + "," + (0.0).ToString().Replace(',', '.') + " ";
                    nominalPolyline += (-22.85).ToString().Replace(',', '.') + "," + (1.4).ToString().Replace(',', '.') + " ";
                    NominalCirclePoints(ref nominalPolyline, 15, 84.3967767, 184.7598168, -20.1517305, 16.1553157, false);
                    break;
                case Rails.r43:
                    break;
                default:
                    break;
            }
            return nominalPolyline;
        }
        private void NominalCirclePoints(ref string nominalPolyline, double radius, double a1, double a2, double x, double y, bool mode)
        {
            double stepX = 90.0 - RadianToDegree(Math.Acos(0.2 / radius));
            double stepY = RadianToDegree(Math.Asin(0.2 / radius));
            double step = Math.Min(stepX, stepY);
            double a = (a2 - a1) % 360.0;
            double xn = 0, yn = 0;

            if (mode)
            {
                while (!IsClose(a, a2, 0.0, step))
                {
                    xn = radius * Math.Cos(DegreeToRadian(a2)) + x;
                    yn = radius * Math.Sin(DegreeToRadian(a2)) * (-1) + y;
                    nominalPolyline += xn.ToString().Replace(',', '.') + "," + yn.ToString().Replace(',', '.') + " ";

                    a2 -= step;
                }

                xn = radius * Math.Cos(DegreeToRadian(a)) + x;
                yn = radius * Math.Sin(DegreeToRadian(a)) * (-1) + y;
                nominalPolyline += xn.ToString().Replace(',', '.') + "," + yn.ToString().Replace(',', '.') + " ";
            }
            else
            {
                while (!IsClose(a, a2, 0.0, step))
                {
                    xn = radius * Math.Cos(DegreeToRadian(a)) + x;
                    yn = radius * Math.Sin(DegreeToRadian(a)) * (-1) + y;
                    nominalPolyline += xn.ToString().Replace(',', '.') + "," + yn.ToString().Replace(',', '.') + " ";

                    a += step;
                }

                xn = radius * Math.Cos(DegreeToRadian(a2)) + x;
                yn = radius * Math.Sin(DegreeToRadian(a2)) * (-1) + y;
                nominalPolyline += xn.ToString().Replace(',', '.') + "," + yn.ToString().Replace(',', '.') + " ";
            }
        }

        /// <summary>
        /// Найти наклон поверхности катания
        /// </summary>
        /// <param name="SSS">Наклон поверхности катания</param>
        /// <param name="arrHeadX">массив координат головки рельса по оси Х</param>
        /// <param name="arrHeadY">массив координат головки рельса по оси Y</param>
        /// <param name="rails">тип рельса</param>
        /// <param name="delta">подуклонка рельса</param>
        /// <param name="x0">предпологаемый центр окружности по оси Х</param>
        /// <param name="y0">предпологаемый центр окружности по оси Y</param>
        private void GetSSS(ref double SSS, double[] arrHeadX, double[] arrHeadY, Rails rails, double delta, double x0, double y0)
        {
            double xc1 = -1, yc1 = -1, xc2 = -1, yc2 = -1;
            switch (rails)
            {
                case Rails.r75:
                    xc2 = (-459.999949) * Math.Cos(delta * Math.PI / 180) - (82.2858112) * Math.Sin(delta * Math.PI / 180) + x0;
                    yc2 = (-459.999949) * Math.Sin(delta * Math.PI / 180) + (82.2858112) * Math.Cos(delta * Math.PI / 180) + y0;
                    break;
                case Rails.r65:
                    xc2 = (-408.9999887) * Math.Cos(delta * Math.PI / 180) - (97.5952637) * Math.Sin(delta * Math.PI / 180) + x0;
                    yc2 = (-408.9999887) * Math.Sin(delta * Math.PI / 180) + (97.5952637) * Math.Cos(delta * Math.PI / 180) + y0;
                    break;
                case Rails.r50:
                    xc2 = (-357.9999183) * Math.Cos(delta * Math.PI / 180) - (83.2608731) * Math.Sin(delta * Math.PI / 180) + x0;
                    yc2 = (-357.9999183) * Math.Sin(delta * Math.PI / 180) + (83.2608731) * Math.Cos(delta * Math.PI / 180) + y0;
                    break;
                case Rails.r43:
                    xc2 = (-357.2103467) * Math.Cos(delta * Math.PI / 180) - (76.7683742) * Math.Sin(delta * Math.PI / 180) + x0;
                    yc2 = (-357.2103467) * Math.Sin(delta * Math.PI / 180) + (76.7683742) * Math.Cos(delta * Math.PI / 180) + y0;
                    break;
                default:
                    SSS = -1;
                    return;
            }

            var temp = arrHeadX.Zip(arrHeadY, (x, y) => new {
                x = x,
                y = y,
                dist = GetDistanceBetweenPoints(x, xc2, y, yc2)
            }).OrderBy(t => t.dist).First();

            switch (rails)
            {
                case Rails.r75:
                    xc2 = (-419.999949) * Math.Cos(delta * Math.PI / 180) - (135.2858112) * Math.Sin(delta * Math.PI / 180) + x0;
                    yc2 = (-419.999949) * Math.Sin(delta * Math.PI / 180) + (135.2858112) * Math.Cos(delta * Math.PI / 180) + y0;
                    break;
                case Rails.r65:
                    xc2 = (-368.9999887) * Math.Cos(delta * Math.PI / 180) - (125.5952637) * Math.Sin(delta * Math.PI / 180) + x0;
                    yc2 = (-368.9999887) * Math.Sin(delta * Math.PI / 180) + (125.5952637) * Math.Cos(delta * Math.PI / 180) + y0;
                    break;
                case Rails.r50:
                    xc2 = (-327.2415861) * Math.Cos(delta * Math.PI / 180) - (77.7121591) * Math.Sin(delta * Math.PI / 180) + x0;
                    yc2 = (-327.2415861) * Math.Sin(delta * Math.PI / 180) + (77.7121591) * Math.Cos(delta * Math.PI / 180) + y0;
                    break;
                case Rails.r43:
                    xc2 = (-317.2103467) * Math.Cos(delta * Math.PI / 180) - (104.7683742) * Math.Sin(delta * Math.PI / 180) + x0;
                    yc2 = (-317.2103467) * Math.Sin(delta * Math.PI / 180) + (104.7683742) * Math.Cos(delta * Math.PI / 180) + y0;
                    break;
                default:
                    SSS = -1;
                    return;
            }

            var temp2 = arrHeadX.Zip(arrHeadY, (x, y) => new {
                x = x,
                y = y,
                dist = GetDistanceBetweenPoints(x, xc2, y, yc2)
            }).OrderBy(t => t.dist).First();

            switch (rails)
            {
                case Rails.r75:
                    xc1 = (-459.999949) * Math.Cos(delta * Math.PI / 180) - (-84.7141888) * Math.Sin(delta * Math.PI / 180) + x0;
                    yc1 = (-459.999949) * Math.Sin(delta * Math.PI / 180) + (-84.7141888) * Math.Cos(delta * Math.PI / 180) + y0;
                    xc2 = (-459.999949) * Math.Cos(delta * Math.PI / 180) - (107.2858112) * Math.Sin(delta * Math.PI / 180) + x0;
                    yc2 = (-459.999949) * Math.Sin(delta * Math.PI / 180) + (107.2858112) * Math.Cos(delta * Math.PI / 180) + y0;
                    break;
                case Rails.r65:
                    xc1 = (-408.9999887) * Math.Cos(delta * Math.PI / 180) - (-82.4047363) * Math.Sin(delta * Math.PI / 180) + x0;
                    yc1 = (-408.9999887) * Math.Sin(delta * Math.PI / 180) + (-82.4047363) * Math.Cos(delta * Math.PI / 180) + y0;
                    xc2 = (-408.9999887) * Math.Cos(delta * Math.PI / 180) - (97.5952637) * Math.Sin(delta * Math.PI / 180) + x0;
                    yc2 = (-408.9999887) * Math.Sin(delta * Math.PI / 180) + (97.5952637) * Math.Cos(delta * Math.PI / 180) + y0;
                    break;
                case Rails.r50:
                    xc1 = (-357.9999183) * Math.Cos(delta * Math.PI / 180) - (-68.7391269) * Math.Sin(delta * Math.PI / 180) + x0;
                    yc1 = (-357.9999183) * Math.Sin(delta * Math.PI / 180) + (-68.7391269) * Math.Cos(delta * Math.PI / 180) + y0;
                    xc2 = (-357.9999183) * Math.Cos(delta * Math.PI / 180) - (83.2608731) * Math.Sin(delta * Math.PI / 180) + x0;
                    yc2 = (-357.9999183) * Math.Sin(delta * Math.PI / 180) + (83.2608731) * Math.Cos(delta * Math.PI / 180) + y0;
                    break;
                case Rails.r43:
                    xc1 = (-357.2103467) * Math.Cos(delta * Math.PI / 180) - (-63.2316258) * Math.Sin(delta * Math.PI / 180) + x0;
                    yc1 = (-357.2103467) * Math.Sin(delta * Math.PI / 180) + (-63.2316258) * Math.Cos(delta * Math.PI / 180) + y0;
                    xc2 = (-357.2103467) * Math.Cos(delta * Math.PI / 180) - (76.7683742) * Math.Sin(delta * Math.PI / 180) + x0;
                    yc2 = (-357.2103467) * Math.Sin(delta * Math.PI / 180) + (76.7683742) * Math.Cos(delta * Math.PI / 180) + y0;
                    break;
                default:
                    SSS = -1;
                    return;
            }
            double x1 = temp2.x - temp.x;
            double y1 = temp2.y - temp.y;
            double x2 = xc1 - xc2;
            double y2 = yc1 - yc2;
            double angle = RadianToDegree(Math.Acos(Math.Abs(x1 * x2 + y1 * y2) / (Math.Sqrt(x1 * x1 + y1 * y1) * Math.Sqrt(x2 * x2 + y2 * y2))));
            SSS = 1 / Math.Tan(DegreeToRadian(angle));
        }
    }


}



