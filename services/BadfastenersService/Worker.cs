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

namespace BadfastenerstService
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
                    string p = Getbadfasteners(trip, km, 53); //стыки

                    _logger.LogInformation(" [x] Getbadfasteners {0} {1} {2}", fileId, kmIndex, p);

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
        /// Сервис Негодных  Скреплений  
        /// </summary>
        /// <param name="trip"></param>
        /// <param name="km"></param>
        /// <param name="distId"></param>
        /// 
        private string Getbadfasteners(Trips trip, Kilometer km, int distId)
        {
            try
            {
                var mainProcess = new MainParametersProcess { Trip_id = trip.Id };
                var distance = AdmStructureService.GetUnit(AdmStructureConst.AdmDistance, distId) as AdmUnit;
                var trackName = AdmStructureService.GetTrackName(km.Track_id);
                var badFasteners = RdStructureService.GetBadRailFasteners(trip.Id, false, distance.Code, trackName, km.Number);

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
                    fastener.Fastening = km.RailsBrace.Any() ? km.RailsBrace.First().Name : "нет данных";

                    fastener.Otst = fastener.Digressions[0].GetName();
                    fastener.Threat_id = fastener.Threat == Threat.Left ? "левая" : "правая";
                }
                AdditionalParametersService.Insert_badfastening(mainProcess.Trip_id, -1, badFasteners);
            }
            catch (Exception e)
            {
                return "Error " + e.Message;
            }

            return "Failed";
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

