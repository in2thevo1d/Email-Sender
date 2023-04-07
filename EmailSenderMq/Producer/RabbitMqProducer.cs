using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace EmailSenderMq.Producer
{
    public class RabbitMqProducer : IRabbitMqProducer
    {
        public void SendEmailMessage<T>(T message)
        {
            var factory = new ConnectionFactory
            {
                HostName = "localhost"
            };

            var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();
            channel.QueueDeclare("email",
                                 exclusive: false);

            var json = JsonConvert.SerializeObject(message);
            var body = Encoding.UTF8.GetBytes(json);
            channel.BasicPublish(exchange: "",
                                 routingKey: "email",
                                 body: body);

            //TODO: Реализовать RCP-запрос.
        }
    }
}
