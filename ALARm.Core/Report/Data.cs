using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALARm.Core.Report
{
    /// <summary>
    /// Характеристики кривой
    /// </summary>
    public class Data
    {
        public Data(List<int> x, List<float> plan, List<float> level, List<float> gauge, List<float> passBoost, List<float> freightBoost, List<int> passSpeed, List<int> freightSpeed, int transitionLength1, int transitionLength2)
        {
            if (x.Count > 0)
            {
                X = x.ToArray();
                Plan = plan.ToArray();
                Level = level.ToArray();
                Gauge = gauge.ToArray();
                PassSpeed = passSpeed.ToArray();
                FreightSpeed = freightSpeed.ToArray();
                PassBoost = passBoost.ToArray();
                FreightBoost = freightBoost.ToArray();
                deltaX1 = transitionLength1;
                deltaX2 = transitionLength2;
                GetK();
            }
        }

        public Data(){}
        private int deltaX1;
        private int deltaX2;
        private int[] X;
        private float[] Plan;
        private float[] Gauge;

        private int[] PassSpeed;
        private int[] FreightSpeed;
        private float[] PassBoost;
        private float[] FreightBoost;
        private float[] Level;

        private float[] KPlanCurrent;
        private float[] KPlan;
        private int[] KPlanIndex;
        private int[] KPlanLength;
        private float[] K2Plan;
        private int[] K2IndexPlan;
        private int[] K2LengthPlan;

        private float[] KLevelCurrent;
        private float[] KLevel;
        private int[] KLevelIndex;
        private int[] KLevelLength;
        private float[] K2Level;
        private int[] K2IndexLevel;
        private int[] K2LengthLevel;
        private float[] CalculatedPassBoost;
        private float[] CalculatedFreightBoost;

        /// <summary>
        /// Начало натурной кривой
        /// </summary>
        public int X0IndexPlan {
            get { return X1IndexPlan - X1LengthPlan; }
        }
        /// <summary>
        /// Начало натурной круговой кривой
        /// </summary>
        public int X1IndexPlan {
            get { return KPlanIndex[Array.IndexOf(KPlan, KPlan.Max())]; }
        }
        /// <summary>
        /// Конец натурной круговой кривой
        /// </summary>
        public int X2IndexPlan {
            get { return K2IndexPlan[Array.IndexOf(K2Plan, K2Plan.Max())]; }
        }
        /// <summary>
        /// Конец кривой
        /// </summary>
        public int X3IndexPlan {
            get { return X2IndexPlan + X2LengthPlan; }
        }

        public float Y0IndexPlan {
            get { return Plan[X0IndexPlan]; }
        }
        public float Y1IndexPlan {
            get { return Plan[X1IndexPlan]; }
        }

        public float Y2IndexPlan {
            get { return Plan[X2IndexPlan]; }
        }
        public float Y3IndexPlan {
            get { return Plan[X3IndexPlan]; }
        }

        public int X1LengthPlan {
            get { return KPlanLength[Array.IndexOf(KPlan, KPlan.Max())]; }
        }
        public int X2LengthPlan {
            get { return K2LengthPlan[Array.IndexOf(K2Plan, K2Plan.Max())]; }
        }

        public int X0IndexLevel {
            get { return X1IndexLevel - X1LengthLevel; }
        }
        public int X1IndexLevel {
            get { return KLevelIndex[Array.IndexOf(KLevel, KLevel.Max())]; }
        }
        public int X2IndexLevel {
            get { return K2IndexLevel[Array.IndexOf(K2Level, K2Level.Max())]; }
        }

        public int X3IndexLevel {
            get { return X2IndexLevel + X2LengthLevel; }
        }

        public float Y0IndexLevel {
            get { return Level[ X0IndexLevel]; }
        }
        public float Y1IndexLevel {
            get { return Level[X1IndexLevel]; }
        }

        public float Y2IndexLevel {
            get { return Level[X2IndexLevel]; }
        }
        public float Y3IndexLevel {
            get { return Level[X3IndexLevel]; }
        }




        public int X1LengthLevel {
            get { return KPlanLength[Array.IndexOf(KLevel, KLevel.Max())]; }
        }
        public int X2LengthLevel {
            get { return K2LengthLevel[Array.IndexOf(K2Level, K2Level.Max())]; }
        }

        public float[] PlanHead;
        public float[] LevelHead;
        public float[] GaugeHead;


        //ASlan мегял min na max v getmin plan and min na max v getmaxplan
        public int GetMinPlan()
        {
            return Convert.ToInt32(17860 / PlanHead.Max());
        }

        public int GetMaxPlan()
        {
            System.Console.WriteLine(PlanHead.Min().ToString());
            return Convert.ToInt32(17860 / (PlanHead.Min()+0.0001));
        }

        public int GetAvgPlan()
        {
            return Convert.ToInt32(17860 /( PlanHead.Average() + 0.0000001));
        }
        public int GetMinPlan(List<RDCurve> t)
        {
            return Convert.ToInt32(17860 / (t.Select(o=>Math.Abs(o.Radius)).Max() + 0.0000001));
            //return (int)(t.Select(o => o.Radius).Min());
        }

        public int GetMaxPlan(List<RDCurve> t)
        {
            var temp = t.Select(o => Math.Abs(o.Radius)).Any() ? t.Select(o => Math.Abs(o.Radius)).Min() + 1 : 1;
            return Convert.ToInt32(17860 / temp);
            //return (int)(t.Select(o => o.Radius).Max());
        }

        public int GetAvgPlan(List<RDCurve> t)
        {
            return (int)(17860 / (Math.Abs(t.Select(o => o.Radius).Average()) + 0.000001));
            //return (int)(t.Select(o => o.Radius).Average());
        }

        public int Get6mmWear()
        {
            return GaugeHead.Where(w => w > 6.0).Count();
        }

        public int Get10mmWear()
        {
            return GaugeHead.Where(w => w > 10.0).Count();
        }

        public int Get15mmWear()
        {
            return GaugeHead.Where(w => w > 15.0).Count();
        }

        public float GetMaxWear()
        {
            return GaugeHead.Max();
        }

        public float GetAvgWear()
        {
            return GaugeHead.Average();
        }

        public int GetMinGauge()
        {
            return Convert.ToInt32(GaugeHead.Max());
        }

        public int GetMaxGauge()
        {
            return Convert.ToInt32(GaugeHead.Min());
        }

        public int GetAvgGauge()
        {
            return Convert.ToInt32(GaugeHead.Average());
        }

        public int GetMinLevel()
        {
            return Convert.ToInt32(LevelHead.Min());
        }

        public int GetMaxLevel()
        {
            return Convert.ToInt32(LevelHead.Max());
        }

        public int GetAvgLevel()
        {
            return Convert.ToInt32(LevelHead.Average());
        }

        public int GetMinLevel(List<RDCurve> t)
        {
            return Convert.ToInt32(t.Select(o=> Math.Abs(o.Level)).Min());
        }

        public int GetMaxLevel(List<RDCurve> t)
        {
            return Convert.ToInt32(t.Select(o => Math.Abs(o.Level)).Max());
        }

        public int GetAvgLevel(List<RDCurve> t)
        {
            return (int)(t.Select(o => o.Level).Average());
        }

        public float GetRanp { get { return Ranp; } }
        public string GetRanpDegree { 
            get {
                switch(Ranp)
                {
                    case float R when (R < 0.3f):
                        return "-";
                    case float R when (R >= 0.3f && R <= 0.4f):
                        return "1";
                    case float R when (R >= 0.4f && R <= 0.6f):
                        return "2";
                    case float R when (R > 0.6f):
                        return "3";
                    default:
                        return "-";
                }
            } 
        }
        private float Ranp { get; set; }
        public float GetRpl { get { return Rpl; } }
        public string GetRplDegree {
            get {
                switch (Rpl)
                {
                    case float R when (R < 0.2f):
                        return "-";
                    case float R when (R >= 0.2f && R <= 0.3f):
                        return "1";
                    case float R when (R >= 0.3f && R <= 0.5f):
                        return "2";
                    case float R when (R > 0.5f):
                        return "3";
                    default:
                        return "-";
                }
            }
        }
        private float Rpl { get; set; }
        public float GetRlevel { get { return Rlevel; } }
        public string GetRlevelDegree {
            get {
                switch (Rlevel)
                {
                    case float R when (R < 1f):
                        return "-";
                    case float R when (R >= 1f && R <= 2f):
                        return "1";
                    case float R when (R >= 2f && R <= 3f):
                        return "2";
                    case float R when (R > 3f):
                        return "3";
                    default:
                        return "-";
                }
            }
        }
        private float Rlevel { get; set; }
        public float GetRdelta { get { return Rdelta; } }
        public string GetRdeltaDegree {
            get {
                switch (Rdelta)
                {
                    case float R when (R < 1f):
                        return "-";
                    case float R when (R >= 1f && R <= 1.5f):
                        return "1";
                    case float R when (R >= 1.5f && R <= 3f):
                        return "2";
                    case float R when (R > 3f):
                        return "3";
                    default:
                        return "-";
                }
            }
        }
        private float Rdelta { get; set; }

        public float GetR()
        {
            Ranp = (PassBoost.Max() - PassBoost.Average()) / 0.7f;
            float K = 1;
            switch (GetAvgPlan())
            {
                case int radius when (radius <= 1200):
                    K = 1;
                    break;
                case int radius when (radius > 1200 && radius <= 3000):
                    K = 0.5f;
                    break;
                case int radius when (radius > 1200 && radius <= 3000):
                    K = 0.5f;
                    break;
                case int radius when (radius > 1200 && radius <= 3000 && PassSpeed[0] > 140):
                    K = 0.2f;
                    break;
                case int radius when (radius > 3000 && PassSpeed[0] <= 140):
                    K = 0f;
                    break;
            }

            Rpl = K * (GetMaxPlan() / GetMinPlan() - 1);
            
            Rlevel = (GetMaxLevel() - GetMinLevel()) / 10;
            int l1 = Math.Abs(X0IndexPlan - X0IndexLevel) + Math.Abs(X1IndexPlan - X1IndexLevel);
            int l2 = Math.Abs(X2IndexPlan - X2IndexLevel) + Math.Abs(X3IndexPlan - X3IndexLevel);
            int deltal = l1 > l2 ? l1 : l2;
            Rdelta = deltal / 20f;
            return Ranp + 0.5f * Rpl + 0.25f * Rlevel + 0.20f * Rdelta;
        }
        /// <summary>
        /// первый переходной кривой
        /// </summary>
        private float[] PlanLeft;
        private float[] LevelLeft;
        /// <summary>
        /// второй переходной кривой
        /// </summary>
        private float[] PlanRight;
        private float[] LevelRight;

        /// <summary>
        /// уклон отвода кривизны для первой переходной
        /// </summary>
        private float[] RetractionSlopePlanLeft;
        private float[] RetractionSlopePlanRight;
        private float[] RetractionSlopeLevelLeft;
        private float[] RetractionSlopeLevelRight;
        private long[] CriticalSpeed, CriticalSpeed03up, CriticalSpeed03down;

        /// <summary>
        /// скорость изменения непогашенного ускорения
        /// </summary>
        private float[] BoostChangeRate;

        private float[] transitionBoost1, transitionBoost2;
        private int tableP1(int speed)
        {
            return speed > 140 ? 40 : speed > 60 ? 30 : 20;
        }

        private int[] TableOfRetractionSlopeSpeed(float reatraction)
        {

            switch (reatraction)
            {
                case float n when (n <= 0.7f):
                    return new int[] { 250, 90 };
                case float n when (n > 0.7f && n <= 0.8f):
                    return new int[] { 220, 90 };
                case float n when (n > 0.8f && n <= 0.9f):
                    return new int[] { 200, 90 };
                case float n when (n > 0.9f && n <= 1.0f):
                    return new int[] { 180, 90 };
                case float n when (n > 1.0f && n <= 1.1f):
                    return new int[] { 160, 90 };
                case float n when (n > 1.1f && n <= 1.2f):
                    return new int[] { 140, 90 };
                case float n when (n > 1.2f && n <= 1.4f):
                    return new int[] { 120, 90 };
                case float n when (n > 1.4f && n <= 1.5f):
                    return new int[] { 110, 90 };
                case float n when (n > 1.5f && n <= 1.6f):
                    return new int[] { 100, 90 };
                case float n when (n > 1.6f && n <= 1.7f):
                    return new int[] { 95, 85 };
                case float n when (n > 1.7f && n <= 1.8f):
                    return new int[] { 90, 80 };
                case float n when (n > 1.8f && n <= 1.9f):
                    return new int[] { 85, 80 };
                case float n when (n > 1.9f && n <= 2.1f):
                    return new int[] { 80, 75 };
                case float n when (n > 2.1f && n <= 2.3f):
                    return new int[] { 75, 70 };
                case float n when (n > 2.3f && n <= 2.5f):
                    return new int[] { 70, 65 };
                case float n when (n > 2.5f && n <= 2.7f):
                    return new int[] { 65, 60 };
                case float n when (n > 2.7f && n <= 2.9f):
                    return new int[] { 55, 55 };
                case float n when (n > 2.9f && n <= 3.0f):
                    return new int[] { 50, 50 };
                case float n when (n > 3.0f && n <= 3.1f):
                    return new int[] { 40, 40 };
                case float n when (n > 3.1f && n <= 3.2f):
                    return new int[] { 25, 25 };
                default:
                    return new int[] { 0, 0 };

            }


        }

        public float GetPlanLeftMaxRetractionSlope()
        {

            return RetractionSlopePlanLeft.Max();
        }

        public float GetPlanLeftAvgRetractionSlope()
        {
            return RetractionSlopePlanLeft.Average();
        }
        
        public float GetPlanRightMaxRetractionSlope()
        {
            return RetractionSlopePlanRight.Max();
        }

        public float GetPlanRightAvgRetractionSlope()
        {
            return RetractionSlopePlanRight.Average();
        }

        public float GetLevelLeftMaxRetractionSlope()
        {

            return RetractionSlopeLevelLeft.Max();
        }

        public float GetLevelLeftAvgRetractionSlope()
        {
            return RetractionSlopeLevelLeft.Average();
        }
        public float GetLevelRightMaxRetractionSlope()
        {
            return RetractionSlopeLevelRight.Max();
        }

        public float GetLevelRightAvgRetractionSlope()
        {
            return RetractionSlopeLevelRight.Average();
        }

        // следующие 4 функции написала Анияр
        public float GetPlanMaxRetractionSlope(List<RDCurve> rdcs, int n)
        {
            float max = 0;
            float trapmax = 0;
            if (rdcs.Count < n)
            {
                n = rdcs.Count/3;
            }
            for (int i=0; i< rdcs.Count - n; i++)
            {
                float trap = (float)(Math.Abs(rdcs[n + i].Trapez_str - rdcs[i].Trapez_str) / n);
                float temp = (float)Math.Abs((Math.Abs(rdcs[n + i].Radius - rdcs[i].Radius )/n - trap));
                if (temp > max)
                {
                    max = temp;
                }
                if (trap > trapmax)
                {
                    trapmax = trap;
                }
            }
            return (trapmax + max)*2;
        }

        public float GetPlanAvgRetractionSlope(List<RDCurve> rdcs, double len)
        {
            var max_rad = rdcs.Select(o => Math.Abs(o.Radius)).Max();
            return (float)(2 * max_rad / Math.Abs(len));
        }

        public float GetLvlMaxRetractionSlope(List<RDCurve> rdcs, int n)
        {
            float max = 0;
            for (int i = 0; i < rdcs.Count - n; i++)
            {
                float temp = (float)(Math.Abs(rdcs[n + i].Level - rdcs[i].Level) / n);
                if (temp > max)
                {
                    max = temp;
                }
            }
            return max ;
        }

        public float GetLvlAvgRetractionSlope(List<RDCurve> rdcs, double len)
        {
            var max_lvl = rdcs.Select(o => Math.Abs(o.Level)).Max();
            return (float)(max_lvl / Math.Abs(len));
        }


        public float GetPlanLeftMaxRetractionSlope(List<RDCurve> t)
        {
            var max_val = t.Select(o => Math.Abs(o.Radius)).Max();
            return max_val / t.Count;
        }
        public float GetPlanLeftAvgRetractionSlope(List<RDCurve> t)
        {
            var avg_val = t.Select(o => Math.Abs(o.Radius)).Average();
            return avg_val / t.Count;
        }
        public float GetPlanRightMaxRetractionSlope(List<RDCurve> t)
        {
            var max_val = t.Select(o => Math.Abs(o.Radius)).Max();
            return max_val / t.Count;
        }
        public float GetPlanRightAvgRetractionSlope(List<RDCurve> t)
        {
            var avg_val = t.Select(o => Math.Abs(o.Radius)).Average();
            return avg_val / t.Count;
        }

        public float GetLevelLeftMaxRetractionSlope(List<RDCurve> t)
        {
            var max_val = t.Select(o => Math.Abs(o.Level)).Max();
            return max_val / t.Count;
        }
        public float GetLevelLeftAvgRetractionSlope(List<RDCurve> t)
        {
            var avg_val = t.Select(o => Math.Abs(o.Level)).Average();
            return avg_val / t.Count;
        }
        public float GetLevelRightMaxRetractionSlope(List<RDCurve> t)
        {
            var max_val = t.Select(o => Math.Abs(o.Level)).Max();
            return max_val / t.Count;
        }
        public float GetLevelRightAvgRetractionSlope(List<RDCurve> t)
        {
            var avg_val = t.Select(o => Math.Abs(o.Level)).Average();
            return avg_val / t.Count;
        }
        /// <summary>
        /// непогашенное ускорение
        /// </summary>
        public float GetUnliquidatedAccelerationPassengerMax()
        {
            return PassBoost.Max();
        }

        public int GetUnliquidatedAccelerationPassengerMaxCoordinate()
        {
            return X[Array.IndexOf(PassBoost, GetUnliquidatedAccelerationPassengerMax())] % 1000;
        }

        public float GetUnliquidatedAccelerationPassengerAvg()
        {
            return PassBoost.Average();
        }

        public float GetUnliquidatedAccelerationFreightMax()
        {
            return FreightBoost.Max();
        }

        public float GetUnliquidatedAccelerationFreightAvg()
        {
            return FreightBoost.Average();
        }
        public float BoostChangeRateMax()
        {
            return BoostChangeRate.Max();
        }

        public int GetIZPassSpeed(List<RDCurve> T)
        {
            float psi = PassBoost[0] > 140 ? 0.4f : 0.6f;

            int speed = (int)((psi / Math.Abs(T.Select(o=>o.LastochkaBoost).Max()) * 3.6 * tableP1(PassSpeed[0])));
            return speed;
        }
        public int GetIZPassSpeed()
        {
            //float psi = PassBoost[0] > 140 ? 0.4f : 0.6f;

            //int speed = (int)((psi / PassBoost.Max() * 3.6 * tableP1(PassSpeed[0])));

            var dl = tableP1(PassSpeed[0]);
            var temp = new List<double> { };
            if (dl < PassBoost.Count())
            {
                for (int i = 0; i < PassBoost.Count() - dl; i++)
                {
                    var t = Math.Abs(PassBoost[i + dl] - PassBoost[i]);

                    temp.Add(t);
                }
            }
            if (dl >= PassBoost.Count())
            {
                //for (int i1 = dl; i1 < dl*2 - PassBoost.Count() ; i1++)
                //{
                //    int i = i1 - dl;
                //    var t = Math.Abs(PassBoost[i+dl] - PassBoost[i1]);

                //    temp.Add(t);
                //}
                var t = Math.Abs(PassBoost[PassBoost.Count()-1] - PassBoost[0])/ PassBoost.Count() * dl;
                temp.Add(t);

            }



            var Viz = (3.6 * dl *0.6) / temp.Max();
            

            return (int)Viz;
        }
        public int GetIZSpeed(double [] T, int speed)
        {
            float psi = PassBoost[0] > 140 ? 0.4f : 0.6f;
            int s = (int)((psi / T.Max() * 3.6 * tableP1(speed)));
            return s;
        }
        public int GetIZFreightSpeed()
        {
            //float psi = 0.6f;

            float psi = PassBoost[0];
            int speed = (int)((psi / PassBoost.Max() * 3.6 * tableP1(FreightSpeed[0])));
            return speed;
        }

        public int BoostChangeRateMaxCoordinate()
        {
            int index = Array.IndexOf(BoostChangeRate, BoostChangeRateMax());
            int temp_index = 0;
            if (index < BoostChangeRate.Length - 2)
                return X[index] % 1000;
            if (index == BoostChangeRate.Length - 2)
            {
                temp_index = Array.IndexOf(X, X0IndexLevel) + Array.IndexOf(transitionBoost1, transitionBoost1.Max());
                if ((temp_index < X.Length) && (temp_index>-1))
                    return X[Array.IndexOf(X, X0IndexLevel) + Array.IndexOf(transitionBoost1, transitionBoost1.Max())] % 1000;
                else
                    return 0;
            }
            else
            {
                temp_index = Array.IndexOf(X, X2IndexLevel) + Array.IndexOf(transitionBoost2, transitionBoost2.Max());
                if ((temp_index < X.Length) && (temp_index > -1))
                    return X[temp_index] % 1000; else
                    return 0;

            }
        }






        private void GetK()
        {


            int headCount = Convert.ToInt32((X.Length / 2) / 3.5);
            float[] planClone = (float[])Plan.Clone();
            float[] planHead = new float[headCount];



            float[] levelClone = (float[])Level.Clone();


            float[] leftClone = new float[X.Length / 2];
            Array.Copy(planClone, 0, leftClone, 0, X.Length / 2);
            Array.Sort(leftClone);
            Array.Copy(leftClone, leftClone.Length - headCount, planHead, 0, headCount);
            float leftHeadAverage = planHead.Average() - planHead.Average() * 0.04f;
            var minPlanLeft = leftClone.Min() + 0.5;

            leftClone = new float[X.Length / 2];
            Array.Copy(planClone, planClone.Length / 2, leftClone, 0, planClone.Length / 2);
            Array.Sort(leftClone);
            Array.Copy(leftClone, leftClone.Length - headCount, planHead, 0, headCount);
            float rightHeadAverage = planHead.Average() - planHead.Average() * 0.04f;
            var minPlanRight = leftClone.Min() + 0.5;

            leftClone = new float[X.Length / 2];
            Array.Copy(levelClone, 0, leftClone, 0, X.Length / 2);
            Array.Sort(leftClone);
            Array.Copy(leftClone, leftClone.Length - headCount, planHead, 0, headCount);
            float leftHeadAverageLevel = planHead.Average() - planHead.Average() * 0.05f;
            var minLevelLeft = leftClone.Min() + 0.5;

            leftClone = new float[X.Length / 2];
            Array.Copy(levelClone, planClone.Length / 2, leftClone, 0, levelClone.Length / 2);
            Array.Sort(leftClone);
            Array.Copy(leftClone, leftClone.Length - headCount, planHead, 0, headCount);
            float rightHeadAverageLevel = planHead.Average() - planHead.Average() * 0.05f;
            var minLevekRight = leftClone.Min() + 0.5;

            int lCount = deltaX1 > deltaX2 ? deltaX1 : deltaX2;



            KPlan = new float[lCount];
            KPlanIndex = new int[lCount];
            KPlanLength = new int[lCount];

            KLevel = new float[lCount];
            KLevelIndex = new int[lCount];
            KLevelLength = new int[lCount];

            K2Plan = new float[lCount];
            K2IndexPlan = new int[lCount];
            K2LengthPlan = new int[lCount];

            K2Level = new float[lCount];
            K2IndexLevel = new int[lCount];
            K2LengthLevel = new int[lCount];


            var minLevel = Level.Min() + 1.5;

            for (int lIndex = 0; lIndex < lCount; lIndex++)
            {
                KPlanCurrent = new float[X.Length / 2];
                KLevelCurrent = new float[X.Length / 2];

                var lcurrent = lIndex + lCount / 3;
                if (KPlanCurrent.Length + lcurrent >= Plan.Length)
                    lcurrent = Plan.Length - KPlanCurrent.Length;

                for (int kIndex = 0; kIndex < KPlanCurrent.Length; kIndex++)
                {
                    if ((Plan[kIndex + lcurrent] >= leftHeadAverage) && (Plan[kIndex] <= minPlanLeft))
                        KPlanCurrent[kIndex] = Math.Abs(Plan[kIndex + lcurrent] - Plan[kIndex]) / lcurrent;

                    if ((Level[kIndex + lcurrent] >= leftHeadAverageLevel) && (Level[kIndex] <= minLevelLeft))
                        KLevelCurrent[kIndex] = Math.Abs(Level[kIndex + lcurrent] - Level[kIndex]) / lcurrent;
                }

                KPlan[lIndex] = KPlanCurrent.Max();
                KPlanIndex[lIndex] = Array.IndexOf(KPlanCurrent, KPlan[lIndex]) + lcurrent;
                KPlanLength[lIndex] = lcurrent;

                KLevel[lIndex] = KLevelCurrent.Max();
                KLevelIndex[lIndex] = Array.IndexOf(KLevelCurrent, KLevel[lIndex]) + lcurrent;
                KLevelLength[lIndex] = lcurrent;

                KPlanCurrent = new float[X.Length / 2];
                KLevelCurrent = new float[X.Length / 2];

                for (int kIndex = KPlanCurrent.Length; kIndex < X.Length - lcurrent; kIndex++)
                {
                    if ((Plan[kIndex] >= rightHeadAverage) && (Plan[kIndex + lcurrent] <= minPlanRight))
                        KPlanCurrent[kIndex - KPlanCurrent.Length] =
                            Math.Abs(Plan[kIndex] - Plan[kIndex + lcurrent]) / lcurrent;

                    if ((Level[kIndex] >= rightHeadAverageLevel) && (Level[kIndex + lcurrent] <= minLevekRight))
                        KLevelCurrent[kIndex - KLevelCurrent.Length] =
                            Math.Abs(Level[kIndex] - Level[kIndex + lcurrent]) / lcurrent;
                }

                K2Plan[lIndex] = KPlanCurrent.Max();
                K2IndexPlan[lIndex] = Array.IndexOf(KPlanCurrent, K2Plan[lIndex]) + KPlanCurrent.Length;
                K2LengthPlan[lIndex] = lcurrent;

                K2Level[lIndex] = KLevelCurrent.Max();
                K2IndexLevel[lIndex] = Array.IndexOf(KLevelCurrent, K2Level[lIndex]) + KLevelCurrent.Length;
                K2LengthLevel[lIndex] = lcurrent;
            }

            PlanLeft = new float[X1LengthPlan];
            Array.Copy(Plan, X0IndexPlan, PlanLeft, 0, X1LengthPlan);
            PlanRight = new float[X2LengthPlan];
            Array.Copy(Plan, X2IndexPlan, PlanRight, 0, X2LengthPlan);
            LevelLeft = new float[X1LengthLevel];
            Array.Copy(Level, X0IndexLevel, LevelLeft, 0, X1LengthLevel);
            LevelRight = new float[X2LengthLevel];
            Array.Copy(Level, X2IndexLevel, LevelRight, 0, X2LengthLevel);


            PlanHead = new float[Math.Abs(X2IndexPlan - X1IndexPlan)];


            Array.Copy(Plan, X1IndexPlan > X2IndexPlan ? X2IndexPlan : X1IndexPlan, PlanHead, 0, PlanHead.Length);
            LevelHead = new float[Math.Abs(X2IndexLevel - X1IndexLevel)];
            Array.Copy(Level, X1IndexLevel > X2IndexLevel ? X2IndexLevel : X1IndexLevel, LevelHead, 0, LevelHead.Length);
            GaugeHead = new float[Math.Abs(X2IndexPlan - X1IndexPlan)];
            Array.Copy(Gauge, X1IndexPlan > X2IndexPlan ? X2IndexPlan : X1IndexPlan, GaugeHead, 0, GaugeHead.Length);
            //расчет уклона отвода для переходных кривых
            RetractionSlopePlanLeft = new float[PlanLeft.Length - 1];
            RetractionSlopePlanRight = new float[PlanRight.Length - 1];
            RetractionSlopeLevelLeft = new float[LevelLeft.Length - 1];
            RetractionSlopeLevelRight = new float[LevelRight.Length - 1];

            for (int index = 1; index < PlanLeft.Length; index++)
            {
                RetractionSlopePlanLeft[index - 1] = Math.Abs(PlanLeft[0] - PlanLeft[index]) / index;
            }
            for (int index = 1; index < PlanRight.Length; index++)
            {
                RetractionSlopePlanRight[index - 1] = Math.Abs(PlanRight[0] - PlanRight[index]) / index;
            }

            for (int index = 1; index < LevelLeft.Length; index++)
            {
                RetractionSlopeLevelLeft[index - 1] = Math.Abs(LevelLeft[0] - LevelLeft[index]) / index;
            }
            for (int index = 1; index < LevelRight.Length; index++)
            {
                RetractionSlopeLevelRight[index - 1] = Math.Abs(LevelRight[0] - LevelRight[index]) / index;
            }

            int deltaL = tableP1(PassSpeed[0]);
            if (X.Length - deltaL + 2 > 1)
            {
                BoostChangeRate = new float[X.Length - deltaL + 2];
                CalculatedPassBoost = new float[X.Length - deltaL];
                CalculatedFreightBoost = new float[X.Length - deltaL];
           
          
          
            for (int index = 0; index < BoostChangeRate.Length - 2- deltaL; index++)
            {
                BoostChangeRate[index] = (Math.Abs(PassBoost[index] - PassBoost[index + deltaL]) * PassSpeed[0]) / (3.6f * deltaL);
                CalculatedPassBoost[index] = Math.Abs(PassBoost[index] - PassBoost[index + deltaL]);
                CalculatedFreightBoost[index] = Math.Abs(FreightBoost[index] - FreightBoost[index + deltaL]);
            }
            }

            if (X.Length - deltaL + 2 < 1)
            {
                BoostChangeRate = new float[-X.Length + deltaL - 2];
                CalculatedPassBoost = new float[-X.Length + deltaL];
                CalculatedFreightBoost = new float[-X.Length +deltaL];



                for (int index1= deltaL - 2; index1 < BoostChangeRate.Length  ; index1++)
                {
                  int   index= index1 -deltaL + 2;
                    BoostChangeRate[index] = (Math.Abs(PassBoost[index] - PassBoost[index - deltaL]) * PassSpeed[0]) / (3.6f * deltaL);
                    CalculatedPassBoost[index] = Math.Abs(PassBoost[index] - PassBoost[index - deltaL]);
                    CalculatedFreightBoost[index] = Math.Abs(FreightBoost[index] - FreightBoost[index - deltaL]);
                }
            }




            transitionBoost1 = new float[X1LengthLevel];
            transitionBoost2 = new float[X2LengthLevel];
            Array.Copy(PassBoost, X0IndexLevel, transitionBoost1, 0, X1LengthLevel);
            BoostChangeRate[BoostChangeRate.Length - 2] = transitionBoost1.Max() * PassSpeed[0] / (3.6f * X1LengthLevel);
            Array.Copy(PassBoost, X2IndexLevel, transitionBoost2, 0, X2LengthLevel);
            BoostChangeRate[BoostChangeRate.Length - 1] = transitionBoost2.Max() * PassSpeed[0] / (3.6f * X2LengthLevel);

            CriticalSpeed = new long[X.Length];
            CriticalSpeed03up = new long[X.Length];
            CriticalSpeed03down = new long[X.Length];
            for (int index = 0; index < X.Length; index++)
            {
                CriticalSpeed[index] = (int)(Math.Sqrt(13 * (17860.0 / (Plan[index] != 0 ? Plan[index] : 0.0001)) * (0.7 + 0.0061 * Level[index])));
                CriticalSpeed03up[index] = (int)(Math.Sqrt(13 * (17860 / (Plan[index] != 0 ? Plan[index] : 0.0001)) * (0.3 + 0.0061 * Level[index])));
                CriticalSpeed03down[index] = (int)(Math.Sqrt(Math.Abs(13 * (17860 / (Plan[index] != 0 ? Plan[index] : 0.0001)) * (-0.3 + 0.0061 * Level[index]))));
            }
        }

        public int GetPassSpeed()
        {
            return PassSpeed[0];
        }
        public int GetFreightSpeed()
        {
            return FreightSpeed[0];
        }

        public int GetCriticalSpeed()
        {
            return (int)CriticalSpeed.Min();
        }
        public int GetCriticalSpeed03up()
        {
            return (int)CriticalSpeed03up.Min();
        }
        public int GetCriticalSpeed03down()
        {
            return (int)CriticalSpeed03down.Min();
        }
        public int[] GetPRSpeed()
        {
            return TableOfRetractionSlopeSpeed(RetractionSlopeLevelLeft.Max() >= RetractionSlopeLevelRight.Max()
                ? RetractionSlopeLevelLeft.Max()
                : RetractionSlopeLevelRight.Max());
        }

        public int[] GetPRSpeed(List<RDCurve> rdcs)
        {
            var PR = new List<float> { };

            for (int index = 0; index < rdcs.Count - 30; index++)
            {
                var PR_value = Math.Abs(rdcs[index + 30].Trapez_level - rdcs[index].Trapez_level) / 30;
                PR.Add(PR_value);
            }

            return TableOfRetractionSlopeSpeed(PR.Max());
        }

        public int GetKRSpeedPass(List<RDCurve> rdcs)
        {
            var KR = new List<int> { };

            for (int index = 0; index < rdcs.Count; index++)
            {
                var R = (17860.0 / (Math.Abs(rdcs[index].Trapez_str) != 0 ? Math.Abs(rdcs[index].Trapez_str) : 0.0001));
                var CR_value = (int)(Math.Sqrt(13.0 * R * (0.7 + 0.0061 * Math.Abs(rdcs[index].Trapez_level))));
                KR.Add(CR_value);
            }
            return KR.Any() ? KR.Min() : -1;
        }
        public int GetKRSpeedFreig(List<RDCurve> rdcs)
        {
            var KR = new List<int> { };

            for (int index = 0; index < rdcs.Count; index++)
            {
                var CR_value = (int)(Math.Sqrt(13.0 * (17860.0 / (Math.Abs(rdcs[index].Trapez_str) != 0 ? Math.Abs(rdcs[index].Trapez_str) : 0.0001)) * (0.7 + 0.0061 * Math.Abs(rdcs[index].Trapez_level))));
                KR.Add(CR_value);
            }
            return KR.Any() ? KR.Min() : -1;
        }
    }
}
