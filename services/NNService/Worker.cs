using MainService;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace NNService
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private ConnectionFactory _connectionFactory;
        private IConnection _connection;
        private IModel _channel;
        private string QueueName = "";
        public int tryCount = 0;

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


                _channel.QueueDeclarePassive(QueueName);


                //_channel.QueueDeclare(queue: QueueName,
                //                   durable: false,
                //                   exclusive: false,
                //                   autoDelete: false,
                //                   arguments: null);

                _channel.BasicQos(0, 1, false);
                _logger.LogInformation($"Queue [{QueueName}] is waiting for messages.");
                var consumer = new EventingBasicConsumer(_channel);
                consumer.Received += async (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body).Replace("'", "\"").Replace("\\", "\\\\");
                    JObject json = JObject.Parse(message);
                    _logger.LogInformation(" [x] Received {0}", message);

                    GetNN((string)json["TripId"], (int)json["Msg"]);
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

        private void GetNN(string TripId, int parameters2)
        {
            Process pr = new Process();
            ProcessStartInfo prs = new ProcessStartInfo();
            prs.FileName = @"C:\ObjectRecRW\Source\rwdet_project1\x64\Release\rwdet_gpu.exe";
            prs.Arguments = $"{TripId} {parameters2}";
            pr.StartInfo = prs;

            ThreadStart ths = new ThreadStart(() => pr.Start());
            Thread th = new Thread(ths);
            th.Start();

            var pr2 = new Process();
            var prs2 = new ProcessStartInfo();
            prs2.FileName = @"C:\ObjectRecRW\Source\rwdet_project1\x64\Release\rwdet_gpu.exe";
            prs2.Arguments = $"{TripId} {parameters2 + 1}";
            pr2.StartInfo = prs2;

            ThreadStart ths2 = new ThreadStart(() => pr2.Start());
            Thread th2 = new Thread(ths2);
            th2.Start();

            //var pr3 = new Process();
            //var prs3 = new ProcessStartInfo();
            //prs3.FileName = @"C:\ObjectRecRW\Source\rwdet_project1\x64\Release\rwdet_gpu.exe";
            //prs3.Arguments = $"{TripId} {parameters2 + 1}";
            //pr3.StartInfo = prs3;

            //ThreadStart ths3 = new ThreadStart(() => pr3.Start());
            //Thread th3 = new Thread(ths3);
            //th3.Start();
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
