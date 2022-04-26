using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using ALARm.Core.Report;

namespace ALARm.Core.AdditionalParameteres
{
    public class CrossRailProfile
    {
        public int NKm { get; set; }
        public Direction TravelDirection { get; set; }

        public List<float> Meters { get; set; } =  new List<float>();
		public List<Int16> Meter { get; set; }
        //Вертикальный износ
        public List<float> VertIznosL { get; set; }
        public List<float> VertIznosR { get; set; }
        public List<float> GarbL { get; set; }
        public List<float> GarbR { get; set; }
        //боковой износ
        public List<float> SideWearLeft { get; set; }
        public List<float> SideWearRight { get; set; }
        //Наклон поверхности катания
        public List<float> TreadTiltLeft { get; set; }
        public List<float> TreadTiltRight { get; set; }
        public List<float> GarbTreadTiltLeft { get; set; }
        public List<float> GarbTreadTiltRight { get; set; }
        //приведенный износ
        public List<float> ReducedWearLeft { get; set; }
        public List<float> ReducedWearRight { get; set; }
        //износ головки 45 градус
        public List<float> HeadWearLeft { get; set; }
        public List<float> HeadWearRight { get; set; }
        //подулконка
        public List<float> DownhillLeft { get; set; }
        public List<float> DownhillRight { get; set; }
        public List<float> GarbDownhillLeft { get; set; }
        public List<float> GarbDownhillRight { get; set; }


        public List<float> Longwavesleft { get; set; }
        public List<float> Longwavesright { get; set; }
        public List<float> Mediumwavesleft { get; set; }
        public List<float> Mediumwavesright { get; set; }
        public List<float> Shortwavesleft { get; set; }
        public List<float> Shortwavesright { get; set; }
        public List<float> XBigleft { get; set; }
        public List<float> Xbig { get; set; }

        public List<float> ImpulsLeft{ get; set; }
        public List<float> ImpulsRight { get; set; }
        public List<float> Kmimp{ get; set; }
        public List<float> Meterimp { get; set; }
        public List<string> Impthreat { get; set; }

        private float CurrentSm = 0;
        private int prevMetr = -1;
        public class Heat : RdObject
        {
            public int Value { get; set; }
        }
        public void ParseDB(CrosProf elem, Kilometer kilometer = null)
        {
            int meter = elem.Meter;

            if (prevMetr != meter)
            {
                prevMetr = meter;
                CurrentSm = 1.0f;
            }
            

            Meters.Add(meter - 1.0f + CurrentSm);
            //нпк
            TreadTiltLeft.Add(elem.Npk_l);
            TreadTiltRight.Add(elem.Npk_r);
            //пу
            //DownhillLeft.Add(elem.Pu_l * 100);
            //DownhillRight.Add(elem.Pu_r * 100);
            ////боковой износ
            //SideWearLeft.Add(elem.Bok_l * 100);
            //SideWearRight.Add(elem.Bok_r * 100);
            ////верт износ
            //VertIznosL.Add(elem.Vert_l * 100);
            //VertIznosR.Add(elem.Vert_r * 100);
            DownhillLeft.Add(elem.Pu_l );
            DownhillRight.Add(elem.Pu_r );
            //боковой износ
            SideWearLeft.Add(elem.Bok_l );
            SideWearRight.Add(elem.Bok_r);
            //верт износ
            VertIznosL.Add(elem.Vert_l );
            VertIznosR.Add(elem.Vert_r );
            //приведенный износ
            var leftRed = (elem.Vert_l + elem.Bok_l / 2) * 1f;
            var rRed = (elem.Vert_r + elem.Bok_r / 2) * 1f;
            ReducedWearLeft.Add( leftRed );
            ReducedWearRight.Add(rRed );
            //износ головки 45 градус
            HeadWearLeft.Add(elem.Iz_45_l );
            HeadWearRight.Add(elem.Iz_45_r );

            Longwavesleft.Add(elem.Longwavesleft );
            Longwavesright.Add(elem.Longwavesright);
            Mediumwavesleft.Add(elem.Mediumwavesleft );
            Mediumwavesright.Add(elem.Mediumwavesright );
            Shortwavesleft.Add(elem.Shortwavesleft );
            Shortwavesright.Add(elem.Shortwavesright );

            //XBigleft.Add(elem.X_big_l);
            //XBigright.Add(elem.X_big_r);
            Xbig.Add((elem.X_big_l - elem.X_big_r)/10);
            CurrentSm -= 0.2f;
            if (kilometer == null)
                return;
            //kilometer.CrossRailProfile.Meter.Last() += 1;

            kilometer.TreadTiltLeft += (GetDIstanceFrom1div60(1 / ((float)elem.Npk_l))).ToString().Replace(",", ".") + "," + kilometer.CrossRailProfile.Meters.Count.ToString() + " ";
            kilometer.TreadTiltRight += (GetDIstanceFrom1div60(1 / ((float)elem.Npk_r))).ToString().Replace(",", ".") + "," + kilometer.CrossRailProfile.Meters.Count.ToString().Replace(",", ".") + " ";
            kilometer.DownHillLeft += (GetDIstanceFrom1div60(1 / ((float)elem.Pu_l))).ToString().Replace(",", ".") + "," + kilometer.CrossRailProfile.Meters.Count.ToString() + " ";
            kilometer.DownHillRight += (GetDIstanceFrom1div60(1 / ((float)elem.Pu_r))).ToString().Replace(",", ".") + "," + kilometer.CrossRailProfile.Meters.Count.ToString().Replace(",", ".") + " ";
            
            kilometer.VertWearLeft += (iznosKoefVert * elem.Vert_l ).ToString().Replace(",", ".") + "," + kilometer.CrossRailProfile.Meters.Count.ToString().Replace(",", ".") + " ";
            kilometer.VertWearRight += (iznosKoefVert * elem.Vert_r ).ToString().Replace(",", ".") + "," + kilometer.CrossRailProfile.Meters.Count.ToString().Replace(",", ".") + " ";
            
            kilometer.SideWearLeft += ((iznosKoef * (elem.Bok_l ))).ToString().Replace(",", ".") + "," + kilometer.CrossRailProfile.Meters.Count.ToString().Replace(",", ".") + " ";
            kilometer.SideWearRight += ((iznosKoef * (elem.Bok_r))).ToString().Replace(",", ".") + "," + kilometer.CrossRailProfile.Meters.Count.ToString().Replace(",", ".") + " ";

            kilometer.GivenWearLeft += ((iznosKoefRedused * leftRed)).ToString().Replace(",", ".") + "," + kilometer.CrossRailProfile.Meters.Count.ToString().Replace(",", ".") + " ";
            kilometer.GivenWearRight += ((iznosKoefRedused * rRed)).ToString().Replace(",", ".") + "," + kilometer.CrossRailProfile.Meters.Count.ToString().Replace(",", ".") + " ";

            kilometer.HeadWear45Left += (iznosKoef45 * elem.Iz_45_l).ToString().Replace(",", ".") + "," + kilometer.CrossRailProfile.Meters.Count.ToString().Replace(",", ".") + " ";
            kilometer.HeadWear45Right += (iznosKoef45 * elem.Iz_45_r).ToString().Replace(",", ".") + "," + kilometer.CrossRailProfile.Meters.Count.ToString().Replace(",", ".") + " ";
            //Gauge.Add(float.Parse(parameters[6], CultureInfo.InvariantCulture.NumberFormat));
            
        }
        public void ParseDB_for_impulses(CrosProf elem, Kilometer kilometer = null)
        {
            int meter = elem.Meter;

            if (prevMetr != meter)
            {
                prevMetr = meter;
                CurrentSm = 1.0f;
            }


            Meters.Add(meter - 1.0f + CurrentSm);
            //нпк
         

         
            CurrentSm -= 0.2f;
            if (kilometer == null)
                return;
            //kilometer.CrossRailProfile.Meter.Last() += 1;

          
            //Gauge.Add(float.Parse(parameters[6], CultureInfo.InvariantCulture.NumberFormat));

        }

        public void ParseDBList(List<CrosProf> elemList, Kilometer kilometer = null)
        {
            foreach(var elem in elemList)
            {
                int meter = elem.Meter;

                if (prevMetr != meter)
                {
                    prevMetr = meter;
                    CurrentSm = 1.0f;
                }


                Meters.Add(meter - 1.0f + CurrentSm);
                //нпк
                TreadTiltLeft.Add(elem.Npk_l );
                TreadTiltRight.Add(elem.Npk_r);
                //пу
                DownhillLeft.Add(elem.Pu_l );
                DownhillRight.Add(elem.Pu_r);
                //боковой износ
                SideWearLeft.Add(elem.Bok_l);
                SideWearRight.Add(elem.Bok_r);
                //верт износ
                VertIznosL.Add(elem.Vert_l);
                VertIznosR.Add(elem.Vert_r);
                //приведенный износ
                var leftRed = (elem.Vert_l + elem.Bok_l / 2) ;
                var rRed = (elem.Vert_r + elem.Bok_r / 2) ;
                ReducedWearLeft.Add(leftRed);
                ReducedWearRight.Add(rRed);
                //износ головки 45 градус
                HeadWearLeft.Add(elem.Iz_45_l);
                HeadWearRight.Add(elem.Iz_45_r);

                Longwavesleft.Add(elem.Longwavesleft);
                Longwavesright.Add(elem.Longwavesright);
                Mediumwavesleft.Add(elem.Mediumwavesleft);
                Mediumwavesright.Add(elem.Mediumwavesright);
                Shortwavesleft.Add(elem.Shortwavesleft);
                Shortwavesright.Add(elem.Shortwavesright);

                
                Impthreat.Add(elem.Impthreat);
                Meterimp.Add(elem.Meterimp);
                Kmimp.Add(elem.Kmimp);
                CurrentSm -= 0.2f;
                if (kilometer == null)
                    return;
                //kilometer.CrossRailProfile.Meter.Last() += 1;

                kilometer.TreadTiltLeft += (GetDIstanceFrom1div60(1 / ((float)elem.Npk_l ))).ToString().Replace(",", ".") + "," + kilometer.CrossRailProfile.Meters.Count.ToString() + " ";
                kilometer.TreadTiltRight += (GetDIstanceFrom1div60(1 / ((float)elem.Npk_r ))).ToString().Replace(",", ".") + "," + kilometer.CrossRailProfile.Meters.Count.ToString().Replace(",", ".") + " ";
                kilometer.DownHillLeft += (GetDIstanceFrom1div60(1 / ((float)elem.Pu_l))).ToString().Replace(",", ".") + "," + kilometer.CrossRailProfile.Meters.Count.ToString() + " ";
                kilometer.DownHillRight += (GetDIstanceFrom1div60(1 / ((float)elem.Pu_r ))).ToString().Replace(",", ".") + "," + kilometer.CrossRailProfile.Meters.Count.ToString().Replace(",", ".") + " ";

                kilometer.VertWearLeft += (iznosKoefVert * elem.Vert_l ).ToString().Replace(",", ".") + "," + kilometer.CrossRailProfile.Meters.Count.ToString().Replace(",", ".") + " ";
                kilometer.VertWearRight += (iznosKoefVert * elem.Vert_r ).ToString().Replace(",", ".") + "," + kilometer.CrossRailProfile.Meters.Count.ToString().Replace(",", ".") + " ";

                kilometer.SideWearLeft += ((iznosKoef * (elem.Bok_l ))).ToString().Replace(",", ".") + "," + kilometer.CrossRailProfile.Meters.Count.ToString().Replace(",", ".") + " ";
                kilometer.SideWearRight += ((iznosKoef * (elem.Bok_r ))).ToString().Replace(",", ".") + "," + kilometer.CrossRailProfile.Meters.Count.ToString().Replace(",", ".") + " ";

                kilometer.GivenWearLeft += ((iznosKoefRedused * leftRed)).ToString().Replace(",", ".") + "," + kilometer.CrossRailProfile.Meters.Count.ToString().Replace(",", ".") + " ";
                kilometer.GivenWearRight += ((iznosKoefRedused * rRed)).ToString().Replace(",", ".") + "," + kilometer.CrossRailProfile.Meters.Count.ToString().Replace(",", ".") + " ";

                kilometer.HeadWear45Left += (iznosKoef45 * elem.Iz_45_l).ToString().Replace(",", ".") + "," + kilometer.CrossRailProfile.Meters.Count.ToString().Replace(",", ".") + " ";
                kilometer.HeadWear45Right += (iznosKoef45 * elem.Iz_45_r).ToString().Replace(",", ".") + "," + kilometer.CrossRailProfile.Meters.Count.ToString().Replace(",", ".") + " ";
                //Gauge.Add(float.Parse(parameters[6], CultureInfo.InvariantCulture.NumberFormat));
            }

        }
        public int widthInPixel = 622;
        public float widthImMM = 155;
        public int xBegin = 145;
        private float angleRuleWidth = 70f;
        private float iznosKoef = 70 / 20f;
        private float iznosKoefVert = 70 / 13f;
        private float iznosKoefRedused = 70 / 16f;
        private float iznosKoef45 = 70 / 18f;
        private float GetDIstanceFrom1div60(float x)
        {
            var koef = (angleRuleWidth / (1 / 12f - 1 / 60f));
            var value = 1 / x - 1 / 60f;

            return koef * value;
        }
        public string MMToPixelChartString(float mm)
        {
            return (widthInPixel / widthImMM * mm + xBegin).ToString().Replace(",", ".");
        }

        public void ParseTextLine(string line)
        {
            
            line = Regex.Replace(line, @"\s+", " ");
            var parameters = line.Split(new char[] { ' ', '\t' });
            {
                
                int meter = (int.Parse(parameters[0]) - 1) * 100 + int.Parse(parameters[1]);
                if (prevMetr != meter)
                {
                    prevMetr = meter;
                    CurrentSm = TravelDirection == Direction.Reverse ? 0 : 0.8f;
                }

                Meters.Add(meter - 1 + CurrentSm);
                TreadTiltLeft.Add(1 / 20 + float.Parse(parameters[4], CultureInfo.InvariantCulture.NumberFormat) / 4f);
                DownhillLeft.Add(1 / 20 + float.Parse(parameters[5], CultureInfo.InvariantCulture.NumberFormat) / 4f);
                TreadTiltRight.Add(1 / 20 + float.Parse(parameters[12], CultureInfo.InvariantCulture.NumberFormat) / 4f);
                DownhillRight.Add(1 / 20 + float.Parse(parameters[13], CultureInfo.InvariantCulture.NumberFormat) / 4f);

                //боковой износ
                SideWearRight.Add(float.Parse(parameters[15], CultureInfo.InvariantCulture.NumberFormat) * 1.3f);
                SideWearLeft.Add(float.Parse(parameters[7], CultureInfo.InvariantCulture.NumberFormat) * 1.3f);

                //TreadTiltLeft.Add(float.Parse(parameters[4], CultureInfo.InvariantCulture.NumberFormat) / 3f);
                //DownhillLeft.Add(float.Parse(parameters[5], CultureInfo.InvariantCulture.NumberFormat) / 3.5f);
                //TreadTiltRight.Add(float.Parse(parameters[12], CultureInfo.InvariantCulture.NumberFormat));
                //DownhillRight.Add(float.Parse(parameters[13], CultureInfo.InvariantCulture.NumberFormat));

                ////боковой износ
                //SideWearRight.Add(float.Parse(parameters[15], CultureInfo.InvariantCulture.NumberFormat));
                //SideWearLeft.Add(float.Parse(parameters[7], CultureInfo.InvariantCulture.NumberFormat));

                //верт износ
                VertIznosR.Add(float.Parse(parameters[14], CultureInfo.InvariantCulture.NumberFormat));
                VertIznosL.Add(float.Parse(parameters[6], CultureInfo.InvariantCulture.NumberFormat));
                //приведенный износ
                ReducedWearRight.Add(float.Parse(parameters[17], CultureInfo.InvariantCulture.NumberFormat));
                ReducedWearLeft.Add(float.Parse(parameters[9], CultureInfo.InvariantCulture.NumberFormat));
                //износ головки 45 градус
                HeadWearRight.Add(float.Parse(parameters[16], CultureInfo.InvariantCulture.NumberFormat));
                HeadWearLeft.Add(float.Parse(parameters[8], CultureInfo.InvariantCulture.NumberFormat));
                
                //Gauge.Add(float.Parse(parameters[6], CultureInfo.InvariantCulture.NumberFormat));
                CurrentSm += 0.20f*(int)TravelDirection;
                
            }
        }

        public CrossRailProfile()
        {
            Meters = new List<float>();
            VertIznosL = new List<float>();
            VertIznosR = new List<float>();
            GarbL = new List<float>();
            GarbR = new List<float>();
            SideWearLeft = new List<float>();
            SideWearRight = new List<float>();
            TreadTiltLeft = new List<float>();
            TreadTiltRight =  new List<float>();
            GarbTreadTiltLeft = new List<float>();
            GarbTreadTiltRight = new List<float>();

            DownhillLeft = new List<float>();
            DownhillRight = new List<float>();

            ReducedWearLeft = new List<float>();
            ReducedWearRight = new List<float>();

            HeadWearLeft = new List<float>();
            HeadWearRight = new List<float>();

            GarbDownhillLeft = new List<float>();
            GarbDownhillRight = new List<float>();

            Longwavesleft = new List<float>();
            Longwavesright = new List<float>();
            Mediumwavesleft = new List<float>();
            Mediumwavesright = new List<float>();
            Shortwavesleft = new List<float>();
            Shortwavesright = new List<float>();

            Xbig = new List<float>();
          


            CurrentSm = -1;
        }

        /// <summary>
        /// TODO
        /// Возможно надо переделать чтобы на остряке не учитывались боковой износ со стороны остряка стрелочного перевода
        /// И обе подуклонки на остряке
        /// </summary>
        /// <returns></returns>
        public List<Digression> GetDigressions()
        {
            var result = new List<Digression>();
            //Ип л              
            bool RWfound3Left = false;
            bool RWfound2Left = false;
            //Ип пр
            bool RWfound3Right = false;
            bool RWfound2Right = false;
            //Ив л              
            bool VIfound3Left = false;
            bool VIfound2Left = false;
            //Ив пр
            bool VIfound3Right = false;
            bool VIfound2Right = false;
            //Иб л
            bool SWfound4Left = false;
            bool SWfound3Left = false;
            bool SWfound2Left = false;
            //Иб пр
            bool SWfound4Right = false;
            bool SWfound3Right = false;
            bool SWfound2Right = false;
            //Нпк
            bool found4Left = false;
            bool found4Right = false;
            bool found3Left = false;
            bool found3Right = false;
            //4
            float OneDiv11 = 1 / 11f;
            float OneDiv61 = 1 / 61f;
            //3
            float OneDiv12 = 1 / 12f;
            float OneDiv13 = 1 / 13f;
            float OneDiv44 = 1 / 44f;
            float OneDiv60 = 1 / 60f;

            //ПУ
            bool PUfound4Left = false;
            bool PUfound4Right = false;
            bool PUfound3Left = false;
            bool PUfound3Right = false;
            float PUOneDiv12 = 1 / 12f;
            float PUOneDiv13 = 1 / 13f;

            //Ип л
            Digression RWdig3Left = null;
            Digression RWdig2Left = null;
            //Ип пр
            Digression RWdig3Right = null;
            Digression RWdig2Right = null;
            //Ив л
            Digression VIdig3Left = null;
            Digression VIdig2Left = null;
            //Ив пр
            Digression VIdig3Right = null;
            Digression VIdig2Right = null;
            //Иб л
            Digression SWdig4Left = null;
            Digression SWdig3Left = null;
            Digression SWdig2Left = null;
            //Иб пр
            Digression SWdig4Right = null;
            Digression SWdig3Right = null;
            Digression SWdig2Right = null;
            //Нпк                     
            Digression dig4Left = null;
            Digression dig4Right = null;
            Digression dig3Left = null;
            Digression dig3Right = null;
            //ПУ                     
            Digression PUdig4Left = null;
            Digression PUdig4Right = null;
            Digression PUdig3Left = null;
            Digression PUdig3Right = null;

            float dig4LeftValue = 0, dig4RightValue = 0,
                  dig3LeftValue = 0, dig3RightValue = 0,
                  //ПУ
                  PUdig4LeftValue = 0, PUdig4RightValue = 0,
                  PUdig3LeftValue = 0, PUdig3RightValue = 0,

                  SWdig4LeftValue = 0, SWdig3LeftValue = 0, SWdig2LeftValue = 0,
                  SWdig4RightValue = 0, SWdig3RightValue = 0, SWdig2RightValue = 0,
                  VIdig3LeftValue = 0, VIdig2LeftValue = 0,
                  VIdig3RightValue = 0, VIdig2RightValue = 0,
                  RWdig3LeftValue = 0, RWdig2LeftValue = 0,
                  RWdig3RightValue = 0, RWdig2RightValue = 0;


            
            for (int i = 0; i < Meters.Count; i++)
            {
                //3 степени Ип Прав
                if (ReducedWearRight[i] > 14)
                {
                    if (!RWfound3Right)
                    {
                        RWdig3Right = new Digression();
                        RWdig3Right.DigName = DigressionName.ReducedWearRight;
                        RWdig3Right.Km = NKm;
                        RWdig3Right.Meter = (int)Meters[i];
                        RWfound3Right = true;
                        RWdig3Right.Typ = 3;
                        if ((RWdig3RightValue > ReducedWearRight[i]) || (RWdig3RightValue == 0))
                            RWdig3RightValue = ReducedWearRight[i];
                    }
                    else
                    {
                        if (RWdig3RightValue < ReducedWearRight[i])
                            RWdig3RightValue = ReducedWearRight[i];
                    }
                }

                if ((ReducedWearRight[i] <= 14) && (RWfound3Right))
                {
                    RWdig3Right.Length = Math.Abs(RWdig3Right.Meter - (int)Meters[i]);
                    RWdig3Right.Kmetr = (int)Meters[i];
                    RWdig3Right.Value = RWdig3RightValue;
                    RWdig3RightValue = 0;
                    result.Add(RWdig3Right);
                    RWfound3Right = false;
                }
                //2 степени Ип прав
                if ((ReducedWearRight[i] <= 14) && (ReducedWearRight[i] > 8))
                {
                    if (!RWfound2Right)
                    {
                        RWdig2Right = new Digression();
                        RWdig2Right.DigName = DigressionName.ReducedWearRight;
                        RWdig2Right.Km = NKm;
                        RWdig2Right.Meter = (int)Meters[i];
                        RWfound2Right = true;
                        RWdig2Right.Typ = 2;
                        if ((RWdig2RightValue > ReducedWearRight[i]) || (RWdig2RightValue == 0))
                            RWdig2RightValue = ReducedWearRight[i];
                    }
                    else
                    {
                        if (RWdig2RightValue < ReducedWearRight[i])
                            RWdig2RightValue = ReducedWearRight[i];
                    }
                }
                else
                if (RWfound2Right)
                {
                    RWdig2Right.Length = Math.Abs(RWdig2Right.Meter - (int)Meters[i]);
                    RWdig2Right.Kmetr = (int)Meters[i];
                    RWdig2Right.Value = RWdig2RightValue;
                    RWdig2RightValue = 0;
                    result.Add(RWdig2Right);
                    RWfound2Right = false;
                }
                //3 степени Ип лев
                if (ReducedWearLeft[i] > 14)
                {
                    if (!RWfound3Left)
                    {
                        RWdig3Left = new Digression();
                        RWdig3Left.DigName = DigressionName.ReducedWearLeft;
                        RWdig3Left.Km = NKm;
                        RWdig3Left.Meter = (int)Meters[i];
                        RWfound3Left = true;
                        RWdig3Left.Typ = 3;
                        if ((RWdig3LeftValue > ReducedWearLeft[i]) || (RWdig3LeftValue == 0))
                            RWdig3LeftValue = ReducedWearLeft[i];
                    }
                    else
                    {
                        if (RWdig3LeftValue < ReducedWearLeft[i])
                            RWdig3LeftValue = ReducedWearLeft[i];
                    }
                }

                if ((ReducedWearLeft[i] <= 14) && (RWfound3Left))
                {
                    RWdig3Left.Length = Math.Abs(RWdig3Left.Meter - (int)Meters[i]);
                    RWdig3Left.Kmetr = (int)Meters[i];
                    RWdig3Left.Value = RWdig3LeftValue;
                    RWdig3LeftValue = 0;
                    result.Add(RWdig3Left);
                    RWfound3Left = false;
                }
                //2 степени Ип лев
                if ((ReducedWearLeft[i] <= 14) && (ReducedWearLeft[i] > 8))
                {
                    if (!RWfound2Left)
                    {
                        RWdig2Left = new Digression();
                        RWdig2Left.DigName = DigressionName.ReducedWearLeft;
                        RWdig2Left.Km = NKm;
                        RWdig2Left.Meter = (int)Meters[i];
                        RWfound2Left = true;
                        RWdig2Left.Typ = 2;
                        if ((RWdig2LeftValue > ReducedWearLeft[i]) || (RWdig2LeftValue == 0))
                            RWdig2LeftValue = ReducedWearLeft[i];
                    }
                    else
                    {
                        if (RWdig2LeftValue < ReducedWearLeft[i])
                            RWdig2LeftValue = ReducedWearLeft[i];
                    }
                }
                else
                if (RWfound2Left)
                {
                    RWdig2Left.Length = Math.Abs(RWdig2Left.Meter - (int)Meters[i]);
                    RWdig2Left.Kmetr = (int)Meters[i];
                    RWdig2Left.Value = RWdig2LeftValue;
                    RWdig2LeftValue = 0;
                    result.Add(RWdig2Left);
                    RWfound2Left = false;
                }
                //3 степени Ив лев
                if (VertIznosL[i] > 12)
                {
                    if (!VIfound3Left)
                    {
                        VIdig3Left = new Digression();
                        VIdig3Left.DigName = DigressionName.VertIznosL;
                        VIdig3Left.Km = NKm;
                        VIdig3Left.Meter = (int)Meters[i];
                        VIfound3Left = true;
                        VIdig3Left.Typ = 3;
                        if ((VIdig3LeftValue > VertIznosL[i]) || (VIdig3LeftValue == 0))
                            VIdig3LeftValue = VertIznosL[i];
                    }
                    else
                    {
                        if (VIdig3LeftValue < VertIznosL[i])
                            VIdig3LeftValue = VertIznosL[i];
                    }
                }

                if ((VertIznosL[i] <= 12) && (VIfound3Left))
                {
                    VIdig3Left.Length = Math.Abs(VIdig3Left.Meter - (int)Meters[i]);
                    VIdig3Left.Kmetr = (int)Meters[i];
                    VIdig3Left.Value = VIdig3LeftValue;
                    VIdig3LeftValue = 0;
                    result.Add(VIdig3Left);
                    VIfound3Left = false;
                }
                //2 степени Ив лев
                if ((VertIznosL[i] <= 12) && (VertIznosL[i] > 4))
                {
                    if (!VIfound2Left)
                    {
                        VIdig2Left = new Digression();
                        VIdig2Left.DigName = DigressionName.VertIznosL;
                        VIdig2Left.Km = NKm;
                        VIdig2Left.Meter = (int)Meters[i];
                        VIfound2Left = true;
                        VIdig2Left.Typ = 2;
                        if ((VIdig2LeftValue > VertIznosL[i]) || (VIdig2LeftValue == 0))
                            VIdig2LeftValue = VertIznosL[i];
                    }
                    else
                    {
                        if (VIdig2LeftValue < VertIznosL[i])
                            VIdig2LeftValue = VertIznosL[i];
                    }
                }
                else
                if (VIfound2Left)
                {
                    VIdig2Left.Length = Math.Abs(VIdig2Left.Meter - (int)Meters[i]);
                    VIdig2Left.Kmetr = (int)Meters[i];
                    VIdig2Left.Value = VIdig2LeftValue;
                    VIdig2LeftValue = 0;
                    result.Add(VIdig2Left);
                    VIfound2Left = false;
                }
                //3 степени Ив прав
                if (VertIznosR[i] > 12)
                {
                    if (!VIfound3Right)
                    {
                        VIdig3Right = new Digression();
                        VIdig3Right.DigName = DigressionName.VertIznosR;
                        VIdig3Right.Km = NKm;
                        VIdig3Right.Meter = (int)Meters[i];
                        VIfound3Right = true;
                        VIdig3Right.Typ = 3;
                        if ((VIdig3RightValue > VertIznosR[i]) || (VIdig3RightValue == 0))
                            VIdig3RightValue = VertIznosR[i];
                    }
                    else
                    {
                        if (VIdig3RightValue < VertIznosR[i])
                            VIdig3RightValue = VertIznosR[i];
                    }
                }

                if ((VertIznosR[i] <= 12) && (VIfound3Right))
                {
                    VIdig3Right.Length = Math.Abs(VIdig3Right.Meter - (int)Meters[i]);
                    VIdig3Right.Kmetr = (int)Meters[i];
                    VIdig3Right.Value = VIdig3RightValue;
                    VIdig3RightValue = 0;
                    result.Add(VIdig3Right);
                    VIfound3Right = false;
                }
                //2 степени Ив прав
                if ((VertIznosR[i] <= 12) && (VertIznosR[i] > 4))
                {
                    if (!VIfound2Right)
                    {
                        VIdig2Right = new Digression();
                        VIdig2Right.DigName = DigressionName.VertIznosR;
                        VIdig2Right.Km = NKm;
                        VIdig2Right.Meter = (int)Meters[i];
                        VIfound2Right = true;
                        VIdig2Right.Typ = 2;
                        if ((VIdig2RightValue > VertIznosR[i]) || (VIdig2RightValue == 0))
                            VIdig2RightValue = VertIznosR[i];
                    }
                    else
                    {
                        if (VIdig2RightValue < VertIznosR[i])
                            VIdig2RightValue = VertIznosR[i];
                    }
                }
                else
                if (VIfound2Right)
                {
                    VIdig2Right.Length = Math.Abs(VIdig2Right.Meter - (int)Meters[i]);
                    VIdig2Right.Kmetr = (int)Meters[i];
                    VIdig2Right.Value = VIdig2RightValue;
                    VIdig2RightValue = 0;
                    result.Add(VIdig2Right);
                    VIfound2Right = false;
                }
                //4 степени Иб прав
                if (SideWearRight[i] > 15)
                {
                    if (!SWfound4Right)
                    {
                        SWdig4Right = new Digression();
                        SWdig4Right.DigName = DigressionName.SideWearRight;
                        SWdig4Right.Km = NKm;
                        SWdig4Right.Meter = (int)Meters[i];
                        SWfound4Right = true;
                        SWdig4Right.Typ = 4;
                        if ((SWdig4RightValue > SideWearRight[i]) || (SWdig4RightValue == 0))
                            SWdig4RightValue = SideWearRight[i];
                    }
                    else
                    {
                        if (SWdig4RightValue < SideWearRight[i])
                            SWdig4RightValue = SideWearRight[i];
                    }
                }

                if ((SideWearRight[i] <= 15) && (SWfound4Right))
                {
                    SWdig4Right.Length = Math.Abs(SWdig4Right.Meter - (int)Meters[i]);
                    SWdig4Right.Kmetr = (int)Meters[i];
                    SWdig4Right.Value = SWdig4RightValue;
                    SWdig4RightValue = 0;
                    result.Add(SWdig4Right);
                    SWfound4Right = false;
                }
                //3 степени Иб прав
                if ((SideWearRight[i] <= 15) && (SideWearRight[i] > 13))
                {
                    if (!SWfound3Right)
                    {
                        SWdig3Right = new Digression();
                        SWdig3Right.DigName = DigressionName.SideWearRight;
                        SWdig3Right.Km = NKm;
                        SWdig3Right.Meter = (int)Meters[i];
                        SWfound3Right = true;
                        SWdig3Right.Typ = 3;
                        if ((SWdig3RightValue > SideWearRight[i]) || (SWdig3RightValue == 0))
                            SWdig3RightValue = SideWearRight[i];
                    }
                    else
                    {
                        if (SWdig3RightValue < SideWearRight[i])
                            SWdig3RightValue = SideWearRight[i];
                    }
                }
                else
                if (SWfound3Right)
                {
                    SWdig3Right.Length = Math.Abs(SWdig3Right.Meter - (int)Meters[i]);
                    SWdig3Right.Kmetr = (int)Meters[i];
                    SWdig3Right.Value = SWdig3RightValue;
                    SWdig3RightValue = 0;
                    result.Add(SWdig3Right);
                    SWfound3Right = false;
                }
                //2 степени Иб прав
                if ((SideWearRight[i] <= 13) && (SideWearRight[i] >= 10.1))
                {
                    if (!SWfound2Right)
                    {
                        SWdig2Right = new Digression();
                        SWdig2Right.DigName = DigressionName.SideWearRight;
                        SWdig2Right.Km = NKm;
                        SWdig2Right.Meter = (int)Meters[i];
                        SWfound2Right = true;
                        SWdig2Right.Typ = 2;
                        if ((SWdig2RightValue > SideWearRight[i]) || (SWdig2RightValue == 0))
                            SWdig2RightValue = SideWearRight[i];
                    }
                    else
                    {
                        if (SWdig2RightValue < SideWearRight[i])
                            SWdig2RightValue = SideWearRight[i];
                    }
                }
                else
                if (SWfound2Right)
                {
                    SWdig2Right.Length = Math.Abs(SWdig2Right.Meter - (int)Meters[i]);
                    SWdig2Right.Kmetr = (int)Meters[i];
                    SWdig2Right.Value = SWdig2RightValue;
                    SWdig2RightValue = 0;
                    result.Add(SWdig2Right);
                    SWfound2Right = false;
                }
                //4 степени Иб лев
                if (SideWearLeft[i] > 15)
                {
                    if (!SWfound4Left)
                    {
                        SWdig4Left = new Digression();
                        SWdig4Left.DigName = DigressionName.SideWearLeft;
                        SWdig4Left.Km = NKm;
                        SWdig4Left.Meter = (int)Meters[i];
                        SWfound4Left = true;
                        SWdig4Left.Typ = 4;
                        if ((SWdig4LeftValue > SideWearLeft[i]) || (SWdig4LeftValue == 0))
                            SWdig4LeftValue = SideWearLeft[i];
                    }
                    else
                    {
                        if (SWdig4LeftValue < SideWearLeft[i])
                            SWdig4LeftValue = SideWearLeft[i];
                    }
                }

                if ((SideWearLeft[i] <= 15) && (SWfound4Left))
                {
                    SWdig4Left.Length = Math.Abs(SWdig4Left.Meter - (int)Meters[i]);
                    SWdig4Left.Kmetr = (int)Meters[i];
                    SWdig4Left.Value = SWdig4LeftValue;
                    SWdig4LeftValue = 0;
                    result.Add(SWdig4Left);
                    SWfound4Left = false;
                }
                //3 степени Иб лев
                if ((SideWearLeft[i] <= 15) && (SideWearLeft[i] > 13))
                {
                    if (!SWfound3Left)
                    {
                        SWdig3Left = new Digression();
                        SWdig3Left.DigName = DigressionName.SideWearLeft;
                        SWdig3Left.Km = NKm;
                        SWdig3Left.Meter = (int)Meters[i];
                        SWfound3Left = true;
                        SWdig3Left.Typ = 3;
                        if ((SWdig3LeftValue > SideWearLeft[i]) || (SWdig3LeftValue == 0))
                            SWdig3LeftValue = SideWearLeft[i];
                    }
                    else
                    {
                        if (SWdig3LeftValue < SideWearLeft[i])
                            SWdig3LeftValue = SideWearLeft[i];
                    }
                }
                else
                if (SWfound3Left)
                {
                    SWdig3Left.Length = Math.Abs(SWdig3Left.Meter - (int)Meters[i]);
                    SWdig3Left.Kmetr = (int)Meters[i];
                    SWdig3Left.Value = SWdig3LeftValue;
                    SWdig3LeftValue = 0;
                    result.Add(SWdig3Left);
                    SWfound3Left = false;
                }
                //2 степени Иб прав
                if ((SideWearLeft[i] <= 13) && (SideWearLeft[i] >= 10.1))
                {
                    if (!SWfound2Left)
                    {
                        SWdig2Left = new Digression();
                        SWdig2Left.DigName = DigressionName.SideWearLeft;
                        SWdig2Left.Km = NKm;
                        SWdig2Left.Meter = (int)Meters[i];
                        SWfound2Left = true;
                        SWdig2Left.Typ = 2;
                        if ((SWdig2LeftValue > SideWearLeft[i]) || (SWdig2LeftValue == 0))
                            SWdig2LeftValue = SideWearLeft[i];
                    }
                    else
                    {
                        if (SWdig2LeftValue < SideWearLeft[i])
                            SWdig2LeftValue = SideWearLeft[i];
                    }
                }
                else
                if (SWfound2Left)
                {
                    SWdig2Left.Length = Math.Abs(SWdig2Left.Meter - (int)Meters[i]);
                    SWdig2Left.Kmetr = (int)Meters[i];
                    SWdig2Left.Value = SWdig2LeftValue;
                    SWdig2LeftValue = 0;
                    result.Add(SWdig2Left);
                    SWfound2Left = false;
                }


                if (TreadTiltLeft[i] >= 1 / 20.0)
                {
                    //4 степени НПК лев
                    if (TreadTiltLeft[i] >= OneDiv11)
                    {
                        if (!found4Left)
                        {
                            dig4Left = new Digression();
                            dig4Left.DigName = DigressionName.TreadTiltLeft;
                            dig4Left.Km = NKm;
                            dig4Left.Meter = (int)Meters[i];
                            found4Left = true;
                            dig4Left.Typ = 4;
                            if ((dig4LeftValue > TreadTiltLeft[i]) || (dig4LeftValue == 0))
                                dig4LeftValue = TreadTiltLeft[i];
                        }
                        else
                        {
                            if (dig4LeftValue < TreadTiltLeft[i])
                                dig4LeftValue = TreadTiltLeft[i];
                        }
                    }

                    if ((TreadTiltLeft[i] <= OneDiv12) && (found4Left))
                    {
                        dig4Left.Length = Math.Abs(dig4Left.Meter - (int)Meters[i]);
                        dig4Left.Kmetr = (int)Meters[i];
                        dig4Left.Value = dig4LeftValue;

                        if (Math.Abs(dig4Left.Meter - (int)Meters[i]) < 4)
                            dig4Left.Typ = 3;

                        dig4LeftValue = 0;
                        result.Add(dig4Left);
                        found4Left = false;
                    }

                    //3 степени НПК лев
                    if ((TreadTiltLeft[i] <= OneDiv12) && (TreadTiltLeft[i] >= OneDiv13))
                    {
                        if (!found3Left)
                        {
                            dig3Left = new Digression();
                            dig3Left.DigName = DigressionName.TreadTiltLeft;
                            dig3Left.Km = NKm;
                            dig3Left.Meter = (int)Meters[i];
                            found3Left = true;
                            dig3Left.Typ = 3;
                            if ((dig3LeftValue > TreadTiltLeft[i]) || (dig3LeftValue == 0))
                                dig3LeftValue = TreadTiltLeft[i];
                        }
                        else
                        {
                            if (dig3LeftValue < TreadTiltLeft[i])
                                dig3LeftValue = TreadTiltLeft[i];
                        }
                    }
                    else if (found3Left)
                    {
                        dig3Left.Length = Math.Abs(dig3Left.Meter - (int)Meters[i]);
                        dig3Left.Kmetr = (int)Meters[i];
                        dig3Left.Value = dig3LeftValue;
                        dig3LeftValue = 0;
                        result.Add(dig3Left);
                        found3Left = false;
                    }
                }
                else
                {
                    //4 степени НПК лев
                    if (TreadTiltLeft[i] <= OneDiv61)
                    {
                        if (!found4Left)
                        {
                            dig4Left = new Digression();
                            dig4Left.DigName = DigressionName.TreadTiltLeft;
                            dig4Left.Km = NKm;
                            dig4Left.Meter = (int)Meters[i];
                            found4Left = true;
                            dig4Left.Typ = 4;
                            if ((dig4LeftValue > TreadTiltLeft[i]) || (dig4LeftValue == 0))
                                dig4LeftValue = TreadTiltLeft[i];
                        }
                        else
                        {
                            if (dig4LeftValue < TreadTiltLeft[i])
                                dig4LeftValue = TreadTiltLeft[i];
                        }
                    }

                    if ((TreadTiltLeft[i] >= OneDiv60) && (found4Left))
                    {
                        dig4Left.Length = Math.Abs(dig4Left.Meter - (int)Meters[i]);
                        dig4Left.Kmetr = (int)Meters[i];
                        dig4Left.Value = dig4LeftValue;

                        if (Math.Abs(dig4Left.Meter - (int)Meters[i]) < 4)
                            dig4Left.Typ = 3;

                        dig4LeftValue = 0;
                        result.Add(dig4Left);
                        found4Left = false;
                    }

                    //3 степени НПК лев
                    if ((TreadTiltLeft[i] <= OneDiv44) && (TreadTiltLeft[i] >= OneDiv60))
                    {
                        if (!found3Left)
                        {
                            dig3Left = new Digression();
                            dig3Left.DigName = DigressionName.TreadTiltLeft;
                            dig3Left.Km = NKm;
                            dig3Left.Meter = (int)Meters[i];
                            found3Left = true;
                            dig3Left.Typ = 3;
                            if ((dig3LeftValue > TreadTiltLeft[i]) || (dig3LeftValue == 0))
                                dig3LeftValue = TreadTiltLeft[i];
                        }
                        else
                        {
                            if (dig3LeftValue < TreadTiltLeft[i])
                                dig3LeftValue = TreadTiltLeft[i];
                        }
                    }
                    else if (found3Left)
                    {
                        dig3Left.Length = Math.Abs(dig3Left.Meter - (int)Meters[i]);
                        dig3Left.Kmetr = (int)Meters[i];
                        dig3Left.Value = dig3LeftValue;
                        dig3LeftValue = 0;
                        result.Add(dig3Left);
                        found3Left = false;
                    }
                }
                if (TreadTiltRight[i] >= 1 / 20.0)
                {
                    //4 степень НПК прав
                    if (TreadTiltRight[i] >= OneDiv11)
                    {
                        if (!found4Right)
                        {
                            dig4Right = new Digression();
                            dig4Right.DigName = DigressionName.TreadTiltRight;
                            dig4Right.Km = NKm;
                            dig4Right.Meter = (int)Meters[i];
                            found4Right = true;
                            dig4Right.Typ = 4;
                            if ((dig4RightValue > TreadTiltRight[i]) || (dig4RightValue == 0))
                                dig4RightValue = TreadTiltRight[i];
                        }
                        else
                        {
                            if (dig4RightValue < TreadTiltRight[i])
                                dig4RightValue = TreadTiltRight[i];
                        }

                    }

                    if ((TreadTiltRight[i] <= OneDiv12) && (found4Right))
                    {
                        dig4Right.Length = Math.Abs(dig4Right.Meter - (int)Meters[i]);
                        dig4Right.Kmetr = (int)Meters[i];
                        dig4Right.Value = dig4RightValue;

                        if (Math.Abs(dig4Right.Meter - (int)Meters[i]) < 4)
                            dig4Right.Typ = 3;

                        dig4RightValue = 0;
                        result.Add(dig4Right);
                        found4Right = false;
                    }
                    //3 степень НПК прав
                    if ((TreadTiltRight[i] <= OneDiv12) && (TreadTiltRight[i] >= OneDiv13))
                    {
                        if (!found3Right)
                        {
                            dig3Right = new Digression();
                            dig3Right.DigName = DigressionName.TreadTiltRight;
                            dig3Right.Km = NKm;
                            dig3Right.Meter = (int)Meters[i];
                            found3Right = true;
                            dig3Right.Typ = 3;
                            if ((dig3RightValue > TreadTiltRight[i]) || (dig3RightValue == 0))
                                dig3RightValue = TreadTiltRight[i];
                        }
                        else
                        {
                            if (dig3RightValue < TreadTiltRight[i])
                                dig3RightValue = TreadTiltRight[i];
                        }

                    }
                    else if (found3Right)
                    {
                        dig3Right.Length = Math.Abs(dig3Right.Meter - (int)Meters[i]);
                        dig3Right.Kmetr = (int)Meters[i];
                        dig3Right.Value = dig3RightValue;
                        dig3RightValue = 0;
                        result.Add(dig3Right);
                        found3Right = false;
                    }
                }
                else
                {
                    //4 степень НПК прав
                    if (TreadTiltRight[i] <= OneDiv61)
                    {
                        if (!found4Right)
                        {
                            dig4Right = new Digression();
                            dig4Right.DigName = DigressionName.TreadTiltRight;
                            dig4Right.Km = NKm;
                            dig4Right.Meter = (int)Meters[i];
                            found4Right = true;
                            dig4Right.Typ = 4;
                            if ((dig4RightValue > TreadTiltRight[i]) || (dig4RightValue == 0))
                                dig4RightValue = TreadTiltRight[i];
                        }
                        else
                        {
                            if (dig4RightValue < TreadTiltRight[i])
                                dig4RightValue = TreadTiltRight[i];
                        }

                    }

                    if ((TreadTiltRight[i] >= OneDiv60) && (found4Right))
                    {
                        dig4Right.Length = Math.Abs(dig4Right.Meter - (int)Meters[i]);
                        dig4Right.Kmetr = (int)Meters[i];
                        dig4Right.Value = dig4RightValue;

                        if (Math.Abs(dig4Right.Meter - (int)Meters[i]) < 4)
                            dig4Right.Typ = 3;


                        dig4RightValue = 0;
                        result.Add(dig4Right);
                        found4Right = false;
                    }
                    //3 степень НПК прав
                    if ((TreadTiltRight[i] <= OneDiv44) && (TreadTiltRight[i] >= OneDiv60))
                    {
                        if (!found3Right)
                        {
                            dig3Right = new Digression();
                            dig3Right.DigName = DigressionName.TreadTiltRight;
                            dig3Right.Km = NKm;
                            dig3Right.Meter = (int)Meters[i];
                            found3Right = true;
                            dig3Right.Typ = 3;
                            if ((dig3RightValue > TreadTiltRight[i]) || (dig3RightValue == 0))
                                dig3RightValue = TreadTiltRight[i];
                        }
                        else
                        {
                            if (dig3RightValue < TreadTiltRight[i])
                                dig3RightValue = TreadTiltRight[i];
                        }

                    }
                    else if (found3Right)
                    {
                        dig3Right.Length = Math.Abs(dig3Right.Meter - (int)Meters[i]);
                        dig3Right.Kmetr = (int)Meters[i];
                        dig3Right.Value = dig3RightValue;
                        dig3RightValue = 0;
                        result.Add(dig3Right);
                        found3Right = false;
                    }
                }
                //----------------------------------------------------------------------------------------------------------
                if (DownhillLeft[i] >= 1 / 20.0)
                {
                    //4 степени ПУ лев
                    if (DownhillLeft[i] >= OneDiv11)
                    {

                        if (!PUfound4Left)
                        {
                            PUdig4Left = new Digression();
                            PUdig4Left.DigName = DigressionName.DownhillLeft;
                            PUdig4Left.Km = NKm;
                            PUdig4Left.Meter = (int)Meters[i];
                            PUfound4Left = true;
                            PUdig4Left.Typ = 4;
                            if ((PUdig4LeftValue > DownhillLeft[i]) || (PUdig4LeftValue == 0))
                                PUdig4LeftValue = DownhillLeft[i];
                        }
                        else
                        {
                            if (PUdig4LeftValue < DownhillLeft[i])
                                PUdig4LeftValue = DownhillLeft[i];
                        }

                    }

                    if ((DownhillLeft[i] <= OneDiv12) && (PUfound4Left))
                    {
                        PUdig4Left.Length = Math.Abs(PUdig4Left.Meter - (int)Meters[i]);
                        PUdig4Left.Kmetr = (int)Meters[i];
                        PUdig4Left.Value = PUdig4LeftValue;
                        PUdig4LeftValue = 0;
                        result.Add(PUdig4Left);
                        PUfound4Left = false;
                    }
                    //3 степени ПУ лев
                    if ((DownhillLeft[i] <= OneDiv12) && (DownhillLeft[i] >= OneDiv13))
                    {
                        if (!PUfound3Left)
                        {
                            PUdig3Left = new Digression();
                            PUdig3Left.DigName = DigressionName.DownhillLeft;
                            PUdig3Left.Km = NKm;
                            PUdig3Left.Meter = (int)Meters[i];
                            PUfound3Left = true;
                            PUdig3Left.Typ = 3;
                            if ((PUdig3LeftValue > DownhillLeft[i]) || (PUdig3LeftValue == 0))
                                PUdig3LeftValue = DownhillLeft[i];
                        }
                        else
                        {
                            if (PUdig3LeftValue < DownhillLeft[i])
                                PUdig3LeftValue = DownhillLeft[i];
                        }

                    }
                    else if (PUfound3Left)
                    {
                        PUdig3Left.Length = Math.Abs(PUdig3Left.Meter - (int)Meters[i]);
                        PUdig3Left.Kmetr = (int)Meters[i];
                        PUdig3Left.Value = PUdig3LeftValue;
                        PUdig3LeftValue = 0;
                        result.Add(PUdig3Left);
                        PUfound3Left = false;
                    }
                }
                else
                {
                    //4 степени ПУ лев
                    if (DownhillLeft[i] <= OneDiv61)
                    {

                        if (!PUfound4Left)
                        {
                            PUdig4Left = new Digression();
                            PUdig4Left.DigName = DigressionName.DownhillLeft;
                            PUdig4Left.Km = NKm;
                            PUdig4Left.Meter = (int)Meters[i];
                            PUfound4Left = true;
                            PUdig4Left.Typ = 4;
                            if ((PUdig4LeftValue > DownhillLeft[i]) || (PUdig4LeftValue == 0))
                                PUdig4LeftValue = DownhillLeft[i];
                        }
                        else
                        {
                            if (PUdig4LeftValue < DownhillLeft[i])
                                PUdig4LeftValue = DownhillLeft[i];
                        }

                    }

                    if ((DownhillLeft[i] >= OneDiv60) && (PUfound4Left))
                    {
                        PUdig4Left.Length = Math.Abs(PUdig4Left.Meter - (int)Meters[i]);
                        PUdig4Left.Kmetr = (int)Meters[i];
                        PUdig4Left.Value = PUdig4LeftValue;
                        PUdig4LeftValue = 0;
                        result.Add(PUdig4Left);
                        PUfound4Left = false;
                    }
                    //3 степени ПУ лев
                    if ((DownhillLeft[i] <= OneDiv44) && (DownhillLeft[i] >= OneDiv60))
                    {
                        if (!PUfound3Left)
                        {
                            PUdig3Left = new Digression();
                            PUdig3Left.DigName = DigressionName.DownhillLeft;
                            PUdig3Left.Km = NKm;
                            PUdig3Left.Meter = (int)Meters[i];
                            PUfound3Left = true;
                            PUdig3Left.Typ = 3;
                            if ((PUdig3LeftValue > DownhillLeft[i]) || (PUdig3LeftValue == 0))
                                PUdig3LeftValue = DownhillLeft[i];
                        }
                        else
                        {
                            if (PUdig3LeftValue < DownhillLeft[i])
                                PUdig3LeftValue = DownhillLeft[i];
                        }

                    }
                    else if (PUfound3Left)
                    {
                        PUdig3Left.Length = Math.Abs(PUdig3Left.Meter - (int)Meters[i]);
                        PUdig3Left.Kmetr = (int)Meters[i];
                        PUdig3Left.Value = PUdig3LeftValue;
                        PUdig3LeftValue = 0;
                        result.Add(PUdig3Left);
                        PUfound3Left = false;
                    }
                }
                //-------------------------------------------------------------------------------------------------------
                if (DownhillRight[i] >= 1 / 20.0)
                {
                    //4 степень ПУ прав
                    if (DownhillRight[i] > PUOneDiv12)
                    {
                        if (!PUfound4Right)
                        {
                            PUdig4Right = new Digression();
                            PUdig4Right.DigName = DigressionName.DownhillRight;
                            PUdig4Right.Km = NKm;
                            PUdig4Right.Meter = (int)Meters[i];
                            PUfound4Right = true;
                            PUdig4Right.Typ = 4;
                            if ((PUdig4RightValue > DownhillRight[i]) || (PUdig4RightValue == 0))
                                PUdig4RightValue = DownhillRight[i];
                        }
                        else
                        {
                            if (PUdig4RightValue < DownhillRight[i])
                                PUdig4RightValue = DownhillRight[i];
                        }

                    }

                    if ((DownhillRight[i] <= PUOneDiv12) && (PUfound4Right))
                    {
                        PUdig4Right.Length = Math.Abs(PUdig4Right.Meter - (int)Meters[i]);
                        PUdig4Right.Kmetr = (int)Meters[i];
                        PUdig4Right.Value = PUdig4RightValue;
                        PUdig4RightValue = 0;
                        result.Add(PUdig4Right);
                        PUfound4Right = false;
                    }
                    //3 степень ПУ прав
                    if ((DownhillRight[i] <= PUOneDiv12) && (DownhillRight[i] >= PUOneDiv13))
                    {
                        if (!PUfound3Right)
                        {
                            PUdig3Right = new Digression();
                            PUdig3Right.DigName = DigressionName.DownhillRight;
                            PUdig3Right.Km = NKm;
                            PUdig3Right.Meter = (int)Meters[i];
                            PUfound3Right = true;
                            PUdig3Right.Typ = 3;
                            if ((PUdig3RightValue > DownhillRight[i]) || (PUdig3RightValue == 0))
                                PUdig3RightValue = DownhillRight[i];
                        }
                        else
                        {
                            if (PUdig3RightValue < DownhillRight[i])
                                PUdig3RightValue = DownhillRight[i];
                        }

                    }
                    else

                    if (PUfound3Right)
                    {
                        PUdig3Right.Length = Math.Abs(PUdig3Right.Meter - (int)Meters[i]);
                        PUdig3Right.Kmetr = (int)Meters[i];
                        PUdig3Right.Value = PUdig3RightValue;
                        PUdig3RightValue = 0;
                        result.Add(PUdig3Right);
                        PUfound3Right = false;
                    }
                }
                else
                {
                    //4 степень ПУ прав
                    if (DownhillRight[i] <= OneDiv61)
                    {
                        if (!PUfound4Right)
                        {
                            PUdig4Right = new Digression();
                            PUdig4Right.DigName = DigressionName.DownhillRight;
                            PUdig4Right.Km = NKm;
                            PUdig4Right.Meter = (int)Meters[i];
                            PUfound4Right = true;
                            PUdig4Right.Typ = 4;
                            if ((PUdig4RightValue > DownhillRight[i]) || (PUdig4RightValue == 0))
                                PUdig4RightValue = DownhillRight[i];
                        }
                        else
                        {
                            if (PUdig4RightValue < DownhillRight[i])
                                PUdig4RightValue = DownhillRight[i];
                        }

                    }

                    if ((DownhillRight[i] >= OneDiv60) && (PUfound4Right))
                    {
                        PUdig4Right.Length = Math.Abs(PUdig4Right.Meter - (int)Meters[i]);
                        PUdig4Right.Km = NKm;
                        PUdig4Right.Kmetr = (int)Meters[i];
                        PUdig4Right.Value = PUdig4RightValue;
                        PUdig4RightValue = 0;
                        result.Add(PUdig4Right);
                        PUfound4Right = false;
                    }
                    //3 степень ПУ прав
                    if ((DownhillRight[i] <= OneDiv44) && (DownhillRight[i] >= OneDiv60))
                    {
                        if (!PUfound3Right)
                        {
                            PUdig3Right = new Digression();
                            PUdig3Right.DigName = DigressionName.DownhillRight;
                            PUdig3Right.Km = NKm;
                            PUdig3Right.Meter = (int)Meters[i];
                            PUfound3Right = true;
                            PUdig3Right.Typ = 3;
                            if ((PUdig3RightValue > DownhillRight[i]) || (PUdig3RightValue == 0))
                                PUdig3RightValue = DownhillRight[i];
                        }
                        else
                        {
                            if (PUdig3RightValue < DownhillRight[i])
                                PUdig3RightValue = DownhillRight[i];
                        }
                    }
                    else

                    if (PUfound3Right)
                    {
                        PUdig3Right.Length = Math.Abs(PUdig3Right.Meter - (int)Meters[i]);
                        PUdig3Right.Kmetr = (int)Meters[i];
                        PUdig3Right.Value = PUdig3RightValue;
                        PUdig3RightValue = 0;
                        result.Add(PUdig3Right);
                        PUfound3Right = false;
                    }
                }
            }
            //-------------------------------------------------------------------------------------------------------
            //ИП
            if (RWfound3Left)
            {
                RWdig3Left.Value = RWdig3LeftValue;
                result.Add(RWdig3Left);
            }
            if (RWfound3Right)
            {
                RWdig3Right.Value = RWdig3RightValue;
                result.Add(RWdig3Right);
            }
            if (RWfound2Left)
            {
                RWdig2Left.Value = RWdig2LeftValue;
                result.Add(RWdig2Left);
            }
            if (RWfound2Right)
            {
                RWdig2Right.Value = RWdig2RightValue;
                result.Add(RWdig2Right);
            }
            //ИВ
            if (VIfound3Left)
            {
                VIdig3Left.Value = VIdig3LeftValue;
                result.Add(VIdig3Left);
            }
            if (VIfound3Right)
            {
                VIdig3Right.Value = VIdig3RightValue;
                result.Add(VIdig3Right);
            }
            if (VIfound2Left)
            {
                VIdig2Left.Value = VIdig2LeftValue;
                result.Add(VIdig2Left);
            }
            if (VIfound2Right)
            {
                VIdig2Right.Value = VIdig2RightValue;
                result.Add(VIdig2Right);
            }
            //Иб
            if (SWfound4Left)
            {
                SWdig4Left.Value = SWdig4LeftValue;
                result.Add(SWdig4Left);
            }
            if (SWfound4Right)
            {
                SWdig4Right.Value = SWdig4RightValue;
                result.Add(SWdig4Right);
            }
            if (SWfound3Left)
            {
                SWdig3Left.Value = SWdig3LeftValue;
                result.Add(SWdig3Left);
            }
            if (SWfound3Right)
            {
                SWdig3Right.Value = SWdig3RightValue;
                result.Add(SWdig3Right);
            }
            if (SWfound2Left)
            {
                SWdig2Left.Value = SWdig2LeftValue;
                result.Add(SWdig2Left);
            }
            if (SWfound2Right)
            {
                SWdig2Right.Value = SWdig2RightValue;
                result.Add(SWdig2Right);
            }
            //нпк
            if (found4Left)
            {
                dig4Left.Value = dig4LeftValue;
                result.Add(dig4Left);
            }

            if (found4Right)
            {
                dig4Right.Value = dig4RightValue;
                result.Add(dig4Right);
            }
            if (found3Left)
            {
                dig3Left.Value = dig3LeftValue;
                result.Add(dig3Left);
            }

            if (found3Right)
            {
                dig3Right.Value = dig3RightValue;
                result.Add(dig3Right);
            }
            //ПУ
            if (PUfound4Left)
            {
                PUdig4Left.Value = PUdig4LeftValue;
                result.Add(PUdig4Left);
            }

            if (PUfound4Right)
            {
                PUdig4Right.Value = PUdig4RightValue;
                result.Add(PUdig4Right);
            }
            if (PUfound3Left)
            {
                PUdig3Left.Value = PUdig3LeftValue;
                result.Add(PUdig3Left);
            }

            if (PUfound3Right)
            {
                PUdig3Right.Value = PUdig3RightValue;
                result.Add(PUdig3Right);
            }

            return result;
        }
        public List<Digression> GetDigressionViznos()
        {
            var result = new List<Digression>();
            bool found4Left = false;
            bool found4Right = false;
            bool found3Left = false;
            bool found3Right = false;
            float OneDiv12 = 1 / 12f;
            float OneDiv13 = 1 / 13f;
            Digression dig4Left = null;
            Digression dig4Right = null;

            Digression dig3Left = null;
            Digression dig3Right = null;

            float dig4LeftValue = 0, dig4RightValue = 0, dig3LeftValue = 0, dig3RightValue = 0;

            for (int i = 0; i < Meters.Count; i++)
            {
                //4 степени НПК лев
                if (TreadTiltLeft[i] > OneDiv12)
                {
                    if (!found4Left)
                    {
                        dig4Left = new Digression();
                        dig4Left.DigName = DigressionName.TreadTiltLeft;
                        dig4Left.Km = NKm;
                        dig4Left.Meter = (int)Meters[i];
                        found4Left = true;
                        dig4Left.Typ = 4;
                        if ((dig4LeftValue > TreadTiltLeft[i]) || (dig4LeftValue == 0))
                            dig4LeftValue = TreadTiltLeft[i];
                    }

                }

                if ((TreadTiltLeft[i] <= OneDiv12) && (found4Left))
                {
                    dig4Left.Length = Math.Abs(dig4Left.Meter - (int)Meters[i]);
                    dig4Left.Kmetr = (int)Meters[i];
                    dig4Left.Value = dig4LeftValue;
                    dig4LeftValue = 0;
                    result.Add(dig4Left);
                    found4Left = false;
                }
                //3 степени НПК лев
                if ((TreadTiltLeft[i] <= OneDiv12) && (TreadTiltLeft[i] >= OneDiv13))
                {
                    if (!found3Left)
                    {
                        dig3Left = new Digression();
                        dig3Left.DigName = DigressionName.TreadTiltLeft;
                        dig3Left.Km = NKm;
                        dig3Left.Meter = (int)Meters[i];
                        found3Left = true;
                        dig3Left.Typ = 3;
                        if ((dig3LeftValue > TreadTiltLeft[i]) || (dig3LeftValue == 0))
                            dig3LeftValue = TreadTiltLeft[i];
                    }

                }
                else

                if (found3Left)
                {
                    dig3Left.Length = Math.Abs(dig3Left.Meter - (int)Meters[i]);
                    dig3Left.Kmetr = (int)Meters[i];
                    dig3Left.Value = dig3LeftValue;
                    dig3LeftValue = 0;
                    result.Add(dig3Left);
                    found3Left = false;
                }

                //4 степень НПК прав
                if (TreadTiltRight[i] > OneDiv12)
                {
                    if (!found4Right)
                    {
                        dig4Right = new Digression();
                        dig4Right.DigName = DigressionName.TreadTiltRight;
                        dig4Right.Km = NKm;
                        dig4Right.Meter = (int)Meters[i];
                        found4Right = true;
                        dig4Right.Typ = 4;
                        if ((dig4RightValue > TreadTiltRight[i]) || (dig4RightValue == 0))
                            dig4RightValue = TreadTiltRight[i];
                    }

                }

                if ((TreadTiltRight[i] <= OneDiv12) && (found4Right))
                {
                    dig4Right.Length = Math.Abs(dig4Right.Meter - (int)Meters[i]);
                    dig4Right.Kmetr = (int)Meters[i];
                    dig4Right.Value = dig4RightValue;
                    dig4RightValue = 0;
                    result.Add(dig4Right);
                    found4Right = false;
                }
                //3 степень НПК прав
                if ((TreadTiltRight[i] <= OneDiv12) && (TreadTiltRight[i] >= OneDiv13))
                {
                    if (!found3Right)
                    {
                        dig3Right = new Digression();
                        dig3Right.DigName = DigressionName.TreadTiltRight;
                        dig3Right.Km = NKm;
                        dig3Right.Meter = (int)Meters[i];
                        found3Right = true;
                        dig3Right.Typ = 3;
                        if ((dig3RightValue > TreadTiltRight[i]) || (dig3RightValue == 0))
                            dig3RightValue = TreadTiltRight[i];
                    }

                }
                else

                if (found3Right)
                {
                    dig3Right.Length = Math.Abs(dig3Right.Meter - (int)Meters[i]);
                    dig3Right.Kmetr = (int)Meters[i];
                    dig3Right.Value = dig3RightValue;
                    dig3RightValue = 0;
                    result.Add(dig3Right);
                    found3Right = false;
                }
            }

            if (found4Left)
            {
                dig4Left.Value = dig4LeftValue;
                result.Add(dig4Left);
            }

            if (found4Right)
            {
                dig4Right.Value = dig4RightValue;
                result.Add(dig4Right);
            }
            if (found3Left)
            {
                dig3Left.Value = dig3LeftValue;
                result.Add(dig3Left);
            }

            if (found3Right)
            {
                dig3Right.Value = dig3RightValue;
                result.Add(dig3Right);
            }
            return result;
        }

        public void ParsevertIznos(string line)
        {
            line = Regex.Replace(line, @"\s+", " ");
            var parameters = line.Split(new char[] { ' ', '\t' });
            {
                int meter = (int.Parse(parameters[0]) - 1) * 100 + int.Parse(parameters[1]);

                if (Meters.Contains(meter) == true)
                {
                    GarbL.Add(float.Parse(parameters[6], CultureInfo.InvariantCulture.NumberFormat));
                    GarbR.Add(float.Parse(parameters[14], CultureInfo.InvariantCulture.NumberFormat));

                    GarbDownhillLeft.Add(float.Parse(parameters[5], CultureInfo.InvariantCulture.NumberFormat));
                    GarbDownhillRight.Add(float.Parse(parameters[13], CultureInfo.InvariantCulture.NumberFormat));

                    GarbTreadTiltLeft.Add(float.Parse(parameters[4], CultureInfo.InvariantCulture.NumberFormat));
                    GarbTreadTiltRight.Add(float.Parse(parameters[12], CultureInfo.InvariantCulture.NumberFormat));
                }

                if (Meters.Contains(meter) == false)
                {
                    if (GarbL.Count == 0)
                    {
                        if (prevMetr != meter)
                        {
                            prevMetr = meter;
                            CurrentSm = TravelDirection == Direction.Direct ? 0 : 0.8f;
                        }
                        Meters.Add(meter);
                        GarbL.Add(float.Parse(parameters[6], CultureInfo.InvariantCulture.NumberFormat));
                        GarbR.Add(float.Parse(parameters[14], CultureInfo.InvariantCulture.NumberFormat));

                        GarbDownhillLeft.Add(float.Parse(parameters[5], CultureInfo.InvariantCulture.NumberFormat));
                        GarbDownhillRight.Add(float.Parse(parameters[13], CultureInfo.InvariantCulture.NumberFormat));

                        GarbTreadTiltLeft.Add(float.Parse(parameters[4], CultureInfo.InvariantCulture.NumberFormat));
                        GarbTreadTiltRight.Add(float.Parse(parameters[12], CultureInfo.InvariantCulture.NumberFormat));
                    }
                    else
                    {
                        Meters.Add(meter);
                        VertIznosL.Add(GarbL.Average());
                        VertIznosR.Add(GarbR.Average());

                        DownhillLeft.Add(GarbDownhillLeft.Average());
                        DownhillRight.Add(GarbDownhillRight.Average());

                        TreadTiltLeft.Add(GarbTreadTiltLeft.Average());
                        TreadTiltRight.Add(GarbTreadTiltRight.Average());

                        GarbL.Clear();
                        GarbR.Clear();
                        GarbDownhillLeft.Clear();
                        GarbDownhillRight.Clear();
                        GarbTreadTiltLeft.Clear();
                        GarbTreadTiltRight.Clear();
                    }
                }
            }
        }
    }
}
