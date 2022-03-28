using Accord.Math;
using ALARm.Core;
using ALARm.Core.AdditionalParameteres;
using ALARm.Core.Report;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlarmPP.Web.Services
{
    public class AppData
    {
        public IMainTrackStructureRepository MainTrackStructureRepository { get; set; }
        public IRdStructureRepository RdStructureRepository { get; set; }
        public IAdditionalParametersRepository AdditionalParametersRepository { get; set; }
        private List<Kilometer> _kilometers;
        public OnlineModeData onlineModeData { get; set; } = new OnlineModeData();
        public List<Kilometer> Kilometers
        {
            get
            {
                return _kilometers;
            }
            set
            {
                _kilometers = value;
                NotifyDataChanged();
            }
        }

        /// <summary>
        /// процедура для рисования ПУ и НПК
        /// </summary>
        
        public List<Kilometer> BedKilometers { get; set; }
        private float angleRuleWidth = 70f;
        public float GetDIstanceFrom1div60(float x)
        {
            var koef = (angleRuleWidth / (1 / 12f - 1 / 60f));
            var value = 1 / x - 1 / 60f;

            return koef * value;
        }

        public bool Loading { get; set; } = false;
        public string LoadingText { get; set; } = "Загрузка...";
        public int CurrentKmMeter { get; set; }
        public int CurrentKmMeterMeter { get; set; }
        public int CurrentKm { get; set; }
        public Kilometer CurrentKilometer { get; set; }
        public bool? MainLoading { get; set; } = true;
        public int EmailCount { get; set; } = 0;
        private double PrevImageIndex { get; set; } = -1;
        public RailImage RailImage { get; set; }
        public Image RightImage { get; set; }
        public double SliderXPosition { get; set; } = 0;
        public double SliderCenterXPosition { get; set; } = 25;
        public double SliderYPosition { get; set; } = 0;
        public Trips Trip { get; set; } = new Trips();
        public bool IsDialogOpen { get; set; } = true;
        private string _color;
        public int TitleRowHeigth = 48;
        public int PasportPosition { get; set; } = 0;
        public int PasportWidth = 60;

        public int CurrDigKm { get; set; }
        public int CurrDigM { get; set; }
        public List<double> Koefs { get; set; } = new List<double> { 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 };

        /// <summary>
        /// основные данные для показа текущих значений графиков
        /// Индексы:
        /// 0 - текущая координата в формате "км.м"
        /// 1 - текущая значение нулевой линии уровня
        /// </summary>
        public string[] Data { get; set; }
        /// <summary>
        /// Показать стыки
        /// </summary>
        /// 
        public bool ShowCorrection { get; set; }
        /// <summary>
        /// Прибивки
        /// </summary>

        public bool ShowJoints { get; set; }
        /// <summary>
        /// показать поперечный профиль рельса
        /// </summary>
        public bool ShowRailProfile { get; set; }
        /// <summary>
        /// начальная позиция столбца стыков
        /// </summary>
        public int WearWidth => 110;
        /// <summary>
        /// начальная позиция столбцов "поперечный профиль рельса"
        /// </summary>
        public int RailProfilePosition {
            get { return ShowJoints ? JointRightPosition + JointWidth : JointPosition; }
        }
        /// <summary>
        /// начальная позиция столбца "Износ бок. Пр"
        /// </summary>
        public int SideWearRightPosition
        {
            get { return RailProfilePosition + WearWidth; }
        }
        /// <summary>
        /// начальная позиция столбца "Износ верт. Л."
        /// </summary>
        public int VertWearLeftPosition
        {
            get { return SideWearRightPosition + WearWidth; }
        }

        /// <summary>
        /// начальная позиция столбца "Износ верт. Пр."
        /// </summary>
        public int VertWearRightPosition
        {
            get { return VertWearLeftPosition + WearWidth; }
        }
        /// <summary>
        /// начальная позиция столбца "Износ прив. Л."
        /// </summary>
        public int GivenWearLeftPosition
        {
            get { return VertWearRightPosition + WearWidth; }
        }

        /// <summary>
        /// начальная позиция столбца "Износ прив. Пр."
        /// </summary>
        public int GivenWearRightPosition
        {
            get { return GivenWearLeftPosition + WearWidth; }
        }

        /// <summary>
        /// начальная позиция столбца "Наклон ПК Л."
        /// </summary>
        public int TreadTiltLeftPosition
        {
            get { return GivenWearRightPosition + WearWidth; }
        }

        /// <summary>
        /// начальная позиция столбца "Наклон ПК Пр."
        /// </summary>
        public int TreadTiltRightPosition
        {
            get { return TreadTiltLeftPosition + WearWidth; }
        }
        /// <summary>
        /// начальная позиция столбца "Подуклонка Л."
        /// </summary>
        public int DownhillLeftPosition
        {
            get { return TreadTiltRightPosition + WearWidth; }
        }

        /// <summary>
        /// начальная позиция столбца "Подуклонка Пр."
        /// </summary>
        public int DownhillRightPosition
        {
            get { return DownhillLeftPosition + WearWidth; }
        }

        /// <summary>
        /// начальная позиция столбца "Износ 45г."
        /// </summary>
        public int HeadWear45LeftPosition
        {
            get { return DownhillRightPosition + WearWidth; }
        }

        /// <summary>
        /// начальная позиция столбца "Износ 45г"
        /// </summary>
        public int HeadWear45RightPosition
        {
            get { return HeadWear45LeftPosition + WearWidth; }
        }

        /// <summary>
        /// начальная позиция столбцов "стыки"
        /// </summary>
        public int JointPosition
        {
            get
            {
                return (ShowMainParams ? MainParamsPosition + MainParamsWidth : MainParamsPosition);
            }
        }

        /// <summary>
        /// ширина забега
        /// </summary>
        public int HeatWidth = 140;
        /// <summary>
        /// позиция нулевой линни забега
        /// </summary>
        public int HeatZero { get { return HeatWidth / 2; } }
        /// <summary>
        /// начальная позиция стобца левого зазора
        /// </summary>
        public int JointLeftPosition { get { return JointPosition + HeatWidth; } }
        /// <summary>
        /// ширина столбца "зазоры"
        /// </summary>
        public int JointWidth = 140;
        /// <summary>
        /// начальная позиция "нулевой линии зазора" 
        /// </summary>
        public int JointZero { get { return JointWidth / 2; } }
        /// <summary>
        /// начальная позиция столбца правого зазора
        /// </summary>
        public int JointRightPosition { get { return JointLeftPosition + JointWidth; } }
        /// <summary>
        /// События
        /// Показать события
        /// </summary>
        public int SpeedPosition
        {
            get { return PasportWidth; }
        }
        public int SpeedWidth = 125;
        public bool ShowEvents { get; set; }
        /// <summary>
        /// начальная позиция столбца "события"
        /// </summary>
        public int EventPosition {
            get { return SpeedPosition + SpeedWidth; }
        }


        /// <summary>
        /// ширина столбца "события"
        /// </summary>
        public int EventWidth = 50;
        /// <summary>
        /// основные параметры
        /// показать сигналы
        /// </summary>
        public bool ShowSignals { get; set; }
        /// <summary>
        /// показать паспортные данные
        /// </summary>
        public bool ShowTestSignals { get; set; } = false;
        /// <summary>
        /// показать паспортные данные
        /// </summary>
        public bool ShowPasport { get; set; }
        /// <summary>
        /// показать нулевые линии
        /// </summary>
        public bool ShowZeroLines { get; set; }
        /// <summary>
        /// показать основные параметры
        /// </summary>
        public bool ShowMainParams { get; set; }
        /// <summary>
        /// начальная позиция основных параметров
        /// </summary>
        public int MainParamsPosition
        {
            get
            {
                return (ShowEvents ? EventPosition + EventWidth : EventPosition);
            }
        }
        /// <summary>
        /// ширина столбца "уровень"
        /// </summary>
        public int LevelWidth => 160;
        /// <summary>
        /// позиция для нулеовй точки уровня
        /// </summary>
        public int LevelZero => 120;
        /// <summary>
        /// ширина стоблца "сочетания"
        /// </summary>
        //public int СombinationWidth => 40;
        public int СombinationWidth => 80;
        public int CombinationPosition {
            get { return MainParamsPosition + LevelWidth; }
        }
        /// <summary>
        /// ширина столбца "рихтовка"
        /// </summary>
        public int StrightWidth => 140;
        public int StrightLeftZero => 100;
        public int StrightRightZero => 40;
   
        public int NoneStrightLeftPosition => 60;
        public int NoneStrightRightPosition => 120;
        public int NoneStrightWidth => 140;
        public int NoneСombinationWidth => 80;
        public int NoneCombinationPosition
        {
            get { return MainParamsPosition + NoneStrightWidth; }
        }
        public int NoneStrgihtRightPosition
        {
            get
            {
                return   NoneStrightLeftPosition + NoneStrightWidth;
            }
        }
        public int NoneStrgihtLeftPosition
        {
            get
            {
                return NoneCombinationPosition + NoneСombinationWidth;
            }
        }
        /// <summary>
        /// начальная позиция левой рихтовки
        /// </summary>
        public int StrightLeftPosition
        {
            get
            {
                return CombinationPosition + СombinationWidth;
            }
        }
        /// <summary>
        /// начальная позиция правой рихтовки
        /// </summary>
        public int StrgihtRightPosition {
            get {
                return StrightLeftPosition + StrightWidth;
            }
        }
        /// <summary>
        /// Ширина столбца "шаблон"
        /// </summary>
        public int GaugeWidth => 80;
        /// <summary>
        /// Начальная позиция столбца шаблон
        /// </summary>
        public int GaugePosition {
            get {
                return StrgihtRightPosition + StrightWidth;
            }
        }
        public int GaugeZero => 20;
        /// <summary>
        /// Ширина столбца "Просадка"
        /// </summary>
        public int DrawdownWidth = 60;
        public int DrawdownLeftPosition {
            get { return GaugePosition + GaugeWidth; }
        }
        public int DrawdownRightPosition {
            get { return DrawdownLeftPosition + DrawdownWidth; }
        }
        public int DrawdownZero = 30;
        /// <summary>
        /// ширина основных параметров
        /// </summary>
        public int MainParamsWidth
        {
            get { return LevelWidth + СombinationWidth + 2 * StrightWidth + GaugeWidth + 2 * DrawdownWidth; }
        }
        /// <summary>
        /// дополнение к основным параметрам
        /// показать дополнение к основным параметрам
        /// </summary>
        public bool ShowMainPlus { get; set; }
        /// <summary>
        /// показать отступления
        /// </summary>
        public bool ShowDigressions { get; set; } = false;
        public bool ShowDangerousDigressions { get; set; } = false;
        public bool ShowDangerousForEmtyWagon { get; set; } = false;
        public bool Show3DegreeDigressions { get; set; } = false;
        public bool ShowCloseToDangerous { get; set; } = false;
        
        public bool ShowCloseTo2Degree { get; set; } = false;

        public bool FirstDegreeDigression { get; set; } = false;
        public bool Show2DegreeDigressions { get; set; } = false;
        public bool Show1DegreeDigressions { get; set; } = false;
        public bool ShowOthersDigressions { get; set; } = false;
        public bool ShowExcludedOnSwitch { get; set; } = false;
        public bool ShowExcludedByOerator { get; set; } = false;
        public bool ShowNotTakenOnRating { get; set; } = false;
        public bool DigressionChecked { get; set; } = false;
        
        public bool AddDigressionChecked { get; set; } = false;
        public bool ByKilometerChecked { get; set; } = false;
        public bool ShowGaps { get; set; } = false;
        public bool ShowGapsCloseToDangerous {get; set; } = false;
        public bool ShowBolts { get; set; } = false;
        public bool ShowFasteners { get; set; } = false;
        public bool ShowPerShpals { get; set; } = false;
        public bool ShowDefShpals { get; set; } = false;

        public bool ShowPU { get; set; } = false;
        public bool ShowNPK { get; set; } = false;
        public bool ShowIznosBok { get; set; } = false;
        public bool ShowIznosVert { get; set; } = false;
        public bool ShowIznosPriv { get; set; } = false;
        public bool ShowLongWaves { get; set; } = false;
        public bool ShowMediumWaves { get; set; } = false;
        public bool ShowShortWaves { get; set; } = false;
        public int GetDistanceFrom1div(int div, float degKoef) {
            var res = 1f / div * degKoef;
            return Convert.ToInt32(res);
        }
        public int Width
        {
            get
            {
                return MainParamsPosition + LevelWidth;
            }
        }
        public string Color
        {
            get
            {
                return _color;
            }
            set
            {
                _color = value;
                NotifyDataChanged();
            }
        }
        public void ReloadKilometers()
        {
            if (Kilometers == null)
                Kilometers = new List<Kilometer>();
            BedKilometers = RdStructureRepository.GetBedemostKilometers();
            foreach (var fragment in Trip.Route)
            {
                var kms = MainTrackStructureRepository.GetKilometersOfFragment(fragment, DateTime.Today, fragment.Direction, Trip.Id);

                if (kms.Count > 0)
                {

                    if (fragment.Direction == Direction.Direct)
                    {
                        kms[0].Start_m = fragment.Start_M;
                        kms[kms.Count - 1].Final_m = fragment.Final_M;
                    }
                    else
                    {
                        kms[0].Final_m = fragment.Start_M;
                        kms[kms.Count - 1].Start_m = fragment.Final_M;
                    }
                }

                foreach (var km in kms)
                {
                    var coord = km.Final_m;
                    if (km.Number == 711)
                    {
                        km.Number = km.Number;
                    }
                    km.Direction = fragment.Direction;
                    bool found = false;
                    bool numberFound = false;
                    foreach (var app_km in Kilometers)
                    {
                        if ((app_km.Number.Equals(km.Number)) && (app_km.Track_id == km.Track_id))
                        {
                            numberFound = true;
                            if ((fragment.Direction == Direction.Reverse) && (app_km.Start_m == km.Final_m))
                            {
                                found = true;
                                app_km.Start_m = km.Start_m;
                                break;
                            }
                            if ((fragment.Direction == Direction.Direct) && (app_km.Final_m == km.Start_m))
                            {
                                found = true;
                                app_km.Final_m = km.Final_m;
                                break;
                            }
                        }
                    }
                    if (!found)
                    {
                        km.LoadTrackPasport(MainTrackStructureRepository, Trip.Trip_date);

                        if (km.Start_Index > -1)
                        {
                            var outData = RdStructureRepository.GetNextOutDatas(km.Start_Index - 1, km.GetLength(), Trip.Id);
                            int meter = 0;
                            foreach (var data in outData)
                            {
                                int currentMetre = fragment.Direction == Direction.Direct ? km.Start_m + meter : km.Final_m - meter;
                                km.AddData(data, currentMetre, Koefs);
                                meter++;
                            }
                            Meter = km.Final_Index;

                            var Curves = new List<NatureCurves>();
                            //km.StrightAvgTrapezoid = km.StrightAvg.GetTrapezoid(new List<double>(), new List<double>(), 4, ref Curves);
                            //km.LevelAvgTrapezoid = km.LevelAvg.GetTrapezoid(new List<double>(), new List<double>(), 4, ref Curves);
                            km.GetZeroLines(outData, Trip, MainTrackStructureRepository);

                            var outDataTrapezPassport = RdStructureRepository.GetNextOutDatas(km.Start_Index - 1000, km.GetLength() + 2000, Trip.Id);
                            var prevCount = 0;
                            var nextCount = km.GetLength() - 1;

                            var temp = outDataTrapezPassport.Where(o => o.RealCoordinate == outData.First().RealCoordinate).ToList();
                            if (temp.Any())
                            {
                                prevCount = outDataTrapezPassport.IndexOf(temp.First());
                            }

                            km.GetZeroLinesTrapez(outDataTrapezPassport, Trip, MainTrackStructureRepository, prevCount, nextCount);

                            //km.StrightAvgTrapezoid = km.StrightAvg.GetTrapezoid(new List<double>(), new List<double>(), 4, ref Curves);
                            //km.LevelAvgTrapezoid = km.LevelAvg.GetTrapezoid(new List<double>(), new List<double>(), 4, ref Curves);
                            //km.TrapezLevel = "";

                            //for (int i = 0; i < km.LevelAvgTrapezoid.Count; i++)
                            //{
                            //    km.TrapezLevel += $"{km.LevelAvgTrapezoid[i] * km.StrightKoef:0.00},{km.Meters[i]} ";
                            //}
                        }
                        if (km.IsPrinted)
                        {
                           
                            km.AdditionalDigressions = RdStructureRepository.GetAdditional(km.Number);
                            km.Digressions = RdStructureRepository.GetDigressionMarks(Trip.Id, km.Number, km.Track_id, new int[] {1,2, 3, 4 });
                            km.CorrectionNotes = RdStructureRepository.GetCorrectionNotes(Trip.Id, km.Track_id, km.Number, coord, km.CorrectionValue);
                            km.Gaps = AdditionalParametersRepository.Check_gap_state(Trip.Id, 999);
                            km.Bolts = AdditionalParametersRepository.Check_bolt_state(Trip.Id, 999);
                            km.Fasteners = AdditionalParametersRepository.Check_badfastening_state(Trip.Id, 999);
                            km.DefShpals = AdditionalParametersRepository.Check_defshpal_state(Trip.Id, 999);
                            km.PerShpals = AdditionalParametersRepository.Check_ViolPerpen(Trip.Id);
                            //km.direction_name = RdStructureService.Check_direction_name(Trip.Id);
                            
                            //if (!km.CorrectionNotes.Any())
                            //{
                            //    km.CorrectionNotes = RdStructureRepository.GetCorrectionNotes(Trip.Id, km.Track_id, km.Number, coord, km.CorrectionValue);
                            //}
                            //else
                            //{
                            //    continue;
                            //}

                        }

                        Kilometers.Add(km);
                    }
                    else if (numberFound && WorkMode == WorkMode.Postprocessing)
                    {
                        if (km.Number == 711)
                        {
                            km.Number = km.Number;
                        }

                        km.Start_Index = Kilometers.Last().Start_Index;
                        km.Start_m = Kilometers.Last().Start_m;
                        km.Final_m = Kilometers.Last().Final_m;
                        Kilometers.Remove(Kilometers.Last());
                        km.LoadTrackPasport(MainTrackStructureRepository, Trip.Trip_date);

                        if (km.Start_Index > -1)
                        {
                            var outData = RdStructureRepository.GetNextOutDatas(km.Start_Index - 1, km.GetLength(), Trip.Id);
                            int meter =0;
                            foreach (var data in outData)
                            {
                                int currentMetre = fragment.Direction == Direction.Direct ? km.Start_m + meter : km.Final_m - meter;
                                km.AddData(data, currentMetre, Koefs);
                                meter++;
                            }
                            Meter = km.Final_Index;

                            var Curves = new List<NatureCurves>();
                            //km.StrightAvgTrapezoid = km.StrightAvg.GetTrapezoid(new List<double>(), new List<double>(), 4, ref Curves);
                            //km.LevelAvgTrapezoid = km.LevelAvg.GetTrapezoid(new List<double>(), new List<double>(), 4, ref Curves);
                            km.GetZeroLines(outData, Trip, MainTrackStructureRepository);

                            var outDataTrapezPassport = RdStructureRepository.GetNextOutDatas(km.Start_Index - 1000, km.GetLength() + 2000, Trip.Id);
                            var prevCount = 0;
                            var nextCount = km.GetLength() - 1;

                            var temp = outDataTrapezPassport.Where(o => o.RealCoordinate == outData.First().RealCoordinate).ToList();
                            if (temp.Any())
                            {
                                prevCount = outDataTrapezPassport.IndexOf(temp.First());
                            }

                            km.GetZeroLinesTrapez(outDataTrapezPassport, Trip, MainTrackStructureRepository, prevCount, nextCount);

                            //km.StrightAvgTrapezoid = km.StrightAvg.GetTrapezoid(new List<double>(), new List<double>(), 4, ref Curves);
                            //km.LevelAvgTrapezoid = km.LevelAvg.GetTrapezoid(new List<double>(), new List<double>(), 4, ref Curves);
                            //km.TrapezLevel = "";

                            //for (int i = 0; i < km.LevelAvgTrapezoid.Count; i++)
                            //{
                            //    km.TrapezLevel += $"{km.LevelAvgTrapezoid[i] * km.StrightKoef:0.00},{km.Meters[i]} ";
                            //}
                            km.Digressions = RdStructureRepository.GetDigressionMarks(Trip.Id, km.Number, km.Track_id, new int[] {1,2, 3, 4 });
                            km.CorrectionNotes = RdStructureRepository.GetCorrectionNotes(Trip.Id, km.Track_id, km.Number, coord, km.CorrectionValue);
                            km.Gaps = AdditionalParametersRepository.Check_gap_state(Trip.Id, 999);
                            km.Bolts = AdditionalParametersRepository.Check_bolt_state(Trip.Id, 999);
                            km.Fasteners = AdditionalParametersRepository.Check_badfastening_state(Trip.Id, 999);
                            km.DefShpals = AdditionalParametersRepository.Check_defshpal_state(Trip.Id, 999);
                            km.PerShpals = AdditionalParametersRepository.Check_ViolPerpen(Trip.Id);
                            km.AdditionalDigressions = RdStructureRepository.GetAdditional(km.Number);
                            //km.direction_name = RdStructureService.GetTracksOnTrip(Trip.Id);
                            Kilometers.Add(km);

                        }

                    }
                    //AppData.Kilometers.AddRange(kms);
                }
            }
        }

        public bool GetData(double yPosition)
        {
            if (_kilometers == null)
                return false;
            Data = new string[] { "", "", "", "", "", "", "", "", "", "", "", "", "", "", "","", "", "", "", "", "", "", "", "", "", "", "", "", "","" };

        
            int position = (int)yPosition;
            int length = 0;
   
                foreach (var km in _kilometers)
                { 
              
                length += km.GetLength();
                if (yPosition < length)
                {
                    CurrentKm = km.Number;
                    CurrentKilometer = km;


                    CurrentKmMeter = (Trip.Travel_Direction == Direction.Direct ? ((int)yPosition - (length - km.GetLength()) + km.Start_m) : km.Final_m - ((int)yPosition - (length - km.GetLength())));

                    Data[(int)Series.Pasport] = km.Number.ToString() + "." + (Trip.Travel_Direction == Direction.Direct ? ((int)yPosition - (length - km.GetLength()) + km.Start_m) : km.Final_m - ((int)yPosition - (length - km.GetLength()))).ToString();
                    if (km.PdbSection.Count > 0)
                        Data[(int)Series.Section] += km.PdbSection[0].ToString();
                    int currentMetre = Trip.Travel_Direction == Direction.Direct ? ((int)yPosition - (length - km.GetLength()) + km.Start_m) : km.Final_m - ((int)yPosition - (length - km.GetLength()));

                    for (int index = 0; index < km.Speed.Count; index++)
                    {
                        int metre = Trip.Travel_Direction != Direction.Direct ? km.Length - index : index;
                        if (metre == currentMetre)
                        {
                            Data[(int)Series.LevelZero] = km.LevelAvg[index].ToString("0.00");
                            // Data[(int)Series.LevelPasport] = km.flvl0[index].ToString("0.00");
                            Data[(int)Series.LevelSignal] = km.Level[index].ToString("0.00");

                            Data[(int)Series.StrightLeftZero] = km.StrightAvg[index].ToString("0.00");
                            //Data[(int)Series.StrightLeftPasport] = km.fZeroStright[index].ToString("0.00");
                            Data[(int)Series.StrightLeftSignal] = km.StrightLeft[index].ToString("0.00");

                            Data[(int)Series.StrightRightZero] = km.StrightAvg[index].ToString("0.00");
                            //Data[(int)Series.StrightRightPasport] = km.fZeroStright[index].ToString("0.00");
                            Data[(int)Series.StrightRightSignal] = km.StrightLeft[index].ToString("0.00");

                            //Data[(int)Series.GaugePasport] = km.fsh0[index].ToString("0.00");
                            Data[(int)Series.GaugeSignal] = km.Gauge[index].ToString("0.00");

                            Data[(int)Series.DrawdownLeft] = km.DrawdownLeft[index].ToString("0.00");
                            Data[(int)Series.DrwadownRight] = km.DrawdownRight[index].ToString("0.00");
                            Data[(int)Series.Speed] = km.Speed[index].ToString();



                            break;
                        }
                    }
                    if (km.CrossRailProfile != null)
                    {
                        int indexCross = km.CrossRailProfile.Meters.IndexOf(Trip.Travel_Direction == Direction.Reverse ? (float)currentMetre :  Math.Abs(km.Length - (float)currentMetre));
                        if (indexCross < 0) indexCross = 0;
                        try
                        {
                            if (indexCross > -2)
                            {

                                Data[(int)Series.SideWearLeft] = km.CrossRailProfile.SideWearLeft[indexCross].ToString("0.00");
                                Data[(int)Series.SideWearRight] = km.CrossRailProfile.SideWearRight[indexCross].ToString("0.00");
                                Data[(int)Series.VertWearLeft] = km.CrossRailProfile.VertIznosL[indexCross].ToString("0.00");
                                Data[(int)Series.VertWearRight] = km.CrossRailProfile.VertIznosR[indexCross].ToString("0.00");
                                Data[(int)Series.GivenWearLeft] = km.CrossRailProfile.ReducedWearLeft[indexCross].ToString("0.00");
                                Data[(int)Series.GivenWearRight] = km.CrossRailProfile.ReducedWearRight[indexCross].ToString("0.00");
                                Data[(int)Series.TreadTiltLeft] = double.IsPositiveInfinity(1 / km.CrossRailProfile.TreadTiltLeft[indexCross]) ? "" : "1/" + (1 / km.CrossRailProfile.TreadTiltLeft[indexCross]).ToString("0");
                                Data[(int)Series.TreadTiltRight] = double.IsPositiveInfinity(1 / km.CrossRailProfile.TreadTiltRight[indexCross]) ? "" : "1/" + (1 / km.CrossRailProfile.TreadTiltRight[indexCross]).ToString("0");
                                Data[(int)Series.DownHillLeft] = double.IsPositiveInfinity(1 / km.CrossRailProfile.DownhillLeft[indexCross]) ? "" : "1/" + (1 / km.CrossRailProfile.DownhillLeft[indexCross]).ToString("0");
                                Data[(int)Series.DownHillRight] = double.IsPositiveInfinity(1 / km.CrossRailProfile.DownhillRight[indexCross]) ? "" : "1/" + (1 / km.CrossRailProfile.DownhillRight[indexCross]).ToString("0");
                                Data[(int)Series.HeadWear45Left] = km.CrossRailProfile.HeadWearLeft[indexCross].ToString("0.00");
                                Data[(int)Series.HeadWear45Right] = km.CrossRailProfile.HeadWearRight[indexCross].ToString("0.00");

                                ViewBoxLeft = "-100 -30 200 300"; // Уакытша
                                ViewBoxRight = "-100 -30 200 300"; // Уакытша
                                NominalTranslateLeft = "-10px,-10px"; // Уакытша
                                NominalTranslateRight = "-10px,-10px"; // Уакытша
                                NominalRotateLeft = "0deg"; // Уакытша
                                NominalRotateRight = "0deg"; // Уакытша
                                CalibrConstLeft = km.CrossRailProfile.DownhillLeft[indexCross].RadianToAngle().ToString("0.00").Replace(",", ".");
                                CalibrConstRight = km.CrossRailProfile.DownhillRight[indexCross].RadianToAngle().ToString("0.00").Replace(",", ".");
                                DownHillLeftValue = "1/" + (int)(1 / km.CrossRailProfile.DownhillLeft[indexCross]);
                                TiltLeftValue = "1/" + (int)(1 / km.CrossRailProfile.TreadTiltLeft[indexCross]);
                                DownHillRightValue = "1/" + (int)(1 / km.CrossRailProfile.DownhillLeft[indexCross]);
                                TiltRightValue = "1/" + (int)(1 / km.CrossRailProfile.TreadTiltLeft[indexCross]);

                                NominalRailScheme = onlineModeData.GetNominalRailScheme(Rails.r50);
                            }
                        }
                        catch(Exception e)
                        {
                            Console.WriteLine("ads" + e);
                        }
                     
                    }
                    //if (km.Gaps != null)
                    //{
                    //    foreach (var gap in km.Gaps)
                    //    {

                    //        if (gap.Meter == currentMetre)
                    //        {
                    //            Data[(int)(gap.Threat == Threat.Right ? Series.GapRight : Series.GapLeft)] = gap.Length.ToString();



                    //            if (PrevImageIndex != yPosition)
                    //            {
                    //                using (var image = AdditionalParametersRepository.GetFrame(gap.Frame_Number, gap.File_Id))
                    //                {
                    //                    if (image != null)
                    //                    {
                    //                        System.IO.MemoryStream ms = new MemoryStream();
                    //                        image.Save(ms, ImageFormat.Png);
                    //                        byte[] byteImage = ms.ToArray();
                    //                        PrevImageIndex = yPosition;
                    //                        RailImage = new RailImage();
                    //                        RailImage.Base64 = Convert.ToBase64String(byteImage);

                    //                        RailImage.Objects = AdditionalParametersRepository.GetObjectsByFrameNumber(gap.Frame_Number, Trip.Id);
                    //                        RailImage.Gaps = AdditionalParametersRepository.GetGapsByFrameNumber(gap.Frame_Number, Trip.Id);
                    //                        RailImage.Width = image.Width;
                    //                        RailImage.Height = image.Height;
                    //                    }
                    //                }
                    //            }
                    //        }
                    //    }
                    //}
                    break;
                }
                
                
            }
            return true;
        }
       
        public string GetCoordinate(double yPosition)
        {
            if (_kilometers == null)
                return "0.00";
            int length = 0;
            foreach (var km in _kilometers)
            {
                length += km.GetLength();
                if (yPosition < length)
                {
                    return km.Number.ToString() + "." + (Trip.Travel_Direction == Direction.Direct ? ((int)yPosition - (length - km.GetLength() + km.Start_m) ) : km.GetLength() - ((int)yPosition - (length - km.GetLength()))).ToString();
                }
            }
            return "0.00";
        }
        public int GetLength()
        { 
            if (_kilometers==null)
                return 0;
            return _kilometers.Select(kilometer => Math.Abs(kilometer.Start_m - kilometer.Final_m)).Sum();
        }
        public string GetSeriesValue(Series serie, double dposition)
        {
            int position = (int)dposition;
            if (_kilometers == null)
                return "";
            switch (serie)
            {
                case Series.LevelZero:
                    {
                        int length = 0;
                        string value = "";
                        foreach (var km in _kilometers)
                        {
                            length += km.Length;
                            if (position < length)
                            {
                                int currentMetre = Trip.Travel_Direction == Direction.Reverse ? km.Length - (length-position) : length-position;
                                for (int index = 0; index < km.furb0.Count; index++)
                                {
                                    int metre = Trip.Travel_Direction == Direction.Reverse ? km.Length - index : index;
                                    if (metre==currentMetre)
                                    {
                                        value = km.furb0[index].ToString("0.00");
                                        break;
                                    }
                                 }
                            }
                        }
                        return value;
                    }
            }
            return "";
        }

        public event Action OnChange;

        private void NotifyDataChanged() => OnChange?.Invoke();
        public string PressedButtonState => "color:chartreuse;font-size: 30px;";
       
        
        ///online mode parameters
        public int CurrentFrameIndex { get; set; } = 0;
        public int CurrentVideoFrame { get; set; } = 0;
        public long VideoMs { get; set; } = 0; 
        public int Speed { get; set; } = 1;
        public bool Processing = true;
        public bool VideoProcessing = false;
        public int Kilometer { get; set; } = -1;
        public int Meter { get; set; } = 0;
        public int ProfileMeter { get; set; } = 0;
        public int Picket { get; set; } = -1;
        public string DataS { get; set; }
        public double[] PointsLeft { get; set; }
        public double[] PointsRight { get; set; }
        public string ViewBox { get; set; }
        public List<int> Dy = new List<int>();
        List<int> dy = new List<int>();
        public WorkMode WorkMode { get; set; } = WorkMode.NotSet;

        [Obsolete]
        public Bitmap CurrentFrame { get; set; }
        public string CalibrConstLeft { get; set; }
        public string CalibrConstRight { get; set; }
        public string DownHillLeftValue { get; set; }
        public string TiltLeftValue { get; set; }
        public string DownHillRightValue { get; set; }
        public string TiltRightValue { get; set; }
        public string NominalRailScheme { get; set; }
        public string NominalTranslateLeft { get; set; }
        public string NominalTranslateRight { get; set; }
        public string NominalRotateLeft { get; set; }
        public string NominalRotateRight { get; set; }
        public string ViewBoxLeft { get; set; }
        public string ViewBoxRight { get; set; }

        public double[] CurrentProfileLeft()
        {

            var filePath = @"D:\common\DATA\IN\2019_04\Vnutr--profil--koridor2020-02-21-12-36-22.Profile-Calibr";
            if (!File.Exists(filePath))
                filePath = @"G:\Data\2019_04\Vnutr--profil--koridor2020-02-21-12-36-22.Profile_Calibr";
            using (BinaryReader reader = new BinaryReader(File.Open(filePath, FileMode.Open)))
            {

                try
                {
                    var data = reader.ReadBytes(8);
                    var count = BitConverter.ToSingle(data, 0);
                    var size = BitConverter.ToSingle(data, 4);
                    long ll = CurrentFrameIndex * (long)size + 8;
                    reader.BaseStream.Seek(ll, SeekOrigin.Begin);
                    var encoderCounter = reader.ReadUInt64();
                    var timestamp = reader.ReadInt64();
                    var frameNumber = reader.ReadUInt64();
                    var camtime = reader.ReadUInt64();
                    var kilometer = reader.ReadInt32();
                    var meter = reader.ReadInt32();
                    //var profile = reader.ReadBytes((int)(size - 40));
                    double[] vector = new double[(int)count * 2 - 10];
                    for (int i = 0; i < vector.Length; i++)
                        vector[i] = reader.ReadSingle();
                    return vector;
                }
                catch
                {
                    return null;
                }

            }
        }

        public double[] CurrentProfileRight()
        {

            var filePath = @"D:\common\DATA\IN\2019_04\Vnutr--profil--kupe2020-02-21-12-36-22.Profile-Calibr";
            if (!File.Exists(filePath))
                filePath = @"G:\Data\2019_04\Vnutr--profil--kupe2020-02-21-12-36-22.Profile_Calibr";
            using (BinaryReader reader = new BinaryReader(File.Open(filePath, FileMode.Open)))
            {

                try
                {
                    var data = reader.ReadBytes(8);
                    var count = BitConverter.ToSingle(data, 0);
                    var size = BitConverter.ToSingle(data, 4);
                    long ll = CurrentFrameIndex * (long)size + 8;
                    reader.BaseStream.Seek(ll, SeekOrigin.Begin);
                    var encoderCounter = reader.ReadUInt64();
                    var timestamp = reader.ReadInt64();
                    var frameNumber = reader.ReadUInt64();
                    var camtime = reader.ReadUInt64();
                    var kilometer = reader.ReadInt32();
                    var meter = reader.ReadInt32();
                    //var profile = reader.ReadBytes((int)(size - 40));
                    double[] vector = new double[(int)count * 2 - 10];
                    for (int i = 0; i < vector.Length; i++)

                        vector[i] = i % 2 == 1 ? (-1) * reader.ReadSingle() : reader.ReadSingle();
                    return vector;
                }
                catch
                {
                    return null;
                }

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
            return $"Километр: {Kilometer} Пикет: {Picket} Метр: {Meter} Текущий кадр: ";
        }
        public string VectorToPoints(int[] vector)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < vector.Length; i++)
            {
                sb.Append($"{i},{vector[i]} ");
            }
            ViewBox = $"0 {vector.Min() - 5} {vector.Length} {vector.Max() + 5}";
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


    }
    
    
    public enum ShowButtons { Pasport= 0, Signal=1, ZeroLines, MainParams, Event, Digression, MainParamsPlus,
        DangerousDigression, DangerousForEmtyWagon, ThirdDegreeDigressions, CloseToDangerous, CloseTo2Degree,
        SecondDegreeDigression,FirstDegreeDigression, OthersDigressions, ExcludedOnSwitch, ExcludedByOerator,
      
        NotTakenOnRating, Joints, RailProfile, Gaps, GapCloseToDangerous, Bolts, Fasteners, PerShpals, DefShpals, 

        PU, NPK, ShortWaves, LongWaves, MediumWaves, IznosBok, IznosVert, IznosPriv, Correction, 

        ByKilometer

    }
 
    
    public enum Series { Pasport = 0, LevelZero = 1, LevelPasport = 2, LevelSignal = 3, 
        StrightRightZero = 4, StrightRightPasport = 5, StrightRightSignal = 6,
        StrightLeftZero = 7, StrightLeftPasport = 8, StrightLeftSignal = 9,
        GaugePasport = 10, GaugeSignal = 11, DrawdownLeft = 12, DrwadownRight = 13, GapLeft = 14, GapRight = 15,
        SideWearLeft = 16, SideWearRight = 17, VertWearLeft = 18, VertWearRight = 19, GivenWearLeft = 20, GivenWearRight = 21,
        TreadTiltLeft = 22, TreadTiltRight = 23, DownHillLeft = 24, DownHillRight = 25, HeadWear45Left = 26, HeadWear45Right = 27, Speed = 28,
        Section = 29
    }
    public enum WorkMode { 
        NotSet = -1, Postprocessing = 0, Online = 1
    }
    }

