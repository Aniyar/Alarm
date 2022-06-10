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

namespace SleepersService
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
                        GetSleepers(trip, km);
                        Console.WriteLine("Ўпалы ќ !");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Ўпалы ERROR! " + e.Message);
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
                        //sector = sector == null ? "Ќет данных" : sector;
                        //pdbSection = MainTrackStructureService.GetMtoObjectsByCoord(trip.Trip_date, digressions[i].Km, MainTrackStructureConst.MtoPdbSection, trip.Direction, trackName.ToString()) as List<PdbSection>;
                        skreplenie = MainTrackStructureService.GetMtoObjectsByCoord(trip.Trip_date, digressions[i].Km, MainTrackStructureConst.MtoRailsBrace, "ѕетропавловск - Ўу", trackName.ToString()) as List<RailsBrace>;
                    }

                    var pdb = km.PdbSection.Count > 0 ? km.PdbSection[0].ToString() : " ѕ„-/ѕ„”-/ѕƒ-/ѕƒЅ-";

                    var data = pdb.Split($"/").ToList();
                    if (data.Any())
                    {
                        pdb = $"{data[1]}/{data[2]}/{data[3]}";
                    }

                    var sector1 = km.StationSection != null && km.StationSection.Count > 0 ?
                                    "—танци€: " + km.StationSection[0].Station : (km.Sector != null ? km.Sector.ToString() : "");

                    previousKm = digressions[i].Km;

                    var otst = "";
                    var meropr = "";

                    switch (digressions[i].Oid)
                    {
                        case (int)VideoObjectType.Railbreak:
                            otst = "продольна€ трещина";
                            meropr = "замена при среднем ремонте ";
                            break;
                        case (int)VideoObjectType.Railbreak_Stone:
                            otst = "продольна€ трещина";
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
                            meropr = "первоначальна€ замена при текущем содержании";
                            break;
                        case (int)VideoObjectType.Railbreak_Stone_vikol:
                            otst = "выкол";
                            meropr = "замена при текущем содержании";
                            break;
                        case (int)VideoObjectType.Railbreak_Stone_raskol:
                            if (i < digressions.Count - 2 && Math.Abs(digressions[i + 1].Meter - digressions[i].Meter) == 1)
                            {
                                otst = "продольный раскол";
                                meropr = "первоначальна€ замена при текущем содержании";
                            }
                            else if (i > 0 && Math.Abs(digressions[i - 1].Meter - digressions[i].Meter) == 1)
                            {
                                otst = "продольный раскол";
                                meropr = "первоначальна€ замена при текущем содержании";
                            }
                            else if (isgap)
                            {
                                otst = "продольный раскол";
                                meropr = "первоначальна€ замена при текущем содержании";
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
                                meropr = "первоначальна€ замена при текущем содержании";
                            }
                            else if (i > 0 && Math.Abs(digressions[i - 1].Meter - digressions[i].Meter) == 1)
                            {
                                otst = "излом в средней части";
                                meropr = "первоначальна€ замена при текущем содержании";
                            }
                            else if (isgap)
                            {
                                otst = "излом в средней части";
                                meropr = "первоначальна€ замена при текущем содержании";
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
                    digressions[i].Fastening = skreplenie.Count > 0 ? skreplenie[0].Name : "Ќет данных";
                    digressions[i].Notice = isgap ? "стык" : "";
                }

                AdditionalParametersService.Insert_defshpal(mainProcess.Trip_id, 1, digressions);

            }
            catch (Exception e)
            {
                Console.WriteLine("GetSleepers " + e.Message);
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