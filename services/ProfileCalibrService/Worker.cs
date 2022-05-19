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

namespace ProfileCalibrService
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
                    // {'FileId':18308, 'Km':707, 'Path': '\DESKTOP-EMAFC5J\common\video_objects\desktop\242_18308_km_707.csv'}
                    Trips trip = RdStructureService.GetTripFromFileId(kmId)[0];
                    int TripId = (int)trip.Id;
                    _logger.LogInformation("got trip");
                    var kilometers = RdStructureService.GetKilometersByTrip(trip);
                    Kilometer km = kilometers.Where(km => km.Number == kmIndex).First();
                    _logger.LogInformation("got km");
                    this.MainTrackStructureRepository = MainTrackStructureService.GetRepository();
                    var outData = (List<OutData>)RdStructureService.GetNextOutDatas(km.Start_Index - 1, km.GetLength() - 1, TripId);
                    km.AddDataRange(outData, km);
                    km.LoadTrackPasport(MainTrackStructureRepository, trip.Trip_date);


                    var Blazor = new Blazor_ProfileData();
                    _logger.LogInformation("started blazor");
                    Blazor.conn = new NpgsqlConnection(Helper.ConnectionString());
                    _logger.LogInformation("started blazor connection");
                    Blazor.conn.Open();
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
                        Console.WriteLine("профайл дата ОК");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("профайл дата ERROR! " + e.Message);
                    }
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
    public class RabbitMQConfiguration
    {
        public string Host { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Queue { get; set; }
        public int Port { get; set; }

    }
}
