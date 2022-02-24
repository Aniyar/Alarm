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

namespace BallastService
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
                    string p = GetBalast(trip, km, 53); //стыки

                    _logger.LogInformation(" [x] GetBalast {0} {1} {2}", fileId, kmIndex, p);

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
        /// Сервис по  Балласт
        /// </summary>
        /// <param name="trip"></param>
        /// <param name="km"></param>
        /// <param name="distId"></param>
        private string GetBalast(Trips trip, Kilometer km, int distId)
        {
            try
            {
                var mainProcess = new MainParametersProcess { Trip_id = trip.Id };
                var distance = AdmStructureService.GetUnit(AdmStructureConst.AdmDistance, distId) as AdmUnit;
                var trackName = AdmStructureService.GetTrackName(km.Track_id);
                var digressions = RdStructureService.GetBadRailFasteners(trip.Id, false, distance.Code, trackName);
                //var digressions = RdStructureService.GetBallast(trip.Id, false, distance.Code, trackName);
                
                // if (badFasteners.Count == 0) continue;
                //digressions = digressions.Where(o => o.Razn > 10 && o.Km > 128).ToList();
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
