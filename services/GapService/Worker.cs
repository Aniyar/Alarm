using ALARm.Core;
using ALARm.Core.AdditionalParameteres;
using ALARm.Core.Report;
using ALARm.DataAccess;
using ALARm.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Npgsql;
using Dapper;
using System.IO;
//using ALARm_Report.controls;

namespace GapService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private ConnectionFactory _connectionFactory;
        private IConnection _connection;
        private IModel _channel;
        private string QueueName = "";
        public int tryCount = 0;
        public IMainTrackStructureRepository MainTrackStructureRepository;

        public Worker(ILogger<Worker> logger, IOptions<RabbitMQConfiguration> options)
        {
            _logger = logger;
            QueueName = options.Value.Queue;

            _connectionFactory = new ConnectionFactory
            {
                HostName = options.Value.Host,

                UserName = options.Value.Username,
                Password = options.Value.Password,
                VirtualHost = "/",
                Port = options.Value.Port,
            };
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            //_logger.LogInformation(Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json"));
            //try
            //{

            //    var blazor = new Blazor_ProfileData();
            //    blazor.conn = new NpgsqlConnection(Helper.ConnectionString());
            //    blazor.conn.Open();
            //    _logger.LogInformation("база ашылды");
            //}
            //catch (Exception e)
            //{
            //    _logger.LogInformation("baseconnect:" + e.Message);
            //}

            try
            {
                _logger.LogInformation($"Connection try [{tryCount++}].");
                _connection = _connectionFactory.CreateConnection();
                _channel = _connection.CreateModel();
                //_channel.QueueDeclarePassive(QueueName);
                _channel.QueueDeclare(queue: QueueName,
                                    durable: false,
                                    exclusive: false,
                                    autoDelete: false,
                                    arguments: null);
                _channel.QueueBind(queue: QueueName,
                                   exchange: "alarm",
                                   routingKey: "");
                _channel.BasicQos(0, 1, false);
                _logger.LogInformation($"Queue [{QueueName}] is waiting for messages.");
                var Blazor = new Blazor_ProfileData();
               
                //////Выбор километров по проезду-----------------
                //var filterForm = new FilterForm();
                //var filters = new List<Filter>();

                //filters.Add(new FloatFilter() { Name = "Начало (км)", Value = lkm.Min() });
                //filters.Add(new FloatFilter() { Name = "Конец (км)", Value = lkm.Max() });

                //filterForm.SetDataSource(filters);
                //if (filterForm.ShowDialog() == DialogResult.Cancel)
                //    return;

                //lkm = lkm.Where(o => ((float)(float)filters[0].Value <= o && o <= (float)(float)filters[1].Value)).ToList();


                var consumer = new EventingBasicConsumer(_channel);
                consumer.Received += async (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    message = message.Replace("\\", "\\\\");
                    _logger.LogInformation(" [x] Received {0}", message);

                    JObject json = JObject.Parse(message);
                    var kmIndex = (int)json["Km"];
                    var kmId = (int)json["FileId"];
                    //var path = json["Path"];
                    // {'FileId':18308, 'Km':707, 'Path': '\DESKTOP-EMAFC5J\common\video_objects\desktop\242_18308_km_707.csv'}
                    Trips trip = RdStructureService.GetTripFromFileId(kmId)[0];
                    int TripId = (int)trip.Id;
                    var kilometers = RdStructureService.GetKilometersByTrip(trip);
                    Kilometer km = kilometers.Where(km => km.Number == kmIndex).First();


                    // Очищает таблицы для сервисов
                    //ClearServiceTables(TripId);
                    //Читает файлы проезда и обновляет номер километра в таблице трип файлс
                    //PutKilometers(TripId);



                    //var kmFinal = (int)json["KmFinal"];

                    //{
                    //    "TripId":242,
                    //    "DistId":45,
                    //    "KmIndex":700
                    //}



                    //var queruString = string.Join(",", fileId);

                    //if (queruString.Length == 0) 
                    //    return;

                    //var trip = RdStructureService.GetTripFromFileId((int)fileId.First()).Last();
                    //var kilometers = RdStructureService.GetKilometersByTrip(trip);
                    //var km = kilometers.Where(km => km.Number == kmIndex).ToList().First();
                    //this.MainTrackStructureRepository = MainTrackStructureService.GetRepository();

                    //var outData = (List<OutData>)RdStructureService.GetNextOutDatas(km.Start_Index - 1, km.GetLength() - 1, trip.Id);
                    //km.AddDataRange(outData, km);
                    //km.LoadTrackPasport(MainTrackStructureRepository, trip.Trip_date);

                    ////Видеоконтроль
                    //// todo distanse id
                    //string p = GetGaps(trip, km, 53, queruString); //стыки


                    this.MainTrackStructureRepository = MainTrackStructureService.GetRepository();


                    var outData = (List<OutData>)RdStructureService.GetNextOutDatas(km.Start_Index - 1, km.GetLength() - 1, TripId);
                    km.AddDataRange(outData, km);

                    km.LoadTrackPasport(MainTrackStructureRepository, trip.Trip_date);

                    //var Blazor2 = new Blazor_TestData();
                    //try
                    //{
                    //    bool flag = true;
                    //    string koridorfile = RdStructureService.GetTripFiles(km.Number, TripId, "ProfilPoverxKoridor");
                    //    string kupefile = RdStructureService.GetTripFiles(km.Number, TripId, "ProfilPoverxKupe");
                    //    while (flag)
                    //    {
                    //        flag = Blazor2.GetBitmapAsync(koridorfile, kupefile);
                    //        Blazor2.CurrentFrameIndex++;
                    //    }
                    //    Console.WriteLine("тест дата ОК");
                    //}
                    //catch (Exception e)
                    //{
                    //    Console.WriteLine("тест дата ERROR! " + e.Message);
                    //}

                    _logger.LogInformation("");
                    var Blazor = new Blazor_ProfileData();
                    Blazor.conn = new NpgsqlConnection(Helper.ConnectionString());
                    try
                    {
                        Blazor.conn.Open();
                        _logger.LogInformation("база ашылды");
                    } catch(Exception e)
                    {
                        _logger.LogInformation(e.Message);
                    }
                    Blazor.in_koridor = new BinaryReader(File.Open(Blazor.Vnutr__profil__koridor, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
                    var data = Blazor.in_koridor.ReadBytes(8);
                    Blazor.in_koridor_count = BitConverter.ToSingle(data, 0);
                    Blazor.in_koridor_size = BitConverter.ToSingle(data, 4);

                    Blazor.in_kupe = new BinaryReader(File.Open(Blazor.Vnutr__profil__kupe, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
                    data = Blazor.in_kupe.ReadBytes(8);
                    Blazor.in_kupe_count = BitConverter.ToSingle(data, 0);
                    Blazor.in_kupe_size = BitConverter.ToSingle(data, 4);
                    
                    try
                    {
                        bool flag = true;
                        while (flag)
                        {
                            flag = Blazor.GetBitmapAsync(km.Number, TripId);
                        }
                        _logger.LogInformation("профайл дата ОК");
                    }
                    catch (Exception e)
                    {
                        _logger.LogInformation("профайл дата ERROR! " + e.Message);
                    }

                    //var Blazor3 = new Blazor3(); //poverh shpal
                    //try
                    //{
                    //    bool flag = true;
                    //    string koridorfile = RdStructureService.GetTripFiles(km.Number, TripId, "ProfilSHpalyKoridor");
                    //    string kupefile = RdStructureService.GetTripFiles(km.Number, TripId, "ProfilSHpalyKupe");
                    //    while (flag)
                    //    {
                    //        flag = Blazor3.GetBitmapAsync(koridorfile, kupefile);
                    //    }
                    //    Console.WriteLine("Поверх шпал дата ОК");
                    //}
                    //catch (Exception e)
                    //{
                    //    Console.WriteLine("Поверх шпал ERROR! " + e.Message);
                    //}

                    try
                    {
                        GetTestData(km.Number, trip.Id); //волны и импульсы
                        Console.WriteLine("волны ОК!");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("волны ERROR! " + e.Message);
                    }

                    try
                    {
                        GetCrossAdditional(trip, km); //стыки
                        Console.WriteLine("Поверхность рельса ОК!");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Поверхность рельса ERROR! " + e.Message);
                    }

                    try
                    {
                        GetBolt(trip, km); //стыки
                        Console.WriteLine("болт ок!");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("болт error! " + e.Message);
                    }
                    try
                    {
                        GetBalast(trip, km); //баласт
                        Console.WriteLine("Баласт ОК!");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Баласт ERROR! " + e.Message);
                    }

                    try
                    {
                        GetPerpen(trip, km);
                        Console.WriteLine("Perpen ОК!");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Perpen ERROR! " + e.Message);
                    }

                    try
                    {
                        GetSleepers(trip, km);
                        Console.WriteLine("Шпалы ОК!");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Шпалы ERROR! " + e.Message);
                    }

                    try
                    {
                        GetdeviationsinSleepers(trip, km); //Огр шпалы
                        Console.WriteLine("Огр шпалы ОК!");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Огр шпалы ERROR! " + e.Message);
                    }

                    try
                    {
                        Getbadfasteners(trip, km); //Скрепление
                        Console.WriteLine("Скрепление ОК!");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Скрепление ERROR! " + e.Message);
                    }
                    try
                    {
                        Getdeviationsinfastening(trip, km); //огр в скреп
                        Console.WriteLine("Огр скор Скрепление ОК!");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Огр скор Скрепление ERROR! " + e.Message);
                    }
                    //}
                };
                _channel.BasicConsume(queue: QueueName,
                                      autoAck: true,
                                      consumer: consumer);

                return base.StartAsync(cancellationToken);
            }
            catch(Exception e)
            {
                StartAsync(cancellationToken);
                return base.StartAsync(cancellationToken);
            }
        }

        /// <summary>
        /// Сервис по доппараметрам
        /// </summary>
        /// <param name="trip"></param>
        /// <param name="km"></param>
        /// <param name="distId"></param>


        private void PutKilometers(int trip_id)
        {
            var con = new NpgsqlConnection(Helper.ConnectionString());
            con.Open();
            List<string> files = con.Query<string>($@"SELECT file_name as filename FROM trip_files WHERE trip_id={trip_id}").ToList();
            foreach (string file in files)
            {

                using (BinaryReader reader = new BinaryReader(File.Open(file, FileMode.Open)))
                {
                    try
                    {
                        int width = reader.ReadInt32();
                        int height = reader.ReadInt32();
                        int frameSize = width * height;
                        reader.BaseStream.Seek(8, SeekOrigin.Begin);
                        byte[] by = reader.ReadBytes(frameSize);
                        int km = BitConverter.ToInt32(by.Skip(40).Take(4).ToArray(), 0);
                            
                        var cmd = new NpgsqlCommand();
                        cmd.Connection = con;
                        cmd.CommandText = $@"UPDATE trip_files SET km_num={km} WHERE file_name='{file}'";
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Не получилось прочитать файл ", file, e.Message);
                    }
                }
                
            }
        }

        private void ClearServiceTables(int trip_id)
        {
            //"testdata_242", "profiledata_242"
            string[] tables = {
                "report_badfasteners",
                "report_bolts",
                "report_defshpal",
                "report_deviationsinballast",
                "report_deviationsinrails",
                "report_fasteningunderrailsole",
                "report_gaps",
                "report_violperpen",
                "s3_additional"
            };

            var con = new NpgsqlConnection(Helper.ConnectionString());
            con.Open();

            var cmd = new NpgsqlCommand();
            cmd.Connection = con;
            foreach (string tablename in tables)
            {
                try
                {
                    var qrStr = $@"DELETE FROM {tablename} WHERE trip_id = {trip_id};";
                    cmd.CommandText = qrStr;
                    cmd.ExecuteNonQuery();
                }
                catch (Exception e)
                {
                    Console.WriteLine("Ошибка записи в БД " + e.Message);
                }
            }
            

        }
        private string GetTestData(int number, long trip_id)
        {
            try
            {
                List<int> Meters = new List<int>();
                List<double> ShortWavesLeft = new List<double>();
                List<double> ShortWavesRight = new List<double>();
                List<double> MediumWavesLeft = new List<double>();
                List<double> MediumWavesRight = new List<double>();
                List<double> LongWavesLeft = new List<double>();
                List<double> LongWavesRight = new List<double>();
                List<double> LongWavesLeft_m = new List<double>();
                List<double> MediumWavesLeft_m = new List<double>();
                List<double> ShortWavesLeft_m = new List<double>();
                List<double> LongWavesRight_m = new List<double>();
                List<double> MediumWavesRight_m = new List<double>();
                List<double> ShortWavesRight_m = new List<double>();
                List<Digression> Impuls = new List<Digression>();
                List<Digression> ImpulsRight = new List<Digression>();
                List<Digression> ImpulsLeft = new List<Digression>();
                List<int> METERS_long_M = new List<int>();
                var Dr = new List<double> { };
                var Dl = new List<double> { };
                var Count = 0;
                for (int i = 0; i < 5; i++)
                {
                    Dr.Add(0.0);
                    Dl.Add(0.0);
                }

                var ShortData = AdditionalParametersService.GetShortRough(trip_id, number);
                var shortl = ShortData.Select(o => o.Diff_l / 8.0 < 1 / 8.0 ? 0 : o.Diff_l / 10.0).ToList();
                var shortr = ShortData.Select(o => o.Diff_r / 8.0 < 1 / 8.0 ? 0 : o.Diff_r / 10.0).ToList();
                var flag = true;
                var short_meter = ShortData.Select(o => o.Meter).ToList();
                Meters.AddRange(ShortData.Select(o => o.Meter).ToList());

                var val = new List<double> { };
                var val_ind = new List<int> { };
                var bolshe0 = new List<DATA0> { };
                var inn = false;

                //left
                for (int i = 0; i < shortl.Count() - 1; i++)
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
                for (int i = 0; i < shortr.Count() - 1; i++)
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

                for (int j = 0; j < shortl.Count(); j++)
                {
                    var m = 0.0;
                    var l = 0.0;
                    var s = 0.0;

                    var mr = 0.0;
                    var lr = 0.0;
                    var sr = 0.0;

                    for (int i = 0; i < 4; i++)
                    {
                        Dr[i] = Dr[i + 1];
                        Dl[i] = Dl[i + 1];

                    }
                    Dr[4] = shortr[j];
                    Dl[4] = shortl[j];
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


                    // Коэфиценты увеличены с 0.15 до 1.15
                    var koef_long = 0.15;
                    var koef_medium = 0.15;
                    var koef_short = 0.15;

                    LongWavesLeft.Add(l * koef_long);
                    MediumWavesLeft.Add(m * koef_medium);
                    ShortWavesLeft.Add(s * koef_short);

                    LongWavesRight.Add(lr * koef_long);
                    MediumWavesRight.Add(mr * koef_medium);
                    ShortWavesRight.Add(sr * koef_short);


                    Count += Count;
                    if (j / 5 == j / 5.0 || (Math.Abs(short_meter[j] - short_meter[j - 1]) > 1 && j > 1) && Count < 5)
                    {

                        Count = 0;
                        LongWavesLeft_m.Add(l * koef_long);
                        MediumWavesLeft_m.Add(m * koef_medium);
                        ShortWavesLeft_m.Add(s * koef_short);

                        LongWavesRight_m.Add(lr * koef_long);
                        MediumWavesRight_m.Add(mr * koef_medium);
                        ShortWavesRight_m.Add(sr * koef_short);

                        METERS_long_M.Add(short_meter[j]);
                    }



                }
                //импульсы
                for (int i = 0; i < bolshe0.Count; i++)
                {
                    if (bolshe0[i].H < 0.8)
                    {
                        ImpulsLeft.Add(new Digression
                        {
                            Km = number,
                            Length = 0,
                            Len = 0,
                            Intensity_ra = 0,
                            Meter = Meters[bolshe0[i].H_ind],
                            Threat = Threat.Left
                        });
                    }
                    else
                    {
                        ImpulsLeft.Add(new Digression
                        {
                            Km = number,
                            Length = (int)bolshe0[i].Count,
                            Len = (int)bolshe0[i].Count,
                            Intensity_ra = bolshe0[i].H,
                            Meter = Meters[bolshe0[i].H_ind],
                            Threat = Threat.Left
                        });
                    }


                }
                for (int i = 0; i < bolshe0_r.Count; i++)
                {
                    if (bolshe0_r[i].H < 0.6)
                    {
                        ImpulsRight.Add(new Digression
                        {
                            Km = number,
                            Length = 0,
                            Len = 0,
                            Intensity_ra = 0,
                            Meter = Meters[bolshe0_r[i].H_ind],
                            Threat = Threat.Right
                        });
                    }
                    else
                    {
                        ImpulsRight.Add(new Digression
                        {
                            Km = number,
                            Length = (int)bolshe0_r[i].Count,
                            Len = (int)bolshe0_r[i].Count,
                            Intensity_ra = bolshe0_r[i].H,
                            Meter = Meters[bolshe0_r[i].H_ind],
                            Threat = Threat.Right
                        });
                    }

                }

                var cs = "Host=DESKTOP-EMAFC5J;Username=postgres;Password=alhafizu;Database=Aniyar_COpy";

                var con = new NpgsqlConnection(cs);
                con.Open();

                var cmd = new NpgsqlCommand();
                cmd.Connection = con;
                //WAVES
                try
                {
                    for (int i = 0; i < METERS_long_M.Count; i++)
                    {
                        var qrStr = $@"UPDATE  profiledata_{trip_id}
                                   SET  longwavesleft = {(LongWavesLeft_m[i]).ToString("0.0000").Replace(",", ".")},
                                   mediumwavesleft =  {(MediumWavesLeft_m[i]).ToString("0.0000").Replace(",", ".")},shortwavesleft = {(ShortWavesLeft_m[i]).ToString("0.0000").Replace(",", ".")},
                                   longwavesright =   {(LongWavesRight_m[i]).ToString("0.0000").Replace(",", ".")},mediumwavesright =  {(MediumWavesRight_m[i]).ToString("0.0000").Replace(",", ".")}
                                 ,shortwavesright = {(ShortWavesRight_m[i]).ToString("0.0000").Replace(",", ".")}
                                   where km = {number} and meter = {METERS_long_M[i]}";

                        cmd.CommandText = qrStr;
                        cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Ошибка записи в БД " + e.Message);
                }

                //IMPULS
                try
                {
                    for (int i = 0; i < ImpulsLeft.Count; i++)
                    {
                        var qrStr = $@"UPDATE  profiledata_{trip_id}
                                   SET   imp_left ={(ImpulsLeft[i].Intensity_ra).ToString("0.0000").Replace(",", ".")},
                                   implen_left = {ImpulsLeft[i].Length},impthreat_left = '{ImpulsLeft[i].Threat}'
                                   where km = {ImpulsLeft[i].Km} and meter = {ImpulsLeft[i].Meter}";
                        cmd.CommandText = qrStr;
                        cmd.ExecuteNonQuery();
                    }
                    for (int i = 0; i < ImpulsRight.Count; i++)
                    {
                        var qrStr = $@"UPDATE  profiledata_{trip_id}
                                   SET   imp_right ={(ImpulsRight[i].Intensity_ra).ToString("0.0000").Replace(",", ".")},
                                   implen_right = {ImpulsRight[i].Length},impthreat_right = '{ImpulsRight[i].Threat}'
                                   where km = {ImpulsRight[i].Km} and meter = {ImpulsRight[i].Meter}";
                        cmd.CommandText = qrStr;
                        cmd.ExecuteNonQuery();
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Ошибка записи в БД " + e.Message);
                }


                var shortRoughness = new ShortRoughness { };

                shortRoughness.ShortWaveRight.AddRange(ShortWavesRight.Select(o => (float)o).ToList());
                shortRoughness.MediumWaveRight.AddRange(MediumWavesRight.Select(o => (float)o).ToList());
                shortRoughness.LongWaveRight.AddRange(LongWavesRight.Select(o => (float)o).ToList());

                shortRoughness.ShortWaveLeft.AddRange(ShortWavesLeft.Select(o => (float)o).ToList());
                shortRoughness.MediumWaveLeft.AddRange(MediumWavesLeft.Select(o => (float)o).ToList());
                shortRoughness.LongWaveLeft.AddRange(LongWavesLeft.Select(o => (float)o).ToList());

                shortRoughness.MetersLeft.AddRange(Meters);
                shortRoughness.MetersRight.AddRange(Meters);
                List<Digression> addDigressions = shortRoughness.GetDigressions_new(number);
                AdditionalParametersService.Insert_additional_param_state_aslan(addDigressions, trip_id);

                con.Close();
                return "Success";



            }
            catch (Exception e)
            {
                return "Error " + e.Message;
            }
        }



        /// <summary>
        /// Сервис по стыкам
        /// </summary>
        /// <param name="trip"></param>
        /// <param name="km"></param>
        /// <param name="distId"></param>
        private string GetCrossAdditional(Trips trip, Kilometer km)
        {
            try
            {
                var mainProcess = new MainParametersProcess { Trip_id = trip.Id };
                //данные
                var DBcrossRailProfile = AdditionalParametersService.GetCrossRailProfileFromDBbyKm(km.Number, trip.Id);
                if (DBcrossRailProfile == null) return "NULL CrossRailProfile";

                var sortedData = DBcrossRailProfile.OrderByDescending(d => d.Meter).ToList();
                var crossRailProfile = AdditionalParametersService.GetCrossRailProfileFromDBParse(sortedData);
                List<Digression> addDigressions = crossRailProfile.GetDigressions();
                AdditionalParametersService.Insert_additional_param_state(addDigressions, km.Number);
                return "Success";
            }
            catch (Exception e)
            {
                return "Error " + e.Message;
            }
        }


        /// <summary>
        /// Сервис по стыкам
        /// </summary>
        /// <param name="trip"></param>
        /// <param name="km"></param>
        /// <param name="distId"></param>
        private string GetBalast(Trips trip, Kilometer km, string query = "")
        {
            try
            {
                var mainProcess = new MainParametersProcess { Trip_id = trip.Id };
                //var distance = AdmStructureService.GetUnit(AdmStructureConst.AdmDistance, distId) as AdmUnit;
                var trackName = AdmStructureService.GetTrackName(km.Track_id);
                var digressions = RdStructureService.GetBadRailFasteners(trip.Id, false, trackName);
                // if (badFasteners.Count == 0) continue;
                digressions = digressions.Where(o => o.Razn > 10 && o.Km > 128).ToList();
                var speed = new List<Speed>();
                RailFastener prev_fastener = null;
                foreach (var fastener in digressions)
                {
                    //string amount = (int)finddeg.Typ == 1025 ? finddeg.Length.ToString() + " шп.ящиков" : finddeg.Length.ToString() + "%";
                    //string meter = (int)finddeg.Typ == 1025 ? (finddeg.Meter).ToString() : "";
                    //string piket = (int)finddeg.Typ != 1026 ? (finddeg.Meter / 100 + 1).ToString() : "";
                    var sector = "";
                    var previousKm = -1;
                    // if (fastener == null) continue;
                    //if (fastener.Razn < 300) continue;

                    if ((prev_fastener == null) || (prev_fastener.Km != fastener.Km))
                    {
                        sector = MainTrackStructureService.GetSector(km.Track_id, fastener.Km, trip.Trip_date);
                        sector = sector == null ? "" : sector;
                        speed = MainTrackStructureService.GetMtoObjectsByCoord(trip.Trip_date, fastener.Km, MainTrackStructureConst.MtoSpeed, trip.Direction, trackName.ToString()) as List<Speed>;
                    }
                    fastener.PdbSection = km.PdbSection.Count > 0 ? $"ПЧУ-{km.PdbSection[0].Pchu}/ПД-{km.PdbSection[0].Pd}/ПДБ-{km.PdbSection[0].Pdb}" : "ПЧУ-/ПД-/ПДБ-";
                    fastener.Station = km.StationSection != null && km.StationSection.Count > 0 ?
                                      "Станция: " + km.StationSection[0].Station : (km.Sector != null ? km.Sector.ToString() : "");
                    prev_fastener = fastener;

                    //   fastener.Fastening = GetName(fastener.Digressions[0].DigName);
                    //fastener.Station = sector;
                    //fastener.Fragment = sector;
                    fastener.Otst = fastener.Digressions[0].GetName();
                    fastener.Threat_id = fastener.Threat == Threat.Left ? "левая" : "правая";
                }

                AdditionalParametersService.Insert_deviationsinballast(mainProcess.Trip_id, -1, digressions);
                return "Success";



            }
            catch (Exception e)
            {
                return "Error " + e.Message;
            }
        }





        /// <summary>
        /// Сервис по стыкам
        /// </summary>
        /// <param name="trip"></param>
        /// <param name="km"></param>
        /// <param name="distId"></param>
        private string GetGaps(Trips trip, Kilometer km, int distId, string query = "")
        {
            try
            {
                var mainProcess = new MainParametersProcess { Trip_id = trip.Id };
                var distance = AdmStructureService.GetUnit(AdmStructureConst.AdmDistance, distId) as AdmUnit;
                var trackName = AdmStructureService.GetTrackName(km.Track_id);

                var gaps = AdditionalParametersService.GetFullGapsByNN(km.Number, trip.Id);


                var pass_speed = km.PdbSection.Count > 0 ? km.Speeds.First().Passenger : -1;
                var fr_speed = km.PdbSection.Count > 0 ? km.Speeds.First().Freight : -1;
                //var pdb = km.PdbSection.Count > 0 ? km.PdbSection[0].ToString() : " ПЧ-/ПЧУ-/ПД-/ПДБ-";
                //var sector = km.StationSection != null && km.StationSection.Count > 0 ?
                //                "Станция: " + km.StationSection[0].Station : (km.Sector != null ? km.Sector.ToString() : "");
                var temperature = MainTrackStructureService.GetTemp(trip.Id, km.Track_id, km.Number);
                var temp = (temperature.Count != 0 ? temperature[0].Kupe.ToString() : " ") + "°";

                foreach (var gap in gaps)
                {
                    var pdb = km.PdbSection.Count > 0 ? km.PdbSection[0].ToString() : " ПЧ-/ПЧУ-/ПД-/ПДБ-";

                    var data = pdb.Split($"/").ToList();
                    if (data.Any())
                    {
                        pdb = $"{data[1]}/{data[2]}/{data[3]}";
                    }

                    var isStation = km.StationSection.Any() ?
                                    km.StationSection.Where(o => gap.Km + gap.Meter / 10000.0 >= o.RealStartCoordinate && o.RealFinalCoordinate >= gap.Km + gap.Meter / 10000.0).ToList() :
                                    new List<StationSection> { };

                    var sector1 = isStation.Any() ? "Станция: " + km.StationSection[0].Station : (km.Sector != null ? km.Sector.ToString() : "");

                    gap.Pdb_section = pdb;
                    gap.Fragment = sector1;
                    gap.temp = temp;

                    gap.PassSpeed = pass_speed;
                    gap.FreightSpeed = fr_speed;

                    if (gap.Zazor != -1 || gap.Zazor != 1)

                        gap.Zazor = (int)(gap.Zazor * 0.8);

                    //gap.Zazor = (int)(gap.Zazor );

                    gap.GetDigressions436();
                    //var gap_l = gaps.Where(o => o.Threat == Threat.Left).ToList();
                    //var gap_r = gaps.Where(o => o.Threat == Threat.Right).ToList();

                    //var r = gap_r.Where(o => o.Km == gap.Km && (o.Meter >= gap.Meter - 1 && o.Meter <= gap.Meter + 1)).ToList();
                    //if (gap_l.Any())
                    //{
                    //    //if (gap.Zazor == -1)
                    //    //{
                    //        double k = (double)gap.H / (double)r.First().H;
                    //        gap.Zazor = (int)(r.First().Zazor * k);
                    //        gap.GetDigressions436();
                    //        if (gap.DigName.Name.Equals("З?"))
                    //            gap.DigName.Name = "З?Л";
                    //        if (gap.DigName.Name.Equals("З"))
                    //            gap.DigName.Name = "ЗЛ";
                    //   // }
                    //    //if (r.First().Zazor == -1)
                    //    //{
                    //    //    double k = (double)r.First().H / (double)gap.H;
                    //    //    r.First().Zazor = (int)(gap.Zazor * k);
                    //    //    r.First().GetDigressions436();
                    //    //    if (gap.DigName.Equals("З?"))
                    //    //        gap.DigName.Name = "З?П";
                    //    //    if (gap.DigName.Equals("З"))
                    //    //        gap.DigName.Name = "ЗП";
                    //    //}

                    //}

                    //if (gap_r.Any())
                    //{


                    //    double k = (double)r.First().H / (double)gap.H;
                    //    r.First().Zazor = (int)(gap.Zazor * k);
                    //    r.First().GetDigressions436();
                    //    if (gap.DigName.Name.Equals("З?"))
                    //        gap.DigName.Name = "З?Л";
                    //    if (gap.DigName.Name.Equals("З"))
                    //        gap.DigName.Name = "ЗЛ";


                    //}

                }

                AdditionalParametersService.Insert_gap(mainProcess.Trip_id, -1, gaps);

                return "Success";
            }
            catch (Exception e)
            {
                return "Error " + e.Message;
            }
        }


        /// <summary>
        /// Сервис Негодных  Скреплений  
        /// </summary>
        /// <param name="trip"></param>
        /// <param name="km"></param>
        /// <param name="distId"></param>
        private void Getbadfasteners(Trips trip, Kilometer km)
        {
            var mainProcess = new MainParametersProcess { Trip_id = trip.Id };
            //var distance = AdmStructureService.GetUnit(AdmStructureConst.AdmDistance, distId) as AdmUnit;
            var trackName = AdmStructureService.GetTrackName(km.Track_id);
            var badFasteners = RdStructureService.GetBadRailFasteners(trip.Id, false, trackName, km.Number);

            // if (badFasteners.Count == 0) continue;

            foreach (var fastener in badFasteners)
            {
                var pdb = km.PdbSection.Count > 0 ? km.PdbSection[0].ToString() : " ПЧ-/ПЧУ-/ПД-/ПДБ-";

                var data = pdb.Split($"/").ToList();
                if (data.Any())
                {
                    pdb = $"{data[1]}/{data[2]}/{data[3]}";
                }

                var sector1 = km.StationSection != null && km.StationSection.Count > 0 ?
                                "Станция: " + km.StationSection[0].Station : (km.Sector != null ? km.Sector.ToString() : "");

                fastener.Pchu = pdb;
                fastener.Station = sector1;
                //fastener.Fastening =(string)GetName(fastener.Digressions[0].DigName);
                //fastener.Fastening = km.RailsBrace.Any() ? km.RailsBrace.First().Name : "нет данных";
                fastener.Fastening = fastener.ToString();

                fastener.Otst = fastener.Digressions[0].GetName();
                fastener.Threat_id = fastener.Threat == Threat.Left ? "левая" : "правая";
            }
            AdditionalParametersService.Insert_badfastening(mainProcess.Trip_id, -1, badFasteners);

        }

        /// <summary>
        /// Сервис огр скор Шпалы
        /// </summary>
        /// <param name="trip"></param>
        /// <param name="km"></param>
        /// <param name="distId"></param>
        private void GetdeviationsinSleepers(Trips trip, Kilometer km)
        {
            var mainProcess = new MainParametersProcess { Trip_id = trip.Id };
            var trackName = AdmStructureService.GetTrackName(km.Track_id);
            //var AbsSleepersList= RdStructureService.GetDigSleepers(mainProcess, MainTrackStructureConst.GetDigSleepers) as List<Digression>;
            var AbsSleepersList = RdStructureService.GetShpal(mainProcess, new int[] { 7 }, km.Number);

            AbsSleepersList = AbsSleepersList.OrderBy(o => o.Km).ThenBy(o => o.Meter).ToList();
            int countSl = 1;
            int prevM = -1;
            var digList = new List<Digression>();
            for (int i = 0; i <= AbsSleepersList.Count - 2; i++)
            {
                prevM = prevM == -1 ? AbsSleepersList[i].Km * 1000 + AbsSleepersList[i].Meter : prevM;
                var nextM = AbsSleepersList[i + 1].Km * 1000 + AbsSleepersList[i + 1].Meter;

                if (Math.Abs(prevM - nextM) < 2)
                {
                    prevM = nextM;
                    countSl++;
                }
                else if (countSl > 2)
                {
                    digList.Add(AbsSleepersList[i]);
                    digList[digList.Count - 1].Velich = countSl;
                    digList[digList.Count - 1].Ots = "КНШ";

                    prevM = nextM;
                    countSl = 1;
                }
                else
                {
                    prevM = nextM;
                    countSl = 1;
                }
            }
            var previousKm = -1;
            var speed = new List<Speed>();
            var pdbSection = new List<PdbSection>();
            var sector = "";

            var rail_type = new List<RailsSections>();
            var skreplenie = new List<RailsBrace>();
            var shpala = new List<CrossTie>();
            var trackclasses = new List<TrackClass>();
            var curves = new List<StCurve>();

            List<Curve> curves1 = RdStructureService.GetCurvesInTrip(trip.Id) as List<Curve>;
            digList = digList.Where(o => o.Km == km.Number).ToList();

            foreach (var item in digList)
            {
                var KM = item.Km;

                //фильтр по выбранным км
                var curves2 = curves1.Where(
                    o => item.Km + item.Meter / 10000.0 >= o.RealStartCoordinate && o.RealFinalCoordinate >= item.Km + item.Meter / 10000.0).ToList();

                if ((previousKm == -1) || (previousKm != KM))
                {
                    //sector = MainTrackStructureService.GetSector(km.Track_id, km.Number, trip.Trip_date);
                    //sector = sector == null ? "" : sector;
                    //speed = MainTrackStructureService.GetMtoObjectsByCoord(trip.Trip_date, km.Number, MainTrackStructureConst.MtoSpeed, trip.Direction, trackName.ToString()) as List<Speed>;
                    //pdbSection = MainTrackStructureService.GetMtoObjectsByCoord(trip.Trip_date, km.Number, MainTrackStructureConst.MtoPdbSection, trip.Direction, trackName.ToString()) as List<PdbSection>;
                    rail_type = MainTrackStructureService.GetMtoObjectsByCoord(trip.Trip_date, km.Number, MainTrackStructureConst.MtoRailSection, trip.Direction, trackName.ToString()) as List<RailsSections>;
                    skreplenie = MainTrackStructureService.GetMtoObjectsByCoord(trip.Trip_date, km.Number, MainTrackStructureConst.MtoRailsBrace, trip.Direction, trackName.ToString()) as List<RailsBrace>;
                    trackclasses = (List<TrackClass>)MainTrackStructureService.GetMtoObjectsByCoord(trip.Trip_date, km.Number, MainTrackStructureConst.MtoTrackClass, km.Track_id);
                    //curves = (List<StCurve>)MainTrackStructureService.GetMtoObjectsByCoord(trip.Trip_date, km.Number, MainTrackStructureConst.MtoStCurve, km.Track_id);
                }
                previousKm = KM;

                var pdb = km.PdbSection.Count > 0 ? km.PdbSection[0].ToString() : " ПЧ-/ПЧУ-/ПД-/ПДБ-";

                var data = pdb.Split($"/").ToList();
                if (data.Any())
                {
                    pdb = $"{data[1]}/{data[2]}/{data[3]}";
                }

                var isStation = km.StationSection.Any() ?
                                km.StationSection.Where(o => item.Km + item.Meter / 10000.0 >= o.RealStartCoordinate && o.RealFinalCoordinate >= item.Km + item.Meter / 10000.0).ToList() :
                                new List<StationSection> { };

                var sector1 = isStation.Any() ? "Станция: " + km.StationSection[0].Station : (km.Sector != null ? km.Sector.ToString() : "");

                if (item.Meter == 559)
                {
                    var rr = 0;
                }

                var curve = curves2.Any() ? curves2.First().Straightenings.Any() ? (int)curves2.First().Straightenings.First().Radius : -1 : -1;

                var ogr = "";

                switch (curve)
                {
                    case int cr when cr == -1 || cr >= 650:
                        if (rail_type[0].Name == "p65" || rail_type[0].Name == "p75")
                        {
                            switch (item.Velich)
                            {
                                case int c when c == 4:
                                    ogr = "60/40";
                                    break;
                                case int c when c == 5:
                                    ogr = "40/25";
                                    break;
                                case int c when c >= 6:
                                    ogr = "15/15";
                                    break;
                                default:
                                    ogr = "";
                                    break;
                            }
                        }
                        if (rail_type[0].Name == "p50")
                        {
                            switch (item.Velich)
                            {
                                case int c when c == 3:
                                    ogr = "50/40";
                                    break;
                                case int c when c == 4:
                                    ogr = "40/25";
                                    break;
                                case int c when c >= 5:
                                    ogr = "15/15";
                                    break;
                                default:
                                    ogr = "";
                                    break;
                            }
                        }
                        break;
                    default:
                        ogr = "";
                        break;
                }


                item.PCHU = pdb;
                item.Station = sector1;
                item.Speed = km.Speeds.Count > 0 ? km.Speeds.Last().ToString() : "-/-/-";

                item.Vpz = km.Speeds.Count > 0 ? km.Speeds[0].Passenger.ToString() + "/" + km.Speeds[0].Freight.ToString() : "-/-";
                item.Ots = item.Ots;
                item.TrackClass = (trackclasses.Count > 0 ? trackclasses[0].Class_Id : -1).ToString();
                item.Tripplan = curve != -1 ? "кривая R-" + curve.ToString() : "прямой";

                //item.Fastening = skreplenie.Count > 0 ? skreplenie[0].Name : "Нет данных";
                item.Fastening = km.RailsBrace.Any() ? km.RailsBrace.First().Name : "нет данных";
                item.Norma = km.Gauge.Count > item.Meter - 1 ? km.Gauge[item.Meter].ToString("0") : "нет данных";
                item.Kol = item.Velich + " шт";
                item.RailType = rail_type.Count > 0 ? rail_type[0].Name : "Нет данных";
                item.Vdop = ogr;

            }

            AdditionalParametersService.Insert_sleepers(mainProcess.Trip_id, -1, digList);
        }

        /// <summary>
        /// Сервис Ведомость отсутствующих болтов
        /// </summary>
        /// <param name="trip">Данные поездки</param>
        /// <param name="km">Километр</param>
        /// <param name="DistId">ПЧ id</param>
        public void GetBolt(Trips trip, Kilometer km)
        {
            km.LoadTrackPasport(MainTrackStructureRepository, trip.Trip_date);
            var mainProcess = new MainParametersProcess { Trip_id = trip.Id };
            //левая сторона
            var AbsBoltListLeft = RdStructureService.NoBolt(mainProcess, Threat.Left, km.Number);
            //правая сторона
            var AbsBoltListRight = RdStructureService.NoBolt(mainProcess, Threat.Right, km.Number);
            List<Digression> AbsBoltList = new List<Digression>(AbsBoltListLeft);
            AbsBoltList.AddRange(AbsBoltListRight);
            AbsBoltList = AbsBoltList.OrderBy(o => o.Km).ThenBy(o => o.Meter).ToList();


        
        
            this.MainTrackStructureRepository = MainTrackStructureService.GetRepository();


         






            foreach (var item in AbsBoltList)
            {
                var pdb = km.PdbSection.Count > 0 ? km.PdbSection[0].ToString() : " ПЧ-/ПЧУ-/ПД-/ПДБ-";

                var data = pdb.Split($"/").ToList();
                if (data.Any())
                {
                    pdb = $"{data[1]}/{data[2]}/{data[3]}";
                }

                var isStation = km.StationSection.Any() ?
                                km.StationSection.Where(o => item.Km + item.Meter / 10000.0 >= o.RealStartCoordinate && o.RealFinalCoordinate >= item.Km + item.Meter / 10000.0).ToList() :
                                new List<StationSection> { };

                var sector1 = isStation.Any() ? "Станция: " + km.StationSection[0].Station : (km.Sector != null ? km.Sector.ToString() : "");

                item.PCHU = pdb;
                item.Station = sector1;
                item.Speed = km.Speeds.Count > 0 ? km.Speeds.Last().Passenger + "/" + km.Speeds.Last().Freight : "-/-";
            }

            AdditionalParametersService.Insert_bolt(mainProcess.Trip_id, -1, AbsBoltList);
        }
        private void GetPerpen(Trips trip, Kilometer km)
        {
            var mainProcess = new MainParametersProcess { Trip_id = trip.Id };
            var trackName = AdmStructureService.GetTrackName(km.Track_id);
            //var skreplenie = MainTrackStructureService.GetMtoObjectsByCoord(trip.Trip_date, km.Number,
            //    MainTrackStructureConst.MtoRailsBrace, trip.Direction, trackName.ToString()) as List<RailsBrace>;
            var skreplenie = MainTrackStructureService.GetMtoObjectsByCoord(trip.Trip_date, km.Number,
              MainTrackStructureConst.MtoRailsBrace, "Петропавловск - Шу", trackName.ToString()) as List<RailsBrace>;
            
            var ViolPerpen = RdStructureService.GetViolPerpen((int)trip.Id, new int[] { 7 }, km.Number);

            AdditionalParametersService.Insert_ViolPerpen(km, skreplenie, ViolPerpen);
        }
        /// <summary>
        /// Сервис по деф шпалам
        /// </summary>
        /// <param name="trip"></param>
        /// <param name="km"></param>
        /// <param name="distId"></param>
        private void GetSleepers(Trips trip, Kilometer km)
        {
            try
            {
                var mainProcess = new MainParametersProcess { Trip_id = trip.Id };
                //var distance = AdmStructureService.GetUnit(AdmStructureConst.AdmDistance, distId) as AdmUnit;
                var trackName = AdmStructureService.GetTrackName(km.Track_id);

                var digressions = RdStructureService.GetShpal(mainProcess, new int[] { 7 }, km.Number);


                List<Gap> check_gap_state = AdditionalParametersService.Check_gap_state(trip.Id, 0);

                var listIS = new List<int> { 10 };
                var listGAP = new List<int> { 7 };

                var previousKm = -1;
                var skreplenie = new List<RailsBrace>();
                var pdbSection = new List<PdbSection>();
                var sector = "";



                for (int i = 0; i < digressions.Count; i++)
                {
                    var isgap = false;

                    var c = check_gap_state.Where(o => o.Km + o.Meter / 10000.0 == digressions[i].Km + digressions[i].Meter / 10000.0).ToList();

                    if (c.Any())
                    {
                        isgap = true;
                    }
                    else
                    {
                        isgap = false;
                    }

                    if (digressions == null || digressions.Count == 0) continue;

                    if ((previousKm == -1) || (previousKm != digressions[i].Km))
                    {
                        //sector = MainTrackStructureService.GetSector(km.Track_id, digressions[i].Km, trip.Trip_date);
                        //sector = sector == null ? "Нет данных" : sector;
                        //pdbSection = MainTrackStructureService.GetMtoObjectsByCoord(trip.Trip_date, digressions[i].Km, MainTrackStructureConst.MtoPdbSection, trip.Direction, trackName.ToString()) as List<PdbSection>;
                        skreplenie = MainTrackStructureService.GetMtoObjectsByCoord(trip.Trip_date, digressions[i].Km, MainTrackStructureConst.MtoRailsBrace, "Петропавловск - Шу", trackName.ToString()) as List<RailsBrace>;
                    }

                    var pdb = km.PdbSection.Count > 0 ? km.PdbSection[0].ToString() : " ПЧ-/ПЧУ-/ПД-/ПДБ-";

                    var data = pdb.Split($"/").ToList(); 
                    if (data.Any())
                    {
                        pdb = $"{data[1]}/{data[2]}/{data[3]}";
                    }

                    var sector1 = km.StationSection != null && km.StationSection.Count > 0 ?
                                    "Станция: " + km.StationSection[0].Station : (km.Sector != null ? km.Sector.ToString() : "");

                    previousKm = digressions[i].Km;

                    var otst = "";
                    var meropr = "";

                    switch (digressions[i].Oid)
                    {
                        case (int)VideoObjectType.Railbreak:
                            otst = "продольная трещина";
                            meropr = "замена при среднем ремонте ";
                            break;
                        case (int)VideoObjectType.Railbreak_Stone:
                            otst = "продольная трещина";
                            meropr = "замена при среднем ремонте";
                            break;
                        case (int)VideoObjectType.Railbreak_vikol:
                            otst = "выкол";
                            meropr = "замена при текущем содержании";
                            break;
                        case (int)VideoObjectType.Railbreak_raskol:
                            otst = "продольный раскол";
                            meropr = "замена при среднем ремонте";
                            break;
                        case (int)VideoObjectType.Railbreak_midsection:
                            otst = "излом в средней части";
                            meropr = "первоначальная замена при текущем содержании";
                            break;
                        case (int)VideoObjectType.Railbreak_Stone_vikol:
                            otst = "выкол";
                            meropr = "замена при текущем содержании";
                            break;
                        case (int)VideoObjectType.Railbreak_Stone_raskol:
                            if (i < digressions.Count - 2 && Math.Abs(digressions[i + 1].Meter - digressions[i].Meter) == 1)
                            {
                                otst = "продольный раскол";
                                meropr = "первоначальная замена при текущем содержании";
                            }
                            else if (i > 0 && Math.Abs(digressions[i - 1].Meter - digressions[i].Meter) == 1)
                            {
                                otst = "продольный раскол";
                                meropr = "первоначальная замена при текущем содержании";
                            }
                            else if (isgap)
                            {
                                otst = "продольный раскол";
                                meropr = "первоначальная замена при текущем содержании";
                            }
                            else
                            {
                                otst = "продольный раскол";
                                meropr = "замена при среднем ремонте";
                            }
                            break;
                        case (int)VideoObjectType.Railbreak_Stone_midsection:
                            if (i < digressions.Count - 2 && Math.Abs(digressions[i + 1].Meter - digressions[i].Meter) == 1)
                            {
                                otst = "излом в средней части";
                                meropr = "первоначальная замена при текущем содержании";
                            }
                            else if (i > 0 && Math.Abs(digressions[i - 1].Meter - digressions[i].Meter) == 1)
                            {
                                otst = "излом в средней части";
                                meropr = "первоначальная замена при текущем содержании";
                            }
                            else if (isgap)
                            {
                                otst = "излом в средней части";
                                meropr = "первоначальная замена при текущем содержании";
                            }
                            else
                            {
                                otst = "излом в средней части";
                                meropr = "замена при среднем ремонте";
                            }
                            break;
                    }

                    digressions[i].Otst = otst;
                    digressions[i].Meropr = meropr;
                    digressions[i].Pchu = pdb;
                    digressions[i].Station = sector1;
                    digressions[i].Fastening = skreplenie.Count > 0 ? skreplenie[0].Name : "Нет данных";
                    digressions[i].Notice = isgap ? "стык" : "";
                }

                AdditionalParametersService.Insert_defshpal(mainProcess.Trip_id, 1, digressions);

            }
            catch (Exception e)
            {
                Console.WriteLine("GetSleepers " + e.Message);
            }
        }
        /// <summary>
        /// Сервис по стыкам
        /// </summary>
        /// <param name="trip"></param>
        /// <param name="km"></param>
        /// <param name="distId"></param>

        private string Getdeviationsinfastening(Trips trip, Kilometer km)
        {
            try
            {
                var mainProcess = new MainParametersProcess { Trip_id = trip.Id };
                //var distance = AdmStructureService.GetUnit(AdmStructureConst.AdmDistance, distId) as AdmUnit;
                var trackName = AdmStructureService.GetTrackName(km.Track_id);

                //var getdeviationfastening = RdStructureService.GetBadRailFasteners(trip.Id, false, distance.Code, trackName);
                var getdeviationfastening = RdStructureService.GetBadRailFasteners(trip.Id, false, trackName, km.Number);
                // if (badFasteners.Count == 0) continue;

                RailFastener prev_fastener = null;
                var sector = "";
                int countSl = 1;
                int prevM = -1;
                int prevThreat = -1;
                var digList = new List<RailFastener>();

                for (int i = 0; i <= getdeviationfastening.Count - 2; i++)
                {
                    prevM = prevM == -1 ? getdeviationfastening[i].Km * 1000 + getdeviationfastening[i].Mtr : prevM;
                    prevThreat = prevThreat == -1 ? (int)getdeviationfastening[i].Threat : prevThreat;

                    var nextM = getdeviationfastening[i + 1].Km * 1000 + getdeviationfastening[i + 1].Mtr;
                    var nextThreat = (int)getdeviationfastening[i + 1].Threat;


                    if (Math.Abs(prevM - nextM) < 2)
                    {
                        if (prevThreat == nextThreat)
                        {
                            prevM = nextM;
                            countSl++;
                        }
                        else
                        {
                            if (countSl > 3)
                            {
                                digList.Add(getdeviationfastening[i]);
                                digList[digList.Count - 1].Count = countSl;
                                digList[digList.Count - 1].Ots = "КНС";

                                prevM = nextM;
                                countSl = 1;
                            }
                        }
                    }
                    else if (countSl > 3)
                    {
                        digList.Add(getdeviationfastening[i]);
                        digList[digList.Count - 1].Count = countSl;
                        digList[digList.Count - 1].Ots = "КНС";

                        prevM = nextM;
                        countSl = 1;

                    }
                    else
                    {
                        prevM = nextM;
                        countSl = 1;
                    }
                }

                RailFastener prev_digression = null;
                var speed = new List<Speed> { };
                var pdbSection = new List<PdbSection> { };
                var curves = new List<StCurve>();
                foreach (var digression in digList)
                {

                    if ((prev_digression == null) || (prev_digression.Km != digression.Km))
                    {
                        //tripplan = digression.Location == Location.OnCurveSection ? $"кривая R-{digression.CurveRadius}" : (digression.Location == Location.OnStrightSection ? "прямой" : "стрелочный перевод");
                        //amount = digression.DigName == DigressionName.KNS ? $"{digression.Count} шт" : $"{digression.Length} %";
                        speed = MainTrackStructureService.GetMtoObjectsByCoord(trip.Trip_date, km.Number, MainTrackStructureConst.MtoSpeed, trip.Direction, trackName.ToString()) as List<Speed>;
                        //pdbSection = MainTrackStructureService.GetMtoObjectsByCoord(trip.Trip_date, km.Number, MainTrackStructureConst.MtoPdbSection, trip.Direction, trackName.ToString()) as List<PdbSection>;
                        //sector = MainTrackStructureService.GetSector(km.Track_id, km.Number, trip.Trip_date);
                        //sector = sector == null ? "" : sector;
                        curves = (List<StCurve>)MainTrackStructureService.GetMtoObjectsByCoord(trip.Trip_date, km.Number, MainTrackStructureConst.MtoStCurve, km.Track_id);
                    }
                    var curve = curves.Count > 0 ? (int)curves[0].Radius : -1;
                    var curveNorma = curves.Count > 0 ? (int)curves[0].Width : -1;

                    var ogr = "";

                    switch (curve)
                    {
                        case int cr when cr == -1 || cr >= 650:
                            switch (digression.Count)
                            {
                                case int c when c == 4:
                                    ogr = "60/60";
                                    break;
                                case int c when c == 5:
                                    ogr = "40/40    ";
                                    break;
                                case int c when c == 6:
                                    ogr = "25/25";
                                    break;
                                case int c when c > 6:
                                    ogr = "15/15";
                                    break;
                                default:
                                    ogr = "";
                                    break;
                            }
                            break;
                        case int cr when cr < 650:
                            switch (digression.Count)
                            {
                                case int c when c == 4:
                                    ogr = "40/40";
                                    break;
                                case int c when c == 5:
                                    ogr = "25/25";
                                    break;
                                case int c when c > 5:
                                    ogr = "15/15";
                                    break;
                                default:
                                    ogr = "";
                                    break;
                            }
                            break;
                        default:
                            ogr = "";
                            break;
                    }
                    var pdb = km.PdbSection.Count > 0 ? km.PdbSection[0].ToString() : " ПЧ-/ПЧУ-/ПД-/ПДБ-";

                    var data = pdb.Split($"/").ToList();
                    if (data.Any())
                    {
                        pdb = $"{data[1]}/{data[2]}/{data[3]}";
                    }

                    var sector1 = km.StationSection != null && km.StationSection.Count > 0 ?
                                    "Станция: " + km.StationSection[0].Station : (km.Sector != null ? km.Sector.ToString() : "");
                    digression.Pchu = pdb;
                    //digression.Norma = ( curveNorma == -1 ? 1520 : curveNorma).ToString();
                    digression.Norma = km.Gauge.Count > digression.Mtr - 1 ? km.Gauge[digression.Mtr].ToString("0") : (curveNorma == -1 ? "нет данных" : curveNorma.ToString());

                    digression.Tripplan = curve != -1 ? "кривая R-" + curve.ToString() : "прямой";
                    digression.Station = sector1;

                    prev_fastener = digression;

                    //digression.Fastening = (string)GetName(digression.Digressions[0].DigName);
                    digression.Fastening = km.RailsBrace.Any() ? km.RailsBrace.First().Name : "нет данных";
                    // fastener.Station = sector;
                    digression.Fragment = sector;
                    digression.Otst = digression.Digressions[0].GetName();
                    digression.Threat_id = digression.Threat == Threat.Left ? "левая" : "правая";
                    digression.Velich = digression.Count + " шт";
                    digression.Vdop = ogr;
                    digression.Vpz = speed.Count > 0 ? speed[0].Passenger + "/" + speed[0].Freight : "";
                }
                AdditionalParametersService.Insert_deviationsinfastening(mainProcess.Trip_id, -1, digList);

                return "Success";
            }
            catch (Exception e)
            {
                return "Error " + e.Message;
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // listen to the RabbitMQ messages, and send emails
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await base.StopAsync(cancellationToken);
            _connection.Close();
            _logger.LogInformation("RabbitMQ connection is closed.");
        }
    }
 }
