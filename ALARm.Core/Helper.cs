using ALARm.Core.Report;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using RabbitMQ.Client;

namespace ALARm.Core
{
    public static class Helper
    {
        /// <summary>
        /// Пороговые величины поперечного непогашенного ускорения и ψ
        /// </summary>
        /// 


        public static ThresholdsAcceleration GetAnp(VagonType vagonType, int curveRadius = -1)
        {
            ThresholdsAcceleration temp = new ThresholdsAcceleration { };

            switch (vagonType)
            {
                case VagonType.Sapsan:
                    if (curveRadius < 1600)
                    {
                        temp.AnpBliskoeKDopusku = 0.67;
                        temp.AnpOgrSkor = 0.7;
                        temp.AgOgrSkor = 0.9;
                    }
                    else if (curveRadius >= 1600 && curveRadius <= 3000)
                    {
                        temp.AnpBliskoeKDopusku = 0.77;
                        temp.AnpOgrSkor = 0.8;
                        temp.AgOgrSkor = 1;
                    }
                    else if (curveRadius > 3000)
                    {
                        temp.AnpBliskoeKDopusku = 0.87;
                        temp.AnpOgrSkor = 0.9;
                        temp.AgOgrSkor = 1.1;
                    }
                    break;

                case VagonType.Lastochka:
                    if (curveRadius <= 400)
                    {
                        temp.AnpBliskoeKDopusku = 0.67;
                        temp.AnpOgrSkor = 0.7;
                        temp.AgOgrSkor = 0.9;
                    }
                    else if (curveRadius > 400)
                    {
                        temp.AnpBliskoeKDopusku = 0.87;
                        temp.AnpOgrSkor = 0.9;
                        temp.AgOgrSkor = 1.1;
                    }
                    break;

                case VagonType.Strizh:
                    temp.AnpBliskoeKDopusku = 1.1;
                    temp.AnpOgrSkor = 1.2;
                    temp.AgOgrSkor = 1.4;
                    break;

                case VagonType.Allegro:
                    temp.AnpBliskoeKDopusku = 0.95;
                    temp.AnpOgrSkor = 1;
                    temp.AgOgrSkor = 1.2;
                    break;

                case VagonType.PassengerAndFreight:
                    temp.AnpBliskoeKDopusku = 0.65;
                    temp.AnpOgrSkor = 0.7;
                    temp.AgOgrSkor = 0.9;
                    break;

                default:
                    break;
            }

            return temp;
        }

        public static SocketState SendMessageFromRabbitMQ(string Host, long TripId, int Msg)
        {
            try
            {
                var factory = new ConnectionFactory() { HostName = "mycomputer", UserName = "beebo", Password = "beebo" };
                using (var connection = factory.CreateConnection())
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: Host,
                                         durable: false,
                                         exclusive: false,
                                         autoDelete: false,
                                         arguments: null);
                    var json = "{'Host':'" + Host + "', 'TripId':'" + TripId + "', 'Msg':" + Msg + "}";
                    var body = Encoding.UTF8.GetBytes(json);

                    channel.BasicPublish(exchange: "",
                                         routingKey: Host,
                                         basicProperties: null,
                                         body: body);
                    Console.WriteLine(" [x] Sent {0}", json);

                }
                return SocketState.Success;
            }
            catch (Exception e)
            {
                return SocketState.Abortively;
            }
        }

        public static SocketState SendMessageToRabbitMQExchange(IConnection connection, string exchange, String message)
        {
            try
            {


                using (var channel = connection.CreateModel())
                {

                    //ToDo queue жоқ болып құрылса extension қосу керек


                    var body = Encoding.UTF8.GetBytes(message);



                    channel.BasicPublish(exchange: exchange,
                                         routingKey: "",
                                         basicProperties: null,
                                         body: body);
                    Console.WriteLine(" [x] Sent {0}", message);

                }
                return SocketState.Success;
            }
            catch (Exception e)
            {
                return SocketState.Abortively;
            }
        }
        public static string GetShortForm(string name)
        {
            if (name == null)
                return "";
            name = name.ToUpper();
            string vowelsList = "АЕЁИОУЫЭЮЯ";
            string result = name.Length > 4 ? name.Substring(0, 4) : name;
            if ((result[3] == 'Ь') && (name.Length > 4))
                result += name[4];
            if (vowelsList.Contains(result[3]))
                result = result.Substring(0, 3);

            return result;
        }

        public static string GetShortFormInNormalString(string name)
        {
            name = GetShortForm(name);
            if (name.Length > 1)
                name = name[0] + name.Substring(1, name.Length - 1).ToLower();
            return name;
        }

        public static string GetResourceName(string name)
        {
            return Properties.Resources.ResourceManager.GetString(name.ToString());
        }
        public static int RoundTo10(this int digit)
        {
            int mod = digit % 10;
            return digit - mod;

        }
        public static int GetNextTop(this List<int> tops, int start, int top, int number)
        {

            for (int i = top; i < number * 100; i += 10)
            {
                if (!tops.Contains(i))
                    return i;
            }
            for (int i = top; i > start.RoundTo10(); i -= 10)
            {
                if (!tops.Contains(i))
                    return i;
            }
            return 10;
        }
        public static int GetNextTopFromBottom(this List<int> tops, int start)
        {

            for (int i = start.RoundTo10() + 10; i <= (start + 1) * 100; i += 10)
            {
                if (!tops.Contains(i))
                    return i;
            }
            return 10;

        }
        public static Picket GetPicket(this List<Picket> pickets, int meter)
        {
            Picket result = null;
            foreach (var picket in pickets)
            {
                if ((meter / 100 + 1) == picket.Number)
                    result = picket;
            }
            if (result == null)
            {
                try
                {
                    if (pickets.Count > 0)
                    {
                        result = new Picket() { Number = pickets[pickets.Count - 1].Number + 1, Start = pickets[pickets.Count - 1].Number * 100 };
                    }
                    else
                    {
                        result = new Picket() { Number = meter / 100 + 1, Start = (meter / 100) * 100 };

                    }
                    pickets.Add(result);
                    return result;
                }
                catch (Exception e)
                {
                    Console.WriteLine("GetPicket" + e.Message);
                }
            }
            if (result == null)
                return result;
            if (!result.IsFilled)
                return result;

            int notFillIndex = -1;
            for (int i = pickets.IndexOf(result) - 1; i >= 0; i--)
            {
                if (!pickets[i].IsFilled)
                {
                    notFillIndex = i;
                    break;
                }
            }
            if (notFillIndex != -1)
            {
                for (int i = notFillIndex; i < pickets.IndexOf(result); i++)
                {
                    var first = pickets[i + 1].Digression.GetFirstMove();
                    if (first != null)
                    {
                        pickets[i].Digression.Add(first);
                        pickets[i + 1].Digression.Remove(first);
                    }

                }
                return result;
            }
            notFillIndex = -1;
            for (int i = pickets.IndexOf(result); i < pickets.Count; i++)
            {
                if (!pickets[i].IsFilled)
                {
                    notFillIndex = i;
                    break;
                }
            }
            if (notFillIndex != -1)
            {
                return pickets[notFillIndex];
            }
            result = new Picket() { Number = pickets[pickets.Count - 1].Number + 1, Start = pickets[pickets.Count - 1].Number * 100 };
            pickets.Add(result);
            return result;
        }
        public static DigressionMark GetFirstMove(this List<DigressionMark> notes)
        {
            DigressionMark result = null;
            for (int i = 0; i < notes.Count; i++)
            {
                if (!(notes[i].Note().Contains("Уст.ск:") || notes[i].Note().Contains("R") || notes[i].NotMoveAlert))
                {
                    result = notes[i];
                    break;
                }
            }
            return result;
        }
        public static Bitmap ToBitmap(this int[,] GreyScaleArray)
        {
            int width = GreyScaleArray.GetLength(1); // read from file
            int height = GreyScaleArray.GetLength(0); // read from file
            var bitmap = new Bitmap(width, height, PixelFormat.Format32bppRgb);

            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                {
                    int red = GreyScaleArray[y, x]; // read from array
                    int green = GreyScaleArray[y, x]; // read from array
                    int blue = GreyScaleArray[y, x]; // read from array
                    bitmap.SetPixel(x, y, Color.FromArgb(0, red, green, blue));
                }
            return bitmap;
        }
        public static Bitmap ToRedBitmap(this int[,] GreyScaleArray)
        {
            int width = GreyScaleArray.GetLength(1); // read from file
            int height = GreyScaleArray.GetLength(0); // read from file
            var bitmap = new Bitmap(width, height, PixelFormat.Format32bppRgb);

            for (int x = 0; x < width; x++)
                for (int y = 0; y < height; y++)
                {
                    int red = GreyScaleArray[y, x]; // read from array
                    //int green = GreyScaleArray[y, x]; // read from array
                    //int blue = GreyScaleArray[y, x]; // read from array
                    bitmap.SetPixel(x, y, Color.FromArgb(0, red, 0, 0));
                }
            return bitmap;
        }
        public static int[,] ToRedMatrix(this Bitmap bitmap)
        {
            int hight = bitmap.Height;
            int width = bitmap.Width;

            int[,] matrix = new int[hight, width];
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < hight; j++)
                {
                    var pixelColor = bitmap.GetPixel(i, j);
                    matrix[j, i] = (int)(pixelColor.R * 0.3 + pixelColor.G * 0.59 + pixelColor.B * 0.11); ;
                }
            }
            return matrix;
        }
        public static int[,] ToMatrix(this Bitmap bitmap)
        {
            int hight = bitmap.Height;
            int width = bitmap.Width;

            int[,] matrix = new int[width, hight];
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < hight; j++)
                {
                    var pixelColor = bitmap.GetPixel(i, j);
                    matrix[i, j] = (int)(pixelColor.R * 0.3 + pixelColor.G * 0.59 + pixelColor.B * 0.11); ;
                }
            }
            return matrix;
        }
            public static SocketState SendMessageFromSocket(string ip, int port, string message)
        {
            try
            {
                // Буфер для входящих данных
                byte[] bytes = new byte[1024];

                // Соединяемся с удаленным устройством

                // Устанавливаем удаленную точку для сокета
                IPHostEntry ipHost = Dns.GetHostEntry(ip);
                IPAddress ipAddr = ipHost.AddressList[0];
                IPEndPoint ipEndPoint = new IPEndPoint(ipAddr, port);
                
                Socket sender = new Socket(ipAddr.AddressFamily, SocketType.Stream, ProtocolType.Tcp);

                // Соединяем сокет с удаленной точкой
                sender.Connect(ipEndPoint);


                byte[] msg = Encoding.UTF8.GetBytes(message);

                // Отправляем данные через сокет
                int bytesSent = sender.Send(msg);

                // Получаем ответ от сервера
                int bytesRec = sender.Receive(bytes);

                var reply = Encoding.UTF8.GetString(bytes, 0, bytesRec);

                // Используем рекурсию для неоднократного вызова SendMessageFromSocket()
                if (reply.IndexOf("<Success>") == -1)
                    return SocketState.Abortively;

                // Освобождаем сокет
                sender.Shutdown(SocketShutdown.Both);
                sender.Close();
                return SocketState.Success;


            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return SocketState.Abortively;
            }
        }



        public static double ToDoubleCoordinate(this int km, int meter)
        {
            return km + meter / 10000.0;
        }
        public static bool Between(this double number, double a, double b)
        {
            return a <= number && number <= b;
        }

        public static bool Between(this int number, int a, int b)
        {
            return a <= number && number <= b;
        }
        public static double RadiusToAvg(this double radius)
        {
            return radius switch
            {
                double rad when rad < 5102 && rad >= 4464 => 3.5,
                double rad when rad < 4464 && rad >= 3968 => 4,
                double rad when rad < 3968 && rad >= 3571 => 5,
                double rad when rad < 3571 && rad >= 3247 => 5.5,
                double rad when rad < 3247 && rad >= 2976 => 6,
                double rad when rad < 2976 && rad >= 2758 => 6.5,
                double rad when rad < 2758 && rad >= 2552 => 7,
                double rad when rad < 2552 && rad >= 2380 => 7.5,
                double rad when rad < 2380 && rad >= 2232 => 8,
                double rad when rad < 2232 && rad >= 2101 => 8.5,
                double rad when rad < 2101 && rad >= 1984 => 9,
                double rad when rad < 1984 && rad >= 1880 => 9.5,
                double rad when rad < 1880 && rad >= 1786 => 10,
                double rad when rad < 1786 && rad >= 1701 => 10.5,
                double rad when rad < 1701 && rad >= 1623 => 11,
                double rad when rad < 1623 && rad >= 1553 => 11.5,
                double rad when rad < 1553 && rad >= 1488 => 12,
                double rad when rad < 1488 && rad >= 1429 => 12.5,
                double rad when rad < 1429 && rad >= 1374 => 13,
                double rad when rad < 1374 && rad >= 1323 => 13.5,
                double rad when rad < 1323 && rad >= 1276 => 14,
                double rad when rad < 1276 && rad >= 1232 => 14.5,
                double rad when rad < 1232 && rad >= 1190 => 15,
                double rad when rad < 1190 && rad >= 1152 => 15.5,
                double rad when rad < 1152 && rad >= 1116 => 16,
                double rad when rad < 1116 && rad >= 1082 => 16.5,
                double rad when rad < 1082 && rad >= 1050 => 17,
                double rad when rad < 1050 && rad >= 1020 => 17.5,
                double rad when rad < 1020 && rad >= 992 => 18,
                double rad when rad < 992 && rad >= 965 => 18.5,
                double rad when rad < 965 && rad >= 940 => 19,
                double rad when rad < 940 && rad >= 916 => 19.5,
                double rad when rad < 916 && rad >= 893 => 20,
                double rad when rad < 893 && rad >= 871 => 20.5,
                double rad when rad < 871 && rad >= 850 => 21,
                double rad when rad < 850 && rad >= 831 => 21.5,
                double rad when rad < 831 && rad >= 812 => 22,
                double rad when rad < 812 && rad >= 794 => 22.5,
                double rad when rad < 794 && rad >= 776 => 23,
                double rad when rad < 776 && rad >= 760 => 23.5,
                double rad when rad < 760 && rad >= 744 => 24,
                double rad when rad < 744 && rad >= 729 => 24.5,
                double rad when rad < 729 && rad >= 714 => 25,
                double rad when rad < 714 && rad >= 700 => 25.5,
                double rad when rad < 700 && rad >= 687 => 26,
                double rad when rad < 687 && rad >= 674 => 26.5,
                double rad when rad < 674 && rad >= 661 => 27,
                double rad when rad < 661 && rad >= 649 => 27.5,
                double rad when rad < 649 && rad >= 638 => 28,
                double rad when rad < 638 && rad >= 627 => 28.5,
                double rad when rad < 627 && rad >= 616 => 29,
                double rad when rad < 616 && rad >= 605 => 29.5,
                double rad when rad < 605 && rad >= 595 => 30,
                double rad when rad < 595 && rad >= 585 => 30.5,
                double rad when rad < 585 && rad >= 576 => 31,
                double rad when rad < 576 && rad >= 567 => 31.5,
                double rad when rad < 567 && rad >= 558 => 32,
                double rad when rad < 558 && rad >= 549 => 32.5,
                double rad when rad < 549 && rad >= 541 => 33,
                double rad when rad < 541 && rad >= 533 => 33.5,
                double rad when rad < 533 && rad >= 525 => 34,
                double rad when rad < 525 && rad >= 518 => 34.5,
                double rad when rad < 518 && rad >= 510 => 35,
                double rad when rad < 510 && rad >= 503 => 35.5,
                double rad when rad < 503 && rad >= 496 => 36,
                double rad when rad < 496 && rad >= 489 => 36.5,
                double rad when rad < 489 && rad >= 483 => 37,
                double rad when rad < 483 && rad >= 476 => 37.5,
                double rad when rad < 476 && rad >= 470 => 38,
                double rad when rad < 470 && rad >= 464 => 38.5,
                double rad when rad < 464 && rad >= 458 => 39,
                double rad when rad < 458 && rad >= 452 => 39.5,
                double rad when rad < 452 && rad >= 446 => 40,
                double rad when rad < 446 && rad >= 441 => 40.5,
                double rad when rad < 441 && rad >= 436 => 41,
                double rad when rad < 436 && rad >= 430 => 41.5,
                double rad when rad < 430 && rad >= 425 => 42,
                double rad when rad < 425 && rad >= 420 => 42.5,
                double rad when rad < 420 && rad >= 415 => 43,
                double rad when rad < 415 && rad >= 411 => 43.5,
                double rad when rad < 411 && rad >= 406 => 44,
                double rad when rad < 406 && rad >= 401 => 44.5,
                double rad when rad < 401 && rad >= 397 => 45,
                double rad when rad < 397 && rad >= 392 => 45.5,
                double rad when rad < 392 && rad >= 388 => 46,
                double rad when rad < 388 && rad >= 384 => 46.5,
                double rad when rad < 384 && rad >= 380 => 47,
                double rad when rad < 380 && rad >= 376 => 47.5,
                double rad when rad < 376 && rad >= 372 => 48,
                double rad when rad < 372 && rad >= 368 => 48.5,
                double rad when rad < 368 && rad >= 364 => 49,
                double rad when rad < 364 && rad >= 361 => 49.5,
                double rad when rad < 361 && rad >= 357 => 50,
                double rad when rad < 357 && rad >= 354 => 50.5,
                double rad when rad < 354 && rad >= 350 => 51,
                double rad when rad < 350 && rad >= 347 => 51.5,
                double rad when rad < 347 && rad >= 343 => 52,
                double rad when rad < 343 && rad >= 340 => 52.5,
                double rad when rad < 340 && rad >= 337 => 53,
                double rad when rad < 337 && rad >= 334 => 53.5,
                double rad when rad < 334 && rad >= 331 => 54,
                double rad when rad < 331 && rad >= 328 => 54.5,
                double rad when rad < 328 && rad >= 325 => 55,
                double rad when rad < 325 && rad >= 322 => 55.5,
                double rad when rad < 322 && rad >= 319 => 56,
                double rad when rad < 319 && rad >= 316 => 56.5,
                double rad when rad < 316 && rad >= 313 => 57,
                double rad when rad < 313 && rad >= 311 => 57.5,
                double rad when rad < 311 && rad >= 308 => 58,
                double rad when rad < 308 && rad >= 305 => 58.5,
                double rad when rad < 305 && rad >= 303 => 59,
                double rad when rad < 303 && rad >= 300 => 59.5,
                double rad when rad < 300 && rad >= 298 => 60,
                double rad when rad < 298 && rad >= 295 => 60.5,
                double rad when rad < 295 && rad >= 293 => 61,
                double rad when rad < 293 && rad >= 288 => 62.5,
                double rad when rad < 288 && rad >= 283 => 63,
                double rad when rad < 283 && rad >= 279 => 63.5,
                double rad when rad < 279 && rad >= 274 => 64,
                double rad when rad < 274 && rad >= 270 => 64.5,
                double rad when rad < 270 && rad >= 266 => 65,
                double rad when rad < 266 && rad >= 263 => 65.5,
                double rad when rad < 263 && rad >= 259 => 66,
                double rad when rad < 259 && rad >= 255 => 66.5,
                double rad when rad < 255 && rad >= 252 => 67,
                double rad when rad < 252 && rad >= 250 => 67.5,
                double rad when rad < 250 && rad >= 247 => 68,
                double rad when rad < 247 && rad >= 245 => 68.5,
                double rad when rad < 245 && rad >= 244 => 69,
                double rad when rad < 244 && rad >= 238 => 71.5,
                double rad when rad < 238 && rad >= 234 => 73,
                double rad when rad < 234 && rad >= 231 => 74.5,
                double rad when rad < 231 && rad >= 228 => 76,
                double rad when rad < 228 && rad >= 225 => 77.5,
                double rad when rad < 225 && rad >= 223 => 78,
                double rad when rad < 223 && rad >= 220 => 79.5,
                double rad when rad < 220 && rad >= 217 => 81,
                double rad when rad < 217 && rad >= 215 => 81.5,
                double rad when rad < 215 && rad >= 212 => 83,
                double rad when rad < 212 && rad >= 210 => 83.5,
                double rad when rad < 210 && rad >= 207 => 85,
                double rad when rad < 207 && rad >= 205 => 85.5,
                double rad when rad < 205 && rad >= 203 => 86,
                double rad when rad < 203 && rad >= 200 => 87.5,
                double rad when rad < 200 && rad >= 198 => 88,
                double rad when rad < 198 && rad >= 196 => 88.5,
                double rad when rad < 196 && rad >= 194 => 89,
                double rad when rad < 194 && rad >= 191 => 90.5,
                double rad when rad < 191 && rad >= 189 => 91,
                double rad when rad < 189 && rad >= 187 => 91.5,
                double rad when rad < 187 && rad >= 185 => 92,
                double rad when rad < 185 && rad >= 183 => 92.5,
                double rad when rad < 183 && rad >= 181 => 93,
                double rad when rad < 181 && rad >= 180 => 93.5,
                double rad when rad < 180 && rad >= 178 => 94,
                double rad when rad < 178 && rad >= 176 => 94.5,
                double rad when rad < 176 && rad >= 174 => 95,
                double rad when rad < 174 && rad >= 172 => 95.5,
                double rad when rad < 172 && rad >= 170 => 96,
                double rad when rad < 170 && rad >= 168 => 96.5,
                double rad when rad < 168 && rad >= 166 => 97,
                _ => 0,
            };
        }
        public static int[] RealCoordinateToKmMeter(this float real)
        {
            var result = new int[] { -1, -1 };
            result[0] = (int)real;
            result[1] = (int)((real % 1) * 10000);
            return result;

        }
        public static int[] RealCoordinateToKmMeter(this double real)
        {
            var result = new int[] { -1, -1 };
            result[0] = (int)real;
            result[1] = (int)((real % 1) * 10000);
            return result;

        }
        public static List<double> GrapLogic(List<double> originalDbPoints, int ibigX, int iN)
        {
            try
            {

                //int fN = n;

                double bettaOld;
                double bettaNew = originalDbPoints[0];
                double alpaOld = 0;
                double alpaNew = 0;


                List<double> alpaFPoints = new List<double>();


                double top1 = 0;
                double top2 = 0;
                double sumY = 0;
                double sumX = 0;
                double bottResult1 = 0;
                double bottomResult2 = 0;

                for (var x = ibigX; x < ibigX + iN; x++)
                {
                    var X = x - ibigX;
                    if (X < 0)
                        Console.Out.WriteLine("minus tabildi " + X);

                    top1 += X * originalDbPoints[x];
                    sumY += originalDbPoints[x];
                    sumX += X;
                    bottResult1 += Math.Pow(X, 2);

                }
                top1 = iN * top1;
                top2 = sumY * sumX;
                bottResult1 = iN * bottResult1;
                bottomResult2 = Math.Pow(sumX, 2);

                alpaOld = alpaNew;

                if (Math.Abs(bottResult1 - bottomResult2) > 0.0000011)
                    alpaNew = (top1 - top2) / (bottResult1 - bottomResult2);

                bettaOld = bettaNew;
                bettaNew = (sumY - alpaNew * sumX) / iN;


                //bettaNew = (alpaOld * (iN) + bettaOld + bettaNew) / 2;
                alpaFPoints.Add(alpaNew);
                alpaFPoints.Add(bettaNew);




                return alpaFPoints;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }

        }
        public static List<double> GetDrawPoints(int maxValue, double alpa, double betta)
        {
            List<double> drawPoints = new List<double>();
            for (var x = 0; x < maxValue; x++)
            {
                drawPoints.Add((x * alpa + betta));
            }
            return drawPoints;
        }
        public static List<double> GetTrapezoid2(this List<double> avg, List<double> prev, List<double> next, bool getPairs = false)
        {
            var alphaBettaPairs = new List<double>();
            var result = new List<double>();
            int n = 50;
            int avgElCount = n;

            int bigX = 0;
            int nOld = n;
            int nNew = n;
            int pointCount = 10;

            var originalDbPoints = new List<double>();
            originalDbPoints.AddRange(prev);
            originalDbPoints.AddRange(avg);
            originalDbPoints.AddRange(next);

            List<double> firstCall = GrapLogic(originalDbPoints, bigX, n);
            //birinshi resultati
            double foundAlpa1 = firstCall[0];
            double foundBetta1 = firstCall[1];

            //ekinshhi
            List<double> secondCall = GrapLogic(originalDbPoints, bigX + n, n);

            //ekinshi resultati
            double foundAlpa2 = secondCall[0];


            double foundBetta2 = secondCall[1];




            while (bigX < originalDbPoints.Count() - 4 * n)
            {
                List<double> lastPoints = new List<double>();
                for (var t = -pointCount; t <= pointCount; t++)
                {
                    var r = Math.Abs(foundAlpa1 * (nNew + t) + foundBetta1 - (foundAlpa2 * t + foundBetta2));
                    lastPoints.Add(r);
                }
                var minPoint = lastPoints.Min();
                var avgPoint = lastPoints.Average();
                var indexOfT = lastPoints.IndexOf(minPoint);

                //nOld = nNew;
                nOld = nNew + (indexOfT - pointCount);
                nNew = n - (indexOfT - pointCount);
                var newAlpaPoints = GetDrawPoints(nOld, foundAlpa1, foundBetta1);
                newAlpaPoints.ForEach(a => result.Add(a));

                bigX += nOld;
                firstCall = GrapLogic(originalDbPoints, bigX, nNew);
                foundAlpa1 = firstCall[0];
                foundBetta1 = firstCall[1];
                secondCall = GrapLogic(originalDbPoints, bigX + nNew, n);
                foundAlpa2 = secondCall[0];
                foundBetta2 = secondCall[1];
            }
            result = result.GetRange(prev.Count, result.Count - prev.Count);

            if (result.Count < avg.Count)
                result.AddRange(avg.GetRange(result.Count, avg.Count - result.Count));
            if (getPairs)
                return alphaBettaPairs;
            return result;
        }
        public static List<double> GetTrapezoid(
            this List<double> avg, List<double> prev, List<double> next,
            int height, ref List<NatureCurves> CurveForLvl,
            bool getPairs = false, Direction naprav = Direction.Direct, List<double> strRealData = null)
        {


            var ZeroDataStright = avg.Select(o => o * 0.0).ToList();

            try
            {
                var listY = new List<double>();
                var listX = new List<double>();

                var transitions = new List<double>();

                var result = new List<double>();

                var originalDbPoints = new List<double>();
                var RealoriginalDbPoints = new List<double>();


                originalDbPoints.AddRange(prev);
                originalDbPoints.AddRange(avg);
                // originalDbPoints.AddRange(avg.Select(p => Math.Abs(p)).ToList());
                originalDbPoints.AddRange(next);

                if (height == 4)
                {
                    RealoriginalDbPoints.AddRange(prev);
                    RealoriginalDbPoints.AddRange(strRealData);
                    RealoriginalDbPoints.AddRange(next);
                }


                if (naprav == Direction.Reverse)
                {
                    originalDbPoints.Reverse();
                    RealoriginalDbPoints.Reverse();
                }



                listY.Add(originalDbPoints[0]);
                listX.Add(0);

                int i = 0;

                for (int j = i + 1; j < originalDbPoints.Count; j++)
                {
                    double
                        Bx = j - i,
                        By = originalDbPoints[j] - originalDbPoints[i];

                    double otnosh = By / Bx;

                    for (int k = i; k < j + 1; k++)
                    {
                        var A = -By;
                        var B = Bx;
                        var C = i * originalDbPoints[j] - j * originalDbPoints[i];

                        var maxDistance = Math.Abs(A * k + B * originalDbPoints[k] + C) / Math.Sqrt(A * A + B * B);

                        var h = Math.Abs(originalDbPoints[j - 1]);

                        var mDis = 2 + (4 * h * h) / (h * h + 200);

                        var kk = 0.1;

                        if (maxDistance > mDis * kk)
                        {
                            listY.Add(originalDbPoints[k + 1]);
                            listX.Add(k + 1);

                            i = k + 1;
                            break;
                        }
                    }
                }

                //listY.Add(0);
                //listX.Add(listX.Last()+100);


                if (!listX.Where(s => s == originalDbPoints.IndexOf(originalDbPoints.Count())).Any())
                {
                    listY.Add(originalDbPoints.Last());
                    listX.Add(originalDbPoints.Count - 1);
                }

                double linearY = originalDbPoints[0];
                var k_linear = new List<double> { };
                var linear_prom = new List<double> { };
                var b_linear = new List<double> { };
                var ind_k_min = new List<int> { };
                var ind_lk = new List<int> { };

                int ind = 0;

                var avglinear = new List<double> { };
                var maxlinear = new List<double> { };

                for (int t = 0; t < listY.Count() - 1; t++)
                {
                    var indAvg = new List<double> { };
                    var indMax = new List<double> { };

                    for (int c = 0; c < listX[t + 1] - listX[t]; c++)
                    {
                        double x = listX[t] + c;
                        double dx1 = x - listX[t];

                        double bottom_dx1 = listX[t + 1] - listX[t];

                        double y2 = listY[t + 1];
                        double dx2 = x - listX[t + 1];
                        double bottom_dx2 = listX[t] - listX[t + 1];

                        double y1 = listY[t];
                        linearY = (y2 - y1) / bottom_dx1 * c + y1;

                        result.Add(linearY);

                        ind++;

                        indAvg.Add(linearY);
                        indMax.Add(Math.Abs(linearY));
                    }
                    //ToDo Tolegen
                    if (indAvg.Count > 0)
                        avglinear.Add(indAvg.Average());
                    if (indMax.Count > 0)
                        maxlinear.Add(indMax.Max());

                    ind_lk.Add(ind);
                    k_linear.Add((listY[t + 1] - listY[t]) / (listX[t + 1] - listX[t]));
                    b_linear.Add(listY[t]);
                }

                var resultTest = new List<double>();
                resultTest = result;

                var ResultTestCurve = new List<ResultTestCurve>();

                if (height == 4)
                {
                    var tempRealY = new List<double> { };
                    var tempY = new List<double> { };
                    var tempX = new List<int> { };

                    //      var tempRes = result[0] > 0 ? 1 : result[0];

                    // tempRealY.Add(RealoriginalDbPoints[0]);
                    ///tempY.Add(result[0]);
                    //tempX.Add(0);
                    var flag_Plus = false;
                    var flag_Minus = false;
                    for (int resInd = 1; resInd < result.Count - 1; resInd++)
                    {
                        //var incVal = result[resInd] == 0 ? 1 : result[resInd];
                        //var incVal = result[resInd] ==0 ? 1 : result[resInd];


                        if (result[resInd] > 7)
                        {
                            tempRealY.Add(RealoriginalDbPoints[resInd]);
                            tempY.Add(result[resInd]);
                            tempX.Add(resInd);
                            flag_Plus = true;
                        }
                        if (result[resInd] < 7 && flag_Plus)
                        {
                            ResultTestCurve.Add(
                                new ResultTestCurve
                                {
                                    RealY = tempRealY,
                                    Y = tempY,
                                    X = tempX
                                });

                            tempRealY = new List<double> { };
                            tempY = new List<double> { };
                            tempX = new List<int> { };
                            flag_Plus = false;
                            //tempRes = result[resInd] == 0 ? 1 : result[resInd];
                        }



                        if (result[resInd] < -7)
                        {
                            tempRealY.Add(RealoriginalDbPoints[resInd]);
                            tempY.Add(result[resInd]);
                            tempX.Add(resInd);
                            flag_Minus = true;
                        }

                        if (result[resInd] > -7 && flag_Minus)
                        {
                            ResultTestCurve.Add(
                                new ResultTestCurve
                                {
                                    RealY = tempRealY,
                                    Y = tempY,
                                    X = tempX
                                });

                            tempRealY = new List<double> { };
                            tempY = new List<double> { };
                            tempX = new List<int> { };
                            flag_Minus = false;
                            //tempRes = result[resInd] == 0 ? 1 : result[resInd];
                        }







                    }


                    //ResultTestCurve.Add(
                    //            new ResultTestCurve
                    //            {
                    //                RealY = tempRealY,
                    //                Y = tempY,
                    //                X = tempX
                    //            });
                }
                if (ResultTestCurve.Any())
                {
                    var aaa = ResultTestCurve.Where(o => o.RealY.Count > 20).ToList();
                    var bbb = aaa.Where(o => o.RealY.Select(o => Math.Abs(o)).Max() >= 35).ToList();
                    var ttt = bbb.Where(o => o.RealY.Count < 100).ToList();

                    if (height == 4)
                    {
                        foreach (var item in ttt)
                        {
                            var localInd = 0;
                            for (int q = item.X.First(); q < item.X.Last(); q++)
                            {
                                result[q] = 0.0;
                                localInd++;
                            }
                            var ppx = item.X.First();
                            var ppy = item.X.Last();
                        }



                    }
                }

                if (height == 4)
                {
                    var Curves = new List<NatureCurves> { };
                    var Curve = new NatureCurves { };

                    var CurveData = new List<double>();
                    var CurveIndex = new List<int>();


                    for (int ii = 0; ii < Math.Min(k_linear.Count, maxlinear.Count) - 1; ii++)
                    {
                        linear_prom.Clear();

                        //0-ге жапсыру
                        if (Math.Abs(maxlinear[ii]) <= 4)
                        {
                            linear_prom.Clear();



                            for (int c = 0; c < listX[ii + 1] - listX[ii]; c++)
                            {
                                var prom = 0.0;
                                linear_prom.Add(prom);
                            }
                            var p = 0;
                            for (int y = (int)listX[ii]; y < listX[ii + 1]; y++)
                            {
                                result[y] = linear_prom[p];
                                p++;
                            }
                            if (Curve.X.Count > 1 && (Curve.Y.Max() > 8 || Curve.Y.Min() < -8))
                            {
                                Curve.Added = true;
                                Curves.Add(Curve);
                                Curve = new NatureCurves { };
                            }

                        }

                        //5-тен жогарылары 

                        if (Math.Abs(maxlinear[ii]) > 4)
                        {
                            for (int y = (int)listX[ii]; y < listX[ii + 1]; y++)
                            {
                                if (Curve.Y.Any() && y - Curve.X.Last() > 4)
                                {
                                    Curves.Add(Curve);
                                    Curve = new NatureCurves { };
                                }

                                CurveData.Add(result[y]);
                                CurveIndex.Add(y);

                                Curve.Y.Add(result[y]);
                                Curve.X.Add(y);
                            }
                        }
                    }

                    if (!Curve.Added)
                    {
                        Curve.Added = true;
                        Curves.Add(Curve);
                        Curve = new NatureCurves { };
                    }

                    //Если нужна средняя линия
                    //var resultCopy = result;
                    var resultCopy = new List<double>(new double[result.Count]);

                    Curves = Curves.Where(o => o.X.Count > 0).ToList();
                    ///                  Расширяем границы
                    ///                  



                    ///Расширяем границы кривой на 30 метров, с высотой меньше 10 мм

                    for (int cInd = 0; cInd < Curves.Count; cInd++)
                    {
                        var tempCurveMaxY = Curves[cInd].Y.Max();

                        if (tempCurveMaxY <= 10 && Curves[cInd].X.Count() > 50)
                        {
                            //начало кривой
                            var delta = 50;
                            var startInd = Curves[cInd].X.First() - delta;

                            if (startInd < 0)
                            {
                                startInd = 0;
                            }
                            var tempCurveLen = Curves[cInd].X.Count() + delta + 15;
                            //конец кривой


                            if (tempCurveLen + startInd > result.Count())
                            {
                                tempCurveLen = result.Count() - startInd;
                            }

                            Curves[cInd] = new NatureCurves { };
                            try
                            {
                                var tY = result.GetRange(startInd, tempCurveLen).ToList();
                                var tX = Enumerable.Range(startInd, tempCurveLen).ToList();
                                Curves[cInd].X.AddRange(tX);
                                Curves[cInd].Y.AddRange(tY);
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.Message);
                            }

                        }

                        if (tempCurveMaxY > 20 && Curves[cInd].X.Count() > 50)
                        {
                            //начало кривой
                            var delta = 10;
                            var startInd = Curves[cInd].X.First() - delta;

                            if (startInd < 0)
                            {
                                startInd = 0;
                            }
                            var tempCurveLen = Curves[cInd].X.Count() + delta + 10;
                            //конец кривой


                            if (tempCurveLen + startInd > result.Count())
                            {
                                tempCurveLen = result.Count();
                            }

                            Curves[cInd] = new NatureCurves { };

                            var tY = result.GetRange(startInd, tempCurveLen).ToList();
                            var tX = Enumerable.Range(startInd, tempCurveLen).ToList();

                            Curves[cInd].X.AddRange(tX);
                            Curves[cInd].Y.AddRange(tY);
                        }




                    }

                    var DopLinear = new List<int>();

                    //Обнуляем места без кривых
                    if (Curves.Count > 0)
                    {
                        CurveForLvl.AddRange(Curves);
                    }

                    if (CurveForLvl.Count() < 1)
                    {
                        for (int index = 0; index < resultCopy.Count(); index++)  //0=25
                        {
                            resultCopy[index] = 0.0;
                        }
                        result = resultCopy;
                    }

                    foreach (var item in Curves)
                    {

                        var ModifiedCurve = new List<double>();

                        //работа с кривой
                        var CurveMax = item.Y.Max(o => Math.Abs(o));
                        var SelectedData = item.Y.Where(o => CurveMax * 0.8 < Math.Abs(o)).ToList();

                        var avgSelD = SelectedData.Average();

                        var Head1 = item.Y.IndexOf(SelectedData.First());
                        var HeadX = item.X[item.Y.IndexOf(SelectedData.First())];
                        //X кордината первой(Первой(First)) точки трапеции  
                        //Y кордината первой(Первой(First)) точки трапеции  
                        var Head2 = item.Y.IndexOf(SelectedData.Last());            //Y кордината второй(Последней(Last)) точки трапеции
                        var HeadX2 = item.X[Head2];                                 //Y кордината второй(Последней(Last)) точки трапеции
                                                                                    //Y кордината первой(Первой(First)) точки трапеции  

                        var min_SelectedData = SelectedData.Select(Math.Abs).Min();


                        /////////////  Yama
                        ///
                        var SelectedData_Yama_Down = new List<Double>();
                        var SelectedData_After_Yama = new List<Double>();
                        var SelectedData_Before_Yama = new List<Double>();
                        var Yama = false;
                        ///////////////////
                        ///

                        //var Yama_Ind_Min = SelectedData.IndexOf( Min_SelectedData_Yama);



                        var Head1_Yama_Up = 0;
                        var HeadX_Yama_Up = 0;
                        var Head2_Yama_Up = 0;            //Y кордината второй(Последней(Last)) точки трапеции
                        var HeadX2_Yama_Up = 0;                                 //Y кордината второй(Последней(Last)) точки трапеции




                        var Head1_Yama_down = 0;
                        var HeadX_Yama_down = 0;
                        var Head2_Yama_down = 0;            //Y кордината второй(Последней(Last)) точки трапеции
                        var HeadX2_Yama_down = 0;
                        var countYama = 0;

                        if (SelectedData.Count > 50 && CurveMax > 20)
                        {

                            // var Yama_Ind_Min = SelectedData.IndexOf(Min_SelectedData_Yama);
                            var SelectedData_Yama = item.Y.Where(o => CurveMax * 0.77 > Math.Abs(o) && item.X[item.Y.IndexOf(o)] < HeadX2 - 10 && item.X[item.Y.IndexOf(o)] > item.X[item.Y.IndexOf(SelectedData.First())] + 10);
                            countYama = SelectedData_Yama.Count();

                            if (SelectedData_Yama.Count() > 40)
                            {
                                //Y кордината второй(Последней(Last)) точки трапеции

                                var Min_SelectedData_Yama = (int)SelectedData_Yama.Select(Math.Abs).Min();
                                SelectedData_Yama_Down.AddRange(item.Y.Where(o => 1.2 * Min_SelectedData_Yama > Math.Abs(o)
                               && item.X[item.Y.IndexOf(o)] < HeadX2 - 10
                               && item.X[item.Y.IndexOf(o)] > item.X[item.Y.IndexOf(SelectedData.First())] + 10));


                                Head1_Yama_Up = item.Y.IndexOf(SelectedData_Yama.First());
                                HeadX_Yama_Up = item.X[item.Y.IndexOf(SelectedData_Yama.First())];
                                Head2_Yama_Up = item.Y.IndexOf(SelectedData_Yama.Last());
                                HeadX2_Yama_Up = item.X[Head2_Yama_Up];

                                Head1_Yama_down = item.Y.IndexOf(SelectedData_Yama_Down.First());
                                HeadX_Yama_down = item.X[item.Y.IndexOf(SelectedData_Yama_Down.First())];
                                Head2_Yama_down = item.Y.IndexOf(SelectedData_Yama_Down.Last());            //Y кордината второй(Последней(Last)) точки трапеции
                                HeadX2_Yama_down = item.X[Head2_Yama_down];


                                SelectedData_After_Yama.AddRange(item.Y.Where(o =>
                              item.X[item.Y.IndexOf(o)] < HeadX2
                             && item.X[item.Y.IndexOf(o)] > HeadX2_Yama_Up)); ;

                                SelectedData_Before_Yama.AddRange(item.Y.Where(o =>
                         item.X[item.Y.IndexOf(o)] > HeadX
                        && item.X[item.Y.IndexOf(o)] < HeadX_Yama_Up));


                                Yama = true;

                            }
                        }




                        var S_Left = new List<double> { };
                        var S_Right = new List<double> { };
                        var Points_manyR_str_RightX = new List<int> { };
                        var Points_manyR_str_LeftX = new List<int> { };

                        var Points_manyR_str_LeftY = new List<double> { };
                        var Points_manyR_str_RightY = new List<double> { };
                        var Points_manyR_str_LeftY_Head = new List<double> { };
                        var Points_manyR_str_RightY_Head = new List<double> { };

                        var flagLeft = 0;
                        var flagRight = 0;
                        for (int iii = 1; iii < 5; iii++)
                        {

                            var SelectedDataLeftI = item.Y.Where(o => CurveMax * 0.15 * (iii + 1) > Math.Abs(o) && Math.Abs(o) > CurveMax * 0.15 * (iii)
                                                       && (item.X[item.Y.IndexOf(o)] < HeadX - 30)).ToList();

                            //var SelectedDataLeftI = item.Y.Where(o => CurveMax * (0.2 * (iii) + 0.2) > Math.Abs(o) && Math.Abs(o) > CurveMax * (0.20 * (iii - 1) + 0.20)
                            // && (item.X[item.Y.IndexOf(o)] < HeadX - 30)).ToList();
                            //   var SelectedDataLeftI  = item.Y.Where(o => CurveMax * (0.2 * (5 - iii + 1)+0.2) > Math.Abs(o) && Math.Abs(o) > CurveMax *( 0.2 * (5 - iii)+0.2)
                            //                   && (item.X[item.Y.IndexOf(o)] < HeadX -20)).ToList();
                            //var SelectedDataRightI = item.Y.Where(o => CurveMax * 0.8 > Math.Abs(o) && (item.X[item.Y.IndexOf(o)] > HeadX2)).ToList();
                            var sl = SelectedDataLeftI.Count;

                            if (sl > 35 && flagLeft == 0 && (countYama < 40))
                            {
                                flagLeft = 1;
                                var Head1I = item.Y.IndexOf(SelectedDataLeftI.First());
                                var HeadXI = item.X[item.Y.IndexOf(SelectedDataLeftI.First())];
                                var Head2I = item.Y.IndexOf(SelectedDataLeftI.Last());            //Y кордината второй(Последней(Last)) точки трапеции
                                var HeadX2I = item.X[Head2I];


                                Points_manyR_str_LeftY_Head.Add(Head1I);
                                Points_manyR_str_LeftY_Head.Add(Head2I);
                                Points_manyR_str_LeftX.Add(HeadXI);
                                Points_manyR_str_LeftX.Add(HeadX2I);

                                Points_manyR_str_LeftY.Add(SelectedDataLeftI.Average());
                                Points_manyR_str_LeftY.Add(SelectedDataLeftI.Average());
                            }
                            var SelectedDataRightI = item.Y.Where(o => CurveMax * 0.2 * (5 - iii + 1) > Math.Abs(o) && Math.Abs(o) > CurveMax * 0.2 * (5 - iii)
                                                       && (item.X[item.Y.IndexOf(o)] > HeadX2 + 10)).ToList();

                            var sr = SelectedDataRightI.Count;
                            if (sr > 50 && flagRight == 0 && (countYama < 40))
                            {
                                flagRight = 1;
                                S_Right.Add(sr);
                                var Head1I = item.Y.IndexOf(SelectedDataRightI.First());
                                var HeadXI = item.X[item.Y.IndexOf(SelectedDataRightI.First())];
                                var Head2I = item.Y.IndexOf(SelectedDataRightI.Last());            //Y кордината второй(Последней(Last)) точки трапеции
                                var HeadX2I = item.X[Head2I];

                                Points_manyR_str_RightY_Head.Add(Head1I);
                                Points_manyR_str_RightY_Head.Add(Head2I);
                                Points_manyR_str_RightX.Add(HeadXI);
                                Points_manyR_str_RightX.Add(HeadX2I);
                                Points_manyR_str_RightY.Add(SelectedDataRightI.Average());
                                Points_manyR_str_RightY.Add(SelectedDataRightI.Average());
                            }
                        }
                        ////////////////////

                        var flagMR1 = 0;

                        var ff1 = flagMR1;

                        var ff11 = flagMR1;

                        if (Points_manyR_str_LeftY.Count > 1)
                        {
                            Head1 = (int)Points_manyR_str_LeftY_Head[0];
                            HeadX = (int)Points_manyR_str_LeftX[0];

                            flagMR1 = 2;
                        }

                        if (Points_manyR_str_RightY.Count > 1)
                        {
                            Head2 = (int)Points_manyR_str_RightY_Head[1];
                            HeadX2 = (int)Points_manyR_str_RightX[1];

                            flagMR1 = 2;
                        }
                        var Hsrt = 20;
                        if (Math.Abs(item.Y[Head1]) <= Hsrt)
                        {
                            var Ss_list = new List<double> { };
                            var S_min_list = new List<double> { };
                            var S_min_k_list = new List<double> { };
                            /////////////////////////////////
                            var kp = 0.01;
                            if (Points_manyR_str_LeftY.Count > 1)//&& Points_manyR_str_LeftY.Count > 1
                            { kp = 0.1; }

                            if (Points_manyR_str_LeftY.Count < 1 && Math.Abs(item.Y[Head1]) < 15)//&& Points_manyR_str_LeftY.Count > 1
                            { kp = 0.125; }
                            for (int k = 0; k < 12; k++)
                            {
                                Ss_list.Clear();
                                for (int ii = 0; ii < 1000; ii++)
                                {
                                    var s = 0.0;
                                    var s2 = 0.0;
                                    var y0 = Math.Abs(item.Y[Head1]);
                                    for (int m = 0; m < Head1; m++)
                                    {
                                        var y1 = (2.0 - 0.1 * k) * y0 + 0.005 * (ii - 500) * m;
                                        var y2 = (2.0 - 0.1 * k) * Math.Abs(item.Y[Head1 - m]);
                                        //if (y1 > y2) { s = s + 2 * (y1 - y2); }
                                        //if (y2 > y1) { s = s + (y2 - y1); } 

                                        s += Math.Abs(y1 - y2) * Math.Exp(-kp * Math.Abs(Head1 / 2.0 - m));
                                    }
                                    Ss_list.Add((int)s);
                                }
                                var testMin0 = Ss_list.Min();
                                var testMinInd0 = Ss_list.IndexOf(testMin0);
                                S_min_list.Add((int)testMin0);
                                S_min_k_list.Add(testMinInd0);
                            }
                            var testMin_min = (int)S_min_list.Min();
                            var testMin_min_Ind = S_min_list.IndexOf(testMin_min);
                            var kof = S_min_k_list[testMin_min_Ind];

                            var vhod = -(int)(0.99 * Math.Abs(item.Y[Head1] / (0.005 * (kof - 500))));

                            HeadX = (int)(HeadX + vhod < 0 ? 0 : HeadX + vhod);

                        }


                        if (Math.Abs(item.Y[Head1]) >= Hsrt)
                        {
                            var M_luch = new List<double> { };
                            var Ss_listH1 = new List<double> { };
                            var S_min_listH1 = new List<double> { };
                            var S_min_k_listH1 = new List<double> { };
                            int dp = 0;
                            var mH1 = 1.0;
                            var step = 0.0005;
                            var kof00 = mH1 * 1.0 * Math.Abs(item.Y[Head1]) / Head1;
                            var kp = 0.02;
                            if (Points_manyR_str_LeftY.Count > 1)//&& Points_manyR_str_LeftY.Count > 1
                            { kp = 0.1; }


                            for (int k = dp; k < 2 * dp + 1; k++)
                            {
                                Ss_listH1.Clear();
                                for (int ii = 1; ii < 2000; ii++)
                                {

                                    var s = 0.0;
                                    var y0 = 2 * item.Y[Head1];
                                    var lambda = 0.95;
                                    double sd = 0;
                                    var y00 = Math.Abs(item.Y[Head1]);
                                    var maxd = 0;

                                    for (int m = 0; m < Head1; m++)
                                    {

                                        var y1 = mH1 * (1 - 0.001 * k) * y00 - (kof00 + (ii - 1000) * step) * m;

                                        var y2 = mH1 * (1 - 0.001 * k) * Math.Abs(item.Y[Head1 - m]);
                                        s += Math.Abs(y1 - y2) * Math.Exp(-kp * Math.Abs(Head1 / 2.0 - m));
                                    }

                                    Ss_listH1.Add(s);

                                }
                                var testMin0H1 = Ss_listH1.Min();
                                var testMinInd0H1 = Ss_listH1.IndexOf(testMin0H1);
                                S_min_listH1.Add((int)testMin0H1);
                                S_min_k_listH1.Add(testMinInd0H1);
                            }

                            var testMin_minH1 = (int)S_min_listH1.Min();
                            var testMin_min_IndH1 = S_min_listH1.IndexOf(testMin_minH1);
                            var popr1H1 = testMin_min_IndH1 - dp;
                            var kofH1 = S_min_k_listH1[testMin_min_IndH1];
                            var vhodH1 = -mH1 * Math.Abs(item.Y[Head1] / (kof00 + (kofH1 - 1000) * step));
                            HeadX = (int)(HeadX + vhodH1 < 0 ? 0 : HeadX + vhodH1);
                        }

                        ////////////////////////
                        ///


                        if (Math.Abs(item.Y[Head2]) >= Hsrt)
                        {
                            var M_luch = new List<double> { };
                            var Ss_listH2 = new List<double> { };
                            var S_min_listH2 = new List<double> { };
                            var S_min_k_listH2 = new List<double> { };
                            int dp = 0;
                            var mH2 = 1.0;
                            var step = 0.0005;
                            var kof00 = mH2 * 1.0 * Math.Abs(item.Y[Head2]) / (item.Y.Count() - Head2);

                            for (int k = dp; k < 2 * dp + 1; k++)
                            {
                                Ss_listH2.Clear();
                                for (int ii = 1; ii < 2000; ii++)
                                {

                                    var s = 0.0;
                                    var y0 = 1 * item.Y[Head2];
                                    var lambda = 0.95;
                                    double sd = 0;
                                    var y00 = Math.Abs(item.Y[Head2]);
                                    var maxd = 0;
                                    var sr = (item.Y.Count() - Head2) / 2;
                                    for (int m = 0; m < item.Y.Count() - Head2; m++)
                                    {

                                        var y1 = 1.0 * (1 - 0.001 * k) * y00 - (kof00 + (ii - 1000) * step) * m;

                                        var y2 = (1 - 0.001 * k) * Math.Abs(item.Y[Head2 + m]);
                                        s += Math.Abs((y2 - y1)) * Math.Exp(-0.01 * Math.Abs(sr - m));
                                    }

                                    Ss_listH2.Add(s);

                                }
                                var testMin0H2 = Ss_listH2.Min();
                                var testMinInd0H2 = Ss_listH2.IndexOf(testMin0H2);
                                S_min_listH2.Add((int)testMin0H2);
                                S_min_k_listH2.Add(testMinInd0H2);
                            }

                            var testMin_minH2 = (int)S_min_listH2.Min();
                            var testMin_min_IndH2 = S_min_listH2.IndexOf(testMin_minH2);
                            var popr1H2 = testMin_min_IndH2 - dp;
                            var kofH2 = S_min_k_listH2[testMin_min_IndH2];
                            var vhodH2 = mH2 * Math.Abs(item.Y[Head2] / (kof00 + (kofH2 - 1000) * step));
                            //  HeadX2 = (int)(HeadX2 + vhodH2 < 0 ? 0 : HeadX2 + vhodH2);
                            HeadX2 = (int)(HeadX2 + vhodH2 > result.Count() ? result.Count() - 1 : HeadX2 + vhodH2);
                        }




                        //----------------------------

                        if (Math.Abs(item.Y[Head2]) <= Hsrt)
                        {
                            var S2_list = new List<double> { };
                            var Ss2_list = new List<double> { };
                            var S2_min_list = new List<double> { };
                            var S2_min_k_list = new List<double> { };

                            for (int k = 8; k < 13; k++)
                            {

                                S2_list.Clear();
                                for (int ii = 0; ii < 1000; ii++)
                                {

                                    var s = 0.0;

                                    var kp = 0.1;
                                    if (Points_manyR_str_RightY.Count > 1)//&& Points_manyR_str_LeftY.Count > 1
                                    { kp = 1; }

                                    for (int m = 0; m < item.Y.Count() - Head2; m++)
                                    {
                                        var sr = (item.Y.Count() - Head2) / 2;
                                        var y00 = item.Y[Head2];
                                        var y1 = (2 - 0.1 * k) * y00 + 0.002 * (ii - 500) * m;
                                        var y2 = (2.0 - 0.1 * k) * item.Y[Head2 + m];
                                        // if (Math.Sign(y) != Math.Sign(item.Y[Head2 + m]) ){ y = 0; }
                                        s += Math.Abs(y1 - y2) * Math.Exp(-kp * Math.Abs(sr - m));
                                    }
                                    S2_list.Add((int)s);
                                }
                                var testMin2 = (S2_list.Min());
                                var testMinInd2 = S2_list.IndexOf(testMin2);
                                S2_min_list.Add(testMin2);
                                S2_min_k_list.Add(testMinInd2);
                            }
                            var testMin_min2 = (S2_min_list.Min());
                            var testMin_min_Ind2 = S2_min_list.IndexOf(testMin_min2);
                            var kof2 = S2_min_k_list[testMin_min_Ind2];

                            var vihod = (int)(0.99 * Math.Abs(item.Y[Head2] / (0.002 * (kof2 - 500))));
                            if (Math.Abs(item.Y[Head2]) < 10 && vihod > 70) vihod = vihod / 1;
                            HeadX2 = (int)(HeadX2 + vihod > result.Count() ? result.Count() - 1 : HeadX2 + vihod);
                        }

                        var listY1 = new List<double>
                        {
                            //Math.Abs(result[HeadX]) <= 5 ? 0 : result[HeadX],
                           0,
                            avgSelD,
                            avgSelD,
                            //Math.Abs(result[HeadX2]) <= 5 ? 0 : result[HeadX2],
                            0,
                        };

                        if (item.X[item.Y.IndexOf(SelectedData.First())] < HeadX)
                        {
                            HeadX = item.X[item.Y.IndexOf(SelectedData.First())];
                        }

                        var delta1 = 0;
                        var delta2 = 0;
                        try
                        {
                            if ((Points_manyR_str_LeftY.Count < 1 && Points_manyR_str_RightY.Count < 1) && SelectedData.Average() < 6)
                            {


                                delta1 = Math.Abs(HeadX - item.X[item.Y.IndexOf(SelectedData.First())]);
                                delta2 = Math.Abs(HeadX2 - item.X[item.Y.IndexOf(SelectedData.Last())]);
                                if (delta1 > 2.5 * delta2 && delta1 > 40 && delta2 < 100)
                                    HeadX2 = item.X[item.Y.IndexOf(SelectedData.Last())] + (int)(1 * delta1);
                                if (delta2 > 2.5 * delta1 & delta2 > 40 && delta2 < 100)
                                    HeadX = item.X[item.Y.IndexOf(SelectedData.First())] - (int)(1 * delta2);
                                HeadX2 = (int)(HeadX2 > result.Count() ? result.Count() - 1 : HeadX2);
                                HeadX = (int)(HeadX < 0 ? 0 : HeadX);
                            }
                        }
                        catch { }

                        /////////////////////
                        ///


                        var CurveMaxF = item.Y.Max(o => Math.Abs(o));
                        var SelectedDataFinal = item.Y.Where(o => CurveMaxF * 0.5 < Math.Abs(o)).ToList();


                        var F1 = item.X[item.Y.IndexOf(SelectedDataFinal.First())]; //центр1 50
                        var V1 = item.X[item.Y.IndexOf(SelectedData.First())];      //круг1 85

                        var F2 = item.X[item.Y.IndexOf(SelectedDataFinal.Last())]; //центр2 50
                        var V2 = item.X[item.Y.IndexOf(SelectedData.Last())];      //круг2 85


                        //----------------------------------------
                        int HalfFirst = (int)(F1 - (V1 + HeadX) / 2) / 2;  //x1
                        int HalflAST = (int)(F2 - (V2 + HeadX2) / 2) / 2;  //x2
                        if (HalflAST + HeadX2 > result.Count() - 400)
                        { HalflAST = 0; }
                        if (HalflAST / 5 + HeadX2 > result.Count())
                        { HalflAST = 0; }
                        //   var yf = 0;
                        var yf = 0;
                        var xpf = (int)(Head1 / 2);

                        if (xpf >= 0 && HeadX - prev.Count() + HalfFirst > 0 && HeadX + HalfFirst > 0) { yf = Math.Abs((int)item.Y[xpf + 1]); }


                        else
                        { HalfFirst = 0; }


                        if ((Points_manyR_str_LeftY.Count < 1 && Points_manyR_str_RightY.Count < 1))
                        {
                            if (Math.Abs(SelectedData.Average()) / 2.0 < yf)

                            {
                                HalfFirst = Math.Abs((HalfFirst));
                            }

                            if (Math.Abs(SelectedData.Average()) / 2.0 > yf)

                            {
                                HalfFirst = -Math.Abs(HalfFirst);
                            }


                            var xplast = (int)(item.Y.Count + Head2 - 1) / 2;
                            var u2 = Math.Abs(SelectedData.Average()) / 2.0;

                            if (xplast > item.Y.Count) { xplast = item.Y.Count - 10; HalflAST = 0; }
                            var yp = Math.Abs(item.Y[xplast]);
                            if ((item.Y.Count + Head2) / 2 > item.Y.Count) { HalflAST = 0; }

                            var xx = 0;
                            if (Math.Abs(SelectedData.Average()) / 2.0 < yp)
                            {
                                HalflAST = Math.Abs(HalflAST);
                            }
                            if (Math.Abs(SelectedData.Average()) / 2.0 > yp)

                            {
                                HalflAST = -Math.Abs(HalflAST);
                            }
                        }
                        else
                        {
                            HalfFirst = 0;
                            HalflAST = 0;
                        }

                        ////////////////////



                        HalfFirst = 0;
                        HalflAST = 0;
                        HeadX = HeadX + HalfFirst;

                        HeadX2 = HeadX2 + HalflAST;
                        var popr0 = Math.Abs(item.X[item.Y.IndexOf(SelectedData.First())] - item.X[item.Y.IndexOf(SelectedData.Last())]);
                        var poprStr = 3;
                        if (popr0 < 10) { poprStr = 3; }
                        if (popr0 > 10 && popr0 < 20) { poprStr = 5; }

                        if (popr0 > 20 && popr0 < 40) { poprStr = 8; }
                        if (popr0 > 40) { poprStr = 8; }

                        HalflAST = poprStr;
                        HalfFirst = poprStr;
                        //  if ((Math.Abs(item.Y[Head2]) < 15) || (Math.Abs(item.Y[Head1]) < 15))
                        //  { poprStr = 0; }
                        if (Points_manyR_str_LeftY.Count < 1 && Points_manyR_str_RightY.Count < 1)
                        {
                            HeadX = HeadX + 5;
                            HeadX2 = HeadX2 - 5;
                        }
                        var listX1 = new List<int>
                        {
                            HeadX ,
                            item.X[item.Y.IndexOf(SelectedData.First())] +  poprStr ,
                            item.X[item.Y.IndexOf(SelectedData.Last())] -  poprStr ,
                            HeadX2
                        };
                        //  for (int t = 0; t < (int)(listY1.Count() / 2 - 2); t++)

                        //Данные для карточки кривой
                        item.StrPoints.Add(HeadX - prev.Count());
                        item.StrPoints.Add(item.X[item.Y.IndexOf(SelectedData.First())] + poprStr - prev.Count());
                        item.StrPoints.Add(item.X[item.Y.IndexOf(SelectedData.Last())] - poprStr - prev.Count());
                        item.StrPoints.Add(HeadX2 - prev.Count());


                        if (Points_manyR_str_LeftY.Count > 1 && Points_manyR_str_RightY.Count < 1)
                        {
                            listY1.Clear();
                            listY1.Add(0);

                            for (var io = 0; io < Points_manyR_str_LeftY.Count; io++)
                            {
                                listY1.Add(Points_manyR_str_LeftY[io]);
                            };
                            listY1.Add(SelectedData.Average());
                            listY1.Add(SelectedData.Average());
                            listY1.Add(0);
                        };


                        if (Points_manyR_str_RightY.Count > 1 && Points_manyR_str_LeftY.Count < 1)
                        {
                            listY1.Clear();
                            listY1.Add(0);


                            listY1.Add(SelectedData.Average());
                            listY1.Add(SelectedData.Average());
                            for (var io = 0; io < Points_manyR_str_RightY.Count; io++)
                            {
                                listY1.Add(Points_manyR_str_RightY[io]);
                            };

                            listY1.Add(0);
                        };

                        if (Points_manyR_str_LeftY.Count > 1 && Points_manyR_str_RightY.Count > 1)
                        {
                            listY1.Clear();
                            listY1.Add(0);

                            for (var io = 0; io < Points_manyR_str_LeftY.Count; io++)
                            {
                                listY1.Add(Points_manyR_str_LeftY[io]);
                            };
                            listY1.Add(SelectedData.Average());
                            listY1.Add(SelectedData.Average());
                            for (var io = 0; io < Points_manyR_str_RightY.Count; io++)
                            {
                                listY1.Add(Points_manyR_str_RightY[io]);
                            };


                            listY1.Add(0);
                        };


                        if (Points_manyR_str_LeftY.Count > 1 && Points_manyR_str_RightY.Count < 1)
                        {
                            listX1.Clear();
                            item.StrPoints.Clear();

                            listX1.Add(HeadX);
                            item.StrPoints.Add(HeadX - prev.Count());

                            for (var io = 0; io < Points_manyR_str_LeftX.Count; io++)
                            {
                                listX1.Add(Points_manyR_str_LeftX[io]);
                                item.StrPoints.Add(Points_manyR_str_LeftX[io] - prev.Count());
                            };
                            listX1.Add(item.X[item.Y.IndexOf(SelectedData.First())]);
                            item.StrPoints.Add(item.X[item.Y.IndexOf(SelectedData.First())] - prev.Count());

                            listX1.Add(item.X[item.Y.IndexOf(SelectedData.Last())]);
                            item.StrPoints.Add(item.X[item.Y.IndexOf(SelectedData.Last())] - prev.Count());

                            listX1.Add(HeadX2);
                            item.StrPoints.Add(HeadX2 - prev.Count());
                        };

                        if (Points_manyR_str_LeftY.Count < 1 && Points_manyR_str_RightY.Count > 1)
                        {
                            listX1.Clear();
                            item.StrPoints.Clear();

                            listX1.Add(HeadX);
                            item.StrPoints.Add(HeadX - prev.Count());


                            listX1.Add(item.X[item.Y.IndexOf(SelectedData.First()) + poprStr]);
                            item.StrPoints.Add(item.X[item.Y.IndexOf(SelectedData.First())] + poprStr - prev.Count());

                            listX1.Add(item.X[item.Y.IndexOf(SelectedData.Last())] - 10 - poprStr);
                            item.StrPoints.Add(item.X[item.Y.IndexOf(SelectedData.Last())] - 10 - poprStr - prev.Count());


                            for (var io = 0; io < Points_manyR_str_RightX.Count; io++)
                            {
                                listX1.Add(Points_manyR_str_RightX[io] - 5);
                                item.StrPoints.Add(Points_manyR_str_RightX[io] - 5 - prev.Count());
                            };


                            listX1.Add(HeadX2);
                            item.StrPoints.Add(HeadX2 - prev.Count());
                        };


                        var popr = 0;
                        var popr1 = 0;

                        if (Points_manyR_str_LeftY.Count > 1 && Points_manyR_str_RightY.Count > 1)
                        {
                            popr = 0;
                            popr1 = 0;
                            listX1.Clear();
                            item.StrPoints.Clear();

                            listX1.Add(HeadX);
                            item.StrPoints.Add(HeadX - prev.Count());

                            for (var io = 0; io < Points_manyR_str_LeftX.Count; io++)

                            {
                                listX1.Add(Points_manyR_str_LeftX[io] + 5);
                                item.StrPoints.Add(Points_manyR_str_LeftX[io] + 5 - prev.Count());
                            };
                            listX1.Add(item.X[item.Y.IndexOf(SelectedData.First())] + 10);
                            item.StrPoints.Add(item.X[item.Y.IndexOf(SelectedData.First())] + 10 - prev.Count());

                            listX1.Add(item.X[item.Y.IndexOf(SelectedData.Last())] - 10);
                            item.StrPoints.Add(item.X[item.Y.IndexOf(SelectedData.Last())] - 10 - prev.Count());
                            for (var io = 0; io < Points_manyR_str_RightX.Count; io++)

                            {

                                listX1.Add(Points_manyR_str_RightX[io] - 5);
                                item.StrPoints.Add(Points_manyR_str_RightX[io] - 5 - prev.Count());
                            };
                            listX1.Add(HeadX2 - popr1);
                            item.StrPoints.Add(HeadX2 - popr1 - prev.Count());
                        };

                        if (Yama)
                        {
                            listY1.Clear();
                            listY1.Add(0);

                            listY1.Add(SelectedData_Before_Yama.Average());
                            listY1.Add(SelectedData_Before_Yama.Average());

                            listY1.Add(SelectedData_Yama_Down.Average());
                            listY1.Add(SelectedData_Yama_Down.Average());
                            listY1.Add(SelectedData_After_Yama.Average());
                            listY1.Add(SelectedData_After_Yama.Average());
                            for (var io = 0; io < Points_manyR_str_RightY.Count; io++)
                            {
                                listY1.Add(Points_manyR_str_RightY[io]);
                            };
                            listY1.Add(0);
                        };

                        if (Yama)
                        {
                            listX1.Clear();
                            item.StrPoints.Clear();

                            listX1.Add(HeadX);
                            item.StrPoints.Add(HeadX - prev.Count());


                            listX1.Add(item.X[item.Y.IndexOf(SelectedData.First())]);
                            item.StrPoints.Add(item.X[item.Y.IndexOf(SelectedData.First())] - prev.Count());

                            //Head1_Yama_Up
                            listX1.Add(HeadX_Yama_Up - 10);
                            item.StrPoints.Add(HeadX_Yama_Up - 10 - prev.Count());


                            listX1.Add(HeadX_Yama_down);
                            item.StrPoints.Add(HeadX_Yama_down - prev.Count());
                            /////////////////
                            ///
                            listX1.Add(HeadX2_Yama_down);
                            item.StrPoints.Add(HeadX2_Yama_down - prev.Count());
                            listX1.Add(HeadX2_Yama_Up);
                            item.StrPoints.Add(HeadX2_Yama_Up - prev.Count());

                            ////////////

                            listX1.Add(item.X[item.Y.IndexOf(SelectedData.Last())] - 5);
                            item.StrPoints.Add(item.X[item.Y.IndexOf(SelectedData.Last())] - 5 - prev.Count());
                            if (Points_manyR_str_RightY.Count > 1)
                            {
                                for (var io = 0; io < Points_manyR_str_RightX.Count; io++)
                                {
                                    listX1.Add(Points_manyR_str_RightX[io] + 0);
                                    item.StrPoints.Add(Points_manyR_str_RightX[io] + 0 - prev.Count());
                                };
                            }

                            listX1.Add(HeadX2);
                            item.StrPoints.Add(HeadX2 - prev.Count());
                        };

                        //Для Пру

                        //интерполяция кривой
                        var prevVal = listY1[1];
                        var xcount = listX1[0];
                        var c_first_count = 0;
                        // listX1.Add(item.Y.Count-1);
                        //listY1.Add(0);
                        //var prom = 0;

                        //for (int t = 1; t < listY1.Count() - 3; t++)

                        //{
                        //    listY1[t] = listY1.Average();


                        //}

                        for (int t = 0; t < listY1.Count() - 1; t++)
                        {
                            for (int c = 0; c < listX1[t + 1] - listX1[t]; c++)
                            {
                                double bottom_dx1 = listX1[t + 1] - listX1[t];

                                double y2 = listY1[t + 1];

                                double y1 = listY1[t];
                                linearY = (y2 - y1) / bottom_dx1 * c + y1;

                                //if (Math.Sign(prevVal) != Math.Sign(linearY))
                                //{
                                //    linearY = 0;
                                //    c_first_count = c;
                                //}

                                ModifiedCurve.Add(linearY);
                            }
                        }

                        HeadX = HeadX;

                        item.lvl_center_value = Math.Abs(avgSelD);
                        item.lvl_center_index = Math.Abs(HeadX2 - HeadX);

                        if (Curves.Count() < 1)
                        {
                            for (int index = 0; index < resultCopy.Count(); index++)
                            {
                                resultCopy[index] = 0.0;
                            }
                        }
                        var InternalIndex = 0;

                        for (int index = HeadX; index < HeadX2; index++)
                        {
                            resultCopy[index] = ModifiedCurve[InternalIndex];
                            //resultCopy[index] = result[index] ;
                            InternalIndex++;
                        }
                        for (int index = HeadX - 1 > 0 ? HeadX + -1 : HeadX; index < HeadX; index++)
                        {
                            resultCopy[index] = 0.0;
                        }
                        for (int index = HeadX2; index < (HeadX2 + 1 <= resultCopy.Count() ? HeadX2 + 1 : HeadX2); index++)  //0=25
                        {
                            resultCopy[index] = 0.0;
                        }
                    }




                    result = resultCopy;
                }


                //Уровеньге 0 линия саламыз
                if (height == 10)
                {
                    var ZeroData = result.Select(o => o * 0.0).ToList();

                    foreach (var item in CurveForLvl)
                    {
                        // new item gen
                        var start = item.X.First() < 0 ? 0 : item.X.First() - 0 * 5;
                        var final = item.X.Last() + 0 * 5 > result.Count - 1 ? result.Count - 1 : item.X.Last() + 0 * 5;

                        var ModifiedCurve = new List<double>();

                        item.Y.Clear();

                        item.Y.AddRange(result.GetRange(start, final - start + 1));

                        var ys = result.IndexOf(item.Y.First());
                        var yx = result.IndexOf(item.Y.Last());

                        item.X.Clear();
                        item.X = Enumerable.Range(ys, yx + 1).ToList();
                        var count = -ys + yx;
                        //item.Y.AddRange(result.GetRange(item.X.First(), item.X.Last() - item.X.First() + 1));

                        //работа с кривой
                        var CurveMax = item.Y.Max(o => Math.Abs(o));
                        var SelectedData = item.Y.Where(o => CurveMax * 0.8 < Math.Abs(o)).ToList();
                        var avgSelD = SelectedData.Average();

                        var Head1 = item.Y.IndexOf(SelectedData.First());
                        var HeadX = item.X[item.Y.IndexOf(SelectedData.First())];

                        var Head2 = item.Y.IndexOf(SelectedData.Last());
                        //  var HeadY2 = item.Y[Head2];
                        var HeadX2 = item.X[Head2];
                        var S_Left = new List<double> { };
                        var S_Right = new List<double> { };

                        var Points_manyR_Level_RightX = new List<int> { };
                        var Points_manyR_Level_LeftX = new List<int> { };

                        var Points_manyR_Level_LeftY = new List<double> { };
                        var Points_manyR_Level_RightY = new List<double> { };
                        var Points_manyR_Level_LeftY_Head = new List<double> { };
                        var Points_manyR_Level_RightY_Head = new List<double> { };
                        var FlagLeft = 0;
                        var flagright = 0;
                        for (int iii = 2; iii < 4; iii++)

                        {


                            var SelectedDataLeftI = item.Y.Where(o => CurveMax * 0.2 * (iii + 1) > Math.Abs(o) && Math.Abs(o) > CurveMax * 0.2 * (iii)
                                                    && (item.X[item.Y.IndexOf(o)] < HeadX - 10)).ToList();



                            var sl = SelectedDataLeftI.Count;
                            if (sl > 50 && FlagLeft == 0)//

                            {
                                FlagLeft = 1;
                                var Head1I = item.Y.IndexOf(SelectedDataLeftI.First());

                                var HeadXI = item.X[Head1I];
                                var Head2I = item.Y.IndexOf(SelectedDataLeftI.Last());            //Y кордината второй(Последней(Last)) точки трапеции
                                var HeadX2I = item.X[Head2I];

                                Points_manyR_Level_LeftY_Head.Add(Head1I);
                                Points_manyR_Level_LeftY_Head.Add(Head2I);
                                Points_manyR_Level_LeftX.Add(HeadXI);
                                Points_manyR_Level_LeftX.Add(HeadX2I);
                                Points_manyR_Level_LeftY.Add(SelectedDataLeftI.Average());
                                Points_manyR_Level_LeftY.Add(SelectedDataLeftI.Average());
                            }
                            var SelectedDataRightI = item.Y.Where(o => CurveMax * 0.2 * (iii + 1) > Math.Abs(o) && Math.Abs(o) > CurveMax * 0.2 * (iii)
                                                        && (item.X[item.Y.IndexOf(o)] > HeadX2 + 10)).ToList();

                            var sr = SelectedDataRightI.Count;
                            if (sr > 50 && flagright == 0)// 
                            {
                                flagright = 1;

                                var Head1I = item.Y.IndexOf(SelectedDataRightI.First());
                                var HeadXI = item.X[item.Y.IndexOf(SelectedDataRightI.First())];
                                var Head2I = item.Y.IndexOf(SelectedDataRightI.Last());            //Y кордината второй(Последней(Last)) точки трапеции
                                var HeadX2I = item.X[Head2I];
                                Points_manyR_Level_RightY_Head.Add(Head1I);
                                Points_manyR_Level_RightY_Head.Add(Head2I);
                                Points_manyR_Level_RightX.Add(HeadXI);
                                Points_manyR_Level_RightX.Add(HeadX2I);
                                Points_manyR_Level_RightY.Add(SelectedDataRightI.Average());
                                Points_manyR_Level_RightY.Add(SelectedDataRightI.Average());
                                // Points_manyR_Level_RightY.Add(SelectedDataRightI.Average()) ;
                            }
                        }

                        var flagMR = 0;

                        var ff = flagMR;

                        var ff1 = flagMR;

                        if (Points_manyR_Level_LeftY.Count > 1)
                        {
                            Head1 = (int)Points_manyR_Level_LeftY_Head[0];
                            HeadX = (int)Points_manyR_Level_LeftX[0];

                            flagMR = 2;
                        }
                        if (Points_manyR_Level_RightY.Count > 1)
                        {
                            Head2 = (int)Points_manyR_Level_RightY_Head[1];
                            HeadX2 = (int)Points_manyR_Level_RightX[1];

                            flagMR = 2;
                        }

                        var H0 = 40;
                        /////////////////////////////////////
                        ///

                        if (Math.Abs(item.Y[Head1]) <= H0)
                        {

                            var Ss0_list = new List<double> { };
                            var S0_min_list = new List<double> { };
                            var S0_min_k_list = new List<double> { };
                            /////////////////////////////////
                            var k_start = 7;
                            for (int k = 0; k < 10; k++)
                            {
                                Ss0_list.Clear();
                                for (int ii = 0; ii < 1000; ii++)
                                {
                                    var s = 0.0;
                                    var y0 = item.Y[Head1];
                                    for (int m = 0; m < Head1; m++)
                                    {
                                        var y1 = (1.0 - 0.001 * k) * y0 + 0.005 * (ii - 500) * m;
                                        var y2 = (1.0 - 0.001 * k) * item.Y[Head1 - m];
                                        s += Math.Abs(y1 - y2);
                                    }
                                    Ss0_list.Add((int)s);
                                }
                                var testMin00 = Ss0_list.Min();
                                var testMinInd00 = Ss0_list.IndexOf(testMin00);
                                S0_min_list.Add((int)testMin00);
                                S0_min_k_list.Add(testMinInd00);
                            }
                            var testMin_min0 = (int)S0_min_list.Min();
                            var testMin_min_Ind0 = S0_min_list.IndexOf(testMin_min0);
                            var kof0 = S0_min_k_list[testMin_min_Ind0];
                            var vhod0 = -Math.Abs(item.Y[Head1] / (0.0049 * (kof0 - 500)));  // (2 - 0.1 * (testMin_min_Ind0 + k_start)) 
                            HeadX = (int)(HeadX + vhod0 < 0 ? 0 : HeadX + vhod0);




                        }










                        /////////////////////////










                        if (Math.Abs(item.Y[Head1]) > H0)
                        {
                            var M_luch = new List<double> { };
                            var Ss_list = new List<double> { };
                            var S_min_list = new List<double> { };
                            var S_min_k_list = new List<double> { };
                            int dp = 0;
                            var step = 0.0005;
                            var kof00 = 1.1 * Math.Abs(item.Y[Head1]) / Head1;
                            for (int k = dp; k < 2 * dp + 1; k++)
                            {
                                Ss_list.Clear();
                                for (int ii = 1; ii < 2000; ii++)
                                {
                                    var s = 0.0;
                                    var y0 = item.Y[Head1];
                                    var lambda = 0.95;
                                    double sd = 0;
                                    var y00 = Math.Abs(item.Y[Head1]);
                                    var maxd = 0;

                                    for (int m = 0; m < Head1; m++)
                                    {

                                        var y1 = 1.0 * (1 - 0.001 * k) * y00 - (kof00 + (ii - 1000) * step) * m;

                                        var y2 = (1 - 0.001 * k) * Math.Abs(item.Y[Head1 - m]);
                                        s += Math.Abs((y2 - y1)) * Math.Exp(-0.1 * Math.Abs(Head1 / 2 - m));
                                    }

                                    Ss_list.Add(s);

                                }
                                var testMin0 = Ss_list.Min();
                                var testMinInd0 = Ss_list.IndexOf(testMin0);
                                S_min_list.Add((int)testMin0);
                                S_min_k_list.Add(testMinInd0);
                            }

                            var testMin_min = (int)S_min_list.Min();
                            var testMin_min_Ind = S_min_list.IndexOf(testMin_min);
                            var popr1 = testMin_min_Ind - dp;
                            var kof = S_min_k_list[testMin_min_Ind];
                            var vhod = -Math.Abs(item.Y[Head1] / (kof00 + (kof - 1000) * step));
                            HeadX = (int)(HeadX + vhod < 0 ? 0 : HeadX + vhod);
                        }



                        if (Math.Abs(item.Y[Head2]) <= H0)
                        {


                            //----------------------------
                            var S2_list = new List<double> { };
                            var Ss2_list = new List<double> { };
                            var S2_min_list = new List<double> { };
                            var S2_min_k_list = new List<double> { };
                            var k_start = 7;
                            for (int k = k_start; k < 12; k++)
                            {

                                S2_list.Clear();
                                for (int ii = 0; ii < 1000; ii++)
                                {
                                    var s = 0.0;

                                    var lambda = 0.95;
                                    for (int m = 0; m < item.Y.Count() - Head2; m++)
                                    {
                                        var y00 = Math.Abs(item.Y[Head2]);
                                        var y1 = 1.0 * (1 - 0.001 * k) * y00 + 0.0005 * (ii - 1000) * m;
                                        var y2 = (1 - 0.001 * k) * Math.Abs(item.Y[Head2 + m]);

                                        if (Math.Abs(y1) > (1 - lambda) * Math.Abs((1 - 0.001 * k) * y00))
                                        {
                                            s += y2;
                                        }
                                        else
                                        { break; }
                                    }
                                    var s_S = (1 - 0.001 * k) * (1 - 0.001 * k) * lambda * lambda * 0.5 * Math.Abs(item.Y[Head2] * item.Y[Head2] / (0.0005 * (ii - 1000)));
                                    var Sdelta = (1 - lambda) * (1 - 0.001 * k) * Math.Abs(lambda * item.Y[Head2]) * Math.Abs((1 - 0.001 * k) * lambda * item.Y[Head2] / (0.0005 * (ii - 1000)));
                                    S2_list.Add((int)Math.Sqrt(Math.Abs((s - Sdelta - s_S))));
                                }
                                var testMin2 = (S2_list.Min());
                                var testMinInd2 = S2_list.IndexOf(testMin2);
                                S2_min_list.Add(testMin2);
                                S2_min_k_list.Add(testMinInd2);
                            }
                            var testMin_min2 = (S2_min_list.Min());
                            var testMin_min_Ind2 = S2_min_list.IndexOf(testMin_min2);
                            var kof2 = S2_min_k_list[testMin_min_Ind2];

                            var vihod = Math.Abs(item.Y[Head2] / (0.00049 * (kof2 - 1000)));//(2 - 0.1 * (testMin_min_Ind2+ k_start)) *
                            var y02 = item.Y[Head2];
                            HeadX2 = (int)(HeadX2 + vihod > result.Count() ? result.Count() - 1 : HeadX2 + vihod);
                        }



                        if (Math.Abs(item.Y[Head2]) > H0)
                        {
                            var M_luch = new List<double> { };
                            var Ss_list = new List<double> { };
                            var S_min_list = new List<double> { };
                            var S_min_k_list = new List<double> { };
                            int dp = 0;
                            var step = 0.0005;
                            var kof00 = 1.0 * Math.Abs(item.Y[Head2]) / (item.Y.Count() - Head2);
                            for (int k = dp; k < 2 * dp + 1; k++)
                            {
                                Ss_list.Clear();
                                var sr = (item.Y.Count() - Head2) / 2;
                                for (int ii = 1; ii < 20100; ii++)
                                {
                                    var s = 0.0;
                                    var y0 = Math.Abs(item.Y[Head2]);
                                    var lambda = 0.95;
                                    double sd = 0;
                                    var y00 = Math.Abs(item.Y[Head2]);
                                    var maxd = 0;

                                    for (int m = 3; m < item.Y.Count() - Head2 - 3; m++)
                                    {
                                        var y1 = 1.0 * (1 - 0.001 * k) * y00 - (kof00 + (ii - 1000) * step) * m;
                                        //if ( y1>0)
                                        //    {
                                        // var y1 = 1.0 * (1 - 0.001 * k) * y00 - (kof00 + (ii - 1000) * step) * m;

                                        var y2 = (1 - 0.001 * k) * Math.Abs(item.Y[Head2 + m]);
                                        s += Math.Abs((y2 - y1)) * Math.Exp(-0.21 * Math.Abs(sr - m) - 0.21 * Math.Abs(sr - m + 3) - 0.21 * Math.Abs(sr - m - 3));
                                        //s += Math.Abs(y2 - y1);
                                        //}
                                    }

                                    Ss_list.Add(Math.Abs(s));

                                }
                                var testMin0 = Ss_list.Min();
                                var testMinInd0 = Ss_list.IndexOf(testMin0);
                                S_min_list.Add((int)testMin0);
                                S_min_k_list.Add(testMinInd0);
                            }

                            var testMin_min = (int)S_min_list.Min();
                            var testMin_min_Ind = S_min_list.IndexOf(testMin_min);
                            var popr1 = testMin_min_Ind - dp;
                            var kof = S_min_k_list[testMin_min_Ind];
                            var vihod = Math.Abs(item.Y[Head2] / (kof00 + (kof - 1000) * step));
                            HeadX2 = (int)(HeadX2 + vihod > result.Count() ? result.Count() - 1 : HeadX2 + vihod);
                        }








                        var listY1 = new List<double>
                                      {
                            0.0,
                            avgSelD,
                                 avgSelD,
                                     0.0,};
                        //----------------------------------------------------------------
                        if (Points_manyR_Level_LeftY.Count > 1 && Points_manyR_Level_RightY.Count < 1)
                        {
                            listY1.Clear();
                            listY1.Add(0);

                            for (var io = 0; io < Points_manyR_Level_LeftY.Count; io++)
                            {
                                listY1.Add(Points_manyR_Level_LeftY[io]);
                            };
                            listY1.Add(SelectedData.Average());
                            listY1.Add(SelectedData.Average());
                            listY1.Add(0);
                        };

                        if (Points_manyR_Level_RightY.Count > 1 && Points_manyR_Level_LeftY.Count < 1)
                        {
                            listY1.Clear();
                            listY1.Add(0);
                            listY1.Add(SelectedData.Average());
                            listY1.Add(SelectedData.Average());


                            for (var io = 0; io < Points_manyR_Level_RightY.Count; io++)
                            {
                                listY1.Add(Points_manyR_Level_RightY[io]);
                            };

                            listY1.Add(0);
                        };


                        if (Points_manyR_Level_RightY.Count > 1 && Points_manyR_Level_LeftY.Count > 1)
                        {
                            listY1.Clear();
                            listY1.Add(0);
                            for (var io = 0; io < Points_manyR_Level_LeftY.Count; io++)
                            {
                                listY1.Add(Points_manyR_Level_LeftY[io]);
                            };
                            listY1.Add(SelectedData.Average());
                            listY1.Add(SelectedData.Average());


                            for (var io = 0; io < Points_manyR_Level_RightY.Count; io++)
                            {
                                listY1.Add(Points_manyR_Level_RightY[io]);
                            };

                            listY1.Add(0);
                        };





                        var HalfFirst = 0;  //x1
                        var HalflAST = 0;  //x2





                        var popr = 10;
                        var delta1 = 0;
                        var delta2 = 0;
                        try
                        {
                            if (Points_manyR_Level_LeftY.Count < 1 && Points_manyR_Level_RightY.Count < 1)//&& Math.Abs(item.Y[HeadX2])> 5
                            {
                                delta1 = Math.Abs(HeadX - item.X[item.Y.IndexOf(SelectedData.First())]);
                                delta2 = Math.Abs(HeadX2 - item.X[item.Y.IndexOf(SelectedData.Last())]);
                                //if (delta1 > 1.5 * delta2 && delta1 > 70 && delta2 > 70)
                                //    HeadX = item.X[item.Y.IndexOf(SelectedData.First())] - delta2;
                                //if (delta2 > 1.5 * delta1 && delta1 > 70 && delta2 > 70)
                                //    HeadX2 = item.X[item.Y.IndexOf(SelectedData.Last())] + delta1;
                                if (delta2 > 100 && (Math.Abs(SelectedData.Average()) < 30))
                                { HeadX2 = item.X[item.Y.IndexOf(SelectedData.Last())] + (int)(0.5 * delta2); }

                                if (delta1 > 110 && (Math.Abs(SelectedData.Average()) < 30))
                                { HeadX = item.X[item.Y.IndexOf(SelectedData.First())] - (int)(0.5 * delta1); }
                            }
                        }
                        catch { }

                        HalfFirst = popr;
                        HalflAST = -popr;
                        var listX1 = new List<int>
                        {
                            HeadX  ,
                           item.X[item.Y.IndexOf(SelectedData.First())] + popr,
                           item.X[item.Y.IndexOf(SelectedData.Last())] -popr,
                           HeadX2
                        };

                        //Данные для карточки кривой

                        item.LevelPoints.Add(HeadX - prev.Count());
                        item.LevelPoints.Add(item.X[item.Y.IndexOf(SelectedData.First())] + popr - prev.Count());
                        item.LevelPoints.Add(item.X[item.Y.IndexOf(SelectedData.Last())] - popr - prev.Count());
                        item.LevelPoints.Add(HeadX2 - prev.Count());


                        if (Points_manyR_Level_LeftY.Count > 1 && Points_manyR_Level_RightY.Count < 1)
                        {
                            listX1.Clear();
                            item.LevelPoints.Clear();

                            listX1.Add(HeadX);
                            item.LevelPoints.Add(HeadX - prev.Count());

                            for (var io = 0; io < Points_manyR_Level_LeftX.Count; io++)
                            {
                                listX1.Add(Points_manyR_Level_LeftX[io] + (io) * 4 + 4 * (-1 + io));
                                item.LevelPoints.Add(Points_manyR_Level_LeftX[io] + (io) * 4 + 4 * (-1 + io) - prev.Count());
                            };
                            listX1.Add(item.X[item.Y.IndexOf(SelectedData.First())] + popr + 10);
                            item.LevelPoints.Add(item.X[item.Y.IndexOf(SelectedData.First())] + popr + 10 - prev.Count());

                            listX1.Add(item.X[item.Y.IndexOf(SelectedData.Last())] - popr);
                            item.LevelPoints.Add(item.X[item.Y.IndexOf(SelectedData.Last())] - popr - prev.Count());

                            listX1.Add(HeadX2);
                            item.LevelPoints.Add(HeadX2 - prev.Count());
                        };

                        if (Points_manyR_Level_LeftY.Count < 1 && Points_manyR_Level_RightY.Count > 1)
                        {
                            listX1.Clear();
                            item.LevelPoints.Clear();

                            listX1.Add(HeadX);
                            item.LevelPoints.Add(HeadX - prev.Count());


                            listX1.Add(item.X[item.Y.IndexOf(SelectedData.First())] + popr);
                            item.LevelPoints.Add(item.X[item.Y.IndexOf(SelectedData.First())] + popr - prev.Count());

                            listX1.Add(item.X[item.Y.IndexOf(SelectedData.Last())] - 10);
                            item.LevelPoints.Add(item.X[item.Y.IndexOf(SelectedData.Last())] - 10 - prev.Count());

                            for (var io = 0; io < Points_manyR_Level_RightX.Count; io++)
                            {
                                listX1.Add(Points_manyR_Level_RightX[io] + (1 - io) * 10);
                                item.LevelPoints.Add(Points_manyR_Level_RightX[io] + (1 - io) * 10 - prev.Count());
                            };
                            listX1.Add(HeadX2);
                            item.LevelPoints.Add(HeadX2 - prev.Count());

                        };


                        if (Points_manyR_Level_LeftY.Count > 1 && Points_manyR_Level_RightY.Count > 1)
                        {
                            listX1.Clear();
                            item.LevelPoints.Clear();

                            listX1.Add(HeadX);
                            item.LevelPoints.Add(HeadX - prev.Count());

                            for (var io = 0; io < Points_manyR_Level_LeftX.Count; io++)
                            {
                                listX1.Add(Points_manyR_Level_LeftX[io] - (io) * 5);
                                item.LevelPoints.Add(Points_manyR_Level_LeftX[io] - (io) * 5 - prev.Count());
                            };
                            listX1.Add(item.X[item.Y.IndexOf(SelectedData.First())] + 5);
                            item.LevelPoints.Add(item.X[item.Y.IndexOf(SelectedData.First())] + 5 - prev.Count());

                            listX1.Add(item.X[item.Y.IndexOf(SelectedData.Last())]);
                            item.LevelPoints.Add(item.X[item.Y.IndexOf(SelectedData.Last())] - prev.Count());

                            //////////
                            ///

                            for (var io = 0; io < Points_manyR_Level_RightX.Count; io++)
                            {
                                listX1.Add(Points_manyR_Level_RightX[io] + (1 - io) * 10);
                                item.LevelPoints.Add(Points_manyR_Level_RightX[io] + (1 - io) * 10 - prev.Count());
                            };
                            //////////////


                            listX1.Add(HeadX2);
                            item.LevelPoints.Add(HeadX2 - prev.Count());
                        };

                        item.Y.Clear();
                        item.Y.AddRange(result.GetRange(item.LevelPoints.First() + prev.Count(), item.LevelPoints.Last() - item.LevelPoints.First() + 1));

                        //интерполяция кривой
                        var ymax = Math.Abs(listY1.Max());
                        var ymin = Math.Abs(listY1.Min());
                        ymax = Math.Max(ymax, ymin);
                        //if (ymax < 20) { continue; }  после нее изчазают много кривых осторожно!
                        for (int t = 0; t < listY1.Count() - 1; t++)
                        {
                            for (int c = 0; c < listX1[t + 1] - listX1[t]; c++)
                            {
                                double bottom_dx1 = listX1[t + 1] - listX1[t];

                                double y2 = listY1[t + 1];

                                double y1 = listY1[t];
                                linearY = (y2 - y1) / bottom_dx1 * c + y1;
                                if ((linearY * y2 < -1) || (linearY * y1 < -1))
                                    linearY = 0;
                                ModifiedCurve.Add(linearY);
                            }
                        }

                        var InternalIndex = 0;
                        for (int index = HeadX; index < HeadX2; index++)
                        {
                            //if (index < ModifiedCurve.Count)
                            {
                                ZeroData[index] = ModifiedCurve[InternalIndex];
                                InternalIndex++;
                            }
                        }
                        //for (int index = item.X.First() - 15 > 0 ? item.X.First() - 15 : item.X.First(); index < item.X.First(); index++)
                        //{
                        // result[index] = 0.0;
                        //}
                        //for (int index = item.X.Last(); index < (item.X.Last() + 15 <= result.Count() ? item.X.Last() + 15 : item.X.Last()); index++)
                        //{
                        // result[index] = 0.0;
                        //}
                    }
                    result = ZeroData;
                }

                if (ResultTestCurve.Any())
                {
                    var aaa = ResultTestCurve.Where(o => o.RealY.Count > 20).ToList();
                    var bbb = aaa.Where(o => o.RealY.Select(o => Math.Abs(o)).Max() >= 35).ToList();
                    var ttt = bbb.Where(o => o.RealY.Count < 100).ToList();
                    if (height == 4)
                    {
                        //  находеждение  закрестовииных кривых
                        foreach (var item in ttt)
                        {
                            // var localInd = 0;
                            // for (int q = item.X.First(); q < item.X.Last(); q++)
                            // {
                            //result[q] = item.Y[localInd] * 1.25;
                            var localInd = item.X.First();
                            // localInd++;
                            // var Upselected
                            var delta = +12;
                            var CurveMax = item.RealY.Max(o => Math.Abs(o));
                            var SelectedDataUp = item.RealY.Where(o => CurveMax * 0.9 > Math.Abs(o));
                            // var Head1 = SelectedDataUp.First();
                            // var Head2 = SelectedDataUp.Last();


                            var Head1 = item.RealY.IndexOf(SelectedDataUp.First());
                            var HeadX = item.X[item.RealY.IndexOf(SelectedDataUp.First())];

                            var Head2 = item.RealY.IndexOf(SelectedDataUp.Last());
                            var HeadX2 = item.X[Head2];

                            var xx1 = item.X.First();
                            var xx4 = item.X.Last();

                            var xx3 = HeadX2 - delta;
                            var y = item.RealY.Last();
                            var xx2 = HeadX + delta;
                            var xx5 = xx4;
                            var yy1 = 0;
                            var yy0 = 0;
                            var yy2 = (item.Y.Average() > 0 ? 0.9 * item.RealY.Max() : 0.9 * item.RealY.Min());
                            var yy3 = yy2;
                            var yy4 = 0;
                            var yy5 = 0;

                            var listX1 = new List<int>
                        {//                  (int)xx0  ,
                           (int)xx1  ,
                          (int) xx2,
                          (int)xx3,
                         (int) xx4,
                         (int) xx5
                        };

                            var listY1 = new List<double>
                        {// yy0,
                            yy1 ,
                           yy2,
                          yy3,
                       yy4,
                          yy5
                        };

                            for (int t = 0; t < listY1.Count() - 1; t++)
                            {
                                for (int c = 0; c < listX1[t + 1] - listX1[t]; c++)
                                {
                                    double bottom_dx1 = listX1[t + 1] - listX1[t];

                                    double y2 = listY1[t + 1];

                                    double y1 = listY1[t];
                                    linearY = (y2 - y1) / bottom_dx1 * c + y1;
                                    if ((linearY * y2 < -1) || (linearY * y1 < -1))
                                        linearY = 0;
                                    result[localInd] = linearY;
                                    localInd++;
                                    // localInd++;
                                    //ModifiedCurve.Add(linearY);
                                }
                            }

                            //}


                        }


                    }
                }

                /////////////
                ///
                //var listX1 = new List<int>
                //        {
                //            HeadX+(int)HalfFirst ,
                //           item.X[item.Y.IndexOf(SelectedData.First())] + popr,
                //           item.X[item.Y.IndexOf(SelectedData.Last())] - popr,
                //           HeadX2+(int)HalflAST
                //        };








                if (naprav == Direction.Reverse)
                {
                    result.Reverse();
                }

                ////басын аяғын қиып аламыз
                var cropped = result.GetRange(prev.Count == 0 ? 0 : prev.Count - 1, avg.Count - 1);
                //var cropped = originalDbPoints.GetRange(prev.Count == 0 ? 0 : prev.Count - 1, avg.Count - 1);

                if (getPairs)
                    return transitions;
                return cropped;
            }

            catch (Exception e)
            {
                System.Console.WriteLine(e.Message);
            }

            //if (ResultTestCurve.Any())
            //{
            //    var ttt = ResultTestCurve.Where(o => o.Y.Select(o => Math.Abs(o)).Max() >= 35).ToList();

            //    if (height == 4)
            //    {
            //        if (naprav == Direction.Reverse)
            //        {
            //            item.Y.Reverse();
            //        }

            //        foreach (var item in ttt)
            //        {
            //            if (naprav == Direction.Reverse)
            //            {
            //                item.Y.Reverse();
            //            }

            //            var localInd = 0;
            //            for (int q = item.X.First(); q < item.X.Last(); q++)
            //            {
            //                var dopMeter = (next.Count == 0 ? 0 : next.Count - 1);
            //                var val = item.Y[localInd] * 1.3;
            //                avg[q - dopMeter * (naprav == Direction.Reverse ? -1 : 1)] = val;
            //                localInd++;
            //            }
            //        }

            //    }
            //}

            return ZeroDataStright;
        }

        public static double GetGapKoef()
        {
            return 0.68;
        }

        public static double Radian(this double angle)
        {
            return (angle / 180.0) * Math.PI;
        }

        public static int MeterToPicket(this int meter)
        {
            return (meter % 100 == 0 && meter != 0) ? meter / 100 : (meter % 100 != 0 ? meter / 100 + 1 : 1);
        }
        public static double ToRadians(this double angleIn10thofaDegree)
        {
            // Angle in 10th of a degree
            return Math.Tan((angleIn10thofaDegree * Math.PI) / 1800);
        }

        public static double RadianToAngle(this float radian)
        {
            return radian * 180.0 / Math.PI;
        }


    }
    public enum SocketState
    {
        Abortively = -1,
        Success = 0
    }

    public enum VagonType
    {
        Sapsan = 0,
        Lastochka = 1,
        Strizh = 2,
        Allegro = 3,
        PassengerAndFreight = 4,
    }
    class ResultTestCurve
    {
        public List<double> RealY = new List<double> { };
        public List<double> Y = new List<double> { };
        public List<int> X = new List<int> { };
    }

    public class NatureCurves
    {
        public List<double> Y = new List<double> { };
        public List<int> X = new List<int> { };
        public bool Added = false;

        public double lvl_center_value = -1;
        public int lvl_center_index = -1;


        public List<int> LevelPoints = new List<int> { };
        public List<int> StrPoints = new List<int> { };

    }

    public class ThresholdsAcceleration
    {
        //АНП, близкое к допустимому, м/с2
        public double AnpBliskoeKDopusku = 0;
        //АНП для ограничения скорости, м/с2
        public double AnpOgrSkor = 0;
        //АГ для ограничения скорости, м/с2
        public double AgOgrSkor = 0;
        //ψ
        public double Psi = 0.6;
    }
}