using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using System.Xml.Linq;

namespace ALARm.Core.Report
{
    public class StraightProfile
    {
        public int CoordAbs { get; set; }
        
        public int X_ind { get; set; }
        public double OriginalYR { get; set; }
        public int Km { get {
                return CoordAbs / 1000;
            } }
        public int M {
            get {
                return CoordAbs % 1000;
            } }
        public double Profile { get; set; }
        public int Len { get; set; }
        public double Slope { get; set; }
        public string SlopeText { get {
                return double.IsNaN(Slope)? "-" : Slope.ToString("0.00").Replace(',', '.');
            } }
        public double SlopeDiff { get; set; }
        public string SlopeDiffText { get {
                return double.IsNaN(SlopeDiff) ? "-" : SlopeDiff.ToString("0.00").Replace(',', '.');
            } }
    }

    public class ProfileData
    {
        public ProfileData(List<int> x, List<double> y)
        {
            X = x.ToArray();
            Y = y.ToArray();
            StraighteningProfile();

            scheme_y = 415.7;
            scheme_len = 1100;
        }

        public ProfileData(List<int> x, List<double> y, List<double> deviations)
        {
            X = x.ToArray();
            Y = y.ToArray();
            D = deviations.ToArray();
            StraighteningProfile();

            scheme_y = 340.183;
            scheme_len = 900;
        }

        //начало - конец участка по выбору
        public int Start;
        public int Final;

        public ProfileData(List<int> x, List<double> y, List<double> deviations, List<double> RepersY_list, List<int> RepersX_list, List<double> OriginYR_list, List<int> Xind_list, float st, float fn)
        {
            Start = (int)st;
            Final = (int)fn;

            X = x.ToArray();
            Y = y.ToArray();

            D = deviations.ToArray();

            RepersY = RepersY_list.ToArray();
            RepersX = RepersX_list.ToArray();

            OriginYR = OriginYR_list.ToArray();
            Xind = Xind_list.ToArray();

            StraighteningProfile();

            scheme_y = 340.183;
            scheme_len = 900;
        }

        private int[] Xind;
        private int[] X;
        private double[] Y;
        private double[] RepersY;
        private int[] RepersX;
        private double[] OriginYR;
        private double[] D;
        
        double scheme_y = 6.7 / 2;
        private double scheme_len = 100;

        public List<StraightProfile> straightProfiles = new List<StraightProfile>();
        public List<StraightProfile> straightProfilesNew = new List<StraightProfile>();
        public List<StraightProfile> straightProfilesReper = new List<StraightProfile>();
        public List<StraightProfile> straightProfilesReperByOrig = new List<StraightProfile>();
        public List<StraightProfile> DicReper = new List<StraightProfile>();



        public void PicketInfo(XElement xElement)
        {
            var straightProfiles = straightProfilesReperByOrig;

            try
            {
                int start = X[0];
                int final = straightProfiles.Max(s => s.Km * 1000 + s.M)+1;
                int startP = (straightProfiles.Min(s => s.Km * 1000 + s.M) % 1000) / 100 + 1;
                int startK = straightProfiles.Min(s => s.Km);
                double lastValue = -1;
                double x_coef =  25 / 5000.0; //x - km

                while (startK * 1000 + (startP - 1) * 100 < final)
                {
                    if (X.Contains(startK * 1000 + (startP - 1) * 100))
                    {
                        xElement.Add(new XElement("pickets",
                            new XAttribute("x1", (startK * 1000 + (startP - 1) * 100 - start) * x_coef),
                            new XAttribute("x2", (startK * 1000 + (startP - 1) * 100 - start) * x_coef),
                            new XAttribute("y1", 3),
                            new XAttribute("y2", 4)));

                        //if (startK * 1000 + (startP - 1) * 100 - start > 15)
                        //{
                        //    xElement.Add(new XElement("pickets_value",
                        //        new XAttribute("x", -2.98),
                        //        new XAttribute("y", (startK * 1000 + (startP - 1) * 100 - start) * x_coef + 0.075),
                        //        new XAttribute("text", Y[X.ToList().IndexOf(startK * 1000 + (startP - 1) * 100)])));
                        //}
                        //else
                        //{
                        //    xElement.Add(new XElement("pickets_value",
                        //        new XAttribute("x", -2.98),
                        //        new XAttribute("y", 0.15),
                        //        new XAttribute("text", Y[X.ToList().IndexOf(startK * 1000 + (startP - 1) * 100)])));
                        //}

                        if (lastValue != -1)
                        {
                            if (lastValue > ORIGINminusLBOplusLRrrr[X.ToList().IndexOf(startK * 1000 + (startP - 1) * 100)])
                            {
                                xElement.Add(new XElement("pickets",
                                    new XAttribute("x1", (startK * 1000 + (startP - 2) * 100 - start) * x_coef),
                                    new XAttribute("x2", (startK * 1000 + (startP - 1) * 100 - start) * x_coef),
                                    new XAttribute("y1", 3),
                                    new XAttribute("y2", 4)));
                            }
                            else
                            {
                                xElement.Add(new XElement("pickets",
                                    new XAttribute("x1", (startK * 1000 + (startP - 1) * 100 - start) * x_coef),
                                    new XAttribute("x2", (startK * 1000 + (startP - 2) * 100 - start) * x_coef),
                                    new XAttribute("y1", 3),
                                    new XAttribute("y2", 4)));
                            }
                        }

                        lastValue = ORIGINminusLBOplusLRrrr[X.ToList().IndexOf(startK * 1000 + (startP - 1) * 100)];
                    }
                    else if (X.ToList().Where(x => x - startK * 1000 + (startP - 1) * 100 >= -10 && x - startK * 1000 + (startP - 1) * 100 <= 10).Any())
                    {
                        int x_temp = X.ToList().Where(x => x - startK * 1000 + (startP - 1) * 100 >= -10 && x - startK * 1000 + (startP - 1) * 100 <= 10).First();

                        xElement.Add(new XElement("pickets",
                            new XAttribute("x1", (startK * 1000 + (startP - 1) * 100 - start) * x_coef),
                            new XAttribute("x2", (startK * 1000 + (startP - 1) * 100 - start) * x_coef),
                            new XAttribute("y1", 3),
                            new XAttribute("y2", 4)));

                        //if (startK * 1000 + (startP - 1) * 100 - start > 15)
                        //{
                        //    xElement.Add(new XElement("pickets_value",
                        //        new XAttribute("x", -2.98),
                        //        new XAttribute("y", (startK * 1000 + (startP - 1) * 100 - start) * x_coef + 0.075),
                        //        new XAttribute("text", Y[X.ToList().IndexOf(x_temp)])));
                        //}
                        //else
                        //{
                        //    xElement.Add(new XElement("pickets_value",
                        //        new XAttribute("x", -2.98),
                        //        new XAttribute("y", 0.15),
                        //        new XAttribute("text", Y[X.ToList().IndexOf(x_temp)])));
                        //}

                        if (lastValue != -1)
                        {
                            if (lastValue > ORIGINminusLBOplusLRrrr[X.ToList().IndexOf(x_temp)])
                            {
                                xElement.Add(new XElement("pickets",
                                    new XAttribute("x1", (startK * 1000 + (startP - 2) * 100 - start) * x_coef),
                                    new XAttribute("x2", (startK * 1000 + (startP - 1) * 100 - start) * x_coef),
                                    new XAttribute("y1", 3),
                                    new XAttribute("y2", 4)));
                            }
                            else
                            {
                                xElement.Add(new XElement("pickets",
                                    new XAttribute("x1", (startK * 1000 + (startP - 1) * 100 - start) * x_coef),
                                    new XAttribute("x2", (startK * 1000 + (startP - 2) * 100 - start) * x_coef),
                                    new XAttribute("y1", 3),
                                    new XAttribute("y2", 4)));
                            }
                        }

                        lastValue = ORIGINminusLBOplusLRrrr[X.ToList().IndexOf(x_temp)];
                    }
                    else
                    {
                        int x1 = X.ToList().Where(x => x - startK * 1000 + (startP - 1) * 100 >= 0).First();
                        int x2;
                        var x2List = X.ToList().Where(x => startK * 1000 + (startP - 1) * 100 - x >= 0);

                        if (x2List!=null && x2List.Count() > 0){
                            x2 = x2List.Last();
                        }
                        else {
                            x2 = x1;
                            
                        }
                        
                        double y1 = ORIGINminusLBOplusLRrrr[X.ToList().IndexOf(x1)];
                        double y2 = ORIGINminusLBOplusLRrrr[X.ToList().IndexOf(x2)];
                        double div = (x2 - x1) + y1;
                        double y;
                        if (div > 0)
                            y = (y2 - y1) * (startK * 1000 + (startP - 1) * 100 - x1) / div;
                        else
                            y = (y2 - y1) * (startK * 1000 + (startP - 1) * 100 - x1) / 1;

                        xElement.Add(new XElement("pickets",
                            new XAttribute("x1", (startK * 1000 + (startP - 1) * 100 - start) * x_coef),
                            new XAttribute("x2", (startK * 1000 + (startP - 1) * 100 - start) * x_coef),
                            new XAttribute("y1", 3),
                            new XAttribute("y2", 4)));

                        //if (startK * 1000 + (startP - 1) * 100 - start > 15)
                        //{
                        //    xElement.Add(new XElement("pickets_value",
                        //        new XAttribute("x", -2.98),
                        //        new XAttribute("y", (startK * 1000 + (startP - 1) * 100 - start) * x_coef + 0.075),
                        //        new XAttribute("text", y)));
                        //}
                        //else
                        //{
                        //    xElement.Add(new XElement("pickets_value",
                        //        new XAttribute("x", -2.98),
                        //        new XAttribute("y", 0.15),
                        //        new XAttribute("text", y)));
                        //}

                        if (lastValue != -1)
                        {
                            if (lastValue > y)
                            {
                                xElement.Add(new XElement("pickets",
                                    new XAttribute("x1", (startK * 1000 + (startP - 2) * 100 - start) * x_coef),
                                    new XAttribute("x2", (startK * 1000 + (startP - 1) * 100 - start) * x_coef),
                                    new XAttribute("y1", 3),
                                    new XAttribute("y2", 4)));
                            }
                            else
                            {
                                xElement.Add(new XElement("pickets",
                                    new XAttribute("x1", (startK * 1000 + (startP - 1) * 100 - start) * x_coef),
                                    new XAttribute("x2", (startK * 1000 + (startP - 2) * 100 - start) * x_coef),
                                    new XAttribute("y1", 3),
                                    new XAttribute("y2", 4)));
                            }
                        }

                        lastValue = y;
                    }

                    startP++;
                    if (startP > 10)
                    {
                        startK++;
                        startP -= 10;
                    }
                }

                xElement.Add(new XElement("pickets",
                    new XAttribute("x1", (startK * 1000 + (startP - 1) * 100 - start) * x_coef),
                    new XAttribute("x2", (startK * 1000 + (startP - 1) * 100 - start) * x_coef),
                    new XAttribute("y1", 3),
                    new XAttribute("y2", 4)));
                //xElement.Add(new XElement("pickets_value",
                //    new XAttribute("x", -2.98),
                //    new XAttribute("y", 24.95),
                //    new XAttribute("text", Y[X.ToList().IndexOf(final)])));
                if (lastValue > ORIGINminusLBOplusLRrrr[X.ToList().IndexOf(final)])
                {
                    xElement.Add(new XElement("pickets",
                        new XAttribute("x1", (startK * 1000 + (startP - 2) * 100 - start) * x_coef),
                        new XAttribute("x2", (startK * 1000 + (startP - 1) * 100 - start) * x_coef),
                        new XAttribute("y1", 3),
                        new XAttribute("y2", 4)));
                }
                else
                {
                    xElement.Add(new XElement("pickets",
                        new XAttribute("x1", (startK * 1000 + (startP - 1) * 100 - start) * x_coef),
                        new XAttribute("x2", (startK * 1000 + (startP - 2) * 100 - start) * x_coef),
                        new XAttribute("y1", 3),
                        new XAttribute("y2", 4)));
                }
            }
            catch (Exception e) {
                
                Console.Error.WriteLine("Tura tohtau kerek goy ===> "+  e.StackTrace);
                return;
                
            }
            
        }

        public void StraightInfo(XElement xElement)
        {
            int start = X[0];
            double x_coef = 25.0 / 5000; //x - km

            var straightProfiles = straightProfilesReper;

            for (int i = 0; i < straightProfiles.Count - 1; i++)
            {
                xElement.Add(new XElement("straights",
                    new XAttribute("x1", (straightProfiles[i].CoordAbs - start) * x_coef),
                    new XAttribute("x2", (straightProfiles[i].CoordAbs - start) * x_coef),
                    new XAttribute("y1", 1),
                    new XAttribute("y2", 2)));

                if (straightProfiles[i].Profile > straightProfiles[i + 1].Profile)
                {
                    xElement.Add(new XElement("straights",
                        new XAttribute("x1", (straightProfiles[i].CoordAbs - start) * x_coef),
                        new XAttribute("x2", (straightProfiles[i + 1].CoordAbs - start) * x_coef),
                        new XAttribute("y1", 1),
                        new XAttribute("y2", 2)));
                    xElement.Add(new XElement("straights_value",
                        new XAttribute("x", (straightProfiles[i + 1].CoordAbs - start) * x_coef - straightProfiles[i].Slope.ToString("0.##").Length * 0.11),
                        new XAttribute("y", 1.2),
                        new XAttribute("rotate", 0),
                        new XAttribute("text", straightProfiles[i].Slope.ToString("0.##").Replace(',', '.'))));
                    xElement.Add(new XElement("straights_value",
                        new XAttribute("x", (straightProfiles[i].CoordAbs - start) * x_coef + 0.01),
                        new XAttribute("y", 1.95),
                        new XAttribute("rotate", 0),
                        new XAttribute("text", straightProfiles[i].Len)));
                }
                else
                {
                    xElement.Add(new XElement("straights",
                        new XAttribute("x2", (straightProfiles[i].CoordAbs - start) * x_coef),
                        new XAttribute("x1", (straightProfiles[i + 1].CoordAbs - start) * x_coef),
                        new XAttribute("y1", 1),
                        new XAttribute("y2", 2)));
                    xElement.Add(new XElement("straights_value",
                        new XAttribute("x", (straightProfiles[i].CoordAbs - start) * x_coef + 0.01),
                        new XAttribute("y", 1.2),
                        new XAttribute("rotate", 0),
                        new XAttribute("text", straightProfiles[i].Slope.ToString("0.##").Replace(',', '.'))));
                    xElement.Add(new XElement("straights_value",
                        new XAttribute("x", (straightProfiles[i + 1].CoordAbs - start) * x_coef - straightProfiles[i].Len.ToString().Length * 0.11),
                        new XAttribute("y", 1.95),
                        new XAttribute("rotate", 0),
                        new XAttribute("text", straightProfiles[i].Len)));
                }

                //Метки числа
                if (straightProfiles[i].CoordAbs - start < 15)
                {
                    xElement.Add(new XElement("straights_value",
                        new XAttribute("x", -0.95),
                        new XAttribute("y", 0.15),
                        new XAttribute("rotate", 270),
                        new XAttribute("text", straightProfiles[i].Profile)));
                }
                else
                {
                    xElement.Add(new XElement("straights_value",
                        new XAttribute("x", -0.95),
                        new XAttribute("y", (straightProfiles[i].CoordAbs - start) * x_coef + 0.075),
                        new XAttribute("rotate", 270),
                        new XAttribute("text", straightProfiles[i].Profile)));
                }
            }

            if (straightProfiles.Last().CoordAbs - start > 4950)
            {
                xElement.Add(new XElement("straights",
                    new XAttribute("x1", (straightProfiles.Last().CoordAbs - start) * x_coef),
                    new XAttribute("x2", (straightProfiles.Last().CoordAbs - start) * x_coef),
                    new XAttribute("y1", 1),
                    new XAttribute("y2", 2)));
                xElement.Add(new XElement("straights_value",
                    new XAttribute("x", -0.95),
                    new XAttribute("y", 24.95),
                    new XAttribute("rotate", 270),
                    new XAttribute("text", straightProfiles.Last().Profile)));
            }
            else
            {
                xElement.Add(new XElement("straights",
                    new XAttribute("x1", (straightProfiles.Last().CoordAbs - start) * x_coef),
                    new XAttribute("x2", (straightProfiles.Last().CoordAbs - start) * x_coef),
                    new XAttribute("y1", 1),
                    new XAttribute("y2", 2)));
                xElement.Add(new XElement("straights_value",
                    new XAttribute("x", -0.95),
                    new XAttribute("y", (straightProfiles.Last().CoordAbs - start) * x_coef + 0.075),
                    new XAttribute("rotate", 270),
                    new XAttribute("text", straightProfiles.Last().Profile)));
            }
        }

        public List<double> originY = new List<double>();
        public List<double> LinearY = new List<double>();
        public List<double> newOriginY = new List<double>();


        public List<String> GetLinearGraph(){
            List<string> dozenLinears = new List<string>();
            List<int> origX = X.ToList();
            int dozenCount = origX.Count % 5000 == 0 ? origX.Count / 5000 : origX.Count / 5000 + 1;
            //en birinshi nukte 
            int x0 = X[0];
            //944 pixel uzindihti jerge 5000 nukte sydiru kerek 
            //25 cm = 944pixel
            double x_coef = 944.9 / 5000.0;
            //grapic sizatin byiktikimiz px
            double grapH = 340.0;
            //grapicti ortasinnan bastap sizu kerek
            double middleH = grapH / 2.0;
            // ortasina barligin sydiru kerek masshtav
            // hagazga baylanisti san ==> 670
            double y_coef = grapH / 670;

            double y0 = Y[0];
            Console.Out.WriteLine("y0==> " + y0);
            for (int di = 0; di < dozenCount; di++) {
                
                string linearGraph = "";
                int xI = 0;
                for (int i = di*5000; i < (di+1)*5000; i++){
                    if (i < Math.Min(linearPointY.Count, Y.Length)) {
                        double calcX = (X[xI] - x0) * x_coef;
                        linearGraph += (calcX).ToString("0.####").Replace(',', '.') + ",";
                        // db dan meter keledi sol metrdi cm ge convert jasau kerek
                        double calcY = (middleH - y_coef * (linearPointY[i] - y0) * 100.0);
                        //eger y mani sizuga tysti byiktikten uken bolsa sydiru kerek
                        //eger 0 bolsa tovesinnen bastau kerek
                        if (calcY > 340){
                            calcY -= 340 * Math.Round(calcY / 340);
                        }
                        if (calcY <= 0){
                            calcY = calcY + 340 + 340 * Math.Round(calcY / 340);
                        }


                        //for deviation
                        //Dictionary<string, string> dPoint = new Dictionary<string, string>();
                        //double devX = calcX;
                        //double devY = calcY - (middleH - y_coef * (Y[i] - y0) * 100.0);

                        //dPoint.Add("x", devX.ToString("0.####").Replace(',', '.'));
                        //dPoint.Add("y", devY.ToString("0.####").Replace(',', '.'));
                        //deviationPoints.Add(dPoint);

                        //end deviation
                        xI++;
                        linearGraph += calcY.ToString("0.####").Replace(',', '.') + " ";

                        LinearY.Add(linearPointY[i]);
                    }
                }
                dozenLinears.Add(linearGraph);
            }
            
            //Console.Out.WriteLine("songi sizilgan nukteler "+linearGraph);
            return dozenLinears;

        }
        public List<String> getLinearGraphReper()
        {
            List<string> dozenLinears = new List<string>();
            List<int> origX = X.ToList();
            int dozenCount = origX.Count % 5000 == 0 ? origX.Count / 5000 : origX.Count / 5000 + 1;
            //en birinshi nukte 
            int x0 = X[0];
            //944 pixel uzindihti jerge 5000 nukte sydiru kerek 
            //25 cm = 944pixel
            double x_coef = 944.9 / 5000.0;
            //grapic sizatin byiktikimiz px
            double grapH = 340.0;
            //grapicti ortasinnan bastap sizu kerek
            double middleH = grapH / 2.0;
            // ortasina barligin sydiru kerek masshtav
            // hagazga baylanisti san ==> 670
            double y_coef = grapH / 670;

            double y0 = Y[0];
            Console.Out.WriteLine("y0==> " + y0);
            for (int di = 0; di < dozenCount; di++)
            {
                string linearGraph = "";
                int xI = 0;
                for (int i = di * 5000; i < (di + 1) * 5000; i++)
                {
                    if (i < linearPointYReper.Count)
                    {
                        double calcX = (X[xI] - x0) * x_coef;
                        linearGraph += (calcX).ToString("0.####").Replace(',', '.') + ",";
                        // db dan meter keledi sol metrdi cm ge convert jasau kerek
                        double calcY = (middleH - y_coef * (linearPointYReper[i] - linearPointYReper.First()) * 100.0);
                        //eger y mani sizuga tysti byiktikten uken bolsa sydiru kerek
                        //eger 0 bolsa tovesinnen bastau kerek
                        if (calcY > 340)
                        {
                            calcY -= 340 * Math.Round(calcY / 340);
                        }
                        if (calcY <= 0)
                        {
                            calcY = calcY + 340 + (340 * Math.Round(calcY / 340));
                        }
                        xI++;
                        linearGraph += calcY.ToString("0.####").Replace(',', '.') + " ";
                    }
                }
                dozenLinears.Add(linearGraph);
            }

            //Console.Out.WriteLine("songi sizilgan nukteler "+linearGraph);
            return dozenLinears;

        }
        public List<String> getLinearGraphReperByOrig()
        {
            List<string> dozenLinears = new List<string>();
            List<int> origX = X.ToList();
            int dozenCount = origX.Count % 5000 == 0 ? origX.Count / 5000 : origX.Count / 5000 + 1;
            //en birinshi nukte 
            int x0 = X[0];
            //944 pixel uzindihti jerge 5000 nukte sydiru kerek 
            //25 cm = 944pixel
            double x_coef = 944.9 / 5000.0;
            //grapic sizatin byiktikimiz px
            double grapH = 340.0;
            //grapicti ortasinnan bastap sizu kerek
            double middleH = grapH / 2.0;
            // ortasina barligin sydiru kerek masshtav
            // hagazga baylanisti san ==> 670
            double y_coef = grapH / 670;

            double y0 = Y[0];
            Console.Out.WriteLine("y0==> " + y0);
            for (int di = 0; di < dozenCount; di++)
            {

                string linearGraph = "";
                int xI = 0;
                for (int i = di * 5000; i < (di + 1) * 5000; i++)
                {
                    if (i < linearPointYReperByOrig.Count)
                    {
                        double calcX = (X[xI] - x0) * x_coef;
                        linearGraph += (calcX).ToString("0.####").Replace(',', '.') + ",";
                        // db dan meter keledi sol metrdi cm ge convert jasau kerek
                        double calcY = (middleH - y_coef * (linearPointYReperByOrig[i] - linearPointYReperByOrig.First()) * 100.0);
                        //eger y mani sizuga tysti byiktikten uken bolsa sydiru kerek
                        //eger 0 bolsa tovesinnen bastau kerek
                        if (calcY > 340)
                        {
                            calcY -= 340 * Math.Round(calcY / 340);
                        }
                        if (calcY <= 0)
                        {
                            calcY = calcY + 340 + 340 * Math.Round(calcY / 340);
                        }
                        xI++;
                        linearGraph += calcY.ToString("0.####").Replace(',', '.') + " ";
                    }
                }
                dozenLinears.Add(linearGraph);
            }

            //Console.Out.WriteLine("songi sizilgan nukteler "+linearGraph);
            return dozenLinears;
        }

        
        public List<double> ORIGINminusLBOplusLRrrr = new List<double>();
        public List<double> XX = new List<double>();

        public List<String> ORIGINminusLBOplusLR()
        {
                 

            for(int n = 0; n < Math.Min(Math.Min(newOriginY.Count, linearPointYReperByOrig.Count), linearPointYReper.Count); n++)
            {
                var ob = (newOriginY[n]) - linearPointYReperByOrig[n] + linearPointYReper[n];
                ORIGINminusLBOplusLRrrr.Add(ob);
            }

            List<string> dozenLinears = new List<string>();
            List<int> origX = X.ToList();
            int dozenCount = origX.Count % 5000 == 0 ? origX.Count / 5000 : origX.Count / 5000 + 1;
            //en birinshi nukte 
            int x0 = X[0];
            //944 pixel uzindihti jerge 5000 nukte sydiru kerek 
            //25 cm = 944pixel
            double x_coef = 944.9 / 5000.0;
            //grapic sizatin byiktikimiz px
            double grapH = 340.0;
            //grapicti ortasinnan bastap sizu kerek
            double middleH = grapH / 2.0;
            // ortasina barligin sydiru kerek masshtav
            // hagazga baylanisti san ==> 670
            double y_coef = (grapH / 670) ; //масштаб 1:100см

            double y0 = ORIGINminusLBOplusLRrrr[0];
            Console.Out.WriteLine("y0==> " + y0);
            for (int di = 0; di < dozenCount; di++)
            {

                string linearGraph = "";
                int xI = 0;
                for (int i = di * 5000; i < (di + 1) * 5000; i++)
                {
                    if (i < ORIGINminusLBOplusLRrrr.Count)
                    {
                        double calcX = (X[xI] - x0) * x_coef;
                        XX.Add(calcX);

                        linearGraph += (calcX).ToString("0.####").Replace(',', '.') + ",";
                        // db dan meter keledi sol metrdi cm ge convert jasau kerek
                        double calcY = (middleH - (25.0 / 57.0) * 1.7 * y_coef * (ORIGINminusLBOplusLRrrr[i] - y0) * 100.0);
                        //eger y mani sizuga tysti byiktikten uken bolsa sydiru kerek
                        //eger 0 bolsa tovesinnen bastau kerek
                        //if (calcY > 340)
                        //{
                        //    calcY = calcY - 340 * Math.Round(calcY / 340);
                        //}
                        //if (calcY < 0)
                        //{
                        //    calcY = calcY + 340 + 340 * Math.Round(calcY / 340);
                        //}


                        int prom2 = 0;
                        prom2 = (int)calcY / 340;

                        if (calcY > 340)
                        {
                            calcY -= 340.0 * prom2;
                            
                        }
                        else if (calcY < 0)
                        {
                            calcY = calcY + 340.0 - 340.0 * prom2;
                        }

                        xI++;
                        linearGraph += calcY.ToString("0.####").Replace(',', '.') + " ";


                        
                    }
                }
                dozenLinears.Add(linearGraph);
            }

            //Console.Out.WriteLine("songi sizilgan nukteler "+linearGraph);
            return dozenLinears;
        }

        public List<String> getDev()
        {
            List<string> dozenLinears = new List<string>();
            List<int> origX = X.ToList();
            int dozenCount = origX.Count % 5000 == 0 ? origX.Count / 5000 : origX.Count / 5000 + 1;
            //en birinshi nukte 
            int x0 = X[0];
            //944 pixel uzindihti jerge 5000 nukte sydiru kerek 
            //25 cm = 944pixel
            double x_coef = 944.9 / 5000.0;
            //grapic sizatin byiktikimiz px
            double grapH = 340.0;
            //grapicti ortasinnan bastap sizu kerek
            double middleH = grapH / 2.0;
            // ortasina barligin sydiru kerek masshtav
            // hagazga baylanisti san ==> 670
            double y_coef = (grapH / 670); //масштаб 1:100см

            double y0 = ORIGINminusLBOplusLRrrr[0];
            Console.Out.WriteLine("y0==> " + y0);
            for (int di = 0; di < dozenCount; di++)
            {
                string linearGraph = "";
                int xI = 0;
                for (int i = di * 5000; i < (di + 1) * 5000; i++)
                {
                    if (i < linearPointYNew.Count)
                    {
                        double calcX = (X[xI] - x0) * x_coef;

                        linearGraph += (calcX).ToString("0.####").Replace(',', '.') + ",";

                        double calcY = (396.5 - y_coef * (linearPointYNew[i] - ORIGINminusLBOplusLRrrr[i]) * 250.0);
                        xI++;
                        linearGraph += calcY.ToString("0.####").Replace(',', '.') + " ";
                    }
                }
                dozenLinears.Add(linearGraph);
            }
            return dozenLinears;
        }

        public List<String> getLinearGraphNew()
        {
            StraighteningProfileNew();

            DEV_GRAPH = "";

            List<string> dozenLinears = new List<string>();
            List<int> origX = X.ToList();
            int dozenCount = linearPointYNew.Count % 5000 == 0 ? linearPointYNew.Count / 5000 : linearPointYNew.Count / 5000 + 1;
            //en birinshi nukte 
            int x0 = X[0];
            //944 pixel uzindihti jerge 5000 nukte sydiru kerek 
            //25 cm = 944pixel
            double x_coef = 944.9 / 5000.0;
            //grapic sizatin byiktikimiz px
            double grapH = 340.0;
            //grapicti ortasinnan bastap sizu kerek
            double middleH = grapH / 2.0;
            // ortasina barligin sydiru kerek masshtav
            // hagazga baylanisti san ==> 670
            double y_coef = (grapH / 670); //масштаб 1:100см

            var Y = newOriginY;

            double y0 = linearPointYNew[0];
            Console.Out.WriteLine("y0==> " + y0);
            for (int di = 0; di < dozenCount; di++)
            {

                string linearGraph = "";
                int xI = 0;
                for (int i = di * 5000; i < (di + 1) * 5000; i++)
                {
                    if (i < Math.Min(linearPointYNew.Count, Y.Count))
                    {
                        double calcX = (X[xI] - x0) * x_coef;
                        linearGraph += (calcX).ToString("0.####").Replace(',', '.') + ",";
                        // db dan meter keledi sol metrdi cm ge convert jasau kerek
                        double calcY = (middleH - (25.0 / 57.0) * 1.7 * y_coef * (linearPointYNew[i] - y0) * 100.0);
                        //eger y mani sizuga tysti byiktikten uken bolsa sydiru kerek
                        //eger 0 bolsa tovesinnen bastau kerek
                        int prom2 = (int)calcY / 340;

                        if (calcY > 340)
                        {
                            calcY -= 340.0 * prom2;
                        }
                        else if (calcY < 0)
                        {
                            calcY = calcY + 340.0 - 340.0 * prom2;
                        }
                        ReperY.Add(calcY);
                        ReperX.Add(calcX);

                        xI++;
                        linearGraph += calcY.ToString("0.####").Replace(',', '.') + " ";
                    }
                }
                dozenLinears.Add(linearGraph);
            }
            return dozenLinears;
        }

        public List<String> getOrigGraph()
        {
            var ExponentCoef = -5;

            List<double> newOriginY1 = new List<double>();

            for (int i = 0; i < Math.Min(Y.Count(), linearPointY.Count()); i++)
            {
                var e = Math.Exp(ExponentCoef * Math.Abs(Y[i] - linearPointY[i]));

                var k = linearPointY[i] + (Y[i] - linearPointY[i]) * e;

                newOriginY1.Add(k);
            }

            var width = 10;
            List<double> RollAver = new List<double>();
            for (int i = 0; i < newOriginY1.Count(); i++)
            {
                if (newOriginY1.Count() >= width)
                {
                    RollAver.Add(newOriginY1[i]);

                    var ra = RollAver.Skip(RollAver.Count() - width).Take(width).Average();

                    newOriginY.Add(ra);
                }
                else
                {
                    RollAver.Add(newOriginY1[i]);
                    newOriginY.Add(newOriginY1[i]);
                }
            }


            List<string> dozenLinears = new List<string>();
            List<int> origX = X.ToList();
            int dozenCount = origX.Count % 5000 == 0 ? origX.Count / 5000 : origX.Count / 5000 + 1;
            //en birinshi nukte 
            int x0 = X[0];
            //944 pixel uzindihti jerge 5000 nukte sydiru kerek 
            //25 cm = 944pixel
            double x_coef = 944.9 / 5000.0;
            //grapic sizatin byiktikimiz px
            double grapH = 340.0;
            //grapicti ortasinnan bastap sizu kerek
            double middleH = grapH / 2.0;
            // ortasina barligin sydiru kerek masshtav
            // hagazga baylanisti san ==> 670
            double y_coef = (grapH / 670); //масштаб 1:100см

            double y0 = newOriginY[0];
            
            Console.Out.WriteLine("y0==> " + y0);


            double calcX = 0.0;
            double calcY = 0.0;

            for (int di = 0; di < dozenCount; di++)
            {
                string linearGraph = "";
                int xI = 0;
                for (int i = di * 5000; i < (di + 1) * 5000; i++)
                {
                    if (i < newOriginY.Count)
                    {
                        calcX = (X[xI] - x0) * x_coef;
                        linearGraph += (calcX).ToString("0.####").Replace(',', '.') + ",";

                        // db dan meter keledi sol metrdi cm ge convert jasau kerek
                        calcY = middleH - (25.0 / 57.0) * 1.7 * y_coef * (newOriginY[i] - y0) * 100.0;

                        ////eger y mani sizuga tysti byiktikten uken bolsa sydiru kerek
                        ////eger 0 bolsa tovesinnen bastau kerek
                        if (calcY > 340)
                        {
                            calcY -= 340 * Math.Round(calcY / 340);
                        }
                        if (calcY <= 0)
                        {
                            calcY = calcY + 340 + 340 * Math.Round(calcY / 340);
                        }
                        //Console.Out.WriteLine("calcY "+calcY);
                        xI++;
                        linearGraph += calcY.ToString("0.####").Replace(',', '.') + " ";
                    }
                }
                dozenLinears.Add(linearGraph);
            }
            return dozenLinears;
        }

        /// <summary>
        /// ДФ - 4,1
        /// </summary>
        /// <returns></returns>
        public Dictionary<String, List<string>> GraphProfile2()
        {

            Console.Out.WriteLine("iishee shaagaad orood irlee de gj");
            Dictionary<string, List<string>> r = new Dictionary<string, List<string>>
            {
                //r.Add("original",dozenProfiles);
                //r.Add("linear", getLinearGraph());
                { "new_original", getOrigGraph() },

                //r.Add("linearReper", getLinearGraphReper());
                //r.Add("linearReperByOrig", getLinearGraphReperByOrig());
                { "ORIGINminusLBOplusLR", ORIGINminusLBOplusLR() },
                //r.Add("ReperLOlinear", getLinearGraphNewReper());
                //r.Add("Dev", getDev());

                { "new_linear", getLinearGraphNew() },

                { "Dev", getDev() }
            };

            return r;
        }
        /// <summary>
        /// ДФ - 4,5
        /// </summary>
        /// <returns></returns>
        public Dictionary<String, List<string>> GraphProfile()
        {

            Console.Out.WriteLine("iishee shaagaad orood irlee de gj");
            Dictionary<string, List<string>> r = new Dictionary<string, List<string>>
            {
                { "new_original", getOrigGraph() },
                { "ORIGINminusLBOplusLR", ORIGINminusLBOplusLR() },
                { "ReperLOlinear", getLinearGraphNewReper() }, //убрать

                { "new_linear", getLinearGraphNew() },

                { "Dev", getDev() }
            };

            return r;
        }

        public string GraphStraightProfile()
        {
            //int start = straightProfiles.Min(s => s.Km) * 1000;
            int start = 0;

            //25 cm = 944pixel
            double x_coef = 944.9 / 5000.0;

            //grapic sizatin byiktikimiz px
            double grapH = 340.0;

            //grapicti ortasinnan bastap sizu kerek
            double middleH = grapH / 2.0;

            // ortasina barligin sydiru kerek masshtav
            // hagazga baylanisti san ==> 670
            double y_coef = grapH / 670;

            string graph = "";
            double y0 = Y[0];
            Console.Out.WriteLine("y0==> " + y0);
            double calcY = 0.0;

            //double x_coef = 944.9 / 5000, y_coef = scheme_y / scheme_len; //x - km, y - sm
            //string graph = "";

            for (int i = 0; i < straightProfiles.Count; i++)
            {
                if (i != 0)
                {
                    if (straightProfiles[i].Profile / scheme_len != straightProfiles[i - 1].Profile / scheme_len)
                    {
                        if (straightProfiles[i].Profile / scheme_len > straightProfiles[i - 1].Profile / scheme_len)
                        {

                            double x_temp = (straightProfiles[i].CoordAbs - straightProfiles[i - 1].CoordAbs) / (straightProfiles[i].Profile % scheme_len + scheme_len - straightProfiles[i - 1].Profile % scheme_len) * (scheme_len - straightProfiles[i - 1].Profile % scheme_len) + straightProfiles[i - 1].CoordAbs - start;
                            //double y_str = straightProfiles[i]-
                            graph += (x_temp * x_coef).ToString("0.####").Replace(',', '.') + ",";
                            graph += (scheme_len * y_coef).ToString("0.####").Replace(',', '.') + " ";
                            graph += (x_temp * x_coef).ToString("0.####").Replace(',', '.') + ",";
                            graph += (0 * y_coef).ToString("0.####").Replace(',', '.') + " ";
                        }
                        else
                        {
                            double x_temp = (straightProfiles[i].CoordAbs - straightProfiles[i - 1].CoordAbs) / (straightProfiles[i].Profile % scheme_len - straightProfiles[i - 1].Profile % scheme_len - scheme_len) * ((-1.0) * straightProfiles[i - 1].Profile % scheme_len) + straightProfiles[i - 1].CoordAbs - start;
                            graph += (x_temp * x_coef).ToString("0.####").Replace(',', '.') + ",";
                            graph += (0 * y_coef).ToString("0.####").Replace(',', '.') + " ";
                            graph += (x_temp * x_coef).ToString("0.####").Replace(',', '.') + ",";
                            graph += (scheme_len * y_coef).ToString("0.####").Replace(',', '.') + " ";
                        }
                    }
                }

                graph += ((straightProfiles[i].CoordAbs - start) * x_coef).ToString("0.####").Replace(',', '.') + ",";
                graph += (scheme_y - (straightProfiles[i].Profile % scheme_len) * y_coef).ToString("0.####").Replace(',', '.') + " ";
            }

            return graph;
        }

        public string GraphDeviation()
        {

            Console.Out.WriteLine("linear y size " + linearPointY.Count());
            //en birinshi nukte 
            int x0 = X[0];
            //944 pixel uzindihti jerge 5000 nukte sydiru kerek 
            //25 cm = 944pixel
            double x_coef = 944.9 / 5000.0;

            //grapic sizatin byiktikimiz px
            double grapH = 340.0;

            //grapicti ortasinnan bastap sizu kerek
            double middleH = grapH / 2.0;

            // ortasina barligin sydiru kerek masshtav
            // hagazga baylanisti san ==> 670
            double y_coef = grapH / 670;

            
            double y0 = Y[0];
          

            //int start = (X.Min() / 1000) * 1000 ;
            //double x_coef = 944.9 / 5000;
            //double y_coef = 37.8167 / 10; //x - km, y - sm
            string graph = "";

            for (int i = 0; i < X.Length; i++)
            {
                //D[i] =D[i] * 500;

                //graph += ((X[i] - x0) * x_coef).ToString("0.####").Replace(',', '.') + ",";
                //graph += ((scheme_y + 113.417 / 2) - D[i] * y_coef).ToString("0.####").Replace(',', '.') + " ";


                graph += ((X[i] - x0) * x_coef).ToString("0.####").Replace(',', '.') + ",";
                // ydin manin tabu

                // db dan meter keledi sol metrdi cm ge convert jasau kerek
                graph = (middleH - y_coef * (D[i] - y0) * 100.0).ToString("0.####").Replace(',', '.') + " "; ;
            }

            return graph;
        }


        List<double> linearPointY = new List<double>();
        public List<double> linearPointYNew = new List<double>();
        public List<double> linearPointYReper = new List<double>();
        public List<double> linearPointYReperByOrig = new List<double>();

        public List<double> linearPointYReperByOrigLinear = new List<double>();

        private void StraighteningProfile()
        {
            straightProfiles.Add(new StraightProfile
            {
                CoordAbs = X[0],
                Profile = Y[0]
            });

            int i = 0;

            for (int j = i + 1; j < X.Length; j++)
            {
                double
                    Bx = X[j] - X[i],
                    By = Y[j] - Y[i];

                double otnosh = By / Bx;


                for (int k = i; k < j + 1; k++)
                {
                    double maxDistance = Math.Abs((Y[k] - Y[i]) - otnosh/5.0 * (X[k] - X[i]));

                    if (maxDistance > 0.15 && Math.Abs(straightProfiles.Last().CoordAbs - X[j - 1]) > 45)
                    {
                        straightProfiles.Add(new StraightProfile
                        {
                            CoordAbs = X[j - 1],
                            Profile = Y[j - 1]
                        });

                        i = j - 1;
                        break;
                    }
                }
            }



            if (!straightProfiles.Where(s => s.CoordAbs == X.Last() && s.Profile == Y.Last()).Any())
            {
                straightProfiles.Add(new StraightProfile
                {
                    CoordAbs = X.Last(),
                    Profile = Y.Last(),
                });
            }

            //Участок по выбору
            
            for (int t = 0; t < straightProfiles.Count() - 1; t++)
            {
                for (int c = 0; c < straightProfiles[t + 1].CoordAbs - straightProfiles[t].CoordAbs; c++)
                {
                    double bottom_dx1 = straightProfiles[t + 1].CoordAbs - straightProfiles[t].CoordAbs;

                    double y2 = straightProfiles[t + 1].Profile;

                    double y1 = straightProfiles[t].Profile;
                    var calc = (y2 - y1) / bottom_dx1 * c + y1;
                    linearPointY.Add(calc);
                  
                }
 
            }
            if (linearPointY.Any()) 
            { linearPointY.Add(linearPointY.Last()); }
           
            

            //Линеар для ориг профиля по реперным точкам
            for (i = 0; i < RepersY.Length; i++)

            {
                
                
                
                  straightProfilesReperByOrig.Add(new StraightProfile
                {
                    CoordAbs = RepersX[i],
                    Profile = OriginYR[i]
                });
            }

            switch (RepersY.Length)
            {
                case int Cn when Cn > 1:

                    for (int t = 0; t < straightProfilesReperByOrig.Count() - 1; t++)
                    {
                        for (int c = 0; c < straightProfilesReperByOrig[t + 1].CoordAbs - straightProfilesReperByOrig[t].CoordAbs; c++)
                        {
                            double bottom_dx1 = straightProfilesReperByOrig[t + 1].CoordAbs - straightProfilesReperByOrig[t].CoordAbs;

                            double y2 = straightProfilesReperByOrig[t + 1].Profile;
                            double y1 = straightProfilesReperByOrig[t].Profile;
                            var calc = (y2 - y1) / bottom_dx1 * c + y1;
                            linearPointYReperByOrig.Add(calc);
                        }
                    }
                    linearPointYReperByOrig.Add(Y.Last());
                    break;
                case int Cn when Cn == 0:
                    for (int t = 0; t < X.Count() - 1; t++)
                    {
                        linearPointYReperByOrig.Add(0.0);
                    }
                    break;
                case int Cn when Cn == 1:
                    for (int t = 0; t < X.Count() - 1; t++)
                    {
                        linearPointYReperByOrig.Add(straightProfilesReperByOrig[0].Profile);
                    }
                    break;
                default:
                    Console.WriteLine("Default case");
                    break;
            }



            //Reper linear
            for (i = 0; i < RepersY.Length; i++)
            {
                straightProfilesReper.Add(new StraightProfile
                {
                    CoordAbs = RepersX[i],
                    Profile = RepersY[i],
                    OriginalYR = OriginYR[i],
                    X_ind = Xind[i]
                    //Len = 0,
                    //Slope = double.NaN,
                    //SlopeDiff = double.NaN
                });
            }
            
            switch (straightProfilesReper.Count)
            {
                case int Cn when Cn > 1:

                    ////до первого репера
                    //double bottom_dx1 = straightProfilesReper[1].CoordAbs - straightProfilesReper[0].CoordAbs;

                    //double y1 = straightProfilesReper[0].Profile;
                    //double y2 = straightProfilesReper[1].Profile;

                    //for (int c = 0; c < straightProfilesReper[0].CoordAbs-X[0]; c++)
                    //{
                    //    var calc = -(y2 - y1) / bottom_dx1 * c + y1;
                    //    linearPointYReper.Add(calc);
                    //}

                    //между реперами
                    for (int t = 0; t < straightProfilesReper.Count() - 1; t++)
                    {
                        for (int c = 0; c < straightProfilesReper[t + 1].CoordAbs - straightProfilesReper[t].CoordAbs; c++)
                        {
                            var bottom_dx1 = straightProfilesReper[t + 1].CoordAbs - straightProfilesReper[t].CoordAbs;

                            var y1 = straightProfilesReper[t].Profile;
                            var y2 = straightProfilesReper[t + 1].Profile;

                            var calc = (y2 - y1) / bottom_dx1 * c + y1;

                            linearPointYReper.Add(calc);
                        }
                    }
                    linearPointYReper.Add(linearPointYReper.Last());

                    for (int ii = 1; ii < straightProfilesReper.Count; ii++)
                    {
                        straightProfilesReper[ii].Len = straightProfilesReper[ii].CoordAbs - straightProfilesReper[ii - 1].CoordAbs;
                        straightProfilesReper[ii].Slope = straightProfilesReper[ii].Profile - straightProfilesReper[ii - 1].Profile;
                    }
                    //после конечного репера
                    //bottom_dx1 = straightProfilesReper[straightProfilesReper.Count-1].CoordAbs - straightProfilesReper[straightProfilesReper.Count-2].CoordAbs;

                    //y1 = straightProfilesReper[straightProfilesReper.Count - 2].Profile;
                    //y2 = straightProfilesReper[straightProfilesReper.Count - 1].Profile;

                    //for (int c = 0; c < X[X.Count()-1] - straightProfilesReper[straightProfilesReper.Count - 1].CoordAbs; c++)
                    //{
                    //    var calc = +(y2 - y1) / bottom_dx1 * c + y1;
                    //    linearPointYReper.Add(calc);
                    //}

                    break;
                case int Cn when Cn == 0:
                    for (int t = 0; t < X.Count() - 1; t++)
                    {
                        linearPointYReper.Add(0.0);
                    }
                    break;
                case int Cn when Cn == 1:
                    for (int t = 0; t < X.Count() - 1; t++)
                    {
                        linearPointYReper.Add(straightProfilesReper[0].Profile);
                    }
                    break;
                default:
                    Console.WriteLine("Default case");
                    break;
            }            
        }

        List<Dictionary<string, string>> deviationPoints = new List<Dictionary<string, string>>();
        public List<Dictionary<string, string>> deviationPointsNew = new List<Dictionary<string, string>>();
        public string DEV_GRAPH = "";

        public String getRefrenceGraph()
        {
            //en birinshi nukte 
            int x0 = X[0];
            //944 pixel uzindihti jerge 5000 nukte sydiru kerek 
            //25 cm = 944pixel
            double x_coef = 944.9 / 5000.0;
            //grapic sizatin byiktikimiz px
            double grapH = 340.0;
            //grapicti ortasinnan bastap sizu kerek
            double middleH = grapH / 2.0;
            // ortasina barligin sydiru kerek masshtav
            // hagazga baylanisti san ==> 670
            double y_coef = grapH / 670;
            string linearGraph = "";
            double y0 = Y[0];
            Console.Out.WriteLine("y0==> " + y0);
            double calcX = 0.0;
            double calcY = 0.0;
            for (int i = 0; i < linearPointY.Count; i++)
            {
                calcX = (X[i] - x0) * x_coef;
                linearGraph += (calcX).ToString("0.####").Replace(',', '.') + ",";
                // db dan meter keledi sol metrdi cm ge convert jasau kerek
                calcY = (middleH - y_coef * (linearPointY[i] - y0) * 100.0);
                //eger y mani sizuga tysti byiktikten uken bolsa sydiru kerek
                //eger 0 bolsa tovesinnen bastau kerek
                if (calcY > 340)
                {
                    calcY -= 340;
                }
                if (calcY <= 0)
                {
                    calcY += 340;
                }
                //for deviation
                //Dictionary<string, string> dPoint = new Dictionary<string, string>();
                //double devX = calcX;
                //double devY = calcY - (middleH - y_coef * (Y[i] - y0) * 100.0);

                //dPoint.Add("x", devX.ToString("0.####").Replace(',', '.'));
                //dPoint.Add("y", devY.ToString("0.####").Replace(',', '.'));
                //deviationPoints.Add(dPoint);
                //end deviation

                linearGraph += calcY.ToString("0.####").Replace(',', '.') + " ";
            }
            //Console.Out.WriteLine("songi sizilgan nukteler "+linearGraph);
            return linearGraph;
        }


        public String getDeviationGraph() {

            var Y = newOriginY;

            //en birinshi nukte 
            int x0 = X[0];
            double y0 = Y[0];
            //944 pixel uzindihti jerge 5000 nukte sydiru kerek 
            //25 cm = 944pixel
            double x_coef = 944.9 / 5000.0;

            //grapic sizatin byiktikimiz px
            double grapH = 340.0;

            //grapicti ortasinnan bastap sizu kerek
            double middleH = grapH / 2.0;

            // ortasina barligin sydiru kerek masshtav
            // hagazga baylanisti san ==> 670
            double y_coef = grapH / 670;
            double calcX = 0.0;
            double calcY = 0.0;
            string devGraph = "";
            for (int i = 0; i < Y.Count(); i++){
                calcX = (X[i] - x0) * x_coef;
                devGraph += (calcX).ToString("0.####").Replace(',', '.') + ",";
                // db dan meter keledi sol metrdi cm ge convert jasau kerek
                calcY = (middleH + 230 - y_coef * (linearPointYNew[i]-Y[i]) * 300.0);
                //eger y mani sizuga tysti byiktikten uken bolsa sydiru kerek
                //eger 0 bolsa tovesinnen bastau kerek
                //if (calcY > 340)
                //{
                //    calcY = calcY - 340;
                //}
                //if (calcY <= 0)
                //{
                //    calcY = calcY + 340;
                //}

                devGraph += calcY.ToString("0.####").Replace(',', '.') + " ";
            }
            Console.Out.WriteLine("deviationGrap shvv de "+devGraph);


            return devGraph;
        }

        public String getDeviationGraphByReper()
        {

            var Y = ORIGINminusLBOplusLRrrr;

            //en birinshi nukte 
            int x0 = X[0];
            double y0 = Y[0];
            //944 pixel uzindihti jerge 5000 nukte sydiru kerek 
            //25 cm = 944pixel
            double x_coef = 944.9 / 5000.0;

            //grapic sizatin byiktikimiz px
            double grapH = 340.0;

            //grapicti ortasinnan bastap sizu kerek
            double middleH = grapH / 2.0;

            // ortasina barligin sydiru kerek masshtav
            // hagazga baylanisti san ==> 670
            double y_coef = grapH / 670;
            double calcX = 0.0;
            double calcY = 0.0;
            string devGraph = "";
            for (int i = 0; i < Y.Count(); i++)
            {
                calcX = (X[i] - x0) * x_coef;
                devGraph += (calcX).ToString("0.####").Replace(',', '.') + ",";
                // db dan meter keledi sol metrdi cm ge convert jasau kerek
                calcY = (395 - y_coef * (Y[i] -linearPointYReper[i]) * 10.0);
                //eger y mani sizuga tysti byiktikten uken bolsa sydiru kerek
                //eger 0 bolsa tovesinnen bastau kerek
                //if (calcY > 340)
                //{
                //    calcY = calcY - 340;
                //}
                //if (calcY <= 0)
                //{
                //    calcY = calcY + 340;
                //}

                devGraph += calcY.ToString("0.####").Replace(',', '.') + " ";
            }
            Console.Out.WriteLine("deviationGrap shvv de " + devGraph);


            return devGraph;
        }



        public Dictionary<String ,String> getRefGraph(List<RdProfile> refPoints, List<RdProfile> rdProfile)
        {
            try
            {
                //double y0 = Y[0];

                for (int t = 0; t < refPoints.Count(); t++)
                {
                    for (int m = 0; m < rdProfile.Count(); m++)
                    {
                        if (refPoints[t].Km == rdProfile[m].Km && refPoints[t].M == rdProfile[m].M)
                        {
                            Console.Out.WriteLine("tauilgan x jane y : " + rdProfile[m].X + " " + rdProfile[m].Y);
                            refPoints[t].X = rdProfile[m].X;
                            refPoints[t].Y = rdProfile[m].Y;
                            //Console.Out.WriteLine("tauilp odan manin algan : " + refPoints[t].X + " " + refPoints[t].Y);
                        }
                    }
                }


                List<int> refX = new List<int>();
                List<Double> refY = new List<double>();

                double linearY = Y[0];


                //944 pixel uzindihti jerge 5000 nukte sydiru kerek 
                //25 cm = 944pixel
                double x_coef = 944.9 / 5000.0;
                //grapic sizatin byiktikimiz px
                double grapH = 340.0;
                //grapicti ortasinnan bastap sizu kerek
                double middleH = grapH / 2.0;
                // ortasina barligin sydiru kerek masshtav
                // hagazga baylanisti san ==> 670
                double y_coef = grapH / 670;
                string refGrap = "";
                string pointGrap = "";


                double calcX = 0.0;
                double calcY = 0.0;
                //tochkalar ushin mark hoyu kerek
                double pointX = 0.0;
                double pointY = 0.0;

                for (int t = 0; t < refPoints.Count() - 1; t++)
                {
                    for (int c = refPoints[t].X; c <= refPoints[t + 1].X; c++)
                    {

                        // Console.Out.WriteLine("str x "+ straightProfiles[t + 1].CoordAbs);
                        double x = c;
                        double dx1 = x - refPoints[t].X;

                        double bottom_dx1 = refPoints[t + 1].X - refPoints[t].X;

                        double y2 = refPoints[t + 1].Y;
                        double dx2 = x - refPoints[t + 1].X;
                        double bottom_dx2 = refPoints[t].X - refPoints[t + 1].X;

                        double y1 = refPoints[t].Y;
                        //linearY = dx1 / bottom_dx1 * y2 + dx2 / bottom_dx2 * y2;
                        linearY = (y2 - y1) / bottom_dx1 * c + y1;
                        //Console.Out.WriteLine("test linear " + linearY);
                        //linearMass[counter] = dx1 / bottom_dx1 * y2 + dx2 / bottom_dx2  +Math.Cos(counter);
                        //counter++;
                        refX.Add(c);
                        refY.Add(linearY);
                    }

                }
                //en birinshi nukte 
                int x0 = refX[0];
                double y0 = refY[0];
                Console.Out.WriteLine("y0==> " + y0);
                Console.Out.WriteLine("refPoint size " + refPoints.Count);
                //for (int k = 0; k < refPoints.Count(); k++){
                //    //pointX = refPoints[k].X - X[0] * x_coef;

                //    //pointGrap += (pointX).ToString("0.####").Replace(',', '.') + ",";
                //    //// db dan meter keledi sol metrdi cm ge convert jasau kerek
                //    //pointY = (middleH - y_coef * (refPoints[k].Y - y0) * 100.0);
                //    ////eger y mani sizuga tysti byiktikten uken bolsa sydiru kerek
                //    ////eger 0 bolsa tovesinnen bastau kerek
                //    //if (pointY > 340)
                //    //{
                //    //    pointY = pointY - 340;
                //    //}
                //    //if (pointY <= 0)
                //    //{
                //    //    pointY = pointY + 340;
                //    //}


                //}
                for (int i = 0; i < refY.Count; i++)
                {
                    //calcX = (refX[i] + refPoints[0].X-X[0]) * x_coef;
                    calcX = (refX[i] - rdProfile[0].X) * x_coef;

                    refGrap += (calcX).ToString("0.####").Replace(',', '.') + ",";
                    // db dan meter keledi sol metrdi cm ge convert jasau kerek
                    calcY = (middleH - y_coef * (refY[i] - y0) * 100.0);



                    //eger y mani sizuga tysti byiktikten uken bolsa sydiru kerek
                    //eger 0 bolsa tovesinnen bastau kerek
                    if (calcY > 340)
                    {
                        calcY = calcY - 340;
                    }
                    if (calcY <= 0)
                    {
                        calcY = calcY + 340;
                    }

                    for (int k = 0; k < refPoints.Count(); k++)
                    {
                        if (refX[i] == refPoints[k].X)
                            pointGrap += $"{calcX.ToString("0.####").Replace(',', '.')},{calcY.ToString("0.####").Replace(',', '.')} ";
                    }
                    refGrap += calcY.ToString("0.####").Replace(',', '.') + " ";
                }

                var graps = new Dictionary<String, String>();
                Console.Out.WriteLine("pizdes " + refGrap);
                graps.Add("pointGrap", pointGrap);
                graps.Add("refGrap", refGrap);



                return graps;
                //Console.Out.WriteLine("songi sizilgan nukteler "+linearGraph);
            }
            catch (Exception e)
            {
                Console.Out.WriteLine("mal shig iishee orood ireed bna da ");
                Console.Error.WriteLine(e.StackTrace);
                return null;
            }

        }

        private void StraighteningProfileNew()
        {
            var Y = ORIGINminusLBOplusLRrrr;

            straightProfilesNew.Add(new StraightProfile
            {
                CoordAbs = X[0],
                Profile = Y[0],
                SlopeDiff = double.NaN
            });

            int i = 0;

            for (int j = i + 1; j < Y.Count - 1; j++)
            {
                double
                    koef_x = 0.2,

                    Bx = X[j] * koef_x - X[i] * koef_x,
                    By = Y[j] - Y[i];

                double otnosh = By / Bx;

                for (int k = i; k < j + 1; k++)
                {
                    var A = otnosh;
                    var B = -1;
                    var C = 0;
                    var Mx = k * koef_x - i * koef_x;
                    var My = Y[k] - Y[i];

                    //Формула для вычисления расстояния от точки до прямой на плоскости
                    var d = Math.Abs(A * Mx + B * My + C) / Math.Sqrt(A * A + B * B);

                    if (d > 0.15 && Math.Abs(straightProfilesNew.Last().CoordAbs - X[j - 1]) > 25)
                    {
                        var tempSP = straightProfilesNew.Last();
                        tempSP.Len = X[j - 1] - tempSP.CoordAbs;
                        //tempSP.Slope = Convert.ToDouble(Y[j - 1] - tempSP.Profile) * 10/ tempSP.Len;
                        tempSP.Slope = Convert.ToDouble(Y[j - 1] - tempSP.Profile);


                        straightProfilesNew.Add(new StraightProfile
                        {
                            CoordAbs = X[j - 1],
                            Profile = Y[j - 1]
                        });

                        i = j - 1;
                        break;
                    }
                }
            }



            if (!straightProfilesNew.Where(s => s.CoordAbs == X[Y.IndexOf(Y.Last())] && s.Profile == Y.Last()).Any())
            {
                var tempSP = straightProfilesNew.Last();
                tempSP.Len = X[Y.IndexOf(Y.Last())] - tempSP.CoordAbs;
                //tempSP.Slope = Convert.ToDouble(Y.Last() - tempSP.Profile) * 10 / tempSP.Len;
                tempSP.Slope = Convert.ToDouble(Y.Last() - tempSP.Profile);

                straightProfilesNew.Add(new StraightProfile
                {

                    CoordAbs = X[Y.IndexOf(Y.Last())],
                    Profile = Y.Last(),
                    Len = 0,
                    Slope = double.NaN,
                    SlopeDiff = double.NaN
                });
            }

            for (i = 1; i < straightProfilesNew.Count - 1; i++)
            {
                straightProfilesNew[i].SlopeDiff = straightProfilesNew[i].Slope - straightProfilesNew[i - 1].Slope;
            }



            double linearY = Y[0];
            //Console.Out.WriteLine("straightProfiles  size " + straightProfiles.Count());
            //int counter = 0;
            for (int t = 0; t < straightProfilesNew.Count() - 1; t++)
            {
                for (int c = 0; c < straightProfilesNew[t + 1].CoordAbs - straightProfilesNew[t].CoordAbs; c++)
                {

                    // Console.Out.WriteLine("str x "+ straightProfiles[t + 1].CoordAbs);
                    double x = straightProfilesNew[t].CoordAbs + c;
                    double dx1 = x - straightProfilesNew[t].CoordAbs;

                    double bottom_dx1 = straightProfilesNew[t + 1].CoordAbs - straightProfilesNew[t].CoordAbs;

                    double y2 = straightProfilesNew[t + 1].Profile;
                    double dx2 = x - straightProfilesNew[t + 1].CoordAbs;
                    double bottom_dx2 = straightProfilesNew[t].CoordAbs - straightProfilesNew[t + 1].CoordAbs;

                    double y1 = straightProfilesNew[t].Profile;
                    //linearY = dx1 / bottom_dx1 * y2 + dx2 / bottom_dx2 * y2;
                    linearY = (y2 - y1) / bottom_dx1 * c + y1;
                    //Console.Out.WriteLine("test linear " + linearY);
                    //linearMass[counter] = dx1 / bottom_dx1 * y2 + dx2 / bottom_dx2  +Math.Cos(counter);
                    //counter++;
                    linearPointYNew.Add(linearY);
                }
            }
        }
        private void ReperLOlinear()
        {
            var Y = ORIGINminusLBOplusLRrrr;
            var straightProfilesNew = DicReper;

            straightProfilesNew.Add(new StraightProfile
            {
                CoordAbs = X[0],
                Profile = Y[0],
                SlopeDiff = double.NaN
            });

            int i = 0;

            for (int j = i + 1; j < Y.Count - 1; j++)
            {
                double
                    Bx = X[j] - X[i],
                    By = Y[j] - Y[i];

                double otnosh = By / Bx;

                for (int k = i; k < j + 1; k++)
                {
                    double maxDistance = Math.Abs((Y[k] - Y[i]) - otnosh * (X[k] - X[i]));

                    if (maxDistance > 0.1 /*&& Math.Abs(straightProfilesNew.Last().CoordAbs - X[j - 1]) > 45*/)
                    {
                        var tempSP = straightProfilesNew.Last();
                        tempSP.Len = X[j - 1] - tempSP.CoordAbs;
                        tempSP.Slope = Convert.ToDouble(Y[j - 1] - tempSP.Profile) * 10 / tempSP.Len;


                        straightProfilesNew.Add(new StraightProfile
                        {
                            CoordAbs = X[j - 1],
                            Profile = Y[j - 1]
                        });

                        i = j - 1;
                        break;
                    }
                }
            }



            if (!straightProfilesNew.Where(s => s.CoordAbs == X.Last() && s.Profile == Y.Last()).Any())
            {
                var tempSP = straightProfilesNew.Last();
                tempSP.Len = X.Last() - tempSP.CoordAbs;
                tempSP.Slope = Convert.ToDouble(Y.Last() - tempSP.Profile) * 10 / tempSP.Len;

                straightProfilesNew.Add(new StraightProfile
                {

                    CoordAbs = X.Last(),
                    Profile = Y.Last(),
                    Len = 0,
                    Slope = double.NaN,
                    SlopeDiff = double.NaN
                });
            }

            for (i = 1; i < straightProfilesNew.Count - 1; i++)
            {
                straightProfilesNew[i].SlopeDiff = straightProfilesNew[i].Slope - straightProfilesNew[i - 1].Slope;
            }



            double linearY = Y[0];
            //Console.Out.WriteLine("straightProfiles  size " + straightProfiles.Count());
            //int counter = 0;
            for (int t = 0; t < straightProfilesNew.Count() - 1; t++)
            {
                for (int c = 0; c < straightProfilesNew[t + 1].CoordAbs - straightProfilesNew[t].CoordAbs; c++)
                {

                    // Console.Out.WriteLine("str x "+ straightProfiles[t + 1].CoordAbs);
                    double x = straightProfilesNew[t].CoordAbs + c;
                    double dx1 = x - straightProfilesNew[t].CoordAbs;

                    double bottom_dx1 = straightProfilesNew[t + 1].CoordAbs - straightProfilesNew[t].CoordAbs;

                    double y2 = straightProfilesNew[t + 1].Profile;
                    double dx2 = x - straightProfilesNew[t + 1].CoordAbs;
                    double bottom_dx2 = straightProfilesNew[t].CoordAbs - straightProfilesNew[t + 1].CoordAbs;

                    double y1 = straightProfilesNew[t].Profile;
                    //linearY = dx1 / bottom_dx1 * y2 + dx2 / bottom_dx2 * y2;
                    linearY = (y2 - y1) / bottom_dx1 * c + y1;
                    //Console.Out.WriteLine("test linear " + linearY);
                    //linearMass[counter] = dx1 / bottom_dx1 * y2 + dx2 / bottom_dx2  +Math.Cos(counter);
                    //counter++;
                    linearPointYReperByOrigLinear.Add(linearY);
                }
            }
        }

        public List<double> ReperY = new List<double>();
        public List<double> ReperX = new List<double>();

        public List<String> getLinearGraphNewReper()
        {
            //ReperLOlinear();
            var linearPointYReperByOrigLinear = linearPointYReper;

            List<string> dozenLinears = new List<string>();
            List<int> origX = X.ToList();
            int dozenCount = linearPointYReperByOrigLinear.Count % 5000 == 0 ? linearPointYReperByOrigLinear.Count / 5000 : linearPointYReperByOrigLinear.Count / 5000 + 1;
            //en birinshi nukte 
            int x0 = X[0];
            //944 pixel uzindihti jerge 5000 nukte sydiru kerek 
            //25 cm = 944pixel
            double x_coef = 944.9 / 5000.0;
            //grapic sizatin byiktikimiz px
            double grapH = 340.0;
            //grapicti ortasinnan bastap sizu kerek
            double middleH = grapH / 2.0;
            // ortasina barligin sydiru kerek masshtav
            // hagazga baylanisti san ==> 670
            double y_coef = (grapH / 670); //масштаб 1:100см

            var Y = linearPointYReperByOrigLinear;

            double y0 = linearPointYReperByOrigLinear[0];
            Console.Out.WriteLine("y0==> " + y0);


            double calcX = 0.0;
            double calcY = 0.0;

            for (int di = 0; di < dozenCount; di++)
            {

                string linearGraph = "";
                int xI = 0;
                for (int i = di * 5000; i < (di + 1) * 5000; i++)
                {
                    if (i < Math.Min(linearPointYReperByOrigLinear.Count, Y.Count))
                    {
                        calcX = (X[xI] - x0) * x_coef;
                        linearGraph += (calcX).ToString("0.####").Replace(',', '.') + ",";
                        // db dan meter keledi sol metrdi cm ge convert jasau kerek
                        calcY = (middleH - (25.0 / 57.0) * 1.7 * y_coef * (linearPointYReperByOrigLinear[i] - y0) * 100.0);

                        //eger y mani sizuga tysti byiktikten uken bolsa sydiru kerek
                        //eger 0 bolsa tovesinnen bastau kerek

                        int prom2 = 0;
                        prom2 = (int)calcY / 340;

                        if (calcY > 340)
                        {
                            calcY = calcY - 340.0 * prom2;
                        }
                        else if (calcY < 0)
                        {
                            calcY = calcY + 340.0 - 340.0 * prom2;
                        }

                        //for deviation
                        //Dictionary<string, string> dPoint = new Dictionary<string, string>();
                        //double devX = calcX;
                        //double devY = calcY - (middleH - y_coef * (Y[i] - y0) * 100.0);

                        //dPoint.Add("x", devX.ToString("0.####").Replace(',', '.'));
                        //dPoint.Add("y", devY.ToString("0.####").Replace(',', '.'));


                        //DEV_GRAPH += devX.ToString("0.####").Replace(',', '.') + "," + (devY).ToString("0.####").Replace(',', '.') + " ";

                        //deviationPointsNew.Add(dPoint);

                        //end deviation
                        xI++;
                        linearGraph += calcY.ToString("0.####").Replace(',', '.') + " ";

                        if (Xind.Where(o => o == i).ToList().Count() > 0)
                        {
                        //    ReperY.Add(calcY);
                        }

                        //LinearY.Add(linearPointYNew[i] - y0);
                    }
                }
                dozenLinears.Add(linearGraph);
            }

            //Console.Out.WriteLine("songi sizilgan nukteler "+linearGraph);
            return dozenLinears;

        }
    }
}
