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

namespace WavesService
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

                    Trips trip = RdStructureService.GetTripFromFileId(kmId)[0];
                    int TripId = (int)trip.Id;
                    var kilometers = RdStructureService.GetKilometersByTrip(trip);
                    Kilometer km = kilometers.Where(km => km.Number == kmIndex).First();



                    this.MainTrackStructureRepository = MainTrackStructureService.GetRepository();


                    var outData = (List<OutData>)RdStructureService.GetNextOutDatas(km.Start_Index - 1, km.GetLength() - 1, TripId);
                    km.AddDataRange(outData, km);

                    km.LoadTrackPasport(MainTrackStructureRepository, trip.Trip_date);
                    try
                    {
                        GetTestData(km.Number, trip.Id); //волны и импульсы
                        Console.WriteLine("волны ОК!");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("волны ERROR! " + e.Message);
                    }


                };
                _channel.BasicConsume(queue: QueueName,
                                      autoAck: true,
                                      consumer: consumer);

                return base.StartAsync(cancellationToken);
            }
            catch (Exception e)
            {
                StartAsync(cancellationToken);
                return base.StartAsync(cancellationToken);
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

                var cs = "Host=DESKTOP-EMAFC5J;Username=postgres;Password=alhafizu;Database=railway_comp";

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