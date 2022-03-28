using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using ALARm.Core.AdditionalParameteres;
using ALARm.Core.Report;

namespace ALARm.Core
{
    public class ShortRoughness
    {
        public string Direction { get; set; }
        public string TrackNumber { get; set; }
        public int KilometrNumber { get; set; }
        public int Legnth { get; set; }
        public CarParameters Car { get; set; }
        public Direction TravelDirection { get; set; }
        public DateTime TravelDate { get; set; }
        public List<float> ShortProminency { get; set; }
        public List<int> MetersRight { get; set; }
        public List<int> MetersLeft { get; set; }

        public List<float> ShortNaturRight { get; set; }
        public List<float> ShortNaturLeft { get; set; }
        public List<float> MediumNaturRight { get; set; }
        public List<float> MediumNaturLeft { get; set; }
        public List<float> LongNaturRight { get; set; }
        public List<float> LongNaturLeft { get; set; }

        public List<float> ShortWaveLeft { get; set; }
        public List<float> ShortWaveRight { get; set; }
        public List<float> MediumWaveLeft { get; set; }
        public List<float> MediumWaveRight { get; set; }
        public List<float> LongWaveLeft { get; set; }
        public List<float> LongWaveRight { get; set; }

        public List<float> ImpulseLeft { get; set; }
        public List<float> ImpulseRight { get; set; }

        private float Koef = 1f;
        private float KoefSr = 1f;
        private float KoefDl = 1f;

        public void ParseDB(CrosProf elem)
        {
            MetersRight.Add(elem.Meter);
            ShortNaturRight.Add(elem.Shortwavesright * Koef);
            LongNaturRight.Add(elem.Longwavesright * KoefDl);
            MediumNaturRight.Add(elem.Mediumwavesright * KoefSr);
        }
        public void ParseDB2(CrosProf elem)
        {
            MetersLeft.Add(elem.Meter);
            ShortNaturLeft.Add(elem.Shortwavesleft * Koef);
            LongNaturLeft.Add(elem.Longwavesleft * KoefDl);
            MediumNaturLeft.Add(elem.Mediumwavesleft * KoefSr);
        }
        public void Parse(string line)
        {
            line = Regex.Replace(line, @"\s+", " ");
            var parameters = line.Split(new char[] { ' ', '\t' });
            {
                MetersRight.Add((int.Parse(parameters[0]) ) * 100 + int.Parse(parameters[1]));
                ShortNaturRight.Add(float.Parse(parameters[2], CultureInfo.InvariantCulture.NumberFormat)*Koef);
            }
        }
        public ShortRoughness()
        {
            MetersRight = new List<int>();
            MetersLeft = new List<int>();
            ShortWaveRight = new List<float>();
            ShortWaveLeft = new List<float>();
            LongWaveLeft = new List<float> ();
            LongWaveRight = new List<float> ();
            MediumWaveLeft = new List<float>();
            MediumWaveRight = new List<float>();
            ImpulseLeft = new List<float>();
            ImpulseRight = new List<float>();

            ShortNaturRight = new List<float>();
            ShortNaturLeft = new List<float>();
            LongNaturLeft = new List<float>();
            LongNaturRight = new List<float>();
            MediumNaturLeft = new List<float>();
            MediumNaturRight = new List<float>();
        }

        public void Correct()
        {
            float DimImpuls = 30;
            ImpulseLeft = new List<float>();
            ImpulseRight = new List<float>();
            for (int i = 0; i < ShortNaturLeft.Count; i++)
            {
                if (ShortNaturLeft[i] >= 2)
                {
                    ImpulseLeft.Add(ShortNaturLeft[i] < DimImpuls ? ShortNaturLeft[i] : 0);
                    ShortNaturLeft[i] = 0;
                }
                else
                {
                    ImpulseLeft.Add(0);
                }
            }
            for (int i = 0; i < ShortNaturRight.Count; i++)
            {
                if (ShortNaturRight[i] >= 2)
                {
                    ImpulseRight.Add(ShortNaturRight[i] < DimImpuls ? ShortNaturRight[i] : 0);
                    ShortNaturRight[i] = 0;
                }
                else
                {
                    ImpulseRight.Add(0);
                }
            }
            ShortWaveRight = Enumerable.Range(0, ShortNaturRight.Count).Select(i => ShortNaturRight.Skip(i).Take(4).Average())
                .ToList();
            MediumWaveRight = Enumerable.Range(0, MediumNaturRight.Count).Select(i => MediumNaturRight.Skip(i).Take(8).Average())
                .ToList();
            LongWaveRight = Enumerable.Range(0, LongNaturRight.Count).Select(i => LongNaturRight.Skip(i).Take(16).Average())
                .ToList();
            ShortWaveLeft = Enumerable.Range(0, ShortNaturLeft.Count).Select(i => ShortNaturLeft.Skip(i).Take(4).Average())
                .ToList();
            MediumWaveLeft = Enumerable.Range(0, MediumNaturLeft.Count).Select(i => MediumNaturLeft.Skip(i).Take(8).Average())
                .ToList();
            LongWaveLeft = Enumerable.Range(0, LongNaturLeft.Count).Select(i => LongNaturLeft.Skip(i).Take(16).Average())
                .ToList();
        }

        public int GetRshp(int speed, float value)
        {
            var rrd = RecommendedRoughnessDepths(speed);
            var maxes = new List<float> {Math.Max(ShortWaveLeft.Max(), ShortWaveRight.Max()), Math.Max(MediumWaveLeft.Max(), MediumWaveRight.Max()) , Math.Max(LongWaveLeft.Max(), LongWaveRight.Max()) };
            var max = maxes.Max();
            var min = rrd[maxes.IndexOf(max)];
            return (int)((max  / value) - 1 < 0 ? 0 : (max / value) - 1);

        }

        public List<float[]> GetIntegratedIndicators(int speed)
        {
            float[] CalcIndicators(float[,] lah,  float[] relativityn, List<float> shortwave, List<float> mediumwave, List<float> longwave)
            {
                var left = new float[8];
                
                left[0] = lah[0, 1];
                left[2] = lah[1, 1];
                left[4] = lah[2, 1];

                for (int i = 0; i < shortwave.Count; i++)
                {
                    left[1] += (float)Math.Pow(shortwave[i] - left[0], 2);
                    left[3] += (float)Math.Pow(mediumwave[i] - left[2], 2);
                    left[5] += (float)Math.Pow(longwave[i] - left[4], 2);
                }

                left[1] = (float)Math.Sqrt(left[1] / shortwave.Count);
                left[3] = (float)Math.Sqrt(left[3] / mediumwave.Count);
                left[5] = (float)Math.Sqrt(left[5] / longwave.Count);

                left[6] = (shortwave.Average() * relativityn[0] + mediumwave.Average() * relativityn[1] +
                           longwave.Average() * relativityn[2]) / relativityn.Sum(); //alpha
                left[7] = (lah[0, 0] * relativityn[0] + lah[1, 0] * relativityn[1] + lah[2, 0] * relativityn[2]) /
                          relativityn.Sum(); //alpha
                return left;
            }
            Correct();
            List<float[]> result = new List<float[]>();
            var relativityN = RelativityN(speed);
            var LAh = CalculateLAh(speed);
            result.Add(CalcIndicators(LAh[0], relativityN, ShortWaveLeft, MediumWaveLeft, LongWaveLeft));
            result.Add(CalcIndicators(LAh[1], relativityN, ShortWaveRight, MediumWaveRight, LongWaveRight));

            return result;


        }

       

        private float[] RelativityN(int speed)
        {
            return speed < 200 ? new float[]{ 2, 0.5f, 0.3f} : new float[] { 2, 0.3f, 0.5f };
        }

        private float[] RecommendedRoughnessDepths(int speed)
        {
            switch (speed)
            {
                case int v when v <= 60:
                    return new float[] {0.025f, 0.20f, 0.30f};
                case int v when (v > 60) && (v <= 100):
                    return new float[] { 0.025f, 0.15f, 0.20f };
                case int v when (v > 100) && (v <= 140):
                    return new float[] { 0.025f, 0.10f, 0.15f };
                case int v when (v > 140) && (v <= 200):
                    return new float[] { 0.020f, 0.08f, 0.10f };
                default:
                    return new float[] { 0.020f, 0.06f, 0.08f };
            }
        }

        private float[,] MinMaxRoughnessBySpeed(int speed)
        {
            float[,] result;
            switch (speed)
            {
                case int v when v <= 60:
                    result = new float[,]
                    {
                        {0.05f, 0.6f} , {0.2f, 1.0f}, {0.3f, 1.3f}
                    };
                    break;
                case int v when (v > 60) && (v <= 100):
                    result = new float[,]
                    {
                        {0.04f, 0.5f}, {0.15f, 0.9f }, {0.2f, 1.2f}
                    };
                    break;
                case int v when (v > 100) && (v <= 140):
                    result = new float[,]
                    {
                        { 0.03f, 0.4f }, { 0.10f, 0.8f }, { 0.15f, 1.1f }
                    };
                    break;
                case int v when (v > 140) && (v <= 200):
                    result = new float[,]
                    {
                        { 0.02f, 0.3f }, { 0.08f, 0.7f }, { 0.10f, 1.0f }
                    };
                    break;
                default:
                    result = new float[,]
                    {
                        { 0.01f, 0.2f }, { 0.06f, 0.6f }, { 0.07f, 0.9f }
                    };
                    break;
            }

            return result;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="speed"></param>
        /// <returns></returns>
        public List<float[,]> CalculateLAh(int speed)
        {
            float[,] CalcSide(float[,] minMax, List<float> shortwave, List<float> mediumwave, List<float> longwave)
            {
                float[,] side = new float[3, 2]
                {
                    {0, 0}, {0, 0}, {0, 0}
                };


                for (int i = 0; i < shortwave.Count; i++)
                {
                    if (shortwave[i] >= minMax[0, 0] && shortwave[i] <= minMax[0, 1])
                    {
                        side[0, 0] += 1;
                        side[0, 1] += shortwave[i];
                    }

                    if (mediumwave[i] >= minMax[1, 0] && mediumwave[i] <= minMax[1, 1])
                    {
                        side[1, 0] += 1;
                        side[1, 1] += mediumwave[i];
                    }

                    if (longwave[i] >= minMax[2, 0] && longwave[i] <= minMax[2, 1])
                    {
                        side[2, 0] += 1;
                        side[2, 1] += longwave[i];
                    }
                }


                for (int i = 0; i < 3; i++)
                {
                    side[i, 1] = side[i, 0] !=0 ? side[i, 1] / side[i, 0] : 0;
                }

                return side;
            }

            List<float[,]> result= new List<float[,]>();
            var minMaxRoughness = MinMaxRoughnessBySpeed(speed);
            result.Add(CalcSide(minMaxRoughness, ShortWaveLeft, MediumWaveLeft, LongWaveLeft));
            result.Add(CalcSide(minMaxRoughness, ShortWaveRight, MediumWaveRight, LongWaveRight));
            return result;
        }

        public void Parse2(string line)
        {
            line = Regex.Replace(line, @"\s+", " ");
            var parameters = line.Split(new char[] { ' ', '\t' });
            {
                MetersLeft.Add((int.Parse(parameters[0]) ) * 100 + int.Parse(parameters[1]));
                ShortNaturLeft.Add(float.Parse(parameters[2], CultureInfo.InvariantCulture.NumberFormat)*Koef);
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<Digression> GetDigressions()
        {
            void CalcDegression(List<Digression> digressions, List<int> meters,List<float> longWave, List<float> middleWave, List<float> shortWave, List<float> impulsWave, DigName l, DigName m, DigName s, DigName im)
            {
                bool found = false, middleFound = false, longFound = false, impulsFound = false;
                int defleng = 0, mDefLeng = 0, lDefLeng = 0, iDefLeng = 0;
                int defstart = 0, mDefStart = 0, lDefStart = 0, iDefStart = 0;
                float def = 0, mDef = 0, lDef = 0, iDef = 0;
                float max = 0, mMax = 0, lMax = 0, iMax= 0;
                for (int i = 0; i < meters.Count; i++)
                {
                    if (shortWave[i] > 0.2f)
                    {
                        if (!found)
                        {
                            defstart = meters[i];
                        }

                        defleng += 100;
                        def = shortWave[i];
                        if (def > max)
                            max = def;
                        found = true;
                    }
                    else if (defleng > 200)
                    {
                        digressions.Add(new Digression()
                            {Meter = defstart, Length = defleng, DigName = s, Value = max});
                        found = false;
                        defstart = 0;
                        defleng = 0;
                        max = 0;
                        def = 0;
                    }
                    if (middleWave[i] > 0.45f)
                    {
                        if (!middleFound)
                        {
                            mDefStart = meters[i];
                        }

                        mDefLeng += 100;
                        mDef = middleWave[i];
                        if (mDef > mMax)
                            mMax = mDef;
                        middleFound = true;
                    }
                    else if (mDefLeng >= 1000)                  {
                        digressions.Add(new Digression()
                            {Meter = mDefStart, Length = mDefLeng, DigName = m, Value = mMax });
                        middleFound = false;
                        mDefStart = 0;
                        mDefLeng = 0;
                        mMax = 0;
                        mDef = 0;
                    }
                    if (longWave[i] > 0.6f)
                    {
                        if (!longFound)
                        {
                            lDefStart = meters[i];
                        }
                        lDefLeng += 100;
                        lDef = longWave[i];
                        if (lDef > lMax)
                            lMax = lDef;
                        longFound = true;
                    }
                    else if (lDefLeng >= 2000)  
                    {
                        digressions.Add(new Digression()
                            {Meter = lDefStart, Length = lDefLeng, DigName = l, Value = lMax });
                        longFound = false;
                        lDefStart = 0;
                        lDefLeng = 0;
                        lMax = 0;
                        lDef = 0;
                    }
                    if(impulsWave.Count() != 0)
                    {
                        if (impulsWave[i] > 0.6f)
                        {
                            if (!impulsFound)
                            {
                                iDefStart = meters[i];
                            }
                            iDefLeng += 100;
                            iDef = impulsWave[i];
                            if (iDef > iMax)
                                iMax = iDef;
                            impulsFound = true;
                        }
                        else if ((iDefLeng > 0) && (iDefLeng < 301))
                        {
                            digressions.Add(new Digression()
                                { Meter = iDefStart, Length = iDefLeng, DigName = im, Value = iMax });
                            impulsFound = false;
                            iDefStart = 0;
                            iDefLeng = 0;
                            iMax = 0;
                            iDef = 0;
                        }
                    }
                }
            }
            var result = new List<Digression>();
            CalcDegression(result, MetersRight, LongWaveRight, MediumWaveRight, ShortWaveRight, ImpulseRight, DigressionName.LongWaveRight, DigressionName.MiddleWaveRight, DigressionName.ShortWaveRight, DigressionName.ImpulsRight);
            CalcDegression(result, MetersLeft, LongWaveLeft, MediumWaveLeft, ShortWaveLeft, ImpulseLeft, DigressionName.LongWaveLeft, DigressionName.MiddleWaveLeft, DigressionName.ShortWaveLeft, DigressionName.ImpulsLeft);
            return result;
        }
        public List<Digression> GetDigressions_new(int km)
        {
            void CalcDegression(List<Digression> digressions, List<int> meters,  List<float> longWave, List<float> middleWave, List<float> shortWave, List<float> impulsWave, DigName l, DigName m, DigName s, DigName im)
            {
                bool found = false, middleFound = false, longFound = false, impulsFound = false;
                int defleng = 0, mDefLeng = 0, lDefLeng = 0, iDefLeng = 0;
                int defstart = 0, mDefStart = 0, lDefStart = 0, iDefStart = 0;
                float def = 0, mDef = 0, lDef = 0, iDef = 0;
                float max = 0, mMax = 0, lMax = 0, iMax = 0;
               
                for (int i = 0; i < meters.Count; i++)
                {
                    if (shortWave.Count() < 1)
                    {
                        continue;
                    }

                    if (shortWave[i] > 0.2f)
                    {
                        if (!found)
                        {
                            defstart = meters[i];
                        }

                        defleng += 100;
                        def = shortWave[i];
                        if (def > max)
                            max = def;
                        found = true;
                    }
                    else if (defleng > 200)
                    {
                        digressions.Add(new Digression()
                        { Meter = defstart, Length = defleng, DigName = s, Value = max, Km = km });
                        found = false;
                        defstart = 0;
                        defleng = 0;
                        max = 0;
                        def = 0;
                    }
                    if (middleWave[i] > 0.45f)
                    {
                        if (!middleFound)
                        {
                            mDefStart = meters[i];
                        }

                        mDefLeng += 100;
                        mDef = middleWave[i];
                        if (mDef > mMax)
                            mMax = mDef;
                        middleFound = true;
                    }
                    else if (mDefLeng >= 1000)
                    {
                        digressions.Add(new Digression()
                        { Meter = mDefStart, Length = mDefLeng, DigName = m, Value = mMax, Km = km });
                        middleFound = false;
                        mDefStart = 0;
                        mDefLeng = 0;
                        mMax = 0;
                        mDef = 0;
                    }
                    if (longWave[i] > 0.6f)
                    {
                        if (!longFound)
                        {
                            lDefStart = meters[i];
                        }
                        lDefLeng += 100;
                        lDef = longWave[i];
                        if (lDef > lMax)
                            lMax = lDef;
                        longFound = true;
                    }
                    else if (lDefLeng >= 2000)
                    {
                        digressions.Add(new Digression()
                        { Meter = lDefStart, Length = lDefLeng, DigName = l, Value = lMax, Km = km });
                        longFound = false;
                        lDefStart = 0;
                        lDefLeng = 0;
                        lMax = 0;
                        lDef = 0;
                    }
                    if (impulsWave.Count() != 0)
                    {
                        if (impulsWave[i] > 0.6f)
                        {
                            if (!impulsFound)
                            {
                                iDefStart = meters[i];
                            }
                            iDefLeng += 100;
                            iDef = impulsWave[i];
                            if (iDef > iMax)
                                iMax = iDef;
                            impulsFound = true;
                        }
                        else if ((iDefLeng > 0) && (iDefLeng < 301))
                        {
                            digressions.Add(new Digression()
                            { Meter = iDefStart, Length = iDefLeng, DigName = im, Value = iMax, Km = km });
                            impulsFound = false;
                            iDefStart = 0;
                            iDefLeng = 0;
                            iMax = 0;
                            iDef = 0;
                        }
                    }
                }
            }
            var result = new List<Digression>();
            CalcDegression(result, MetersRight, LongWaveRight, MediumWaveRight, ShortWaveRight, ImpulseRight, DigressionName.LongWaveRight, DigressionName.MiddleWaveRight, DigressionName.ShortWaveRight, DigressionName.ImpulsRight);
            CalcDegression(result, MetersLeft, LongWaveLeft, MediumWaveLeft, ShortWaveLeft, ImpulseLeft, DigressionName.LongWaveLeft, DigressionName.MiddleWaveLeft, DigressionName.ShortWaveLeft, DigressionName.ImpulsLeft);
            return result;
        }

    }
}
