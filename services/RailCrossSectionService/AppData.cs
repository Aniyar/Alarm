using Npgsql;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace RailCrossSectionService
{
    class AppData
    {
        public string Cs = "Host=DESKTOP-EMAFC5J;Username=postgres;Password=alhafizu;Database=railway_copy";
        public NpgsqlConnection Conn { get; set; }
        public int CurrentFrameIndex { get; set; } = 10653; 

        //public string Vnutr__profil__koridor = @"C:\sntfi\ProfileProject\213_Vnutr__profil__koridor_2020_10_26_10_38_24.Profile_Calibr";
        //public string Vnutr__profil__kupe = @"C:\sntfi\ProfileProject\213_Vnutr__profil__kupe_2020_10_26_10_38_24.Profile_Calibr";

        public string Vnutr__profil__kupe = @"F:\o59m\ShablonKORR\Profile_Calibr\242_Vnutr__profil__koridor_2021_10_18_16_43_58.Profile_Calibr";
        public string Vnutr__profil__koridor = @"F:\o59m\ShablonKUPE\Profile_Calibr\242_Vnutr__profil__kupe_2021_10_18_16_43_58.Profile_Calibr";

        public BinaryReader In_koridor { get; set; }
        public BinaryReader In_kupe { get; set; }
        public BinaryReader Top_koridor { get; set; }
        public BinaryReader Top_kupe { get; set; }
        public int Kilometer { get; set; } = -1;
        public int Meter { get; set; } = -1;
        public int Picket { get; set; } = -1;

        internal float In_kupe_count;
        internal float In_kupe_size;
        internal float In_koridor_count;
        internal float In_koridor_size;

        public string NominalRailScheme { get; set; }
        public string GetNominalRailScheme(Rails rails)
        {
            throw new NotImplementedException();
        }
        public enum Rails { r75 = 192, r65 = 180, r50 = 152, r43 = 140 }
        public enum Side { Left = -1, Right = 1 }
        public bool Status { get; set; } = true;
        public List<double> pu_l { get; set; } = new List<double>(); //пу л
        public List<double> pu_r { get; set; } = new List<double>();//пу пр
        public List<double> vert_l { get; set; } = new List<double>();//верт
        public List<double> vert_r { get; set; } = new List<double>();
        public List<double> bok_l { get; set; } = new List<double>(); //иб л
        public List<double> bok_r { get; set; } = new List<double>();//иб пр
        public List<double> npk_l { get; set; } = new List<double>();//нпк л
        public List<double> npk_r { get; set; } = new List<double>();//нпк л
        public double Xtest1 { get; set; } //шейка 2 нукте лев
        public double Ytest1 { get; set; }
        public double Xtest2 { get; set; }
        public double Ytest2 { get; set; }
        public double Xtest3 { get; set; } //головка 2 нукте лев
        public double Ytest3 { get; set; }
        public double Xtest4 { get; set; }
        public double Ytest4 { get; set; }
        public double Xtest1_r { get; set; } // шейка 2 нукте прав
        public double Ytest1_r { get; set; }
        public double Xtest2_r { get; set; }
        public double Ytest2_r { get; set; }
        public double Xtest3_r { get; set; } //головка 2 нукте  прав
        public double Ytest3_r { get; set; }
        public double Xtest4_r { get; set; }
        public double Ytest4_r { get; set; }
        public double Xtest5 { get; set; }
        public double Ytest5 { get; set; }
        public double Xtest5_r { get; set; }
        public double Ytest5_r { get; set; }

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
        public void CalcProfile(Rails rails, List<double> arrX, List<double> arrY, Side side, ref List<double> vector, ref string viewBox)
        {
            double vbMinX, vbMinY, vbX, vbY, trX, trY, rotate;
            ProfileCalcParameter calcParam;
            switch (rails)
            {
                case Rails.r75:
                    calcParam = new ProfileCalcParameter(61.5, 450, 25, 85.1668859, 46, 192, 81.4, 29.3486179, 17.4545352, 425.0000933);
                    break;
                case Rails.r65:
                    calcParam = new ProfileCalcParameter(61.9, 400, 25, 84.5623876, 35.6, 180, 71.25, 33.28728189, 17.31756333, 375.0000105);
                    break;
                case Rails.r50:
                    calcParam = new ProfileCalcParameter(50.5, 350, 20, 85.0600398, 33, 152, 52.75, 29.92824734, 17.4493911, 330.0000048);
                    break;
                case Rails.r43:
                    calcParam = new ProfileCalcParameter(41.1, 350, 15, 85.176281, 30.4, 140, 56.75, 34.72074314, 16.49536762, 335.0000848);
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
                    pu_l.Add(-1);
                    vert_l.Add(-1);
                    bok_l.Add(-1);
                    npk_l.Add(-1);
                }
                else
                {
                    pu_r.Add(-1);
                    vert_r.Add(-1);
                    bok_r.Add(-1);
                    npk_r.Add(-1);
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

            //if (arrSideY.First() < arrSideY.Last())
            //{
            //    Array.Reverse(arrSideX);
            //    Array.Reverse(arrSideY);
            //}

            //double headDownY = arrHeadY.Min();
            //double toDown = headDownY - calcParam.DownParam;
            //int toDownIndex = -1;
            //for (int i = 0; i < arrSideY.Length; i++)
            //{
            //    if (arrSideY[i] < toDown)
            //    {
            //        toDownIndex = i;
            //        break;
            //    }
            //}
            ////---Табаны-----------------------------------------------------------
            //List<double> sideX1 = new List<double>(), sideY1 = new List<double>();
            //List<double> sideX2 = new List<double>(), sideY2 = new List<double>();
            //var side_count = arrSideY.Length / 3;
            //var level_side_count = side_count / 3;

            //for (int i = 0; i <= side_count; i++)
            //{
            //    if (i < level_side_count)
            //    {
            //        sideX1.Add(arrSideX[i]);
            //        sideY1.Add(arrSideY[i]);
            //    }
            //    if (i > level_side_count * 2)
            //    {
            //        sideX2.Add(arrSideX[i]);
            //        sideY2.Add(arrSideY[i]);
            //    }
            //}

            ////шейка 2 нукте лев
            //if (side == Side.Left)
            //{
            //    var x1min = sideX1.Min();
            //    var y1min = sideY1[sideX1.IndexOf(x1min)];

            //    var x2min = sideX2.Min();
            //    var y2min = sideY2[sideX2.IndexOf(x2min)];

            //    Xtest1 = x1min; //green
            //    Ytest1 = y1min * (-1) + Math.Max(arrHeadY.Max(), arrSideY.Max());
            //    Xtest2 = x2min; //red
            //    Ytest2 = y2min * (-1) + Math.Max(arrHeadY.Max(), arrSideY.Max());
            //}
            ////шейка 2 нукте прав
            //if (side == Side.Right)
            //{
            //    var x1min = sideX1.Min();
            //    var y1min = sideY1[sideX1.IndexOf(x1min)];

            //    var x2min = sideX2.Min();
            //    var y2min = sideY2[sideX2.IndexOf(x2min)];

            //    Xtest1_r = x1min * (-1) + Math.Max(arrHeadX.Max(), arrSideX.Max());
            //    Ytest1_r = y1min * (-1) + Math.Max(arrHeadY.Max(), arrSideY.Max());
            //    Xtest2_r = x2min * (-1) + Math.Max(arrHeadX.Max(), arrSideX.Max());
            //    Ytest2_r = y2min * (-1) + Math.Max(arrHeadY.Max(), arrSideY.Max());
            //}
            ////----головка---------------------------------------------------------
            //List<double> headX1 = new List<double>(), headY1 = new List<double>();
            //List<double> headX2 = new List<double>(), headY2 = new List<double>();
            //var head_count = arrHeadY.Length / 3;
            //var level_head_count = head_count / 3;

            //if (side == Side.Left)
            //{
            //    Array.Reverse(arrHeadX);
            //    Array.Reverse(arrHeadY);
            //    for (int i = 0; i <= head_count; i++)
            //    {
            //        if (i < level_head_count)
            //        {
            //            headX1.Add(arrHeadX[i]);
            //            headY1.Add(arrHeadY[i]);
            //        }
            //        if (i > level_head_count * 2)
            //        {
            //            headX2.Add(arrHeadX[i]);
            //            headY2.Add(arrHeadY[i]);
            //        }
            //    }
            //    var x1min = headX1[headY1.IndexOf(headY1.Min())];
            //    var y1min = headY1.Min();

            //    var x2min = headX2[headY2.IndexOf(headY2.Min())];
            //    var y2min = headY2.Min();

            //    var vremX1 = new List<double> { };
            //    var vremY1 = new List<double> { };
            //    var vremX2 = new List<double> { };
            //    var vremY2 = new List<double> { };

            //    //g
            //    for (int i = headY1.IndexOf(headY1.Min()); i < headY1.IndexOf(headY1.Min()) + 7; i++)
            //    {
            //        vremX1.Add(headX1[i]);
            //        vremY1.Add(headY1[i]);
            //    }
            //    //r
            //    for (int i = headY2.IndexOf(headY2.Min()) - 7; i < headY2.IndexOf(headY2.Min()); i++)
            //    {
            //        vremX2.Add(headX2[i]);
            //        vremY2.Add(headY2[i]);
            //    }

            //    //тобедегы 2 нукте
            //    Xtest3 = vremX1.Average(); //g
            //    Ytest3 = vremY1.Average() * (-1) + Math.Max(arrHeadY.Max(), arrSideY.Max());
            //    Xtest4 = vremX2.Average(); //r
            //    Ytest4 = vremY2.Average() * (-1) + Math.Max(arrHeadY.Max(), arrSideY.Max());
            //}
            //if (side == Side.Right)
            //{
            //    Array.Reverse(arrHeadX);
            //    Array.Reverse(arrHeadY);
            //    for (int i = 0; i <= arrHeadY.Length - 1; i++)
            //    {
            //        if (i > head_count * 2 && i < head_count * 2 + level_head_count)
            //        {
            //            headX1.Add(arrHeadX[i]);
            //            headY1.Add(arrHeadY[i]);
            //        }
            //        if (i >= head_count * 2 + level_head_count * 2)
            //        {
            //            headX2.Add(arrHeadX[i]);
            //            headY2.Add(arrHeadY[i]);
            //        }
            //    }
            //    var x2min = headX1[headY1.IndexOf(headY1.Min())];
            //    var y2min = headY1.Min();

            //    var x1min = headX2[headY2.IndexOf(headY2.Min())];
            //    var y1min = headY2.Min();

            //    var vremX1 = new List<double> { };
            //    var vremY1 = new List<double> { };
            //    var vremX2 = new List<double> { };
            //    var vremY2 = new List<double> { };

            //    //g
            //    for (int i = headY1.IndexOf(headY1.Min()); i < headY1.IndexOf(headY1.Min()) + 7; i++)
            //    {
            //        vremX1.Add(headX1[i]);
            //        vremY1.Add(headY1[i]);
            //    }
            //    //r
            //    for (int i = headY2.IndexOf(headY2.Min()); i < headY2.IndexOf(headY2.Min()) + 7; i++)
            //    {
            //        vremX2.Add(headX2[i]);
            //        vremY2.Add(headY2[i]);
            //    }

            //    //тобедегы 2 нукте
            //    Xtest3_r = vremX2.Average() * (-1) + Math.Max(arrHeadX.Max(), arrSideX.Max());
            //    Ytest3_r = vremY2.Average() * (-1) + Math.Max(arrHeadY.Max(), arrSideY.Max());
            //    Xtest4_r = vremX1.Average() * (-1) + Math.Max(arrHeadX.Max(), arrSideX.Max());
            //    Ytest4_r = vremY1.Average() * (-1) + Math.Max(arrHeadY.Max(), arrSideY.Max());
            //}

        }

        internal void CurrentProfileLeft()
        {
            BinaryReader reader = In_koridor;
            {
                try
                {
                    long ll = CurrentFrameIndex * ((long)In_koridor_size) + 8;
                    reader.BaseStream.Seek(ll, SeekOrigin.Begin);
                    var data = reader.ReadBytes(4);
                    Array.Reverse(data);
                    var U32EncoderCounter_1 = BitConverter.ToUInt32(data);
                    var Speed = reader.ReadInt32();
                    var TimeStamp = reader.ReadDouble();
                    var U32EncoderCounter_3 = reader.ReadInt32();
                    var U32CadrCouter = reader.ReadInt32();
                    var camtime = reader.ReadUInt64();
                    data = reader.ReadBytes(4);
                    Array.Reverse(data);
                    Kilometer = BitConverter.ToInt32(data);
                    data = reader.ReadBytes(4);
                    Array.Reverse(data);
                    Meter = BitConverter.ToInt32(data);
                    Picket = Meter / 100 + 1;

                    Console.WriteLine($"Профиль лв: КМ {Kilometer} М {Meter} encoderCounter {U32EncoderCounter_1} FrameNum {CurrentFrameIndex}" );

                    List<double> arrX = new List<double>();
                    List<double> arrY = new List<double>();
                    List<double> vector = new List<double>();
                    string viewBox = String.Empty;

                    for (int i = 0; i < ((int)In_koridor_size - 40) / 8; i++)
                    {
                        double x = reader.ReadSingle();
                        double y = reader.ReadSingle();
                        if (!Double.IsNaN(x) && !Double.IsNaN(y) && x < 250 && y < 135)
                        {
                            arrX.Add(x);
                            arrY.Add(y);
                        }
                    }
                    CalcProfile(Rails.r65, arrX, arrY, Side.Left, ref vector, ref viewBox);

                    //PointsRight = vector.ToArray();
                    //ViewBoxRight = viewBox;
                }
                catch (Exception e)
                {
                    System.Console.WriteLine(e.Message);
                    //PointsRight = Array.Empty<double>();
                    //ViewBoxRight = "0 0 0 0";
                }
            }
        }
        public void CurrentProfileRight()
        {
            BinaryReader reader = In_kupe;
            {
                try
                {
                    long ll = CurrentFrameIndex * ((long)In_kupe_size) + 8;
                    reader.BaseStream.Seek(ll, SeekOrigin.Begin);
                    var data = reader.ReadBytes(4);
                    Array.Reverse(data);
                    var U32EncoderCounter_1 = BitConverter.ToUInt32(data);
                    var Speed = reader.ReadInt32();
                    var TimeStamp = reader.ReadDouble();
                    var U32EncoderCounter_3 = reader.ReadInt32();
                    var U32CadrCouter = reader.ReadInt32();
                    var camtime = reader.ReadUInt64();
                    data = reader.ReadBytes(4);
                    Array.Reverse(data);
                    var kilometer = BitConverter.ToInt32(data);
                    data = reader.ReadBytes(4);
                    Array.Reverse(data);
                    var meter = BitConverter.ToInt32(data);

                    Console.WriteLine($"Профиль пр: КМ {Kilometer} М {Meter} encoderCounter {U32EncoderCounter_1} FrameNum {CurrentFrameIndex}");



                    List<double> arrX = new List<double>();
                    List<double> arrY = new List<double>();
                    List<double> vector = new List<double>();
                    string viewBox = String.Empty;

                    for (int i = 0; i < ((int)In_kupe_size - 40) / 8; i++)
                    {
                        double x = reader.ReadSingle();
                        double y = reader.ReadSingle();
                        if (!Double.IsNaN(x) && !Double.IsNaN(y) && x < 250 && y < 135)
                        {
                            arrX.Add(x);
                            arrY.Add(y);
                        }
                    }
                    CalcProfile(Rails.r65, arrX, arrY, Side.Right, ref vector, ref viewBox);

                    //PointsRight = vector.ToArray();
                    //ViewBoxRight = viewBox;
                }
                catch (Exception e)
                {
                    System.Console.WriteLine(e.Message);
                    //PointsRight = Array.Empty<double>();
                    //ViewBoxRight = "0 0 0 0";
                }
            }
        }
    }
}
