using ALARm.Core;
using ALARm.Core.Report;
using ALARm.DataAccess;
using ALARm.Services;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using Npgsql;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace RailCrossSectionService
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
                //consumer.Received += async (model, ea) =>
                {
                    //var body = ea.Body.ToArray();
                    //var message = Encoding.UTF8.GetString(body);
                    //_logger.LogInformation(" [x] Received {0}", message);

                    try
                    {
                        //{
                        //    "TripId" : 213,
                        //    "DistId" : 53,
                        //    "KmIndex" : 40157
                        //}
                        //JObject json = JObject.Parse(message);
                        //Console.WriteLine(message);
                        //Console.WriteLine($"{(int)json["TripId"]} {(int)json["DistId"]} {(int)json["KmIndex"]}");

                        //var TripId = (int)json["TripId"];

                        var AppData = new AppData { };

                        AppData.Conn = new NpgsqlConnection(AppData.Cs);
                        AppData.Conn.Open();

                        AppData.In_koridor = new BinaryReader(File.Open(AppData.Vnutr__profil__koridor, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
                        var data = AppData.In_koridor.ReadBytes(8);
                        AppData.In_koridor_count = BitConverter.ToSingle(data, 0);
                        AppData.In_koridor_size = BitConverter.ToSingle(data, 4);

                        AppData.In_kupe = new BinaryReader(File.Open(AppData.Vnutr__profil__kupe, FileMode.Open, FileAccess.Read, FileShare.ReadWrite));
                        data = AppData.In_kupe.ReadBytes(8);
                        AppData.In_kupe_count = BitConverter.ToSingle(data, 0);
                        AppData.In_kupe_size = BitConverter.ToSingle(data, 4);

                        //todo Выбор рельсовой нити по пасспорту
                        //AppData.NominalRailScheme = AppData.GetNominalRailScheme(AppData.Rails.r65);

                        while (AppData.Status)
                        {
                            AppData.CurrentProfileLeft(); //Koridor
                            AppData.CurrentProfileRight(); //Kupe

                            AppData.CurrentFrameIndex++;
                        }

                        if (!AppData.Status)
                            Console.WriteLine("Файл закончен");


                        //var trip = RdStructureService.GetTripFromFileId(fileId).Last();
                        //var kilometers = RdStructureService.GetKilometersByTrip(trip);
                        //var km = kilometers.Where(km => km.Number == kmIndex).ToList().First();
                        //this.MainTrackStructureRepository = MainTrackStructureService.GetRepository();

                        //var outData = (List<OutData>)RdStructureService.GetNextOutDatas(km.Start_Index - 1, km.GetLength() - 1, trip.Id);
                        //km.AddDataRange(outData, km);
                        //km.LoadTrackPasport(MainTrackStructureRepository, trip.Trip_date);

                        ////Видеоконтроль
                        //// todo distanse id
                        //string p = GetGaps(trip, km, 53); //стыки

                        //_logger.LogInformation(" [x] GapService {0} {1} {2}", fileId, kmIndex, p);

                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("RailCrossSectionService JSON PARSE ERROR " + e.Message);
                    }
                };
                _channel.BasicConsume(queue: QueueName, autoAck: true, consumer: consumer);
                return base.StartAsync(cancellationToken);
            }
            catch
            {
                StartAsync(cancellationToken);
                return base.StartAsync(cancellationToken);
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
