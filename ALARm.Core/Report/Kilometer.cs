using ALARm.Core.AdditionalParameteres;
using ALARm.Core.Report;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace ALARm.Core
{

    public class Kilometer
    {
        public List<int> passSpeed = new List<int>();
        public List<int> freightSpeed = new List<int>();
        public List<int> remontPassSpeed = new List<int>();
        public List<int> remontFreightSpeed = new List<int>();
        public List<float> prosLeft = new List<float>();
        public List<float> prosRight = new List<float>();
        public List<double> fsh0 = new List<double>();
        public List<int> sign = new List<int>();
        public List<int> signTrapez = new List<int>();
        public List<int> meter = new List<int>();
        public List<double> frad = new List<double>();
        public List<float> flvl = new List<float>();
        public List<double> flvl0 = new List<double>();
        public List<float> fsh = new List<float>();
        public List<float> fsr_rh1 = new List<float>();
        public List<float> frih1 = new List<float>();
        public List<double> fZeroStright = new List<double>();
        public List<float> fsr_rh2 = new List<float>();
        public List<float> frih2 = new List<float>();
        public List<float> fsr_urb = new List<float>();
        public List<float> furb = new List<float>();
        public List<float> furb0 = new List<float>();
        public List<int> snorm = new List<int>();
        public List<int> fnorm = new List<int>();

        public List<AlertNote> LevelNotes = new List<AlertNote>();
        public List<AlertNote> StrigthLeftNotes = new List<AlertNote>();
        public List<AlertNote> StrigthRightNotes = new List<AlertNote>();
        public List<AlertNote> GaugeNotes = new List<AlertNote>();
        public List<AlertNote> DrawdownLeftNotes = new List<AlertNote>();
        public List<AlertNote> DrawdownRightNotes = new List<AlertNote>();
        public DateTime TripDate { get; set; }

        public bool Crashed { get; set; } = false;
        public bool Corrected { get; set; }

        public int Meter = 0;
        public int RealMeterInOnline => Meter + Start_m;
        public int Correction = 0;
        public List<float> piketSpeeds;
        /// <summary>
        /// установленные скорости
        /// </summary>
        public List<Speed> Speeds;
        /// <summary>
        /// искусственные сооружения
        /// </summary>
        public List<ArtificialConstruction> Artificials;
        /// <summary>
        /// административное деление
        /// </summary>
        public List<PdbSection> PdbSection;

        public List<RepairProject> RepairProjects { get; private set; }

        /// <summary>
        /// тип шпал
        /// </summary>
        public List<CrossTie> CrossTies;
        /// <summary>
        /// стрелочные переводы
        /// </summary>
        public List<Switch> Switches { get; set; }

        public List<Elevation> ElevationData;

        public List<CheckSection> CheckSections { get; set; }
        public List<Deep> Depths { get; set; }
        public List<StraighteningThread> StraighteningThreads { get; set; }
        public List<ProfileObject> IsoJoints { get; set; }
        public List<RailsBrace> RailsBrace { get; set; }
        public List<RailsSections> RailSection { get; set; }
        public List<NonstandardKm> NonstandardKms { get; set; }
        public Fragment Fragment { get; set; }
        public string Sector { get; set; }

        public string Rating_bedomost { get; set; }

        public List<StationSection> StationSection { get; set; }
        public List<LongRails> LongRailses { get; set; }
        public List<Curve> Curves { get; set; }
        public List<NormaWidth> Normas { get; set; }
        public List<VPicket> VPickets { get; set; }
        public string Primech { get; set; }
        public int SecondDegreeCountLevel { get; set; }//2
        public int ThirdDegreeCountLevel { get; set; }//3
        public int FourthDegreeCountLevel { get; set; }//4
        public int SecondDegreeCountSag { get; set; }//2
        public int ThirdDegreeCountSag { get; set; }//3
        public int ThirdDegreeCountSagFreit { get; set; }//3
        public int FourthDegreeCountSag { get; set; }//4
        public int SecondDegreeCountStrightening { get; set; }//2
        public int ThirdDegreeCountStrightening { get; set; }//3
        public int FourthDegreeCountStrightening { get; set; }
        public int SecondDegreeCountGauge { get; set; }//2
        public int ThirdDegreeCountGauge { get; set; }//3
        public int Length { get; set; }
        public int FourthDegreeCountGauge { get; set; } //4

        public int SecondDegreeCountDrawdown { get; set; }//2
        public int ThirdDegreeCountDrawdown { get; set; }//3
        public int FourthDegreeCountDrawdown { get; set; }//4

        public int FDBroad { get; set; }
        public int FDConstrict { get; set; }
        public int FDSkew { get; set; }
        public int FDDrawdown { get; set; }
        public int FDStright { get; set; }
        public int FDLevel { get; set; }

        public int Id
        {
            get; set;
        }

        public int Number { get; set; }
        public string SpeedLim { get; set; }
        public int Start_m { get; set; } = 1;
        public int Final_m { get; set; } = 1000;

        public string Direction_name { get; set; }
        public string Direction_code { get; set; }
        public string PchCode { get; set; }
        public string PchuCode { get; set; }
        public string PdCode { get; set; }
        public string PdbCode { get; set; }
        public string PchuChief { get; set; }
        public string PdChief { get; set; }
        public string PdbChief { get; set; }


        public float Lkm { get; set; }
        public int GetLength() {
            return Math.Abs(Start_m - Final_m)+1 ;
            //return Math.Abs(Start_m - Final_m) + 1;
        }
        public int Track_id { get; set; }
        public string Track_name { get; set; }
        public DateTime Passage_time { get; set; }
        public string SignalGauge { get; set; } = "";
        public string Kupe { get; set; } = "";
        public string Koridor { get; set; } = "";
        public string PasportLevel { get; set; } = "";
        public string SignalLevel { get; set; } = "";
        public string SpeedSeries { get; set; } = "";
       // public float StrightKoef = 3.5f;
        public float StrightKoef = 1f;
        public float GaugeKoef = 2;
        public float WearKoef = 5;
        public float WavesKoef = 30;
        public float DegKoef = 1000;

        public List<double> Level = new List<double>();
        public List<double> LevelAvg = new List<double>();
        public List<double> Gauge = new List<double>();
        public List<double> _Kupe = new List<double>();
        public List<double> _Koridor = new List<double>();
        public List<double> StrightLeft = new List<double>();




        public List<double> LevelAvgTrapezoid { get; set; }

        public List<double> StrightAvgTrapezoid { get; set; }
        public List<NatureCurves> CurvesNature { get; set; } = new List<NatureCurves> { };
        public int CorrCount { get; set; }
        public void Clear(Kilometer kilometer = null, int clearCount = -1)
        {
            if (kilometer != null)
            {
                for (int i = 0; i < Speed.Count; i++)
                {
                    kilometer.ShortWavesLeft.Add(ShortWavesLeft[i]);
                    kilometer.ShortWavesRight.Add(ShortWavesRight[i]);

                    kilometer.MediumWavesLeft.Add(MediumWavesLeft[i]);
                    kilometer.MediumWavesRight.Add(MediumWavesRight[i]);

                    kilometer.LongWavesLeft.Add(LongWavesLeft[i]);
                    kilometer.LongWavesRight.Add(LongWavesRight[i]);

                    kilometer.Speed.Add(Speed[i]);
                    kilometer.DrawdownRight.Add(DrawdownRight[i]);
                    kilometer.DrawdownLeft.Add(DrawdownLeft[i]);
                    kilometer.Gauge.Add(Gauge[i]);
                    kilometer.StrightRight.Add(StrightRight[i]);
                    kilometer.StrightLeft.Add(StrightLeft[i]);

                    kilometer.Level.Add(Level[i]);
                    kilometer.LevelAvg.Add(LevelAvg[i]);
                    kilometer.StrightAvg.Add(StrightAvg[i]);
                    kilometer.Level0.Add(Level0[i]);
                    kilometer.Meters.Add(Meters[i]);
                    kilometer.RealKm.Add(RealKm[i]);
                    kilometer.Latitude.Add(Latitude[i]);
                    kilometer.Longitude.Add(Longitude[i]);
                    kilometer.XCamLeft.Add(XCamLeft[i]);
                    kilometer.XCamRight.Add(XCamRight[i]);
                    kilometer.SwitchesReal.Add(SwitchesReal[i]);
                    kilometer.LevelCorrection.Add(Level[i]);
                    kilometer.GaugeCorrection.Add(GaugeCorrection[i]);
                    kilometer._Kupe.Add(_Kupe[i]);
                    kilometer._Koridor.Add(_Koridor[i]);
                    kilometer.GaugeCorrection.Add(GaugeCorrection[i]);
                    kilometer.Corrections.Add(Corrections[i]);


                    

                    kilometer.SignalLevel += (-1 * Level[i]).ToString().Replace(",", ".") + "," + kilometer.Meter.ToString() + " ";
                    kilometer.ZeroLevel += (-1 * LevelAvg[i]).ToString().Replace(",", ".") + "," + kilometer.Meter.ToString() + " ";
                    kilometer.SignalStraightLeft += (-1 * StrightLeft[i] * kilometer.StrightKoef).ToString().Replace(",", ".") + "," + kilometer.Meter.ToString() + " ";
                    kilometer.SignalStraightRight += (-1 * StrightRight[i] * kilometer.StrightKoef).ToString().Replace(",", ".") + "," + kilometer.Meter.ToString() + " ";
                    //kilometer.LevelAvg += (-1 * LevelAvg[i]).ToString().Replace(",", ".") + "," + kilometer.Meter.ToString() + " "; 

                    kilometer.ZeroStraightLeft += (-1 * StrightAvg[i] * kilometer.StrightKoef).ToString().Replace(",", ".") + "," + kilometer.Meter.ToString() + " ";
                    kilometer.ZeroStraightRight += (-1 * StrightAvg[i] * kilometer.StrightKoef).ToString().Replace(",", ".") + "," + kilometer.Meter.ToString() + " ";
                    kilometer.SignalDrawdownLeft += (-1 * DrawdownLeft[i]).ToString().Replace(",", ".") + "," + kilometer.Meter.ToString() + " ";
                    kilometer.SignalDrawdownRight += (-1 * DrawdownRight[i]).ToString().Replace(",", ".") + "," + kilometer.Meter.ToString() + " ";
                    kilometer.SignalGauge += ((Gauge[i] - 1520) * kilometer.GaugeKoef).ToString("0.00").Replace(",", ".") + "," + kilometer.Meter.ToString() + " ";
                    kilometer.Kupe += ((Gauge[i] - 1520) * kilometer.GaugeKoef).ToString("0.00").Replace(",", ".") + "," + kilometer.Meter.ToString() + " ";
                    kilometer.Koridor += ((Gauge[i] - 1520) * kilometer.GaugeKoef).ToString("0.00").Replace(",", ".") + "," + kilometer.Meter.ToString() + " ";
                    kilometer.Meter++;
                }
                kilometer.CorrectionValue = this.CorrectionValue;
                kilometer.CorrectionMeter = this.CorrectionMeter;
                kilometer.CorrectionType = this.CorrectionType;
                this.CorrectionType = CorrectionType.None;
                this.CorrectionValue = -1;
                this.CorrectionMeter = -1;
            }
            if (clearCount == -1)
            {
                ShortWavesRight.Clear();
                ShortWavesLeft.Clear();
                MediumWavesLeft.Clear();
                MediumWavesRight.Clear();
                LongWavesLeft.Clear();
                LongWavesRight.Clear();

                Speed.Clear();
                DrawdownRight.Clear();
                DrawdownLeft.Clear();
                Gauge.Clear();
                StrightRight.Clear();
                StrightLeft.Clear();
                Level.Clear();
                LevelAvg.Clear();
                StrightAvg.Clear();
                Level0.Clear();
                Meters.Clear();
                RealKm.Clear();
                Latitude.Clear();
                Longitude.Clear();
                XCamLeft.Clear();
                XCamRight.Clear();
                SwitchesReal.Clear();
                LevelCorrection.Clear();
                GaugeCorrection.Clear();
                Corrections.Clear();
                SignalLevel = " ";
                ZeroLevel = " ";
                SignalStraightLeft = " ";
                SignalStraightRight = " ";
                ZeroStraightLeft = " ";
                ZeroStraightRight = " ";
                SignalDrawdownLeft = " ";
                SignalDrawdownRight = " ";
                SignalGauge = " ";
                Meter = 0;
            }
            else
            {

                int clearIndex = Speed.Count - clearCount;
                SignalLevel = SignalLevel.CutFromIndex(" ", clearIndex);
                ZeroLevel = ZeroLevel.CutFromIndex(" ", clearIndex);
                SignalStraightLeft = SignalStraightLeft.CutFromIndex(" ", clearIndex);
                SignalStraightRight = SignalStraightRight.CutFromIndex(" ", clearIndex);
                ZeroStraightLeft = ZeroStraightLeft.CutFromIndex(" ", clearIndex);
                ZeroStraightRight = ZeroStraightRight.CutFromIndex(" ", clearIndex);
                SignalDrawdownLeft = SignalDrawdownLeft.CutFromIndex(" ", clearIndex);
                SignalDrawdownRight = SignalDrawdownRight.CutFromIndex(" ", clearIndex);
                SignalGauge = SignalGauge.CutFromIndex(" ", clearIndex);
                Kupe = Kupe.CutFromIndex(" ", clearIndex);
                Koridor = Koridor.CutFromIndex(" ", clearIndex);
                Speed.RemoveRange(clearIndex, clearCount);
                DrawdownRight.RemoveRange(clearIndex, clearCount);
                DrawdownLeft.RemoveRange(clearIndex, clearCount);
                Gauge.RemoveRange(clearIndex, clearCount);
                StrightRight.RemoveRange(clearIndex, clearCount);
                StrightLeft.RemoveRange(clearIndex, clearCount);
                Level.RemoveRange(clearIndex, clearCount);
                LevelAvg.RemoveRange(clearIndex, clearCount);
                StrightAvg.RemoveRange(clearIndex, clearCount);
                Level0.RemoveRange(clearIndex, clearCount);
                Meters.RemoveRange(clearIndex, clearCount);
                RealKm.RemoveRange(clearIndex, clearCount);
                Latitude.RemoveRange(clearIndex, clearCount);
                Longitude.RemoveRange(clearIndex, clearCount);
                XCamLeft.RemoveRange(clearIndex, clearCount);
                XCamRight.RemoveRange(clearIndex, clearCount);
                SwitchesReal.RemoveRange(clearIndex, clearCount);
                LevelCorrection.RemoveRange(clearIndex, clearCount);
                GaugeCorrection.RemoveRange( clearIndex, clearCount);
                Corrections.RemoveRange(clearIndex, clearCount);
            }
        }

        public Rating CalcQualitiveRating(int mainParamPointSum, int districtedCount)
        {
            if (districtedCount > 0)
                return Rating.Н;
            AllowedSpeed = Speeds.Count > 0 ? Speeds.Last().Passenger : DefaultSpeed;
            if (AllowedSpeed.Between(61, 250)) {
                if (mainParamPointSum <= 5)
                    return Rating.О;
                if (mainParamPointSum.Between(6, 25))
                    return Rating.Х;
                if (mainParamPointSum.Between(26, 100))
                    return Rating.У;
                if (mainParamPointSum >= 101)
                    return Rating.Н;
            } else
           if (AllowedSpeed <= 60)
            {
                if (mainParamPointSum <= 10)
                    return Rating.О;
                if (mainParamPointSum.Between(11, 40))
                    return Rating.Х;
                if (mainParamPointSum.Between(41, 200))
                    return Rating.У;
                if (mainParamPointSum >= 201)
                    return Rating.Н;
            }
            return Rating.О;
        }

        public void AddData(OutData outdata, int direction, int meter, List<double> koefs)
        {

            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en-US");
            Speed.Add(outdata.speed);
            DrawdownRight.Add(outdata.drawdown_right);
            DrawdownLeft.Add(outdata.drawdown_left);
            Gauge.Add(outdata.gauge);
            StrightRight.Add(outdata.stright_left);
            StrightLeft.Add(outdata.stright_right);
            Level.Add(outdata.level);
            LevelAvg.Add(outdata.level_avg);
            StrightAvg.Add(outdata.stright_avg);
            Level0.Add(outdata.level_zero);
            Meters.Add(meter);
            _Meters.Add(outdata._meters);
            RealKm.Add(outdata.km);
            Latitude.Add(outdata.latitude);
            Longitude.Add(outdata.longitude);
            XCamLeft.Add(outdata.x101_kupe);
            XCamRight.Add(outdata.x102_koridor);
            SwitchesReal.Add(0);
            LevelCorrection.Add(outdata.level_correction);
            GaugeCorrection.Add(outdata.gauge_correction);
            Corrections.Add(outdata.correction);

            _Kupe.Add(outdata.y101_kupe);
            _Koridor.Add(outdata.y102_koridor);

            SignalLevel += $"{-1 * outdata.level:0.00},{Meter} ";
            SpeedSeries += $"{outdata.speed * 0.5:0.00},{Meter} ";
            ZeroLevel += $"{-1 * outdata.level_avg:0.00},{Meter} ";
            SignalStraightLeft += $"{-1 * outdata.stright_left * StrightKoef:0.00},{Meter} ";
            SignalStraightRight += $"{-1 * outdata.stright_right * StrightKoef:0.00},{Meter} ";
            ZeroStraightLeft += $"{-1 * outdata.stright_avg * StrightKoef:0.00},{Meter} ";
            ZeroStraightRight += $"{-1 * outdata.stright_avg * StrightKoef:0.00},{Meter} ";
            SignalDrawdownLeft += $"{-1 * outdata.drawdown_left:0.00},{Meter} ";
            SignalDrawdownRight += $"{-1 * outdata.drawdown_right:0.00},{Meter} ";
            SignalGauge += $"{(outdata.gauge - 1520) * GaugeKoef:0.00},{Meter} ";
            Kupe += $"{(outdata.x101_kupe - 86) * -3:0.00},{Meter} ";
            Koridor += $"{(outdata.x102_koridor - 100) * -3:0.00}, {Meter} ";

            Level1 += $"{-1 * outdata.level1 * koefs[0]:0.00},{Meter} ";
            Level2 += $"{-1 * outdata.level2 * koefs[1]:0.00},{Meter} ";
            Level3 += $"{-1 * outdata.level3 * koefs[2]:0.00},{Meter} ";
            Level4 += $"{-1 * outdata.level4 * koefs[3]:0.00},{Meter} ";
            Level5 += $"{-1 * outdata.level5 * koefs[4]:0.00},{Meter} ";

            Stright1 += $"{-1 * outdata.stright1 * koefs[1]:0.00},{Meter} ";
            Stright2 += $"{-1 * outdata.stright2 * koefs[2]:0.00},{Meter} ";
            Stright3 += $"{-1 * outdata.stright3 * koefs[3]:0.00},{Meter} ";
            Stright4 += $"{-1 * outdata.stright4 * koefs[4]:0.00},{Meter} ";
            Stright5 += $"{-1 * outdata.stright5 * koefs[5]:0.00},{Meter} ";



            Meter++;
        }

        public void AddData(OutData outdata, int meter, List<double> koefs, CrosProf profdata = null)
        {

            int sign = 1;
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en-US");
            Speed.Add(outdata.speed);
            DrawdownRight.Add(outdata.drawdown_right);
            DrawdownLeft.Add(outdata.drawdown_left);
            Gauge.Add(outdata.gauge);
            StrightRight.Add(sign * outdata.stright_left);
            StrightLeft.Add(sign * outdata.stright_right);
            Level.Add(sign * outdata.level);
            LevelAvg.Add(sign * outdata.level_avg);
            StrightAvg.Add(sign * outdata.stright_avg);
            Level0.Add(sign * outdata.level_zero);
            Meters.Add(meter);
            _Meters.Add(outdata._meters);
            RealKm.Add(outdata.km);
            Latitude.Add(outdata.latitude);
            Longitude.Add(outdata.longitude);
            XCamLeft.Add(outdata.x101_kupe);
            XCamRight.Add(outdata.x102_koridor);
            SwitchesReal.Add(0);
            LevelCorrection.Add(outdata.level_correction);
            GaugeCorrection.Add(outdata.gauge_correction);
            Corrections.Add(outdata.correction);

            _Kupe.Add(outdata.y101_kupe);
            _Koridor.Add(outdata.y102_koridor);
            SpeedSeries += $"{outdata.speed * 0.5:0.00},{Meter} ";
            SignalLevel += $"{sign * outdata.level:0.00},{Meter} ";
            ZeroLevel += $"{sign * outdata.level_avg:0.00},{Meter} ";
            SignalStraightLeft += $"{sign * outdata.stright_left * StrightKoef:0.00},{Meter} ";
            SignalStraightRight += $"{sign * outdata.stright_right * StrightKoef:0.00},{Meter} ";
            ZeroStraightLeft += $"{sign * outdata.stright_avg * StrightKoef:0.00},{Meter} ";
            ZeroStraightRight += $"{sign * outdata.stright_avg * StrightKoef:0.00},{Meter} ";
            SignalDrawdownLeft += $"{sign * outdata.drawdown_left:0.00},{Meter} ";
            SignalDrawdownRight += $"{sign * outdata.drawdown_right:0.00},{Meter} ";
            SignalGauge += $"{(outdata.gauge - 1520) * GaugeKoef:0.00},{Meter} ";
            Kupe += $"{(outdata.x101_kupe - 86) * -3:0.00},{Meter} ";
            Koridor += $"{(outdata.x102_koridor - 100) * -3:0.00}, {Meter} ";

            Level1 += $"{-1 * outdata.level1 * koefs[0]:0.00},{Meter} ";
            Level2 += $"{-1 * outdata.level2 * koefs[1]:0.00},{Meter} ";
            Level3 += $"{-1 * outdata.level3 * koefs[2]:0.00},{Meter} ";
            Level4 += $"{-1 * outdata.level4 * koefs[3]:0.00},{Meter} ";
            Level5 += $"{-1 * outdata.level5 * koefs[4]:0.00},{Meter} ";

            Stright1 += $"{-1 * outdata.stright1 * koefs[1]:0.00},{Meter} ";
            Stright2 += $"{-1 * outdata.stright2 * koefs[2]:0.00},{Meter} ";
            Stright3 += $"{-1 * outdata.stright3 * koefs[3]:0.00},{Meter} ";
            Stright4 += $"{-1 * outdata.stright4 * koefs[4]:0.00},{Meter} ";
            Stright5 += $"{-1 * outdata.stright5 * koefs[5]:0.00},{Meter} ";
            Meter++;
        }


        public void AddDataRange(List<OutData> outdatas, Kilometer km)
        {
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfo("en-US");
            int sign = 1;
            foreach (var outdata in outdatas)
            {

                //km.LongWavesLeft
                //km.LongWavesRight
                //km.ShortwavesLeft
                //km.ShortWavesRight
                //km.LongWavesLeft
                //km.LongWavesRight


                km.Speed.Add(outdata.speed);
                km.DrawdownRight.Add(outdata.drawdown_right);
                km.DrawdownLeft.Add(outdata.drawdown_left);
                km.Gauge.Add(outdata.gauge);
                km.StrightRight.Add(sign * outdata.stright_left);
                km.StrightLeft.Add(sign * outdata.stright_right);
                km.Level.Add(sign * outdata.level);
                km.LevelAvg.Add(sign * outdata.level_avg);
                km.StrightAvg.Add(sign * outdata.stright_avg);
                km.Level0.Add(sign * outdata.level_zero);
                km.Meters.Add(outdata.meter);
                km._Meters.Add(outdata._meters);
                km.RealKm.Add(outdata.km);
                km.Latitude.Add(outdata.latitude);
                km.Longitude.Add(outdata.longitude);
                km.XCamLeft.Add(outdata.x101_kupe);
                km.XCamRight.Add(outdata.x102_koridor);
                km.SwitchesReal.Add(0);
                km.LevelCorrection.Add(outdata.level_correction);
                km.GaugeCorrection.Add(outdata.gauge_correction);
                km.Corrections.Add(outdata.correction);

                km._Kupe.Add(outdata.y101_kupe);
                km._Koridor.Add(outdata.y102_koridor);

                km.SignalLevel += $"{sign * outdata.level:0.00},{Meter} ";
                km.ZeroLevel += $"{sign * outdata.level_avg:0.00},{Meter} ";
                km.SignalStraightLeft += $"{sign * outdata.stright_left * StrightKoef:0.00},{Meter} ";
                km.SignalStraightRight += $"{sign * outdata.stright_right * StrightKoef:0.00},{Meter} ";
                km.ZeroStraightLeft += $"{sign * outdata.stright_avg * StrightKoef:0.00},{Meter} ";
                km.ZeroStraightRight += $"{sign * outdata.stright_avg * StrightKoef:0.00},{Meter} ";
                km.SignalDrawdownLeft += $"{sign * outdata.drawdown_left:0.00},{Meter} ";
                km.SignalDrawdownRight += $"{sign * outdata.drawdown_right:0.00},{Meter} ";
                km.SignalGauge += $"{(outdata.gauge - 1520) * GaugeKoef:0.00},{Meter} ";
                km.Kupe += $"{(outdata.x101_kupe - 86) * -3:0.00},{Meter} ";
                km.Koridor += $"{(outdata.x102_koridor - 100) * -3:0.00}, {Meter} ";

                //km.Level1 += $"{(-1 * outdata.level1 * koefs[0]).ToString("0.00")},{Meter} ";
                //km.Level2 += $"{(-1 * outdata.level2 * koefs[1]).ToString("0.00")},{Meter} ";
                //km.Level3 += $"{(-1 * outdata.level3 * koefs[2]).ToString("0.00")},{Meter} ";
                //km.Level4 += $"{(-1 * outdata.level4 * koefs[3]).ToString("0.00")},{Meter} ";
                //km.Level5 += $"{(-1 * outdata.level5 * koefs[4]).ToString("0.00")},{Meter} ";

                //km.Stright1 += $"{(-1 * outdata.stright1 * koefs[1]).ToString("0.00")},{Meter} ";
                //km.Stright2 += $"{(-1 * outdata.stright2 * koefs[2]).ToString("0.00")},{Meter} ";
                //km.Stright3 += $"{(-1 * outdata.stright3 * koefs[3]).ToString("0.00")},{Meter} ";
                //km.Stright4 += $"{(-1 * outdata.stright4 * koefs[4]).ToString("0.00")},{Meter} ";
                //km.Stright5 += $"{(-1 * outdata.stright5 * koefs[5]).ToString("0.00")},{Meter} ";
            }
        }

        public List<double> StrightRight = new List<double>();
        public List<double> StrightAvg = new List<double>();
        public List<double> Level0 = new List<double>();
        public List<double> GaugeCorrection = new List<double>();
        public List<double> LevelCorrection = new List<double>();
        public List<double> SideWearLeft_ = new List<double>();
        public List<double> SideWearRight_ = new List<double>();
        public List<double> RealKm = new List<double>();
        public List<double> Latitude = new List<double>();
        public List<double> Longitude = new List<double>();
        public List<int> Speed = new List<int>();
        public List<int> Corrections = new List<int>();
        public List<double> DrawdownLeft = new List<double>();
        public List<double> DrawdownRight = new List<double>();

        public List<double> ShortWavesLeft = new List<double>();
        public List<double> ShortWavesRight = new List<double>();

        public List<double> MediumWavesLeft = new List<double>();
        public List<double> MediumWavesRight = new List<double>();

        public List<double> LongWavesLeft = new List<double>();
        public List<double> LongWavesRight = new List<double>();

        public List<double> XCamLeft = new List<double>();
        public List<double> XCamRight = new List<double>();
        public List<double> SwitchesReal = new List<double>();
        public List<int> Meters = new List<int>();
        public List<int> _Meters = new List<int>();
        public List<Picket> Pickets = new List<Picket>();
        public int CorrectionValue { get; set; } = -1;
        public int CorrectionMeter { get; set; } = -1;
        public CorrectionType CorrectionType { get; set; } = CorrectionType.None;
        public List<DigressionMark> Digressions { get; set; } = new List<DigressionMark>();

        public List<Digression> AdditionalDigressions { get; set; } = new List<Digression>();
        //List<> GetCrossRailProfileFromDBbyKm(int kilometer, long trip_id);
        public List<CorrectionNote> CorrectionNotes = new List<CorrectionNote>();
        public List<Gap> Gaps { get; set; } = new List<Gap>();
        public List<Digression> Bolts { get; set; } = new List<Digression>();
        public List<Digression> Fasteners { get; set; } = new List<Digression>();
        public List<Digression> DefShpals { get; set; } = new List<Digression>();
        public List<Digression> PerShpals { get; set; } = new List<Digression>();
        public List<Heat> Heats { get; set; }
        
    
        private bool ShifrovkaGenerated { get; set; } = false;
        public bool IsPrinted { get; set; } = false;

        public CrossRailProfile CrossRailProfile { get; set; } = new CrossRailProfile();

        public bool WriteCurrentData(Trips trip, List<double> prev50, List<double> next50, List<double> prevStrightAvgPart, List<double> nextStrightAvgPart, IMainTrackStructureRepository mainTrackStructureRepository, IRdStructureRepository rdStructureRepository)
        {
            ShifrovkaGenerated = true;
            CultureInfo customCulture = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentCulture = customCulture;
            var _ = "\t";
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
            //birinshi
            var outData = rdStructureRepository.GetNextOutDatas(Start_Index - 1, GetLength() - 1, trip.Id);
            GetZeroLines(outData, trip, mainTrackStructureRepository);

            //Кривой басы аягы
            var Curves = new List<NatureCurves> { };

            StrightAvgTrapezoid = StrightAvg.GetTrapezoid(prevStrightAvgPart, nextStrightAvgPart, 4, ref Curves);
            LevelAvgTrapezoid = LevelAvg.GetTrapezoid(prev50, next50, 10, ref Curves);
            return ShifrovkaGenerated;
        }

        public bool WriteData(
            Trips trip, 
            List<double> prev50, List<double> next50, List<double> prevStrightAvgPart, List<double> nextStrightAvgPart, 
            IMainTrackStructureRepository mainTrackStructureRepository, IRdStructureRepository rdStructureRepository,
            List<Kilometer> KMS = null)
        {
            if (ShifrovkaGenerated)
                return false;

            CultureInfo customCulture = new CultureInfo("en-US");
            Thread.CurrentThread.CurrentCulture = customCulture;
            var _ = "\t";
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            //birinshi
            var outData = rdStructureRepository.GetNextOutDatas(Start_Index - 1, GetLength() - 1, Trip.Id);
            if (this.Number == 707)
                this.Number = this.Number;
            GetZeroLines(outData, Trip, mainTrackStructureRepository);
            //var profileData = rdStructureRepository.GetNextProfileDatas(Start_Index - 1, GetLength() - 1, Trip.Id);


            var outDataTrapezPassport = rdStructureRepository.GetNextOutDatas(Start_Index - 1000, GetLength() + 2000, Trip.Id);

            var prevCount = 0;
            var nextCount = GetLength() - 1;

            var temp = outDataTrapezPassport.Where(o => o.RealCoordinate == outData.First().RealCoordinate).ToList();
            if (temp.Any())
            {
                prevCount = outDataTrapezPassport.IndexOf(temp.First());
            }

            if (KMS == null)
            {
                GetZeroLinesTrapez(outDataTrapezPassport, Trip, mainTrackStructureRepository, prevCount, nextCount);
            }
            else
            {
                GetZeroLinesTrapez(outDataTrapezPassport, Trip, mainTrackStructureRepository, prevCount, nextCount, KMS:KMS);
            }
            


            //Кривой басы аягы
            var Curves = new List<NatureCurves> { };

            StrightAvgTrapezoid = StrightAvg.GetTrapezoid(prevStrightAvgPart, nextStrightAvgPart, 4, ref Curves, naprav: trip.Travel_Direction, strRealData: StrightRight);
            LevelAvgTrapezoid = LevelAvg.GetTrapezoid(prev50, next50, 10, ref Curves, naprav: trip.Travel_Direction);


            //-------------------------------------------------
            //---Трапезойд рихт танбасын рихт нитька береміз---
            //-------------------------------------------------
            for (int i = 0; i < StrightAvgTrapezoid.Count; i++)
            {
                if(StrightAvgTrapezoid[i] != 0)
                {
                    if(i < signTrapez.Count)
                    {
                        signTrapez[i] = Math.Sign(StrightAvgTrapezoid[i]);
                    }
                }
            }
            //-------------------------------------------------
            //-------------------------------------------------
            //-------------------------------------------------


            CurvesNature.AddRange(Curves);

            var fileName = $@"G:\work_shifrovka\km_{Number}_{Track_id}.svgpdat";

            try
            {
                using StreamWriter writetext = new StreamWriter(fileName, false, Encoding.GetEncoding(1251));
                writetext.WriteLine($@"{trip.Id}");
                writetext.WriteLine($@"{trip.Direction_Name}");
                writetext.WriteLine(trip.Direction_id);
                writetext.WriteLine(trip.Chief);
                writetext.WriteLine(Number);



                writetext.WriteLine(Track_name);
                writetext.WriteLine(Track_id);
                writetext.WriteLine(trip.Travel_Direction == Direction.Direct ? "Прямой" : "Обратный");
                writetext.WriteLine(trip.Trip_date.ToString("dd.MM.yyyy  HH:mm"));
                writetext.WriteLine(trip.Car);
                writetext.WriteLine();
                int j = 0;
                int i = 0;
                for (i = trip.Travel_Direction == Direction.Direct ? Start_m : Final_m; i != (trip.Travel_Direction == Direction.Direct ? Final_m : Start_m); i += (int)trip.Travel_Direction)
                {
                    if ((j < Speed.Count) && (j >= 0))
                    {
                        var pointLVL = 0;
                        var pointSTR = 0;

                        foreach (var item in Curves)
                        {
                            var selLvl = item.LevelPoints.Select(o => o).ToList();
                            var selStr = item.StrPoints.Select(o => o).ToList();

                            if (selLvl.Contains(j))
                                pointLVL = 1;
                            if (selStr.Contains(j))
                                pointSTR = 1;
                        }

                        //                      0       1                2              3                   4                         5                  6                 7                             8                  9          |                                      10                                        |                                                  11                       |          12        |                                                   13                            |               14                 15                    16                        17                          18                 19        20            21                  22                      23               24           25
                        writetext.WriteLine($@"{j}{_}{Speed[j]}{_}{(i / 100) + 1}{_}{i % 100}{_}{DrawdownRight[j]:0.00}{_}{DrawdownLeft[j]:0.00}{_}{Gauge[j]:0.00}{_}{StrightRight[j]:0.00}{_}{StrightLeft[j]:0.00}{_}{Level[j]:0.00}{_}{(j < StrightAvgTrapezoid.Count ? StrightAvgTrapezoid[j] : StrightAvg[j]):0.00}{_}{(j < LevelAvgTrapezoid.Count ? LevelAvgTrapezoid[j] : LevelAvg[j]):0.00}{_}{LevelAvg[j]:0.00}{_}{(j < StrightAvgTrapezoid.Count ? StrightAvgTrapezoid[j] : StrightAvg[j]):0.00}{_}{Latitude[j]:0.00}{_}{Longitude[j]:0.00}{_}{SwitchesReal[j]}{_}{LevelCorrection[j]:0.00}{_}{GaugeCorrection[j]:0.00}{_}{0:0.00}{_}{0:0.00}{_}{Corrections[j]}{_}{StrightAvg[j]:0.00}{_}{LevelAvg[j]:0.00}{_}{pointLVL}{_}{pointSTR}{_}{signTrapez[j]}");
                        j++;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Ошибка при формировании уоркшифровки: " + e.Message);
            }
            ShifrovkaGenerated = true;

            var process = System.Diagnostics.Process.Start(@"C:\sntfi\ALARm5\ALARmProcees\Win32\Debug\ALARmProcess.exe", $"{fileName} {Correction}");
            //var process = System.Diagnostics.Process.Start(@"D:\aALARMSVN\ALARm\ALARmProcees\Win32\Debug\ALARmProcess.exe", $"{fileName} {Correction}");
            process.WaitForExit();

            return ShifrovkaGenerated;
        }


        public void CalcRailProfileLines(Trips trip)
        {
            //if (CrossRailProfile == null)
                //return;
         


            var shortwavesLeft = new StringBuilder();
            var shortwavesRight = new StringBuilder();
            var mediumwavesLeft = new StringBuilder();
            var mediumwavesRight = new StringBuilder();
            var longwavesLeft = new StringBuilder();
            var longwavesRight = new StringBuilder();


          
            foreach (var meter in CrossRailProfile.Meters.Where(meter => meter % 1 == 0).ToList())
            {

                int index = CrossRailProfile.Meters.IndexOf(meter);
                var cmeter = trip.Travel_Direction == Direction.Reverse ? meter : Length - meter;
               

                shortwavesLeft.Append((CrossRailProfile.Shortwavesleft[index] * WavesKoef).ToString().Replace(",", ".") + "," + cmeter.ToString().Replace(",", ".") + " ");
                shortwavesRight.Append((CrossRailProfile.Shortwavesright[index] * WavesKoef).ToString().Replace(",", ".") + "," + cmeter.ToString().Replace(",", ".") + " ");
                mediumwavesLeft.Append((CrossRailProfile.Mediumwavesleft[index] * WavesKoef).ToString().Replace(",", ".") + "," + cmeter.ToString().Replace(",", ".") + " ");
                mediumwavesRight.Append((CrossRailProfile.Mediumwavesright[index] * WavesKoef).ToString().Replace(",", ".") + "," + cmeter.ToString().Replace(",", ".") + " ");
                longwavesLeft.Append((CrossRailProfile.Longwavesleft[index] * WavesKoef).ToString().Replace(",", ".") + "," + cmeter.ToString().Replace(",", ".") + " ");
                longwavesRight.Append((CrossRailProfile.Longwavesright[index] * WavesKoef).ToString().Replace(",", ".") + "," + cmeter.ToString().Replace(",", ".") + " ");

            }
        

            ShortwavesLeft = shortwavesLeft.ToString();
            ShortwavesRight = shortwavesRight.ToString();

            MediumwavesLeft = mediumwavesLeft.ToString();
            MediumwavesRight = mediumwavesRight.ToString();
         
            LongwavesLeft = longwavesLeft.ToString();
            LongwavesRight = longwavesRight.ToString();
            
        }

        public string GetdigressionsCount =>
            $"2:{ SecondDegreeCountDrawdown + SecondDegreeCountGauge + SecondDegreeCountLevel + SecondDegreeCountSag + SecondDegreeCountStrightening}; 3:{ThirdDegreeCountDrawdown + ThirdDegreeCountGauge + ThirdDegreeCountLevel + ThirdDegreeCountSag + ThirdDegreeCountStrightening}; 4:{FourthDegreeCountDrawdown + FourthDegreeCountGauge+ FourthDegreeCountLevel + FourthDegreeCountSag + FourthDegreeCountStrightening};";
        public string CalcMainPoint()
        {
            int MainParamPointSum = 0, CurvePointSum = 0;

            foreach (var digression in Digressions)
            {
                var Digression1 = (DigName)digression.DigName;
                if ((Digression1.Value == DigressionName.PsiNear.Value || Digression1.Value == DigressionName.Psi.Value) && digression.LimitSpeedToString() == "-/-")
                {
                    continue;
                }

                if (digression.Digtype == DigressionType.Main)
                {
                    MainParamPointSum += digression.GetPoint(this);
                    //ToDo - ағамен ақылдасу керек
                    CurvePointSum += digression.GetCurvePoint(this);
                }
            }
            return $"Балл - {MainParamPointSum + CurvePointSum} / кривые {CurvePointSum} /";
        }

        public void LoadPasport(IMainTrackStructureRepository mainTrackStructureRepository)
        {
            Speeds = mainTrackStructureRepository.GetMtoObjectsByCoord(Passage_time, Number, MainTrackStructureConst.MtoSpeed, Direction_name, Track_name) as List<Speed>;
            Artificials = mainTrackStructureRepository.GetMtoObjectsByCoord(Passage_time, Number, MainTrackStructureConst.MtoArtificialConstruction, Direction_name, Track_name) as List<ArtificialConstruction>;
            PdbSection = mainTrackStructureRepository.GetMtoObjectsByCoord(Passage_time, Number, MainTrackStructureConst.MtoPdbSection, Direction_name, Track_name) as List<PdbSection>;
            CrossTies = mainTrackStructureRepository.GetMtoObjectsByCoord(Passage_time, Number, MainTrackStructureConst.MtoCrossTie, Direction_name, Track_name) as List<CrossTie>;
            Switches = mainTrackStructureRepository.GetMtoObjectsByCoord(Passage_time, Number, MainTrackStructureConst.MtoSwitch, Direction_name, Track_name) as List<Switch>;
            StraighteningThreads = mainTrackStructureRepository.GetMtoObjectsByCoord(Passage_time, Number, MainTrackStructureConst.MtoStraighteningThread, Direction_name, Track_name) as List<StraighteningThread>;
            LongRailses = mainTrackStructureRepository.GetMtoObjectsByCoord(Passage_time, Number, MainTrackStructureConst.MtoLongRails, Direction_name, Track_name) as List<LongRails>;
            CheckSections = mainTrackStructureRepository.GetMtoObjectsByCoord(Passage_time, Number, MainTrackStructureConst.MtoCheckSection, Direction_name, Track_name) as List<CheckSection>;
            Depths = mainTrackStructureRepository.GetMtoObjectsByCoord(Passage_time, Number, MainTrackStructureConst.MtoDeep, Direction_name, Track_name) as List<Deep>;
            Curves = mainTrackStructureRepository.GetMtoObjectsByCoord(Passage_time, Number, MainTrackStructureConst.MtoCurve, Direction_name, Track_name) as List<Curve>;
            StraighteningThreads = mainTrackStructureRepository.GetMtoObjectsByCoord(Passage_time, Number, MainTrackStructureConst.MtoStraighteningThread, Direction_name, Track_name) as List<StraighteningThread>;
            IsoJoints = mainTrackStructureRepository.GetMtoObjectsByCoord(Passage_time, Number, MainTrackStructureConst.MtoProfileObject, Direction_name, Track_name) as List<ProfileObject>;
            RailsBrace = mainTrackStructureRepository.GetMtoObjectsByCoord(Passage_time, Number, MainTrackStructureConst.MtoRailsBrace, Direction_name, Track_name) as List<RailsBrace>;
        }

        public void LoadTrackPasport(IMainTrackStructureRepository mainTrackStructureRepository, DateTime trip_date)
        {
            Speeds = mainTrackStructureRepository.GetMtoObjectsByCoord(trip_date, Number, MainTrackStructureConst.MtoSpeed, Track_id) as List<Speed>;
            ElevationData = mainTrackStructureRepository.GetMtoObjectsByCoord(trip_date, Number, MainTrackStructureConst.MtoElevation, Track_id) as List<Elevation>;
            Artificials = mainTrackStructureRepository.GetMtoObjectsByCoord(trip_date, Number, MainTrackStructureConst.MtoArtificialConstruction, Track_id) as List<ArtificialConstruction>;
            PdbSection = mainTrackStructureRepository.GetMtoObjectsByCoord(trip_date, Number, MainTrackStructureConst.MtoPdbSection, Track_id) as List<PdbSection>;
            CrossTies = mainTrackStructureRepository.GetMtoObjectsByCoord(trip_date, Number, MainTrackStructureConst.MtoCrossTie, Track_id) as List<CrossTie>;
            if (CrossTies.Count == 0)
            {
                CrossTies.Add(new CrossTie { Crosstie_type_id = (int)CrosTieType.Woody, Start_Km = Number, Start_M = 1, Final_Km = Number, Final_M = Final_m });
            }
            Switches = mainTrackStructureRepository.GetMtoObjectsByCoord(trip_date, Number, MainTrackStructureConst.MtoSwitch, Track_id) as List<Switch>;
            StraighteningThreads = mainTrackStructureRepository.GetMtoObjectsByCoord(trip_date, Number, MainTrackStructureConst.MtoStraighteningThread, Track_id) as List<StraighteningThread>;
            LongRailses = mainTrackStructureRepository.GetMtoObjectsByCoord(trip_date, Number, MainTrackStructureConst.MtoLongRails, Track_id) as List<LongRails>;
            CheckSections = mainTrackStructureRepository.GetMtoObjectsByCoord(trip_date, Number, MainTrackStructureConst.MtoCheckSection, Track_id) as List<CheckSection>;
            Depths = mainTrackStructureRepository.GetMtoObjectsByCoord(trip_date, Number, MainTrackStructureConst.MtoDeep, Track_id) as List<Deep>;
            Curves = mainTrackStructureRepository.GetMtoObjectsByCoord(trip_date, Number, MainTrackStructureConst.MtoCurve, Track_id) as List<Curve>;
            Normas = mainTrackStructureRepository.GetMtoObjectsByCoord(trip_date, Number, MainTrackStructureConst.MtoNormaWidth, Track_id) as List<NormaWidth>;
            IsoJoints = mainTrackStructureRepository.GetMtoObjectsByCoord(trip_date, Number, MainTrackStructureConst.MtoProfileObject, Track_id) as List<ProfileObject>;
            NonstandardKms = mainTrackStructureRepository.GetMtoObjectsByCoord(trip_date, Number, MainTrackStructureConst.MtoNonStandard, Track_id) as List<NonstandardKm>;
            Fragment = mainTrackStructureRepository.GetMtoObjectsByCoord(trip_date, Number, MainTrackStructureConst.Fragments, Track_id) as Fragment;
            Sector = mainTrackStructureRepository.GetSector(Track_id, Number, trip_date);
            StationSection = mainTrackStructureRepository.GetMtoObjectsByCoord(trip_date, Number, MainTrackStructureConst.MtoStationSection, Track_id) as List<StationSection>;
            PdbSection = mainTrackStructureRepository.GetMtoObjectsByCoord(trip_date, Number, MainTrackStructureConst.MtoPdbSection, Track_id) as List<PdbSection>;
            if (Direction_name != null && !Direction_name.Equals(""))
            {
                RailsBrace = mainTrackStructureRepository.GetMtoObjectsByCoord(trip_date, Number, MainTrackStructureConst.MtoRailsBrace, Direction_name, Track_name) as List<RailsBrace>;
                RailSection = mainTrackStructureRepository.GetMtoObjectsByCoord(trip_date, Number, MainTrackStructureConst.MtoRailSection, Direction_name, Track_name) as List<RailsSections>;
            }
            RepairProjects = mainTrackStructureRepository.GetMtoObjectsByCoord(trip_date, Number, MainTrackStructureConst.MtoRepairProject, Track_id) as List<RepairProject>;
            //Генерация пикетов изменения установленной скорости движения
            VPickets = new List<VPicket>();
            if (Speeds != null)
            {
                foreach (var speed1 in Speeds)
                {
                    foreach (var speed2 in Speeds)
                    {
                        if (speed1 != speed2 && speed1.Final_Km == speed2.Start_Km && speed1.Final_Km == Number && speed1.RealFinalCoordinate <= speed2.RealStartCoordinate
                            && Math.Abs(speed1.Final_M - speed2.Start_M) <= 10 && speed1.RealStartCoordinate < speed1.RealFinalCoordinate && speed2.RealStartCoordinate < speed2.RealFinalCoordinate
                            && (speed1.Passenger != speed2.Passenger || speed1.Freight != speed2.Freight) && 0 < speed1.Final_M && speed2.Start_M <= Length)
                        {
                            VPickets.Add(new VPicket()
                            {
                                Meter1 = speed1.Final_M,
                                Meter2 = speed2.Start_M,
                                Picket1 = (speed1.Final_M - 10 > 0 ? speed1.Final_M - 10 : speed1.Final_M).MeterToPicket(),
                                Picket2 = (speed2.Final_M - 10 > 0 ? speed1.Final_M - 10 : speed1.Final_M).MeterToPicket(),
                                Picket1PassengerSpeed = speed1.Passenger,
                                Picket1FreightSpeed = speed1.Freight,
                                Picket2PassengerSpeed = speed2.Passenger,
                                Picket2FreightSpeed = speed2.Freight

                            });
                        }

                    }
                }
            }
            else
            {
                Console.WriteLine("Нет паспортных данных по установленным скоростям");
            }
            CalcGlobalSpeed();
            Pickets = new List<Picket>();
            for (int p = (Start_m / 100) + 1; p <= Final_m / 100; p++)
            {
                Pickets.Add(new Picket() { Number = p, Start = (Start_m / 100) + 1 == p ? Start_m.RoundTo10() : (p - 1) * 100 });
            }




        }

        public void LoadDigresions(IRdStructureRepository rdStructureRepository, IMainTrackStructureRepository mainTrackStructureRepository, Trips trip, bool AdditionalParam = false, string CNI = "")
        {
            if (Digressions == null || !Digressions.Any())
                Digressions = new List<DigressionMark>();


            if (!AdditionalParam)
            {
                foreach (var sw in Switches)
                {
                    if ((sw.Km == Number ) && (sw.Final_Km == Number ) && (sw.Start_Km == Number))
                        Digressions.Add(new DigressionMark() { Meter = sw.Meter, Alert = $"{sw.Meter} Стрелка № {sw.Num} {(sw.Dir_Id == SwitchDirection.Direct ? "ПШ" : "ПРШ")} {(sw.Side_Id == Side.Left ? "Лев." : "Прав.")} {sw.Mark}" });

                }

                foreach (var curve in Curves)
                {
                    if(curve.Straightenings.Count > curve.Elevations.Count)
                    {
                        foreach (var item in curve.Straightenings)
                        {
                            var h = 0;
                            var temp = curve.Elevations.Where(o => item.FirstTransitionEnd.Between(o.RealStartCoordinate, o.RealFinalCoordinate)).ToList();

                            if (temp.Any())
                            {
                                h = (int)temp.First().Lvl;
                            }
                            if (Number == Math.Round(trip.Travel_Direction == Direction.Reverse ? item.FirstTransitionEnd : item.SecondTransitionStart))
                            {
                                Digressions.Add(
                                new DigressionMark()
                                {
                                    Meter = item.Start_M + item.Transition_1 ,
                                    Alert = $"{item.Start_M + item.Transition_1} R:{item.Radius} h:{h} Ш:{item.Width} И:{item.Wear}"
                                });
                            }
                        }                        
                    }
                    else
                    {
                        foreach (var item in curve.Elevations)
                        {
                            var Radius = 0;
                            var Width = 0;
                            var Wear = 0;

                            var temp = curve.Straightenings.Where(o => item.FirstTransitionEnd.Between(o.RealStartCoordinate, o.RealFinalCoordinate)).ToList();

                            if (temp.Any())
                            {
                                Radius = (int)temp.First().Radius;
                                Width = (int)temp.First().Width;
                                Wear = (int)temp.First().Wear;
                            }

                            if (Number == Math.Round(trip.Travel_Direction == Direction.Reverse ? item.FirstTransitionEnd : item.SecondTransitionStart))
                            {
                                Digressions.Add(
                                new DigressionMark()
                                {
                                    Meter = item.Start_M + item.Transition_1 ,
                                    Alert = $"{item.Start_M + item.Transition_1} R:{Radius} h:{item.Lvl} Ш:{Width} И:{Wear}"
                                });
                            }
                        }
                    }

                    //if (curve.Start_Km == Number)
                    //{
                    //    Digressions.Add(new DigressionMark() { Meter = curve.Start_M, Alert = $"{curve.Start_M} R:{curve.Straightenings[0].Radius} h:{curve.Elevations[0].Lvl} Ш:{curve.Straightenings[0].Width} И:{curve.Straightenings[0].Wear}" });
                    //}
                }

                Digressions.AddRange((
                       from vpicket in VPickets
                       select new DigressionMark()
                       {
                           Meter = vpicket.Meter1,
                           Alert = $"{vpicket.Meter2} Уст.ск:{vpicket.Picket2PassengerSpeed}/{vpicket.Picket2FreightSpeed} Уст.ск:{vpicket.Picket1PassengerSpeed}/{vpicket.Picket1FreightSpeed}"
                       }).ToList());

                if (CorrectionMeter > -1)
                {
                    var tempKms= rdStructureRepository.GetKilometersByTrip(trip);
                    var currentKm = tempKms.Where(o => o.Number == Number).ToList();
                    var prevKm = tempKms.Where(o => o.Number == Number -1).ToList();
                    var count = tempKms.Count;

                    var coord = Final_m;
                    if(trip.Travel_Direction == Direction.Reverse)
                    {
                        coord = Start_m;
                        prevKm = tempKms.Where(o => o.Number == Number + 1).ToList();
                    }

                    if ((currentKm.Any() &&  prevKm.Any()) ||( count==1)) ///&& prevKm.Any()
                    {
                        var currentOutData = (List<OutData>)rdStructureRepository.GetNextOutDatas(currentKm.First().Start_Index - 1, currentKm.First().GetLength() - 1, currentKm.First().Trip.Id);
                        var prevOutData = (List<OutData>)rdStructureRepository.GetNextOutDatas(prevKm.First().Start_Index - 1, prevKm.First().GetLength() - 1, prevKm.First().Trip.Id);

                        var currnetM = currentOutData.Max(o => o.meter);
                        var prevM = prevOutData.Max(o => o.meter);


                        var currentMeterNature = currnetM;
                        var currentMeterPassport = currentKm.First().Length;

                        var PrevMeterNature = prevM;
                        var PrevMeterPassport = prevKm.First().Length;

                        var valuecurrent = currentMeterNature - currentMeterPassport;
                        var valueprev = PrevMeterNature - PrevMeterPassport;
                    

                        Digressions.Add(
                           new DigressionMark()
                           {
                               NotMoveAlert = true,
                               Meter = coord,
                               Alert = $"{coord} Привязка координат: {(CorrectionType == CorrectionType.Manual ? "РП" : "АП")} коррекция; начальной привязки на {CorrectionValue}   метр"
                           }) ;
                        var ANIAYRAMOLODEC = rdStructureRepository.InsertCorrection(trip.Id, Track_id, Number, coord, CorrectionValue);

                    }
                }
                
                if (StationSection.Count > 0)
                {
                    if (StationSection[0].Start_Km == Number)
                    {
                        Sector = mainTrackStructureRepository.GetSector(Track_id, Number - 1, trip.Trip_date) ?? "";
                        Digressions.Add(new DigressionMark() { NotMoveAlert = true, Meter = StationSection[0].Start_M, Alert = $"{StationSection[0].Start_M} Станция: {StationSection[0].Station};        {Sector}" });
                    }
                    else
                    if (StationSection[0].Final_Km == Number)
                    {
                        Sector = mainTrackStructureRepository.GetSector(Track_id, Number + 1, trip.Trip_date) ?? "";
                        Digressions.Add(new DigressionMark() { NotMoveAlert = true, Meter = StationSection[0].Final_M, Alert = $"       {Sector};{StationSection[0].Final_M} Станция: {StationSection[0].Station}" });
                    }
                }

                Digressions.AddRange(rdStructureRepository.GetDigressionMarks(trip.Id, Track_id, Number));
            }
            else
            {
                foreach (var curve in Curves)
                {
                    if ( curve.Start_Km== 705)
                    {
                        var x = curve.Start_Km;
                    }
                    if (curve.Start_Km == Number && curve.Final_Km == Number)
                        Digressions.Add(new DigressionMark() { Meter = curve.Start_M, Alert = $"{curve.Start_M} R:{curve.Straightenings[0].Radius} h:{curve.Elevations[0].Lvl} Ш:{curve.Straightenings[0].Width} И:{curve.Straightenings[0].Wear}" });
                }

                int pas = 999, gruz = 999;
            

                foreach (var item in Digressions)
                {
                    if (item.Meter == 386)
                    { 

                    }
                    if (item.Digression == DigressionName.SideWearLeft || item.Digression == DigressionName.SideWearRight)
                    {
                        var c = Curves.Where(o => o.RealStartCoordinate <= Number + item.Meter / 10000.0 && o.RealFinalCoordinate >= Number + item.Meter / 10000.0).ToList();

                        if (c.Any())
                        {
                            item.GetAllowSpeedAddParam(Speeds.First(), c.First().Straightenings[0].Radius, item.Value);

                            if (item.PassengerSpeedLimit != -1 && item.PassengerSpeedLimit < pas)
                            {
                                pas = item.PassengerSpeedLimit;
                            }
                            if (item.FreightSpeedLimit != -1 && item.FreightSpeedLimit < gruz)
                            {
                                gruz = item.FreightSpeedLimit;
                            }
                        }
                    } else if (item.Digression == DigressionName.Gap)
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

                SpeedLimit = $"{(Speeds.First().Passenger <= pas || pas == 999 ? "-/" : $"{pas}/")}{(Speeds.First().Freight <= gruz || gruz == 999 ? "-" : $"{gruz}")}";

                Digressions.AddRange((
                       from vpicket in VPickets
                       select new DigressionMark()
                       {
                           Meter = vpicket.Meter1,
                           Alert = $"{vpicket.Meter2} Уст.ск:{vpicket.Picket2PassengerSpeed}/{vpicket.Picket2FreightSpeed} Уст.ск:{vpicket.Picket1PassengerSpeed}/{vpicket.Picket1FreightSpeed}"
                       }).ToList());
            }
            //Digressions = rdStructureRepository.GetDigressionMarks(Trip.Id, km.Number, km.Track_id, new int[] { 2, 3, 4 });
            //Gaps = mainTrackStructureRepository.GetGaps(Trip.Id, GapSource.Laser, km.Number);

            //Убираем дублирование отступлении
            var newList = new List<DigressionMark> { };

            foreach (var digression in Digressions)
            {
                var isDublicate = newList.Where(o => o.Km == digression.Km && o.Meter == digression.Meter && o.Alert == digression.Alert && o.DigName == digression.DigName).ToList();
                if (isDublicate.Any())
                    continue;
                newList.Add(digression);
            }

            Digressions = newList;

            foreach (var digression in Digressions)
            {
                var picket = Pickets.GetPicket(digression.Meter);
                if (picket != null)
                {
                    if (CNI != "")
                        digression.CNI = CNI;

                    picket.Digression.Add(digression);
                }
            }

            if (!AdditionalParam)
            {
                var bedemost = rdStructureRepository.GetBedemost(trip.Id, Track_id, Number);
                if (bedemost.Count > 0)
                {
                    SpeedLimit = (string)bedemost["limit"];
                    Point = (int)bedemost["ball"];
                }
            }
        }

        public int TrackType { get; set; } = -1;
        public string File_path { get; set; }
        public int Next_Km { get; set; }
        public int Point { get; set; } = -1;
        public string ZeroLevel { get; set; } = "";
        public string ZeroStraightRight { get; set; } = "";
        public string PasportStraightRight { get; set; } = "";
        public string SignalStraightRight { get; set; } = "";
        public string ZeroStraightLeft { get; set; } = "";
        public string PasportStraightLeft { get; set; } = "";
        public string SignalStraightLeft { get; set; } = "";
        public string PasportGauge { get; set; } = "";
        public string SignalDrawdownLeft { get; set; } = "";
        public string SignalDrawdownRight { get; set; } = "";
        public string SideWearLeft { get; set; } = "";
        public string VertWearLeft { get; set; } = "";
        public string VertWearRight { get; set; } = "";
        public string GivenWearLeft { get; set; } = "";
        public string GivenWearRight { get; set; } = "";
        public string TreadTiltLeft { get; set; } = "";
        public string SideWearRight { get; set; } = "";
        public string TreadTiltRight { get; set; } = "";
        //подкулонка
        public string DownHillLeft { get; set; } = "";
        public string DownHillRight { get; set; } = "";
        public string HeadWear45Left { get; set; } = "";
           
        public string HeadWear45Right { get; set; } = "";
        public string MediumwavesLeft { get; set; } = "";
        public string MediumwavesRight { get; set; } = "";
        public string ShortwavesLeft { get; set; } = "";
        public string ShortwavesRight { get; set; } = "";
        public string LongwavesLeft { get; set; } = "";
        public string LongwavesRight { get; set; } = "";

        public int Start_Index { get; set; } = -1;
        public int Final_Index => Start_Index + GetLength();
        public string TrapezLevel { get; set; } = "";
        public string Level1 { get; set; } = "";
        public string Level2 { get; set; } = "";
        public string Level3 { get; set; } = "";
        public string Level4 { get; set; } = "";
        public string Level5 { get; set; } = "";
        public string Stright1 { get; set; } = "";
        public string Stright2 { get; set; } = "";
        public string Stright3 { get; set; } = "";
        public string Stright4 { get; set; } = "";
        public string Stright5 { get; set; } = "";
        public bool Rep_type_cni { get; set; } = false;
        public string SpeedLimit { get; private set; }
        
        public int GlobPassSpeed { get; set; } = -1;
        public int GlobFreightSpeed { get; set; } = -1;
        public int AllowedSpeed { get; private set; }
        public Trips Trip { get; set; }
        public bool IsLowActivity { get {
                if (Speeds != null)
                    foreach (var speed in Speeds)
                    {
                        if (speed.Passenger <= 60)
                        {
                            return true;
                        }

                    }
                return false;
            } }

        public Direction Direction { get; set; }

        public static readonly int DefaultSpeed = 40;
        public static bool RepairProjectFlag = false;
        public static bool GlbSpeedLimited = false;
        

        public void CalcGlobalSpeed()
        {
            int mxVp = 0;
            int mxVf = 0;
            GlobPassSpeed = DefaultSpeed;
            GlobFreightSpeed = DefaultSpeed;
            for (int i = 0; i < meter.Count; i++)
            {
                passSpeed.Add(DefaultSpeed);
                freightSpeed.Add(DefaultSpeed);
                var currentCoord = Number.ToDoubleCoordinate(meter[i]);
                if (Speeds != null)
                    foreach (var speed in Speeds)
                    {
                        if (currentCoord.Between(speed.RealStartCoordinate, speed.RealFinalCoordinate))
                        {
                            passSpeed[i] = speed.Passenger;
                            freightSpeed[i] = speed.Passenger;
                            GlobPassSpeed = speed.Passenger;
                            GlobFreightSpeed = speed.Freight;
                            if (mxVp < GlobPassSpeed)
                                mxVp = GlobPassSpeed;
                            if (mxVf < GlobFreightSpeed)
                                mxVf = GlobFreightSpeed;
                        }
                    }
                if (RepairProjects != null)
                    foreach (var repairProject in RepairProjects)
                    {
                        if (currentCoord.Between(repairProject.RealStartCoordinate, repairProject.RealFinalCoordinate))
                        {
                            passSpeed[i] = repairProject.Speed;
                            freightSpeed[i] = repairProject.Speed;
                            RepairProjectFlag = true;
                        }
                    }
            }
        }

        public override string ToString()
        {
            return $@"{Number} - {Start_m} - {Final_m}";
        }
        public string GetFill()
        {
            return Point switch
            {
                10 => "#33FF3C",
                40 => "blue",
                150 => "yellow",
                500 => "red",
                _ => "black",
            };
        }
        /// <summary>
        /// Километр бағасын қайтару
        /// </summary>
        /// <param name="full">бастыпқы мәні жалған (бағаны қысқартылған түрде қайтарады), толық түрде алу үшін ақиқат мәнін беру қажет</param>
        /// <returns>КМ бағасын мәтіндік түрде қайтарады</returns>
        public string GetGrade(bool full = false)
        {
            if (Point < 0)
                throw new Exception("Не загружены оценочные данные текущего километра");
            return Point switch
            {
                10 => full ? "Отлично" : "О",
                40 => full ? "Хорошо" : "Х",
                150 => full ? "Удовлетворительно" : "У",
                500 => full ? "Неудовлетворительно" : "Н",
                _ => "",
            };
        }
       /// <summary>
       /// паспорттық сызықтар сызу
       /// </summary>
       /// <param name="outdatas"></param>
       /// <param name="trip"></param>
       /// <param name="mainTrackStructureRepository"></param>
       /// <param name="direction">әзірге 1 деп алдық , негізі ойластыру керек</param>
        public void GetZeroLines(List<OutData> outdatas, Trips trip, IMainTrackStructureRepository mainTrackStructureRepository, int direction = 1)
        {
            if ((outdatas == null) || (outdatas.Count < 1))
                return;

            int size = outdatas.Count();
            fsh0 = new List<double>(new double[size]);
            flvl = new List<float>(new float[size]);
            flvl0 = new List<double>(new double[size]);
            fZeroStright = new List<double>(new double[size]);

            frad = new List<double>(new double[size]);
            snorm = new List<int>(new int[size]);
            fnorm = new List<int>(new int[size]);
            sign = new List<int>(new int[size]);
            signTrapez = new List<int>(new int[size]);
            meter = new List<int>(new int[size]);
            int j = 0;
            var distanceFrom = 0.0021;

            LoadTrackPasport(mainTrackStructureRepository, trip.Trip_date);
            for (int i = trip.Travel_Direction == Direction.Direct ? Start_m : Final_m; i != (trip.Travel_Direction == Direction.Direct ? Final_m : Start_m); i += (int)trip.Travel_Direction)
            {
                var rad = 10000.0;
                var utem = 0;
                var yxr01 = 0.0;
                var yxu = 0.0;
                meter[j] = i;
                int lvlsign = 1;

                try
                {

                    outdatas[j] = outdatas[j];
                    fsh0[j] = 1520;
                    snorm[j] = 1520;
                    fnorm[j] = 1520;
                    sign[j] = -1;
                    signTrapez[j] = -1;

                    var currentCoordReal = Number.ToDoubleCoordinate(i);
                    foreach (var stritheningThread in StraighteningThreads)
                    {
                        if (stritheningThread.RealStartCoordinate <= currentCoordReal && currentCoordReal <= stritheningThread.RealFinalCoordinate)
                        {
                            sign[j] = stritheningThread.Side_Id == (int)Side.Right ? 1 : -1;
                            signTrapez[j] = stritheningThread.Side_Id == (int)Side.Right ? 1 : -1;
                        }
                    }

                    foreach (var norma in Normas)
                    {
                        if ((currentCoordReal >= norma.RealStartCoordinate) && (currentCoordReal <= norma.RealFinalCoordinate))
                        {
                            fnorm[j] = norma.Norma_Width;
                            snorm[j] = norma.Norma_Width;
                        }
                    }


                    var nrm = snorm[j];
                    if ((1524 < snorm[j]) && (snorm[j] <= 1529))
                        nrm = 1524;
                    double zxu = nrm;
                    if (this.Number == 725)
                        Number = Number;
                    foreach (var curve in Curves)
                    {
                        foreach (var stright in curve.Straightenings)
                        {

                            if (stright.Transition_2 == 0)
                            {
                                var ssss = 1;
                            }
                            if (currentCoordReal.Between(stright.RealStartCoordinate, stright.RealFinalCoordinate))
                            {
                                fnorm[j] = stright.Width;
                                rad = stright.Radius;
                            }

                            if (currentCoordReal.Between(stright.RealStartCoordinate - distanceFrom, stright.RealFinalCoordinate + distanceFrom))
                            {
                                utem = 1;
                            }
                            if (currentCoordReal.Between(stright.RealStartCoordinate, stright.FirstTransitionEnd))
                            {


                                var KmMeter = stright.RealStartCoordinate.RealCoordinateToKmMeter();

                                var len1 = Math.Abs(mainTrackStructureRepository.GetDistanceBetween2Coord(Number, i, KmMeter[0], KmMeter[1], Track_id, trip.Trip_date));
                              //  sign[j] = curve.Side_id == 2 ? 1 : -1;
                               // sign[j] = curve.Side_id ==2 1 : -1;
                               sign[j] =StrightAvg.GetSign(Number, this.Meters, stright.RealStartCoordinate, stright.RealFinalCoordinate);
                                signTrapez[j] = StrightAvg.GetSign(Number, this.Meters, stright.RealStartCoordinate, stright.RealFinalCoordinate);

                                var gg1 = stright.Transition_1 != 0 ? stright.Transition_1 + 0.0 : 30.0;
                                var oldYxr = yxr01;
                                yxr01 = (stright.Radius.RadiusToAvg() / gg1) * len1;
                                var ist = curve.Straightenings.IndexOf(stright);
                                if (ist > 0)
                                {
                                    if ((curve.Straightenings[ist - 1].Transition_2 == 0) && (stright.Radius.RadiusToAvg() > curve.Straightenings[ist - 1].Radius.RadiusToAvg()))
                                    {
                                        yxr01 = ((stright.Radius.RadiusToAvg() - curve.Straightenings[ist - 1].Radius.RadiusToAvg()) / gg1) * len1;
                                        yxr01 += curve.Straightenings[ist - 1].Radius.RadiusToAvg();
                                    }
                                }

                                var fsrs = Math.Abs(stright.Width - snorm[j]);
                                zxu = snorm[j] + (fsrs / gg1) * len1;
                            }
                            else
                            if (currentCoordReal.Between(stright.FirstTransitionEnd, stright.SecondTransitionStart))
                            {
                                //sign[j] = curve.Side_id == 2 ? -1 : 1;
                                sign[j] =StrightAvg.GetSign(Number, this.Meters, stright.RealStartCoordinate, stright.RealFinalCoordinate);
                                //signTrapez[j] = StrightAvg.GetSign(Number, this.Meters, stright.RealStartCoordinate, stright.RealFinalCoordinate);

                                yxr01 = stright.Radius.RadiusToAvg();
                                zxu = stright.Width;
                            }
                            else
                            if (currentCoordReal.Between(stright.SecondTransitionStart, stright.RealFinalCoordinate))
                            {
                                //sign[j] = curve.Side_id == 2 ? -1 : 1;

                                sign[j] = StrightAvg.GetSign(Number, this.Meters, stright.RealStartCoordinate, stright.RealFinalCoordinate);
                                signTrapez[j] = StrightAvg.GetSign(Number, this.Meters, stright.RealStartCoordinate, stright.RealFinalCoordinate);

                                var gg2 = stright.Transition_2 != 0 ? stright.Transition_2 + 0.0 : 30.0;


                                var len2 = Math.Abs(mainTrackStructureRepository.GetDistanceBetween2Coord(Number, i, stright.Final_Km, stright.Final_M, Track_id, trip.Trip_date));
                                yxr01 = (stright.Radius.RadiusToAvg() / gg2) * len2;

                                var fsrs = Math.Abs(stright.Width - snorm[j]);
                                zxu = snorm[j] + (fsrs / gg2) * len2;
                            }
                        }

                        foreach (var elevation in curve.Elevations)
                        {
                            if (currentCoordReal.Between(elevation.RealStartCoordinate - distanceFrom, elevation.RealFinalCoordinate + distanceFrom))
                            {
                                utem = 1;
                            }

                            //бірінші өту қиысығы
                            if (currentCoordReal.Between(elevation.RealStartCoordinate, elevation.FirstTransitionEnd))
                            {
                                var KmMeter = elevation.RealStartCoordinate.RealCoordinateToKmMeter();
                                var len1 = Math.Abs(mainTrackStructureRepository.GetDistanceBetween2Coord(Number, i, KmMeter[0], KmMeter[1], Track_id, trip.Trip_date));
                                lvlsign = StrightAvg.GetSign(Number, this.Meters, elevation.RealStartCoordinate, elevation.RealFinalCoordinate);
                                var gg1 = elevation.Transition_1 != 0 ? elevation.Transition_1 + 0.0 : 30.0;
                                yxu = (elevation.Lvl / gg1) * len1;
                                var ist = curve.Elevations.IndexOf(elevation);
                                if (ist > 0)
                                {
                                    if ((curve.Elevations[ist - 1].Transition_2 == 0) && (elevation.Lvl > curve.Elevations[ist - 1].Lvl))
                                    {
                                        yxu = ((elevation.Lvl - curve.Elevations[ist - 1].Lvl) / gg1) * len1;
                                        yxu += curve.Elevations[ist - 1].Lvl;
                                    }
                                }
                            }
                            else
                            if (currentCoordReal.Between(elevation.FirstTransitionEnd, elevation.SecondTransitionStart))
                            {
                                lvlsign =StrightAvg.GetSign(Number, this.Meters, elevation.RealStartCoordinate, elevation.RealFinalCoordinate);
                                yxu = elevation.Lvl;
                            }
                            else
                            if (currentCoordReal.Between(elevation.SecondTransitionStart, elevation.RealFinalCoordinate))
                            {
                                lvlsign = StrightAvg.GetSign(Number, this.Meters, elevation.RealStartCoordinate, elevation.RealFinalCoordinate);
                                var gg2 = elevation.Transition_2 != 0 ? elevation.Transition_2 + 0.0 : 30.0;
                                var len2 = Math.Abs(mainTrackStructureRepository.GetDistanceBetween2Coord(Number, i, elevation.Final_Km, elevation.Final_M, Track_id, trip.Trip_date));
                                yxu = (elevation.Lvl / gg2) * len2;

                            }
                        }
                    }
                    foreach (var sw in Switches)
                    {
                        if (currentCoordReal.Between(sw.RealStartCoordinate, sw.RealFinalCoordinate))
                        {
                            var inCurve = false;
                            foreach (var curve in Curves)
                            {
                                foreach (var st in curve.Straightenings)
                                {
                                    if (((sw.Mark.Contains("1/22") && st.Radius < 1500) || (!sw.Mark.Contains("1/22") && st.Radius < 1000)) && (currentCoordReal.Between(st.RealStartCoordinate, st.RealFinalCoordinate)))
                                    {
                                        inCurve = true;
                                    }
                                }
                            }
                            if (!inCurve)
                            {
                                sign[j] = sw.Side_Id == Side.Right ? 1 : -1;
                                signTrapez[j] = sw.Side_Id == Side.Right ? 1 : -1;
                            }

                        }
                    }

                   flvl[j] = utem;
                   flvl0[j] = lvlsign * yxu;
                   fZeroStright[j] = sign[j] * yxr01;
                    fsh0[j] = zxu;
                    frad[j] = Math.Round(rad);
                    //    var tempSign = StrightAvg[j] / Math.Abs(StrightAvg[j]) * direction;
                    //PasportLevel += $"{-1 * Math.Abs(yxu) * tempSign:0.00},{j} ";

                    PasportGauge += $"{(fsh0[j] - 1520) * GaugeKoef:0.00},{j} ";
                    //PasportStraightLeft += $"{-1 * Math.Abs(yxr01) * tempSign * StrightKoef:0.00},{j} ";


                    j++;
                }

                catch (Exception e)
                {
                    Console.WriteLine("Kilometer.GetZeroLines Error: " + e.Message);
                }
            }
            for (int i = 0; i < fsh0.Count - 1; i++)
            {
                var d = (int)Math.Abs(fsh0[i] - fsh0[i + 1]);
                if (d >= 4)
                {
                    if ((fsh0[i] > fsh0[i + 1]) && (Math.Abs(fsh0[i] - fsh0[i + 1]) >= 4))
                    {
                        int k = i - 20;
                        if (i - 20 < 0)
                            k = i;
                        var s1 = 1524.0;
                        for (int ji = k; ji <= i + 1; ji++)
                        {
                            s1 -= 4 / 20;
                            fsh0[ji] = s1;
                        }
                    }
                    if ((fsh0[i] < fsh0[i + 1]) && (Math.Abs(fsh0[i] - fsh0[i + 1]) >= 4))
                    {
                        int k = i + 21;
                        if (i + 21 > fsh0.Count - 1)
                            k = fsh0.Count - 1;
                        var s1 = 1520.0;
                        for (int ji = k; ji <= i + 1; ji++)
                        {
                            s1 += 4 / 20;
                            fsh0[ji] = s1;
                        }
                    }
                }
            }
        }
        public void GetZeroLinesTrapez(List<OutData> outdatas, Trips trip, IMainTrackStructureRepository mainTrackStructureRepository, int prevCount, int nextCount, List<Kilometer> KMS = null)
        {


            //fZeroStright - данные пасспорта рихтовки
            //flvl0 - данные пасспорта уровня
            if (this.Number == 725)
                Number = Number;
            if ((outdatas == null) || (outdatas.Count < 1))
                return;

            int size = nextCount;

            var riht = fZeroStright;
            var lvl = flvl0;

            fZeroStright = new List<double>(new double[size]);
            flvl0 = new List<double>(new double[size]);

            //LoadTrackPasport(mainTrackStructureRepository, trip.Trip_date);

            //Возвышение 6мм
            try
            {
                foreach (var ElData in ElevationData)
                {
                    ElData.Level_Id = -1 * ElData.Level_Id;

                    var ListX_Vozv6mm = new List<int> { };
                    var ListY_Vozv6mm = new List<double> { };

                    if (trip.Travel_Direction == Direction.Direct)
                    {
                        ListX_Vozv6mm.AddRange(new List<int>
                        {
                            getIndex(outdatas, ElData.RealStartCoordinate, ElData.Start_Km),
                            getIndex(outdatas, ElData.RealStartCoordinate + 10/10000.0, ElData.Start_Km),
                            getIndex(outdatas, ElData.RealFinalCoordinate - 10/10000.0, ElData.Final_Km),
                            getIndex(outdatas, ElData.RealFinalCoordinate, ElData.Final_Km),
                        });

                        ListY_Vozv6mm.AddRange(new List<double>
                        {
                            0.0,
                            ElData.Level_Id,
                            ElData.Level_Id,
                            0.0,
                        });
                    }
                    else
                    {
                        ListX_Vozv6mm = new List<int>
                        {
                            getIndex(outdatas, ElData.RealFinalCoordinate, ElData.Final_Km),
                            getIndex(outdatas, ElData.Final_Km + (ElData.Final_M - 10) / 10000.0, ElData.Final_Km),
                            getIndex(outdatas, ElData.Start_Km + (ElData.Start_M + 10) / 10000.0, ElData.Start_Km),
                            getIndex(outdatas, ElData.RealStartCoordinate, ElData.Start_Km),
                        };

                        ListY_Vozv6mm = new List<double>
                        {
                            0.0,
                            ElData.Level_Id,
                            ElData.Level_Id,
                            0.0,
                        };
                    }
                    try
                    {
                        var ModifiedCurveSrt = new List<double>();

                        //интерполяция кривой                
                        for (int t = 0; t < ListY_Vozv6mm.Count() - 1; t++)
                        {
                            for (int c = 0; c < ListX_Vozv6mm[t + 1] - ListX_Vozv6mm[t]; c++)
                            {
                                if (ListX_Vozv6mm[t + 1] - ListX_Vozv6mm[t] == 0)
                                    ListX_Vozv6mm[t + 1] = ListX_Vozv6mm[t + 1];

                                double bottom_dx1 = ListX_Vozv6mm[t + 1] - ListX_Vozv6mm[t];
                                double y2 = ListY_Vozv6mm[t + 1];
                                double y1 = ListY_Vozv6mm[t];
                                var linearY = (y2 - y1) / bottom_dx1 * c + y1;
                                ModifiedCurveSrt.Add(linearY);
                            }
                        }

                        var InternalIndex = 0;
                        //егер кривойдын жартысы текущии километр алдында калып койса
                        var prevCurveDataDiv = 0;
                        var prevCurveDataDivStartkm = 0;
                        var prevCurveDataDivFinalkm = 0;
                        int j = 0;
                        for (int i = trip.Travel_Direction == Direction.Direct ? Start_m : Final_m; i != (trip.Travel_Direction == Direction.Direct ? Final_m : Start_m); i += (int)trip.Travel_Direction)
                        {
                            var currentCoordReal = Number.ToDoubleCoordinate(i);
                            try
                            {
                                if (currentCoordReal.Between(ElData.RealStartCoordinate, ElData.RealFinalCoordinate))
                                {
                                    if (prevCurveDataDiv == 0)
                                    {
                                        if (trip.Travel_Direction == Direction.Direct)
                                        {
                                            var curCoord = getIndex(outdatas, currentCoordReal, 0);
                                            var realStartCoord = getIndex(outdatas, ElData.RealStartCoordinate, 0);

                                            if (realStartCoord < curCoord)
                                            {
                                                prevCurveDataDiv = curCoord - realStartCoord;
                                            }
                                        }
                                        else
                                        {
                                            var curCoord = getIndex(outdatas, currentCoordReal, 0);
                                            var realStartCoord = getIndex(outdatas, ElData.RealFinalCoordinate, 0);

                                            if (realStartCoord < curCoord)
                                            {
                                                prevCurveDataDiv = curCoord - realStartCoord;

                                            }
                                        }
                                    }

                                    flvl0[j] = ModifiedCurveSrt[InternalIndex + prevCurveDataDiv];

                                    InternalIndex++;
                                }
                            }
                            catch (Exception)
                            {
                                //Console.WriteLine("Ошибка при записи возв 6мм ");
                            }

                            j++;
                        }
                    }
                    catch (Exception)
                    {
                        //Console.WriteLine("Ошибка интерполяция точек кривой возв 6мм");
                    }
                }
            }
            catch (Exception e)
            {
                //Console.WriteLine($"Возвышение 6мм ====> ошибка {e.Message}");
            }

            foreach (var curve in Curves)
            {
                var ListX_str = new List<int> { };
                var ListY_str = new List<double> { };

                var ListX_lvl = new List<int> { };
                var ListY_lvl = new List<double> { };


                //Рихтовка
                var startCurve = -1.0;

                foreach (var stright in curve.Straightenings)
                {
                    
                    if (trip.Travel_Direction == Direction.Direct)
                    {
                        
                        var tempX = new List<int>
                        //ListX_str.AddRange(new List<int>
                        {
                            getIndex(outdatas, stright.RealStartCoordinate, stright.Start_Km),
                            getIndex(outdatas, stright.FirstTransitionEnd, stright.Start_Km),
                            getIndex(outdatas, stright.SecondTransitionStart, stright.Final_Km),
                            getIndex(outdatas, stright.RealFinalCoordinate, stright.Final_Km),
                        };

                        ListX_str.AddRange(tempX);

                        var strH = 17860 / stright.Radius;
                        if (this.Number == 707)
                            Number = Number;
                        var tempY = new List<double>
                        {
                            0.0,
                            strH,
                            strH,
                            0.0
                        };

                        ListY_str.AddRange(tempY);

                        startCurve = strH;
                    }
                    else
                    {
                        var tempX = new List<int>
                        {
                            getIndex(outdatas, stright.RealFinalCoordinate, stright.Final_Km),
                            getIndex(outdatas, stright.SecondTransitionStart, stright.Final_Km),
                            getIndex(outdatas, stright.FirstTransitionEnd, stright.Start_Km),
                            getIndex(outdatas, stright.RealStartCoordinate, stright.Start_Km),
                        };
                        tempX.AddRange(ListX_str);
                        ListX_str = tempX;

                        var strH = 17860 / stright.Radius;

                        var tempY = new List<double>
                        {
                            0.0,
                            strH,
                            strH,
                            0.0
                        };

                        tempY.AddRange(ListY_str);
                        ListY_str = tempY;

                        startCurve = strH;
                    }
                }


                var vremX_str = new List<int> { };
                var vremY_str = new List<double> { };

                vremY_str.Add(ListY_str.First());
                vremX_str.Add(ListX_str.First());

                for (int i = 1; i < ListY_str.Count - 1; i++)
                {
                    if (ListY_str[i] == 0 && ListY_str[i + 1] == 0 || ListY_str[i - 1] == 0 && ListY_str[i] == 0)
                    {
                    }
                    else
                    {
                        vremY_str.Add(ListY_str[i]);
                        vremX_str.Add(ListX_str[i]);
                    }
                }
                vremY_str.Add(ListY_str.Last());
                vremX_str.Add(ListX_str.Last());

                ListY_str = vremY_str;
                ListX_str = vremX_str;
                var ModifiedCurveSrtF = new List<double>();
                var fZeroStrightF = new List<double>();
                var prevCurveDataDivF = 0;
                try
                {
                    var ModifiedCurveSrt = new List<double>();

                    //интерполяция кривой                
                    for (int t = 0; t < ListY_str.Count() - 1; t++)
                    {
                        for (int c = 0; c < ListX_str[t + 1] - ListX_str[t]; c++)
                        {
                            if (ListX_str[t + 1] - ListX_str[t] == 0)
                                ListX_str[t + 1] = ListX_str[t + 1];

                            double bottom_dx1 = ListX_str[t + 1] - ListX_str[t];
                            double y2 = ListY_str[t + 1];
                            double y1 = ListY_str[t];
                            var linearY = (y2 - y1) / bottom_dx1 * c + y1;
                            ModifiedCurveSrt.Add(linearY);
                        }
                    }
                    ModifiedCurveSrtF = ModifiedCurveSrt;
                    //если счет км обратный
                    //if(trip.Travel_Direction == Direction.Reverse)
                    //{
                    //    ModifiedCurveSrt.Reverse();
                    //}

                    var InternalIndex = 0;

                    //егер кривойдын жартысы текущии километр алдында калып койса
                    var prevCurveDataDiv = 0;
                    if (this.Number == 704 || this.Number == 705)
                    { 
                        Number = Number;
                    }
                    int j = 0;
                    for (int i = trip.Travel_Direction == Direction.Direct ? Start_m : Final_m; i != (trip.Travel_Direction == Direction.Direct ? Final_m : Start_m); i += (int)trip.Travel_Direction)
                    {
                        try
                        {
                            var currentCoordReal = Number.ToDoubleCoordinate(i);

                            if (currentCoordReal.Between(curve.RealStartCoordinate, curve.RealFinalCoordinate - (trip.Travel_Direction == Direction.Direct ? (1.0 / 10000.0) : 0)))
                            {
                                if (prevCurveDataDiv == 0)
                                {
                                    if (trip.Travel_Direction == Direction.Direct)
                                    {
                                        var curCoord = getIndex(outdatas, currentCoordReal, 0);
                                        var realStartCoord = getIndex(outdatas, curve.RealStartCoordinate, 0);

                                        if (realStartCoord < curCoord)
                                        {
                                            prevCurveDataDiv = curCoord - realStartCoord;
                                        }
                                    }
                                    else
                                    {
                                        var curCoord = getIndex(outdatas, currentCoordReal, 0);
                                        var realStartCoord = getIndex(outdatas, curve.RealFinalCoordinate, 0);

                                        if (realStartCoord < curCoord)
                                        {
                                            prevCurveDataDiv = curCoord - realStartCoord;
                                        }
                                    }
                                }

                                if (InternalIndex > 800)
                                {
                                    InternalIndex = InternalIndex;
                                }
                                if ((ModifiedCurveSrt.Count() > InternalIndex + prevCurveDataDiv + 2) && (InternalIndex + prevCurveDataDiv > 0)  &&(j< sign.Count ) &&(j< fZeroStright.Count) )
                                {
                                    fZeroStright[j] = sign[j] * Math.Abs(ModifiedCurveSrt[InternalIndex + prevCurveDataDiv]);

                                    InternalIndex++;
                                }


                            }
                            prevCurveDataDivF = InternalIndex;





                        }
                        catch (Exception)
                        {


                            Console.WriteLine("Ошибка при записи пасспорт Рихтовка " + " InternalIndex");
                        }
                        j++;
                    }

                }
                catch (Exception)
                {
                    Console.WriteLine("Ошибка интерполяция точек кривой Рихтовка");
                }
            

                var countzero = 0;
                var valstr = 0.0;
                var perstr = 0.0;
                var x_perstr = 0;
                var flag = true;
                if (trip.Travel_Direction == Direction.Reverse)///&& trip.Travel_Direction == Direction.Direct
                {
                    int i_start = 0;
                    for (int ii = 0; ii < fZeroStright.Count; ii++)
                    {


                        if (Math.Abs(fZeroStright[ii]) < 0.1 && flag)
                        {
                            countzero++;
                            if (ii - i_start > 2) { countzero = 0; }

                            i_start = ii;
                        }
                        else flag = false;


                    }
                    fZeroStrightF = fZeroStright;
                    if (countzero > 150)
                    {
                        countzero = 0;

                    }

                    for (int ii1 = 0; (ii1 < countzero + 1) && (countzero < fZeroStright.Count - 3); ii1++)

                    {
                        var signdiv = Math.Sign(fZeroStright[countzero + 1] - fZeroStright[countzero + 3]);

                        fZeroStrightF[countzero - ii1] = fZeroStright[countzero + 1] + signdiv * 0.25 * ii1 * Math.Abs(fZeroStright[countzero + 1] - fZeroStright[0]) / (countzero + 1);// - sign[countzero - ii1] * ii1 * (fZeroStright[countzero+1] - fZeroStright[0]) / (countzero + 1)


                    }


                    fZeroStright = fZeroStrightF;



                }

                if (trip.Travel_Direction == Direction.Direct)///&& trip.Travel_Direction == Direction.Direct
                {
                    int i_start = 0;
                    for (int ii = 0; ii < fZeroStright.Count; ii++)
                    {


                        if (Math.Abs(fZeroStright[ii]) < 0.1 && flag)
                        {
                            countzero++;
                            if (ii - i_start > 2) { countzero = 0; }


                            i_start = ii;
                            if (countzero > 150)
                            {
                                countzero = 0;

                            }

                        }
                        else flag = false;



                    }
                    fZeroStrightF = fZeroStright;
                    // if (countzero > fZeroStright.Count-3)
                    // { continue; }
                    for (int ii1 = 0; (ii1 < countzero + 1) && ii1 < fZeroStright.Count && (countzero < fZeroStright.Count - 3); ii1++)

                    {
                        var signdiv = Math.Sign(fZeroStright[countzero + 1] - fZeroStright[countzero + 3]);

                        fZeroStrightF[countzero - ii1] = fZeroStright[countzero + 1] + signdiv * 0.25 * ii1 * Math.Abs(fZeroStright[countzero + 1] - fZeroStright[0]) / (countzero + 1);
                    }


                    fZeroStright = fZeroStrightF;



                }
                             //fZeroStright[j] = sign[j] * Math.Abs(ModifiedCurveSrt[InternalIndex + prevCurveDataDiv]);
                //Уровень
                var startLvl = -1.0;
                foreach (var elevations in curve.Elevations)
                {
                    if (trip.Travel_Direction == Direction.Direct)
                    {
                        var tempX = new List<int>
                        //ListX_lvl.AddRange(new List<int>
                        {
                            getIndex(outdatas, elevations.RealStartCoordinate, elevations.Start_Km),
                            getIndex(outdatas, elevations.FirstTransitionEnd, elevations.Start_Km),
                            getIndex(outdatas, elevations.SecondTransitionStart, elevations.Final_Km),
                            getIndex(outdatas, elevations.RealFinalCoordinate, elevations.Final_Km),
                        };

                        ListX_lvl.AddRange(tempX);

                        var strH = elevations.Lvl;

                        var tempY = new List<double>
                        {
                            0.0,
                            strH,
                            strH,
                            0.0,
                        };

                        ListY_lvl.AddRange(tempY);

                        startCurve = strH;
                    }
                    else
                    {
                        var tempX = new List<int>
                        {
                            getIndex(outdatas, elevations.RealFinalCoordinate, elevations.Final_Km),
                            getIndex(outdatas, elevations.SecondTransitionStart, elevations.Final_Km),
                            getIndex(outdatas, elevations.FirstTransitionEnd, elevations.Start_Km),
                            getIndex(outdatas, elevations.RealStartCoordinate, elevations.Start_Km),
                        };

                        tempX.AddRange(ListX_lvl);
                        ListX_lvl = tempX;

                        var strH = elevations.Lvl;

                        var tempY = new List<double>
                        {
                            0.0,
                            strH,
                            strH,
                            0.0,
                        };

                        tempY.AddRange(ListY_lvl);
                        ListY_lvl = tempY;

                        startCurve = strH;
                    }
                }

                var vremX_lvl = new List<int> { };
                var vremY_lvl = new List<double> { };

                vremY_lvl.Add(ListY_lvl.First());
                vremX_lvl.Add(ListX_lvl.First());

                for (int i = 1; i < ListY_lvl.Count - 1; i++)
                {
                    if (ListY_lvl[i] == 0 && ListY_lvl[i + 1] == 0 || ListY_lvl[i - 1] == 0 && ListY_lvl[i] == 0)
                    {
                    }
                    else
                    {
                        vremY_lvl.Add(ListY_lvl[i]);
                        vremX_lvl.Add(ListX_lvl[i]);
                    }
                }
                vremY_lvl.Add(ListY_lvl.Last());
                vremX_lvl.Add(ListX_lvl.Last());

                ListY_lvl = vremY_lvl;
                ListX_lvl = vremX_lvl;

                try
                {
                    var ModifiedCurveLvl = new List<double>();

                    //интерполяция кривой                
                    for (int t = 0; t < ListY_lvl.Count() - 1; t++)
                    {
                        for (int c = 0; c < ListX_lvl[t + 1] - ListX_lvl[t]; c++)
                        {
                            double bottom_dx1 = ListX_lvl[t + 1] - ListX_lvl[t];
                            double y2 = ListY_lvl[t + 1];
                            double y1 = ListY_lvl[t];
                            var linearY = (y2 - y1) / bottom_dx1 * c + y1;
                            ModifiedCurveLvl.Add(linearY);
                        }
                    }
                    //если счет км обратный
                    //if (trip.Travel_Direction == Direction.Reverse)
                    //{
                    //    ModifiedCurveLvl.Reverse();
                    //}

                    var InternalIndex = 0;
                    //егер кривойдын жартысы текущии километр алдында калып койса
                    var prevCurveDataDiv = 0;

                    int j = 0;
                    for (int i = trip.Travel_Direction == Direction.Direct ? Start_m : Final_m; i != (trip.Travel_Direction == Direction.Direct ? Final_m : Start_m); i += (int)trip.Travel_Direction)
                    {
                        try
                        {
                            var currentCoordReal = Number.ToDoubleCoordinate(i);

                            if (currentCoordReal.Between(curve.RealStartCoordinate, curve.RealFinalCoordinate - (trip.Travel_Direction == Direction.Direct ? (1.0 / 10000.0) : 0)))
                            {
                                if (prevCurveDataDiv == 0)
                                {
                                    if (trip.Travel_Direction == Direction.Direct)
                                    {
                                        var curCoord = getIndex(outdatas, currentCoordReal, 0);
                                        var realStartCoord = getIndex(outdatas, curve.RealStartCoordinate, 0);

                                        if (realStartCoord < curCoord)
                                        {
                                            prevCurveDataDiv = curCoord - realStartCoord;
                                        }
                                    }
                                    else
                                    {
                                        var curCoord = getIndex(outdatas, currentCoordReal, 0);
                                        var realStartCoord = getIndex(outdatas, curve.RealFinalCoordinate, 0);
                                        if (realStartCoord < curCoord)

                                        {
                                            prevCurveDataDiv = curCoord - realStartCoord;
                                        }
                                    }
                                }
                                
                                    if ((ModifiedCurveLvl.Count() > InternalIndex + prevCurveDataDiv + 2) && (InternalIndex + prevCurveDataDiv > 0) && (j < sign.Count) && (j < flvl0.Count))
                                {
                                    flvl0[j] = sign[j] * Math.Abs(ModifiedCurveLvl[InternalIndex + prevCurveDataDiv]);
                                    InternalIndex++;
                                }
                            }
                            j++;
                        }
                        catch (Exception)
                        {
                            //Console.WriteLine("Ошибка при записи пасспорт трапез уровень");
                        }
                    }
                }
                catch (Exception)
                {
                    //Console.WriteLine("Ошибка интерполяция точек кривой уровень");

                }

                ///////////////////////////
                ///




                //var flvl0F = new List<double> { };
                //flvl0F = flvl0;
                //countzero = 0;
                //valstr = 0.0;
                //perstr = 0.0;
                //x_perstr = 0;
                //flag = true;
                //if (trip.Travel_Direction == Direction.Reverse)///&& trip.Travel_Direction == Direction.Direct
                //{
                //    int i_start = 0;
                //    for (int ii = 0; ii < flvl0.Count; ii++)
                //    {


                //        if (Math.Abs(flvl0[ii]) < 0.1 && flag)
                //        {
                //            countzero++;
                //            if (ii - i_start > 2) { countzero = 0; }

                //            i_start = ii;
                //        }
                //        else flag = false;


                //    }

                //    if (countzero > 150)
                //    {
                //        countzero = 0;

                //    }

                //    for (int ii1 = 0; (ii1 < countzero + 1) && (countzero < flvl0.Count - 3); ii1++)

                //    {
                //        var signdiv = Math.Sign(flvl0[countzero + 1] - fZeroStright[countzero + 3]);

                //        flvl0F[countzero - ii1] = flvl0[countzero + 1] + signdiv * 0.25 * ii1 * Math.Abs(flvl0[countzero + 1] - flvl0[0]) / (countzero + 1);// - sign[countzero - ii1] * ii1 * (fZeroStright[countzero+1] - fZeroStright[0]) / (countzero + 1)


                //    }


                //}

                //flvl0F = flvl0;
                //countzero = 0;
                //valstr = 0.0;
                //perstr = 0.0;
                //x_perstr = 0;
                //flag = true;
                //if (trip.Travel_Direction == Direction.Direct)///&& trip.Travel_Direction == Direction.Direct
                //{
                //    int i_start = 0;
                //    for (int ii = 0; ii < flvl0.Count; ii++)
                //    {


                //        if (Math.Abs(flvl0[ii]) < 0.1 && flag)
                //        {
                //            countzero++;
                //            if (ii - i_start > 2) { countzero = 0; }

                //            i_start = ii;
                //        }
                //        else flag = false;


                //    }

                //    if (countzero > 150)
                //    {
                //        countzero = 0;

                //    }

                //    for (int ii1 = 0; (ii1 < countzero + 1) && (countzero < flvl0.Count - 3); ii1++)

                //    {
                //        var signdiv = Math.Sign(flvl0[countzero + 1] - fZeroStright[countzero + 3]);

                //        flvl0F[countzero - ii1] = flvl0[countzero + 1] + signdiv * 0.25 * ii1 * Math.Abs(flvl0[countzero + 1] - flvl0[0]) / (countzero + 1);// - sign[countzero - ii1] * ii1 * (fZeroStright[countzero+1] - fZeroStright[0]) / (countzero + 1)


                //    }


                //}









                ////



















                PasportLevel = "";
                PasportStraightLeft = "";
                int direction = -1;



                for (int j = 0; j < flvl0.Count; j++)
                {
                    PasportLevel += $"{(flvl0[j]) },{j} ";
                    PasportStraightLeft += $"{(fZeroStright[j]) },{j} ";

                    j++;
                }
            }

            
        }

        private int getIndex(List<OutData> outdatas, double dCoord, int km)
        {
            var tempData = outdatas.Where(o => o.RealCoordinate == dCoord).ToList();

            if (tempData.Any())
            {
                var ind = tempData.Last().x;

                //var prevKm = Math.Round(dCoord);
                //if (Number != prevKm)
                //{
                //    if(Trip.Travel_Direction == Direction.Reverse)
                //    {
                //        if (Number > prevKm)
                //        {
                //            var minData = outdatas.Where(o => o.km == prevKm).ToList();
                //        }
                //        else
                //        {

                //        }
                //    }                    
                //}

                return ind;
            }
            else
            {
                var temp = outdatas.Where(o => o.km == km).ToList();

                if (dCoord > temp.Last().RealCoordinate)
                {
                    var d = (dCoord - temp.Last().RealCoordinate)*10000;
                    var t = temp.Last().x + (int)d;
                    return t;
                }
                else
                {
                    var t = temp.First().x;
                    return t;
                }
                Console.WriteLine("Не удалость найти индекс кривой " + dCoord);
                return 0;
            }
        }

        /// <summary>
        /// Километрдің бағасын қайта есептеу
        /// </summary>
        public void CalcPoint() { 
            string Point500Reason = string.Empty;
            int secondDegreeCommonCount = 0, thirdDegreeWithoutGaugeCount =0, thirdDegreeGaugeCount =0, fourthDegreeConmmonCount = 0, secondDegreeDrawdownSagStright =0;
            foreach (var digression in Digressions)
            {
                // ескертпе метрінің индексі
                int mIndex = meter.IndexOf(digression.Meter);
                if (mIndex > -1)
                {
                    if (digression.PassengerSpeedLimit.Between(0, passSpeed[mIndex] - 1) || digression.FreightSpeedLimit.Between(0, freightSpeed[mIndex] - 1))
                        GlbSpeedLimited = true;
                }
                switch (digression.Digression.Name) {
                    case string digname when digname == DigressionName.Strightening.Name
                    || digname == DigressionName.DrawdownLeft.Name
                    || digname == DigressionName.DrawdownRight.Name
                    || digname == DigressionName.Sag.Name:
                        if (digression.Degree == 3)
                            thirdDegreeWithoutGaugeCount += digression.GetCount();
                        else if (digression.Degree == 2)
                        {
                            secondDegreeDrawdownSagStright += 1;
                            secondDegreeCommonCount += digression.GetCount();
                        }
                        else if (digression.Degree == 4)
                            fourthDegreeConmmonCount += digression.GetCount();
                        break;
                    case string digname when digname == DigressionName.Level.Name:
                        if (digression.Degree == 3)
                            thirdDegreeWithoutGaugeCount += digression.GetCount();
                        else if (digression.Degree == 2)
                            secondDegreeCommonCount += digression.GetCount();
                        else if (digression.Degree == 4)
                            fourthDegreeConmmonCount += digression.GetCount();
                        break;
                    case string digname when digname == DigressionName.Broadening.Name
                    || digname == DigressionName.Constriction.Name:
                        if (digression.Degree == 3)
                            thirdDegreeGaugeCount += digression.GetCount();
                        else if (digression.Degree == 2)
                            secondDegreeCommonCount += digression.GetCount();
                        else if (digression.Degree == 4)
                            fourthDegreeConmmonCount += digression.GetCount();
                        break;
                }
            }
            if (GlobPassSpeed <= 60)
            {
                if (secondDegreeCommonCount <= 3)
                    Point = 10;
                if (secondDegreeCommonCount.Between(4, 12))
                    Point = 40;
                if ((secondDegreeCommonCount > 12) || thirdDegreeWithoutGaugeCount.Between(1, 3) || thirdDegreeGaugeCount.Between(1, 10))
                    Point = 150;
                if (thirdDegreeWithoutGaugeCount > 3 || thirdDegreeGaugeCount > 10)
                    Point = 500;
                if (thirdDegreeWithoutGaugeCount > 3)
                    Point500Reason = "3cт";
                if (fourthDegreeConmmonCount > 0)
                {
                    Point500Reason = "4cт";
                    Point = 500;
                }
               
            }
            else
            {
                if (secondDegreeCommonCount <= 5)
                    Point = 10;
                if (secondDegreeCommonCount.Between(6, 25))
                    Point = 40;
                if (secondDegreeCommonCount.Between(26,100) || secondDegreeDrawdownSagStright.Between(26,60) || thirdDegreeWithoutGaugeCount.Between(1, 6) || thirdDegreeGaugeCount.Between(1, 10))
                    Point = 150;
                if (thirdDegreeWithoutGaugeCount > 6) {
                    Point = 500;
                    Point500Reason = "3cт V=60";
                }
                if (thirdDegreeGaugeCount > 10) {
                    Point500Reason = "Ш10 V=60";
                    Point = 500;
                }

                if (fourthDegreeConmmonCount > 0)
                {
                    Point500Reason = "4cт";
                    Point = 500;
                }
            }
            if (RepairProjectFlag)
                switch (Point)
                {
                    case 500:
                        Point = 150;
                        break;
                    case 150:
                        Point = 40;
                        break;
                    case 40:
                        Point = 10;
                        break;
                }
            if (GlbSpeedLimited)
                Point = 500;
        }
    }
    public static class MyList {

        public static string CutFromIndex(this string str, string value, int cutIndex)
        {
            if (String.IsNullOrEmpty(value))
                throw new ArgumentException("the string to find may not be empty", "value");
            var foundCount = 0;
            for (int index = 0; ; index += value.Length)
            {
                index = str.IndexOf(value, index);
                if (index == -1)
                    return str;
                foundCount++;
                if (foundCount == cutIndex)
                    return str.Remove(index, str.Length - index);
                
            }
        }

        public static int GetSign(this List<double> list, int km, List<int> meter, double a, double b)
        {
            var result = 1.0;

            var abs = 0.0;
            for (int i = 0; i < list.Count(); i++)
            {
                var currCoord = km.ToDoubleCoordinate(meter[i]);
                if (currCoord.Between(a,b))
                {
                    if (Math.Abs(list[i]) > abs)
                    {
                        abs = Math.Abs(list[i]);
                        result = list[i] / abs;
                    }
                }
            }
            return (int)result;
        }
        

    }
}

