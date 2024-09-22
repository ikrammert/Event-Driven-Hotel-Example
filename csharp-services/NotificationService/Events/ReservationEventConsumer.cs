using System.Text;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Newtonsoft.Json;
using NotificationService.Services;

namespace NotificationService.Events;

public class ReservationEventConsumer : BackgroundService
{
    private readonly IConnection _connection;
    private readonly IModel _channel;
    private readonly INotificationService _notificationService;
    private readonly ILogger<ReservationEventConsumer> _logger;

    public ReservationEventConsumer(
        RabbitMQService rabbitMQService,
        INotificationService notificationService,
        ILogger<ReservationEventConsumer> logger)
    {
        _connection = rabbitMQService.CreateConnection();
        _channel = _connection.CreateModel();
        _notificationService = notificationService;
        _logger = logger;

        _channel.ExchangeDeclare(
            "reservation_events", 
            ExchangeType.Topic,
            durable: true, 
            autoDelete: false,
            arguments: null
        );
        _channel.QueueDeclare("notification_reservation_queue", false, false, false, null);
        _channel.QueueBind("notification_reservation_queue", "reservation_events", "reservation.#");
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var consumer = new EventingBasicConsumer(_channel);

        consumer.Received += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var routingKey = ea.RoutingKey;

            _logger.LogInformation($"Received message: {message}, RoutingKey: {routingKey}");

            
            if (routingKey == "reservation.created")
            {
                var reservation = JsonConvert.DeserializeObject<ReservationCreatedEvent>(message);
                await _notificationService.SendNotification($"New reservation created for {reservation.GuestName}");
            }

            _channel.BasicAck(ea.DeliveryTag, false);
        };

        _channel.BasicConsume("notification_reservation_queue", false, consumer);

        return Task.CompletedTask;
    }

    public override void Dispose()
    {
        _channel.Close();
        _connection.Close();
        base.Dispose();
    }
}

public class ReservationCreatedEvent
{
    public string Id { get; set; }
    public string HotelId { get; set; }
    public string GuestName { get; set; }
    public DateTime CheckIn { get; set; }
    public DateTime CheckOut { get; set; }
}