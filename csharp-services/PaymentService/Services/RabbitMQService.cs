using RabbitMQ.Client;

namespace PaymentService.Services;

public class RabbitMQService
{
    private readonly ConnectionFactory _factory;

    public RabbitMQService(string hostName, string userName, string password)
    {
        _factory = new ConnectionFactory
        {
            HostName = hostName,
            UserName = userName,
            Password = password
        };
    }

    public IConnection CreateConnection()
    {
        return _factory.CreateConnection();
    }
}