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

namespace Deviationinsleepers
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
                    _logger.LogInformation(" [x] Received {0}", message);

                    JObject json = JObject.Parse(message);

                    var kmIndex = (int)json["KmIndex"];
                    var fileId = (int)json["FileId"];

                    var trip = RdStructureService.GetTripFromFileId(fileId).Last();
                    var kilometers = RdStructureService.GetKilometersByTrip(trip);
                    var km = kilometers.Where(km => km.Number == kmIndex).ToList().First();
                    this.MainTrackStructureRepository = MainTrackStructureService.GetRepository();

                    var outData = (List<OutData>)RdStructureService.GetNextOutDatas(km.Start_Index - 1, km.GetLength() - 1, trip.Id);
                    km.AddDataRange(outData, km);
                    km.LoadTrackPasport(MainTrackStructureRepository, trip.Trip_date);

                    //Видеоконтроль
                    // todo distanse id
                    string p = GetdeviationsinSleepers(trip, km, 53); //стыки

                    _logger.LogInformation(" [x] GetdeviationsinSleepers {0} {1} {2}", fileId, kmIndex, p);

                };
                _channel.BasicConsume(queue: QueueName,
                                      autoAck: true,
                                      consumer: consumer);

                return base.StartAsync(cancellationToken);
            }
            catch
            {
                StartAsync(cancellationToken);
                return base.StartAsync(cancellationToken);
            }
        }

        /// <summary>
        /// Сервис по стыкам
        /// </summary>
        /// <param name="trip"></param>
        /// <param name="km"></param>
        /// <param name="distId"></param>

        private string GetdeviationsinSleepers(Trips trip, Kilometer km, int distId)
        {
            try { 
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





