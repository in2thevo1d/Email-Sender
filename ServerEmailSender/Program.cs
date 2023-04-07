using System.Net.Mail;
using System.Net;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;
using ServerEmailSender;

MailMessage mail = new MailMessage();
var factory = new ConnectionFactory
{
	HostName = "localhost"
};
using (var connection = factory.CreateConnection())
using (var channel = connection.CreateModel())
{
	channel.QueueDeclare(queue: "email",
						 durable: false,
						 exclusive: false,
						 autoDelete: true,
						 arguments: null);

	var consumer = new EventingBasicConsumer(channel);
	channel.BasicConsume(queue: "email",
						 autoAck: false,
						 consumer: consumer);

	consumer.Received += (model, ea) =>
	{
		var body = ea.Body.ToArray();
		var json = Encoding.UTF8.GetString(body.ToArray());
		var email = JsonSerializer.Deserialize<Email>(json);

		var replyProps = channel.CreateBasicProperties();
		// TODO: Реализовать RCP-ответ
		/*
		replyProps.CorrelationId = ea.BasicProperties.CorrelationId;
		replyProps.ReplyTo = ea.BasicProperties.ReplyTo;
		string response = string.Empty;
		*/
		try
		{
			mail.From = new MailAddress(""); // Адрес электронной почты, с которой отправляем сообщения
			mail.To.Add(new MailAddress(email.EmailAddress)); // Адрес получателя
			mail.Subject = email.EmailSubject;
			mail.Body = email.EmailBody;

			SmtpClient client = new SmtpClient();
			client.Host = "smtp.mail.ru";
			client.Port = 587; // Порт
			client.EnableSsl = true;
			client.Credentials = new NetworkCredential("", ""); // Адресс электронной почты и пароль для внешнего приложения
			client.Send(mail);
			//response = "Сообщение отправлено!";
		}

		catch (Exception e)
		{
			//response = "Произошла ошибка. Неверно введены данные электронной почты";
		}

		finally
		{
			//var responseBytes = Encoding.UTF8.GetBytes(response);
			/*channel.BasicPublish(exchange: "",
								 routingKey: replyProps.ReplyTo,
								 basicProperties: replyProps,
								 body: responseBytes);

			channel.BasicAck(deliveryTag: ea.DeliveryTag, multiple: false);*/
		}
	};
	Console.WriteLine(" Press [enter] to exit.");
	Console.ReadLine();
}