using Accord.Imaging.Converters;
using Accord.Math;
using ALARm.Core;
using MatBlazor;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlarmPP.Web.Services
{
    public class OnlineModeData
    {
        public IMainTrackStructureRepository MainTrackStructureRepository { get; set; }
        public bool RouteEditable = false;
        public bool FirstLoad = true;
        public int CurrentFrameIndex { get; set; } = 0;
        public int Speed { get; set; } = 100;
        public bool Processing = true;
        public bool AutoScroll = true;
        
        public bool AutoPrint = false;
        public int Kilometer { get; set; } = -1;
        public int Meter { get; set; } = -1;
        public int GlobalMeter { get; set; } = 0;
        public int Picket { get; set; } = -1;
        public string Data { get; set; }
        public double[] PointsLeft { get; set; }
        public double[] PointsRight { get; set; }
        public string NominalRailScheme { get; set; }
        public string NominalTranslateLeft { get; set; }
        public string NominalTranslateRight { get; set; }
        public string NominalRotateLeft { get; set; }
        public string NominalRotateRight { get; set; }
        public string ViewBoxLeft { get; set; }
        public string ViewBoxRight { get; set; }
        public double VertWearLeft { get; set; }
        public double VertWearRight { get; set; }
        public double SideWearLeft { get; set; }
        public double SideWearRight { get; set; }
        public double Wear45Left { get; set; } 
        public double Wear45Right { get; set; }
        public double DownhillLeft { get; set; }
        public double DownhillRight { get; set; } 
        public string ViewBox { get; set; }
        public List<int> Dy = new List<int>();
        public int CalibrConstLeft { get; set; } = 0;
        public int CalibrConstRight { get; set; } = 0;
        public int CalibrConstRight1 { get; set; } = 0;
        public int ScaleCoef = 1000;
        public int WearCoef = 4;
        public List<String> CorrectionsList = new List<string>();
        public List<int> Corrections = new List<int>();
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
        List<int> dy = new List<int>();
        public Bitmap GetBitmapAsync(String filePath, int frameNumber)
        {
            try
            {

                using BinaryReader reader = new BinaryReader(File.Open(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
                try
                {
                    //var data = r;
                    //Array.Reverse(data);
                    int width = reader.ReadInt32();
                    //var data = reader.ReadBytes(2);
                    //Array.Reverse(data);
                    //reader.ReadByte();
                    int height = reader.ReadInt32();
                    int frameSize = width * height;
                    long position = (long)frameNumber * (long)frameSize + 8;
                    reader.BaseStream.Seek(position, SeekOrigin.Begin);
                    var beginMark = reader.ReadInt64();
                    var encoderCounter1 = reader.ReadInt32();
                    var speed = reader.ReadInt32();
                    var datetime = reader.ReadDouble();
                    var encoderCounter3 = reader.ReadInt32();
                    var frameNumberCam = reader.ReadInt32();
                    var camTimer = reader.ReadInt64();
                    Kilometer = reader.ReadInt32();


                    reader.BaseStream.Seek(position, SeekOrigin.Begin);


                    byte[] by = reader.ReadBytes(frameSize);
                    var result = ConvertMatrix(Array.ConvertAll(by, Convert.ToInt32), height, width);

                    // var vect = GetIndexesOfColumnsMax(result);
                    //Data = VectorToPoints(vect);

                    //CurrentProfileLeft();
                    //CurrentProfileRight();

                    reader.Close();
                    return result.ToBitmap();

                }
                catch
                {
                    CurrentFrameIndex = -1;
                    Processing = false;
                    return null;
                }
            }
            catch (Exception e)
            {
                
                Console.WriteLine(e.Message);
                return null;
            }
        }
        public void CurrentProfileLeft()
        {

            var filePath = @"D:\common\DATA\IN\2019_04\Vnutr__profil__koridor2020_03_05_13_57_30.Profile_Calibr";
            if (!File.Exists(filePath))
                filePath = @"E:\059M\ShablonKORR\Profile_Calibr\Vnutr__profil__koridor2020_06_12_23_27_22.Profile_Calibr";
            if (File.Exists(filePath))
                using (BinaryReader reader = new BinaryReader(File.OpenRead(filePath)))
            {

                try
                {
                    var count = reader.ReadSingle();
                    var size = reader.ReadSingle();    
                    
                    long ll = CurrentFrameIndex * (long)size + 8;
                    reader.BaseStream.Seek(ll, SeekOrigin.Begin);
                    
                    var encoderCounter = reader.ReadUInt32();
                    var speed = reader.ReadInt32();
                    reader.ReadBytes(12);
                   var frameNumber = reader.ReadUInt32();
                   var kilometer = reader.ReadUInt32();
                    var meter = reader.ReadUInt32();
                   reader.ReadBytes(16);


                                       
                    List<double> arrX = new List<double>();
                    List<double> arrY = new List<double>();
                    List<double> vector = new List<double>();
                    string viewBox = String.Empty;
                    for (int i = 0; i < ((int)size - 40) / 8; i++)
                    {
                        double x = reader.ReadSingle();
                        double y = reader.ReadSingle();
                        if (!Double.IsNaN(x) && !Double.IsNaN(y) && x < 250)
                        {
                            arrX.Add(x);
                            arrY.Add(y);
                        }
                    }
                    CalcProfile(Rails.r50, arrX, arrY, Side.Left, ref vector, ref viewBox);

                    PointsLeft = vector.ToArray();
                    ViewBoxLeft = viewBox;
                }
                catch (Exception e)
                {
                    System.Console.WriteLine(e.Message);
                    PointsLeft = null;
                    ViewBoxLeft = "0 0 0 0";
                }
            }
        }

        public void CurrentProfileRight()
        {

            var filePath = @"D:\common\DATA\IN\2019_04\Vnutr__profil__kupe2020_03_05_13_57_30.Profile_Calibr";
            if (!File.Exists(filePath))
                filePath = @"E:\059M\ShablonKUPE\Profile_Calibr\Vnutr__profil__kupe2020_06_12_23_27_22.Profile_Calibr";
            if (File.Exists(filePath))
                using (BinaryReader reader = new BinaryReader(File.Open(filePath, FileMode.Open, FileAccess.Read,FileShare.ReadWrite)))
            {
                try
                {
                        var count = reader.ReadSingle();
                        var size = reader.ReadSingle();

                        long ll = CurrentFrameIndex * (long)size + 8;
                        reader.BaseStream.Seek(ll, SeekOrigin.Begin);

                        var encoderCounter = reader.ReadUInt32();
                        var speed = reader.ReadInt32();
                        reader.ReadBytes(12);
                        var frameNumber = reader.ReadUInt32();
                        var kilometer = reader.ReadUInt32();
                        var meter = reader.ReadUInt32();
                        reader.ReadBytes(16);

                    List<double> arrX = new List<double>();
                    List<double> arrY = new List<double>();
                    List<double> vector = new List<double>();
                    string viewBox = String.Empty;
                    for (int i = 0; i < ((int)size - 40) / 8; i++)
                    {
                        double x = reader.ReadSingle();
                        double y = reader.ReadSingle();
                        if (!Double.IsNaN(x) && !Double.IsNaN(y) && x < 250)
                        {
                            arrX.Add(x);
                            arrY.Add(y);
                        }
                    }
                    CalcProfile(Rails.r50, arrX, arrY, Side.Right, ref vector, ref viewBox);

                    PointsRight = vector.ToArray();
                    ViewBoxRight = viewBox;
                }
                catch (Exception e)
                {
                    System.Console.WriteLine(e.Message);
                    PointsRight = null;
                    ViewBoxRight = "0 0 0 0";
                }
            }
        }
        public void GetBitmapAsync()
        {
           
                  //  CurrentProfileLeft();
                   // CurrentProfileRight();
                   

               
          
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
        /// <summary>
        ///  Расчет профиля головки рельса
        /// </summary>
        /// <param name="rails">тип рельсы</param>
        /// <param name="arrX">координаты головки по оси х</param>
        /// <param name="arrY">координаты головки по оси Y</param>
        /// <param name="side">рельсовая нить</param>
        /// <param name="vector">массив координат</param>
        public void CalcProfile(Rails rails, List<double> arrX, List<double> arrY, Side side, ref List<double> vector, ref string viewBox)
        {
            double vbMinX, vbMinY, vbX, vbY, trX, trY, rotate;

            ProfileCalcParameter calcParam;
            switch (rails)
            {
                case Rails.r75:
                    calcParam = new ProfileCalcParameter(61.5, 450, 25, 85.1668859, 46 - 5, 192, 81.4 + 20, 29.3486179, 17.4545352, 425.0000933);
                    break;
                case Rails.r65:
                    calcParam = new ProfileCalcParameter(61.9, 400, 25, 84.5623876, 35.6 - 5, 180, 71.25 + 20, 33.28728189, 17.31756333, 375.0000105);
                    break;
                case Rails.r50:
                    calcParam = new ProfileCalcParameter(50.5, 350, 20, 85.0600398, 33 - 5, 152, 52.75 + 20, 29.92824734, 17.4493911, 330.0000048);
                    break;
                case Rails.r43:
                    calcParam = new ProfileCalcParameter(41.1, 350, 15, 85.176281, 30.4 - 5, 140, 56.75 + 20, 34.72074314, 16.49536762, 335.0000848);
                    break;
                default:
                    return;
            }

            double maxY = arrY.Max();
            arrY = arrY.Select(a => a * (-1) + maxY).ToList();

            List<double> sideX = new List<double>(), sideY = new List<double>(), headX = new List<double>(), headY = new List<double>();
            TryReadProfile(ref sideX, ref sideY, ref headX, ref headY, arrX.ToArray(), arrY.ToArray(), calcParam.HeadCoef, calcParam.BottomSideCoef, calcParam.TopSideCoef);

            double[] arrSideX = sideX.ToArray();
            double[] arrSideY = sideY.ToArray();
            double[] arrHeadX = headX.ToArray();
            double[] arrHeadY = headY.ToArray();

            if (arrSideX.Length < 1 || arrSideY.Length < 1 || arrHeadX.Length < 1 || arrHeadY.Length < 1)
            {
                if (side == Side.Left)
                {
                    DownhillLeft = -1;
                    VertWearLeft = -1;
                }
                else
                {
                    DownhillRight = -1;
                    VertWearRight = -1;
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

            double xrb = -1, yrb = -1, delta = -1;

            SearchCenterPointsAndDelta(ref xrb, ref yrb, ref delta, arrSideX, arrSideY, toDownIndex, calcParam);

            vbMinX = vector.Where((x, i) => i % 2 == 0).Min() - 10.0;
            vbX = vector.Where((x, i) => i % 2 == 0).Max() - vector.Where((x, i) => i % 2 == 0).Min() + 20.0;
            vbMinY = vector.Where((y, i) => i % 2 != 0).Min() - 10.0;
            vbY = vector.Where((y, i) => i % 2 != 0).Max() - vector.Where((y, i) => i % 2 != 0).Min() + 20.0;
            viewBox = vbMinX.ToString().Replace(",", ".") + " " + vbMinY.ToString().Replace(",", ".") + " " + vbX.ToString().Replace(",", ".") + " " + vbY.ToString().Replace(",", ".");

            if (xrb < 0 && yrb < 0)
            {
                if (side == Side.Left)
                    DownhillLeft = 0;
                else
                    DownhillRight = 0;

                NominalTranslateLeft = "0,0";
                NominalTranslateRight = "0,0";
                NominalRotateLeft = "0";
                NominalRotateRight = "0";
                return;
            }

            switch (rails)
            {
                case Rails.r75:
                    NominalTranslateLeft = "0,0";
                    NominalTranslateRight = "0,0";
                    NominalRotateLeft = "0";
                    NominalRotateRight = "0";
                    break;
                case Rails.r65:
                    NominalTranslateLeft = "0,0";
                    NominalTranslateRight = "0,0";
                    NominalRotateLeft = "0";
                    NominalRotateRight = "0";
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
                    NominalTranslateLeft = "0,0";
                    NominalTranslateRight = "0,0";
                    NominalRotateLeft = "0";
                    NominalRotateRight = "0";
                    break;
                default:
                    break;
            }

            //GetProfileDiagram(ref schemeVector, xrb, yrb, delta, rails, side, Math.Max(arrHeadX.Max(), arrSideX.Max()), Math.Max(arrHeadY.Max(), arrSideY.Max()));

            if (side == Side.Left)
                DownhillLeft =(-(delta + CalibrConstLeft));
            else
                DownhillRight = (-(delta + CalibrConstRight1 + CalibrConstRight));

            double wear_delta = -1, wear45_delta = -1, wear_side = -1;
            GetWears(ref wear_delta, ref wear45_delta, ref wear_side, xrb, yrb, arrHeadX, arrHeadY, delta, rails);

            if (side == Side.Left)
            {
                VertWearLeft = wear_delta;
                SideWearLeft = wear_side;
                Wear45Left = wear45_delta;
            }
            else
            {
                VertWearRight = wear_delta;
                SideWearRight = wear_side;
                Wear45Right = wear45_delta;
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
                my[i - 1] = tempArrY.Skip((i - 1) * 25 + x0).Take(25).Max();
            }

            topY = my.Min();

            for (int i = 0; i < Math.Min(arrX.Length, arrY.Length); i++)
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
                    double x1 = arrSideX.Get(Math.Max(i - 2, 0), i + 3).Average(), y1 = arrSideY.Get(Math.Max(i - 2, 0), i + 3).Average();
                    if (GetDistanceBetweenPoints(arrSideX.First(), x1, arrSideY.First(), y1) > calcParam.DistBigRad)
                    {
                        double x2r = 0.0, y2r = 0.0;
                        x1 = arrSideX.Get(Math.Max(toDownIndex - 2, 0), (toDownIndex + 3) % arrSideX.Length).Average();
                        y1 = arrSideY.Get(Math.Max(toDownIndex - 2, 0), (toDownIndex + 3) % arrSideX.Length).Average();
                        bool isFirst = true;

                        for (int j = toDownIndex + 1; j < arrSideX.Length; j++)
                        {
                            double x2 = arrSideX.Get(j - 2, j + 3).Average(), y2 = arrSideY.Get(j - 2, j + 3).Average();
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
                        double x2 = arrSideX.Get(Math.Max(j - 2, 0), j + 3).Average(), y2 = arrSideY.Get(Math.Max(j - 2, 0), j + 3).Average();
                        if (IsClose(GetDistanceBetweenPoints(x1, x2, y1, y2), calcParam.DistBigRad, 0.01, 0.1))
                        {
                            for (int k = j + 1; j < arrSideX.Length - 2; k++)
                            {
                                double x3 = arrSideX.Get(k - 2, k + 3).Average(), y3 = arrSideY.Get(k - 2, k + 3).Average();
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
                    return true;
                }
                else
                {
                    x0 = x02;
                    y0 = y02;
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
        /// <returns>Отрицательное значение угла если точка находится в левую сторону от центра окружности, в противном случае положительное значение угла в радиусах</returns>
        private double GetAngleDiff(double xr, double yr, double x2r, double y2r, double radius)
        {
            double ab = Math.Sqrt(Math.Pow(x2r - xr, 2.0) + Math.Pow(y2r - (yr - radius), 2.0));
            double alpha = ((180 / Math.PI) * Math.Asin(ab / (radius * 2))) * 2;

            if (x2r <= xr)
                return -alpha;
            else
                return alpha;
        }

        /// <summary>
        /// Нахождение вертикального и 45 градусного износа
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
                    break;
                case Rails.r65:
                    /*xc1 = (-408.9999887) * Math.Cos(delta * Math.PI / 180) - (-82.4047363) * Math.Sin(delta * Math.PI / 180) + x0;
                    yc1 = (-408.9999887) * Math.Sin(delta * Math.PI / 180) + (-82.4047363) * Math.Cos(delta * Math.PI / 180) + y0;*/
                    xc2 = (-408.9999887) * Math.Cos(delta * Math.PI / 180) - (97.5952637) * Math.Sin(delta * Math.PI / 180) + x0;
                    yc2 = (-408.9999887) * Math.Sin(delta * Math.PI / 180) + (97.5952637) * Math.Cos(delta * Math.PI / 180) + y0;
                    break;
                case Rails.r50:
                    /*xc1 = (-357.9999183) * Math.Cos(delta * Math.PI / 180) - (-68.7391269) * Math.Sin(delta * Math.PI / 180) + x0;
                    yc1 = (-357.9999183) * Math.Sin(delta * Math.PI / 180) + (-68.7391269) * Math.Cos(delta * Math.PI / 180) + y0;*/
                    xc2 = (-357.9999183) * Math.Cos(delta * Math.PI / 180) - (83.2608731) * Math.Sin(delta * Math.PI / 180) + x0;
                    yc2 = (-357.9999183) * Math.Sin(delta * Math.PI / 180) + (83.2608731) * Math.Cos(delta * Math.PI / 180) + y0;
                    break;
                case Rails.r43:
                    /*xc1 = (-357.2103467) * Math.Cos(delta * Math.PI / 180) - (-63.2316258) * Math.Sin(delta * Math.PI / 180) + x0;
                    yc1 = (-357.2103467) * Math.Sin(delta * Math.PI / 180) + (-63.2316258) * Math.Cos(delta * Math.PI / 180) + y0;*/
                    xc2 = (-357.2103467) * Math.Cos(delta * Math.PI / 180) - (76.7683742) * Math.Sin(delta * Math.PI / 180) + x0;
                    yc2 = (-357.2103467) * Math.Sin(delta * Math.PI / 180) + (76.7683742) * Math.Cos(delta * Math.PI / 180) + y0;
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
            delta_wear = arrHeadX.Zip(arrHeadY, (x, y) => GetDistanceBetweenPoints(x, xc2, y, yc2)).Min();

            switch (rails)
            {
                case Rails.r75:
                    /*xc1 = (-459.999949) * Math.Cos(delta * Math.PI / 180) - (95.2858112) * Math.Sin(delta * Math.PI / 180) + x0;
                    yc1 = (-459.999949) * Math.Sin(delta * Math.PI / 180) + (95.2858112) * Math.Cos(delta * Math.PI / 180) + y0;*/
                    xc2 = (-419.999949) * Math.Cos(delta * Math.PI / 180) - (135.2858112) * Math.Sin(delta * Math.PI / 180) + x0;
                    yc2 = (-419.999949) * Math.Sin(delta * Math.PI / 180) + (135.2858112) * Math.Cos(delta * Math.PI / 180) + y0;
                    break;
                case Rails.r65:
                    /*xc1 = (-408.9999887) * Math.Cos(delta * Math.PI / 180) - (85.5952637) * Math.Sin(delta * Math.PI / 180) + x0;
                    yc1 = (-408.9999887) * Math.Sin(delta * Math.PI / 180) + (85.5952637) * Math.Cos(delta * Math.PI / 180) + y0;*/
                    xc2 = (-368.9999887) * Math.Cos(delta * Math.PI / 180) - (125.5952637) * Math.Sin(delta * Math.PI / 180) + x0;
                    yc2 = (-368.9999887) * Math.Sin(delta * Math.PI / 180) + (125.5952637) * Math.Cos(delta * Math.PI / 180) + y0;
                    break;
                case Rails.r50:
                    /*xc1 = (-357.9999183) * Math.Cos(delta * Math.PI / 180) - (71.2608731) * Math.Sin(delta * Math.PI / 180) + x0;
                    yc1 = (-357.9999183) * Math.Sin(delta * Math.PI / 180) + (71.2608731) * Math.Cos(delta * Math.PI / 180) + y0;*/
                    xc2 = (-327.2415861) * Math.Cos(delta * Math.PI / 180) - (77.7121591) * Math.Sin(delta * Math.PI / 180) + x0;
                    yc2 = (-327.2415861) * Math.Sin(delta * Math.PI / 180) + (77.7121591) * Math.Cos(delta * Math.PI / 180) + y0;
                    break;
                case Rails.r43:
                    /*xc1 = (-357.2103467) * Math.Cos(delta * Math.PI / 180) - (64.7683742) * Math.Sin(delta * Math.PI / 180) + x0;
                    yc1 = (-357.2103467) * Math.Sin(delta * Math.PI / 180) + (64.7683742) * Math.Cos(delta * Math.PI / 180) + y0;*/
                    xc2 = (-317.2103467) * Math.Cos(delta * Math.PI / 180) - (104.7683742) * Math.Sin(delta * Math.PI / 180) + x0;
                    yc2 = (-317.2103467) * Math.Sin(delta * Math.PI / 180) + (104.7683742) * Math.Cos(delta * Math.PI / 180) + y0;
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
            delta_45wear = arrHeadX.Zip(arrHeadY, (x, y) => GetDistanceBetweenPoints(x, xc2, y, yc2)).Min();

            switch (rails)
            {
                case Rails.r75:
                    /*xc1 = (-459.999949) * Math.Cos(delta * Math.PI / 180) - (95.2858112) * Math.Sin(delta * Math.PI / 180) + x0;
                    yc1 = (-459.999949) * Math.Sin(delta * Math.PI / 180) + (95.2858112) * Math.Cos(delta * Math.PI / 180) + y0;*/
                    xc2 = (-423.999949) * Math.Cos(delta * Math.PI / 180) - (91.6858112) * Math.Sin(delta * Math.PI / 180) + x0;
                    yc2 = (-423.999949) * Math.Sin(delta * Math.PI / 180) + (91.6858112) * Math.Cos(delta * Math.PI / 180) + y0;
                    break;
                case Rails.r65:
                    /*xc1 = (-408.9999887) * Math.Cos(delta * Math.PI / 180) - (85.5952637) * Math.Sin(delta * Math.PI / 180) + x0;
                    yc1 = (-408.9999887) * Math.Sin(delta * Math.PI / 180) + (85.5952637) * Math.Cos(delta * Math.PI / 180) + y0;*/
                    xc2 = (-372.4999887) * Math.Cos(delta * Math.PI / 180) - (81.8952637) * Math.Sin(delta * Math.PI / 180) + x0;
                    yc2 = (-372.4999887) * Math.Sin(delta * Math.PI / 180) + (81.8952637) * Math.Cos(delta * Math.PI / 180) + y0;
                    break;
                case Rails.r50:
                    /*xc1 = (-357.9999183) * Math.Cos(delta * Math.PI / 180) - (71.2608731) * Math.Sin(delta * Math.PI / 180) + x0;
                    yc1 = (-357.9999183) * Math.Sin(delta * Math.PI / 180) + (71.2608731) * Math.Cos(delta * Math.PI / 180) + y0;*/
                    xc2 = (-322.8999183) * Math.Cos(delta * Math.PI / 180) - (67.8608731) * Math.Sin(delta * Math.PI / 180) + x0;
                    yc2 = (-322.8999183) * Math.Sin(delta * Math.PI / 180) + (67.8608731) * Math.Cos(delta * Math.PI / 180) + y0;
                    break;
                case Rails.r43:
                    /*xc1 = (-357.2103467) * Math.Cos(delta * Math.PI / 180) - (64.7683742) * Math.Sin(delta * Math.PI / 180) + x0;
                    yc1 = (-357.2103467) * Math.Sin(delta * Math.PI / 180) + (64.7683742) * Math.Cos(delta * Math.PI / 180) + y0;*/
                    xc2 = (-319.2103467) * Math.Cos(delta * Math.PI / 180) - (62.8683742) * Math.Sin(delta * Math.PI / 180) + x0;
                    yc2 = (-319.2103467) * Math.Sin(delta * Math.PI / 180) + (62.8683742) * Math.Cos(delta * Math.PI / 180) + y0;
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

            delta_sidewear = Math.Sqrt(Math.Pow((xc2 - xc), 2.0) + Math.Pow((yc2 - yc), 2.0));
            delta_sidewear = distArr.Min();*/
            delta_sidewear = arrHeadX.Zip(arrHeadY, (x, y) => GetDistanceBetweenPoints(x, xc2, y, yc2)).Min();
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
    }
    public enum Rails { r75 = 192, r65 = 180, r50 = 152, r43 = 140 }
    public enum Side { Left = -1, Right = 1 }

    public class ProfileCalcParameter
    {
        public double DownParam { get; set; } = -1;
        public double Radius { get; set; } = -1;
        public double LittleRadius { get; set; } = -1;
        public double AngleG { get; set; } = -1;
        public double HeadCoef { get; set; } = 0;
        public double TopSideCoef { get; set; } = 0;
        public double BottomSideCoef { get; set; } = 0;
        public double DistBigRad { get; set; } = 0;
        public double DistLitRad { get; set; } = 0;
        public double DistParam { get; set; } = 0;
        public ProfileCalcParameter(double downParam, double radius, double l_radius, double angle_g, double coefHead, double coefSideBot, double coefSideTop, double dist_big_rad, double dist_lit_rad, double dist_param)
        {
            DownParam = downParam;
            Radius = radius;
            LittleRadius = l_radius;
            AngleG = angle_g;
            HeadCoef = coefHead;
            TopSideCoef = coefSideTop;
            BottomSideCoef = coefSideBot;
            DistLitRad = dist_lit_rad;
            DistBigRad = dist_big_rad;
            DistParam = dist_param;
        }
    }
}
