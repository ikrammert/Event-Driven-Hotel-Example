using NotificationService.Services;
using NotificationService.Events;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var rabbitMQConfig = builder.Configuration.GetSection("RabbitMQ");
builder.Services.AddSingleton<RabbitMQService>(sp => new RabbitMQService(
    rabbitMQConfig["HostName"],
    rabbitMQConfig["UserName"],
    rabbitMQConfig["Password"]
));

builder.Services.AddSingleton<INotificationService, NotificationService.Services.NotificationService>();
builder.Services.AddHostedService<PaymentEventConsumer>();
builder.Services.AddHostedService<ReservationEventConsumer>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();