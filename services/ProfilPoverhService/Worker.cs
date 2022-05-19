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

namespace ProfilPoverhService
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

                    var Blazor2 = new Blazor_TestData();
                    try
                    {
                        bool flag = true;
                        string koridorfile = RdStructureService.GetTripFiles(km.Number, TripId, "ProfilPoverxKoridor");
                        string kupefile = RdStructureService.GetTripFiles(km.Number, TripId, "ProfilPoverxKupe");
                        while (flag)
                        {
                            flag = Blazor2.GetBitmapAsync(koridorfile, kupefile);
                            Blazor2.CurrentFrameIndex++;
                        }
                        Console.WriteLine("тест дата ОК");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("тест дата ERROR! " + e.Message);
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

        /// <summary>
        /// Сервис по доппараметрам
        /// </summary>
        /// <param name="trip"></param>
        /// <param name="km"></param>
        /// <param name="distId"></param>


        private void PutKilometers(int trip_id)
        {
            var con = new NpgsqlConnection(Helper.ConnectionString());
            con.Open();
            List<string> files = con.Query<string>($@"SELECT file_name as filename FROM trip_files WHERE trip_id={trip_id}").ToList();
            foreach (string file in files)
            {

                using (BinaryReader reader = new BinaryReader(File.Open(file, FileMode.Open)))
                {
                    try
                    {
                        int width = reader.ReadInt32();
                        int height = reader.ReadInt32();
                        int frameSize = width * height;
                        reader.BaseStream.Seek(8, SeekOrigin.Begin);
                        byte[] by = reader.ReadBytes(frameSize);
                        int km = BitConverter.ToInt32(by.Skip(40).Take(4).ToArray(), 0);

                        var cmd = new NpgsqlCommand();
                        cmd.Connection = con;
                        cmd.CommandText = $@"UPDATE trip_files SET km_num={km} WHERE file_name='{file}'";
                        cmd.ExecuteNonQuery();
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Не получилось прочитать файл ", file, e.Message);
                    }
                }

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