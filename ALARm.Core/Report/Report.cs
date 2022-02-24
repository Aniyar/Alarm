
using MetroFramework.Controls;
using System;

namespace ALARm.Core.Report
{
    public abstract class Report : ReportTemplate
    {
        public long CurveId { get; set; }
        public string softVersion = "ALARm 1.0 (№436-р)";
        public double curveAngleByString(int a, int b, int c)
        {
            double y = -c;
            double xStart = b;
            double xFinish = a;
            double deltaX = Math.Abs(xStart - xFinish);
            return (Math.Atan(y / deltaX) * 180) / Math.PI; 
        }
        public int GetTransitionLength(int Km, int M, int Lvl_km, int lvl_m, NonstandardKm nonStandardkm)
        {
            return Km < Lvl_km
                ? nonStandardkm is null ? 1000 : nonStandardkm.Len - M + lvl_m
                : lvl_m - M;
        }
        public int RoundNumToFive(int num)
        {
            int rem = num % 5;
            return num - rem;
        }
        public double CurveAngle(float r, int length)
        {
            //return (Math.Atan(y / length) * 180) / Math.PI;
            var angle = Math.Asin(length / (2.0 * r)) * (180.0 / Math.PI) * 2.0;
            return angle;
        }

       

        
        public virtual void Process(long admUnitId, ReportTemplate template, long processId) { }
        public virtual void Process(long parentId, ReportTemplate template, ReportPeriod period, MetroProgressBar progressBar ) { }
        
    }
}
