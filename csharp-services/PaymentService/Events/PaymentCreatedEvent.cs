using PaymentService.Models;

namespace PaymentService.Events;

public class PaymentCreatedEvent
{
    public int Id { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; }
    public string ReservationId { get; set; }
    public DateTime CreatedAt { get; set; }

    public static PaymentCreatedEvent FromPayment(Payment payment)
    {
        return new PaymentCreatedEvent
        {
            Id = payment.Id,
            Amount = payment.Amount,
            Currency = payment.Currency,
            ReservationId = payment.ReservationId,
            CreatedAt = payment.CreatedAt
        };
    }
}