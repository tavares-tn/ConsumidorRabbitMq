using consumer.Domain;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace consumer
{
    public class Program
    {
        static void Main(string[] args)
        {
            // cria conexão com RabbitMQ
            var factory = new ConnectionFactory()
            {
                HostName = "localhost",
                UserName = ConnectionFactory.DefaultUser,
                Password = ConnectionFactory.DefaultPass,
                Port = AmqpTcpEndpoint.UseDefaultPort
            };

            // cria conexão
            var connection = factory.CreateConnection();

            // cria a canal de comunicação com RabbitMQ
            var channel = connection.CreateModel();


            //Cria a fila caso não exista
            channel.QueueDeclare(queue: "orderQueue",
                                 durable: false,
                                 exclusive: false,
                                 autoDelete: false,
                                 arguments: null);

            var consumer = new EventingBasicConsumer(channel);

            consumer.Received += (model, ea) =>
            {
                try
                {
                    var message = Encoding.UTF8.GetString(ea.Body.ToArray());
                    var order = JsonSerializer.Deserialize<Order>(message);

                    Console.WriteLine($"OrderNumber: {order.OrderNumber}");
                    Console.WriteLine($"OrderName: {order.ItemName}");
                    Console.WriteLine($"Price: {order.Price}");
                }
                catch (Exception ex)
                {
                    // Logger
                    channel.BasicNack(ea.DeliveryTag, false, true);
                }
            };
            

            channel.BasicConsume(queue: "orderQueue",
                                 autoAck: false,
                                 consumer: consumer);

        }
    }
}
