using ALARm.Core;
using ALARm.Core.AdditionalParameteres;
using ALARm.Core.Report;
using ALARm.DataAccess;
using ALARm.Services;
using ALARm_Report.controls;
using MetroFramework.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Xsl;

namespace ALARm_Report.Forms
{
    public class BY_KILOMETER_SHEET : Report
    {
        public override void Process(long parentId, ReportTemplate template, ReportPeriod period, MetroProgressBar progressBar)
        {
            List<long> admTracksId = new List<long>();
            using (var choiceForm = new ChoiseForm(0))
            {
                choiceForm.SetTripsDataSource(parentId, period);
                choiceForm.ShowDialog();
                if (choiceForm.dialogResult == DialogResult.Cancel)
                    return;
                admTracksId = choiceForm.admTracksIDs;
            }

            XDocument htReport = new XDocument();
            using (XmlWriter writer = htReport.CreateWriter())
            {
                XDocument xdReport = new XDocument();

                var distance =
                    AdmStructureService.GetUnit(AdmStructureConst.AdmDistance, parentId) as AdmUnit;
                var road = AdmStructureService.GetRoadName(distance.Id, AdmStructureConst.AdmDistance, true);

                var tripProcesses = RdStructureService.GetMainParametersProcess(period, distance.Code);
                if (tripProcesses.Count == 0)
                {
                    MessageBox.Show(Properties.Resources.paramDataMissing);
                    return;
                }

                XElement report = new XElement("report");
                foreach (var tripProcess in tripProcesses)
                {
                    foreach (var track_id in admTracksId)
                    {
                        //  pdbSection = MainTrackStructureService.GetMtoObjectsByCoord(tripProcess.Date_Vrem, gap.Km, MainTrackStructureConst.MtoPdbSection, tripProcess.DirectionName, trackName.ToString()) as List<PdbSection>;
                        var directName = AdditionalParametersService.DirectName(tripProcess.Id, (int)distance.Id);
                        var trackName = AdmStructureService.GetTrackName(track_id);//tripProcess.Id);//

                        XElement tripElem = new XElement("trip",
                            new XAttribute("Poezdka", road),
                            new XAttribute("ps", tripProcess.Car),
                            new XAttribute("check", tripProcess.GetProcessTypeName),
                            new XAttribute("date_statement", period.Period),
                            new XAttribute("period", period.Period),
                            new XAttribute("type", tripProcess.GetProcessTypeName),
                            new XAttribute("car", tripProcess.Car),
                            new XAttribute("chief", tripProcess.Chief),
                            new XAttribute("trip_date", tripProcess.Date_Vrem.ToString("dd.MM.yyyy_hh:mm")),
                            new XAttribute("info", tripProcess.Car + " " + tripProcess.Chief),
                            new XAttribute("napr", road));

                        List<Digression> Check_sleepers_state = AdditionalParametersService.Check_sleepers_state(tripProcess.Trip_id, template.ID);
                        List<Digression> Check_deviationsinfastening_state = AdditionalParametersService.Check_deviationsinfastening_state(tripProcess.Trip_id, template.ID);
                        List<Digression> Check_Total_balast = AdditionalParametersService.Check_Total_balast(tripProcess.Trip_id, template.ID); // балласт
                        List<Digression> Check_Total_state_Jointless = AdditionalParametersService.Check_Total_state_Jointless(tripProcess.Trip_id, template.ID); // бесстыковой путь
                        List<Digression> Check_Total_state_rails = AdditionalParametersService.Check_Total_state_rails(tripProcess.Trip_id, template.ID) as List<Digression>; //
                        List<Digression> Check_bolt_state = AdditionalParametersService.Check_bolt_state(tripProcess.Trip_id, template.ID);
                        List<Digression> Check_badfastening_state = AdditionalParametersService.Check_badfastening_state(tripProcess.Trip_id, template.ID);

                        List<Gap> Check_ListOgDerogations_state = AdditionalParametersService.Check_ListOgDerogations_state(tripProcess.Trip_id, template.ID);


                        Check_ListOgDerogations_state = Check_ListOgDerogations_state.OrderBy(o => o.Km).ThenBy(o => o.Meter).ToList();
                     
                       



                        var SumOfTheDep = RdStructureService.SumOfTheDep(tripProcess.Id) as List<SumOfTheDep>;
                        //функция вывода шпал по км
                        List<Digression> Total = AdditionalParametersService.Total(tripProcess.Trip_id, template.ID);

                        //var kilometers = RdStructureService.GetKilometersByProcessId(tripProcess.Trip_id); RdStructureService.GetKilometersByTrip
                        var trips = RdStructureService.GetTrips();
                        var trip = trips.Where(o => o.Id == tripProcess.Trip_id).ToList().First();
                        var kilometers = RdStructureService.GetKilometersByTrip(trip);
                        //.Where(o => o.Number > 128)
                        kilometers = kilometers.OrderBy(o => o.Number).ToList();
                      

                        if (kilometers.Count() == 0)
                         return;

                        var ListS3 = RdStructureService.GetS3(kilometers.First().Trip.Id) as List<S3>;
                        //ListS3 = ListS3.Where(o => o.Km > 128).ToList();
                        foreach (var km in kilometers)
                        {
                            km.PchuCode = ListS3.Where(o => o.Km == km.Number && o.Pch == distance.Code).Select(o => o.Pchu).FirstOrDefault().ToString();
                        }

                        progressBar.Maximum = kilometers.Count();

                        int sumFastener = 0, sumGaps = 0, sumShpala = 0, sumNPK_L = 0, sumBallast = 0,
                            sumBezStyk = 0, sumItogoItogo = 0, sumProchie = 0, sumVsegoOgrSpeed = 0, sumKm = 0;

                        

                        var GroupByPchu = kilometers.GroupBy(x => new { _pchu = x.PchuCode, _chief = x.PchuChief });


                    
                        foreach (var kms in GroupByPchu)
                        {

                            XElement direct = new XElement("direction");
                            direct.Add(new XAttribute("name", tripProcess.DirectionName + " (" + tripProcess.DirectionCode + "), Путь: " + trackName));
                            var pchuInfo = new XAttribute("pchuInfo", "ПЧ: " + distance.Code + ",ПЧУ-" + kms.Key._pchu + "    " + kms.Key._chief);
                            direct.Add(pchuInfo);
                            direct.Add(new XAttribute("chief", tripProcess.Chief));

                            int Itogo_skrep = 0, Itogo_styk = 0, Itogo_shpal = 0, Itogo_NPK_L = 0, Itogo_ballast = 0,
                            Itogo_bezStyk = 0, Itogo_itogo = 0, Itogo_vsegoOgrSpeed = 0, Itogo_prochie = 0;

                            //если есть отст по безстык пути
                            List<DigressionMark> gapoid = GetBesPutDig(Check_ListOgDerogations_state);
                          

                            foreach (var km in kms)
                            {
                                int FasteningStateCnt = 0; // количество скреплении в км
                                int JointlessCnt = 0;      // количество бесстыковой пути в км
                                int BallastDigCnt = 0;     // количество отступлении по балласту
                                int SleepersDigCnt = 0;    // количество отступлении по шпалу
                                int Prochie = 0;
                                int NPK_L = 0;             // количество отступлении поверхности катания рельсов
                                int ItogCol = 0;
                                int GapsCnt = 0;
                                int VsegoOgrSpeed = 0;

                                //данные Поверхность   катания    рельсов
                                var DBcrossRailProfile = AdditionalParametersService.GetCrossRailProfileFromDBbyKm(km.Number, trip.Id);
                                if (DBcrossRailProfile == null) continue;

                                var sortedData = DBcrossRailProfile.OrderByDescending(d => d.Meter).ToList();
                                var crossRailProfile = AdditionalParametersService.GetCrossRailProfileFromDBParse(sortedData);
                                //----------------------------------------------------------------------------------------------------------------------
                                List<Digression> addDigressions = crossRailProfile.GetDigressions();
                                addDigressions = addDigressions.OrderBy(o => o.Km).ToList();
                                var dignatur = new List<DigressionMark> { };

                                foreach (var dig in addDigressions)
                                {
                                    int count = dig.Length / 4;
                                    count += dig.Length % 4 > 0 ? 1 : 0;

                                    if (dig.Length < 4 && (int)dig.Typ == 3) continue;

                                    if (
                                        dig.DigName == DigressionName.SideWearLeft || dig.DigName == DigressionName.SideWearRight ||
                                        dig.DigName == DigressionName.VertIznosL || dig.DigName == DigressionName.VertIznosR ||
                                        dig.DigName == DigressionName.ReducedWearLeft || dig.DigName == DigressionName.ReducedWearRight ||
                                        dig.DigName == DigressionName.HeadWearLeft || dig.DigName == DigressionName.HeadWearRight)
                                    {
                                        continue;
                                    }

                                    dignatur.Add(
                                        new DigressionMark()
                                        {
                                            Digression = dig.DigName,
                                            NotMoveAlert = false,
                                            Meter = dig.Kmetr,
                                            finish_meter = dig.Kmetr + dig.Length,
                                            Degree = (int)dig.Typ,
                                            Length = dig.Length,
                                            Value = dig.Value,
                                            Count = dig.Count,
                                            DigName = dig.GetName(),
                                            PassengerSpeedLimit = -1,
                                            FreightSpeedLimit = -1,
                                            Comment = "",
                                            Diagram_type = "GD_PR"

                                        });
                                }
                               
                                    km.Digressions.AddRange(dignatur);
                             
                                
                                //km.LoadDigresions(RdStructureRepository, MainTrackStructureRepository, trip, AdditionalParam: true);


                                progressBar.Value += 1; //km.LoadTrackPasport(MainTrackStructureService.GetRepository(), tripProcess.Date_Vrem);

                                GapsCnt = Total.Where(o => o.Km == km.Number && o.Total_gaps > 0).Select(o => o.Total_gaps).Sum();
                                //NPK_L = Check_sleepers_state.Where(o => o.Km == km.Number && o.Total_NPK_l > 0).Select(o => o.Total_NPK_l).Sum();

                                NPK_L = dignatur.Select(o => o.Degree).ToList().Count();
                                //NPK_L = Check_sleepers_state.Where(o => o.Km == km.Number && o.Total_NPK_l > 0).Select(o => o.Total_NPK_l).Sum();
                                SleepersDigCnt = Total.Where(o => o.Km == km.Number && o.Total_shpala > 0).Select(o => o.Total_shpala).Sum();
                                FasteningStateCnt = Total.Where(o => o.Km == km.Number && o.Total_b_fastener > 0).Select(o => o.Total_b_fastener).Sum();
                               // BallastDigCnt = Check_Total_balast.Where(o => o.Km == km.Number).ToList().Count();
                                var Ballast = RdStructureService.GetBallast(tripProcess);
                                BallastDigCnt = Ballast.Where(o => o.Km == km.Number).ToList().Count();

                                //JointlessCnt = Check_Total_state_Jointless.Where(o => o.Km == km.Number).ToList().Count;

                                // JointlessCnt = addDigressions.Select(o => o.Typ).ToList().Count();
                                 JointlessCnt = gapoid.Where(o => o.Km == km.Number).ToList().Count;
                            
                                //SleepersDigCnt = Check_sleepers_state.Where(o => o.Km == km.Number).ToList().Count;
                                //FasteningStateCnt = Check_deviationsinfastening_state.Where(o => o.Km == km.Number).ToList().Count;
                                //SurfaceRailDigCnt = Check_Total_state_rails.Where(o => o.Km == km.Number).ToList().Count;
                                // BadFasteningCnt = Check_badfastening_state.Where(o => o.Km == km.Number).ToList().Count;
                                //BadFasteningCnt += Check_bolt_state.Where(o => o.Km == km.Number).ToList().Count;
                                VsegoOgrSpeed = ListS3.Where(o => o.Km == km.Number && o.Pch == distance.Code && (o.Ovp > 0 || o.Ogp > 0)).Count();
                                ItogCol = GapsCnt + NPK_L + SleepersDigCnt + FasteningStateCnt + BallastDigCnt + JointlessCnt;

                                Itogo_skrep += FasteningStateCnt;
                                Itogo_styk += GapsCnt;
                                Itogo_shpal += SleepersDigCnt;
                                Itogo_NPK_L += NPK_L;
                                Itogo_ballast += BallastDigCnt;
                                Itogo_bezStyk += JointlessCnt;
                                Itogo_prochie += Prochie;
                                Itogo_itogo += ItogCol;
                                Itogo_vsegoOgrSpeed += VsegoOgrSpeed;
                             

                                XElement Notes = new XElement("Notes");
                                Notes.Add(
                                    new XAttribute("km", km.Number),
                                    new XAttribute("checkKm", ""),
                                    new XAttribute("skrep", FasteningStateCnt == 0 ? "" : FasteningStateCnt.ToString()),
                                    new XAttribute("styk", GapsCnt == 0 ? "" : GapsCnt.ToString()),
                                    new XAttribute("shpal", SleepersDigCnt == 0 ? "" : SleepersDigCnt.ToString()),
                                    new XAttribute("npk", NPK_L == 0 ? "" : NPK_L.ToString()),
                                    new XAttribute("ballast", BallastDigCnt == 0 ? "" : BallastDigCnt.ToString()),
                                    new XAttribute("bezStyk", JointlessCnt == 0 ? "" : JointlessCnt.ToString()),
                                    new XAttribute("prochie", Prochie == 0 ? "" : Prochie.ToString()),
                                    new XAttribute("itogo", ItogCol == 0 ? "" : ItogCol.ToString()),
                                    new XAttribute("vsegoOgrSpeed", VsegoOgrSpeed == 0 ? "" : VsegoOgrSpeed.ToString())
                                    );
                                direct.Add(Notes);
                                //if (km.Number < 129)
                                //    break;                            
                            }

                            sumFastener += Itogo_skrep; sumGaps += Itogo_styk; sumShpala += Itogo_shpal; sumKm += kms.Count(); sumProchie += Itogo_prochie;
                            sumNPK_L += Itogo_NPK_L; sumBallast += Itogo_ballast; sumBezStyk += Itogo_bezStyk; sumItogoItogo += Itogo_itogo; sumVsegoOgrSpeed += Itogo_vsegoOgrSpeed;

                            XElement Itogo = new XElement("Itogo");
                            Itogo.Add(
                                    new XAttribute("Itogo_checkKm", kms.Count()),
                                    new XAttribute("Itogo_skrep", Itogo_skrep == 0 ? "" : Itogo_skrep.ToString()),
                                    new XAttribute("Itogo_styk", Itogo_styk == 0 ? "" : Itogo_styk.ToString()),
                                    new XAttribute("Itogo_shpal", Itogo_shpal == 0 ? "" : Itogo_shpal.ToString()),
                                    new XAttribute("Itogo_NPK_L", Itogo_NPK_L == 0 ? "" : Itogo_NPK_L.ToString()),
                                    new XAttribute("Itogo_ballast", Itogo_ballast == 0 ? "" : Itogo_ballast.ToString()),
                                    new XAttribute("Itogo_bezStyk", Itogo_bezStyk == 0 ? "" : Itogo_bezStyk.ToString()),
                                    new XAttribute("Itogo_prochie", Itogo_prochie == 0 ? "" : Itogo_prochie.ToString()),
                                    new XAttribute("Itogo_itogo", Itogo_itogo == 0 ? "" : Itogo_itogo.ToString()),
                                    new XAttribute("Itogo_vsegoOgrSpeed", Itogo_vsegoOgrSpeed == 0 ? "" : Itogo_vsegoOgrSpeed.ToString()));
                            direct.Add(Itogo);

                            tripElem.Add(direct);
                        }

                        XElement Itogs = new XElement("Itogs");
                        Itogs.Add(
                                new XAttribute("Itogs_checkKm", sumKm == 0 ? "" : sumKm.ToString()),
                                new XAttribute("Itogs_skrep", sumFastener == 0 ? "" : sumFastener.ToString()),
                                new XAttribute("Itogs_styk", sumGaps == 0 ? "" : sumGaps.ToString()),
                                new XAttribute("Itogs_shpal", sumShpala == 0 ? "" : sumShpala.ToString()),
                                new XAttribute("Itogs_NPK_L", sumNPK_L == 0 ? "" : sumNPK_L.ToString()),
                                new XAttribute("Itogs_ballast", sumBallast == 0 ? "" : sumBallast.ToString()),
                                new XAttribute("Itogs_bezStyk", sumBezStyk == 0 ? "" : sumBezStyk.ToString()),
                                new XAttribute("Itogs_prochie", sumProchie == 0 ? "" : sumProchie.ToString()),
                                new XAttribute("Itogs_Itogo", sumItogoItogo == 0 ? "" : sumItogoItogo.ToString()),
                                new XAttribute("Itogs_vsegoOgrSpeed", sumVsegoOgrSpeed == 0 ? "" : sumVsegoOgrSpeed.ToString())
                                );
                        tripElem.Add(Itogs);
                        report.Add(tripElem);
                    }
                }
                xdReport.Add(report);
                XslCompiledTransform transform = new XslCompiledTransform();
                transform.Load(XmlReader.Create(new StringReader(template.Xsl)));
                transform.Transform(xdReport.CreateReader(), writer);
            }
            try
            {
                htReport.Save(Path.GetTempPath() + "/report_by_kilometer_sheet.html");
            }
            catch
            {
                MessageBox.Show("Ошибка сохранения файлы");
            }
            finally
            {
                System.Diagnostics.Process.Start(Path.GetTempPath() + "/report_by_kilometer_sheet.html");
            }
        }

        private List<DigressionMark> GetBesPutDig(List<Gap> check_ListOgDerogations_state)
        {
            var gapoid = new List<DigressionMark> { };

            //подсчет коллчиство бестыкогого пути по "ОРП" и "ОРШ"
            foreach (var elem in check_ListOgDerogations_state)
            {
                var nextfd = check_ListOgDerogations_state[check_ListOgDerogations_state.IndexOf(elem)] == check_ListOgDerogations_state[check_ListOgDerogations_state.Count - 1] ? null
                    : check_ListOgDerogations_state[check_ListOgDerogations_state.IndexOf(elem) + 1];

                if (nextfd != null)
                {
                    if ((elem.Oid == 13 && nextfd.Oid == 13) || (elem.Oid == 14 && nextfd.Oid == 14)) continue;
                }
                if (nextfd != null)
                {
                    if (elem.Threat == nextfd.Threat &&
                        elem.Km == nextfd.Km &&
                        elem.Meter == nextfd.Meter &&
                        elem.Fnum == nextfd.Fnum &&
                        elem.Local_fnum == nextfd.Local_fnum &&
                        ((elem.Oid == 13 && nextfd.Oid == 14) || (elem.Oid == 14 && nextfd.Oid == 13))
                        )
                    {

                        gapoid.Add(
                                 new DigressionMark()
                                 {
                                     Km = elem.Km,
                                     Comment = elem.Threat == Threat.Right ? Math.Abs(elem.Y - nextfd.Y).ToString() : "0", //zazorR //ZazorR
                                     Diagram_type = elem.Threat == Threat.Left ? Math.Abs(elem.Y - nextfd.Y).ToString() : "0" //zazorL
                                 }
                                 );

                    }
                    else if (elem.Oid == 13)
                    {

                        gapoid.Add(
                               new DigressionMark()
                               {
                                   Km = elem.Km,
                                   Comment = elem.Threat == Threat.Right && elem.Oid == 13 ? "ОРШ" : Math.Abs(elem.Y - nextfd.Y).ToString(), //ZazorR
                                   Diagram_type = elem.Threat == Threat.Left && elem.Oid == 13 ? "ОРШ" : Math.Abs(elem.Y - nextfd.Y).ToString() //ZazorL
                               }
                                  );
                    }
                    else if (elem.Oid == 14)
                    {

                        gapoid.Add
                            (
                            new DigressionMark()
                            {
                                Km = elem.Km,
                                Comment = elem.Threat == Threat.Right && elem.Oid == 14 ? "ОРП" : Math.Abs(elem.Y - nextfd.Y).ToString(), //ZazorR
                                Diagram_type = elem.Threat == Threat.Left && elem.Oid == 14 ? "ОРП" : Math.Abs(elem.Y - nextfd.Y).ToString()//ZazorL
                            }
                            );
                    }
                }
            }
            return gapoid;
        }
    }
}