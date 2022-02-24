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

namespace DeviationsinfasteningService
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

                    //var kmIndex = (int)json["KmIndex"];
                    //var fileId = (int)json["FileId"];
                    var TripId = (int)json["TripId"];
                    var DistId = (int)json["DistId"];
                    var kmIndex = (int)json["KmIndex"];

                    var trip = RdStructureService.GetTripFromFileId(TripId).Last();
                    var kilometers = RdStructureService.GetKilometersByTrip(trip);
                    var km = kilometers.Where(km => km.Number == kmIndex).ToList().First();
                    this.MainTrackStructureRepository = MainTrackStructureService.GetRepository();

                    var outData = (List<OutData>)RdStructureService.GetNextOutDatas(km.Start_Index - 1, km.GetLength() - 1, trip.Id);
                    km.AddDataRange(outData, km);
                    km.LoadTrackPasport(MainTrackStructureRepository, trip.Trip_date);

                  

                    //Видеоконтроль
                    // todo distanse id
                    string p = Getdeviationsinfastening(trip, km, DistId); //стыки

                    _logger.LogInformation(" [x] Getdeviationsinfastening {0} {1} {2}", TripId, kmIndex, p);

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

        private string Getdeviationsinfastening(Trips trip, Kilometer km, int distId)
        {
            try { 
            var mainProcess = new MainParametersProcess { Trip_id = trip.Id };
            var distance = AdmStructureService.GetUnit(AdmStructureConst.AdmDistance, distId) as AdmUnit;
            var trackName = AdmStructureService.GetTrackName(km.Track_id);

            //var getdeviationfastening = RdStructureService.GetBadRailFasteners(trip.Id, false, distance.Code, trackName);
            var getdeviationfastening = RdStructureService.GetBadRailFasteners(trip.Id, false, distance.Code, trackName, km.Number);
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





