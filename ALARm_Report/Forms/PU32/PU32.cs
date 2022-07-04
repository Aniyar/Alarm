using ALARm.Core;
using ALARm.Core.AdditionalParameteres;
using ALARm.Core.Report;
using ALARm.Services;
using ALARm_Report.controls;
using MetroFramework.Controls;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Xsl;

namespace ALARm_Report.Forms
{
	public class PU32 : Report
	{
		private string engineer { get; set; } = "Komissia K";
		private string chief { get; set; } = "Komissia K";
		private DateTime from, to;
		private TripType tripType, comparativeTripType;
		private PU32Type reportType;
		private ReportPeriod comparativePeriod;

		public override void Process(long parentId, ReportTemplate template, ReportPeriod period, MetroProgressBar progressBar)
		{
			NumberFormatInfo nfi = new CultureInfo("en-US", false).NumberFormat;
			using (var filterForm = new FilterForm())
			{
				filterForm.ReportPeriod = period;
				var filters = new List<Filter>();
				filters.Add(new StringFilter() { Name = "Начальник путеизмерительного вагона: ", Value = chief });
				filters.Add(new StringFilter() { Name = "Данные обработали и оформили ведомость: ", Value = engineer });
				filters.Add(new TripTypeFilter() { Name = "Тип поездки", Value = "рабочая" });
				filters.Add(new PU32TypeFilter { Name = "Тип отчета", Value = "Оценка состояния пути по ПЧ" });



				filterForm.SetDataSource(filters);
				filterForm.ReportClasssName = "PU32";

				if (filterForm.ShowDialog() == DialogResult.Cancel)
					return;

				chief = (string)filters[0].Value;
				engineer = (string)filters[1].Value;
				tripType = ((TripTypeFilter)filters[2]).TripType;
				reportType = ((PU32TypeFilter)filters[3]).PU32Type;
				if (reportType == PU32Type.Comparative)
				{
					from = period.StartDate;
					to = period.FinishDate;
					comparativePeriod = ((PeriodFilter)filters[4]).PeriodValue;
					comparativeTripType = ((TripTypeFilter)filters[5]).TripType;

				}
				else
				{
					from = DateTime.Parse((string)filters[4].Value);
					to = DateTime.Parse((string)filters[5].Value + " 23:59:59");
				}
			}
			XDocument htReport = new XDocument();
			using (XmlWriter writer = htReport.CreateWriter())
			{
				var roadName = AdmStructureService.GetRoadName(parentId, AdmStructureConst.AdmDistance, true);
				var distance = AdmStructureService.GetUnit(AdmStructureConst.AdmDistance, parentId) as AdmUnit;

				XDocument xdReport = new XDocument();

				XElement comparativeTrip, comparativeSection, comparativeByKilometer;

				var kilometers = RdStructureService.GetPU32Kilometers(from, to, parentId, tripType); //.GetRange(65,15);
				// kilometers = kilometers.OrderBy(km => km.Number).ToList();
				var cn = kilometers.Count;
				var comparativeKilometers = new List<Kilometer>();
				if (comparativePeriod != null)
					comparativeKilometers = RdStructureService.GetPU32Kilometers(comparativePeriod.StartDate, comparativePeriod.FinishDate, parentId, comparativeTripType); //.GetRange(65,15);

				if (kilometers.Count == 0)
				{
					MessageBox.Show("Нет отчетных данных по выбранным параметрам");
					return;
				}
				if (reportType == PU32Type.Comparative && comparativeKilometers.Count == 0)
				{
					MessageBox.Show("Нет отчетных данных для сравнения");
					return;
				}


				XElement report = new XElement("report",
					new XAttribute("version", $"{DateTime.Now} v{Assembly.GetExecutingAssembly().GetName().Version.ToString()}"),
					new XAttribute("engineer", engineer),
					new XAttribute("from", reportType == PU32Type.Comparative ? ((tripType == TripType.Control ? "контрольная" : tripType == TripType.Work ? "рабочая" : "дополнительная") + " " + period.Period) : from.ToString("dd")),
					new XAttribute("to", reportType == PU32Type.Comparative ? ((comparativeTripType == TripType.Control ? "контрольная" : tripType == TripType.Work ? "рабочая" : "дополнительная") + " " + comparativePeriod.Period) : to.ToString("dd MMMM yyyy")),
					new XAttribute("road", roadName),
					new XAttribute("distance", distance.Code),
					new XAttribute("month", period.Period),
					new XAttribute("triptype", tripType == TripType.Control ? "контрольной" : tripType == TripType.Work ? "рабочая " : "дополнительной"),
					new XAttribute("soft", "ALARm 1.0(436-р)"),
					new XAttribute("tripdate", kilometers[0].TripDate.ToString("dd MMMM yyyy")),
					new XAttribute("car", kilometers[0].Trip.Car.ToString()),
					new XAttribute("chief", chief),
					 new XAttribute("Count", cn),
					new XAttribute("reptype", reportType)

				);


				var DevDegree2 = 0;
				var compDevDegree2 = 0;


				var tripElment = new XElement("trip");

				var byKilometer = new XElement("bykilometer",
									new XAttribute("code", kilometers[0].Direction_code),
									new XAttribute("track", kilometers[0].Track_name),
									new XAttribute("name", kilometers[0].Direction_name),
									new XAttribute("PDcode", kilometers[0].PdCode),
									new XAttribute("car", kilometers[0].Trip.Car.ToString()),
									new XAttribute("pch", distance.Code));

				var distanceTotal = new Total
				{
					Code = distance.Code
				};
				Total comparativeTotal = null;
				var sectionTotal = new Total
				{

					Code = kilometers[0].Track_name,
					DirectionCode = kilometers[0].Direction_code,
					DirectionName = kilometers[0].Direction_name
				};

				//запрос доп параметров с бд
				var AddParam = AdditionalParametersService.GetAddParam(kilometers.First().Trip.Id) as List<S3>; //износы
				var comparativeAddParam = new List<S3>();

				//if (AddParam == null)
				//{
				//	MessageBox.Show("Не удалось сформировать отчет, так как возникала ошибка во время загрузки данных по доп параметрам");
				//	return;
				//}

				List<Gap> comparative_check_gap_state = new List<Gap>();
				List<Gap> comparative_gapV = new List<Gap>(), comparativePu32_gap = new List<Gap>();


				List<S3> comparativePRU = new List<S3>();
				var PRU = RdStructureService.GetS3(kilometers.First().Trip.Id, DigressionName.Pru.Name) as List<S3>; //пру

				if (reportType == PU32Type.Comparative)
				{
					comparativeTrip = new XElement("trip",
						new XAttribute("rtype", (comparativeTripType == TripType.Additional ? "Доп." : tripType == TripType.Control ? "Конт." : "Раб.") + " " + comparativePeriod.Period));
					comparativeByKilometer = new XElement("bykilometer",
									new XAttribute("code", comparativeKilometers[0].Direction_code),
									new XAttribute("track", comparativeKilometers[0].Track_name),
									new XAttribute("name", comparativeKilometers[0].Direction_name),
									new XAttribute("PDcode", comparativeKilometers[0].PdCode),
									new XAttribute("pch", distance.Code));
					comparativeSection = new XElement("section",
						new XAttribute("name", comparativeKilometers[0].Direction_name),
						new XAttribute("code", comparativeKilometers[0].Direction_code),
						new XAttribute("track", comparativeKilometers[0].Track_name)
						);
					comparativeTotal = new Total
					{
						Code = comparativeKilometers[0].Track_name,
						DirectionCode = comparativeKilometers[0].Direction_code,
						DirectionName = comparativeKilometers[0].Direction_name
					};
					

					comparativeAddParam = AdditionalParametersService.GetAddParam(comparativeKilometers.First().Trip.Id) as List<S3>; //износы
					comparative_check_gap_state = AdditionalParametersService.Check_gap_state(comparativeKilometers.First().Trip.Id, template.ID); //стыки
					comparativePRU = RdStructureService.GetS3(comparativeKilometers.First().Trip.Id, DigressionName.Pru.Name) as List<S3>; //пру
					comparative_gapV = comparative_check_gap_state.Where(o => o.Vdop != "" && o.Vdop != "-/-").ToList();
					comparativePu32_gap = comparative_check_gap_state.Where(o => o.Otst_l.Contains("З") || o.Otst_r.Contains("З?") || o.Otst_r.Contains("З") || o.Otst_l.Contains("З?")).ToList();
				}





				var comparative_dopBall = comparativePRU.Count() * 50 + (comparative_gapV.Count() > 0 ? 50 : 0) + (comparativeAddParam.Count() > 0 ? 50 : 0);
				var comparative_DopCount = comparative_gapV.Count() + comparativeAddParam.Count();


				var sectionElement = new XElement("section",
					new XAttribute("name", kilometers[0].Direction_name),
					new XAttribute("code", kilometers[0].Direction_code),
					new XAttribute("track", kilometers[0].Track_name)
					);
				var pchuElement = new XElement("pchu",
					new XAttribute("pch", distance.Code),
					new XAttribute("code", kilometers[0].PchuCode),
					new XAttribute("chief", kilometers[0].PchuChief)
					);
				var pchuTotal = new Total();
				pchuTotal.Code = kilometers[0].PchuCode;
			
			
				// var distancecode = new Total();
				//distancecode.distancecode = distance.Code;
				var pdElement = new XElement("pd",
					new XAttribute("code", kilometers[0].PdCode),
					new XAttribute("chief", kilometers[0].PdChief),
					new XAttribute("pchu", kilometers[0].PchuCode ),
					new XAttribute("PDcode", kilometers[0].PdCode),
					new XAttribute("pch", distance.Code)
					);
				var pdTotal = new Total();
				pdTotal.Code = kilometers[0].PdCode;


				var pdbElement = new XElement("pdb",
					new XAttribute("PDcode", kilometers[0].PdCode),
					new XAttribute("code", kilometers[0].PdbCode),
					new XAttribute("chief", kilometers[0].PdbChief));


				var pdElementn = new XElement("pdn",
					new XAttribute("code", kilometers[0].PdCode),
					new XAttribute("chief", kilometers[0].PdChief),
					new XAttribute("pchu", kilometers[0].PchuCode),
					new XAttribute("PDcode", kilometers[0].PdCode),
					new XAttribute("pch", distance.Code)
					);

				var pdTotaln = new Total();
				pdTotaln.Code = kilometers[0].PdCode;



				XElement compPdbElement = null;
				Total compPdbTotal = null;
				Total compPdTotal = null;
				Total compPchuTotal = null;
				Total compSectionTotal = null;
				Total compDistanceTotal = null;
				if (reportType == PU32Type.Comparative)
				{
					compPdbTotal = new Total() { Code = kilometers[0].PdbCode };
					compPdTotal = new Total() { Code = kilometers[0].PdCode }; ;
					compPchuTotal = new Total() { Code = kilometers[0].PchuCode }; ;
					compSectionTotal = new Total() { Code = kilometers[0].Track_name }; ;
					compDistanceTotal = new Total() { Code = kilometers[0].Direction_code }; ;

				}
				var pdbTotal = new Total();
				pdbTotal.Code = kilometers[0].PdbCode;

				Kilometer compKm = null;

				int Grk = 0, Sochet = 0, Kriv = 0, Pru = 0, Oshk = 0, Iznos = 0, Zazor = 0, CompZazor = 0, NerProf = 0, KmSpeedLimit = 0, Pu32_gap_count = 0, gapV_count = 0;

				progressBar.Maximum = kilometers.Count;

				var start_TrackName = " ";
				var flag_start = true ;
				var km_startNumber = 0;
				var trackI=0;
				var pdbTotaln = new Total();
				var allpdbcodes = kilometers.Select(x => x.PdbCode).Distinct().ToList();
				for (int i = 0; i < allpdbcodes.Count; i++)
				{
					pdbTotaln.Code = allpdbcodes[i];
					var pdbElementn = new XElement("pdbn", new XAttribute("code", allpdbcodes[i]), new XAttribute("chief", chief));
					foreach (var km in kilometers)
					{
						if (km.PdbCode == pdbTotaln.Code)
						{
							List<Gap> check_gap_state = AdditionalParametersService.Check_gap_state(km.Trip.Id, template.ID); //стыки
							var gapV = check_gap_state.Where(o => o.Vdop != "" && o.Vdop != "-/-").ToList();
							var Pu32_gap = check_gap_state.Where(o => o.Otst_l.Contains("З") || o.Otst_r.Contains("З") || o.Otst_l.Contains("З?") || o.Otst_r.Contains("З?")).ToList();

							var gapvPlusPu32_gap = Pu32_gap.Count() + gapV.Count();
							//var pu32_gap =;
							if (gapvPlusPu32_gap != null)
							{
								Pu32_gap_count = gapvPlusPu32_gap;
							};

							//var gapv =;
							if (gapV.Any()) gapV_count++;

							var dopBall = PRU.Count() * 50 + (gapV.Count() > 0 ? 50 : 0) + (AddParam.Count() > 0 ? 50 : 0);
							var DopCount = gapV.Count() + AddParam.Count();

							if (reportType == PU32Type.Comparative)
							{
								try
								{
									compKm = comparativeKilometers.Where(ck => ck.Number == km.Number && ck.Track_id == km.Track_id).ToList().First();
									compKm.LoadTrackPasport(MainTrackStructureService.GetRepository(), compKm.TripDate);
									compKm.Digressions = RdStructureService.GetDigressionMarks(compKm.Trip.Id, compKm.Track_id, compKm.Number);
								}
								catch
								{
									compKm = null;
								}
								if (compKm == null)
								{
									continue;
								}
							}
							progressBar.Value += 1;
							if (km.Number == 709)
							{
								km.Number = km.Number;
							}

							km.LoadTrackPasport(MainTrackStructureService.GetRepository(), km.TripDate);
							km.Digressions = RdStructureService.GetDigressionMarks(km.Trip.Id, km.Track_id, km.Number);

							//данные стыка
							var gaps = new List<Digression> { };
							var tempGaps = Pu32_gap.Where(o => o.Km == km.Number).ToList();
							foreach (var gap in tempGaps)
							{
								var PassSpeed = km.Speeds.Count > 0 ? km.Speeds[0].Passenger : -1;
								var FreightSpeed = km.Speeds.Count > 0 ? km.Speeds[0].Freight : -1;

								var digression = new Digression { };

								digression.Meter = gap.Meter;

								if (gap.Otst_l == "З" || gap.Otst_r == "З")
								{
									digression.PassengerSpeedLimit = int.Parse(gap.Vdop.Split('/')[0]);
									digression.FreightSpeedLimit = int.Parse(gap.Vdop.Split('/')[1]);
								}
								else
								{
									digression.PassengerSpeedLimit = -1;
									digression.FreightSpeedLimit = -1;
								}
								digression.AllowSpeed = gap.Vdop;
								//digression.Velich = Math.Max(gap.Zazor, gap.R_zazor);
								//digression.DigName = gap.Otst == "З" ? DigressionName.Gap : DigressionName.GapSimbol;
								if (gap.Otst_l != "")
								{
									digression.DigName = gap.Otst_l == "З" ? DigressionName.Gap_left : DigressionName.GapSimbol_left;
									digression.Velich = gap.Zazor;
									digression.prim += digression.DigName.Name + " " + digression.Velich;
								}

								if (gap.Otst_r != "")
								{
									digression.DigName = gap.Otst_r == "З" ? DigressionName.Gap_right : DigressionName.GapSimbol_right;
									digression.Velich = gap.R_zazor;
									digression.prim += digression.DigName.Name + " " + digression.Velich;
								}
								gaps.Add(digression);
							}
							foreach (var dig in gaps)
							{
								int count = dig.Length / 4;
								count += dig.Length % 4 > 0 ? 1 : 0;

								var side = " " + (dig.Threat == (Threat)Threat.Right ? "п." : dig.Threat == (Threat)Threat.Left ? "л." : "");

								km.Digressions.Add(
									new DigressionMark()
									{
										Digression = dig.DigName,
										NotMoveAlert = false,
										Meter = dig.Meter,
										finish_meter = dig.Kmetr,
										Length = dig.Length,
										Value = dig.Velich,
										Count = count,
										DigName = dig.GetName(),
										PassengerSpeedLimit = dig.PassengerSpeedLimit,
										FreightSpeedLimit = dig.FreightSpeedLimit,
										Comment = "",
										AllowSpeed = dig.AllowSpeed
									});
							}

							//данные Износа рельса Бок.из.
							var DBcrossRailProfile = AdditionalParametersService.GetCrossRailProfileFromDBbyKm(km.Number, km.Trip.Id);
							if (DBcrossRailProfile == null)
								DBcrossRailProfile = new List<CrosProf> { };
							//continue;

							var sortedData = DBcrossRailProfile.OrderByDescending(d => d.Meter).ToList();
							var crossRailProfile = AdditionalParametersService.GetCrossRailProfileFromDBParse(sortedData);

							List<Digression> addDigressions = crossRailProfile.GetDigressions();

							var dignatur = new List<DigressionMark> { };
							foreach (var dig in addDigressions)
							{
								int count = dig.Length / 4;
								count += dig.Length % 4 > 0 ? 1 : 0;

								if (dig.DigName == DigressionName.SideWearLeft || dig.DigName == DigressionName.SideWearRight)
								{
									dignatur.Add(new DigressionMark()
									{
										Digression = dig.DigName,
										NotMoveAlert = false,
										Meter = dig.Meter,
										finish_meter = dig.Kmetr,
										Degree = (int)dig.Typ,
										Length = dig.Length,
										Value = dig.Value,
										Count = count,
										DigName = dig.GetName(),
										PassengerSpeedLimit = -1,
										FreightSpeedLimit = -1,
										Comment = "",
										Diagram_type = "Iznos_relsov",
										Digtype = DigressionType.Additional
									});
								}
							}
							//выч-е скорости бок износа
							int pas = 999, gruz = 999;
							foreach (DigressionMark item in dignatur)
							{
								if (item.Digression == DigressionName.SideWearLeft || item.Digression == DigressionName.SideWearRight)
								{
									var c = km.Curves.Where(o => o.RealStartCoordinate <= km.Number + item.Meter / 10000.0 && o.RealFinalCoordinate >= km.Number + item.Meter / 10000.0).ToList();

									if (c.Any())
									{
										item.GetAllowSpeedAddParam(km.Speeds.First(), c.First().Straightenings[0].Radius, item.Value);

										if (item.PassengerSpeedLimit != -1 && item.PassengerSpeedLimit < pas)
										{
											pas = item.PassengerSpeedLimit;
										}
										if (item.FreightSpeedLimit != -1 && item.FreightSpeedLimit < gruz)
										{
											gruz = item.FreightSpeedLimit;
										}
									}
								}
								else if (item.Digression == DigressionName.Gap)
								{
									if (item.PassengerSpeedLimit != -1 && item.PassengerSpeedLimit < pas)
									{
										pas = item.PassengerSpeedLimit;
									}
									if (item.FreightSpeedLimit != -1 && item.FreightSpeedLimit < gruz)
									{
										gruz = item.FreightSpeedLimit;
									}
								}

							}
							dignatur = dignatur.Where(o => o.PassengerSpeedLimit >= 0 || o.FreightSpeedLimit >= 0).ToList();

							km.Digressions.AddRange(dignatur);

							var gap_dig = AddParam.Where(o => o.Km == km.Number).ToList();
							foreach (var item in gap_dig)
							{
								km.Digressions.Add(new DigressionMark
								{
									Km = item.Km,
									Meter = item.Meter,
									Length = item.Len,
									DigName = item.Ots,
									Count = item.Kol,
									Value = item.Otkl,
									PassengerSpeedLimit = item.Ogp,
									FreightSpeedLimit = item.Ogp,
									Digtype = DigressionType.Additional
								});
							}
							var Glag_grk = false;
							var kriv = km.Digressions.Where(o => !o.DigName.Contains("кривая факт.") && !o.DigName.Contains("З?") && !o.DigName.Equals("ПрУ")
							&& (o.DigName.Equals("Укл") || o.DigName.Equals("?Уkл") || o.DigName.Equals("Анп") || o.DigName.Equals("Пси") && !o.Comment.Contains("-/-") && !o.Comment.Contains("") && !o.FreightSpeedLimit.Equals("-") && !o.PassengerSpeedLimit.Equals("-"))).ToList();

							if (kriv.Any()) Kriv += kriv.Count;

							var pru = km.Digressions.Where(o => !o.DigName.Contains("кривая факт.") && !o.DigName.Contains("З?") && o.DigName.Equals("ПрУ")).ToList();
							if (pru.Any()) Pru += pru.Count;

							var oshk = km.Digressions.Where(o => !o.DigName.Contains("кривая факт.") && !o.DigName.Contains("З?") && !o.DigName.Equals("ПрУ") && o.DigName.Equals("Oтв.ш")).ToList();
							if (oshk.Any()) Oshk += oshk.Count;

							var iznos = km.Digressions.Where(o => o.DigName.Equals("Иб.л") || o.DigName.Equals("Иб.п") || o.DigName.Equals("Ив.л") || o.DigName.Equals("Ив.п") ||
																  o.DigName.Equals("Ип.л") || o.DigName.Equals("Ип.п")).ToList();
							if (iznos.Any()) Iznos += iznos.Count;

							var zazor = km.Digressions.Where(o => !o.DigName.Contains("кривая факт.") && !o.DigName.Equals("ПрУ") && (o.DigName.Equals("З?Л") || o.DigName.Equals("ЗЛ") || o.DigName.Equals("З?П") || o.DigName.Equals("ЗП")) && km.Number == km.Digressions.Select(x => x.Km).ToList().First()).ToList();
							if (zazor.Any()) Zazor += zazor.Count;

							//Pu32_gap.Count() + (gapV.Count()
							var kmTotal = new Total();
							Total comparativeKMTotal = null;

							kmTotal.IsKM = true;
							//if (grk.Any())
							//{
							//	kmTotal.Grk = grk.Count;
							//}
							if (kriv.Any())
							{
								kmTotal.Kriv = kriv.Count;
							}
							if (pru.Any())
							{
								kmTotal.Pru = pru.Count;
							}
							if (oshk.Any())
							{
								kmTotal.Oshk = oshk.Count;
							}
							if (iznos.Any())
							{
								kmTotal.Iznos = iznos.Count;
							}
							if (zazor.Any())
							{
								km.Number = km.Number;
								kmTotal.Zazor = zazor.Count;
							}




							var add_dig_str = "";
							var comp_add_dig_str = "";
							var Curve_dig_str = "";
							var comp_curve_dig_str = "";


							//Зазор баллы
							var isgap = Pu32_gap.Where(o => o.Km == km.Number).ToList();
							if (isgap.Any())
							{
								if (isgap.Where(o => o.Otst_l == "З" || o.Otst_r == "З").ToList().Count > 0)
								{
									kmTotal.AddParamPointSum += 50;
									kmTotal.Additional += isgap.Where(x => x.Otst_l == "З" || x.Otst_r == "З").Count();

								}

								else if (isgap.Where(o => o.Otst_l == "З?" || o.Otst_r == "З?").ToList().Count > 0)
								{
									kmTotal.AddParamPointSum += 20;

								}

								Zazor += isgap.Count;
							}
							var prevCurve_id = -1;

							//переменная огр скорости для качественной оценки км
							var Districted = 0;

							foreach (var digression in km.Digressions)
							{
								if (digression.DigName == "П" && digression.Km == 731 && digression.Meter == 590)
								{

								}
								if ((digression.DigName.Contains("Пси") || digression.DigName.Contains("?Пси")) && digression.LimitSpeedToString() == "-/-")
									continue;
								if (km.Number == 720)
								{

								}
								//&& !digression.Comment.Contains("Натурная кривая")
								if ((digression.LimitSpeedToString() != "-/-") && (digression.LimitSpeedToString() != ""))
								{
									Districted++;
								}

								if (digression.Comment != "" || digression.Comment == null)
								{
									if (Districted > 0)
									{
										Districted = Districted - 1;
									}
								}






								if (digression.Digtype == DigressionType.Main)
								{
									kmTotal.MainParamPointSum += digression.GetPoint(km);
									//ToDo - ағамен ақылдасу керек
									kmTotal.CurvePointSum += digression.GetCurvePoint(km);
								}

								//Износ баллы
								if (digression.Digression == DigressionName.SideWearLeft || digression.Digression == DigressionName.SideWearRight)
								{
									if (digression.Km == 708)
									{

									}
									var noteCoord = km.Number.ToDoubleCoordinate(digression.Meter);
									var isCurve = km.Curves.Where(o => o.RealStartCoordinate <= noteCoord && o.RealFinalCoordinate >= noteCoord).ToList();
									if (isCurve.Any())
									{
										if (prevCurve_id != (int)isCurve.First().Id)
										{
											kmTotal.AddParamPointSum += 50;
											kmTotal.Additional++;
										}
										prevCurve_id = (int)isCurve.First().Id;
									}
									add_dig_str += $"V={digression.PassengerSpeedLimit}/{digression.FreightSpeedLimit} пк{(digression.Meter - 1) / 100 + 1} {digression.DigName} {digression.Value:0.0}/{digression.Length}; ";
								}
								//Пру кривая баллы
								if (digression.Digression == DigressionName.Pru)
								{
									kmTotal.Curves++;

									Curve_dig_str += $"пк{(digression.Meter - 1) / 100 + 1} {digression.DigName} {digression.Value}/{digression.Length}; ";
								}
								//ПСИ АНП Укл Аг кривая 
								if (digression.Digression == DigressionName.Psi ||
									digression.Digression == DigressionName.SpeedUp ||
									digression.Digression == DigressionName.Ramp)
								{
									var otkl = "";
									if (digression.Comment.Any())
									{
										try
										{
											otkl = digression.Comment.Split().First().Split(':').Last();
										}
										catch
										{
											otkl = "";
										}
									}
									Curve_dig_str += $"V={digression.PassengerSpeedLimit}/{digression.FreightSpeedLimit} пк{(digression.Meter - 1) / 100 + 1} {digression.DigName} {otkl}; ";
									kmTotal.Curves++;
								}
								//зазоры
								if (digression.Digression.Name == "ЗЛ" || digression.Digression.Name == "ЗП")
								{
									add_dig_str += $"V={digression.PassengerSpeedLimit}/{digression.FreightSpeedLimit} пк{(digression.Meter - 1) / 100 + 1} {digression.DigName} {digression.Value}; ";
								}
								else if (digression.Digression.Name == "З?Л" || digression.Digression.Name == "З?П")
								{
									add_dig_str += $"пк{(digression.Meter - 1) / 100 + 1} {digression.DigName} {digression.Value}; ";
								}
								digression.DigNameToDigression(digression.DigName);

								if (digression.Degree < 5)
									switch (digression.DigName)
									{
										case string digname when digname.Equals("Суж"):
											digression.Digression = DigressionName.Constriction;
											kmTotal.Constriction.Degrees[digression.Degree - 1] += digression.GetCount();
											break;

										case string digname when digname.Equals("Уш"):
											digression.Digression = DigressionName.Broadening;
											kmTotal.Broadening.Degrees[digression.Degree - 1] += digression.GetCount();
											break;

										case string digname when digname.Equals("У"):
											digression.Digression = DigressionName.Level;
											kmTotal.Level.Degrees[digression.Degree - 1] += digression.GetCount();
											break;

										case string digname when digname.Equals("П"):
											digression.Digression = DigressionName.Sag;
											kmTotal.Sag.Degrees[digression.Degree - 1] += digression.GetCount();
											break;
										case string digname when digname.Equals("Пр.л") || digname.Equals("Пр.п"):
											digression.Digression = (digname.Equals("Пр.л")) ? DigressionName.DrawdownLeft : DigressionName.DrawdownRight;
											kmTotal.Drawdown.Degrees[digression.Degree - 1] += digression.GetCount();
											break;
										case string digname when digname.Equals("Р"):
											digression.Digression = DigressionName.Strightening;
											kmTotal.Strightening.Degrees[digression.Degree - 1] += digression.GetCount();
											break;
										case string digname when (digname.Equals("Рнр") && digression.Degree == 4):
											digression.Digression = DigressionName.Strightening;
											kmTotal.Strightening.Degrees[digression.Degree - 1] += digression.GetCount();
											break;
										case string digname when digname.Equals("Рст"):
											digression.Digression = DigressionName.Strightening;
											kmTotal.Strightening.Degrees[digression.Degree - 1] += digression.GetCount();
											break;
									}

								if (digression.DigName.Equals("Р") && digression.Degree == 2)
								{
									var kmq = digression.Km;
									var met = digression.Meter;
									DevDegree2++;
								}
								var flag_gr = false;
								if (km.Number == 709)
							{
								var kmmm = km.Number;
							}
								if (digression.Comment != null )  if (digression.Comment.Contains("гр")) flag_gr = true;
								if (((digression.LimitSpeedToString().Any(char.IsDigit) || flag_gr) && !digression.DigName.Equals("ПрУ")
									&& !digression.DigName.Contains("кривая") && !digression.DigName.Contains("З?") && !digression.Comment.Contains("-/-") && !digression.Comment.Contains("кривая")))
								{
									kmTotal.IsLimited = 1;
									//MessageBox.Show(digression.Km.ToString() + '-' +digression.DigName + '-' + digression.LimitSpeedToString());
									if (digression.Km == 710 && digression.DigName == DigressionName.Broadening.ToString())
									{
										digression.Count = digression.Count;

									}
									if (digression.Degree == 4 && digression.Digtype == DigressionType.Main && !digression.DigName.Contains("З?")
										&& !digression.DigName.Contains("кривая факт.") && !digression.DigName.Equals("ПрУ"))
									{
										kmTotal.Fourth = kmTotal.Fourth + digression.GetCount();
										//kmTotal.Fourth++;
									}
									//if (digression.Digtype != DigressionType.Main && !digression.DigName.Contains("З?") && !digression.DigName.Equals("ПрУ") && !digression.DigName.Contains("кривая факт."))
									if (digression.DigName.Contains("З") && !digression.DigName.Contains("З?") && !digression.DigName.Equals("ПрУ") && !digression.DigName.Contains("кривая факт.") && digression.DigName.Contains("Изн"))

									{
										kmTotal.Additional++;
									}

								}
								if (digression.Comment != null)
								{

									if ((digression.Comment.Contains("ОШК") || digression.Comment.Contains("ОШП") || digression.Comment.Contains("Уобр")) &&

										!digression.Comment.Contains("м") && !digression.Comment.Contains("гр") && !digression.DigName.Contains("З") && !digression.DigName.Equals("ПрУ") && !digression.DigName.Contains("кривая факт.")
										&& !(digression.Degree == 4 && digression.Digtype == DigressionType.Main) && !digression.DigName.Contains("Изн") && digression.LimitSpeedToString().Any(char.IsDigit)

										)


									{
										//kmTotal.IsLimited = 1;
										kmTotal.Other++;


									}
									//&& (digression.Degree == 3)
									if (((digression.Comment.Contains("гр") || digression.Comment.Contains("м") ) && (digression.Degree == 3)) && !digression.DigName.Contains("З") && !digression.DigName.Equals("ПрУ") && !digression.DigName.Contains("кривая факт.")
									&& (!(digression.Degree == 4) && digression.Digtype == DigressionType.Main) && !digression.DigName.Contains("Изн"))
										///сделать сортировку по "гр.ис.Пр,м" с органичемнием скрости  что то типо digression.Comment.Contains("м") || digression.Comment.Contains("ис")
										///проверка на наличие ограничения скорости 
									//	var flag_gr = false;
									//if (digression.Comment != null) if (digression.Comment.Contains("гр")) flag_gr = true;

									//if ((digression.LimitSpeedToString().Any(char.IsDigit) && !digression.Equals("ПрУ") && !digression.DigName.Contains("кривая факт.")) || flag_gr)
									{
										kmTotal.Grk = kmTotal.Grk + 1;

									}

									if ((digression.Degree == 3) && !digression.DigName.Contains("З") && !digression.DigName.Equals("ПрУ") && !digression.DigName.Contains("кривая факт.")
										&& (!(digression.Degree == 3) && digression.Digtype == DigressionType.Main) && !digression.DigName.Contains("Изн")
											&&
											(digression.DigName.Contains("П") && digression.DigName.Contains("Р") || digression.DigName.Contains("П") && digression.DigName.Contains("Пр") ||
											digression.DigName.Contains("Пр") && digression.DigName.Contains("Р")
												))
									{
										kmTotal.Sochet = Sochet + 1;

									}

								}


							}
							kmTotal.Grk = kmTotal.Grk + kmTotal.Fourth;
							kmTotal.Broadening.Degrees[0] = km.FDBroad;
							kmTotal.Constriction.Degrees[0] = km.FDConstrict;
							kmTotal.Sag.Degrees[0] = km.FDSkew;
							kmTotal.Strightening.Degrees[0] = km.FDStright;
							kmTotal.Drawdown.Degrees[0] = km.FDDrawdown;
							kmTotal.Level.Degrees[0] = km.FDLevel;

							if (km.Number == 709)
							{
								var kmmm = km.Number;
							}
							kmTotal.QualitiveRating = km.CalcQualitiveRating(kmTotal.MainParamPointSum + kmTotal.CurvePointSum, Districted);
							var kmElement = new XElement("km",
							new XAttribute("n", km.Number),
							new XAttribute("len", km.Lkm % 1 == 0 ? km.Lkm.ToString("0", nfi) : km.Lkm.ToString("0.000", nfi)),
							new XAttribute("c1", kmTotal.Constriction.ToString()),
							new XAttribute("c2", kmTotal.Broadening.ToString()),
							new XAttribute("c3", kmTotal.Level.ToString()),
							new XAttribute("c4", kmTotal.Sag.ToString()),
							new XAttribute("c5", kmTotal.Drawdown.ToString()),
							new XAttribute("c6", kmTotal.Strightening.ToString()),
							new XAttribute("c7", kmTotal.Common),
							new XAttribute("c8", kmTotal.FourthOtherAdd == "-/ - / - " ? "" : kmTotal.FourthOtherAdd),
							new XAttribute("c9", $"{kmTotal.MainParamPointSum + kmTotal.CurvePointSum}/{kmTotal.AddParamPointSum}"),
							new XAttribute("c10", kmTotal.QualitiveRating),
							new XAttribute("c11", km.Primech + " "), // Отступления с таблицы бедомость
							new XAttribute("c12", add_dig_str), //Отступления доп пар
							new XAttribute("c13", Curve_dig_str), //Отступления кривые
							new XAttribute("apoint", (kmTotal.AddParamPointSum > 0 ? kmTotal.AddParamPointSum.ToString() : "---")), //балл отст для доп пар

							new XAttribute("speed", $"{(km.Speeds.Count > 0 ? $"{km.Speeds.Last().Passenger}/{km.Speeds.Last().Freight}" : $"{km.GlobPassSpeed}/{km.GlobFreightSpeed}")}"),
							new XAttribute("cor", (km.CorrCount > 0 ? "+" : "") + (km.IsLowActivity ? "*" : "")),
							new XAttribute("mpoint", kmTotal.MainParamPointSum),

							new XAttribute("allsum", (kmTotal.MainParamPointSum + kmTotal.CurvePointSum + kmTotal.AddParamPointSum)), //итого по км

							new XAttribute("cpoint", (kmTotal.CurvePointSum > 0 ? kmTotal.CurvePointSum.ToString() : "---"))

							);

							int inserted = RdStructureService.InsertRating(km.Number, kmTotal.QualitiveRating.ToString(), km.Track_name);
						   kmTotal.Length = km.Lkm;

							pdbTotaln += kmTotal;
							pdbElementn.Add(kmElement);

						}

					}
					PDBnTotalGenerate(ref pdbElementn, ref pdbTotaln, ref pdElementn, ref pdTotaln, pdbTotaln.Code, "", ref compPdbTotal, ref compPdTotal);
				
					// pdTotaln += pdbTotaln;
					// pdElementn.Add(pdbElementn);
				}
				foreach (var km in kilometers)
				{
						List<Gap> check_gap_state = AdditionalParametersService.Check_gap_state(km.Trip.Id, template.ID); //стыки
						var gapV = check_gap_state.Where(o => o.Vdop != "" && o.Vdop != "-/-").ToList();
					var Pu32_gap = check_gap_state.Where(o => o.Otst_l.Contains("З") || o.Otst_r.Contains("З") || o.Otst_l.Contains("З?") || o.Otst_r.Contains("З?")).ToList();

					var gapvPlusPu32_gap = Pu32_gap.Count() + gapV.Count();
						//var pu32_gap =;
						if (gapvPlusPu32_gap != null)
						{
							Pu32_gap_count = gapvPlusPu32_gap;
						};

						//var gapv =;
						if (gapV.Any()) gapV_count++;

						var dopBall = PRU.Count() * 50 + (gapV.Count() > 0 ? 50 : 0) + (AddParam.Count() > 0 ? 50 : 0);
						var DopCount = gapV.Count() + AddParam.Count();

						if (reportType == PU32Type.Comparative)
						{
							try
							{
								compKm = comparativeKilometers.Where(ck => ck.Number == km.Number && ck.Track_id == km.Track_id).ToList().First();
								compKm.LoadTrackPasport(MainTrackStructureService.GetRepository(), compKm.TripDate);
								compKm.Digressions = RdStructureService.GetDigressionMarks(compKm.Trip.Id, compKm.Track_id, compKm.Number);
							}
							catch
							{
								compKm = null;
							}
							if (compKm == null)
							{
								continue;
							}
						}
						progressBar.Value += 1;

						km.LoadTrackPasport(MainTrackStructureService.GetRepository(), km.TripDate);

						if (!sectionTotal.Code.Equals(km.Track_name) || !sectionTotal.DirectionCode.Equals(km.Direction_code))
						{
							PDBTotalGenerate(ref pdbElement, ref pdbTotal, ref pdElement, ref pdTotal, km.PdbCode, km.PdbChief, ref compPdbTotal, ref compPdTotal);
							PDTotalGenerate(ref pdElement, ref pdTotal, ref pchuElement, ref pchuTotal, km.PdCode, km.PdChief, ref compPdTotal, ref compPchuTotal);
							PCHUTotalGenerate(ref pchuElement, ref pchuTotal,  ref sectionElement, ref sectionTotal, km.PchuCode, km.PchuChief, ref compPchuTotal, ref compSectionTotal, distanceTotal.Code);
							SectionTotalGenerate(ref sectionElement, ref sectionTotal, ref byKilometer, ref distanceTotal, km.Track_name, km.Direction_code, km.Direction_name, ref compSectionTotal, ref compDistanceTotal);
						}
						if (!pchuTotal.Code.Equals(km.PchuCode))
						{
							PDBTotalGenerate(ref pdbElement, ref pdbTotal, ref pdElement, ref pdTotal, km.PdbCode, km.PdbChief, ref compPdbTotal, ref compPdTotal);
							PDTotalGenerate(ref pdElement, ref pdTotal, ref pchuElement, ref pchuTotal, km.PdCode, km.PdChief, ref compPdTotal, ref compPchuTotal);
							PCHUTotalGenerate(ref pchuElement, ref pchuTotal ,ref sectionElement, ref sectionTotal, km.PchuCode, km.PchuChief, ref compPchuTotal, ref compSectionTotal, distanceTotal.Code);
						//distancecode
						}
						if (!pdTotal.Code.Equals(km.PdCode))
						{
							PDBTotalGenerate(ref pdbElement, ref pdbTotal, ref pdElement, ref pdTotal, km.PdbCode, km.PdbChief, ref compPdbTotal, ref compPdTotal);
							PDTotalGenerate(ref pdElement, ref pdTotal, ref pchuElement, ref pchuTotal, km.PdCode, km.PdChief, ref compPdTotal, ref compPchuTotal);
						}

						if (!pdbTotal.Code.Equals(km.PdbCode))
						{
							PDBTotalGenerate(ref pdbElement, ref pdbTotal, ref pdElement, ref pdTotal, km.PdbCode, km.PdbChief, ref compPdbTotal, ref compPdTotal);
						}
					

					km.Digressions = RdStructureService.GetDigressionMarks(km.Trip.Id, km.Track_id, km.Number);

						//данные стыка
						var gaps = new List<Digression> { };
						var tempGaps = Pu32_gap.Where(o => o.Km == km.Number).ToList();
						foreach (var gap in tempGaps)
						{
							var PassSpeed = km.Speeds.Count > 0 ? km.Speeds[0].Passenger : -1;
							var FreightSpeed = km.Speeds.Count > 0 ? km.Speeds[0].Freight : -1;

							var digression = new Digression { };

							digression.Meter = gap.Meter;

							if (gap.Otst_l == "З" || gap.Otst_r == "З")
							{
								digression.PassengerSpeedLimit = int.Parse(gap.Vdop.Split('/')[0]);
								digression.FreightSpeedLimit = int.Parse(gap.Vdop.Split('/')[1]);
							}
							else
							{
								digression.PassengerSpeedLimit = -1;
								digression.FreightSpeedLimit = -1;
							}
							digression.AllowSpeed = gap.Vdop;
							//digression.Velich = Math.Max(gap.Zazor, gap.R_zazor);
							//digression.DigName = gap.Otst == "З" ? DigressionName.Gap : DigressionName.GapSimbol;
						if (gap.Otst_l != "")
						{
							digression.DigName = gap.Otst_l == "З" ? DigressionName.Gap_left : DigressionName.GapSimbol_left;
							digression.Velich = gap.Zazor;
							digression.prim += digression.DigName.Name + " " + digression.Velich;
						}

						if (gap.Otst_r != "")
						{
							digression.DigName = gap.Otst_r == "З" ? DigressionName.Gap_right : DigressionName.GapSimbol_right;
							digression.Velich = gap.R_zazor;
							digression.prim += digression.DigName.Name + " " + digression.Velich;
						}
						gaps.Add(digression);
						}
						foreach (var dig in gaps)
						{
							int count = dig.Length / 4;
							count += dig.Length % 4 > 0 ? 1 : 0;

							var side = " " + (dig.Threat == (Threat)Threat.Right ? "п." : dig.Threat == (Threat)Threat.Left ? "л." : "");

							km.Digressions.Add(
								new DigressionMark()
								{
									Digression = dig.DigName,
									NotMoveAlert = false,
									Meter = dig.Meter,
									finish_meter = dig.Kmetr,
									Length = dig.Length,
									Value = dig.Velich,
									Count = count,
									DigName = dig.GetName(),
									PassengerSpeedLimit = dig.PassengerSpeedLimit,
									FreightSpeedLimit = dig.FreightSpeedLimit,
									Comment = "",
									AllowSpeed = dig.AllowSpeed
								});
						}

						//данные Износа рельса Бок.из.
						var DBcrossRailProfile = AdditionalParametersService.GetCrossRailProfileFromDBbyKm(km.Number, km.Trip.Id);
						if (DBcrossRailProfile == null)
							DBcrossRailProfile = new List<CrosProf> { };
						//continue;

						var sortedData = DBcrossRailProfile.OrderByDescending(d => d.Meter).ToList();
						var crossRailProfile = AdditionalParametersService.GetCrossRailProfileFromDBParse(sortedData);

						List<Digression> addDigressions = crossRailProfile.GetDigressions();

						var dignatur = new List<DigressionMark> { };
						foreach (var dig in addDigressions)
						{
							int count = dig.Length / 4;
							count += dig.Length % 4 > 0 ? 1 : 0;

							if (dig.DigName == DigressionName.SideWearLeft || dig.DigName == DigressionName.SideWearRight)
							{
								dignatur.Add(new DigressionMark()
								{
									Digression = dig.DigName,
									NotMoveAlert = false,
									Meter = dig.Meter,
									finish_meter = dig.Kmetr,
									Degree = (int)dig.Typ,
									Length = dig.Length,
									Value = dig.Value,
									Count = count,
									DigName = dig.GetName(),
									PassengerSpeedLimit = -1,
									FreightSpeedLimit = -1,
									Comment = "",
									Diagram_type = "Iznos_relsov",
									Digtype = DigressionType.Additional
								});
							}
						}
						//выч-е скорости бок износа
						int pas = 999, gruz = 999;
						foreach (DigressionMark item in dignatur)
						{
							if (item.Digression == DigressionName.SideWearLeft || item.Digression == DigressionName.SideWearRight)
							{
								var c = km.Curves.Where(o => o.RealStartCoordinate <= km.Number + item.Meter / 10000.0 && o.RealFinalCoordinate >= km.Number + item.Meter / 10000.0).ToList();

								if (c.Any())
								{
									item.GetAllowSpeedAddParam(km.Speeds.First(), c.First().Straightenings[0].Radius, item.Value);

									if (item.PassengerSpeedLimit != -1 && item.PassengerSpeedLimit < pas)
									{
										pas = item.PassengerSpeedLimit;
									}
									if (item.FreightSpeedLimit != -1 && item.FreightSpeedLimit < gruz)
									{
										gruz = item.FreightSpeedLimit;
									}
								}
							}
							else if (item.Digression == DigressionName.Gap)
							{
								if (item.PassengerSpeedLimit != -1 && item.PassengerSpeedLimit < pas)
								{
									pas = item.PassengerSpeedLimit;
								}
								if (item.FreightSpeedLimit != -1 && item.FreightSpeedLimit < gruz)
								{
									gruz = item.FreightSpeedLimit;
								}
							}

						}
						dignatur = dignatur.Where(o => o.PassengerSpeedLimit >= 0 || o.FreightSpeedLimit >= 0).ToList();

						km.Digressions.AddRange(dignatur);

						var gap_dig = AddParam.Where(o => o.Km == km.Number).ToList();
						foreach (var item in gap_dig)
						{
							km.Digressions.Add(new DigressionMark
							{
								Km = item.Km,
								Meter = item.Meter,
								Length = item.Len,
								DigName = item.Ots,
								Count = item.Kol,
								Value = item.Otkl,
								PassengerSpeedLimit = item.Ogp,
								FreightSpeedLimit = item.Ogp,
								Digtype = DigressionType.Additional
							});
						}


						var Glag_grk = false;




						var kriv = km.Digressions.Where(o => !o.DigName.Contains("кривая факт.") && !o.DigName.Contains("З?") && !o.DigName.Equals("ПрУ")
						&& (o.DigName.Equals("Укл") || o.DigName.Equals("?Уkл") || o.DigName.Equals("Анп") || o.DigName.Equals("Пси") && !o.Comment.Contains("-/-") && !o.Comment.Contains("") && !o.FreightSpeedLimit.Equals("-") && !o.PassengerSpeedLimit.Equals("-"))).ToList();
						if (kriv.Count > 0)
						{

						};
						if (kriv.Any()) Kriv += kriv.Count;


						var pru = km.Digressions.Where(o => !o.DigName.Contains("кривая факт.") && !o.DigName.Contains("З?") && o.DigName.Equals("ПрУ")).ToList();
						if (pru.Any()) Pru += pru.Count;

						var oshk = km.Digressions.Where(o => !o.DigName.Contains("кривая факт.") && !o.DigName.Contains("З?") && !o.DigName.Equals("ПрУ") && o.DigName.Equals("Oтв.ш")).ToList();
						if (oshk.Any()) Oshk += oshk.Count;

						var iznos = km.Digressions.Where(o => o.DigName.Equals("Иб.л") || o.DigName.Equals("Иб.п") || o.DigName.Equals("Ив.л") || o.DigName.Equals("Ив.п") ||
															  o.DigName.Equals("Ип.л") || o.DigName.Equals("Ип.п")).ToList();
						if (iznos.Any()) Iznos += iznos.Count;

						var zazor = km.Digressions.Where(o => !o.DigName.Contains("кривая факт.") && !o.DigName.Equals("ПрУ") && (o.DigName.Equals("З?Л") || o.DigName.Equals("ЗЛ") || o.DigName.Equals("З?П") || o.DigName.Equals("ЗП")  ) && km.Number == km.Digressions.Select(x => x.Km).ToList().First()).ToList();
						if (zazor.Any()) Zazor += zazor.Count;




						//Pu32_gap.Count() + (gapV.Count()
						var kmTotal = new Total();
						Total comparativeKMTotal = null;

						kmTotal.IsKM = true;
						//if (grk.Any())
						//{
						//	kmTotal.Grk = grk.Count;
						//}
						if (kriv.Any())
						{
							kmTotal.Kriv = kriv.Count;
						}
						if (pru.Any())
						{
							kmTotal.Pru = pru.Count;
						}
						if (oshk.Any())
						{
							kmTotal.Oshk = oshk.Count;
						}
						if (iznos.Any())
						{
							kmTotal.Iznos = iznos.Count;
						}
						if (zazor.Any())
						{
							km.Number = km.Number;
							kmTotal.Zazor = zazor.Count;
						}




						var add_dig_str = "";
						var comp_add_dig_str = "";
						var Curve_dig_str = "";
						var comp_curve_dig_str = "";


						//Зазор баллы
						var isgap = Pu32_gap.Where(o => o.Km == km.Number).ToList();
						if (isgap.Any())
						{
							if (isgap.Where(o => o.Otst_l == "З" || o.Otst_r == "З").ToList().Count > 0)
							{
								kmTotal.AddParamPointSum += 50;
								kmTotal.Additional += isgap.Where(x => x.Otst_l == "З" || x.Otst_r == "З").Count();

							}

							else if (isgap.Where(o => o.Otst_l == "З?" || o.Otst_r == "З?").ToList().Count > 0)
							{
								kmTotal.AddParamPointSum += 20;

							}

							Zazor += isgap.Count;
						}



						//Бок из примеч
						//var countBokIz = km.Digressions.Where(o => o.Digression == DigressionName.SideWearLeft || o.Digression == DigressionName.SideWearRight).ToList();
						//               if (countBokIz.Any())
						//               {
						//                   add_dig_str += $" Бок. из {countBokIz.Count}шт";
						//               }


						var prevCurve_id = -1;

						//переменная огр скорости для качественной оценки км
						var Districted = 0;

						if (km.Number == 722)
						{

						}

						foreach (var digression in km.Digressions)
						{
							if (digression.DigName == "П" && digression.Km == 731 && digression.Meter == 590)
							{

							}
							if ((digression.DigName.Contains("Пси") || digression.DigName.Contains("?Пси")) && digression.LimitSpeedToString() == "-/-")
								continue;
							if (km.Number == 720)
							{

							}
							//&& !digression.Comment.Contains("Натурная кривая")
							if ((digression.LimitSpeedToString() != "-/-") && (digression.LimitSpeedToString() != ""))
							{
								Districted++;
							}

							if (digression.Comment != "" || digression.Comment == null)
							{
								if (Districted > 0)
								{
									Districted = Districted - 1;
								}
							}






							if (digression.Digtype == DigressionType.Main)
							{
								kmTotal.MainParamPointSum += digression.GetPoint(km);
								//ToDo - ағамен ақылдасу керек
								kmTotal.CurvePointSum += digression.GetCurvePoint(km);
							}

							//Износ баллы
							if (digression.Digression == DigressionName.SideWearLeft || digression.Digression == DigressionName.SideWearRight)
							{
								if (digression.Km == 708)
								{

								}
								var noteCoord = km.Number.ToDoubleCoordinate(digression.Meter);
								var isCurve = km.Curves.Where(o => o.RealStartCoordinate <= noteCoord && o.RealFinalCoordinate >= noteCoord).ToList();
								if (isCurve.Any())
								{
									if (prevCurve_id != (int)isCurve.First().Id)
									{
										kmTotal.AddParamPointSum += 50;
										kmTotal.Additional++;
									}
									prevCurve_id = (int)isCurve.First().Id;
								}
								add_dig_str += $"V={digression.PassengerSpeedLimit}/{digression.FreightSpeedLimit} пк{(digression.Meter - 1) / 100 + 1} {digression.DigName} {digression.Value:0.0}/{digression.Length}; ";
							}
							//Пру кривая баллы
							if (digression.Digression == DigressionName.Pru)
							{
								kmTotal.Curves++;

								Curve_dig_str += $"пк{(digression.Meter - 1) / 100 + 1} {digression.DigName} {digression.Value}/{digression.Length}; ";
							}
							//ПСИ АНП Укл Аг кривая 
							if (digression.Digression == DigressionName.Psi ||
								digression.Digression == DigressionName.SpeedUp ||
								digression.Digression == DigressionName.Ramp)
							{
								var otkl = "";
								if (digression.Comment.Any())
								{
									try
									{
										otkl = digression.Comment.Split().First().Split(':').Last();
									}
									catch
									{
										otkl = "";
									}
								}
								Curve_dig_str += $"V={digression.PassengerSpeedLimit}/{digression.FreightSpeedLimit} пк{(digression.Meter - 1) / 100 + 1} {digression.DigName} {otkl}; ";
								kmTotal.Curves++;
							}
							//зазоры
							if (digression.Digression.Name == "ЗЛ" || digression.Digression.Name == "ЗП")
							{
								add_dig_str += $"V={digression.PassengerSpeedLimit}/{digression.FreightSpeedLimit} пк{(digression.Meter - 1) / 100 + 1} {digression.DigName} {digression.Value}; ";
							}
							else if (digression.Digression.Name == "З?Л" || digression.Digression.Name == "З?П")
							{
								add_dig_str += $"пк{(digression.Meter - 1) / 100 + 1} {digression.DigName} {digression.Value}; ";
							}


							//if (digression.Digtype == DigressionType.Additional)
							//{

							//	var ball = find_add == true ? 0 : 50;
							//	kmTotal.AddParamPointSum += ball;
							//	find_add = true;

							//	add_dig_str += $"V={digression.PassengerSpeedLimit}/{digression.FreightSpeedLimit} пк{digression.Meter / 100 + 1} {digression.DigName} {digression.Value}/{digression.Length} ";
							//}

							digression.DigNameToDigression(digression.DigName);

							if (digression.Degree < 5)
								switch (digression.DigName)
								{
									case string digname when digname.Equals("Суж"):
										digression.Digression = DigressionName.Constriction;
										kmTotal.Constriction.Degrees[digression.Degree - 1] += digression.GetCount();
										break;

									case string digname when digname.Equals("Уш"):
										digression.Digression = DigressionName.Broadening;
										kmTotal.Broadening.Degrees[digression.Degree - 1] += digression.GetCount();
										break;

									case string digname when digname.Equals("У"):
										digression.Digression = DigressionName.Level;
										kmTotal.Level.Degrees[digression.Degree - 1] += digression.GetCount();
										break;

									case string digname when digname.Equals("П"):
										digression.Digression = DigressionName.Sag;
										kmTotal.Sag.Degrees[digression.Degree - 1] += digression.GetCount();
										break;
									case string digname when digname.Equals("Пр.л") || digname.Equals("Пр.п"):
										digression.Digression = (digname.Equals("Пр.л")) ? DigressionName.DrawdownLeft : DigressionName.DrawdownRight;
										kmTotal.Drawdown.Degrees[digression.Degree - 1] += digression.GetCount();
										break;
									case string digname when digname.Equals("Р"):
										digression.Digression = DigressionName.Strightening;
										kmTotal.Strightening.Degrees[digression.Degree - 1] += digression.GetCount();
										break;
									case string digname when (digname.Equals("Рнр") && digression.Degree == 4):
										digression.Digression = DigressionName.Strightening;
										kmTotal.Strightening.Degrees[digression.Degree - 1] += digression.GetCount();
										break;
									case string digname when digname.Equals("Рст"):
										digression.Digression = DigressionName.Strightening;
										kmTotal.Strightening.Degrees[digression.Degree - 1] += digression.GetCount();
										break;
								}

							if (digression.DigName.Equals("Р") && digression.Degree == 2)
							{
								var kmq = digression.Km;
								var met = digression.Meter;
								DevDegree2++;
							}
							var flag_gr = false;
							if (digression.Comment != null) if (digression.primech == null)  if (digression.Comment.Contains("гр")) flag_gr = true;
							if (((digression.LimitSpeedToString().Any(char.IsDigit) || flag_gr) && !digression.DigName.Equals("ПрУ")
								&& !digression.DigName.Contains("кривая факт.") && !digression.DigName.Contains("З?") && !digression.Comment.Contains("-/-")))
							{
								kmTotal.IsLimited = 1;
								//MessageBox.Show(digression.Km.ToString() + '-' +digression.DigName + '-' + digression.LimitSpeedToString());

								if (digression.Degree == 4 && digression.Digtype == DigressionType.Main && !digression.DigName.Contains("З?")
									&& !digression.DigName.Contains("кривая факт.") && !digression.DigName.Equals("ПрУ"))
								{
									kmTotal.Fourth = kmTotal.Fourth + digression.GetCount();
								}
								//if (digression.Digtype != DigressionType.Main && !digression.DigName.Contains("З?") && !digression.DigName.Equals("ПрУ") && !digression.DigName.Contains("кривая факт."))
								if (digression.DigName.Contains("З") && !digression.DigName.Contains("З?") && !digression.DigName.Equals("ПрУ") && !digression.DigName.Contains("кривая факт.") && digression.DigName.Contains("Изн"))

								{
									kmTotal.Additional++;
								}

							}
							if (digression.Comment != null)
							{

								if ((digression.Comment.Contains("ОШК") || digression.Comment.Contains("ОШП") || digression.Comment.Contains("Уобр")) &&

									!digression.primech.Contains("м") && !digression.Comment.Contains("гр") && !digression.DigName.Contains("З") && !digression.DigName.Equals("ПрУ") && !digression.DigName.Contains("кривая факт.")
									&& !(digression.Degree == 4 && digression.Digtype == DigressionType.Main) && !digression.DigName.Contains("Изн") && digression.LimitSpeedToString().Any(char.IsDigit)

									)


								{
									//kmTotal.IsLimited = 1;
									kmTotal.Other++; 


								}

						//
							if (((digression.Comment.Contains("гр") || digression.Comment.Contains("м") ) && (digression.Degree == 3)) 
								&& !digression.DigName.Contains("З") && !digression.DigName.Equals("ПрУ") && !digression.DigName.Contains("кривая факт.")
								&& (!(digression.Degree == 4) && digression.Digtype == DigressionType.Main) && !digression.DigName.Contains("Изн"))
							///сделать сортировку по "гр.ис.Пр,м" с органичемнием скрости  что то типо digression.Comment.Contains("м") || digression.Comment.Contains("ис")


							{
								kmTotal.Grk = kmTotal.Grk + 1;

								}

								if ((digression.Degree == 3) && !digression.DigName.Contains("З") && !digression.DigName.Equals("ПрУ") && !digression.DigName.Contains("кривая факт.")
									&& (!(digression.Degree == 3) && digression.Digtype == DigressionType.Main) && !digression.DigName.Contains("Изн")
										&&
										(digression.DigName.Contains("П") && digression.DigName.Contains("Р") || digression.DigName.Contains("П") && digression.DigName.Contains("Пр") ||
										digression.DigName.Contains("Пр") && digression.DigName.Contains("Р")
											))
								{
									kmTotal.Sochet = Sochet + 1;

								}

							}


						}
						kmTotal.Grk = kmTotal.Grk + kmTotal.Fourth;
						kmTotal.Broadening.Degrees[0] = km.FDBroad;
						kmTotal.Constriction.Degrees[0] = km.FDConstrict;
						kmTotal.Sag.Degrees[0] = km.FDSkew;
						kmTotal.Strightening.Degrees[0] = km.FDStright;
						kmTotal.Drawdown.Degrees[0] = km.FDDrawdown;
						kmTotal.Level.Degrees[0] = km.FDLevel;

						if (km.Number == 720)
						{
							var kmmm = km.Number;
						}
						kmTotal.QualitiveRating = km.CalcQualitiveRating(kmTotal.MainParamPointSum + kmTotal.CurvePointSum, Districted);
						var kmElement = new XElement("km",
						new XAttribute("n", km.Number),
						new XAttribute("len", km.Lkm % 1 == 0 ? km.Lkm.ToString("0", nfi) : km.Lkm.ToString("0.000", nfi)),
						new XAttribute("c1", kmTotal.Constriction.ToString()),
						new XAttribute("c2", kmTotal.Broadening.ToString()),
						new XAttribute("c3", kmTotal.Level.ToString()),
						new XAttribute("c4", kmTotal.Sag.ToString()),
						new XAttribute("c5", kmTotal.Drawdown.ToString()),
						new XAttribute("c6", kmTotal.Strightening.ToString()),
						new XAttribute("c7", kmTotal.Common),
						new XAttribute("c8", kmTotal.FourthOtherAdd == "-/ - / - " ? "" : kmTotal.FourthOtherAdd),
						new XAttribute("c9", $"{kmTotal.MainParamPointSum + kmTotal.CurvePointSum}/{kmTotal.AddParamPointSum}"),
						new XAttribute("c10", kmTotal.QualitiveRating),
						new XAttribute("c11", km.Primech + " "), // Отступления с таблицы бедомость
						new XAttribute("c12", add_dig_str), //Отступления доп пар
						new XAttribute("c13", Curve_dig_str), //Отступления кривые
						new XAttribute("apoint", (kmTotal.AddParamPointSum > 0 ? kmTotal.AddParamPointSum.ToString() : "---")), //балл отст для доп пар

						new XAttribute("speed", $"{(km.Speeds.Count > 0 ? $"{km.Speeds.Last().Passenger}/{km.Speeds.Last().Freight}" : $"{km.GlobPassSpeed}/{km.GlobFreightSpeed}")}"),
						new XAttribute("cor", (km.CorrCount > 0 ? "+" : "") + (km.IsLowActivity ? "*" : "")),
						new XAttribute("mpoint", kmTotal.MainParamPointSum),

						new XAttribute("allsum", (kmTotal.MainParamPointSum + kmTotal.CurvePointSum + kmTotal.AddParamPointSum)), //итого по км

						new XAttribute("cpoint", (kmTotal.CurvePointSum > 0 ? kmTotal.CurvePointSum.ToString() : "---"))

						);
						if (reportType == PU32Type.Comparative)
						{
							var rowPeriod = period.Period.Equals(comparativePeriod.Period) ? (tripType == TripType.Control ? "контрольная" : tripType == TripType.Work ? "рабочая" : "дополнительная") : period.PeriodMonthMonth;
							kmElement.Add(new XAttribute("period", rowPeriod));
						}
						int kmRowCount = 1;

						if (reportType == PU32Type.Comparative && compKm != null)
						{
							comparativeKMTotal = new Total() { IsKM = true };
							var compGap = comparativePu32_gap.Where(o => o.Km == compKm.Number).ToList();
							if (compGap.Any())
							{
								if (compGap.Where(o => o.Otst_l == "З" || o.Otst_r == "З").ToList().Count > 0)
									comparativeKMTotal.AddParamPointSum += 50;
								else if (compGap.Where(o => o.Otst_l == "З?" || o.Otst_r == "З?").ToList().Count > 0)
									comparativeKMTotal.AddParamPointSum += 20;
								CompZazor++;
							}
							prevCurve_id = -1;
							var compDistricted = 0;
							foreach (var digression in compKm.Digressions)
							{
								if ((digression.DigName.Contains("Пси") || digression.DigName.Contains("?Пси")) && digression.LimitSpeedToString() == "-/-")
									continue;

								if ((digression.LimitSpeedToString() != "-/-") && (digression.LimitSpeedToString() != ""))
								{
									compDistricted++;
								}

								if (digression.Digtype == DigressionType.Main)
								{
									comparativeKMTotal.MainParamPointSum += digression.GetPoint(compKm);
									//ToDo - ағамен ақылдасу керек
									comparativeKMTotal.CurvePointSum += digression.GetCurvePoint(compKm);
								}

								//Износ баллы		
								if (digression.Digression == DigressionName.SideWearLeft || digression.Digression == DigressionName.SideWearRight)
								{
									var noteCoord = compKm.Number.ToDoubleCoordinate(digression.Meter);
									var isCurve = compKm.Curves.Where(o => o.RealStartCoordinate <= noteCoord && o.RealFinalCoordinate >= noteCoord).ToList();
									if (isCurve.Any())
									{
										if (prevCurve_id != (int)isCurve.First().Id)
										{
											comparativeKMTotal.AddParamPointSum += 50;
											comparativeKMTotal.Additional++;
										}
										prevCurve_id = (int)isCurve.First().Id;
									}
									comp_add_dig_str += $"V={digression.PassengerSpeedLimit}/{digression.FreightSpeedLimit} пк{(digression.Meter - 1) / 100 + 1} {digression.DigName} {digression.Value:0.0}/{digression.Length}; ";
								}
								//Пру кривая баллы
								if (digression.Digression == DigressionName.Pru)
								{
									comparativeKMTotal.Curves++;

									comp_curve_dig_str += $"пк{(digression.Meter - 1) / 100 + 1} {digression.DigName} {digression.Value}/{digression.Length}; ";
								}
								//ПСИ АНП Укл Аг кривая 
								if (digression.Digression == DigressionName.Psi ||
									digression.Digression == DigressionName.SpeedUp ||
									digression.Digression == DigressionName.Ramp)
								{
									var otkl = "";
									if (digression.Comment.Any())
									{
										try
										{
											otkl = digression.Comment.Split().First().Split(':').Last();
										}
										catch
										{
											otkl = "";
										}
									}
									comp_curve_dig_str += $"V={digression.PassengerSpeedLimit}/{digression.FreightSpeedLimit} пк{(digression.Meter - 1) / 100 + 1} {digression.DigName} {otkl}; ";
									comparativeKMTotal.Curves++;
								}
								//зазоры
								if (digression.Digression.Name == "ЗЛ" || digression.Digression.Name == "ЗП")
								{
									comp_add_dig_str += $"V={digression.PassengerSpeedLimit}/{digression.FreightSpeedLimit} пк{(digression.Meter - 1) / 100 + 1} {digression.DigName} {digression.Value}; ";
								}
								else if (digression.Digression.Name == "З?Л" || digression.Digression.Name == "З?П")
							{
									comp_add_dig_str += $"пк{(digression.Meter - 1) / 100 + 1} {digression.DigName} {digression.Value}; ";
								}


								//if (digression.Digtype == DigressionType.Additional)
								//{

								//	var ball = find_add == true ? 0 : 50;
								//	kmTotal.AddParamPointSum += ball;
								//	find_add = true;

								//	add_dig_str += $"V={digression.PassengerSpeedLimit}/{digression.FreightSpeedLimit} пк{digression.Meter / 100 + 1} {digression.DigName} {digression.Value}/{digression.Length} ";
								//}

								digression.DigNameToDigression(digression.DigName);





								if (digression.Degree < 5)
									switch (digression.DigName)
									{
										case string digname when digname.Equals("Суж"):
											digression.Digression = DigressionName.Constriction;
											comparativeKMTotal.Constriction.Degrees[digression.Degree - 1] += digression.GetCount();
											break;
										case string digname when digname.Equals("Уш"):
											digression.Digression = DigressionName.Broadening;
											comparativeKMTotal.Broadening.Degrees[digression.Degree - 1] += digression.GetCount();
											break;
										case string digname when digname.Equals("У"):
											digression.Digression = DigressionName.Level;
											comparativeKMTotal.Level.Degrees[digression.Degree - 1] += digression.GetCount();
											break;
										case string digname when digname.Equals("П"):
											digression.Digression = DigressionName.Sag;
											comparativeKMTotal.Sag.Degrees[digression.Degree - 1] += digression.GetCount();
											break;
										case string digname when digname.Equals("Пр.л") || digname.Equals("Пр.п"):
											digression.Digression = (digname.Equals("Пр.л")) ? DigressionName.DrawdownLeft : DigressionName.DrawdownRight;
											comparativeKMTotal.Drawdown.Degrees[digression.Degree - 1] += digression.GetCount();
											break;
										case string digname when digname.Equals("Р"):
											digression.Digression = DigressionName.Strightening;
											comparativeKMTotal.Strightening.Degrees[digression.Degree - 1] += digression.GetCount();
											break;
										case string digname when digname.Equals("Рнр") && digression.Degree == 4:
											digression.Digression = DigressionName.Strightening;
											comparativeKMTotal.Strightening.Degrees[digression.Degree - 1] += digression.GetCount();
											break;
									}

								if (digression.DigName.Equals("Р") && digression.Degree == 2)
								{
									var kmq = digression.Km;
									var met = digression.Meter;
									compDevDegree2++;
								}

								var flag_gr = false;
								if (digression.Comment != null) if (digression.Comment.Contains("гр")) flag_gr = true;

								if ((digression.LimitSpeedToString().Any(char.IsDigit) && !digression.Equals("ПрУ") && !digression.DigName.Contains("кривая факт.")) || flag_gr)
								{
									comparativeKMTotal.IsLimited = 1;

									if (digression.Degree == 4 && digression.Digtype == DigressionType.Main)
									{
										comparativeKMTotal.Fourth = comparativeKMTotal.Fourth + digression.GetCount();
									}

								}
							}
							comparativeKMTotal.Broadening.Degrees[0] = compKm.FDBroad;
							comparativeKMTotal.Constriction.Degrees[0] = compKm.FDConstrict;
							comparativeKMTotal.Sag.Degrees[0] = compKm.FDSkew;
							comparativeKMTotal.Strightening.Degrees[0] = compKm.FDStright;
							comparativeKMTotal.Drawdown.Degrees[0] = compKm.FDDrawdown;
							comparativeKMTotal.Level.Degrees[0] = compKm.FDLevel;

							comparativeKMTotal.QualitiveRating = compKm.CalcQualitiveRating(comparativeKMTotal.MainParamPointSum + comparativeKMTotal.CurvePointSum, compDistricted);

							var cRowPeriod = period.Period.Equals(comparativePeriod.Period) ? (comparativeTripType == TripType.Control ? "контрольная" : comparativeTripType == TripType.Work ? "рабочая" : "дополнительная") : comparativePeriod.PeriodMonthMonth;
							kmElement.Add(
								new XAttribute("clen", compKm.Lkm % 1 == 0 ? compKm.Lkm.ToString("0", nfi) : compKm.Lkm.ToString("0.000", nfi)),
								new XAttribute("cc1", comparativeKMTotal.Constriction.ToString()),
								new XAttribute("cc2", comparativeKMTotal.Broadening.ToString()),
								new XAttribute("cc3", comparativeKMTotal.Level.ToString()),
								new XAttribute("cc4", comparativeKMTotal.Sag.ToString()),
								new XAttribute("cc5", comparativeKMTotal.Drawdown.ToString()),
								new XAttribute("cc6", comparativeKMTotal.Strightening.ToString()),
								new XAttribute("cc7", comparativeKMTotal.Common),
								new XAttribute("cc8", comparativeKMTotal.FourthOtherAdd == "-/ - / - " ? "" : comparativeKMTotal.FourthOtherAdd),
								new XAttribute("cc9", $"{comparativeKMTotal.MainParamPointSum + comparativeKMTotal.CurvePointSum}/{comparativeKMTotal.AddParamPointSum}"),
								new XAttribute("cc10", comparativeKMTotal.QualitiveRating),
								new XAttribute("cc11", compKm.Primech + " "), // Отступления с таблицы бедомость
								new XAttribute("cc12", comp_add_dig_str), //Отступления доп пар
								new XAttribute("cc13", comp_curve_dig_str), //Отступления кривые
								new XAttribute("capoint", (comparativeKMTotal.AddParamPointSum > 0 ? comparativeKMTotal.AddParamPointSum.ToString() : "---")), //балл отст для доп пар

								new XAttribute("cspeed", $"{(compKm.Speeds.Count > 0 ? $"{compKm.Speeds.Last().Passenger}/{compKm.Speeds.Last().Freight}" : $"{compKm.GlobPassSpeed}/{compKm.GlobFreightSpeed}")}"),
								new XAttribute("ccor", (compKm.CorrCount > 0 ? "+" : "") + (compKm.IsLowActivity ? "*" : "")),
								new XAttribute("cmpoint", comparativeKMTotal.MainParamPointSum),

								new XAttribute("callsum", (comparativeKMTotal.MainParamPointSum + comparativeKMTotal.CurvePointSum + comparativeKMTotal.AddParamPointSum)), //итого по км
								new XAttribute("ccpoint", (comparativeKMTotal.CurvePointSum > 0 ? comparativeKMTotal.CurvePointSum.ToString() : "---")),

								new XAttribute("cperiod", cRowPeriod)

								);
							kmRowCount = 2;

						}
						kmElement.Add(new XAttribute("rowCount", kmRowCount));
						kmElement.Add(new XAttribute("isComp", (reportType == PU32Type.Comparative ? 1 : 0)));


						//TODO-----
						//kmTotal.QualitiveRating = km.Primech.Length > 0 ? Rating.Н : kmTotal.QualitiveRating;
						//---------




						kmTotal.Length = km.Lkm;

						if (compPdbTotal != null && comparativeKMTotal != null)
						{
							comparativeKMTotal.Length = compKm.Lkm;
							compPdbTotal += comparativeKMTotal;


						}
						pdbTotal += kmTotal;

						pdbElement.Add(kmElement);

				

				}


					int sumGrk = 0, sumKriv = 0, sumPru = 0, sumOshk = 0, sumIznos = 0, sumPu32_gap_count = 0, sumKNerProf = 0, sumZazor = 0;



				PDBTotalGenerate(ref pdbElement, ref pdbTotal, ref pdElement, ref pdTotal, "", "", ref compPdbTotal, ref compPdTotal);
				PDTotalGenerate(ref pdElement, ref pdTotal, ref pchuElement, ref pchuTotal, "", "", ref compPdTotal, ref compPchuTotal);
				PCHUTotalGenerate(ref pchuElement, ref pchuTotal, ref sectionElement, ref sectionTotal, "", "", ref compPchuTotal, ref compSectionTotal, distanceTotal.Code);
				SectionTotalGenerate(ref sectionElement, ref sectionTotal, ref byKilometer, ref distanceTotal, "", "", "", ref compSectionTotal, ref compDistanceTotal);

				var avgBall1 = (distanceTotal.MainParamPointSum + distanceTotal.CurvePointSum) / distanceTotal.Length;
				var avgBall2 = (distanceTotal.MainParamPointSum + distanceTotal.CurvePointSum + distanceTotal.AddParamPointSum) / distanceTotal.Length;

				byKilometer.Add(new XAttribute("len", distanceTotal.Length % 1 == 0 ? distanceTotal.Length.ToString("0", nfi) : distanceTotal.Length.ToString("0.000", nfi)),
					new XAttribute("point", $"{distanceTotal.MainParamPointSum + distanceTotal.CurvePointSum}/{distanceTotal.AddParamPointSum}"),
					new XAttribute("rating", distanceTotal.GetSectorQualitiveRating()),
					new XAttribute("ratecount",
					$"Отл - {distanceTotal.RatingCounts[0].ToString("0", nfi)}; Хор - {distanceTotal.RatingCounts[1].ToString("0", nfi)}; Уд - {distanceTotal.RatingCounts[2].ToString("0", nfi)}; Неуд - {distanceTotal.RatingCounts[3].ToString("0", nfi)}; Средний балл - {avgBall1:0}/{avgBall2:0}"),
					new XAttribute("c1", distanceTotal.Constriction.ToString()),
					new XAttribute("c2", distanceTotal.Broadening.ToString()),
					new XAttribute("c3", distanceTotal.Level.ToString()),
					new XAttribute("c4", distanceTotal.Sag.ToString()),
					new XAttribute("c5", distanceTotal.Drawdown.ToString()),
					new XAttribute("c6", distanceTotal.Strightening.ToString()),
					new XAttribute("c7", distanceTotal.Common),
					new XAttribute("c8", distanceTotal.FourthOtherAdd),
					new XAttribute("fourt", $"{(distanceTotal.Fourth > 0 ? distanceTotal.Fourth.ToString() : "-")}"),
					new XAttribute("excellent", distanceTotal.RatingCounts[0].ToString("0.000", nfi)),
					new XAttribute("good", distanceTotal.RatingCounts[1].ToString("0.000", nfi)),
					new XAttribute("satisfactory", distanceTotal.RatingCounts[2].ToString("0.000", nfi)),
					new XAttribute("bad", distanceTotal.RatingCounts[3].ToString("0.000", nfi)),
					new XAttribute("limit", distanceTotal.IsLimited),
					new XAttribute("d4", distanceTotal.Fourth),
					//
					new XAttribute("SOCHET", "0"),//to doo 
					new XAttribute("Grk", distanceTotal.Grk),
					new XAttribute("KRIV", distanceTotal.Kriv),
					new XAttribute("PRU", distanceTotal.Pru),
					new XAttribute("OSHK", distanceTotal.Oshk),
					new XAttribute("IZNOS", distanceTotal.Iznos),
					new XAttribute("ZAZOR", distanceTotal.Zazor), //
					new XAttribute("NEROVNOSTY", distanceTotal.NerProf),
					//


					new XAttribute("other", (distanceTotal.Combination + distanceTotal.Curves + distanceTotal.Other)),
					new XAttribute("add", distanceTotal.Additional),
					new XAttribute("repair", distanceTotal.Repairing),
					new XAttribute("mainavg", avgBall1.ToString("0.0", nfi)),
					new XAttribute("addavg", avgBall2.ToString("0.0", nfi)),
					new XAttribute("ns", distanceTotal.GetSectorQualitiveRating().Split('/')[0]),
					new XAttribute("ratings", distanceTotal.GetSectorQualitiveRating().Split('/')[1]),
					new XAttribute("count", distanceTotal.Count),
					new XAttribute("ttype1", reportType == PU32Type.Comparative ? (tripType == TripType.Control ? "контрольная" : (tripType == TripType.Additional ? "дополнительная" : "рабочая")) : ""),

					new XAttribute("ttype", (tripType == TripType.Control ? "контрольная " : tripType == TripType.Work ? "рабочая " : "дополнительная ") + period.PeriodMonthMonth + " " + period.PeriodYear + "г."));
				if (compDistanceTotal != null)
				{
					var cavgBall1 = (compDistanceTotal.MainParamPointSum + compDistanceTotal.CurvePointSum) / compDistanceTotal.Length;
					var cavgBall2 = (compDistanceTotal.MainParamPointSum + compDistanceTotal.CurvePointSum + compDistanceTotal.AddParamPointSum) / compDistanceTotal.Length;
					byKilometer.Add(new XAttribute("clen", compDistanceTotal.Length % 1 == 0 ? compDistanceTotal.Length.ToString("0", nfi) : compDistanceTotal.Length.ToString("0.000", nfi)),
					new XAttribute("cpoint", $"{compDistanceTotal.MainParamPointSum + compDistanceTotal.CurvePointSum}/{compDistanceTotal.AddParamPointSum}"),
					new XAttribute("crating", compDistanceTotal.GetSectorQualitiveRating()),
					new XAttribute("cratecount",
					$"Отл - {compDistanceTotal.RatingCounts[0].ToString("0", nfi)}; Хор - {compDistanceTotal.RatingCounts[1].ToString("0", nfi)}; Уд - {compDistanceTotal.RatingCounts[2].ToString("0", nfi)}; Неуд - {compDistanceTotal.RatingCounts[3].ToString("0", nfi)}; Средний балл - {cavgBall1:0}/{cavgBall2:0}"),
					new XAttribute("cc1", compDistanceTotal.Constriction.ToString()),
					new XAttribute("cc2", compDistanceTotal.Broadening.ToString()),
					new XAttribute("cc3", compDistanceTotal.Level.ToString()),
					new XAttribute("cc4", compDistanceTotal.Sag.ToString()),
					new XAttribute("cc5", compDistanceTotal.Drawdown.ToString()),
					new XAttribute("cc6", compDistanceTotal.Strightening.ToString()),
					new XAttribute("cc7", compDistanceTotal.Common),
					new XAttribute("cc8", compDistanceTotal.FourthOtherAdd),
					new XAttribute("cfourt", $"{(compDistanceTotal.Fourth > 0 ? compDistanceTotal.Fourth.ToString() : "-")}"),
					new XAttribute("cexcellent", compDistanceTotal.RatingCounts[0].ToString("0.000", nfi)),
					new XAttribute("cgood", compDistanceTotal.RatingCounts[1].ToString("0.000", nfi)),
					new XAttribute("csatisfactory", compDistanceTotal.RatingCounts[2].ToString("0.000", nfi)),
					new XAttribute("cbad", compDistanceTotal.RatingCounts[3].ToString("0.000", nfi)),
					new XAttribute("climit", compDistanceTotal.IsLimited),
					new XAttribute("cd4", compDistanceTotal.Fourth),
					new XAttribute("cother", (compDistanceTotal.Combination + compDistanceTotal.Curves + compDistanceTotal.Other)),
					new XAttribute("cadd", compDistanceTotal.Additional),
					new XAttribute("crepair", compDistanceTotal.Repairing),
					new XAttribute("cmainavg", cavgBall1.ToString("0.0", nfi)),
					new XAttribute("caddavg", cavgBall2.ToString("0.0", nfi)),
					new XAttribute("cns", compDistanceTotal.GetSectorQualitiveRating().Split('/')[0]),
					new XAttribute("cratings", compDistanceTotal.GetSectorQualitiveRating().Split('/')[1]),
					new XAttribute("cttype1", reportType == PU32Type.Comparative ? (comparativeTripType == TripType.Control ? "контрольная" : (comparativeTripType == TripType.Additional ? "дополнительная" : "рабочая")) : ""),

					new XAttribute("cttype", (comparativeTripType == TripType.Control ? "контрольная " : comparativeTripType == TripType.Work ? "рабочая " : "дополнительная ") + comparativePeriod.PeriodMonthMonth + " " + comparativePeriod.PeriodYear + "г.")

					);

				}

				for (int i = 0; i < 4; i++)
				{
					var countByTypeL = new XElement("countbytype");
					countByTypeL.Add(
						new XAttribute("degree", (i == 0 ? "I" : i == 1 ? "II" : i == 2 ? "III" : i == 3 ? "IV" : "")),
						new XAttribute("const", distanceTotal.Constriction.Degrees[i] > 0 ? $"{distanceTotal.Constriction.Degrees[i]}" : ""),
						new XAttribute("broad", distanceTotal.Broadening.Degrees[i] > 0 ? $"{distanceTotal.Broadening.Degrees[i]}" : ""),
						new XAttribute("level", distanceTotal.Level.Degrees[i] > 0 ? $"{distanceTotal.Level.Degrees[i]}" : ""),
						new XAttribute("sag", distanceTotal.Sag.Degrees[i] > 0 ? $"{distanceTotal.Sag.Degrees[i]}" : ""),
						new XAttribute("down", distanceTotal.Drawdown.Degrees[i] > 0 ? $"{distanceTotal.Drawdown.Degrees[i]}" : ""),
						new XAttribute("stright", distanceTotal.Strightening.Degrees[i] > 0 ? $"{distanceTotal.Strightening.Degrees[i]}" : ""),

						new XAttribute("sum", (distanceTotal.Constriction.Degrees[i] + distanceTotal.Broadening.Degrees[i] +
												distanceTotal.Level.Degrees[i] + distanceTotal.Sag.Degrees[i] +
												distanceTotal.Strightening.Degrees[i] + distanceTotal.Drawdown.Degrees[i]))

						);
					if (compDistanceTotal != null)
					{
						countByTypeL.Add(
						new XAttribute("cconst", compDistanceTotal.Constriction.Degrees[i] > 0 ? $"{compDistanceTotal.Constriction.Degrees[i]}" : ""),
						new XAttribute("cbroad", compDistanceTotal.Broadening.Degrees[i] > 0 ? $"{compDistanceTotal.Broadening.Degrees[i]}" : ""),
						new XAttribute("clevel", compDistanceTotal.Level.Degrees[i] > 0 ? $"{compDistanceTotal.Level.Degrees[i]}" : ""),
						new XAttribute("csag", compDistanceTotal.Sag.Degrees[i] > 0 ? $"{compDistanceTotal.Sag.Degrees[i]}" : ""),
						new XAttribute("cdown", compDistanceTotal.Drawdown.Degrees[i] > 0 ? $"{compDistanceTotal.Drawdown.Degrees[i]}" : ""),
						new XAttribute("cstright", compDistanceTotal.Strightening.Degrees[i] > 0 ? $"{compDistanceTotal.Strightening.Degrees[i]}" : ""),

						new XAttribute("csum", (compDistanceTotal.Constriction.Degrees[i] + compDistanceTotal.Broadening.Degrees[i] +
												compDistanceTotal.Level.Degrees[i] + compDistanceTotal.Sag.Degrees[i] +
												compDistanceTotal.Strightening.Degrees[i] + compDistanceTotal.Drawdown.Degrees[i]))
						);

					}
					byKilometer.Add(countByTypeL);
				}

				var countByType = new XElement("countbytype",
					new XAttribute("combination", distanceTotal.Combination == 0 ? "" : distanceTotal.Combination.ToString()),
					new XAttribute("other", distanceTotal.Other == 0 ? "" : distanceTotal.Other.ToString()),
					new XAttribute("curves", distanceTotal.Curves == 0 ? "" : distanceTotal.Curves.ToString()),
					new XAttribute("additional", distanceTotal.Additional == 0 ? "" : distanceTotal.Additional.ToString()),
					new XAttribute("sum", distanceTotal.Combination + distanceTotal.Other + distanceTotal.Curves + distanceTotal.Additional)
					);
				if (compDistanceTotal != null)
				{
					countByType.Add(
						new XAttribute("ccombination", compDistanceTotal.Combination == 0 ? "" : compDistanceTotal.Combination.ToString()),
						new XAttribute("cother", compDistanceTotal.Other == 0 ? "" : compDistanceTotal.Other.ToString()),
						new XAttribute("ccurves", compDistanceTotal.Curves == 0 ? "" : compDistanceTotal.Curves.ToString()),
						new XAttribute("cadditional", compDistanceTotal.Additional == 0 ? "" : compDistanceTotal.Additional.ToString()),
						new XAttribute("csum", compDistanceTotal.Combination + compDistanceTotal.Other + compDistanceTotal.Curves + compDistanceTotal.Additional)

						);
					byKilometer.Add(countByType);
				}
				var constriction = distanceTotal.Constriction.Degrees.Sum();
				var broadening = distanceTotal.Broadening.Degrees.Sum();
				var level = distanceTotal.Level.Degrees.Sum();
				var sag = distanceTotal.Sag.Degrees.Sum();
				var drawdown = distanceTotal.Drawdown.Degrees.Sum();
				var strightening = distanceTotal.Strightening.Degrees.Sum();
				var sum = distanceTotal.Combination + distanceTotal.Other + distanceTotal.Curves + distanceTotal.Additional
						+ constriction + broadening + level + sag + drawdown + strightening;

				byKilometer.Add(new XElement("countbytype",
						new XAttribute("degree", "Итого"),
						new XAttribute("const", constriction == 0 ? "" : constriction.ToString()),
						new XAttribute("broad", broadening == 0 ? "" : broadening.ToString()),
						new XAttribute("level", level == 0 ? "" : level.ToString()),
						new XAttribute("sag", sag == 0 ? "" : sag.ToString()),
						new XAttribute("down", drawdown == 0 ? "" : drawdown.ToString()),
						new XAttribute("stright", strightening == 0 ? "" : strightening.ToString()),
						new XAttribute("combination", distanceTotal.Combination == 0 ? "" : distanceTotal.Combination.ToString()),
						new XAttribute("other", distanceTotal.Other == 0 ? "" : distanceTotal.Other.ToString()),
						new XAttribute("curves", distanceTotal.Curves == 0 ? "" : distanceTotal.Curves.ToString()),
						new XAttribute("additional", distanceTotal.Additional == 0 ? "" : distanceTotal.Additional.ToString()),

						new XAttribute("sum", sum)));

				byKilometer.Add(new XElement("countbytype",
						new XAttribute("degree", "%"),
						new XAttribute("const", GetPercentage(constriction, sum)),
						new XAttribute("broad", GetPercentage(broadening, sum)),
						new XAttribute("level", GetPercentage(level, sum)),
						new XAttribute("sag", GetPercentage(sag, sum)),
						new XAttribute("down", GetPercentage(drawdown, sum)),
						new XAttribute("stright", GetPercentage(strightening, sum)),
						new XAttribute("combination", GetPercentage(distanceTotal.Combination, sum)),
						new XAttribute("other", GetPercentage(distanceTotal.Other, sum)),
						new XAttribute("curves", GetPercentage(distanceTotal.Curves, sum)),
						new XAttribute("additional", GetPercentage(distanceTotal.Additional, sum)),
						new XAttribute("sum", 100)));
				tripElment.Add(byKilometer,
					new XAttribute("len", distanceTotal.Length % 1 == 0 ? distanceTotal.Length.ToString("0", nfi) : distanceTotal.Length.ToString("0.000", nfi)),
					new XAttribute("c1", distanceTotal.Constriction),
					new XAttribute("c2", distanceTotal.Broadening),
					new XAttribute("c3", distanceTotal.Level),
					new XAttribute("c4", distanceTotal.Sag),
					new XAttribute("c5", distanceTotal.Drawdown),
					new XAttribute("c6", distanceTotal.Strightening),
					new XAttribute("c7", distanceTotal.Common),
					new XAttribute("c8", distanceTotal.FourthOtherAdd),
					new XAttribute("ratecount", $"Отл - {distanceTotal.RatingCounts[0].ToString("0", nfi)}; Хор - {distanceTotal.RatingCounts[1].ToString("0", nfi)}; Уд - {distanceTotal.RatingCounts[2].ToString("0", nfi)}; Неуд - {distanceTotal.RatingCounts[3].ToString("0", nfi)}; Средний балл - {avgBall1:0}/{avgBall2:0}"),

					new XAttribute("point", $"{distanceTotal.MainParamPointSum + distanceTotal.CurvePointSum}/{distanceTotal.AddParamPointSum}"),
					new XAttribute("rating", distanceTotal.GetSectorQualitiveRating().Split('/')[1]),
					new XAttribute("rtype", (tripType == TripType.Additional ? "Доп." : tripType == TripType.Control ? "Конт." : "Раб.") + " " + period.Period)); ; ;


				tripElment.Add(pdElementn);

				report.Add(tripElment,
					new XAttribute("code", kilometers[0].Direction_code),
					new XAttribute("track", kilometers[0].Track_name),
					new XAttribute("name", kilometers[0].Direction_name),
					new XAttribute("pch", distance.Code));

				xdReport.Add(report);
				XslCompiledTransform transform = new XslCompiledTransform();
				transform.Load(XmlReader.Create(new StringReader(template.Xsl)));
				transform.Transform(xdReport.CreateReader(), writer);
			}
			
			try
			{
				htReport.Save(Path.GetTempPath() + "/report-pu32.html");
				htReport.Save($@"g:\form\1.Основные и дополнительные параметры геометрии рельсовой колеи (ГРК)\Ведомость оценки состояния пути по ПЧ (ПУ-32 ).html");

			}
			catch
			{
				MessageBox.Show("Ошибка сохранения файлы");
			}
			finally
			{
				System.Diagnostics.Process.Start(Path.GetTempPath() + "/report-pu32.html");
				progressBar.Value = 0;
			}

			void SectionTotalGenerate(ref XElement sectionElement, ref Total sectionTotal, ref XElement distanceElement, ref Total distanceTotal,
				string code, string directionCode, string directionName, ref Total compSectionTotal, ref Total compDistanceTotal)
			{
				var avgBall1 = (sectionTotal.MainParamPointSum + sectionTotal.CurvePointSum) / sectionTotal.Length;
				var avgBall2 = (sectionTotal.MainParamPointSum + sectionTotal.CurvePointSum + sectionTotal.AddParamPointSum) / sectionTotal.Length;

				sectionElement.Add(
					new XAttribute("len", sectionTotal.Length % 1 == 0 ? sectionTotal.Length.ToString("0", nfi) : sectionTotal.Length.ToString("0.000", nfi)),
					new XAttribute("excellent", sectionTotal.RatingCounts[0].ToString("0.000", nfi)),
					new XAttribute("good", sectionTotal.RatingCounts[1].ToString("0.000", nfi)),
					new XAttribute("satisfactory", sectionTotal.RatingCounts[2].ToString("0.000", nfi)),
					new XAttribute("bad", sectionTotal.RatingCounts[3].ToString("0.000", nfi)),
					new XAttribute("limit", sectionTotal.IsLimited),
					//
					new XAttribute("d4", sectionTotal.Fourth ),

					new XAttribute("c1", sectionTotal.Constriction),
					new XAttribute("c2", sectionTotal.Broadening),
					new XAttribute("c3", sectionTotal.Level),
					new XAttribute("c4", sectionTotal.Sag),
					new XAttribute("c5", sectionTotal.Drawdown),
					new XAttribute("c6", sectionTotal.Strightening),
					new XAttribute("c7", sectionTotal.Common),
					new XAttribute("c8", sectionTotal.FourthOtherAdd),
					new XAttribute("point", $"{sectionTotal.MainParamPointSum + sectionTotal.CurvePointSum}/{sectionTotal.AddParamPointSum}"),
			

					new XAttribute("SOCHET", "0"),//to doo 
					new XAttribute("Grk", sectionTotal.Grk),
					new XAttribute("KRIV", sectionTotal.Kriv),
					new XAttribute("PRU", sectionTotal.Pru),
					new XAttribute("OSHK", sectionTotal.Oshk),
					new XAttribute("IZNOS", sectionTotal.Iznos),
					new XAttribute("ZAZOR", sectionTotal.Zazor), //
					new XAttribute("NEROVNOSTY", sectionTotal.NerProf),
					//
					new XAttribute("other", (sectionTotal.Combination + sectionTotal.Curves + sectionTotal.Other)),
					new XAttribute("add", sectionTotal.Additional),
					new XAttribute("repair", sectionTotal.Repairing),
					new XAttribute("mainavg", avgBall1.ToString("0.0", nfi)),
					new XAttribute("addavg", avgBall2.ToString("0.0", nfi)),
					new XAttribute("ns", sectionTotal.GetSectorQualitiveRating().Split('/')[0]),
					new XAttribute("rating", sectionTotal.GetSectorQualitiveRating().Split('/')[1]),
					new XAttribute("ratecount", $"Отл - {sectionTotal.RatingCounts[0].ToString("0", nfi)}; Хор - {sectionTotal.RatingCounts[1].ToString("0", nfi)}; Уд - {sectionTotal.RatingCounts[2].ToString("0", nfi)}; Неуд - {sectionTotal.RatingCounts[3].ToString("0", nfi)}; Средний балл - {avgBall1:0}/{avgBall2:0}")
);

				distanceTotal += sectionTotal;



				if (compSectionTotal != null)
				{
					var cavgBall1 = (compSectionTotal.MainParamPointSum + compSectionTotal.CurvePointSum) / compSectionTotal.Length;
					var cavgBall2 = (compSectionTotal.MainParamPointSum + compSectionTotal.CurvePointSum + compSectionTotal.AddParamPointSum) / compSectionTotal.Length;

					sectionElement.Add(
					  new XAttribute("clen", compSectionTotal.Length % 1 == 0 ? compSectionTotal.Length.ToString("0", nfi) : compSectionTotal.Length.ToString("0.000", nfi)),
					  new XAttribute("cexcellent", compSectionTotal.RatingCounts[0].ToString("0.000", nfi)),
					  new XAttribute("cgood", compSectionTotal.RatingCounts[1].ToString("0.000", nfi)),

								new XAttribute("csatisfactory", compSectionTotal.RatingCounts[2].ToString("0.000", nfi)),
								new XAttribute("cbad", compSectionTotal.RatingCounts[3].ToString("0.000", nfi)),
								new XAttribute("climit", compSectionTotal.IsLimited),
								new XAttribute("cd4", compSectionTotal.Fourth),
								new XAttribute("cother", (compSectionTotal.Combination + compSectionTotal.Curves + compSectionTotal.Other)),
								new XAttribute("cadd", compSectionTotal.Additional),
								new XAttribute("crepair", compSectionTotal.Repairing),
								new XAttribute("cmainavg", cavgBall1.ToString("0.0", nfi)),
								new XAttribute("caddavg", cavgBall2.ToString("0.0", nfi)),
								new XAttribute("cns", compSectionTotal.GetSectorQualitiveRating().Split('/')[0]),
					  new XAttribute("crating", compSectionTotal.GetSectorQualitiveRating().Split('/')[1]));

					compDistanceTotal += compSectionTotal;
					compSectionTotal = new Total();
					compSectionTotal.Code = code;
					compSectionTotal.DirectionCode = directionCode;
					compSectionTotal.DirectionName = directionCode;
				}
				sectionTotal = new Total();
				sectionTotal.Code = code;
				sectionTotal.DirectionCode = directionCode;
				sectionTotal.DirectionName = directionCode;
				distanceElement.Add(sectionElement);
				sectionElement = new XElement("section",
									new XAttribute("name", directionName),
									new XAttribute("code", directionCode),
									new XAttribute("track", code));
			}
			string GetPercentage(int value, int total)
			{
				return (value * 100 / (double)total).ToString("0.00", nfi) == "0.00" ? "" : (value * 100 / (double)total).ToString("0.00", nfi);
			}

			void PCHUTotalGenerate(ref XElement pchuElement, ref Total pchuTotal,  ref XElement distanceElement, ref Total distanceTotal,string code, string chief, ref Total compPchuTotal, ref Total compSectionTotal,string distanceCode)
			{
				var avgBall1 = (pchuTotal.MainParamPointSum + pchuTotal.CurvePointSum) / pchuTotal.Length;
				var avgBall2 = (pchuTotal.MainParamPointSum + pchuTotal.CurvePointSum + pchuTotal.AddParamPointSum) / pchuTotal.Length;

				pchuElement.Add(
					new XAttribute("len", pchuTotal.Length % 1 == 0 ? pchuTotal.Length.ToString("0", nfi) : pchuTotal.Length.ToString("0.000", nfi)),
					new XAttribute("point", $"{pchuTotal.MainParamPointSum + pchuTotal.CurvePointSum}/{pchuTotal.AddParamPointSum}"),
					new XAttribute("rating", pchuTotal.GetSectorQualitiveRating()),
					new XAttribute("ratecount", $"Отл - {pchuTotal.RatingCounts[0].ToString("0", nfi)}; Хор - {pchuTotal.RatingCounts[1].ToString("0", nfi)}; Уд - {pchuTotal.RatingCounts[2].ToString("0", nfi)}; Неуд - {pchuTotal.RatingCounts[3].ToString("0", nfi)}; Средний балл - {avgBall1:0}/{avgBall2:0}"),
					new XAttribute("c1", pchuTotal.Constriction.ToString()),
					new XAttribute("c2", pchuTotal.Broadening.ToString()),
					new XAttribute("c3", pchuTotal.Level.ToString()),
					new XAttribute("c4", pchuTotal.Sag.ToString()),
					new XAttribute("c5", pchuTotal.Drawdown.ToString()),
					new XAttribute("c6", pchuTotal.Strightening.ToString()),
					new XAttribute("c7", pchuTotal.Common),
					new XAttribute("c8", pchuTotal.FourthOtherAdd),

					new XAttribute("sperdball", $" {avgBall1:0}"),
					new XAttribute("ttype", reportType == PU32Type.Comparative ? (tripType == TripType.Control ? "контрольная" : (tripType == TripType.Additional ? "дополнительная" : "рабочая")) : "")

                    );

                distanceTotal += pchuTotal;
			
			   pchuTotal = new Total();
				pchuTotal.Code = code;
				
				if (compPchuTotal != null)
				{
					var cavgBall1 = (compPchuTotal.MainParamPointSum + compPchuTotal.CurvePointSum) / compPchuTotal.Length;
					var cavgBall2 = (compPchuTotal.MainParamPointSum + compPchuTotal.CurvePointSum + compPchuTotal.AddParamPointSum) / compPchuTotal.Length;

					pchuElement.Add(
						new XAttribute("clen", compPchuTotal.Length % 1 == 0 ? compPchuTotal.Length.ToString("0", nfi) : compPchuTotal.Length.ToString("0.000", nfi)),
						new XAttribute("cpoint", $"{compPchuTotal.MainParamPointSum + compPchuTotal.CurvePointSum}/{compPchuTotal.AddParamPointSum}"),
						new XAttribute("crating", compPchuTotal.GetSectorQualitiveRating()),
						new XAttribute("cratecount", $"Отл - {compPchuTotal.RatingCounts[0].ToString("0", nfi)}; Хор - {compPchuTotal.RatingCounts[1].ToString("0", nfi)}; Уд - {compPchuTotal.RatingCounts[2].ToString("0", nfi)}; Неуд - {compPchuTotal.RatingCounts[3].ToString("0", nfi)}; Средний балл - {cavgBall1:0}/{cavgBall2:0}"),
						new XAttribute("cc1", compPchuTotal.Constriction.ToString()),
						new XAttribute("cc2", compPchuTotal.Broadening.ToString()),
						new XAttribute("cc3", compPchuTotal.Level.ToString()),
						new XAttribute("cc4", compPchuTotal.Sag.ToString()),
						new XAttribute("cc5", compPchuTotal.Drawdown.ToString()),
						new XAttribute("cc6", compPchuTotal.Strightening.ToString()),
						new XAttribute("cc7", compPchuTotal.Common),
						new XAttribute("cc8", compPchuTotal.FourthOtherAdd),
						new XAttribute("cttype", reportType == PU32Type.Comparative ? (comparativeTripType == TripType.Control ? "контрольная" : (comparativeTripType == TripType.Additional ? "дополнительная" : "рабочая")) : ""),

						new XAttribute("csperdball", $" {cavgBall1:0}")
						);

					compSectionTotal += compPchuTotal;
					compPchuTotal = new Total();
					compPchuTotal.Code = code;
				}

				distanceElement.Add(pchuElement);
				pchuElement = new XElement("pchu",
							   //new XAttribute("pch", distance.Code  (24)),
							   new XAttribute("pch", distanceCode),
							   new XAttribute("code", code),
							   new XAttribute("chief", chief));
			}

			void PDTotalGenerate(ref XElement pdElement, ref Total pdTotal, ref XElement pchuElement, ref Total pchuTotal, string code, string chief, ref Total compPdTotal, ref Total compPchuTotal)
			{
				var avgBall1 = (pdTotal.MainParamPointSum + pdTotal.CurvePointSum) / pdTotal.Length;
				var avgBall2 = (pdTotal.MainParamPointSum + pdTotal.CurvePointSum + pdTotal.AddParamPointSum) / pdTotal.Length;

				pdElement.Add(
					new XAttribute("len", pdTotal.Length % 1 == 0 ? pdTotal.Length.ToString("0", nfi) : pdTotal.Length.ToString("0.000", nfi)),
					new XAttribute("point", $"{pdTotal.MainParamPointSum + pdTotal.CurvePointSum}/{pdTotal.AddParamPointSum}"),
					new XAttribute("rating", pdTotal.GetSectorQualitiveRating()),
					new XAttribute("ratecount", $"Отл - {pdTotal.RatingCounts[0].ToString("0", nfi)}; Хор - {pdTotal.RatingCounts[1].ToString("0", nfi)}; Уд - {pdTotal.RatingCounts[2].ToString("0", nfi)}; Неуд - {pdTotal.RatingCounts[3].ToString("0", nfi)}; Средний балл - {avgBall1:0}/{avgBall2:0}"),

					new XAttribute("avgLine", $" {avgBall2:0}/{pdTotal.GetSectorQualitiveRating().Split('/')[1]}"),

					new XAttribute("AvgSredBall", $" {avgBall2:0} "),
					new XAttribute("AvgOCENKA", $" { pdTotal.GetSectorQualitiveRating().Split('/')[1] }"),
					new XAttribute("NUCH", $" { pdTotal.GetSectorQualitiveRating().Split('/')[1] }"),

					new XAttribute("c1", pdTotal.Constriction.ToString()),
					new XAttribute("c2", pdTotal.Broadening.ToString()),
					new XAttribute("c3", pdTotal.Level.ToString()),
					new XAttribute("c4", pdTotal.Sag.ToString()),
					new XAttribute("c5", pdTotal.Drawdown.ToString()),
					new XAttribute("c6", pdTotal.Strightening.ToString()),
					new XAttribute("c7", pdTotal.Common),
					new XAttribute("c8", pdTotal.FourthOtherAdd),
					new XAttribute("ttype", reportType == PU32Type.Comparative ? (tripType == TripType.Control ? "контрольная" : (tripType == TripType.Additional ? "дополнительная" : "рабочая")) : ""));



				if (compPchuTotal != null)
				{
					var cavgBall1 = (compPdTotal.MainParamPointSum + compPdTotal.CurvePointSum) / compPdTotal.Length;
					var cavgBall2 = (compPdTotal.MainParamPointSum + compPdTotal.CurvePointSum + compPdTotal.AddParamPointSum) / compPdTotal.Length;

					pdElement.Add(
						new XAttribute("clen", compPdTotal.Length % 1 == 0 ? compPdTotal.Length.ToString("0", nfi) : compPdTotal.Length.ToString("0.000", nfi)),
						new XAttribute("cpoint", $"{compPdTotal.MainParamPointSum + compPdTotal.CurvePointSum}/{compPdTotal.AddParamPointSum}"),
						new XAttribute("crating", compPdTotal.GetSectorQualitiveRating()),
						new XAttribute("cratecount", $"Отл - {compPdTotal.RatingCounts[0].ToString("0", nfi)}; Хор - {compPdTotal.RatingCounts[1].ToString("0", nfi)}; Уд - {compPdTotal.RatingCounts[2].ToString("0", nfi)}; Неуд - {compPdTotal.RatingCounts[3].ToString("0", nfi)}; Средний балл - {cavgBall1:0}/{cavgBall2:0}"),

						new XAttribute("cavgLine", $" {cavgBall2:0}/{compPdTotal.GetSectorQualitiveRating().Split('/')[1]}"),

						new XAttribute("cAvgSredBall", $" {cavgBall2:0} "),
						new XAttribute("cAvgOCENKA", $" { compPdTotal.GetSectorQualitiveRating().Split('/')[1] }"),
						new XAttribute("cNUCH", $" { compPdTotal.GetSectorQualitiveRating().Split('/')[1] }"),

						new XAttribute("cc1", compPdTotal.Constriction.ToString()),
						new XAttribute("cc2", compPdTotal.Broadening.ToString()),
						new XAttribute("cc3", compPdTotal.Level.ToString()),
						new XAttribute("cc4", compPdTotal.Sag.ToString()),
						new XAttribute("cc5", compPdTotal.Drawdown.ToString()),
						new XAttribute("cc6", compPdTotal.Strightening.ToString()),
						new XAttribute("cc7", compPdTotal.Common),
						new XAttribute("cc8", compPdTotal.FourthOtherAdd),
						new XAttribute("cttype", reportType == PU32Type.Comparative ? (comparativeTripType == TripType.Control ? "контрольная" : (comparativeTripType == TripType.Additional ? "дополнительная" : "рабочая")) : ""));


					compPchuTotal += compPdTotal;
					compPdTotal = new Total();
					compPdTotal.Code = code;
				}
				pchuTotal += pdTotal;
				pdTotal = new Total();
				pdTotal.Code = code;
				pchuElement.Add(pdElement);

				pdElement = new XElement("pd",
								new XAttribute("code", code),
								new XAttribute("chief", chief));
			}



	void PDBnTotalGenerate(ref XElement pdbElement, ref Total pdbTotal, ref XElement pdElement, ref Total pdTotal, string code, string chief, ref Total compPdbTotal, ref Total compPdTotal)
			{
				var avgBall1 = (pdbTotal.MainParamPointSum + pdbTotal.CurvePointSum) / pdbTotal.Length;
				var avgBall2 = (pdbTotal.MainParamPointSum + pdbTotal.CurvePointSum + pdbTotal.AddParamPointSum) / pdbTotal.Length;

				pdbElement.Add(
					new XAttribute("c1", pdbTotal.Constriction.ToString()),
					new XAttribute("c2", pdbTotal.Broadening.ToString()),
					new XAttribute("c3", pdbTotal.Level.ToString()),
					new XAttribute("c4", pdbTotal.Sag.ToString()),
					new XAttribute("c5", pdbTotal.Drawdown.ToString()),
					new XAttribute("c6", pdbTotal.Strightening.ToString()),
					new XAttribute("c7", pdbTotal.Common),
					new XAttribute("c8", pdbTotal.FourthOtherAdd),

					new XAttribute("avgLine", $" {avgBall2:0}/{pdbTotal.GetSectorQualitiveRating().Split('/')[1]}"),

					new XAttribute("len", pdbTotal.Length % 1 == 0 ? pdbTotal.Length.ToString("0", nfi) : pdbTotal.Length.ToString("0.000", nfi)),
					new XAttribute("point", $"{pdbTotal.MainParamPointSum + pdbTotal.CurvePointSum}/{pdbTotal.AddParamPointSum}"),
					new XAttribute("rating", pdbTotal.GetSectorQualitiveRating()),
					new XAttribute("ratecount",

					$"Отл - {pdbTotal.RatingCounts[0].ToString("0", nfi)}; Хор - {pdbTotal.RatingCounts[1].ToString("0", nfi)}; Уд - {pdbTotal.RatingCounts[2].ToString("0", nfi)}; Неуд - {pdbTotal.RatingCounts[3].ToString("0", nfi)}; Средний балл - {avgBall1:0}/{avgBall2:0}"),
					new XAttribute("ttype", reportType == PU32Type.Comparative ? (tripType == TripType.Control ? "контрольная" : (tripType == TripType.Additional ? "дополнительная" : "рабочая")) : ""));

				pdTotal += pdbTotal;
				pdbTotal = new Total();
				pdbTotal.Code = code;

				pdElement.Add(pdbElement);

				pdbElement = new XElement("pdbn", new XAttribute("code", code), new XAttribute("chief", chief));
			}
		

	void PDBTotalGenerate(ref XElement pdbElement, ref Total pdbTotal, ref XElement pdElement, ref Total pdTotal, string code, string chief, ref Total compPdbTotal, ref Total compPdTotal)
			{
				var avgBall1 = (pdbTotal.MainParamPointSum + pdbTotal.CurvePointSum) / pdbTotal.Length;
				var avgBall2 = (pdbTotal.MainParamPointSum + pdbTotal.CurvePointSum + pdbTotal.AddParamPointSum) / pdbTotal.Length;

				pdbElement.Add(
					new XAttribute("c1", pdbTotal.Constriction.ToString()),
					new XAttribute("c2", pdbTotal.Broadening.ToString()),
					new XAttribute("c3", pdbTotal.Level.ToString()),
					new XAttribute("c4", pdbTotal.Sag.ToString()),
					new XAttribute("c5", pdbTotal.Drawdown.ToString()),
					new XAttribute("c6", pdbTotal.Strightening.ToString()),
					new XAttribute("c7", pdbTotal.Common),
					new XAttribute("c8", pdbTotal.FourthOtherAdd),

					new XAttribute("avgLine", $" {avgBall2:0}/{pdbTotal.GetSectorQualitiveRating().Split('/')[1]}"),

					new XAttribute("len", pdbTotal.Length % 1 == 0 ? pdbTotal.Length.ToString("0", nfi) : pdbTotal.Length.ToString("0.000", nfi)),
					new XAttribute("point", $"{pdbTotal.MainParamPointSum + pdbTotal.CurvePointSum}/{pdbTotal.AddParamPointSum}"),
					new XAttribute("rating", pdbTotal.GetSectorQualitiveRating()),
					new XAttribute("ratecount",

					$"Отл - {pdbTotal.RatingCounts[0].ToString("0", nfi)}; Хор - {pdbTotal.RatingCounts[1].ToString("0", nfi)}; Уд - {pdbTotal.RatingCounts[2].ToString("0", nfi)}; Неуд - {pdbTotal.RatingCounts[3].ToString("0", nfi)}; Средний балл - {avgBall1:0}/{avgBall2:0}"),
					new XAttribute("ttype", reportType == PU32Type.Comparative ? (tripType == TripType.Control ? "контрольная" : (tripType == TripType.Additional ? "дополнительная" : "рабочая")) : ""));


				if (compPdbTotal != null)
				{
					var cavgBall1 = (compPdbTotal.MainParamPointSum + compPdbTotal.CurvePointSum) / compPdbTotal.Length;
					var cavgBall2 = (compPdbTotal.MainParamPointSum + compPdbTotal.CurvePointSum + compPdbTotal.AddParamPointSum) / compPdbTotal.Length;
					pdbElement.Add(
					new XAttribute("cc1", compPdbTotal.Constriction.ToString()),
					new XAttribute("cc2", compPdbTotal.Broadening.ToString()),
					new XAttribute("cc3", compPdbTotal.Level.ToString()),
					new XAttribute("cc4", compPdbTotal.Sag.ToString()),
					new XAttribute("cc5", compPdbTotal.Drawdown.ToString()),
					new XAttribute("cc6", compPdbTotal.Strightening.ToString()),
					new XAttribute("cc7", compPdbTotal.Common),
					new XAttribute("cc8", compPdbTotal.FourthOtherAdd),

					new XAttribute("cavgLine", $" {cavgBall2:0}/{compPdbTotal.GetSectorQualitiveRating().Split('/')[1]}"),

					new XAttribute("clen", compPdbTotal.Length % 1 == 0 ? compPdbTotal.Length.ToString("0", nfi) : compPdbTotal.Length.ToString("0.000", nfi)),
					new XAttribute("cpoint", $"{compPdbTotal.MainParamPointSum + compPdbTotal.CurvePointSum}/{compPdbTotal.AddParamPointSum}"),
					new XAttribute("crating", compPdbTotal.GetSectorQualitiveRating()),
					new XAttribute("cratecount",
					$"Отл - {compPdbTotal.RatingCounts[0].ToString("0", nfi)}; Хор - {compPdbTotal.RatingCounts[1].ToString("0", nfi)}; Уд - {compPdbTotal.RatingCounts[2].ToString("0", nfi)}; Неуд - {compPdbTotal.RatingCounts[3].ToString("0", nfi)}; Средний балл - {cavgBall1:0}/{cavgBall2:0}"),
					new XAttribute("cttype", reportType == PU32Type.Comparative ? (comparativeTripType == TripType.Control ? "контрольная" : (comparativeTripType == TripType.Additional ? "дополнительная" : "рабочая")) : "")
					);
					compPdTotal += compPdbTotal;
					compPdbTotal = new Total();
					compPdbTotal.Code = code;
				}
				pdTotal += pdbTotal;
				pdbTotal = new Total();
				pdbTotal.Code = code;

				pdElement.Add(pdbElement);

				pdbElement = new XElement("pdb", new XAttribute("code", code), new XAttribute("chief", chief));
			}
		}
	}
	public class Total
	{
		public string distancecode { get; set; }
		public string Code { get; set; }
		public int Count { get; set; }
		public string DirectionCode { get; set; }
		public bool IsKM { get; set; } = false;
		public TotalPart Constriction { get; set; } = new TotalPart();
		public TotalPart Broadening { get; set; } = new TotalPart();
		public TotalPart Level { get; set; } = new TotalPart();
		public TotalPart Sag { get; set; } = new TotalPart();
		public TotalPart Drawdown { get; set; } = new TotalPart();
		public TotalPart Strightening { get; set; } = new TotalPart();
		public int IsLimited { get; set; } = 0;
		public int Repairing { get; set; } = 0;

		public double[] RatingCounts { get; set; } = new double[] { 0, 0, 0, 0 };
		public int Fourth { get; set; }
		public int Other { get; set; }
		public int Combination { get; set; }
		public int Curves { get; set; }
		public int MainParamPointSum { get; set; }
		public int AddParamPointSum { get; set; }
		public int CurvePointSum { get; set; }







		//GRK KRIV...
		public int Grk { get; set; }
		public int Kriv { get; set; }
		public int Pru { get; set; }
		public int Oshk { get; set; }
		public int Iznos { get; set; }
		public int Sochet { get; set; }
		public int Zazor { get; set; }
		public int Pu32_gap_count { get; set; }
		public int NerProf { get; set; }
		//TO DOO SOC

		//

		//ToDo Tolegen
		public int Additional { get; set; }

		public TotalPart Common => Constriction + Broadening + Level + Sag + Drawdown + Strightening;
		public static Total operator +(Total t1, Total t2)
		{
			if (t2.IsKM)
			{
				if (t2.QualitiveRating == Rating.О)
					t1.RatingCounts[0] += t2.Length;
				if (t2.QualitiveRating == Rating.Х)
					t1.RatingCounts[1] += t2.Length;
				if (t2.QualitiveRating == Rating.У)
					t1.RatingCounts[2] += t2.Length;
				if (t2.QualitiveRating == Rating.Н)
					t1.RatingCounts[3] += t2.Length;

			}
			else
			{
				for (int i = 0; i < t1.RatingCounts.Length; i++)
				{
					t1.RatingCounts[i] += t2.RatingCounts[i];

				}

			}

			return new Total
			{
				Grk = t1.Grk + t2.Grk,
				Sochet = t1.Sochet + t2.Sochet,
				Pru = t1.Pru + t2.Pru,
				Kriv = t1.Kriv + t2.Kriv,
				Oshk = t1.Oshk + t2.Oshk,
				Iznos = t1.Iznos + t2.Iznos,
				NerProf = t1.NerProf + t2.NerProf,
				
				Zazor = t1.Zazor + t2.Zazor,

				DirectionCode = t1.DirectionCode,
				Code = t1.Code,
				IsLimited = t1.IsLimited + t2.IsLimited,
				Constriction = t1.Constriction + t2.Constriction,
				Strightening = t1.Strightening + t2.Strightening,
				Broadening = t1.Broadening + t2.Broadening,
				Level = t1.Level + t2.Level,
				Sag = t1.Sag + t2.Sag,
				Drawdown = t1.Drawdown + t2.Drawdown,
				Fourth = t1.Fourth + t2.Fourth,
				Other = t1.Other + t2.Other,
				Additional = t1.Additional + t2.Additional,
				Curves = t1.Curves + t2.Curves,
				Length = t1.Length + t2.Length,
				MainParamPointSum = t1.MainParamPointSum + t2.MainParamPointSum,
				AddParamPointSum = t1.AddParamPointSum + t2.AddParamPointSum,
				CurvePointSum = t1.CurvePointSum + t2.CurvePointSum,
				RatingCounts = t1.RatingCounts ,
				Count = t1.Count + 1
			};
		}

		public string FourthOtherAdd => $"{(Fourth > 0 ? Fourth.ToString() : "-")}/{(Other + Curves > 0 ? (Other + Curves).ToString() : " - ")}/{(Additional > 0 ? Additional.ToString() : " - ")}";
		//public string Fourth => $"{(Fourth > 0 ? Fourth.ToString() : "-")}";
		//public string Other => $"{(Other > 0 ? Other.ToString() : " - ")}";
		//public string Additional => $"{(Additional > 0 ? Additional.ToString() : " - ")}";


		public Rating QualitiveRating { get; set; }
		public double Length { get; set; }
		public string DirectionName { get; internal set; }

		public string GetSectorQualitiveRating()
		{
			NumberFormatInfo nfi = new CultureInfo("en-US", false).NumberFormat;

			var NSector = (5 * RatingCounts[0] + 4 * RatingCounts[1] + 3 * RatingCounts[2] - 5 * RatingCounts[3]) / Length;

			QualitiveRating = NSector > 4.5 ? Rating.О : (NSector.Between(3.9, 4.5) ? Rating.Х : (NSector.Between(3.0, 3.8) ? Rating.У : Rating.Н));

			//Қосымша тексеру керек
			NSector = NSector < 0 ? 0 : NSector;
			
			return $"{NSector.ToString("0.0", nfi)}/{QualitiveRating}";
	

		}
	}

	public class TotalPart
	{
		public int[] Degrees { get; set; } = new int[] { 0, 0, 0, 0 };
		public static TotalPart operator +(TotalPart t1, TotalPart t2)
		{
			return new TotalPart
			{
				Degrees = new int[]
				{
					t1.Degrees[0] + t2.Degrees[0],
					t1.Degrees[1] + t2.Degrees[1],
					t1.Degrees[2] + t2.Degrees[2],
					t1.Degrees[3] + t2.Degrees[3],
				}
			};
		}
		public override string ToString()
		{
			if (Degrees[1] == 0 && Degrees[2] == 0)
				return "";
			if (Degrees[1] == 0 && Degrees[2] != 0)
				return $"/{Degrees[2]}";
			if (Degrees[1] != 0 && Degrees[2] == 0)
				return $"{Degrees[1]}/";
			return $"{Degrees[1]}/{Degrees[2]}";
		}
	}
}
