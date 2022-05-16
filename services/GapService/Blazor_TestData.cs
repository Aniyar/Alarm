using Accord.Math;
using ALARm.Core;
using Dapper;
using Npgsql;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

namespace GapService
{
    public class Blazor_TestData
    {
        static bool Check_short = false;

        static bool KM710 = false;

        static List<int> KMS = new List<int>();
        static List<int> METERS = new List<int>();
        static List<int> CANTIMETR = new List<int>();
        static List<int> METERS_long = new List<int>();
        static List<int> CANTIMETR_long = new List<int>();
        static List<int> METERS_long_M = new List<int>();


        static List<double> ral = new List<double> { };
        static List<double> rar = new List<double> { };

        static List<double> Vector_prev_l = new List<double>();
        static List<double> vector_prev_r = new List<double>();

        public string Diff_str_l;
        public string Diff_str_r;

        public string Diff_str_lwm;
        public string Diff_str_lwl;

        public string Diff_str_rw;

        public string Diff_str_l_g;
        public string Diff_str_r_g;

        public string Diff_str_l_i;
        public string Diff_str_r_i;

        public List<double> Diff_l { get; set; } = new List<double>();
        public List<double> Diff_r { get; set; } = new List<double>();
        public List<double> Diff_lw { get; set; } = new List<double>();
        public List<double> Diff_rw { get; set; } = new List<double>();

        public List<double> Diff_l_g { get; set; } = new List<double>();
        public List<double> Diff_r_g { get; set; } = new List<double>();

        public List<double> Diff_l_i { get; set; } = new List<double>();
        public List<double> Diff_r_i { get; set; } = new List<double>();

        public int CurrentFrameIndex { get; set; } = 0; //Километр: 301 Пикет: 1 Метр: 1 599086
        public int LocalFrameIndex { get; set; } = 0;
        public int Speed { get; set; } = 100;
        public bool Processing = true;
        public int Kilometer { get; set; } = -1;
        public int Meter { get; set; } = -1;
        public int Cantimetr { get; set; } = -1;

        public int Prevkilometr { get; set; } = -1;
        public int Picket { get; set; } = -1;
        public string DataLeft { get; set; }
        public string DataRight { get; set; }
        public string NominalRailScheme { get; set; }
        public string ViewBoxPoverhLeft { get; set; }
        public string ViewBoxPoverhRight { get; set; }

        public int ScaleCoef = 1000;
        public int WearCoef = 4;
        static List<double> ShortWavesLeft { get; set; } = new List<double>();
        static List<double> ShortWavesRight { get; set; } = new List<double>();
        static List<double> MediumWavesLeft { get; set; } = new List<double>();
        static List<double> MediumWavesRight { get; set; } = new List<double>();
        static List<double> LongWavesLeft { get; set; } = new List<double>();
        static List<double> LongWavesRight { get; set; } = new List<double>();
        /// <summary>
        /// для Profiledata_ средния значния на метр
        /// </summary>
        static List<double> LongWavesLeft_m { get; set; } = new List<double>();

        static List<double> ShortWavesLeft_m { get; set; } = new List<double>();
        static List<double> ShortWavesRight_m { get; set; } = new List<double>();
        static List<double> MediumWavesLeft_m { get; set; } = new List<double>();
        static List<double> MediumWavesRight_m { get; set; } = new List<double>();

        static List<double> LongWavesRight_m { get; set; } = new List<double>();
        /// <summary>
        /// /
        /// </summary>

        public List<List<double>> Vector_list_l { get; set; } = new List<List<double>>();
        public List<List<double>> Vector_list_r { get; set; } = new List<List<double>>();
        public int Vector_heigth = 5;
        public Bitmap FrameImgLeft { get; set; }
        public Bitmap FrameImgRight { get; set; }
        public double ExponentCoef = -1;
        public int width = 20;
        public int meter_count = 900;
        public string Poverh__profil__koridor = @"\\DESKTOP-EMAFC5J\o59m\PKR1-1_IP_113\2021_10\2021_10_18__16_43_58\242_ProfilPoverxKoridor_2021_10_18__16_43_58.s3_0033";
        public string Poverh__profil__kupe = @"\\DESKTOP-EMAFC5J\o59m\PKR1-2_IP_114\2021_10\2021_10_18__16_43_58\242_ProfilPoverxKupe_2021_10_18__16_43_58.s4_0033";
        public int Filenum = 0;
        public string[] files_KUpe = Directory.GetFiles(@"\\DESKTOP-EMAFC5J\o59m\PKR1-2_IP_114\2021_10\2021_10_18__16_43_58", "*.s4*");
        public string[] files_koridor = Directory.GetFiles(@"\\DESKTOP-EMAFC5J\o59m\PKR1-1_IP_113\2021_10\2021_10_18__16_43_58", "*.s3*");


        public double DegreeToRadian(double angle)
        {
            return Math.PI * angle / 180.0;
        }
        public double RadianToDegree(double radian)
        {
            return radian * 180.0 / Math.PI;
        }

        public bool CurrentPoverhProfLeft(string filePath)
        {
            using (BinaryReader reader = new BinaryReader(File.Open(filePath, FileMode.Open)))
            {
                try
                {
                    int width = reader.ReadInt32();
                    int height = reader.ReadInt32();

                    int frameSize = width * height;

                    long position = CurrentFrameIndex * (long)frameSize + 8;
                    if (reader.BaseStream.Length < position)
                    {
                        //CurrentFrameIndex += LocalFrameIndex;
                        CurrentFrameIndex = 0;
                        return false;
                    }
                    reader.BaseStream.Seek(position, SeekOrigin.Begin);
                    byte[] by = reader.ReadBytes(frameSize);


                    var framerbegin = BitConverter.ToInt64(by.Take(8).ToArray(), 0);
                    int encoderCounter_1 = BitConverter.ToInt32(by.Skip(8).Take(4).ToArray(), 0);
                    int speed = BitConverter.ToInt32(by.Skip(12).Take(4).ToArray(), 0);
                    var nanoseconds = BitConverter.ToInt64(by.Skip(16).Take(8).ToArray(), 0);
                    int encoderCounter_3 = BitConverter.ToInt32(by.Skip(24).Take(4).ToArray(), 0);
                    int u32CardCounter = BitConverter.ToInt32(by.Skip(28).Take(4).ToArray(), 0);
                    var cameratime = BitConverter.ToInt64(by.Skip(32).Take(8).ToArray(), 0);
                    int km = BitConverter.ToInt32(by.Skip(40).Take(4).ToArray(), 0);
                    int meter = BitConverter.ToInt32(by.Skip(44).Take(4).ToArray(), 0);


                    Kilometer = km;


                    Picket = meter / 100 + 1;
                    Meter = meter;

                    Cantimetr = encoderCounter_1;

                    if (Meter % 100 == 0)
                    {
                        Console.WriteLine($"{Kilometer} - {Meter}");
                    }
                    


                    var result = ConvertMatrix(Array.ConvertAll(by, Convert.ToInt32), height, width);

                    List<double> y1 = new List<double>();
                    List<double> y2 = new List<double>();
                    List<double> y1_1 = new List<double>();
                    List<double> y2_1 = new List<double>();
                    List<double> y1_2 = new List<double>();
                    List<double> y2_2 = new List<double>();

                    for (int i = 0; i <= height - 1; i++)
                    {
                        if (i > height / 2)
                        {
                            y1_1.Add(i * Math.Exp(2 * result[i, 300]));
                            y1_2.Add(Math.Exp(2 * result[i, 300]));

                        }
                        if (i < height / 2)
                        {
                            y2_1.Add(i * Math.Exp(2 * result[i, 350]));
                            y2_2.Add(Math.Exp(2 * result[i, 350]));
                        }
                    }

                    var y1Sum = y2_1.Sum();
                    var y12Sum = y2_2.Sum();
                    var y1_aver = y1_1.Sum() / y1_2.Sum();
                    var y2_aver = y2_1.Sum() / y2_2.Sum();

                    var y1_m = y1_1.IndexOf(y1_1.Max());
                    var y2_m = y2_1.IndexOf(y2_1.Max());

                    var grad = (180 / Math.PI) * Math.Atan((y1_aver - y2_aver) / 150);

                    var frame = result.ToBitmap();
                    frame = RotateImage(frame, (float)(grad / 45.0 + 6.5));

                    width = frame.Width - 280;
                    height = frame.Height;

                    frame = frame.Clone(new Rectangle(130, 0, width, height), frame.PixelFormat);


                    var mtx = frame.ToRedMatrix();

                    List<double> width_y = new List<double>();
                    for (int j = 0; j < height; j++)
                    {
                        List<double> rol_aver = new List<double>();
                        for (int i = 0; i < width; i++)
                        {
                            rol_aver.Add(Math.Exp(2 * mtx[j, i]));
                        }
                        width_y.Add(rol_aver.Average());
                    }
                    var maxIndex = width_y.IndexOf(width_y.Max());

                    if (maxIndex > 20)
                    {
                        frame = frame.Clone(new Rectangle(0, maxIndex - 20, frame.Width, 30), frame.PixelFormat);
                    }


                    mtx = frame.ToRedMatrix();

                    var param1 = 0;
                    var param2 = 0;

                    double[] graphic = new double[width - (param1 + param2)];
                    try
                    {
                        for (int x = param1; x < frame.Width - param2; x++)
                        {
                            var s1 = 0.0;
                            var s2 = 0.0;
                            for (int y = 0; y < frame.Height; y++) //heigth
                            {
                                s1 += Math.Exp(2 * mtx[y, x]);
                                s2 += y * Math.Exp(2 * mtx[y, x]);
                            }
                            graphic[x - param1] = (s2 / s1);
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Left exponent error");
                    }

                    DataLeft = VectorToPoints(graphic, Threat.Left);
                    FrameImgLeft = frame;
                    DataLeft = "";

                }
                catch (Exception e)
                {
                    System.Console.WriteLine(e.Message);
                    FrameImgLeft = null;
                }
                long totalMemory = GC.GetTotalMemory(false);

                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
            return true;


        }
        public void CurrentPoverhProfRight(string filePath)
        {
            using (BinaryReader reader = new BinaryReader(File.Open(filePath, FileMode.Open)))
            {
                try
                {
                    int width = reader.ReadInt32();
                    int height = reader.ReadInt32();

                    int frameSize = width * height;

                    long position = CurrentFrameIndex * (long)frameSize + 8;

                    reader.BaseStream.Seek(position, SeekOrigin.Begin);
                    byte[] by = reader.ReadBytes(frameSize);


                    var framerbegin = BitConverter.ToInt64(by.Take(8).ToArray());
                    int encoderCounter_1 = BitConverter.ToInt32(by.Skip(8).Take(4).ToArray());
                    int speed = BitConverter.ToInt32(by.Skip(12).Take(4).ToArray());
                    var nanoseconds = BitConverter.ToInt64(by.Skip(16).Take(8).ToArray());
                    int encoderCounter_3 = BitConverter.ToInt32(by.Skip(24).Take(4).ToArray());
                    int u32CardCounter = BitConverter.ToInt32(by.Skip(28).Take(4).ToArray());
                    var cameratime = BitConverter.ToInt64(by.Skip(32).Take(8).ToArray());
                    int km = BitConverter.ToInt32(by.Skip(40).Take(4).ToArray());
                    int meter = BitConverter.ToInt32(by.Skip(44).Take(4).ToArray());
                    //Kilometer = km;
                    //Picket = by[22] * 256 + by[23];
                    //Meter = meter;

                    var result = ConvertMatrix(Array.ConvertAll(by, Convert.ToInt32), height, width);

                    List<double> y1 = new List<double>();
                    List<double> y2 = new List<double>();
                    List<double> y1_1 = new List<double>();
                    List<double> y2_1 = new List<double>();
                    List<double> y1_2 = new List<double>();
                    List<double> y2_2 = new List<double>();

                    for (int i = 0; i <= height - 1; i++)
                    {
                        if (i > height / 2)
                        {
                            y1_1.Add(i * Math.Exp(2 * result[i, 300]));
                            y1_2.Add(Math.Exp(2 * result[i, 300]));

                        }
                        if (i < height / 2)
                        {
                            y2_1.Add(i * Math.Exp(2 * result[i, 350]));
                            y2_2.Add(Math.Exp(2 * result[i, 350]));
                        }
                    }

                    var y1Sum = y2_1.Sum();
                    var y12Sum = y2_2.Sum();
                    var y1_aver = y1_1.Sum() / y1_2.Sum();
                    var y2_aver = y2_1.Sum() / y2_2.Sum();

                    var y1_m = y1_1.IndexOf(y1_1.Max());
                    var y2_m = y2_1.IndexOf(y2_1.Max());

                    var grad = (180 / Math.PI) * Math.Atan((y1_aver - y2_aver) / 150);

                    var frame = result.ToBitmap();
                    frame = RotateImage(frame, (float)(grad / 45.0) * (-1) - 1.5f);

                    width = frame.Width;
                    height = frame.Height;

                    frame = frame.Clone(new Rectangle(300, 0, frame.Width - 450, frame.Height), frame.PixelFormat);

                    width = width - 450;
                    height = height;

                    var mtx = frame.ToRedMatrix();

                    List<double> width_y = new List<double>();
                    for (int j = 0; j < height; j++)
                    {
                        List<double> rol_aver = new List<double>();
                        for (int i = 0; i < width; i++)
                        {
                            rol_aver.Add(Math.Exp(2 * mtx[j, i]));
                        }
                        width_y.Add(rol_aver.Average());
                    }
                    var maxIndex = width_y.IndexOf(width_y.Max());
                    if (maxIndex > 20)
                    {
                        frame = frame.Clone(new Rectangle(0, maxIndex - 20, frame.Width, 30), frame.PixelFormat);
                    }


                    mtx = frame.ToRedMatrix();

                    var param1 = 0;
                    var param2 = 0;

                    double[] graphic = new double[width - (param1 + param2)];
                    try
                    {
                        for (int x = param1; x < frame.Width - param2; x++)
                        {
                            var s1 = 0.0;
                            var s2 = 0.0;
                            for (int y = 0; y < frame.Height; y++) //heigth
                            {
                                s1 += Math.Exp(2 * mtx[y, x]);
                                s2 += y * Math.Exp(2 * mtx[y, x]);
                            }
                            graphic[x - param1] = (s2 / s1);
                        }
                    }
                    catch
                    {
                        Console.WriteLine("Left exponent error");
                    }

                    DataRight = VectorToPoints(graphic, Threat.Right);

                    FrameImgRight = frame;

                    KMS.Add(Kilometer);
                    METERS.Add(Meter);
                    CANTIMETR.Add(Cantimetr);

                }
                catch (Exception e)
                {
                    System.Console.WriteLine(e.Message);
                    FrameImgRight = null;
                }
            }
        }

        private void SendShortDataDB()
        {
            //if (900 < Meter) continue;

            var trip_id = 242;

            var con = new NpgsqlConnection(ALARm.DataAccess.Helper.ConnectionString());
            con.Open();
            var cmd = new NpgsqlCommand();
            cmd.Connection = con;

            for (int j = 0; j < Diff_lw.Count; j++)
            {
                try
                {
                    var qrStr = $@"INSERT INTO testdata_{trip_id} (km, meter, cantimetr, diff_l, diff_r) 
                                                       VALUES({KMS[j]}, {METERS[j]}, {CANTIMETR[j]},{(Diff_lw[j]).ToString().Replace(",", ".")}, {(Diff_rw[j]).ToString().Replace(",", ".")} )";
                    cmd.CommandText = qrStr;
                    cmd.ExecuteNonQuery();
                }

                catch (Exception e)
                {
                    Console.WriteLine("Ошибка записи в БД " + e.Message);
                }

            }
            con.Close();

            if (Kilometer != Prevkilometr && Prevkilometr != -1)
            {
                GetTestData((int)Prevkilometr);


            }
            if (Kilometer != Prevkilometr)
            {
                Prevkilometr = Kilometer; ;
            }
            KMS.Clear(); METERS.Clear(); Diff_lw.Clear(); Diff_rw.Clear(); CANTIMETR.Clear();
        }

        private void SendShortDataDBMEDIUMWAYES()
        {
            var trip_id = 242;


            var con = new NpgsqlConnection(Helper.ConnectionString());
            con.Open();

            var cmd = new NpgsqlCommand();
            cmd.Connection = con;
            for (int i = 0; i < ShortWavesRight.Count; i++)
            {
                try
                {
                    var qrStr = $@"UPDATE  testdata_{trip_id}
                                   SET  longwavesleft = {(LongWavesLeft[i]).ToString().Replace(",", ".")},
                                   mediumwavesleft =  {(MediumWavesLeft[i]).ToString().Replace(",", ".")},shortwavesleft = {(ShortWavesLeft[i]).ToString().Replace(",", ".")},
                                   longwavesright =   {(LongWavesRight[i]).ToString().Replace(",", ".")},mediumwavesright =  {(MediumWavesRight[i]).ToString().Replace(",", ".")}
                                  ,shortwavesright = {(ShortWavesRight[i]).ToString().Replace(",", ".")}
                                   where km = {Prevkilometr} and meter= {METERS_long[i]} and cantimetr = {CANTIMETR_long[i]}";

                    cmd.CommandText = qrStr;
                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    Console.WriteLine("Ошибка записи в БД " + e.Message);
                }
            }
            con.Close();

            LongWavesLeft.Clear(); MediumWavesLeft.Clear(); ShortWavesLeft.Clear(); LongWavesRight.Clear();
            MediumWavesRight.Clear();
            ShortWavesRight.Clear();
            METERS.Clear(); CANTIMETR.Clear();
        }

        private void SendShortDataDBMEDIUMWAYES_M()
        {
            var trip_id = 242;


            var con = new NpgsqlConnection(Helper.ConnectionString());
            con.Open();

            var cmd = new NpgsqlCommand();
            cmd.Connection = con;

            for (int i = 0; i < ShortWavesRight_m.Count; i++)
            {
                try
                {
                    var qrStr = $@"UPDATE  testdata_{trip_id}
                                   SET  longwavesleft = {(LongWavesLeft_m[i]).ToString("0.00000").Replace(",", ".")},
                                   mediumwavesleft =  {(MediumWavesLeft_m[i]).ToString("0.00000").Replace(",", ".")},shortwavesleft = {(ShortWavesLeft_m[i]).ToString("0.00000").Replace(",", ".")},
                                   longwavesright =   {(LongWavesRight_m[i]).ToString("0.00000").Replace(",", ".")},mediumwavesright =  {(MediumWavesRight_m[i]).ToString("0.00000").Replace(",", ".")}
                                  ,shortwavesright = {(ShortWavesRight_m[i]).ToString("0.00000").Replace(",", ".")}
                                   where km = {Prevkilometr} and meter= {METERS_long_M[i]} ";

                    cmd.CommandText = qrStr;
                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    Console.WriteLine("Ошибка записи в БД " + e.Message);
                }
            }

            con.Close();

            LongWavesLeft_m.Clear(); MediumWavesLeft_m.Clear(); ShortWavesLeft_m.Clear(); LongWavesRight_m.Clear();
            MediumWavesRight_m.Clear();
            ShortWavesRight_m.Clear();
            METERS_long_M.Clear();
        }


        private Bitmap RotateImage(Bitmap bmp, float angle)
        {
            Bitmap rotatedImage = new Bitmap(bmp.Width, bmp.Height);
            rotatedImage.SetResolution(bmp.HorizontalResolution, bmp.VerticalResolution);

            using (Graphics g = Graphics.FromImage(rotatedImage))
            {
                // Set the rotation point to the center in the matrix
                g.TranslateTransform(bmp.Width / 2, bmp.Height / 2);
                // Rotate
                g.RotateTransform(angle);
                // Restore rotation point in the matrix
                g.TranslateTransform(-bmp.Width / 2, -bmp.Height / 2);
                // Draw the image on the bitmap
                g.DrawImage(bmp, new Point(0, 0));
            }

            return rotatedImage;
        }


        public bool GetBitmapAsync(string file_kor, string file_kupe)
        {
            {
                GC.Collect();

                try
                {
                    if (Check_short == false)
                    {

                        if (!CurrentPoverhProfLeft(file_kor))
                        {
                            return false;
                        }
                        CurrentPoverhProfRight(file_kupe);
                        SendShortDataDB();
                        if (Meter == 981)
                        {
                            var a = 0;
                        }
                    }

                    return true;
                }
                catch (Exception e)
                {
                    CurrentFrameIndex = -1;
                    Processing = false;
                    return false;
                }
            }
        }

        private void GetTestData(int km)
        {
            LongWavesLeft.Reverse();
            LongWavesRight.Reverse();
            MediumWavesLeft.Reverse();
            MediumWavesRight.Reverse();
            ShortWavesLeft.Reverse();
            ShortWavesRight.Reverse();

            var connection = new Npgsql.NpgsqlConnection(Helper.ConnectionString());
            var ShortData = connection.Query<DataFlow>($@"SELECT * FROM testdata_242 where km = {km}  ").ToList();

            // var shortl = ShortData.Select(o => o.Diff_l / 8.0 < 1 / 8.0 ? 0 : o.Diff_l / 8.0).ToList();
            //var shortr = ShortData.Select(o => o.Diff_r / 8.0 < 1 / 8.0 ? 0 : o.Diff_r / 8.0).ToList();

            var shortl = ShortData.Select(o => o.Diff_l / 8.0 < 1 / 8.0 ? 0 : o.Diff_l / 8.0).ToList();
            var shortr = ShortData.Select(o => o.Diff_r / 8.0 < 1 / 8.0 ? 0 : o.Diff_r / 8.0).ToList();


            var short_canti = ShortData.Select(o => o.cantimetr).ToList();
            var short_meter = ShortData.Select(o => o.Meter).ToList();
            var val = new List<double> { };
            var val_ind = new List<int> { };
            var bolshe0 = new List<DATA0> { };
            var inn = false;

            //left
            for (int i = 0; i < shortl.Count - 1; i++)

            {
                var temp = shortl[i];
                var next_temp = shortl[i + 1];

                if (!inn && 0 < next_temp)
                {
                    val.Add(temp);
                    val_ind.Add(i);

                    val.Add(next_temp);
                    val_ind.Add(i + 1);

                    inn = true;
                }
                else if (inn && 0 < next_temp)
                {
                    val.Add(next_temp);
                    val_ind.Add(i + 1);

                }
                else if (inn && 0 == next_temp)
                {
                    if (val.Any())
                    {
                        val.Add(next_temp);
                        val_ind.Add(i + 1);

                        var d = new DATA0
                        {
                            Count = val.Count,
                            H = val.Max() * 0.8,
                            H_ind = val_ind[val.IndexOf(val.Max())],
                        };

                        bolshe0.Add(d);

                        inn = false;
                        val.Clear();
                        val_ind.Clear();
                    }
                }
            }


            var val_r = new List<double> { };
            var val_ind_r = new List<int> { };
            var bolshe0_r = new List<DATA0> { };
            var inn_r = false;

            //right
            for (int i = 0; i < shortr.Count - 1; i++)
            {
                var temp = shortr[i];
                var next_temp = shortr[i + 1];

                if (!inn_r && 0 < next_temp)
                {
                    val_r.Add(temp);
                    val_ind_r.Add(i);

                    val_r.Add(next_temp);
                    val_ind_r.Add(i + 1);

                    inn_r = true;
                }
                else if (inn_r && 0 < next_temp)
                {
                    val_r.Add(next_temp);
                    val_ind_r.Add(i + 1);

                }
                else if (inn_r && 0 == next_temp)
                {
                    if (val_r.Any())
                    {
                        val_r.Add(next_temp);
                        val_ind_r.Add(i + 1);

                        var d = new DATA0
                        {
                            Count = val_r.Count,
                            H = val_r.Max() * 0.8,
                            H_ind = val_ind_r[val_r.IndexOf(val_r.Max())],
                        };

                        bolshe0_r.Add(d);

                        inn_r = false;
                        val_r.Clear();
                        val_ind_r.Clear();
                    }
                }
            }

            for (int j = 0; j < shortl.Count; j++)
            {
                var m = 0.0;
                var l = 0.0;
                var s = 0.0;

                var mr = 0.0;
                var lr = 0.0;
                var sr = 0.0;

                for (int i = 0; i < bolshe0.Count; i++)
                {
                    l += bolshe0[i].H * Math.Exp(-0.003 * Math.Pow(bolshe0[i].H_ind - j, 2) / bolshe0[i].Count);
                    m += bolshe0[i].H * Math.Exp(-0.02 * Math.Pow(bolshe0[i].H_ind - j, 2) / bolshe0[i].Count);
                    s += bolshe0[i].H * Math.Exp(-0.3 * Math.Pow(bolshe0[i].H_ind - j, 2) / bolshe0[i].Count);
                }

                for (int i = 0; i < bolshe0_r.Count; i++)
                {
                    lr += bolshe0_r[i].H * Math.Exp(-0.003 * Math.Pow(bolshe0_r[i].H_ind - j, 2) / bolshe0_r[i].Count);
                    mr += bolshe0_r[i].H * Math.Exp(-0.02 * Math.Pow(bolshe0_r[i].H_ind - j, 2) / bolshe0_r[i].Count);
                    sr += bolshe0_r[i].H * Math.Exp(-0.3 * Math.Pow(bolshe0_r[i].H_ind - j, 2) / bolshe0_r[i].Count);


                }

                LongWavesLeft.Add(l * 0.1);
                MediumWavesLeft.Add(m * 0.1);
                ShortWavesLeft.Add(s * 0.1);

                LongWavesRight.Add(lr * 0.1);
                MediumWavesRight.Add(mr * 0.1);
                ShortWavesRight.Add(sr * 0.1);

                METERS_long.Add(short_meter[j]);
                CANTIMETR_long.Add(short_canti[j]);
                if (j / 5 == j / 5.0)
                {
                    LongWavesLeft_m.Add(l * 0.1);
                    MediumWavesLeft_m.Add(m * 0.1);
                    ShortWavesLeft_m.Add(s * 0.1);

                    LongWavesRight_m.Add(lr * 0.1);
                    MediumWavesRight_m.Add(mr * 0.1);
                    ShortWavesRight_m.Add(sr * 0.1);

                    METERS_long_M.Add(short_meter[j]);

                }



            }
            //SendShortDataDBMEDIUMWAYES();
            //SendShortDataDBMEDIUMWAYES_M();

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
        public string VectorToPoints(double[] vector, Threat Threat)
        {
            var diff1 = new List<double> { };


            if (Threat == Threat.Left)
            {
                if (Vector_list_l.Count() >= Vector_heigth)
                {
                    var etalon = new List<double> { };
                    Vector_list_l = Vector_list_l.GetRange(Vector_list_l.Count() - Vector_heigth, Vector_heigth).ToList();

                    for (int i = 0; i < vector.Length; i++)
                    {
                        var avg_vec = Vector_list_l.Select(obj => obj[i]).ToList();
                        etalon.Add(avg_vec.Average());
                    }

                    Vector_list_l.Add(vector.ToList());

                    //разница
                    for (int i = 0; i < vector.Length; i++)
                    {
                        vector[i] = Vector_list_l[2][i] - etalon[i];
                    }

                    //скольз среднее
                    var avg_vec1 = new List<double> { };
                    for (int i = 1; i < vector.Count(); i++)
                    {
                        var temp1 = vector.ToList().GetRange(i - 1, 1).ToList();
                        avg_vec1.Add(temp1.Average());
                    }
                    vector = avg_vec1.ToArray();
                }
                else
                {
                    Vector_list_l.Add(vector.ToList());
                }

                //------------------------------------------------
                if (Vector_prev_l.Count == 0)
                {
                    Vector_prev_l.AddRange(vector.ToList());
                }

                var newV = new double[vector.Length];
                for (int i = 0; i < vector.Length; i++)
                {
                    newV[i] = Math.Abs(vector[i] - Vector_prev_l[i]);
                }
                //------------------------------------------------


                //Короткие неровности
                var diff = newV.Average();

                Vector_prev_l.Clear();
                Vector_prev_l.AddRange(vector.ToList());


                Diff_l.Add(diff);
                Diff_lw.Add(diff);
                Diff_str_l = Diff_str_l + $"{ (diff).ToString("0.00").Replace(",", ".")},{CurrentFrameIndex} ";

                if (MediumWavesLeft.Count > CurrentFrameIndex)
                {
                    Diff_str_lwm = Diff_str_lwm + $"{ (MediumWavesLeft[CurrentFrameIndex]).ToString("0.00").Replace(",", ".")},{CurrentFrameIndex} ";
                    Diff_str_lwl = Diff_str_lwl + $"{ (LongWavesLeft[CurrentFrameIndex]).ToString("0.00").Replace(",", ".")},{CurrentFrameIndex} ";
                }

                //Импульсы
                if (diff >= 1)
                {
                    Diff_l_i.Add(diff);
                    Diff_str_l_i = Diff_str_l_i + $"{ (diff).ToString("0.00").Replace(",", ".")},{CurrentFrameIndex} ";
                }
                else
                {
                    Diff_l_i.Add(0);
                    Diff_str_l_i = Diff_str_l_i + $"{ (0).ToString("0.00").Replace(",", ".")},{CurrentFrameIndex} ";
                }

                //Стыки
                var diffgap = vector.Max() - vector.Min();
                Diff_l_g.Add(diffgap);
                Diff_str_l_g = Diff_str_l_g + $"{ (diffgap * WearCoef * 3).ToString("0.00").Replace(",", ".")},{CurrentFrameIndex} ";
            }

            if (Threat == Threat.Right)
            {
                if (Vector_list_r.Count() >= Vector_heigth)
                {
                    var etalon = new List<double> { };
                    Vector_list_r = Vector_list_r.GetRange(Vector_list_r.Count() - Vector_heigth, Vector_heigth).ToList();

                    for (int i = 0; i < vector.Length; i++)
                    {
                        var avg_vec = Vector_list_r.Select(obj => obj[i]).ToList();
                        etalon.Add(avg_vec.Average());
                    }

                    Vector_list_r.Add(vector.ToList());

                    //разница
                    for (int i = 0; i < vector.Length; i++)
                    {
                        vector[i] = Vector_list_l[2][i] - etalon[i];
                    }

                    //скольз среднее
                    var avg_vec1 = new List<double> { };
                    for (int i = 1; i < vector.Count(); i++)
                    {
                        var temp1 = vector.ToList().GetRange(i - 1, 1).ToList();
                        avg_vec1.Add(temp1.Average());
                    }
                    vector = avg_vec1.ToArray();
                }
                else
                {
                    Vector_list_r.Add(vector.ToList());
                }

                //------------------------------------------------
                if (vector_prev_r.Count == 0)
                {
                    vector_prev_r.AddRange(vector.ToList());
                }

                var newV = new double[vector.Length];
                for (int i = 0; i < vector.Length; i++)
                {
                    newV[i] = Math.Abs(vector[i] - vector_prev_r[i]);
                }
                //------------------------------------------------

                //Короткие неровности
                var diff = newV.Average();
                vector_prev_r.Clear();
                vector_prev_r.AddRange(vector.ToList());

                Diff_r.Add(diff);
                Diff_rw.Add(diff);
                Diff_str_r = Diff_str_r + $"{ (diff).ToString("0.00").Replace(",", ".")},{CurrentFrameIndex} ";


                //Импульсы
                if (diff >= 2)
                {
                    Diff_r_i.Add(diff);
                    Diff_str_r_i = Diff_str_r_i + $"{ (diff).ToString("0.00").Replace(",", ".")},{CurrentFrameIndex} ";
                }
                else
                {
                    Diff_r_i.Add(0);
                    Diff_str_r_i = Diff_str_r_i + $"{ (0).ToString("0.00").Replace(",", ".")},{CurrentFrameIndex} ";
                }

                //Стыки
                var diffgap = vector.Max() - vector.Min();
                Diff_r_g.Add(diffgap);
                Diff_str_r_g = Diff_str_r_g + $"{ (diffgap * WearCoef * 3).ToString("0.00").Replace(",", ".")},{CurrentFrameIndex} ";
            }


            StringBuilder sb = new StringBuilder();



            for (int i = 0; i < vector.Length; i++)
            {
                var st = $"{vector[i] - 20}".Replace(",", ".");
                sb.Append($"{i},{st} ");
            }

            var minc = $"{vector.Min() - 50}".Replace(",", ".");
            var maxc = $"{vector.Max() + 50}".Replace(",", ".");
            //ViewBox = $"0 {minc} {vector.Length} {maxc}";

            if (Threat == Threat.Left)
            {
                ViewBoxPoverhLeft = $"0 {minc} {vector.Length} {maxc}";
            }
            else
            {
                ViewBoxPoverhRight = $"0 {minc} {vector.Length} {maxc}";
            }
            return sb.ToString();
        }

        private void SendPoverhDataDB(string threat_id, int km, int m, double intensity_ra, double intensity_formula, double diff, double len)
        {
            var trip_id = "242";
            threat_id = threat_id == "Left" ? "1" : "2";

            var con = new NpgsqlConnection(Helper.ConnectionString());
            con.Open();

            var cmd = new NpgsqlCommand();
            cmd.Connection = con;
            cmd.CommandText = "DROP TABLE IF EXISTS impulses_" + (trip_id == "-1" ? "0" : trip_id);
            cmd.ExecuteNonQuery();

            cmd.CommandText = "CREATE TABLE impulses_" + (trip_id == "-1" ? "0" : trip_id) + @"(id serial, 
                                                                                                    km smallint, 
                                                                                                    meter smallint, 
                                                                                                    intensity_ra numeric, 
                                                                                                    intensity_formula numeric, 
                                                                                                    diff numeric, 
                                                                                                    len numeric, 
                                                                                                    threat_id smallint)";
            cmd.ExecuteNonQuery();

            try
            {
                var qrStr = "INSERT INTO impulses_" + (trip_id == "-1" ? "0" : trip_id) + @"(km, 
                                                                                            meter, 
                                                                                            intensity_ra, 
                                                                                            intensity_formula, 
                                                                                            diff, 
                                                                                            len, 
                                                                                            threat_id) VALUES(" +
                                                                                            (km - 301).ToString() + "," +
                                                                                            m.ToString() + "," +
                                                                                            intensity_ra.ToString().Replace(",", ".") + "," +
                                                                                            intensity_formula.ToString().Replace(",", ".") + "," +
                                                                                            diff.ToString().Replace(",", ".") + "," +
                                                                                            len.ToString().Replace(",", ".") + "," +
                                                                                            threat_id.ToString() + ")";
                cmd.CommandText = qrStr;
                cmd.ExecuteNonQuery();
            }
            catch (Exception e)
            {
                Console.WriteLine("Ошибка записи в БД " + e.Message);
            }
            con.Close();
        }

     
    }

    //public enum Rails { r75 = 192, r65 = 180, r50 = 152, r43 = 140 }
    //public enum Side { Left = -1, Right = 1 }
    //public enum Threat { Left = 1, Right = 2 }

}
