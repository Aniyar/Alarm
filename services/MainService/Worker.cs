using ALARm.Core;
using Microsoft.AspNetCore.Components;
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
using ALARm.DataAccess;


namespace MainService
{
    public class ProcessedKm
    {
        public File file ;
        public int Number = 0;
        public string message;

        public ProcessedKm(File file, int num, string mess)
        {
            this.file = file;
            this.Number = num;
            this.message = mess;
        }
    }
    public class Worker : BackgroundService
    {
       
        
        private readonly ILogger<Worker> _logger;
        private ConnectionFactory _connectionFactory;
        private IConnection _connection;
        private IModel _channel;
        private string QueueName = "";
        public IMainTrackStructureRepository MainTrackStructureRepository;
        public IRdStructureRepository RdStructureRepository = new RdStructureRepository();
        public int tryCount = 0;
        //{'FileId':17105, 'Km':42, 'Path': '\SCL-12\common\video_objects\desktop\213_17105_km_42.csv'}
        public Worker(ILogger<Worker> logger, IOptions<RabbitMQConfiguration> options)
        {
            QueueName = options.Value.Queue;
            _logger = logger;
            
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
                _channel = _connection.CreateModel();;
                _channel.QueueDeclare(queue: QueueName,
                                    durable: false,
                                    exclusive: false,
                                    autoDelete: false,
                                    arguments: null);
                _channel.QueueBind(queue: QueueName,
                                   exchange: "Neural",
                                   routingKey: "");
                _channel.BasicQos(0, 1, false);
                _logger.LogInformation($"Queue [{QueueName}] is waiting for messages.");
                var consumer = new EventingBasicConsumer(_channel);
                var processed = new List<ProcessedKm>();
                consumer.Received += async (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body).Replace("'","\"").Replace("\\", "\\\\");
                    _logger.LogInformation(" [x] Received {0}", message);
                    var file = JObject.Parse(message).ToObject<File>();
                    file.Path = "\\" + file.Path;
                    var res = RdStructureRepository.RunRvoDataInsert(file.Km, file.FileId, file.Path);
                    
                    if (res != null)
                    {
                        if (res.Count() == 4)
                        {
                            ALARm.Core.Helper.SendMessageToRabbitMQExchange(_connection, "alarm", message);
                        }
                    }


                    //if (processed.Where(o => o.file.Km == file.Km).Any())
                    //{
                    //    processed.Where(o => o.file.Km == file.Km).First().Number++;
                    //}
                    //else
                    //{
                    //    processed.Add(new ProcessedKm(file, 1, message));
                    //}
                    //if (processed.Where(o => o.file.Km == file.Km).First().Number == 4)
                    //{
                    //    Thread.Sleep(20000);
                    //    ALARm.Core.Helper.SendMessageToRabbitMQExchange(_connection, "alarm", message);
                    //    _logger.LogInformation(res != null ? res.ToString() : "processed km " + file.Km.ToString());
                    //}


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
