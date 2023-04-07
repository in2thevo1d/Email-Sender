namespace EmailSenderMq.Producer
{
    public interface IRabbitMqProducer
    {
        public void SendEmailMessage<T>(T message);
    }
}
