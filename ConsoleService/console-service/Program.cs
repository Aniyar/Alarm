using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;

namespace console_service
{
    class Program
    {
        static void Main(string[] args)
        {
            var msg = @"{
                        ""TripId"" : 213,
                        ""DistId"" : 53,
                        ""KmId"" : 25844
                        }";
            //SendToRabbitMQ(msg);
            GetReceiveRabbitMQ();

        }

        private static void SendToRabbitMQ(string msg)
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                channel.QueueDeclare(queue: "hello",
                                     durable: false,
                                     exclusive: false,
                                     autoDelete: false,
                                     arguments: null);

                var body = Encoding.UTF8.GetBytes(msg);

                channel.BasicPublish(exchange: "",
                                     routingKey: "hello",
                                     basicProperties: null,
                                     body: body);
                Console.WriteLine(" [x] Sent {0}", msg);
            }

            Console.WriteLine(" Press [enter] to exit.");
            Console.ReadLine();
        }

        private static void GetReceiveRabbitMQ()
        {
            var factory = new ConnectionFactory() { HostName = "localhost" };
            using (var connection = factory.CreateConnection())
            using (var channel = connection.CreateModel())
            {
                //channel.QueueDeclare(queue: "QueueName",
                //                     durable: false,
                //                     exclusive: false,
                //                     autoDelete: false,
                //                     arguments: null);

                var consumer = new EventingBasicConsumer(channel);
                consumer.Received += (model, ea) =>
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    Console.WriteLine(" [x] Received {0}", message);
                };
                channel.BasicConsume(queue: "QueueName",
                                     autoAck: true,
                                     consumer: consumer);

                Console.WriteLine(" Press [enter] to exit.");
                Console.ReadLine();
            }
        }
    }
}