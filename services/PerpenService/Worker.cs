using ALARm.Core;
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

namespace PerpenService
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
                    string p = GetPerpen(trip, km, 53); //стыки

                    _logger.LogInformation(" [x] GetPerpen {0} {1} {2}", fileId, kmIndex, p);

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
        /// Сервис по стыкам
        /// </summary>
        /// <param name="trip"></param>
        /// <param name="km"></param>
        /// <param name="distId"></param>
        private string GetPerpen(Trips trip, Kilometer km, int distId)
        {
            try
            {

                var mainProcess = new MainParametersProcess { Trip_id = trip.Id };
                var trackName = AdmStructureService.GetTrackName(km.Track_id);
                var skreplenie = MainTrackStructureService.GetMtoObjectsByCoord(trip.Trip_date, km.Number,
                    MainTrackStructureConst.MtoRailsBrace, trip.Direction, trackName.ToString()) as List<RailsBrace>;

                var ViolPerpen = RdStructureService.GetViolPerpen((int)trip.Id, new int[] { 7 }, km.Number);

                AdditionalParametersService.Insert_ViolPerpen(km, skreplenie, ViolPerpen);
          

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
