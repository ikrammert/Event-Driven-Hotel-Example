using PaymentService.Models;
using PaymentService.Events;

namespace PaymentService.Services;

public interface IPaymentService
{
    Task<Payment> ProcessPayment(Payment payment);
}

public class PaymentService : IPaymentService
{
    private readonly IPaymentEventProducer _eventProducer;

    public PaymentService(IPaymentEventProducer eventProducer)
    {
        _eventProducer = eventProducer;
    }

    public async Task<Payment> ProcessPayment(Payment payment)
    {
        payment.Id = payment.Id; //Guid için hazır
        payment.CreatedAt = DateTime.UtcNow;

        // created event
        await _eventProducer.PublishPaymentCreatedEvent(payment);

        return payment;
    }
}