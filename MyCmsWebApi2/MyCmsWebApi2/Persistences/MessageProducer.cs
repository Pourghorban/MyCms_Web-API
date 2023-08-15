using MyCmsWebApi2.Applications.MessageServices;
using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

namespace MyCmsWebApi2.Persistences
{
    public class MessageProducer : IMessageProducer
    {
        void IMessageProducer.SendingMessages<T>(T message)
        {
            var factory = new ConnectionFactory()
            {
                HostName = "localhost",
                UserName = "user",
                Password = "mypass",
                VirtualHost = "/"
            };
            var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();
            channel.QueueDeclare("news", durable: true, exclusive: true);

            var jsonString = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(jsonString);
            channel.BasicPublish("", "news", body: body);
        }
    }
}
