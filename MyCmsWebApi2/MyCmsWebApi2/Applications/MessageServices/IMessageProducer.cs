namespace MyCmsWebApi2.Applications.MessageServices
{
    public interface IMessageProducer
    {
        public void SendingMessages<T>(T message);
    }
}
