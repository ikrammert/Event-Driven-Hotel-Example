using System.Text;
using System.Text.Json;
using PaymentService.Models;
using PaymentService.Services;
using RabbitMQ.Client;

namespace PaymentService.Events;

public interface IPaymentEventProducer
{
    Task PublishPaymentCreatedEvent(Payment payment);
}

public class PaymentEventProducer : IPaymentEventProducer
{
    private readonly RabbitMQService _rabbitMQService;
    private const string ExchangeName = "payment_events";
    private const string RoutingKey = "payment.created";

    public PaymentEventProducer(RabbitMQService rabbitMQService)
    {
        _rabbitMQService = rabbitMQService;
    }

    public Task PublishPaymentCreatedEvent(Payment payment)
    {
        var @event = PaymentCreatedEvent.FromPayment(payment);
        var message = JsonSerializer.Serialize(@event);
        var body = Encoding.UTF8.GetBytes(message);

        using var connection = _rabbitMQService.CreateConnection();
        using var channel = connection.CreateModel();

        channel.ExchangeDeclare(ExchangeName, ExchangeType.Topic, durable: true);

        channel.BasicPublish(
            exchange: ExchangeName,
            routingKey: RoutingKey,
            basicProperties: null,
            body: body);

        return Task.CompletedTask;
    }
}