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

namespace BoltService
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

                    var TripId = (int)json["TripId"];
                    var DistId = (int)json["DistId"];
                    var kmIndex = (int)json["KmIndex"];

                    var trips = RdStructureService.GetTrips();
                    var trip = trips.Where(trip => trip.Id == TripId).ToList().First();

                    var kilometers = RdStructureService.GetKilometersByTrip(trip);
                    var km = kilometers.Where(km => km.Number == kmIndex).ToList().First();
                    this.MainTrackStructureRepository = MainTrackStructureService.GetRepository();

                    var outData = (List<OutData>)RdStructureService.GetNextOutDatas(km.Start_Index - 1, km.GetLength() - 1, TripId);
                    km.AddDataRange(outData, km);

                    km.LoadTrackPasport(MainTrackStructureRepository, trip.Trip_date);
                    //Видеоконтроль
                    // todo distanse id
                    GetBolt(trip, km, DistId);

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



        /// <summary>
        /// Сервис Ведомость отсутствующих болтов
        /// </summary>
        /// <param name="trip">Данные поездки</param>
        /// <param name="km">Километр</param>
        /// <param name="DistId">ПЧ id</param>
        public string GetBolt(Trips trip, Kilometer km, int DistId)
        {
            try
            {
                var mainProcess = new MainParametersProcess { Trip_id = trip.Id };
                //левая сторона
                var AbsBoltListLeft = RdStructureService.NoBolt(mainProcess, Threat.Left, km.Number);
                //правая сторона
                var AbsBoltListRight = RdStructureService.NoBolt(mainProcess, Threat.Right, km.Number);
                List<Digression> AbsBoltList = new List<Digression>(AbsBoltListLeft);
                AbsBoltList.AddRange(AbsBoltListRight);
                AbsBoltList = AbsBoltList.OrderBy(o => o.Km).ThenBy(o => o.Meter).ToList();

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
